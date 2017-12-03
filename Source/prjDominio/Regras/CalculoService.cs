using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Regras
{
    public class CalculoService
    {
        public decimal CalcularVolatilidadeHistorica(IEnumerable<double> valores)
        {
            var logs = valores.Select(v => Math.Log(v));
            var desvioPadrao = CalcularDesvioPadrao(logs.ToArray());
            double volatilidade = desvioPadrao * Math.Sqrt(252);
            return Math.Round( new decimal(volatilidade),4) ;
        }

        private double CalcularDesvioPadrao(ICollection<double> valores)
        {
            double media = valores.Average();
            var quadrados = valores.Select(v => Math.Pow(v - media, 2));
            //return Math.Sqrt(quadrados.Average());
            return Math.Sqrt(quadrados.Sum() / (valores.Count - 1));
        }
    }
}
