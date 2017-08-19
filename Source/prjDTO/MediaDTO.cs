using System;
using System.Drawing;
namespace DTO
{

	public class MediaDTO
	{

		public MediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado)
		{
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;
			Dado = pstrDado;
		}


		public MediaDTO(string pstrTipo, int pintNumPeriodos)
		{
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;

		}

		public MediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado, int pintNumRegistros)
		{
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;
			Dado = pstrDado;
			NumRegistros = pintNumRegistros;
		}

		public MediaDTO(string pstrTipo, int pintNumPeriodos, string pstrDado, Color pobjCor)
		{
			Tipo = pstrTipo;
			NumPeriodos = pintNumPeriodos;
			Dado = pstrDado;
			Cor = pobjCor;
		}

		// tipo da média
	    //número de períodos utilizado no cálculo da média
	    //número de registros  para a média encontrado em determinado período.
	    //dado utilizado no cálculo da média: "VALOR" OU "VOLUME"

	    public string Tipo { get; }

	    public int NumPeriodos { get; }

	    public int NumRegistros { get; private set; }

	    public string Dado { get; }

	    public Color Cor { get; }

	    public void IncrementaNumRegistros(int pintIncremento)
		{
			NumRegistros = NumRegistros + pintIncremento;
		}

		public string CampoTipoBd {
			get {
				string campoBd;
				if (Tipo == "E") {
					campoBd = "MME";
				} else {
					switch (Dado) {
						case "VALOR":
							campoBd = "MMA";
							break;
						case "VOLUME":
							campoBd = "VMA";
							break;
						case "IFR2":
							campoBd = "IFR2";
							break;
						default:
							campoBd = String.Empty;
							break;
					}
				}

				return campoBd;

			}
		}


		public string GetAlias => CampoTipoBd + NumPeriodos;

        #region equality members
	    protected bool Equals(MediaDTO other)
	    {
	        return string.Equals(Tipo, other.Tipo) && NumPeriodos == other.NumPeriodos && string.Equals(Dado, other.Dado);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((MediaDTO)obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Tipo != null ? Tipo.GetHashCode() : 0);
	            hashCode = (hashCode * 397) ^ NumPeriodos;
	            hashCode = (hashCode * 397) ^ (Dado != null ? Dado.GetHashCode() : 0);
	            return hashCode;
	        }
	    }


        #endregion
    }
}
