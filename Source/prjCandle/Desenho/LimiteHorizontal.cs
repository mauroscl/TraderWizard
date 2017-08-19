namespace prjCandle.Desenho
{
    public class LimiteHorizontal
    {
        public LimiteHorizontal(int indice, int coordenadaX)
        {
            Indice = indice;
            CoordenadaX = coordenadaX;
        }

        public int Indice { get; private set; }
        public int CoordenadaX { get; private set; }
    }
}
