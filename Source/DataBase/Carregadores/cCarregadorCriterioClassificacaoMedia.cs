using System.Collections.Generic;
using System.Linq;
using prjModelo.Entidades;
//Imports DataBase
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{

	public class cCarregadorCriterioClassificacaoMedia
	{

		//Private ReadOnly objConexao As cConexao


		private readonly IList<cCriterioClassifMedia> lstTodosCriterios;

		//Public Sub New(ByVal pobjConexao As cConexao)

		//objConexao = pobjConexao


		public cCarregadorCriterioClassificacaoMedia()
		{
			lstTodosCriterios = new List<cCriterioClassifMedia>();

			lstTodosCriterios.Add(new cCriterioClassifMediaMM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM49());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM200());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM49());

		}

		public IList<cCriterioClassifMedia> CarregaTodos()
		{
			return lstTodosCriterios;
		}

		public cCriterioClassifMedia CarregaPorID(cEnum.enumCriterioClassificacaoMedia pintID)
		{
			return lstTodosCriterios.FirstOrDefault(x => x.ID == (decimal) pintID);
		}

	}
}
