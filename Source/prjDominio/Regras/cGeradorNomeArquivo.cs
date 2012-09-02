using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo
{
	public class cGeradorNomeArquivo
	{

		private static string FormatarComDoisDigitos(int pintNumero)
		{

			if (pintNumero < 10) {
				return "0" + Convert.ToString(pintNumero);
			} else {
				return Convert.ToString(pintNumero);
			}

		}

		public static string GerarNomeArquivoRemoto(System.DateTime pdtmData)
		{

			string strDia = null;

			string strMes = null;

			strDia = FormatarComDoisDigitos(pdtmData.Day);

			strMes = FormatarComDoisDigitos(pdtmData.Month);

			return "bdi" + strMes + strDia + ".zip";

		}

		public static string GerarNomeArquivoLocal(DateTime pdtmData)
		{

			string strDia = null;
			string strMes = null;

			strDia = FormatarComDoisDigitos(pdtmData.Day);
			strMes = FormatarComDoisDigitos(pdtmData.Month);

			return "bdi" + Convert.ToString(pdtmData.Year) + strMes + strDia + ".zip";

		}


	}
}
