using System;
using System.Linq;
using DataBase;
using prjDominio.Entidades;
using prjModelo.Carregadores;
using prjModelo.Entidades;

namespace prjServicoNegocio
{

	public class cCalculadorMelhorEntrada
	{

		private readonly cConexao _conexao;
	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;
		public cCalculadorMelhorEntrada(cConexao pobjConexao, ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
		{
		    _conexao = pobjConexao;
		    _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
		}

	    /// <summary>
		/// Verifica se o detalhe que está sendo calculado é a melhor entrada. Caso positivo, se existir outras simulações pertencentes ao mesmo agrupador de entradas
		/// ou que possua a mesma data de saída desta, ajusta a flag de melhor entrada destas simulações para FALSE
		/// </summary>
		/// <param name="pobjSimulacaoDiariaDetalhe">Detalhe da Simulação que está sendo calculada</param>
		/// <param name="pblnGerouNovoAgrupadorDeTentativas">Indica se para esta simulação foi gerado um novo agrupador de tentativas</param>
		/// <remarks></remarks>
		public void Calcular(cIFRSimulacaoDiariaDetalhe pobjSimulacaoDiariaDetalhe, bool pblnGerouNovoAgrupadorDeTentativas)
		{

			var objManipuladorDetalhe = new cManipuladorIFRSimulacaoDiariaDetalhe(_conexao);

			//detalhes anteriores (já foram persistidos) que terão a flag de melhor entrada alteradas.
			cIFRSimulacaoDiariaDetalhe objDetalheAlterado;

			if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.PercentualMaximo < 5) {
                pobjSimulacaoDiariaDetalhe.AlterarMelhorEntrada(false);

			}

			cIFRSimulacaoDiaria objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas = null;

			//carregar a melhor entrada para o mesmo agrupador de tentativas, caso necessário
			var objCarregadorDeSimulacoes = new cCarregadorSimulacaoIFRDiario(_conexao);

			//Só é necessário verificar se já existe outra entrada marcada como melhor entrada se o agrupador de tentativas não foi gerado para esta entrada, 
			//pois caso tenha sido gerado para essa entrada, esta é a única.

			if (!pblnGerouNovoAgrupadorDeTentativas) {
				//verifica se o agrupador de tentativas tem outra entrada considerada como melhor entrada
				objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas = objCarregadorDeSimulacoes.CarregarMelhorEntradaPorAgrupadorDeTentativas(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Ativo, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Setup, pobjSimulacaoDiariaDetalhe.IFRSobreVendido, pobjSimulacaoDiariaDetalhe.AgrupadorDeTentativas);

				if ((objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas != null)) {

                    cCotacaoAbstract objCotacaoParaConverter = ConverterCotacao(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria, objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas);

					//verificar qual das duas entradas é a melhor
					if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.EhMelhorEntrada(objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas, objCotacaoParaConverter)) {
						//se a simulação atual é melhor entrada que a anterior do mesmo agrupador...

						//desmarca a flag melhor entrada da outra.
						objDetalheAlterado = objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas.Detalhes.First(x => x.IFRSobreVendido.Equals(pobjSimulacaoDiariaDetalhe.IFRSobreVendido));

						objDetalheAlterado.AlterarMelhorEntrada(false);

						objManipuladorDetalhe.Adicionar(objDetalheAlterado, "UPDATE");


					} else {
                        pobjSimulacaoDiariaDetalhe.AlterarMelhorEntrada(false);

					}

				}

			}


			//verificar se existem outra melhor entrada para a mesma data de saida 
			cIFRSimulacaoDiaria objSimulacaoComMelhorEntradaNaMesmaDataDeSaida = null;

			objSimulacaoComMelhorEntradaNaMesmaDataDeSaida = objCarregadorDeSimulacoes.CarregarMelhorEntradaPorDataDeSaida(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Ativo, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.Setup, pobjSimulacaoDiariaDetalhe.IFRSobreVendido, pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.DataSaida);

			//Verifica se a simulação com melhor entrada por data de saída não é a mesma que tem melhor entrada por agrupador de tentivas. Se for a mesma não precisa
			//comparar com a simulação atual, pois esta comparação já foi feita no item anterior.


			if ((objSimulacaoComMelhorEntradaNaMesmaDataDeSaida != null) && !objSimulacaoComMelhorEntradaNaMesmaDataDeSaida.Equals(objSimulacaoComMelhorEntradaPorAgrupadorDeTentativas)) {
                cCotacaoAbstract objCotacaoParaConverter = ConverterCotacao(pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria, objSimulacaoComMelhorEntradaNaMesmaDataDeSaida);

				if (pobjSimulacaoDiariaDetalhe.IFRSimulacaoDiaria.EhMelhorEntrada(objSimulacaoComMelhorEntradaNaMesmaDataDeSaida,objCotacaoParaConverter)) {
					objDetalheAlterado = objSimulacaoComMelhorEntradaNaMesmaDataDeSaida.Detalhes.First(x => x.IFRSobreVendido.Equals(pobjSimulacaoDiariaDetalhe.IFRSobreVendido));
					objDetalheAlterado.AlterarMelhorEntrada(false);
					objManipuladorDetalhe.Adicionar(objDetalheAlterado, "UPDATE");
				} else {
                    pobjSimulacaoDiariaDetalhe.AlterarMelhorEntrada(false);
				}

			}


			//chama o manipulador de detalhes para atualizar os detalhes que tiveram a coluna "MelhorEntrada" alterada de TRUE para FALSE.
			objManipuladorDetalhe.Executar();

            pobjSimulacaoDiariaDetalhe.AlterarMelhorEntrada(true);

		}

	    private cCotacaoAbstract ConverterCotacao(cIFRSimulacaoDiaria simulacaoDiariaOrigem, cIFRSimulacaoDiaria simulacaoDiariaDestino)
	    {

            var objAjustarCotacao = new cAjustarCotacao(_servicoDeCotacaoDeAtivo);

            //obtém a cotação na data de entrada da simulação recebida por parâmetro
            var servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(simulacaoDiariaOrigem.Ativo, _conexao);

            cCotacaoAbstract objCotacaoParaConverter = servicoDeCotacaoDeAtivo.ObterCotacaoNaData(simulacaoDiariaOrigem.DataEntradaEfetiva);

            //converte a cotação para a data de entrada desta simulação. OBS: se houver splits a função de conversão clona o objeto de cotação 
            //e o que está na lista de cotações do ativo permanece inalterado para não intervir no resultado de outras simulações.
            objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria)objCotacaoParaConverter, simulacaoDiariaDestino.DataEntradaEfetiva);

            return objCotacaoParaConverter;
   
	    }

	}

}
