using System.Diagnostics;

namespace prjCandle.Desenho
{

	public class LinhaTendencia:Linha
	{
        public LinhaTendencia(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
            : base(
                pontoInicial, pontoFinal, areaDeDesenho, false)
        {
            Debug.Print("Construindo Linha de Tendência - P1({0},{1}) - P2({2},{3})", PontoInicial.Ponto.X, pontoInicial.Ponto.Y, PontoFinal.Ponto.X, pontoFinal.Ponto.Y);
        }
	}
}
