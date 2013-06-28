using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Carregadores;
using prjModelo.Entidades;

namespace prjModelo.Regras
{

	public class cCalculadorMelhorEntrada
	{


		private readonly cConexao objConexao;
		public cCalculadorMelhorEntrada(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		/// <summary>
		/// Verifica se o detalhe que está sendo calculado é a melhor entrada. Caso positivo, se existir outras simulações pertencentes ao mesmo agrupador de entradas
		/// ou que possua a mesma data de saída desta, ajusta a flag de melhor entrada destas simulações para FALSE
		/// </summary>
		/// <param name="pobjSimulacaoDiariaDetalhe">Detalhe da Simulação que está sendo calculada</param>
		/// <param name="pblnGerouNovoAgrupadorDeTentativas">Indica se para esta simulação foi gerado um novo agrupador de tentativas</param>
		/// <remarks></remarks>
		public bool Calcular(cIFRSimulacaoDiariaDetalhe pobjSimulacaoDiariaDetalhe, bool pblnGerouNovoAgrupadorDeTentativas)
		{

			cManipuladorIFRSimulacaoDiariaDetalhe objManipuladorDetalhe = new cManipuladorIFRSimulacaoDiariaDetalhe(objConexao);

			//detalhes anteriores (já foram persistidos) que terão a flag de melhor entrada alteradas.
			cIFRSimulacaoDiariaDetalhe objDetalheAlterado = null;


			if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.PercentualMaximo < 5) {
				return false;

			}

			cIFRSimulacaoDiaria objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas = null;

			//carregar a melhor entrada para o mesmo agrupador de tentativas, caso necessário
			cCarregadorSimulacaoIFRDiario objCarregadorDeSimulacoes = new cCarregadorSimulacaoIFRDiario(objConexao);

			//Só é necessário verificar se já existe outra entrada marcada como melhor entrada se o agrupador de tentativas não foi gerado para esta entrada, 
			//pois caso tenha sido gerado para essa entrada, esta é a única.

			if (!pblnGerouNovoAgrupadorDeTentativas) {
				//verifica se o agrupador de tentativas tem outra entrada considerada como melhor entrada
				objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas = objCarregadorDeSimulacoes.CarregarMelhorEntradaPorAgrupadorDeTentativas(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Ativo, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Setup, pobjSimulacaoDiariaDetalhe.IFRSobreVendido, pobjSimulacaoDiariaDetalhe.AgrupadorDeTentativas);


				if ((objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas != null)) {
					//verificar qual das duas entradas é a melhor
					if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.EhMelhorEntrada(objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas)) {
						//se a simulação atual é melhor entrada que a anterior do mesmo agrupador...

						//desmarca a flag melhor entrada da outra.
						objDetalheAlterado = objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas.Detalhes.Where(x => x.IFRSobreVendido.Equals(pobjSimulacaoDiariaDetalhe.IFRSobreVendido)).FirstOrDefault();

						objDetalheAlterado.AlterarMelhorEntrada(false);

						objManipuladorDetalhe.Adicionar(objDetalheAlterado, "UPDATE");


					} else {
						return false;

					}

				}

			}


			//verificar se existem outra melhor entrada para a mesma data de saida 
			cIFRSimulacaoDiaria objSimulacaoComMelhorEntradaNaMesmaDataDeSaida = null;

			objSimulacaoComMelhorEntradaNaMesmaDataDeSaida = objCarregadorDeSimulacoes.CarregarMelhorEntradaPorDataDeSaida(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Ativo, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Setup, pobjSimulacaoDiariaDetalhe.IFRSobreVendido, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.DataSaida);

			//Verifica se a simulação com melhor entrada por data de saída não é a mesma que tem melhor entrada por agrupador de tentivas. Se for a mesma não precisa
			//comparar com a simulação atual, pois esta comparação já foi feita no item anterior.


			if ((objSimulacaoComMelhorEntradaNaMesmaDataDeSaida != null) && !objSimulacaoComMelhorEntradaNaMesmaDataDeSaida.Equals(objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas)) {
				if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.EhMelhorEntrada(objSimulacaoComMelhorEntradaNaMesmaDataDeSaida)) {
					objDetalheAlterado = objSimulacaoComMelhorEntradaNaMesmaDataDeSaida.Detalhes.FirstOrDefault(x => x.IFRSobreVendido.Equals(pobjSimulacaoDiariaDetalhe.IFRSobreVendido));
					objDetalheAlterado.AlterarMelhorEntrada(false);
					objManipuladorDetalhe.Adicionar(objDetalheAlterado, "UPDATE");
				} else {
					return false;
				}

			}


			//chama o manipulador de detalhes para atualizar os detalhes que tiveram a coluna "MelhorEntrada" alterada de TRUE para FALSE.
			objManipuladorDetalhe.Executar();

			return true;

		}

	}

}
