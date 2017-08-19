using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
//Imports DataBase
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{

	public class cCarregadorCriterioClassificacaoMedia
	{

		//Private ReadOnly objConexao As cConexao


		private readonly IList<CriterioClassifMedia> lstTodosCriterios;

		//Public Sub New(ByVal pobjConexao As cConexao)

		//objConexao = pobjConexao


		public cCarregadorCriterioClassificacaoMedia()
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
