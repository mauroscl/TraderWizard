using System.Collections.Generic;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using prjDominio.ValueObjects;

namespace ServicoNegocio
{

	public class VerificaSeDeveGerarEntrada
	{
        
		private readonly Conexao _conexao;
		public VerificaSeDeveGerarEntrada(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		/// <summary>
		/// Recebe a classificação de média atual e os valores dos critérios desta classificação verifica se existe alguma faixa de classificação que contém estes valores
		/// </summary>
		/// <param name="plstFaixasRet"></param>
		/// <returns>
		/// TRUE - somatório dos critérios que não foram atendidos. Se todos os critérios foram atendidos retorna 0.
		/// </returns>
		/// <remarks></remarks>
		public int Verificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO, ValorCriterioClassifMediaVO pobjValorCriterioClassifMediaVO, IList<IFRSimulacaoDiariaFaixa> plstFaixasRet)
		{

			bool blnNumTentativasOK = true;
			bool blnNumTentativasOKAux = false;

			int intSomatorioCriterios = 0;

			CarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new CarregadorCriterioClassificacaoMedia();

			IList<CriterioClassifMedia> lstCriterioCM = objCarregadorCriterioCM.CarregaTodos();

		    VerificaSePossuiFaixaDoIFR objVerificaSePossuiFaixa = new VerificaSePossuiFaixaDoIFR(_conexao);
			VerificaSeValorEstaDentroDaFaixa objVerificaSeValorEstaDentroDaFaixa = new VerificaSeValorEstaDentroDaFaixa(_conexao);


			if (objVerificaSePossuiFaixa.VerificaPorClassificacaoMedia(pobjSimulacaoDiariaVO.Ativo.Codigo, pobjSimulacaoDiariaVO.ClassificacaoMedia, pobjSimulacaoDiariaVO.IFRSobrevendido)) {
				//Inicializa com 0 para indicar que está tudo OK
				intSomatorioCriterios = 0;


				foreach (CriterioClassifMedia objCriterioCM in lstCriterioCM) {
					var objIFRFaixa = objVerificaSeValorEstaDentroDaFaixa.CriterioVerificar(pobjSimulacaoDiariaVO, pobjValorCriterioClassifMediaVO, objCriterioCM, ref blnNumTentativasOKAux);

					if (objIFRFaixa == null) {
						intSomatorioCriterios += objCriterioCM.Peso;
					}

					if ((plstFaixasRet != null)) {
						if (intSomatorioCriterios == 0) {
							plstFaixasRet.Add(objIFRFaixa);
						} else {
							plstFaixasRet.Clear();
						}
					}

					if (!blnNumTentativasOKAux) {
						blnNumTentativasOK = false;
					}

				}


				if (!blnNumTentativasOK) {
					intSomatorioCriterios += 32;

				}

			    VerificaSeAtingiuPercentualMinimo objVerificaPercentualMinimo = new VerificaSeAtingiuPercentualMinimo(_conexao);


				if (!objVerificaPercentualMinimo.Verificar(pobjSimulacaoDiariaVO)) {
					intSomatorioCriterios += 64;
				}

			} else {
				//Ainda não foi rodada a simulação
				intSomatorioCriterios = -1;
			}

			return intSomatorioCriterios;

		}

	}

}
