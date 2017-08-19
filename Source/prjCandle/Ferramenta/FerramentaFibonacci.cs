using prjCandle.Desenho;

namespace prjCandle
{
    public class FerramentaFibonacci: FerramentaDeDesenho
    {
        public FerramentaFibonacci(AreaDeDesenho areaDeDesenho) : base(2, areaDeDesenho)
        {
        }

        protected override void CriarDesenho()
        {
            DesenhoGerado = new FibonacciRetracement(Pontos[0], Pontos[1], AreaDeDesenho);
        }
    }
}
