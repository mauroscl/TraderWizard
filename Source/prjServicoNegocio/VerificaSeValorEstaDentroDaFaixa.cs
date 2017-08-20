using System.Collections.Generic;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using prjDominio.Regras;
using prjDominio.ValueObjects;

namespace ServicoNegocio
{

	public class VerificaSeValorEstaDentroDaFaixa
	{


		private readonly Conexao _conexao;
		public VerificaSeValorEstaDentroDaFaixa(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		/// <summary>
		/// Verifica se o valor atual está dentro da faixa calculada
		/// </summary>
		/// <param name="pobjSimulacaoDiariaVO">dados da simulação que deve ser verificada se está dentro da faixa.</param>
		/// <param name="pobjCriterioCM">critério da simulação que deve ser verificado</param>
		/// <param name="pblnNumTentativasOK">Retorna TRUE se o número de tentativas realizados até o momento é maior ou igual ao número de tentativas mínimo da faixa. 
		/// Caso não seja encontrada nenhuma faixa o parâmetro também retorna TRUE</param>
		/// <returns>Retorna a faixa correspondente ao critério de classificação de média recebido por parâmetro (pobjCriterioCM). 
		/// Caso o valor do critério não se enquadre em nenhum faixa retorna "Nothing"</returns>
		/// <remarks></remarks>
		public IFRSimulacaoDiariaFaixa CriterioVerificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO, ValorCriterioClassifMediaVO pobjValorCriterioClassifMediaVO, CriterioClassifMedia pobjCriterioCM, ref bool pblnNumTentativasOK)
		{

			var objCarregadorFaixa = new CarregadorIFRDiarioFaixa(_conexao);

			IFRSimulacaoDiariaFaixa objRetorno = null;

			IList<IFRSimulacaoDiariaFaixa> lstFaixas = objCarregadorFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(pobjSimulacaoDiariaVO.Ativo.Codigo, pobjSimulacaoDiariaVO.Setup, pobjSimulacaoDiariaVO.ClassificacaoMedia, pobjCriterioCM, pobjSimulacaoDiariaVO.IFRSobrevendido, pobjSimulacaoDiariaVO.DataEntradaEfetiva);

			pblnNumTentativasOK = true;

			System.Double dblValorCriterio = cObterValorCriterioClassificacaoMedia.ObterValor(pobjValorCriterioClassifMediaVO, pobjCriterioCM);


			foreach (IFRSimulacaoDiariaFaixa objIFRFaixa in lstFaixas) {

				if (dblValorCriterio >= objIFRFaixa.ValorMinimo && dblValorCriterio <= objIFRFaixa.ValorMaximo) {
					objRetorno = objIFRFaixa;

					if (pobjSimulacaoDiariaVO.NumTentativas >= objIFRFaixa.NumTentativasMinimo) {
						pblnNumTentativasOK = true;
					} else {
						pblnNumTentativasOK = false;
					}

				}

			}

			return objRetorno;

		}
	}
}
