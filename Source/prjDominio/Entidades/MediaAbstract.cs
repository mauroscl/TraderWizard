using System;
using DTO;

namespace Dominio.Entidades
{

	public abstract class MediaAbstract
	{
	    protected bool Equals(MediaAbstract other)
	    {
	        return Equals(Cotacao, other.Cotacao) && string.Equals(Tipo, other.Tipo) && NumPeriodos == other.NumPeriodos;
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            int hashCode = (Cotacao != null ? Cotacao.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (Tipo != null ? Tipo.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ NumPeriodos;
	            return hashCode;
	        }
	    }

	    public CotacaoAbstract Cotacao;
		public string Tipo;
		public int NumPeriodos;

		public double Valor;
		public MediaAbstract(CotacaoAbstract pobjCotacao, string pstrTipo, int pintNumPeriodos, double pdecValor)
		{
			Cotacao = pobjCotacao;
			Valor = pdecValor;
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;
		}

		public override bool Equals(object obj)
		{
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((MediaAbstract) obj);
		}

		public MediaDTO ObtemDTO()
		{

			string strDado;
			string strTipo;

			switch (Tipo) {
				case "MME":
					strTipo = "E";
					strDado = "VALOR";
					break;
				case "MMA":
					strTipo = "A";
					strDado = "VALOR";
					break;
				case "VMA":
					strTipo = "A";
					strDado = "VOLUME";
					break;
				case "IFR2":
					strTipo = "A";
					strDado = "IFR2";
					break;
				default:

					throw new Exception("Tipo não encontrado para conversão: " + Tipo);
			}

			return new MediaDTO(strTipo, NumPeriodos, strDado);

		}


	}

	public class MediaDiaria : MediaAbstract
	{

		public MediaDiaria(CotacaoAbstract pobjCotacao, string pstrTipo, int pintNumPeriodos, double pdecValor) : base(pobjCotacao, pstrTipo, pintNumPeriodos, pdecValor)
		{
		}

	}
}
