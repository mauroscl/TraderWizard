using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDominio.Entidades;
using prjModelo.Entidades;
using prjModelo.ValueObjects;

namespace prjModelo.VOBuilders
{
	public class cValorCriterioClassifMediaVOBuilder
	{

		public cValorCriterioClassifMediaVO Build(cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{


			cValorCriterioClassifMediaVO objRetorno = new cValorCriterioClassifMediaVO();

			objRetorno.PercentualMM21 = Convert.ToDouble( pobjIFRSimulacaoDiaria.PercentualMME21);
			objRetorno.PercentualMM49 = Convert.ToDouble( pobjIFRSimulacaoDiaria.PercentualMME49);
			objRetorno.PercentualMM200 = Convert.ToDouble( pobjIFRSimulacaoDiaria.PercentualMME200);
			objRetorno.PercentualMM200MM21 = Convert.ToDouble( pobjIFRSimulacaoDiaria.PercentualMME200MME21);
			objRetorno.PercentualMM200MM49 = Convert.ToDouble( pobjIFRSimulacaoDiaria.PercentualMME200MME49);

			return objRetorno;

		}

	}
}
