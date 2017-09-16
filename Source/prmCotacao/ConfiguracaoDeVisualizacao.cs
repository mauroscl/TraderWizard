using System;
using System.Collections.Generic;
using DTO;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ConfiguracaoDeVisualizacao
    {
        public ConfiguracaoDeVisualizacao(string codigoAtivo, string periodicidade, bool mediaDesenhar, List<MediaDTO> mediasSelecionadas, bool volumeDesenhar, bool ifrDesenhar,
            int ifrNumPeriodos, DateTime dataInicial, DateTime dataFinal)
        {
            CodigoAtivo = codigoAtivo;
            Periodicidade = periodicidade;
            MediaDesenhar = mediaDesenhar;
            MediasSelecionadas = mediasSelecionadas;
            VolumeDesenhar = volumeDesenhar;
            IFRDesenhar = ifrDesenhar;
            IFRNumPeriodos = ifrNumPeriodos;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
        }

        public string CodigoAtivo { get; }
        public string Periodicidade { get; }
        public bool MediaDesenhar { get; }
        public List<MediaDTO> MediasSelecionadas { get; }
        public bool VolumeDesenhar { get; }
        public bool IFRDesenhar { get; }
        public int IFRNumPeriodos { get; }
        public DateTime DataInicial { get; }
        public DateTime DataFinal { get; }

        public ConfiguracaoDeVisualizacao AlterarPeriodo(DateTime dataInicial, DateTime dataFinal)
        {
            return new ConfiguracaoDeVisualizacao(this.CodigoAtivo, this.Periodicidade, this.MediaDesenhar, this.MediasSelecionadas, this.VolumeDesenhar, this.IFRDesenhar, this.IFRNumPeriodos,dataInicial, dataFinal);
        }
    }
}