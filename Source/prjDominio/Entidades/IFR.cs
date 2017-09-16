namespace Dominio.Entidades
{

	public abstract class cIFR
	{

		public CotacaoAbstract Cotacao;
		public int NumPeriodos;

		public double Valor;

	    protected cIFR(CotacaoAbstract pobjCotacao, int pintNumPeriodos, double pdecValor)
		{
			Cotacao = pobjCotacao;
			Valor = pdecValor;
			NumPeriodos = pintNumPeriodos;
		}

		public override bool Equals(object obj)
		{

			var objIFR = (cIFR)obj;

			if (Cotacao.Equals(objIFR.Cotacao) && NumPeriodos == objIFR.NumPeriodos) {
				return true;
			} else {
				return false;
			}

		}

	    protected bool Equals(cIFR other)
	    {
	        return Equals(Cotacao, other.Cotacao) && NumPeriodos == other.NumPeriodos;
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((Cotacao != null ? Cotacao.GetHashCode() : 0)*397) ^ NumPeriodos;
	        }
	    }
	}

	public class cIFRDiario : cIFR
	{

		public cIFRDiario(CotacaoAbstract pobjCotacao, int pintNumPeriodos, double pdecValor) : base(pobjCotacao, pintNumPeriodos, pdecValor)
		{
		}

	}

}
