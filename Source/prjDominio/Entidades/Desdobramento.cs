using System;

namespace Dominio.Entidades
{

	public abstract class Desdobramento
	{
	    private readonly DateTime dtmData;
		private readonly double dblNumeradorDaConversao;

		private readonly double dblDenominadorDaConversao;
		private readonly double dblRazao;
		private readonly double dblRazaoInvertida;
		protected abstract bool AplicarNoVolume { get; }


	    protected Desdobramento(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao)
		{
		    dtmData = pdtmData;
			dblNumeradorDaConversao = pdblNumeradorDaConversao;
			dblDenominadorDaConversao = pdblDenominadorDaConversao;

			dblRazao = dblNumeradorDaConversao / dblDenominadorDaConversao;
			dblRazaoInvertida = dblDenominadorDaConversao / dblNumeradorDaConversao;

		}

		public DateTime Data {
			get { return dtmData; }
		}

		public double Razao {
			get { return dblRazao; }
		}

		public override string ToString()
		{
			return dblNumeradorDaConversao.ToString() + "/" + dblDenominadorDaConversao.ToString();
		}


		public void ConverterCotacao(CotacaoDiaria pobjCotacaoOriginal)
		{
			if (pobjCotacaoOriginal.Data < dtmData) {
				//se a data do valor original é anterior à data do split tem que multiplica pela razão
				pobjCotacaoOriginal.Converter(dblRazao);
			} else if (pobjCotacaoOriginal.Data > dtmData) {
				//se a data do valor original é maior do que a data do split tem que multiplica pela razão invertida
				pobjCotacaoOriginal.Converter(dblRazaoInvertida);
			}

		}

		//TODO: Criar método para converter volume. Tem que considerar a propriedade "AplicarNoVolume"

	}

	public class cSplit_Grupammento : Desdobramento
	{

		public cSplit_Grupammento(Ativo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return true; }
		}
	}

	public class cJurosSobreCapitalProprio : Desdobramento
	{

		public cJurosSobreCapitalProprio(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cDividendo : Desdobramento
	{

		public cDividendo(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cCisao : Desdobramento
	{

		public cCisao(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cRendimento : Desdobramento
	{

		public cRendimento(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cRCDIN : Desdobramento
	{

		public cRCDIN(DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}



}
