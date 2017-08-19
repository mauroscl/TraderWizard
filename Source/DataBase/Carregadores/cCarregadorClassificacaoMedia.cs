using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{

	public class cCarregadorClassificacaoMedia
	{


		private readonly IList<ClassifMedia> lstTodasClassificacoes;

		public cCarregadorClassificacaoMedia()
		{
			lstTodasClassificacoes = new List<ClassifMedia>
			                             {
			                                 new cClassifMediaAltaAlinhada(),
			                                 new cClassifMediaAltaDesalinhada(),
			                                 new cClassifMediaBaixaAlinhada(),
			                                 new cClassifMediaBaixaDesalinhada(),
			                                 new cClassifMediaPrimAltaSecBaixa(),
			                                 new cClassifMediaPrimBaixaSecAlta()
			                             };
		}

		public IList<ClassifMedia> CarregaTodos()
		{
			return lstTodasClassificacoes;
		}

		public ClassifMedia CarregaPorID(cEnum.enumClassifMedia pintID)
		{
			return lstTodasClassificacoes.FirstOrDefault(x => x.ID == (decimal) pintID);
		}

	}
}
