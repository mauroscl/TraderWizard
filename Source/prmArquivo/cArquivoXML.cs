using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

using System.Xml;
namespace prmArquivo
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
				Interaction.MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Abrir Arquivo XML");

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
				Interaction.MsgBox("Erro ao fechar arquivo XML " + ex.Message + ".", MsgBoxStyle.Information, "XML");

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
			object functionReturnValue = null;

			//LerAtributo = objXMLReader.GetAttribute(pstrAtributoNome)
			bool blnOK = objXMLReader.MoveToAttribute(pstrAtributoNome);


			if (blnOK) {
				functionReturnValue = objXMLReader.Value;


				if (Strings.Trim(LerAtributo()) == Constants.vbNullString) {
					functionReturnValue = pobjRetornoErro;

				}

			} else {
				functionReturnValue = pobjRetornoErro;
			}
			return functionReturnValue;

		}

	}
}
