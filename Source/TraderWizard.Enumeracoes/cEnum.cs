using System.ComponentModel;

namespace TraderWizard.Enumeracoes
{

	public class cEnum
	{

		public enum enumRetorno
		{

			RetornoErroInesperado = 0,
			RetornoOK = 1,
			RetornoErro2 = 2,
			RetornoErro3 = 3,
			RetornoErro4 = 4,
			RetornoErro5 = 5,
			RetornoErro6 = 6,
			RetornoErro7 = 7,
			RetornoErro8 = 8

		}

		public enum enumTelaEstado
		{

			EstadoEDICAO = 1,
			EstadoCONSULTA = 2

		}

		public enum enumRealizacaoParcialTipo
		{
			SemRealizacaoParcial = 0,
			PrimeiroLucro = 1,
			AlijamentoRisco = 2,
			PercentualFixo = 3
		}

		public enum enumProventoTipo
		{
			Dividendo = 1,
			JurosCapitalProprio = 2,
			Rendimento = 3,
			RestCapDin = 4
		}

		public enum enumMediaTipo
		{
			[Description("Aritmética")]
			Aritmetica = 1,
			[Description("Exponencial")]
			Exponencial = 2
		}

		public enum enumIFRTipo
		{
			SemFiltro = 0,
			ComFiltro = 1
		}

		public enum enumSetup
		{
			IFRSemFiltro = 1,
			IFRComFiltro = 2,
			MM91 = 3,
			MM92 = 4,
			MM93 = 5,
			CandleForaBB = 6,
			FFFD = 7,
			IFRSemFiltroRP = 10

		}

		public enum enumClassifMedia
		{

			AltaAlinha = 1,
			PrimAltaSecBaixa = 2,
			PrimBaixaSecAlta = 3,
			BaixaAlinhada = 4,
			BaixaDesalinhada = 5,
			AltaDesalinhada = 6,
			Indefinida = 7

		}

		public enum enumCriterioClassificacaoMedia
		{

			PercentualMM21 = 1,
			PercentualMM49 = 2,
			PercentualMM200 = 3,
			PercentualDiferencaMM200MM21 = 4,
			PercentualDiferencaMM200MM49 = 5

		}

		public enum Periodicidade
		{
			Diario = 1,
			Semanal = 2
		}

        public enum Escala
        {
            Aritmetica,
            Logaritmica
        }

        public enum SentidaHorizontalDaLinha
        {
            EsquerdaParaDireita,
            DireitaParaEsquerda,
            Indefinido
        }

        public enum EstadoDoDesenho
        {
            NaoIniciado,
            EmAndamento,
            Completo,
            Concluido
        }

        public enum EventoDoMouse
        {
            Click,
            Move
        }

	    public enum BancoDeDados
	    {
	        SqlServer,
            Access
	    }
           

	}
}


