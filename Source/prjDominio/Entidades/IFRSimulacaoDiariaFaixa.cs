using System;

namespace Dominio.Entidades
{

	public class IFRSimulacaoDiariaFaixa
	{
	    public string Codigo { get; private set; }
        public ClassifMedia ClassificacaoDaMedia { get; private set; }
        public CriterioClassifMedia CriterioDeClassificacaoDaMedia { get; private set; }
        public Setup Setup { get; private set; }
        public IFRSobrevendido IfrSobrevendido { get; private set; }
	    public DateTime Data { get; set; }

        public long? Id { get; private set; }

        public double ValorMinimo { get; private set; }

        public double ValorMaximo { get; private set; }

        public int NumTentativasMinimo { get; private set; }
        
        public IFRSimulacaoDiariaFaixa(string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, CriterioClassifMedia pobjCriterioDeClassificacaoDaMedia, IFRSobrevendido pobjIFRSobrevendido, System.DateTime pdtmData, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
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


		public IFRSimulacaoDiariaFaixa(long plngID, string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, CriterioClassifMedia pobjCriterioDeClassificacaoDaMedia, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
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
