using System;

namespace Dominio.Regras
{
	public class GeradorNomeArquivo
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
		    string strDia = FormatarComDoisDigitos(pdtmData.Day);

			string strMes = FormatarComDoisDigitos(pdtmData.Month);

			return "bdi" + strMes + strDia + ".zip";

		}

		public static string GerarNomeArquivoLocal(DateTime pdtmData)
		{
		    string strDia = FormatarComDoisDigitos(pdtmData.Day);
			string strMes = FormatarComDoisDigitos(pdtmData.Month);

			return "bdi" + Convert.ToString(pdtmData.Year) + strMes + strDia + ".zip";

		}


	}
}
