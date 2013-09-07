using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDominio.Entidades;
using prjModelo.Entidades;

namespace prjModelo.ValueObjects
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
