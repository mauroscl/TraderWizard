using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using frwInterface;
using prjModelo.DomainServices;
using prjDTO;
using prjModelo.Carregadores;

namespace prjModelo.Entidades
{

	public class cAtivo
	{

		private readonly cConexao objConexao;

	    public cAtivo(string pstrCodigo, cConexao pobjConexao)
		{
			objConexao = pobjConexao;
			strCodigo = pstrCodigo;
			CotacoesDiarias = new List<cCotacaoDiaria>();
			lstSimulacoes = new SortedList<System.DateTime, cIFRSimulacaoDiaria>();
			Desdobramentos = new List<cDesdobramento>();
		}

		public cAtivo(string pstrCodigo, string pstrDescricao)
		{
			strCodigo = pstrCodigo;
			Descricao = pstrDescricao;
			CotacoesDiarias = new List<cCotacaoDiaria>();
			lstSimulacoes = new SortedList<DateTime, cIFRSimulacaoDiaria>();
			Desdobramentos = new List<cDesdobramento>();
		}

		private readonly string strCodigo;
		public string Descricao { get; set; }
		public IList<cCotacaoDiaria> CotacoesDiarias { get; set; }
		private SortedList<DateTime, cIFRSimulacaoDiaria> lstSimulacoes { get; set; }

		public SortedList<DateTime, cIFRSimulacaoDiaria> Simulacoes {
			get { return lstSimulacoes; }
		}

		public override bool Equals(object obj)
		{
		    if (obj is DBNull)
		    {
		        return false;
		    }
		    var objAtivo = (cAtivo) obj;
		    return (Codigo == objAtivo.Codigo);
		}

	    public IList<cDesdobramento> Desdobramentos { get; private set; }

	    public string Codigo {
			get { return strCodigo; }
		}

		public override string ToString()
		{
			return strCodigo + " - " + Descricao;
		}


		private void AtualizarListaDeCotacoes(IList<cCotacaoDiaria> plstNovasCotacoes)
		{
			foreach (cCotacaoDiaria objCotacaoDiaria in plstNovasCotacoes) {
				if (!CotacoesDiarias.Contains(objCotacaoDiaria)) {
					CotacoesDiarias.Add(objCotacaoDiaria);
				}
			}

			CotacoesDiarias = CotacoesDiarias.OrderBy(x => x.Data).ToList();

		}


		private void _AdicionarSimulacao(cIFRSimulacaoDiaria pobjSimulacao)
		{

			if (!Simulacoes.Values.Any(s => s.Equals(pobjSimulacao))) {
				lstSimulacoes.Add(pobjSimulacao.DataEntradaEfetiva, pobjSimulacao);

			}
		}

