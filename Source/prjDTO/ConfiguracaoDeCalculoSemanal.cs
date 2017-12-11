namespace DTO
{
    public class ConfiguracaoDeCalculoSemanal: ConfiguracaoDeCalculoDiario
    {
        public ConfiguracaoDeCalculoSemanal(bool oscilacaoCalcular, bool oscilacaoPercentualCalcular, bool ifrCalcular, bool mediasCalcular, 
            bool volumeMedioCalcular, bool ifrMedioCalcular, bool volatilidadeCalcular , bool mediaNegociosCalcular, bool calcularDadosGerais) : 
            base(oscilacaoCalcular, oscilacaoPercentualCalcular,  ifrCalcular, mediasCalcular, volumeMedioCalcular, ifrMedioCalcular, volatilidadeCalcular, mediaNegociosCalcular)
        {
            CalcularDadosGerais = calcularDadosGerais;
        }

        public bool CalcularDadosGerais { get; }

    }
}
