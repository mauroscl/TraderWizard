using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;
using frwInterface;

namespace prjModelo.Carregadores
{

	public class cCarregadorSplit
	{


		private readonly cConexao objConexao;
		public cCarregadorSplit(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		private string QuerySplitGerar(string pstrCodigo, System.DateTime pdtmDataInicial, string pstrOrdem, System.DateTime pdtmDataFinal, string pstrTipo = "")
		{

			string strQuery = string.Empty;


			if (pstrTipo == String.Empty | pstrTipo == "DIV" | pstrTipo == "JCP" | pstrTipo == "REND" | pstrTipo == "RCDIN") {
				//Tratamento para outros tipos 

				strQuery = " select Data, (Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)) / AVG(QuantidadePosterior) as Razao " + Environment.NewLine + ", AVG(QuantidadePosterior) / (Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior)) as RazaoInvertida " + Environment.NewLine + ", CSTR((Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior))) & " + FuncoesBD.CampoStringFormatar("/") + " & CSTR(AVG(QuantidadePosterior)) as Razao2 " + Environment.NewLine + ", CSTR(AVG(QuantidadePosterior)) & " + FuncoesBD.CampoStringFormatar("/") + " & CSTR((Avg(QuantidadePosterior) - Sum(QuantidadePosterior - QuantidadeAnterior))) as RazaoInvertida2 " + Environment.NewLine + ", " + FuncoesBD.CampoStringFormatar("PROV") + " AS Tipo " + " from Split " + Environment.NewLine + " where Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " and Data >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;


				if (pdtmDataFinal != cConst.DataInvalida) {
					strQuery = strQuery + " and Data <=  " + FuncoesBD.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

				}


				if (pstrTipo != String.Empty) {
					//Se é um tipo específico
					strQuery = strQuery + " AND Tipo = " + FuncoesBD.CampoStringFormatar(pstrTipo) + Environment.NewLine;


				} else {
					//senão registro pelos dois tipos: DIV, JCP.
					strQuery = strQuery + " AND Tipo IN(" + FuncoesBD.CampoStringFormatar("DIV") + ", " + FuncoesBD.CampoStringFormatar("JCP") + ", " + FuncoesBD.CampoStringFormatar("REND") + ", " + FuncoesBD.CampoStringFormatar("RCDIN") + ")" + Environment.NewLine;

				}

				strQuery = strQuery + " GROUP BY Data" + Environment.NewLine;


			}


			if (pstrTipo == String.Empty | pstrTipo == "DESD" | pstrTipo == "CISAO") {
				//Tratamento para desdobramentos e cisão. Só há um desdobramento ou cisão por dia.
				//Por isso não é necessário fazer somatórios das quantidades 
				//anteriores e posteriores agrupados por data

				if (strQuery != String.Empty) {
					strQuery = strQuery + " UNION " + Environment.NewLine;

				}

				strQuery = strQuery + " select Data, QuantidadeAnterior / QuantidadePosterior as Razao " + Environment.NewLine + ", QuantidadePosterior / QuantidadeAnterior as RazaoInvertida " + Environment.NewLine + ", QuantidadeAnterior & " + FuncoesBD.CampoStringFormatar("/") + " & QuantidadePosterior as Razao2 " + Environment.NewLine + ", QuantidadePosterior & " + FuncoesBD.CampoStringFormatar("/") + " & QuantidadeAnterior as RazaoInvertida2 " + Environment.NewLine + ", Tipo " + " from Split " + Environment.NewLine + " where Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " and Data >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;


				if (pdtmDataFinal != cConst.DataInvalida) {
					strQuery = strQuery + " and Data <=  " + FuncoesBD.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

				}

				strQuery = strQuery + " AND Tipo IN (" + FuncoesBD.CampoStringFormatar("DESD") + ", " + FuncoesBD.CampoStringFormatar("CISAO") + ")";


			}

			strQuery = strQuery + " ORDER BY Data";


			if (pstrOrdem == "D") {
				//se o ordenamento é descente tem que inserir a opção DESC, quando for ascente não precisa porque é o padrão
				strQuery = strQuery + " desc ";

			}

			return strQuery;

		}


		/// <summary>
		/// Consulta os splits de um determinado ativo em um determinado período.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataInicial">período inicial de consulta</param>
		/// <param name="pdtmDataFinal">periodo final de consulta</param>
		/// <param name="pobjRSRet">Record Set contendo as datas e a razão dos splits</param>
		/// <param name="pstrOrdem">
		/// indica a ordem de retorno do split
		/// A - ascendente
		/// D - descendente
		/// </param>
		/// <param name="pstrTipo">Tipo de Split. Se não for recebido nenhum tipo serão considerados
		/// todos os tipos. As opções possíveis são:
		/// DESD = desdobramentos que são splits ou grupamentos
		/// DIV = dividendos
		/// JCP = juros sobre capital próprio
		/// REND, RCDIN = 
		/// CISAO = Cisão, onde um papel é dividido em dois. É gerado outro papel para ser 
		/// negociado permanecendo o original.
		/// </param>
		/// <returns>
		/// True - ativo tem splits no periodo recebido por parâmetro
		/// False - ativo não tem splits no periodo recebido por parâmetro
		/// </returns>
		/// <remarks></remarks>
		//Public Function SplitConsultar(ByVal pstrCodigo As String _
		//                               , ByVal pdtmDataInicial As Date, ByVal pstrOrdem As String, ByRef pobjRSRet As cRS _
		//                               , Optional ByVal pdtmDataFinal As Date = cConst.DataInvalida _
		//                               , Optional ByVal pstrTipo As String = vbNullString) As Boolean

		//    Dim objRS As New cRS(objConexao)

		//    Dim strQuery As String

		//    strQuery = QuerySplitGerar(pstrCodigo, pdtmDataInicial, pstrOrdem, pdtmDataFinal, pstrTipo)

		//    objRS.ExecuteQuery(strQuery)

		//    pobjRSRet = objRS

		//    Return objRS.DadosExistir

		//End Function


		public bool SplitConsultar(string pstrCodigo, DateTime pdtmDataInicial, string pstrOrdem, ref cRSList pobjRSListRet, DateTime pdtmDataFinal, string pstrTipo = "")
		{

			cRSList objRSList = new cRSList(objConexao);

			string strQuery = null;

			strQuery = QuerySplitGerar(pstrCodigo, pdtmDataInicial, pstrOrdem, pdtmDataFinal, pstrTipo);

			objRSList.AdicionarQuery(strQuery);

			objRSList.ExecuteQuery();

			pobjRSListRet = objRSList;

			return objRSList.DadosExistir;

		}

		public IList<cDesdobramento> CarregarTodos(cAtivo pobjAtivo)
		{

			string strSQL = null;

			strSQL = "SELECT Data, Tipo, QuantidadeAnterior, QuantidadePosterior " + Environment.NewLine;
			strSQL += "FROM Split " + Environment.NewLine;
			strSQL += "WHERE Codigo = " + FuncoesBD.CampoFormatar(pobjAtivo.Codigo);
			strSQL += "ORDER BY Data ";

			cRS objRS = new cRS(objConexao);

			objRS.ExecuteQuery(strSQL);

			List<cDesdobramento> lstRetorno = new List<cDesdobramento>();

			DateTime dtmData = default(DateTime);
			string strTipo = null;
			double dblQuantidadeAnterior = 0;
			double dblQuantidadePosterior = 0;


			while (!objRS.EOF) {
				dtmData = Convert.ToDateTime(objRS.Field("Data"));
				strTipo = Convert.ToString(objRS.Field("Tipo"));
				dblQuantidadeAnterior = Convert.ToDouble(objRS.Field("QuantidadeAnterior"));
				dblQuantidadePosterior = Convert.ToDouble(objRS.Field("QuantidadePosterior"));

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
