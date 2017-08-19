using System;
using Dominio.Entidades;
using prjDominio.ValueObjects;

namespace prjDominio.VOBuilders
{
	public class ValorCriterioClassifMediaVOBuilder
	{

		public ValorCriterioClassifMediaVO Build(IFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{


			var objRetorno = new ValorCriterioClassifMediaVO
			{
			    PercentualMM21 = Convert.ToDouble(pobjIFRSimulacaoDiaria.PercentualMME21),
			    PercentualMM49 = Convert.ToDouble(pobjIFRSimulacaoDiaria.PercentualMME49),
			    PercentualMM200 = Convert.ToDouble(pobjIFRSimulacaoDiaria.PercentualMME200),
			    PercentualMM200MM21 = Convert.ToDouble(pobjIFRSimulacaoDiaria.PercentualMME200MME21),
			    PercentualMM200MM49 = Convert.ToDouble(pobjIFRSimulacaoDiaria.PercentualMME200MME49)
			};

		    return objRetorno;

		}

	}
}
