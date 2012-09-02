using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using frwInterface;
namespace prjCandle
{

	public class cCandle
	{


		private DateTime dtmData;
		//utiliza quando o gráfico é semanal

		private System.DateTime dtmDataFinal;
		//L = LINHA
		//R = RETANGULO

		private string strCorpoTipo;
		//RETANGULO CONTENDO O CORPO DO CANDLE

		private Rectangle objRectCorpo;

		Brush objCorpoCor;
		//objeto pen que contém a cor das linha da borda ou da cauda do candle. Por padrão será preto.

		private Pen objLinePen;
		//PONTOS X, Y DA LINHA HORIZONTAL DO CORPO, QUANDO VALOR ABERTURA E FECHAMENTO FOREM IGUAIS
		private int intLinhaCorpoX1;
		private int intLinhaCorpoY;

		private int intLinhaCorpoX2;
		//PONTOS X, Y DA LINHA VERTICAL DA CAUDA DO CANDLE
		private int intLinhaCaudaX;
		private int intLinhaCaudaY1;

		private int intLinhaCaudaY2;
		//retangulo contendo a área total do candle: corpo + cauda

		private Rectangle objRectAreaTotal;
		//VALORES DO CANDLE
		private decimal decValorAbertura;
		private decimal decValorFechamento;
		private decimal decValorMinimo;
		private decimal decValorMaximo;
		private decimal decOscilacao;
		private double dblIFR14 = -1;
		private double dblVolume;

		private double dblVolumeMedio;

		private Dictionary <string, cEstrutura.structMediaValor> colMedia;

		private double dblIFRMedio = -1;
		//PROPERTY GET E PROPERTY SET

		/// <summary>
		/// Indica o tipo de corpo do candle. Uma linha quando valor de abertura é igual ao valor de fechamento,
		/// ou um retângulo    
		/// </summary>
		/// <value>
		/// R = RETÂNGULO
		/// L = LINHA    
		/// </value>
		/// <remarks></remarks>
		public string CorpoTipo {
			get { return strCorpoTipo; }
			set { strCorpoTipo = value; }
		}


		public Rectangle RectCorpo {
			get { return objRectCorpo; }
			set { objRectCorpo = value; }
		}

		public Brush CorpoCor {
			set { objCorpoCor = value; }
		}

		public int LinhaCorpoX1 {
			get { return intLinhaCorpoX1; }
			set { intLinhaCorpoX1 = value; }
		}

		public int LinhaCorpoY {
			set { intLinhaCorpoY = value; }
		}

		public int LinhaCorpoX2 {
			set { intLinhaCorpoX2 = value; }
		}

		public Rectangle RectAreaTotal {
			get { return objRectAreaTotal; }
			set { objRectAreaTotal = value; }
		}

		public int LinhaCaudaX {

			get { return intLinhaCaudaX; }
			set { intLinhaCaudaX = value; }
		}

		public int LinhaCaudaY1 {
			set { intLinhaCaudaY1 = value; }
		}

		public int LinhaCaudaY2 {
			set { intLinhaCaudaY2 = value; }
		}

		public System.DateTime Data {
			get { return dtmData; }
		}

		public DateTime DataFinal {
			get { return dtmDataFinal; }
			set { dtmDataFinal = value; }
		}

		public decimal ValorAbertura {
			get { return decValorAbertura; }
		}

		public decimal ValorFechamento {
			get { return decValorFechamento; }
		}

		public decimal ValorMinimo {
			get { return decValorMinimo; }
		}

		public decimal ValorMaximo {
			get { return decValorMaximo; }
		}

		public decimal Oscilacao {
			get { return decOscilacao; }
		}

		public double IFR14 {
			get { return dblIFR14; }
			set { dblIFR14 = value; }
		}

		public double Volume {
			get { return dblVolume; }
			set { dblVolume = value; }
		}

		public double VolumeMedio {
			get { return dblVolumeMedio; }
			set { dblVolumeMedio = value; }
		}

		public Dictionary<string, cEstrutura.structMediaValor> Media {
			get { return colMedia; }
		}

		public double IFRMedio {
			get { return dblIFRMedio; }
			set { dblIFRMedio = value; }
		}

		//Public Sub New(ByVal pdtmData As Date, ByVal pdecValorAbertura As Decimal, ByVal pdecValorFechamento As Decimal _
		//, ByVal pdecValorMaximo As Decimal, ByVal pdecValorMinimo As Decimal _
		//, ByVal pdblIFR14 As Double, ByVal pdecOscilacao As Decimal, ByVal pblnMediaArmazenar As Boolean)

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdtmData"></param>
		/// <param name="pdecValorAbertura"></param>
		/// <param name="pdecValorFechamento"></param>
		/// <param name="pdecValorMaximo"></param>
		/// <param name="pdecValorMinimo"></param>
		/// <param name="pdecOscilacao"></param>
		/// <param name="pblnMediaArmazenar">Indica se alguma média será mostrada no gráfico</param>
		/// <remarks></remarks>


