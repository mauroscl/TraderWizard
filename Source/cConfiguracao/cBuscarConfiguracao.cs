using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Configuration;
namespace cConfiguracao
{

	public class cBuscarConfiguracao
	{

		public static string ObtemCaminhoPadrao()
		{

			string strCaminho = ConfigurationManager.AppSettings["CaminhoPadrao"];


			if (Strings.Right(strCaminho, 1) != "\\") {
				strCaminho = strCaminho + "\\";

			}

			return strCaminho;

		}

		public static string ObterConnectionStringPadrao()
		{
			return ConfigurationManager.ConnectionStrings["Padrao"].ConnectionString;
		}

		public static int NumeroDeAtivosCotacaoIntraday()
		{
			return Convert.ToInt32(ConfigurationManager.AppSettings["NumeroAtivosAtualizacaoIntraday"]);
		}

	}
}
