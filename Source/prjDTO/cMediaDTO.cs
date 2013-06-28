using System;
using System.Drawing;
namespace prjDTO
{

	public class cMediaDTO
	{

		public cMediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado)
		{
			strTipo = pstrTipo;
			intNumPeriodos = pintNumPeriodos;
			strDado = pstrDado;
		}


		public cMediaDTO(string pstrTipo, int pintNumPeriodos)
		{
			strTipo = pstrTipo;
			intNumPeriodos = pintNumPeriodos;

		}

		public cMediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado, int pintNumRegistros)
		{
			strTipo = pstrTipo;
			intNumPeriodos = pintNumPeriodos;
			strDado = pstrDado;
			intNumRegistros = pintNumRegistros;
		}

		public cMediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado, Color pobjCor)
		{
			strTipo = pstrTipo;
			intNumPeriodos = pintNumPeriodos;
			strDado = pstrDado;
			objCor = pobjCor;
		}

			// tipo da média
		private readonly string strTipo;
			//número de períodos utilizado no cálculo da média
		private readonly int intNumPeriodos;
			//número de registros  para a média encontrado em determinado período.
		private int intNumRegistros;
			//dado utilizado no cálculo da média: "VALOR" OU "VOLUME"
		private readonly string strDado;

		private readonly Color objCor;
		public string Tipo {
			get { return strTipo; }
		}

		public int NumPeriodos {
			get { return intNumPeriodos; }
		}

		public int NumRegistros {
			get { return intNumRegistros; }
		}

		public string Dado {
			get { return strDado; }
		}

		public Color Cor {
			get { return objCor; }
		}

		public override bool Equals(object obj)
		{

			//Return MyBase.Equals(obj)
            cMediaDTO objAux = (cMediaDTO)obj;

            if (strTipo == objAux.Tipo && intNumPeriodos == objAux.NumPeriodos && strDado == objAux.Dado)
            {
				//Se o tipo da média e o número de peridos são iguais, os objetos são considerados iguais.
				return true;
			} else {
				return false;
			}

		}

		public void IncrementaNumRegistros(int pintIncremento)
		{
			intNumRegistros = intNumRegistros + pintIncremento;
		}

		public string CampoTipoBD {
			get {
				string strCampoBD = null;
				if (Tipo == "E") {
					strCampoBD = "MME";
				} else {
					switch (Dado) {
						case "VALOR":
							strCampoBD = "MMA";
							break;
						case "VOLUME":
							strCampoBD = "VMA";
							break;
						case "IFR2":
							strCampoBD = "IFR2";
							break;
						default:
							strCampoBD = String.Empty;
							break;
					}
				}

				return strCampoBD;

			}
		}


		public string GetAlias {
			get { return CampoTipoBD + NumPeriodos.ToString(); }
		}

	}
}
