using System;
using prjDominio.Entidades;

namespace prjDominio.ValueObjects
{
	public class cCalculoFaixaResumoVO
	{

		private readonly DateTime dtmDataSaida;

		private readonly cClassifMedia objClassifMedia;
		public DateTime DataSaida {
			get { return dtmDataSaida; }
		}

		public cClassifMedia ClassifMedia {
			get { return objClassifMedia; }
		}

		public double ValorMenorIFR { get; set; }

		public cCalculoFaixaResumoVO(System.DateTime pdtmDataSaida, double pdblValorMenorIFR, cClassifMedia pobjClassifMedia)
		{
			dtmDataSaida = pdtmDataSaida;
			ValorMenorIFR = pdblValorMenorIFR;
			objClassifMedia = pobjClassifMedia;
		}

		public override bool Equals(object obj)
		{
			var objCalculadorFaixaResumoVO = (cCalculoFaixaResumoVO)obj;
			return (objCalculadorFaixaResumoVO.DataSaida == dtmDataSaida && objCalculadorFaixaResumoVO.ClassifMedia.Equals(objClassifMedia));
		}


	}
}
