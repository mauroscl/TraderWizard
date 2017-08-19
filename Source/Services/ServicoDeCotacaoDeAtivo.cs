using System;
using System.Collections.Generic;
using System.Linq;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using DTO;
using prjModelo.Carregadores;
using prjModelo.DomainServices;
using TraderWizard.Enumeracoes;

namespace Services
{

	public class ServicoDeCotacaoDeAtivo
	{

		private readonly Conexao _conexao;
	    private readonly Ativo _ativo;

	    public ServicoDeCotacaoDeAtivo(Ativo ativo, Conexao pobjConexao)
	    {
	        _ativo = ativo;
			_conexao = pobjConexao;
			CotacoesDiarias = new List<CotacaoDiaria>();
			lstSimulacoes = new SortedList<DateTime, IFRSimulacaoDiaria>();
			Desdobramentos = new List<Desdobramento>();
		}

		public IList<CotacaoDiaria> CotacoesDiarias { get; set; }
		private SortedList<DateTime, IFRSimulacaoDiaria> lstSimulacoes { get; set; }

		public SortedList<DateTime, IFRSimulacaoDiaria> Simulacoes {
			get { return lstSimulacoes; }
		}

	    public IList<Desdobramento> Desdobramentos { get; private set; }

		private void AtualizarListaDeCotacoes(IList<CotacaoDiaria> plstNovasCotacoes)
		{
			foreach (CotacaoDiaria objCotacaoDiaria in plstNovasCotacoes) {
				if (!CotacoesDiarias.Contains(objCotacaoDiaria)) {
					CotacoesDiarias.Add(objCotacaoDiaria);
				}
			}

			CotacoesDiarias = CotacoesDiarias.OrderBy(x => x.Data).ToList();

		}


		private void _AdicionarSimulacao(IFRSimulacaoDiaria pobjSimulacao)
		{

			if (!Simulacoes.Values.Any(s => s.Equals(pobjSimulacao))) {
				lstSimulacoes.Add(pobjSimulacao.DataEntradaEfetiva, pobjSimulacao);

			}
		}

