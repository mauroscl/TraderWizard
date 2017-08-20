using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class CarregadorClassificacaoMedia
	{


		private readonly IList<ClassifMedia> lstTodasClassificacoes;

		public CarregadorClassificacaoMedia()
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
