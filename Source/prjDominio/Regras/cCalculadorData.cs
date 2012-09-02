using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using frwInterface;
using prjDTO;
using pWeb;

namespace prjModelo.Regras
{

	public class cCalculadorData
	{


		private readonly cConexao objConexao;

		public cCalculadorData(cConexao pobjConexao)
		{
			objConexao = pobjConexao;

		}

		/// <summary>
		/// Calcula a primeira data anterior à data informada, que é um dia útil.
		/// </summary>
		/// <param name="pdtmData">Data base para o cálculo</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public System.DateTime DiaUtilAnteriorCalcular(System.DateTime pdtmData)
		{

			System.DateTime dtmData = default(System.DateTime);

			bool blnOK = false;

			dtmData = pdtmData;


			do {
				//busca o dia anterior
				dtmData = dtmData.AddDays(-1);

				//chama função que verifica se a data é um dia útil.
				//considera o dia da semana e se a data é um feriado
				blnOK = DiaUtilVerificar(dtmData);

			} while (!(blnOK));

			return dtmData;

		}

		/// <summary>
		/// Calcula a primeira data posterior à data informada que é um dia útil.
		/// </summary>
		/// <param name="pdtmData">Data base para o cálculo</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public System.DateTime DiaUtilSeguinteCalcular(System.DateTime pdtmData)
		{

			System.DateTime dtmData = default(System.DateTime);

			bool blnOK = false;

			dtmData = pdtmData;


			do {
				//busca o dia anterior
				dtmData = dtmData.AddDays(1);

				//chama função que verifica se a data é um dia útil.
				//considera o dia da semana e se a data é um feriado
				blnOK = DiaUtilVerificar(dtmData);

			} while (!(blnOK));

			return dtmData;

		}

		/// <summary>
		/// Verifica se uma data é dia útil.
		/// Um dia útil é uma data entre segunda-feira e sexta-feira.    
		/// Também verifica a tabela de feriados    
		/// </summary>
		/// <param name="pdtmData">Data que será verificado se é dia útil</param>
		/// <returns>
		/// True = a data informada é um dia útil.    
		/// False = a data informada não é um dia útil.        
		/// </returns>
		/// <remarks></remarks>
		///    
		public bool DiaUtilVerificar(DateTime pdtmData)
		{
			bool functionReturnValue = false;

			//verifica se o dia da semana está entre segunda-feira e sexta-feira

			if ((pdtmData.DayOfWeek != DayOfWeek.Sunday) && (pdtmData.DayOfWeek != DayOfWeek.Saturday)) {
				cRS objRS = new cRS(objConexao);

				//se está entre segunda e sexta verifica se a data não está cadastrada na tabela de feriados
				objRS.ExecuteQuery(" select 1" + " from Feriado " + " where Data = " + FuncoesBD.CampoDateFormatar(pdtmData));

				//se a data é um feriado retorna false, pois não é um dia útil.
				//caso contrário retorna true.
				functionReturnValue = !objRS.DadosExistir;

				objRS.Fechar();

			} else {
				//se é um sábado ou domingo retorna FALSE
				functionReturnValue = false;
			}
			return functionReturnValue;

		}


		/// <summary>
		/// Calcula a data do próximo período para um determinado ativo
		/// </summary>
		/// <param name="pstrCodigo">código do ativo</param>
		/// <param name="pdtmDataAtual">data base para calcular o próximo período</param>
		/// <param name="pstrTabelaCotacao">Nome da tabela de cotação: COTACAO ou COTACAO_SEMANAL</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public System.DateTime CalcularDataProximoPeriodo(string pstrCodigo, System.DateTime pdtmDataAtual, string pstrTabelaCotacao)
		{
			System.DateTime functionReturnValue = default(System.DateTime);

			cRS objRS = new cRS(objConexao);

			string strSQL = null;

			strSQL = "SELECT TOP 1 Data " + Environment.NewLine;
			strSQL = strSQL + "FROM " + pstrTabelaCotacao + Environment.NewLine;
			strSQL = strSQL + " WHERE Codigo = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND Data > " + FuncoesBD.CampoFormatar(pdtmDataAtual);
			strSQL = strSQL + " ORDER BY Data ";

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = Convert.ToDateTime(objRS.Field("Data", cConst.DataInvalida));

			objRS.Fechar();
			return functionReturnValue;

		}

		public System.DateTime ObtemDataDaUltimaCotacao()
		{
			System.DateTime functionReturnValue = default(System.DateTime);

			cRS objRS = new cRS(objConexao);

			//busca a data da última cotação armazenada

			objRS.ExecuteQuery("SELECT Data_Ultima_Cotacao" + " FROM Resumo");

			//busca a próxima data que é um dia útil, após a última cotação.
			functionReturnValue = Convert.ToDateTime(objRS.Field("Data_Ultima_Cotacao"));

			objRS.Fechar();
			return functionReturnValue;

		}

		public cSugerirAtualizacaoCotacaoDTO SugerirAtualizarCotacao()
		{

			//data que tem que buscar a próxima cotação.
			System.DateTime dtmDataInicial = default(System.DateTime);
			System.DateTime dtmDataFinal = default(System.DateTime);

			dtmDataInicial = DiaUtilSeguinteCalcular(ObtemDataDaUltimaCotacao());


			//Verifica se a data atual já tem cotação
			cWeb objWeb = new cWeb(objConexao);

			if (objWeb.VerificarLink("http://www.bmfbovespa.com.br/fechamento-pregao/bdi/" + cGeradorNomeArquivo.GerarNomeArquivoRemoto(DateTime.Now))) {
				dtmDataFinal = DateTime.Now;
			} else {
				//Calcula o dia útil anterior à data atual
				dtmDataFinal = DiaUtilAnteriorCalcular(DateTime.Now);
			}


			if (dtmDataFinal >= dtmDataInicial) {
				return new cSugerirAtualizacaoCotacaoDTO("online", dtmDataInicial, dtmDataFinal);

			}
		    return DiaUtilVerificar(DateTime.Now) ? new cSugerirAtualizacaoCotacaoDTO("daytrade", DateTime.Now.Date, DateTime.Now.Date) : null;
		}


		/// <summary>
		/// Consulta a maior data em que existe cotação para um determinado ativo
		/// </summary>
		/// <param name="pstrCodigo">Código do papel para o qual deve ser buscada a data máxima</param>
		/// <param name="pstrTabela">Tabela de cotação que deve ser consultada. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
		/// <returns>A maior data em que há cotação para o papel</returns>
		/// <remarks>A função é pública porque é utilizada também pela classe cRelatorio</remarks>
		public DateTime CotacaoDataMaximaConsultar(string pstrCodigo, string pstrTabela)
		{
		    cRS objRS = new cRS(this.objConexao);

			objRS.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Codigo = " + FuncoesBD.CampoStringFormatar(pstrCodigo));

			DateTime functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();
			return functionReturnValue;

		}


	}
}
