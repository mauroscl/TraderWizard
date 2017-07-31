using System;
using System.Collections.Generic;
using prjDominio.Entidades;

namespace DataBase.Carregadores
{

	public class cCarregadorIFRSobrevendido
	{


		private readonly Conexao objConexao;
		public cCarregadorIFRSobrevendido(Conexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public cIFRSobrevendido CarregaPorValorMaximo(double pdblValorMaximo)
		{

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT ID " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSQL = strSQL + " WHERE ValorMaximo = " + FuncoesBd.CampoFormatar(pdblValorMaximo);

			objRS.ExecuteQuery(strSQL);

			cIFRSobrevendido objRetorno = null;


			if (!objRS.EOF) {
				objRetorno = new cIFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), pdblValorMaximo);

			}

			objRS.Fechar();

			return objRetorno;

		}


		public IList<cIFRSobrevendido> CarregarTodos()
		{

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT ID, ValorMaximo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Sobrevendido " + Environment.NewLine;

			objRS.ExecuteQuery(strSQL);

			IList<cIFRSobrevendido> lstRetorno = new List<cIFRSobrevendido>();


			while (!objRS.EOF) {
				lstRetorno.Add(new cIFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), Convert.ToDouble(objRS.Field("ValorMaximo"))));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return lstRetorno;

		}

		/// <summary>
		/// Retorna todos os IFR Sobrevendido cujo valor máximo seja maior que o valor de IFR recebido por parâmetro
		/// </summary>
		/// <param name="pdblValor"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public IList<cIFRSobrevendido> CarregaPorValor(double pdblValor)
		{

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT ID, ValorMaximo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSQL = strSQL + " WHERE ValorMaximo >= " + FuncoesBd.CampoFormatar(pdblValor);

			objRS.ExecuteQuery(strSQL);

			IList<cIFRSobrevendido> lstRetorno = new List<cIFRSobrevendido>();


			while (!objRS.EOF) {
				lstRetorno.Add(new cIFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), Convert.ToDouble(objRS.Field("ValorMaximo"))));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return lstRetorno;

		}

		public cIFRSobrevendido CarregaPorID(int pintID)
		{
			cIFRSobrevendido functionReturnValue = null;

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT ValorMaximo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSQL = strSQL + " WHERE ID = " + FuncoesBd.CampoFormatar(pintID);

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = new cIFRSobrevendido(pintID, Convert.ToDouble(objRS.Field("ValorMaximo")));

			objRS.Fechar();
			return functionReturnValue;

		}


	}
}
