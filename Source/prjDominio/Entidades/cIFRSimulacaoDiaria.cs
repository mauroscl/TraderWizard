using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjDTO;
using prjModelo.DomainServices;
using prjModelo.Regras;

namespace prjModelo.Entidades
{

	public class cIFRSimulacaoDiaria : cModelo
	{


		private readonly cConexao objConexao;

		public System.DateTime? DataCruzamentoMedia = null;
		public cAtivo Ativo { get; set; }

		public Setup Setup { get; set; }
		public cClassifMedia ClassificacaoMedia { get; set; }
		public DateTime DataEntradaEfetiva { get; set; }
		public Int32 Sequencial { get; set; }
		public System.Decimal ValorEntradaOriginal { get; set; }
		public System.Decimal ValorEntradaAjustado { get; set; }
		public System.Double ValorIFR { get; set; }
		public System.Decimal ValorMaximo { get; set; }
		public System.Decimal PercentualMaximo { get; set; }
		public DateTime DataSaida { get; set; }
		public System.Decimal ValorSaida { get; set; }
		public System.Decimal PercentualSaida { get; set; }
		public System.Decimal PercentualMME21 { get; set; }
		public System.Decimal PercentualMME49 { get; set; }
		public System.Decimal PercentualMME200 { get; set; }
		public System.Decimal ValorStopLossInicial { get; set; }
		public System.Boolean Verdadeiro { get; set; }
		public System.Decimal ValorRealizacaoParcial { get; set; }
		public int ValorAmplitude { get; set; }
		public System.Decimal ValorFechamentoMinimo { get; set; }
		public double? MediaIFR { get; set; }
		public double? ValorMME21Minima { get; set; }
		public double? ValorMME49Minima { get; set; }

		public IList<cIFRSimulacaoDiariaDetalhe> Detalhes { get; set; }

		public cIFRSimulacaoDiaria(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
			Detalhes = new List<cIFRSimulacaoDiariaDetalhe>();
		}


		public cIFRSimulacaoDiaria(cConexao pobjConexao, cAtivo pobjAtivo, Setup pobjSetup, cCotacaoDiaria pobjCotacaoDeAcionamentoDoSetup, cCotacaoDiaria pobjCotacaoDeEntrada, cCotacaoDiaria pobjCotacaoDoValorMaximo, cCotacaoDiaria pobjCotacaoDeSaida, InformacoesDoTradeDTO pobjInformacoesDoTradeDTO, IList<cIFRSobrevendido> plstIFRSobrevendido)
		{
			objConexao = pobjConexao;
			Detalhes = new List<cIFRSimulacaoDiariaDetalhe>();

			Ativo = pobjAtivo;
			Setup = pobjSetup;

			ClassificacaoMedia = cUtil.ClassifMediaCalcular(pobjCotacaoDeEntrada);
			DataEntradaEfetiva = pobjCotacaoDeEntrada.Data;
			Sequencial = pobjCotacaoDeEntrada.Sequencial;
			ValorEntradaOriginal = pobjInformacoesDoTradeDTO.ValorDeEntradaOriginal;

			ValorEntradaAjustado = Setup.CalculaValorEntrada(pobjCotacaoDeEntrada);

			if (pobjSetup.TemFiltro) {
				ValorIFR = (double) pobjInformacoesDoTradeDTO.ValorIFRMinimo;
			} else {
				ValorIFR = pobjCotacaoDeAcionamentoDoSetup.IFR.Valor;
			}


			if (pobjCotacaoDeEntrada.Data == pobjCotacaoDeSaida.Data) {
				//se entrou e saiu na mesma data considera como valor máximo o valor de entrada
				ValorMaximo = ValorEntradaAjustado;


			} else {
				//se a entrada e a saída foram em dias diferentes

				//Neste caso, nunca se sabe se a cotação atingiu o valor máximo deste candle
				//antes de estopar (cotação de saída). Então consideramos apenas o valor de abertura 
				//para comparar com o valor máximo.

				if ((pobjCotacaoDoValorMaximo == null) || pobjCotacaoDeSaida.ValorAbertura > pobjCotacaoDoValorMaximo.ValorMaximo) {
					ValorMaximo = pobjCotacaoDeSaida.ValorAbertura;


				} else {
					ValorMaximo = pobjCotacaoDoValorMaximo.ValorMaximo;

				}

			}

			PercentualMaximo = (ValorMaximo / ValorEntradaAjustado - 1) * 100;

			DataSaida = pobjCotacaoDeSaida.Data;

			if (pobjCotacaoDeSaida.ValorAbertura < pobjInformacoesDoTradeDTO.ValorDoStopLoss) {
				//Se abriu em gap e o valor de abertura for menor do que o stop loss,
				//então o valor de saida já é o valor de abertura
				ValorSaida = pobjCotacaoDeSaida.ValorAbertura;
			} else {
				ValorSaida = pobjInformacoesDoTradeDTO.ValorDoStopLoss;
			}

			PercentualSaida = (ValorSaida / ValorEntradaAjustado - 1) * 100;

			PercentualMME21 = pobjCotacaoDeEntrada.CalculaPercentualDoFechamentoEmRelacaoAMedia(new cMediaDTO("E", 21, "VALOR"));
			PercentualMME49 = pobjCotacaoDeEntrada.CalculaPercentualDoFechamentoEmRelacaoAMedia(new cMediaDTO("E", 49, "VALOR"));
			PercentualMME200 = pobjCotacaoDeEntrada.CalculaPercentualDoFechamentoEmRelacaoAMedia(new cMediaDTO("E", 200, "VALOR"));

			ValorStopLossInicial = pobjSetup.CalculaValorStopLossInicial(pobjCotacaoDeAcionamentoDoSetup);

			ValorRealizacaoParcial = pobjInformacoesDoTradeDTO.ValorRealizacaoParcial;
			Verdadeiro = (ValorMaximo >= ValorRealizacaoParcial);
			ValorAmplitude = (int) pobjCotacaoDeAcionamentoDoSetup.Amplitude;
			MediaIFR = pobjCotacaoDeAcionamentoDoSetup.Medias.Single(x => x.Tipo == "IFR2" && x.NumPeriodos == 13).Valor;

			ValorFechamentoMinimo =  pobjInformacoesDoTradeDTO.ValorFechamentoMinimo.Value;
			ValorMME21Minima = pobjInformacoesDoTradeDTO.MME21Minima;
			ValorMME49Minima = pobjInformacoesDoTradeDTO.MME49Minima;

			cCalculadorIFRSimulacaoDiariaDetalhe objCalculadorDetalhe = new cCalculadorIFRSimulacaoDiariaDetalhe(objConexao);
			objCalculadorDetalhe.CalcularDetalhes(this, plstIFRSobrevendido);

		}

