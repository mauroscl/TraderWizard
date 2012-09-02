using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using prjModelo.Carregadores;
using DataBase;
using frwInterface;
using prjModelo;
namespace prmCotacao
{

	public class cCotacaoAjustada
	{

		private readonly cConexao objConexao;

		private readonly string strCodigoAtivo;

		public cCotacaoAjustada(cConexao pobjConexao, string pstrCaminhoPadrao, string pstrCodigoAtivo)
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

				string strSQL = null;

				//Multiplicador gerado pela acumulação de splits
				double dblMultiplicador = 1;

				objCommand.BeginTrans();

				cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConexao);

				//busca os splits em ordem descrescente
				objCarregadorSplit.SplitConsultar(strCodigoAtivo, new DateTime(2007, 1, 1), "D", ref objRSSplit,cConst.DataInvalida);

				//busca todas as cotações da tabela "COTACAO" em ordem decrescente
				strSQL = "SELECT Data, ValorAbertura, ValorFechamento, ValorMaximo, ValorMinimo, Diferenca, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial " + Environment.NewLine;
				strSQL = strSQL + "FROM Cotacao " + Environment.NewLine;
				strSQL = strSQL + "WHERE Codigo = " + FuncoesBD.CampoStringFormatar(strCodigoAtivo);
				strSQL = strSQL + " ORDER BY Data DESC";

				objRSCotacao.ExecuteQuery(strSQL);


				while ((!objRSCotacao.EOF) & objCommand.TransStatus) {
					strSQL = "INSERT INTO Cotacao_Ajustada " + Environment.NewLine;
					strSQL = strSQL + "(Codigo, Data, Sequencial, ValorAbertura, ValorFechamento, ValorMaximo, ValorMinimo, Diferenca " + Environment.NewLine;
					strSQL = strSQL + ", Oscilacao, Negocios_Total, Titulos_Total, Valor_Total) " + Environment.NewLine;
					strSQL = strSQL + "VALUES " + Environment.NewLine;
					strSQL = strSQL + "(" + FuncoesBD.CampoStringFormatar(strCodigoAtivo);
					strSQL = strSQL + "," + FuncoesBD.CampoDateFormatar(Convert.ToDateTime(objRSCotacao.Field("Data")));
					strSQL = strSQL + "," + objRSCotacao.Field("Sequencial");
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorAbertura")) * (decimal) dblMultiplicador);
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorFechamento")) * (decimal) dblMultiplicador);
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorMaximo")) * (decimal) dblMultiplicador);
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("ValorMinimo")) * (decimal) dblMultiplicador);
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Diferenca")));
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Oscilacao")));
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Negocios_Total")));
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Titulos_Total")) / (decimal) dblMultiplicador);
					strSQL = strSQL + "," + FuncoesBD.CampoDecimalFormatar(Convert.ToDecimal(objRSCotacao.Field("Valor_Total")));
					strSQL = strSQL + ")";

					objCommand.Execute(strSQL);

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
