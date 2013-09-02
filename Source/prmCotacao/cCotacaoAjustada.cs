using System;
using prjModelo.Carregadores;
using DataBase;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{

	public class cCotacaoAjustada
	{

		private readonly cConexao objConexao;

		private readonly string strCodigoAtivo;

		public cCotacaoAjustada(cConexao pobjConexao, string pstrCodigoAtivo)
		{
			objConexao = pobjConexao;
			strCodigoAtivo = pstrCodigoAtivo;

		}

		/// <summary>
		/// Preenche a tabela Cotacao_Ajustada a partir da tabela Cotacao fazendo as conversões de acordo com os desdobramentos e os splits e agrupamentos.
		/// </summary>
		/// <returns>
		/// RetornoOK = operação realizada com sucesso.
		/// RetornoErroInesperado = erro inesperado que fez a execução cair para o tratamento de erros.
		/// </returns>
		/// <remarks></remarks>
		public cEnum.enumRetorno Carregar()
		{


			try {
				cCommand objCommand = new cCommand(objConexao);
				cRSList objRSSplit = null;
				cRS objRSCotacao = new cRS(objConexao);

			    //Multiplicador gerado pela acumulação de splits
				double dblMultiplicador = 1;

				objCommand.BeginTrans();

				var objCarregadorSplit = new cCarregadorSplit(objConexao);

				//busca os splits em ordem descrescente
				objCarregadorSplit.SplitConsultar(strCodigoAtivo, new DateTime(2007, 1, 1), "D", ref objRSSplit,cConst.DataInvalida);

                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

				//busca todas as cotações da tabela "COTACAO" em ordem decrescente
				string strSql = "SELECT Data, ValorAbertura, ValorFechamento, ValorMaximo, ValorMinimo, Diferenca, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial " + Environment.NewLine;
				strSql = strSql + "FROM Cotacao " + Environment.NewLine;
				strSql = strSql + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigoAtivo);
				strSql = strSql + " ORDER BY Data DESC";

				objRSCotacao.ExecuteQuery(strSql);

				while ((!objRSCotacao.EOF) && objCommand.TransStatus) {
					strSql = "INSERT INTO Cotacao_Ajustada " + Environment.NewLine;
					strSql = strSql + "(Codigo, Data, Sequencial, ValorAbertura, ValorFechamento, ValorMaximo, ValorMinimo, Diferenca " + Environment.NewLine;
					strSql = strSql + ", Oscilacao, Negocios_Total, Titulos_Total, Valor_Total) " + Environment.NewLine;
					strSql = strSql + "VALUES " + Environment.NewLine;
					strSql = strSql + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtivo);
					strSql = strSql + "," + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRSCotacao.Field("Data")));
					strSql = strSql + "," + objRSCotacao.Field("Sequencial");
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorAbertura")) * (decimal) dblMultiplicador);
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorFechamento")) * (decimal) dblMultiplicador);
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorMaximo")) * (decimal) dblMultiplicador);
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorMinimo")) * (decimal) dblMultiplicador);
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Diferenca")));
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Oscilacao")));
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Negocios_Total")));
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Titulos_Total")) / (decimal) dblMultiplicador);
					strSql = strSql + "," + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Valor_Total")));
					strSql = strSql + ")";

					objCommand.Execute(strSql);

					//verifica se o RS de split ainda possui registros não percorridos

					if (!objRSSplit.EOF) {
						//compara a data do split e a data da cotação

						if (Convert.ToDateTime(objRSCotacao.Field("Data")) == Convert.ToDateTime(objRSSplit.Field("Data"))) {
							//Se as datas são as mesmas recalcula o multiplicador, multiplicando pela quantidade anterior e dividindo pela quantidade posterior.
							dblMultiplicador = dblMultiplicador * Convert.ToDouble(objRSSplit.Field("Razao"));

							objRSSplit.MoveNext();

						}

					}

					objRSCotacao.MoveNext();

				}

				objRSCotacao.Fechar();

				objCommand.CommitTrans();

				Console.WriteLine("Operação realizada com sucesso.");

				return cEnum.enumRetorno.RetornoOK;


			} catch (Exception ex) {
				Console.WriteLine(ex.Message);

				return cEnum.enumRetorno.RetornoErroInesperado;

			}

		}



	}
}