		public void AdicionarSimulacao(cIFRSimulacaoDiaria pobjSimulacao)
		{
			_AdicionarSimulacao(pobjSimulacao);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <param name="plstMedias"></param>
		/// <param name="pblnCarregarIFR"></param>
		/// <returns>True - encontrou cotações no período
		/// False - não encontrou cotações no período</returns>
		/// <remarks></remarks>
		public bool CarregarCotacoes(System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, IList<cMediaDTO> plstMedias, bool pblnCarregarIFR)
		{

			cCarregadorCotacaoDiaria objCarregadorCotacoes = new cCarregadorCotacaoDiaria();

			IList<cCotacaoDiaria> lstCotacoesDoCarregador = objCarregadorCotacoes.CarregarPorPeriodo(this, pdtmDataInicial, pdtmDataFinal, string.Empty, plstMedias, pblnCarregarIFR);

			if (lstCotacoesDoCarregador.Count() > 0) {
				AtualizarListaDeCotacoes(lstCotacoesDoCarregador);
				return true;
			} else {
				return false;
			}

		}

		public cCotacaoAbstract ObterCotacaoNaData(DateTime pdtmData)
		{

			var objRetorno = CotacoesDiarias.Where(x => x.Data == pdtmData).SingleOrDefault();

			if ((objRetorno == null)) {
				//Obtémm a lista de médias que devem ser carregadas de uma outra cotação qualquer
				var lstMediaDTO = CotacoesDiarias.First().ObtemListaDeMediasDTO();
				//verifica se uma outra cotação qualquer carregou o IFR. Se carregou esta também tem que carregar
				var blnCarregarIFR = (CotacoesDiarias.FirstOrDefault().IFR != null);
				if (CarregarCotacoes(pdtmData, pdtmData, lstMediaDTO, blnCarregarIFR)) {
					objRetorno = CotacoesDiarias.Where(x => x.Data == pdtmData).SingleOrDefault();
				}
			}

			return objRetorno;

		}


		public void CarregarMedias(DateTime pdtmData, IList<cMediaDTO> plstMedias)
		{
			int intContadorDeRegistros = 0;

			intContadorDeRegistros = (from c in CotacoesDiarias from m in c.Medias where m.Cotacao.Data == pdtmData select m.Cotacao.Data).Count();


			if (intContadorDeRegistros == 0) {
				cCarregadorMediaDiaria objCarregadorMedias = new cCarregadorMediaDiaria();

				cMediaAbstract objMedia = null;

				cCotacaoAbstract objCotacao = CotacoesDiarias.Where(c => c.Data == pdtmData).FirstOrDefault();


				foreach (cMediaDTO objMediaDTO in plstMedias) {
					objMedia = objCarregadorMedias.CarregarPorData((cCotacaoDiaria) objCotacao, objMediaDTO);

					objCotacao.Medias.Add(objMedia);

				}

			}

		}


		public void CarregarIFRs(DateTime pdtmData, int pintNumPeriodos)
		{
			int intContadorDeRegistros = 0;

            intContadorDeRegistros =  CotacoesDiarias.Count(x => x.IFR != null);


			if (intContadorDeRegistros == 0) {
				cCarregadorIFRDiario objCarregadorIFR = new cCarregadorIFRDiario();

				cCotacaoAbstract objCotacao = CotacoesDiarias.Where(c => c.Data == pdtmData).FirstOrDefault();

				objCotacao.IFR = objCarregadorIFR.CarregarPorData((cCotacaoDiaria) objCotacao, pintNumPeriodos);

			}

		}

		public cClassifMedia ObterClassificacaoDeMediaNaData(DateTime pdtmData)
		{

			var objCotacaoDiaria = CotacoesDiarias.Where(x => x.Data == pdtmData).FirstOrDefault();

			return cUtil.ClassifMediaCalcular(objCotacaoDiaria);

		}


		public void CarregarCotacoesIFRSobrevendidoSemSimulacao(Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, frwInterface.cEnum.enumMediaTipo pintMediaTipo)
		{
			cCarregadorCotacaoDiaria objCarregador = new cCarregadorCotacaoDiaria(objConexao);

			var lstCotacoesDoCarregador = objCarregador.CarregaComIFRSobrevendidoSemSimulacao(this, pobjSetup, pdblValorMaximoIFRSobrevendido, pintMediaTipo);

			AtualizarListaDeCotacoes(lstCotacoesDoCarregador);

		}


		public void CarregarCotacoesParaIFRComFiltro(Setup pobjSetup, frwInterface.cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial)
		{
			cCarregadorCotacaoDiaria objCarregador = new cCarregadorCotacaoDiaria(objConexao);

			var lstCotacoesDoCarregador = objCarregador.CarregarParaIFRComFiltro(this, pobjSetup, pintMediaTipo, pdtmDataInicial);

			AtualizarListaDeCotacoes(lstCotacoesDoCarregador);

		}


		public void CarregarUltimasSimulacoes(Setup pobjSetup, IList<cIFRSobrevendido> plstIFRSobrevendido, DateTime pdtmDataReferencia)
		{
			cCarregadorSimulacaoIFRDiario objCarregador = new cCarregadorSimulacaoIFRDiario(objConexao);

			//Ordena a lista de IFR Sobrevendido pela ordem do maior valor máximo para carregar corretamente as últimas simulações no for each a seguir.
			//Carregando primeiro os que tem valor mais alto vai garantir que sejam carregados sempre o último de cada ifr.

			var lstIFRSobrevendidoAux = plstIFRSobrevendido.OrderByDescending(ifr => ifr.ValorMaximo);

			//percorre todos os ifrsobrevendido 

			foreach (cIFRSobrevendido objIfrSobrevendido in lstIFRSobrevendidoAux) {
				//se o ifr sobrevendido ainda não está na lista tenta buscar a última simulação que contenha este ifr anterior à data de referência.

				if (!Simulacoes.Any(x => x.Value.DataEntradaEfetiva < pdtmDataReferencia && x.Value.Detalhes.Any(y => y.IFRSobreVendido.Equals(objIfrSobrevendido)))) {
					var lstSimulacao = objCarregador.CarregarUltimasSimulacoesPorIFRSobrevendido(this, pobjSetup, objIfrSobrevendido, pdtmDataReferencia);


					foreach (cIFRSimulacaoDiaria objSimulacao in lstSimulacao) {
						_AdicionarSimulacao(objSimulacao);

					}

				}

			}

		}

		public void CarregarTodosDesdobramentos()
		{
			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConexao);
			Desdobramentos = objCarregadorSplit.CarregarTodos(this);
		}

		public IList<cDesdobramento> RetornaListaParcialDeDesdobramentos(DateTime pdtmDataInicial, DateTime pdtmDataFinal)
		{
			return Desdobramentos.Where(x => x.Data >= pdtmDataInicial && x.Data <= pdtmDataFinal).ToList();
		}

		public IList<cDesdobramento> RetornaDesdobramentosDeUmaData(DateTime pdtmData)
		{
			return Desdobramentos.Where(x => x.Data == pdtmData).ToList();
		}

	}

}
