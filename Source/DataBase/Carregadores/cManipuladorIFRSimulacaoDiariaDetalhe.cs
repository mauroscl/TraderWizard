using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjDominio.Entidades;
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
		    cIFRSimulacaoDiariaDetalhe objItem = (cIFRSimulacaoDiariaDetalhe)pobjModelo;

		    FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			string strSQL = " INSERT INTO IFR_Simulacao_Diaria_Detalhe " + Environment.NewLine;
			strSQL = strSQL + "(Codigo, ID_Setup, ID_IFR_SobreVendido, Data_Entrada_Efetiva, NumTentativas, MelhorEntrada, SomatorioCriterios, AgrupadorTentativas) " + Environment.NewLine;
			strSQL = strSQL + " VALUES " + Environment.NewLine;
			strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.Ativo.Codigo);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.Setup.Id);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.IFRSobreVendido.ID);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.DataEntradaEfetiva);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.NumTentativas);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.MelhorEntrada);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.SomatorioCriterios);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.AgrupadorDeTentativas);
			strSQL = strSQL + ") " + Environment.NewLine;

			return strSQL;

		}

		public override string GeraUpdate(cModelo pobjModelo)
		{
		    cIFRSimulacaoDiariaDetalhe objItem = (cIFRSimulacaoDiariaDetalhe)pobjModelo;

            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			string strSQL = " UPDATE IFR_Simulacao_Diaria_Detalhe SET " + Environment.NewLine;
			strSQL = strSQL + "NumTentativas = " + FuncoesBd.CampoFormatar(objItem.NumTentativas) + Environment.NewLine;
			strSQL = strSQL + ", MelhorEntrada = " + FuncoesBd.CampoFormatar(objItem.MelhorEntrada) + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.Ativo.Codigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.Setup.Id) + Environment.NewLine;
			strSQL = strSQL + " AND ID_IFR_SobreVendido = " + FuncoesBd.CampoFormatar(objItem.IFRSobreVendido.ID) + Environment.NewLine;
			strSQL = strSQL + " AND Data_Entrada_Efetiva = " + FuncoesBd.CampoFormatar(objItem.IFRSimulacaoDiaria.DataEntradaEfetiva);

			return strSQL;

		}


	}
}
