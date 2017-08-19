using System;
using System.Windows.Forms;
using System.Xml;

namespace Services
{

	public class ArquivoXml
	{

		private readonly string _caminhoCompleto;

		private XmlReader _xmlReader;

		public ArquivoXml(string pstrCaminhoCompleto)
		{
			_caminhoCompleto = pstrCaminhoCompleto;

		}

		public bool Abrir()
		{
			bool functionReturnValue;


			try {
				_xmlReader = XmlReader.Create(_caminhoCompleto);

				_xmlReader.MoveToContent();

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
				_xmlReader.Close();

				functionReturnValue = true;


			} catch (Exception ex) {
				MessageBox.Show("Erro ao fechar arquivo XML " + ex.Message + ".","XML",MessageBoxButtons.OK ,MessageBoxIcon.Error);

				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		public bool EOF()
		{

			return _xmlReader.EOF;

		}

		public bool LerNodo()
		{
			return _xmlReader.Read();
		}

		public object LerAtributo(string pstrAtributoNome, object pobjRetornoErro = null)
		{
			object functionReturnValue;

			//LerAtributo = objXMLReader.GetAttribute(pstrAtributoNome)
			bool blnOK = _xmlReader.MoveToAttribute(pstrAtributoNome);


			if (blnOK) {
				functionReturnValue = _xmlReader.Value;


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
