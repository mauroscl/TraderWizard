using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using DTO;
using prjDominio.Regras;
using prjDominio.ValueObjects;
using prmCotacao;
using Services;
using ServicoNegocio;
using TraderWizard.Enumeracoes;

namespace Cotacao
{

	public class cRelatorio
	{


		private readonly Conexao _conexao;
	    private readonly CotacaoData _cotacaoData;
	    private readonly FuncoesBd _funcoesBd;

		public cRelatorio(Conexao conexao)
		{
			_conexao = conexao;
            this._cotacaoData = new CotacaoData();
		    this._funcoesBd = conexao.ObterFormatadorDeCampo();
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
		    //, dia_atual.valorfechamento, dia_atual.mmexp9

		    string strTabelaMME21 = pstrTabelaCotacao.ToUpper() == "COTACAO" ? "MME21_Diario" : "MME21_Semanal";

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			string strQuery = " SELECT dia_atual.codigo, dia_atual.ValorFechamento " + Environment.NewLine + ", ROUND(mmexp9, 2) AS MMExp9 " + Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " + Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " + Environment.NewLine + ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " + Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " + Environment.NewLine + ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " + Environment.NewLine;


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) " + "* (1 + " + FuncoesBd.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 4) AS STOP_GAIN " + Environment.NewLine;

			}

		    //****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			string strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2)  as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento < Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAnterior) + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor as MMExp21 " + " FROM ";

			//ligação da cotação com a tabela de médias para fazer o filtro com a média de 9 perídos.
			string strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			//ligaçao com a tabela de médias para calcular o percentual em relação à média de 21 períodos
			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " On c.Codigo = MME21.Codigo " + " And c.Data = MME21.Data)";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = 'MME'" + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual);


			//--NÃO PRECISA DESTE WHERE PORQUE FORAM CRIADAS VIEWS PARA AS MÉDIAS DE 21 DIAS, DIÁRIA E SEMANAL.
			//WHERE DA MÉDIA DE 21 PERIODOS

			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBd.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBd.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBd.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBd.CampoDecimalFormatar(pdecValorTotal);

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
		private string SetupMME92QueryGerar(DateTime pdtmDataAnterior, DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, bool pblnAlijamentoCalcular, 
            decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1,decimal pdecPercentualStopGain = -1)
		{

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

		    string strTabelaMME21 = pstrTabelaCotacao.ToUpper() == "COTACAO" ? "MME21_Diario" : "MME21_Semanal";

			//, dia_atual.valorfechamento, dia_atual.mmexp9
		    string strQuery = " SELECT dia_atual.codigo, dia_atual.ValorFechamento " + ", ROUND(mmexp9, 2) AS MMExp9 " +
		                      Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " +
		                      Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " +
		                      Environment.NewLine +
		                      ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " +
		                      Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " +
		                      Environment.NewLine +
		                      ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " +
		                      Environment.NewLine;


			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) " + "* (1 + " + FuncoesBd.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN " + Environment.NewLine;

			}

		    //****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			string strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2) as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total, valorminimo " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAnterior) + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor as MMExp21 " + " FROM ";

			string strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " ON C.Codigo = MME21.Codigo " + " AND C.Data = MME21.Data) ";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = 'MME'" + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + " AND c.Oscilacao < 0 ";


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBd.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBd.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBd.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBd.CampoDecimalFormatar(pdecValorTotal);

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
		    FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			string strTabelaMME21 = pstrTabelaCotacao.ToUpper() == "COTACAO" ? "MME21_Diario" : "MME21_Semanal";

			string strQuery = " SELECT dia_atual.Codigo, dia_atual.ValorFechamento " + ", ROUND(mmexp9, 2) AS MMExp9 " + Environment.NewLine + ", ROUND((dia_atual.ValorFechamento / MMExp9 - 1) * 100, 4) AS Perc_MME9 " + Environment.NewLine + ", ROUND(dia_atual.valormaximo + 0.01, 2) As ENTRADA " + ", ROUND(((dia_atual.valormaximo + 0.01) / dia_atual.valorfechamento - 1) * 100, 4) As PERC_ENTRADA " + Environment.NewLine + ", ROUND(dia_atual.valorminimo - 0.03, 2) As STOP_LOSS " + Environment.NewLine + ", ROUND(((dia_atual.valorminimo - 0.03) / (dia_atual.valormaximo + 0.01)  - 1) * 100, 4) As PERC_STOP_LOSS " + Environment.NewLine;

			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((dia_atual.ValorMaximo + 0.01) " + "* (1 + " + FuncoesBd.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN " + Environment.NewLine;

			}

			//****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			string strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + " / (dia_atual.valormaximo + 0.01) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "((dia_atual.valormaximo + 0.01) - (dia_atual.valorminimo - 0.03)) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (dia_atual.valormaximo + 0.01) AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * ((dia_atual.valormaximo + 0.01) -  (dia_atual.valorminimo - 0.03)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND((dia_atual.valormaximo + 0.01) * 2 - (dia_atual.valorminimo - 0.03), 2)  as STOP_GAIN " + Environment.NewLine;

			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((dia_atual.ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			strQuery = strQuery + " FROM " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, titulos_total " + " FROM " + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data " + " WHERE m.tipo = 'MME' " + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAnterior) + " AND c.Oscilacao < 0 " + ") As dia_anterior " + " INNER JOIN " + "(" + " SELECT c.codigo, valorabertura, valorfechamento, m.valor As mmexp9 " + " , titulos_total, valorminimo, valormaximo, MME21.Valor AS MMExp21 " + " FROM ";

			string strTabelaAux = "(" + pstrTabelaCotacao + " c INNER JOIN " + pstrTabelaMedia + " m " + " On c.codigo = m.codigo " + " And c.data = m.data) ";


			if (pdblTitulosTotal != -1) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabelaAux = "(" + strTabelaAux + " INNER JOIN " + pstrTabelaMedia + " AS MV " + " ON C.Codigo = MV.Codigo " + " AND C.Data = MV.Data) ";

			}

			strTabelaAux = "(" + strTabelaAux + " LEFT JOIN " + strTabelaMME21 + " AS MME21 " + " ON C.Codigo = MME21.Codigo " + " AND C.Data = MME21.Data) ";

			strQuery = strQuery + strTabelaAux;

			strQuery = strQuery + " WHERE m.tipo = " + FuncoesBd.CampoStringFormatar("MME") + " And m.numPeriodos = 9 " + " And valorfechamento > Round(m.valor, 2) " + " And c.data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + " AND c.Oscilacao < 0 ";


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + FuncoesBd.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + FuncoesBd.CampoFloatFormatar(pdblTitulosTotal);

			}


