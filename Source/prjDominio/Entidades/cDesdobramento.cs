using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo.Entidades
{

	public abstract class cDesdobramento
	{

		private readonly cAtivo objAtivo;
		private readonly DateTime dtmData;
		private readonly double dblNumeradorDaConversao;

		private readonly double dblDenominadorDaConversao;
		private readonly double dblRazao;
		private readonly double dblRazaoInvertida;
		protected abstract bool AplicarNoVolume { get; }


		public cDesdobramento(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao)
		{
			objAtivo = pobjAtivo;
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


		public void ConverterCotacao(cCotacaoDiaria pobjCotacaoOriginal)
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

	public class cSplit_Grupammento : cDesdobramento
	{

		public cSplit_Grupammento(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return true; }
		}
	}

	public class cJurosSobreCapitalProprio : cDesdobramento
	{

		public cJurosSobreCapitalProprio(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cDividendo : cDesdobramento
	{

		public cDividendo(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cCisao : cDesdobramento
	{

		public cCisao(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cRendimento : cDesdobramento
	{

		public cRendimento(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}

	public class cRCDIN : cDesdobramento
	{

		public cRCDIN(cAtivo pobjAtivo, DateTime pdtmData, double pdblNumeradorDaConversao, double pdblDenominadorDaConversao) : base(pobjAtivo, pdtmData, pdblNumeradorDaConversao, pdblDenominadorDaConversao)
		{
		}

		protected override bool AplicarNoVolume {
			get { return false; }
		}
	}



}
