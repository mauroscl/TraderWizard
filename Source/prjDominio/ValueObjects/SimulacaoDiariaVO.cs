using System;
using Dominio.Entidades;

namespace prjDominio.ValueObjects
{

	public class SimulacaoDiariaVO
	{

		public Ativo Ativo { get; set; }
		public DateTime DataEntradaEfetiva { get; set; }
		public Setup Setup { get; set; }
		public ClassifMedia ClassificacaoMedia { get; set; }
		public IFRSobrevendido IFRSobrevendido { get; set; }
		public int NumTentativas { get; set; }


	}
}
