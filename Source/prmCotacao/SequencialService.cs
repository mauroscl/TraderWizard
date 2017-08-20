using System;
using DataBase;

namespace Cotacao
{
    public class SequencialService
    {
        private readonly Conexao _conexao;

        public SequencialService()
        {
            this._conexao = new Conexao();
        }

        //~SequencialService()
        //{
        //    this._conexao.FecharConexao();
        //}

        /// <summary>
        /// Preenche o campo SEQUENCIAL para um determinado ativo em uma determinada tabela
        /// </summary>
        /// <param name="pstrCodigo">Código do ativo</param>
        /// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
        /// <returns>status da transação</returns>
        /// <remarks></remarks>
        private bool SequencialAtivoPreencher(string pstrCodigo, string pstrTabela)
        {

            Command objCommand = new Command(_conexao);
            RS objRS = new RS(_conexao);
            objCommand.BeginTrans();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //BUSCA AS COTAÇÕES ORDENADAS POR DATA
            objRS.ExecuteQuery(" SELECT Data " + " FROM " + pstrTabela + " WHERE Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + " ORDER BY Data ");

            //INICIALIZA O SEQUENCIAL EM 1
            long lngSequencial = 1;

            //PARA CADA DATA ONDE HÁ COTACAO  
            while (!objRS.Eof)
            {
                //ATUALIZA O SEQUENCIAL
                objCommand.Execute(" UPDATE " + pstrTabela + " SET " + "Sequencial = " + lngSequencial.ToString() + " WHERE Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + " AND Data = " + funcoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("Data"))));

                //INCREMENTA PARA A PRÓXIMA ITERAÇÃO
                lngSequencial = lngSequencial + 1;

                objRS.MoveNext();

            }

            objRS.Fechar();

            objCommand.CommitTrans();

            return objCommand.TransStatus;

        }

        private string SequencialQueryDivergenciaGerar(string pstrTabelaCotacao)
        {

            return " SELECT Codigo " + Environment.NewLine + " FROM Ativo A " + Environment.NewLine + " WHERE " + Environment.NewLine + "(" + Environment.NewLine + '\t' + "SELECT Max(sequencial) " + Environment.NewLine + '\t' + " FROM " + pstrTabelaCotacao + " C " + Environment.NewLine + '\t' + " WHERE A.Codigo = C.Codigo " + Environment.NewLine + ") <> " + "(" + Environment.NewLine + '\t' + " SELECT Count(1) " + Environment.NewLine + '\t' + " FROM " + pstrTabelaCotacao + " C " + Environment.NewLine + '\t' + " WHERE A.Codigo = C.Codigo" + Environment.NewLine + ")";

        }

        private bool SequencialPeriodicidadePreencher(string pstrPeriodicidade)
        {

            RS objRS = new RS(_conexao);

            bool blnOK = true;

            string strTabelaCotacao;

            switch (pstrPeriodicidade)
            {
                case "DIARIO":
                    strTabelaCotacao = "Cotacao";
                    break;
                case "SEMANAL":
                    strTabelaCotacao = "Cotacao_Semanal";
                    break;
                default:
                    strTabelaCotacao = String.Empty;
                    break;
            }

            string strQuery = SequencialQueryDivergenciaGerar(strTabelaCotacao);

            //BUSCA TODOS OS ATIVOS
            objRS.ExecuteQuery(strQuery);

            //PARA CADA ATIVO...

            while ((!objRS.Eof) && blnOK)
            {
                //CHAMA FUNÇÃO PARA ATUALIZAR SEQUENCIAL NAS COTAÇÕES DIÁRIAS

                if (!SequencialAtivoPreencher((string)objRS.Field("Codigo"), strTabelaCotacao))
                {
                    blnOK = false;

                }

                objRS.MoveNext();

            }

            objRS.Fechar();

            return blnOK;

        }

        /// <summary>
        /// Preenche o campo sequencial para todos os ativos nas tabelas COTACAO e COTACAO_SEMANAL.
        /// Serão considerados apenas os ativos cadastrados na tabeal ATIVO.
        /// </summary>
        /// <returns>STATUS DA TRANSAÇÃO</returns>
        /// <remarks></remarks>
        public bool SequencialPreencher()
        {
            bool blnOk = SequencialPeriodicidadePreencher("DIARIO");

            if (blnOk)
            {
                blnOk = SequencialPeriodicidadePreencher("SEMANAL");

            }

            return blnOk;

        }

        /// <summary>
        /// Calcula o valor máximo da coluna sequencial para um determinado ativo em uma determinada periodicidade 
        /// (DIÁRIO ou SEMANAL).
        /// </summary>
        /// <param name="pstrCodigo">Código do ativo</param>
        /// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
        /// <param name="pobjConexao"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private long AtivoSequencialMaximoBuscar(string pstrCodigo, string pstrTabela, Conexao pobjConexao = null)
        {

            RS objRs = pobjConexao == null ? new RS(_conexao) : new RS(pobjConexao);
            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //busca o maior sequencial utilizado
            objRs.ExecuteQuery(" SELECT MAX(Sequencial) AS Sequencial " + " FROM " + pstrTabela + " WHERE Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo));

            long functionReturnValue = Convert.ToInt64(objRs.Field("Sequencial", 0));

            objRs.Fechar();
            return functionReturnValue;

        }

        /// <summary>
        /// Calcula o novo sequencial para um determinado ativo, de acordo com o último sequencial utilizado
        /// </summary>
        /// <param name="pstrCodigo">Código do ativo</param>
        /// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
        /// <param name="pobjConexao">Conexão para fazer a consulta. Obrigado a receber a mesma conexão
        /// do objCommand que controla a transação da função principal que chamada esta,
        ///  pois muitas vezes é feito delete dos dados e para não deixarmos espaços em branco 
        /// no sequencial precisamos saber que os dados já foram excluídos e podemos utilizar 
        /// um número que foi utilizado antes, mas foi excluído. Conseguimos fazer isso apenas utilizando 
        /// a mesma conexão</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public long SequencialCalcular(string pstrCodigo, string pstrTabela, Conexao pobjConexao)
        {

            //incrementa 1 no último sequencial utiliazado.
            //retorno erro é 0, caso o ativo ainda não tenha registro inserido (primeiro dia de negociação)
            return AtivoSequencialMaximoBuscar(pstrCodigo, pstrTabela, pobjConexao) + 1;

        }

    }
}
