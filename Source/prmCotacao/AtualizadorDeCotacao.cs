using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Configuracao;
using DataBase;
using Services;
using ServicoNegocio;
using TraderWizard.Enumeracoes;
using WebAccess;

namespace TraderWizard.ServicosDeAplicacao
{
    public class AtualizadorDeCotacao
    {

        private readonly Conexao _conexao;
        private readonly SequencialService _sequencialService;
        private readonly CotacaoDataService _cotacaoData;
        private readonly ServicoDeCotacao _servicoDeCotacao;

        public AtualizadorDeCotacao()
        {
            this._conexao = new Conexao();
            this._sequencialService = new SequencialService();
            this._cotacaoData = new CotacaoDataService();
            this._servicoDeCotacao = new ServicoDeCotacao();
        }

        /// <summary>
        /// Atualiza as cotações em todas as datas em que há pregão em um determinado período.
        /// </summary>
        /// <param name="pdtmDataInicial">Data inicial de importação</param>
        /// <param name="pdtmDataFinal">Data final de importação</param>
        /// <param name="pstrCodigoUnico">Indica se é para importar o código de um único ativo. Se o parâmetro for uma string vazia deve importar todos os arquivos</param>
        /// <param name="pblnCalcularDados">Indica se após atualizar as cotações deve calcular indicadores como média, IFR, etc</param>
        /// <param name="historica"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool CotacaoPeriodoAtualizar(DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrCodigoUnico, bool pblnCalcularDados, bool historica)
        {
            IImportadorCotacao importador;
            if (historica)
            {
                importador = new ImportadorDadosHistoricos();
            }
            else
            {
                importador = new ImportadorBoletimDiario();
            }

            RS objRS = new RS(_conexao);

            //utilizado para calcular o sequencial do ativo.

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" SELECT DISTINCT Data " + " FROM Cotacao_Intraday " + " WHERE Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial));

            var datas = new Collection<DateTime>();

            while (!objRS.Eof)
            {
                datas.Add(Convert.ToDateTime(objRS.Field("Data")));
            }
            objRS.Fechar();

            if (datas.Count > 0)
            {

                if (CotacaoExcluir(datas.ToArray(), false) != cEnum.enumRetorno.RetornoOK)
                {
                    throw new Exception("Erro ao excluir as cotações intraday.");

                }

            }

            //inicializa a data com data inválida. Vai ser atribuido nesta variável o primeiro dia útil.

            DateTime? dtmDataUltimaCotacao = null;

            var calculadorData = new CalculadorData(_conexao);

            ICollection<string> ativosDesconsiderados = ObterAtivosDesconsiderados();

            var command = new Command(this._conexao);

            if (!calculadorData.DiaUtilVerificar(pdtmDataInicial))
            {
                pdtmDataInicial = calculadorData.DiaUtilSeguinteCalcular(pdtmDataInicial);
            }

            var dataInicioRecalculo = pdtmDataInicial;

            //enquanto a data inicial for menor que a data final.
            while (pdtmDataInicial <= pdtmDataFinal)
            {

                bool blnCotacaoExistir = pstrCodigoUnico == string.Empty
                    ? this._cotacaoData.CotacaoDataExistir(pdtmDataInicial, "Cotacao")
                    : this._cotacaoData.CotacaoDataExistir(pdtmDataInicial, "Cotacao", pstrCodigoUnico);

                if (blnCotacaoExistir)
                {
                    throw new Exception($"Já existe cotação na data {pdtmDataInicial:d}");
                }

                IEnumerable<CotacaoImportacao> cotacoes = importador.ObterCotacoes(pdtmDataInicial, pstrCodigoUnico, ativosDesconsiderados);

                try
                {
                    command.BeginTrans();

                    foreach (var cotacao in cotacoes)
                    {

                        var sb = new StringBuilder();
                        var dataFormatada = funcoesBd.CampoDateFormatar(cotacao.Data);
                        sb
                            .Append(" insert into Cotacao ")
                            .Append("(Codigo, Data, DataFinal, ValorAbertura, ValorFechamento, ValorMinimo, ValorMedio, ValorMaximo, Oscilacao")
                            .Append(", Valor_Total, Negocios_Total, Titulos_Total, Sequencial)")
                            .Append(" values ")
                            .Append($"({funcoesBd.CampoStringFormatar(cotacao.Codigo)}, ")
                            .Append($"{dataFormatada}, ")
                            .Append($"{dataFormatada}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.ValorAbertura)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.ValorFechamento)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.ValorMinimo)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.PrecoMedio)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.ValorMaximo)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.Oscilacao)}, ")
                            .Append($"{funcoesBd.CampoDecimalFormatar(cotacao.VolumeFinanceiro)}, ")
                            .Append($"{cotacao.QuantidadeNegocios}, {cotacao.QuantidadeNegociada}, {cotacao.Sequencial})");

                        command.Execute(sb.ToString());

                    }

                    command.CommitTrans();

                }
                catch
                {
                    command.RollBackTrans();
                    throw;
                }

                dtmDataUltimaCotacao = pdtmDataInicial;
                //incrementa um dia na data inicial
                pdtmDataInicial = calculadorData.DiaUtilSeguinteCalcular(pdtmDataInicial);

            }

            if (dtmDataUltimaCotacao.HasValue)
            {
                //se alguma cotação foi atualizada com sucesso atualiza a tabela de resumo.
                TabelaResumoAtualizar(dtmDataUltimaCotacao.Value, Constantes.DataInvalida);


                if (pblnCalcularDados)
                {
                    _servicoDeCotacao.DadosRecalcular(true, false, true, true, true, true, true, true, true, true, true, dataInicioRecalculo);

                }

                MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Não existem cotações para serem atualizadas.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }

            return true;

        }


