using System.Collections.Generic;
using System.Linq;
using DTO;

namespace Cotacao
{
    public class ValoresExtremos
    {
        public ValoresExtremos(decimal valorMinimo, decimal valorMaximo, double volumeMinimo, double volumeMaximo, int contadorIFR, List<MediaDTO> medias, int volumeMedioNumRegistros)
        {
            ValorMinimo = valorMinimo;
            ValorMaximo = valorMaximo;
            VolumeMinimo = volumeMinimo;
            VolumeMaximo = volumeMaximo;
            ContadorIFR = contadorIFR;
            Medias = medias;
            VolumeMedioNumRegistros = volumeMedioNumRegistros;
        }

        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
        public double VolumeMinimo { get; set; }
        public double VolumeMaximo { get; set; }
        public int ContadorIFR { get; set; }
        public List<MediaDTO> Medias { get; set; }
        public int VolumeMedioNumRegistros { get; set; }
        public int ContadorIFRMedio { get; set; }

        public ValoresExtremos AgruparCom(ValoresExtremos novo)
        {
            decimal valorMinimo = novo.ValorMinimo < this.ValorMinimo ? novo.ValorMinimo : this.ValorMinimo;
            decimal valorMaximo = novo.ValorMaximo > this.ValorMaximo ? novo.ValorMaximo : this.ValorMaximo;
            double volumeMinimo = novo.VolumeMinimo < this.VolumeMinimo ? novo.VolumeMinimo : this.VolumeMinimo;
            double volumeMaximo = novo.VolumeMaximo > this.VolumeMaximo ? novo.VolumeMaximo : this.VolumeMaximo;
            int contadorIfr = this.ContadorIFR + novo.ContadorIFR;
            int volumeMedioNumRegistros = this.VolumeMedioNumRegistros + novo.VolumeMedioNumRegistros;

            var medias  = new List<MediaDTO>();

            foreach (var media in novo.Medias)
            {
                MediaDTO mediaEncontrada = this.Medias.FirstOrDefault(m => m.Equals(media));

                if (mediaEncontrada != null)
                {
                    media.IncrementaNumRegistros(mediaEncontrada.NumRegistros);
                }

                medias.Add(media);
            }

            medias.AddRange(this.Medias.FindAll(m => !medias.Contains(m)));

            return new ValoresExtremos(valorMinimo, valorMaximo, volumeMinimo, volumeMaximo, contadorIfr, medias, volumeMedioNumRegistros);
        }
    }
}