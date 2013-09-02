namespace TraderWizard.Enumeracoes
{

	public class cEstrutura
	{

		//estrutura que contém a data e o valor de uma média móvel na respectiva data
		//Public Structure structMMExpValor

		//    Dim dtmData As Date
		//    Dim dblMedia As Double

		//End Structure

		//estrutura utilizada para armazenar  a escolha de indicadores pelo usuário.
		//O usuário escolhe o período utilizado no cálculo do indicador e a cor que será 
		//utilizada para desenhar o indicador neste período.
		//Inicialmente será utilizado para indicadores de média e IFR.
		public struct structIndicadorEscolha
		{

			public int intPeriodo;
				//MME = MÉDIA MÓVEL EXPONENCIAL; MMA = MÉDIA MÓVEL ARITMÉTICA
			public string strTipo;
			public System.Drawing.Color objCor;
				//NÚMERO DE REGISTROS COM VALOR MAIOR DO QUE ZERO EM UM DETERMINADO REGISTRO. 
			public int intNumRegistros;
				//VALOR = VALOR DAS COTAÇÕES; VOLUME = VOLUME DAS COTAÇÕES
			public string strDado;

		}

		/// <summary>
		/// Estrutura utilizada para armazenar as principais propriedades do indicador MEDIA
		/// </summary>
		/// <remarks></remarks>
		public struct structMediaValor
		{

			//NÚMERO DE PERÍODOS UTILIZADOS NO CÁLCULO.
			public int intPeriodo;
			//E = EXPONENCIAL; A = ARITMÉTICA
			public string strTipo;
			//MÉDIA CALCULADA

			public double dblValor;
		}


		public struct structBackTestSetup
		{

			//CÓDIGO DO SETUP

			public string strCodigoSetup;
			//tipo de realizalção parcial

			public cEnum.enumRealizacaoParcialTipo intRealizacaoParcialTipo;
			//Valor máximo para considerar o IFR 2 sobrevendido.

			public double dblIFR2SobrevendidoValorMaximo;
			//quando a realização parcial for um percentual fixo, determina o valor do percentual

			public decimal decPercentualRealizacaoParcialFixo;
			//quando a realização parcial for no primeiro fechamento com um percentual mínimo, indica este percentual

			public decimal decPrimeiroLucroPercentualMinimo;
			//indica se é para filtrar os preços pela MME 49.

			public bool blnMME49Filtrar;
		}

	}
}
