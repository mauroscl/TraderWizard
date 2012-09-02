using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Interfaces;
using prjModelo.Entidades;
using prjDTO;

namespace prjModelo.Carregadores
{

	public class cCarregadorMediaDiaria : cCarregadorGenerico, ICarregadorMedia
	{

		public cCarregadorMediaDiaria(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorMediaDiaria() : base()
		{
		}

		public cMediaAbstract CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDTO)
		{
			cMediaAbstract functionReturnValue = null;

			cRS objRS = new cRS(Conexao);

			string strSQL = null;

			strSQL = "SELECT Valor " + Environment.NewLine;
			strSQL = strSQL + " FROM Media_Diaria " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSQL = strSQL + " AND Data = " + FuncoesBD.CampoFormatar(pobjCotacaoDiaria.Data);
			strSQL = strSQL + " AND Tipo = " + FuncoesBD.CampoFormatar(pobjMediaDTO.CampoTipoBD);
			strSQL = strSQL + " AND NumPeriodos = " + FuncoesBD.CampoFormatar(pobjMediaDTO.NumPeriodos);

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = new cMediaDiaria(pobjCotacaoDiaria, pobjMediaDTO.Tipo, pobjMediaDTO.NumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}

	}
}
