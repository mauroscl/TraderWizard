using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class CarregadorCriterioClassificacaoMedia
	{

		//Private ReadOnly objConexao As cConexao


		private readonly IList<CriterioClassifMedia> lstTodosCriterios;

		//Public Sub New(ByVal pobjConexao As cConexao)

		//objConexao = pobjConexao


		public CarregadorCriterioClassificacaoMedia()
		{
			lstTodosCriterios = new List<CriterioClassifMedia>();

			lstTodosCriterios.Add(new cCriterioClassifMediaMM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM49());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM200());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM49());

		}

		public IList<CriterioClassifMedia> CarregaTodos()
		{
			return lstTodosCriterios;
		}

		public CriterioClassifMedia CarregaPorID(cEnum.enumCriterioClassificacaoMedia pintID)
		{
			return lstTodosCriterios.FirstOrDefault(x => x.ID == (decimal) pintID);
		}

	}
}
