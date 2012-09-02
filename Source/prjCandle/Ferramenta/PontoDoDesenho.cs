using System.Drawing;

namespace prjCandle
{
    public class PontoDoDesenho
    {
        public PontoDoDesenho(Point ponto, int indice)
        {
            Ponto = ponto;
            Indice = indice;
        }

        public Point Ponto { get; private set; }
        public int Indice { get; private set; }
        public decimal ValorEmMoeda { get; set; }

        public void AlterarPonto(int x, int y)
        {
            Ponto = new Point(x, y);
        }
        public void AlterarCoordenadaY(int novoY)
        {
            Ponto = new Point(Ponto.X, novoY);
        }
    }
}