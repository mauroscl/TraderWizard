using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DataBase;
using Ionic.Zip;
using prjConfiguracao;
using prjDominio.Regras;
using prjServicoNegocio;
using pWeb;
using Services;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{
    public class AtualizadorDeCotacao
    {

        private readonly Conexao _conexao;
        /// <summary>
        /// contém o objeto que faz downloads na internet.
        /// </summary>
        /// <remarks></remarks>

        private readonly cWeb _web;

        private readonly SequencialService _sequencialService;
        private readonly CotacaoData _cotacaoData;
        private readonly ServicoDeCotacao _servicoDeCotacao;


        public AtualizadorDeCotacao()
        {
            this._conexao = new Conexao();
            this._web = new cWeb(this._conexao);
            this._sequencialService = new SequencialService();
            this._cotacaoData = new CotacaoData();
            this._servicoDeCotacao = new ServicoDeCotacao();
        }

        /// <summary>
        /// Atualiza as cotações em todas as datas em que há pregão em um determinado período.
        /// </summary>
        /// <param name="pdtmDataInicial">Data inicial de importação</param>
        /// <param name="pdtmDataFinal">Data final de importação</param>
        /// <param name="pstrCodigoUnico">Indica se é para importar o código de um único ativo. Se o parâmetro for uma string vazia deve importar todos os arquivos</param>
        /// <param name="pblnCalcularDados">Indica se após atualizar as cotações deve calcular indicadores como média, IFR, etc</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CotacaoPeriodoAtualizar(DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrCodigoUnico, bool pblnCalcularDados)
        {

            //inicializa a data com data inválida. Vai ser atribuido nesta variável o primeiro dia útil.
            var dtmDataInicialAux = Constantes.DataInvalida;

            bool blnOk = true;

            DateTime dtmDataUltimaCotacao = Constantes.DataInvalida;

            var objCalculadorData = new cCalculadorData(_conexao);

            //enquanto a data inicial for menor que a data final.

            while ((pdtmDataInicial <= pdtmDataFinal) && blnOk)
            {

                if (objCalculadorData.DiaUtilVerificar(pdtmDataInicial))
                {
                    if (dtmDataInicialAux == Constantes.DataInvalida)
                    {
                        //se a data que será utilizada nos cálculos de IFR, MÉDIA, ETC 
                        //ainda não foi atribuida, atribui com o primeiro dia útil.
                        dtmDataInicialAux = pdtmDataInicial;
                    }

                    cEnum.enumRetorno intRetorno = CotacaoDataAtualizar(pdtmDataInicial, pstrCodigoUnico);


                    if (intRetorno == cEnum.enumRetorno.RetornoOK)
                    {
                        dtmDataUltimaCotacao = pdtmDataInicial;


                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErro2)
                    {
                        //não conseguiu baixar o arquivo na data.

                        //se é uma data única avisa para o usuário.
                        //se é mais de uma data não faz nada e busca a próxima data

                        //If blnDataUnica Then

                        blnOk = false;
                        MessageBox.Show("Não foi possível baixar o arquivo de cotações na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        //End If


                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErro3)
                    {
                        blnOk = false;
                        //já existe cotação na data
                        MessageBox.Show("Já existe cotação na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);



                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErroInesperado)
                    {
                        //erro na transação. Sai fora porque ocorreu um erro
                        blnOk = false;
                        MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }

                }

                //incrementa um dia na data inicial
                pdtmDataInicial = pdtmDataInicial.AddDays(1);

            }

            //********MAURO, 11/05/2010
            //Atualiza a tabela de resumo.

            if (dtmDataUltimaCotacao != Constantes.DataInvalida)
            {
                //se alguma cotação foi atualizada com sucesso atualiza a tabela de resumo.
                TabelaResumoAtualizar(dtmDataUltimaCotacao, Constantes.DataInvalida);


                if (blnOk)
                {

                    if (pblnCalcularDados)
                    {
                        _servicoDeCotacao.DadosRecalcular(true, false, true, true, true, true, true, true, true, true,
                            true, dtmDataInicialAux);

                    }

                    MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                }


            }
            else
            {
                MessageBox.Show("Não existem cotações para serem atualizadas.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

            return blnOk;

        }


        /// <summary>
        /// Lita todos os ativos que não devem ser considerados separados pelo simbolo "#"
        /// </summary>
        /// <returns>String contendo a lista de ativos não considerados</returns>
        /// <remarks></remarks>
        private string AtivosDesconsideradosListar()
        {

            cRS objRS = new cRS(_conexao);

            string strLista = String.Empty;

            objRS.ExecuteQuery(" select Codigo " + " from Ativos_Desconsiderados");


            while (!objRS.EOF)
            {
                strLista = strLista + "#" + objRS.Field("Codigo");

                objRS.MoveNext();

            }

            objRS.Fechar();

            //tem que colocar o # para não ocorrer erros do tipo ter um código desconsiderado VALE50
            //e o ativo VALE5 NÃO SER IMPORTADO
            strLista = strLista + "#";

            return strLista;

        }

        /// <summary>
        /// "Faz download do arquivo de cotações da Bovespa de uma determinada data e salva na tabela em Access"
        /// </summary>
        /// <param name="pdtmData"></param>
        /// <param name="pstrCodigoUnico">Indica se é para importar o código de um único ativo. Se o parâmetro for uma string vazia deve importar todos os arquivos</param>
        /// <returns></returns>
        ///RetornoOK = operação realizada com sucesso.
        ///RetornoErro2 = não foi possível baixar o arquivo na data especificada
        ///RetornoErroInesperado = Erro ao executar a operação de atualização de cotas.             
        ///RetornoErro3 = Já existe cotações na data específicada    
        /// <remarks></remarks>
        private cEnum.enumRetorno CotacaoDataAtualizar(DateTime pdtmData, string pstrCodigoUnico)
        {
            cCommand objCommand = new cCommand(_conexao);

            IList<string> colLinha;

            string strCodigoAtivo = String.Empty;

            decimal decValorFechamento = default(decimal);
            decimal decValorMinimo = default(decimal);
            decimal decValorMedio = default(decimal);
            decimal decValorMaximo = default(decimal);

            decimal decOscilacao = default(decimal);

            long lngNegociosTotal = 0;
            long lngTitulosTotal = 0;
            decimal decValorTotal = default(decimal);

            const char strSeparadorDecimal = ',';

            string[] tiposDeCotacao = { "0202010", "0205010" };


            cRS objRS = new cRS(_conexao);

            //utilizado para calcular o sequencial do ativo.

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + funcoesBd.CampoDateFormatar(pdtmData));


            if ((int)objRS.Field("Contador") > 0)
            {
                DateTime[] arrData = { pdtmData };


                if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK)
                {
                    objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);


                    return cEnum.enumRetorno.RetornoErroInesperado;

                }

            }

            objRS.Fechar();

            objCommand.BeginTrans();

            bool blnCotacaoExistir = pstrCodigoUnico == string.Empty 
                ? this._cotacaoData.CotacaoDataExistir(pdtmData, "Cotacao") 
                : this._cotacaoData.CotacaoDataExistir(pdtmData, "Cotacao", pstrCodigoUnico);

            if (blnCotacaoExistir)
            {
                //se já existe alguma cotação na data,
                //faz rollback e sai da função
                objCommand.RollBackTrans();

                return cEnum.enumRetorno.RetornoErro3;

            }

            //baixar as cotações do site da bovespa

            if (ArquivoDataBaixar(pdtmData, out colLinha))
            {
                //busca os ativos que não devem ser consideros
                string strAtivosDesconsiderados = AtivosDesconsideradosListar();

                //se foi possivel baixar...
                //percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados

                int intI;
                for (intI = 0; intI < colLinha.Count; intI++)
                {
                    //coloca a linha na variável auxiliar
                    string strLinhaAux = colLinha[intI];

                    //os dois primeiros caracteres indicam o tipo de registro.
                    //o tipo de registro 02 indica que é a cotação de um papel
                    //o terceiro e quarto caracteres indicam o código BDI do papel.
                    //O código 02 indica que é um papel do lote padrão
                    //posição 70 - 73 indica o tipo  de mercado do ativo
                    //o tipo de mercado 010 é o mercado A VISTA
                    bool blnImportarLinha;
                    bool blnInserir;
                    string strOscilacao;
                    decimal decValorAbertura = default(decimal);
                    if (tiposDeCotacao.Contains(strLinhaAux.Substring(0, 4) + strLinhaAux.Substring(69, 3)) )
                    {
                        //se é a cotação de um papel
                        //e é do mercado à vista.

                        //busca código do ativo, posicao 58-69
                        strCodigoAtivo = strLinhaAux.Substring(57, 12).Trim();


                        if (pstrCodigoUnico == string.Empty)
                        {
                            //Se é para importar todos os códigos, verifica se o mesmo não se encontra na lista de ativos desconsiderados
                            blnImportarLinha = (strAtivosDesconsiderados.IndexOf("#" + strCodigoAtivo + "#", StringComparison.InvariantCultureIgnoreCase) == -1);

                        }
                        else
                        {
                            //Se é para importar um único ativo, verificar se a linha refere-se ao código que deve ser importado
                            blnImportarLinha = (strCodigoAtivo == pstrCodigoUnico);
                        }

                        //verifica se deve importar a linha
                        if (blnImportarLinha)
                        {
                            //se o ativo não foi encontrado na lista dos desconsiderados.

                            //TOTAL DE NEGÓCIOS (174-178)
                            lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(173, 5));

                            //algumas ações não tem negócios no dia. Estas cotações não serão importadas.

                            if (lngNegociosTotal > 0)
                            {
                                //busca valor de abertura do ativo: 91 - 99 (inteiro), 100-101 (decimal)
                                decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring(90, 9) + strSeparadorDecimal + strLinhaAux.Substring(99, 2));

                                //busca o valor máximo do ativo: 102-110 (inteiro), 111-112 (decimal)
                                decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(101, 9) + strSeparadorDecimal + strLinhaAux.Substring(110, 2));

                                //busca o valor mínimo do ativo: 113-121 (inteiro), 122-123 (decimal)
                                decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(112, 9) + strSeparadorDecimal + strLinhaAux.Substring(121, 2));

                                //busca o valor médio do ativo: 124-132 (inteiro), 133-134 (decimal)
                                decValorMedio = Convert.ToDecimal(strLinhaAux.Substring(123, 9) + strSeparadorDecimal + strLinhaAux.Substring(132, 2));

                                //busca o valor de fechamento do ativo: 135-143 (inteiro), 144-145 (decimal)
                                decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(134, 9) + strSeparadorDecimal + strLinhaAux.Substring(143, 2));

                                //busca a oscilação do papel em relação ao dia anterior
                                //146 = sinal da oscilação (+ ou -)
                                //147-149 = parte inteira da oscilação
                                //150-151 = parte decimal da oscilação

                                strOscilacao = strLinhaAux.Substring(146, 3) + strSeparadorDecimal + strLinhaAux.Substring(149, 2);


                                if (strLinhaAux.Substring(145, 1) == "-")
                                {
                                    strOscilacao = "-" + strOscilacao;

                                }

                                decOscilacao = Convert.ToDecimal(strOscilacao);

                                //TOTAL DE TÍTULOS NEGOCIADOS (179-193)
                                lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(178, 15));

                                //VALOR TOTAL NEGOCIADO: 194-208 (inteiro), 209-210 (decimal)
                                decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(193, 15) + strSeparadorDecimal + strLinhaAux.Substring(208, 2));

                                blnInserir = true;

                            }
                            else
                            {
                                //se não teve negócios no dia
                                blnInserir = false;

                            }
                            //if lngNegocios_Total > 0 then


                        }
                        else
                        {
                            //se o ativo deve ser desconsiderado
                            blnInserir = false;

                        }


                    }
                    else if (strLinhaAux.Substring(0, 12) == "0101IBOVESPA")
                    {
                        if (pstrCodigoUnico == string.Empty)
                        {
                            blnImportarLinha = true;
                        }
                        else
                        {
                            blnImportarLinha = (pstrCodigoUnico == "IBOV");
                        }


                        if (blnImportarLinha)
                        {
                            //é o indice BOVESPA
                            strCodigoAtivo = "IBOV";

                            //busca valor de abertura do ativo: 35 - 40
                            decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring(34, 6));

                            //busca o valor máximo do ativo: 41-46
                            decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(40, 6));

                            //busca o valor mínimo do ativo: 47-52
                            decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(46, 6));

                            //busca o valor médio do ativo: 53-58
                            decValorMedio = Convert.ToDecimal(strLinhaAux.Substring(52, 6));

                            //busca o valor de fechamento do ativo: 93-98
                            decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(92, 6));

                            //busca a oscilação do papel em relação ao dia anterior
                            //99 = sinal da oscilação (+ ou -)
                            //100-102 = parte inteira da oscilação
                            //103-104 = parte decimal da oscilação

                            strOscilacao = strLinhaAux.Substring(99, 3) + strSeparadorDecimal + strLinhaAux.Substring(102, 2);


                            if (strLinhaAux.Substring(98, 1) == "-")
                            {
                                strOscilacao = "-" + strOscilacao;

                            }

                            decOscilacao = Convert.ToDecimal(strOscilacao);

                            //TOTAL DE NEGÓCIOS (159-164)
                            lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(158, 6));

                            //TOTAL DE TÍTULOS NEGOCIADOS (165-179)
                            lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(164, 15));

                            //VALOR TOTAL NEGOCIADO: 180-194 (inteiro), 195-196 (decimal)
                            decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(179, 15) + strSeparadorDecimal + strLinhaAux.Substring(194, 2));

                            blnInserir = true;


                        }
                        else
                        {
                            blnInserir = false;

                        }


                    }
                    else
                    {
                        //não é um ativo do mercado à vista, nem o indice BOVESPA
                        blnInserir = false;

                    }
                    //se é uma linha do mercado à vista.


                    if (blnInserir)
                    {
                        //calcula o sequencial do ativo
                        long lngSequencial = _sequencialService.SequencialCalcular(strCodigoAtivo, "Cotacao", objCommand.Conexao);

                        //insere na tabela
                        var dataFormatada = funcoesBd.CampoDateFormatar(pdtmData);

                        var insertBuilder = new StringBuilder()
                            .Append(" insert into Cotacao ")
                            .Append("(Codigo, Data, DataFinal, ValorAbertura, ValorFechamento, ValorMinimo, ValorMedio, ValorMaximo, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial) ")
                            .Append(" VALUES ")
                            .AppendFormat("({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})",
                                FuncoesBd.CampoStringFormatar(strCodigoAtivo), dataFormatada, dataFormatada, FuncoesBd.CampoDecimalFormatar(decValorAbertura),
                                FuncoesBd.CampoDecimalFormatar(decValorFechamento), FuncoesBd.CampoDecimalFormatar(decValorMinimo), FuncoesBd.CampoDecimalFormatar(decValorMedio),
                                FuncoesBd.CampoDecimalFormatar(decValorMaximo), FuncoesBd.CampoDecimalFormatar(decOscilacao),
                                lngNegociosTotal, lngTitulosTotal, FuncoesBd.CampoDecimalFormatar(decValorTotal), lngSequencial);

                        objCommand.Execute(insertBuilder.ToString());

                    }

                }


            }
            else
            {
                //se não conseguiu baixar o arquivo
                objCommand.RollBackTrans();

                return cEnum.enumRetorno.RetornoErro2;

            }
            //if ArquivoDataBaixar

            objCommand.CommitTrans();

            cEnum.enumRetorno functionReturnValue = objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

            return functionReturnValue;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pstrCaminho"> Pasta onde se encontra o arquivo </param>
        /// <param name="pstrArquivoZip"> Nome do arquivo que deve ser descompactado </param>
        /// <param name="pstrArquivoTexto"> Nome do arquivo contido dentro do arquivo zip  </param>
        /// <param name="pcolLinhaRet"> retorna collection com todas as linhas do arquivo texto descompactado</param>
        /// <returns> 
        /// True = arquivo zip descomptactado e arquivo texto lido com sucesso 
        /// False = algum erro na descompactação do arquivo zip ou leitura do arquivo texto
        /// </returns>
        /// <remarks></remarks>
        private bool ArquivoDescompactar(string pstrCaminho, string pstrArquivoZip, string pstrArquivoTexto, out IList<string> pcolLinhaRet)
        {
            var zipFile = new ZipFile(pstrCaminho + "\\" + pstrArquivoZip);

            zipFile.ExtractAll(pstrCaminho);

            IFileService fileService = new FileService();

            //o conteúdo do arquivo zipado é um arquivo chamado BDIN, sem extensão
            //abre o arquivo para leitura
            pcolLinhaRet = fileService.ReadAllLines(pstrCaminho + "\\" + pstrArquivoTexto);

            //apaga o arquivo BDIN que foi extraído do zip.
            fileService.Delete(pstrCaminho + "\\" + pstrArquivoTexto);

            return true;

        }


        private bool ArquivoDataBaixar(DateTime pdtmData, out IList<string> pcolLinhaRet)
        {
            string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

            strPathZip = strPathZip + "Arquivos";

            //Verifica se existe o diretório para armazenar os arquivos baixados

            var fileService = new FileService();
            fileService.CreateFolder(strPathZip);

            //gera o nome do arquivo que deve ser baixado
            string strArquivoBaixar = cGeradorNomeArquivo.GerarNomeArquivoRemoto(pdtmData);

            //Nome que será dado ao arquivo baixado :"bdi" + yyyymmdd + ".zip"
            string strArquivoZipDestino = cGeradorNomeArquivo.GerarNomeArquivoLocal(pdtmData);

            if (!fileService.FileExists(strPathZip + "\\" + strArquivoZipDestino))
            {
                string urlBase = ConfigurationManager.AppSettings["UrlDownloadoArquivoFechamentoPregao"];
                if (!_web.DownloadWithProxy(urlBase + strArquivoBaixar, strPathZip, strArquivoZipDestino))
                {
                    pcolLinhaRet = new List<string>();
                    return false;
                }
            }

            //CHAMA FUNÇÃO QUE DESCOMPACTA O ARQUIVO, ABRE O ARQUIVO TEXTO, FAZ A LEITURA E RETORNA TODAS AS LINHAS
            //EM UMA COLLECTION.
            bool blnDescompactar = ArquivoDescompactar(strPathZip, strArquivoZipDestino, "BDIN", out pcolLinhaRet);

            return blnDescompactar;

        }

        /// <summary>
        /// Recebe uma data e procura um arquivo de cotação histórica na pasta Arquivos.
        /// Importa as cotações deste arquivo e salva na tabela.
        /// </summary>
        /// <param name="pdtmData"> Indica a data que deve ser feita a atualização </param>
        /// <returns>
        ///RetornoOK = operação realizada com sucesso.
        ///RetornoErro2 = não foi possível encontrar o arquivo na data especificada
        ///RetornoErroInesperado = Erro ao executar a operação de atualização de cotas.             
        ///RetornoErro3 = Já existe cotações na data específicada       
        /// </returns>
        /// <remarks></remarks>
        private cEnum.enumRetorno CotacaoHistoricaDataAtualizar(DateTime pdtmData)
        {

            cCommand objCommand = new cCommand(_conexao);

            IList<string> colLinha;

            //nome dado ao arquivo zip que for baixado

            string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

            strPathZip = strPathZip + "Arquivos";

            //Nome do arquivo zip baixado :"COTAHIST_D" + ddmmyyyy + ".zip"
            string strArquivoZipDestino = "COTAHIST_D" + pdtmData.ToString("ddMMyyyy") + ".ZIP";

            string strArquivoTextoDestino = "COTAHIST_D" + pdtmData.ToString("ddMMyyyy") + ".TXT";

            cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + funcoesBd.CampoDateFormatar(pdtmData));


            if (Convert.ToInt32(objRS.Field("Contador")) > 0)
            {
                DateTime[] arrData = { pdtmData };


                if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK)
                {
                    objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return cEnum.enumRetorno.RetornoErroInesperado;

                }

            }

            objRS.Fechar();

            //**************** INICIO DA TRANSAÇÃO
            objCommand.BeginTrans();


            if (this._cotacaoData.CotacaoDataExistir(pdtmData, "Cotacao"))
            {
                //se já existe cotação na data faz rollback para não sobrescrever
                objCommand.RollBackTrans();

                //retorno erro conforme descrito no cabeçalho da função
                return cEnum.enumRetorno.RetornoErro3;

            }

            //descompactar o arquivo que contém as cotações

            if (ArquivoDescompactar(strPathZip, strArquivoZipDestino, strArquivoTextoDestino, out colLinha))
            {
                CotacoesImportar(colLinha, objCommand);

            }

            //**************** FIM DA TRANSAÇÃO
            objCommand.CommitTrans();

            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;
        }

        /// <summary>
        /// Atualiza as cotações de um ano todo
        /// </summary>
        /// <param name="pintAno">Ano da cotação</param>
        /// <returns></returns>
        /// RetornoOK - Operação realizada com sucesso.
        /// RetornoErroInesperado - algum erro inesperado de banco de dados ou de programação
        /// RetornoErro2 - Já existe cotação no ano.
        /// RetornoErro3 - Não foi possível descompactar o arquivo zip ou abrir o arquivo txt.
        /// <remarks></remarks>
        public cEnum.enumRetorno CotacaoHistoricaAnoAtualizar(int pintAno)
        {

            cCommand objCommand = new cCommand(_conexao);

            //objConexao = objCommand.Conexao

            cRS objRS = new cRS(_conexao);

            IList<string> colLinha;

            //nome dado ao arquivo zip que for baixado

            string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

            strPathZip = strPathZip + "Arquivos";

            //Nome do arquivo zip baixado :"COTAHIST_A" + YYYY + ".zip"
            string strArquivoZipDestino = "COTAHIST_A" + Convert.ToString(pintAno) + ".ZIP";

            string strArquivoTextoDestino = "COTAHIST_A" + Convert.ToString(pintAno) + ".TXT";

            //**************** INICIO DA TRANSAÇÃO
            objCommand.BeginTrans();

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            //verifica se tem alguma cotação no ano. Se já existir avisa para o usuário.
            objRS.ExecuteQuery(" select count(1) as contador " + " from Cotacao " + " where Data >= " +
                               FuncoesBd.CampoDateFormatar(new DateTime(pintAno, 1, 1)) + " and Data <= " +
                               FuncoesBd.CampoDateFormatar(new DateTime(pintAno, 12, 31)));


            if (Convert.ToInt64(objRS.Field("Contador", 0)) > 0)
            {
                //se já existe cotação na data faz rollback para não sobrescrever
                objCommand.RollBackTrans();

                objRS.Fechar();

                //retorno erro conforme descrito no cabeçalho da função
                return cEnum.enumRetorno.RetornoErro2;

            }

            objRS.Fechar();

            //descompactar o arquivo que contém as cotações

            if (ArquivoDescompactar(strPathZip, strArquivoZipDestino, strArquivoTextoDestino, out colLinha))
            {
                CotacoesImportar(colLinha, objCommand);


            }
            else
            {
                objCommand.RollBackTrans();
                return cEnum.enumRetorno.RetornoErro3;

            }

            //**************** FIM DA TRANSAÇÃO
            objCommand.CommitTrans();

            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

        }

        /// <summary>
        /// Atualiza as cotações em todas as datas em que há pregão em um determinado período.
        /// Os arquivos já devem estar baixados
        /// </summary>
        /// <param name="pdtmDataInicial">Data inicial do cálculo</param>
        /// <param name="pdtmDataFinal">Data final do cálculo</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CotacaoHistoricaPeriodoAtualizar(DateTime pdtmDataInicial, DateTime pdtmDataFinal, bool pblnCalcularDados)
        {

            //inicializa a data com data inválida. Vai ser atribuido nesta variável o primeiro dia útil.
            DateTime dtmDataInicialAux = Constantes.DataInvalida;

            //indica se o período informado é uma única data
            bool blnDataUnica = (pdtmDataInicial == pdtmDataFinal);

            bool blnOK = true;

            DateTime dtmData_Ultima_Cotacao = Constantes.DataInvalida;

            cCalculadorData objCalculadorData = new cCalculadorData(_conexao);

            //enquanto a data inicial for menor que a data final.

            while ((pdtmDataInicial <= pdtmDataFinal) && blnOK)
            {

                if (objCalculadorData.DiaUtilVerificar(pdtmDataInicial))
                {
                    if (dtmDataInicialAux == Constantes.DataInvalida)
                    {
                        //se a data que será utilizada nos cálculos de IFR, MÉDIA, ETC 
                        //ainda não foi atribuida, atribui com o primeiro dia útil.
                        dtmDataInicialAux = pdtmDataInicial;
                    }

                    cEnum.enumRetorno intRetorno = CotacaoHistoricaDataAtualizar(pdtmDataInicial);


                    if (intRetorno == cEnum.enumRetorno.RetornoOK)
                    {
                        dtmData_Ultima_Cotacao = pdtmDataInicial;


                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErro2)
                    {
                        //não conseguiu baixar o arquivo na data.

                        //se é uma data única avisa para o usuário.
                        //se é mais de uma data não faz nada e busca a próxima data
                        if (blnDataUnica)
                        {
                            blnOK = false;
                            MessageBox.Show("Não foi possível baixar o arquivo de cotações na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }


                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErro3)
                    {
                        blnOK = false;
                        //já existe cotação na data
                        MessageBox.Show("Já existe cotação na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);



                    }
                    else if (intRetorno == cEnum.enumRetorno.RetornoErroInesperado)
                    {
                        //erro na transação. Sai fora porque ocorreu um erro
                        blnOK = false;
                        MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }

                }

                //incrementa um dia na data inicial
                pdtmDataInicial = pdtmDataInicial.AddDays(1);

            }

            //********MAURO, 11/05/2010
            //Atualiza a tabela de resumo.

            if (dtmData_Ultima_Cotacao != Constantes.DataInvalida)
            {
                //se alguma cotação foi atualizada com sucesso atualiza a tabela de resumo.
                TabelaResumoAtualizar(dtmData_Ultima_Cotacao, Constantes.DataInvalida);

            }

            if (blnOK)
            {
                if (pblnCalcularDados)
                {
                    this._servicoDeCotacao.DadosRecalcular(true, true, true, true, true, true, true, true, true, true,
                        true, dtmDataInicialAux);
                }

                MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

            return blnOK;

        }

        /// <summary>
        /// Recebe uma collection com as linhas de um arquivo
        /// </summary>
        /// <param name="pcolLinha">Collection contendo as linhas do arquivo de cotações</param>
        /// <param name="pobjCommand"></param>
        /// <returns>status da transação</returns>
        /// <remarks></remarks>
        private void CotacoesImportar(IList<string> pcolLinha, cCommand pobjCommand)
        {
            int intI;

            DateTime dtmCotacaoData = default(DateTime);

            //utilizado para calcular o sequencial do ativo.

            const char strSeparadorDecimal = ',';

            //busca os ativos que não devem ser consideros
            string strAtivosDesconsiderados = AtivosDesconsideradosListar();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //se foi possivel baixar...
            //percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados

            for (intI = 1; intI <= pcolLinha.Count; intI++)
            {
                //coloca a linha na variável auxiliar
                string strLinhaAux = pcolLinha[intI];

                //os dois primeiros caracteres indicam o tipo de registro.
                //o tipo de registro 01 indica que é a cotação de um papel do mercado a vista

                //posição 11-12 indica o código BDI do papel. O código 02 indica que é um LOTE PADRÃO

                //posição 25 - 27 indica o tipo  de mercado do ativo
                //o tipo de mercado 010 é o mercado A VISTA
                if (strLinhaAux.Substring(0, 2) + strLinhaAux.Substring(10, 2) + strLinhaAux.Substring(24, 3) == "0102010")
                {
                    //se é a cotação de um papel
                    //e é do mercado à vista.

                    //busca código do ativo, posicao 13-24
                    string strCodigoAtivo = strLinhaAux.Substring(12, 12).Trim();

                    //verifica se o ativo não deve ser desconsiderado
                    if (strAtivosDesconsiderados.IndexOf("#" + strCodigoAtivo + "#", StringComparison.InvariantCultureIgnoreCase) < 0)
                    {
                        //se o ativo não foi encontrado na lista dos desconsiderados.

                        //TOTAL DE NEGÓCIOS (148-152)
                        long lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(147, 5));

                        //ALGUNS ATIVOS NÃO TEM NEGÓCIOS E NÃO DEVEM SER IMPORTADOS

                        if (lngNegociosTotal > 0)
                        {
                            //busca a data da cotação: 3-10 no formato YYYYMMDD
                            dtmCotacaoData = new DateTime(Convert.ToInt32(strLinhaAux.Substring(2, 4)), Convert.ToInt32(strLinhaAux.Substring(6, 2)), Convert.ToInt32(strLinhaAux.Substring(8, 2)));

                            //busca valor de abertura do ativo: 57 - 67 (inteiro), 68-69 (decimal)
                            decimal decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring(56, 11) + strSeparadorDecimal + strLinhaAux.Substring(67, 2));

                            //busca o valor máximo do ativo: 70-80 (inteiro), 81-82 (decimal)
                            decimal decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(69, 11) + strSeparadorDecimal + strLinhaAux.Substring(80, 2));

                            //busca o valor mínimo do ativo: 83-93 (inteiro), 94-95 (decimal)
                            decimal decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(82, 11) + strSeparadorDecimal + strLinhaAux.Substring(93, 2));

                            //busca o valor médio do ativo: 96-106 (inteiro), 107-108 (decimal)
                            decimal decValorMedio = Convert.ToDecimal(strLinhaAux.Substring(95, 11) + strSeparadorDecimal + strLinhaAux.Substring(106, 2));

                            //busca o valor de fechamento do ativo: 109-119 (inteiro), 120-121 (decimal)
                            decimal decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(108, 11) + strSeparadorDecimal + strLinhaAux.Substring(119, 2));

                            //TOTAL DE TÍTULOS NEGOCIADOS (153-170)
                            long lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(152, 18));

                            //VALOR TOTAL NEGOCIADO: 171-186 (inteiro), 187-188 (decimal)
                            decimal decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(170, 16) + strSeparadorDecimal + strLinhaAux.Substring(186, 2));

                            //calcula o sequencial do ativo
                            long lngSequencial = _sequencialService.SequencialCalcular(strCodigoAtivo, "Cotacao", pobjCommand.Conexao);

                            //insere na tabela
                            var dataFormatada = funcoesBd.CampoDateFormatar(dtmCotacaoData);
                            string strQuery = " insert into Cotacao " + "(Codigo, Data, DataFinal, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo " +
                                              ", Negocios_Total, Titulos_Total, Valor_Total, Sequencial) " + " values "
                                              + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtivo) + "," + dataFormatada + "," + dataFormatada + ","
                                              + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + "," + FuncoesBd.CampoDecimalFormatar(decValorFechamento)
                                              + "," + FuncoesBd.CampoDecimalFormatar(decValorMinimo) + "," + FuncoesBd.CampoDecimalFormatar(decValorMedio)
                                              + "," + FuncoesBd.CampoDecimalFormatar(decValorMaximo) + "," + lngNegociosTotal + "," + lngTitulosTotal + ","
                                              + FuncoesBd.CampoDecimalFormatar(decValorTotal) + "," + lngSequencial + ")";

                            pobjCommand.Execute(strQuery);

                        }
                        //if lngNegocios_Total > 0 then
                    }
                    //se o ativo não foi desconsiderado
                }
                //se é uma cotação à vista.
            }

            //atualiza a tabela de resumo
            TabelaResumoAtualizar(dtmCotacaoData, Constantes.DataInvalida);
        }

        /// <summary>
        /// 'Atualiza o registro da tabela Resumo. Esta tabela contém apenas sempre 1 registro.
        /// </summary>
        /// <param name="pdtmDataUltimaCotacao">Data em que foi buscada a última cotação das ações.
        ///     Não devem ser consideradas as cotações intraday </param>
        /// <param name="pdtmDataUltimoProvento">Data em que foi atualizado o último arquivo de proventos</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private void TabelaResumoAtualizar(DateTime pdtmDataUltimaCotacao, DateTime pdtmDataUltimoProvento)
        {

            cCommand objCommand = new cCommand(_conexao);

            string strQuery = String.Empty;

            bool blnTransacaoInterna = !_conexao.TransAberta;

            if (blnTransacaoInterna)
                objCommand.BeginTrans();

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            if (pdtmDataUltimaCotacao != Constantes.DataInvalida)
            {
                strQuery = "Data_Ultima_Cotacao = " + FuncoesBd.CampoDateFormatar(pdtmDataUltimaCotacao) + Environment.NewLine;

            }


            if (pdtmDataUltimoProvento != Constantes.DataInvalida)
            {

                if (strQuery != String.Empty)
                {
                    strQuery = strQuery + ", ";

                }

                strQuery = strQuery + "Data_Ultimo_Provento = " + FuncoesBd.CampoDateFormatar(pdtmDataUltimoProvento) + Environment.NewLine;

            }

            strQuery = "UPDATE Resumo SET " + Environment.NewLine + strQuery;

            //quando estiver atualizando a data da última cotaçao atualiza somente se a data for maior do que a data
            //já existente na tabela

            if (pdtmDataUltimaCotacao != Constantes.DataInvalida)
            {
                strQuery = strQuery + Environment.NewLine + " WHERE Data_Ultima_Cotacao < " + FuncoesBd.CampoDateFormatar(pdtmDataUltimaCotacao);

            }

            objCommand.Execute(strQuery);

            if (blnTransacaoInterna)
                objCommand.CommitTrans();

        }

        /// <summary>
        /// Busca as cotações direto do site da bovespa, baixa o xml para a pasta temporária,
        /// lê o xml e atualiza as cotações.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CotacaoIntraDayAtualizar(DateTime pdtmData, bool pblnCalcularDados)
        {
            string strAtivos = String.Empty;

            cCommand objCommand = new cCommand(_conexao);

            cRS objRS = new cRS(_conexao);

            IList<string> colAtivos = new List<string>();

            int intContador = 0;

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + funcoesBd.CampoDateFormatar(pdtmData));

            if ((int)objRS.Field("Contador") > 0)
            {
                //blnCotacaoIntradayExistir = True

                DateTime[] arrData = { pdtmData };


                if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK)
                {
                    objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return false;

                }
            }

            objRS.Fechar();

            objCommand.BeginTrans();

            //busca os ativos para os quais devem ser buscadas as cotações.
            //DEVE buscar todos os ativos que não foram desconsiderados.
            //Os ativos que ainda não foram cadastrados na tabela Ativo não serão considerados

            objRS.ExecuteQuery(" select Codigo " + " from Ativo " + " where Codigo not in " + "(" + " select Codigo " + " from Ativos_Desconsiderados" + ")");

            int intNumeroDeAtivosCotacaoIntraday = cBuscarConfiguracao.NumeroDeAtivosCotacaoIntraday();


            while (!objRS.EOF)
            {
                //Gera os códigos separados por "|"

                if (strAtivos != String.Empty)
                {
                    //teste para gerar o pipe a partir do segundo e não gerar para o último.
                    strAtivos = strAtivos + "|";

                }

                strAtivos = strAtivos + objRS.Field("Codigo");

                intContador = intContador + 1;


                if (intContador == intNumeroDeAtivosCotacaoIntraday)
                {
                    colAtivos.Add(strAtivos);

                    intContador = 0;

                    strAtivos = String.Empty;

                }

                objRS.MoveNext();

            }


            if (strAtivos != String.Empty)
            {
                colAtivos.Add(strAtivos);

            }

            objRS.Fechar();

            cWeb objWebLocal = new cWeb(_conexao);

            DateTime dtmData = pdtmData;
            //Dim strDataAux As String

            double dblNegociosTotal = 0;
            double dblTitulosTotal = 0;
            double dblValorTotal = 0;

            string strCaminhoPadrao = cBuscarConfiguracao.ObtemCaminhoPadrao();

            foreach (string strAtivosLoop in colAtivos)
            {
                string strUrl = "http://www.bmfbovespa.com.br/Pregao-Online/ExecutaAcaoAjax.asp?CodigoPapel=" + strAtivosLoop;


                if (objWebLocal.DownloadWithProxy(strUrl, strCaminhoPadrao + "temp", "cotacao.xml"))
                {
                    var objArquivoXml = new cArquivoXML(strCaminhoPadrao + "temp\\cotacao.xml");


                    if (!objArquivoXml.Abrir())
                    {
                        objCommand.RollBackTrans();

                        return false;

                    }


                    while ((!objArquivoXml.EOF()) && (objCommand.TransStatus))
                    {
                        decimal decValorAbertura = Convert.ToDecimal(objArquivoXml.LerAtributo("Abertura", 0));


                        if (decValorAbertura != 0)
                        {
                            var strCodigo = (string)objArquivoXml.LerAtributo("Codigo");

                            decimal decValorFechamento = Convert.ToDecimal(objArquivoXml.LerAtributo("Ultimo", 0));

                            decimal decValorMinimo = Convert.ToDecimal(objArquivoXml.LerAtributo("Minimo", 0));

                            decimal decValorMaximo = Convert.ToDecimal(objArquivoXml.LerAtributo("Maximo", 0));

                            decimal decValorMedio = Convert.ToDecimal(objArquivoXml.LerAtributo("Medio", 0));

                            decimal decOscilacao = Convert.ToDecimal(objArquivoXml.LerAtributo("Oscilacao", 0));

                            //CONSULTA A COTAÇÃO ANTERIOR PARA O ATIVO
                            DateTime dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(strCodigo, pdtmData, "Cotacao");


                            if (dtmCotacaoAnteriorData != Constantes.DataInvalida)
                            {
                                //QUANDO A COTAÇAO É INTRADAY, COMO NÃO TEMOS DADOS DE VOLUME,
                                //UTILIZA OS DADOS DA COTAÇÃO ANTERIOR.
                                objRS.ExecuteQuery("SELECT Titulos_Total, Negocios_Total, Valor_Total " + " FROM Cotacao " + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo) + " AND Data = " + funcoesBd.CampoDateFormatar(dtmCotacaoAnteriorData));

                                dblNegociosTotal = Convert.ToDouble(objRS.Field("Negocios_Total"));

                                dblTitulosTotal = Convert.ToDouble(objRS.Field("Titulos_Total"));

                                dblValorTotal = Convert.ToDouble(objRS.Field("Valor_Total"));

                                objRS.Fechar();

                            }

                            //calcula o sequencial do ativo
                            long lngSequencial = _sequencialService.SequencialCalcular(strCodigo, "Cotacao", objCommand.Conexao);

                            string strQuery = " insert into Cotacao " + "(Codigo, Data, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo, Oscilacao " + ", Titulos_Total, Negocios_Total, Valor_Total, Sequencial) " + " values " + "(" + FuncoesBd.CampoStringFormatar(strCodigo) + "," + funcoesBd.CampoDateFormatar(dtmData) + "," + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + "," + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + "," + FuncoesBd.CampoDecimalFormatar(decValorMinimo) + "," + FuncoesBd.CampoDecimalFormatar(decValorMedio) + "," + FuncoesBd.CampoDecimalFormatar(decValorMaximo) + "," + FuncoesBd.CampoDecimalFormatar(decOscilacao) + "," + FuncoesBd.CampoFloatFormatar(dblTitulosTotal) + "," + FuncoesBd.CampoFloatFormatar(dblNegociosTotal) + "," + FuncoesBd.CampoFloatFormatar(dblValorTotal) + "," + lngSequencial.ToString() + ")";

                            objCommand.Execute(strQuery);

                        }

                        objArquivoXml.LerNodo();

                    }

                    objArquivoXml.Fechar();


                }
                else
                {
                    objCommand.RollBackTrans();

                }

            }

            //inserir registro na tabela COTACAO_INTRADAY, para saber que esta cotação
            //foi salva com dados intraday e não com dados oficiais de fechamento

            //primeiro verifica se já existe registro na tabela, porque esta cotação pode ser baixada
            //várias vezes durante o dia.

            //ALTERADO EM 12/03/2010 PARA SEMPRE INSERIR O REGISTRO NA TABELA COTACAO_INTRADAY
            //POIS O REGISTRO É EXCLUIDO PELA FUNÇÃO COTACAOEXCLUIR QUE É CHAMADA ANTERIORMENTE
            //NESTA FUNÇÃO.

            //If Not blnCotacaoIntradayExistir Then

            objCommand.Execute(" INSERT INTO Cotacao_Intraday " + " (Data)" + " VALUES " + "(" + funcoesBd.CampoDateFormatar(dtmData) + ")");

            //End If

            objCommand.CommitTrans();


            if (objCommand.TransStatus)
            {
                if (pblnCalcularDados)
                {
                    _servicoDeCotacao.DadosRecalcular(true, false, true, true, true, true, true, true, true, true,
                        true, dtmData);
                }


                MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Information);


            }
            else
            {
                MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

            return objCommand.TransStatus;

        }

        /// <summary>
        /// Exclui as cotações das datas recebidas por parâmetros e recalcula os dados semanais se
        /// for necessário
        /// </summary>
        /// <param name="parrData">Array contendo a data das cotações que devem ser excluídas.
        /// O array está ordenado em ordem crescente.</param>
        /// <param name="pblnDadosSemanaisRecalcular"></param>
        /// <returns>
        /// RetornoOK = operação executada com sucesso.
        /// RetornoErroInesperado = algum erro de banco de dados ou de programação.
        /// RetornoErro2 = existem cotações posteriores às datas recebidas por parâmetro.
        /// </returns>
        /// <remarks></remarks>
        public cEnum.enumRetorno CotacaoExcluir(DateTime[] parrData, bool pblnDadosSemanaisRecalcular)
        {
            cEnum.enumRetorno functionReturnValue;

            cCommand objCommand = new cCommand(_conexao);

            cRS objRS = new cRS(_conexao);

            objCommand.BeginTrans();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //consistência para evitar buracos nas cotações.
            //Para isso verificar se existe alguma cotação maior do que a primeira do array
            //e diferente de todas as outras do array.

            string strQuery = " SELECT COUNT(1) AS Contador " + " FROM Cotacao " + " WHERE Data > " + funcoesBd.CampoDateFormatar(parrData[0]);


            for (int intI = 1; intI <= parrData.Length - 1; intI++)
            {
                strQuery = strQuery + " AND Data <> " + funcoesBd.CampoDateFormatar(parrData[intI]);

            }

            objRS.ExecuteQuery(strQuery);


            if (Convert.ToInt32(objRS.Field("Contador")) > 0)
            {
                //se existem alguma data intermediária não pode deixar buraco.
                objRS.Fechar();

                objCommand.RollBackTrans();

                return cEnum.enumRetorno.RetornoErro2;

            }

            objRS.Fechar();

            //COTACAO
            objCommand.Execute(" DELETE " + " FROM Cotacao " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

            //MEDIA_DIARIA

            objCommand.Execute(" DELETE " + " FROM Media_Diaria " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

            //IFR_DIARIO
            objCommand.Execute(" DELETE " + " FROM IFR_Diario " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

            var ativosBuilder = new StringBuilder();

            if (pblnDadosSemanaisRecalcular)
            {
                //DAS COTAÇÕES SEMANAIS DEVE EXCLUIR OS REGISTROS ONDE A PRIMEIRA DATA DA SEMANA
                //É MAIOR OU IGUAL A PRIMEIRA DATA DO ARRAY

                //COTACAO_SEMANAL
                objCommand.Execute(" DELETE " + " FROM Cotacao_Semanal " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

                //MEDIA_SEMANAL
                objCommand.Execute(" DELETE " + " FROM Media_Semanal " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

                //IFR_SEMANAL
                objCommand.Execute(" DELETE " + " FROM IFR_Semanal " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

                //verifica se existem cotações intraday para todos os ativos ou somente para ativos específicos
                objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

                if (Convert.ToInt32(objRS.Field("Contador")) == 0)
                {
                    objRS.Fechar();

                    //se não tem cotações intraday para todos os ativos
                    //lista apenas os ativos que tiveram cotação intraday para 
                    //fazer o recálculo dos dados semanais.
                    objRS.ExecuteQuery(" SELECT Codigo " + " FROM Cotacao_Intraday_Ativo " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]) + " GROUP BY Codigo");

                    const string separadorDeAtivos = "#";

                    if (objRS.DadosExistir)
                    {
                        ativosBuilder.Append(separadorDeAtivos);
                    }

                    while (!objRS.EOF)
                    {

                        ativosBuilder.Append(objRS.Field("Codigo"))
                            .Append(separadorDeAtivos);

                        objRS.MoveNext();

                    }

                }

                objRS.Fechar();

            }

            //EXCLUI REGISTROS DA TABELA COTACAO_INTRADAY
            objCommand.Execute(" DELETE " + " FROM Cotacao_Intraday " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

            //EXCLUI REGISTROS DA TABELA COTACAO_INTRADAY_ATIVO
            objCommand.Execute(" DELETE " + " FROM Cotacao_Intraday_Ativo " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(parrData[0]));

            objCommand.CommitTrans();


            if (objCommand.TransStatus)
            {

                if (pblnDadosSemanaisRecalcular)
                {
                    //CHAMA A FUNÇÃO DE RECÁLCULO PARA OS DADOS SEMANAIS.
                    //SE NÃO HOUVER REGISTROS NÃO CALCULARÁ
                    this._servicoDeCotacao.CotacaoSemanalDadosAtualizar(true, true, true, true, true, parrData[0], ativosBuilder.ToString());

                }

                functionReturnValue = cEnum.enumRetorno.RetornoOK;


            }
            else
            {
                functionReturnValue = cEnum.enumRetorno.RetornoErroInesperado;

            }
            return functionReturnValue;

        }


    }
}
