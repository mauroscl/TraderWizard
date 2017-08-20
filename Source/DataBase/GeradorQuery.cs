using System;
using prjDominio.Regras;
using TraderWizard.Enumeracoes;

namespace DataBase
{

	public class GeradorQuery
	{
	    private readonly FuncoesBd _formatadorDeCampo;

	    public GeradorQuery(FuncoesBd formatadorDeCampo)
	    {
	        _formatadorDeCampo = formatadorDeCampo;
	    }

	    /// <summary>
		/// Gera a query que retorna as datas e os valores de entrada utilizando o setup do IFR 2 COM FILTRO.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrPeriodicidade">PERÍODO DO GRÁFICO. VALORES VÁLIDOS: "DIARIO", "SEMANAL"</param>
		/// <param name="pstrFiltroMME49">Indica como deve ser feito o filtro em relação à média móvel exponencial de 49 períodos.
		/// Os valores possíveis são:
		/// "ACIMA": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// "ABAIXO": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// "TODOS": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// ou igual à média de 49 períodos</param>
		/// <param name="pintOrdem">Ordem de preferência na execução do setup. É um valor constante e serve para ordenar as entradas
		/// caso seja utilizado mais de um setup ao mesmo tempo e mais de um deles der entrada na mesma data</param>
		/// <param name="pdtmDataInicial">data inicial para começar os testes. Se não for recebido nenhum parâmetro
		/// os testes serão feitos desde a primeira cotação</param>
		/// <param name="pblnAcionamentoGerar">Indica</param>
		/// Indica também se deve gerar a data de acionamento do setup, já que o setup gera a entrada, mas só é acionado 
		/// 'se no próximo período os preços atingirem o valor de entrada. 
		/// <param name="pblnMME21Incluir">Indica se a query deve ou não retornar o valor da MME21 </param>
		/// <param name="pintMediaTipo">
		/// Indica o tipo de média que deve ser utilizada nos cálculos (aritmética ou exponencial)
		/// </param>
		/// <returns></returns>
		/// <remarks>ATENÇÃO: OS SPLITS JÁ SÃO TRATADOS DENTRO DA PRÓPRIA QUERY.</remarks>
		/// <alteracoes>
		/// 17/12/2010 - Inclusão da média móvel de 200 e do tipo de média para indicar se devem ser usadas médias exponencias ou aritméticas
		///</alteracoes>
		public static string BackTestingIFRComFiltroEntradaQueryGerar(FuncoesBd FuncoesBd, string pstrCodigo, string pstrPeriodicidade, string pstrFiltroMME49, int pintOrdem, cEnum.enumMediaTipo pintMediaTipo, 
            DateTime pdtmDataInicial, bool pblnAcionamentoGerar = true, bool pblnMME21Incluir = false)
		{

			string strTabelaCotacao = String.Empty;
			string strTabelaMedia = String.Empty;
			string strTabelaIFR = String.Empty;

			//Busca o nome das tabelas
			cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

		    string strMediaTipo;

			switch (pintMediaTipo)
			{
			    case cEnum.enumMediaTipo.Aritmetica:
			        strMediaTipo = "MMA";
			        break;
			    case cEnum.enumMediaTipo.Exponencial:
			        strMediaTipo = "MME";
			        break;
			    default:
			        strMediaTipo = String.Empty;
			        break;
			}

			string strQuery = "SELECT " + FuncoesBd.CampoStringFormatar("IFR2>MMA13") + " AS SETUP";

			if (pblnAcionamentoGerar) {
				//se é para gerar a data de acionamento retorna a data do 3º período, que é a data que supera
				//a máxima do dia em que o IFR cruzou a média
				strQuery = strQuery + ", DIA3.DATA As DATA_ENTRADA, DIA3.Sequencial";


			} else {
				//se NÃO é para gerar a data de acionamento retorna a data do 2º período, que é a data do dia 
				//em que o IFR superou a média. Neste caso não temos certeza que vai haver a entrada efetiva no setup.
				strQuery = strQuery + ", DIA2.DATA As DATA_ENTRADA, DIA2.Sequencial";

			}

			strQuery = strQuery + ", DIA2.Valor_Entrada, DIA2.Valor_Stop_Loss " + ", " + pintOrdem.ToString() + " As ORDEM " + ", DIA2.Media_IFR " + Environment.NewLine + ", DIA2.Valor_Entrada + DIA2.ValorMaximo - DIA2.ValorMinimo AS Valor_Realizacao_Parcial " + Environment.NewLine + ", DIA2.ValorFechamento, DIA2.ValorAbertura, DIA2.ValorMaximo, DIA2.ValorMinimo " + Environment.NewLine;


			if (pstrFiltroMME49 != "TODOS") {
				strQuery = strQuery + ", DIA2.MME49 " + Environment.NewLine;

			}


			if (pblnMME21Incluir) {
				strQuery = strQuery + ", DIA2.MME21 " + Environment.NewLine;

			}

			strQuery = strQuery + "FROM " + Environment.NewLine;


			if (pblnAcionamentoGerar) {
				//só deve havar este primeiro parentese se for gerada query para o acionamento, pois neste caso será gerada
				//query para o terceiro dia.
				strQuery = strQuery + "(";

			}

			//#####Início do Dia 1
			strQuery = strQuery + "(" + Environment.NewLine + '\t' + "SELECT C.data, valorfechamento, SEQUENCIAL " + Environment.NewLine + '\t' + "FROM ((" + strTabelaCotacao + " C INNER JOIN " + strTabelaMedia + " M " + Environment.NewLine + '\t' + "On C.CODIGO = M.CODIGO " + Environment.NewLine + '\t' + "And C.DATA = M.DATA) " + Environment.NewLine + '\t' + "INNER JOIN " + strTabelaIFR + " I " + Environment.NewLine + '\t' + "On C.CODIGO = I.CODIGO " + Environment.NewLine + '\t' + "And C.DATA = I.DATA) " + Environment.NewLine + '\t' + "WHERE C.Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + '\t' + " And M.TIPO = " + FuncoesBd.CampoStringFormatar("IFR2") + Environment.NewLine + '\t' + " And M.NUMPERIODOS = 13 " + Environment.NewLine + '\t' + " And I.NUMPERIODOS = 2 " + Environment.NewLine + '\t' + " And I.VALOR < M.VALOR " + Environment.NewLine;


			if (pdtmDataInicial != Constantes.DataInvalida) {
				strQuery = strQuery + '\t' + " AND C.Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;

			}

			strQuery = strQuery + ") As DIA1 " + Environment.NewLine;

			//#####Fim do Dia 1

			//#####Início do Dia 2

			strQuery = strQuery + " INNER JOIN " + Environment.NewLine + " (" + Environment.NewLine + '\t' + " SELECT C.data, SEQUENCIAL, ValorAbertura, ValorFechamento, ValorMaximo, ValorMinimo " + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + '\t' + ", ValorMinimo - IIF(ROUND(ValorMinimo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBd.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMinimo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBd.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Stop_Loss " + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + '\t' + ", ValorMaximo + IIF(ROUND(ValorMaximo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBd.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMaximo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBd.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Entrada " + Environment.NewLine;


			if (pstrFiltroMME49 != "TODOS") {
				strQuery = strQuery + '\t' + ", MM49_FECH.Valor AS MME49";

			}


			if (pblnMME21Incluir) {
				strQuery = strQuery + '\t' + ", MM21_FECH.Valor AS MME21";

			}

			//Retorna também o valor da média móvel aritmética de 13 períodos do IFR
			strQuery = strQuery + ", MM_IFR.Valor AS Media_IFR, ValorMinimo, ValorMaximo, MM200_FECH.Valor AS MM200 " + Environment.NewLine;

		    string strTabelaSegundoDia = "((" + strTabelaCotacao + " C INNER JOIN " + strTabelaMedia + " MM_IFR " +
		                                 Environment.NewLine + '\t' + " On C.CODIGO = MM_IFR.CODIGO " + Environment.NewLine +
		                                 '\t' + " And C.DATA = MM_IFR.DATA) " + Environment.NewLine + '\t' + " INNER JOIN " +
		                                 strTabelaIFR + " I " + Environment.NewLine + '\t' + " On C.CODIGO = I.CODIGO " +
		                                 Environment.NewLine + '\t' + " And C.DATA = I.DATA) " + Environment.NewLine;


			if (pstrFiltroMME49 != "TODOS") {
				//quando tem filtro pela MME 49, tem que inserir mais uma vez a tabela de média
				strTabelaSegundoDia = "(" + strTabelaSegundoDia + '\t' + " INNER JOIN " + strTabelaMedia + " MM49_FECH " + Environment.NewLine + '\t' + " On C.CODIGO = MM49_FECH.CODIGO " + Environment.NewLine + '\t' + " And C.DATA = MM49_FECH.DATA) " + Environment.NewLine;

			}


			if (pblnMME21Incluir) {
				strTabelaSegundoDia = "(" + strTabelaSegundoDia + '\t' + " INNER JOIN " + strTabelaMedia + " MM21_FECH " + Environment.NewLine + '\t' + " On C.CODIGO = MM21_FECH.CODIGO " + Environment.NewLine + '\t' + " And C.DATA = MM21_FECH.DATA) " + Environment.NewLine;

			}

			//Faz um SELECT para gerar uma tabela interna com a média de 200 períodos. 
			//Não pode fazer uma junção direta com a tabela de médias.

		    string strTabelaMme200 = "(SELECT Codigo, Data, Valor" + Environment.NewLine + " FROM " + strTabelaMedia +
		                             Environment.NewLine + " WHERE Tipo = " + FuncoesBd.CampoStringFormatar(strMediaTipo) +
		                             Environment.NewLine + " AND NumPeriodos = 200 " + Environment.NewLine + " AND Codigo = " +
		                             FuncoesBd.CampoStringFormatar(pstrCodigo) + ") AS MM200_FECH";

			//concatena a tabela da MME200 na tabela do segundo dia.
			strTabelaSegundoDia = "(" + strTabelaSegundoDia + " INNER JOIN " + Environment.NewLine + strTabelaMme200 + " On C.CODIGO = MM200_FECH.CODIGO " + Environment.NewLine + " And C.DATA = MM200_FECH.DATA) " + Environment.NewLine;

			strQuery = strQuery + '\t' + " FROM " + strTabelaSegundoDia + '\t' + " WHERE C.Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + '\t' + " And MM_IFR.TIPO = " + FuncoesBd.CampoStringFormatar("IFR2") + Environment.NewLine + '\t' + " And MM_IFR.NUMPERIODOS = 13 " + Environment.NewLine + '\t' + " And I.NUMPERIODOS = 2 " + Environment.NewLine + '\t' + " And I.VALOR > MM_IFR.VALOR " + Environment.NewLine;


			if (pstrFiltroMME49 != "TODOS") {
				strQuery = strQuery + '\t' + " And MM49_FECH.TIPO = " + FuncoesBd.CampoStringFormatar(strMediaTipo) + Environment.NewLine + '\t' + " And MM49_FECH.NUMPERIODOS = 49 " + Environment.NewLine;


				if (pstrFiltroMME49 == "ACIMA") {
					strQuery = strQuery + '\t' + " And C.VALORFECHAMENTO >= MM49_FECH.VALOR " + Environment.NewLine;


				} else {
					strQuery = strQuery + '\t' + " And C.VALORFECHAMENTO < MM49_FECH.VALOR " + Environment.NewLine;

				}

			}


			if (pblnMME21Incluir) {
				strQuery = strQuery + '\t' + " And MM21_FECH.TIPO = " + FuncoesBd.CampoStringFormatar(strMediaTipo) + Environment.NewLine + '\t' + " And MM21_FECH.NUMPERIODOS = 21 " + Environment.NewLine;

			}


			if (pdtmDataInicial != Constantes.DataInvalida) {
				strQuery = strQuery + '\t' + " AND C.Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;

			}

			//NO QUERY DO TERCEIRO PERÍODO, CASO HAJA SPLIT, MULTIPLICA OS VALORES PELO RAZÃO DO SPLIT INVERTIDA,
			//POIS O SEGUNDO DIA NÃO ESTÁ COM A COTAÇÃO CONVERTIDA E NO "ON" DO "LEFT JOIN" DO DIA2 COM O DIA3 
			//OS VALORES ENTRE AS DUAS TABELAS SÃO COMPARADOS. POR ISSO EXISTE A NECESSIDADE DE AS DUAS ESTAREM 
			//NA MESMA RAZÃO.
			strQuery = strQuery + " ) As DIA2 ";

			//#####Fim do Dia 2

			strQuery = strQuery + " On (DIA2.SEQUENCIAL - DIA1.SEQUENCIAL) = 1 ";


			if (pblnAcionamentoGerar) {
				//FECHA PARENTESE DO JOIN DO DIA1 COM O DIA2
				strQuery = strQuery + ")";

				//Só gera query para o terceiro dia quando tiver que retornar a data de acionamento da query.

				//#####Início do Dia 3
				strQuery = strQuery + " INNER JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + " SELECT " + strTabelaCotacao + ".DATA, SEQUENCIAL " + Environment.NewLine + '\t' + ", VALORMINIMO * IIF(SPLIT.CODIGO IS NULL, 1, QuantidadePosterior / QuantidadeAnterior) AS VALOR_MINIMO " + Environment.NewLine + '\t' + ", VALORMAXIMO * IIF(SPLIT.CODIGO IS NULL, 1, QuantidadePosterior / QuantidadeAnterior) AS VALOR_MAXIMO " + Environment.NewLine + '\t' + " FROM " + strTabelaCotacao + " LEFT JOIN SPLIT " + Environment.NewLine + '\t' + " ON " + strTabelaCotacao + ".Codigo = SPLIT.Codigo " + Environment.NewLine;


				if (pstrPeriodicidade == "DIARIO") {
					//SE O GRÁFICO É DIÁRIO, LIGA A DATA DO SPLIT EXATAMENTE COM A DATA DA COTAÇÃO
					strQuery = strQuery + '\t' + " AND " + strTabelaCotacao + ".Data = SPLIT.Data " + Environment.NewLine;


				} else if (pstrPeriodicidade == "SEMANAL") {
					//SE O GRÁFICO É SEMANAL, A DATA DO SPLIT PODE TER OCORRIDO EM QUALQUER DIA DA SEMANA.
					//ENTÃO TEM QUE GARANTIR QUE O SPLIT ESTÁ ENTRE A DATA INICIAL E A DATA FINAL.
					strQuery = strQuery + '\t' + " AND " + strTabelaCotacao + ".Data <= SPLIT.Data " + Environment.NewLine + '\t' + " AND " + strTabelaCotacao + ".DataFinal >= SPLIT.Data & vbNewLine ";

				}

				strQuery = strQuery + '\t' + " WHERE " + strTabelaCotacao + ".CODIGO = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine;


				if (pdtmDataInicial != Constantes.DataInvalida) {
					strQuery = strQuery + '\t' + " AND " + strTabelaCotacao + ".Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + Environment.NewLine;

				}

				//#####Fim do Dia 3

				//ESTA PARTE DA QUERY GARANTE QUE NO PERÍODO SEGUINTE AO ACIONAMENTO DA ENTRADA O VALOR DE 
				//ENTRADA ESTÁ ENTRE O VALOR MÍNIMO E O VALOR MÁXIMO. 
				//PODE ACONTECER QUE OCORRA UM GAP DE ALTA E JÁ ABRA ASSIM DO VALOR DE ENTRADA E ATÉ O FINAL DO PERÍODO
				//O PREÇO NÃO RECUE ATÉ ESTE VALOR. SE ISTO NÃO OCORRER CONSIDERA-SE QUE A ENTRADA NA OPERAÇÃO NÃO OCORREU.
				strQuery = strQuery + ") As DIA3 " + Environment.NewLine + " On (DIA3.SEQUENCIAL - DIA2.SEQUENCIAL = 1) " + Environment.NewLine + " And (DIA2.Valor_Entrada >= DIA3.VALOR_MINIMO) " + Environment.NewLine + " And (DIA2.Valor_Entrada <= DIA3.VALOR_MAXIMO) " + Environment.NewLine;

			}

			return strQuery;

		}

