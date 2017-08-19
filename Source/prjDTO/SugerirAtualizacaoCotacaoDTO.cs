using System;

namespace DTO
{
	public class SugerirAtualizacaoCotacaoDTO
	{
	    public SugerirAtualizacaoCotacaoDTO(string pstrTipo, DateTime pdtmDataInicial, DateTime pdtmDataFinal)
		{
			Tipo = pstrTipo;
			DataInicial = pdtmDataInicial;
			DataFinal = pdtmDataFinal;
		}

	    public string Tipo { get; }

	    public DateTime DataInicial { get; }

	    public DateTime DataFinal { get; }


	}
}
