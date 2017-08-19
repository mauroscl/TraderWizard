using System.Collections.Generic;
using TraderWizard.Enumeracoes;

namespace prjCandle
{
    public abstract class FerramentaDeDesenho
    {
        private readonly int _numeroDeCliques;
        protected readonly AreaDeDesenho AreaDeDesenho;
        private int _cliquesRealizados;
        protected IList<PontoDoDesenho> Pontos { get; private set; }
        public Desenho.Desenho DesenhoGerado { get;  protected set; }
        protected cEnum.EstadoDoDesenho EstadoDoDesenho
        {
            get
            {
                if (Pontos.Count == 0)
                {
                    return cEnum.EstadoDoDesenho.NaoIniciado;
                }
                if (Pontos.Count < _numeroDeCliques)
                {
                    return cEnum.EstadoDoDesenho.EmAndamento;
                }
                if (Pontos.Count == _numeroDeCliques && _cliquesRealizados != _numeroDeCliques)
                {
                    return  cEnum.EstadoDoDesenho.Completo;
                }
                return  cEnum.EstadoDoDesenho.Concluido;
            }
        }

        

        protected FerramentaDeDesenho(int numeroDeCliques, AreaDeDesenho areaDeDesenho)
        {
            Pontos = new List<PontoDoDesenho>();
            _numeroDeCliques = numeroDeCliques;
            AreaDeDesenho = areaDeDesenho;
            DesenhoGerado = null;
        }

        protected void AdicionarPonto(PontoDoDesenho ponto)
        {

            Pontos.Add(ponto);
            
        }

        protected void AlterarUltimoPonto(PontoDoDesenho ponto)
        {
            Pontos[Pontos.Count - 1] = ponto;
            DesenhoGerado.AlterarPontoFinal(ponto);
        }

        protected abstract void CriarDesenho();

        public void Move(PontoDoDesenho ponto)
        {
            if (EstadoDoDesenho == cEnum.EstadoDoDesenho.NaoIniciado)
            {
                return;
            }

            if (EstadoDoDesenho == cEnum.EstadoDoDesenho.Concluido)
            {
                ReiniciarDesenho();
                return;
            }

            var estadoDoDesenhoAntesDestePonto = EstadoDoDesenho;

            if (Pontos.Count == _cliquesRealizados)
            {
                //Quando o número de pontos adicionados é igual ao número de cliques realizados significa
                //que o evento anterior foi um clique e agora o move vai criar um novo ponto
                AdicionarPonto(ponto);
            }
            else
            {
                //Se o número de pontos é maior que o número de cliques significa que o evento anterior 
                //também foi um move e o usuário está deslocando o desenho até encontrar a posição correta
                AlterarUltimoPonto(ponto);
            }
            
            if (estadoDoDesenhoAntesDestePonto == cEnum.EstadoDoDesenho.EmAndamento)
            {
                CriarDesenho();
            }
        }

        public void Click(PontoDoDesenho ponto)
        {

            if (EstadoDoDesenho == cEnum.EstadoDoDesenho.Concluido)
            {
                ReiniciarDesenho();
            }

            if (Pontos.Count == 0)
            {
                //apenas no primeiro clique  é que adiciona um ponto, pois é o evento inicial para criação do desenho
                AdicionarPonto(ponto);
            }
            else
            {
                //Depois que já tem um ponto, o clique apenas finaliza o ponto já criado pelo move
                AlterarUltimoPonto(ponto);
            }

            _cliquesRealizados++;

        }

        private void ReiniciarDesenho()
        {
            Pontos.Clear();
            _cliquesRealizados = 0;
            DesenhoGerado = null;
        }
    }
}
