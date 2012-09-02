using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DataBase;
namespace prmCotacao
{

	public class cFuncUtil
	{


		private cConexao objConexao;

		public cFuncUtil(cConexao pobjConexao)
		{
			objConexao = pobjConexao;

		}

		public bool IFRMEDIODOISPRIMEIROSDIASREMOVER(string pstrPeriodo)
		{

			cCommand objCommand = new cCommand(objConexao);

			//Dim objConnAux As cConexao = New cConexao()

			cRS objRS = new cRS(objConexao);


			cRS objRSAux = new cRS(objConexao);

			string strQuery = null;

			string strTabelaCotacao = null;
			string strTabelaMedia = null;

			if (pstrPeriodo == "DIARIO") {
				strTabelaCotacao = "COTACAO";
				strTabelaMedia = "MEDIA_DIARIA";
			} else {
				strTabelaCotacao = "COTACAO_SEMANAL";
				strTabelaMedia = "MEDIA_SEMANAL";
			}

			objCommand.BeginTrans();

			strQuery = "Select codigo " + "FROM ativo " + "WHERE codigo Not In " + "(" + " SELECT codigo " + "FROM ativos_desconsiderados" + ")" + " And " + " (( " + " Select Count(1) " + " FROM " + strTabelaCotacao + " WHERE ativo.codigo = " + strTabelaCotacao + ".codigo " + " ) >= 15)" + " And (( " + " Select Count(1) " + " FROM " + strTabelaCotacao + " WHERE ativo.codigo = " + strTabelaCotacao + ".codigo " + ") - " + "(" + " Select Count(1) " + " FROM " + strTabelaMedia + " WHERE ativo.codigo = " + strTabelaMedia + ".codigo " + " And tipo = 'IFR2' " + " And numperiodos = 13 " + ") <> 14) " + " And Exists " + "(" + " Select 1 " + " FROM " + strTabelaCotacao + " WHERE ATIVO.CODIGO = " + strTabelaCotacao + ".CODIGO " + " And " + strTabelaCotacao + ".DATA = #12-14-2009# " + ")";

			objRS.ExecuteQuery(strQuery);



			while (!objRS.EOF) {
                strQuery = " SELECT TOP 2 DATA " + " FROM " + "(" + " SELECT DATA " + " FROM " + strTabelaMedia + " WHERE CODIGO = " + FuncoesBD.CampoStringFormatar(Convert.ToString(objRS.Field("Codigo"))) + " and Tipo = " + FuncoesBD.CampoStringFormatar("IFR2") + " And NumPeriodos = 13 " + " order by data " + ")";

				objRSAux.ExecuteQuery(strQuery);


				while (!objRSAux.EOF) {
                    strQuery = "DELETE" + " FROM " + strTabelaMedia + " WHERE CODIGO = " + FuncoesBD.CampoStringFormatar(Convert.ToString(objRS.Field("Codigo"))) + " and Tipo = " + FuncoesBD.CampoStringFormatar("IFR2") + " And NumPeriodos = 13 " + " AND DATA = " + FuncoesBD.CampoDateFormatar((DateTime) objRSAux.Field("Data"));

					objCommand.Execute(strQuery);

					objRSAux.MoveNext();

				}

				objRSAux.Fechar();

				objRS.MoveNext();

			}

			objCommand.CommitTrans();

			objRS.Fechar();

			return objCommand.TransStatus;

		}


	}
}
