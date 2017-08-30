using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using DataBase;
using Services;

namespace WebAccess
{

	public class Web
	{
		private readonly WebConfiguracao _webConfiguracao;
	    private readonly IFileService _fileService;

		public Web(Conexao pobjConexao)
		{
			//intancia a classe de configuração.
			//durante a instanciação já são buscadas as configurações cadastradas
			//nos parâmetros do sistema.
			_webConfiguracao = new WebConfiguracao(true, pobjConexao);
            _fileService = new FileService();

		}

		public bool DownloadWithProxy(string url, string pstrCaminhoDestino, string pstrArquivoDestino)
		{
			WebProxy objWebProxy = null;

			switch (_webConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(_webConfiguracao.ProxyManualHTTP, _webConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(WebRequest.DefaultWebProxy.GetProxy(new Uri(url)));

					break;

			}


			if (_webConfiguracao.CredencialUtilizar && objWebProxy != null) {
				objWebProxy.Credentials = new NetworkCredential(_webConfiguracao.Usuario, _webConfiguracao.Senha, _webConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			var objHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);

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
                        bytesRead = responseStream?.Read(buffer, 0, bufferSize) ?? throw new Exception("Não foi possível ler o conteúdo do arquivo");
                        bufferTotal = bufferTotal.Concat(buffer.ToList().GetRange(0, bytesRead)).ToList();

                    } while (bytesRead > 0);
                }

                _fileService.Save(pstrCaminhoDestino + "\\" + pstrArquivoDestino,bufferTotal.ToArray());

			    objHttpWebResponse.Close();


			} catch (Exception ex) {
                MessageBox.Show(ex.Message + " - URL: " + url, "Web", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;


			} 

			return true;

		}

		public bool VerificarLink(string pstrUrl)
		{
			WebProxy objWebProxy = null;

			switch (_webConfiguracao.ProxyTipo) {

				case "PM":

					//PROXY MANUAL
					objWebProxy = new WebProxy(_webConfiguracao.ProxyManualHTTP, _webConfiguracao.ProxyManualPorta);

					break;
				case "PA":

					//PROXY AUTOMÁTICO. Passa o endereço e o proxy é retornado.
					objWebProxy = new WebProxy(WebRequest.DefaultWebProxy.GetProxy(new Uri(pstrUrl)));

					break;
			}


			if (_webConfiguracao.CredencialUtilizar) {
				objWebProxy.Credentials = new NetworkCredential(_webConfiguracao.Usuario, _webConfiguracao.Senha, _webConfiguracao.Dominio);

			}

			// Create a new request to the mentioned URL.                
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(pstrUrl);

			HttpWebResponse objHttpWebResponse = null;

            bool functionReturnValue;

			try {
				httpWebRequest.Proxy = objWebProxy;

				objHttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

				functionReturnValue = (objHttpWebResponse.StatusCode == HttpStatusCode.OK);


			} catch (Exception) {
				functionReturnValue = false;


			} finally
			{
			    objHttpWebResponse?.Close();
			}
			return functionReturnValue;

		}

	}
}
