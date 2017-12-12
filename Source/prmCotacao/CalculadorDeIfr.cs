using System;
using System.Collections.Generic;
using System.Linq;
using Configuracao;
using DataBase;
using DataBase.Carregadores;
using DTO;
using Services;
using ServicoNegocio;
using TraderWizard.Enumeracoes;

namespace TraderWizard.ServicosDeAplicacao
{
    public class CalculadorDeIfr
    {

        private readonly Conexao _conexao;
        private readonly CotacaoDataService _cotacaoData;

        public CalculadorDeIfr()
        {
            this._conexao = new Conexao();
            _cotacaoData = new CotacaoDataService();
        }

        /// <summary>
        /// Atualiza o valor do IFR no banco de dados
        /// </summary>
        /// <param name="pstrCodigo">Código do papel</param>
        /// <param name="pdtmData"></param>
        /// <param name="pintPeriodo"></param>
        /// <param name="pstrTabela"></param>
        /// <param name="pdblIFR"></param>
        /// <param name="pdblMediaAlta"></param>
        /// <param name="pdblMediaBaixa"></param>
        /// <param name="pobjConexao"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private void IFRAtualizar(string pstrCodigo, DateTime pdtmData, int pintPeriodo, string pstrTabela, double pdblIFR, double pdblMediaAlta, double pdblMediaBaixa, Conexao pobjConexao = null)
        {
            Command objCommand = pobjConexao == null ? new Command(_conexao) : new Command(pobjConexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //******ALTERADO POR MAURO, 19/12/2009
            //******ARMAZENAMENTO DO IFR EM TABELA PRÓPRIA PARA O IFR DIÁRIO E PARA O IFR SEMANAL
            string strQuery = " INSERT INTO " + pstrTabela + "(Codigo, Data, NumPeriodos, MediaBaixa, MediaAlta, Valor) " + " VALUES " + 
                "(" + funcoesBd.CampoStringFormatar(pstrCodigo) + ", " + funcoesBd.CampoDateFormatar(pdtmData) + ", " + pintPeriodo + ", " + 
                funcoesBd.CampoDecimalFormatar((decimal?)pdblMediaBaixa) + ", " + funcoesBd.CampoDecimalFormatar((decimal?)pdblMediaAlta) + ", " + 
                funcoesBd.CampoDecimalFormatar((decimal?)pdblIFR) + ")";

            objCommand.Execute(strQuery);
        }

        private decimal IFRCalcular(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, int pintNumPeriodos, string pstrTabela,
            out double pdblMediaAltaAnteriorRet, out double pdblMediaBaixaAnteriorRet, Conexao pobjConexao = null)
        {
            RS objRS = pobjConexao == null ? new RS(_conexao) : new RS(pobjConexao);

            RSList objRSListSplit = null;

            //contém a razão acumulada de todos os splits do período.
            double dblSplitAcumulado = 1;
            double dblDiferencaPositivaAcumulada = 0;
            double dblDiferencaNegativaAcumulada = 0;

            double dblIFR;

            CarregadorSplit objCarregadorSplit = new CarregadorSplit(objRS.Conexao);

            //busca os splits do ativo no período, ordenado pelo último split.
            //Tem que começar a buscar um dia após 
            bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial.AddDays(1), "D", ref objRSListSplit, pdtmDataFinal);


            if (blnSplitExistir)
            {
                DateTime dtmDataFinalSplit = pdtmDataFinal;

                //quando tem script todas as cotaçoes tem que ser convertidas de acordo com o split

                DateTime dtmDataInicialSplit;
                while (!objRSListSplit.Eof)
                {

                    if (Convert.ToDateTime(objRSListSplit.Field("Data")) != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)))
                    {
                        dtmDataInicialSplit = pstrTabela.ToUpper() == "COTACAO" ? Convert.ToDateTime(objRSListSplit.Field("Data")) : _cotacaoData.PrimeiraSemanaDataCalcular(Convert.ToDateTime(objRSListSplit.Field("Data")));

                        //acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
                        dblDiferencaPositivaAcumulada = dblDiferencaPositivaAcumulada + (double)CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "P", pstrTabela) * dblSplitAcumulado;

                        //acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
                        dblDiferencaNegativaAcumulada = dblDiferencaNegativaAcumulada + (double)CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "N", pstrTabela) * dblSplitAcumulado;

