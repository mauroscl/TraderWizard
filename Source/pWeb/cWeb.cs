using System;
using System.Net;
using System.IO;
using DataBase;
using System.Windows.Forms;
using Services;

namespace pWeb
{

	public class cWeb
	{
		private cWebConfiguracao objWebConfiguracao;
	    private readonly IFileService _fileService;

		public cWeb(cConexao pobjConexao)
		{
			//intancia a classe de configuração.
			//durante a instanciação já são buscadas as configurações cadastradas
			//nos parâmetros do sistema.
			objWebConfiguracao = new cWebConfiguracao(true, pobjConexao);
            _fileService = new FileService();

		}


		/*public bool ArquivoBaixar(string pstrURL, string pstrCaminhoDestino, string pstrArquivoDestino)
		{


			try {
				pWeb.My.MyProject.Computer.Network.DownloadFile(pstrURL, pstrCaminhoDestino + "\\" + pstrArquivoDestino);
                

				return true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message + " - URL: " + pstrURL, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

				//Catch exWeb As System.Net.WebException

				//    MsgBox(exWeb.Message, MsgBoxStyle.Critical)

				//    Return False

			}

		}*/

		public bool DownloadWithProxy(string pstrURL, string pstrCaminhoDestino, string pstrArquivoDestino)
		{
			bool functionReturnValue = false;

			WebProxy objWebProxy = null;

			switch (objWebConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(objWebConfiguracao.ProxyManualHTTP, objWebConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(HttpWebRequest.DefaultWebProxy.GetProxy(new Uri(pstrURL)));

					break;
			}


			if (objWebConfiguracao.CredencialUtilizar) {
				objWebProxy.Credentials = new NetworkCredential(objWebConfiguracao.Usuario, objWebConfiguracao.Senha, objWebConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			HttpWebRequest objHTTPWebRequest = (HttpWebRequest)WebRequest.Create(pstrURL);

			HttpWebResponse objHTTPWebResponse = null;

			Stream objReceivedStream = null;

			try {
				objHTTPWebRequest.Proxy = objWebProxy;

				objHTTPWebResponse = (HttpWebResponse)objHTTPWebRequest.GetResponse();

				objReceivedStream = objHTTPWebResponse.GetResponseStream();

                _fileService.Save(pstrCaminhoDestino + "\\" + pstrArquivoDestino,objReceivedStream);    

				functionReturnValue = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message + " - URL: " + pstrURL, "Web", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;


			} finally {
				if ((objReceivedStream != null)) {
					objReceivedStream.Close();
				}

				if ((objHTTPWebResponse != null)) {
					objHTTPWebResponse.Close();
				}

			}
			return functionReturnValue;

		}

		public bool VerificarLink(string pstrURL)
		{
			bool functionReturnValue = false;

			WebProxy objWebProxy = null;

			switch (objWebConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(objWebConfiguracao.ProxyManualHTTP, objWebConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(HttpWebRequest.DefaultWebProxy.GetProxy(new Uri(pstrURL)));

					break;
			}


			if (objWebConfiguracao.CredencialUtilizar) {
				objWebProxy.Credentials = new NetworkCredential(objWebConfiguracao.Usuario, objWebConfiguracao.Senha, objWebConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			HttpWebRequest objHTTPWebRequest = (HttpWebRequest)WebRequest.Create(pstrURL);

			HttpWebResponse objHTTPWebResponse = null;


			try {
				objHTTPWebRequest.Proxy = objWebProxy;

				objHTTPWebResponse = (HttpWebResponse)objHTTPWebRequest.GetResponse();

				functionReturnValue = (objHTTPWebResponse.StatusCode == HttpStatusCode.OK);


			} catch (Exception ex) {
				functionReturnValue = false;


			} finally {
				if ((objHTTPWebResponse != null)) {
					objHTTPWebResponse.Close();
				}

			}
			return functionReturnValue;

		}

	}
}
