using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo
{
	public class cCalculadorTabelas
	{

		/// <summary>
		/// Retorna o nome das tabelas de cotação, média e IFR de acordo com a periodicidade recebida por parâmetro
		/// </summary>
		/// <param name="pstrPeriodicidade">DIARIO ou SEMANAL</param>
		/// <param name="pstrTabelaCotacaoRet">retorna o nome da tabela de cotações</param>
		/// <param name="pstrTabelaMediaRet">retorna o nome da tabela de médias</param>
		/// <param name="pstrTabelaIFRRet">retorna o nome da tabela de IFR</param>
		/// <remarks></remarks>

		public static void TabelasCalcular(string pstrPeriodicidade, ref string pstrTabelaCotacaoRet, ref string pstrTabelaMediaRet, ref string pstrTabelaIFRRet)
		{

			if (pstrPeriodicidade == "DIARIO") {
				pstrTabelaCotacaoRet = "COTACAO";
				pstrTabelaMediaRet = "MEDIA_DIARIA";
				pstrTabelaIFRRet = "IFR_DIARIO";


			} else if (pstrPeriodicidade == "SEMANAL") {
				pstrTabelaCotacaoRet = "COTACAO_SEMANAL";
				pstrTabelaMediaRet = "MEDIA_SEMANAL";
				pstrTabelaIFRRet = "IFR_SEMANAL";


			} else {
				pstrTabelaCotacaoRet = String.Empty;
				pstrTabelaMediaRet = String.Empty;
				pstrTabelaIFRRet = String.Empty;

			}

		}
	}
}
