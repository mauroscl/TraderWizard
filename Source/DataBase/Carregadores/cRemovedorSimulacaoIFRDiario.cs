using System;
using prjDominio.Entidades;

namespace DataBase.Carregadores
{

	public class cRemovedorSimulacaoIFRDiario
	{

		public cConexao objConexao { get; set; }

		public cRemovedorSimulacaoIFRDiario(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public bool ExcluirSimulacoesAnteriores(string pstrCodigo, Setup pobjSetup)
		{

			cCommand objCommand = new cCommand(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_DETALHE " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA_RESUMO " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id);

			objCommand.Execute(strSQL);

			return objCommand.TransStatus;

		}


	}
}
