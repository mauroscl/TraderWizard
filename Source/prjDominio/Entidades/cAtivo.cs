using System;

namespace prjDominio.Entidades
{
	public class cAtivo
	{

	    public cAtivo(string pstrCodigo)
		{
			Codigo = pstrCodigo;
		}

		public cAtivo(string pstrCodigo, string pstrDescricao)
		{
			Codigo = pstrCodigo;
			Descricao = pstrDescricao;
		}

	    public string Descricao { get; set; }
        public string Codigo { get; private set; }

        public override bool Equals(object obj)
		{
		    if (obj is DBNull)
		    {
		        return false;
		    }
		    var objAtivo = (cAtivo) obj;
		    return (Codigo == objAtivo.Codigo);
		}

	    public override string ToString()
		{
			return Codigo + " - " + Descricao;
		}


	}

}
