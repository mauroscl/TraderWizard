using System;

namespace frwTela
{
	public class cColuna
	{
	    public cColuna(string pstrNome, string pstrCaption, System.Type pobjTipo)
		{
			Nome = pstrNome;
			Caption = pstrCaption;
			Tipo = pobjTipo;

		}

	    public string Nome { get; private set; }

	    public string Caption { get; private set; }

	    public Type Tipo { get; private set; }
	}
}
