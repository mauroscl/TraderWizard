using System.Drawing;

namespace prjCandle
{
    public class NullDesenho: Desenho
    {
        public NullDesenho(PontoDoDesenho pontoInicial = null, PontoDoDesenho pontoFinal = null, AreaDeDesenho areaDeDesenho = null) 
            : base(pontoInicial, pontoFinal, areaDeDesenho)
        {
        }

        public override void AtualizarPontos(int novaCoordenadaXDoPontoInicial, int novaCoordenadaXDoPontoFinal)
        {
            
        }

        public override void Desenhar(Graphics pobjGraphics)
        {
            
        }

    }
}
