using System;

namespace prjDominio.Entidades
{
	public class cIFRSobrevendido
	{
	    protected bool Equals(cIFRSobrevendido other)
	    {
	        return Id == other.Id;
	    }

	    public override int GetHashCode()
	    {
	        return Id;
	    }

	    public int Id { get; set; }

	    public double ValorMaximo { get; set; }


	    public cIFRSobrevendido(Int32 plngID, double pdblValorMaximo)
		{
			Id = plngID;
			ValorMaximo = pdblValorMaximo;

		}

		public override bool Equals(object obj)
		{
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((cIFRSobrevendido) obj);
		}

	    public override string ToString()
		{
			return Convert.ToString(ValorMaximo);
		}

	}
}
