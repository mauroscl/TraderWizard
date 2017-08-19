using System.Drawing;

namespace prjCandle.Desenho
{

	public class Canal: Desenho
	{
	    /// <summary>
	    /// Ferramenta constru�da com tr�s clicks do mouse. O primeiro e o segundo cliques definem a primeira linha do canal, que tamb�m serve como base para construir a segunda linha.
	    /// O terceiro clique define o deslocamento do canal em rela��o � primeira linha. A segunda linha � sempre paralela � primeira.
	    /// </summary>
	    /// <param name="pontoInicial">ponto 1 da linha base</param>
	    /// <param name="pontoIntermediario"> ponto 2 da linha base</param>
	    /// <param name="pontoFinal">deslocamento em rela��o � linha base</param>
	    /// <param name="areaDeDesenho">�rea que ser� efetuado o desenho </param>
	    public Canal(PontoDoDesenho pontoInicial, PontoDoDesenho pontoIntermediario, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
            :base(pontoInicial, pontoFinal, areaDeDesenho)
        {
            _linha1 = new LinhaTendencia(pontoInicial,pontoIntermediario, AreaDeDesenho);

            //calcula o ponto m�dia da primeira reta, pois as coordenadas da segunda linha s�o dadas pelo deslocamento do pontoFinal em rela��o ao ponto m�dio
            _pontoMedioDaLinha1 = new Point((pontoInicial.Ponto.X + pontoIntermediario.Ponto.X)/2, (pontoInicial.Ponto.Y + pontoIntermediario.Ponto.Y)/2);

            CalcularLinha2(pontoFinal);

        }

	    private readonly LinhaTendencia _linha1;
	    private LinhaTendencia _linha2;
	    private readonly Point _pontoMedioDaLinha1;

        private void CalcularLinha2(PontoDoDesenho pontoFinal)
        {
            //Coordenada X igual ao pontoInicial da linha 1.Coordenada Y � o deslocamento do ponto final em rela��o ao ponto inicial e o ponto m�dio da linha 1
            var pontoInicialDaLinha2 = new PontoDoDesenho(new Point(_linha1.PontoInicial.Ponto.X, _linha1.PontoInicial.Ponto.Y - _pontoMedioDaLinha1.Y + pontoFinal.Ponto.Y), _linha1.PontoInicial.Indice);
            //Coordenada X igual ao pontoFinal da linha 1.Coordenada Y � o deslocamento do ponto final em rela��o ao ponto final e o ponto m�dio da linha 1
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
