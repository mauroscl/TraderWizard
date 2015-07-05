using System;
using System.Collections.Generic;
using DataBase;
using prjDominio.Entidades;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{
	public class cCarregadorIFRDiarioFaixa
	{

		private readonly Conexao objConexao;
		private DateTime dtmDataSolicitacao;

		private DateTime dtmUltimaData;
		public cCarregadorIFRDiarioFaixa(Conexao pobjConexao)
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

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT MAX(Data) AS Data " + Environment.NewLine;
			strSQL += " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL += " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL += " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL += " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSQL += " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSQL += " AND Data <= " + FuncoesBd.CampoFormatar(dtmDataSolicitacao);

			if ((pobjCriterioCM != null)) {
				strSQL += " AND ID_Criterio_CM = " + FuncoesBd.CampoFormatar(pobjCriterioCM.ID);
			}

			objRS.ExecuteQuery(strSQL);

			dtmUltimaData = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

			objRS.Fechar();

		}

		public IList<cIFRSimulacaoDiariaFaixa> CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioCM, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{

			if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido, pdtmData);
			}

			IList<cIFRSimulacaoDiariaFaixa> objListaRetorno = new List<cIFRSimulacaoDiariaFaixa>();

			cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT ID, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Criterio_CM = " + FuncoesBd.CampoFormatar(pobjCriterioCM.ID);
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSQL = strSQL + " AND Data = " + FuncoesBd.CampoFormatar(dtmUltimaData);

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
		    cRS objRS = new cRS(objConexao);

		    if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, null, pobjIFRSobrevendido, pdtmData);
			}

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			string strSQL = "SELECT COUNT(1) AS Contador " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSQL = strSQL + " AND Data = " + FuncoesBd.CampoFormatar(dtmUltimaData);

			objRS.ExecuteQuery(strSQL);

			bool functionReturnValue = (Convert.ToInt16(objRS.Field("Contador")) > 0);

			objRS.Fechar();
			return functionReturnValue;

		}

	}
}
