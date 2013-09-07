using System;
using System.Collections.Generic;
using DataBase.Carregadores;
using prjDominio.Entidades;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using TraderWizard.Enumeracoes;

namespace ServicosDeInterface
{
	public class GeradorToolTip
	{

		public static string GerarToolTipRelatorioSetupEntrada(string pstrCodigo, cEnum.enumSetup pintIdSetup, cIFRSobrevendido pobjIfrSobrevendido, 
            cEnum.enumClassifMedia pintIdClassificaoMedia, cEnum.enumCriterioClassificacaoMedia pintIdCriterioClassificacaoMedia, DateTime pdtmData)
		{

			cConexao objConexao = new cConexao();

			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();
			cCarregadorClassificacaoMedia objCarregadorCM = new cCarregadorClassificacaoMedia();
			cCarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new cCarregadorCriterioClassificacaoMedia();
			cCarregadorIFRDiarioFaixa objCarregadorIFRDiarioFaixa = new cCarregadorIFRDiarioFaixa(objConexao);

			Setup objSetup = objCarregadorSetup.CarregaPorID(pintIdSetup);
			cClassifMedia objCM = objCarregadorCM.CarregaPorID(pintIdClassificaoMedia);
			cCriterioClassifMedia objCriterioCM = objCarregadorCriterioCM.CarregaPorID(pintIdCriterioClassificacaoMedia);

			IList<cIFRSimulacaoDiariaFaixa> lstFaixas = objCarregadorIFRDiarioFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(pstrCodigo, objSetup, objCM, objCriterioCM, pobjIfrSobrevendido, pdtmData);

			objConexao.FecharConexao();

			string strDescricao = string.Empty;


		    for (int intI = 0; intI <= lstFaixas.Count - 1; intI++) {
				cIFRSimulacaoDiariaFaixa objFaixa = lstFaixas[intI];

				if (strDescricao != string.Empty) {
					strDescricao += Environment.NewLine;
				}

				strDescricao += intI.ToString() + ": [" + objFaixa.ValorMinimo.ToString() + ";" + objFaixa.ValorMaximo.ToString() + "]";

			}

			return strDescricao;

		}

	}
}
