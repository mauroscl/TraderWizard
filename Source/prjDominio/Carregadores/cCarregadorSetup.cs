using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using frwInterface;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public class cCarregadorSetup
	{


		private readonly IList<Setup> lstTodosSetups;

		public cCarregadorSetup()
		{
			lstTodosSetups = new List<Setup>();

			lstTodosSetups.Add(new SetupIFR2SemFiltro());
			lstTodosSetups.Add(new SetupIFR2SemFiltroRealizacaoParcial());
			lstTodosSetups.Add(new SetupIFR2ComFiltro());

		}

		public Setup CarregaPorID(cEnum.enumSetup pintIDSetup)
		{

			return lstTodosSetups.FirstOrDefault(x => x.ID == (int) pintIDSetup);

		}

	}

}
