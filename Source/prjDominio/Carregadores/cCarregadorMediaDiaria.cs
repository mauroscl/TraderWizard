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

		public cCarregadorMediaDiaria()
		{
		}

		public cMediaAbstract CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDTO)
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

			cMediaAbstract functionReturnValue = new cMediaDiaria(pobjCotacaoDiaria, pobjMediaDTO.Tipo, pobjMediaDTO.NumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}

	}
}
