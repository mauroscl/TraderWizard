using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
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

			return lstTodosSetups.FirstOrDefault(x => x.Id == (int) pintIDSetup);

		}

	}

}
