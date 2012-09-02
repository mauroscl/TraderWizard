using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
namespace DataBase
{
	public class cCampoDB
	{


		private string strCampo;

		private string strValor;

		private bool blnChave;
		public string Campo {
			get { return strCampo; }
		}

		public bool Chave {
			get { return blnChave; }
		}

		public string Valor {
			get { return strValor; }
			set { strValor = value; }
		}



		public cCampoDB(string pstrCampo, bool pblnChave, string pstrValor)
		{
			strCampo = pstrCampo;

			blnChave = pblnChave;

			strValor = pstrValor;

		}

	}
}
