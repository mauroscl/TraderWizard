using System;
using DataBase.Interfaces;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Carregadores;
using prjModelo.Entidades;

namespace DataBase.Carregadores
{

	public class CarregadorMediaDiaria : CarregadorGenerico, ICarregadorMedia
	{

		public CarregadorMediaDiaria(Conexao pobjConexao) : base(pobjConexao)
		{
		}

		public CarregadorMediaDiaria()
		{
		}

		public MediaAbstract CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, MediaDTO pobjMediaDTO)
		{
		    cRS objRS = new cRS(Conexao);

            FuncoesBd  FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT Valor " + Environment.NewLine;
			strSQL = strSQL + " FROM Media_Diaria " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSQL = strSQL + " AND Data = " + FuncoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSQL = strSQL + " AND Tipo = " + FuncoesBd.CampoFormatar(pobjMediaDTO.CampoTipoBd);
			strSQL = strSQL + " AND NumPeriodos = " + FuncoesBd.CampoFormatar(pobjMediaDTO.NumPeriodos);

			objRS.ExecuteQuery(strSQL);

			MediaAbstract functionReturnValue = new MediaDiaria(pobjCotacaoDiaria, pobjMediaDTO.Tipo, pobjMediaDTO.NumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}

	}
}
