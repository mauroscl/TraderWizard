using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace prjCandle
{

	public class FibonacciRetracement: Desenho
	{

        public FibonacciRetracement(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
            :base(pontoInicial,pontoFinal, areaDeDesenho)
		{

            AdicionarRetracoes();
		}

	    private IList<LinhaHorizontal> _retracoes;

        private void AdicionaRetracao(int coordenadaY, string percentualDaRetracao)
        {
            _retracoes.Add(new LinhaHorizontal(new PontoDoDesenho(new Point(PontoInicial.Ponto.X, coordenadaY), PontoInicial.Indice)
                , new PontoDoDesenho(new Point(PontoFinal.Ponto.X, coordenadaY), PontoFinal.Indice), AreaDeDesenho, percentualDaRetracao));

            Debug.Print("Linha Adicionada - Y: {0}- Perc: {1}", coordenadaY,percentualDaRetracao);
            
        }

		private void AdicionarRetracoes()
		{
            _retracoes = new List<LinhaHorizontal>();
            float diferencaEmPixels = PontoFinal.Ponto.Y - PontoInicial.Ponto.Y;

            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0F), "0%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0.236F), "23,6%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0.382F), "38,2%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0.5F), "50%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0.618F), "61,8%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 0.768F), "76,8%");
            AdicionaRetracao((int)(PontoInicial.Ponto.Y + diferencaEmPixels * 1F), "100%");
		}


	    public override void AlterarPontoFinal(PontoDoDesenho novoPontoFinal)
	    {
	        novoPontoFinal.ValorEmMoeda = AreaDeDesenho.CalcularValorDoPontoEmModa(novoPontoFinal.Ponto);
	        PontoFinal = novoPontoFinal;
            AdicionarRetracoes();
	    }

	    public override void AtualizarPontos(int novaCoordenadaXDoPontoInicial, int novaCoordenadaXDoPontoFinal)
	    {
	        foreach (var linhaHorizontal in _retracoes)
	        {
	            linhaHorizontal.AtualizarPontos(novaCoordenadaXDoPontoInicial, novaCoordenadaXDoPontoFinal);
	        }
	    }

	    public override void Desenhar(Graphics pobjGraphics)
		{
		    foreach (var linhaHorizontal in _retracoes)
		    {
		        linhaHorizontal.Desenhar(pobjGraphics);
		    }

		}

	}
}
