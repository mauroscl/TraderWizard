using System;
using prjDominio.Entidades;

namespace prjDominio.ValueObjects
{

	public class SimulacaoDiariaVO
	{

		public Ativo Ativo { get; set; }
		public DateTime DataEntradaEfetiva { get; set; }
		public Setup Setup { get; set; }
		public cClassifMedia ClassificacaoMedia { get; set; }
		public cIFRSobrevendido IFRSobrevendido { get; set; }
		public int NumTentativas { get; set; }


	}
}
