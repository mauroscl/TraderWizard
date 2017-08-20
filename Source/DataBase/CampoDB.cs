namespace DataBase
{
	public class CampoDb
	{
	    public string Campo { get; }

	    public bool Chave { get; }

	    public string Valor { get; set; }


	    public CampoDb(string pstrCampo, bool pblnChave, string pstrValor)
		{
			Campo = pstrCampo;

			Chave = pblnChave;

			Valor = pstrValor;

		}

	}
}
