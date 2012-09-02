using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using prjModelo.ValueObjects;

namespace prjModelo.Regras
{
	public class cCalculadorDeTentativas
	{

		public cTentativaVO Calcular(cIFRSimulacaoDiaria pobjSimulacaoParaCalcular, cIFRSobrevendido pobjIFRSobreVendido)
		{

			//criar um DTO para retornar estas 3 propriedades

			cIFRSimulacaoDiaria objSimulacaoAnterior = pobjSimulacaoParaCalcular.BuscaSimulacaoAnterior(pobjIFRSobreVendido);

			uint intAgrupadorDeTentativas;
			byte bytNumTentativas;
			bool blnGerouNovoAgrupadorDeTentativas = false;


			if ((objSimulacaoAnterior != null)) {
				//busca o detalhe da simulação anterior do IFR sobrevendido recebido por parâmetro
				cIFRSimulacaoDiariaDetalhe objDetalheAnterior = objSimulacaoAnterior.Detalhes.Where(x => x.IFRSobreVendido.Equals(pobjIFRSobreVendido)).Single();

				//inicializa o agrupador de tentativas e o número de tentativas com os valores da simulação anterior
				intAgrupadorDeTentativas = objDetalheAnterior.AgrupadorDeTentativas;
				bytNumTentativas = objDetalheAnterior.NumTentativas;

				var lngSequencialInicial = objSimulacaoAnterior.Sequencial;

				IList<cCotacaoDiaria> lstCotacoesEntreSimulacoes = null;

				//Busca as cotações entre a data efetiva da simulação anterior e a data efetiva da simulação atual (inclusive)
				lstCotacoesEntreSimulacoes = objSimulacaoAnterior.Ativo.CotacoesDiarias.Where(c => c.IFR.Valor <= pobjIFRSobreVendido.ValorMaximo & c.Data > objDetalheAnterior.IFRSimulacaoDiaria.DataEntradaEfetiva & c.Data <= pobjSimulacaoParaCalcular.DataEntradaEfetiva).ToList();


				foreach (cCotacaoDiaria objCotacao in lstCotacoesEntreSimulacoes) {
					if (objCotacao.Sequencial - lngSequencialInicial <= 2) {
						//Se o sequencial tem no máximo dois períodos de diferença continua sendo do mesmo sequencial
						//intAgrupadorDeTentativas = objDetalheAnterior.AgrupadorDeTentativas
                        bytNumTentativas += Convert.ToByte(objCotacao.Sequencial - lngSequencialInicial);
					} else {
						//Gera novo agrupador
						intAgrupadorDeTentativas = objDetalheAnterior.AgrupadorDeTentativas + 1;
						bytNumTentativas = 1;
					}

					lngSequencialInicial = objCotacao.Sequencial;

				}

				blnGerouNovoAgrupadorDeTentativas = (objDetalheAnterior.AgrupadorDeTentativas != intAgrupadorDeTentativas);

			} else {
				intAgrupadorDeTentativas = 1;
				blnGerouNovoAgrupadorDeTentativas = true;
				bytNumTentativas = 1;
			}

			return new cTentativaVO(intAgrupadorDeTentativas, bytNumTentativas, blnGerouNovoAgrupadorDeTentativas);

		}

	}
}
