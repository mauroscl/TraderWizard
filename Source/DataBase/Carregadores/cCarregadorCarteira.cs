using System;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public class CarregadorCarteira : CarregadorGenerico
	{

		public CarregadorCarteira(Conexao pobjConexao) : base(pobjConexao)
		{
		}

		public Carteira CarregaAtiva(IFRSobrevendido pobjIFRSobrevendido)
		{
			Carteira functionReturnValue = null;

			var strSQL = "SELECT IdCarteira, Descricao, Ativo, Data_Inicio, Data_Fim " + Environment.NewLine;
			strSQL += " FROM Carteira " + Environment.NewLine;
			strSQL += " WHERE ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;
			strSQL += " AND Ativo = " + FuncoesBd.CampoFormatar(true);

			var objRS = new cRS(Conexao);

			objRS.ExecuteQuery(strSQL);


			if (objRS.DadosExistir) {
				var objRetorno = new Carteira(Convert.ToInt32(objRS.Field("IdCarteira")), Convert.ToString(objRS.Field("Descricao"))
                    , pobjIFRSobrevendido, true, Convert.ToDateTime(objRS.Field("Data_Inicio")), Convert.ToDateTime(objRS.Field("Data_Fim")));

				objRS.Fechar();

				strSQL = "SELECT Codigo " + Environment.NewLine;
				strSQL += " FROM Carteira_Ativo " + Environment.NewLine;
				strSQL += " WHERE IdCarteira = " + FuncoesBd.CampoFormatar(objRetorno.IdCarteira);

				objRS.ExecuteQuery(strSQL);


				while (!objRS.EOF) {
					var objAtivo = new Ativo(Convert.ToString(objRS.Field("Codigo")), string.Empty);

					objRetorno.AdicionaAtivo(objAtivo);

					objRS.MoveNext();

				}

				functionReturnValue = objRetorno;
			}

		    objRS.Fechar();
			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}
	}
}
