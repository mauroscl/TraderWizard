using prjCandle.Desenho;

namespace prjCandle
{
    public class FerramentaLinhaHorizontal: FerramentaDeDesenho
    {
        public FerramentaLinhaHorizontal(AreaDeDesenho areaDeDesenho) : base(2,areaDeDesenho)
        {
        }

        protected override void CriarDesenho()
        {
            DesenhoGerado = new LinhaHorizontal(Pontos[0], Pontos[1], AreaDeDesenho);
        }

    }
}
