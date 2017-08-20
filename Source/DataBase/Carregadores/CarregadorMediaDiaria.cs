using System;
using DataBase.Interfaces;
using Dominio.Entidades;
using DTO;

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

            FuncoesBd  funcoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSql = "SELECT Valor " + Environment.NewLine;
			strSql = strSql + " FROM Media_Diaria " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSql = strSql + " AND Data = " + funcoesBd.CampoFormatar(pobjCotacaoDiaria.Data);
			strSql = strSql + " AND Tipo = " + funcoesBd.CampoFormatar(pobjMediaDTO.CampoTipoBd);
			strSql = strSql + " AND NumPeriodos = " + funcoesBd.CampoFormatar(pobjMediaDTO.NumPeriodos);

			objRS.ExecuteQuery(strSql);

			MediaAbstract functionReturnValue = new MediaDiaria(pobjCotacaoDiaria, pobjMediaDTO.Tipo, pobjMediaDTO.NumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}

	}
}
