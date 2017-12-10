using System;

namespace Dominio.ValueObjects
{
    public class Volatilidade
    {
        public string Codigo { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
    }
}
