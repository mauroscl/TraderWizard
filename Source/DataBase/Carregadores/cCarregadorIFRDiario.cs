using System;
using DataBase.Interfaces;
using prjDominio.Entidades;
using prjModelo.Entidades;

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

		    FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			string strSQL = "SELECT Valor " + Environment.NewLine;
			strSQL += " FROM IFR_Diario " + Environment.NewLine;
			strSQL += " WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSQL += " AND Data = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSQL += " AND NumPeriodos = " + FuncoesBd.CampoFormatar(pintNumPeriodos);

			objRS.ExecuteQuery(strSQL);

			cIFR functionReturnValue = new cIFRDiario(pobjCotacaoDiaria, pintNumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}
	}
}