			if (pintNegociosTotal != -1) {
				strQuery = strQuery + " And c.negocios_total >= " + FuncoesBd.CampoFloatFormatar(pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strQuery = strQuery + " And c.valor_total >= " + FuncoesBd.CampoDecimalFormatar(pdecValorTotal);

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

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();


		    string strQuery = " SELECT " + pstrTabelaCotacao + ".codigo, ValorFechamento " + ", ROUND(" + pstrTabelaIFR + ".Valor, 2) AS IFR2" + Environment.NewLine  + ", ROUND(valorfechamento,2) As entrada " + Environment.NewLine + ", ROUND(valorminimo - (valormaximo - valorminimo) * 1.3, 2) As stop_loss " + Environment.NewLine + ", ROUND(((valorminimo - (valormaximo - valorminimo) * 1.3) / valorfechamento -1) * 100, 4) As perc_stop_loss " + Environment.NewLine;

			if (pdecPercentualStopGain != -1) {
				strQuery += ", ROUND(VALORFECHAMENTO " + "* (1 + " + funcoesBd.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 4) AS STOP_GAIN " + Environment.NewLine;
			}

		    //****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade de lotes fechados

		    string formulaDaQuantidadeDeCapital;

		    string valorDoCapitalFormatado = funcoesBd.CampoDecimalFormatar(pdecValorCapital);

		    if (_conexao.BancoDeDados == cEnum.BancoDeDados.Access)
		    {
		        //1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
		        //com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
		        formulaDaQuantidadeDeCapital = "CSTR(" + valorDoCapitalFormatado +
		                                              " / ValorFechamento / 100)";

		        //2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
		        //Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
		        //esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

		        formulaDaQuantidadeDeCapital = "CINT(LEFT(" + formulaDaQuantidadeDeCapital + ", " + " IIF(INSTR(" +
		                                       formulaDaQuantidadeDeCapital + "," + funcoesBd.CampoStringFormatar(",") +
		                                       ") > 0 " + ", INSTR(" + formulaDaQuantidadeDeCapital + "," +
		                                       funcoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" +
		                                       formulaDaQuantidadeDeCapital + ")" + "))) * 100";
		    }
		    else
		    {
                formulaDaQuantidadeDeCapital = $"ROUND ({valorDoCapitalFormatado} / ValorFechamento, -2, 1)";
		    }

			strQuery += ", " + formulaDaQuantidadeDeCapital + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery += ", " + formulaDaQuantidadeDeCapital + " * ValorFechamento AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar tudo que for possível sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
		    const string perdaPorAcao = " (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) ";

		    strQuery += ", ROUND((" + formulaDaQuantidadeDeCapital + " * " + perdaPorAcao + "/ " + valorDoCapitalFormatado + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;


			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

		    string valorPerdaManejoFormatado = funcoesBd.CampoDecimalFormatar(pdecValorPerdaManejo);
		    string formulaValorMaximoDiferenteValorMinimo = valorPerdaManejoFormatado + " / " + perdaPorAcao;

            string projecaoCondicional = funcoesBd.Condicional("ValorMaximo <> ValorMinimo", formulaValorMaximoDiferenteValorMinimo, valorPerdaManejoFormatado);

		    string formulaDaQuantidadeComManejo;

		    if (_conexao.BancoDeDados == cEnum.BancoDeDados.Access)
		    {
		        //1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
		        //pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
		        //OBS: EXISTE UM IIF TESTANDO SE O VALOR MÁXIMO É DIFERENTE DO VALOR MÍNIMO PARA EVITAR ERROS
		        //DE DIVISÃO POR ZERO. ESTA CONDIÇÃO SÓ VAI ACONTECER EM AÇÕES MUITO POUCO LÍQUIDAS
		        formulaDaQuantidadeComManejo = string.Format("CSTR({0} / 100)", projecaoCondicional);
		        //2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
		        //Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
		        //esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

		        formulaDaQuantidadeComManejo = "CINT(LEFT(" + formulaDaQuantidadeComManejo + ", " + " IIF(INSTR(" +
		                                       formulaDaQuantidadeComManejo + "," + funcoesBd.CampoStringFormatar(",") +
		                                       ") > 0 " + ", INSTR(" + formulaDaQuantidadeComManejo + "," +
		                                       funcoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" +
		                                       formulaDaQuantidadeComManejo + ")" + "))) * 100";

                strQuery += ", " + formulaDaQuantidadeComManejo + " AS Quantidade_Manejo" + Environment.NewLine;
		    }
		    else
		    {
                formulaDaQuantidadeComManejo = string.Format("ROUND ({0}, -2, 1)  ", projecaoCondicional);
                strQuery += ", CONVERT(decimal, " + formulaDaQuantidadeComManejo + ") AS Quantidade_Manejo" + Environment.NewLine;    
		    }
            

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
            strQuery = strQuery + ", " + formulaDaQuantidadeComManejo + " * ValorFechamento AS Valor_Manejo" + Environment.NewLine;

		    string formulaDoPercentualDeRiscoQuandoPuderComprarPeloMenosUmLote = string.Format("ROUND({0} * {1} / {2} * 100, 2)", formulaDaQuantidadeComManejo, perdaPorAcao, valorDoCapitalFormatado);
            
		    if (_conexao.BancoDeDados == cEnum.BancoDeDados.SqlServer)
		    {
                strQuery += string.Format(", {0}  AS Perc_Risco_Manejo ", funcoesBd.Condicional(string.Format("{0} = 0", formulaDaQuantidadeDeCapital), "0", formulaDoPercentualDeRiscoQuandoPuderComprarPeloMenosUmLote));
		    }
		    else
		    {
                //obtem o percentual de risco em relação ao capital total se for aplicar o que o manejo de risco permitir.
                //Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
                strQuery += string.Format(", {0}  AS Perc_Risco_Manejo ", formulaDoPercentualDeRiscoQuandoPuderComprarPeloMenosUmLote) + Environment.NewLine;
		    }

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND(VALORFECHAMENTO * 2 - (valorminimo - (valormaximo - valorminimo) * 1.3), 2) as STOP_GAIN " + Environment.NewLine;
			}

