using System;
using Dominio.Entidades;

namespace prjDominio.ValueObjects
{
	public class CalculoFaixaResumoVO
	{

		private readonly DateTime dtmDataSaida;

		private readonly ClassifMedia objClassifMedia;
		public DateTime DataSaida {
			get { return dtmDataSaida; }
		}

		public ClassifMedia ClassifMedia {
			get { return objClassifMedia; }
		}

		public double ValorMenorIFR { get; set; }

		public CalculoFaixaResumoVO(System.DateTime pdtmDataSaida, double pdblValorMenorIFR, ClassifMedia pobjClassifMedia)
		{
			dtmDataSaida = pdtmDataSaida;
			ValorMenorIFR = pdblValorMenorIFR;
			objClassifMedia = pobjClassifMedia;
		}

		public override bool Equals(object obj)
		{
			var objCalculadorFaixaResumoVO = (CalculoFaixaResumoVO)obj;
			return (objCalculadorFaixaResumoVO.DataSaida == dtmDataSaida && objCalculadorFaixaResumoVO.ClassifMedia.Equals(objClassifMedia));
		}


	}
}
