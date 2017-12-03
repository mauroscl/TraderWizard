using System;

namespace DTO
{
    public class ConfiguracaoDeCalculoDiario
    {
        public ConfiguracaoDeCalculoDiario(bool oscilacaoCalcular, bool oscilacaoPercentualCalcular, bool ifrCalcular, bool mediasCalcular, bool volumeMedioCalcular, 
            bool ifrMedioCalcular, bool volatilidadeCalcular)
        {
            OscilacaoCalcular = oscilacaoCalcular;
            OscilacaoPercentualCalcular = oscilacaoPercentualCalcular;
            IfrCalcular = ifrCalcular;
            MediasCalcular = mediasCalcular;
            VolumeMedioCalcular = volumeMedioCalcular;
            IfrMedioCalcular = ifrMedioCalcular;
            VolatilidadeCalcular = volatilidadeCalcular;
        }

        public bool OscilacaoCalcular { get; }
        public bool OscilacaoPercentualCalcular { get; }
        public bool IfrCalcular { get; }
        public bool MediasCalcular { get; }
        public bool VolumeMedioCalcular { get; }
        public bool IfrMedioCalcular { get; }
        public bool VolatilidadeCalcular { get; }
    }
}