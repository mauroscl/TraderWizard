using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class CarregadorSetup
	{


		private readonly IList<Setup> lstTodosSetups;

		public CarregadorSetup()
		{
			lstTodosSetups = new List<Setup>();

			lstTodosSetups.Add(new SetupIFR2SemFiltro());
			lstTodosSetups.Add(new SetupIFR2SemFiltroRealizacaoParcial());
			lstTodosSetups.Add(new SetupIFR2ComFiltro());

		}

		public Setup CarregaPorId(cEnum.enumSetup idSetup)
		{

			return lstTodosSetups.FirstOrDefault(x => x.Id == (int) idSetup);

		}

	}

}
