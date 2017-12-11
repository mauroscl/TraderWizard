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
		    RS objRS = new RS(Conexao);

		    FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

			string strSql = "SELECT Negocios " + Environment.NewLine;
			strSql += " FROM IFR_Diario " + Environment.NewLine;
			strSql += " WHERE Codigo = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSql += " AND Data = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSql += " AND NumPeriodos = " + funcoesBd.CampoFormatar(pintNumPeriodos);

			objRS.ExecuteQuery(strSql);

			cIFR functionReturnValue = new cIFRDiario(pobjCotacaoDiaria, pintNumPeriodos, Convert.ToDouble(objRS.Field("Negocios")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}
	}
}
