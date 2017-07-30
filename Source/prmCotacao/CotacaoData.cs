using System;
using DataBase;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{
    public class CotacaoData
    {
        private readonly Conexao _conexao;

        public CotacaoData()
        {
            this._conexao = new Conexao();

        }

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

            objRs.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

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
                               " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine +
                               " AND Data = " + funcoesBd.CampoDateFormatar(pdtmPrimeiroDiaSemana));

            DateTime dataDoUltimoDiaDaSemana = (DateTime)objRS.Field("DataFinal");

            objRS.Fechar();

            return dataDoUltimoDiaDaSemana;

        }

    }
}