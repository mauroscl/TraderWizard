using System;
using System.Drawing;
using TraderWizard.Enumeracoes;

namespace prjCandle
{
    public class AreaDeDesenho
    {
        public AreaDeDesenho(double pixelsPorReal, int valorMaximoDoEixoY, decimal valorDoPrecoMaximo, cEnum.Escala escala
            , LimiteHorizontal limiteEsquerdo, LimiteHorizontal limiteDireito)
        {
            PixelsPorReal = pixelsPorReal;
            ValorMaximoDoEixoY = valorMaximoDoEixoY;
            ValorDoPrecoMaximo = valorDoPrecoMaximo;
            Escala = escala;
            LimiteEsquerdo = limiteEsquerdo;
            LimiteDireito = limiteDireito;
        }

        public double PixelsPorReal { get; private set; }
        public int ValorMaximoDoEixoY { get; private set; }
        public decimal ValorDoPrecoMaximo { get; private set; }
        public cEnum.Escala Escala { get; private set; }
        public LimiteHorizontal LimiteEsquerdo { get; private set; }
        public LimiteHorizontal LimiteDireito { get; private set; }


        public void AlterarValores(decimal novoValorDoPrecoMaximo, double novoPixelsPorReal
            , LimiteHorizontal novoLimiteEsquerdo, LimiteHorizontal novoLimiteDireito)
        {
            ValorDoPrecoMaximo = novoValorDoPrecoMaximo;
            PixelsPorReal = novoPixelsPorReal;
            LimiteEsquerdo = novoLimiteEsquerdo;
            LimiteDireito = novoLimiteDireito;
        }

        public decimal CalcularValorDoPontoEmModa(Point ponto)
        {
            double dblValorReta = (ValorMaximoDoEixoY + (double)ValorDoPrecoMaximo * PixelsPorReal - ponto.Y) / PixelsPorReal;

            if (Escala == cEnum.Escala.Logaritmica)
            {
                //se a escala é logarítmica, tem que aplicar a função exponencial, pois valor calculado já 
                //está em escala logarítmica e preciso ser convertido para o seu valor real.
                dblValorReta = Math.Exp(dblValorReta);

            }

            return (decimal) dblValorReta;
        }

        public int CalculaCoordenadaYPeloValor(decimal valor)
        {
            //calculo do percentual 0
            double dblValorNaEscala = (double) valor;

            if (Escala == cEnum.Escala.Logaritmica)
            {
                dblValorNaEscala = Math.Log(dblValorNaEscala);

            }
            return ValorMaximoDoEixoY + (int)(((double)ValorDoPrecoMaximo - dblValorNaEscala) * PixelsPorReal);

        }

        private bool IndiceEntreOsLimites(int indice)
        {
            return indice >= LimiteEsquerdo.Indice && indice <= LimiteDireito.Indice;
        }
        private bool IndiceForaDosLimites(int indice)
        {
            return indice < LimiteEsquerdo.Indice && indice > LimiteDireito.Indice;
        }

        private bool IndiceAEsquerdaDoLimite(int indice)
        {
            return indice < LimiteEsquerdo.Indice;
        }

        private bool IndiceADireitaDoLimite(int indice)
        {
            return indice > LimiteDireito.Indice;
        }


        /// <summary>
        /// Indica se a área de desenho contém uma determinada figura
        /// </summary>
        /// <param name="desenho"></param>
        /// <returns>
        /// true - área contém todo ou parcialmente o desenho
        /// false - área não contém o desenho
        /// </returns>
        public bool Contem(Desenho desenho)
        {

            return IndiceEntreOsLimites(desenho.PontoInicial.Indice) || IndiceEntreOsLimites(desenho.PontoFinal.Indice)
                   || (IndiceAEsquerdaDoLimite(desenho.PontoInicial.Indice) && IndiceADireitaDoLimite(desenho.PontoFinal.Indice)
                   || (IndiceAEsquerdaDoLimite(desenho.PontoFinal.Indice) && IndiceADireitaDoLimite(desenho.PontoInicial.Indice)));

        }


    }
}