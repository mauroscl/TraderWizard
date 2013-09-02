using System;
using System.Diagnostics;
using System.Drawing;
using TraderWizard.Enumeracoes;

namespace prjCandle
{

	public abstract class Linha: Desenho
	{

        protected Linha(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho, bool imprimeLabel)
            :base(pontoInicial,pontoFinal, areaDeDesenho)
        {
            _imprimeLabel = imprimeLabel;
            Debug.Print("Construtor da Linha - P1({0},{1}) - P2({2},{3})", pontoInicial.Ponto.X, pontoInicial.Ponto.Y, pontoFinal.Ponto.X, pontoFinal.Ponto.Y);

        }

        protected Linha(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho, bool imprimeLabel, string labelFixo)
            : this(pontoInicial, pontoFinal,areaDeDesenho,imprimeLabel)
        {
            _labelFixo = labelFixo;
        }
            
	    private readonly bool _imprimeLabel;
	    private readonly string _labelFixo;

        public override void AtualizarPontos(int novaCoordenadaXDoPontoInicial, int novaCoordenadaXDoPontoFinal)
        {
            PontoInicial.AlterarPonto(novaCoordenadaXDoPontoInicial, AreaDeDesenho.CalculaCoordenadaYPeloValor(PontoInicial.ValorEmMoeda));

            PontoFinal.AlterarPonto(novaCoordenadaXDoPontoFinal, AreaDeDesenho.CalculaCoordenadaYPeloValor(PontoFinal.ValorEmMoeda));
        }

        public override void AlterarPontoFinal(PontoDoDesenho novoPontoFinal)
        {
            PontoFinal = novoPontoFinal;
            PontoFinal.ValorEmMoeda = AreaDeDesenho.CalcularValorDoPontoEmModa(novoPontoFinal.Ponto);
        }


	    public override void Desenhar(Graphics pobjGraphics)
		{
			if (!AreaDeDesenho.Contem(this)) {
				return;
			}

	        //calcula a coordenada do label
			int coordenadaXDoLabel = SentidoHorizontal == cEnum.SentidaHorizontalDaLinha.EsquerdaParaDireita ? PontoInicial.Ponto.X : PontoFinal.Ponto.X;

		    string strRetaTexto = String.Format("{0:0.00}", PontoFinal.ValorEmMoeda) + (string.IsNullOrEmpty(_labelFixo)? "":" - "  + _labelFixo); 

			//desenha a linha
			pobjGraphics.DrawLine(Pens.Blue, PontoInicial.Ponto, PontoFinal.Ponto);
            Debug.Print("Desenhando Linha - P1({0},{1}) - P2({2},{3})", PontoInicial.Ponto.X, PontoInicial.Ponto.Y, PontoFinal.Ponto.X, PontoFinal.Ponto.Y);

			//desenha o texto da linha.
	        if (_imprimeLabel)
	        {
                pobjGraphics.DrawString(strRetaTexto, SystemFonts.DefaultFont, Brushes.Blue, new PointF(coordenadaXDoLabel, PontoFinal.Ponto.Y - 13));
	            
	        }

		}


	}
}
