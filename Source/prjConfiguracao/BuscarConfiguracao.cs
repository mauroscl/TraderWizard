using System;
using System.Configuration;
using FluentNHibernate.Cfg.Db;
using TraderWizard.Enumeracoes;

namespace Configuracao
{
    public class BuscarConfiguracao
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

        public static string ObterConnectionStringPadrao(cEnum.BancoDeDados bancoDeDados)
        {
            string nomeDaConexao = "";
            switch (bancoDeDados)
            {
                case cEnum.BancoDeDados.SqlServer:
                    nomeDaConexao = "PadraoSqlServer";
                    break;
                case cEnum.BancoDeDados.Access:
                    nomeDaConexao = "PadraoAccess";
                    break;
            }
            return ConfigurationManager.ConnectionStrings[nomeDaConexao].ConnectionString;

        }

        public static string ObterConnectionStringDoNHibernate(cEnum.BancoDeDados bancoDeDados)
        {
            string nomeDaConexao = "";
            switch (bancoDeDados)
            {
                case cEnum.BancoDeDados.SqlServer:
                    nomeDaConexao = "NHibernateSqlServer";
                    break;
                case cEnum.BancoDeDados.Access:
                    nomeDaConexao = "NHibernateAccess";
                    break;
            }
            return ConfigurationManager.ConnectionStrings[nomeDaConexao].ConnectionString;

        }


        public static int NumeroDeAtivosCotacaoIntraday()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings.Get("NumeroAtivosAtualizacaoIntraday"));
        }

        public static cEnum.BancoDeDados ObterBancoDeDados()
        {
            var bancoDeDados = ConfigurationManager.AppSettings.Get("BancoDeDados");
            if (string.IsNullOrEmpty(bancoDeDados))
            {
                throw new Exception("Banco de dados não configurado para a aplicação. Verifique o arquivo de configuração.");
            }
            return (cEnum.BancoDeDados) Enum.Parse(typeof (cEnum.BancoDeDados), bancoDeDados, true);
        }

        public static IPersistenceConfigurer ObterConfiguracaoDoNHibernate()
        {
            cEnum.BancoDeDados bancoDeDados = ObterBancoDeDados();
            string connectionString = ObterConnectionStringDoNHibernate(bancoDeDados);
            switch (bancoDeDados)
            {
                case cEnum.BancoDeDados.Access:
                    return JetDriverConfiguration.Standard.ConnectionString(c => c.Is(connectionString));
                case cEnum.BancoDeDados.SqlServer:
                    return MsSqlConfiguration.MsSql2008.ConnectionString(c => c.Is(connectionString));
                default:
                    throw new ConfigurationErrorsException( "configuração de bando de dados inválida");
            }
        }

        public static String ObterUrlBoletimDiario ()
        {
            var url = ConfigurationManager.AppSettings.Get("UrlBoletimDiario");
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Url do boletim diário não configurada para a aplicação. Verifique o arquivo de configuração.");
            }

            return url;
        }

        public static String ObterUrlCotacaoHistorica()
        {
            var url = ConfigurationManager.AppSettings.Get("UrlCotacaoHistorica");
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("Url do boletim diário não configurada para a aplicação. Verifique o arquivo de configuração.");
            }

            return url;
        }

    }
}
