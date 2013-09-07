using System.Collections.Generic;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Interfaces
{

	public interface ICarregadorCotacao
	{

		IList<CotacaoDiaria> CarregarPorPeriodo(Ativo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<cMediaDTO> psltMedias, bool pblnCarregarIFR);

		IList<CotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(Ativo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo);

		IList<CotacaoDiaria> CarregarParaIFRComFiltro(Ativo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial);
	}
}