        private ICollection<string> ObterAtivosDesconsiderados()
        {
            RS rs = new RS(_conexao);
            rs.ExecuteQuery(" select Codigo " + " from Ativos_Desconsiderados");

            var ativos = new Collection<string>();

            while (!rs.Eof)
            {
                ativos.Add(rs.Field("Codigo").ToString());

                rs.MoveNext();

            }

            rs.Fechar();

            return ativos;

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

            Command objCommand = new Command(_conexao);

            //objConexao = objCommand.Conexao

            RS objRS = new RS(_conexao);


            //**************** INICIO DA TRANSAÇÃO
            objCommand.BeginTrans();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //verifica se tem alguma cotação no ano. Se já existir avisa para o usuário.
            var ultimaData = new DateTime(pintAno, 12, 31);
            objRS.ExecuteQuery(" select count(1) as contador " + " from Cotacao " + " where Data >= " +
                               funcoesBd.CampoDateFormatar(new DateTime(pintAno, 1, 1)) + " and Data <= " +
                               funcoesBd.CampoDateFormatar(ultimaData));


            var contador = Convert.ToInt64(objRS.Field("Contador", 0));
            objRS.Fechar();
            if (contador > 0)
            {
                //se já existe cotação na data faz rollback para não sobrescrever
                objCommand.RollBackTrans();

                //retorno erro conforme descrito no cabeçalho da função
                return cEnum.enumRetorno.RetornoErro2;

            }


            TabelaResumoAtualizar(ultimaData, Constantes.DataInvalida);

            //descompactar o arquivo que contém as cotações

            //**************** FIM DA TRANSAÇÃO
            objCommand.CommitTrans();

            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

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

            Command objCommand = new Command(_conexao);

            string strQuery = String.Empty;

            bool blnTransacaoInterna = !_conexao.TransAberta;

            if (blnTransacaoInterna)
                objCommand.BeginTrans();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            if (pdtmDataUltimaCotacao != Constantes.DataInvalida)
            {
                strQuery = "Data_Ultima_Cotacao = " + funcoesBd.CampoDateFormatar(pdtmDataUltimaCotacao) + Environment.NewLine;

            }


            if (pdtmDataUltimoProvento != Constantes.DataInvalida)
            {

                if (strQuery != String.Empty)
                {
                    strQuery = strQuery + ", ";

                }

                strQuery = strQuery + "Data_Ultimo_Provento = " + funcoesBd.CampoDateFormatar(pdtmDataUltimoProvento) + Environment.NewLine;

            }

            strQuery = "UPDATE Resumo SET " + Environment.NewLine + strQuery;

            //quando estiver atualizando a data da última cotaçao atualiza somente se a data for maior do que a data
            //já existente na tabela

            if (pdtmDataUltimaCotacao != Constantes.DataInvalida)
            {
                strQuery = strQuery + Environment.NewLine + " WHERE Data_Ultima_Cotacao < " + funcoesBd.CampoDateFormatar(pdtmDataUltimaCotacao);

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

            Command objCommand = new Command(_conexao);

            RS objRS = new RS(_conexao);

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

            int intNumeroDeAtivosCotacaoIntraday = BuscarConfiguracao.NumeroDeAtivosCotacaoIntraday();


            while (!objRS.Eof)
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

            Web objWebLocal = new Web(_conexao);

            DateTime dtmData = pdtmData;
            //Dim strDataAux As String

            double dblNegociosTotal = 0;
            double dblTitulosTotal = 0;
            double dblValorTotal = 0;

            string strCaminhoPadrao = BuscarConfiguracao.ObtemCaminhoPadrao();

            foreach (string strAtivosLoop in colAtivos)
            {
                string strUrl = "http://www.bmfbovespa.com.br/Pregao-Online/ExecutaAcaoAjax.asp?CodigoPapel=" + strAtivosLoop;


                if (objWebLocal.DownloadWithProxy(strUrl, strCaminhoPadrao + "temp", "cotacao.xml"))
                {
                    var objArquivoXml = new ArquivoXml(strCaminhoPadrao + "temp\\cotacao.xml");


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
                                objRS.ExecuteQuery("SELECT Titulos_Total, Negocios_Total, Valor_Total " + " FROM Cotacao " + " WHERE Codigo = " + funcoesBd.CampoStringFormatar(strCodigo) + " AND Data = " + funcoesBd.CampoDateFormatar(dtmCotacaoAnteriorData));

                                dblNegociosTotal = Convert.ToDouble(objRS.Field("Negocios_Total"));

                                dblTitulosTotal = Convert.ToDouble(objRS.Field("Titulos_Total"));

                                dblValorTotal = Convert.ToDouble(objRS.Field("Valor_Total"));

                                objRS.Fechar();

                            }

                            //calcula o sequencial do ativo
                            long lngSequencial = _sequencialService.SequencialCalcular(strCodigo, "Cotacao", objCommand.Conexao);

                            string strQuery = " insert into Cotacao " + "(Codigo, Data, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo, Oscilacao " + ", Titulos_Total, Negocios_Total, Valor_Total, Sequencial) " + " values " + "(" + funcoesBd.CampoStringFormatar(strCodigo) + "," + funcoesBd.CampoDateFormatar(dtmData) + "," + funcoesBd.CampoDecimalFormatar(decValorAbertura) + "," + funcoesBd.CampoDecimalFormatar(decValorFechamento) + "," + funcoesBd.CampoDecimalFormatar(decValorMinimo) + "," + funcoesBd.CampoDecimalFormatar(decValorMedio) + "," + funcoesBd.CampoDecimalFormatar(decValorMaximo) + "," + funcoesBd.CampoDecimalFormatar(decOscilacao) + "," + funcoesBd.CampoFloatFormatar(dblTitulosTotal) + "," + funcoesBd.CampoFloatFormatar(dblNegociosTotal) + "," + funcoesBd.CampoFloatFormatar(dblValorTotal) + "," + lngSequencial + ")";

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

            Command objCommand = new Command(_conexao);

            RS objRS = new RS(_conexao);

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

                    while (!objRS.Eof)
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