		/// <summary>
		/// Gera a query que retorna as datas e os valores de entrada utilizando este setup.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrPeriodicidade">PERÍODO DO GRÁFICO. VALORES VÁLIDOS: DIARIO, SEMANAL</param>
		/// <param name="pstrFiltroMME49">Indica como deve ser feito o filtro em relação à média móvel exponencial de 49 períodos.
		/// Os valores possíveis são:
		/// "ACIMA": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// "ABAIXO": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// "TODOS": gera entrada apenas quando os preços fecharem maior ou igual à média móvel exponencial de 49 períodos.
		/// ou igual à média de 49 períodos</param>
		/// <param name="pintOrdem">Ordem de preferência na execução do setup. É um valor constante e serve para ordenar as entradas
		/// caso seja utilizado mais de um setup ao mesmo tempo e mais de um deles der entrada na mesma data</param>
		/// <param name="pdblIFRValorMaximoSobrevendido">Valor para o qual o IFR tem que ser menor ou igual para ser considerado sobrevendido, ou seja, gerar entrada neste setup.
		/// O padrão que se utiliza é 5.</param>
		/// <param name="pblnMME21Incluir">Indica se a query deve ou não retornar o valor da MME21 </param>
		/// <param name="pintMediaTipo">
		/// Indica o tipo de média que deve ser utilizada nos cálculos (aritmética ou exponencial)
		/// </param>
		/// <returns></returns>
		/// <remarks></remarks>
		/// <alteracoes>
		/// 17/12/2010 - Inclusão da média móvel de 200 e do tipo de média para indicar se devem ser usadas médias exponencias ou aritméticas
		///</alteracoes>
		public static string BackTestingIFRSemFiltroEntradaQueryGerar(string pstrCodigo, string pstrPeriodicidade, string pstrFiltroMME49, double pdblIFRValorMaximoSobrevendido, int pintOrdem, cEnum.enumMediaTipo pintMediaTipo, FuncoesBd funcoesBd, bool pblnMME21Incluir = false)
		{

			string strTabelaCotacao = String.Empty;
			string strTabelaMedia = String.Empty;
			string strTabelaIFR = String.Empty;

			string strMediaTipo;

			if (pintMediaTipo == cEnum.enumMediaTipo.Aritmetica) {
				strMediaTipo = "MMA";
			} else if (pintMediaTipo == cEnum.enumMediaTipo.Exponencial) {
				strMediaTipo = "MME";
			} else {
				strMediaTipo = String.Empty;
			}

			//Busca o nome das tabelas
			cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

		    var strTabela = "(" + strTabelaCotacao + " C" + " INNER JOIN " + strTabelaIFR + " I " + Environment.NewLine + " On C.CODIGO = I.CODIGO " + Environment.NewLine + " And C.DATA = I.DATA" + ")" + Environment.NewLine;

			//Inicio da tabela da média de 49
			strTabela = "(" + strTabela;

			strTabela = strTabela + " INNER JOIN " + strTabelaMedia + " MME49 " + Environment.NewLine + " On C.CODIGO = MME49.CODIGO " + Environment.NewLine + " And C.DATA = MME49.DATA " + Environment.NewLine;

			strTabela = strTabela + ")";

			//Fim da tabela da média de 49


			if (pblnMME21Incluir) {
				strTabela = "(" + strTabela;

				strTabela = strTabela + " INNER JOIN " + strTabelaMedia + " MME21 " + Environment.NewLine + " On C.CODIGO = MME21.CODIGO " + Environment.NewLine + " And C.DATA = MME21.DATA " + Environment.NewLine;

				strTabela = strTabela + ")";

			}

			//Faz um SELECT para gerar uma tabela interna com a média de 200 períodos. 
			//Não pode fazer uma junção direta com a tabela de médias.

		    var strTabelaMme200 = "(" + Environment.NewLine + '\t' + "SELECT Codigo, Data, Valor" + Environment.NewLine + '\t' + " FROM " + strTabelaMedia + Environment.NewLine + '\t' + " WHERE Tipo = " + funcoesBd.CampoStringFormatar(strMediaTipo) + Environment.NewLine + '\t' + " AND NumPeriodos = 200 " + Environment.NewLine + '\t' + " AND Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + ") AS MM200" + Environment.NewLine;

