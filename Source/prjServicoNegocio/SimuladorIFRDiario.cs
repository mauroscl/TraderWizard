using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using Dominio.ValueObjects;
using DTO;
using Services;
using TraderWizard.Enumeracoes;

namespace ServicoNegocio
{

	public class SimuladorIFRDiario
	{

		private Conexao Conexao { get; }
		/// DTO que contém todos os parâmetros necessários para gerar o relatório
		/// - Codigo: Código do ativo
		/// - MediaTipo: Indica o tipo de média utilizada nos cálculos (aritmética ou exponencial)
		/// - IFRTipo: Indica se o setup é IFR sem filtro ou com filtro

		private readonly SetupIFR2SimularCodigoDto objSetupIFR2SimularCodigoDTO;

		private readonly Ativo objAtivo;

		private Setup objSetup;

	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;

		private void CarregarSetup()
		{
			CarregadorSetup objCarregadorSetup = new CarregadorSetup();

			if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.ComFiltro) {
                objSetup = objCarregadorSetup.CarregaPorId(cEnum.enumSetup.IFRComFiltro);
			} else if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.SemFiltro) {
				if (objSetupIFR2SimularCodigoDTO.SubirStopApenasAposRealizacaoParcial) {
                    objSetup = objCarregadorSetup.CarregaPorId(cEnum.enumSetup.IFRSemFiltroRP);
				} else {
                    objSetup = objCarregadorSetup.CarregaPorId(cEnum.enumSetup.IFRSemFiltro);
				}
			}
		}


		public SimuladorIFRDiario(SetupIFR2SimularCodigoDto pobjSetupIFR2SimularCodigoDTO)
		{
			objSetupIFR2SimularCodigoDTO = pobjSetupIFR2SimularCodigoDTO;

			Conexao = new Conexao();

			objAtivo = new Ativo(objSetupIFR2SimularCodigoDTO.Codigo);

            _servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(objAtivo, Conexao);

			CarregarSetup();

		}


		///<summary>
		/// Simula a execução do setup IFR 2 para um determinado ativo em um determinado periodo
		/// </summary>
		/// <returns>
		/// True = operação realizada SEM erros de programação ou de banco de dados
		/// False = operação realizada COM erros de programação ou de banco de dados
		/// </returns>
		/// <remarks></remarks>
		/// <history>
		/// 12/12/2010 - Correção de bug que subia o stop para o setup IFR sem filtro, mesmo que ainda não tenha 
		/// ocorrido um cruzamento para cima do IFR com a média
		/// </history>
		public void SetupIFR2Simular(object stateInfo)
		{

			Trace.WriteLine("Iniciando simulacao: " + objSetupIFR2SimularCodigoDTO.Codigo);

			RS objRSAux = new RS(Conexao);
			//RS auxiliar, pode ser utilizado quando for necessário executar uma query.


		    if (objSetupIFR2SimularCodigoDTO.ExcluirSimulacoesAnteriores) {
				RemovedorSimulacaoIFRDiario objRemoverSimulacaoIFRDiario = new RemovedorSimulacaoIFRDiario(Conexao);
				objRemoverSimulacaoIFRDiario.ExcluirSimulacoesAnteriores(objSetupIFR2SimularCodigoDTO.Codigo, objSetup);

			}

			//carrega todos os desdobramentos do ativo para ser utilizado posteriormente
			_servicoDeCotacaoDeAtivo.CarregarTodosDesdobramentos();


			if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.ComFiltro) {

                FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

			    //consulta a data inicial da última operação executada para o papel no setup recebido por parâmetro.
			    string strQuery = "SELECT MAX(Data) AS Data " + Environment.NewLine + " FROM IFR_Simulacao_Diaria " +
			                      Environment.NewLine + " WHERE Codigo = " +
			                      funcoesBd.CampoFormatar(objSetupIFR2SimularCodigoDTO.Codigo) + Environment.NewLine +
			                      " AND ID_Setup = " + funcoesBd.CampoFormatar(objSetup.Id);

				objRSAux.ExecuteQuery(strQuery);

				DateTime dtmDataInicial = Convert.ToDateTime(objRSAux.Field("Data", Constantes.DataInvalida));

				objRSAux.Fechar();

				_servicoDeCotacaoDeAtivo.CarregarCotacoesParaIFRComFiltro(objSetup, objSetupIFR2SimularCodigoDTO.MediaTipo, dtmDataInicial);



			} else {
				_servicoDeCotacaoDeAtivo.CarregarCotacoesIFRSobrevendidoSemSimulacao(objSetup, 10, objSetupIFR2SimularCodigoDTO.MediaTipo);

			}

			List<CalculoFaixaResumo> lstDatasParaCalculosAdicionais = new List<CalculoFaixaResumo>();

		    CalculadorFaixasEResumoIFRDiario objCalculadorDeFaixasEResumo = new CalculadorFaixasEResumoIFRDiario(Conexao, objAtivo, objSetup);

			IList<IFRSobrevendido> lstIFRSobrevendido = null;

			//separa as cotações com ifr sobrevendido, pois novas cotações podem ser adicionadas no ativo durante a execução da simulação.
			var lstCotacoesComIfrSobrevendido = (from c in _servicoDeCotacaoDeAtivo.CotacoesDiarias select c).ToList();

			if (!lstCotacoesComIfrSobrevendido.Any()) {

                ((System.Threading.AutoResetEvent)stateInfo).Set();
			}


			if (objSetup.RealizarCalculosAdicionais) {
				CarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new CarregadorIFRSobrevendido(Conexao);

				lstIFRSobrevendido = objCarregadorIFRSobrevendido.CarregarTodos();

				//carrega última simulação de cada ifr sobrevendido para depois ser utilizado no cálculo das próprias
				_servicoDeCotacaoDeAtivo.CarregarUltimasSimulacoes(objSetup, lstIFRSobrevendido, lstCotacoesComIfrSobrevendido.First().Data);

			}

			SimuladorDeTrade objSimuladorDeTrade = new SimuladorDeTrade(Conexao, objSetup, objAtivo, lstIFRSobrevendido);

			foreach (CotacaoDiaria objCotacaoDeInicioDaSimulacao in lstCotacoesComIfrSobrevendido) {

				if (objSetup.RealizarCalculosAdicionais)
				{
				    //deve calcular faixas e resumos para as datas das simulações anteriores até a data desta simulação.
				    IList<CalculoFaixaResumo> lstDatasParaCalcular = lstDatasParaCalculosAdicionais.Where(x => x.DataSaida <= objCotacaoDeInicioDaSimulacao.Data).ToList();


				    foreach (CalculoFaixaResumo objCalculoFaixaResumoVO in lstDatasParaCalcular) {
						objCalculadorDeFaixasEResumo.Calcular(objCalculoFaixaResumoVO, lstIFRSobrevendido);

						lstDatasParaCalculosAdicionais.Remove(objCalculoFaixaResumoVO);
					}
				}


			    IFRSimulacaoDiaria objRetorno = objSimuladorDeTrade.Simular(objCotacaoDeInicioDaSimulacao);


				if ((objRetorno != null)) {
					//Verifica se já existe existe registro com data de saida e classificação média na lista
					var objCalculoFaixaResumoVOParaAdicionar = lstDatasParaCalculosAdicionais.SingleOrDefault(x => x.DataSaida == objRetorno.DataSaida && x.ClassifMedia.Equals(objRetorno.ClassificacaoMedia));

					if ((objCalculoFaixaResumoVOParaAdicionar == null)) {
						//se ainda não existe cria VO e adiciona na lista
						objCalculoFaixaResumoVOParaAdicionar = new CalculoFaixaResumo(objRetorno.DataSaida, objRetorno.ValorIFR, objRetorno.ClassificacaoMedia);

						lstDatasParaCalculosAdicionais.Add(objCalculoFaixaResumoVOParaAdicionar);
					} else {
						//se já existe verifica se o valor do IFR da última simulação é menor do que o valor que está na collection,.
						//Caso isto seja verdadeiro atualiza o valor do objeto da collection para o menor valor.
						if (objRetorno.ValorIFR < objCalculoFaixaResumoVOParaAdicionar.ValorMenorIFR) {
							objCalculoFaixaResumoVOParaAdicionar.ValorMenorIFR = objRetorno.ValorIFR;
						}
					}

				}

			}
			//fim do loop para todas as entradas

			//Após percorrer o loop tem que calcular faixa e resumo para as datas que ainda não foram calculadas. pelo menos para a data de saída da última simulação 
			//pode ser que tenha que calcular, a menos que tenha ocorrido a tentativa de executar uma última simulação que não tenha sido concluída e que tenha feito 
			//com que o cálculo fosse realizada para a data de saída da última simulação completa
			foreach (CalculoFaixaResumo objCalculoFaixaResumoVO in lstDatasParaCalculosAdicionais) {
				objCalculadorDeFaixasEResumo.Calcular(objCalculoFaixaResumoVO, lstIFRSobrevendido);
			}

			Conexao.FecharConexao();

			Trace.WriteLine("Finalizando simulacao: " + objAtivo.Codigo);

			((System.Threading.AutoResetEvent)stateInfo).Set();

		}

	}
}
