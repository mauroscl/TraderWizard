using System.Globalization;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using frwInterface;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using prjModelo;
using prjDTO;
using prjModelo.Regras;
using prjModelo.ValueObjects;
using prjServicoNegocio;

namespace prmCotacao
{

	public class cRelatorio
	{


		private readonly cConexao objConexao;

		public cRelatorio(cConexao pobjConexao)
		{
			objConexao = pobjConexao;

		}

		/// <summary>
		/// Remove a parte decimal de um número e retorna apenas a parte inteira
		/// </summary>
		/// <param name="pdblNumero">Número que deve ser truncado</param>
		/// <returns>A parte inteira de um  número</returns>
		/// <remarks></remarks>
		private long Truncar(double pdblNumero)
		{
			long functionReturnValue = 0;

			string strAux = null;

			//converte o número para texto
			strAux = pdblNumero.ToString();

			int intPosicao = 0;

			//procura o ponto decimal
			intPosicao = strAux.IndexOf(",", StringComparison.InvariantCultureIgnoreCase);

			if (intPosicao > 0) {
				//retorna os caracteres à esquerda do ponto.
				functionReturnValue = Convert.ToInt64(strAux.Substring(1, intPosicao));
			} else {
				//se não encontrou o ponto decimal, retorna o próprio número, pois significa que é 
				//um número inteiro
				functionReturnValue = Convert.ToInt64(pdblNumero);
			}
			return functionReturnValue;

		}

