using System;
using DataBase;
using TraderWizard.Enumeracoes;

namespace Cotacao
{
    public class CotacaoData
    {
        private readonly Conexao _conexao;
        private readonly FuncoesBd _funcoesBd;

        public CotacaoData()
        {
            this._conexao = new Conexao();
            this._funcoesBd = this._conexao.ObterFormatadorDeCampo();

        }

        //~CotacaoData()
        //{
        //    this._conexao.FecharConexao();
        //}

        /// <summary>
        /// Recebe uma data e busca o primeiro dia da semana para a qual esta data pertence.
        /// Se a data recebida for um dia sem cota��o, como um s�bado ou domingo, ir� buscar a primeira data da
        /// semana anterior.
        /// Se for um feriado no meio da semana ir� buscar a primeira data da mesma semana do feriado
        /// </summary>
        /// <param name="pdtmDataBase"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime CotacaoSemanalPrimeiroDiaSemanaCalcular(DateTime pdtmDataBase)
        {

            cRS objRS = new cRS(this._conexao);

            FuncoesBd funcoesBd = this._conexao.ObterFormatadorDeCampo();

            //primeiro busca a primeira data na cota��o di�ria com data menor ou igual � data recebida por par�metro.
            objRS.ExecuteQuery("SELECT MAX(Data) as Data " + "FROM Cotacao " + "WHERE Data <= " + funcoesBd.CampoDateFormatar(pdtmDataBase));

            DateTime dtmDataAux = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

            objRS.Fechar();


            if (dtmDataAux != Constantes.DataInvalida) {
                //se existe uma data menor ou igual � data informada

                //busca a menor data inicial de semana na qual esta cota��o est� contida
                objRS.ExecuteQuery("SELECT MIN(Data) as Data " + "FROM Cotacao_Semanal " + "WHERE Data <= " + funcoesBd.CampoDateFormatar(dtmDataAux) + " AND DataFinal >= " + funcoesBd.CampoDateFormatar(dtmDataAux));

                dtmDataAux = Convert.ToDateTime(objRS.Field("Data"));

                objRS.Fechar();

            }

            return dtmDataAux;

        }

        /// <summary>
        /// Calcula, para um ativo, qual � a primeira semana que este ativo tem cota��o a semana completa
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pobjConexao"></param>
        /// <returns>a data da segunda-feira da primeira semana em que existe uma cota��o completa</returns>
        /// <remarks></remarks>
        public DateTime PrimeiraSemanaDataCalcular(string pstrCodigo, Conexao pobjConexao = null)
        {
            cRS objRs = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            objRs.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + _funcoesBd.CampoStringFormatar(pstrCodigo));

            DateTime dtmDataAux = Convert.ToDateTime(objRs.Field("Data"));

            objRs.Fechar();

            return PrimeiraSemanaDataCalcular(dtmDataAux);

        }

        /// <summary>
        /// Calcula, para um ativo, qual � a primeira semana que este ativo tem cota��o a semana completa
        /// </summary>
        /// <param name="pdtmDataBase">Data recebida para iniciar os c�lculos. 
        /// A primeira semana de c�lculo tem que compreender
        /// esta data ou ser uma semana seguinte a esta data</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime PrimeiraSemanaDataCalcular(DateTime pdtmDataBase)
        {

            //se a data � uma segunda-feira retorna a pr�pria data
            DateTime dtmDataAux = pdtmDataBase;

            DayOfWeek diaDaSemana = dtmDataAux.DayOfWeek;

            if (diaDaSemana == DayOfWeek.Monday)
            {
                return dtmDataAux;
            }

            double dblIntervalo;
            if (diaDaSemana == DayOfWeek.Saturday || diaDaSemana == DayOfWeek.Sunday)
            {
                //se o dia da semana � s�bado ou domingo tem que buscar a pr�xima segunda-feira,
                //ent�o o incremento tem que ser positivo para a data ir para uma data posterior
                dblIntervalo = 1;


            }
            else
            {
                //se a data est� entre ter�a e sexta, o incremento tem que ser negativo para a data
                //voltar at� a segunda-feira anterior
                dblIntervalo = -1;

            }


            while (diaDaSemana != DayOfWeek.Monday)
            {
                dtmDataAux = dtmDataAux.AddDays(dblIntervalo);

                //tem que descontar 1 para bater com o enum.
                diaDaSemana = dtmDataAux.DayOfWeek;

            }

            return dtmDataAux;

        }

