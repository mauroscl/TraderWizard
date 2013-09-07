using System;
using DataBase.Interfaces;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Carregadores;
using prjModelo.Entidades;

namespace DataBase.Carregadores
{

	public class cCarregadorMediaDiaria : cCarregadorGenerico, ICarregadorMedia
	{

		public cCarregadorMediaDiaria(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorMediaDiaria()
		{
		}

		public MediaAbstract CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDTO)
		{
		    cRS objRS = new cRS(Conexao);

            FuncoesBd  FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT Valor " + Environment.NewLine;
			strSQL = strSQL + " FROM Media_Diaria " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSQL = strSQL + " AND Data = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSQL = strSQL + " AND Tipo = " + FuncoesBd.CampoFormatar(pobjMediaDTO.CampoTipoBD);
			strSQL = strSQL + " AND NumPeriodos = " + FuncoesBd.CampoFormatar(pobjMediaDTO.NumPeriodos);

			objRS.ExecuteQuery(strSQL);

			MediaAbstract functionReturnValue = new MediaDiaria(pobjCotacaoDiaria, pobjMediaDTO.Tipo, pobjMediaDTO.NumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}

	}
}
