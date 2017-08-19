namespace DTO
{
	public class TradeDto
	{

		public TradeDto(double pdblValor, bool pblnVerdadeiro, bool pblnMelhorEntrada, int pintNumTentativas)
		{
			Valor = pdblValor;
			Verdadeiro = pblnVerdadeiro;
			MelhorEntrada = pblnMelhorEntrada;
			NumTentativas = pintNumTentativas;
		}

	    public double Valor { get; }

	    public bool Verdadeiro { get; }

	    public bool MelhorEntrada { get; }

	    public int NumTentativas { get; }
	}
}
