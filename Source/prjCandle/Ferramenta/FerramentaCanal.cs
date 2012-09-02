namespace prjCandle
{
    public class FerramentaCanal: FerramentaDeDesenho
    {
        public FerramentaCanal(AreaDeDesenho areaDeDesenho) : base(3, areaDeDesenho)
        {
        }

        protected override void CriarDesenho()
        {
            //DesenhoGerado = Pontos.Count == 2 ? _ferramentaAuxiliar.DesenhoGerado : new Canal(Pontos[0], Pontos[1], Pontos[2], AreaDeDesenho);
            if (Pontos.Count == 2)
            {
                DesenhoGerado = new LinhaTendencia(Pontos[0],Pontos[1], AreaDeDesenho);
            }
            else
            {
                DesenhoGerado = new Canal(Pontos[0], Pontos[1], Pontos[2], AreaDeDesenho);
            }
        }

    }
}
