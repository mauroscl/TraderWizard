using prjDominio.Entidades;

namespace prjModelo.Entidades
{

	public abstract class cIFR
	{

		public CotacaoAbstract Cotacao;
		public int NumPeriodos;

		public double Valor;
		public cIFR(CotacaoAbstract pobjCotacao, int pintNumPeriodos, double pdecValor)
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

	}

	public class cIFRDiario : cIFR
	{

		public cIFRDiario(CotacaoAbstract pobjCotacao, int pintNumPeriodos, double pdecValor) : base(pobjCotacao, pintNumPeriodos, pdecValor)
		{
		}

	}

}
