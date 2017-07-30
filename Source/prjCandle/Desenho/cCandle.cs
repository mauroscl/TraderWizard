using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using TraderWizard.Enumeracoes;

namespace prjCandle
{

	public class cCandle
	{
	    //utiliza quando o gráfico é semanal

	    //L = LINHA
		//R = RETANGULO

	    //RETANGULO CONTENDO O CORPO DO CANDLE

	    //objeto pen que contém a cor das linha da borda ou da cauda do candle. Por padrão será preto.

		private readonly Pen _linePen;
		//PONTOS X, Y DA LINHA HORIZONTAL DO CORPO, QUANDO VALOR ABERTURA E FECHAMENTO FOREM IGUAIS

	    //PONTOS X, Y DA LINHA VERTICAL DA CAUDA DO CANDLE

	    //retangulo contendo a área total do candle: corpo + cauda

	    //VALORES DO CANDLE

	    private Dictionary <string, cEstrutura.structMediaValor> _medias;

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
		public string CorpoTipo { get; set; }


	    public Rectangle RectCorpo { get; set; }

	    public Brush CorpoCor { get; set; }

	    public int LinhaCorpoX1 { get; set; }

	    public int LinhaCorpoY { get; set; }

	    public int LinhaCorpoX2 { get; set; }

	    public Rectangle RectAreaTotal { get; set; }

	    public int LinhaCaudaX { get; set; }

	    public int LinhaCaudaY1 { get; set; }

	    public int LinhaCaudaY2 { get; set; }

	    public DateTime Data { get; }

	    public DateTime DataFinal { get; set; }

	    public decimal ValorAbertura { get; }

	    public decimal ValorFechamento { get; }

	    public decimal ValorMinimo { get; }

	    public decimal ValorMaximo { get; }

	    public decimal Oscilacao { get; }

	    public double IFR14 { get; set; } = -1;

	    public double Volume { get; set; }

	    public double VolumeMedio { get; set; }

        public Dictionary<string, cEstrutura.structMediaValor> GetMedia()
        {
            return _medias;
        }

        //public double IFRMedio { get; set; } = -1;

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


		public cCandle(DateTime pdtmData, decimal pdecValorAbertura, decimal pdecValorFechamento, decimal pdecValorMaximo, decimal pdecValorMinimo, decimal pdecOscilacao, bool pblnMediaArmazenar)
		{
			Data = pdtmData;
			ValorAbertura = pdecValorAbertura;
			ValorFechamento = pdecValorFechamento;
			ValorMinimo = pdecValorMinimo;
			ValorMaximo = pdecValorMaximo;
			//dblIFR14 = pdblIFR14
			Oscilacao = pdecOscilacao;


			if (pblnMediaArmazenar) {
                _medias = new Dictionary<string, cEstrutura.structMediaValor>();

			}

			_linePen = new Pen(Color.Black);

		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name=" pobjStructMediaValor"></param>
	    /// <param name="pobjStructMediaValor"></param>
	    /// estrutura contendo o período, o tipo, e o valor da média.
	    /// E = EXPONENCIAL
	    /// A = ARITMÉTICA
	    /// <returns></returns>
	    /// <remarks></remarks>
	    public bool MediaAtribuir( cEstrutura.structMediaValor pobjStructMediaValor)
		{
			bool functionReturnValue;


			try {

				if (_medias == null) {
                    _medias = new Dictionary<string,cEstrutura.structMediaValor>();

				}

				//o número de períodos e o tipo de média formam a chave da collection
                _medias.Add(pobjStructMediaValor.intPeriodo + pobjStructMediaValor.strTipo, pobjStructMediaValor);

				functionReturnValue = true;

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;
			}
			return functionReturnValue;

		}

		public bool MediaRemover(string pstrKey)
		{
			bool functionReturnValue;


			try {
				//o número de períodos e o tipo de média formam a chave da collection
				if (_medias.Count > 1) {
					_medias.Remove(pstrKey);
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
				_medias.Clear();
			    functionReturnValue = true;

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return functionReturnValue;

		}


		public double MediaBuscar(int pintNumPeriodos, string pstrMediaTipo)
		{
			double functionReturnValue = 0;


			try
			{
			    functionReturnValue = _medias.Count > 0 ? _medias[pintNumPeriodos + pstrMediaTipo].dblValor : 0;
			} catch (Exception) {
				//MsgBox(ex.Message, MsgBoxStyle.Critical, "Trader Wizard")
				functionReturnValue = 0;

			}
			return functionReturnValue;

		}

        
		//public override void Desenhar(PaintEventArgs e)
        public void Desenhar(Graphics pobjGraphics)
		{
			//primeiro tem que desenhar a linha da cauda do candle
			pobjGraphics.DrawLine(_linePen, LinhaCaudaX, LinhaCaudaY1, LinhaCaudaX, LinhaCaudaY2);

			//para desenhar o corpo do candle tem que saber se o corpo é um retângulo ou uma linha.

			if (CorpoTipo == "R") {
				//preenche a cor do candle de acordo com os valores de abertura e fechamento
				pobjGraphics.FillRectangle(CorpoCor, RectCorpo);

				//desenha o corpo do candle.
				pobjGraphics.DrawRectangle(_linePen, RectCorpo);


			} else if (CorpoTipo == "L") {
				//se é uma linha desenha a linha
				pobjGraphics.DrawLine(_linePen, LinhaCorpoX1, LinhaCorpoY, LinhaCorpoX2, LinhaCorpoY);

			}

		}

		public int CoordenadaX1 => CorpoTipo == "R" ? RectCorpo.X : LinhaCorpoX1;
	}
}
