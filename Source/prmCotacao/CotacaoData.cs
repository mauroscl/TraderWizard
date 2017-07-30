using System;
using DataBase;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{
    public class CotacaoData
    {
        private readonly Conexao conexao;

        public CotacaoData()
        {
            this.conexao = new Conexao();

        }

        /// <summary>
        /// Recebe uma data e busca o primeiro dia da semana para a qual esta data pertence.
        /// Se a data recebida for um dia sem cotação, como um sábado ou domingo, irá buscar a primeira data da
        /// semana anterior.
        /// Se for um feriado no meio da semana irá buscar a primeira data da mesma semana do feriado
        /// </summary>
        /// <param name="pdtmDataBase"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime CotacaoSemanalPrimeiroDiaSemanaCalcular(DateTime pdtmDataBase)
        {

            cRS objRS = new cRS(this.conexao);

            FuncoesBd funcoesBd = this.conexao.ObterFormatadorDeCampo();

            //primeiro busca a primeira data na cotação diária com data menor ou igual à data recebida por parâmetro.
            objRS.ExecuteQuery("SELECT MAX(Data) as Data " + "FROM Cotacao " + "WHERE Data <= " + funcoesBd.CampoDateFormatar(pdtmDataBase));

            DateTime dtmDataAux = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

            objRS.Fechar();


            if (dtmDataAux != Constantes.DataInvalida) {
                //se existe uma data menor ou igual à data informada

                //busca a menor data inicial de semana na qual esta cotação está contida
                objRS.ExecuteQuery("SELECT MIN(Data) as Data " + "FROM Cotacao_Semanal " + "WHERE Data <= " + funcoesBd.CampoDateFormatar(dtmDataAux) + " AND DataFinal >= " + funcoesBd.CampoDateFormatar(dtmDataAux));

                dtmDataAux = Convert.ToDateTime(objRS.Field("Data"));

                objRS.Fechar();

            }

            return dtmDataAux;

        }
    }
}