			//percentual em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((ValorFechamento / MME21.Valor - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

			string strTabela = "(" + pstrTabelaCotacao + " INNER JOIN " + pstrTabelaIFR + Environment.NewLine + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaIFR + ".Codigo " + Environment.NewLine + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaIFR + ".Data) " + Environment.NewLine;

			if (pdblTitulosTotal != -1.0) {
				//O filtro por quantidade de ações negociadas utilizada a média de 21 períodos do volume

				strTabela = "(" + strTabela + " INNER JOIN " + pstrTabelaMedia + " AS MV " + Environment.NewLine + " ON " + pstrTabelaCotacao + ".Codigo = MV.Codigo " + Environment.NewLine + " AND " + pstrTabelaCotacao + ".Data = MV.Data) " + Environment.NewLine;

			}

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 21 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 21 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 21 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME21
			strTabela = "(" + strTabela + " LEFT JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + 
                " WHERE Data = " + funcoesBd.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' +
                " AND Tipo = " + funcoesBd.CampoStringFormatar("MMA") + Environment.NewLine + '\t' + " And NumPeriodos = 21 " + Environment.NewLine + ") AS MME21 " + Environment.NewLine + " On " + pstrTabelaCotacao + ".Codigo = MME21.Codigo " + Environment.NewLine + " And " + pstrTabelaCotacao + ".Data = MME21.Data) " + Environment.NewLine;

			strQuery = strQuery + " FROM " + strTabela + " WHERE " + pstrTabelaCotacao + ".Data = " + funcoesBd.CampoDateFormatar(pdtmDataAtual) + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + " And " + pstrTabelaIFR + ".Valor <= " + funcoesBd.CampoFloatFormatar(pdblIFR2LimiteSuperior);


			if (pblnAcimaMME49) {
				strQuery = strQuery + " And " + pstrTabelaCotacao + ".ValorFechamento >= MME49.Valor ";

			}


			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + " And MV.Tipo = " + funcoesBd.CampoStringFormatar("VMA") + " And MV.NumPeriodos = 21 " + " And MV.Valor >= " + funcoesBd.CampoFloatFormatar(pdblTitulosTotal);

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

			cRS objRS = new cRS(_conexao);

		    dynamic lngSequencialAtual = plngSequencialInicial;
			bool blnOK = false;
			long lngUltimoSequencialSobrevendido = plngSequencialInicial;