			//concatena a tabela da MME200  na tabela do segundo dia.
			strTabela = "(" + strTabela + " INNER JOIN " + Environment.NewLine + strTabelaMme200 + " On C.CODIGO = MM200.CODIGO " + Environment.NewLine + " And C.DATA = MM200.DATA) " + Environment.NewLine;


			string strTabelaMediaIFR = "(" + Environment.NewLine + '\t' + "SELECT Codigo, Data, Valor" + Environment.NewLine + '\t' + " FROM " + strTabelaMedia + Environment.NewLine + '\t' +
                " WHERE Tipo = " + funcoesBd.CampoStringFormatar("IFR2") + Environment.NewLine + '\t' + " AND NumPeriodos = 13 " + Environment.NewLine + '\t' + " AND Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + ") AS MMIFR" + Environment.NewLine;

			//concatena a tabela da MME200  na tabela do segundo dia.
			strTabela = "(" + strTabela + " INNER JOIN " + Environment.NewLine + strTabelaMediaIFR + " On C.CODIGO = MMIFR.CODIGO " + Environment.NewLine + " And C.DATA = MMIFR.DATA) " + Environment.NewLine;


			var strQuery = "SELECT " + funcoesBd.CampoStringFormatar("IFR2SOBREVEND") + " AS SETUP" + ", " + " C.DATA As DATA_ENTRADA, VALORFECHAMENTO As VALOR_ENTRADA " + ", Round(VALORMINIMO - (VALORMAXIMO - VALORMINIMO) * 1.3, 2) As VALOR_STOP_LOSS " + ", " + pintOrdem.ToString() + " As ORDEM " + ", Sequencial, I.Valor AS VALOR_IFR, ValorAbertura, ValorMaximo, ValorMinimo, MME49.VALOR AS MME49, MM200.Valor AS MME200, MMIFR.Valor AS MMIFR ";


