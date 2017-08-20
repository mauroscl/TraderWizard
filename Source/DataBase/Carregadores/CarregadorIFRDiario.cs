using System;
using DataBase.Interfaces;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public class CarregadorIFRDiario : CarregadorGenerico, ICarregadorIFR
	{

		public CarregadorIFRDiario(Conexao pobjConexao) : base(pobjConexao)
		{
		}

		public CarregadorIFRDiario()
		{
		}

		public cIFR CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos)
		{
		    cRS objRS = new cRS(Conexao);

		    FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

			string strSql = "SELECT Valor " + Environment.NewLine;
			strSql += " FROM IFR_Diario " + Environment.NewLine;
			strSql += " WHERE Codigo = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSql += " AND Data = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSql += " AND NumPeriodos = " + funcoesBd.CampoFormatar(pintNumPeriodos);

			objRS.ExecuteQuery(strSql);

			cIFR functionReturnValue = new cIFRDiario(pobjCotacaoDiaria, pintNumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}
	}
}
