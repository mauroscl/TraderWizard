using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataBase.Interfaces;
using Dominio.Entidades;
using DTO;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{

	public class CarregadorCotacaoDiaria : CarregadorGenerico, ICarregadorCotacao
	{

		public CarregadorCotacaoDiaria(Conexao pobjConexao) : base(pobjConexao)
		{
		}

		public CarregadorCotacaoDiaria()
		{
		}

		public IList<CotacaoDiaria> CarregarPorPeriodo(Ativo pobjAtivo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, string pstrOrdem, IList<MediaDTO> plstMedias, bool pblnCarregarIFR)
		{

			RS objRS = new RS(Conexao);


		    string strSelect = "SELECT C.Data, Sequencial, ValorAbertura, ValorMinimo, ValorMaximo, ValorFechamento";

            FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

		    string strFrom = "Cotacao C " + Environment.NewLine;

		    string strWhere = " WHERE C.Codigo = " + funcoesBd.CampoFormatar(pobjAtivo.Codigo) + Environment.NewLine;
			strWhere += " AND C.Data BETWEEN " + funcoesBd.CampoFormatar(pdtmDataInicial) + " AND " + funcoesBd.CampoFormatar(pdtmDataFinal) + Environment.NewLine;


		    foreach (MediaDTO objMediaDTO in plstMedias) {
				string strAliasTabelaMedia = objMediaDTO.GetAlias;

				strSelect += ", " + strAliasTabelaMedia + ".Valor AS " + strAliasTabelaMedia;

				//tem que gerar na forma de um subselect para trazer os valores corretos com o LEFT JOIN devido ao WHERE 
				//necessário nos campos "Tipo" e "NumPeriodos"
				string strTabelaMedia = "(" + Environment.NewLine;
				strTabelaMedia += '\t' + "SELECT Codigo, Data, Valor" + Environment.NewLine;
				strTabelaMedia += '\t' + " FROM Media_Diaria " + strAliasTabelaMedia + Environment.NewLine;
				strTabelaMedia += '\t' + " WHERE Tipo = " + funcoesBd.CampoStringFormatar(objMediaDTO.CampoTipoBd) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND NumPeriodos = " + funcoesBd.CampoFormatar(objMediaDTO.NumPeriodos) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND Codigo = " + funcoesBd.CampoStringFormatar(pobjAtivo.Codigo) + Environment.NewLine;
				strTabelaMedia += '\t' + " AND Data BETWEEN " + funcoesBd.CampoFormatar(pdtmDataInicial) + " AND " + funcoesBd.CampoFormatar(pdtmDataFinal) + Environment.NewLine;
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

			List<CotacaoDiaria> lstRetorno = new List<CotacaoDiaria>();


		    while (!objRS.Eof) {
				CotacaoDiaria objCotacaoDiaria = new CotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

		        foreach (MediaDTO mediaDto in plstMedias)
				{
				    if (double.TryParse(Convert.ToString( objRS.Field(mediaDto.GetAlias)),out var valordaMedia)) {
                        objCotacaoDiaria.Medias.Add(new MediaDiaria(objCotacaoDiaria, mediaDto.CampoTipoBd, mediaDto.NumPeriodos, valordaMedia));
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

	    public IList<CotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(Ativo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido, cEnum.enumMediaTipo pintMediaTipo)
	    {
	        FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();
	        string strSql = GeradorQuery.BackTestingIFRSemFiltroEntradaQueryGerar(pobjAtivo.Codigo, "DIARIO", "TODOS", pdblValorMaximoIFRSobrevendido, 1, pintMediaTipo, funcoesBd, true);

			//Somente utiliza este filtro quando a tabela envolvida for a tabela do IFR diário
			strSql += " AND NOT EXISTS " + Environment.NewLine;
			strSql += "(";
			strSql += '\t' + " SELECT 1 " + Environment.NewLine;
			strSql += '\t' + " FROM IFR_Simulacao_Diaria S " + Environment.NewLine;
			strSql += '\t' + " WHERE C.Codigo = S.Codigo " + Environment.NewLine;
			strSql += '\t' + " AND C.Data = S.Data_Entrada_Efetiva " + Environment.NewLine;
			strSql += '\t' + " AND S.ID_Setup = " + funcoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSql += ")";

			//inclui ordenamento por data.
			strSql += " ORDER BY C.Data ";

			RS objRS = new RS(Conexao);

			List<CotacaoDiaria> lstRetorno = new List<CotacaoDiaria>();

	        string strTipoMedia = MediaTipoCalcular(pintMediaTipo);

			objRS.ExecuteQuery(strSql);


			while (!objRS.Eof) {
				CotacaoDiaria objCotacaoDiaria = new CotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data_Entrada")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("VALOR_ENTRADA"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

                MediaDiaria objMedia = new MediaDiaria(objCotacaoDiaria, strTipoMedia, 21, Convert.ToDouble(objRS.Field("MME21")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new MediaDiaria(objCotacaoDiaria, strTipoMedia, 49, Convert.ToDouble(objRS.Field("MME49")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new MediaDiaria(objCotacaoDiaria, strTipoMedia, 200, Convert.ToDouble(objRS.Field("MME200")));
				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new MediaDiaria(objCotacaoDiaria, "IFR2", 13, Convert.ToDouble(objRS.Field("MMIFR")));
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

		public IList<CotacaoDiaria> CarregarParaIFRComFiltro(Ativo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, System.DateTime pdtmDataInicial)
		{

			//Parâmetros: 
			//* pintOrdem: não terá importância. Só é importante quando são executados mais de um setup simualtaneamente
			//* pblnAcionamentoGerar: indica se é para retornar a data de acionamento do setup. Será passado
			//FALSE nesta chamada porque será feito uma verificação aqui nesta função se o setup será acionado.
			//Se o parâmetro for passado com valor TRUE retornará apenas as datas em que houver rompimento 
			//no período seguinte ao período em que o IFR cruzou a média. 
			string strSql = GeradorQuery.BackTestingIFRComFiltroEntradaQueryGerar(Conexao.ObterFormatadorDeCampo() , pobjAtivo.Codigo, "DIARIO", "TODOS", 1, pintMediaTipo, pdtmDataInicial, false, true);

			//inclui ordenamento por data.
			strSql += " ORDER BY Dia2.Data ";

			RS objRS = new RS(Conexao);

			objRS.ExecuteQuery(strSql);

			List<CotacaoDiaria> lstRetorno = new List<CotacaoDiaria>();

		    string strTipoMedia = MediaTipoCalcular(pintMediaTipo);

			objRS.ExecuteQuery(strSql);


			while (!objRS.Eof) {
				CotacaoDiaria objCotacaoDiaria = new CotacaoDiaria(pobjAtivo, Convert.ToDateTime(objRS.Field("Data_Entrada")));

				objCotacaoDiaria.Sequencial = Convert.ToInt32(objRS.Field("Sequencial"));
				objCotacaoDiaria.ValorAbertura = Convert.ToDecimal(objRS.Field("ValorAbertura"));
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));
				objCotacaoDiaria.ValorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
				objCotacaoDiaria.ValorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));

                MediaDiaria objMedia = new MediaDiaria(objCotacaoDiaria, strTipoMedia, 200, Convert.ToDouble(objRS.Field("MME200")));

				objCotacaoDiaria.Medias.Add(objMedia);

                objMedia = new MediaDiaria(objCotacaoDiaria, "IFR2", 13, Convert.ToDouble(objRS.Field("Media_IFR")));

				objCotacaoDiaria.Medias.Add(objMedia);

				lstRetorno.Add(objCotacaoDiaria);

				objRS.MoveNext();

			}

			objRS.Fechar();

			VerificaSeDeveFecharConexao();

			return lstRetorno;

		}

	    public ICollection<CotacaoOscilacao> CarregarOscilacaoPorAtivo(string codigo)
	    {
	        var funcoesBd = Conexao.ObterFormatadorDeCampo();
	        var sb = new StringBuilder();
	        sb
	            .Append("SELECT Data, 1 + Oscilacao / 100 AS Oscilacao ")
	            .Append("FROM Cotacao ")
	            .Append($"WHERE Codigo = {funcoesBd.CampoStringFormatar(codigo)} ")
                .Append("ORDER BY Data");

	        var rs = new RS(Conexao);
            rs.ExecuteQuery(sb.ToString());

	        var oscilacoes = new Collection<CotacaoOscilacao>();

	        while (!rs.Eof)
	        {
                var oscilacao = new CotacaoOscilacao
                {
                    Data = Convert.ToDateTime(rs.Field("Data")),
                    Oscilacao = Convert.ToDecimal(rs.Field("Oscilacao"))
                };
                oscilacoes.Add(oscilacao);
	            rs.MoveNext();
	        }

            rs.Fechar();

	        return oscilacoes;

	    }

	    public IDictionary<string, List<CotacaoOscilacao>> CarregarOscilacaoAPartirDe(DateTime dataInicialDados, ICollection<string> ativos)
	    {
	        var funcoesBd = Conexao.ObterFormatadorDeCampo();
	        var sb = new StringBuilder();
	        sb
	            .Append("SELECT Codigo, Data, 1 + Oscilacao / 100 AS Oscilacao ")
	            .Append("FROM Cotacao ")
	            .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicialDados)} ");

	        if (ativos.Any())
	        {
	            sb.Append($" AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar).ToArray())})");
	        }

	        sb.Append(" ORDER BY Codigo, Data");

	        var rs = new RS(Conexao);
	        rs.ExecuteQuery(sb.ToString());

	        var oscilacoes = new Collection<CotacaoOscilacao>();

	        while (!rs.Eof)
	        {
	            var oscilacao = new CotacaoOscilacao
	            {
                    Codigo = rs.Field("Codigo").ToString(),
	                Data = Convert.ToDateTime(rs.Field("Data")),
	                Oscilacao = Convert.ToDecimal(rs.Field("Oscilacao"))
	            };
	            oscilacoes.Add(oscilacao);
	            rs.MoveNext();
	        }

	        rs.Fechar();

            //ativos que não tiverem pelo menos 21 oscilações não pode ser calculada a volatilidade
	        IDictionary<string, List<CotacaoOscilacao>> dictionary = oscilacoes.GroupBy(o => o.Codigo)
                .Where(g => g.Count() >= 21)
                .ToDictionary(x => x.Key, x => x.ToList());

	        return dictionary;
	    }

	    public IDictionary<string, List<CotacaoNegocios>> CarregarNegociosAPartirDe(DateTime dataInicialDados, ICollection<string> ativos)
	    {
	        var funcoesBd = Conexao.ObterFormatadorDeCampo();
	        var sb = new StringBuilder();
	        sb
	            .Append("SELECT Codigo, Data, Negocios_Total ")
	            .Append("FROM Cotacao ")
	            .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicialDados)} ")
                .Append("AND Codigo NOT IN (SELECT CODIGO FROM ATIVOS_DESCONSIDERADOS) ");

	        if (ativos.Any())
	        {
	            sb.Append($" AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar).ToArray())})");
	        }

	        sb.Append(" ORDER BY Codigo, Data");

	        var rs = new RS(Conexao);
	        rs.ExecuteQuery(sb.ToString());

	        var negocios = new Collection<CotacaoNegocios>();

	        while (!rs.Eof)
	        {
	            var oscilacao = new CotacaoNegocios
                {
	                Codigo = rs.Field("Codigo").ToString(),
	                Data = Convert.ToDateTime(rs.Field("Data")),
	                Negocios = Convert.ToDecimal(rs.Field("Negocios_Total"))
	            };
	            negocios.Add(oscilacao);
	            rs.MoveNext();
	        }

	        rs.Fechar();

	        //ativos que não tiverem pelo menos 21 oscilações não pode ser calculada a volatilidade
	        IDictionary<string, List<CotacaoNegocios>> dictionary = negocios.GroupBy(o => o.Codigo)
	            .Where(g => g.Count() >= 21)
	            .ToDictionary(x => x.Key, x => x.ToList());

	        return dictionary;
	    }

    }

}
