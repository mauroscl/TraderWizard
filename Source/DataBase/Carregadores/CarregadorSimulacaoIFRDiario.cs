using System;
using System.Collections.Generic;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class CarregadorSimulacaoIFRDiario : CarregadorGenerico
	{

		public CarregadorSimulacaoIFRDiario(Conexao pobjConexao) : base(pobjConexao)
		{
		}

		private string GerarQuery(Ativo pobjAtivo, Setup pobjSetup, string pstrWhereAdicional, int? pintTop, string pstrOrderBy)
		{
            FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = "SELECT " + (pintTop.HasValue ? "TOP  " + pintTop : string.Empty);
			strSQL += " S.ID_CM, S.Sequencial, S.Data_Entrada_Efetiva, S.Valor_Entrada_Original, S.Valor_Entrada_Ajustado, " + Environment.NewLine;
			strSQL += "S.Valor_IFR_Minimo, S.Valor_Maximo, S.Percentual_Maximo, S.Data_Saida, S.Percentual_Saida, S.Percentual_MME21, " + Environment.NewLine;
			strSQL += "S.Percentual_MME49, S.Percentual_MME200, S.Valor_Stop_Loss_Inicial, S.Verdadeiro " + Environment.NewLine;
			strSQL += " FROM IFR_Simulacao_Diaria S " + Environment.NewLine;
			strSQL += " WHERE S.Codigo = " + funcoesBd.CampoFormatar(pobjAtivo.Codigo);
			strSQL += " AND S.ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id);
			strSQL += pstrWhereAdicional;

			if (pstrOrderBy != string.Empty) {
				strSQL += " ORDER BY " + pstrOrderBy;
			}

			return strSQL;

		}

		private IFRSimulacaoDiaria ConstruirObjetoDaSimulacao(Ativo pobjAtivo, Setup pobjSetup, Dictionary<string, object> pcolLinha)
		{

			var objRetorno = new IFRSimulacaoDiaria();
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

			var objCarregadorClassifMedia = new CarregadorClassificacaoMedia();
            objRetorno.ClassificacaoMedia = objCarregadorClassifMedia.CarregaPorID((cEnum.enumClassifMedia)Enum.Parse(typeof(cEnum.enumClassifMedia), Convert.ToString( pcolLinha["ID_CM"])));

			return objRetorno;

		}

		private IFRSimulacaoDiaria Carregar(Ativo pobjAtivo, Setup pobjSetup, string pstrSQL)
		{
			IFRSimulacaoDiaria functionReturnValue;

			RSList objRS = new RSList(Conexao);

			objRS.AdicionarQuery(pstrSQL);
			objRS.ExecuteQuery();


			if (objRS.DadosExistir) {
				var objRetorno = ConstruirObjetoDaSimulacao(pobjAtivo, pobjSetup, objRS.RetornaLinhaAtual());

				CarregadorIFRSimulacaoDiariaDetalhe objCarregadorDetalhe = new CarregadorIFRSimulacaoDiariaDetalhe(Conexao);

				objRetorno.Detalhes = objCarregadorDetalhe.CarregarTodosDeUmaSimulacao(objRetorno);

				functionReturnValue = objRetorno;

			} else {
				functionReturnValue = null;
			}

			VerificaSeDeveFecharConexao();
			return functionReturnValue;

		}


		private IList<IFRSimulacaoDiaria> CarregarLista(Ativo pobjAtivo, Setup pobjSetup, string pstrSQL)
		{

			RSList objRS = new RSList(Conexao);

			objRS.AdicionarQuery(pstrSQL);
			objRS.ExecuteQuery();

			List<IFRSimulacaoDiaria> lstRetorno = new List<IFRSimulacaoDiaria>();


			while (!objRS.EOF) {
				var objRetorno = ConstruirObjetoDaSimulacao(pobjAtivo, pobjSetup, objRS.RetornaLinhaAtual());

				CarregadorIFRSimulacaoDiariaDetalhe objCarregadorDetalhe = new CarregadorIFRSimulacaoDiariaDetalhe(Conexao);

				objRetorno.Detalhes = objCarregadorDetalhe.CarregarTodosDeUmaSimulacao(objRetorno);

				lstRetorno.Add(objRetorno);

				objRS.MoveNext();

			}

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

		public IFRSimulacaoDiaria CarregarMelhorEntradaPorAgrupadorDeTentativas(Ativo pobjAtivo, Setup pobjSetup, IFRSobrevendido pobjIFRSobrevendido, UInt32 pintAgrupadorDeTentativas)
		{
		    FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();
		    string strWhereAdicional = " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + funcoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.MelhorEntrada = " + funcoesBd.CampoFormatar(true) + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.AgrupadorTentativas = " + funcoesBd.CampoFormatar(pintAgrupadorDeTentativas);
			strWhereAdicional += ")" + Environment.NewLine;

			var strSQL = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);
			return Carregar(pobjAtivo, pobjSetup, strSQL);

		}

		public IFRSimulacaoDiaria CarregarMelhorEntradaPorDataDeSaida(Ativo pobjAtivo, Setup pobjSetup, IFRSobrevendido pobjIFRSobrevendido, DateTime pdtmDataSaida)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strWhereAdicional = " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;
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
		public IList<IFRSimulacaoDiaria> CarregarUltimasSimulacoesPorIFRSobrevendido(Ativo pobjAtivo, Setup pobjSetup, IFRSobrevendido pobjIFRSobrevendido, DateTime pdtmDataReferencia)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strWhereAdicional = " AND Data_Entrada_Efetiva < " + FuncoesBd.CampoFormatar(pdtmDataReferencia);
			strWhereAdicional += " AND EXISTS ( " + Environment.NewLine;
			strWhereAdicional += '\t' + " SELECT 1 " + Environment.NewLine;
			strWhereAdicional += '\t' + " FROM IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
			strWhereAdicional += '\t' + " WHERE D.Codigo = S.Codigo " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_Setup = S.ID_Setup " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.Data_Entrada_Efetiva = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strWhereAdicional += '\t' + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
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

		public IFRSimulacaoDiaria CarregaPorDataEntradaEfetiva(Ativo pobjAtivo, Setup pobjSetup, DateTime pdtmDataEntradaEfetiva)
		{
            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			var strWhereAdicional = " AND Data_Entrada_Efetiva = " + FuncoesBd.CampoFormatar(pdtmDataEntradaEfetiva);
			var strSQL = GerarQuery(pobjAtivo, pobjSetup, strWhereAdicional, null, string.Empty);
			return Carregar(pobjAtivo, pobjSetup, strSQL);
		}
	}

}
