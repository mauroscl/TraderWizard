using System;

namespace prjDominio.Entidades
{
	public class cAtivo
    {
        #region Equality Members
        protected bool Equals(cAtivo other)
	    {
	        return string.Equals(Codigo, other.Codigo);
	    }

	    public override int GetHashCode()
	    {
	        return Codigo.GetHashCode();
	    }


#endregion

	    public cAtivo(string pstrCodigo)
		{
			Codigo = pstrCodigo;
		}

		public cAtivo(string pstrCodigo, string pstrDescricao)
		{
			Codigo = pstrCodigo;
			Descricao = pstrDescricao;
		}

        protected cAtivo(){}

	    public virtual string Descricao { get; protected  internal set; }
        public virtual string Codigo { get; protected  internal set; }

	    public override string ToString()
		{
			return Codigo + " - " + Descricao;
		}


	}

}
