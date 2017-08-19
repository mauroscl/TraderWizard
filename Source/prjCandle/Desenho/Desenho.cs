using System.Drawing;
using TraderWizard.Enumeracoes;

namespace prjCandle.Desenho
{
    public abstract class Desenho
    {
        protected AreaDeDesenho AreaDeDesenho { get; set; }
        public PontoDoDesenho PontoInicial { get; }
        public PontoDoDesenho PontoFinal { get; protected set; }
        public cEnum.SentidaHorizontalDaLinha SentidoHorizontal
        {
            get
            {
                if (PontoInicial.Ponto.X < PontoFinal.Ponto.X)
                {
                    return cEnum.SentidaHorizontalDaLinha.EsquerdaParaDireita; 
                }
                if (PontoInicial.Ponto.X > PontoFinal.Ponto.X)
                {
                    return cEnum.SentidaHorizontalDaLinha.DireitaParaEsquerda;
                }
                return cEnum.SentidaHorizontalDaLinha.Indefinido;
            }
        }


        protected Desenho(PontoDoDesenho pontoInicial, PontoDoDesenho pontoFinal, AreaDeDesenho areaDeDesenho)
        {
            AreaDeDesenho = areaDeDesenho;
            PontoInicial = pontoInicial;
            PontoInicial.ValorEmMoeda = areaDeDesenho.CalcularValorDoPontoEmModa(PontoInicial.Ponto);
            PontoFinal = pontoFinal;
            PontoFinal.ValorEmMoeda = areaDeDesenho.CalcularValorDoPontoEmModa(PontoFinal.Ponto);
        }

        //função utilizada quando é dado o clique final
        public abstract void AlterarPontoFinal(PontoDoDesenho novoPontoFinal);

        public abstract void AtualizarPontos(int novaCoordenadaXDoPontoInicial, int novaCoordenadaXDoPontoFinal);

        public abstract void Desenhar(Graphics pobjGraphics);
    }
}