                        //a data final para o próximo período e um dia antes ao periodo anterior
                        dtmDataFinalSplit = dtmDataInicialSplit.AddDays(-1);

                    }

                    //multiplica o split anterior pelo split da data para o próximo periodo
                    dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("Razao"));

                    objRSListSplit.MoveNext();

                }

                //após o loop de splits ainda tem que somar o acumulado entre a data inicial e o primeiro split.
                dtmDataInicialSplit = pdtmDataInicial;

                //acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
                dblDiferencaPositivaAcumulada = dblDiferencaPositivaAcumulada + (double)CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "P", pstrTabela) * dblSplitAcumulado;

                //acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
                dblDiferencaNegativaAcumulada = dblDiferencaNegativaAcumulada + (double)CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "N", pstrTabela) * dblSplitAcumulado;

                //calcula a média das diferenças acumuladas
                pdblMediaAltaAnteriorRet = dblDiferencaPositivaAcumulada / pintNumPeriodos;
                pdblMediaBaixaAnteriorRet = dblDiferencaNegativaAcumulada / pintNumPeriodos;


            }
            else
            {
                //SE NÃO TEM SPLIT FAZ O CÁLCULO NORMAL.
                FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

                //calcula a média do valor de fechamento nos ultimos pintNumPeriodos com cotação positiva
                string strQuery = " select ROUND(SUM(Diferenca)/" + pintNumPeriodos + ",6) as MediaPositiva " + " from " + pstrTabela + 
                    " where Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal) + " and Diferenca > 0 ";

                objRS.ExecuteQuery(strQuery);

                pdblMediaAltaAnteriorRet = Convert.ToDouble(objRS.Field("MediaPositiva", 0));

                objRS.Fechar();

                //calcula a média do valor de fechamento nos ultimos pintNumPeriodos com cotação negativa.
                //ANTES DE SOMAR CONVERTE O VALOR PARA O VALOR ABSOLUTO, POIS A DIFERENÇA NESTES CASOS ESTÁ COM SINAL NEGATIVO
                strQuery = " select ROUND(SUM(ABS(Diferenca)) / " + pintNumPeriodos + ",6) as MediaNegativa " + " from " + pstrTabela + 
                    " where Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal) + " and Diferenca < 0 ";

                objRS.ExecuteQuery(strQuery);

                pdblMediaBaixaAnteriorRet = Convert.ToDouble(objRS.Field("MediaNegativa", 0));

                objRS.Fechar();

            }

            //calcula o INDICE DE FORÇA RELATIVA
            //IFR = 100 - (100 / (1 + U / D))
            //U = média das cotações dos últimos N dias em que a cotação subiu
            //D = média das cotações dos últimos N dias em que a cotação desceu 


            if (pdblMediaBaixaAnteriorRet != 0)
            {
                dblIFR = 100 - (100 / (1 + pdblMediaAltaAnteriorRet / pdblMediaBaixaAnteriorRet));
            }
            else
            {
                dblIFR = 100;
            }

            return Convert.ToDecimal(dblIFR);

        }
        /// <summary>
        /// Calcula o IFR para um determinado número de períodos tendo a informação de médias do último
        /// período e uma diferença entre de cotação em relação ao último período.
        /// </summary>
        /// <param name="pintNumPeriodos">Número de períodos utilizados no cálculo</param>
        /// <param name="pdblMediaAltaAnterior">Média de altas dos últimos pintNumPeriodos</param>
        /// <param name="pdblMediaBaixaAnterior">Média de baixas dos últimos pintNumPeriodos</param>
        /// <param name="pdecDiferenca">diferença entre o período que será calculado o IFR e o período imediatamente anterior</param>
        /// <param name="pdblMediaAltaAtualRet">Retorna a média de alta já considerando o período para o qual o IFR está sendo calculado</param>
        /// <param name="pdblMediaBaixaAtualRet">Retorna a média de baixa já considerando o período para o qual o IFR está sendo calculado</param>
        /// <returns>O indice de força relativa calculado</returns>
        /// <remarks></remarks>
        private double IFRCalcular(int pintNumPeriodos, double pdblMediaAltaAnterior, double pdblMediaBaixaAnterior, decimal pdecDiferenca, ref double pdblMediaAltaAtualRet, ref double pdblMediaBaixaAtualRet)
        {

            double dblIFR;

            //Mup = ((Mup anterior)*(pd-1)+M) / pd;
            pdblMediaAltaAtualRet = (pdblMediaAltaAnterior * (pintNumPeriodos - 1) + Convert.ToDouble((pdecDiferenca > 0 ? pdecDiferenca : 0))) / pintNumPeriodos;

            //Mdown = ((Mdown anterior)*(pd-1)+M) / pd;
            pdblMediaBaixaAtualRet = (pdblMediaBaixaAnterior * (pintNumPeriodos - 1) + Convert.ToDouble((pdecDiferenca < 0 ? Math.Abs(pdecDiferenca) : 0))) / pintNumPeriodos;

            //calcula o INDICE DE FORÇA RELATIVA
            //IFR = 100 - (100 / (1 + U / D))
            //U = média das cotações dos últimos N dias em que a cotação subiu
            //D = média das cotações dos últimos N dias em que a cotação desceu 


            if (pdblMediaBaixaAtualRet != 0)
            {
                dblIFR = 100 - (100 / (1 + pdblMediaAltaAtualRet / pdblMediaBaixaAtualRet));
            }
            else
            {
                //se a média de baixa está zerada, ou seja, não tem cotação em baixa,
                //o IFR é 100.
                dblIFR = 100;
            }

            return dblIFR;

        }

        /// <summary>
        /// Calcula o IFR de 14 períodos para uma diferença recebida por parâmetro em relação 
        /// ao último IFR calculado
        /// </summary>
        /// <param name="pstrCodigo"></param>
        /// <param name="pstrTabela"></param>
        /// <param name="pdecDiferenca"></param>
        /// <returns></returns>
        /// <remarks></remarks>

        public double IFRSimuladoCalcular(string pstrCodigo, string pstrTabela, decimal pdecDiferenca)
        {

            RS objRS = new RS(_conexao);

            CalculadorData objCalculadorData = new CalculadorData(this._conexao);

            DateTime dtmDataMaxima = objCalculadorData.CotacaoDataMaximaConsultar(pstrCodigo, pstrTabela);

            string strTabelaIFR = pstrTabela.ToUpper() == "COTACAO" ? "IFR_DIARIO" : "IFR_SEMANAL";

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" select MediaBaixa, MediaAlta " + " from " + strTabelaIFR + " where Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + funcoesBd.CampoDateFormatar(dtmDataMaxima) + " and NumPeriodos = 14 ");

            double pdblMediaBaixaAtualRet = 0.0;
            double pdblMediaAltaAtualRet = 0.0;
            double dblIFR = IFRCalcular(14, Convert.ToDouble(objRS.Field("MediaAlta")), Convert.ToDouble(objRS.Field("MediaBaixa")), pdecDiferenca, ref pdblMediaAltaAtualRet, ref pdblMediaBaixaAtualRet);

            objRS.Fechar();

            return dblIFR;

        }

        /// <summary>
        /// Calcula o IFR a partir de uma data base até a cotação mais existente
        /// </summary>
        /// <param name="pstrCodigo">Código do ativo</param>
        /// <param name="pdtmDataBase">data inicial utilizada no cálculo</param>
        /// <param name="pintNumPeriodos">número de períodos utilizado no cálculo do IFR</param>
        /// <param name="pstrTabela">
        /// Cotacao
        /// Cotacao_Semanal
        /// </param>
        /// <param name="pdtmCotacaoAnteriorData"></param>
        /// <returns>
        /// RETORNOOK = OPERAÇÃO REALIZADA COM SUCESSO
        /// RETORNOERROINESPERADO = ALGUM ERRO DE BANCO DE DADOS OU PROGRAMAÇÃO
        /// RETORNOERRO2 = NÃO EXISTE UM NÚMERO SUFICIENTES DE PERÍODO PARA FAZER O CÁLCULO.
        /// </returns>
        /// <remarks></remarks>
        private cEnum.enumRetorno IFRRetroativoUnitCalcular(string pstrCodigo, DateTime pdtmDataBase, int pintNumPeriodos, string pstrTabela, DateTime pdtmCotacaoAnteriorData)
        {
            Conexao objConnAux = new Conexao();

            CarregadorSplit objCarregadorSplit = new CarregadorSplit(objConnAux);

            Command objCommand = new Command(objConnAux);

            RS objRS = new RS(objConnAux);
            RSList objRSSplit = null;

            double dblIFR;
            double dblMediaAltaAnterior = 0;
            double dblMediaBaixaAnterior = 0;

            double dblMediaAlta = 0;
            double dblMediaBaixa = 0;

            DateTime dtmDataInicial = default(DateTime);
            DateTime dtmDataFinal = default(DateTime);

            bool blnPeriodoCalcular = false;

            //tabela onde devem ser buscados / armazenados os dados do IFR
            pstrTabela = pstrTabela.ToUpper();

            string strTabelaIfr = pstrTabela == "COTACAO" ? "IFR_DIARIO" : "IFR_SEMANAL";

            //**********************inicia transação
            objCommand.BeginTrans();

            DateTime dtmCotacaoAnteriorData = pdtmCotacaoAnteriorData;

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            if (dtmCotacaoAnteriorData != Constantes.DataInvalida)
            {
                //se tem busca as médias do IFR na data
                objRS.ExecuteQuery(" select MediaAlta, MediaBaixa " + " from " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData) + " AND NumPeriodos = " + pintNumPeriodos.ToString());

                //verifica se os campos estão preenchidos e estão com valor maior do que zero.

                if (Convert.ToDouble(objRS.Field("MediaAlta", 0)) > 0 && Convert.ToDouble(objRS.Field("MediaBaixa", 0)) > 0)
                {
                    dblMediaAltaAnterior = Convert.ToDouble(objRS.Field("MediaAlta"));
                    dblMediaBaixaAnterior = Convert.ToDouble(objRS.Field("MediaBaixa"));
                }
                else
                {
                    //se tem cotação mas não tem as médias calculadas, tem que calcular o período inicial.
                    blnPeriodoCalcular = true;
                }

                objRS.Fechar();


            }
            else
            {
                //se não encontrou cotação tem que calcular o período inicial
                blnPeriodoCalcular = true;

            }


            if (blnPeriodoCalcular)
            {
                //verifica se existe o número de períodos necessários para fazer pelo menos um cálculo e retorna o
                //periodo para calcular o IFR inicial.
                //Para calcular o periodo inicial não pode considerar o primeiro dia, pois o primeiro dia não tem a diferença.

                if (this._cotacaoData.NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, false, ref dtmDataInicial, ref dtmDataFinal, pstrTabela, -1, objConnAux))
                {
                    //calcula o IFR inicial no período retornado pela função
                    dblIFR = Convert.ToDouble(IFRCalcular(pstrCodigo, dtmDataInicial, dtmDataFinal, pintNumPeriodos, pstrTabela, out dblMediaAltaAnterior, out dblMediaBaixaAnterior, objConnAux));

                    //tem que excluir os registros caso já existam
                    objCommand.Execute(" DELETE " + " FROM " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataFinal) + " and NumPeriodos = " + pintNumPeriodos.ToString());

                    //atualiza o IFR na tabela 
                    IFRAtualizar(pstrCodigo, dtmDataFinal, pintNumPeriodos, strTabelaIfr, dblIFR, dblMediaAltaAnterior, dblMediaBaixaAnterior, objConnAux);

                    //neste caso a data base tem que ser um dia posterior à data final da data calculada
                    //pdtmDataBase = DateAdd(DateInterval.Day, 1, dtmDataFinal)
                    var objCalculadorData = new CalculadorData(objConnAux);
                    pdtmDataBase = objCalculadorData.CalcularDataProximoPeriodo(pstrCodigo, dtmDataFinal, pstrTabela);


                }
                else
                {

                    //se não encontrou um intervalo de datas que também tenha o mesmo número de periodos 
                    //sai da função retornando o erro.
                    //Antes de sair tem que fazer rollback para não deixar a transação aberta.
                    objCommand.RollBackTrans();

                    return cEnum.enumRetorno.RetornoErro2;
                }

            }

            if (pdtmDataBase != Constantes.DataInvalida)
            {

                if (!blnPeriodoCalcular)
                {
                    //tem que excluir os registros caso já existam e não tenha sido necessário calcular o período.
                    //se o período foi calculado, os registros já foram excluidos
                    objCommand.Execute(" DELETE " + " FROM " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " and NumPeriodos = " + pintNumPeriodos.ToString());

                }

                //busca todos os splits a partir da data base em ordem ascendente
                objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataBase, "A", ref objRSSplit, Constantes.DataInvalida);

                //busca todas as cotações a partir da data base. Busca a diferença, pois o IFR é calculado em cima deste valor.

                string strQuery = " select Data, Diferenca";

                if (pstrTabela == "COTACAO_SEMANAL")
                {
                    //SE É COTAÇÃO SEMANAL TEM QUE BUSCAR A DATA FINAL PARA USAR NO CÁLCULO DOS SPLITS
                    strQuery = strQuery + ", DataFinal";

                }

                strQuery = strQuery + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " order by Data ";

                objRS.ExecuteQuery(strQuery);

                var cotacoes = new List<CotacaoDiferencaDto>();

                while (!objRS.Eof)
                {
                    var cotacaoDiferencaDto = new CotacaoDiferencaDto
                    {
                        DataInicial = Convert.ToDateTime(objRS.Field("Data")),
                        Diferenca = Convert.ToDecimal(objRS.Field("Diferenca"))
                    };
                    if (pstrTabela == "COTACAO_SEMANAL")
                    {
                        cotacaoDiferencaDto.DataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));
                    }
                    cotacoes.Add(cotacaoDiferencaDto);

                    objRS.MoveNext();
                }
                objRS.Fechar();
                //loop para calcular a média exponencial em todas as datas subsequentes

                foreach (var cotacaoDiferencaDto in cotacoes)
                {

                    if (!objRSSplit.Eof)
                    {
                        bool blnContinuarLoop;


                        if (pstrTabela == "COTACAO")
                        {
                            //********TRATAMENTO PARA A COTAÇÃO DIÁRIA

                            blnContinuarLoop = (cotacaoDiferencaDto.DataInicial == Convert.ToDateTime(objRSSplit.Field("Data")));


                            while (blnContinuarLoop)
                            {
                                //se a data do split é a mesma data do cálculo da média

                                //multiplica a média anterior pelo split
                                dblMediaAltaAnterior = dblMediaAltaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));
                                dblMediaBaixaAnterior = dblMediaBaixaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));

                                //passa para o próximo registro
                                objRSSplit.MoveNext();

                                if (objRSSplit.Eof)
                                {
                                    blnContinuarLoop = false;
                                }
                                else
                                {

                                    if (cotacaoDiferencaDto.DataInicial != Convert.ToDateTime(objRSSplit.Field("Data")))
                                    {
                                        blnContinuarLoop = false;

                                    }

                                }

                            }

                        }
                        else
                        {
                            //para aplicar o split na cotação semanal, a data do split tem que estar entre 
                            //o primeiro e ó último dia da semana.

                            blnContinuarLoop = (Convert.ToDateTime(objRSSplit.Field("Data")) >= cotacaoDiferencaDto.DataInicial && Convert.ToDateTime(objRSSplit.Field("Data")) <= cotacaoDiferencaDto.DataFinal);

                            while (blnContinuarLoop)
                            {
                                //multiplica a média anterior pelo split
                                dblMediaAltaAnterior = dblMediaAltaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));
                                dblMediaBaixaAnterior = dblMediaBaixaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));

                                //passa para o próximo registro
                                objRSSplit.MoveNext();

                                if (objRSSplit.Eof)
                                {
                                    blnContinuarLoop = false;

                                }
                                else
                                {
                                    blnContinuarLoop = (Convert.ToDateTime(objRSSplit.Field("Data")) >= cotacaoDiferencaDto.DataInicial && Convert.ToDateTime(objRSSplit.Field("Data")) <= cotacaoDiferencaDto.DataFinal);

                                }

                            }

                        }

                    }

                    //calcula o IFR
                    dblIFR = IFRCalcular(pintNumPeriodos, dblMediaAltaAnterior, dblMediaBaixaAnterior, cotacaoDiferencaDto.Diferenca, ref dblMediaAlta, ref dblMediaBaixa);

                    //atualiza na tabela o IFR e as médias dos períodos de alta e baixa
                    IFRAtualizar(pstrCodigo, cotacaoDiferencaDto.DataInicial, pintNumPeriodos, strTabelaIfr, dblIFR, dblMediaAlta, dblMediaBaixa, objConnAux);

                    //atribui a média calculada como média anterior para a próxima iteração;
                    dblMediaAltaAnterior = dblMediaAlta;
                    dblMediaBaixaAnterior = dblMediaBaixa;

                }


            }
            // pdtmDataBase <> DataInvalida Then

            //******************ENCERRA TRANSAÇÃO
            objCommand.CommitTrans();

            //retorna de acordo com o status da transação.
            cEnum.enumRetorno functionReturnValue = objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

            objConnAux.FecharConexao();

            return functionReturnValue;

        }

        /// <summary>
        /// Calcula o IFR para todos os ativos a partir da data inicial
        /// </summary>
        /// <param name="pcolPeriodos">Collection contendo o número de períodos para os quais deve ser feito o cálculo</param>
        /// <param name="pPeriodicidade"></param>
        /// <param name="pdtmDataInicial"></param>
        /// <param name="ativos"></param>
        /// <returns>
        /// TRUE - Cálculo correto para todos os ativos
        /// FALSE - Cálculo errado para pelo menos um dos ativos
        /// </returns>
        /// <remarks></remarks>
        public bool IFRGeralCalcular(IList<int> pcolPeriodos, cEnum.Periodicidade pPeriodicidade, DateTime pdtmDataInicial, ICollection<string> ativos)
        {
            bool functionReturnValue = false;

            //**********PARA BUSCAR OS ATIVOS NÃO PODE USAR A MESMA CONEXÃO DA TRANSAÇÃO,
            //**********POIS SE A TRANSAÇÃO FIZER ROLLBACK PARA UM ATIVO O RECORDSET NÃO IRÁ FUNCIONAR MAIS.
            RS objRSAtivo = new RS();

            //Dim objRSCotacao As cRS = New cRS(objConexao)

            bool blnRetorno = true;

            string strWhere = String.Empty;

            string strLog = "";

            //indica o nome da tabela de cotações, de acordo com a duração do período das cotações
            string strTabela = null;

            switch (pPeriodicidade)
            {
                case cEnum.Periodicidade.Diario:
                    strTabela = "Cotacao";
                    break;
                case cEnum.Periodicidade.Semanal:
                    strTabela = "Cotacao_Semanal";
                    break;
            }

            //busca todos os ativos do período e a menor data para ser utilizada como data base.
            string strQuery = " select Codigo, min(Data) as DataInicial " + " from " + strTabela;

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            if (pdtmDataInicial != Constantes.DataInvalida)
            {
                if (!string.IsNullOrEmpty(strWhere))
                    strWhere = strWhere + " and ";


                if (pPeriodicidade == cEnum.Periodicidade.Diario)
                {
                    //se passou uma data inicial busca as cotações a partir de uma data.
                    strWhere = strWhere + " Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial);


                }
                else if (pPeriodicidade == cEnum.Periodicidade.Semanal)
                {
                    //se é uma cotação semanal, a data recebida por parâmetro tem que estar entre 
                    //a data inicial e a data final da semana,
                    //ou então tem que estar na próxima semana, caso a data informada seja uma data de final de 
                    //semana ou feriado que esteja ligado com final de semana 
                    strWhere = strWhere + " ((Data <= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + ")" + " or Data > " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + ")";

                }

            }


            if (ativos.Count > 0)
            {
                if (!string.IsNullOrEmpty(strWhere))
                    strWhere = strWhere + " And ";

                strWhere += "Codigo IN (" + string.Join(", ", funcoesBd.GerarParametrosParaConditionIn(ativos)) + ")";

            }

            if (!string.IsNullOrEmpty(strWhere))
            {
                strQuery = strQuery + " where " + strWhere;
            }

            strQuery = strQuery + " group by Codigo ";

            objRSAtivo.ExecuteQuery(strQuery);

            var cotacaoData = new List<CotacaoDataDto>();

            while ((!objRSAtivo.Eof))
            {
                cotacaoData.Add(new CotacaoDataDto
                {
                    Codigo = (string)objRSAtivo.Field("Codigo"),
                    Data = Convert.ToDateTime(objRSAtivo.Field("DataInicial"))
                });
                objRSAtivo.MoveNext();
            }

            objRSAtivo.Fechar();

            foreach (var cotacaoDataDto in cotacaoData)
            {

                DateTime dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(cotacaoDataDto.Codigo, cotacaoDataDto.Data, strTabela);

                foreach (int intNumPeriodos in pcolPeriodos)
                {
                    cEnum.enumRetorno intRetorno = IFRRetroativoUnitCalcular(cotacaoDataDto.Codigo, cotacaoDataDto.Data, intNumPeriodos, strTabela, dtmCotacaoAnteriorData);

                    if (intRetorno != cEnum.enumRetorno.RetornoOK)
                    {
                        blnRetorno = false;

                        if (!string.IsNullOrEmpty(strLog))
                        {
                            //coloca um ENTER PARA QUEBRAR A LINHA
                            strLog = strLog + Environment.NewLine;
                        }
                        strLog = strLog + " Código = " + cotacaoDataDto.Codigo + " - Período: " + intNumPeriodos + " - Data Inicial: " + cotacaoDataDto.Data;
                    }
                }
            }


            if (!string.IsNullOrEmpty(strLog))
            {
                string strArquivoNome = null;

                if (pPeriodicidade == cEnum.Periodicidade.Diario)
                {
                    strArquivoNome = "Log_IFR_Diario.txt";
                }
                else if (pPeriodicidade == cEnum.Periodicidade.Semanal)
                {
                    strArquivoNome = "Log_IFR_Semanal.txt";
                }

                var fileService = new FileService();
                fileService.Save(BuscarConfiguracao.ObtemCaminhoPadrao() + strArquivoNome, strLog);

            }


            if (objRSAtivo.QueryStatus)
            {
                functionReturnValue = blnRetorno;
            }

            return functionReturnValue;

        }

        /// <summary>
        /// Soma o valor absoluto das diferenças de cotações em um período.
        /// </summary>
        /// <param name="pstrCodigo"></param>
        /// <param name="pdtmDataInicial"></param>
        /// <param name="pdtmDataFinal"></param>
        /// <param name="pstrSinal">
        /// Indica se deve considerar somente cotações positivas ou somente cotações negativas
        /// P - POSITIVA
        /// N - NEGATIVA
        /// </param>
        /// <param name="pstrTabela">
        /// Cotacao
        /// Cotacao_Semanal
        /// </param>
        /// <returns>Soma da diferença das cotações do período, conforme o sinal recebido por parâmetro</returns>
        /// <remarks></remarks>
        private decimal CotacaoPeriodoDiferencaSomar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrSinal, string pstrTabela)
        {
            RS objRS = new RS(_conexao);

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            //TEM QUE SER O VALOR ABSOLUTO, POIS NO CÁLCULO OS VALORES TEM QUE SER SEMPRE POSITIVOS.
            string strQuery = " select sum(ABS(Diferenca)) as Total " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);


            if (pstrSinal == "P")
            {
                strQuery = strQuery + " and Diferenca > 0 ";


            }
            else if (pstrSinal == "N")
            {
                strQuery = strQuery + " and Diferenca < 0 ";

            }

            objRS.ExecuteQuery(strQuery);

            decimal functionReturnValue = Convert.ToDecimal(objRS.Field("Total"));

            objRS.Fechar();
            return functionReturnValue;

        }
    }
}
