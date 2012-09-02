using System.Drawing;

namespace prjCandle
{
    public class LinhaHorizontal: Linha
    {
        public LinhaHorizontal(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
            : this(pontoInicial, pontoFinal, areaDeDesenho,"")
        {
            
        }

        public LinhaHorizontal(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho, string labelFixo)
            : base(new PontoDoDesenho(new Point(pontoInicial.Ponto.X, pontoFinal.Ponto.Y), pontoInicial.Indice)
                ,pontoFinal, areaDeDesenho, true,labelFixo)
        {}

        public override void AlterarPontoFinal(PontoDoDesenho novoPontoFinal)
        {
            base.AlterarPontoFinal(novoPontoFinal);
            PontoInicial.AlterarCoordenadaY(novoPontoFinal.Ponto.Y);
        }

    }
}
