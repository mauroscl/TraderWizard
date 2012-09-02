using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo.Entidades
{
	public class cIFRSobrevendido
	{
	    protected bool Equals(cIFRSobrevendido other)
	    {
	        return lngID == other.lngID;
	    }

	    public override int GetHashCode()
	    {
	        return lngID;
	    }

	    private Int32 lngID;

		private double dblValorMaximo;
		public Int32 ID {
			get { return lngID; }
			set { lngID = value; }
		}

		public double ValorMaximo {
			get { return dblValorMaximo; }
			set { dblValorMaximo = value; }
		}


		public cIFRSobrevendido(Int32 plngID, double pdblValorMaximo)
		{
			lngID = plngID;
			dblValorMaximo = pdblValorMaximo;

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
			return Convert.ToString(dblValorMaximo);
		}

	}
}
