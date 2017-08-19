using System;
using System.Collections.Generic;
using DataBase.Carregadores;
using prjModelo.Carregadores;
using DataBase;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace ServicosDeInterface
{
	public class GeradorToolTip
	{

		public static string GerarToolTipRelatorioSetupEntrada(string pstrCodigo, cEnum.enumSetup pintIdSetup, IFRSobrevendido pobjIfrSobrevendido, 
            cEnum.enumClassifMedia pintIdClassificaoMedia, cEnum.enumCriterioClassificacaoMedia pintIdCriterioClassificacaoMedia, DateTime pdtmData)
		{

			Conexao objConexao = new Conexao();

			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();
			cCarregadorClassificacaoMedia objCarregadorCM = new cCarregadorClassificacaoMedia();
			cCarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new cCarregadorCriterioClassificacaoMedia();
			cCarregadorIFRDiarioFaixa objCarregadorIFRDiarioFaixa = new cCarregadorIFRDiarioFaixa(objConexao);

			Setup objSetup = objCarregadorSetup.CarregaPorID(pintIdSetup);
			ClassifMedia objCM = objCarregadorCM.CarregaPorID(pintIdClassificaoMedia);
			CriterioClassifMedia objCriterioCM = objCarregadorCriterioCM.CarregaPorID(pintIdCriterioClassificacaoMedia);

			IList<IFRSimulacaoDiariaFaixa> lstFaixas = objCarregadorIFRDiarioFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(pstrCodigo, objSetup, objCM, objCriterioCM, pobjIfrSobrevendido, pdtmData);

			objConexao.FecharConexao();

			string strDescricao = string.Empty;


		    for (int intI = 0; intI <= lstFaixas.Count - 1; intI++) {
				IFRSimulacaoDiariaFaixa objFaixa = lstFaixas[intI];

				if (strDescricao != string.Empty) {
					strDescricao += Environment.NewLine;
				}

				strDescricao += intI.ToString() + ": [" + objFaixa.ValorMinimo.ToString() + ";" + objFaixa.ValorMaximo.ToString() + "]";

			}

			return strDescricao;

		}

	}
}
