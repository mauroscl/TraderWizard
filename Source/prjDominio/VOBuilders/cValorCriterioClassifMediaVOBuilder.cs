using System;
using prjDominio.Entidades;
using prjDominio.ValueObjects;

namespace prjDominio.VOBuilders
{
	public class cValorCriterioClassifMediaVOBuilder
	{

		public cValorCriterioClassifMediaVO Build(cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{


			var objRetorno = new cValorCriterioClassifMediaVO
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
