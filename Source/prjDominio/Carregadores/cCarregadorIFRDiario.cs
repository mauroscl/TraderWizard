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

namespace prjModelo.Carregadores
{

	public class cCarregadorIFRDiario : cCarregadorGenerico, ICarregadorIFR
	{

		public cCarregadorIFRDiario(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorIFRDiario() : base()
		{
		}


		public cIFR CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos)
		{
			cIFR functionReturnValue = null;

			cRS objRS = new cRS(Conexao);

			string strSQL = null;

			strSQL = "SELECT Valor " + Environment.NewLine;
			strSQL += " FROM IFR_Diario " + Environment.NewLine;
			strSQL += " WHERE Codigo = " + FuncoesBD.CampoFormatar(pobjCotacaoDiaria.Ativo.Codigo);
			strSQL += " AND Data = " + FuncoesBD.CampoFormatar(pobjCotacaoDiaria.Data);
			strSQL += " AND NumPeriodos = " + FuncoesBD.CampoFormatar(pintNumPeriodos);

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = new cIFRDiario(pobjCotacaoDiaria, pintNumPeriodos, Convert.ToDouble(objRS.Field("Valor")));

			objRS.Fechar();

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}
	}
}
