using System;

namespace TraderWizard.ServicosDeAplicacao
{
    public class CotacaoImportacao
    {
        public DateTime Data { get; set; }
        public string Codigo { get; set; }
        public long Sequencial { get; set; }
        public long? QuantidadeNegocios { get; set; }
        public long QuantidadeNegociada { get; set; }
        public decimal VolumeFinanceiro { get; set; }
        public decimal ValorAbertura { get; set; }
        public decimal ValorFechamento { get; set; }
        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
        public decimal? Oscilacao { get; set; }
        public decimal PrecoMedio { get; set; }
    }
}
