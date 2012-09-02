namespace prjDTO
{
	public class cTradeDTO
	{

		public cTradeDTO(double pdblValor, bool pblnVerdadeiro, bool pblnMelhorEntrada, int pintNumTentativas)
		{
			dblValor = pdblValor;
			blnVerdadeiro = pblnVerdadeiro;
			blnMelhorEntrada = pblnMelhorEntrada;
			intNumTentativas = pintNumTentativas;
		}

		private double dblValor;
		private bool blnVerdadeiro;
		private bool blnMelhorEntrada;

		private int intNumTentativas;
		public double Valor {
			get { return dblValor; }
		}

		public bool Verdadeiro {
			get { return blnVerdadeiro; }
		}

		public bool MelhorEntrada {
			get { return blnMelhorEntrada; }
		}

		public int NumTentativas {
			get { return intNumTentativas; }
		}


	}
}
