using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using frwInterface;
using prjDTO;
using prjModelo.Entidades;

namespace prjModelo.Interfaces
{

	public interface ICarregadorCotacao
	{

		IList<cCotacaoDiaria> CarregarPorPeriodo(cAtivo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<cMediaDTO> psltMedias, bool pblnCarregarIFR);

		IList<cCotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(cAtivo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, frwInterface.cEnum.enumMediaTipo pintMediaTipo);

		IList<cCotacaoDiaria> CarregarParaIFRComFiltro(cAtivo pobjAtivo, Setup pobjSetup, frwInterface.cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial);
	}
}
