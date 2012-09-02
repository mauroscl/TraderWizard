using System;

namespace prjDTO
{
	public class cSugerirAtualizacaoCotacaoDTO
	{
	    public cSugerirAtualizacaoCotacaoDTO(string pstrTipo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal)
		{
			Tipo = pstrTipo;
			DataInicial = pdtmDataInicial;
			DataFinal = pdtmDataFinal;
		}

	    public string Tipo { get; private set; }

	    public DateTime DataInicial { get; private set; }

	    public DateTime DataFinal { get; private set; }


	}
}