        /// <summary>
        /// Consulta o primeiro dia de cota��o  de uma semana para um determinado ativo.
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pdtmDataBase">Data para o qual deve ser buscada o primeiro dia da semana</param>
        /// <returns>A data do primeiro dia da semana</returns>
        /// <remarks></remarks>
        public DateTime AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(string pstrCodigo, DateTime pdtmDataBase)
        {

            if (pdtmDataBase == Constantes.DataInvalida)
            {
                return Constantes.DataInvalida;
            }

            cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery(" select max(Data) as Data " + Environment.NewLine + " from Cotacao_Semanal" +
                               Environment.NewLine + " where Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) +
                               Environment.NewLine + " and Data <= " + funcoesBd.CampoFormatar(pdtmDataBase));

            DateTime functionReturnValue = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

            objRS.Fechar();
            return functionReturnValue;

        }

        /// <summary>
        /// Consulta o �ltimo dia de cota��o de uma semana.
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pdtmPrimeiroDiaSemana">Primeiro dia de cota��o de uma semana</param>
        /// <returns>Data do �ltimo dia de cota��o de uma semana.</returns>
        /// <remarks></remarks>
        public DateTime AtivoCotacaoSemanalUltimoDiaSemanaCalcular(string pstrCodigo, DateTime pdtmPrimeiroDiaSemana)
        {
            cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery("SELECT DataFinal " + Environment.NewLine + " FROM Cotacao_Semanal " + Environment.NewLine +
                               " WHERE Codigo = " + _funcoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine +
                               " AND Data = " + funcoesBd.CampoDateFormatar(pdtmPrimeiroDiaSemana));

            DateTime dataDoUltimoDiaDaSemana = (DateTime)objRS.Field("DataFinal");

            objRS.Fechar();

            return dataDoUltimoDiaDaSemana;

        }

        /// <summary>
        /// Verifica se existe alguma cota��o em uma determinada data
        /// </summary>
        /// <param name="pdtmData"> Data que deve ser verificada se existe cota��o </param>
        /// <returns>
        /// True = Existe cota��o na data
        /// False = N�o existe cota��o na data
        /// </returns>
        /// <remarks></remarks>
        public bool CotacaoDataExistir(DateTime pdtmData, string pstrTabela)
        {
            cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            string strQuery = " select 1 " + " from " + pstrTabela + " where Data = " + funcoesBd.CampoDateFormatar(pdtmData);

            objRS.ExecuteQuery(strQuery);

            bool functionReturnValue = objRS.DadosExistir;

            objRS.Fechar();
            return functionReturnValue;

        }

        /// <summary>
        /// Verifica se existe alguma cota��o em uma determinada data para um determinado ativo
        /// </summary>
        /// <param name="pdtmData"> Data que deve ser verificada se existe cota��o </param>
        /// <returns>
        /// True = Existe cota��o na data
        /// False = N�o existe cota��o na data
        /// </returns>
        /// <remarks></remarks>
        public bool CotacaoDataExistir(DateTime pdtmData, string pstrTabela, string pstrCodigoAtivo)
        {
            cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            string strQuery = " select 1 " + " FROM " + pstrTabela + " WHERE Data = " + funcoesBd.CampoFormatar(pdtmData) + " AND Codigo = " + funcoesBd.CampoFormatar(pstrCodigoAtivo);

            objRS.ExecuteQuery(strQuery);

            bool functionReturnValue = objRS.DadosExistir;

            objRS.Fechar();
            return functionReturnValue;

        }

