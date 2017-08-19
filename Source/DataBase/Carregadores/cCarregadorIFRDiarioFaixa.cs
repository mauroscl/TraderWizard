using System;
using System.Collections.Generic;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
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

		private void CalcularUltimaData(string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, CriterioClassifMedia pobjCriterioCM, IFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{
			dtmDataSolicitacao = pdtmData;

			cRS objRS = new cRS(objConexao);

            FuncoesBd funcoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSql = "SELECT MAX(Data) AS Data " + Environment.NewLine;
			strSql += " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSql += " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSql += " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSql += " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSql += " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSql += " AND Data <= " + funcoesBd.CampoFormatar(dtmDataSolicitacao);

			if ((pobjCriterioCM != null)) {
				strSql += " AND ID_Criterio_CM = " + FuncoesBd.CampoFormatar(pobjCriterioCM.ID);
			}

			objRS.ExecuteQuery(strSql);

			dtmUltimaData = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

			objRS.Fechar();

		}

		public IList<IFRSimulacaoDiariaFaixa> CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, CriterioClassifMedia pobjCriterioCM, IFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{

			if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido, pdtmData);
			}

			IList<IFRSimulacaoDiariaFaixa> objListaRetorno = new List<IFRSimulacaoDiariaFaixa>();

			cRS objRS = new cRS(objConexao);

            FuncoesBd funcoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSql = "SELECT ID, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSql = strSql + " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSql = strSql + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSql = strSql + " AND ID_Criterio_CM = " + FuncoesBd.CampoFormatar(pobjCriterioCM.ID);
			strSql = strSql + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSql = strSql + " AND Data = " + funcoesBd.CampoFormatar(dtmUltimaData);

			objRS.ExecuteQuery(strSql);


			while (!objRS.EOF) {
				objListaRetorno.Add(new IFRSimulacaoDiariaFaixa(Convert.ToInt64(objRS.Field("ID")), pstrCodigo, pobjSetup, pobjCM, pobjCriterioCM, Convert.ToInt32(objRS.Field("NumTentativas_Minimo")), Convert.ToDouble(objRS.Field("Valor_Minimo")), Convert.ToDouble(objRS.Field("Valor_Maximo"))));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return objListaRetorno;

		}

		public bool ExisteFaixaParaCriterio(string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, IFRSobrevendido pobjIFRSobrevendido, DateTime pdtmData)
		{
		    cRS objRS = new cRS(objConexao);

		    if (dtmUltimaData != pdtmData) {
				CalcularUltimaData(pstrCodigo, pobjSetup, pobjCM, null, pobjIFRSobrevendido, pdtmData);
			}

            FuncoesBd funcoesBd = objConexao.ObterFormatadorDeCampo();

			string strSql = "SELECT COUNT(1) AS Contador " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Simulacao_Diaria_Faixa F1 " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSql = strSql + " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strSql = strSql + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSql = strSql + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
			strSql = strSql + " AND Data = " + funcoesBd.CampoFormatar(dtmUltimaData);

			objRS.ExecuteQuery(strSql);

			bool functionReturnValue = (Convert.ToInt16(objRS.Field("Contador")) > 0);

			objRS.Fechar();
			return functionReturnValue;

		}

	}
}
