using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;
using frwInterface;

namespace prjModelo.Carregadores
{

	public class cCarregadorSimulacaoIFRDiario : cCarregadorGenerico
	{

		public cCarregadorSimulacaoIFRDiario(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorSimulacaoIFRDiario()
		{
		}

		private string GerarQuery(cAtivo pobjAtivo, Setup pobjSetup, string pstrWhereAdicional, int? pintTop, string pstrOrderBy)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT " + (pintTop.HasValue ? "TOP  " + pintTop : string.Empty);
			strSQL += " S.ID_CM, S.Sequencial, S.Data_Entrada_Efetiva, S.Valor_Entrada_Original, S.Valor_Entrada_Ajustado, " + Environment.NewLine;
			strSQL += "S.Valor_IFR_Minimo, S.Valor_Maximo, S.Percentual_Maximo, S.Data_Saida, S.Percentual_Saida, S.Percentual_MME21, " + Environment.NewLine;
			strSQL += "S.Percentual_MME49, S.Percentual_MME200, S.Valor_Stop_Loss_Inicial, S.Verdadeiro " + Environment.NewLine;
			strSQL += " FROM IFR_Simulacao_Diaria S " + Environment.NewLine;
			strSQL += " WHERE S.Codigo = " + FuncoesBd.CampoFormatar(pobjAtivo.Codigo);
			strSQL += " AND S.ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.ID);
			strSQL += pstrWhereAdicional;

			if (pstrOrderBy != string.Empty) {
				strSQL += " ORDER BY " + pstrOrderBy;
			}

			return strSQL;

		}

		private cIFRSimulacaoDiaria ConstruirObjetoDaSimulacao(cAtivo pobjAtivo, Setup pobjSetup, Dictionary<string, object> pcolLinha)
		{

			cIFRSimulacaoDiaria objRetorno = new cIFRSimulacaoDiaria(Conexao);
			objRetorno.Ativo = pobjAtivo;
			objRetorno.Setup = pobjSetup;

			objRetorno.Sequencial = Convert.ToInt32(pcolLinha["Sequencial"]);
			objRetorno.DataEntradaEfetiva = Convert.ToDateTime(pcolLinha["Data_Entrada_Efetiva"]);
			objRetorno.ValorEntradaOriginal = Convert.ToDecimal(pcolLinha["Valor_Entrada_Original"]);
			objRetorno.ValorEntradaAjustado = Convert.ToDecimal(pcolLinha["Valor_Entrada_Ajustado"]);
			objRetorno.ValorIFR = Convert.ToDouble(pcolLinha["Valor_IFR_Minimo"]);
			objRetorno.ValorMaximo = Convert.ToDecimal(pcolLinha["Valor_Maximo"]);
			objRetorno.PercentualMaximo = Convert.ToDecimal(pcolLinha["Percentual_Maximo"]);
			objRetorno.DataSaida = Convert.ToDateTime(pcolLinha["Data_Saida"]);
			objRetorno.PercentualSaida = Convert.ToDecimal(pcolLinha["Percentual_Saida"]);
			objRetorno.PercentualMME21 = Convert.ToDecimal(pcolLinha["Percentual_MME21"]);
			objRetorno.PercentualMME49 = Convert.ToDecimal(pcolLinha["Percentual_MME49"]);
			objRetorno.PercentualMME200 = Convert.ToDecimal(pcolLinha["Percentual_MME200"]);
			objRetorno.ValorStopLossInicial = Convert.ToDecimal(pcolLinha["Valor_Stop_Loss_Inicial"]);
			objRetorno.Verdadeiro = Convert.ToBoolean(pcolLinha["Verdadeiro"]);

			cCarregadorClassificacaoMedia objCarregadorClassifMedia = new cCarregadorClassificacaoMedia();
            objRetorno.ClassificacaoMedia = objCarregadorClassifMedia.CarregaPorID((cEnum.enumClassifMedia)Enum.Parse(typeof(cEnum.enumClassifMedia), Convert.ToString( pcolLinha["ID_CM"])));

			return objRetorno;

		}

		private cIFRSimulacaoDiaria Carregar(cAtivo pobjAtivo, Setup pobjSetup, string pstrSQL)
		{
			cIFRSimulacaoDiaria functionReturnValue;

			cRSList objRS = new cRSList(Conexao);

			objRS.AdicionarQuery(pstrSQL);
			objRS.ExecuteQuery();


			if (objRS.DadosExistir) {
				var objRetorno = ConstruirObjetoDaSimulacao(pobjAtivo, pobjSetup, objRS.RetornaLinhaAtual());

				cCarregadorIFRSimulacaoDiariaDetalhe objCarregadorDetalhe = new cCarregadorIFRSimulacaoDiariaDetalhe(Conexao);

				objRetorno.Detalhes = objCarregadorDetalhe.CarregarTodosDeUmaSimulacao(objRetorno);

				functionReturnValue = objRetorno;

			} else {
				functionReturnValue = null;
			}

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}


