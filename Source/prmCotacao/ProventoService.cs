using System;
using System.Diagnostics;
using DataBase;
using prjConfiguracao;
using prjServicoNegocio;
using Services;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{
    public class ProventoService
    {

        private readonly Conexao _conexao;
        private readonly CotacaoData _cotacaoData;
        private readonly ServicoDeCotacao _servicoDeCotacao;

        public ProventoService()
        {
            this._conexao = new Conexao();
            this._cotacaoData = new CotacaoData();
            this._servicoDeCotacao = new ServicoDeCotacao();
        }
        /// <summary>
        /// Busca os registros de proventos (dividendos e juros) na tabela de Proventos e atualiza na tabela Split
        /// </summary>
        /// <param name="pdtmDataFinal">última data para a busca dos proventos</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ProventoAtualizar(DateTime pdtmDataFinal)
        {
            cCommand objCommand = new cCommand(_conexao);

            cRS objRS = new cRS(_conexao);

            cRS objRSAtivo = new cRS(_conexao);

            string strWhere = String.Empty;

            //Dim dtmData_Ultimo_Provento As Date

            string strCodigoAtual = String.Empty;
            DateTime dtmDataInicioRecalculo = Constantes.DataInvalida;

            //código no sistema de cotações equivalente ao conjunto Nome_Pregao + Tipo_Acao
            //Dim strCodigoProvento As String

            string strNomePregaoAtual = String.Empty;
            string strTipoAcaoAtual = String.Empty;

            string strNomePregaoNaoEncontrado = String.Empty;
            string strTipoAcaoNaoEncontrado = String.Empty;

            string strUltimoNomePregao = String.Empty;
            string strUltimoTipoAcao = String.Empty;

            //indica se o provento foi importado, ou seja, foi encontrado o ativo na tabela de ativos 
            //para importá-lo.
            bool blnImportado = false;

            //usado para o cálculo da primeira dat ex-provento

            //contém a data da primeira cotação para o papel

            string strTipoProvento = null;

            var objCalculadorData = new cCalculadorData(_conexao);

            //prepara os dados da tabela temporária
            objCommand.BeginTrans();

            //Exclui os registros em branco do final da planilha do Excel que também são importados
            objCommand.Execute("DELETE " + Environment.NewLine + " FROM Proventos_Temp " + Environment.NewLine + " WHERE Nome_Pregao Is Null");

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //Atualiza campo Data_Aprovacao para uma formato de data. O campo é do tipo string na tabela
            //Temporária e na hora de importar fica com um formato numérico contando os dias que se passaram
            //desde 30/12/1899. Não faz isso se o campo já possuir 10 caracteres, pois o formato dd/MM/yyyy ocupa 
            //10 caracteres e se já for encontrado o separados das datas ("/") também.

            objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine +
                               " DATA_APROVACAO = " + funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_APROVACAO) " + Environment.NewLine +
                               " WHERE DATA_APROVACAO <> " + FuncoesBd.CampoStringFormatar("ESTATUTÁRIO") + Environment.NewLine +
                               " And len(DATA_APROVACAO) <> 10 " + Environment.NewLine +
                               " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "DATA_APROVACAO") + " = 0 ");


            objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine +
                               "DATA_ULTIMO_PRECO_COM = " + funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_ULTIMO_PRECO_COM) " + Environment.NewLine +
                               "WHERE DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar("PREÇO TEÓRICO") + Environment.NewLine +
                               " And len(DATA_ULTIMO_PRECO_COM) <> 10 " + Environment.NewLine +
                               " And DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar(" ") + Environment.NewLine +
                               " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "DATA_ULTIMO_PRECO_COM") + " = 0 ");

            objCommand.Execute(" UPDATE PROVENTOS_TEMP SET " + " ULTIMO_DIA_COM = " +
                               funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(ULTIMO_DIA_COM) " +
                               Environment.NewLine +
                               " WHERE(Len(ULTIMO_DIA_COM) <> 10) " +
                               " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "ULTIMO_DIA_COM") + " = 0 ");

            objCommand.CommitTrans();


            if (!objCommand.TransStatus)
            {
                return false;

            }

            //consulta a data da primeira cotação e do último provento na tabela Resumo
            objRS.ExecuteQuery("SELECT Data_Primeira_Cotacao " + Environment.NewLine + "FROM Resumo ");

            DateTime dtmData_Primeira_Cotacao = Convert.ToDateTime(objRS.Field("Data_Primeira_Cotacao", Constantes.DataInvalida));
            //dtmData_Ultimo_Provento = CDate(objRS.Field("Data_Ultimo_Provento", DataInvalida))

            objRS.Fechar();

            //busca todos os proventos da tabela de proventos após a data do último provento, 
            //caso a mesma esteja preenchida.

            string strQuery = "SELECT Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + ", SUM(Valor_Provento) AS Valor_Provento " + "FROM Proventos_Temp PT " + Environment.NewLine + " WHERE ";


            strWhere = "CDBL(Ultimo_Preco_com) > 0 " + Environment.NewLine;


            if (dtmData_Primeira_Cotacao != Constantes.DataInvalida)
            {
                //busca os proventos a partir da data da primeira cotação.
                strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) >= " + funcoesBd.CampoDateFormatar(dtmData_Primeira_Cotacao) + Environment.NewLine;

            }


            if (pdtmDataFinal != Constantes.DataInvalida)
            {
                //a data final é a maior data para qual deve ser buscada os proventos.
                //geralmente é utilizada somente quando é necessário atualizar proventos 
                //anteriores aos proventos já cadastrados.
                strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

            }

            //WHERE que verifica se o registro já foi processado.
            strWhere = strWhere + " AND NOT EXISTS ( " + Environment.NewLine + " SELECT 1 " + Environment.NewLine + " FROM Proventos P " + Environment.NewLine + " WHERE PT.Nome_Pregao = P.Nome_Pregao " + Environment.NewLine + " AND PT.Tipo_Acao = P.Tipo_Acao " + Environment.NewLine + " AND PT.Tipo_Provento = P.Tipo_Provento " + Environment.NewLine + " AND PT.Ultimo_Dia_Com = P.Ultimo_Dia_Com " + Environment.NewLine + ")";

            strQuery = strQuery + strWhere + " GROUP BY Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + " ORDER BY Nome_Pregao, Tipo_Acao, CDATE(Ultimo_Dia_Com)";

            objRS.ExecuteQuery(strQuery);

            string conteudoDoArquivoDeLog = "";

            //No loop a seguir podem existir vários registros seguidos para a mesma ação
            //(Nome_Pregao + Tipo_Acao). Por isso a cada iteração tem que verificar se o ativo mudou
            while ((!objRS.EOF) && (objCommand.TransStatus))
            {

                var strNomePregao = (string)objRS.Field("Nome_Pregao");
                var strTipoAcao = (string)objRS.Field("Tipo_Acao");
                if (strNomePregaoNaoEncontrado != strNomePregao || strTipoAcaoNaoEncontrado != strTipoAcao)
                {
                    //se a ação não está marcada como não encontrada na tabela de ativos prossegue.
                    //Caso contrário o programa vai para o próximo registro pois não entrará neste IF.


                    if (strNomePregaoAtual != strNomePregao || strTipoAcaoAtual != strTipoAcao)
                    {
                        //inicia transação para trabalhar com os dados deste ativo.
                        objCommand.BeginTrans();

                        //inicializa o campo de data de início do recálculo
                        dtmDataInicioRecalculo = Constantes.DataInvalida;

                        //se é um ativo novo comparado com o ativo anterior
                        objRSAtivo.ExecuteQuery("SELECT Codigo " + Environment.NewLine + " FROM Ativo " + Environment.NewLine + " WHERE Descricao = " + FuncoesBd.CampoStringFormatar(objRS.Field("Nome_Pregao") + " " + objRS.Field("Tipo_Acao")) + Environment.NewLine + " OR Descricao = " + FuncoesBd.CampoStringFormatar((string)objRS.Field("Nome_Pregao") + (string)objRS.Field("Tipo_Acao")));


                        if (objRSAtivo.DadosExistir)
                        {
                            //ativo encontrado
                            strCodigoAtual = (string)objRSAtivo.Field("Codigo");

                            strNomePregaoAtual = (string)objRS.Field("Nome_Pregao");
                            strTipoAcaoAtual = (string)objRS.Field("Tipo_Acao");



                        }
                        else
                        {
                            //seta o código para vazio para neste caso não fazer insert na tabela de split
                            strCodigoAtual = String.Empty;

                            strNomePregaoNaoEncontrado = (string)objRS.Field("Nome_Pregao");
                            strTipoAcaoNaoEncontrado = (string)objRS.Field("Tipo_Acao");

                            //neste caso com certeza não vai importar os proventos.
                            blnImportado = false;

                            //lança log informando que não encontrou o ativo.
                            conteudoDoArquivoDeLog += "Ativo não encontrado - Nome: " + strNomePregaoNaoEncontrado + " - Tipo de ação: " + strTipoAcaoNaoEncontrado;

                        }

                        objRSAtivo.Fechar();

                        strUltimoNomePregao = (string)objRS.Field("Nome_Pregao");
                        strUltimoTipoAcao = (string)objRS.Field("Tipo_Acao");

                    }
                    //fim do if que testa se é um código novo de ativo.


                    if (strCodigoAtual != String.Empty)
                    {

                        //busca a data da primeira cotação para o papel
                        DateTime dtmDataPrimeiraCotacao = AtivoPrimeiraCotacaoDataConsultar(strCodigoAtual);

                        if (Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao)
                        {

                            if (dtmDataInicioRecalculo == Constantes.DataInvalida)
                            {
                                dtmDataInicioRecalculo = Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com"));

                            }

                            //calcula a data ex-provento, que é a próxima data em que houver cotação para o ativo
                            //após o "último dia com"
                            DateTime dtmDataExProvento = AtivoCotacaoPosteriorDataConsultar(strCodigoAtual, Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));


                            if (dtmDataExProvento == Constantes.DataInvalida)
                            {
                                conteudoDoArquivoDeLog += "Não foram encontradas cotações após o dia ex-provento." + "Verificar a data cadastrada na tabela Split - Código: " + strCodigoAtual + " - Data ex-provento: " + Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")).ToString("dd/MM/yyyy");

                                //se não encontrou a data da próxima cotação é porque a atualização de proventos 
                                //veio no dia da última cotação. Neste caso busca o próximo dia útil como sendo data-ex.
                                //Tem que cuida para caso haja algum feriado ainda não cadastrado na tabela "Feriado"
                                //que pode fazer com que a data do próximo dia útil fique calculada errada e por 
                                //consequencia também a data do último dia com provento.
                                //Para alertar o usuário lança log informando que não encontrou cotação após o
                                //dia ex-dividendo.
                                dtmDataExProvento = objCalculadorData.DiaUtilSeguinteCalcular(Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));

                            }

                            var strTipoDeProventoPorExtenso = (string)objRS.Field("Tipo_Provento");

                            if (strTipoDeProventoPorExtenso == "DIVIDENDO")
                            {
                                strTipoProvento = "DIV";
                            }
                            else if ((strTipoDeProventoPorExtenso == "JRS CAP PRÓPRIO") || (strTipoDeProventoPorExtenso == "JRS CAP PROPRIO"))
                            {
                                strTipoProvento = "JCP";
                            }
                            else if (strTipoDeProventoPorExtenso == "RENDIMENTO")
                            {
                                strTipoProvento = "REND";
                            }
                            else if (strTipoDeProventoPorExtenso == "REST CAP DIN")
                            {
                                strTipoProvento = "RCDIN";
                            }
                            else
                            {
                                strTipoProvento = String.Empty;

                            }

                            //Verifica se o provento já está cadastrado
                            objRSAtivo.ExecuteQuery("SELECT COUNT(1) AS Contador " + Environment.NewLine + " FROM Split " + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigoAtual) + Environment.NewLine + " AND Data = " + funcoesBd.CampoDateFormatar(dtmDataExProvento) + Environment.NewLine + " AND Tipo = " + FuncoesBd.CampoStringFormatar(strTipoProvento));


                            if (Convert.ToInt32(objRSAtivo.Field("Contador")) == 0)
                            {
                                //Se o provento ainda não está cadastrado, cadastra-o.
                                strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtual) + ", " + funcoesBd.CampoDateFormatar(dtmDataExProvento) + ", " + FuncoesBd.CampoStringFormatar(strTipoProvento);

                                double dblQuantidadeAnterior = Convert.ToDouble(objRS.Field("Ultimo_Preco_Com")) - Convert.ToDouble(objRS.Field("Valor_Provento"));

                                strQuery = strQuery + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))) + ")";

                                objCommand.Execute(strQuery);

                                //marca que o ativo será importado para depois carregar da tabela
                                //Proventos_Temp para a tabela Proventos
                                blnImportado = true;


                            }
                            else
                            {
                                //lança log informando que já existe provento cadastrado
                                conteudoDoArquivoDeLog += "Já existe provento cadastrado - Codigo: " + strCodigoAtual + " - Data: " + dtmDataExProvento.ToString("dd/MM/yyyy") + " - Tipo de Provento: " + objRS.Field("Tipo_Provento");

                                blnImportado = false;

                            }


                        }
                        else
                        {
                            //Se a data do provento é anterior à data da primeira cotação do ativo não importa
                            blnImportado = false;

                        }
                        //If CDate(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao Then

                        //copia os dados do provento da tabela temporária para a tabela definitiva.
                        //copia somente para os registros que estão sendo processados no momento utilizando
                        //como campos chaves: Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com, Ultimo_Preco_Com
                        objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco" + ", " + (blnImportado ? "1" : "0") + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine + " AND Tipo_Provento = " + FuncoesBd.CampoStringFormatar((string)objRS.Field("Tipo_Provento")) + Environment.NewLine + " AND Ultimo_Dia_Com = " + FuncoesBd.CampoStringFormatar((string)objRS.Field("Ultimo_Dia_Com")) + " AND Ultimo_Preco_Com = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))));

                    }
                    //If strCodigoAtual <> vbNullString Then

                }
                //fim do if que testa se é um código não encontrado

                objRS.MoveNext();

                bool blnExecutar;
                if (objRS.EOF)
                {
                    //assinala que tem que executar para que o último registro não fique sem execução
                    blnExecutar = true;

                }
                else
                {
                    if (strUltimoNomePregao != (string)objRS.Field("Nome_Pregao") || strUltimoTipoAcao != (string)objRS.Field("Tipo_Acao"))
                    {
                        //se vai trocar de ativo na próxima iteração tem que executar
                        blnExecutar = true;
                    }
                    else
                    {
                        blnExecutar = false;
                    }

                }


                if (blnExecutar)
                {

                    if (strUltimoNomePregao == strNomePregaoNaoEncontrado && strUltimoTipoAcao == strTipoAcaoNaoEncontrado)
                    {
                        //se o último papel é um papel não encontrado copia todos os registros deste papel
                        //da tabela Proventos_Temp para a tabela Proventos com o campo Importado = 0.
                        objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco, 0" + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine);

                    }

                    //quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
                    //em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
                    objCommand.CommitTrans();

                    //Este teste do IF indica que o ativo anterior da tabela Proventos_Temp
                    //foi encontrado na tabela Ativo e importado, portanto deve ser feito recálculo.

                    if (blnImportado)
                    {
                        conteudoDoArquivoDeLog += "Atualizando indicadores - Código: " + strCodigoAtual + " - Data Início: " + dtmDataInicioRecalculo.ToString("dd/MM/yyyy") + "- Tipo de Provento: " + strTipoProvento;

                        //chamar função para fazer recálculo dos dados do ativo anterior....
                        //Não é necessário recalcular o volume médio nas periodicidades diário e semanal
                        //porque os proventos não alteram o volume de ações. Apenas os desdobramentos
                        //que alteram
                        _servicoDeCotacao.DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
                            true, dtmDataInicioRecalculo, "#" + strCodigoAtual + "#", true, true);

                    }

                }

            }

            objRS.Fechar();

            string caminhoDoArquivo = cBuscarConfiguracao.ObtemCaminhoPadrao() + "Log_Proventos\\Log_Proventos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            IFileService fileService = new FileService();
            fileService.Save(caminhoDoArquivo, conteudoDoArquivoDeLog);

            Process.Start("c:\\windows\\notepad.exe", caminhoDoArquivo);
            return objCommand.TransStatus;

        }

        /// <summary>
        /// Cadastra o provento na tabela "Proventos", cadastra o provento na tabela "Split", atualiza as cotações a
        /// partir da data Ex do provento.
        /// </summary>
        /// <param name="pstrCodigo">Código do ativo</param>
        /// <param name="pintProventoTipo">Tipo de provento que foi pago: dividendo, juros, etc</param>
        /// <param name="pdtmDataAprovacao">Data em que foi aprovando o pagamento do provento</param>
        /// <param name="pdtmDataEx">Data ex-provento</param>
        /// <param name="pdecValorPorAcao">valor do provento pago por ação</param>
        /// <returns></returns>
        /// RetornoOK = operação executada com sucesso.
        /// RetonoErroInesperado = erro de banco de dados ou de programação
        /// RetornoErro2 = não existe cotação na data Ex do provento
        /// <remarks></remarks>
        public cEnum.enumRetorno ProventoCadastrar(string pstrCodigo, cEnum.enumProventoTipo pintProventoTipo, DateTime pdtmDataAprovacao, DateTime pdtmDataEx, decimal pdecValorPorAcao)
        {
            cEnum.enumRetorno functionReturnValue;

            decimal decUltimoPrecoCom = default(decimal);
            const string strTabelaCotacao = "COTACAO";
            string strProventoTipoAbreviatura;
            string strProventoTipoDescricao;

            cCommand objCommand = new cCommand(_conexao);

            cCalculadorData objCalculadorData = new cCalculadorData(_conexao);


            objCommand.BeginTrans();

            //Verifica se existe cotação na data Ex.

            if (!this._cotacaoData.CotacaoDataExistir(pdtmDataEx, strTabelaCotacao))
            {
                objCommand.RollBackTrans();

                return cEnum.enumRetorno.RetornoErro2;

            }

            //busca a última data em que houve cotação antes da data ex-provento
            DateTime dtmDataUltimoPrecoCom = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataEx, strTabelaCotacao);

            //busca a cotação na última data com
            decimal pdecValorAberturaRet = -1M;
            this._servicoDeCotacao.CotacaoConsultar(pstrCodigo, dtmDataUltimoPrecoCom, strTabelaCotacao, ref decUltimoPrecoCom, ref pdecValorAberturaRet);

            //busca o último dia útil antes da data ex-provento
            DateTime dtmUltimoDiaCom = objCalculadorData.DiaUtilAnteriorCalcular(pdtmDataEx);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            switch (pintProventoTipo)
            {

                case cEnum.enumProventoTipo.Dividendo:

                    strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("DIV");
                    strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("DIVIDENDO");

                    break;
                case cEnum.enumProventoTipo.JurosCapitalProprio:

                    strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("JCP");
                    strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("JRS CAP PRÓPRIO");

                    break;
                case cEnum.enumProventoTipo.Rendimento:

                    strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("REND");
                    strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("RENDIMENTO");

                    break;
                case cEnum.enumProventoTipo.RestCapDin:

                    strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("RCDIN");
                    strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("REST CAP DIN");

                    break;
                default:

                    strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar(String.Empty);
                    strProventoTipoDescricao = FuncoesBd.CampoStringFormatar(String.Empty);

                    break;
            }

            double dblQuantidadeAnterior = Convert.ToDouble(decUltimoPrecoCom) - Convert.ToDouble(pdecValorPorAcao);

            string strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + funcoesBd.CampoDateFormatar(pdtmDataEx) + ", " + strProventoTipoAbreviatura + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ")";

            objCommand.Execute(strQuery);

            cRS objRS = new cRS(_conexao);

            //consulta a descrição do ativo
            strQuery = "SELECT Descricao " + Environment.NewLine + "FROM Ativo " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo);

            objRS.ExecuteQuery(strQuery);

            string strNomePregao = Convert.ToString(objRS.Field("Descricao")).Substring(0, Convert.ToString(objRS.Field("Descricao")).Length - 3).Trim();

            string strTipoAcao = Convert.ToString(objRS.Field("Descricao")).Substring(Convert.ToString(objRS.Field("Descricao")).Length - 3).Trim();

            objRS.Fechar();

            strQuery = "INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strNomePregao) + ", " + FuncoesBd.CampoStringFormatar(strTipoAcao) + ", " + FuncoesBd.CampoStringFormatar(pdtmDataAprovacao.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(pdecValorPorAcao)) + ", 1, " + strProventoTipoDescricao + ", " + FuncoesBd.CampoStringFormatar(dtmUltimoDiaCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoStringFormatar(dtmDataUltimoPrecoCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ", 1" + ", " + FuncoesBd.CampoStringFormatar((pdecValorPorAcao / decUltimoPrecoCom * 100M).ToString("0##.##")) + ", 1)";

            objCommand.Execute(strQuery);


            //quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
            //em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
            objCommand.CommitTrans();



            if (objCommand.TransStatus)
            {
                functionReturnValue = cEnum.enumRetorno.RetornoOK;

                //chamar função para fazer recálculo dos dados do ativo anterior....
                //Não é necessário recalcular o volume médio nas periodicidades diário e semanal
                //porque os proventos não alteram o volume de ações. Apenas os desdobramentos
                //que alteram
                _servicoDeCotacao.DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
                    true, dtmUltimoDiaCom, "#" + pstrCodigo + "#", true, true);


            }
            else
            {
                functionReturnValue = cEnum.enumRetorno.RetornoErroInesperado;

            }
            return functionReturnValue;

        }

        /// <summary>
        /// Calcula a cotação imediatamente posterior a uma data para um determinado ativo.
        /// </summary>
        /// <param name="pstrCodigo"></param>
        /// <param name="pdtmDataBase"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private DateTime AtivoCotacaoPosteriorDataConsultar(string pstrCodigo, DateTime pdtmDataBase)
        {
            cRS objRsData = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRsData.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data > " + funcoesBd.CampoDateFormatar(pdtmDataBase));

            DateTime functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

            objRsData.Fechar();
            return functionReturnValue;

        }

        private DateTime AtivoPrimeiraCotacaoDataConsultar(string pstrCodigo)
        {
            cRS objRS = new cRS(_conexao);

            objRS.ExecuteQuery("SELECT MIN(Data) AS Data " + Environment.NewLine + "FROM Cotacao " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

            var functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

            objRS.Fechar();
            return functionReturnValue;

        }

    }
}
