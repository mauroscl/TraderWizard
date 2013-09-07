using System.Collections.Generic;
using System.Linq;
using prjDominio.Entidades;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{

	public class cCarregadorClassificacaoMedia
	{


		private readonly IList<cClassifMedia> lstTodasClassificacoes;

		public cCarregadorClassificacaoMedia()
		{
			lstTodasClassificacoes = new List<cClassifMedia>
			                             {
			                                 new cClassifMediaAltaAlinhada(),
			                                 new cClassifMediaAltaDesalinhada(),
			                                 new cClassifMediaBaixaAlinhada(),
			                                 new cClassifMediaBaixaDesalinhada(),
			                                 new cClassifMediaPrimAltaSecBaixa(),
			                                 new cClassifMediaPrimBaixaSecAlta()
			                             };
		}

		public IList<cClassifMedia> CarregaTodos()
		{
			return lstTodasClassificacoes;
		}

		public cClassifMedia CarregaPorID(cEnum.enumClassifMedia pintID)
		{
			return lstTodasClassificacoes.FirstOrDefault(x => x.ID == (decimal) pintID);
		}

	}
}
