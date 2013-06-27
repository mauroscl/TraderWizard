using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
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
			bool functionReturnValue;

			WebProxy objWebProxy = null;

			switch (objWebConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(objWebConfiguracao.ProxyManualHTTP, objWebConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(WebRequest.DefaultWebProxy.GetProxy(new Uri(pstrURL)));

					break;

			}


			if (objWebConfiguracao.CredencialUtilizar && objWebProxy != null) {
				objWebProxy.Credentials = new NetworkCredential(objWebConfiguracao.Usuario, objWebConfiguracao.Senha, objWebConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			var objHttpWebRequest = (HttpWebRequest)WebRequest.Create(pstrURL);

			try {
				objHttpWebRequest.Proxy = objWebProxy;

                var objHttpWebResponse = objHttpWebRequest.GetResponse();

                var bufferTotal = new List<byte>();

                const int bufferSize = 4096;

                using (var responseStream = objHttpWebResponse.GetResponseStream())
                {
                    var buffer = new byte[bufferSize];
                    int bytesRead;
                    do
                    {
                        bytesRead = responseStream.Read(buffer, 0, bufferSize);
                        bufferTotal = bufferTotal.Concat(buffer.ToList().GetRange(0, bytesRead)).ToList();

                    } while (bytesRead > 0);
                }

                _fileService.Save(pstrCaminhoDestino + "\\" + pstrArquivoDestino,bufferTotal.ToArray());

			    objHttpWebResponse.Close();


			    functionReturnValue = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message + " - URL: " + pstrURL, "Web", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;


			} 

			return functionReturnValue;

		}

		public bool VerificarLink(string pstrUrl)
		{
			WebProxy objWebProxy = null;

			switch (objWebConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(objWebConfiguracao.ProxyManualHTTP, objWebConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(HttpWebRequest.DefaultWebProxy.GetProxy(new Uri(pstrUrl)));

					break;
			}


			if (objWebConfiguracao.CredencialUtilizar) {
				objWebProxy.Credentials = new NetworkCredential(objWebConfiguracao.Usuario, objWebConfiguracao.Senha, objWebConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			var objHTTPWebRequest = (HttpWebRequest)WebRequest.Create(pstrUrl);

			HttpWebResponse objHttpWebResponse = null;

            bool functionReturnValue;

			try {
				objHTTPWebRequest.Proxy = objWebProxy;

				objHttpWebResponse = (HttpWebResponse)objHTTPWebRequest.GetResponse();

				functionReturnValue = (objHttpWebResponse.StatusCode == HttpStatusCode.OK);


			} catch (Exception) {
				functionReturnValue = false;


			} finally {
				if ((objHttpWebResponse != null)) {
					objHttpWebResponse.Close();
				}

			}
			return functionReturnValue;

		}

	}
}
