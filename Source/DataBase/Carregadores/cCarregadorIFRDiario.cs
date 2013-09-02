using DataBase.Interfaces;
using System;
using DataBase;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public class cCarregadorIFRDiario : cCarregadorGenerico, ICarregadorIFR
	{

		public cCarregadorIFRDiario(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorIFRDiario() : base()
		{
		}


		public cIFR CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos)
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
