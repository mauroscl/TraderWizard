using System;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public class RemovedorSimulacaoIFRDiario
	{

		public Conexao Conexao { get; set; }

		public RemovedorSimulacaoIFRDiario(Conexao pobjConexao)
		{
			Conexao = pobjConexao;
		}

		public bool ExcluirSimulacoesAnteriores(string pstrCodigo, Setup pobjSetup)
		{

			cCommand objCommand = new cCommand(Conexao);

            FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSql = "DELETE " + Environment.NewLine;
			strSql = strSql + " FROM IFR_SIMULACAO_DIARIA_DETALHE " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo);
			strSql = strSql + " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSql);

			strSql = "DELETE " + Environment.NewLine;
			strSql = strSql + " FROM IFR_SIMULACAO_DIARIA_FAIXA " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo);
			strSql = strSql + " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSql);

			strSql = "DELETE " + Environment.NewLine;
			strSql = strSql + " FROM IFR_SIMULACAO_DIARIA_FAIXA_RESUMO " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo);
			strSql = strSql + " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSql);

			strSql = "DELETE " + Environment.NewLine;
			strSql = strSql + " FROM IFR_SIMULACAO_DIARIA " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo);
			strSql = strSql + " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSql);

			return objCommand.TransStatus;

		}


	}
}
