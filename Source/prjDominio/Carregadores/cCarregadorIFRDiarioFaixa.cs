using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;
using frwInterface;

namespace prjModelo.Carregadores
{
	public class cCarregadorIFRDiarioFaixa
	{

		private readonly cConexao objConexao;
		private DateTime dtmDataSolicitacao;

		private DateTime dtmUltimaData;
		public cCarregadorIFRDiarioFaixa(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pobjSetup"></param>
		/// <param name="pobjCM"></param>
		/// <param name="pobjCriterioCM"></param>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="pdtmData"></param>
		/// <remarks></remarks>

		private void CalcularUltimaData(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioCM, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{
			dtmDataSolicitacao = pdtmData;

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT MAX(Data) AS Data " + Environment.NewLine;
			strSQL += " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL += " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL += " AND ID_CM = " + FuncoesBD.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL += " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID) + Environment.NewLine;
			strSQL += " AND ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID);
			strSQL += " AND Data <= " + FuncoesBD.CampoFormatar(dtmDataSolicitacao);

			if ((pobjCriterioCM != null)) {
				strSQL += " AND ID_Criterio_CM = " + FuncoesBD.CampoFormatar(pobjCriterioCM.ID);
			}

			objRS.ExecuteQuery(strSQL);

			dtmUltimaData = Convert.ToDateTime(objRS.Field("Data", frwInterface.cConst.DataInvalida));

			objRS.Fechar();

		}

		public IList<cIFRSimulacaoDiariaFaixa> CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioCM, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{

			if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido, pdtmData);
			}

			IList<cIFRSimulacaoDiariaFaixa> objListaRetorno = new List<cIFRSimulacaoDiariaFaixa>();

			cRS objRS = new cRS(objConexao);
			string strSQL = null;

			strSQL = "SELECT ID, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBD.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Criterio_CM = " + FuncoesBD.CampoFormatar(pobjCriterioCM.ID);
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID);
			strSQL = strSQL + " AND Data = " + FuncoesBD.CampoFormatar(dtmUltimaData);

			objRS.ExecuteQuery(strSQL);


			while (!objRS.EOF) {
				objListaRetorno.Add(new cIFRSimulacaoDiariaFaixa(Convert.ToInt64(objRS.Field("ID")), pstrCodigo, pobjSetup, pobjCM, pobjCriterioCM, Convert.ToInt32(objRS.Field("NumTentativas_Minimo")), Convert.ToDouble(objRS.Field("Valor_Minimo")), Convert.ToDouble(objRS.Field("Valor_Maximo"))));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return objListaRetorno;

		}

		public bool ExisteFaixaParaCriterio(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{
			bool functionReturnValue = false;

			cRS objRS = new cRS(objConexao);
			string strSQL = null;

			if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, null, pobjIFRSobrevendido, pdtmData);
			}

			strSQL = "SELECT COUNT(1) AS Contador " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBD.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID);
			strSQL = strSQL + " AND Data = " + FuncoesBD.CampoFormatar(dtmUltimaData);

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = (Convert.ToInt16(objRS.Field("Contador")) > 0);

			objRS.Fechar();
			return functionReturnValue;

		}

	}
}
