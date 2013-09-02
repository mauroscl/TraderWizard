using System;
using prjDominio.Entidades;

namespace prjModelo.Entidades
{

	public class cIFRSimulacaoDiariaFaixaResumo
	{

		public cAtivo Ativo { get; private set; }
        public Setup Setup { get; private set; }
        public cClassifMedia ClassificacaoDaMedia { get; private set; }
        public cIFRSobrevendido IfrSobrevendido { get; private set; }
		private int intNumTradesSemFiltro;
		private int intNumAcertosSemFiltro;
		public double PercentualAcertosSemFiltro { get; private set; }
        private int intNumTradesComFiltro;
		private int intNumAcertosComFiltro;
		public DateTime Data { get; set; }
		public double PercentualAcertosComFiltro { get; set; }


		public cIFRSimulacaoDiariaFaixaResumo(cAtivo pobjAtivo, Setup pobjSetup, cClassifMedia pobjCM, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{
			Ativo = pobjAtivo;
			Setup = pobjSetup;
			ClassificacaoDaMedia = pobjCM;
			IfrSobrevendido = pobjIFRSobrevendido;
			Data = pdtmData;

		}
		public int NumTradesSemFiltro {
			get { return intNumTradesSemFiltro; }
			set {
				intNumTradesSemFiltro = value;
				CalcularPercentualAcertosSemFiltro();
			}
		}

		public int NumAcertosSemFiltro {
			get { return intNumAcertosSemFiltro; }
			set {
				intNumAcertosSemFiltro = value;
				CalcularPercentualAcertosSemFiltro();
			}
		}

		public int NumTradesComFiltro {
			get { return intNumTradesComFiltro; }
			set {
				intNumTradesComFiltro = value;
				CalcularPercentualAcertosComFiltro();
			}
		}

		public int NumAcertosComFiltro {
			get { return intNumAcertosComFiltro; }
			set {
				intNumAcertosComFiltro = value;
				CalcularPercentualAcertosComFiltro();
			}
		}


		private void CalcularPercentualAcertosSemFiltro()
		{
			if (intNumTradesSemFiltro != 0) {
				PercentualAcertosSemFiltro = intNumAcertosSemFiltro / intNumTradesSemFiltro * 100;
			} else {
				PercentualAcertosSemFiltro = 0;
			}
		}

		private void CalcularPercentualAcertosComFiltro()
		{
			if (intNumTradesComFiltro != 0) {
				PercentualAcertosComFiltro = intNumAcertosComFiltro / intNumTradesComFiltro * 100;
			} else {
				PercentualAcertosComFiltro = 0;
			}
		}


	}
}
