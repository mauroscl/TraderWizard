using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;
using prjDominio.ValueObjects;
using Services;

namespace ServicoNegocio
{
	public class CalculadorDeTentativas
	{

	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;

	    public CalculadorDeTentativas(ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
	    {
	        _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
	    }

	    public TentativaVO Calcular(IFRSimulacaoDiaria pobjSimulacaoParaCalcular, IFRSobrevendido pobjIFRSobreVendido)
		{

			IFRSimulacaoDiaria objSimulacaoAnterior = _servicoDeCotacaoDeAtivo.BuscaSimulacaoAnterior(pobjIFRSobreVendido, pobjSimulacaoParaCalcular.DataEntradaEfetiva);

			uint intAgrupadorDeTentativas;
			byte bytNumTentativas;
			bool blnGerouNovoAgrupadorDeTentativas;

			if ((objSimulacaoAnterior != null)) {
				//busca o detalhe da simulação anterior do IFR sobrevendido recebido por parâmetro
				cIFRSimulacaoDiariaDetalhe objDetalheAnterior = objSimulacaoAnterior.Detalhes.Single(x => x.IFRSobreVendido.Equals(pobjIFRSobreVendido));

				//inicializa o agrupador de tentativas e o número de tentativas com os valores da simulação anterior
				intAgrupadorDeTentativas = objDetalheAnterior.AgrupadorDeTentativas;
				bytNumTentativas = objDetalheAnterior.NumTentativas;

				var lngSequencialInicial = objSimulacaoAnterior.Sequencial;

				IList<CotacaoDiaria> lstCotacoesEntreSimulacoes = null;

				//Busca as cotações entre a data efetiva da simulação anterior e a data efetiva da simulação atual (inclusive)
				lstCotacoesEntreSimulacoes = _servicoDeCotacaoDeAtivo.CotacoesDiarias.Where(c => c.IFR.Valor <= pobjIFRSobreVendido.ValorMaximo 
                    && c.Data > objDetalheAnterior.IFRSimulacaoDiaria.DataEntradaEfetiva && c.Data <= pobjSimulacaoParaCalcular.DataEntradaEfetiva).ToList();


				foreach (CotacaoDiaria objCotacao in lstCotacoesEntreSimulacoes) {
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

			return new TentativaVO(intAgrupadorDeTentativas, bytNumTentativas, blnGerouNovoAgrupadorDeTentativas);

		}

	}
}
