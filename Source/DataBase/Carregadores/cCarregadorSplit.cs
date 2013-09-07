using System;
using System.Collections.Generic;
using prjDominio.Entidades;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class cCarregadorSplit
	{


		private readonly cConexao _conexao;
		public cCarregadorSplit(cConexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		private string QuerySplitGerar(string pstrCodigo, DateTime pdtmDataInicial, string pstrOrdem, DateTime pdtmDataFinal, string pstrTipo = "")
		{

			string strQuery = string.Empty;

		    FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

		    string razao2, razaoInvertida2;

			if (pstrTipo == String.Empty || pstrTipo == "DIV" || pstrTipo == "JCP" || pstrTipo == "REND" || pstrTipo == "RCDIN") {
				//Tratamento para outros tipos 

                razao2 = FuncoesBd.ConcatenarString(FuncoesBd.ConvertParaString("Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)"), FuncoesBd.CampoStringFormatar("/"));
                razao2 = FuncoesBd.ConcatenarString(razao2, FuncoesBd.ConvertParaString("AVG(QuantidadePosterior)") ) + " AS Razao2";

                razaoInvertida2 = FuncoesBd.ConcatenarString(FuncoesBd.ConvertParaString("Avg(QuantidadePosterior)"), FuncoesBd.CampoStringFormatar("/"));
                razaoInvertida2 = FuncoesBd.ConcatenarString(razaoInvertida2,FuncoesBd.ConvertParaString("Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)")) + " As RazaoInvertida2";

			    strQuery =
			        " select Data, (Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)) / AVG(QuantidadePosterior) as Razao " + Environment.NewLine +
			        ", AVG(QuantidadePosterior) / (Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)) as RazaoInvertida " + Environment.NewLine +
                    ", " + razao2 + Environment.NewLine +
			        ", " + razaoInvertida2 + Environment.NewLine + 
                    ", " + FuncoesBd.CampoStringFormatar("PROV") + " AS Tipo " + 
                    " from Split " + Environment.NewLine + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine +
			        " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;


				if (pdtmDataFinal != Constantes.DataInvalida) {
					strQuery = strQuery + " and Data <=  " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

				}


				if (pstrTipo != String.Empty) {
					//Se é um tipo específico
					strQuery = strQuery + " AND Tipo = " + FuncoesBd.CampoStringFormatar(pstrTipo) + Environment.NewLine;


				} else {
					//senão registro pelos dois tipos: DIV, JCP.
				    strQuery = strQuery + " AND Tipo IN(" + FuncoesBd.CampoStringFormatar("DIV") + ", " +
				               FuncoesBd.CampoStringFormatar("JCP") + ", " + FuncoesBd.CampoStringFormatar("REND") + ", " +
				               FuncoesBd.CampoStringFormatar("RCDIN") + ")" + Environment.NewLine;

				}

				strQuery = strQuery + " GROUP BY Data" + Environment.NewLine;


			}


			if (pstrTipo == String.Empty || pstrTipo == "DESD" || pstrTipo == "CISAO") {
				//Tratamento para desdobramentos e cisão. Só há um desdobramento ou cisão por dia.
				//Por isso não é necessário fazer somatórios das quantidades 
				//anteriores e posteriores agrupados por data

				if (strQuery != String.Empty) {
					strQuery = strQuery + " UNION " + Environment.NewLine;

				}

                razao2 = FuncoesBd.ConcatenarString(FuncoesBd.ConvertParaString("QuantidadeAnterior"),FuncoesBd.CampoStringFormatar("/"));
			    razao2 = FuncoesBd.ConcatenarString(razao2, FuncoesBd.ConvertParaString("QuantidadePosterior")) + " As Razao2";

                razaoInvertida2 = FuncoesBd.ConcatenarString(FuncoesBd.ConvertParaString("QuantidadePosterior"),FuncoesBd.CampoStringFormatar("/"));
                razaoInvertida2 = FuncoesBd.ConcatenarString(razaoInvertida2, FuncoesBd.ConvertParaString("QuantidadeAnterior")) + " As RazaoInvertida2";

			    strQuery = strQuery + " select Data, QuantidadeAnterior / QuantidadePosterior as Razao " + Environment.NewLine +
			               ", QuantidadePosterior / QuantidadeAnterior as RazaoInvertida " + Environment.NewLine +
			               ", " + razao2 + Environment.NewLine + 
                           ", "  + razaoInvertida2 + Environment.NewLine + ", Tipo " + 
                           " from Split " +
			               Environment.NewLine + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) +
			               Environment.NewLine + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) +
			               Environment.NewLine;


				if (pdtmDataFinal != Constantes.DataInvalida) {
					strQuery = strQuery + " and Data <=  " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

				}

				strQuery = strQuery + " AND Tipo IN (" + FuncoesBd.CampoStringFormatar("DESD") + ", " + FuncoesBd.CampoStringFormatar("CISAO") + ")";


			}

			strQuery = strQuery + " ORDER BY Data";


			if (pstrOrdem == "D") {
				//se o ordenamento é descente tem que inserir a opção DESC, quando for ascente não precisa porque é o padrão
				strQuery = strQuery + " desc ";

			}

			return strQuery;

		}

		public bool SplitConsultar(string pstrCodigo, DateTime pdtmDataInicial, string pstrOrdem, ref cRSList pobjRSListRet, DateTime pdtmDataFinal, string pstrTipo = "")
		{

			cRSList objRSList = new cRSList(_conexao);

		    string strQuery = QuerySplitGerar(pstrCodigo, pdtmDataInicial, pstrOrdem, pdtmDataFinal, pstrTipo);

			objRSList.AdicionarQuery(strQuery);

			objRSList.ExecuteQuery();

			pobjRSListRet = objRSList;

			return objRSList.DadosExistir;

		}

		public IList<cDesdobramento> CarregarTodos(Ativo pobjAtivo)
		{

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT Data, Tipo, QuantidadeAnterior, QuantidadePosterior " + Environment.NewLine;
			strSQL += "FROM Split " + Environment.NewLine;
			strSQL += "WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjAtivo.Codigo);
			strSQL += "ORDER BY Data ";

			cRS objRS = new cRS(_conexao);

			objRS.ExecuteQuery(strSQL);

			List<cDesdobramento> lstRetorno = new List<cDesdobramento>();

		    while (!objRS.EOF) {
				DateTime dtmData = Convert.ToDateTime(objRS.Field("Data"));
				string strTipo = Convert.ToString(objRS.Field("Tipo"));
				double dblQuantidadeAnterior = Convert.ToDouble(objRS.Field("QuantidadeAnterior"));
				double dblQuantidadePosterior = Convert.ToDouble(objRS.Field("QuantidadePosterior"));

				switch (strTipo) {

					case "DESD":
						lstRetorno.Add(new cSplit_Grupammento(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					case "DIV":
						lstRetorno.Add(new cDividendo(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					case "JCP":
						lstRetorno.Add(new cJurosSobreCapitalProprio(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					case "CISAO":
						lstRetorno.Add(new cCisao(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					case "RCDIN":
						lstRetorno.Add(new cRCDIN(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					case "REND":
						lstRetorno.Add(new cRendimento(pobjAtivo, dtmData, dblQuantidadeAnterior, dblQuantidadePosterior));
						break;
					default:

						throw new Exception("Tipo de desdobramento inválido.");
				}

				objRS.MoveNext();
			}

			objRS.Fechar();

			return lstRetorno;

		}


	}
}