		public decimal PercentualMME200MME49 {
			get { return PercentualMME200 - PercentualMME49; }
		}

		public decimal PercentualMME200MME21 {
			get { return PercentualMME200 - PercentualMME21; }
		}

		public override bool Equals(object obj)
		{

			if ((obj == null)) {
				return false;
			}

			var objIFRSimulacaoDiaria = (cIFRSimulacaoDiaria)obj;

			if (Ativo.Equals(objIFRSimulacaoDiaria.Ativo) && Setup.Equals(objIFRSimulacaoDiaria.Setup) && DataEntradaEfetiva == objIFRSimulacaoDiaria.DataEntradaEfetiva) {
				return true;
			} else {
				return false;
			}


		}

		/// <summary>
		///  Verifica se esta simulação é melhor entrada (valor de fechamento mais baixo) do que a simulação recebida por parâmetro. 
		/// </summary>
		/// <param name="pobjOutraSimulacao"></param>
		/// <returns>
		/// TRUE - Esta simulação é melhor entrada do que a simulação recebida por parâmetro
		/// FALSE - A simulação recebida por parâmetro é melhor entrada do que esta simulação
		/// </returns>
		/// <remarks>Para fazer a comparação sao considerados os desdobramentos</remarks>
		public bool EhMelhorEntrada(cIFRSimulacaoDiaria pobjOutraSimulacao)
		{

			cAjustarCotacao objAjustarCotacao = new cAjustarCotacao();

			//Dim decValorConvertido As System.Decimal

			//converte o valor de entrada da simulação recebida por parâmetro para a mesma data de entrada desta simulação
			//decValorConvertido = objAjustarCotacao.ConverterCotacaoParaData(Ativo.Codigo, pobjOutraSimulacao.DataEntradaEfetiva, pobjOutraSimulacao.ValorEntradaOriginal, DataEntradaEfetiva)

			//obtém a cotação na data de entrada da simulação recebida por parâmetro
			var objCotacaoParaConverter = pobjOutraSimulacao.Ativo.ObterCotacaoNaData(pobjOutraSimulacao.DataEntradaEfetiva);

			//converte a cotação para a data de entrada desta simulação. OBS: se houver splits a função de conversão clona o objeto de cotação 
			//e o que está na lista de cotações do ativo permanece inalterado para não intervir no resultado de outras simulações.
			objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria) objCotacaoParaConverter, DataEntradaEfetiva);

			//o setup calcula qual seria o valor de entrada com os valores convertidos
			var decValorDeEntradaDaOutraSimulacaoConvertido = pobjOutraSimulacao.Setup.CalculaValorEntrada(objCotacaoParaConverter);

			//compara os valores
			if (ValorEntradaOriginal < decValorDeEntradaDaOutraSimulacaoConvertido) {
				return true;
			} else if (ValorEntradaOriginal > decValorDeEntradaDaOutraSimulacaoConvertido) {
				return false;
			} else {
				//se as duas entradas tem o mesmo valor, a melhor entrada é a de maior data.
				return DataEntradaEfetiva > pobjOutraSimulacao.DataEntradaEfetiva;
			}

		}

		public cIFRSimulacaoDiaria BuscaSimulacaoAnterior(cIFRSobrevendido pobjIFRSobrevendido)
		{

			return (from s in Ativo.Simulacoes from d in s.Value.Detalhes where s.Value.DataEntradaEfetiva < DataEntradaEfetiva && d.IFRSobreVendido.Equals(pobjIFRSobrevendido) select s ).LastOrDefault().Value;

		}



	}

}