			if (pblnMME21Incluir) {
				strQuery = strQuery + ", MME21.Valor as MME21 ";

			}

			strQuery = strQuery + Environment.NewLine;

			strQuery = strQuery + " FROM " + strTabela + " WHERE C.Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + 
                " And I.NumPeriodos = 2 " + Environment.NewLine + " And I.Valor <= " + funcoesBd.CampoFloatFormatar(pdblIFRValorMaximoSobrevendido) + Environment.NewLine;

			//inicio do where relacionado à média de 49
			strQuery = strQuery + " And MME49.Tipo = " + funcoesBd.CampoStringFormatar(strMediaTipo) + Environment.NewLine + " And MME49.NumPeriodos = 49 " + Environment.NewLine;


			if (pstrFiltroMME49 == "ACIMA") {
				strQuery = strQuery + " And C.ValorFechamento >= MME49.Valor " + Environment.NewLine;


			} else if (pstrFiltroMME49 == "ABAIXO") {
				strQuery = strQuery + " And C.ValorFechamento < MME49.Valor " + Environment.NewLine;

			}

			//fim do where relacionado à média de 49


			if (pblnMME21Incluir) {
				strQuery = strQuery + " And MME21.Tipo = " + funcoesBd.CampoStringFormatar(strMediaTipo) + Environment.NewLine + " And MME21.NumPeriodos = 21 " + Environment.NewLine;

			}

