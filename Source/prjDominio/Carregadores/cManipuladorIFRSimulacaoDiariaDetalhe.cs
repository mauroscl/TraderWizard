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

	public class cManipuladorIFRSimulacaoDiariaDetalhe : cGeradorOperacaoBDPadrao
	{

		public cManipuladorIFRSimulacaoDiariaDetalhe(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public override string GeraInsert(cModelo pobjModelo)
		{

			string strSQL = null;

			cIFRSimulacaoDiariaDetalhe objItem = (cIFRSimulacaoDiariaDetalhe)pobjModelo;

			strSQL = " INSERT INTO IFR_Simulacao_Diaria_Detalhe " + Environment.NewLine;
			strSQL = strSQL + "(Codigo, ID_Setup, ID_IFR_SobreVendido, Data_Entrada_Efetiva, NumTentativas, MelhorEntrada, SomatorioCriterios, AgrupadorTentativas) " + Environment.NewLine;
			strSQL = strSQL + " VALUES " + Environment.NewLine;
			strSQL = strSQL + "(" + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.Ativo.Codigo);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.Setup.ID);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.IFRSobreVendido.ID);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.DataEntradaEfetiva);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.NumTentativas);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.MelhorEntrada);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.SomatorioCriterios);
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.AgrupadorDeTentativas);
			strSQL = strSQL + ") " + Environment.NewLine;

			return strSQL;

		}

		public override string GeraUpdate(cModelo pobjModelo)
		{

			string strSQL = null;

			cIFRSimulacaoDiariaDetalhe objItem = (cIFRSimulacaoDiariaDetalhe)pobjModelo;

			strSQL = " UPDATE IFR_Simulacao_Diaria_Detalhe SET " + Environment.NewLine;
			strSQL = strSQL + "NumTentativas = " + FuncoesBD.CampoFormatar(objItem.NumTentativas) + Environment.NewLine;
			strSQL = strSQL + ", MelhorEntrada = " + FuncoesBD.CampoFormatar(objItem.MelhorEntrada) + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.Ativo.Codigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.Setup.ID) + Environment.NewLine;
			strSQL = strSQL + " AND ID_IFR_SobreVendido = " + FuncoesBD.CampoFormatar(objItem.IFRSobreVendido.ID) + Environment.NewLine;
			strSQL = strSQL + " AND Data_Entrada_Efetiva = " + FuncoesBD.CampoFormatar(objItem.IFRSimulacaoDiaria.DataEntradaEfetiva);

			return strSQL;

		}


	}
}
