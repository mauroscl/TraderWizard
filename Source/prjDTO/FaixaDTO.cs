namespace DTO
{
	public class FaixaDto
	{


		public FaixaDto(double pdblValorMinimo, double pdblValorMaximo, int pintNumTentativasMinimo, int pintNumTradesTotal, int pintNumTradesVerdadeiro, int pintNumTradesMelhorEntrada)
		{
			ValorMinimo = pdblValorMinimo;
			ValorMaximo = pdblValorMaximo;
			NumTentativasMinimo = pintNumTentativasMinimo;
			NumTradesTotal = pintNumTradesTotal;
			NumTradesVerdadeiro = pintNumTradesVerdadeiro;
			NumTradesMelhorEntrada = pintNumTradesMelhorEntrada;

		}

	    public int NumTradesTotal { get; }

	    public int NumTradesVerdadeiro { get; }

	    public int NumTradesMelhorEntrada { get; }

	    public int NumTentativasMinimo { get; }

	    public double ValorMinimo { get; }

	    public double ValorMaximo { get; }
	}
}
