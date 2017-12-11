using System;
using System.Collections.Generic;
using Dominio.Entidades;
using DTO;
using TraderWizard.Enumeracoes;

namespace DataBase.Interfaces
{

	public interface ICarregadorCotacao
	{

		IList<CotacaoDiaria> CarregarPorPeriodo(Ativo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<MediaDTO> psltMedias, bool pblnCarregarIFR);

		IList<CotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(Ativo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo);

		IList<CotacaoDiaria> CarregarParaIFRComFiltro(Ativo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial);
	    ICollection<CotacaoOscilacao> CarregarOscilacaoPorAtivo(string codigo);
	    IDictionary<string, List<CotacaoOscilacao>> CarregarOscilacaoAPartirDe(DateTime dataInicialDados, ICollection<string> ativos);
	    IDictionary<string, List<CotacaoNegocios>> CarregarNegociosAPartirDe(DateTime dataInicialDados, ICollection<string> ativos);

	}
}
