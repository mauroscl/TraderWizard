using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
namespace frwTela
{
	public class cColuna
	{

		private string strNome;
		private string strCaption;

		private System.Type objTipo;

		public cColuna(string pstrNome, string pstrCaption, System.Type pobjTipo)
		{
			strNome = pstrNome;
			strCaption = pstrCaption;
			objTipo = pobjTipo;

		}

		public string Nome {
			get { return strNome; }
		}

		public string Caption {
			get { return strCaption; }
		}

		public System.Type Tipo {
			get { return objTipo; }
		}

	}
}
