using System;
using System.Collections.Generic;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public class CarregadorIFRSobrevendido
	{


		private readonly Conexao _conexao;
	    private readonly FuncoesBd _funcoesBd;
		public CarregadorIFRSobrevendido(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		    _funcoesBd = pobjConexao.ObterFormatadorDeCampo();
		}

		public IFRSobrevendido CarregaPorValorMaximo(double pdblValorMaximo)
		{

			cRS objRS = new cRS(_conexao);

		    var strSql = "SELECT ID " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSql = strSql + " WHERE ValorMaximo = " + _funcoesBd.CampoFormatar(pdblValorMaximo);

			objRS.ExecuteQuery(strSql);

			IFRSobrevendido objRetorno = null;


			if (!objRS.EOF) {
				objRetorno = new IFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), pdblValorMaximo);

			}

			objRS.Fechar();

			return objRetorno;

		}


		public IList<IFRSobrevendido> CarregarTodos()
		{

			cRS objRS = new cRS(_conexao);

			string strSQL = null;

			strSQL = "SELECT ID, ValorMaximo " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Sobrevendido " + Environment.NewLine;

			objRS.ExecuteQuery(strSQL);

			IList<IFRSobrevendido> lstRetorno = new List<IFRSobrevendido>();


			while (!objRS.EOF) {
				lstRetorno.Add(new IFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), Convert.ToDouble(objRS.Field("ValorMaximo"))));

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
		public IList<IFRSobrevendido> CarregaPorValor(double pdblValor)
		{

			cRS objRS = new cRS(_conexao);

		    var strSql = "SELECT ID, ValorMaximo " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSql = strSql + " WHERE ValorMaximo >= " + _funcoesBd.CampoFormatar(pdblValor);

			objRS.ExecuteQuery(strSql);

			IList<IFRSobrevendido> lstRetorno = new List<IFRSobrevendido>();


			while (!objRS.EOF) {
				lstRetorno.Add(new IFRSobrevendido(Convert.ToInt32(objRS.Field("ID")), Convert.ToDouble(objRS.Field("ValorMaximo"))));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return lstRetorno;

		}

		public IFRSobrevendido CarregaPorID(int pintID)
		{
		    cRS objRS = new cRS(_conexao);

		    var strSql = "SELECT ValorMaximo " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Sobrevendido " + Environment.NewLine;
			strSql = strSql + " WHERE ID = " + _funcoesBd.CampoFormatar(pintID);

			objRS.ExecuteQuery(strSql);

			var functionReturnValue = new IFRSobrevendido(pintID, Convert.ToDouble(objRS.Field("ValorMaximo")));

			objRS.Fechar();
			return functionReturnValue;

		}


	}
}
