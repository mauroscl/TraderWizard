using System;
using DataBase;

namespace WebAccess
{

	public class WebConfiguracao
	{
	    //conexão com o banco de dados
		private readonly Conexao _conexao;

        public string ProxyTipo { get; private set; }

	    public string ProxyManualHTTP { get; private set; }

	    public int ProxyManualPorta { get; private set; }

	    public bool CredencialUtilizar { get; private set; }

	    public string Dominio { get; private set; }

	    public string Usuario { get; private set; }

	    public string Senha { get; private set; }


	    public WebConfiguracao(bool pblnConfiguracaoBuscar, Conexao pobjConexao)
		{
			_conexao = pobjConexao;

			if (pblnConfiguracaoBuscar) {
				ConfiguracoesBuscar();
			}
		}


		public void ConfiguracoesBuscar()
		{
		    string strValor = string.Empty;

			//consulta o tipo de proxy
			ParametroConsultar("ProxyTipo", ref strValor);


			if (string.IsNullOrEmpty( strValor)) {
				//se o parâmetro não está cadastrado o padrão é sem PROXY.
				strValor = "SP";

			}

			ProxyTipo = strValor;


			if (strValor == "PM") {
				//caso o proxy seja manual, tem que consultar o endereço HTTP e a porta que devem ser utilizadas
				ParametroConsultar("ProxyManualHTTP", ref strValor);

				ProxyManualHTTP = strValor;

				ParametroConsultar("ProxyManualPorta", ref strValor);

				ProxyManualPorta = Convert.ToInt32(strValor);

			}

			ParametroConsultar("ProxyCredencialUtilizar", ref strValor);

			CredencialUtilizar = (strValor == "SIM");


			if (strValor == "SIM") {
				ParametroConsultar("ProxyCredencialDominio", ref strValor);

				Dominio = strValor;

				ParametroConsultar("ProxyCredencialUsuario", ref strValor);

				Usuario = strValor;

				ParametroConsultar("ProxyCredencialSenha", ref strValor);

				Criptografia objCriptografia = new Criptografia();

				//a senha é armazenada no banco criptografada. Por isso precisa ser descriptografada
				Senha = objCriptografia.Descriptograr(strValor);

			}

		}

		public bool ParametroConsultar(string pstrParametro, ref string pstrValorRet)
		{

			DadosDb objDadosDB = new DadosDb(_conexao, "Configuracao");

			objDadosDB.CampoAdicionar("Parametro", true, pstrParametro);

			objDadosDB.CampoAdicionar("Valor", false, string.Empty);

			objDadosDB.DadosBDConsultar();

			pstrValorRet = objDadosDB.CampoConsultar("Valor");

			return true;

		}

	}
}
