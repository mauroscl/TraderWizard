using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using prjModelo.DomainServices;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using prjDTO;
using prjModelo.ValueObjects;
using frwInterface;
namespace prjServicoNegocio
{

	public class cSimuladorIFRDiario
	{

		private cConexao objConexao { get; set; }
		/// DTO que contém todos os parâmetros necessários para gerar o relatório
		/// - Codigo: Código do ativo
		/// - MediaTipo: Indica o tipo de média utilizada nos cálculos (aritmética ou exponencial)
		/// - IFRTipo: Indica se o setup é IFR sem filtro ou com filtro

		private readonly cSetupIFR2SimularCodigoDTO objSetupIFR2SimularCodigoDTO;

		private readonly cAtivo objAtivo;

		private Setup objSetup = null;

		private void CarregarSetup()
		{
			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();

			if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.ComFiltro) {
                objSetup = objCarregadorSetup.CarregaPorID(cEnum.enumSetup.IFRComFiltro);
			} else if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.SemFiltro) {
				if (objSetupIFR2SimularCodigoDTO.SubirStopApenasAposRealizacaoParcial) {
                    objSetup = objCarregadorSetup.CarregaPorID(cEnum.enumSetup.IFRSemFiltroRP);
				} else {
                    objSetup = objCarregadorSetup.CarregaPorID(cEnum.enumSetup.IFRSemFiltro);
				}
			}
		}


		public cSimuladorIFRDiario(cSetupIFR2SimularCodigoDTO pobjSetupIFR2SimularCodigoDTO)
		{
			objSetupIFR2SimularCodigoDTO = pobjSetupIFR2SimularCodigoDTO;

			objConexao = new cConexao();

			objAtivo = new cAtivo(objSetupIFR2SimularCodigoDTO.Codigo, objConexao);

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

			cRS objRSAux = new cRS(objConexao);
			//RS auxiliar, pode ser utilizado quando for necessário executar uma query.


		    if (objSetupIFR2SimularCodigoDTO.ExcluirSimulacoesAnteriores) {
				cRemovedorSimulacaoIFRDiario objRemoverSimulacaoIFRDiario = new cRemovedorSimulacaoIFRDiario(objConexao);
				objRemoverSimulacaoIFRDiario.ExcluirSimulacoesAnteriores(objSetupIFR2SimularCodigoDTO.Codigo, objSetup);

			}

			//carrega todos os desdobramentos do ativo para ser utilizado posteriormente
			objAtivo.CarregarTodosDesdobramentos();


			if (objSetupIFR2SimularCodigoDTO.IFRTipo == cEnum.enumIFRTipo.ComFiltro) {

                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			    //consulta a data inicial da última operação executada para o papel no setup recebido por parâmetro.
			    string strQuery = "SELECT MAX(Data) AS Data " + Environment.NewLine + " FROM IFR_Simulacao_Diaria " +
			                      Environment.NewLine + " WHERE Codigo = " +
			                      FuncoesBd.CampoFormatar(objSetupIFR2SimularCodigoDTO.Codigo) + Environment.NewLine +
			                      " AND ID_Setup = " + FuncoesBd.CampoFormatar(objSetup.ID);

				objRSAux.ExecuteQuery(strQuery);

				DateTime dtmDataInicial = Convert.ToDateTime(objRSAux.Field("Data", cConst.DataInvalida));

				objRSAux.Fechar();

				objAtivo.CarregarCotacoesParaIFRComFiltro(objSetup, objSetupIFR2SimularCodigoDTO.MediaTipo, dtmDataInicial);



			} else {
				objAtivo.CarregarCotacoesIFRSobrevendidoSemSimulacao(objSetup, 10, objSetupIFR2SimularCodigoDTO.MediaTipo);

			}

			List<cCalculoFaixaResumoVO> lstDatasParaCalculosAdicionais = new List<cCalculoFaixaResumoVO>();

		    cCalculadorFaixasEResumoIFRDiario objCalculadorDeFaixasEResumo = new cCalculadorFaixasEResumoIFRDiario(objConexao, objAtivo, objSetup);

			IList<cIFRSobrevendido> lstIFRSobrevendido = null;

			//separa as cotações com ifr sobrevendido, pois novas cotações podem ser adicionadas no ativo durante a execução da simulação.
			var lstCotacoesComIfrSobrevendido = (from c in objAtivo.CotacoesDiarias select c).ToList();

			if (!lstCotacoesComIfrSobrevendido.Any()) {

                ((System.Threading.AutoResetEvent)stateInfo).Set();
			}


			if (objSetup.RealizarCalculosAdicionais) {
				cCarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(objConexao);

				lstIFRSobrevendido = objCarregadorIFRSobrevendido.CarregarTodos();

				//carrega última simulação de cada ifr sobrevendido para depois ser utilizado no cálculo das próprias
				objAtivo.CarregarUltimasSimulacoes(objSetup, lstIFRSobrevendido, lstCotacoesComIfrSobrevendido.First().Data);

			}

			SimuladorDeTrade objSimuladorDeTrade = new SimuladorDeTrade(objConexao, objSetup, objAtivo, lstIFRSobrevendido);

			foreach (cCotacaoDiaria objCotacaoDeInicioDaSimulacao in lstCotacoesComIfrSobrevendido) {

				if (objSetup.RealizarCalculosAdicionais)
				{
				    //deve calcular faixas e resumos para as datas das simulações anteriores até a data desta simulação.
				    IList<cCalculoFaixaResumoVO> lstDatasParaCalcular = lstDatasParaCalculosAdicionais.Where(x => x.DataSaida <= objCotacaoDeInicioDaSimulacao.Data).ToList();


				    foreach (cCalculoFaixaResumoVO objCalculoFaixaResumoVO in lstDatasParaCalcular) {
						objCalculadorDeFaixasEResumo.Calcular(objCalculoFaixaResumoVO, lstIFRSobrevendido);

						lstDatasParaCalculosAdicionais.Remove(objCalculoFaixaResumoVO);
					}
				}


			    cIFRSimulacaoDiaria objRetorno = objSimuladorDeTrade.Simular(objCotacaoDeInicioDaSimulacao);


				if ((objRetorno != null)) {
					//Verifica se já existe existe registro com data de saida e classificação média na lista
					var objCalculoFaixaResumoVOParaAdicionar = lstDatasParaCalculosAdicionais.SingleOrDefault(x => x.DataSaida == objRetorno.DataSaida && x.ClassifMedia.Equals(objRetorno.ClassificacaoMedia));

					if ((objCalculoFaixaResumoVOParaAdicionar == null)) {
						//se ainda não existe cria VO e adiciona na lista
						objCalculoFaixaResumoVOParaAdicionar = new cCalculoFaixaResumoVO(objRetorno.DataSaida, objRetorno.ValorIFR, objRetorno.ClassificacaoMedia);

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
			foreach (cCalculoFaixaResumoVO objCalculoFaixaResumoVO in lstDatasParaCalculosAdicionais) {
				objCalculadorDeFaixasEResumo.Calcular(objCalculoFaixaResumoVO, lstIFRSobrevendido);
			}

			objConexao.FecharConexao();

			Trace.WriteLine("Finalizando simulacao: " + objAtivo.Codigo);

			((System.Threading.AutoResetEvent)stateInfo).Set();

		}

	}
}
