using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace prjConfiguracao
{
    public class cBuscarConfiguracao
    {

        public static string ObtemCaminhoPadrao()
        {
            string strCaminho = ConfigurationManager.AppSettings.Get("CaminhoPadrao");

            if (strCaminho.Substring(strCaminho.Length - 1, 1) != "\\")
            {
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
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("NumeroAtivosAtualizacaoIntraday"));
        }
    }
}
