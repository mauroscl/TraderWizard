using System;
using DataBase;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public class cCarregadorCarteira : cCarregadorGenerico
	{

		public cCarregadorCarteira(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarteira CarregaAtiva(cIFRSobrevendido pobjIFRSobrevendido)
		{
			cCarteira functionReturnValue = null;

			var strSQL = "SELECT IdCarteira, Descricao, Ativo, Data_Inicio, Data_Fim " + Environment.NewLine;
			strSQL += " FROM Carteira " + Environment.NewLine;
			strSQL += " WHERE ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;
			strSQL += " AND Ativo = " + FuncoesBD.CampoFormatar(true);

			var objRS = new cRS(Conexao);

			objRS.ExecuteQuery(strSQL);


			if (objRS.DadosExistir) {
				var objRetorno = new cCarteira(Convert.ToInt32(objRS.Field("IdCarteira")), Convert.ToString(objRS.Field("Descricao"))
                    , pobjIFRSobrevendido, true, Convert.ToDateTime(objRS.Field("Data_Inicio")), Convert.ToDateTime(objRS.Field("Data_Fim")));

				objRS.Fechar();

				strSQL = "SELECT Codigo " + Environment.NewLine;
				strSQL += " FROM Carteira_Ativo " + Environment.NewLine;
				strSQL += " WHERE IdCarteira = " + FuncoesBD.CampoFormatar(objRetorno.IdCarteira);

				objRS.ExecuteQuery(strSQL);


				while (!objRS.EOF) {
					var objAtivo = new cAtivo(Convert.ToString(objRS.Field("Codigo")), string.Empty);

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
