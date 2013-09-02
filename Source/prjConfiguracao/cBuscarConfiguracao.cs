using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FluentNHibernate.Cfg.Db;
using TraderWizard.Enumeracoes;

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

    }
}
