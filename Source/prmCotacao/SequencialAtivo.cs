using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraderWizard.ServicosDeAplicacao
{
    public class SequencialAtivo
    {
        public SequencialAtivo(string codigo, long sequencial)
        {
            Codigo = codigo;
            Sequencial = sequencial;
        }

        public string Codigo { get; set; }
        public long Sequencial { get; set; }
    }
}