			//Enquanto não encontrar o número de tentativas

		    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			while (!blnOK) {
				string strSQL = "SELECT TOP 5 Sequencial, Valor " + Environment.NewLine;
				strSQL = strSQL + " FROM Cotacao C INNER JOIN IFR_Diario IFR " + Environment.NewLine;
				strSQL = strSQL + " ON C.Codigo = IFR.Codigo " + Environment.NewLine;
				strSQL = strSQL + " AND C.Data = IFR.Data " + Environment.NewLine;
				strSQL = strSQL + " WHERE C.Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
				strSQL = strSQL + " AND Sequencial < " + funcoesBd.CampoFormatar(lngSequencialAtual) + Environment.NewLine;
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

			return (int)  (plngSequencialInicial - lngUltimoSequencialSobrevendido + 1);

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

		public void CalcularValoresRealizacao(string pstrCodigo, Setup pobjSetup, ClassifMedia pobjCM, IFRSobrevendido pobjIFRSobrevendido, IList<IFRSimulacaoDiariaFaixa> plstFaixas, decimal pdecValorEntrada, DateTime pdtmDataEntrada, ref string pstrValorRealizacaoParcialRet, ref string pstrValorRealizacaoFinalRet)
		{
		    string strWhere = String.Empty;

			string strSQL = " SELECT MIN(Percentual_Maximo) AS Percentual_Realizacao_Parcial, MAX(Percentual_Saida) AS Percentual_Saida_Maximo " + Environment.NewLine;

            FuncoesBd FuncoesBd  = _conexao.ObterFormatadorDeCampo();

			strWhere += " FROM IFR_SIMULACAO_DIARIA D " + Environment.NewLine;
			strWhere += " WHERE D.Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strWhere += " AND D.ID_Setup = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strWhere += " AND D.ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID) + Environment.NewLine;
			strWhere += " AND Verdadeiro = " + FuncoesBd.CampoFormatar(true) + Environment.NewLine;
			strWhere += " AND Valor_IFR_Minimo <= " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ValorMaximo) + Environment.NewLine;
			strWhere += " AND Data_Saida <= " + FuncoesBd.CampoFormatar(pdtmDataEntrada);


			foreach (IFRSimulacaoDiariaFaixa objIFRFaixa in plstFaixas) {
				strWhere += " AND EXISTS " + Environment.NewLine;
				strWhere += "(" + Environment.NewLine;
				strWhere += '\t' + " SELECT 1 " + Environment.NewLine;
				strWhere += '\t' + " FROM IFR_Simulacao_Diaria_Faixa F " + Environment.NewLine;
				strWhere += '\t' + " WHERE ID = " + FuncoesBd.CampoFormatar(objIFRFaixa.Id) + Environment.NewLine;
				strWhere += '\t' + " AND " + objIFRFaixa.CriterioDeClassificacaoDaMedia.CampoBD + " BETWEEN Valor_Minimo AND Valor_Maximo " + Environment.NewLine;
				strWhere += '\t' + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;
				strWhere += ")";

			}

			strSQL = strSQL + strWhere;

			cRS objRS = new cRS(_conexao);

			objRS.ExecuteQuery(strSQL);

			decimal decPercentualRealizacaoParcial = Math.Floor(Convert.ToDecimal(objRS.Field("Percentual_Realizacao_Parcial")));

			double decPercentualSaidaMaximo = Convert.ToDouble(objRS.Field("Percentual_Saida_Maximo"));

			pstrValorRealizacaoParcialRet = Math.Round(pdecValorEntrada * (1 + decPercentualRealizacaoParcial / 100), 2).ToString() + " (" + decPercentualRealizacaoParcial + " %)";

			objRS.Fechar();

			strSQL = " SELECT MIN(Percentual_Maximo) AS Percentual_Realizacao_Final " + Environment.NewLine;

			strSQL = strSQL + strWhere;
			strSQL = strSQL + " AND Percentual_Maximo > " + FuncoesBd.CampoFormatar(decPercentualSaidaMaximo);

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
			string functionReturnValue;

			CarregadorDeResumoDoIFRDiario objCarregadorResumo = new CarregadorDeResumoDoIFRDiario(_conexao);

			IFRSimulacaoDiariaFaixaResumo objResumo = objCarregadorResumo.Carregar(pobjSimulacaoDiariaVO);


			if (objResumo != null) {
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
            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

		    string strTabela = "COTACAO CH INNER JOIN IFR_DIARIO IFRH " + Environment.NewLine;
			strTabela = strTabela + " On CH.CODIGO = IFRH.CODIGO " + Environment.NewLine;
			strTabela = strTabela + " And CH.DATA = IFRH.DATA " + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strTabela = "(" + strTabela + ") INNER JOIN Media_Diaria MV" + Environment.NewLine;
				strTabela = strTabela + " ON CH.Codigo = MV.Codigo " + Environment.NewLine;
				strTabela = strTabela + " AND CH.Data = MV.Data " + Environment.NewLine;

			}

			string strSQL = " SELECT CH.CODIGO " + Environment.NewLine;
			strSQL = strSQL + " FROM " + strTabela;

			strSQL = strSQL + " WHERE CH.DATA = " + FuncoesBd.CampoFormatar(pdtmDataAtual) + Environment.NewLine;
			strSQL = strSQL + " And CH.SEQUENCIAL > 200 " + Environment.NewLine;
			//Não faz sentido começar a verificação se o papel tem apenas 200 períodos, pois precisamos da média de 200 nos cálculos

			strSQL = strSQL + " AND IFRH.NumPeriodos = 2 " + Environment.NewLine;
			strSQL = strSQL + " AND IFRH.VALOR <= 10 " + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strSQL = strSQL + " AND MV.Tipo = " + FuncoesBd.CampoFormatar("VMA") + Environment.NewLine;
				strSQL = strSQL + " AND MV.NumPeriodos = 21 " + Environment.NewLine;
				strSQL = strSQL + " AND MV.Valor >= " + FuncoesBd.CampoFormatar(pdblTitulosTotal) + Environment.NewLine;

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
			strSQL += '\t' + '\t' + " AND C2.Data <= " + FuncoesBd.CampoFormatar(pdtmDataAtual);
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
			strSQL += '\t' + '\t' + " And S.ID_SETUP = " + FuncoesBd.CampoFormatar(pobjSetup.Id) + Environment.NewLine;
			strSQL += '\t' + " ) " + Environment.NewLine;
			strSQL += " )" + Environment.NewLine;

			cRS objRS = new cRS(_conexao);

			//Debug.Print(strSQL)

			objRS.ExecuteQuery(strSQL);

			IList<string> lstAtivos = new List<string>();


			while (!objRS.EOF) {
				lstAtivos.Add(Convert.ToString(objRS.Field("Codigo")));

				objRS.MoveNext();

			}

			objRS.Fechar();

			//lstAtivos = {"INPR3", "AGEN11", "BRTO4", "AEDU3", "BBDC4", "CYRE3"}


			SetupIFR2SimularDto objSetupIFRSimularDTO = new SetupIFR2SimularDto();

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


		public bool SimularIFRDiarioParaListaDeAtivos(IList<string> plstAtivos, SetupIFR2SimularDto pobjSetupIFRSimularDTO)
		{


			try {
				System.Threading.AutoResetEvent[] arrayAutoResetEvent = new  System.Threading.AutoResetEvent[plstAtivos.Count];
				//Dim arrayAutoResetEvent(lstAtivos.Count - 1) As Threading.WaitHandle

				//obtém os valores antes de alterar para depois da execução restaurar
				int intMinWorkThreads;
				int intMinIoThreads;
				System.Threading.ThreadPool.GetMinThreads(out intMinWorkThreads, out intMinIoThreads);

				int intMaxWorkThreads;
				int intMaxIoThreads;
				System.Threading.ThreadPool.GetMaxThreads(out intMaxWorkThreads, out intMaxIoThreads);

				System.Threading.ThreadPool.SetMinThreads(4, 2);
				System.Threading.ThreadPool.SetMaxThreads(4, 2);

			    for (int intI = 0; intI <= plstAtivos.Count - 1; intI++) {
					string strCodigoAtivo = plstAtivos[intI];

					SetupIFR2SimularCodigoDto objSetupIFR2SimularCodigoDTO = new SetupIFR2SimularCodigoDto(pobjSetupIFRSimularDTO, strCodigoAtivo);

					SimuladorIFRDiario objSimuladorIFRDiario = new SimuladorIFRDiario(objSetupIFR2SimularCodigoDTO);

					//System.Threading.WaitCallback objCallBack = new System.Threading.WaitCallback(objSimuladorIFRDiario.SetupIFR2Simular);
                    System.Threading.WaitCallback objCallBack = objSimuladorIFRDiario.SetupIFR2Simular;

					System.Threading.AutoResetEvent objAutoResetEvent = new System.Threading.AutoResetEvent(false);
					arrayAutoResetEvent.SetValue(objAutoResetEvent, intI);

					System.Threading.ThreadPool.QueueUserWorkItem(objCallBack, objAutoResetEvent);

				}

				// Threading.WaitHandle.WaitAll(arrayAutoResetEvent)


				for (int intI = 0; intI <= arrayAutoResetEvent.Count() - 1; intI++) {
					//Threading.WaitHandle.WaitAny(arrayAutoResetEvent.GetValue(intI))
					((System.Threading.AutoResetEvent)arrayAutoResetEvent.GetValue(intI)).WaitOne();

				}

				//Restaura os valores anteriores
				System.Threading.ThreadPool.SetMinThreads(intMinWorkThreads, intMinIoThreads);
				System.Threading.ThreadPool.SetMaxThreads(intMaxWorkThreads, intMaxIoThreads);

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
		private string RelatIFR2SemFiltroDiarioPersonalizadoGerar(Setup pobjSetup, DateTime pdtmDataAtual, decimal pdecValorCapital, decimal pdecValorPerdaManejo, IFRSobrevendido pobjIFRSobrevendido, double pdblIFR2LimiteSuperior = 5, double pdblTitulosTotal = -1, Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1)
		{

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			//Chama função que verifica se existe alguma simulação pendente e pergunta se o usuário que executá-la antes de iniciar o relatório.
			SetupIFR2SemFiltroDiarioPersonalizadoVerificar(pobjSetup, pdtmDataAtual, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal);

			cCommand objCommand = new cCommand(_conexao);

			objCommand.BeginTrans();

		    string strSQL = "DELETE " + Environment.NewLine;
			strSQL = strSQL + " FROM Relat_IFR2_Sem_Filtro " + Environment.NewLine;

			objCommand.Execute(strSQL);

			cRS objRS = new cRS(_conexao);


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

		    //****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			string strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + " / ValorFechamento / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strSQL = strSQL + ", " + strAux + " AS Quantidade_Sem_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strSQL = strSQL + ", " + strAux + " * ValorFechamento AS Valor_Total_Sem_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar tudo que for possível sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strSQL = strSQL + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;


			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			//OBS: EXISTE UM IIF TESTANDO SE O VALOR MÁXIMO É DIFERENTE DO VALOR MÍNIMO PARA EVITAR ERROS
			//DE DIVISÃO POR ZERO. ESTA CONDIÇÃO SÓ VAI ACONTECER EM AÇÕES MUITO POUCO LÍQUIDAS
			strAux = "CSTR(IIF(ValorMaximo <> ValorMinimo " + ", " + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "(ValorFechamento  - (ValorMinimo - (ValorMaximo - ValorMinimo) * 1.3)) " + ", " + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + ") / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strSQL = strSQL + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strSQL = strSQL + ", " + strAux + " * ValorFechamento AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar o que o manejo de risco permitir.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strSQL = strSQL + ", ROUND((" + strAux + " * (valorfechamento  - (valorminimo - (valormaximo - valorminimo) * 1.3)) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 2)  AS Perc_Risco_Manejo " + Environment.NewLine;


			//****Fim dos cálculos com manejo de risco.

			//percentual em relação à média de 21 períodos
			strSQL = strSQL + ", ROUND((ValorFechamento / MME21.Valor - 1) * 100, 4) AS Perc_MME21 " + Environment.NewLine;

		    string strTabela = "(Cotacao C INNER JOIN IFR_Diario IFR" + Environment.NewLine + " On C.Codigo = IFR.Codigo " + Environment.NewLine + " And C.Data = IFR.Data) " + Environment.NewLine;



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



			strSQL = strSQL + " FROM " + strTabela + Environment.NewLine + " WHERE C.Data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + " AND C.Sequencial > 200" + " And IFR.NumPeriodos = 2 " + Environment.NewLine + " And IFR.Valor <= " + FuncoesBd.CampoFloatFormatar(pdblIFR2LimiteSuperior) + Environment.NewLine;


			if (pdblTitulosTotal != -1) {
				strSQL = strSQL + " And MV.Tipo = " + FuncoesBd.CampoStringFormatar("VMA") + Environment.NewLine + " And MV.NumPeriodos = 21 " + Environment.NewLine + " And MV.Valor >= " + FuncoesBd.CampoFloatFormatar(pdblTitulosTotal) + Environment.NewLine;

			}

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 21 PERIODOS
			strSQL += " AND MME21.Tipo = " + FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME21.NumPeriodos = 21 " + Environment.NewLine;

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 49 PERIODOS
			strSQL += " AND MME49.Tipo = " + FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME49.NumPeriodos = 49 " + Environment.NewLine;

			//WHERE RELATIVO AO RELACIONAMENTO DA COTAÇÃO COM A MÉDIA DE 200 PERIODOS
			strSQL += " AND MME200.Tipo = " + FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine;
			strSQL += " AND MME200.NumPeriodos = 200 " + Environment.NewLine;



			if (pintNegociosTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeNegociosGerar("C", "Cotacao", pintNegociosTotal);

			}


			if (pdecValorTotal != -1) {
				strSQL = strSQL + " AND " + FiltroVolumeFinanceiroGerar("C", "Cotacao", pdecValorTotal);

			}

			objRS.ExecuteQuery(strSQL);

		    IList<IFRSimulacaoDiariaFaixa> lstFaixas = new List<IFRSimulacaoDiariaFaixa>();

		    ValorCriterioClassifMediaVO objValorCriterioCMVO = new ValorCriterioClassifMediaVO();

		    dynamic objCarregadorCarteira = new CarregadorCarteira(_conexao);
			dynamic objCarteiraAtiva = objCarregadorCarteira.CarregaAtiva(pobjIFRSobrevendido);

		    //Para cada um dos itens que está com IFR sobrevendido.

			while ((!objRS.EOF) && (_conexao.TransStatus)) {
				lstFaixas.Clear();

				string strValorRealizacaoParcial = string.Empty;

				string strValorRealizacaoFinal = string.Empty;

				Ativo objAtivo = new Ativo((string) objRS.Field("Codigo"), string.Empty);

			    var servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(objAtivo, _conexao);

				CotacaoDiaria objCotacaoDiaria = new CotacaoDiaria(objAtivo, pdtmDataAtual);
				objCotacaoDiaria.ValorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));

				objCotacaoDiaria.Medias.Add(new MediaDiaria(objCotacaoDiaria, "MME", 49, Convert.ToDouble(objRS.Field("MME49"))));
				objCotacaoDiaria.Medias.Add(new MediaDiaria(objCotacaoDiaria, "MME", 200, Convert.ToDouble(objRS.Field("MME200"))));

                servicoDeCotacaoDeAtivo.CotacoesDiarias.Add(objCotacaoDiaria);

				//Calcula a classificação da média
                ClassifMedia objClassifMedia = servicoDeCotacaoDeAtivo.ObterClassificacaoDeMediaNaData(pdtmDataAtual);

				//Calcula o número de tentativas
				int intNumTentativas = NumTentativasCalcular((string) objRS.Field("Codigo"), Convert.ToInt64(objRS.Field("Sequencial")), pobjIFRSobrevendido.ValorMaximo);

				//Verifica se atende a todos os critérios
				var objVerificaSeDeveGerarEntrada = new VerificaSeDeveGerarEntrada(_conexao);

				objValorCriterioCMVO.PercentualMM21 = Convert.ToDouble(objRS.Field("Percentual_MME21"));
				objValorCriterioCMVO.PercentualMM49 = Convert.ToDouble(objRS.Field("Percentual_MME49"));
				objValorCriterioCMVO.PercentualMM200 = Convert.ToDouble(objRS.Field("Percentual_MME200"));
				objValorCriterioCMVO.PercentualMM200MM21 = Convert.ToDouble(objRS.Field("Diferenca_MM200_MM21"));
				objValorCriterioCMVO.PercentualMM200MM49 = Convert.ToDouble(objRS.Field("Diferenca_MM200_MM49"));

				var objSimulacaoDiariaVO = new SimulacaoDiariaVO();
				objSimulacaoDiariaVO.Ativo = objAtivo;
				objSimulacaoDiariaVO.Setup = pobjSetup;
				objSimulacaoDiariaVO.ClassificacaoMedia = objClassifMedia;
				objSimulacaoDiariaVO.DataEntradaEfetiva = pdtmDataAtual;
				objSimulacaoDiariaVO.IFRSobrevendido = pobjIFRSobrevendido;
				objSimulacaoDiariaVO.NumTentativas = intNumTentativas;

				//Verifica se deve gerar entrada: atende a todos os critérios de classificação da média, quantidade de tentativas e percentual de acertos.
				//Caso deva gerar entrada, retorna uma lista das faixas em que o trade se faixa (uma faixa para cada critério).
				int intSomatorioCriterios = objVerificaSeDeveGerarEntrada.Verificar(objSimulacaoDiariaVO, objValorCriterioCMVO, lstFaixas);

				if (intSomatorioCriterios == 0) {
					//Se todos os critérios foram atendidos, calcula o valor de realização parcial e final em função das faixas selecionadas
					CalcularValoresRealizacao((string) objRS.Field("Codigo"), pobjSetup, objClassifMedia, pobjIFRSobrevendido, lstFaixas, Convert.ToDecimal(objRS.Field("ValorFechamento")), pdtmDataAtual, ref strValorRealizacaoParcial, ref strValorRealizacaoFinal);

				}

				string strAproveitamento = AproveitamentoConsultar(objSimulacaoDiariaVO);

				string strCodigoAtivo = objRS.Field("Codigo").ToString();


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
				strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(strCodigoAtivo);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDouble(objRS.Field("IFR2")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("ValorFechamento")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(strValorRealizacaoParcial);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(strValorRealizacaoFinal);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("stop_loss")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("perc_stop_loss")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToInt32(objRS.Field("Quantidade_Sem_Manejo")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("Valor_Total_Sem_Manejo")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDouble(objRS.Field("Perc_Risco_Capital")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToInt32(objRS.Field("Quantidade_Manejo")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("Valor_Manejo")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Convert.ToDouble(objRS.Field("Perc_Risco_Manejo")));
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objClassifMedia.ID);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(strAproveitamento);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objValorCriterioCMVO.PercentualMM21);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objValorCriterioCMVO.PercentualMM49);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objValorCriterioCMVO.PercentualMM200);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objValorCriterioCMVO.PercentualMM200MM21);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objValorCriterioCMVO.PercentualMM200MM49);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumTentativas);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intSomatorioCriterios) + ")";

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
			strSQL = strSQL + " WHERE ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;
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
		private string SetupIFR2ComFiltroQueryGerar(DateTime pdtmDataAnterior, DateTime pdtmDataAtual, string pstrTabelaCotacao, string pstrTabelaMedia, string pstrTabelaIFR, bool pblnAlijamentoCalcular, bool pblnAcimaMME49, decimal pdecValorCapital, decimal pdecValorPerdaManejo, double pdblTitulosTotal = -1,
		Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1, decimal pdecPercentualStopGain = -1)
		{
		    string strQuery = " SELECT segundo_dia.Codigo, segundo_dia.ValorFechamento " + ", ROUND(segundo_dia.Valor, 2) AS IFR2" + Environment.NewLine + ", MMExp49 AS Valor_MME49 " + ", ROUND((ValorFechamento / MMExp49 - 1) * 100, 4) AS Perc_MME49 " + Environment.NewLine + ", ROUND(segundo_dia.Valor_Entrada, 2) As entrada " + ", ROUND(((segundo_dia.Valor_Entrada) / segundo_dia.ValorFechamento - 1) * 100, 4) As perc_entrada " + ", ROUND(segundo_dia.Valor_Stop_Loss, 2) As stop_loss " + ", ROUND(((segundo_dia.Valor_Stop_Loss) / (segundo_dia.Valor_Entrada) -1) * 100, 4) As perc_stop_loss ";

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

			if (pdecPercentualStopGain != -1) {
				strQuery = strQuery + ", ROUND((segundo_dia.Valor_Entrada) " + "* (1 + " + FuncoesBd.CampoDecimalFormatar(pdecPercentualStopGain) + " / 100), 2) AS STOP_GAIN ";

			}

		    //****início cálculo da quantidade e valor total que pode ser comprada com todo o capital disponível.

			//cálculo da quantidade

			//1)divide o total do capital pelo valor de entrada e por 100, pois os lotes são de 100 ações.
			//com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			string strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + " / segundo_dia.Valor_Entrada / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Capital" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * (segundo_dia.Valor_Entrada) AS Valor_Capital" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando todo o capital sem manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (segundo_dia.Valor_Entrada - segundo_dia.Valor_Stop_Loss) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Capital " + Environment.NewLine;

			//****Fim dos cálculos com todo o capital.

			//****início cálculo da quantidade e valor total que pode ser comprada com o manejo de risco.

			//cálculo da quantidade

			//1)divide o total do capital que pode ser perdido pelo valor de perda por ação e por 100, 
			//pois os lotes são de 100 ações. com isso teremos o nº de lotes em decimais, ou seja pode haver lotes quebrados
			strAux = "CSTR(" + FuncoesBd.CampoDecimalFormatar(pdecValorPerdaManejo) + " / " + "(segundo_dia.Valor_Entrada - segundo_dia.Valor_Stop_Loss) / 100)";

			//2) transforma o número em uma string e retorna apenas os números à esquerda da vírgula.
			//Com isso temos o número de lotes inteiros que podem ser comprados. Transformamos então
			//esse número em inteiro novamente e multiplicamos por 100 obtendo assim o total de ações que podem ser compradas

			strAux = "CINT(LEFT(" + strAux + ", " + " IIF(INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") > 0 " + ", INSTR(" + strAux + "," + FuncoesBd.CampoStringFormatar(",") + ") - 1" + "," + "LEN(" + strAux + ")" + "))) * 100";

			strQuery = strQuery + ", " + strAux + " AS Quantidade_Manejo" + Environment.NewLine;

			//multiplica a quantidade de ações que podem ser compradas com todo o capital pelo valor de entrada
			//para obter o valor total que pode será gasto. 
			strQuery = strQuery + ", " + strAux + " * segundo_dia.Valor_Entrada AS Valor_Manejo" + Environment.NewLine;

			//obtem o percentual de risco em relação ao capital total se for aplicar utilizando manejo de risco.
			//Para isso multiplica a quantidade que pode ser comprada pela perda por ação (entrada - stop)
			strQuery = strQuery + ", ROUND((" + strAux + " * (segundo_dia.Valor_Entrada -  segundo_dia.Valor_Stop_Loss) " + "/ " + FuncoesBd.CampoDecimalFormatar(pdecValorCapital) + ") * 100, 4)  AS Perc_Risco_Manejo " + Environment.NewLine;

			//****Fim dos cálculos com manejo de risco.


			if (pblnAlijamentoCalcular) {
				strQuery = strQuery + ", ROUND(segundo_dia.Valor_Entrada * 2 - segundo_dia.Valor_Stop_Loss, 2)  as STOP_GAIN ";
			}

			//percentual do valor de fechamento em relação à média de 21 períodos
			strQuery = strQuery + ", ROUND((ValorFechamento / MMExp21 - 1) * 100, 4) AS Perc_MME21 ";

			strQuery = strQuery + " FROM " + Environment.NewLine + "(" + Environment.NewLine + '\t' + " SELECT " + pstrTabelaCotacao + ".Codigo " + Environment.NewLine + '\t' + " FROM ((" + pstrTabelaCotacao + " INNER JOIN " + pstrTabelaMedia + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaMedia + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaMedia + ".Data) " + Environment.NewLine + '\t' + " INNER JOIN " + pstrTabelaIFR + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = " + pstrTabelaIFR + ".Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = " + pstrTabelaIFR + ".Data) " + Environment.NewLine + '\t' + " WHERE " + pstrTabelaCotacao + ".Data = " + FuncoesBd.CampoDateFormatar(pdtmDataAnterior) + Environment.NewLine + '\t' + " And " + pstrTabelaMedia + ".Tipo = " + FuncoesBd.CampoStringFormatar("IFR2") + Environment.NewLine + '\t' + " And " + pstrTabelaMedia + ".NumPeriodos = 13 " + Environment.NewLine + '\t' + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + Environment.NewLine + '\t' + " And " + pstrTabelaIFR + ".Valor < " + pstrTabelaMedia + ".Valor " + Environment.NewLine + ") As primeiro_dia " + Environment.NewLine + " INNER JOIN " + Environment.NewLine + "(" + Environment.NewLine + '\t' + " SELECT " + pstrTabelaCotacao + ".Codigo, valorfechamento " + '\t' + ", MME21.Valor AS MMExp21, MME49.Valor AS MMExp49, " + pstrTabelaIFR + ".Valor" + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + ", ValorMinimo - IIF(ROUND(ValorMinimo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBd.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMinimo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBd.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Stop_Loss " + Environment.NewLine;

			//Para calcular o valor de entrada aplica 0,25% sobre o valor máximo do dia em que o IFR cruza a média. 
			//Se este valor for maior ou igual a 0, 01 soma este valor ao valor máximo. Caso contrário soma 0,01 (1 centavo)
			//ao valor máximo.
			strQuery = strQuery + ", ValorMaximo + IIF(ROUND(ValorMaximo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2) >= " + FuncoesBd.CampoDecimalFormatar(0.01M) + ", ROUND(ValorMaximo * " + FuncoesBd.CampoDecimalFormatar(0.0025M) + ", 2), " + FuncoesBd.CampoDecimalFormatar(0.01M) + ")" + " AS Valor_Entrada " + Environment.NewLine;

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
			strTabela = "(" + strTabela + '\t' + " LEFT JOIN " + Environment.NewLine + '\t' + "(" + Environment.NewLine + '\t' + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + '\t' + " WHERE Data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + '\t' + " AND Tipo = " + FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine + '\t' + '\t' + " And NumPeriodos = 21 " + Environment.NewLine + '\t' + ") AS MME21 " + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = MME21.Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = MME21.Data) " + Environment.NewLine;

			//GERA TABELA PARA BUSCAR A MÉDIA MÓVEL DE 49 PERÍODOS.
			//NÃO PODE FAZER SIMPLESMENTE UM LEFT JOIN COM  A TABELA DE MÉDIAS
			//PORQUE O WHERE PELO TIPO = 'MME' AND NUMPERIODOS = 49 FAZ COM QUE OS REGISTROS
			//QUE AINDA NÃO TENHAM A MÉDIA DE 49 NÃO SEJAM CONSIDERADOS.
			//POR ISSO TEMOS QUE CRIAR UM SELECT INTERNO QUE FORMA A TABELA MME49
			strTabela = "(" + strTabela + '\t' + " LEFT JOIN " + Environment.NewLine + '\t' + "(" + Environment.NewLine + '\t' + '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + '\t' + "FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + '\t' + " WHERE Data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + Environment.NewLine + '\t' + '\t' + " AND Tipo = " + FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine + '\t' + '\t' + " And NumPeriodos = 49 " + Environment.NewLine + '\t' + ") AS MME49 " + Environment.NewLine + '\t' + " On " + pstrTabelaCotacao + ".Codigo = MME49.Codigo " + Environment.NewLine + '\t' + " And " + pstrTabelaCotacao + ".Data = MME49.Data) " + Environment.NewLine;

			strQuery = strQuery + '\t' + " FROM " + strTabela + '\t' + " WHERE " + pstrTabelaCotacao + ".Data = " + FuncoesBd.CampoDateFormatar(pdtmDataAtual) + '\t' + " And " + pstrTabelaMedia + ".Tipo = " + FuncoesBd.CampoStringFormatar("IFR2") + '\t' + " And " + pstrTabelaMedia + ".NumPeriodos = 13 " + '\t' + " And " + pstrTabelaIFR + ".NumPeriodos = 2 " + '\t' + " And " + pstrTabelaIFR + ".Valor > " + pstrTabelaMedia + ".Valor ";


			if (pblnAcimaMME49) {
				strQuery = strQuery + '\t' + " And " + pstrTabelaCotacao + ".ValorFechamento >= MME49.Valor " + Environment.NewLine;

			}

			if (pdblTitulosTotal != -1) {
				strQuery = strQuery + '\t' + " And MV.Tipo = " + FuncoesBd.CampoStringFormatar("VMA") + Environment.NewLine + '\t' + " And MV.NumPeriodos = 21 " + Environment.NewLine + '\t' + " And MV.Valor >= " + FuncoesBd.CampoFloatFormatar(pdblTitulosTotal) + Environment.NewLine;

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
		Int32 pintNegociosTotal = -1, decimal pdecValorTotal = -1, IFRSobrevendido pobjIFRSobrevendido = null)
		{
			string functionReturnValue;

			ServicoDeCotacao objCotacao = new ServicoDeCotacao(_conexao);

			System.DateTime dtmDataAtual;
			System.DateTime dtmDataAnterior = default(System.DateTime);

			string strTabelaCotacao = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "COTACAO", "COTACAO_SEMANAL")
			string strTabelaMedia = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "MEDIA_DIARIA", "MEDIA_SEMANAL")
			string strTabelaIFR = String.Empty;
			//= IIf(pstrPeriodo = "DIARIO", "IFR_DIARIO", "IFR_SEMANAL")

			//valor máximo que pode ser perdido utilizando o manejo.
			decimal decValorPerdaManejo = pdecCapitalTotal * pdecPercentualManejo / 100;

		    var cotacaoData = new CotacaoData();
            cCalculadorTabelas.TabelasCalcular(pstrPeriodo, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

			//verifica se a data recebida é uma data de cotação
			if (pstrPeriodo == "DIARIO") {

				dtmDataAtual = _cotacaoData.CotacaoDataExistir(pdtmData, strTabelaCotacao) ? pdtmData : objCotacao.CotacaoAnteriorDataConsultar(pdtmData, strTabelaCotacao);


			} else
			{
			    
			    dtmDataAtual = cotacaoData.CotacaoSemanalPrimeiroDiaSemanaCalcular(pdtmData);

			}

			//verifica se o setup utiliza cotacao de data anterior. se utiliza tem que buscar a data anterior.
			//o único que não utiliza é o IFR 2 abaixo de 5.

			if (pintSetup != cEnum.enumSetup.IFRSemFiltro && pintSetup != cEnum.enumSetup.IFRSemFiltroRP)
			{
			    dtmDataAnterior = pstrPeriodo == "DIARIO" ? objCotacao.CotacaoAnteriorDataConsultar(dtmDataAtual, strTabelaCotacao) : cotacaoData.CotacaoSemanalPrimeiroDiaSemanaCalcular(dtmDataAtual.AddDays(-1));
			}


			if (pstrPeriodo == "SEMANAL") {

				if (pdblTitulosTotal != -1 || pintNegociosTotal != -1 || pdecValorTotal != -1) {
					//Os filtros referentes ao volume negociado são informados em dias.
					//Quando as cotações são semanais, tem que verificar quantos dias tem na semana e multiplicar
					//os valores informados pelo número de dias
					cRS objRS = new cRS(_conexao);

					//Dim strQuery As String


				    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

					//primeiro busca a data final da cotação semanal
					objRS.ExecuteQuery(" SELECT Max(datafinal) As datafinal" + " FROM cotacao_semanal " + " WHERE Data = " + funcoesBd.CampoDateFormatar(dtmDataAtual));

					DateTime dtmCotacaoSemanalDataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));

					objRS.Fechar();

				    string subTabela = " SELECT codigo, Count(1) As contador " + " FROM cotacao " + " WHERE Data >= " +
				                       funcoesBd.CampoDateFormatar(dtmDataAtual) + " And data <= " +
				                       funcoesBd.CampoDateFormatar(dtmCotacaoSemanalDataFinal) + " GROUP BY codigo ";

                    objRS.ExecuteQuery(" SELECT Max (contador) As contador " + " FROM " + funcoesBd.FormataSubSelect(subTabela));


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

					CarregadorSetup objCarregorSetup = new CarregadorSetup();

					Setup objSetup = objCarregorSetup.CarregaPorId(pintSetup);

					functionReturnValue = RelatIFR2SemFiltroDiarioPersonalizadoGerar(objSetup, dtmDataAtual, pdecCapitalTotal, decValorPerdaManejo, pobjIFRSobrevendido, pdblIFR2LimiteSuperior, pdblTitulosTotal, pintNegociosTotal, pdecValorTotal);

					break;
				default:

					functionReturnValue = String.Empty;

					break;
			}
			return functionReturnValue;

		}

		/// <summary>
		/// Ainda não está preparada para as cotações semanais
		/// </summary>
		/// <param name="pstrAliasTabelaCotacaoPrincipal"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FiltroVolumeNegociosGerar(string pstrAliasTabelaCotacaoPrincipal, string pstrTabelaCotacaoFiltro, Int32 pintValor)
		{
		    //Calcula a média de 21 dias do volume de negócios e compara com o valor atual
			var filtro = "(" +  _funcoesBd.CampoFormatar(pintValor) + " < " + Environment.NewLine;
			filtro = filtro + "(" + Environment.NewLine;
			filtro = filtro + '\t' + " SELECT AVG(Negocios_Total) " + Environment.NewLine;
			filtro = filtro + '\t' + " FROM " + pstrTabelaCotacaoFiltro + " CN " + Environment.NewLine;
			filtro = filtro + '\t' + " WHERE " + pstrAliasTabelaCotacaoPrincipal + ".Codigo = CN.Codigo " + Environment.NewLine;
			filtro = filtro + '\t' + " AND CN.Sequencial >= (" + pstrAliasTabelaCotacaoPrincipal + ".Sequencial - 20) " + Environment.NewLine;
			filtro = filtro + "))";


			return filtro;

		}

		/// <summary>
		/// Ainda não está preparada para as cotações semanais
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public string FiltroVolumeFinanceiroGerar(string pstrAliasTabelaCotacaoPrincipal, string pstrTabelaCotacaoFiltro, decimal pdecValor)
		{
		    //Calcula a média de 21 dias do volume financeiro e compara com o valor atual
			var strFiltro = "(" + _funcoesBd.CampoFormatar(pdecValor) + " < " + Environment.NewLine;
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
