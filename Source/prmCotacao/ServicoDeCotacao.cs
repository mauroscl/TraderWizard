using System.Linq;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataBase.Carregadores;
using prjDominio.Regras;
using prjServicoNegocio;
using Services;
using DataBase;
using prjDTO;
using prjConfiguracao;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{
    public class ServicoDeCotacao
	{

		private readonly Conexao _conexao;

	    private readonly SequencialService _sequencialService;
	    private readonly CotacaoData _cotacaoData;
	    private readonly GeradorQuery _geradorQuery;
	    private readonly AtualizadorDeIfr _atualizadorDeIfr;

	    //private readonly bool _localDataBaseConnection;

        private readonly IEnumerable<MediaDTO> _mediasDeFechamento = new List<MediaDTO>
        {
            new MediaDTO("A", 10, "VALOR"),
            new MediaDTO("A", 21, "VALOR"),
            new MediaDTO("A", 200, "VALOR")
        };

	    public ServicoDeCotacao(Conexao pobjConexao)
		{
		    _conexao = pobjConexao;
            _sequencialService = new SequencialService();
            _cotacaoData = new CotacaoData();
            _geradorQuery = new GeradorQuery(_conexao.ObterFormatadorDeCampo());
            _atualizadorDeIfr = new AtualizadorDeIfr();
		}

	    public ServicoDeCotacao():this(new Conexao())
	    {
	        //_localDataBaseConnection = true;
	    }

	    //~ServicoDeCotacao()
	    //{
	    //    if (this._localDataBaseConnection)
	    //    {
     //           this._conexao.FecharConexao();	            
	    //    }
	    //}


		public decimal UltimaMediaConsultar(string pstrCodigo)
		{
		    cRS objRS = new cRS(_conexao);

		    string strQuery = " select ValorMedio " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " order by Data desc ";

			objRS.ExecuteQuery(strQuery);

			decimal functionReturnValue = Convert.ToDecimal(objRS.Field("ValorMedio", "0"));

			objRS.Fechar();
			return functionReturnValue;

		}


		/// <summary>
		/// Consulta alguns dados da tabela de cotação ou cotação semanal
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmData">data em que deve ser feita a consulta</param>
		/// <param name="pstrTabela">
		/// COTACAO
		/// COTACAO_SEMANAL
		/// </param>
		/// <param name="pdecValorFechamentoRet"></param>
		/// <param name="pdecValorAberturaRet"></param>
		/// <param name="pobjConexao"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CotacaoConsultar(string pstrCodigo, DateTime pdtmData, string pstrTabela, ref decimal pdecValorFechamentoRet, ref decimal pdecValorAberturaRet, Conexao pobjConexao = null)
		{

		    cRS objRS = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

		    string strQuery = " select ValorAbertura, ValorFechamento " + " from " + pstrTabela + " where Data = " + FuncoesBd.CampoDateFormatar(pdtmData) + " and Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo);

			objRS.ExecuteQuery(strQuery);

			pdecValorAberturaRet = Convert.ToDecimal(objRS.Field("ValorAbertura", "0"));
			pdecValorFechamentoRet = Convert.ToDecimal(objRS.Field("ValorFechamento", "0"));

			bool functionReturnValue = objRS.DadosExistir;

			objRS.Fechar();
			return functionReturnValue;

		}



		/// <summary>
		/// Calcula a cotação imediatamente posterior a uma data para um determinado ativo.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pdtmDataBase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private DateTime AtivoCotacaoPosteriorDataConsultar(string pstrCodigo, DateTime pdtmDataBase)
		{
		    cRS objRsData = new cRS(_conexao);

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			objRsData.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data > " + FuncoesBd.CampoDateFormatar(pdtmDataBase));

			DateTime functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

			objRsData.Fechar();
			return functionReturnValue;

		}

		private DateTime AtivoPrimeiraCotacaoDataConsultar(string pstrCodigo)
		{
		    cRS objRS = new cRS(_conexao);

			objRS.ExecuteQuery("SELECT MIN(Data) AS Data " + Environment.NewLine + "FROM Cotacao " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

			var functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Busca a data da cotação imediatamente anterior a uma data recebida por parâmetro
		/// </summary>
		/// <param name="pdtmDataBase">Data base utilizada para buscar a cotação anterior</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime CotacaoAnteriorDataConsultar(DateTime pdtmDataBase, string pstrTabela, Conexao pobjConexao = null)
		{
		    cRS objRsData = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			//Busca a data imediatamente anterior que tem uma cotação para o ativo recebido por parâmetro.
			objRsData.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Data < " + funcoesBd.CampoDateFormatar(pdtmDataBase));

			DateTime functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

			objRsData.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a oscilação de todas as cotações
		/// </summary>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer os cálculos 
		/// apenas nas datas em que houver split. Esta opção é utilizada no recálculo quando
		/// há importação de proventos. 
		/// ATENÇÃO: quando este parâmetro for TRUE, é obrigatório passar o parâmetro
		/// "pdtmDataInicial" com uma data válida</param>
		/// <param name="pblnPercentualCalcular">Indica se é para calcular o percentual. 
		/// A diferença sempre é calculada</param>
		/// <param name="pdtmDataInicial">Data inicial para o início dos cálculos</param>
		/// <param name="pstrAtivos">Lista dos ativos para os quais deve ser feito
		/// os cálculos de oscilação separados por "#[código]#"
		/// </param>
		/// <returns>status da transação</returns>
		/// <remarks></remarks>
		private bool OscilacaoGeralCalcular(bool pblnPercentualCalcular, DateTime pdtmDataInicial, string pstrAtivos = "", bool pblnConsiderarApenasDataSplit = false)
		{

			if (pblnConsiderarApenasDataSplit && pdtmDataInicial == Constantes.DataInvalida) {
				//esta mensagem dificilmente será disparada. Só vai acontecer se houver erro de programação,
				//pois o desenvolvedor tem que saber que se quiser calcular apenas nas datas em que há split
				//tem que passar a data inicial como parâmetro. Este controle é apenas uma segurança para 
				//evitar erros na base sem que sejam percebidos.
                MessageBox.Show("Não é possivel calcular as oscilações apenas nas datas em que há splits " + "se não for recebida uma data inicial válida", "Trader Wizard",MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

			}

			cCommand objCommand = null;

			//não passa a conexão global para criar uma nova conexão
			cRS objRSAtivo = new cRS(_conexao);
			cRS objRSCotacao = null;

			cRSList objRSSplit = null;

			//Dim blnSplitExistir As Boolean

			Conexao objConnAux = new Conexao();


		    decimal decOscilacao = default(decimal);
		    decimal decCotacaoAnterior = default(decimal);

		    //contador do número de iterações por papel

		    DateTime dtmCalculoDataInicial = Constantes.DataInvalida;

			string strWhere = String.Empty;

			bool blnRetorno = true;

		    var conexaoSplit = new Conexao();

		    var objCarregadorSplit = new cCarregadorSplit(conexaoSplit);

			//busca todos os ativos do período.
			//utiliza o group by que parece ser mais eficiente que o distinct

			string strQuery = " select Codigo " + " from Cotacao ";

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			if (pdtmDataInicial != Constantes.DataInvalida) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";

				//se passou uma data inicial busca as cotações a partir de uma data.
				strWhere = strWhere + " Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial);

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";


			    if (_conexao.BancoDeDados == cEnum.BancoDeDados.SqlServer)
			    {
			        //remove sustenido do inicio
			        string ativosAux = pstrAtivos.Remove(0, 1);

			        //remove ultimo sustenido
			        ativosAux = ativosAux.Remove(ativosAux.Length - 1);

			        //faz split pelo sustenido
			        string[] ativosSelecionados = ativosAux.Split('#');

			        strWhere += "Codigo IN (" + string.Join(", ", ativosSelecionados.Select(funcoesBd.CampoFormatar)) + ")";

			    }
			    else
			    {
			        string sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
			        strWhere += funcoesBd.IndiceDaSubString(funcoesBd.ConcatenarStrings(new[] {sustenidoFormatado, "Codigo", sustenidoFormatado}),FuncoesBd.CampoStringFormatar(pstrAtivos)) + " > 0";
			    }

			}

			if (!string.IsNullOrEmpty(strWhere)) {
				strQuery = strQuery + " where " + strWhere;

			}

			strQuery = strQuery + " group by Codigo ";

			objRSAtivo.ExecuteQuery(strQuery);

			//loop para percorrer todos os códigos de ativos

			while ((!objRSAtivo.EOF) && (blnRetorno)) {
				//utiliza uma variável para não precisar chamar a função field várias vezes
				var strCodigo = (string) objRSAtivo.Field("Codigo");


				if (pblnConsiderarApenasDataSplit) {
					//chama função que calcula a oscilação apenas nas datas em que há split
					blnRetorno = OscilacaoUnitCalcularApenasSplit(strCodigo, pdtmDataInicial);


				} else {
					if (objRSCotacao == null) {
						objRSCotacao = new cRS(objConnAux);
					}

					if (objCommand == null) {
						objCommand = new cCommand(objConnAux);
					}

					//inicializa o contador para cada ativo
					int intContador = 1;

					objCommand.BeginTrans();


					if (pdtmDataInicial != Constantes.DataInvalida) {
						dtmCalculoDataInicial = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(strCodigo, pdtmDataInicial, "Cotacao", objConnAux);

					}

					//busca as cotações do ativo ordenado por data crescente
					//a partir da cotação anterior à data inicial recebida por parâmetro 
					//(se for recebida uma data válida)
					strQuery = " select Data, ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo);


					if (dtmCalculoDataInicial != Constantes.DataInvalida) {
						strQuery = strQuery + " and Data >= " + funcoesBd.CampoDateFormatar(dtmCalculoDataInicial);

					}

					strQuery = strQuery + " order by Data ";

					objRSCotacao.ExecuteQuery(strQuery);

				    IList<CotacaoFechamentoDto> cotacoes = new List<CotacaoFechamentoDto>();

				    while (!objRSCotacao.EOF)
				    {
				        cotacoes.Add(new CotacaoFechamentoDto
				        {
                            DataInicial = Convert.ToDateTime(objRSCotacao.Field("Data")),
                            ValorDeFechamento = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento"))
				        });

                        objRSCotacao.MoveNext();
				    }

                    objRSCotacao.Fechar();

				    CotacaoFechamentoDto primeiraCotacao = cotacoes.First();

				    //verifica se tem splits. Tem que verificar se tem um dia após a primeira data. 
					//A primeira data não tem que ser considerada, pois a oscilação é calculada
					//apenas a partir da segunda data.
					objCarregadorSplit.SplitConsultar(strCodigo, primeiraCotacao.DataInicial.AddDays(1), "A", ref objRSSplit,Constantes.DataInvalida);


				    foreach (var cotacao in cotacoes)
				    {
				        if (!objCommand.TransStatus)
				        {
				            break;
				        }
		        
						if (intContador > 1) {
							//só pode colocar a oscilação depois da segunda cotação pois precisa de duas para
							//fazer a relação de uma para outra.


							if (!objRSSplit.EOF) {
								//*****IMPORTANTE: Alterado de IF para WHILE porque podem ocorrer casos em que na mesma data haja um desdobramento e uma bonificação.
								//Neste caso o RS de splits vai retornar mais de um registro para a mesma data
								bool blnContinuarLoop = (cotacao.DataInicial == Convert.ToDateTime(objRSSplit.Field("Data")));


								while (blnContinuarLoop) {
									//se tem split na data, tem que multiplicar a cotação da data anterior pelo split
									decCotacaoAnterior = decCotacaoAnterior * Convert.ToDecimal(objRSSplit.Field("Razao"));

									//move para o próximo registro de split.
									objRSSplit.MoveNext();

									if (objRSSplit.EOF) {
										blnContinuarLoop = false;
									} else {

										if (cotacao.DataInicial != Convert.ToDateTime(objRSSplit.Field("Data"))) {
											blnContinuarLoop = false;

										}

									}

								}

							}


							if (pblnPercentualCalcular) {
								decOscilacao = decCotacaoAnterior > 0 ? Math.Round((cotacao.ValorDeFechamento / decCotacaoAnterior - 1) * 100, 2) : 0;
							}


							//atualiza o registro com a oscilacao
							strQuery = " UPDATE Cotacao SET " + " Diferenca = " + FuncoesBd.CampoDecimalFormatar(cotacao.ValorDeFechamento - decCotacaoAnterior);


							if (pblnPercentualCalcular) {
								strQuery = strQuery + ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao);

							}

							strQuery = strQuery + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo) + " AND Data = " + funcoesBd.CampoDateFormatar(cotacao.DataInicial);

							objCommand.Execute(strQuery);

						}
						//if intContador > 1

						//cotação anterior recebe a cotação atual para ser usada na próxima iteração.
						decCotacaoAnterior = cotacao.ValorDeFechamento;

						intContador = intContador + 1;

					}
					//fim do while que retorna as cotações de um ativo específico

					objCommand.CommitTrans();

					if (!objCommand.TransStatus) {
						//se ocorrer erro na transação realizada para um dos ativos retorna FALSE
						blnRetorno = false;

					}

					//Alterado por mauro, 12/05/2010 - removido porque não faz sentido fechar a conexão dentro do loop;
					//objConnAux.FecharConexao()

					//objCommand = Nothing

				}
				//if pblnConsiderarApenasDataSplit

				objRSAtivo.MoveNext();

			}
			//fim do while do RS que contém todos os arquivos.

		    objConnAux.FecharConexao();
		    objRSAtivo.Fechar();

			return blnRetorno;

		}

		/// <summary>
		/// Calcula a oscilação apenas nas datas em que há split. Esta é uma função unitária,
		/// ou seja, calcula a oscilação para apenas um ativo.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo para o qual será feito o cálculo.</param>
		/// <param name="pdtmDataInicial">Data inicial dos cálculos</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool OscilacaoUnitCalcularApenasSplit(string pstrCodigo, DateTime pdtmDataInicial)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRSCotacao = new cRS(objConnAux);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

		    cRSList objRssListSplit = null;

		    double dblSplitAcumulado = 1;

			objCommand.BeginTrans();

			objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial.AddDays(1), "A", ref objRssListSplit, Constantes.DataInvalida);

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			while (!objRssListSplit.EOF) {
				//fica multiplicando o split. pode haver casos em que tenho mais de um registro para a mesma data
				dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRssListSplit.Field("Razao"));

				DateTime dtmDataAtual = Convert.ToDateTime(objRssListSplit.Field("Data"));


				if (dtmDataAtual != Convert.ToDateTime(objRssListSplit.NextField("Data", Constantes.DataInvalida))) {
					//Quando o próximo registro tiver data diferente tem que atualizar a oscilação e a diferença
					// da data atual, pois na próxima iteração os cálculos já serão referentes à outra data.

					DateTime dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataAtual, "Cotacao", objConnAux);

					//busca a cotação anterior do ativo.
					string strQuery = " select ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData);

					objRSCotacao.ExecuteQuery(strQuery);

					//multiplica a cotação anterior ao split
					decimal decCotacaoAnterior = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento")) * (decimal) dblSplitAcumulado;

					//fecha o RS para ser utilizado novamente.
					objRSCotacao.Fechar();

					//consulta a cotação atual. Esta não deve ser multiplicada pelo split
					strQuery = " select ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmDataAtual);

					objRSCotacao.ExecuteQuery(strQuery);

					decimal decCotacaoAtual = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento"));

					objRSCotacao.Fechar();

				    decimal decOscilacao = decCotacaoAnterior > 0 ? Math.Round((decCotacaoAtual / decCotacaoAnterior - 1) * 100, 2) : 0;

					//atualiza o registro com a oscilacao
					strQuery = " UPDATE Cotacao SET " + Environment.NewLine + " Diferenca = " + FuncoesBd.CampoDecimalFormatar(decCotacaoAtual - decCotacaoAnterior) + Environment.NewLine + ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " AND Data = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRssListSplit.Field("Data")));

					objCommand.Execute(strQuery);

					//Inicializa a variável com 1 novamente, pois a próxima iteração irá se referir a uma data diferente
					dblSplitAcumulado = 1;

				}
				//If dtmDataAtual <> CDate(objRSSListSplit.NextField("Data", DataInvalida)) Then

				objRssListSplit.MoveNext();

			}

			objCommand.CommitTrans();

		    objConnAux.FecharConexao();
            return objCommand.TransStatus;

		}

		/// <summary>
		/// Soma o campo de uma determinada tabela no período indicado.
		/// </summary>
        /// <param name="pstrCodigo"></param>
        /// <param name="pdtmDataInicial"></param>
        /// <param name="pdtmDataFinal"></param>
        /// <param name="pstrTabela">Cotacao ou Cotacao_Semanal</param>
        /// <param name="pstrCampo">Volume ou ValorFechamento ou outro campo que for necessário</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private decimal CotacaoPeriodoCampoSomar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrTabela, string pstrCampo)
		{
		    cRS objRS = new cRS(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			objRS.ExecuteQuery(" select sum(" + pstrCampo + ") as total " + " from " + pstrTabela + " where codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal));

			decimal functionReturnValue = Convert.ToDecimal(objRS.Field("total"));

			objRS.Fechar();
			return functionReturnValue;

		}

	    /// <summary>
	    /// Calcula a média móvel aritmética para um determinado número de períodos
	    /// </summary>
	    /// <param name="pstrCodigo">Código do ativo que será calcula a média móvel simples</param>
	    /// <param name="pdtmDataInicial"> Data inicial do cálculo da média</param>
	    /// <param name="pdtmDataFinal">Data final do cálculo da média. Quando o período for semanal, 
	    /// esta data equivale ao primeiro dia da última semana.</param>
	    /// <param name="pstrTabela">
	    /// Cotacao
	    /// Cotacao_Semanal
	    /// </param>
	    /// <param name="pstrDado">"VALOR" ou "VOLUME"</param>
	    /// <param name="proximoPeriodo"></param>
	    /// <param name="pintNumPeriodos">número de períodos utilizado no cálculo da média</param>
	    /// <param name="pobjConexao">objeto de conexão com o banco de dados</param>
	    /// <returns>
	    /// Retorna a média calculada.
	    /// </returns>
	    /// <remarks></remarks>
	    private double MediaAritmeticaCalcular(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, DateTime proximoPeriodo, int pintNumPeriodos, string pstrTabela, string pstrDado, Conexao pobjConexao = null)
		{
			double functionReturnValue;

		    cRS objRS = pobjConexao == null ? new cRS(_conexao) : new cRS(pobjConexao);

			cRSList objRSListSplit = null;

			bool blnSplitExistir = false;

		    //contém a razão acumulada de todos os splits do período.
			double dblSplitAcumulado = 1;
			double dblDadoAcumulado = 0;

			string strCampo;

			int intIFRNumPeriodos = -1;

			string strSplitTipo = String.Empty;

			if (pstrDado == "VALOR") {
				strCampo = "ValorFechamento";

			} else if (pstrDado == "VOLUME") {
				strCampo = "Titulos_Total";
				//Quando é média de volume considera apenas os desdobramentos.
				strSplitTipo = "DESD";

			} else {
				//IFR
				strCampo = "Valor";

				//quando é IFR o dado está sempre no formato IFRxx, onde xx é o IFR de xx períodos, para
				//o qual deve ser calculada a média.
				//com isso, para saber o número de períodos busca a substring a partir do 4º caracter,
				//pois os três primeiros são: "IFR"
                intIFRNumPeriodos = Convert.ToInt16(pstrDado.Substring(3));

			}


			if (intIFRNumPeriodos == -1) {

                DateTime dtmDataFinalBuscaSplit = pstrTabela.ToUpper() == "COTACAO_SEMANAL" ? _cotacaoData.AtivoCotacaoSemanalUltimoDiaSemanaCalcular(pstrCodigo, pdtmDataFinal) : pdtmDataFinal;

				var objCarregadorSplit = new cCarregadorSplit(objRS.Conexao);

				//busca os splits do ativo no período, ordenado pelo último split.
				//Tem que começar a buscar um dia após 
				blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, proximoPeriodo, "D", ref objRSListSplit, dtmDataFinalBuscaSplit, strSplitTipo);

			}


		    if (blnSplitExistir) {

				DateTime dtmDataFinalSplit = pdtmDataFinal;

				//quando tem split todas as cotaçoes tem que ser convertidas de acordo com o split
		        DateTime dataDoUltimoSplit = Constantes.DataInvalida;

		        DateTime dtmDataInicialSplit;
		        while (!objRSListSplit.EOF) {
		            DateTime dataDoSplitAtual = Convert.ToDateTime(objRSListSplit.Field("Data"));

		            if (dataDoUltimoSplit != dataDoSplitAtual)
                    {
                        dtmDataInicialSplit = pstrTabela.ToUpper() == "COTACAO" ?  dataDoSplitAtual : _cotacaoData.PrimeiraSemanaDataCalcular(dataDoSplitAtual);

						dblDadoAcumulado = dblDadoAcumulado + (double) CotacaoPeriodoCampoSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, pstrTabela, strCampo) * dblSplitAcumulado;

						//a data final para o próximo período e um dia antes ao periodo anterior
						dtmDataFinalSplit = dtmDataInicialSplit.AddDays(-1);

					}


					if (pstrDado == "VALOR") {
						//acumula as cotações entre as datas do split multiplicando pelo split acumulado
						//multiplica o split anterior pelo split da data para o próximo periodo
						dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("Razao"));



					} else {
						//TRATA O VOLUME
						dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));

					}


		            dataDoUltimoSplit = dataDoSplitAtual;

					objRSListSplit.MoveNext();

				}

				//após o loop de splits ainda tem que somar o acumulado entre a data inicial e o primeiro split.
				dtmDataInicialSplit = pdtmDataInicial;

				dblDadoAcumulado = dblDadoAcumulado + (double) CotacaoPeriodoCampoSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, pstrTabela, strCampo) * dblSplitAcumulado;

				//divide a cotação acumulada pelo número de periodos
				functionReturnValue = dblDadoAcumulado / pintNumPeriodos;


			} else {

                FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

				//se não tem split calcula a média no próprio banco.
				string strQuery = " select avg(" + funcoesBd.ConverterParaPontoFlutuante(strCampo)  + ") as Media " + 
                    " from " + pstrTabela + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + 
                    " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + 
                    " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal);

				if (intIFRNumPeriodos != -1) {
					//se está calculando média do IFR, coloca no where o número de periodos
					//do IFR que está sendo calculado a média.
					strQuery = strQuery + " AND NumPeriodos = " + intIFRNumPeriodos;

				}

				objRS.ExecuteQuery(strQuery);

				functionReturnValue = Convert.ToDouble(objRS.Field("Media", "0"));

			}

			objRS.Fechar();

			return functionReturnValue;

		}

	    private double MediaExponencialCalcular(decimal pdecValorFechamento, int pintPeriodo, decimal pdecMMExpAnterior)
	    {
	        //MMEx = ME(x-1) + K x {Fech(x) – ME(x-1)} 

			//* MMEx representa a média móvel exponencial no dia x
			//* ME(x-1) representa a média móvel exponencial no dia x-1
			//* N é o número de dias para os quais se quer o cálculo
			//* Constante K = {2 ÷ (N+1)} 

			// calcula a constante K
	        decimal decConstanteK = new decimal(2) / (pintPeriodo + 1);

	        return Convert.ToDouble(pdecMMExpAnterior + decConstanteK * (pdecValorFechamento - pdecMMExpAnterior));
	    }


	    /// <summary>
	    /// Atualiza a média no banco de dados na tabela de média diária e média semanal
	    /// </summary>
	    /// <param name="pstrCodigo">Código do ativo</param>
	    /// <param name="pdtmData">data da média</param>
	    /// <param name="pintNumPeriodos"></param>
	    /// <param name="pstrTabela">
	    ///     Cotacao
	    ///     Cotacao_Semanal
	    /// </param>
	    /// <param name="pdblMedia">Valor da média que será atualizada
	    /// </param>
	    /// <param name="pstrMediaTipo"></param>
	    /// <param name="pobjConexao"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private void MediaAtualizar(string pstrCodigo, DateTime pdtmData, int pintNumPeriodos, string pstrTabela, double pdblMedia, string pstrMediaTipo, Conexao pobjConexao = null)
		{
		    cCommand objCommand = pobjConexao == null ? new cCommand(_conexao) : new cCommand(pobjConexao);

		    pstrTabela = pstrTabela.ToUpper();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			string strQuery = " INSERT INTO " + pstrTabela + "(Codigo, Data, NumPeriodos, Tipo, Valor)" + " VALUES " + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + funcoesBd.CampoDateFormatar(pdtmData) + ", " + pintNumPeriodos.ToString() + ", " + FuncoesBd.CampoStringFormatar(pstrMediaTipo) + ", " + FuncoesBd.CampoFloatFormatar(pdblMedia) + ")";

			objCommand.Execute(strQuery);
		}

		/// <summary>
		/// Calcula a média móvel exponencial para um determinado ativo desde a data inicial até a data da cotação 
		/// mais recente encontrada
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">data inicial para cálculo </param>
		/// <param name="pintNumPeriodos">número de dias utilizado no cálculo da média</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// DATABASE = SALVA OS VALORES CALCULADOS NO DATABASE
		/// MEMORIA = SALVA OS VALORES CALCULADOS EM UMA ESTRUTURA DE MEMÓRIA
		/// </param>
		/// <param name="pdtmCotacaoAnteriorData">Data da cotação anterior. Só é passado um valor válido para este parâmetro
		///  quando o parâmetro pdtmDataBase não é uma DATAINVALIDA. Este parâmetro é utilizado para otimizar o código,
		/// já que a função CotacaoAnteriorDataConsultar pode ser utilizada várias vezes se a função de cálculo de média 
		/// for chamada várias vezes.</param>
		/// <returns>
		/// RetornoOK = operação realizada com sucesso
		/// RetornoErroInesperado = algum erro de banco de dados ou de programação.
		/// RetornoErro2 = erro na execução da operação em um ou mais dias
		/// RetornoErro3 = Não existe o número de cotações suficientes para fazer o cálculo.
		/// </returns>
		/// <remarks></remarks>
		private cEnum.enumRetorno MediaMovelExponencialRetroativaUnitCalcular(string pstrCodigo, DateTime pdtmDataBase, int pintNumPeriodos, string pstrTabela
            , DateTime pdtmCotacaoAnteriorData)
		{
			Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRS = new cRS(objConnAux);
			cRSList objRsSplit = null;

		    pstrTabela = pstrTabela.ToUpper();

		    string strTabelaMedia = pstrTabela == "COTACAO" ? "Media_Diaria" : "Media_Semanal";

			double dblMmExpAnterior = 0;
		    //Dim dtmDataFinalSplit As Date = DataInvalida

		    bool blnPeriodoCalcular = false;


			//**********************inicia transação
			objCommand.BeginTrans();

			//If pintNumPeriodos = 21 Then

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			if (pdtmDataBase != Constantes.DataInvalida)
			{
			    //verifica se existe uma média móvel em uma data anterior.
				//para isso busca a cotação anterior a esta

				//--04/01/2010
				//alterado para receber a data da cotação anterior por parâmetro.
				//dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataBase, pstrTabela, objConnAux)

			    DateTime dtmCotacaoAnteriorData = pdtmCotacaoAnteriorData;
			    //--fim da alteração do dia 04/01/2010


				if (dtmCotacaoAnteriorData != Constantes.DataInvalida) {
					//se tem busca a média móvel de 21 dias na data

					objRS.ExecuteQuery(" select Valor " + " from " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData) + " and NumPeriodos = " + pintNumPeriodos.ToString() + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME"));

					//verifica se o campo está preenchido e está com valor maior que zero.
					//If CDec(objRS.Field("MMExp21", 0)) > 0 Then
					if (Convert.ToDecimal(objRS.Field("Valor", 0)) > 0) {
						dblMmExpAnterior = Convert.ToDouble(objRS.Field("Valor"));
					} else {
						//se tem cotação mas não tem média calculada, tem que calcular o período inicial.
						blnPeriodoCalcular = true;
					}

					objRS.Fechar();


				} else {
					//se não encontrou cotação tem que calcular o período inicial
					blnPeriodoCalcular = true;

				}
			}
			else {
				//se não recebeu uma data válida para utilizar como data base, então tem que calcular o período
				blnPeriodoCalcular = true;

			}


			if (blnPeriodoCalcular)
			{
			    //verifica se existe o número de períodos necessários para fazer pelo menos um cálculo e retorna o
				//periodo para calcular a primeira média, que é a média simples.

			    var dtmDataInicial = new DateTime();
			    var dtmDataFinal = new DateTime();
			    if (_cotacaoData.NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true, ref dtmDataInicial, ref dtmDataFinal, pstrTabela,-1 , objConnAux))
				{
				    var calculadorData = new cCalculadorData(_conexao);

				    DateTime proximoPeriodo = calculadorData.CalcularDataProximoPeriodo(pstrCodigo, dtmDataInicial, pstrTabela);

				    //calcula a média simples no periodo, que será usado como primeira média
					dblMmExpAnterior = MediaAritmeticaCalcular(pstrCodigo, dtmDataInicial, dtmDataFinal, proximoPeriodo, pintNumPeriodos, pstrTabela, "VALOR", objConnAux);

					objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataFinal) + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME") + " and NumPeriodos = " + pintNumPeriodos.ToString());

					//atualiza a média na tabela 
					MediaAtualizar(pstrCodigo, dtmDataFinal, pintNumPeriodos, strTabelaMedia, dblMmExpAnterior, "MME", objConnAux);

					//Calcula a data da próxima cotação conforme a periodicidade do cálculo.
					cCalculadorData objCalculadorData = new cCalculadorData(objConnAux);

					//Se a data base retornar DATAINVALIDA significa que a média aritmética que foi calculada anteriormente é exata a do número de periodos
					//que estamos calculando e nesse caso não tem mais nenhuma média para calcular.
					pdtmDataBase = objCalculadorData.CalcularDataProximoPeriodo(pstrCodigo, dtmDataFinal, pstrTabela);

				} else {
					//se não encontrou um intervalo de datas que também tenha o mesmo número de periodos sai da função retornando o erro.
					objCommand.RollBackTrans();
					return cEnum.enumRetorno.RetornoErro3;
				}
			}


		    if (pdtmDataBase != Constantes.DataInvalida) {

				if (!blnPeriodoCalcular) {
					//EXCLUI OS DADOS A PARTIR DA DATA BASE DO CÁLCULO, POIS ESTES DADOS SERÃO RECALCULADOS.
					//NÃO EXCLUI SE O PERÍODO FOI CALCULADO. NESTE CASO JÁ FOI EXCLUÍDO ANTERIORMENTE.
					objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME") + " and NumPeriodos = " + pintNumPeriodos.ToString());

				}

				//busca todas as cotações a partir da data base. Busca o valor de fechamento, pois a média é calculada em cima deste valor.
				string strQuery = " select Data, ValorFechamento ";


				if (pstrTabela.ToUpper() == "COTACAO_SEMANAL") {
					//SE É COTAÇÃO SEMANAL TEM QUE BUSCAR A DATA FINAL PARA USAR NO CÁLCULO DOS SPLITS
					strQuery = strQuery + ", DataFinal";

				}

				strQuery = strQuery + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " order by Data ";

				objRS.ExecuteQuery(strQuery);

			    var cotacoes = new List<CotacaoFechamentoDto>();

			    while (!objRS.EOF)
			    {
			        var cotacaoFechamentoDto = new CotacaoFechamentoDto
			        {
			            DataInicial = Convert.ToDateTime(objRS.Field("Data")),
			            ValorDeFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"))
			        };
			        if (pstrTabela == "COTACAO_SEMANAL")
			        {
			            cotacaoFechamentoDto.DataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));
			        }
                    
                    cotacoes.Add(cotacaoFechamentoDto);
			        objRS.MoveNext();
			    }
                objRS.Fechar();

				cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

				//busca todos os splits a partir da data base em ordem ascendente
				objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataBase, "A", ref objRsSplit, Constantes.DataInvalida);

				//loop para calcular a média exponencial em todas as datas subsequentes

				foreach (var cotacaoFechamentoDto in cotacoes) {

					if (!objRsSplit.EOF) {
						bool blnContinuarLoop;


						if (pstrTabela == "COTACAO") {
							//********TRATAMENTO PARA A COTAÇÃO DIÁRIA
							blnContinuarLoop = (cotacaoFechamentoDto.DataInicial == Convert.ToDateTime(objRsSplit.Field("Data")));

							//verifica se tem split na data.

							while (blnContinuarLoop) {
								//se a data do split é a mesma data do cálculo da média

								//multiplica a média anterior pelo split
								dblMmExpAnterior = dblMmExpAnterior * Convert.ToDouble(objRsSplit.Field("Razao"));

								//passa para o próximo registro
								objRsSplit.MoveNext();

								if (objRsSplit.EOF) {
									blnContinuarLoop = false;
								} else {

									if (cotacaoFechamentoDto.DataInicial != Convert.ToDateTime(objRsSplit.Field("Data"))) {
										blnContinuarLoop = false;

									}

								}

							}


						} else {
							//*********TRATAMENTO PARA A COTAÇÃO SEMANAL
							//para aplicar o split na cotação semanal, a data do split tem que estar entre 
							//o primeiro e ó último dia da semana.

							blnContinuarLoop = (Convert.ToDateTime(objRsSplit.Field("Data")) >= cotacaoFechamentoDto.DataInicial 
                                && Convert.ToDateTime(objRsSplit.Field("Data")) <= cotacaoFechamentoDto.DataFinal);


							while (blnContinuarLoop) {
								//multiplica a média anterior pelo split
								dblMmExpAnterior = dblMmExpAnterior * Convert.ToDouble(objRsSplit.Field("Razao"));

								//passa para o próximo registro
								objRsSplit.MoveNext();

								if (objRsSplit.EOF) {
									blnContinuarLoop = false;

								} else {
									blnContinuarLoop = (Convert.ToDateTime(objRsSplit.Field("Data")) >= cotacaoFechamentoDto.DataInicial 
                                        && Convert.ToDateTime(objRsSplit.Field("Data")) <= cotacaoFechamentoDto.DataFinal);

								}

							}

						}

					}

					//calcula a média 
					double dblMediaMovelMExponencial = MediaExponencialCalcular(cotacaoFechamentoDto.ValorDeFechamento, pintNumPeriodos, (decimal) dblMmExpAnterior);

                    MediaAtualizar(pstrCodigo, cotacaoFechamentoDto.DataInicial, pintNumPeriodos, strTabelaMedia, dblMediaMovelMExponencial, "MME", objConnAux);

					//atribui a média calculada como média anterior para a próxima iteração;
					dblMmExpAnterior = dblMediaMovelMExponencial;

				}

			}

            objCommand.CommitTrans();

			objConnAux.FecharConexao();
            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		}

	    ///  <summary>
	    ///  Calcula a médida móvel exponencial de todos os papéis.
	    ///  </summary>
	    ///  <param name="pdtmDataInicial">DAta inicial para fazer o cálculo. 
	    ///  Se não for passsada uma data inicial calcula desde a primeira cotação</param>
	    /// <param name="pstrPeriodoDuracao">
	    ///  Período utilizado no cálculo: "DIARIO" ou "SEMANAL"
	    ///  </param>
	    ///  <param name="plstMedias">lista contendo as médias que devem ser calculadas</param>
	    ///  <param name="pstrAtivos">
	    ///  Código dos ativos separados pelo caracter "#"    
	    ///  </param>    
	    ///  <returns>
	    ///  TRUE - A MÉDIA MÓVEL EXPONENCIAL FOI CALCULADA PARA TODOS OS ATIVOS SEM NENHUM ERRO.
	    ///  FALSE - OCORREU ERRO NA EXECUÇÃO DA MÉDIA MÓVEL PARA PELO MENOS UM DOS ATIVOS.
	    ///  </returns>
	    ///  <remarks></remarks>
	    private bool MediaMovelGeralCalcular(string pstrPeriodoDuracao, IEnumerable<MediaDTO> plstMedias, DateTime pdtmDataInicial, string pstrAtivos)
		{
			bool functionReturnValue = false;

			//**********PARA BUSCAR OS ATIVOS NÃO PODE USAR A MESMA CONEXÃO DA TRANSAÇÃO,
			//**********POIS SE A TRANSAÇÃO FIZER ROLLBACK PARA UM ATIVO O RECORDSET NÃO IRÁ FUNCIONAR MAIS.
			cRS objRSAtivo = new cRS();

		    bool blnRetorno = true;

		    string strWhere = String.Empty;

			string strLog = "";

			//indica o nome da tabela de cotações, de acordo com a duração do período das cotações

		    string strTabela = pstrPeriodoDuracao == "DIARIO" ? "Cotacao" : "Cotacao_Semanal";


			//busca todos os ativos do período e a menor data para ser utilizada como data base.
			string strQuery = " select Codigo";

			if (pdtmDataInicial != Constantes.DataInvalida) {
				//se foi passado uma data inicial válida, calcula a data inicial a partir desta data.
				strQuery = strQuery + ", min(Data) as DataInicial ";
			}

			strQuery = strQuery + " from " + strTabela;

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			if (pdtmDataInicial != Constantes.DataInvalida) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";


				if (pstrPeriodoDuracao == "DIARIO") {
					//se passou uma data inicial busca as cotações a partir de uma data.
					strWhere = strWhere + " Data >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial);


				} else {
					//se é uma cotação semanal, a data recebida por parâmetro tem que estar entre 
					//a data inicial e a data final da semana,
					//ou então tem que estar na próxima semana, caso a data informada seja uma data de final de 
					//semana ou feriado que esteja ligado com final de semana 
					strWhere = strWhere + " ((Data <= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal >= " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + ")" + " or Data > " + funcoesBd.CampoDateFormatar(pdtmDataInicial) + ")";

				}

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " And ";

			    if (_conexao.BancoDeDados == cEnum.BancoDeDados.SqlServer)
			    {
                    //remove sustenido do inicio
			        string ativosAux = pstrAtivos.Remove(0, 1);

                    //remove ultimo sustenido
			        ativosAux = ativosAux.Remove(ativosAux.Length - 1);

                    //faz split pelo sustenido
			        string[] ativosSelecionados = ativosAux.Split('#');

			        strWhere += "Codigo IN (" + string.Join(", ", ativosSelecionados.Select(funcoesBd.CampoFormatar)) +  ")";

			    }
			    else
			    {
                    string sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
                    strWhere += funcoesBd.IndiceDaSubString(funcoesBd.ConcatenarStrings(new[] { sustenidoFormatado, "Codigo", sustenidoFormatado }), FuncoesBd.CampoStringFormatar(pstrAtivos)) + " > 0";
			        
			    }


			}

			if (!string.IsNullOrEmpty(strWhere)) {
				strQuery = strQuery + " where " + strWhere;

			}

			strQuery = strQuery + " group by Codigo ";

			objRSAtivo.ExecuteQuery(strQuery);

		    DateTime dtmCotacaoAnteriorData = Constantes.DataInvalida;

		    var ativos = new List<CotacaoDataDto>();

		    while (!objRSAtivo.EOF)
		    {

		        var cotacaoDataDto = new CotacaoDataDto
		        {
		            Codigo = (string) objRSAtivo.Field("Codigo")
		        };

                if (pdtmDataInicial != Constantes.DataInvalida)
                {
                    cotacaoDataDto.Data = Convert.ToDateTime(objRSAtivo.Field("DataInicial"));
                }

                ativos.Add(cotacaoDataDto);

                objRSAtivo.MoveNext();
		    }

            objRSAtivo.Fechar();

			foreach (var cotacaoDataDto in ativos) {
			    DateTime dtmDataInicialAux;
			    if (pdtmDataInicial == Constantes.DataInvalida) {
					//se não recebeu uma data inicial válida, não faz sentido buscar a menor data
					//porque estas datas não serão utilizadas pelas funções de cálculo de média, 
					//já que as funções terão que calcular o período inicial. Neste caso, a query
					//retorna a data inválida.
					dtmDataInicialAux = Constantes.DataInvalida;

				} else {
					dtmDataInicialAux = cotacaoDataDto.Data ;
					dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(cotacaoDataDto.Codigo, dtmDataInicialAux, strTabela);
				}

				//For Each objStructMedia In pcolMedia

				foreach (MediaDTO mediaDto in plstMedias)
				{
				    //If objStructMedia.strTipo = "E" Then

				    cEnum.enumRetorno intRetorno = mediaDto.Tipo == "E" ? 
				        MediaMovelExponencialRetroativaUnitCalcular(cotacaoDataDto.Codigo, dtmDataInicialAux, mediaDto.NumPeriodos, strTabela, dtmCotacaoAnteriorData) :
                        MediaMovelAritmeticaPorAtivoCalcular(cotacaoDataDto.Codigo, mediaDto.Dado, pstrPeriodoDuracao, mediaDto.NumPeriodos, dtmDataInicialAux);


				    if (intRetorno != cEnum.enumRetorno.RetornoOK) {
						blnRetorno = false;

						if (!string.IsNullOrEmpty(strLog)) {
							//coloca um ENTER PARA QUEBRAR A LINHA
							strLog = strLog + Environment.NewLine;
						}

						strLog = strLog + " Código: " + cotacaoDataDto.Codigo + " - Tabela: " + strTabela + " - Período: " + mediaDto.NumPeriodos + " - Tipo: " + mediaDto.Tipo;


						if (dtmDataInicialAux != Constantes.DataInvalida) {
							strLog = strLog + " - Data Inicial: " + dtmDataInicialAux;

						}

					}
				}

			}


			if (!string.IsNullOrEmpty(strLog)) {
			    string strArquivoNome = pstrPeriodoDuracao == "DIARIO" ? "Log_MMExp_Diario.txt" : "Log_MMExp_Semanal.txt";

			    var fileService = new FileService();
                fileService.Save(cBuscarConfiguracao.ObtemCaminhoPadrao() + strArquivoNome, strLog);

			}

			if (objRSAtivo.QueryStatus) {
				functionReturnValue = blnRetorno;
			}

			return functionReturnValue;

		}


		private void CotacaoSemanalDadosGerar(string pstrCodigo, DateTime pdtmDataSegundaFeira, DateTime pdtmDataSextaFeira, string pstrOperacaoBD, Conexao pobjConnAux, ref decimal pdecCotacaoAnteriorRet)
		{
			cCommand objCommand = new cCommand(pobjConnAux);
			cRS objRS = new cRS(pobjConnAux);
			cRSList objRsSplit = null;

		    double dblSplitRazaoCotacaoAbertura = 0;

			//indica se tem splits em todo o período calculado

		    //indica se tem splits na semana do cálculo
			bool blnSplitSemana = false;

		    cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(pobjConnAux);

			//busca os splits. 
			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataSegundaFeira, "D", ref objRsSplit, pdtmDataSextaFeira);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			if (blnSplitExistir) {
				//busca a primeira data da semana que tem cotação
				objRS.ExecuteQuery(" select min(Data) as DataInicial " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataSegundaFeira) + " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataSextaFeira));

				DateTime dtmDataPrimeiraCotacaoSemana = Convert.ToDateTime(objRS.Field("DataInicial"));

				objRS.Fechar();

				dblSplitRazaoCotacaoAbertura = 1;
				double dblSplitRazaoCotacaoAnterior = 1;
				//dblSplitRazaoInvertida = 1


				while (!objRsSplit.EOF) {

					if (Convert.ToDateTime(objRsSplit.Field("Data")) != dtmDataPrimeiraCotacaoSemana) {
						//se a data  de pelo menos um dos splits não é a data da primeira cotação da semana, tem que fazer as conversões.
						blnSplitSemana = true;

						//dtmSplitData = CDate(objRSSplit.Field("Data"))
						dblSplitRazaoCotacaoAbertura *= Convert.ToDouble(objRsSplit.Field("Razao"));

					}

					dblSplitRazaoCotacaoAnterior *= Convert.ToDouble(objRsSplit.Field("Razao"));

					//se encontrou passa para o próximo split.
					objRsSplit.MoveNext();

				}

				//se tem split tem que multiplicar a cotação anterior pela razão do split,
				//independentemente se é no primeiro dia da semana ou não, porque a cotação da semana 
				//anterior estava sem o split.
				pdecCotacaoAnteriorRet = Convert.ToDecimal(pdecCotacaoAnteriorRet * (decimal) dblSplitRazaoCotacaoAnterior);

			}


			if (blnSplitSemana) {
			    List<string> lstQueries = ConsultaQueriesGerar(pstrCodigo, pdtmDataSegundaFeira, pdtmDataSextaFeira, "DIARIO", "COTACAO", pdtmDataSextaFeira, true, true);

				string strFrom = String.Empty;


				foreach (string strSQL in lstQueries) {

					if (strFrom != String.Empty) {
						strFrom = strFrom + " UNION " + Environment.NewLine;

					}

					strFrom = strFrom + strSQL;

				}

				//se tem split as data anteriores ao split tem que ser convertidas
				//a primeira parte da query traz as cotações anteriores ao split. 
				//Estas cotações tem que ser multiplicadas pela razão do split
				//A segunda parte traz as cotações já com o split e não precisam ser convertidas
			    string query = " select min(Data) as DataInicial, max(Data) as DataFinal, min(ValorMinimo) as ValorMinimo " + 
			                       ", max(ValorMaximo) as ValorMaximo, sum(Negocios_Total) as Negocios_Total " 
			                       + ", sum(Titulos_Total) as Titulos_Total, sum(Valor_Total) as Valor_Total " 
			                       + ", count(1) as Contador " + 
			                       " from (" + strFrom + ")";

			    if (pobjConnAux.BancoDeDados == cEnum.BancoDeDados.SqlServer)
			    {
                    //se for sql server tem que dar um alias para o subselect
			        query += "as tabela";
			    }

			    objRS.ExecuteQuery(query);

			} else {
				//se não tem split faz a busca normal

				//busca o resumo das cotações para o ativo na semana.
				objRS.ExecuteQuery(" select min(Data) as DataInicial, max(Data) as DataFinal, min(ValorMinimo) as ValorMinimo " +
                    ", max(ValorMaximo) as ValorMaximo, sum(Negocios_Total) as Negocios_Total " +
                    ", sum(Titulos_Total) as Titulos_Total, sum(Valor_Total) as Valor_Total " + 
                    ", count(1) as Contador " + 
                    " from Cotacao " + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + 
                    " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataSegundaFeira) + 
                    " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataSextaFeira));

			}

		    int contador = Convert.ToInt32(objRS.Field("Contador"));
		    if (contador <= 0)
		    {
                objRS.Fechar();
		        return;
		    }


		    decimal decValorAbertura = default(decimal);
		    decimal decValorFechamento = default(decimal);
		    decimal decOscilacao;
		    decimal decDiferenca;
		    var dataInicial  = Convert.ToDateTime(objRS.Field("DataInicial"));
		    var dataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));
		    decimal valorMinimo = Convert.ToDecimal(objRS.Field("ValorMinimo"));
		    decimal valorMaximo = Convert.ToDecimal(objRS.Field("ValorMaximo"));
		    double negociosTotal = Convert.ToDouble(objRS.Field("Negocios_Total"));
		    double titulosTotal = Convert.ToDouble(objRS.Field("Titulos_Total"));
		    double valorTotal = Convert.ToDouble(objRS.Field("Valor_Total"));

		    objRS.Fechar();

		    //busca o valor de abertura da primeira data da semana
		    decimal pdecValorFechamentoRet = -1M;
		    CotacaoConsultar(pstrCodigo, dataInicial, "Cotacao", ref pdecValorFechamentoRet , ref decValorAbertura, pobjConnAux);


		    if (blnSplitSemana) {
		        //se tem split na semana, a cotação de abertura é anterior 
		        //ao split e tem que fazer a multiplicaçao pela razão
		        decValorAbertura = Convert.ToDecimal(decValorAbertura * (decimal) dblSplitRazaoCotacaoAbertura);

		    }

		    //busca o valor de fechamento da última data da semana.
		    decimal pdecValorAberturaRet = -1M;
		    CotacaoConsultar(pstrCodigo, dataFinal, "Cotacao", ref decValorFechamento, ref pdecValorAberturaRet , pobjConnAux);


		    if (pdecCotacaoAnteriorRet > 0) {
		        //calcula a diferença entre o valor de fechamento e o valor de abertura
		        decDiferenca = decValorFechamento - pdecCotacaoAnteriorRet;
		        decOscilacao = Math.Round((decValorFechamento / pdecCotacaoAnteriorRet - 1) * 100, 2);
		    } else {
		        decDiferenca = 0;
		        decOscilacao = 0;
		    }

		    if (pstrOperacaoBD == "INSERT")
		    {
		        //calcula o sequencial do ativo
		        long lngSequencial = _sequencialService.SequencialCalcular(pstrCodigo, "Cotacao_Semanal", objCommand.Conexao);

		        objCommand.Execute(" INSERT INTO Cotacao_Semanal " + "(Codigo, Data, DataFinal, ValorAbertura, ValorMinimo, ValorMaximo, ValorFechamento " + 
		                           ", Diferenca, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial) " + 
		                           " values " + 
		                           "( " + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + funcoesBd.CampoDateFormatar(dataInicial) + ", " +
		                           funcoesBd.CampoDateFormatar(dataFinal) + ", " + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + ", " 
		                           + FuncoesBd.CampoDecimalFormatar(valorMinimo) + ", " + 
		                           FuncoesBd.CampoDecimalFormatar(valorMaximo) + ", " + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + ", " + 
		                           FuncoesBd.CampoDecimalFormatar(decDiferenca) + ", " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + ", " + 
		                           FuncoesBd.CampoFloatFormatar(negociosTotal) + ", " + FuncoesBd.CampoFloatFormatar(titulosTotal) + 
		                           ", " + FuncoesBd.CampoFloatFormatar(valorTotal) + ", " + lngSequencial + ")");
		    }
		    else {
		        objCommand.Execute(" UPDATE Cotacao_Semanal SET " + 
		                           " DataFinal = " + funcoesBd.CampoDateFormatar(dataFinal) + 
		                           ", ValorAbertura = " + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + 
		                           ", ValorMinimo = " + FuncoesBd.CampoDecimalFormatar(valorMinimo) + 
		                           ", ValorMaximo = " + FuncoesBd.CampoDecimalFormatar(valorMaximo) + 
		                           ", ValorFechamento = " + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + 
		                           ", Diferenca = " + FuncoesBd.CampoDecimalFormatar(decDiferenca) + 
		                           ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + 
		                           ", Negocios_Total = " + FuncoesBd.CampoFloatFormatar(negociosTotal) + 
		                           ", Titulos_Total = " + FuncoesBd.CampoFloatFormatar(titulosTotal) + 
		                           ", Valor_Total = " + FuncoesBd.CampoFloatFormatar(valorTotal) + 
		                           " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + 
		                           " and Data = " + funcoesBd.CampoDateFormatar(dataInicial));

		    }

		    //ajuste de variáveis para a próxima iteração
		    pdecCotacaoAnteriorRet = decValorFechamento;
		    //se tem dados no período.


		}

		/// <summary>
		/// Calcula os dados da cotação semanal para um ativo recebido por parâmetro
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">Data inicial para começar o cálculo. Quando este parâmetro for igual 
		/// a "DataInvalida" calcula a partir da última cotação</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool CotacaoSemanalUnitRetroativoCalcular(string pstrCodigo, DateTime pdtmDataBase)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);
			cRS objRS = new cRS(objConnAux);

			cCalculadorData objCalculadorData = new cCalculadorData(objConnAux);

			objCommand.BeginTrans();

			//busca a maior data em que há cotação para o ativo recebido por parâmetro
			DateTime dtmDataMaxima = objCalculadorData.CotacaoDataMaximaConsultar(pstrCodigo, "Cotacao");

			DateTime dtmDataSegundaFeira;

			if (pdtmDataBase != Constantes.DataInvalida) {
				dtmDataSegundaFeira = _cotacaoData.PrimeiraSemanaDataCalcular(pdtmDataBase);
			} else {
				dtmDataSegundaFeira = _cotacaoData.PrimeiraSemanaDataCalcular(pstrCodigo, objConnAux);
			}

		    //a data maior da semana é sempre a data de sexta-feira, que são quatro dias após a segunda
			DateTime dtmDataSextaFeira = dtmDataSegundaFeira.AddDays(4);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			//EXCLUI OS DADOS A PARTIR DA PRIMEIRA SEGUNDA-FEIRA, POIS ESTES DADOS SERÃO RECALCULADOS.
			objCommand.Execute(" DELETE " + " FROM Cotacao_Semanal " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) +
                " and Data >= " + funcoesBd.CampoDateFormatar(dtmDataSegundaFeira));

			//Dim decValorAbertura As Decimal
			//Dim decValorFechamento As Decimal
			//Dim decOscilacao As Decimal
			//Dim decDiferenca As Decimal
			decimal decCotacaoAnterior = default(decimal);


			if (pdtmDataBase != Constantes.DataInvalida) {
			    //consulta a primeira data anterior que tem cotação.

				//dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataBase _
				//, "Cotacao_Semanal", objConnAux)

				DateTime dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataSegundaFeira, "Cotacao_Semanal", objConnAux);

                decimal valorDeAbertura = 0;
				//busca o valor de fechamento nesta data.
				CotacaoConsultar(pstrCodigo, dtmCotacaoAnteriorData, "Cotacao_Semanal", ref decCotacaoAnterior, ref valorDeAbertura , objConnAux);

			} else {
				//se não passou uma data base, então vai calcular tudo desde o começo.
				//Neste caso a cotação anterior é 0.
				decCotacaoAnterior = 0;
			}

		    objRS.ExecuteQuery(" select count(1) as contador " + " from Cotacao_Semanal " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(dtmDataSegundaFeira) + " and Data <= " + funcoesBd.CampoDateFormatar(dtmDataSextaFeira));

            //indica se a operação no banco de dados deve ser um INSERT ou um UPDATE.
            string strOperacaoBd = Convert.ToInt32(objRS.Field("Contador")) > 0 ? "UPDATE" : "INSERT";

			objRS.Fechar();

			while ((dtmDataSegundaFeira <= dtmDataMaxima) && (objCommand.TransStatus)) {
				CotacaoSemanalDadosGerar(pstrCodigo, dtmDataSegundaFeira, dtmDataSextaFeira, strOperacaoBd, objConnAux, ref decCotacaoAnterior);

				//incrementa a data de segunda-feira em uma semana.
				dtmDataSegundaFeira = dtmDataSegundaFeira.AddDays(7);

				//incrementa a data de sexta-feira em uma semana.
				dtmDataSextaFeira = dtmDataSextaFeira.AddDays(7);

				//depois da primeira iteração a operação é sempre INSERT
				strOperacaoBd = "INSERT";

				objRS.Fechar();

			}

			objCommand.CommitTrans();

			bool functionReturnValue = objCommand.TransStatus;

			objConnAux.FecharConexao();
			return functionReturnValue;

		}


		private bool CotacaoSemanalUnitRetroativoCalcularSplit(string pstrCodigo, DateTime pdtmDataBase)
		{

			Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);
			cRSList objRSSplit = null;

		    decimal decCotacaoAnterior = default(decimal);

		    objCommand.BeginTrans();

			DateTime dtmDataSegundaFeira = _cotacaoData.PrimeiraSemanaDataCalcular(pdtmDataBase);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

			//busca os splits
			objCarregadorSplit.SplitConsultar(pstrCodigo, dtmDataSegundaFeira, "A", ref objRSSplit,Constantes.DataInvalida);

			//PARA CADA UM DOS SPLITS

			while (!objRSSplit.EOF) {
				//CALCULA A DATA DA SEGUNDA-FEIRA E DA SEXTA-FEIRA DA SEMANA EM QUE HÁ SPLIT.
				dtmDataSegundaFeira = _cotacaoData.PrimeiraSemanaDataCalcular(Convert.ToDateTime(objRSSplit.Field("Data")));

				//a data maior da semana é sempre a data de sexta-feira, que são quatro dias após a segunda
				DateTime dtmDataSextaFeira = dtmDataSegundaFeira.AddDays(4);

				DateTime dtmCotacaoAnteriorData = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataSegundaFeira, "Cotacao_Semanal", objConnAux);

				//busca o valor de fechamento nesta data.
			    decimal pdecValorAberturaRet = -1M;
			    CotacaoConsultar(pstrCodigo, dtmCotacaoAnteriorData, "Cotacao_Semanal", ref decCotacaoAnterior, ref pdecValorAberturaRet , objConnAux);

				CotacaoSemanalDadosGerar(pstrCodigo, dtmDataSegundaFeira, dtmDataSextaFeira, "UPDATE", objConnAux, ref decCotacaoAnterior);


				objRSSplit.MoveNext();

			}

			objCommand.CommitTrans();

			objConnAux.FecharConexao();

			return objCommand.TransStatus;

		}


	    /// <summary>
	    /// Calcula os dados da cotação semanal para todos os ativos
	    /// </summary>
	    /// <param name="pdtmData">
	    /// Quando este parâmetro é uma data válida calcula os dados a partir desta data, inclusive.
	    /// Quando este parâmetro é uma data inválida calcula os dados desde a primeira data em que existe cotação diária.
	    /// </param>
	    /// '''
	    /// <param name="pstrAtivos">Lista dos ativos para os quais será calculada a cotação</param>
	    /// <param name="pblnCalcularApenasEmSplit">Indica se é para fazer o recálculo das cotações semanais apenas 
	    /// nas semanas que houver split. Neste caso também tem que calcular na semana subsequente, pois esta
	    /// é dependente da primeira para calcular os campos "Diferenca" e "Oscilacao"</param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private bool CotacaoSemanalRetroativoGeralCalcular(DateTime pdtmData, string pstrAtivos = "", bool pblnCalcularApenasEmSplit = false)
		{

			var objRS = new cRS();

			string strLog = "";

		    string strQuery = " select Codigo " + 
                " from Cotacao " + 
                " where not exists " + 
                "(" + " select 1 " + " from Ativos_Desconsiderados " + " where Cotacao.Codigo = Ativos_Desconsiderados.Codigo " + ")";

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			if (pdtmData != Constantes.DataInvalida) {
                strQuery = strQuery + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmData);
			}



			if (pstrAtivos != String.Empty) {

                if (_conexao.BancoDeDados == cEnum.BancoDeDados.SqlServer)
                {
                    //remove sustenido do inicio
                    string ativosAux = pstrAtivos.Remove(0, 1);

                    //remove ultimo sustenido
                    ativosAux = ativosAux.Remove(ativosAux.Length - 1);

                    //faz split pelo sustenido
                    string[] ativosSelecionados = ativosAux.Split('#');

                    strQuery += "AND Codigo IN (" + string.Join(", ", ativosSelecionados.Select(funcoesBd.CampoFormatar)) + ")";

                }

                else
                {
                    string sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
                    strQuery += funcoesBd.IndiceDaSubString(funcoesBd.ConcatenarStrings(new[] { sustenidoFormatado, "Codigo", sustenidoFormatado }), FuncoesBd.CampoStringFormatar(pstrAtivos)) + " > 0";
                }

			}

			strQuery = strQuery + " group by Codigo ";

			objRS.ExecuteQuery(strQuery);


			while (!objRS.EOF) {
			    bool blnRetorno = pblnCalcularApenasEmSplit ?  
                    CotacaoSemanalUnitRetroativoCalcularSplit((string) objRS.Field("Codigo"), pdtmData) : 
                    CotacaoSemanalUnitRetroativoCalcular((string) objRS.Field("Codigo"), pdtmData);


				if (!blnRetorno) {
					if (!string.IsNullOrEmpty(strLog)) {
						strLog = strLog + Environment.NewLine;
					}

					strLog = strLog + " Código = " + objRS.Field("Codigo");

				}

				objRS.MoveNext();

			}

			if (!string.IsNullOrEmpty(strLog)) {
			    var fileService = new FileService();
                fileService.Save(cBuscarConfiguracao.ObtemCaminhoPadrao() + "Log_Cotacao_Semanal.txt",strLog);
			}

			objRS.Fechar();

			return (string.IsNullOrEmpty(strLog));

		}

		/// <summary>
		/// Calcula todos os dados da cotação diária a partir de uma determinada data ou desde o início das cotações.
		/// </summary>
		/// <param name="pblnOscilacaoCalcular">se for TRUE indica que é para calcular a oscilação das cotações</param>
		/// <param name="pblnOscilacaoPercentualCalcular">quando for para calcular a oscilação indica se é para calcular o percentualou apenas a diferença</param>
		/// <param name="pblnIFRCalcular">se for TRUE indica que é para calcular o indice de força relativa</param>
		/// <param name="pblnMMExpCalcular">se for TRUE indica que é para calcular a média móvel exponencial</param>
		/// <param name="pdtmDataBase"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer os cálculos 
		/// apenas nas datas em que houver split. Esta opção é utilizada no recálculo quando
		/// há importação de proventos. 
		/// ATENÇÃO: quando este parâmetro for TRUE, é obrigatório passar o parâmetro
		/// "pdtmDataInicial" com uma data válida
		/// </param>
		/// <param name="pblnCotacaoAnteriorInicializar">Indica se é para inicializar a tabela de cotações anteriores
		/// para não ser necessário buscar o valor da cotação anterior em todos os cálculos de indicadores. Esta busca
		/// é um pouco demorada e esta opção foi criada para otimizar o tempo de atualização das cotações. </param>
		/// <param name="pblnIFRMedioCalcular">Indica se é para calcular o IFR médio das cotações</param>
		/// <param name="pblnVolumeMedioCalcular">Indica se é para calcular o volume médio das cotações</param>
		/// <param name="pstrAtivos">Lista de ativos para os quais deve ser feito o cálculo</param>
		/// <returns>
		/// ''' RetornoOK = todos os cálculos foram realizados com sucesso.
		/// RetornoErro2 = erro ao transportar os dados das cotações diárias para a cotação semanal
		/// RetornoErro3 = erro ao calcular o índice de força relativa
		/// RetornoErro4 = erro ao calcular a média móvel exponencial.
		///</returns>
		/// <remarks></remarks>
		private bool CotacaoDiariaDadosAtualizar(bool pblnOscilacaoCalcular, bool pblnOscilacaoPercentualCalcular, bool pblnIFRCalcular, bool pblnMMExpCalcular, bool pblnVolumeMedioCalcular, bool pblnIFRMedioCalcular, DateTime pdtmDataBase, string pstrAtivos = "", bool pblnCotacaoAnteriorInicializar = true, bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOk = true;
			bool blnIfrok = true;
			bool blnMmExpOk = true;

			if ((pdtmDataBase != Constantes.DataInvalida) && pblnCotacaoAnteriorInicializar) {
				blnOk = CotacaoAnteriorInicializar("DIARIO", pdtmDataBase, pstrAtivos);

				if (!blnOk)
                {
                    MessageBox.Show("Ocorreram erros ao inicializar os dados das cotações anteriores.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return false;
				}

			}

			if (pblnOscilacaoCalcular) {
				blnOk = OscilacaoGeralCalcular(pblnOscilacaoPercentualCalcular, pdtmDataBase, pstrAtivos, pblnConsiderarApenasDataSplit);

			}

			if (blnOk) {

				if (pblnIFRCalcular) {
					IList<int> colPeriodos = new List<int>();

					colPeriodos.Add(2);
                    colPeriodos.Add(14);

				    blnIfrok = _atualizadorDeIfr.IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Diario, pdtmDataBase, pstrAtivos);

				}
			}
            else
            {
                MessageBox.Show("Ocorreram erros ao calcular os dados das cotações diárias.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

            List<MediaDTO> lstMediasSelecionadas = null;

			if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular) {
				lstMediasSelecionadas = new List<MediaDTO>();

			}

			if (pblnMMExpCalcular) {
                foreach (var media in this._mediasDeFechamento)
                {
                    lstMediasSelecionadas.Add(media);
                }
            }

			if (pblnIFRMedioCalcular) {
                lstMediasSelecionadas.Add(new MediaDTO("A", 13, "IFR2"));
			}

			if (pblnVolumeMedioCalcular)
            {
				lstMediasSelecionadas.Add(new MediaDTO("A", 21, "VOLUME"));
			}

			if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular)
			{
			    blnMmExpOk = MediaMovelGeralCalcular("DIARIO", lstMediasSelecionadas, pdtmDataBase, pstrAtivos);
			}

		    return (blnOk && blnIfrok && blnMmExpOk);

		}


		/// <summary>
		/// Calcula todos os dados da cotação semanal a partir de uma determinada data ou desde o início das cotações.
		/// </summary>
		/// <param name="pblnDadosCalcular">se for TRUE indica que é para calcular os dados da cotação utilizando como base a tabela de cotações diárias</param>
		/// <param name="pblnIFRCalcular">se for TRUE indica que é para calcular o indice de força relativa</param>
		/// <param name="pblnMMExpCalcular">se for TRUE indica que é para calcular a média móvel exponencial</param>
		/// <param name="pdtmDataBase"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer o recálculo das cotações semanais apenas 
		/// nas semanas que houver split. Neste caso também tem que calcular na semana subsequente, pois esta
		/// é dependente da primeira para calcular os campos "Diferenca" e "Oscilacao"
		/// </param>
		/// <returns>
		/// ''' RetornoOK = todos os cálculos foram realizados com sucesso.
		/// RetornoErro2 = erro ao transportar os dados das cotações diárias para a cotação semanal
		/// RetornoErro3 = erro ao calcular o índice de força relativa
		/// RetornoErro4 = erro ao calcular a média móvel exponencial.
		///</returns>
		/// <remarks></remarks>
		public bool CotacaoSemanalDadosAtualizar(bool pblnDadosCalcular, bool pblnIFRCalcular, bool pblnMMExpCalcular, bool pblnVolumeMedioCalcular, bool pblnIFRMedioCalcular, DateTime pdtmDataBase, string pstrAtivos = "", bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOk = true;

			if ((pdtmDataBase != Constantes.DataInvalida)) {
				//INICIALIZA AS COTAÇÕES DE DATAS ANTERIORES ANTES DE TODAS AS OPERAÇÕES.
				blnOk = CotacaoAnteriorInicializar("SEMANAL", pdtmDataBase, pstrAtivos);


				if (!blnOk) {
                    MessageBox.Show("Ocorreram erros ao inicializar os dados das cotações anteriores.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return false;

				}

			}


			if (pblnDadosCalcular) {
				//atualiza os dados da tabela de cotação semanal buscando da tabela de cotação diária
				blnOk = CotacaoSemanalRetroativoGeralCalcular(pdtmDataBase, pstrAtivos, pblnConsiderarApenasDataSplit);

			}


			if (blnOk) {

				if (pblnIFRCalcular) {
					IList<int> colPeriodos = new List<int>();

					colPeriodos.Add(2);

                    colPeriodos.Add(14);

					if (!_atualizadorDeIfr.IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Semanal, pdtmDataBase, pstrAtivos)) {
						blnOk = false;
					}
				}

				List<MediaDTO> lstMediasSelecionadas = null;

				if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular) {
					lstMediasSelecionadas = new List<MediaDTO>();
				}

				if (pblnMMExpCalcular) {

                    foreach (var media in this._mediasDeFechamento)
                    {
                        lstMediasSelecionadas.Add(media);
                    }
				}

				if (pblnIFRMedioCalcular) {
					lstMediasSelecionadas.Add(new MediaDTO("A", 13, "IFR2"));
				}

				if (pblnVolumeMedioCalcular) {
					lstMediasSelecionadas.Add(new MediaDTO("A", 21, "VOLUME"));
				}

				if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular)
				{
				    if (!MediaMovelGeralCalcular("SEMANAL", lstMediasSelecionadas, pdtmDataBase, pstrAtivos))
                    {
						blnOk = false;
					}
				}
			}
            else
            {
			    MessageBox.Show("Ocorreram erros ao calcular os dados das cotações semanais.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}

			return blnOk;

		}

		/// <summary>
		/// Função que centraliza a chamada de recálculo de todos os indicadores.
		/// </summary>
		/// <param name="pblnCotacaoDiariaOscilacaoCalcular"></param>
		/// <param name="pblnCotacaoDiariaOscilacaoPercentualCalcular"></param>
		/// <param name="pblnCotacaoDiariaIFRCalcular"></param>
		/// <param name="pblnCotacaoDiariaMMExpCalcular"></param>
		/// <param name="pblnCotacaoDiariaVolumeMedioCalcular"></param>
		/// <param name="pblnCotacaoDiariaIFRMedioCalcular"></param>
		/// <param name="pblnCotacaoSemanalDadosCalcular"></param>
		/// <param name="pblnCotacaoSemanalIFRCalcular"></param>
		/// <param name="pblnCotacaoSemanalMMExpCalcular"></param>
		/// <param name="pblnCotacaoSemanalVolumeMedioCalcular"></param>
		/// <param name="pblnCotacaoSemanalIFRMedioCalcular"></param>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pstrAtivos"></param>
		/// <param name="pblnCotacaoAnteriorInicializar"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica para fazer cálculos apenas 
		/// nas datas em que há splits. É utilizado no cálculo da oscilação na cotação diária
		/// e no cálculo de todos os dados da tabela COTACAO_SEMANAL</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool DadosRecalcular(bool pblnCotacaoDiariaOscilacaoCalcular, bool pblnCotacaoDiariaOscilacaoPercentualCalcular, bool pblnCotacaoDiariaIFRCalcular, bool pblnCotacaoDiariaMMExpCalcular, bool pblnCotacaoDiariaVolumeMedioCalcular, bool pblnCotacaoDiariaIFRMedioCalcular, bool pblnCotacaoSemanalDadosCalcular, bool pblnCotacaoSemanalIFRCalcular, bool pblnCotacaoSemanalMMExpCalcular, bool pblnCotacaoSemanalVolumeMedioCalcular,
		bool pblnCotacaoSemanalIFRMedioCalcular, DateTime pdtmDataInicial, string pstrAtivos = "", bool pblnCotacaoAnteriorInicializar = true, bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOkCotacaoDiaria = true;
			bool blnOkCotacaoSemanal = true;


			if (pblnCotacaoDiariaOscilacaoCalcular || pblnCotacaoDiariaIFRCalcular || pblnCotacaoDiariaMMExpCalcular || pblnCotacaoDiariaVolumeMedioCalcular || pblnCotacaoDiariaIFRMedioCalcular) {
				blnOkCotacaoDiaria = CotacaoDiariaDadosAtualizar(pblnCotacaoDiariaOscilacaoCalcular, pblnCotacaoDiariaOscilacaoPercentualCalcular, pblnCotacaoDiariaIFRCalcular, pblnCotacaoDiariaMMExpCalcular, pblnCotacaoDiariaVolumeMedioCalcular, pblnCotacaoDiariaIFRMedioCalcular, pdtmDataInicial, pstrAtivos, pblnCotacaoAnteriorInicializar, pblnConsiderarApenasDataSplit);

			}


			if (pblnCotacaoSemanalDadosCalcular || pblnCotacaoSemanalIFRCalcular || pblnCotacaoSemanalMMExpCalcular || pblnCotacaoSemanalVolumeMedioCalcular || pblnCotacaoSemanalIFRMedioCalcular) {
				blnOkCotacaoSemanal = CotacaoSemanalDadosAtualizar(pblnCotacaoSemanalDadosCalcular, pblnCotacaoSemanalIFRCalcular, pblnCotacaoSemanalMMExpCalcular, pblnCotacaoSemanalVolumeMedioCalcular, pblnCotacaoSemanalIFRMedioCalcular, pdtmDataInicial, pstrAtivos, pblnConsiderarApenasDataSplit);

			}

			return blnOkCotacaoDiaria && blnOkCotacaoSemanal;

		}

		public bool MediaAtualizar(string pstrCodigo, List<MediaDTO> plstMediasSelecionadas, string pstrPeriodoDuracao)
		{

			string strTabelaMedia;
			string strTabelaCotacao;


			if (pstrPeriodoDuracao == "DIARIO") {
				strTabelaCotacao = "Cotacao";
				strTabelaMedia = "Media_Diaria";


			} else {
				strTabelaCotacao = "Cotacao_Semanal";
				strTabelaMedia = "Media_Semanal";

			}

			bool blnRetorno = true;

			cRS objRS = new cRS(_conexao);

			var lstMediasSelecionadasAux = new List<MediaDTO>();

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			foreach (MediaDTO mediaDto in plstMediasSelecionadas) {
				//BUSCA A MAIOR DATA EM QUE JÁ EXISTE MÉDIA PARA O ATIVO RECEBIDO POR PARÂMETRO
				string strQuery = " select MAX(Data) as Data " + " FROM " + strTabelaMedia + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and NumPeriodos = " + mediaDto.NumPeriodos + " and Tipo = " + FuncoesBd.CampoStringFormatar((mediaDto.Tipo == "E" ? "MME" : "MMA"));

				objRS.ExecuteQuery(strQuery);

				DateTime dtmDataInicial = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

				objRS.Fechar();

				if (dtmDataInicial != Constantes.DataInvalida) {

					//se encontrou uma data, verifica se  tem cotação após esta data. 
					//Só precisa calcular a média, se possuir cotação
					objRS.ExecuteQuery(" select max(Data) as Data " + " from " + strTabelaCotacao + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data > " + funcoesBd.CampoDateFormatar(dtmDataInicial));

                    var dataPosterior = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

                    objRS.Fechar();


                    if (dataPosterior != Constantes.DataInvalida) {
						lstMediasSelecionadasAux.Clear();

						lstMediasSelecionadasAux.Add(mediaDto);

					    blnRetorno = MediaMovelGeralCalcular(pstrPeriodoDuracao, lstMediasSelecionadasAux, dataPosterior, "#" + pstrCodigo + "#");

					}


				} else {
					lstMediasSelecionadasAux.Clear();

					lstMediasSelecionadasAux.Add(mediaDto);

					//se não tem média calculada, tem que calcular para todo o período calculado
				    blnRetorno = MediaMovelGeralCalcular(pstrPeriodoDuracao, lstMediasSelecionadasAux,Constantes.DataInvalida , "#" + pstrCodigo + "#");

				}

			}

			return blnRetorno;

		}


		/// <summary>
		/// Calcula a média aritmética de uma cotação ou de um volume.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pstrDado">
		/// VALOR = CALCULA A MÉDIA DE COTAÇÕES
		/// VOLUME = CALCULA A MÉDIA DE VOLUME
		/// </param>
		/// <param name="pstrPeriodoDuracao"></param>
		/// <param name="pintNumPeriodos"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <returns></returns>
		/// retornook = operação realizada com sucesso
		/// retornoerroinesperado = algum erro de banco de dados ou de programação.
		/// retornoerro2 = não existe o número de cotações suficientes para fazer o cálculo.
		/// <remarks></remarks>
		private cEnum.enumRetorno MediaMovelAritmeticaPorAtivoCalcular(string pstrCodigo, string pstrDado, string pstrPeriodoDuracao, int pintNumPeriodos, DateTime pdtmDataFinal)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRS = new cRS(objConnAux);

			string strQuery;

			DateTime dtmDataInicial = default(DateTime);

		    string strTabelaDados;
			//TABELA ONDE SERÃO BUSCADOS OS DADOS PARA CALCULAR A MÉDIA
			string strTabelaMedia;
			//TABELA ONDE O CÁLCULO DA MÉDIA SERÁ ARMAZENADO

			//INDICA O NÚMERO DE PERÍODOS QUE DEVE SER UTILIZADO NA BUSCA DOS DADOS, QUANDO OS DADOS FOREM O IFR, POR EXEMPLO,
			//QUE É UMA TABELA QUE PODE CONTER DADOS DE VÁRIOS PERÍODOS. 

			//INICIALIZA COM -1, QUE É O VALOR PADRÃO PARA NÃO SER CONSIDERADO
			int intNumPeriodosTabelaDados = -1;

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

		    if (pstrPeriodoDuracao == "DIARIO") {
				if (pstrDado == "VALOR" || pstrDado == "VOLUME") {
					//SE A MÉDIA É CALCULADA SOBRE O VALOR OU SOBRE O VOLUME
					//BUSCA OS DADOS NA TABELA DE COTAÇÃO
					strTabelaDados = "Cotacao";
				} else {
					//SE A MÉDIA É CALCULADA SOBRE O IFR, BUSCA OS DADOS NA TABELA IFR_DIARIO
					strTabelaDados = "IFR_DIARIO";
					//POR ENQUANTO QUANDO A TABELA DE DADOS FOR O IFR, SEMPRE SERÁ O IFR DE 2 PERÍODOS.
					intNumPeriodosTabelaDados = 2;

				}

				strTabelaMedia = "Media_Diaria";

			} else {
				if (pstrDado == "VALOR" || pstrDado == "VOLUME")
                {
					//SE A MÉDIA É CALCULADA SOBRE O VALOR OU SOBRE O VOLUME
					//BUSCA OS DADOS NA TABELA DE COTAÇÃO
					strTabelaDados = "Cotacao_Semanal";

				} else {
					//SE A MÉDIA É CALCULADA SOBRE O IFR, BUSCA OS DADOS NA TABELA IFR_DIARIO
					strTabelaDados = "IFR_SEMANAL";

					//POR ENQUANTO QUANDO A TABELA DE DADOS FOR O IFR, SEMPRE SERÁ O IFR DE 2 PERÍODOS.
					intNumPeriodosTabelaDados = 2;

				}

				strTabelaMedia = "Media_Semanal";

			}

			string strMediaTipo;

			switch (pstrDado)
			{
			    case "VALOR":
			        strMediaTipo = "MMA";
			        break;
			    case "VOLUME":
			        strMediaTipo = "VMA";
			        break;
			    default:
			        strMediaTipo = "IFR2";
			        break;
			}

			objCommand.BeginTrans();

			if (pdtmDataFinal == Constantes.DataInvalida) {
				//se não recebeu uma data final, que é a data da primeira média que deve ser calculada, então tem
				//que calcular desde o inicio das cotações. Para descobrir o primeiro período chamada a função

				if (!this._cotacaoData.NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true, ref dtmDataInicial,ref pdtmDataFinal, strTabelaDados, intNumPeriodosTabelaDados)) {
					//se não foi possível calcular o período, provavelmente porque não tem cotações
					//suficientes faz rollback.
					objCommand.RollBackTrans();

					return cEnum.enumRetorno.RetornoErro2;

				}


			} else {
				//se já recebeu a data final por parâmetro tem apenas que calcular a data inicial.
				//Para isso:
				//Buscar os registros anteriores à data final, considerando na busca a data final, 
				//ordenando por data decrescente crescente. Em cima destes dados, buscar 
				//os top intNumPeriodos registros e dentro destes top registros buscar 
				//o registro com menor data. Esta será a data inicial.

                var subQuery = " select top " + pintNumPeriodos + " Data " + 
                    "FROM " + strTabelaDados + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal);

				if (intNumPeriodosTabelaDados != -1) {

                    subQuery += " and NumPeriodos = " + intNumPeriodosTabelaDados;

				}

				subQuery += " order by Data desc " ;

                strQuery = "select min(Data) as DataInicial " + " from " + funcoesBd.FormataSubSelect(subQuery);

				objRS.ExecuteQuery(strQuery);

				dtmDataInicial = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

				objRS.Fechar();

				//verifica se o período entre as datas calculas é o mesmo recebido por parâmetro

				if (!_cotacaoData.IntervaloNumPeriodosVerificar(pstrCodigo, dtmDataInicial, pdtmDataFinal, pintNumPeriodos, strTabelaDados, intNumPeriodosTabelaDados, objConnAux)) {
					//se o intervalo calculado não contém o número de períodos necessários,
					//então tenta buscar um intervalo desde o início das cotações

					if (!_cotacaoData.NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true,ref dtmDataInicial, ref pdtmDataFinal, strTabelaDados, intNumPeriodosTabelaDados)) {
						//se não foi possível calcular o período, provavelmente porque não tem cotações
						//suficientes faz rollback.
						objCommand.RollBackTrans();

						return cEnum.enumRetorno.RetornoErro2;

					}


				} 

			}

			//Criar dois recordsets.  O primeiro (RS1) deve buscar todas as datas de cotação com 
			//data maior ou igual à data inicial da primeira média, ordenados pela data.  
			//O segundo (RS2) deve buscar todas as datas de cotação com data maior ou igual à data final 
			//da primeira media, ordenados por data. 

			//Os dois recordsets devem andar juntos em um loop com a finalidade de informar 
			//a cada iteração a data inicial e a data final do cálculo da média.  Os recordsets 
			//devem ser percorridos até que o RS2 chegue ao final, pois este tem data maior 
			//e terminará primeiro. A cada iteração do recordset deve ser feita uma query 
			//que calcula a média entre a data inicial e a data final utilizando a função AVG, 
			//nativa do MS Access. Utilizar a função já existente na classe cCotacao: MMAritmeticaCalcular.  
			//Atualizar a média calculada na tabela de média, conforme calculado no item b, 
			//sempre utilizando como data da média a data do RS2.

			//se chegou até este ponto, então significa que conseguiu calcular as datas inicial e final.
			cRS objRsDataInicial = new cRS(objConnAux);
			cRS objRsDataFinal = new cRS(objConnAux);

		    //executa o recordset de datas iniciais

			strQuery = "select Data " + " from " + strTabelaDados + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(dtmDataInicial);


			if (intNumPeriodosTabelaDados != -1) {

				strQuery = strQuery + " and NumPeriodos = " + intNumPeriodosTabelaDados;

			}

			strQuery = strQuery + " order by Data ";

			objRsDataInicial.ExecuteQuery(strQuery);

		    var datasIniciais = new List<DateTime>() ;

            while (!objRsDataInicial.EOF)
            {
                datasIniciais.Add(Convert.ToDateTime(objRsDataInicial.Field("Data")));
                objRsDataInicial.MoveNext();
	        }

            objRsDataInicial.Fechar();

			//executa o recordset de datas iniciais
			strQuery = "select Data " + " from " + strTabelaDados + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataFinal);


			if (intNumPeriodosTabelaDados != -1) {

				strQuery = strQuery + " and NumPeriodos = " + intNumPeriodosTabelaDados;

			}

			strQuery = strQuery + " order by Data ";

			objRsDataFinal.ExecuteQuery(strQuery);

		    var datasFinais = new List<DateTime>();

		    while (!objRsDataFinal.EOF)
		    {
                datasFinais.Add(Convert.ToDateTime(objRsDataFinal.Field("Data")));
		        objRsDataFinal.MoveNext();
		    }
            objRsDataFinal.Fechar();

			//exclui as médias já existentes a partir da primeira data que será calculada,
			//caso exista
			objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + funcoesBd.CampoDateFormatar(pdtmDataFinal) + " AND Tipo = " + FuncoesBd.CampoStringFormatar(strMediaTipo) + " AND NumPeriodos = " + pintNumPeriodos);

			for (int i= 0; i < datasFinais.Count; i++)
			{
			    var dataInicial = datasIniciais[i];
			    var dataFinal = datasFinais[i];
				//calcula a média entre os dois recordsets
                double dblMedia = MediaAritmeticaCalcular(pstrCodigo, dataInicial, dataFinal, datasIniciais[i + 1], pintNumPeriodos, strTabelaDados, pstrDado, objConnAux);

				//atualiza a média no banco de dados
				MediaAtualizar(pstrCodigo, dataFinal, pintNumPeriodos, strTabelaMedia, dblMedia, strMediaTipo, objConnAux);

			}

			objCommand.CommitTrans();

			objConnAux.FecharConexao();
            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		}



		private bool CotacaoAnteriorInicializar(string pstrPeriodo, DateTime pdtmDataInicial, string pstrAtivos = "")
		{

			Conexao objConexaoAux = new Conexao();

			cCommand objCommand = new cCommand(objConexaoAux);

			cRS objRS = new cRS(objConexaoAux);

			cRS objRsSemanal = new cRS(objConexaoAux);

		    //indica o nome da tabela de cotações, de acordo com a duração do período das cotações

		    string strTabela = pstrPeriodo == "DIARIO" ? "Cotacao" : "Cotacao_Semanal";

			string strWhere = String.Empty;

			objCommand.BeginTrans();

			//exclui todos os registros da tabela de cotação antes de inserir os novos
			//a decisão de excluir todos e não apenas os registros da data "pdtmDataInicial"
			//é para a tabela não ficar muito populada, pois o objetivo desta tabela é consultar
			//rapidamente a data anterior
			objCommand.Execute("DELETE " + " FROM Cotacao_Anterior ");

			//busca todos os ativos do período e a menor data para ser utilizada como data base.
			string strQuery = " select Codigo, max(Data) as DataAnterior " + " from " + strTabela;

			if (!string.IsNullOrEmpty(strWhere))
				strWhere = strWhere + " and ";

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			if (pstrPeriodo == "DIARIO") {
				//se passou uma data inicial busca as cotações a partir de uma data.
				strWhere = strWhere + " Data < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);


			} else {
				//se é uma cotação semanal, a data recebida por parâmetro tem que ser menor do que 
				//a data inicial e a data final da semana
				strWhere = strWhere + " Data < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " And ";

			    var sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
			    strWhere +=
			        FuncoesBd.IndiceDaSubString(
			            FuncoesBd.ConcatenarStrings(new[] {sustenidoFormatado, "Codigo", sustenidoFormatado}),
			            FuncoesBd.CampoStringFormatar(pstrAtivos)) + "  > 0 ";

			}

			strQuery = strQuery + " WHERE " + strWhere;

			strQuery = strQuery + " GROUP BY Codigo ";

			objRS.ExecuteQuery(strQuery);

		    var cotacoesAnteriores = new List<CotacaoDataDto>();

		    while (!objRS.EOF)
		    {
                cotacoesAnteriores.Add(new CotacaoDataDto
                {
                    Codigo = Convert.ToString(objRS.Field("Codigo")),
                    Data = Convert.ToDateTime(objRS.Field("DataAnterior")) 
                });
                objRS.MoveNext();
		    }


            objRS.Fechar();

		    foreach (var cotacaoAnteriorDto in cotacoesAnteriores)
		    {
			    DateTime dtmData;
			    if (pstrPeriodo == "DIARIO") {
					dtmData = pdtmDataInicial;

				} else {
					//busca da primeira data da semana, pois pdtmDataInicial recebido por parâmetro pode 
					//uma data de meio de semana
				    objRsSemanal.ExecuteQuery(" SELECT Data " + "FROM Cotacao_Semanal " + "WHERE Codigo = " +
				                              FuncoesBd.CampoStringFormatar(cotacaoAnteriorDto.Codigo) + " AND Data <= " +
				                              FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " AND DataFinal >= " +
				                              FuncoesBd.CampoDateFormatar(pdtmDataInicial));

					dtmData = Convert.ToDateTime(objRsSemanal.Field("Data",Constantes.DataInvalida));

					objRsSemanal.Fechar();

				}

				//Para cada ativo do loop, faz a inserção na tabela Cotacao_Anterior
		        objCommand.Execute("INSERT INTO Cotacao_Anterior " + "(Codigo, Data, Periodo, Data_Anterior) " +
		                           "VALUES " +
		                           "(" + FuncoesBd.CampoStringFormatar(cotacaoAnteriorDto.Codigo) + ", " +
		                           FuncoesBd.CampoDateFormatar(dtmData) + ", " + FuncoesBd.CampoStringFormatar(pstrPeriodo) +
		                           ", " + FuncoesBd.CampoDateFormatar(cotacaoAnteriorDto.Data) + ")");

		    }

			objCommand.CommitTrans();

			objConexaoAux.FecharConexao();

		    return objCommand.TransStatus;

		}

		/// <summary>
		/// Busca os registros de proventos (dividendos e juros) na tabela de Proventos e atualiza na tabela Split
		/// </summary>
		/// <param name="pdtmDataFinal">última data para a busca dos proventos</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool ProventoAtualizar(DateTime pdtmDataFinal)
		{
			cCommand objCommand = new cCommand(_conexao);

			cRS objRS = new cRS(_conexao);

			cRS objRSAtivo = new cRS(_conexao);

		    string strWhere = String.Empty;

		    //Dim dtmData_Ultimo_Provento As Date

			string strCodigoAtual = String.Empty;
			DateTime dtmDataInicioRecalculo = Constantes.DataInvalida;

			//código no sistema de cotações equivalente ao conjunto Nome_Pregao + Tipo_Acao
			//Dim strCodigoProvento As String

			string strNomePregaoAtual = String.Empty;
			string strTipoAcaoAtual = String.Empty;

			string strNomePregaoNaoEncontrado = String.Empty;
			string strTipoAcaoNaoEncontrado = String.Empty;

			string strUltimoNomePregao = String.Empty;
			string strUltimoTipoAcao = String.Empty;

		    //indica se o provento foi importado, ou seja, foi encontrado o ativo na tabela de ativos 
			//para importá-lo.
			bool blnImportado = false;

		    //usado para o cálculo da primeira dat ex-provento

		    //contém a data da primeira cotação para o papel

		    string strTipoProvento = null;

			var objCalculadorData = new cCalculadorData(_conexao);

			//prepara os dados da tabela temporária
			objCommand.BeginTrans();

			//Exclui os registros em branco do final da planilha do Excel que também são importados
			objCommand.Execute("DELETE " + Environment.NewLine + " FROM Proventos_Temp " + Environment.NewLine + " WHERE Nome_Pregao Is Null");

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			//Atualiza campo Data_Aprovacao para uma formato de data. O campo é do tipo string na tabela
			//Temporária e na hora de importar fica com um formato numérico contando os dias que se passaram
			//desde 30/12/1899. Não faz isso se o campo já possuir 10 caracteres, pois o formato dd/MM/yyyy ocupa 
			//10 caracteres e se já for encontrado o separados das datas ("/") também.

			objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine + 
                " DATA_APROVACAO = " + funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_APROVACAO) " + Environment.NewLine +
                " WHERE DATA_APROVACAO <> " + FuncoesBd.CampoStringFormatar("ESTATUTÁRIO") + Environment.NewLine +
                " And len(DATA_APROVACAO) <> 10 " + Environment.NewLine + 
                " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "DATA_APROVACAO") + " = 0 ");


			objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine + 
                "DATA_ULTIMO_PRECO_COM = " + funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_ULTIMO_PRECO_COM) " + Environment.NewLine + 
                "WHERE DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar("PREÇO TEÓRICO") + Environment.NewLine + 
                " And len(DATA_ULTIMO_PRECO_COM) <> 10 " + Environment.NewLine +
                " And DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar(" ") + Environment.NewLine +
		        " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "DATA_ULTIMO_PRECO_COM") + " = 0 ");

		    objCommand.Execute(" UPDATE PROVENTOS_TEMP SET " + " ULTIMO_DIA_COM = " +
		                       funcoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(ULTIMO_DIA_COM) " +
		                       Environment.NewLine +
		                       " WHERE(Len(ULTIMO_DIA_COM) <> 10) " +
		                       " And " + funcoesBd.IndiceDaSubString(funcoesBd.CampoFormatar("/"), "ULTIMO_DIA_COM") + " = 0 ");

			objCommand.CommitTrans();


			if (!objCommand.TransStatus) {
				return false;

			}

			//consulta a data da primeira cotação e do último provento na tabela Resumo
			objRS.ExecuteQuery("SELECT Data_Primeira_Cotacao " + Environment.NewLine + "FROM Resumo ");

			DateTime dtmData_Primeira_Cotacao = Convert.ToDateTime(objRS.Field("Data_Primeira_Cotacao", Constantes.DataInvalida));
			//dtmData_Ultimo_Provento = CDate(objRS.Field("Data_Ultimo_Provento", DataInvalida))

			objRS.Fechar();

			//busca todos os proventos da tabela de proventos após a data do último provento, 
			//caso a mesma esteja preenchida.

			string strQuery = "SELECT Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + ", SUM(Valor_Provento) AS Valor_Provento " + "FROM Proventos_Temp PT " + Environment.NewLine + " WHERE ";


			strWhere = "CDBL(Ultimo_Preco_com) > 0 " + Environment.NewLine;


			if (dtmData_Primeira_Cotacao != Constantes.DataInvalida) {
				//busca os proventos a partir da data da primeira cotação.
				strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) >= " + funcoesBd.CampoDateFormatar(dtmData_Primeira_Cotacao) + Environment.NewLine;

			}


			if (pdtmDataFinal != Constantes.DataInvalida) {
				//a data final é a maior data para qual deve ser buscada os proventos.
				//geralmente é utilizada somente quando é necessário atualizar proventos 
				//anteriores aos proventos já cadastrados.
				strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) <= " + funcoesBd.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

			}

			//WHERE que verifica se o registro já foi processado.
			strWhere = strWhere + " AND NOT EXISTS ( " + Environment.NewLine + " SELECT 1 " + Environment.NewLine + " FROM Proventos P " + Environment.NewLine + " WHERE PT.Nome_Pregao = P.Nome_Pregao " + Environment.NewLine + " AND PT.Tipo_Acao = P.Tipo_Acao " + Environment.NewLine + " AND PT.Tipo_Provento = P.Tipo_Provento " + Environment.NewLine + " AND PT.Ultimo_Dia_Com = P.Ultimo_Dia_Com " + Environment.NewLine + ")";

			strQuery = strQuery + strWhere + " GROUP BY Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + " ORDER BY Nome_Pregao, Tipo_Acao, CDATE(Ultimo_Dia_Com)";

			objRS.ExecuteQuery(strQuery);

		    string conteudoDoArquivoDeLog = "";

			//No loop a seguir podem existir vários registros seguidos para a mesma ação
			//(Nome_Pregao + Tipo_Acao). Por isso a cada iteração tem que verificar se o ativo mudou
			while ((!objRS.EOF) && (objCommand.TransStatus))
			{

			    var strNomePregao = (string) objRS.Field("Nome_Pregao");
			    var strTipoAcao = (string) objRS.Field("Tipo_Acao");
				if (strNomePregaoNaoEncontrado != strNomePregao || strTipoAcaoNaoEncontrado != strTipoAcao) {
					//se a ação não está marcada como não encontrada na tabela de ativos prossegue.
					//Caso contrário o programa vai para o próximo registro pois não entrará neste IF.


				    if (strNomePregaoAtual != strNomePregao || strTipoAcaoAtual != strTipoAcao) {
						//inicia transação para trabalhar com os dados deste ativo.
						objCommand.BeginTrans();

						//inicializa o campo de data de início do recálculo
						dtmDataInicioRecalculo = Constantes.DataInvalida;

						//se é um ativo novo comparado com o ativo anterior
						objRSAtivo.ExecuteQuery("SELECT Codigo " + Environment.NewLine + " FROM Ativo " + Environment.NewLine + " WHERE Descricao = " + FuncoesBd.CampoStringFormatar(objRS.Field("Nome_Pregao") + " " + objRS.Field("Tipo_Acao")) + Environment.NewLine + " OR Descricao = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Nome_Pregao") + (string) objRS.Field("Tipo_Acao")));


						if (objRSAtivo.DadosExistir) {
							//ativo encontrado
							strCodigoAtual = (string) objRSAtivo.Field("Codigo");

							strNomePregaoAtual = (string) objRS.Field("Nome_Pregao");
							strTipoAcaoAtual = (string) objRS.Field("Tipo_Acao");



						} else {
							//seta o código para vazio para neste caso não fazer insert na tabela de split
							strCodigoAtual = String.Empty;

							strNomePregaoNaoEncontrado = (string) objRS.Field("Nome_Pregao");
							strTipoAcaoNaoEncontrado = (string) objRS.Field("Tipo_Acao");

							//neste caso com certeza não vai importar os proventos.
							blnImportado = false;

							//lança log informando que não encontrou o ativo.
                            conteudoDoArquivoDeLog += "Ativo não encontrado - Nome: " + strNomePregaoNaoEncontrado + " - Tipo de ação: " + strTipoAcaoNaoEncontrado;

						}

						objRSAtivo.Fechar();

						strUltimoNomePregao = (string) objRS.Field("Nome_Pregao");
						strUltimoTipoAcao = (string) objRS.Field("Tipo_Acao");

					}
					//fim do if que testa se é um código novo de ativo.


					if (strCodigoAtual != String.Empty) {

                        //busca a data da primeira cotação para o papel
                        DateTime dtmDataPrimeiraCotacao = AtivoPrimeiraCotacaoDataConsultar(strCodigoAtual);

						if (Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao) {

							if (dtmDataInicioRecalculo == Constantes.DataInvalida) {
								dtmDataInicioRecalculo = Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com"));

							}

							//calcula a data ex-provento, que é a próxima data em que houver cotação para o ativo
							//após o "último dia com"
							DateTime dtmDataExProvento = AtivoCotacaoPosteriorDataConsultar(strCodigoAtual, Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));


							if (dtmDataExProvento == Constantes.DataInvalida) {
                                conteudoDoArquivoDeLog += "Não foram encontradas cotações após o dia ex-provento." + "Verificar a data cadastrada na tabela Split - Código: " + strCodigoAtual + " - Data ex-provento: " + Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")).ToString("dd/MM/yyyy");

								//se não encontrou a data da próxima cotação é porque a atualização de proventos 
								//veio no dia da última cotação. Neste caso busca o próximo dia útil como sendo data-ex.
								//Tem que cuida para caso haja algum feriado ainda não cadastrado na tabela "Feriado"
								//que pode fazer com que a data do próximo dia útil fique calculada errada e por 
								//consequencia também a data do último dia com provento.
								//Para alertar o usuário lança log informando que não encontrou cotação após o
								//dia ex-dividendo.
								dtmDataExProvento = objCalculadorData.DiaUtilSeguinteCalcular(Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));

							}

						    var strTipoDeProventoPorExtenso = (string) objRS.Field("Tipo_Provento");

                            if (strTipoDeProventoPorExtenso == "DIVIDENDO")
                            {
								strTipoProvento = "DIV";
                            }
                            else if ((strTipoDeProventoPorExtenso == "JRS CAP PRÓPRIO") || (strTipoDeProventoPorExtenso == "JRS CAP PROPRIO"))
                            {
								strTipoProvento = "JCP";
                            }
                            else if (strTipoDeProventoPorExtenso == "RENDIMENTO")
                            {
								strTipoProvento = "REND";
                            }
                            else if (strTipoDeProventoPorExtenso == "REST CAP DIN")
                            {
								strTipoProvento = "RCDIN";
							} else {
								strTipoProvento = String.Empty;

							}

							//Verifica se o provento já está cadastrado
							objRSAtivo.ExecuteQuery("SELECT COUNT(1) AS Contador " + Environment.NewLine + " FROM Split " + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigoAtual) + Environment.NewLine + " AND Data = " + funcoesBd.CampoDateFormatar(dtmDataExProvento) + Environment.NewLine + " AND Tipo = " + FuncoesBd.CampoStringFormatar(strTipoProvento));


							if (Convert.ToInt32(objRSAtivo.Field("Contador")) == 0) {
								//Se o provento ainda não está cadastrado, cadastra-o.
								strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtual) + ", " + funcoesBd.CampoDateFormatar(dtmDataExProvento) + ", " + FuncoesBd.CampoStringFormatar(strTipoProvento);

								double dblQuantidadeAnterior = Convert.ToDouble(objRS.Field("Ultimo_Preco_Com")) - Convert.ToDouble(objRS.Field("Valor_Provento"));

								strQuery = strQuery + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))) + ")";

								objCommand.Execute(strQuery);

								//marca que o ativo será importado para depois carregar da tabela
								//Proventos_Temp para a tabela Proventos
								blnImportado = true;


							} else {
								//lança log informando que já existe provento cadastrado
                                conteudoDoArquivoDeLog += "Já existe provento cadastrado - Codigo: " + strCodigoAtual + " - Data: " + dtmDataExProvento.ToString("dd/MM/yyyy") + " - Tipo de Provento: " + objRS.Field("Tipo_Provento");

								blnImportado = false;

							}


						} else {
							//Se a data do provento é anterior à data da primeira cotação do ativo não importa
							blnImportado = false;

						}
						//If CDate(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao Then

						//copia os dados do provento da tabela temporária para a tabela definitiva.
						//copia somente para os registros que estão sendo processados no momento utilizando
						//como campos chaves: Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com, Ultimo_Preco_Com
						objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco" + ", " + (blnImportado ? "1" : "0") + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine + " AND Tipo_Provento = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Tipo_Provento")) + Environment.NewLine + " AND Ultimo_Dia_Com = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Ultimo_Dia_Com")) + " AND Ultimo_Preco_Com = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))));

					}
					//If strCodigoAtual <> vbNullString Then

				}
				//fim do if que testa se é um código não encontrado

				objRS.MoveNext();

			    bool blnExecutar;
			    if (objRS.EOF) {
					//assinala que tem que executar para que o último registro não fique sem execução
					blnExecutar = true;

				} else {
					if (strUltimoNomePregao != (string) objRS.Field("Nome_Pregao") || strUltimoTipoAcao != (string) objRS.Field("Tipo_Acao")) {
						//se vai trocar de ativo na próxima iteração tem que executar
						blnExecutar = true;
					} else {
						blnExecutar = false;
					}

				}


				if (blnExecutar) {

					if (strUltimoNomePregao == strNomePregaoNaoEncontrado && strUltimoTipoAcao == strTipoAcaoNaoEncontrado) {
						//se o último papel é um papel não encontrado copia todos os registros deste papel
						//da tabela Proventos_Temp para a tabela Proventos com o campo Importado = 0.
						objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco, 0" + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine);

					}

					//quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
					//em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
					objCommand.CommitTrans();

					//Este teste do IF indica que o ativo anterior da tabela Proventos_Temp
					//foi encontrado na tabela Ativo e importado, portanto deve ser feito recálculo.

					if (blnImportado) {
                        conteudoDoArquivoDeLog += "Atualizando indicadores - Código: " + strCodigoAtual + " - Data Início: " + dtmDataInicioRecalculo.ToString("dd/MM/yyyy") + "- Tipo de Provento: " + strTipoProvento;

						//chamar função para fazer recálculo dos dados do ativo anterior....
						//Não é necessário recalcular o volume médio nas periodicidades diário e semanal
						//porque os proventos não alteram o volume de ações. Apenas os desdobramentos
						//que alteram
						DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
						true, dtmDataInicioRecalculo, "#" + strCodigoAtual + "#",true , true);

					}

				}

			}

			objRS.Fechar();

            string caminhoDoArquivo = cBuscarConfiguracao.ObtemCaminhoPadrao() + "Log_Proventos\\Log_Proventos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            IFileService fileService = new FileService();
            fileService.Save(caminhoDoArquivo, conteudoDoArquivoDeLog);

			Process.Start("c:\\windows\\notepad.exe", caminhoDoArquivo);
            return objCommand.TransStatus;

		}

		/// <summary>
		/// Busca a data de uma cotação a partir de um número sequencial.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="plngSequencial"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime AtivoSequencialDataBuscar(string pstrCodigo, long plngSequencial, string pstrTabelaCotacao)
		{
		    cRS objRS = new cRS(_conexao);

			objRS.ExecuteQuery("SELECT Data " + Environment.NewLine + " FROM " + pstrTabelaCotacao + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " AND Sequencial = " + plngSequencial);

			DateTime data = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();

		    return data;

		}

		/// <summary>
		/// Cadastra o provento na tabela "Proventos", cadastra o provento na tabela "Split", atualiza as cotações a
		/// partir da data Ex do provento.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pintProventoTipo">Tipo de provento que foi pago: dividendo, juros, etc</param>
		/// <param name="pdtmDataAprovacao">Data em que foi aprovando o pagamento do provento</param>
		/// <param name="pdtmDataEx">Data ex-provento</param>
		/// <param name="pdecValorPorAcao">valor do provento pago por ação</param>
		/// <returns></returns>
		/// RetornoOK = operação executada com sucesso.
		/// RetonoErroInesperado = erro de banco de dados ou de programação
		/// RetornoErro2 = não existe cotação na data Ex do provento
		/// <remarks></remarks>
		public cEnum.enumRetorno ProventoCadastrar(string pstrCodigo, cEnum.enumProventoTipo pintProventoTipo, DateTime pdtmDataAprovacao, DateTime pdtmDataEx, decimal pdecValorPorAcao)
		{
			cEnum.enumRetorno functionReturnValue;

		    decimal decUltimoPrecoCom = default(decimal);
		    const string strTabelaCotacao = "COTACAO";
			string strProventoTipoAbreviatura;
			string strProventoTipoDescricao;

		    cCommand objCommand = new cCommand(_conexao);

		    cCalculadorData objCalculadorData = new cCalculadorData(_conexao);


			objCommand.BeginTrans();

			//Verifica se existe cotação na data Ex.

			if (!this._cotacaoData.CotacaoDataExistir(pdtmDataEx, strTabelaCotacao)) {
				objCommand.RollBackTrans();

				return cEnum.enumRetorno.RetornoErro2;

			}

			//busca a última data em que houve cotação antes da data ex-provento
			DateTime dtmDataUltimoPrecoCom = this._cotacaoData.AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataEx, strTabelaCotacao);

			//busca a cotação na última data com
		    decimal pdecValorAberturaRet = -1M;
		    CotacaoConsultar(pstrCodigo, dtmDataUltimoPrecoCom, strTabelaCotacao, ref decUltimoPrecoCom,ref pdecValorAberturaRet);

			//busca o último dia útil antes da data ex-provento
			DateTime dtmUltimoDiaCom = objCalculadorData.DiaUtilAnteriorCalcular(pdtmDataEx);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			switch (pintProventoTipo) {

				case cEnum.enumProventoTipo.Dividendo:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("DIV");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("DIVIDENDO");

					break;
				case cEnum.enumProventoTipo.JurosCapitalProprio:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("JCP");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("JRS CAP PRÓPRIO");

					break;
				case cEnum.enumProventoTipo.Rendimento:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("REND");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("RENDIMENTO");

					break;
				case cEnum.enumProventoTipo.RestCapDin:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("RCDIN");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("REST CAP DIN");

					break;
				default:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar(String.Empty);
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar(String.Empty);

					break;
			}

			double dblQuantidadeAnterior = Convert.ToDouble(decUltimoPrecoCom) - Convert.ToDouble(pdecValorPorAcao);

			string strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + funcoesBd.CampoDateFormatar(pdtmDataEx) + ", " + strProventoTipoAbreviatura + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ")";

			objCommand.Execute(strQuery);

			cRS objRS = new cRS(_conexao);

			//consulta a descrição do ativo
			strQuery = "SELECT Descricao " + Environment.NewLine + "FROM Ativo " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo);

			objRS.ExecuteQuery(strQuery);

			string strNomePregao = Convert.ToString(objRS.Field("Descricao")).Substring(0,Convert.ToString(objRS.Field("Descricao")).Length- 3).Trim();

			string strTipoAcao = Convert.ToString(objRS.Field("Descricao")).Substring(Convert.ToString(objRS.Field("Descricao")).Length -3).Trim();

			objRS.Fechar();

		    strQuery = "INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strNomePregao) + ", " + FuncoesBd.CampoStringFormatar(strTipoAcao) + ", " + FuncoesBd.CampoStringFormatar(pdtmDataAprovacao.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(pdecValorPorAcao)) + ", 1, " + strProventoTipoDescricao + ", " + FuncoesBd.CampoStringFormatar(dtmUltimoDiaCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoStringFormatar(dtmDataUltimoPrecoCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ", 1" + ", " + FuncoesBd.CampoStringFormatar((pdecValorPorAcao / decUltimoPrecoCom * 100M).ToString("0##.##")) + ", 1)";

			objCommand.Execute(strQuery);


			//quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
			//em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
			objCommand.CommitTrans();



			if (objCommand.TransStatus) {
				functionReturnValue = cEnum.enumRetorno.RetornoOK;

				//chamar função para fazer recálculo dos dados do ativo anterior....
				//Não é necessário recalcular o volume médio nas periodicidades diário e semanal
				//porque os proventos não alteram o volume de ações. Apenas os desdobramentos
				//que alteram
				DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
				true, dtmUltimoDiaCom, "#" + pstrCodigo + "#",true , true);


			} else {
				functionReturnValue = cEnum.enumRetorno.RetornoErroInesperado;

			}
			return functionReturnValue;

		}

	    /// <summary>
	    /// Realiza as consultas necessárias dos dados que serão mostrados no gráfico e retorna um objeto cRSList com estes dados
	    /// </summary>
	    /// <param name="pstrCodigoAtivo"></param>
	    /// <param name="pdtmDataInicial"></param>
	    /// <param name="pdtmDataFinal"></param>
	    /// <param name="pstrPeriodicidade"></param>
	    /// <param name="pstrOrigemDado">
	    /// Indica de onde serão buscados os dados.
	    /// Valores possíveis: "COTACAO", "MEDIA"
	    /// </param>
	    /// <param name="pstrMediaTipo">Tipo da média que deve ser consultada. Necessário informar apenas quando a função é utilizada</param>
	    /// <param name="pintNumPeriodos">Número de períodos da média que deve ser consultada. Necessário informar apenas quando a função é utilizada</param>
	    /// <param name="pstrDado">VALOR OU VOLUME. Necessário informar apenas quando a função é utilizada para calcular médias</param>
	    /// <param name="pblnCotacaoBuscar">Indica se é para buscar dados das cotações</param>
	    /// <param name="pblnVolumeBuscar">Indica se é para buscar dados do volume</param>
	    /// <param name="pdtmDataMaximaSplit">Data máxima (pdtmDataFinal) que deve ser utilizada na busca do split. Caso seja necessários consultar
	    /// todos os splits até a data mais recente, parâmetro não deve ser passado. </param>
	    /// <param name="pstrOrderBy"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private List<string> ConsultaQueriesGerar(string pstrCodigoAtivo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrPeriodicidade, string pstrOrigemDado, 
            DateTime pdtmDataMaximaSplit, bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrMediaTipo = "", int pintNumPeriodos = -1,
		string pstrDado = "", string pstrOrderBy = "")
		{

			cRSList objRSListSplit = null;
			var lstQueries = new List<string>();

			string strSql;

			string strTabela = string.Empty;
			string strTabelaCotacao = string.Empty;
			string strTabelaMedia = string.Empty;
		    string strTabelaIfr = string.Empty;

			cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIfr);

			if (pstrOrigemDado == "COTACAO") {
				strTabela = strTabelaCotacao;
			} else if (pstrOrigemDado == "MEDIA") {
				strTabela = strTabelaMedia;
			}

			double dblOperador = 1;
			double dblOperadorInvertido = 1;

			//inicializa os operadores com 1 para que a multiplicação até o primeiro split não altere os valores
			double dblOperadorAux = 1;
			double dblOperadorInvertidoAux = 1;

			//Se o dado que será consultado for o volume, considera apenas os splits de desdobramento. 
			//Caso contrário considera todos os tipos.
			string strSplitTipo = (pstrDado == "VOLUME" ? "DESD" : string.Empty);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this._conexao);

			//consulta os splits
			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigoAtivo, pdtmDataInicial, "D", ref objRSListSplit, pdtmDataMaximaSplit, strSplitTipo);


			if (blnSplitExistir) {
				bool blnRealizarConsultaUnitaria = false;
				bool blnAtribuirOperador = false;

				DateTime dtmDataMinima = Convert.ToDateTime(objRSListSplit.Field("Data"));

				//Inicializa a data máxima com a data final porque na primeira iteração, se já for necessário calcular valores (dtmDataMinima < pdtmDataFinal)
				//os valores têm que ser calculados da data do primeiro split até a data final do gráfico.
				DateTime dtmDataMaxima = pdtmDataFinal;

				//Fica em loop buscando os registros até que a data máxima fique menor do que a data mínima.
				//Isto vai acontecer logo após a iteração referente as datas entre a data inicial (pdtmDataInicial) 
				//e o Split com menor data, que no caso será o último split do RS, pois o mesmo está em ordem 
				//decrescente de data.

				//Tem que colocar um OR pelo EOF do RS porque quando a área de desenho vai mostrar os dados que não são as últimas cotações
				//pode acontecer de inicialmente a data máxima ser menor do que a data mínima. Isto vai acontecer se a data máximo for menor 
				//do que a data do split mais recente.

				while ((dtmDataMaxima >= dtmDataMinima) || (!objRSListSplit.EOF)) {

					if (!objRSListSplit.EOF) {

						if (pstrPeriodicidade == "SEMANAL") {
							//quando a cotação é semanal, a data inicial tem que ser a data menor ou igual a data do split,
							//pois se ocorrer um split em uma data que não for a primeira data da semana, toda as cotações
							//desta semana já foram convertidas para a razão do split
							dtmDataMinima = _cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, dtmDataMinima);

						}

					}


					if (pstrPeriodicidade == "DIARIO") {
						blnRealizarConsultaUnitaria = dtmDataMinima <= pdtmDataFinal && dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));
					} else if (pstrPeriodicidade == "SEMANAL")
					{
					    DateTime dataDoProximoSplit = Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));
                        DateTime primeiraDataDaSemana = dataDoProximoSplit == Constantes.DataInvalida ? Constantes.DataInvalida :_cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo,dataDoProximoSplit);
						blnRealizarConsultaUnitaria = (dtmDataMinima <= pdtmDataFinal) && (dtmDataMinima != primeiraDataDaSemana);
					}


					if (blnRealizarConsultaUnitaria) {
						//tem que gerar a tabela somente se o split estiver dentro da área de dados,
						//senão tem que ficar calculando apenas o operador para fazer as multiplicações depois.

						//Regra do parâmetro "pdtmDataFinal": se a data final de busca (pdtmDataFinal) for menor que a data maxima da iteração
						//tem que utilizar a data final, senão utiliza a data máxima
					    strSql = _geradorQuery.ConsultaUnitariaGerar(pstrCodigoAtivo, dtmDataMinima, dtmDataMaxima, strTabela, dblOperador, dblOperadorInvertido, "TODOS", pstrMediaTipo, pintNumPeriodos, pstrDado,
						pblnCotacaoBuscar, pblnVolumeBuscar, pstrOrderBy);

						lstQueries.Add(strSql);

						//para a próxima iteração a data máxima é a data anterior ao split que está acabando
						dtmDataMaxima = dtmDataMinima.AddDays(-1);

					}
					//If dtmDataMinima <= pdtmDataFinal Then


					if (!objRSListSplit.EOF) {
						dblOperadorAux = dblOperadorAux * Convert.ToDouble(objRSListSplit.Field("Razao"));

						//A razão invertida só é utilizada no cálculo do volume. 
						//O volume só deve utilizar os splits de desdobramento (tipo = 'DESD').
						//Por isso, só multiplica o operador invertido pela razão invertida neste caso.
						if ((string) objRSListSplit.Field("Tipo") == "DESD") {
							dblOperadorInvertidoAux = dblOperadorInvertidoAux * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));
						}

						//Ajusta o operador quando a próxima data for diferente.
						//Tem que colocar este IF antes do MOVENEXT, pois caso contrário vai alterar o operador antes de chamar
						//a função "ConsultaUnitariaGerar", fazendo com que os valores sejam multiplicados em um intervalo 
						//imediatamente anterior ao que deve ser multiplicado


						if (pstrPeriodicidade == "DIARIO") {
							blnAtribuirOperador = dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


						} else if (pstrPeriodicidade == "SEMANAL") {
							blnAtribuirOperador = dtmDataMinima != _cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));

						}


						if (blnAtribuirOperador) {
							dblOperador = dblOperadorAux;
							dblOperadorInvertido = dblOperadorInvertidoAux;

						}

						objRSListSplit.MoveNext();

						dtmDataMinima = objRSListSplit.EOF ? pdtmDataInicial : Convert.ToDateTime(objRSListSplit.Field("Data"));

					}

				}
				//If Not objRSSplit.EOF Then


			} else {
				//Não tem split: consulta única.
			    strSql = _geradorQuery.ConsultaUnitariaGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, strTabela, dblOperador, dblOperadorInvertido, "TODOS", pstrMediaTipo, pintNumPeriodos, pstrDado,
				pblnCotacaoBuscar, pblnVolumeBuscar, pstrOrderBy);

				lstQueries.Add(strSql);


			}

			return lstQueries;

		}

		public cRSList ConsultaExecutar(string pstrCodigoAtivo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrPeriodicidade, string pstrOrigemDado, DateTime pdtmDataMaximaSplit, bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrMediaTipo = "", int pintNumPeriodos = -1,
		string pstrDado = "")
		{

			cRSList objRSList = new cRSList(_conexao);

			objRSList.Queries = ConsultaQueriesGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, pstrPeriodicidade, pstrOrigemDado, pdtmDataMaximaSplit, pblnCotacaoBuscar, pblnVolumeBuscar, pstrMediaTipo, pintNumPeriodos,
			pstrDado, "DATA DESC");

			//Executa as queries
			objRSList.ExecuteQuery();

			//Retorna o List.
			return objRSList;

		}

		public void RecalcularIndicadores()
		{
			cRSList objRS = new cRSList(_conexao);

		    string strSQL = " SELECT CODIGO, Min(DATA)  As DATA " + "FROM " + "(" + " SELECT CODIGO, DATA " + " FROM Split " + " WHERE TIPO Not In('DESD', 'CISAO')" + " And codigo Not In ('BBAS3', 'BBDC4', 'BRAP4', 'CIEL3', 'COCE5', 'CSNA3', 'ELET3', 'FFTL4', 'ITSA4', 'ITUB3', 'ITUB4', 'POMO4', 'TNLP4', 'USIM5', 'VALE3', 'VALE5', 'BVMF3' " + ", 'PINE4', 'TMAR5', 'BEES3', 'PSSA3', 'ITSA3', 'POSI3', 'USIM3', 'PETR3', 'PETR4', 'ETER3', 'TLPP3', 'TLPP4', 'NATU3', 'GETI3', 'GETI4', 'AMAR3', 'VIVO4') " + " And data <= #2011-05-02# " + " GROUP BY CODIGO, DATA " + " HAVING(Count(1) > 1) " + ") " + "GROUP BY CODIGO " + "ORDER BY Min(data) ";

			objRS.AdicionarQuery(strSQL);
			objRS.ExecuteQuery();


			while (!objRS.EOF) {
				DadosRecalcular(true, true, true, true, true, true, true, true, true, true,
				true, Convert.ToDateTime(objRS.Field("Data")), "#" + Convert.ToString(objRS.Field("Codigo")) + "#");

				objRS.MoveNext();

			}

		}

	}
}
