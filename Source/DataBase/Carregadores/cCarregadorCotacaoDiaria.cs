using DataBase.Interfaces;
using System;
using System.Collections.Generic;
using DataBase;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Entidades;
using prjModelo.Regras;
using TraderWizard.Enumeracoes;

namespace prjModelo.Carregadores
{

	public class cCarregadorCotacaoDiaria : cCarregadorGenerico, ICarregadorCotacao
	{

		public cCarregadorCotacaoDiaria(cConexao pobjConexao) : base(pobjConexao)
		{
		}

		public cCarregadorCotacaoDiaria() : base()
		{
		}

		public IList<cCotacaoDiaria> CarregarPorPeriodo(cAtivo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<cMediaDTO> plstMedias, bool pblnCarregarIFR)
		{

			cRS objRS = new cRS(Conexao);


		    string strSelect = "SELECT C.Data, Sequencial, ValorAbertura, ValorMinimo, ValorMaximo, ValorFechamento";

            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strFrom = "Cotacao C " + Environment.NewLine;

		    string strWhere = " WHERE C.Codigo = " + FuncoesBd.CampoFormatar(pobjAtivo.Codigo) + Environment.NewLine;
			strWhere += " AND C.Data BETWEEN " + FuncoesBd.CampoFormatar(pdtmDataInicial) + " AND " + FuncoesBd.CampoFormatar(pdtmDataFinal) + Environment.NewLine;


		    foreach (cMediaDTO objMediaDTO in plstMedias) {
				string strAliasTabelaMedia = objMediaDTO.GetAlias;

				strSelect += ", " + strAliasTabelaMedia + ".Valor AS " + strAliasTabelaMedia;

				//tem que gerar na forma de um subselect para trazer os valores corretos com o LEFT JOIN devido ao WHERE 
				//necessário nos campos "Tipo" e "NumPeriodos"
				string strTabelaMedia = "(" + Environment.NewLine;
				strTabelaMedia += '\t' + "SELECT Codigo, Data, Valor" + Environment.NewLine;
				strTabelaMedia += '\t' + " FROM Media_Diaria " + strAliasTabelaMedia + Environment.NewLine;
				strTabelaMedia += '\t' + " WHERE Tipo = " + FuncoesBd.CampoStringFormatar(objMediaDTO.CampoTipoBD) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND NumPeriodos = " + FuncoesBd.CampoFormatar(objMediaDTO.NumPeriodos) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND Codigo = " + FuncoesBd.CampoStringFormatar(pobjAtivo.Codigo) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND Data BETWEEN " + FuncoesBd.CampoFormatar(pdtmDataInicial) + " AND " + FuncoesBd.CampoFormatar(pdtmDataFinal) + Environment.NewLine;
				strTabelaMedia += ") AS " + strAliasTabelaMedia + Environment.NewLine;

				strFrom = "(" + strFrom + "LEFT JOIN " + strTabelaMedia;
				strFrom += "ON C.Codigo = " + strAliasTabelaMedia + ".Codigo " + Environment.NewLine;
				strFrom += "AND C.Data = " + strAliasTabelaMedia + ".Data) " + Environment.NewLine;

			}


			if (pblnCarregarIFR) {
				strSelect += ", IFR.Valor  AS IFR ";

				strFrom = Environment.NewLine + "(" + strFrom + " LEFT JOIN IFR_Diario IFR " + Environment.NewLine;
				strFrom += " ON C.Codigo = IFR.Codigo " + Environment.NewLine;
				strFrom += " AND C.Data = IFR.Data)" + Environment.NewLine;

				strWhere += "AND IFR.NumPeriodos = 2 " + Environment.NewLine;

			}


			string strSQL = strSelect + " FROM " + strFrom + strWhere;


			if (pstrOrdem != string.Empty) {
				strSQL = strSQL + " ORDER BY " + pstrOrdem;
			}

			objRS.ExecuteQuery(strSQL);

			List<cCotacaoDiaria> lstRetorno = new List<cCotacaoDiaria>();


		    while (!objRS.EOF) {
				cCotacaoDiaria objCotacaoDiaria = new cCotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

		        foreach (cMediaDTO objMediaDTO in plstMedias)
				{
				    double valordaMedia;
				    if (double.TryParse(Convert.ToString( objRS.Field(objMediaDTO.GetAlias)),out valordaMedia)) {
                        objCotacaoDiaria.Medias.Add(new cMediaDiaria(objCotacaoDiaria, objMediaDTO.CampoTipoBD, objMediaDTO.NumPeriodos, valordaMedia));
					}
				}


		        if (pblnCarregarIFR) {
					objCotacaoDiaria.IFR = new cIFRDiario(objCotacaoDiaria, 2, Convert.ToDouble(objRS.Field("IFR")));


				}

				lstRetorno.Add(objCotacaoDiaria);

				objRS.MoveNext();

			}
			objRS.Fechar();

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

		private string MediaTipoCalcular(cEnum.enumMediaTipo pintMediaTipo)
		{
		    switch (pintMediaTipo)
		    {
		        case cEnum.enumMediaTipo.Aritmetica:
		            return "MMA";
		        case cEnum.enumMediaTipo.Exponencial:
		            return "MME";
		        default:
		            return string.Empty;
		    }
		}

	    public IList<cCotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(cAtivo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo)
		{

			string strSQL = cGeradorQuery.BackTestingIFRSemFiltroEntradaQueryGerar(pobjAtivo.Codigo, "DIARIO", "TODOS", pdblValorMaximoIFRSobrevendido, 1, pintMediaTipo, true);

			//Somente utiliza este filtro quando a tabela envolvida for a tabela do IFR diário
			strSQL += " AND NOT EXISTS " + Environment.NewLine;
			strSQL += "(";
			strSQL += '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + " FROM IFR_Simulacao_Diaria S " + Environment.NewLine;
			strSQL += '\t' + " WHERE C.Codigo = S.Codigo " + Environment.NewLine;
			strSQL += '\t' + " AND C.Data = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strSQL += '\t' + " AND S.ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSQL += ")";

			//inclui ordenamento por data.
			strSQL += " ORDER BY C.Data ";

			cRS objRS = new cRS(Conexao);

			List<cCotacaoDiaria> lstRetorno = new List<cCotacaoDiaria>();

	        string strTipoMedia = MediaTipoCalcular(pintMediaTipo);

			objRS.ExecuteQuery(strSQL);


			while (!objRS.EOF) {
				cCotacaoDiaria objCotacaoDiaria = new cCotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data_Entrada")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("VALOR_ENTRADA"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

                cMediaDiaria objMedia = new cMediaDiaria(objCotacaoDiaria, strTipoMedia, 21, Convert.ToDouble(objRS.Field("MME21")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new cMediaDiaria(objCotacaoDiaria, strTipoMedia, 49, Convert.ToDouble(objRS.Field("MME49")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new cMediaDiaria(objCotacaoDiaria, strTipoMedia, 200, Convert.ToDouble(objRS.Field("MME200")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new cMediaDiaria(objCotacaoDiaria, "IFR2", 13, Convert.ToDouble(objRS.Field("MMIFR")));
				objCotacaoDiaria.Medias.Add(objMedia);


				cIFRDiario objIFRDiario = new cIFRDiario(objCotacaoDiaria, 2, Convert.ToDouble(objRS.Field("VALOR_IFR")));
				objCotacaoDiaria.IFR = objIFRDiario;

				lstRetorno.Add(objCotacaoDiaria);

				objRS.MoveNext();

			}

			objRS.Fechar();

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

		public IList<cCotacaoDiaria> CarregarParaIFRComFiltro(cAtivo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial)
		{

			//Parâmetros: 
			//* pintOrdem: não terá importância. Só é importante quando são executados mais de um setup simualtaneamente
			//* pblnAcionamentoGerar: indica se é para retornar a data de acionamento do setup. Será passado
			//FALSE nesta chamada porque será feito uma verificação aqui nesta função se o setup será acionado.
			//Se o parâmetro for passado com valor TRUE retornará apenas as datas em que houver rompimento 
			//no período seguinte ao período em que o IFR cruzou a média. 
			string strSQL = cGeradorQuery.BackTestingIFRComFiltroEntradaQueryGerar(Conexao.ObterFormatadorDeCampo() , pobjAtivo.Codigo, "DIARIO", "TODOS", 1, pintMediaTipo, pdtmDataInicial, false, true);

			//inclui ordenamento por data.
			strSQL += " ORDER BY Dia2.Data ";

			cRS objRS = new cRS(Conexao);

			objRS.ExecuteQuery(strSQL);

			List<cCotacaoDiaria> lstRetorno = new List<cCotacaoDiaria>();

		    string strTipoMedia = MediaTipoCalcular(pintMediaTipo);

			objRS.ExecuteQuery(strSQL);


			while (!objRS.EOF) {
				cCotacaoDiaria objCotacaoDiaria = new cCotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data_Entrada")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

                cMediaDiaria objMedia = new cMediaDiaria(objCotacaoDiaria, strTipoMedia, 200, Convert.ToDouble(objRS.Field("MME200")));

				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new cMediaDiaria(objCotacaoDiaria, "IFR2", 13, Convert.ToDouble(objRS.Field("Media_IFR")));

				objCotacaoDiaria.Medias.Add(objMedia);

				lstRetorno.Add(objCotacaoDiaria);

				objRS.MoveNext();

			}

			objRS.Fechar();

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

	}

}
