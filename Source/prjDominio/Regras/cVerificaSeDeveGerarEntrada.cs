using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Carregadores;
using prjModelo.Entidades;
using DataBase;
using prjModelo.ValueObjects;

namespace prjModelo.Regras
{

	public class cVerificaSeDeveGerarEntrada
	{


		private readonly cConexao objConexao;
		public cVerificaSeDeveGerarEntrada(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		/// <summary>
		/// Recebe a classificação de média atual e os valores dos critérios desta classificação verifica se existe alguma faixa de classificação que contém estes valores
		/// </summary>
		/// <param name="plstFaixasRet"></param>
		/// <returns>
		/// TRUE - somatório dos critérios que não foram atendidos. Se todos os critérios foram atendidos retorna 0.
		/// </returns>
		/// <remarks></remarks>
		public int Verificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO, cValorCriterioClassifMediaVO pobjValorCriterioClassifMediaVO, IList<cIFRSimulacaoDiariaFaixa> plstFaixasRet)
		{

			bool blnNumTentativasOK = true;
			bool blnNumTentativasOKAux = false;

			int intSomatorioCriterios = 0;

			cCarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new cCarregadorCriterioClassificacaoMedia();

			IList<cCriterioClassifMedia> lstCriterioCM = objCarregadorCriterioCM.CarregaTodos();

			cIFRSimulacaoDiariaFaixa objIFRFaixa = null;
			VerificaSePossuiFaixaDoIFR objVerificaSePossuiFaixa = new VerificaSePossuiFaixaDoIFR(objConexao);
			cVerificaSeValorEstaDentroDaFaixa objVerificaSeValorEstaDentroDaFaixa = new cVerificaSeValorEstaDentroDaFaixa(objConexao);


			if (objVerificaSePossuiFaixa.VerificaPorClassificacaoMedia(pobjSimulacaoDiariaVO.Ativo.Codigo, pobjSimulacaoDiariaVO.ClassificacaoMedia, pobjSimulacaoDiariaVO.IFRSobrevendido)) {
				//Inicializa com 0 para indicar que está tudo OK
				intSomatorioCriterios = 0;


				foreach (cCriterioClassifMedia objCriterioCM in lstCriterioCM) {
					objIFRFaixa = objVerificaSeValorEstaDentroDaFaixa.CriterioVerificar(pobjSimulacaoDiariaVO, pobjValorCriterioClassifMediaVO, objCriterioCM, ref blnNumTentativasOKAux);

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

				cVerificaSeAtingiuPercentualMinimo objVerificaPercentualMinimo = new cVerificaSeAtingiuPercentualMinimo(objConexao);


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