        /// <summary>
        /// Busca a data da cota��o imediatamente anterior a uma data recebida por par�metro
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pdtmDataBase">Data base utilizada para buscar a cota��o anterior</param>
        /// <param name="pstrTabela">
        /// Cotacao
        /// Cotacao_Semanal
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime AtivoCotacaoAnteriorDataConsultar(string pstrCodigo, DateTime pdtmDataBase, string pstrTabela, Conexao pobjConexao = null)
        {
            DateTime functionReturnValue;

            var objRsData = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            string strPeriodo;

            pstrTabela = pstrTabela.ToUpper();

            switch (pstrTabela)
            {
                case "COTACAO":
                    strPeriodo = "DIARIO";
                    break;
                case "COTACAO_SEMANAL":
                    strPeriodo = "SEMANAL";
                    break;
                default:
                    strPeriodo = String.Empty;
                    break;
            }

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            objRsData.ExecuteQuery("SELECT Data_Anterior" + " FROM Cotacao_Anterior" + " WHERE Codigo = " + _funcoesBd.CampoStringFormatar(pstrCodigo) + " AND Data = " + funcoesBd.CampoDateFormatar(pdtmDataBase) +
                " AND Periodo = " + _funcoesBd.CampoStringFormatar(strPeriodo));


            if (objRsData.DadosExistir)
            {
                functionReturnValue = Convert.ToDateTime(objRsData.Field("Data_Anterior"));


            }
            else
            {
                objRsData.Fechar();

                //Busca a data imediatamente anterior que tem uma cota��o para o ativo recebido por par�metro.
                objRsData.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Codigo = " + _funcoesBd.CampoStringFormatar(pstrCodigo) + " and Data < " + funcoesBd.CampoDateFormatar(pdtmDataBase));

                functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

            }

            objRsData.Fechar();
            return functionReturnValue;

        }

        /// <summary>
        /// Para um determinado ativo retorna a data inicial e a data final em que existem os primeiros
        /// "pintNumPeriodos" com cota��o.
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pintNumPeriodos">N�mero de per�odos desejado</param>
        /// <param name="pdtmDataInicialRet">Retorna a data inicial do n�mero de per�odos desejado</param>
        /// <param name="pdtmDataFinalRet">Retorna a data final do n�mero de per�odos desejado</param>
        /// <returns>    
        /// TRUE - Foi poss�vel encontrar o n�mero de per�odos desejados
        /// FALSE - N�o foi poss�vel encontrar o n�mero de per�odos desejados 
        /// <param name="pblnPrimeiraDataConsiderar">Indica se deve ser considerada a primeira cota��o. 
        /// Em c�lculos em que � necess�rio utilizar a Oscila��o ou a Diferen�a s� pode ser utilizado a partir 
        /// do segundo per�odo, pois � s� a partir do segundo per�odo que esta informa��oexiste.
        /// TRUE - considera a primeira data
        /// FALSE - n�o considera a primeira data.
        /// </param>
        /// <param name="pstrTabela">
        /// Cotacao
        /// Cotacao_Semanal
        /// </param>
        /// <param name="pintNumPeriodoTabelaDados"></param>
        /// Quando pstrTabelaDados for uma tabela de IFR, indica qual o per�odo do IFR que deve ser utilizado
        /// para calcular a m�dia.
        /// </returns>
        /// <remarks></remarks>
        public bool NumPeriodosDataInicialCalcular(string pstrCodigo, int pintNumPeriodos, bool pblnPrimeiraDataConsiderar, ref DateTime pdtmDataInicialRet, ref DateTime pdtmDataFinalRet, string pstrTabela, int pintNumPeriodosTabelaDados = -1, Conexao pobjConexao = null)
        {
            string strQuery;

            cRS objRS = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            string strCodigoFormatado = _funcoesBd.CampoStringFormatar(pstrCodigo);

            int intNumPeriodosFinal;

            string subQuery;

            if (pblnPrimeiraDataConsiderar)
            {
                //se � para considerar a primeira cota��o, para calcular a data inicial 
                //basta pegar um min(Data) das cota��es do papel

                strQuery = " select min(Data) as DataInicial " + " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;


                if (pintNumPeriodosTabelaDados != -1)
                {
                    strQuery = strQuery + " and NumPeriodos = " + pintNumPeriodosTabelaDados;

                }

                objRS.ExecuteQuery(strQuery);

                pdtmDataInicialRet = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

                //Se � para considerar a primeira data a �ltima cota��o que deve ser buscada 
                //� a cota��o do n�mero de periodos
                intNumPeriodosFinal = pintNumPeriodos;


            }
            else
            {
                //se n�o � para considerar a primeira data, pega a segunda data.

                subQuery = " select top 2 Data " + " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;

                if (pintNumPeriodosTabelaDados != -1)
                {
                    subQuery += " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();

                }

                subQuery += " order by Data ";

                strQuery = " select max(Data) as DataInicial " + " from " + funcoesBd.FormataSubSelect(subQuery);

                objRS.ExecuteQuery(strQuery);

                pdtmDataInicialRet = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

                //Se n�o � para considerar a primeira data a �ltima cota��o que deve ser buscada 
                //� a cota��o do n�mero de periodos + 1
                intNumPeriodosFinal = pintNumPeriodos + 1;

            }

            objRS.Fechar();

            if (pdtmDataInicialRet == Constantes.DataInvalida)
            {
                //se n�o encontrou data inicial significa que n�o existem cota��es para o papel
                return false;

            }

            //para obter a data final da cota��o que cont�m o n�mero do periodos desejados tem que fazer os seguintes passos:
            //1) Ordenar as cota��es em ordem crescente
            //2) Pegar as "pintNumPeriodos" primeiras
            //3) Pegar a data m�xima entre "pintNumPeriodos" primeiras
            subQuery = " select top " + intNumPeriodosFinal + " Data " +
                       " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;


            if (pintNumPeriodosTabelaDados != -1)
            {
                subQuery += " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();

            }

            subQuery += " order by Data ";

            strQuery = " select max(Data) as DataMaxima FROM " + funcoesBd.FormataSubSelect(subQuery);

            objRS.ExecuteQuery(strQuery);

            pdtmDataFinalRet = Convert.ToDateTime(objRS.Field("DataMaxima", Constantes.DataInvalida));

            objRS.Fechar();

            //retorna resultado da fun��o que verifica se o intervalo realmente tem o n�mero de per�odos informado.
            bool functionReturnValue = IntervaloNumPeriodosVerificar(pstrCodigo, pdtmDataInicialRet, pdtmDataFinalRet, pintNumPeriodos, pstrTabela, pintNumPeriodosTabelaDados, objRS.Conexao);

            return functionReturnValue;

        }

