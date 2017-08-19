using System.Drawing;

namespace prjCandle.Desenho
{

	public class Canal: Desenho
	{
	    /// <summary>
	    /// Ferramenta construída com três clicks do mouse. O primeiro e o segundo cliques definem a primeira linha do canal, que também serve como base para construir a segunda linha.
	    /// O terceiro clique define o deslocamento do canal em relação à primeira linha. A segunda linha é sempre paralela à primeira.
	    /// </summary>
	    /// <param name="pontoInicial">ponto 1 da linha base</param>
	    /// <param name="pontoIntermediario"> ponto 2 da linha base</param>
	    /// <param name="pontoFinal">deslocamento em relação à linha base</param>
	    /// <param name="areaDeDesenho">área que será efetuado o desenho </param>
	    public Canal(PontoDoDesenho pontoInicial, PontoDoDesenho pontoIntermediario, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
            :base(pontoInicial, pontoFinal, areaDeDesenho)
        {
            _linha1 = new LinhaTendencia(pontoInicial,pontoIntermediario, AreaDeDesenho);

            //calcula o ponto média da primeira reta, pois as coordenadas da segunda linha são dadas pelo deslocamento do pontoFinal em relação ao ponto médio
            _pontoMedioDaLinha1 = new Point((pontoInicial.Ponto.X + pontoIntermediario.Ponto.X)/2, (pontoInicial.Ponto.Y + pontoIntermediario.Ponto.Y)/2);

            CalcularLinha2(pontoFinal);

        }

	    private readonly LinhaTendencia _linha1;
	    private LinhaTendencia _linha2;
	    private readonly Point _pontoMedioDaLinha1;

        private void CalcularLinha2(PontoDoDesenho pontoFinal)
        {
            //Coordenada X igual ao pontoInicial da linha 1.Coordenada Y é o deslocamento do ponto final em relação ao ponto inicial e o ponto médio da linha 1
            var pontoInicialDaLinha2 = new PontoDoDesenho(new Point(_linha1.PontoInicial.Ponto.X, _linha1.PontoInicial.Ponto.Y - _pontoMedioDaLinha1.Y + pontoFinal.Ponto.Y), _linha1.PontoInicial.Indice);
            //Coordenada X igual ao pontoFinal da linha 1.Coordenada Y é o deslocamento do ponto final em relação ao ponto final e o ponto médio da linha 1
            var pontoFinalDaLinha2 = new PontoDoDesenho(new Point(_linha1.PontoFinal.Ponto.X, _linha1.PontoFinal.Ponto.Y - _pontoMedioDaLinha1.Y + pontoFinal.Ponto.Y), pontoFinal.Indice);

            _linha2 = new LinhaTendencia(pontoInicialDaLinha2, pontoFinalDaLinha2, AreaDeDesenho);
            
        }

	    public override void AlterarPontoFinal(PontoDoDesenho novoPontoFinal)
	    {
            CalcularLinha2(novoPontoFinal);
	    }

	    public override void AtualizarPontos(int novaCoordenadaXDoPontoInicial, int novaCoordenadaXDoPontoFinal)
	    {
	        _linha1.AtualizarPontos(novaCoordenadaXDoPontoInicial, novaCoordenadaXDoPontoFinal);
	    }

	    public override void Desenhar(Graphics pobjGraphics)
	    {
	        _linha1.Desenhar(pobjGraphics);
            _linha2.Desenhar(pobjGraphics);
	    }
	}
}