		public cCandle(System.DateTime pdtmData, decimal pdecValorAbertura, decimal pdecValorFechamento, decimal pdecValorMaximo, decimal pdecValorMinimo, decimal pdecOscilacao, bool pblnMediaArmazenar)
		{
			dtmData = pdtmData;
			decValorAbertura = pdecValorAbertura;
			decValorFechamento = pdecValorFechamento;
			decValorMinimo = pdecValorMinimo;
			decValorMaximo = pdecValorMaximo;
			//dblIFR14 = pdblIFR14
			decOscilacao = pdecOscilacao;


			if (pblnMediaArmazenar) {
                colMedia = new Dictionary<string, cEstrutura.structMediaValor>();

			}

			objLinePen = new Pen(Color.Black);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name=" pobjStructMediaValor"></param>
		/// estrutura contendo o período, o tipo, e o valor da média.
		/// E = EXPONENCIAL
		/// A = ARITMÉTICA
		/// <returns></returns>
		/// <remarks></remarks>
		public bool MediaAtribuir( cEstrutura.structMediaValor pobjStructMediaValor)
		{
			bool functionReturnValue = false;


			try {

				if (colMedia == null) {
                    colMedia = new Dictionary<string,cEstrutura.structMediaValor>();

				}

				//o número de períodos e o tipo de média formam a chave da collection
                colMedia.Add(pobjStructMediaValor.intPeriodo.ToString() + pobjStructMediaValor.strTipo, pobjStructMediaValor);

				functionReturnValue = true;

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;
			}
			return functionReturnValue;

		}

		public bool MediaRemover(string pstrKey)
		{
			bool functionReturnValue = false;


			try {
				//o número de períodos e o tipo de média formam a chave da collection
				if (colMedia.Count > 1) {
					colMedia.Remove(pstrKey);
				}

				functionReturnValue = true;


			} catch (Exception ex) {
				//*****19/01/2010 - NÃO PRECISA EMITIR A MENSAGEM QUANDO DER ERRO,
				//*****PORQUE É NORMAL QUE ALGUNS CANDLES NÃO TENHAM AS MÉDIAS CALCULADAS
				//*****JÁ QUE OS PRIMEIROS N-1 CANDLES NÃO TEM A MÉDIA DE N DIAS CALCULADAS,
				//*****POIS SÃO NECESSÁRIO N PERÍODOS PARA PODER FAZER O CÁLCULO.

				//MsgBox(ex.Message, MsgBoxStyle.Critical, "Trader Wizard")

				functionReturnValue = false;
			}
			return functionReturnValue;

		}

		public bool MediaRemoverTodas()
		{
			bool functionReturnValue = false;


			try {
				colMedia.Clear();
			    functionReturnValue = true;

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return functionReturnValue;

		}


		public double MediaBuscar(int pintNumPeriodos, string pstrMediaTipo)
		{
			double functionReturnValue = 0;


			try {
				if (colMedia.Count > 0) {
					functionReturnValue = colMedia[pintNumPeriodos.ToString() + pstrMediaTipo].dblValor;
				} else {
					functionReturnValue = 0;
				}


			} catch (Exception ex) {
				//MsgBox(ex.Message, MsgBoxStyle.Critical, "Trader Wizard")
				functionReturnValue = 0;

			}
			return functionReturnValue;

		}

        
		//public override void Desenhar(PaintEventArgs e)
        public void Desenhar(Graphics pobjGraphics)
		{
			//primeiro tem que desenhar a linha da cauda do candle
			pobjGraphics.DrawLine(objLinePen, intLinhaCaudaX, intLinhaCaudaY1, intLinhaCaudaX, intLinhaCaudaY2);

			//para desenhar o corpo do candle tem que saber se o corpo é um retângulo ou uma linha.

			if (strCorpoTipo == "R") {
				//preenche a cor do candle de acordo com os valores de abertura e fechamento
				pobjGraphics.FillRectangle(objCorpoCor, objRectCorpo);

				//desenha o corpo do candle.
				pobjGraphics.DrawRectangle(objLinePen, objRectCorpo);


			} else if (strCorpoTipo == "L") {
				//se é uma linha desenha a linha
				pobjGraphics.DrawLine(objLinePen, intLinhaCorpoX1, intLinhaCorpoY, intLinhaCorpoX2, intLinhaCorpoY);

			}

		}

		public int CoordenadaX1
		{
            get
            {
                
			    int functionReturnValue = 0;

			    if (strCorpoTipo == "R") {
				    //se o corpo do candle é um retângulo
				    functionReturnValue = objRectCorpo.X;
			    } else {
				    //se o corpo do candle é uma linha, ou seja, valor abertura e valor fechamento são iguais.
				    functionReturnValue = intLinhaCorpoX1;
			    }
			    return functionReturnValue;

            }

		}

	}
}
