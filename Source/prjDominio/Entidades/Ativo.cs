namespace Dominio.Entidades
{
	public class Ativo
    {
        #region Equality Members
        protected bool Equals(Ativo other)
	    {
	        return string.Equals(Codigo, other.Codigo);
	    }

	    public override int GetHashCode()
	    {
	        return Codigo.GetHashCode();
	    }


#endregion

	    public Ativo(string pstrCodigo)
		{
			Codigo = pstrCodigo;
		}

		public Ativo(string pstrCodigo, string pstrDescricao)
		{
			Codigo = pstrCodigo;
			Descricao = pstrDescricao;
		}

        protected Ativo(){}

	    public virtual string Descricao { get; protected  internal set; }
        public virtual string Codigo { get; protected  internal set; }

	    public override string ToString()
		{
			return Codigo + " - " + Descricao;
		}


	}

}
