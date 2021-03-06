﻿using System;
using Configuracao;
using DataBase;
using DTO;
using TraderWizard.Enumeracoes;
using WebAccess;

namespace ServicoNegocio
{

	public class CalculadorData
	{

		private readonly Conexao _conexao;

		public CalculadorData(Conexao pobjConexao)
		{
			_conexao = pobjConexao;

		}

	    public enum EnumAtualizacaoDiariaTipo
	    {
	        BoletimDiario = 1,
	        Historica = 2,
	        IntraDay = 3
	    }

	    /// <summary>
        /// Calcula a primeira data anterior à data informada, que é um dia útil.
        /// </summary>
        /// <param name="pdtmData">Data base para o cálculo</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime DiaUtilAnteriorCalcular(DateTime pdtmData)
		{
		    bool blnOK;

			DateTime dtmData = pdtmData;

			do {
				//busca o dia anterior
				dtmData = dtmData.AddDays(-1);

				//chama função que verifica se a data é um dia útil.
				//considera o dia da semana e se a data é um feriado
				blnOK = DiaUtilVerificar(dtmData);

			} while (!blnOK);

			return dtmData;

		}

		/// <summary>
		/// Calcula a primeira data posterior à data informada que é um dia útil.
		/// </summary>
		/// <param name="pdtmData">Data base para o cálculo</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime DiaUtilSeguinteCalcular(DateTime pdtmData)
		{
		    bool encontrado;

			DateTime dtmData = pdtmData;


			do {
				//busca o dia anterior
				dtmData = dtmData.AddDays(1);

				//chama função que verifica se a data é um dia útil.
				//considera o dia da semana e se a data é um feriado
				encontrado = DiaUtilVerificar(dtmData);

			} while (!(encontrado));

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
			bool functionReturnValue;

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();
			//verifica se o dia da semana está entre segunda-feira e sexta-feira

			if ((pdtmData.DayOfWeek != DayOfWeek.Sunday) && (pdtmData.DayOfWeek != DayOfWeek.Saturday)) {
				RS objRS = new RS(_conexao);

				//se está entre segunda e sexta verifica se a data não está cadastrada na tabela de feriados
				objRS.ExecuteQuery(" select 1" + " from Feriado " + " where Data = " + funcoesBd.CampoDateFormatar(pdtmData));

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
		public DateTime CalcularDataProximoPeriodo(string pstrCodigo, DateTime pdtmDataAtual, string pstrTabelaCotacao)
		{
		    RS objRS = new RS(_conexao);

            FuncoesBd funcoesBd  = _conexao.ObterFormatadorDeCampo();

		    string strSql = "SELECT TOP 1 Data " + Environment.NewLine;
			strSql = strSql + "FROM " + pstrTabelaCotacao + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSql = strSql + " AND Data > " + funcoesBd.CampoFormatar(pdtmDataAtual);
			strSql = strSql + " ORDER BY Data ";

			objRS.ExecuteQuery(strSql);

			DateTime functionReturnValue = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

			objRS.Fechar();
			return functionReturnValue;

		}

		public DateTime ObterDataDaUltimaCotacao()
		{
		    RS objRS = new RS(_conexao);

			//busca a data da última cotação armazenada

			objRS.ExecuteQuery("SELECT Data_Ultima_Cotacao" + " FROM Resumo");

			//busca a próxima data que é um dia útil, após a última cotação.
			DateTime dataDaUltimaCotaco = Convert.ToDateTime(objRS.Field("Data_Ultima_Cotacao"));

			objRS.Fechar();
			return dataDaUltimaCotaco;

		}

		public SugerirAtualizacaoCotacaoDTO SugerirAtualizarCotacao(EnumAtualizacaoDiariaTipo tipo)
		{

			//data que tem que buscar a próxima cotação.
		    DateTime dtmDataFinal;

			DateTime dtmDataInicial = DiaUtilSeguinteCalcular(ObterDataDaUltimaCotacao());

		    bool hojeDiaUtil = DiaUtilVerificar(DateTime.Now);
		    if (hojeDiaUtil && DateTime.Now.Hour >= 18) {
				dtmDataFinal = DateTime.Now;
			} else {
				//Calcula o dia útil anterior à data atual
				dtmDataFinal = DiaUtilAnteriorCalcular(DateTime.Now);
			}

			if (dtmDataFinal >= dtmDataInicial) {
				return new SugerirAtualizacaoCotacaoDTO("online", dtmDataInicial, dtmDataFinal);
			}
		    return hojeDiaUtil && DateTime.Now.Hour >= 10 ? new SugerirAtualizacaoCotacaoDTO("daytrade", DateTime.Now.Date, DateTime.Now.Date) : null;
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
		    RS objRS = new RS(this._conexao);

		    var funcoesBd = this._conexao.ObterFormatadorDeCampo();

			objRS.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Codigo = " + funcoesBd.CampoStringFormatar(pstrCodigo));

			DateTime functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();
			return functionReturnValue;

		}
	}
}
