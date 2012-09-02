using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace prjCandle
{
    public class FerramentaLinhaTendencia: FerramentaDeDesenho
    {
        public FerramentaLinhaTendencia(AreaDeDesenho areaDeDesenho) : base(2, areaDeDesenho)
        {
        }

        protected override void CriarDesenho()
        {
            DesenhoGerado = new LinhaTendencia(Pontos[0], Pontos[1], AreaDeDesenho);
        }
    }
}
