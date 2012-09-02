using System;
using System.Collections.Generic;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using frwInterface;
namespace ServicosDeInterface
{
	public class GeradorToolTip
	{

		public static string GerarToolTipRelatorioSetupEntrada(string pstrCodigo, cEnum.enumSetup pintIDSetup, cIFRSobrevendido pobjIFRSobrevendido, cEnum.enumClassifMedia pintID_CM, cEnum.enumCriterioClassificacaoMedia pintIDCriterioCM, DateTime pdtmData)
		{

			cConexao objConexao = new cConexao();

			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();
			cCarregadorClassificacaoMedia objCarregadorCM = new cCarregadorClassificacaoMedia();
			cCarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new cCarregadorCriterioClassificacaoMedia();
			cCarregadorIFRDiarioFaixa objCarregadorIFRDiarioFaixa = new cCarregadorIFRDiarioFaixa(objConexao);

			Setup objSetup = objCarregadorSetup.CarregaPorID(pintIDSetup);
			cClassifMedia objCM = objCarregadorCM.CarregaPorID(pintID_CM);
			cCriterioClassifMedia objCriterioCM = objCarregadorCriterioCM.CarregaPorID(pintIDCriterioCM);

			IList<cIFRSimulacaoDiariaFaixa> lstFaixas = objCarregadorIFRDiarioFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(pstrCodigo, objSetup, objCM, objCriterioCM, pobjIFRSobrevendido, pdtmData);

			objConexao.FecharConexao();

			int intI = 0;

			string strDescricao = string.Empty;

			cIFRSimulacaoDiariaFaixa objFaixa = null;


			for (intI = 0; intI <= lstFaixas.Count - 1; intI++) {
				objFaixa = lstFaixas[intI];

				if (strDescricao != string.Empty) {
					strDescricao += Environment.NewLine;
				}

				strDescricao += intI.ToString() + ": [" + objFaixa.ValorMinimo.ToString() + ";" + objFaixa.ValorMaximo.ToString() + "]";

			}

			return strDescricao;

		}

	}
}
