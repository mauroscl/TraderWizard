using System;
using System.Collections.Generic;
using DataBase.Carregadores;
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

			CarregadorSetup objCarregadorSetup = new CarregadorSetup();
			CarregadorClassificacaoMedia objCarregadorCM = new CarregadorClassificacaoMedia();
			CarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new CarregadorCriterioClassificacaoMedia();
			CarregadorIFRDiarioFaixa objCarregadorIFRDiarioFaixa = new CarregadorIFRDiarioFaixa(objConexao);

			Setup objSetup = objCarregadorSetup.CarregaPorId(pintIdSetup);
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