			//21/01/2011 - Removido o critério para uma data específica porque pode acontecer de numa sequencia de datas em que o setup der entrada
			//haja uma data intermediária em que a simulação ainda não ficou completa e uma data posterior a esta em que a simulação ficou completa. 
			//Caso isso ocorresse e somente fossem pegos os casos com data maior que a última data de simulação esta data intermediária nunca entraria na 
			//simulação. 
			//No lugar deste critério será adiconado um critério verificando se a data já existe na tabela de cotação diária.
			//Este critério será adicionado fora desta função.

			//If pdtmDataInicial <> DataInvalida Then

			//    strQuery = strQuery _
			//    & " AND C.Data >= " & FuncoesBD.CampoDateFormatar(pdtmDataInicial)

			//End If



			//strQuery = strQuery _
			//& " ORDER BY C.Data "

			return strQuery;

		}

	    /// <summary>
	    /// Gera as cláusulas FROM e WHERE para serem utilizadas com as queries que buscam os dados para gerar os gráficos.
	    /// </summary>
	    /// <param name="pstrCodigoAtivo"></param>
	    /// <param name="pdtmDataMinima">Data inicial de busca dos dados</param>
	    /// <param name="pdtmDataMaxima">Data final de busca dos dados</param>
	    /// <param name="pstrTabela">
	    /// Indica a tabela que será utilizada para buscar os dados do gráfico.
	    /// Os valores possíveis são: COTACAO, COTACAO_SEMANAL, MEDIA_DIARIA, MEDIA_SEMANAL.
	    /// </param>
	    /// <param name="pdblOperador">Quando a cotação possuir splits, contém todas as operações para transformar
	    /// as cotações de períodos anteriores ao split na mesma razão da última cotação</param>
	    /// <param name="pdblOperadorInvertido">Mesma função do parâmetro pstrOperador, porém com as operações invertidas,
	    /// caso seja necessário obter os valores do split invertido. Este operador é utilizado geralmente para calcular o volume
	    /// </param>
	    /// <param name="pintNumPeriodos">Número de períodos da média que dever ser buscada. Necessário informar apenas quando a função é utilizada
	    /// para calcular médias</param>
	    /// <param name="pstrDado">VALOR OU VOLUME. Necessário informar apenas quando a função é utilizada
	    /// para calcular médias</param>
	    /// <param name="pstrFinalidade">Indica se esta função está sendo chamada para obter todos os registros de um determinado período
	    /// ou apenas os valores mínimos e máximos
	    /// Valores possíveis: "TODOS", "EXTREMOS"</param>
	    /// <param name="pblnCotacaoBuscar">Indica se devem ser buscadas dados relativos ao preço do ativo</param>
	    /// <param name="pblnVolumeBuscar">Indica se devem ser buscados dados relativos ao volume do ativo</param>
	    /// <param name="pstrMediaTipo">Tipo da média que deve ser consultada. Valores possíveis: "E" (exponencial), "A" (aritmética). 
	    /// Necessário informar apenas quando a função é utilizada para calcular médias</param>
	    /// <param name="pstrOrderBy">Indica o ordenamento da consulta. Deve ser informado apenas quando o parâmetro "pstrFinalidade" estiver
	    /// com o valor "TODOS", mas não é obrigatório informar neste caso.</param>
	    /// <returns>string contendo as cláusulas from e where</returns>
	    /// <remarks></remarks>
	    public string ConsultaUnitariaGerar(string pstrCodigoAtivo, DateTime pdtmDataMinima, DateTime pdtmDataMaxima, string pstrTabela, double pdblOperador, double pdblOperadorInvertido, string pstrFinalidade, string pstrMediaTipo = "", int pintNumPeriodos = -1, string pstrDado = "",
	        bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrOrderBy = "")
	    {

	        string strSql = String.Empty;

	        string strColunas = String.Empty;

	        string strTabelaAux = pstrTabela.ToUpper();

	        if (strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL")
	        {

	            if (pstrFinalidade == "TODOS")
	            {

	                if (pblnCotacaoBuscar)
	                {
	                    strColunas = " data, valorabertura * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + " as valorabertura " + Environment.NewLine + ", valorfechamento * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + " as valorfechamento " + Environment.NewLine + ", valorminimo * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + " as valorminimo " + Environment.NewLine + ", valormaximo * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + " as valormaximo " + Environment.NewLine + ", Oscilacao ";


	                    if (strTabelaAux == "COTACAO_SEMANAL")
	                    {
	                        strColunas = strColunas + ", DataFinal";

	                    }

	                }


	            }
	            else if (pstrFinalidade == "EXTREMOS")
	            {
	                strSql = " SELECT MIN(ValorMinimo * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + ") AS ValorMinimo " + ", MAX(ValorMaximo * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + ") AS ValorMaximo ";

	            }


	        }
	        else
	        {
	            //se é a tabela de médias
	            if (pstrDado == "VALOR")
	            {
	                //SE É MEDIA DE VALOR

	                if (pstrFinalidade == "TODOS")
	                {
	                    strSql = " SELECT Data, Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + " as Valor " + Environment.NewLine;


	                }
	                else if (pstrFinalidade == "EXTREMOS")
	                {
	                    strSql = " SELECT MIN(Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + ") AS ValorMinimo " + Environment.NewLine + ", MAX(Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperador) + ") AS ValorMaximo " + Environment.NewLine + ", COUNT(1) as NumRegistros " + Environment.NewLine;

	                }
	            }
	            else
	            {
	                //SE É MEDIA DE VOLUME

	                if (pstrFinalidade == "TODOS")
	                {
	                    strSql = '\t' + " select Data, Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + " as Valor " + Environment.NewLine;


	                }
	                else if (pstrFinalidade == "EXTREMOS")
	                {
	                    strSql = " SELECT MIN(Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + ") AS ValorMinimo " + Environment.NewLine + ", MAX(Valor * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + ") AS ValorMaximo " + Environment.NewLine + ", COUNT(1) as NumRegistros " + Environment.NewLine;

	                }

	            }

	        }

	        //Quando é uma das tabelas de cotações, calcula o volume também.

	        if (strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL")
	        {

	            if (pstrFinalidade == "TODOS")
	            {

	                if (pblnVolumeBuscar)
	                {
	                    if (strColunas != String.Empty)
	                    {
	                        strColunas = strColunas + ", ";
	                    }

	                    strColunas = strColunas + "titulos_total * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + " as Titulos_Total " + Environment.NewLine + ", Negocios_Total, Valor_Total " + Environment.NewLine;

	                }


	            }
	            else
	            {
	                strSql = strSql + ", MIN(titulos_total * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + ") as Volume_Minimo " + Environment.NewLine + ", MAX(titulos_total * " + _formatadorDeCampo.CampoFormatar(pdblOperadorInvertido) + ") as Volume_Maximo " + Environment.NewLine + ", COUNT(1) AS ContadorVolumeMedio " + Environment.NewLine;

	            }

	        }


	        if ((strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL") && pstrFinalidade == "TODOS")
	        {
	            strSql = "SELECT " + strColunas;

	        }

	        //********INICIO DO TRATAMENTO DO FROM, WHERE e ORDER BY 
	        strSql = strSql + " from " + pstrTabela + Environment.NewLine;

	        strSql = strSql + " where codigo = " + _formatadorDeCampo.CampoStringFormatar(pstrCodigoAtivo) + Environment.NewLine + 
                " and data >= " + this._formatadorDeCampo.CampoDateFormatar(pdtmDataMinima) + Environment.NewLine +
                " and data <= " + this._formatadorDeCampo.CampoDateFormatar(pdtmDataMaxima) + Environment.NewLine;


	        if (strTabelaAux == "MEDIA_DIARIA" || strTabelaAux == "MEDIA_SEMANAL")
	        {
	            string strMediaTipoAux;

	            if (pstrDado == "VALOR")
	            {
	                if (pstrMediaTipo == "E")
	                {
	                    strMediaTipoAux = "MME";
	                }
	                else if (pstrMediaTipo == "A")
	                {
	                    strMediaTipoAux = "MMA";
	                }
	                else
	                {
	                    strMediaTipoAux = String.Empty;
	                }
	            }
	            else if (pstrDado == "VOLUME")
	            {
	                //volume não verifica qual o tipo de média. É sempre aritmética
	                strMediaTipoAux = "VMA";
	            }
	            else
	            {
	                strMediaTipoAux = String.Empty;
	            }

	            strSql = strSql + " and Tipo = " + _formatadorDeCampo.CampoStringFormatar(strMediaTipoAux) + Environment.NewLine + " and NumPeriodos = " + pintNumPeriodos + Environment.NewLine;

	        }


	        if (pstrFinalidade == "TODOS" && pstrOrderBy != String.Empty)
	        {
	            //se é para listar todos os registros e foi passado um ORDER BY
	            strSql = strSql + " ORDER BY " + pstrOrderBy;

	        }

	        return strSql;

	    }

    }
}
