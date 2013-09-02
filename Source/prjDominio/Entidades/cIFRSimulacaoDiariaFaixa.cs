using System;
using prjModelo.Entidades;

namespace prjDominio.Entidades
{

	public class cIFRSimulacaoDiariaFaixa
	{
	    public string Codigo { get; private set; }
        public cClassifMedia ClassificacaoDaMedia { get; private set; }
        public cCriterioClassifMedia CriterioDeClassificacaoDaMedia { get; private set; }
        public Setup Setup { get; private set; }
        public cIFRSobrevendido IfrSobrevendido { get; private set; }
	    public DateTime Data { get; set; }

        public long? Id { get; private set; }

        public double ValorMinimo { get; private set; }

        public double ValorMaximo { get; private set; }

        public int NumTentativasMinimo { get; private set; }
        
        public cIFRSimulacaoDiariaFaixa(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioDeClassificacaoDaMedia, cIFRSobrevendido pobjIFRSobrevendido, System.DateTime pdtmData, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
		{
			Codigo = pstrCodigo;
			ClassificacaoDaMedia = pobjCM;
			Setup = pobjSetup;
			CriterioDeClassificacaoDaMedia = pobjCriterioDeClassificacaoDaMedia;
			IfrSobrevendido = pobjIFRSobrevendido;
			this.Data = pdtmData;

			ValorMinimo = pdblValorMinimo;
			ValorMaximo = pdblValorMaximo;

			NumTentativasMinimo = pintNumTentativasMinimo;

		}


		public cIFRSimulacaoDiariaFaixa(long plngID, string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioDeClassificacaoDaMedia, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
		{
			Id = plngID;
			Codigo = pstrCodigo;
			ClassificacaoDaMedia = pobjCM;
			Setup = pobjSetup;
			CriterioDeClassificacaoDaMedia = pobjCriterioDeClassificacaoDaMedia;
			ValorMinimo = pdblValorMinimo;
			ValorMaximo = pdblValorMaximo;

			NumTentativasMinimo = pintNumTentativasMinimo;

		}

	}
}