		public void AdicionarSimulacao(IFRSimulacaoDiaria pobjSimulacao)
		{
			_AdicionarSimulacao(pobjSimulacao);
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="ativo">ativo para o qual deve ser carregado as cotações</param>
	    /// <param name="pdtmDataInicial"></param>
	    /// <param name="pdtmDataFinal"></param>
	    /// <param name="plstMedias"></param>
	    /// <param name="pblnCarregarIFR"></param>
	    /// <returns>True - encontrou cotações no período
	    /// False - não encontrou cotações no período</returns>
	    /// <remarks></remarks>
	    public bool CarregarCotacoes(DateTime pdtmDataInicial, DateTime pdtmDataFinal, IList<MediaDTO> plstMedias, bool pblnCarregarIFR)
		{

			var objCarregadorCotacoes = new CarregadorCotacaoDiaria();

			IList<CotacaoDiaria> lstCotacoesDoCarregador = objCarregadorCotacoes.CarregarPorPeriodo(_ativo, pdtmDataInicial, pdtmDataFinal, string.Empty, plstMedias, pblnCarregarIFR);

			if (lstCotacoesDoCarregador.Count() > 0) {
				AtualizarListaDeCotacoes(lstCotacoesDoCarregador);
				return true;
			} else {
				return false;
			}

		}

		public CotacaoAbstract ObterCotacaoNaData(DateTime pdtmData)
		{

			var objRetorno = CotacoesDiarias.SingleOrDefault(x => x.Data == pdtmData);

			if ((objRetorno == null)) {
				//Obtémm a lista de médias que devem ser carregadas de uma outra cotação qualquer
				var lstMediaDTO = CotacoesDiarias.First().ObtemListaDeMediasDTO();
				//verifica se uma outra cotação qualquer carregou o IFR. Se carregou esta também tem que carregar
			    var cotacaoDiaria = CotacoesDiarias.FirstOrDefault();
			    var blnCarregarIFR = cotacaoDiaria != null && (cotacaoDiaria.IFR != null);
				if (CarregarCotacoes(pdtmData, pdtmData, lstMediaDTO, blnCarregarIFR)) {
					objRetorno = CotacoesDiarias.SingleOrDefault(x => x.Data == pdtmData);
				}
			}

			return objRetorno;

		}


		public void CarregarMedias(DateTime pdtmData, IList<MediaDTO> plstMedias)
		{
		    int intContadorDeRegistros = (from c in CotacoesDiarias from m in c.Medias where m.Cotacao.Data == pdtmData select m.Cotacao.Data).Count();

		    if (intContadorDeRegistros == 0) {
				var objCarregadorMedias = new CarregadorMediaDiaria();

		        CotacaoAbstract objCotacao = CotacoesDiarias.First(c => c.Data == pdtmData);


				foreach (MediaDTO objMediaDTO in plstMedias)
				{
				    MediaAbstract objMedia = objCarregadorMedias.CarregarPorData((CotacaoDiaria) objCotacao, objMediaDTO);

				    objCotacao.Medias.Add(objMedia);
				}
		    }
		}


	    public void CarregarIFRs(DateTime pdtmData, int pintNumPeriodos)
		{
			int intContadorDeRegistros = 0;

            intContadorDeRegistros =  CotacoesDiarias.Count(x => x.IFR != null);


			if (intContadorDeRegistros == 0) {
				var objCarregadorIFR = new CarregadorIFRDiario();

				CotacaoAbstract objCotacao = CotacoesDiarias.First(c => c.Data == pdtmData);

				objCotacao.IFR = objCarregadorIFR.CarregarPorData((CotacaoDiaria) objCotacao, pintNumPeriodos);

			}

		}

		public ClassifMedia ObterClassificacaoDeMediaNaData(DateTime pdtmData)
		{

			var objCotacaoDiaria = CotacoesDiarias.FirstOrDefault(x => x.Data == pdtmData);

			return cUtil.ClassifMediaCalcular(objCotacaoDiaria);

		}


		public void CarregarCotacoesIFRSobrevendidoSemSimulacao(Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo)
		{
			var objCarregador = new CarregadorCotacaoDiaria(_conexao);

			var lstCotacoesDoCarregador = objCarregador.CarregaComIFRSobrevendidoSemSimulacao(_ativo, pobjSetup, pdblValorMaximoIFRSobrevendido, pintMediaTipo);

			AtualizarListaDeCotacoes(lstCotacoesDoCarregador);

		}


		public void CarregarCotacoesParaIFRComFiltro(Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, DateTime pdtmDataInicial)
		{
			var objCarregador = new CarregadorCotacaoDiaria(_conexao);

			var lstCotacoesDoCarregador = objCarregador.CarregarParaIFRComFiltro(_ativo, pobjSetup, pintMediaTipo, pdtmDataInicial);

			AtualizarListaDeCotacoes(lstCotacoesDoCarregador);

		}


		public void CarregarUltimasSimulacoes(Setup pobjSetup, IList<IFRSobrevendido> plstIFRSobrevendido, DateTime pdtmDataReferencia)
		{
			var objCarregador = new CarregadorSimulacaoIFRDiario(_conexao);

			//Ordena a lista de IFR Sobrevendido pela ordem do maior valor máximo para carregar corretamente as últimas simulações no for each a seguir.
			//Carregando primeiro os que tem valor mais alto vai garantir que sejam carregados sempre o último de cada ifr.

			var lstIFRSobrevendidoAux = plstIFRSobrevendido.OrderByDescending(ifr => ifr.ValorMaximo);

			//percorre todos os ifrsobrevendido 

			foreach (IFRSobrevendido objIfrSobrevendido in lstIFRSobrevendidoAux) {
				//se o ifr sobrevendido ainda não está na lista tenta buscar a última simulação que contenha este ifr anterior à data de referência.

				if (!Simulacoes.Any(x => x.Value.DataEntradaEfetiva < pdtmDataReferencia && x.Value.Detalhes.Any(y => y.IFRSobreVendido.Equals(objIfrSobrevendido)))) {
					var lstSimulacao = objCarregador.CarregarUltimasSimulacoesPorIFRSobrevendido(_ativo, pobjSetup, objIfrSobrevendido, pdtmDataReferencia);


					foreach (IFRSimulacaoDiaria objSimulacao in lstSimulacao) {
						_AdicionarSimulacao(objSimulacao);

					}

				}

			}

		}

		public void CarregarTodosDesdobramentos()
		{
			var objCarregadorSplit = new cCarregadorSplit(_conexao);
			Desdobramentos = objCarregadorSplit.CarregarTodos(_ativo);
		}

		public IList<Desdobramento> RetornaListaParcialDeDesdobramentos(DateTime pdtmDataInicial, DateTime pdtmDataFinal)
		{
			return Desdobramentos.Where(x => x.Data >= pdtmDataInicial && x.Data <= pdtmDataFinal).ToList();
		}

		public IList<Desdobramento> RetornaDesdobramentosDeUmaData(DateTime pdtmData)
		{
			return Desdobramentos.Where(x => x.Data == pdtmData).ToList();
		}

        public IFRSimulacaoDiaria BuscaSimulacaoAnterior(IFRSobrevendido pobjIFRSobrevendido, DateTime dataEntradaEfetiva)
        {

            return (from s in Simulacoes from d in s.Value.Detalhes
                    where s.Value.DataEntradaEfetiva < dataEntradaEfetiva 
                        && d.IFRSobreVendido.Equals(pobjIFRSobrevendido) 
                    select s).LastOrDefault().Value;

        }

        private void CarregarCotacoesAnteriores(CotacaoAbstract cotacao)
        {
            //obtem o último dia anterior a este em que há cotações para o ativo
            //Dim dtmDataDaUltimaCotacaoAnterior As DateTime? = Ativo.CotacoesDiarias.Where(Function(x) x.Data < Data).Max(Of DateTime)(Function(y) y.Data)
            var objUltimaCotacaoAnterior = (from c in CotacoesDiarias where c.Data < cotacao.Data select c).LastOrDefault();

            DateTime dtmDataInicial = cotacao.Data;

            var lstMediasDTO = cotacao.ObtemListaDeMediasDTO();

            var blnEncontrouCotacoes = false;


            while (!blnEncontrouCotacoes)
            {
                DateTime dtmDataFinal = dtmDataInicial.AddDays(-1);
                DateTime dtmDataDoPrimeiroDiaDoMes = new DateTime(dtmDataFinal.Year, dtmDataFinal.Month, 1);

                if ((objUltimaCotacaoAnterior == null) || dtmDataDoPrimeiroDiaDoMes >= objUltimaCotacaoAnterior.Data.AddDays(1))
                {
                    //se não tem cotações anteriores a data atual ou se a data do primeiro dia do mês é maior ou igual a data da última cotação
                    //anterior utiliza a primeira data do mês para buscar todas as cotações do mês.
                    dtmDataInicial = dtmDataDoPrimeiroDiaDoMes;
                }
                else
                {
                    //caso contrário busca a partir do primeiro dia após a última cotação
                    dtmDataInicial = objUltimaCotacaoAnterior.Data.AddDays(1);
                }

                blnEncontrouCotacoes = CarregarCotacoes(dtmDataInicial, dtmDataFinal, lstMediasDTO, true);

            }

        }

        public CotacaoAbstract CotacaoAnterior(CotacaoAbstract cotacao)
        {
            CotacaoAbstract objCotacaoAnterior = CotacoesDiarias.SingleOrDefault(x => x.Sequencial == cotacao.Sequencial - 1);

            if ((objCotacaoAnterior == null) && cotacao.Sequencial > 1)
            {
                CarregarCotacoesAnteriores(cotacao);
            }

            //retorna a cotação cujo sequencial é uma posição anterior ao sequencial da cotação atual.
            return CotacoesDiarias.SingleOrDefault(x => x.Sequencial == cotacao.Sequencial - 1);

        }



	}

}
