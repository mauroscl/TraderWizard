using System;
using Dominio.Entidades;

namespace Dominio.ValueObjects
{
	public class CalculoFaixaResumo
	{
	    public DateTime DataSaida { get; }

	    public ClassifMedia ClassifMedia { get; }

	    public double ValorMenorIFR { get; set; }

		public CalculoFaixaResumo(DateTime pdtmDataSaida, double pdblValorMenorIFR, ClassifMedia pobjClassifMedia)
		{
			DataSaida = pdtmDataSaida;
			ValorMenorIFR = pdblValorMenorIFR;
			ClassifMedia = pobjClassifMedia;
		}

    }
}