        /// <summary>
        /// Verifica se o n�mero de per�odos em um determinado intervalo de datas em que h� cota��es � o n�mero de per�odos que se deseja
        /// </summary>
        /// <param name="pstrCodigo">C�digo do ativo</param>
        /// <param name="pdtmDataInicial">data inicial do intervalo</param>
        /// <param name="pdtmDataFinal">data final do intervalo</param>
        /// <param name="pintNumPeriodos">n�mero de per�odos do intervalo que devem ter cota��es</param>
        /// <param name="pstrTabela">
        /// Cotacao
        /// Cotacao_Semanal
        /// </param>
        /// <returns>
        /// TRUE - O n�mero de cota��es � igual ao n�mero esperado
        /// FALSE - O n�mero de cota��es � diferente do n�mero esperado.
        /// </returns>
        /// <remarks></remarks>
        public bool IntervaloNumPeriodosVerificar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, int pintNumPeriodos, string pstrTabela, int pintNumPeriodosTabelaDados = -1, Conexao pobjConexao = null)
        {
            cRS objRs = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //calcula o n�mero de per�odos em que h� cota��es para o papel no intervalo de datas recebido.
            string strQuery = " Select count(1) as Contador " + " from " + pstrTabela + " where Codigo = " + _funcoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal);

            if (pintNumPeriodosTabelaDados != -1)
            {
                strQuery = strQuery + " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();
            }

            objRs.ExecuteQuery(strQuery);

            bool functionReturnValue = Convert.ToInt32(objRs.Field("Contador", "0")) == pintNumPeriodos;

            objRs.Fechar();
            return functionReturnValue;

        }


    }
}