		private IList<cIFRSimulacaoDiaria> CarregarLista(cAtivo pobjAtivo, Setup pobjSetup, string pstrSQL)
		{

			cRSList objRS = new cRSList(Conexao);

			objRS.AdicionarQuery(pstrSQL);
			objRS.ExecuteQuery();

			List<cIFRSimulacaoDiaria> lstRetorno = new List<cIFRSimulacaoDiaria>();


			while (!objRS.EOF) {
				var objRetorno = ConstruirObjetoDaSimulacao(pobjAtivo, pobjSetup, objRS.RetornaLinhaAtual());

				cCarregadorIFRSimulacaoDiariaDetalhe objCarregadorDetalhe = new cCarregadorIFRSimulacaoDiariaDetalhe(Conexao);

				objRetorno.Detalhes = objCarregadorDetalhe.CarregarTodosDeUmaSimulacao(objRetorno);

				lstRetorno.Add(objRetorno);

				objRS.MoveNext();

			}

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

		public cIFRSimulacaoDiaria CarregarMelhorEntradaPorAgrupadorDeTentativas(cAtivo pobjAtivo, Setup pobjSetup, cIFRSobrevendido pobjIFRSobrevendido, UInt32 pintAgrupadorDeTentativas)
		{
		    string strWhereAdicional = " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.MelhorEntrada = " + FuncoesBd.CampoFormatar(true) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.AgrupadorTentativas = " + FuncoesBd.CampoFormatar(pintAgrupadorDeTentativas);
			strWhereAdicional += ")" + Environment.NewLine;

			var strSQL = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);
			return Carregar(pobjAtivo, pobjSetup, strSQL);

		}

		public cIFRSimulacaoDiaria CarregarMelhorEntradaPorDataDeSaida(cAtivo pobjAtivo, Setup pobjSetup, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmDataSaida)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strWhereAdicional = " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.MelhorEntrada = " + FuncoesBd.CampoFormatar(true) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND S.Data_Saida = " + FuncoesBd.CampoFormatar(pdtmDataSaida);
            strWhereAdicional += ")" + Environment.NewLine;

			var strSQL = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);
			return Carregar(pobjAtivo, pobjSetup, strSQL);

		}

		/// <summary>
		/// Carrega a última simulação anterior a data de referencia e todas as simulações posteriores à data de referencia
		/// </summary>
		/// <param name="pobjAtivo"></param>
		/// <param name="pobjSetup"></param>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="pdtmDataReferencia"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public IList<cIFRSimulacaoDiaria> CarregarUltimasSimulacoesPorIFRSobrevendido(cAtivo pobjAtivo, Setup pobjSetup, cIFRSobrevendido pobjIFRSobrevendido, DateTime pdtmDataReferencia)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strWhereAdicional = " AND Data_Entrada_Efetiva < " + FuncoesBd.CampoFormatar(pdtmDataReferencia);
			strWhereAdicional += " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ID);
			strWhereAdicional += ")" + Environment.NewLine;

			//gera query que retorna a última simulação anterior à data de referência
			var strSQL1 = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, 1, "S.Data_Entrada_Efetiva DESC ");

			//Gera a query  que retorna todas as simulações posteriores à data de referência
			strWhereAdicional = " AND Data_Entrada_Efetiva > " + FuncoesBd.CampoFormatar(pdtmDataReferencia);
			var strSQL2 = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);

			var strSQL = "(" + strSQL1 + ") UNION (" + strSQL2 + ")" + Environment.NewLine;
			strSQL += "ORDER BY Data_Entrada_Efetiva ";

			return CarregarLista(pobjAtivo, pobjSetup, strSQL);

		}

		public cIFRSimulacaoDiaria CarregaPorDataEntradaEfetiva(cAtivo pobjAtivo, Setup pobjSetup, DateTime pdtmDataEntradaEfetiva)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			var strWhereAdicional = " AND Data_Entrada_Efetiva = " + FuncoesBd.CampoFormatar(pdtmDataEntradaEfetiva);
			var strSQL = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);
			return Carregar(pobjAtivo, pobjSetup, strSQL);
		}
	}

}
