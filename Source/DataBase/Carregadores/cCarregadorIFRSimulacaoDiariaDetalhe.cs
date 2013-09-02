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
	//Imports Repositorio.Interface
	//Imports Repositorio.Implementacao

	public class cCarregadorIFRSimulacaoDiariaDetalhe
	{

		private cConexao Conexao { get; set; }
		//Private Property objRepIFRSimulacao As IRepIFRSimulacaoFaixa

		public cCarregadorIFRSimulacaoDiariaDetalhe(cConexao pobjConexao)
		{
			Conexao = pobjConexao;
			//objRepIFRSimulacao = New RepIFRSimulacaoFaixa(pobjConexao)
		}
		public IList<cIFRSimulacaoDiariaDetalhe> CarregarTodosDeUmaSimulacao(cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT ID_IFR_Sobrevendido, NumTentativas, MelhorEntrada, Data_Entrada_Efetiva, SomatorioCriterios, AgrupadorTentativas " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria_Detalhe " + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.Ativo.Codigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.Setup.Id) + Environment.NewLine;
			strSQL = strSQL + " AND Data_Entrada_Efetiva = " + FuncoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.DataEntradaEfetiva);

			cRS objRS = new cRS(Conexao);

			List<cIFRSimulacaoDiariaDetalhe> lstRetorno = new List<cIFRSimulacaoDiariaDetalhe>();

			objRS.ExecuteQuery(strSQL);

			cCarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(Conexao);


			while (!objRS.EOF) {
				lstRetorno.Add(new cIFRSimulacaoDiariaDetalhe(objCarregadorIFRSobrevendido.CarregaPorID(Convert.ToInt16(objRS.Field("ID_IFR_Sobrevendido"))), Convert.ToByte(objRS.Field("NumTentativas")), Convert.ToBoolean(objRS.Field("MelhorEntrada")), Convert.ToInt16(objRS.Field("SomatorioCriterios")), Convert.ToUInt32(objRS.Field("AgrupadorTentativas")), pobjIFRSimulacaoDiaria));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return lstRetorno;

		}

	}
}
