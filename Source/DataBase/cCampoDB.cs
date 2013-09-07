namespace DataBase
{
	public class cCampoDB
	{
	    public string Campo { get; private set; }

	    public bool Chave { get; private set; }

	    public string Valor { get; set; }


	    public cCampoDB(string pstrCampo, bool pblnChave, string pstrValor)
		{
			Campo = pstrCampo;

			Chave = pblnChave;

			Valor = pstrValor;

		}

	}
}
