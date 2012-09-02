namespace prjDTO
{
	public class cFaixaDTO
	{


		public cFaixaDTO(double pdblValorMinimo, double pdblValorMaximo, int pintNumTentativasMinimo, int pintNumTradesTotal, int pintNumTradesVerdadeiro, int pintNumTradesMelhorEntrada)
		{
			dblValorMinimo = pdblValorMinimo;
			dblValorMaximo = pdblValorMaximo;
			intNumTentativasMinimo = pintNumTentativasMinimo;
			intNumTradesTotal = pintNumTradesTotal;
			intNumTradesVerdadeiro = pintNumTradesVerdadeiro;
			intNumTradesMelhorEntrada = pintNumTradesMelhorEntrada;

		}

		private double dblValorMinimo;
		private double dblValorMaximo;
		private int intNumTentativasMinimo;
		private int intNumTradesTotal;
		private int intNumTradesVerdadeiro;

		private int intNumTradesMelhorEntrada;
		public int NumTradesTotal {
			get { return intNumTradesTotal; }
		}

		public int NumTradesVerdadeiro {
			get { return intNumTradesVerdadeiro; }
		}

		public int NumTradesMelhorEntrada {
			get { return intNumTradesMelhorEntrada; }
		}

		public int NumTentativasMinimo {
			get { return intNumTentativasMinimo; }
		}

		public double ValorMinimo {
			get { return dblValorMinimo; }
		}

		public double ValorMaximo {
			get { return dblValorMaximo; }
		}

	}
}