		/// <summary>
		/// Gera a query retorna TODOS os ativos que estão gerando entrada no próximo período caso ultrapasse a máxima deste
		/// período. Utiliza a regra de cruzamento da média de 9 períodos de baixo para cima para gerar a entrada
		/// </summary>
		/// <param name="pdtmDataAnterior">Data do período anterior. utilizado para comparar as médias de 9 do período anterior
		/// com o periodo atual</param>
		/// <param name="pdtmDataAtual">data do período em que devem ser buscados os ativos que estão dando entrada pelo setup</param>
		/// <param name="pstrTabelaCotacao">COTACAO ou COTACAO_SEMANAL</param>
		/// <param name="pstrTabelaMedia">MEDIA_DIARIA ou MEDIA_SEMANAL</param>
		/// <param name="pblnAlijamentoCalcular">Indica se é para calcular o valor de venda parcial para alijar o risco.</param>
		/// <param name="pdblTitulosTotal"></param>
		/// <param name="pintNegociosTotal"></param>
		/// <param name="pdecValorTotal"></param>
		/// <param name="pdecPercentualStopGain"></param>
		/// <param name="pdecValorCapital">Valor total do capital</param>
		/// <param name="pdecValorPerdaManejo">Valor máximo que pode ser perdido de todo o capital</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string SetupMME91QueryGerar(System.DateTime pdtmDataAnterior, System.DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, bool pblnAlijamentoCalcular, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1,
		decimal pdecPercentualStopGain = -1)
		{

			string strQuery = null;

			//, dia_atual.valorfechamento, dia_atual.mmexp9

			string strTabelaAux = null;

			string strTabelaMME21 = null;

			if (pstrTabelaCotacao.ToUpper() == "COTACAO") {
				strTabelaMME21 = "MME21_Diario";
			} else {
				strTabelaMME21 = "MME21_Semanal";
			}

			strQuery = " SELECT dia_atual.codigo, dia_atual.ValorFechamento " + Environment.NewLine + ", ROUND(mmexp9, 2) AS MMExp9 " + Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " + Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " + Environment.NewLine + ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " + Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " + Environment.NewLine + ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " + Environment.NewLine;


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) " + "* (1 + " + FuncoesBD.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 4) AS STOP_GAIN " + Environment.NewLine;

			}

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2)  as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento < Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAnterior) + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor as MMExp21 " + " FROM ";

			//ligação da cotação com a tabela de médias para fazer o filtro com a média de 9 perídos.
			strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			//ligaçao com a tabela de médias para calcular o percentual em relação à média de 21 períodos
			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " On c.Codigo = MME21.Codigo " + " And c.Data = MME21.Data)";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = 'MME'" + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual);


			//--NÃO PRECISA DESTE WHERE PORQUE FORAM CRIADAS VIEWS PARA AS MÉDIAS DE 21 DIAS, DIÁRIA E SEMANAL.
			//WHERE DA MÉDIA DE 21 PERIODOS

			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBD.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBD.CampoDecimalFormatar(pdecValorTotal);

			}

			strQuery = strQuery + ") As dia_atual " + " On dia_anterior.codigo = dia_atual.codigo " + " ORDER BY (dia_atual.valorminimo - 0.03)  / (dia_atual.valormaximo + 0.01) desc ";

			return strQuery;

		}

		/// <summary>
		/// Gera a query retorna TODOS os ativos que estão gerando entrada no próximo período caso ultrapasse a máxima deste
		/// período. Utiliza a regra dos preços estarem acima da MME de 9 períodos e o valor de fechamento for menor que a mínima
		/// do período anterior.
		/// </summary>
		/// <param name="pdtmDataAnterior"></param>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pstrTabelaCotacao"></param>
		/// <param name="pstrTabelaMedia"></param>
		/// <param name="pblnAlijamentoCalcular"></param>
		/// <param name="pdecValorCapital"></param>
		/// <param name="pdecValorPerdaManejo"></param>
		/// <param name="pdblTitulosTotal"></param>
		/// <param name="pintNegociosTotal"></param>
		/// <param name="pdecValorTotal"></param>
		/// <param name="pdecPercentualStopGain"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string SetupMME92QueryGerar(System.DateTime pdtmDataAnterior, System.DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, bool pblnAlijamentoCalcular, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1,
		decimal pdecPercentualStopGain = -1)
		{

			string strQuery = null;

			string strTabelaAux = String.Empty;

			string strTabelaMME21 = null;

			if (pstrTabelaCotacao.ToUpper() == "COTACAO") {
				strTabelaMME21 = "MME21_Diario";
			} else {
				strTabelaMME21 = "MME21_Semanal";
			}

			//, dia_atual.valorfechamento, dia_atual.mmexp9
			strQuery = " SELECT dia_atual.codigo, dia_atual.ValorFechamento " + ", ROUND(mmexp9, 2) AS MMExp9 " + Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " + Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " + Environment.NewLine + ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " + Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " + Environment.NewLine + ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " + Environment.NewLine;


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) " + "* (1 + " + FuncoesBD.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN " + Environment.NewLine;

			}

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2) as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total, valorminimo " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAnterior) + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor as MMExp21 " + " FROM ";

			strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " ON C.Codigo = MME21.Codigo " + " AND C.Data = MME21.Data) ";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = 'MME'" + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + " AND c.Oscilacao < 0 ";


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBD.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBD.CampoDecimalFormatar(pdecValorTotal);

			}

			strQuery = strQuery + ") As dia_atual " + " On dia_anterior.codigo = dia_atual.codigo " + " WHERE dia_atual.valorfechamento < dia_anterior.valorminimo " + " ORDER BY (dia_atual.valorminimo - 0.03)  / (dia_atual.valormaximo + 0.01) desc ";

			return strQuery;

		}

		/// <summary>
		/// Gera a query retorna TODOS os ativos que estão gerando entrada no próximo período caso ultrapasse a máxima deste
		/// período. Utiliza a regra dos preços estarem acima da MME de 9 períodos e o papel vem de 2 períodos seguidos de queda.
		/// </summary>
		/// <param name="pdtmDataAnterior"></param>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pstrTabelaCotacao"></param>
		/// <param name="pstrTabelaMedia"></param>
		/// <param name="pblnAlijamentoCalcular"></param>
		/// <param name="pdecValorCapital"></param>
		/// <param name="pdecValorPerdaManejo"></param>
		/// <param name="pdblTitulosTotal"></param>
		/// <param name="pintNegociosTotal"></param>
		/// <param name="pdecValorTotal"></param>
		/// <param name="pdecPercentualStopGain"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string SetupMME93QueryGerar(System.DateTime pdtmDataAnterior, System.DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, bool pblnAlijamentoCalcular, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1,
		decimal pdecPercentualStopGain = -1)
		{

			string strQuery = null;

			string strTabelaAux = String.Empty;

			string strTabelaMME21 = null;

			if (pstrTabelaCotacao.ToUpper() == "COTACAO") {
				strTabelaMME21 = "MME21_Diario";
			} else {
				strTabelaMME21 = "MME21_Semanal";
			}

			//, dia_atual.valorfechamento, dia_atual.mmexp9

			strQuery = " SELECT dia_atual.Codigo, dia_atual.ValorFechamento " + ", ROUND(mmexp9, 2) AS MMExp9 " + Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " + Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " + ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " + Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " + Environment.NewLine + ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " + Environment.NewLine;


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.ValorMaximo + 0.01) " + "* (1 + " + FuncoesBD.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN " + Environment.NewLine;

			}

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2)  as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAnterior) + " AND c.Oscilacao < 0 " + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor AS MMExp21 " + " FROM ";

			strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " ON C.Codigo = MME21.Codigo " + " AND C.Data = MME21.Data) ";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = " + FuncoesBD.CampoStringFormatar("MME") + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + " AND c.Oscilacao < 0 ";


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBD.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBD.CampoDecimalFormatar(pdecValorTotal);

			}

			strQuery = strQuery + ") As dia_atual " + " On dia_anterior.codigo = dia_atual.codigo " + " ORDER BY (dia_atual.valorminimo - 0.03)  / (dia_atual.valormaximo + 0.01) desc ";

			return strQuery;

		}

		/// <summary>
		/// Gera query que verifica para TODOS os ativos quais estão dando entrada pelo setup do IFR 2 sobrevendido.
		/// </summary>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pstrTabelaCotacao"></param>
		/// <param name="pstrTabelaMedia"></param>
		/// <param name="pstrTabelaIFR"></param>
		/// <param name="pblnAlijamentoCalcular"></param>
		/// <param name="pblnAcimaMME49"></param>
		/// <param name="pdecValorCapital"></param>
		/// <param name="pdecValorPerdaManejo"></param>
		/// <param name="pdblIFR2LimiteSuperior"></param>
		/// <param name="pdblTitulosTotal">Filtro pelo número de titulos negociados no período. Quando o valor for igual a "-1" 
		/// o filtro é desconsiderado.</param>
		/// <param name="pintNegociosTotal">Filtro pelo número de negócios realizados no período. Quando o valor for igual a "-1" 
		/// o filtro é desconsiderado.</param>
		/// <param name="pdecValorTotal">Filtro pelo valor total em moeda negociado no período. Quando o valor for igual a "-1" 
		/// o filtro é desconsiderado.</param>
		/// <param name="pdecPercentualStopGain"></param>
		/// <returns></returns>
		/// <remarks>Esta função é utilizada pela tela que gera o relatório</remarks>
		private string SetupIFR2SemFiltroQueryGerar(DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, string pstrTabelaIFR, bool pblnAlijamentoCalcular, bool pblnAcimaMME49, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblIFR2LimiteSuperior = 5, double pdblTitulosTotal = -1,
		int pintNegociosTotal = -1, decimal pdecValorTotal = -1, decimal pdecPercentualStopGain = -1)
		{

			string strQuery = null;

			string strTabela = null;

			strQuery = " SELECT " + pstrTabelaCotacao + ".codigo, ValorFechamento " + ", ROUND(" + pstrTabelaIFR + ".Valor, 2) AS IFR2" + Environment.NewLine + ", MME49.Valor as Valor_MME49 " + ", ROUND((ValorFechamento / MME49.Valor - 1) * 100, 4) AS Perc_MME49 " + Environment.NewLine + ", ROUND(valorfechamento,2) As entrada " + Environment.NewLine + ", ROUND(valorminimo - (valormaximo - valorminimo) * 1.3, 2) As stop_loss " + Environment.NewLine + ", ROUND(((valorminimo - (valormaximo - valorminimo) * 1.3) / valorfechamento -1) * 100, 4) As perc_stop_loss " + Environment.NewLine;



			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND(VALORFECHAMENTO " + "* (1 + " + FuncoesBD.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 4) AS STOP_GAIN " + Environment.NewLine;

			}

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / ValorFechamento / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * ValorFechamento AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar tudo que for possível sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;


			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			//OBS: EXISTE UM IIF TESTANDO SE O VALOR MÁXIMO É DIFERENTE DO VALOR MÍNIMO PARA EVITAR ERROS
			//DE DIVISÃO POR ZERO. ESTA CONDIÇÃO SÓ VAI ACONTECER EM AÇÕES MUITO POUCO LÍQUIDAS
			strAux = "CSTR(IIF(ValorMaximo <> ValorMinimo " + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "(ValorFechamento  - (ValorMinimo - (ValorMaximo - ValorMinimo) * 1.3)) " + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + ") / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * ValorFechamento AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar o que o manejo de risco permitir.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 2)  AS Perc_Risco_Manejo " + Environment.NewLine;


			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND(VALORFECHAMENTO * 2 - (valorminimo - (valormaximo - valorminimo) * 1.3), 2) as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((ValorFechamento / MME21.Valor - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;


			strTabela = "(" + pstrTabelaCotacao + " INNER JOIN " + pstrTabelaIFR + Environment.NewLine + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaIFR + ".Codigo " + Environment.NewLine + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaIFR + ".Data) " + Environment.NewLine;



			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabela = "(" + strTabela + " INNER JOIN " + pstrTabelaMedia + " AS MV " + Environment.NewLine + " ON " + pstrTabelaCotacao + ".Codigo = MV.Codigo " + Environment.NewLine + " AND " + pstrTabelaCotacao + ".Data = MV.Data) " + Environment.NewLine;

			}

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 21 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 21 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 21 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME21
			strTabela = "(" + strTabela + " LEFT JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + " WHERE Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + " And NumPeriodos = 21 " + Environment.NewLine + ") AS MME21 " + Environment.NewLine + " On " + pstrTabelaCotacao + ".Codigo = MME21.Codigo " + Environment.NewLine + " And " + pstrTabelaCotacao + ".Data = MME21.Data) " + Environment.NewLine;

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 49 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 49 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 49 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME49
			strTabela = "(" + strTabela + " LEFT JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + " WHERE Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + " And NumPeriodos = 49 " + Environment.NewLine + ") AS MME49 " + Environment.NewLine + " On " + pstrTabelaCotacao + ".Codigo = MME49.Codigo " + Environment.NewLine + " And " + pstrTabelaCotacao + ".Data = MME49.Data) " + Environment.NewLine;


			strQuery = strQuery + " FROM " + strTabela + " WHERE " + pstrTabelaCotacao + ".Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + " And " + pstrTabelaIFR + ".Valor <= " + FuncoesBD.CampoFloatFormatar(pdblIFR2LimiteSuperior);


			if (pblnAcimaMME49) {
				strQuery = strQuery + " And " + pstrTabelaCotacao + ".ValorFechamento >= MME49.Valor ";

			}


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {

				strQuery = strQuery + " AND " + FiltroVolumeNegociosGerar(pstrTabelaCotacao, pstrTabelaCotacao, pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {

				strQuery = strQuery + " AND " + FiltroVolumeFinanceiroGerar(pstrTabelaCotacao, pstrTabelaCotacao, pdecValorTotal);


			}

			strQuery = strQuery + " ORDER BY  (valorminimo - (valormaximo - valorminimo) * 1.3) / valorfechamento desc ";

			return strQuery;


		}

		/// <summary>
		/// Para um determinado sequencial de um ativo, determina quantos períodos o ativo está em um valor considerado sobrevendido
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="plngSequencialInicial">Sequencial da cotação para a qual se quer saber quantos períodos está sobrevendido</param>
		/// <param name="pdblValorMaximoIFRSobrevendido">Valor a partir do qual o IFR estando abaixo deste valor é considerado sobrevendido </param>
		/// <returns></returns>
		/// <remarks></remarks>
		private int NumTentativasCalcular(string pstrCodigo, long plngSequencialInicial, double pdblValorMaximoIFRSobrevendido)
		{

			cRS objRS = new cRS(objConexao);

		    dynamic lngSequencialAtual = plngSequencialInicial;
			bool blnOK = false;
			dynamic lngUltimoSequencialSobrevendido = plngSequencialInicial;

			//Enquanto não encontrar o número de tentativas

			while (!blnOK) {
				string strSQL = "SELECT TOP 5 Sequencial, Valor " + Environment.NewLine;
				strSQL = strSQL + " FROM Cotacao C INNER JOIN IFR_Diario IFR " + Environment.NewLine;
				strSQL = strSQL + " ON C.Codigo = IFR.Codigo " + Environment.NewLine;
				strSQL = strSQL + " AND C.Data = IFR.Data " + Environment.NewLine;
				strSQL = strSQL + " WHERE C.Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
				strSQL = strSQL + " AND Sequencial < " + FuncoesBD.CampoFormatar(lngSequencialAtual) + Environment.NewLine;
				strSQL = strSQL + " ORDER BY Sequencial DESC ";

				objRS.ExecuteQuery(strSQL);


				while ((!objRS.EOF) && (!blnOK)) {


					if (Convert.ToDouble(objRS.Field("Valor")) <= pdblValorMaximoIFRSobrevendido) {
						lngUltimoSequencialSobrevendido = Convert.ToInt64(objRS.Field("Sequencial"));

					} else {

						if ((lngUltimoSequencialSobrevendido - Convert.ToInt64(objRS.Field("Sequencial"))) >= 2) {
							blnOK = true;

						}
					}

					objRS.MoveNext();

				}

				lngSequencialAtual = lngSequencialAtual - 5;

				objRS.Fechar();

			}

			return plngSequencialInicial - lngUltimoSequencialSobrevendido + 1;

		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pobjSetup"></param>
		/// <param name="pobjCM"></param>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="plstFaixas">Lista de faixas em que o trade se enquadra. Tem uma faixa para cada critério</param>
		/// <param name="pdecValorEntrada">valor base utilizado para aplicar os percentuais de realização parcial e final</param>
		/// <param name="pdtmDataEntrada">Data de entrada no trade. Utilizado para só utilizar as simulações até esta data no cálculo dos valores.</param>
		/// <param name="pstrValorRealizacaoParcialRet"></param>
		/// <param name="pstrValorRealizacaoFinalRet"></param>
		/// <remarks></remarks>

		public void CalcularValoresRealizacao(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cIFRSobrevendido pobjIFRSobrevendido, IList<cIFRSimulacaoDiariaFaixa> plstFaixas, decimal pdecValorEntrada, DateTime pdtmDataEntrada, ref string pstrValorRealizacaoParcialRet, ref string pstrValorRealizacaoFinalRet)
		{
			string strSQL = null;
			string strWhere = String.Empty;

			strSQL = " SELECT MIN(Percentual_Maximo) AS Percentual_Realizacao_Parcial, MAX(Percentual_Saida) AS Percentual_Saida_Maximo " + Environment.NewLine;


			strWhere += " FROM IFR_SIMULACAO_DIARIA D " + Environment.NewLine;
			strWhere += " WHERE D.Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strWhere += " AND D.ID_Setup = " + FuncoesBD.CampoFormatar(pobjSetup.ID) + Environment.NewLine;
			strWhere += " AND D.ID_CM = " + FuncoesBD.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strWhere += " AND Verdadeiro = " + FuncoesBD.CampoFormatar(true) + Environment.NewLine;
			strWhere += " AND Valor_IFR_Minimo <= " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ValorMaximo) + Environment.NewLine;
			strWhere += " AND Data_Saida <= " + FuncoesBD.CampoFormatar(pdtmDataEntrada);


			foreach (cIFRSimulacaoDiariaFaixa objIFRFaixa in plstFaixas) {
				strWhere += " AND EXISTS " + Environment.NewLine;
				strWhere += "(" + Environment.NewLine;
				strWhere += '\t' + " SELECT 1 " + Environment.NewLine;
				strWhere += '\t' + " FROM IFR_Simulacao_Diaria_Faixa F " + Environment.NewLine;
				strWhere += '\t' + " WHERE ID = " + FuncoesBD.CampoFormatar(objIFRFaixa.ID) + Environment.NewLine;
				strWhere += '\t' + " AND " + objIFRFaixa.CriterioCM.CampoBD + " BETWEEN Valor_Minimo AND Valor_Maximo " + Environment.NewLine;
				strWhere += '\t' + " AND ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;
				strWhere += ")";

			}

			strSQL = strSQL + strWhere;

			cRS objRS = new cRS(objConexao);

			objRS.ExecuteQuery(strSQL);

			decimal decPercentualRealizacaoParcial = Math.Floor(Convert.ToDecimal(objRS.Field("Percentual_Realizacao_Parcial")));

			double decPercentualSaidaMaximo = Convert.ToDouble(objRS.Field("Percentual_Saida_Maximo"));

			pstrValorRealizacaoParcialRet = Math.Round(pdecValorEntrada * (1 + decPercentualRealizacaoParcial / 100), 2).ToString() + " (" + decPercentualRealizacaoParcial.ToString() + " %)";

			objRS.Fechar();

			strSQL = " SELECT MIN(Percentual_Maximo) AS Percentual_Realizacao_Final " + Environment.NewLine;

			strSQL = strSQL + strWhere;
			strSQL = strSQL + " AND Percentual_Maximo > " + FuncoesBD.CampoFormatar(decPercentualSaidaMaximo);

			objRS.ExecuteQuery(strSQL);

			decimal decPercentualRealizacaoFinal;


			if (!objRS.EOF) {
				//Se encontrou registro, verifica se o percentual é maior do que o percentual da realização parcial
				if (Convert.ToDecimal(objRS.Field("Percentual_Realizacao_Final")) > decPercentualRealizacaoParcial) {
					decPercentualRealizacaoFinal = (decimal) Math.Floor(Convert.ToDouble(objRS.Field("Percentual_Realizacao_Final")));
				} else {
					decPercentualRealizacaoFinal = decPercentualRealizacaoParcial;
				}



			} else {
				decPercentualRealizacaoFinal = decPercentualRealizacaoParcial;

			}

			pstrValorRealizacaoFinalRet = Math.Round(pdecValorEntrada * (1 + Math.Floor(decPercentualRealizacaoFinal) / 100), 2).ToString("0##.##") + " (" + decPercentualRealizacaoFinal.ToString("0#") + " %)";


		}

		private string AproveitamentoConsultar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{
			string functionReturnValue = null;

			cCarregadorDeResumoDoIFRDiario objCarregadorResumo = new cCarregadorDeResumoDoIFRDiario(objConexao);

			cIFRSimulacaoDiariaFaixaResumo objResumo = objCarregadorResumo.Carregar(pobjSimulacaoDiariaVO);


			if ((objResumo != null)) {
				functionReturnValue = objResumo.PercentualAcertosComFiltro.ToString("###,###0.00") + "% (" + objResumo.NumAcertosComFiltro.ToString("###") + " / " + objResumo.NumTradesComFiltro.ToString("###") + ")";
                

			} else {
				functionReturnValue = string.Empty;

			}
			return functionReturnValue;

		}

		/// <summary>
		/// Verifica se o setup precisa ser simulado antes de gerar o relatório. Importante para que todas as estatísticas sejam calculadas
		/// antes de exibir o relatório e não fique nenhum ativo com simulação pendente.
		/// </summary>
		/// <param name="pobjSetup"></param>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pdblTitulosTotal"></param>
		/// <param name="pintNegociosTotal"></param>
		/// <param name="pdecValorTotal"></param>
		/// <returns></returns>
		/// <remarks></remarks>

		private void SetupIFR2SemFiltroDiarioPersonalizadoVerificar(Setup pobjSetup, System.DateTime pdtmDataAtual, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1)
		{
			string strSQL = null;
			string strTabela = null;

			strTabela = "COTACAO CH INNER JOIN IFR_DIARIO IFRH " + Environment.NewLine;
			strTabela = strTabela + " On CH.CODIGO = IFRH.CODIGO " + Environment.NewLine;
			strTabela = strTabela + " And CH.DATA = IFRH.DATA " + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strTabela = "(" + strTabela + ") INNER JOIN Media_Diaria MV" + Environment.NewLine;
				strTabela = strTabela + " ON CH.Codigo = MV.Codigo " + Environment.NewLine;
				strTabela = strTabela + " AND CH.Data = MV.Data " + Environment.NewLine;

			}

			strSQL = " SELECT CH.CODIGO " + Environment.NewLine;
			strSQL = strSQL + " FROM " + strTabela;

			strSQL = strSQL + " WHERE CH.DATA = " + FuncoesBD.CampoFormatar(pdtmDataAtual) + Environment.NewLine;
			strSQL = strSQL + " And CH.SEQUENCIAL > 200 " + Environment.NewLine;
			//Não faz sentido começar a verificação se o papel tem apenas 200 períodos, pois precisamos da média de 200 nos cálculos

			strSQL = strSQL + " AND IFRH.NumPeriodos = 2 " + Environment.NewLine;
			strSQL = strSQL + " AND IFRH.VALOR <= 10 " + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strSQL = strSQL + " AND MV.Tipo = " + FuncoesBD.CampoFormatar("VMA") + Environment.NewLine;
				strSQL = strSQL + " AND MV.NumPeriodos = 21 " + Environment.NewLine;
				strSQL = strSQL + " AND MV.Valor >= " + FuncoesBD.CampoFormatar(pdblTitulosTotal) + Environment.NewLine;

			}


			if (pdecValorTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeFinanceiroGerar("CH", "Cotacao", pdecValorTotal);

			}


			if (pintNegociosTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeNegociosGerar("CH", "Cotacao ", pintNegociosTotal);

			}

			strSQL += " And EXISTS " + Environment.NewLine;
			strSQL += " ( " + Environment.NewLine;
			strSQL += '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + " FROM COTACAO C1 INNER JOIN IFR_DIARIO IFR1 " + Environment.NewLine;
			strSQL += '\t' + " On C1.CODIGO = IFR1.CODIGO " + Environment.NewLine;
			strSQL += '\t' + " And C1.DATA = IFR1.DATA " + Environment.NewLine;
			strSQL += '\t' + " WHERE CH.CODIGO = C1.CODIGO " + Environment.NewLine;
			strSQL += '\t' + " And C1.DATA < CH.DATA " + Environment.NewLine;
			strSQL += '\t' + " And IFR1.VALOR <=10 " + Environment.NewLine;
			strSQL += '\t' + " And  C1.SEQUENCIAL >= 200 " + Environment.NewLine;
			strSQL += '\t' + " And EXISTS " + Environment.NewLine;
			strSQL += '\t' + " ( " + Environment.NewLine;
			strSQL += '\t' + '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + '\t' + " FROM COTACAO C2 " + Environment.NewLine;
			strSQL += '\t' + '\t' + " WHERE C1.CODIGO = C2.CODIGO " + Environment.NewLine;
			strSQL += '\t' + '\t' + " AND C2.Data <= " + FuncoesBD.CampoFormatar(pdtmDataAtual);
			//Só pode verificar até a data de geração do relatório
			strSQL += '\t' + '\t' + " And C2.DATA > C1.DATA " + Environment.NewLine;
			strSQL += '\t' + '\t' + " And " + Environment.NewLine;
			strSQL += '\t' + '\t' + " ( " + Environment.NewLine;
			strSQL += '\t' + '\t' + '\t' + " (C2.VALORMAXIMO >= C1.VALORFECHAMENTO * 1.05) " + Environment.NewLine;
			//VERIFICA SE HOUVE REALIZAÇÃO PARCIAL (5%)
			strSQL += '\t' + '\t' + '\t' + " OR (C2.VALORMINIMO <= (C1.VALORMINIMO - (C1.VALORMAXIMO - C1.VALORMINIMO) * 1.3)) " + Environment.NewLine;
			//OU SE FOI ESTOPADO (130% DO CANDLE DE ENTRADA PARA BAIXO)
			strSQL += '\t' + '\t' + " ) " + Environment.NewLine;
			strSQL += '\t' + " )" + Environment.NewLine;
			strSQL += '\t' + " AND NOT EXISTS " + Environment.NewLine;
			strSQL += '\t' + " ( " + Environment.NewLine;
			strSQL += '\t' + '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + '\t' + " FROM IFR_SIMULACAO_DIARIA S " + Environment.NewLine;
			strSQL += '\t' + '\t' + " WHERE C1.CODIGO = S.CODIGO " + Environment.NewLine;
			strSQL += '\t' + '\t' + " And C1.DATA = S.DATA_ENTRADA_EFETIVA " + Environment.NewLine;
			strSQL += '\t' + '\t' + " And S.ID_SETUP = " + FuncoesBD.CampoFormatar(pobjSetup.ID) + Environment.NewLine;
			strSQL += '\t' + " ) " + Environment.NewLine;
			strSQL += " )" + Environment.NewLine;

			cRS objRS = new cRS(objConexao);

			//Debug.Print(strSQL)

			objRS.ExecuteQuery(strSQL);

			IList<string> lstAtivos = new List<string>();


			while (!objRS.EOF) {
				lstAtivos.Add(Convert.ToString(objRS.Field("Codigo")));

				objRS.MoveNext();

			}

			objRS.Fechar();

			//lstAtivos = {"INPR3", "AGEN11", "BRTO4", "AEDU3", "BBDC4", "CYRE3"}


			cSetupIFR2SimularDTO objSetupIFRSimularDTO = new cSetupIFR2SimularDTO();

			objSetupIFRSimularDTO.IFRTipo = cEnum.enumIFRTipo.SemFiltro;
			objSetupIFRSimularDTO.MediaTipo = cEnum.enumMediaTipo.Exponencial;
			objSetupIFRSimularDTO.SubirStopApenasAposRealizacaoParcial = true;
			//objSetupIFRSimularDTO.ValorMaximoIFRSobrevendido = 10
			objSetupIFRSimularDTO.ExcluirSimulacoesAnteriores = false;



			if (lstAtivos.Count > 0) {
				string strDescricao = string.Empty;


				foreach (string strCodigoAtivo in lstAtivos) {
					if (strDescricao != string.Empty) {
						strDescricao = strDescricao + ", ";
					}

					strDescricao = strDescricao + strCodigoAtivo;

				}


				//if (Interaction.MsgBox("Os Ativos (" + strDescricao + ") estão com simulações pendentes. Deseja executar a simulação destes ativos antes de gerar o relatório?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) == MsgBoxResult.Yes) {
                if (MessageBox.Show("Os Ativos (" + strDescricao + ") estão com simulações pendentes. Deseja executar a simulação destes ativos antes de gerar o relatório?","Trader Wizard",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
					SimularIFRDiarioParaListaDeAtivos(lstAtivos, objSetupIFRSimularDTO);

				}

			}

		}


		public bool SimularIFRDiarioParaListaDeAtivos(IList<string> plstAtivos, cSetupIFR2SimularDTO pobjSetupIFRSimularDTO)
		{


			try {
				System.Threading.AutoResetEvent[] arrayAutoResetEvent = new  System.Threading.AutoResetEvent[plstAtivos.Count];
				//Dim arrayAutoResetEvent(lstAtivos.Count - 1) As Threading.WaitHandle

				//obtém os valores antes de alterar para depois da execução restaurar
				int intMinWorkThreads = 0;
				int intMinIOThreads = 0;
				System.Threading.ThreadPool.GetMinThreads(out intMinWorkThreads, out intMinIOThreads);

				int intMaxWorkThreads = 0;
				int intMaxIOThreads = 0;
				System.Threading.ThreadPool.GetMaxThreads(out intMaxWorkThreads, out intMaxIOThreads);

				System.Threading.ThreadPool.SetMinThreads(4, 2);
				System.Threading.ThreadPool.SetMaxThreads(4, 2);

				int intI = 0;

				string strCodigoAtivo = null;

				DateTime dtmHoraInicial = DateTime.Now.Date;


				for (intI = 0; intI <= plstAtivos.Count - 1; intI++) {
					strCodigoAtivo = plstAtivos[intI];

					cSetupIFR2SimularCodigoDTO objSetupIFR2SimularCodigoDTO = new cSetupIFR2SimularCodigoDTO(pobjSetupIFRSimularDTO, strCodigoAtivo);

					cSimuladorIFRDiario objSimuladorIFRDiario = new cSimuladorIFRDiario(objSetupIFR2SimularCodigoDTO);

					//System.Threading.WaitCallback objCallBack = new System.Threading.WaitCallback(objSimuladorIFRDiario.SetupIFR2Simular);
                    System.Threading.WaitCallback objCallBack = objSimuladorIFRDiario.SetupIFR2Simular;

					System.Threading.AutoResetEvent objAutoResetEvent = new System.Threading.AutoResetEvent(false);
					arrayAutoResetEvent.SetValue(objAutoResetEvent, intI);

					System.Threading.ThreadPool.QueueUserWorkItem(objCallBack, objAutoResetEvent);

				}

				// Threading.WaitHandle.WaitAll(arrayAutoResetEvent)


				for (intI = 0; intI <= arrayAutoResetEvent.Count() - 1; intI++) {
					//Threading.WaitHandle.WaitAny(arrayAutoResetEvent.GetValue(intI))
					((System.Threading.AutoResetEvent)arrayAutoResetEvent.GetValue(intI)).WaitOne();

				}

				//Restaura os valores anteriores
				System.Threading.ThreadPool.SetMinThreads(intMinWorkThreads, intMinIOThreads);
				System.Threading.ThreadPool.SetMaxThreads(intMaxWorkThreads, intMaxIOThreads);

				return true;


			} catch (Exception ex) {
			    MessageBox.Show("Ocorreram erros ao executar a operação. Mensagem de Erro: " + ex.Message);

				return false;

			}

		}

		/// <summary>
		/// Gera o relatório de IFR personalizado sem filtro na tabela temporária e retorna a string para consultar as colunas desta tabela
		/// que serão mostradas no grid
		/// </summary>
		/// <param name="pobjSetup"></param>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pdecValorCapital"></param>
		/// <param name="pdecValorPerdaManejo"></param>
		/// <param name="pdblIFR2LimiteSuperior"></param>
		/// <param name="pdblTitulosTotal">Filtro pelo total de ações negociadas</param>
		/// <param name="pintNegociosTotal">Filtro pelo número de negócios realizados</param>
		/// <param name="pdecValorTotal">Filtro pelo valor total em moeda negociado</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string RelatIFR2SemFiltroDiarioPersonalizadoGerar(Setup pobjSetup, System.DateTime pdtmDataAtual, decimal pdecValorCapital, decimal pdecValorPerdaManejo, cIFRSobrevendido pobjIFRSobrevendido, double pdblIFR2LimiteSuperior = 5, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1)
		{

			//Chama função que verifica se existe alguma simulação pendente e pergunta se o usuário que executá-la antes de iniciar o relatório.
			SetupIFR2SemFiltroDiarioPersonalizadoVerificar(pobjSetup, pdtmDataAtual, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal);

			cCommand objCommand = new cCommand(objConexao);

			objCommand.BeginTrans();

			string strSQL = null;

			strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM Relat_IFR2_Sem_Filtro " + Environment.NewLine;

			objCommand.Execute(strSQL);

			cRS objRS = new cRS(objConexao);


			strSQL = " SELECT Sequencial, C.Codigo, ValorFechamento ";
			strSQL += ", ROUND(IFR.Valor, 2) AS IFR2" + Environment.NewLine;
			strSQL += ", MME49.Valor AS MME49, MME200.Valor AS MME200";
			strSQL += ", ROUND((ValorFechamento / MME21.Valor - 1) * 100, 4) AS Percentual_MME21 " + Environment.NewLine;
			strSQL += ", ROUND((ValorFechamento / MME49.Valor - 1) * 100, 4) AS Percentual_MME49 " + Environment.NewLine;
			strSQL += ", ROUND((ValorFechamento / MME200.Valor - 1) * 100, 4) AS Percentual_MME200 " + Environment.NewLine;
			strSQL += ", ROUND(((ValorFechamento / MME200.Valor - 1) * 100) - ((ValorFechamento / MME21.Valor - 1) * 100), 4) AS Diferenca_MM200_MM21" + Environment.NewLine;
			strSQL += ", ROUND(((ValorFechamento / MME200.Valor - 1) * 100) - ((ValorFechamento / MME49.Valor - 1) * 100), 4) AS Diferenca_MM200_MM49" + Environment.NewLine;
			strSQL += ", ROUND(valorminimo - (valormaximo - valorminimo) * 1.3, 2) As stop_loss " + Environment.NewLine;
			strSQL += ", ROUND(((valorminimo - (valormaximo - valorminimo) * 1.3) / valorfechamento -1) * 100, 4) As perc_stop_loss " + Environment.NewLine;

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / ValorFechamento / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strSQL = strSQL + ", " + strAux + " AS Quantidade_Sem_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strSQL = strSQL + ", " + strAux + " * ValorFechamento AS Valor_Total_Sem_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar tudo que for possível sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strSQL = strSQL + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;


			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			//OBS: EXISTE UM IIF TESTANDO SE O VALOR MÁXIMO É DIFERENTE DO VALOR MÍNIMO PARA EVITAR ERROS
			//DE DIVISÃO POR ZERO. ESTA CONDIÇÃO SÓ VAI ACONTECER EM AÇÕES MUITO POUCO LÍQUIDAS
			strAux = "CSTR(IIF(ValorMaximo <> ValorMinimo " + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "(ValorFechamento  - (ValorMinimo - (ValorMaximo - ValorMinimo) * 1.3)) " + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + ") / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strSQL = strSQL + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strSQL = strSQL + ", " + strAux + " * ValorFechamento AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar o que o manejo de risco permitir.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strSQL = strSQL + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 2)  AS Perc_Risco_Manejo " + Environment.NewLine;


			//****Fim dos cálculos com manejo de risco.

			//percentual em relação à média de 21 períodos
			strSQL = strSQL + ", ROUND((ValorFechamento / MME21.Valor - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			string strTabela = null;

			strTabela = "(Cotacao C INNER JOIN IFR_Diario IFR" + Environment.NewLine + " On C.Codigo = IFR.Codigo " + Environment.NewLine + " And C.Data = IFR.Data) " + Environment.NewLine;



			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabela = "(" + strTabela + " INNER JOIN Media_Diaria MV " + Environment.NewLine + " ON C.Codigo = MV.Codigo " + Environment.NewLine + " AND C.Data = MV.Data) " + Environment.NewLine;

			}

			//JUNÇÃO COM A MÉDIA DE 21 PERÍODOS
			strTabela = "(" + strTabela + " INNER JOIN Media_Diaria MME21 " + Environment.NewLine + " ON C.Codigo = MME21.Codigo " + Environment.NewLine + " AND C.Data = MME21.Data) " + Environment.NewLine;

			//JUNÇÃO COM A MÉDIA DE 49 PERÍODOS
			strTabela = "(" + strTabela + " INNER JOIN Media_Diaria MME49 " + Environment.NewLine + " ON C.Codigo = MME49.Codigo " + Environment.NewLine + " AND C.Data = MME49.Data) " + Environment.NewLine;

			//JUNÇÃO COM A MÉDIA DE 200 PERÍODOS
			strTabela = "(" + strTabela + " INNER JOIN Media_Diaria MME200 " + Environment.NewLine + " ON C.Codigo = MME200.Codigo " + Environment.NewLine + " AND C.Data = MME200.Data) " + Environment.NewLine;



			strSQL = strSQL + " FROM " + strTabela + Environment.NewLine + " WHERE C.Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + " AND C.Sequencial > 200" + " And IFR.NumPeriodos = 2 " + Environment.NewLine + " And IFR.Valor <= " + FuncoesBD.CampoFloatFormatar(pdblIFR2LimiteSuperior) + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strSQL = strSQL + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + Environment.NewLine + " And MV.NumPeriodos = 21 " + Environment.NewLine + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal) + Environment.NewLine;

			}

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 21 PERIODOS
			strSQL += " AND MME21.Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME21.NumPeriodos = 21 " + Environment.NewLine;

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 49 PERIODOS
			strSQL += " AND MME49.Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME49.NumPeriodos = 49 " + Environment.NewLine;

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 200 PERIODOS
			strSQL += " AND MME200.Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME200.NumPeriodos = 200 " + Environment.NewLine;



			if (pintNegociosTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeNegociosGerar("C", "Cotacao", pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeFinanceiroGerar("C", "Cotacao", pdecValorTotal);

			}

			objRS.ExecuteQuery(strSQL);

			cClassifMedia objClassifMedia = null;

			int intNumTentativas = 0;

			int intSomatorioCriterios = 0;

			IList<cIFRSimulacaoDiariaFaixa> lstFaixas = new List<cIFRSimulacaoDiariaFaixa>();

			string strValorRealizacaoParcial = null;
			string strValorRealizacaoFinal = null;

			string strAproveitamento = null;

			cValorCriterioClassifMediaVO objValorCriterioCMVO = new cValorCriterioClassifMediaVO();

			cAtivo objAtivo = null;
			cCotacaoDiaria objCotacaoDiaria = null;

			dynamic objCarregadorCarteira = new cCarregadorCarteira(objConexao);
			dynamic objCarteiraAtiva = objCarregadorCarteira.CarregaAtiva(pobjIFRSobrevendido);

			string strCodigoAtivo = null;

			//Para cada um dos itens que está com IFR sobrevendido.

			while ((!objRS.EOF) && (objConexao.TransStatus)) {
				lstFaixas.Clear();

				strValorRealizacaoParcial = string.Empty;

				strValorRealizacaoFinal = string.Empty;

				objAtivo = new cAtivo((string) objRS.Field("Codigo"), string.Empty);

				objCotacaoDiaria = new cCotacaoDiaria(objAtivo, pdtmDataAtual);
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));

				objCotacaoDiaria.Medias.Add(new cMediaDiaria(objCotacaoDiaria, "MME", 49, Convert.ToDouble(objRS.Field("MME49"))));
				objCotacaoDiaria.Medias.Add(new cMediaDiaria(objCotacaoDiaria, "MME", 200, Convert.ToDouble(objRS.Field("MME200"))));

				objAtivo.CotacoesDiarias.Add(objCotacaoDiaria);

				//Calcula a classificação da média
				objClassifMedia = objAtivo.ObterClassificacaoDeMediaNaData(pdtmDataAtual);

				//Calcula o número de tentativas
				intNumTentativas = NumTentativasCalcular((string) objRS.Field("Codigo"), Convert.ToInt64(objRS.Field("Sequencial")), pobjIFRSobrevendido.ValorMaximo);

				//Verifica se atende a todos os critérios
				cVerificaSeDeveGerarEntrada objVerificaSeDeveGerarEntrada = new cVerificaSeDeveGerarEntrada(objConexao);

				objValorCriterioCMVO.PercentualMM21 = Convert.ToDouble(objRS.Field("Percentual_MME21"));
				objValorCriterioCMVO.PercentualMM49 = Convert.ToDouble(objRS.Field("Percentual_MME49"));
				objValorCriterioCMVO.PercentualMM200 = Convert.ToDouble(objRS.Field("Percentual_MME200"));
				objValorCriterioCMVO.PercentualMM200MM21 = Convert.ToDouble(objRS.Field("Diferenca_MM200_MM21"));
				objValorCriterioCMVO.PercentualMM200MM49 = Convert.ToDouble(objRS.Field("Diferenca_MM200_MM49"));

				SimulacaoDiariaVO objSimulacaoDiariaVO = new SimulacaoDiariaVO();
				objSimulacaoDiariaVO.Ativo = objAtivo;
				objSimulacaoDiariaVO.Setup = pobjSetup;
				objSimulacaoDiariaVO.ClassificacaoMedia = objClassifMedia;
				objSimulacaoDiariaVO.DataEntradaEfetiva = pdtmDataAtual;
				objSimulacaoDiariaVO.IFRSobrevendido = pobjIFRSobrevendido;
				objSimulacaoDiariaVO.NumTentativas = intNumTentativas;

				//Verifica se deve gerar entrada: atende a todos os critérios de classificação da média, quantidade de tentativas e percentual de acertos.
				//Caso deva gerar entrada, retorna uma lista das faixas em que o trade se faixa (uma faixa para cada critério).
				intSomatorioCriterios = objVerificaSeDeveGerarEntrada.Verificar(objSimulacaoDiariaVO, objValorCriterioCMVO, lstFaixas);

				if (intSomatorioCriterios == 0) {
					//Se todos os critérios foram atendidos, calcula o valor de realização parcial e final em função das faixas selecionadas
					CalcularValoresRealizacao((string) objRS.Field("Codigo"), pobjSetup, objClassifMedia, pobjIFRSobrevendido, lstFaixas, Convert.ToDecimal(objRS.Field("ValorFechamento")), pdtmDataAtual, ref strValorRealizacaoParcial, ref strValorRealizacaoFinal);

				}

				strAproveitamento = AproveitamentoConsultar(objSimulacaoDiariaVO);

				strCodigoAtivo = objRS.Field("Codigo").ToString();


				if (objCarteiraAtiva.AtivoEstaNaCarteira(objAtivo)) {
					strCodigoAtivo += " *";

				}

				//Lança na tabela temporária
				strSQL = "INSERT INTO Relat_IFR2_Sem_Filtro " + Environment.NewLine;
				strSQL = strSQL + "(Codigo, ID_IFR_Sobrevendido, Valor_IFR, Valor_Entrada, Valor_Realizacao_Parcial, Valor_Realizacao_Final " + Environment.NewLine;
				strSQL = strSQL + ", Valor_Stop_Loss, Percentual_Stop_Loss, Quantidade_Sem_Manejo, Valor_Total_Sem_Manejo " + Environment.NewLine;
				strSQL = strSQL + ", Percentual_Risco_Sem_Manejo, Quantidade_Com_Manejo, Valor_Total_Com_Manejo, Percentual_Risco_Com_Manejo, ID_CM " + Environment.NewLine;
				strSQL = strSQL + ", Aproveitamento, Percentual_MM21, Percentual_MM49, Percentual_MM200 " + Environment.NewLine;
				strSQL = strSQL + ", Diferenca_MM200_MM21, Diferenca_MM200_MM49, NumTentativas, SomatorioCriterios) " + Environment.NewLine;
				strSQL = strSQL + " VALUES " + Environment.NewLine;
				strSQL = strSQL + "(" + FuncoesBD.CampoFormatar(strCodigoAtivo);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDouble(objRS.Field("IFR2")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDecimal(objRS.Field("ValorFechamento")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(strValorRealizacaoParcial);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(strValorRealizacaoFinal);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDecimal(objRS.Field("stop_loss")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDecimal(objRS.Field("perc_stop_loss")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToInt32(objRS.Field("Quantidade_Sem_Manejo")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDecimal(objRS.Field("Valor_Total_Sem_Manejo")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDouble(objRS.Field("Perc_Risco_Capital")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToInt32(objRS.Field("Quantidade_Manejo")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDecimal(objRS.Field("Valor_Manejo")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Convert.ToDouble(objRS.Field("Perc_Risco_Manejo")));
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objClassifMedia.ID);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(strAproveitamento);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objValorCriterioCMVO.PercentualMM21);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objValorCriterioCMVO.PercentualMM49);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objValorCriterioCMVO.PercentualMM200);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objValorCriterioCMVO.PercentualMM200MM21);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objValorCriterioCMVO.PercentualMM200MM49);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intNumTentativas);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intSomatorioCriterios) + ")";

				objCommand.Execute(strSQL);

				objRS.MoveNext();

			}

			objCommand.CommitTrans();

			objRS.Fechar();

			strSQL = " SELECT Codigo, Valor_IFR, Valor_Entrada, Valor_Realizacao_Parcial, Valor_Realizacao_Final " + Environment.NewLine;
			strSQL = strSQL + ", Valor_Stop_Loss, Percentual_Stop_Loss, Quantidade_Sem_Manejo, Valor_Total_Sem_Manejo " + Environment.NewLine;
			strSQL = strSQL + ", Percentual_Risco_Sem_Manejo, Quantidade_Com_Manejo, Valor_Total_Com_Manejo, Percentual_Risco_Com_Manejo, ID_CM " + Environment.NewLine;
			strSQL = strSQL + ", Aproveitamento ";
			strSQL = strSQL + ", CSTR(Percentual_MM21) AS Percentual_MM21 , CSTR(Percentual_MM49) AS Percentual_MM49, CSTR(Percentual_MM200) AS Percentual_MM200 " + Environment.NewLine;
			strSQL = strSQL + ", CSTR(Diferenca_MM200_MM21) AS Diferenca_MM200_MM21, CSTR(Diferenca_MM200_MM49) AS Diferenca_MM200_MM49, NumTentativas " + Environment.NewLine;

			strSQL = strSQL + ", SomatorioCriterios " + Environment.NewLine;
			strSQL = strSQL + " FROM Relat_IFR2_Sem_Filtro " + Environment.NewLine;
			strSQL = strSQL + " WHERE ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;
			strSQL = strSQL + " ORDER BY Percentual_Stop_Loss DESC";

			return strSQL;

		}

		/// <summary>
		/// Gera query que verifica para TODOS os ativos quais estão dando entrada pelo setup do IFR 2 com filtro.
		/// </summary>
		/// <param name="pdtmDataAnterior"></param>
		/// <param name="pdtmDataAtual"></param>
		/// <param name="pstrTabelaCotacao"></param>
		/// <param name="pstrTabelaMedia"></param>
		/// <param name="pstrTabelaIFR"></param>
		/// <param name="pblnAlijamentoCalcular"></param>
		/// <param name="pblnAcimaMME49"></param>
		/// <param name="pdecValorCapital"></param>
		/// <param name="pdecValorPerdaManejo"></param>
		/// <param name="pdblTitulosTotal"></param>
		/// <param name="pintNegociosTotal"></param>
		/// <param name="pdecValorTotal"></param>
		/// <param name="pdecPercentualStopGain"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string SetupIFR2ComFiltroQueryGerar(System.DateTime pdtmDataAnterior, System.DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, string pstrTabelaIFR, bool pblnAlijamentoCalcular, bool pblnAcimaMME49, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1,
		Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1, decimal pdecPercentualStopGain = -1)
		{
		    string strQuery = " SELECT segundo_dia.Codigo, segundo_dia.ValorFechamento " + ", ROUND(segundo_dia.Valor, 2) AS IFR2" + Environment.NewLine + ", MMExp49 AS Valor_MME49 " + ", ROUND((ValorFechamento / MMExp49 - 1) * 100, 4) AS Perc_MME49 " + Environment.NewLine + ", ROUND(segundo_dia.Valor_Entrada, 2) As entrada " + ", ROUND(((segundo_dia.Valor_Entrada) / segundo_dia.ValorFechamento - 1) * 100, 4) As perc_entrada " + ", ROUND(segundo_dia.Valor_Stop_Loss, 2) As stop_loss " + ", ROUND(((segundo_dia.Valor_Stop_Loss) / (segundo_dia.Valor_Entrada) -1) * 100, 4) As perc_stop_loss ";


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((segundo_dia.Valor_Entrada) " + "* (1 + " + FuncoesBD.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN ";

			}

			string strAux = null;

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + " / segundo_dia.Valor_Entrada / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (segundo_dia.Valor_Entrada) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (segundo_dia.Valor_Entrada - segundo_dia.Valor_Stop_Loss) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBD.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "(segundo_dia.Valor_Entrada - segundo_dia.Valor_Stop_Loss) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBD.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * segundo_dia.Valor_Entrada AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (segundo_dia.Valor_Entrada -  segundo_dia.Valor_Stop_Loss) " + "/ " + FuncoesBD.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND(segundo_dia.Valor_Entrada * 2 - segundo_dia.Valor_Stop_Loss, 2)  as STOP_GAIN ";

			}

			//percentual do valor de fechamento em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 ";

			strQuery = strQuery + " FROM " + Environment.NewLine + "(" + Environment.NewLine + '\t' + " SELECT " + pstrTabelaCotacao + ".Codigo " + Environment.NewLine + '\t' + " FROM ((" + pstrTabelaCotacao + " INNER JOIN " + pstrTabelaMedia + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaMedia + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaMedia + ".Data) " + Environment.NewLine + '\t' + " INNER JOIN " + pstrTabelaIFR + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaIFR + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaIFR + ".Data) " + Environment.NewLine + '\t' + " WHERE " + pstrTabelaCotacao + ".Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAnterior) + Environment.NewLine + '\t' + " And " + pstrTabelaMedia + ".Tipo = " + FuncoesBD.CampoStringFormatar("IFR2") + Environment.NewLine + '\t' + " And " + pstrTabelaMedia + ".NumPeriodos = 13 " + Environment.NewLine + '\t' + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + Environment.NewLine + '\t' + " And " + pstrTabelaIFR + ".Valor < " + pstrTabelaMedia + ".Valor " + Environment.NewLine + ") As primeiro_dia " + Environment.NewLine + " INNER JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + " SELECT " + pstrTabelaCotacao + ".Codigo, valorfechamento " + '\t' + ", MME21.Valor AS MMExp21, MME49.Valor AS MMExp49, " + pstrTabelaIFR + ".Valor" + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + ", ValorMinimo - IIF(ROUND(ValorMinimo * " + FuncoesBD.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBD.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMinimo * " + FuncoesBD.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBD.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Stop_Loss " + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + ", ValorMaximo + IIF(ROUND(ValorMaximo * " + FuncoesBD.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBD.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMaximo * " + FuncoesBD.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBD.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Entrada " + Environment.NewLine;

			string strTabela = " ((" + pstrTabelaCotacao + " INNER JOIN " + pstrTabelaMedia + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaMedia + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaMedia + ".Data) " + Environment.NewLine + '\t' + " INNER JOIN " + pstrTabelaIFR + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaIFR + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaIFR + ".Data) " + Environment.NewLine;


			if (pdblTitulosTotal != -1.0) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabela = "(" + strTabela + '\t' + " INNER JOIN " + pstrTabelaMedia + " AS MV " + Environment.NewLine + '\t' + " ON " + pstrTabelaCotacao + ".Codigo = MV.Codigo " + Environment.NewLine + '\t' + " AND " + pstrTabelaCotacao + ".Data = MV.Data) " + Environment.NewLine;

			}

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 21 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 21 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 21 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME21
			strTabela = "(" + strTabela + '\t' + " LEFT JOIN " + Environment.NewLine + '\t' + "(" + Environment.NewLine + '\t' + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + '\t' + " WHERE Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + '\t' + " And NumPeriodos = 21 " + Environment.NewLine + '\t' + ") AS MME21 " + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = MME21.Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = MME21.Data) " + Environment.NewLine;

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 49 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 49 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 49 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME49
			strTabela = "(" + strTabela + '\t' + " LEFT JOIN " + Environment.NewLine + '\t' + "(" + Environment.NewLine + '\t' + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + '\t' + " WHERE Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + '\t' + " And NumPeriodos = 49 " + Environment.NewLine + '\t' + ") AS MME49 " + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = MME49.Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = MME49.Data) " + Environment.NewLine;

			strQuery = strQuery + '\t' + " FROM " + strTabela + '\t' + " WHERE " + pstrTabelaCotacao + ".Data = " + FuncoesBD.CampoDateFormatar(pdtmDataAtual) + '\t' + " And " + pstrTabelaMedia + ".Tipo = " + FuncoesBD.CampoStringFormatar("IFR2") + '\t' + " And " + pstrTabelaMedia + ".NumPeriodos = 13 " + '\t' + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + '\t' + " And " + pstrTabelaIFR + ".Valor > " + pstrTabelaMedia + ".Valor ";


			if (pblnAcimaMME49) {
				strQuery = strQuery + '\t' + " And " + pstrTabelaCotacao + ".ValorFechamento >= MME49.Valor " + Environment.NewLine;

			}

			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + '\t' + " And MV.Tipo = " + FuncoesBD.CampoStringFormatar("VMA") + Environment.NewLine + '\t' + " And MV.NumPeriodos = 21 " + Environment.NewLine + '\t' + " And MV.Valor >= " + FuncoesBD.CampoFloatFormatar(pdblTitulosTotal) + Environment.NewLine;

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " AND " + FiltroVolumeNegociosGerar(pstrTabelaCotacao, pstrTabelaCotacao, pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " AND " + FiltroVolumeFinanceiroGerar(pstrTabelaCotacao, pstrTabelaCotacao, pdecValorTotal);


			}

			strQuery = strQuery + " ) As segundo_dia " + Environment.NewLine + " On primeiro_dia.Codigo = segundo_dia.Codigo " + Environment.NewLine + " ORDER BY (segundo_dia.Valor_Entrada) / (segundo_dia.Valor_Stop_Loss) ";

			return strQuery;

		}


		/// <summary>
		/// Gera um relatório de listagem de um setup para todos os ativos em uma determinada data.
		/// O relatório é salvo em tabelas para posterior apresentação
		/// </summary>
		/// <param name="pdtmData">Data principal do SETUP. É a data onde são procuradas as entradas.
		/// Se o setup utilizar mais de uma data, então as outras datas serão anteriores à data informada. </param>
		/// <param name="pintSetup">Código do SETUP.
		/// </param>
		/// <param name="pstrPeriodo">DIARIO ou SEMANAL</param>
		/// <param name="pdecPercentualRealizacaoParcial">Quando for informado, indicado o percentual que
		/// deve ser aplicado ao valor de entrada para que se obtenha o valor de realização</param>
		/// <param name="pdblTitulosTotal">Filtro utilizada na média de 21 dias da quantidade de ações negociadas.
		/// Quando este filtro for utilizado o número médio de ações negociadas tem que ser maior ou igual ao valor
		/// informado no filtro</param>
		/// <param name="pblnAlijamentoCalcular">Indica se é para calcular o valor de venda para alijar o risco </param>
		/// <param name="pblnAcimaMME49">Indica se é para considerar apenas os ativos cujo valor de fechamento 
		/// é igual ou maior do que a MME de 49 períodos. Utilizado somente pelos setups de IFR 2</param>
		/// <param name="pdblIFR2LimiteSuperior">Valor máximo do IFR que é considerado como sobrevendido. 
		/// O padrão do setup é 5, mas o usuário pode variá-lo</param>
		/// <param name="pintNegociosTotal">Filtro pelo total de negócios no período. Se este filtro for passado
		/// serão considerados apenas os ativos cuja média de 21 períodos do total de negócios for igual ou superior ao valor do filtro</param>
		/// <param name="pdecValorTotal">Filtro pelo valor total em dinheiro negociado no período. 
		/// Se este filtro for passado serão considerados apenas os ativos cujo valor total em dinheiro 
		/// for igual ou superior ao valor do filtro</param>
		/// <param name="pdecCapitalTotal">Indica o total de capital disponível para aplicar.
		/// </param>
		/// <param name="pdecPercentualManejo">Indica o percentual máximo de todo o capital que 
		/// é admissível perder.</param>
		/// <returns>A query que deve ser executada para retornar os dados do relatório</returns>
		/// <remarks></remarks>
		public string RelatListagemCalcular(System.DateTime pdtmData, cEnum.enumSetup pintSetup, string pstrPeriodo, bool pblnAlijamentoCalcular, decimal pdecCapitalTotal, decimal pdecPercentualManejo, decimal pdecPercentualRealizacaoParcial = -1, double pdblIFR2LimiteSuperior = -1, bool pblnAcimaMME49 = true, double pdblTitulosTotal = -1,
		Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1, cIFRSobrevendido pobjIFRSobrevendido = null)
		{
			string functionReturnValue = null;

			cCotacao objCotacao = new cCotacao(objConexao);

			System.DateTime dtmDataAtual = default(System.DateTime);
			System.DateTime dtmDataAnterior = default(System.DateTime);

			string strTabelaCotacao = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "COTACAO", "COTACAO_SEMANAL")
			string strTabelaMedia = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "MEDIA_DIARIA", "MEDIA_SEMANAL")
			string strTabelaIFR = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "IFR_DIARIO", "IFR_SEMANAL")

			//valor máximo que pode ser perdido utilizando o manejo.
			decimal decValorPerdaManejo = pdecCapitalTotal * pdecPercentualManejo / 100;

			cCalculadorTabelas.TabelasCalcular(pstrPeriodo, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

			//verifica se a data recebida é uma data de cotação


			if (pstrPeriodo == "DIARIO") {

				if (objCotacao.CotacaoDataExistir(pdtmData, strTabelaCotacao)) {
					//se a data recebida por parâmetro é a data de cotação, então utiliza esta data.
					dtmDataAtual = pdtmData;


				} else {
					//se a data recebida por parâmetro não é uma data de cotação, 
					//busca a primeira cotação com data anterior.
					dtmDataAtual = objCotacao.CotacaoAnteriorDataConsultar(pdtmData, strTabelaCotacao);

				}


			} else {
				dtmDataAtual = objCotacao.CotacaoSemanalPrimeiroDiaSemanaCalcular(pdtmData);

			}

			//verifica se o setup utiliza cotacao de data anterior. se utiliza tem que buscar a data anterior.
			//o único que não utiliza é o IFR 2 abaixo de 5.

			if (pintSetup != cEnum.enumSetup.IFRSemFiltro && pintSetup != cEnum.enumSetup.IFRSemFiltroRP) {

				if (pstrPeriodo == "DIARIO") {
					dtmDataAnterior = objCotacao.CotacaoAnteriorDataConsultar(dtmDataAtual, strTabelaCotacao);


				} else {
					//TEM QUE PASSAR COMO PARÂMETRO UMA DIA ANTES DA DATA ENCONTRADA COMO PRIMEIRO DIA DA SEMANA
					//MAIS ATUAL, POIS CASO CONTRÁRIO A FUNÇÃO RETORNARÁ A MESMA DATA.
					dtmDataAnterior = objCotacao.CotacaoSemanalPrimeiroDiaSemanaCalcular(dtmDataAtual.AddDays(-1));

				}

			}


			if (pstrPeriodo == "SEMANAL") {

				if (pdblTitulosTotal != -1 || pintNegociosTotal != -1 || pdecValorTotal != -1) {
					//Os filtros referentes ao volume negociado são informados em dias.
					//Quando as cotações são semanais, tem que verificar quantos dias tem na semana e multiplicar
					//os valores informados pelo número de dias
					cRS objRS = new cRS(objConexao);

					//Dim strQuery As String

					System.DateTime dtmCotacaoSemanalDataFinal = default(System.DateTime);

					//primeiro busca a data final da cotação semanal

					objRS.ExecuteQuery(" SELECT Max(datafinal) As datafinal" + " FROM cotacao_semanal " + " WHERE Data = " + FuncoesBD.CampoDateFormatar(dtmDataAtual));

					dtmCotacaoSemanalDataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));

					objRS.Fechar();

					objRS.ExecuteQuery(" SELECT Max (contador) As contador " + " FROM " + "(" + " SELECT codigo, Count(1) As contador " + " FROM cotacao " + " WHERE Data >= " + FuncoesBD.CampoDateFormatar(dtmDataAtual) + " And data <= " + FuncoesBD.CampoDateFormatar(dtmCotacaoSemanalDataFinal) + " GROUP BY codigo " + ")");


					if (pdblTitulosTotal != -1) {
						pdblTitulosTotal = pdblTitulosTotal * Convert.ToInt32(objRS.Field("contador"));

					}


					if (pintNegociosTotal != -1) {
						pintNegociosTotal = pintNegociosTotal * Convert.ToInt32(objRS.Field("contador"));

					}


					if (pdecValorTotal != -1) {
						pdecValorTotal = pdecValorTotal * Convert.ToInt32(objRS.Field("contador"));

					}

					objRS.Fechar();

				}

			}

			switch (pintSetup) {

				case cEnum.enumSetup.MM91:

					functionReturnValue = SetupMME91QueryGerar(dtmDataAnterior, dtmDataAtual, strTabelaCotacao, strTabelaMedia, pblnAlijamentoCalcular, pdecCapitalTotal, decValorPerdaManejo, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal,
					pdecPercentualRealizacaoParcial);

					break;
				case cEnum.enumSetup.MM92:

					functionReturnValue = SetupMME92QueryGerar(dtmDataAnterior, dtmDataAtual, strTabelaCotacao, strTabelaMedia, pblnAlijamentoCalcular, pdecCapitalTotal, decValorPerdaManejo, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal,
					pdecPercentualRealizacaoParcial);

					break;
				case cEnum.enumSetup.MM93:

					functionReturnValue = SetupMME93QueryGerar(dtmDataAnterior, dtmDataAtual, strTabelaCotacao, strTabelaMedia, pblnAlijamentoCalcular, pdecCapitalTotal, decValorPerdaManejo, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal,
					pdecPercentualRealizacaoParcial);

					break;
				case cEnum.enumSetup.IFRSemFiltro:

					functionReturnValue = SetupIFR2SemFiltroQueryGerar(dtmDataAtual, strTabelaCotacao, strTabelaMedia, strTabelaIFR, pblnAlijamentoCalcular, pblnAcimaMME49, pdecCapitalTotal, decValorPerdaManejo, pdblIFR2LimiteSuperior, pdblTitulosTotal,
					pintNegociosTotal, pdecValorTotal, pdecPercentualRealizacaoParcial);

					break;
				case cEnum.enumSetup.IFRComFiltro:

					functionReturnValue = SetupIFR2ComFiltroQueryGerar(dtmDataAnterior, dtmDataAtual, strTabelaCotacao, strTabelaMedia, strTabelaIFR, pblnAlijamentoCalcular, pblnAcimaMME49, pdecCapitalTotal, decValorPerdaManejo, pdblTitulosTotal,
					pintNegociosTotal, pdecValorTotal, pdecPercentualRealizacaoParcial);

					break;
				case cEnum.enumSetup.IFRSemFiltroRP:

					cCarregadorSetup objCarregorSetup = new cCarregadorSetup();

					Setup objSetup = objCarregorSetup.CarregaPorID(pintSetup);

					functionReturnValue = RelatIFR2SemFiltroDiarioPersonalizadoGerar(objSetup, dtmDataAtual, pdecCapitalTotal, decValorPerdaManejo, pobjIFRSobrevendido, pdblIFR2LimiteSuperior, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal);

					break;
				default:

					functionReturnValue = String.Empty;

					break;
			}
			return functionReturnValue;

		}




		/// <summary>
		/// Gera a query para descobrir a próxima data em que haverá cruzamento da mín
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pstrPeriodicidade"></param>
		/// <param name="pintMediaNumPeriodos"></param>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pdecValorStopAnterior"></param>
		/// <param name="pdtmDataStopRet"></param>
		/// <param name="pdecValorStopRet"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private string DataProximoCruzamentoMediaQueryGerar(string pstrCodigo, string pstrPeriodicidade, int pintMediaNumPeriodos, System.DateTime pdtmDataInicial, decimal pdecValorStopAnterior, ref DateTime pdtmDataStopRet, ref decimal pdecValorStopRet, System.DateTime pdtmDataFinal)
		{

			string strTabelaCotacao = String.Empty;
			string strTabelaMedia = String.Empty;

			//Busca o nome das tabelas
		    string pstrTabelaIFRRet = string.Empty;
		    cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia,ref pstrTabelaIFRRet);

			string strQuery = null;

			strQuery = "Select TOP 1 DIA2.DATA " + ", DIA2.VALORMINIMO - 0.03  As STOP_LOSS " + "FROM " + "(" + "SELECT SEQUENCIAL " + "FROM " + strTabelaCotacao + " C INNER JOIN " + strTabelaMedia + " M " + "On C.CODIGO = M.CODIGO " + "WHERE C.CODIGO = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + " And C.DATA = M.DATA " + " And M.TIPO = " + FuncoesBD.CampoStringFormatar("MME") + " And M.NUMPERIODOS = " + pintMediaNumPeriodos.ToString() + " And C.VALORFECHAMENTO >= M.VALOR " + " And C.DATA >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial);

			//se a data de stop final é uma data válida coloca o filtro no where

			if (pdtmDataFinal != frwInterface.cConst.DataInvalida) {
				strQuery = strQuery + " And C.DATA <= " + FuncoesBD.CampoDateFormatar(pdtmDataFinal);

			}

			strQuery = strQuery + " ) As DIA1 " + " INNER JOIN " + "(" + " SELECT SEQUENCIAL, C.DATA, VALORMINIMO " + " FROM " + strTabelaCotacao + " C INNER JOIN " + strTabelaMedia + " M " + " On C.CODIGO = M.CODIGO " + " And C.DATA = M.DATA " + " WHERE C.CODIGO = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + " And M.TIPO = " + FuncoesBD.CampoStringFormatar("MME") + " And M.NUMPERIODOS = " + pintMediaNumPeriodos.ToString() + " And C.VALORFECHAMENTO < M.VALOR " + " And C.DATA >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial) + " And C.VALORMINIMO - 0.03 > " + FuncoesBD.CampoDecimalFormatar(pdecValorStopAnterior);
			//stop tem que estar sempre acima do valor do stop anterior

			//se a data de stop final é uma data válida coloca o filtro no where

			if (pdtmDataFinal != frwInterface.cConst.DataInvalida) {
				strQuery = strQuery + " And C.DATA <= " + FuncoesBD.CampoDateFormatar(pdtmDataFinal);

			}

			strQuery = strQuery + ") As DIA2 " + " On DIA2.SEQUENCIAL - DIA1.SEQUENCIAL = 1 " + " ORDER BY DATA ";

			return strQuery;

		}


		/// <summary>
		/// Calcula a data e o valor do novo STOP LOSS quando a cotação fechar abaixo da média 
		/// de <paramref name=" pintMediaNumPeriodos"></paramref> períodos. O valor do stop loss será a mínima deste períodos
		/// menos 3 centavos.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrPeriodicidade">DIARIO ou SEMANAL</param>
		/// <param name="pintMediaNumPeriodos">número do períodos da média utilizada como base para indicar o STOP LOSS</param>
		/// <param name="pdtmDataInicial">Data inicial da busca. Será a data inicial da operação</param>
		/// <param name="pdecValorStopAnterior">Valor de stop anterior </param>
		/// <param name="pdtmDataFinal">Data limite para procurar novos stops. Essa data é informado quando já se tem uma data de stop
		/// inicial definida e estamos procurando outro stop até esta data para ver se é estopado primeiro que a data do stop inicial</param>
		/// <returns>
		/// TRUE - Se encontrou uma nova data para o rompimento de mínima entre a data inicial e a data final recebida por parâmetro
		/// FALSE - Se NÃO encontrou uma nova data para o rompimento de mínima entre a data inicial e a data final recebida por parâmetro
		/// </returns>
		/// <remarks></remarks>
		private bool DataProximoCruzamentoMediaCalcular(string pstrCodigo, string pstrPeriodicidade, int pintMediaNumPeriodos, System.DateTime pdtmDataInicial, decimal pdecValorStopAnterior, ref System.DateTime pdtmDataStopRet, ref decimal pdecValorStopRet, System.DateTime pdtmDataFinal)
		{
			bool functionReturnValue = false;

			cRS objRS = new cRS(objConexao);

			cRSList objRSSplit = null;
			bool blnSplitExistir = false;

			string strQuery = null;

			//inicializa o fator de conversão com 1.
			double dblSplitFatorAcumulado = 1;

			//indica se encontrou um cruzamento de média com preço.
			bool blnCruzamentoEncontrado = false;

			System.DateTime dtmDataInicial = pdtmDataInicial;
			System.DateTime dtmDataFinal = default(System.DateTime);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this.objConexao);

			//consulta os splits 
			blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial, "A",ref objRSSplit, pdtmDataFinal);


			if (blnSplitExistir) {
				//se tem splits tem que ir consultando as datas dos splits separadamente até chegar na data
				//final recebida por parâmetro ou encontrar um cruzamento.
				//Se um cruzamento for encontrado, os próximos splits não precisam ser procurados.

				while (!objRSSplit.EOF && !blnCruzamentoEncontrado) {
					//busca a data um dia anteriro à data do split
					dtmDataFinal = Convert.ToDateTime(objRSSplit.Field("Data")).AddDays(-1);

					//estamos sempre buscando as datas anteriores a data do registro corrente do split

					strQuery = DataProximoCruzamentoMediaQueryGerar(pstrCodigo, pstrPeriodicidade, pintMediaNumPeriodos, dtmDataInicial, pdecValorStopAnterior * (decimal) dblSplitFatorAcumulado, ref pdtmDataStopRet, ref pdecValorStopRet, dtmDataFinal);

					objRS.ExecuteQuery(strQuery);


					if (objRS.DadosExistir) {
						//caso tenha encontrado o cruzamento, marca variável para em seguida sair do loop.
						blnCruzamentoEncontrado = true;

						pdtmDataStopRet = Convert.ToDateTime(objRS.Field("DATA", frwInterface.cConst.DataInvalida));

						pdecValorStopRet = Convert.ToDecimal(objRS.Field("STOP_LOSS", 0));

						functionReturnValue = true;

					}

					//para a próxima iteração a data inicial é a data final desta iteração.
					dtmDataInicial = dtmDataFinal;

					//multiplica o fator para a próxima iteração.
					dblSplitFatorAcumulado = dblSplitFatorAcumulado * Convert.ToDouble(objRSSplit.Field("Razao"));

					//sempre tem que fechar o RS.
					objRS.Fechar();

					objRSSplit.MoveNext();

				}

				//se não encontrou ainda o cruzamento, tem que fazer a última busca que é fora do loop.
				//A busca ocorre desde o último split até a data final recebida por parâmetro ou então até
				//o final das cotações

				if (!blnCruzamentoEncontrado) {
					dtmDataFinal = pdtmDataFinal;

					strQuery = DataProximoCruzamentoMediaQueryGerar(pstrCodigo, pstrPeriodicidade, pintMediaNumPeriodos, dtmDataInicial, pdecValorStopAnterior, ref pdtmDataStopRet, ref pdecValorStopRet, pdtmDataFinal);

					objRS.ExecuteQuery(strQuery);

					pdtmDataStopRet = Convert.ToDateTime(objRS.Field("DATA", frwInterface.cConst.DataInvalida));

					pdecValorStopRet = Convert.ToDecimal(objRS.Field("STOP_LOSS", 0));

					functionReturnValue = objRS.DadosExistir;

					objRS.Fechar();

				}


			} else {
				//se não tem split faz o cálculo normal até a data final ou até o final das cotações, caso 
				//a data final não seja válida (não exista)
				strQuery = DataProximoCruzamentoMediaQueryGerar(pstrCodigo, pstrPeriodicidade, pintMediaNumPeriodos, pdtmDataInicial, pdecValorStopAnterior, ref pdtmDataStopRet, ref pdecValorStopRet, pdtmDataFinal);

				objRS.ExecuteQuery(strQuery);

				pdtmDataStopRet = Convert.ToDateTime(objRS.Field("DATA", frwInterface.cConst.DataInvalida));

				pdecValorStopRet = Convert.ToDecimal(objRS.Field("STOP_LOSS", 0));

				functionReturnValue = objRS.DadosExistir;

				objRS.Fechar();

			}
			return functionReturnValue;

		}

		/// <summary>
		/// Consulta o saldo atual da conta, imediatamente anterior à data recebida por parâmetro
		/// </summary>
		/// <param name="pdtmData">data base utilizada para buscar o saldo na primeira data anterior a esta</param>
		/// <returns>O saldo atual na data anterior à data recebida por parâmetro</returns>
		/// <remarks></remarks>
		private decimal TMP_CONTA_Saldo_Anterior_Consultar(long plngCod_Relatorio, System.DateTime pdtmData)
		{
			decimal functionReturnValue = default(decimal);

			cRS objRS = new cRS(objConexao);

			//PARA CONSULTA A DATA IMEDIATAMENTE ANTERIOR TEM QUE ORDENAR AS DATAS EM ORDEM DECRESCENTE (DESC).
			//ORDENA TAMBÉM POR SEQUENCIA DESC PARA O CASO DE HAVER MAIS DE UM REGISTRO NA MESMA DATA.
			objRS.ExecuteQuery(" SELECT TOP 1 Saldo_Atual " + " FROM tmp_conta " + " WHERE COD_RELATORIO = " + plngCod_Relatorio.ToString() + " AND Data < " + FuncoesBD.CampoDateFormatar(pdtmData) + " ORDER BY Data DESC, Sequencia DESC");

			functionReturnValue = Convert.ToDecimal(objRS.Field("Saldo_Atual", 0));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Consulta o saldo da conta na data recebida por parâmetro. Se não houver registro nesta data busca em data anterior.
		/// </summary>
		/// <param name="plngCod_Relatorio">Código do relatório que está sendo gerado</param>
		/// <param name="pdtmData">Data de consulta do saldo</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private decimal TMP_CONTA_Saldo_Atual_Consultar(long plngCod_Relatorio, System.DateTime pdtmData)
		{
			decimal functionReturnValue = default(decimal);

			cRS objRS = new cRS(objConexao);

			//PARA CONSULTA A DATA IMEDIATAMENTE ANTERIOR TEM QUE ORDENAR AS DATAS EM ORDEM DECRESCENTE (DESC)
			objRS.ExecuteQuery(" SELECT TOP 1 Data, Saldo_Atual " + " FROM tmp_conta " + " WHERE COD_RELATORIO = " + plngCod_Relatorio.ToString() + " AND Data <= " + FuncoesBD.CampoDateFormatar(pdtmData) + " ORDER BY Data DESC, Sequencia DESC");

			functionReturnValue = Convert.ToDecimal(objRS.Field("Saldo_Atual", 0));

			objRS.Fechar();
			return functionReturnValue;

		}

		private bool TMP_CONTA_Saldo_Recalcular(long plngCod_Relatorio, System.DateTime pdtmData)
		{

			cRS objRS = new cRS(objConexao);

			decimal decSaldoAnterior = 0;
		    long lngSequencia = 0;

			//verifica se existem movimentações com data maior que a data recebida por parâmetro
			objRS.ExecuteQuery("SELECT Data, Sequencia, Valor_Movimentado " + " FROM tmp_conta " + " WHERE cod_relatorio = " + plngCod_Relatorio.ToString() + " AND Data > " + FuncoesBD.CampoDateFormatar(pdtmData) + " ORDER BY Data, Sequencia");

			cCommand objCommand = new cCommand(objConexao);


			if (objRS.DadosExistir) {
				//para a primeira data do RS tem que buscar o saldo anterior.
				//para as demais dadas já vamos calculando o saldo dentro da operação.
				decSaldoAnterior = TMP_CONTA_Saldo_Anterior_Consultar(plngCod_Relatorio, Convert.ToDateTime(objRS.Field("Data")));

			}


			while (!objRS.EOF) {
				lngSequencia = Convert.ToInt64(objRS.Field("Sequencia"));

				decimal decValorMovimentado = Convert.ToDecimal(objRS.Field("Valor_Movimentado"));

				//atualiza o saldo, na data, somando o valor movimentado ao saldo anterior.

				objCommand.Execute("UPDATE TMP_CONTA SET " + "Saldo_Anterior = " + FuncoesBD.CampoDecimalFormatar(decSaldoAnterior) + ", Saldo_Atual = " + FuncoesBD.CampoDecimalFormatar(decSaldoAnterior + decValorMovimentado) + " WHERE cod_relatorio = " + plngCod_Relatorio.ToString() + " AND Data = " + FuncoesBD.CampoDateFormatar(Convert.ToDateTime(objRS.Field("Data"))) + " AND Sequencia = " + lngSequencia.ToString());

				//atualiza o saldo anterior na variável para a próxima iteração.
				//com isso não precisaremos consultar o saldo anterior no banco de dados novamente.
				decSaldoAnterior = decSaldoAnterior + decValorMovimentado;

				objRS.MoveNext();

			}

			objRS.Fechar();

			return objCommand.TransStatus;

		}


		/// <summary>
		/// Atualiza o saldo da conta, inserindo um novo registro na data recebida por parâmetro.
		/// </summary>
		/// <param name="plngCod_Relatorio">Código agrupador de todas as operações</param>
		/// <param name="pdtmData">Data da movimentação</param>
		/// <param name="pdecValorMovimentado">Valor da movimentação que será registrado nesta operação</param>
		/// <returns>status da transação</returns>
		/// <remarks></remarks>
		private bool TMP_CONTA_Saldo_Inserir(long plngCod_Relatorio, System.DateTime pdtmData, decimal pdecValorMovimentado)
		{

			cCommand objCommand = new cCommand(objConexao);

			decimal decSaldoAnterior = default(decimal);

			decSaldoAnterior = TMP_CONTA_Saldo_Atual_Consultar(plngCod_Relatorio, pdtmData);

			objCommand.Execute(" INSERT INTO TMP_CONTA " + "(cod_relatorio, Data, Saldo_Anterior, Valor_Movimentado, Saldo_Atual) " + " VALUES " + "(" + plngCod_Relatorio.ToString() + ", " + FuncoesBD.CampoDateFormatar(pdtmData) + ", " + FuncoesBD.CampoDecimalFormatar(decSaldoAnterior) + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorMovimentado) + ", " + FuncoesBD.CampoDecimalFormatar(decSaldoAnterior + pdecValorMovimentado) + ")");

			//chama a função que recalcula o saldo nas movimentações com data posterior à data em que foi inserida a movimentação.
			//Se não existirem movimentações em datas posteriores nada será executado.
			TMP_CONTA_Saldo_Recalcular(plngCod_Relatorio, pdtmData);

			return objCommand.TransStatus;

		}

		/// <summary>
		/// Gera a query que busca a data de acionamento de um stop.
		/// </summary>
		/// <param name="pstrCodigo">Código do papel</param>
		/// <param name="pdecValorStop">Valor do stop. Se a cotação atingir este valor ou inferior, significa que foi estopado</param>
		/// <param name="pdtmDataInicial">Data inicial para começar a procurar o pregão em que ocorrerá o acionamento do stop</param>
		/// <param name="pdtmDataFinal">Quando for recebida, indica uma data limite para fazer a busca se o stop será acionado.
		/// Esta data será passada geralmente quando houver tratamento de splits.</param>
		/// <returns>A query que será utilizada para fazer a busca no banco de dados</returns>
		/// <remarks></remarks>
		private string StopAcionamentoQueryGerar(string pstrCodigo, decimal pdecValorStop, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal)
		{

			string strQuery = null;

			//para verificar em qual data ocorrerá utiliza o gráfico diário, porque pode ocorrer em qualquer dia da semana.
			//busca a menor data em que atingir valores menores ou iguais ao valor de stop informado. 
			//Utiliza valores menores que o stop informado porque pode ocorrer um gap e o mercado já abrir abaixo do stop.
			//Se ocorrer este gap, será considerado que a venda será no valor de ABERTURA desta data, pois no momento que abrir abaixo
			//do stop será considerado que o stop será disparado a valor de mercado.
			strQuery = " SELECT MIN(Data) as Data " + " FROM Cotacao " + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + " AND Data >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial) + " AND ValorMinimo <= " + FuncoesBD.CampoDecimalFormatar(pdecValorStop);


			if (pdtmDataFinal != frwInterface.cConst.DataInvalida) {
				strQuery = strQuery + " AND Data <= " + FuncoesBD.CampoDateFormatar(pdtmDataFinal);

			}

			return strQuery;

		}

		/// <summary>
		/// Calcula a data em que ocorrerá o acionamento de um stop.
		/// </summary>
		/// <param name="pstrCodigo">Código do papel</param>
		/// <param name="pdecValorStop">Valor do stop. Se a cotação atingir este valor ou inferior, significa que foi estopado</param>
		/// <param name="pdtmDataInicial">Data inicial para começar a procurar o pregão em que ocorrerá o acionamento do stop</param>
		/// <returns>A data em que ocorrer o acionamento do stop</returns>
		/// <remarks></remarks>
		private System.DateTime StopAcionamentoDataCalcular(string pstrCodigo, decimal pdecValorStop, System.DateTime pdtmDataInicial)
		{
			System.DateTime functionReturnValue = default(System.DateTime);

			cRSList objRSSplit = null;
			bool blnSplitExistir = false;

			cRS objRS = new cRS(objConexao);

			string strQuery = null;

			//indica se encontrou um acionamento para o split.
			bool blnAcionamentoEncontrado = false;

			System.DateTime dtmDataInicial = pdtmDataInicial;
			System.DateTime dtmDataFinal = default(System.DateTime);

			double dblSplitFatorAcumulado = 1;

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this.objConexao);

			//consulta os splits 
			blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial, "A", ref objRSSplit, cConst.DataInvalida);


			if (blnSplitExistir) {
				dtmDataInicial = pdtmDataInicial;


				while (!objRSSplit.EOF && !blnAcionamentoEncontrado) {
					//a data final é sempre um dia antes da data do split

					dtmDataFinal = Convert.ToDateTime(objRSSplit.Field("Data")).AddDays(-1);

					//estamos sempre buscando as datas anteriores a data do registro corrente do split

					strQuery = StopAcionamentoQueryGerar(pstrCodigo, pdecValorStop * (decimal) dblSplitFatorAcumulado, dtmDataInicial, dtmDataFinal);

					objRS.ExecuteQuery(strQuery);

					//Não pode utilizar a função DadosExistir, pois quando é utilizada a função de agregação MIN,
					//sempre retorna uma linha, mesmo que não sejam encontrados dados. Neste caso a propriedade
					//DadosExistir sempre retorna TRUE.
					//If objRS.DadosExistir Then


					if (Convert.ToDateTime(objRS.Field("Data", frwInterface.cConst.DataInvalida)) != frwInterface.cConst.DataInvalida) {
						//caso tenha encontrado o acionamento, marca variável para em seguida sair do loop.
						blnAcionamentoEncontrado = true;

						functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

					}

					//para a próxima iteração a data inicial é a data final desta iteração.
					dtmDataInicial = dtmDataFinal;

					//multiplica o fator para a próxima iteração.
					dblSplitFatorAcumulado = dblSplitFatorAcumulado * Convert.ToDouble(objRSSplit.Field("Razao"));

					//sempre tem que fechar o RS.
					objRS.Fechar();

					objRSSplit.MoveNext();

				}

				//se não encontrou ainda o acionamento, tem que fazer a última busca que é fora do loop.
				//A busca ocorre desde o último split até a data final recebida por parâmetro ou então até
				//o final das cotações

				if (!blnAcionamentoEncontrado) {
					strQuery = StopAcionamentoQueryGerar(pstrCodigo, pdecValorStop * (decimal) dblSplitFatorAcumulado, dtmDataInicial,cConst.DataInvalida);

					objRS.ExecuteQuery(strQuery);


					if (objRS.DadosExistir) {
					    functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

					}

					objRS.Fechar();

				}


			} else {
				strQuery = StopAcionamentoQueryGerar(pstrCodigo, pdecValorStop, pdtmDataInicial,cConst.DataInvalida);

				objRS.ExecuteQuery(strQuery);

				functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

				objRS.Fechar();

			}
			return functionReturnValue;

		}

		/// <summary>
		/// Gera a query para calcular a data de realização parcial em função do tipo de realização parcial 
		/// e da periodicidade da operação.
		/// </summary>
		/// <param name="pstrCodigo">Código do papel</param>
		/// <param name="pintRealizacaoParcialTipo">alijamento de risco, percentual fixo ou primeiro fechamento com lucro</param>
		/// <param name="pstrPeriodicidade">DIARIO ou SEMANAL</param>
		/// <param name="pblnRealizacaoParcialPermitirDayTrade">Indica se a operação pode ser realizada na mesma data em que foi efetuada a compra</param>
		/// <param name="pdtmDataInicial">Data inicial de busca da realização parcial na tabela de cotações</param>
		/// <param name="pdecValorRealizacaoParcial">Valor da realização parcial, que já vem calculado em função do valor de entrada
		/// e do tipo de realização escolhido</param>
		/// <param name="pstrTabelaCotacao">Indica a tabela em que devem ser buscadas as cotações. Utilizada somente no tipo de realização
		/// parcial no primeiro fechamento com lucro, pois as demais sempre utilizam a cotação diária (tabela COTACAO). Os valores possíves
		/// são COTACAO ou COTACAO_SEMANAL</param>
		/// <param name="pdtmDataFinal">Data final de busca da realização parcial na tabela de cotações</param>
		/// <returns>String contendo a query que deve ser executada</returns>
		/// <remarks></remarks>
		private string RealizacaoParcialQueryGerar(string pstrCodigo, cEnum.enumRealizacaoParcialTipo pintRealizacaoParcialTipo, string pstrPeriodicidade, bool pblnRealizacaoParcialPermitirDayTrade, System.DateTime pdtmDataInicial, decimal pdecValorRealizacaoParcial, string pstrTabelaCotacao, System.DateTime pdtmDataFinal)
		{

			string strQuery = null;


			if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.AlijamentoRisco || pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PercentualFixo) {
				//*********MONTA QUERY PARA A REALIZAÇÃO PARCIAL COM ALIJAMENTO DE RISCO OU COM PERCENTUAL FIXO*******

				//Para estes dois tipos de realização temos que usar a tabela de cotações diárias, 
				//pois a realização pode acontecer em qualquer dia da semana.

				//se tem realização parcial, busca a data da realização parcial de acordo com o tipo de realização
				strQuery = "SELECT MIN(Data) AS Data " + " FROM Cotacao " + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo);


				if (pblnRealizacaoParcialPermitirDayTrade) {
					//SE PERMITE DAY TRADE, A DATA DE REALIZAÇÃO PODE SER A MESMA DATA DE ENTRADA,
					//OU ENTÃO UMA DATA POSTERIOR
					strQuery = strQuery + " AND Data >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial);


				} else {
					//SE NÃO PERMITE DAY TRADE NA REALIZAÇÃO PARCIAL, ENTÃO A DATA TEM QUE SER 
					//OBRIGATORIAMENTE UMA DATA POSTERIOR À DATA DE ENTRADA
					strQuery = strQuery + " AND Data > " + FuncoesBD.CampoDateFormatar(pdtmDataInicial);

				}


				if (pdtmDataFinal != frwInterface.cConst.DataInvalida) {
					strQuery = strQuery + " AND Data <= " + FuncoesBD.CampoDateFormatar(pdtmDataFinal);

				}

				strQuery = strQuery + " AND ValorMaximo >= " + FuncoesBD.CampoDecimalFormatar(pdecValorRealizacaoParcial);


			} else if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PrimeiroLucro) {
				//se a realização for no primeiro fechamento com lucro, tem que utilizar a tabela
				//de cotações diária ou semanal, conforme a periodicidade do gráfico.


				if (pstrPeriodicidade == "DIARIO") {
					strQuery = "SELECT MIN(Data) AS Data ";


				} else {
					//quando a periodicidade é "SEMANAL" tem que buscar a data final, que é a data de realização
					//e a data inicial para buscar os demais dados da cotação
					strQuery = "SELECT MIN(Data) AS Data, MIN(DataFinal) AS DataFinal ";

				}

				strQuery = strQuery + " FROM " + pstrTabelaCotacao + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo);


				if (pblnRealizacaoParcialPermitirDayTrade) {
					//SE PERMITE DAY TRADE, A DATA DE REALIZAÇÃO PODE SER A MESMA DATA DE ENTRADA,
					//OU ENTÃO UMA DATA POSTERIOR
					strQuery = strQuery + " AND Data >= " + FuncoesBD.CampoDateFormatar(pdtmDataInicial);


				} else {
					//SE NÃO PERMITE DAY TRADE NA REALIZAÇÃO PARCIAL, ENTÃO A DATA TEM QUE SER 
					//OBRIGATORIAMENTE UMA DATA POSTERIOR À DATA DE ENTRADA.

					//PARA OS CASOS DE PERIODICIDADE "SEMANAL", CONSIDERA A DATA FINAL, POIS O FECHAMENTO
					//SEMPRE ACONTECE NA DATA FINAL
					strQuery = strQuery + " AND " + (pstrPeriodicidade == "DIARIO" ? "Data" : "DataFinal") + " > " + FuncoesBD.CampoDateFormatar(pdtmDataInicial);


					if (pdtmDataFinal != frwInterface.cConst.DataInvalida) {
						//busca as datas de realização sempre anteriores à data do stop, 
						//pois se ocorrerem depois não adianta, pois a operação será estopada antes.
						//essa restrição de datas também vai melhorar o tempo de resposta da query,
						//pois o número de registros a serem verificados será menor
						strQuery = strQuery + " AND " + (pstrPeriodicidade == "DIARIO" ? "Data" : "DataFinal") + " < " + FuncoesBD.CampoDateFormatar(pdtmDataFinal);

					}

					//para a realização no primeiro fechamento com lucro, tem que buscar o fechamento
					//em que o percentual entre o valor de fechamento e o valor de entrada na operação
					//for maior ou igual ao percentual configurado
					strQuery = strQuery + " AND ValorFechamento >= " + FuncoesBD.CampoDecimalFormatar(pdecValorRealizacaoParcial);

				}


			} else {
				strQuery = String.Empty;

			}

			return strQuery;

		}

		/// <summary>
		/// Executa uma operação no mercado, se houver saldo na conta.
		/// </summary>
		/// <param name="pstrCodigo">Código do papel para o qual será feita a operação</param>
		/// <param name="pstrPeriodicidade">Periodicidade utilizada na operação: DIARIO ou SEMANAL</param>
		/// <param name="pdtmDataEntrada">Data em que será executada a operação.</param>
		/// <param name="pdecValorEntrada">Valor que será pago ao realizar a compra</param>
		/// <param name="pdecValorStopInicial">Valor do STOP inicial da operação, que é o stop calculado no momento de fazer a compra</param>
		/// <param name="plngCodigoRelatorio">Código único utilizado para identificar todas as operações do relatório</param>
		/// <param name="pintRealizacaoParcialTipo">tipo de realização parcial: sem realização, primeiro candle com lucro acima de x %,
		/// percentual fixo, alijamento do risco (valor do stop inicial plotado para cima)</param> 
		///<param name="pblnRealizacaoParcialPermitirDayTrade">Quando houver realização parcial, indica se é permitida a realização parcial
		/// no mesmo dia da data de entrada</param>
		/// <param name="pintStopViradaMediaNumPeriodos">Número de períodos considerados para calcular o stop pela virada da média.
		/// Quando o valor de fechamento ficar abaixo da média deste número X de períodos o stop deve subir para a mínima deste dia
		/// subtraído de 3 centavos.</param>
		/// <param name="pdecPercentualRealizacaoParcialFixo">quando o tipo de realização for por percentual fixo, indica qual é este percentual</param>
		/// <param name="pdecPrimeiroLucroPercentualMinimo">quando o tipo de realização parcial for o primeiro lucro, indica o percentual
		/// mínimo que deve ser considerado para fazer a realização parcial.</param>
		/// <param name="pintNumAcoesLote">Indica a quantidade de ações que compõem um lote mínimo do papel.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool OperacaoExecutar(string pstrCodigo, string pstrPeriodicidade, System.DateTime pdtmDataEntrada, decimal pdecValorEntrada, decimal pdecValorStopInicial, long plngCodigoRelatorio, cEnum.enumRealizacaoParcialTipo pintRealizacaoParcialTipo, bool pblnRealizacaoParcialPermitirDayTrade, int pintStopViradaMediaNumPeriodos, decimal pdecPercentualRealizacaoParcialFixo = 0,
		decimal pdecPrimeiroLucroPercentualMinimo = 0, int pintNumAcoesLote = 100)
		{

			//CONTROLE DE COMANDOS: A TRANSAÇÃO COMEÇA FORA DESTA FUNÇÃO. UTILIZA A CONEXÃO PRINCIPAL DO FRAMEWORK
			cCommand objCommand = new cCommand(objConexao);

			cRS objRS = new cRS(objConexao);

			string strQuery = null;

			cCotacao objCotacao = new cCotacao(objConexao);
			cCalculadorData objCalculadorData = new cCalculadorData(this.objConexao);

			//utilizado na busca dos splits durante o período de duração da operação
			cRSList objRSSplit = null;

			//TABELA UTILIZADA PARA ARMAZENAR OS DADOS DA OPERAÇÃO
			string strTabelaBackTesting = null;

			if (pstrPeriodicidade == "DIARIO") {
				strTabelaBackTesting = "RELAT_BACKTESTING_DIARIO";
			} else if (pstrPeriodicidade == "SEMANAL") {
				strTabelaBackTesting = "RELAT_BACKTESTING_SEMANAL";
			} else {
				strTabelaBackTesting = String.Empty;
			}

			string strTabelaCotacao = null;

			if (pstrPeriodicidade == "DIARIO") {
				strTabelaCotacao = "COTACAO";
			} else if (pstrPeriodicidade == "SEMANAL") {
				strTabelaCotacao = "COTACAO_SEMANAL";
			} else {
				strTabelaCotacao = String.Empty;
			}

			//SALDO DA CONTA UTILIZADA NA OPERAÇÃO.
			decimal decSaldo = default(decimal);

			//data em que ocorre a realização parcial
			System.DateTime dtmDataRealizacaoParcial = cConst.DataInvalida;

			//valor da realização parcial
			decimal decValorRealizacaoParcial = default(decimal);

			//valor da realização parcial convertido pelos splits quando houver.
			decimal decValorRealizacaoParcialAux = 0;

			//é imprescindivel inicializar com 0 porque não sabemos se haverá realização parcial
			int intQuantidadeRealizacaoParcial = 0;

			//quantidade de ações compradas na operação inicial 
			int intQuantidadeEntrada = 0;

			decimal decPercentualRealizacaoParcial = default(decimal);

			decimal decPercentualSaida = default(decimal);

			decimal decValorFinalOperacao = default(decimal);

			decimal decPercentualFinalOperacao = default(decimal);

			//data em que ocorre a saída total da operação.
			//Dim dtmDataSaida As Date

			//DATA EM QUE A COTAÇÃO ATINGIRÁ O VALOR DO STOP INICIAL
			//Dim dtmDataStopInicial As Date

			//DATA INICIAL UTILIZADA NOS CÁLCULOS DE VIRADA DE MÉDIA.
			//BUSCA AS DATAS SEMPRE A PARTIR DESTA.
			System.DateTime dtmViradaMediaDataInicial = default(System.DateTime);

			//DATA EM QUE OCORRERÁ A VIRADA DA MÉDIA. NÃO É A DATA EM QUE OCORRE O STOP E SIM A DATA EM QUE
			//A MÉDIA VIROU, GERANDO UM NOVO VALOR DE STOP LOSS
			System.DateTime dtmDataViradaMedia = default(System.DateTime);

			//DATA EM QUE A OPERAÇÃO SERÁ ESTOPADA PELA VIRADA DA MÉDIA
			System.DateTime dtmDataStopViradaMedia = default(System.DateTime);

			//VALOR DO STOP QUANDO OCORRER A VIRADA DA MÉDIA. 
			//O VALOR DO STOP É SEMPRE 3 CENTAVOS ABAIXO DA MÍNIMA DO CANDLE EM QUE OCORRER A VIRADA DA MÉDIA.
			decimal decValorStopViradaMedia = default(decimal);

			//DATA EM QUE REALMENTE OCORRERÁ O STOP. PODE SER TANTO PELO STOP INICIAL
			//COMO PELA VIRADA DA MÉDIA
			System.DateTime dtmDataStopReal = frwInterface.cConst.DataInvalida;

			//DATA EM QUE OCORRERÁ O ACIONAMENTO DO STOP NO VALOR DO PARÂMETRO "pdecValorStopInicial"
			System.DateTime dtmDataAcionamentoStopInicial = default(System.DateTime);

			//variável de controle que indica se o stop foi ou não acionado.
			bool blnStopCalculado = false;

			//variável de controle que indica se a realização parcial ´foi calculada.
			//Dim blnRealizacaoParcialCalculada As Boolean = False

			//variável de controle que indica que todo o período de cotações já foi percorrido.
			bool blnPeriodoTotalPercorrido = false;

			//variáveis auxiliares utilizadas para consultar dados da tabela de cotação.
			System.DateTime dtmDataPrimeiroDiaAux = default(System.DateTime);
			decimal decValorAberturaAux = default(decimal);
			//Dim decValorFechamentoAux As Decimal
			//Dim decValorMinimoAux As Decimal
			//Dim decValorMaximoAux As Decimal

			int intNumLotes = 0;
			int intNumAcoesEMCARTEIRA = 0;
			double dblNumAcoesEMCARTEIRAAux = 0;
			//utilizada para o cálculo do novo número de ações em carteira quando há splits.
			decimal decValorFechamentoAux = default(decimal);
			//utilizado na busca do valor de fechamento de uma cotação.

			//data final utilizada nas buscas de stop e realização parcial.
			System.DateTime dtmPeriodoDataFinal = default(System.DateTime);

			//contém a data final de busca para a realização parcial
			System.DateTime dtmRealizacaoParcialDataFinalBusca = default(System.DateTime);

			//variável auxiliar utilizada na busca da data de realização parcial.
			//inicializa com a data de entrada.
			//Se houver splits na operação a data inicial vai ser a data de cada split.
			System.DateTime dtmRealizacaoParcialDataInicial = pdtmDataEntrada;

			//variável auxiliar utilizada em loop.
			bool blnDataEncontradaAux = false;

			//Data máxima em que há cotação para o papel.
			System.DateTime dtmCotacaoDataMaxima = default(System.DateTime);

			//valor de entrada corrigido pelos splits.
			//inicializa atribuindo o valor de entrada. Se houver splits vai corrigindo o valor.
			decimal decValorEntradaAux = pdecValorEntrada;

			//consulta o saldo da conta para saber se pode executar a operação.
			decSaldo = TMP_CONTA_Saldo_Anterior_Consultar(plngCodigoRelatorio, pdtmDataEntrada);

			//calcula quantos lotes de ações pode comprar.
			intNumLotes = (int) Truncar((double) (decSaldo / pdecValorEntrada / pintNumAcoesLote));

			//verifica se pode comprar pelo menos um lote

			if (intNumLotes >= 1) {
				//calcula o número exato de ações pode comprar.
				//para obter este valor, multiplica o número de lotes pelo nº de ações que compõem um lote.
				intNumAcoesEMCARTEIRA = intNumLotes * pintNumAcoesLote;

				//variável que armazena a quantidade de entrada inicial para depois lançar no registro da operação
				intQuantidadeEntrada = intNumAcoesEMCARTEIRA;

				//atualiza o saldo da conta considerando a entrada na operação.
				//é uma movimentação negativa porque está saindo dinheiro da conta para comprar ações
				TMP_CONTA_Saldo_Inserir(plngCodigoRelatorio, pdtmDataEntrada, 0 - intNumAcoesEMCARTEIRA * pdecValorEntrada);

				//chama função que faz o recálculo da movimentação, caso já existam outra movimentações 
				//na mesma data ou em data posterior.
				//se nao existir movimentações, a função "TMP_CONTA_Saldo_Recalcular" não faz nada.
				//TMP_CONTA_Saldo_Recalcular(plngCodigoRelatorio, pdtmDataEntrada)

				//***************VERIFICA EM QUAL DATA OCORRERÁ O STOP**********************

				//inicializa a data inicial de busca da virada da média com a data de entrada na operação
				dtmViradaMediaDataInicial = pdtmDataEntrada;

				//calcula a data de acionamento do stop pelo valor do stop inicial da operação.
				dtmDataAcionamentoStopInicial = StopAcionamentoDataCalcular(pstrCodigo, pdecValorStopInicial, dtmViradaMediaDataInicial);

				cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this.objConexao);

				//busca os splits em ordem crescente até a data de acionamento do stop inicial da operação
				objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataEntrada, "A", ref objRSSplit, dtmDataAcionamentoStopInicial);


				//****calcula o valor da realização parcial
				if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.AlijamentoRisco) {
					//soma na entrada o valor que pagaria de stop
					//decValorRealizacaoParcial = pdecValorEntrada + (pdecValorEntrada - pdecValorStopInicial)
					decValorRealizacaoParcial = decValorEntradaAux + (decValorEntradaAux - pdecValorStopInicial);
				} else if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PercentualFixo) {
					//decValorRealizacaoParcial = pdecValorEntrada * (100 + pdecPercentualRealizacaoParcialFixo) / 100
					decValorRealizacaoParcial = decValorEntradaAux * (100 + pdecPercentualRealizacaoParcialFixo) / 100;
				} else {
					//primeiro lucro. Neste caso calcula o valor mínimo pelo qual permite realizar
					//decValorRealizacaoParcial = pdecValorEntrada * (100 + pdecPrimeiroLucroPercentualMinimo) / 100
					decValorRealizacaoParcial = decValorEntradaAux * (100 + pdecPrimeiroLucroPercentualMinimo) / 100;
				}

				//While Not blnPeriodoTotalPercorrido And (Not blnStopCalculado Or Not blnRealizacaoParcialCalculada)

				while (!blnPeriodoTotalPercorrido && (!blnStopCalculado)) {

					if (objRSSplit.EOF) {

						if (dtmDataAcionamentoStopInicial != frwInterface.cConst.DataInvalida) {
							//Se encontrou uma data para o acionamento do stop inicial.

							//se já percorreu todo o RS de splits, então a data final é a data de acionamento do stop inicial
							dtmPeriodoDataFinal = dtmDataAcionamentoStopInicial.AddDays(-1);

						//comentado porque foi removido para fora do loop já que tem sempre o mesmo comportamento.
						//****a data final de busca para realização parcial é um dia anterior ao stop
						//dtmRealizacaoParcialDataFinalBusca = DateAdd(DateInterval.Day, -1, dtmDataAcionamentoStopInicial)



						} else {
							//se não encontrou uma data de acionamento para o stop inicial, 
							//busca a data máxima que há cotação para o papel para definir como data limite de busca
							dtmCotacaoDataMaxima = objCalculadorData.CotacaoDataMaximaConsultar(pstrCodigo, strTabelaCotacao);

							dtmPeriodoDataFinal = dtmCotacaoDataMaxima;

							//****comentado porque foi removido para fora do loop já que tem sempre o mesmo comportamento.
							//dtmRealizacaoParcialDataFinalBusca = dtmCotacaoDataMaxima

						}

						//se já percorreu todo o RS de splits marca a variavel para que na próxima iteração saia do loop
						blnPeriodoTotalPercorrido = true;


					} else {
						//Se ainda não percorreu todos os RS dos splits...

						if (dtmDataAcionamentoStopInicial != frwInterface.cConst.DataInvalida) {

							if (dtmDataAcionamentoStopInicial < Convert.ToDateTime(objRSSplit.Field("Data")).AddDays(-1)) {
								//se a data de acionamento do stop inicial é anterior ao split,
								//o período final é um dia antes do stop inicial.
                                dtmPeriodoDataFinal = dtmDataAcionamentoStopInicial.AddDays(-1);


							} else {
								//se a data do stop inicial é posterior a data do split, então o período final é um
								//dia antes do split.
								dtmPeriodoDataFinal = Convert.ToDateTime(objRSSplit.Field("Data")).AddDays(-1);

							}


						} else {
							//Entra aqui se não encontrou data de acionamento para o stop inicial.
							//se ainda esta percorrendo os splits, a data final é um dia anterior ao split
							dtmPeriodoDataFinal = Convert.ToDateTime(objRSSplit.Field("Data")).AddDays(-1);

						}

						//no caso de estar percorrendo os splits a data final de busca da realização parcial também
						//é um dia anterior à data do split.
						//Esta data pode ser alterado se a data do stop real for encontrado. Neste caso a data de realização 
						//parcial será um dia antes da data do stop

					}
					// If objRSSplit.EOF Then

					//atribuição da data final de busca da realização parcial.
					dtmRealizacaoParcialDataFinalBusca = dtmPeriodoDataFinal;

					//inicializa com true, para entrar no loop na primeira iteração.
					//caso não encontre data, retorna false.
					blnDataEncontradaAux = true;

					//While (blnDataEncontradaAux) And (dtmDataStopReal = DataInvalida)
					//While (blnDataEncontradaAux) And (Not blnStopCalculado)
					//ATENÇÃO: Quando a variável blnStopCalculado = TRUE, não pode sair do loop,
					//porque pode haver uma data anterior que faça com que a operação seja estopada primeiro.
					//Só pode sair do loop quando não forem encontradas mais datas anteriores à data previamente encontrada

					while ((blnDataEncontradaAux)) {
						//vai calculando a data em que ocorrerá stop pela virada da média e comparando com a data do stop inicial

						//busca a data em que a operação será estopada pela virada da média. 

						//tem que ser uma data anterior à data em que ocorrera o stop inicial (dtmDataStopInicial). Se ocorrer depois não interessa,
						//pois já seria estopado primeiro.

						//Para isso, chama função que busca a primeira data em que o valor de fechamento da cotação 
						//cruza a média para baixo. A função traz também o novo valor do stop loos. 
						//A data final para esta busca é a data do stop real, ou seja, a data que o stop anterior
						//foi acionado
						blnDataEncontradaAux = DataProximoCruzamentoMediaCalcular(pstrCodigo, pstrPeriodicidade, pintStopViradaMediaNumPeriodos, dtmViradaMediaDataInicial, pdecValorStopInicial, ref dtmDataViradaMedia, ref decValorStopViradaMedia, dtmPeriodoDataFinal);


						if (blnDataEncontradaAux) {
							//a nova data inicial de virada da média que será utilizada nas próximas buscas 
							//é a data de cruzamento calculada pela função adicionado de 1 dia, para não ocorrer de na 
							//próxima iteração buscar a mesma data
							dtmViradaMediaDataInicial = dtmDataViradaMedia.AddDays(1);

							//atribui na variável de stop inicial utilizada no loop para que na próxima iteração,
							//busque stops acima deste preço.
							pdecValorStopInicial = decValorStopViradaMedia;

							//busca a data em que o stop loss calculado pela função chamada anteriormente será acionado
							//Passa a data de virada da média como data inicial para essa busca
							dtmDataStopViradaMedia = StopAcionamentoDataCalcular(pstrCodigo, decValorStopViradaMedia, dtmViradaMediaDataInicial);


							if (dtmDataStopViradaMedia != cConst.DataInvalida) {
								//se encontrou a data de acionamento do stop, então atribui na variável.
								//Esta data será sempre anterior a data de stop real anterior.
								dtmDataStopReal = dtmDataStopViradaMedia;

								//QUANDO encontra uma data de acionamento de stop ajusta o periodo final de busca de stops
								//para o dia anterior à esta data, para que na próxima iteração seja feita a busca apenas até este período.
								//A execução irá sair do loop quando não forem encontradas mais datas.
								dtmPeriodoDataFinal = dtmDataStopReal.AddDays(-1);

								blnStopCalculado = true;

							}

							//Else

							//    'se não encontrou um novo rompimento então acabou a busca pelo stop.
							//    'o mesmo foi encontrado no passo anterior.
							//    blnStopCalculado = True

						}

					}
					//While blnDataEncontradaAux


					if (!blnStopCalculado) {
						//se não encontrou stop no loop, então o stop será na data do acionamento do stop inicial da operação.
						//se o stop inicial da operação não ocorrer a variável será preenchida com a constante "DataInvalida"
						dtmDataStopReal = dtmDataAcionamentoStopInicial;


						if (dtmDataAcionamentoStopInicial != cConst.DataInvalida) {

							if (dtmDataAcionamentoStopInicial <= dtmPeriodoDataFinal) {
								//se a data de acionamento do stop inicial for anterior ou igual à data final do período.
								//marca que o stop foi encontrado
								blnStopCalculado = true;

							}

						}

					}


					//****************FIM DO CÁLCULO DA DATA DO STOP*******************

					//****************INÍCIO DO TRATAMENTO PARA A REALIZAÇÃO PARCIAL*******************

					//só haverá realização parcial se o parâmetro estiver configurado e se houver mais do que um lote na operação,

					if ((pintRealizacaoParcialTipo != cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial) && (intNumLotes >= 2)) {
						//Se a data em que o stop real será acionado já é conhecida (dtmDataStopReal <> DataInvalida),
						//esta é a data limite para buscar a realização parcial. Caso esta data ainda não tenha sido
						//conhecida a data máxima para buscar a realização parcial está na variável dtmPeriodoDataFinal.

						if (dtmDataStopReal != cConst.DataInvalida) {
							dtmRealizacaoParcialDataFinalBusca = dtmDataStopReal.AddDays(-1);

						}


						//*****CHAMAR AQUI FUNÇÃO QUE CALCULA A DATA DE REALIZAÇÃO PARCIAL
						strQuery = RealizacaoParcialQueryGerar(pstrCodigo, pintRealizacaoParcialTipo, pstrPeriodicidade, pblnRealizacaoParcialPermitirDayTrade, dtmRealizacaoParcialDataInicial, decValorRealizacaoParcial, strTabelaCotacao, dtmRealizacaoParcialDataFinalBusca);

						//executa a query que busca a data de realização parcial
						objRS.ExecuteQuery(strQuery);


						if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PrimeiroLucro) {
							dtmDataPrimeiroDiaAux = Convert.ToDateTime(objRS.Field("Data", cConst.DataInvalida));

							if (pstrPeriodicidade == "DIARIO") {
								dtmDataRealizacaoParcial = Convert.ToDateTime(objRS.Field("Data", cConst.DataInvalida));
							} else if (pstrPeriodicidade == "SEMANAL") {
								dtmDataRealizacaoParcial = Convert.ToDateTime(objRS.Field("DataFinal", cConst.DataInvalida));
							}


						} else {
							dtmDataRealizacaoParcial = Convert.ToDateTime(objRS.Field("Data", cConst.DataInvalida));

						}

						objRS.Fechar();

						//se a data do stop inicial for maior que a data de realização parcial, faz a realização parcial,
						//caso contrário não faz nada porque a operação será estopada antes mesmo da realização parcial.


						if (dtmDataRealizacaoParcial != cConst.DataInvalida) {
							//******************ENTRA NESTE TRECHO DO CÓDIGO QUANDO EFETIVAMENTE OCORRER REALIZAÇÃO PARCIAL

							//MARCA VARIÁVEL QUE INDICA QUE REALMENTE HOUVE REALIZAÇÃO PARCIAL
							//blnRealizacaoParcialCalculada = True

							//se encontrou uma data de realização parcial antes da data do stop inicial.

							//*****busca os demais dados sobre a realização parcial


							if (pintRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PrimeiroLucro)
							{
							    //primeiro fechamento com lucro: busca o valor de fechamento na data de realização parcial
								//Para os outros tipos de realização parcial o valor já foi calculado anteriormente.
								//Para estes casos mesmo que ocorra um gap de alta e o mercado abra acima do valor do stop gain
								//se for disparar uma ordem neste valor ela sempre será executada.
							    decimal decValorAberturaRet = 0;
							    objCotacao.CotacaoConsultar(pstrCodigo, dtmDataPrimeiroDiaAux, strTabelaCotacao, ref decValorRealizacaoParcial, ref decValorAberturaRet);
							}

						    //calcula a quantidade da realização parcial. Se for possível dividir o número de lotes por 2
							//vai sempre realizar metade. Se não for possível o número de lotes realizado será o primerio
							//número inteiro abaixo do resultado da divisão do número de lotes total por 2.
							//Ou seja, neste caso vai realizar menos que a metade.
							//Este número tem que ser multiplicado por pelo número de ações que compõe o lote minimo.
							intQuantidadeRealizacaoParcial = (int) (Truncar(intNumLotes / 2) * pintNumAcoesLote);

							//calcula o percentual de ganho da realização parcial em função do valor de entrada.
							decPercentualRealizacaoParcial = (decValorRealizacaoParcial / decValorEntradaAux - 1) * 100;

							//Atualiza o número de ações EM CARTEIRA, subtraindo o número de ações da realização parcial
							intNumAcoesEMCARTEIRA = intNumAcoesEMCARTEIRA - intQuantidadeRealizacaoParcial;

							//***************REALIZA A MOVIMENTAÇÃO DA CONTA EM FUNÇÃO DA REALIZAÇÃO PARCIAL

							//PRIMEIRO BUSCA O SALDO ATUAL

							//ATUALIZA O SALDO DA CONTA, CREDITANDO DINHEIRO NA CONTA, JÁ QUE FORAM VENDIDAS AÇÕES
							TMP_CONTA_Saldo_Inserir(plngCodigoRelatorio, dtmDataRealizacaoParcial, intQuantidadeRealizacaoParcial * decValorRealizacaoParcial);

							//chama função que faz o recálculo da movimentação, caso já exista uma outra movimentação na mesma data.
							//se nao existir movimentação na mesma data, a função "TMP_CONTA_Saldo_Recalcular" não faz nada.
							//TMP_CONTA_Saldo_Recalcular(plngCodigoRelatorio, dtmDataRealizacaoParcial)


						}
						//FIM DO IF QUE TESTA SE REALMENTE HÁ REALIZAÇÃO PARCIAL.

					}
					//fim do if que trata as realizações parciais.

					//****************FIM DO TRATAMENTO PARA A REALIZAÇÃO PARCIAL*******************


					if ((!blnStopCalculado) && (!objRSSplit.EOF)) {
						//só tem que fazer as alterações da quantidade de ações EM CARTEIRA em função do split,
						//se não conseguiu fazer o stop antes do split.

						//faz o ajuste do número de lotes e da quantidade total de ações de acordo com o split ocorrido.
						//****************ATENÇÃO:O ajuste tem que ser feito antes de mover o RS para o próximo registro.

						//para obter a nova quantidade de ações multiplica a quantidade atual pela razão invertida do split.
						dblNumAcoesEMCARTEIRAAux = intNumAcoesEMCARTEIRA * Convert.ToDouble(objRSSplit.Field("RazaoInvertida"));

						//ajusta também o valor auxiliar de entrada em função do split, para não gerar erro quando for
						//calcular o percentual de saída da operação.
						decValorEntradaAux = decValorEntradaAux * Convert.ToDecimal(objRSSplit.Field("Razao"));

						//AJUSTA O VALOR DE STOP INICIAL
						pdecValorStopInicial = pdecValorStopInicial * Convert.ToDecimal(objRSSplit.Field("Razao"));

						//AJUSTA O VALOR DA REALIZAÇÃO PARCIAL
						decValorRealizacaoParcial = decValorRealizacaoParcial * Convert.ToDecimal(objRSSplit.Field("Razao"));


						if (dblNumAcoesEMCARTEIRAAux % pintNumAcoesLote == 0) {
							//Se após o split o número de ações ainda fecha um lote certo tem que apenas atribuir na variável principal 
							//o novo número de ações.
							intNumAcoesEMCARTEIRA = Convert.ToInt32(dblNumAcoesEMCARTEIRAAux);


						} else {
							//se o número de ações em carteira não fechar exatamente um lote as ações FRACIONÁRIAS
							//devem ser consideradas como vendidas pelo valor da cotação de fechamento da data do split.
							intNumAcoesEMCARTEIRA = (int) (Truncar(dblNumAcoesEMCARTEIRAAux / pintNumAcoesLote) * pintNumAcoesLote);

						    decimal pdecValorAberturaRet = 0M;
						    objCotacao.CotacaoConsultar(pstrCodigo, Convert.ToDateTime(objRSSplit.Field("Data")), "COTACAO", ref decValorFechamentoAux, ref pdecValorAberturaRet);

							//chama função que faz o recálculo da movimentação, caso já exista uma outra movimentação na mesma data.
							//se nao existir movimentação na mesma data, a função "TMP_CONTA_Saldo_Recalcular" não faz nada.
							//TMP_CONTA_Saldo_Recalcular(plngCodigoRelatorio, CDate(objRSSplit.Field("Data")))


							//o valor da venda destas ações pelo valor de fechamento da cotaçao no dia do split
							//deve ser creditado na conta.
							decSaldo = TMP_CONTA_Saldo_Atual_Consultar(plngCodigoRelatorio, Convert.ToDateTime(objRSSplit.Field("Data")));

							TMP_CONTA_Saldo_Inserir(plngCodigoRelatorio, Convert.ToDateTime(objRSSplit.Field("Data")),((decimal) (dblNumAcoesEMCARTEIRAAux - intNumAcoesEMCARTEIRA)) * decValorFechamentoAux);

						}

						//calcula o novo número de lotes, pois é alterado em função do split.
						intNumLotes = intNumAcoesEMCARTEIRA / pintNumAcoesLote;

						//ajusta a data inicial para a data do split.
						//Tem que ser antes do MoveNext porque tem que trabalhar com  a data do split atual
						//e um dia antes do split anterior.
						dtmRealizacaoParcialDataInicial = Convert.ToDateTime(objRSSplit.Field("Data"));

						//movimenta o recordset para o próximo registro caso o mesmo ainda não tenha chegado até o final

						if (!objRSSplit.EOF) {
							objRSSplit.MoveNext();

						}

					}
					//if (Not blnStopCalculado) And (Not objRSSplit.EOF) Then ...


				}
				//While Not blnPeriodoTotalPercorrido And (Not blnStopCalculado Or Not blnRealizacaoParcialCalculada)


				//faz os tratamentos necessários caso a operação tenha terminado, ou seja,
				//tenha encontrado uma data de saída da operação.

				if (dtmDataStopReal != frwInterface.cConst.DataInvalida) {
                    
					//consulta o valor de abertura no dia do stop, ou seja, no dia da saída total da operação.
				    decimal pdecValorFechamentoRet = -1;
				    objCotacao.CotacaoConsultar(pstrCodigo, dtmDataStopReal, strTabelaCotacao, ref pdecValorFechamentoRet, ref decValorAberturaAux);


					if (decValorAberturaAux < pdecValorStopInicial) {
						//se abriu abaixo do valor de de stop, considera que foi vendido a mercado no valor de abertura.
						pdecValorStopInicial = decValorAberturaAux;

					}

					//PERCENTUAL DO VALOR DA SAÍDA TOTAL DA OPERAÇÃO EM RELAÇÃO AO VALOR DE ENTRADA
					//decPercentualSaida = (pdecValorStopInicial / pdecValorEntrada - 1) * 100
					decPercentualSaida = (pdecValorStopInicial / decValorEntradaAux - 1) * 100;

					//VALOR ACUMULADO AO FINAL DA OPERAÇÃO:
					//QUANTIDADE DA REALIZAÇÃO PARCIAL MULTIPLICADO PELO VALOR DA REALIZACAO PARCIAL
					//SOMADO COM A QUANTIDADE RESTANTE APÓS A REALIZAÇÃO MULTIPLICADO PELO VALOR DA SAIDA FINAL
					decValorFinalOperacao = (decValorRealizacaoParcial * intQuantidadeRealizacaoParcial + pdecValorStopInicial * intNumAcoesEMCARTEIRA);

					//PERCENTUAL FINAL DA OPERAÇÃO, CONSIDERANDO A REALIZAÇÃO PARCIAL E A REALIZAÇÃO FINAL
					decPercentualFinalOperacao = (decValorFinalOperacao / (pdecValorEntrada * intQuantidadeEntrada) - 1) * 100;

					//**************atualiza o saldo da conta na data de saída total da operação.


					//chama função que faz o recálculo da movimentação, caso já exista uma outra movimentação na mesma data.
					//se nao existir movimentação na mesma data, a função "TMP_CONTA_Saldo_Recalcular" não faz nada.
					//TMP_CONTA_Saldo_Recalcular(plngCodigoRelatorio, dtmDataStopReal)

					//antes disso, consulta o saldo da conta na data de realização final
					decSaldo = TMP_CONTA_Saldo_Atual_Consultar(plngCodigoRelatorio, dtmDataStopReal);

					//o valor creditado na conta é as ações que ainda estavam compradas multiplicado pelo
					//valor do stop.
					TMP_CONTA_Saldo_Inserir(plngCodigoRelatorio, dtmDataStopReal, intNumAcoesEMCARTEIRA * pdecValorStopInicial);

				}

				long lngOrdem = 0;

				//calcula o código sequencial da operação
				lngOrdem = objRS.CodigoSequencialCalcular(strTabelaBackTesting, "ORDEM", "COD_RELATORIO = " + plngCodigoRelatorio.ToString());

				//lança o registro da operação na tabela de backtesting, de acordo com o período da operação.
				strQuery = " INSERT INTO " + strTabelaBackTesting + " (COD_RELATORIO, ORDEM, DATA_ENTRADA, QUANTIDADE_ENTRADA, VALOR_COTACAO_ENTRADA " + ", VALOR_STOP_LOSS, PERCENTUAL_STOP_LOSS, QUANTIDADE_SAIDA";



				if (dtmDataRealizacaoParcial != frwInterface.cConst.DataInvalida) {
					//SE TEM REALIZAÇÃO PARCIAL, ADICIONA OS CAMPOS NA QUERY
					strQuery = strQuery + ", DATA_REALIZACAO_PARCIAL, QUANTIDADE_REALIZACAO_PARCIAL " + ", VALOR_REALIZACAO_PARCIAL, PERCENTUAL_REALIZACAO_PARCIAL";

				}


				if (dtmDataStopReal != frwInterface.cConst.DataInvalida) {
					//SE ENCONTROU UMA DATA DE SAÍDA DA OPERAÇÃO ENTÃO INCLUI OS RESPECTIVOS CAMPOS NO INSERT
					strQuery = strQuery + ", DATA_SAIDA, VALOR_SAIDA, PERCENTUAL_SAIDA, VALOR_FINAL_OPERACAO " + ", PERCENTUAL_FINAL_OPERACAO";

				}

				strQuery = strQuery + ") VALUES " + "(" + plngCodigoRelatorio.ToString() + ", " + lngOrdem.ToString() + ", " + FuncoesBD.CampoDateFormatar(pdtmDataEntrada) + ", " + intQuantidadeEntrada.ToString() + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorEntrada) + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorStopInicial) + ", " + FuncoesBD.CampoDecimalFormatar((pdecValorStopInicial / pdecValorEntrada - 1) * 100) + ", " + intNumAcoesEMCARTEIRA.ToString();


				if (dtmDataRealizacaoParcial != cConst.DataInvalida) {
					//SE TEM REALIZAÇÃO PARCIAL, ADICIONA OS CAMPOS NA QUERY
					strQuery = strQuery + ", " + FuncoesBD.CampoDateFormatar(dtmDataRealizacaoParcial) + ", " + intQuantidadeRealizacaoParcial.ToString() + ", " + FuncoesBD.CampoDecimalFormatar(decValorRealizacaoParcial) + ", " + FuncoesBD.CampoDecimalFormatar(decPercentualRealizacaoParcial);

				}


				if (dtmDataStopReal != cConst.DataInvalida) {
					//SE ENCONTROU UMA DATA DE SAÍDA DA OPERAÇÃO ENTÃO INCLUI OS RESPECTIVOS CAMPOS NO INSERT
					strQuery = strQuery + ", " + FuncoesBD.CampoDateFormatar(dtmDataStopReal) + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorStopInicial) + ", " + FuncoesBD.CampoDecimalFormatar(decPercentualSaida) + ", " + FuncoesBD.CampoDecimalFormatar(decValorFinalOperacao) + ", " + FuncoesBD.CampoDecimalFormatar(decPercentualFinalOperacao);

				}

				strQuery = strQuery + ")";

				objCommand.Execute(strQuery);

			}
			//fim do if que testa se pode comprar pelo menos um lote

			return objCommand.TransStatus;

		}

		/// <summary>
		/// Função principal que busca todas as operações para os setups escolhidos, no período escolhido
		/// e as executa individualmente.
		/// </summary>
		/// <param name="pstrCodigo">Código do papel</param>
		/// <param name="parrSetup">Array de estruturas contendo todos os parâmetros específicos de cada setup</param>
		/// <param name="pstrPeriodicidade">DIARIO ou SEMANAL</param>
		/// <param name="pdecValorCapitalInicial">Montante inicial para entrada na operação</param>
		/// <param name="pintNumAcoesLote">Número de açoes que compõem um lote mínimo do papel</param>
		/// <param name="pdtmDataInicial">Data  inicial de execução das operações. Quando este parâmetro for preenchido
		/// com seu valor padrão executa a operação desde a primeira cotação.
		/// </param>
		/// <param name="pblnRealizacaoParcialPermitirDayTrade">Indica se permite realizações parciais em day trade</param>
		/// <param name="pstrRelatoriosSpoolDescricao">Descrição que será inserida na tabela RELATORIOS_SPOOL</param>
		/// <param name="pintMediaTipo">Indica o tipo de média que deve ser utilizada no cálculo (aritmética ou exponencial)</param>
		/// <returns>STATUS DA TRANSAÇÃO</returns>
		/// <remarks></remarks>
		public bool RelatBackTestOperacoesExecutar(string pstrCodigo, cEstrutura.structBackTestSetup[] parrSetup, string pstrPeriodicidade, decimal pdecValorCapitalInicial, bool pblnRealizacaoParcialPermitirDayTrade, string pstrRelatoriosSpoolDescricao, cEnum.enumMediaTipo pintMediaTipo, int pintNumAcoesLote, System.DateTime pdtmDataInicial, ref long plngCodRelatorioRet)
		{

			cCommand objCommand = new cCommand(objConexao);

			cRS objRSOperacao = new cRS(objConexao);

			cCotacao objCotacao = new cCotacao(objConexao);

			string strQueryPrincipal = String.Empty;
			string strQueryAux = null;

			int intI = 0;

			//código único identificador da operação.

			System.DateTime dtmOperacaoDataInicial = default(System.DateTime);

			string strTabelaBackTest = null;

			if (pstrPeriodicidade == "DIARIO") {
				strTabelaBackTest = "RELAT_BACKTESTING_DIARIO";
			} else if (pstrPeriodicidade == "SEMANAL") {
				strTabelaBackTest = "RELAT_BACKTESTING_SEMANAL";
			} else {
				strTabelaBackTest = String.Empty;
			}

			//valores auxiliares que variam de acordo com o setup
			int intSetupIndex = 0;

			int intStopViradaMediaNumPeriodos = 0;

			//contendo todos os códigos de setup utilizados na operação e separados por um "-"
			string strCodigoSetup = String.Empty;

			//utilizada como acumulador para calcualar o saldo ao final das operações.
			decimal decSaldoFinal = 0;

			string strFiltroMME49 = null;


			objCommand.BeginTrans();

			//Calcula a data inicial da operação

			if (pdtmDataInicial == frwInterface.cConst.DataInvalida) {
				//se não há uma data inicial busca a data da primeira cotação
				objRSOperacao.ExecuteQuery("SELECT Data " + " FROM Cotacao " + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo) + " AND Sequencial = 1");

				dtmOperacaoDataInicial = Convert.ToDateTime(objRSOperacao.Field("Data"));

				objRSOperacao.Fechar();


			} else {
				//se existe uma data inicial de cálculo do back test, então inicializa a conta um dia antes, para
				//o caso de já haver entrada no primeiro dia, já encontrar saldo na conta, já que a verificação 
				//do saldo é feita um dia antes da data de entrada
				dtmOperacaoDataInicial = pdtmDataInicial.AddDays(-1);

			}

			//gera a query que contém todas as operações do período
			//para isso percorre o array dos setups escolhidos

			for (intI = 0; intI <= parrSetup.Length - 1; intI++) {
				switch (parrSetup[intI].strCodigoSetup) {

					case "MME9.1":

						strQueryAux = String.Empty;

						break;
					case "MME9.2":

						strQueryAux = String.Empty;

						break;
					case "MME9.3":

						strQueryAux = String.Empty;

						break;
					case "IFR2SOBREVEND":

						if (parrSetup[intI].blnMME49Filtrar) {
							strFiltroMME49 = "ACIMA";
						} else {
							strFiltroMME49 = "TODOS";
						}

						strQueryAux = cGeradorQuery.BackTestingIFRSemFiltroEntradaQueryGerar(pstrCodigo, pstrPeriodicidade, strFiltroMME49, parrSetup[intI].dblIFR2SobrevendidoValorMaximo, intI + 1, pintMediaTipo);

						break;
					case "IFR2>MMA13":

						if (parrSetup[intI].blnMME49Filtrar) {
							strFiltroMME49 = "ACIMA";
						} else {
							strFiltroMME49 = "TODOS";
						}

						strQueryAux = cGeradorQuery.BackTestingIFRComFiltroEntradaQueryGerar(pstrCodigo, pstrPeriodicidade, strFiltroMME49, intI + 1, pintMediaTipo, pdtmDataInicial);

						break;
					default:

						strQueryAux = String.Empty;

						break;
				}


				if (strQueryPrincipal != String.Empty) {
					strQueryPrincipal = strQueryPrincipal + " UNION ";

				}

				//concatena a query gerada 
				strQueryPrincipal = strQueryPrincipal + strQueryAux;

				//concatena o código da query na variável
				if (strCodigoSetup != String.Empty) {
					strCodigoSetup = strCodigoSetup + " - ";
				}

				strCodigoSetup = strCodigoSetup + parrSetup[intI].strCodigoSetup;

			}

			//gera o código identificador da operação
			plngCodRelatorioRet = objRSOperacao.CodigoSequencialCalcular(strTabelaBackTest, "COD_RELATORIO","");

			//inicializa o saldo da conta com o montante inicial de capital
			TMP_CONTA_Saldo_Inserir(plngCodRelatorioRet, dtmOperacaoDataInicial, pdecValorCapitalInicial);

			//ordena os registros da query por DATA_ENTRADA e pela ordem do setup.
			strQueryPrincipal = strQueryPrincipal + " ORDER BY DATA_ENTRADA, ORDEM ";

			//executa a query que busca todas as operações
			objRSOperacao.ExecuteQuery(strQueryPrincipal);

			bool blnOK = true;

			//para cada operação encontrada chama a função que executa ou não conforme a disponibilidade de saldo na "conta"

			while ((!objRSOperacao.EOF) && blnOK) {
				//calculo para saber em qual indice está o setup que será executado.

				for (intI = 0; intI <= parrSetup.Length - 1; intI++) {

					if (parrSetup[intI].strCodigoSetup == Convert.ToString(objRSOperacao.Field("SETUP"))) {
						intSetupIndex = intI;

						//no momento em que encontrar o setup pode sair do loop.
						break; // TODO: might not be correct. Was : Exit For

					}

				}

				if (Convert.ToString(objRSOperacao.Field("SETUP")).Substring(0, 3) == "MME") {
					//quando o setup é do cruzamento da média de 9 períodos, o número de períodos 
					//utilizado no stop é 9
					intStopViradaMediaNumPeriodos = 9;
				} else {
					//se o stop é o IFR a média é de 5 períodos.
					intStopViradaMediaNumPeriodos = 5;
				}

				blnOK = OperacaoExecutar(pstrCodigo, pstrPeriodicidade, Convert.ToDateTime(objRSOperacao.Field("DATA_ENTRADA")), Convert.ToDecimal(objRSOperacao.Field("VALOR_ENTRADA")), Convert.ToDecimal(objRSOperacao.Field("VALOR_STOP_LOSS")), plngCodRelatorioRet, parrSetup[intSetupIndex].intRealizacaoParcialTipo, pblnRealizacaoParcialPermitirDayTrade, intStopViradaMediaNumPeriodos, parrSetup[intSetupIndex].decPercentualRealizacaoParcialFixo,
				parrSetup[intSetupIndex].decPrimeiroLucroPercentualMinimo, pintNumAcoesLote);

				objRSOperacao.MoveNext();

			}

			objRSOperacao.Fechar();

			//busca a quantidade de ações que estão em carteira, ou seja, em que a data de saída
			//ainda não está preenchida. 

			objRSOperacao.ExecuteQuery("SELECT sum(Quantidade_Saida) AS Quantidade_em_Carteira  " + " FROM " + strTabelaBackTest + " WHERE COD_RELATORIO = " + plngCodRelatorioRet.ToString() + " AND DATA_SAIDA IS NULL");


			if (objRSOperacao.DadosExistir) {
			    //se existem operações em aberto.
				decimal decValorFechamentoUltimaCotacao;

				//consulta o valor de fechamento da última cotação
				decValorFechamentoUltimaCotacao = objCotacao.CotacaoUltimaValorFechamentoConsultar(pstrCodigo);

				//multiplica a quantidade de ações pela última cotação.
				decSaldoFinal = Convert.ToInt32(objRSOperacao.Field("Quantidade_em_Carteira")) * decValorFechamentoUltimaCotacao;

			}

			objRSOperacao.Fechar();

			//CONSULTA O SALDO APÓS A ÚLTIMA OPERAÇÃO
			strQueryAux = "SELECT TOP 1 Saldo_Atual " + " FROM TMP_CONTA " + " WHERE COD_RELATORIO = " + plngCodRelatorioRet.ToString() + " ORDER BY Data DESC, Sequencia DESC ";

			objRSOperacao.ExecuteQuery(strQueryAux);

			//O saldo final é o saldo que está em  carteira somado ao montante 
			//que está na conta e não está aplicado.
			decSaldoFinal = decSaldoFinal + Convert.ToDecimal(objRSOperacao.Field("Saldo_Atual"));

			objRSOperacao.Fechar();

			//insere o registro principal da operação na tabela RELATORIOS_SPOOL
			objCommand.Execute("INSERT INTO RELATORIOS_SPOOL " + "(COD_RELATORIO, DESCRICAO, CODIGO_SETUP, CODIGO, PERIODO, SALDO_INICIAL " + ", SALDO_FINAL, PERCENTUAL_ACUMULADO, DATA)" + " VALUES " + "(" + plngCodRelatorioRet.ToString() + ", " + FuncoesBD.CampoStringFormatar(pstrRelatoriosSpoolDescricao) + ", " + FuncoesBD.CampoStringFormatar(strCodigoSetup) + ", " + FuncoesBD.CampoStringFormatar(pstrCodigo) + ", " + FuncoesBD.CampoStringFormatar(pstrPeriodicidade) + ", " + FuncoesBD.CampoDecimalFormatar(pdecValorCapitalInicial) + ", " + FuncoesBD.CampoDecimalFormatar(decSaldoFinal) + ", " + FuncoesBD.CampoDecimalFormatar((decSaldoFinal / pdecValorCapitalInicial - 1) * 100) + ", NOW)");

			objCommand.CommitTrans();

			return objCommand.TransStatus;

		}

		public bool RelatorioSpoolExcluir(long plngcodRelatorio)
		{

			cCommand objCommand = new cCommand(objConexao);

			cRS objRS = new cRS(objConexao);

			string strTabelaBackTest = String.Empty;

			objCommand.BeginTrans();

			//verifica qual o período de geração do relatório: diário ou semanal
			objRS.ExecuteQuery("SELECT Periodo " + "FROM Relatorios_Spool " + "WHERE cod_relatorio = " + plngcodRelatorio.ToString(CultureInfo.InvariantCulture));

		    string strPeriodo = (string) objRS.Field("Periodo");

			if (strPeriodo == "DIARIO") {
				strTabelaBackTest = "RELAT_BACKTESTING_DIARIO";
			} else if (strPeriodo == "SEMANAL") {
				strTabelaBackTest = "RELAT_BACKTESTING_SEMANAL";
			}
            else
			{
			    throw  new Exception("Período Inválido: " + strPeriodo);
			}

			objRS.Fechar();

			//EXCLUI REGISTRO DO SPOOL DE RELATÓRIOS
			objCommand.Execute("DELETE " + "FROM Relatorios_Spool " + "WHERE cod_relatorio = " + plngcodRelatorio.ToString());

			//EXCLUI O REGISTRO DA TABELA TMP_CONTA
			objCommand.Execute("DELETE " + "FROM TMP_CONTA " + "WHERE cod_relatorio = " + plngcodRelatorio.ToString());

			//EXCLUI O REGISTRO DA TABELA DE BACK TESTES
			objCommand.Execute("DELETE " + "FROM " + strTabelaBackTest + " WHERE cod_relatorio = " + plngcodRelatorio.ToString());

			objCommand.CommitTrans();

			return objCommand.TransStatus;

		}


		/// <summary>
		/// Ainda não está preparada para as cotações semanais
		/// </summary>
		/// <param name="pstrAliasTabelaCotacaoPrincipal"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FiltroVolumeNegociosGerar(string pstrAliasTabelaCotacaoPrincipal, string pstrTabelaCotacaoFiltro, Int32 pintValor)
		{

			string strFiltro = null;

			//Calcula a média de 21 dias do volume de negócios e compara com o valor atual
			strFiltro = "(" + FuncoesBD.CampoFormatar(pintValor) + " < " + Environment.NewLine;
			strFiltro = strFiltro + "(" + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " SELECT AVG(Negocios_Total) " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " FROM " + pstrTabelaCotacaoFiltro + " CN " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " WHERE " + pstrAliasTabelaCotacaoPrincipal + ".Codigo = CN.Codigo " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " AND CN.Sequencial >= (" + pstrAliasTabelaCotacaoPrincipal + ".Sequencial - 20) " + Environment.NewLine;
			strFiltro = strFiltro + "))";


			return strFiltro;

		}

		/// <summary>
		/// Ainda não está preparada para as cotações semanais
		/// </summary>
		/// <param name="pstrAliasTabelaCotacaoPrincipal"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FiltroVolumeFinanceiroGerar(string pstrAliasTabelaCotacaoPrincipal, string pstrTabelaCotacaoFiltro, decimal pdecValor)
		{

			string strFiltro = null;

			//Calcula a média de 21 dias do volume financeiro e compara com o valor atual
			strFiltro = "(" + FuncoesBD.CampoFormatar(pdecValor) + " < " + Environment.NewLine;
			strFiltro = strFiltro + "(" + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " SELECT AVG(Valor_Total) " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " FROM " + pstrTabelaCotacaoFiltro + " CN " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " WHERE " + pstrAliasTabelaCotacaoPrincipal + ".Codigo = CN.Codigo " + Environment.NewLine;
			strFiltro = strFiltro + '\t' + " AND CN.Sequencial >= (" + pstrAliasTabelaCotacaoPrincipal + ".Sequencial - 20) " + Environment.NewLine;
			strFiltro = strFiltro + "))";

			return strFiltro;

		}

	}
}
