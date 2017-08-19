using System;

namespace Dominio.Entidades
{
	public class IFRSobrevendido
	{
	    protected bool Equals(IFRSobrevendido other)
	    {
	        return Id == other.Id;
	    }

	    public override int GetHashCode()
	    {
	        return Id;
	    }

	    public int Id { get; set; }

	    public double ValorMaximo { get; set; }


	    public IFRSobrevendido(Int32 plngID, double pdblValorMaximo)
		{
			Id = plngID;
			ValorMaximo = pdblValorMaximo;

		}

		public override bool Equals(object obj)
		{
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((IFRSobrevendido) obj);
		}

	    public override string ToString()
		{
			return Convert.ToString(ValorMaximo);
		}

	}
}
