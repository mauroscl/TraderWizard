using System.Collections.Generic;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Interfaces
{

	public interface ICarregadorCotacao
	{

		IList<cCotacaoDiaria> CarregarPorPeriodo(cAtivo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<cMediaDTO> psltMedias, bool pblnCarregarIFR);

		IList<cCotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(cAtivo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo);

		IList<cCotacaoDiaria> CarregarParaIFRComFiltro(cAtivo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial);
	}
}
