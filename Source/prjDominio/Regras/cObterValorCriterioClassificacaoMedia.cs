using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using prjModelo.ValueObjects;

namespace prjModelo.Regras
{

	public class cObterValorCriterioClassificacaoMedia
	{

		public static double ObterValor(cValorCriterioClassifMediaVO pobjValorCriterioClassifMediaVO, cCriterioClassifMedia pobjCriterioClassifMedia)
		{

			if (pobjCriterioClassifMedia is cCriterioClassifMediaMM21) {
				return pobjValorCriterioClassifMediaVO.PercentualMM21;
			} else if (pobjCriterioClassifMedia is cCriterioClassifMediaMM49) {
				return pobjValorCriterioClassifMediaVO.PercentualMM49;
			} else if (pobjCriterioClassifMedia is cCriterioClassifMediaMM200) {
				return pobjValorCriterioClassifMediaVO.PercentualMM200;
			} else if (pobjCriterioClassifMedia is cCriterioClassifMediaDifMM200MM49) {
				return pobjValorCriterioClassifMediaVO.PercentualMM200MM49;
			} else if (pobjCriterioClassifMedia is cCriterioClassifMediaDifMM200MM21) {
				return pobjValorCriterioClassifMediaVO.PercentualMM200MM21;

			} else {
				throw new Exception("Classificação de média indeterminada");

			}

		}



	}
}
