namespace prjDTO
{
	public class InformacoesDoTradeDTO
	{

		//Public Property ValorDoStopLossInicial As Decimal
		public decimal ValorDeEntradaOriginal { get; set; }
		public decimal ValorDoStopLoss { get; set; }
		public decimal ValorRealizacaoParcial { get; set; }
		public decimal ValorMaximo { get; set; }
		//Indica se o IFR já cruzou a média para cima. Utilizado para controlar o stop.
		//Quando o setup for IFR com filtro (1.2 ou 2.2) esta variável tem que ser TRUE, porque 
		//é justamente o cruzamento do IFR com a média que gera entrada pelo setup com filtro.
		//A necessidade de existir esta variável é o IFR sem filtro, para que o stop não seja subido
		//indevidamente antes do IFR cruzar a média para cima pelo menos uma vez.
		public bool IFRCruzouMediaParaCima { get; set; }

		public double? ValorIFRMinimo { get; set; }
		public decimal? ValorFechamentoMinimo { get; set; }
		public double? MME21Minima { get; set; }
		public double? MME49Minima { get; set; }

		public bool PermitiuRealizarParcial {
			get { return ValorMaximo >= ValorRealizacaoParcial; }
		}

	}
}
