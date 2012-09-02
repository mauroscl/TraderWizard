using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
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

			string strSQL = null;

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_DETALHE " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA_RESUMO " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID);

			objCommand.Execute(strSQL);

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo);
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID);

			objCommand.Execute(strSQL);

			return objCommand.TransStatus;

		}


	}
}
