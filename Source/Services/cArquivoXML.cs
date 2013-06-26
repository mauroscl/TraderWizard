using System;
using System.Windows.Forms;
using System.Xml;

namespace Services
{

	public class cArquivoXML
	{

		private string strCaminhoCompleto;

		private XmlReader objXMLReader;

		public cArquivoXML(string pstrCaminhoCompleto)
		{
			strCaminhoCompleto = pstrCaminhoCompleto;

		}

		public bool Abrir()
		{
			bool functionReturnValue = false;


			try {
				objXMLReader = XmlReader.Create(strCaminhoCompleto);

				objXMLReader.MoveToContent();

				functionReturnValue = true;

			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Abrir Arquivo XML",MessageBoxButtons.OK, MessageBoxIcon.Error);

				Fechar();

				functionReturnValue = false;

			}
			return functionReturnValue;


		}

		public bool Fechar()
		{
			bool functionReturnValue = false;


			try {
				objXMLReader.Close();

				functionReturnValue = true;


			} catch (Exception ex) {
				MessageBox.Show("Erro ao fechar arquivo XML " + ex.Message + ".","XML",MessageBoxButtons.OK ,MessageBoxIcon.Error);

				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		public bool EOF()
		{

			return objXMLReader.EOF;

		}

		public bool LerNodo()
		{
			return objXMLReader.Read();
		}

		public object LerAtributo(string pstrAtributoNome, object pobjRetornoErro = null)
		{
			object functionReturnValue;

			//LerAtributo = objXMLReader.GetAttribute(pstrAtributoNome)
			bool blnOK = objXMLReader.MoveToAttribute(pstrAtributoNome);


			if (blnOK) {
				functionReturnValue = objXMLReader.Value;


                if (String.IsNullOrEmpty(functionReturnValue.ToString()))
                {
					functionReturnValue = pobjRetornoErro;

				}

			} else {
				functionReturnValue = pobjRetornoErro;
			}
			return functionReturnValue;

		}

	}
}
