using System.Windows.Forms;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using prjModelo.ValueObjects;
namespace prjServicoNegocio
{

	public class cCalculadorResumoIFRDiario
	{

		private readonly cConexao objConexao;
		private readonly cAtivo objAtivo;

		private readonly Setup objSetup;
		public cCalculadorResumoIFRDiario(cConexao pobjConexao, Setup pobjSetup, cAtivo pobjAtivo)
		{
			objConexao = pobjConexao;
			objAtivo = pobjAtivo;
			objSetup = pobjSetup;
		}

		public bool Calcular(cIFRSobrevendido pobjIFRSobrevendido, cCalculoFaixaResumoVO pobjCalculoResumoFaixaVO)
		{


			try {
				cRS objRS = new cRS(objConexao);

				string strSQL = null;

				cIFRSimulacaoDiariaFaixaResumo objRetorno = new cIFRSimulacaoDiariaFaixaResumo(objAtivo, objSetup, pobjCalculoResumoFaixaVO.ClassifMedia, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida);

				cCarregadorCriterioClassificacaoMedia objCarregadorCriterioClassifMedia = new cCarregadorCriterioClassificacaoMedia();

				IList<cCriterioClassifMedia> lstCriteriosCM = objCarregadorCriterioClassifMedia.CarregaTodos();

				string strWherePadrao = null;

				strWherePadrao = " WHERE Codigo = " + FuncoesBD.CampoFormatar(objAtivo.Codigo) + Environment.NewLine;
				strWherePadrao += " AND ID_Setup = " + FuncoesBD.CampoFormatar(objSetup.ID) + Environment.NewLine;
				strWherePadrao += " AND ID_CM = " + FuncoesBD.CampoFormatar(pobjCalculoResumoFaixaVO.ClassifMedia.ID) + Environment.NewLine;
				strWherePadrao += " AND Valor_IFR_Minimo <= " + pobjIFRSobrevendido.ValorMaximo + Environment.NewLine;
				strWherePadrao += " AND Data_Saida <= " + FuncoesBD.CampoFormatar(pobjCalculoResumoFaixaVO.DataSaida) + Environment.NewLine;


				//Calcula o Número de Trades total e certos, sem utilizar filtro
				strSQL = "SELECT COUNT(1) AS NumTrades ";
				strSQL = strSQL + " , SUM(IIF(Verdadeiro = " + FuncoesBD.CampoFormatar(true) + " , 1, 0 )) AS NumAcertos " + Environment.NewLine;
				strSQL = strSQL + " FROM IFR_Simulacao_Diaria D " + Environment.NewLine;
				strSQL = strSQL + strWherePadrao;

				objRS.ExecuteQuery(strSQL);

				objRetorno.NumTradesSemFiltro = Convert.ToInt32(objRS.Field("NumTrades"));
				objRetorno.NumAcertosSemFiltro = Convert.ToInt32(objRS.Field("NumAcertos"));

				objRS.Fechar();

				if (objRetorno.NumTradesSemFiltro == 0) {
					//caso não haja trade para a classificação retorna TRUE
					return true;
				}

				IList<cIFRSimulacaoDiariaFaixa> lstFaixas = null;

				cCarregadorIFRDiarioFaixa objCarradorFaixa = new cCarregadorIFRDiarioFaixa(objConexao);

				//Verifica se já existe faixa para o critério. Vai existir quando já houver alguma entrada que é MELHOR ENTRADA

				if (objCarradorFaixa.ExisteFaixaParaCriterio(objAtivo.Codigo, objSetup, pobjCalculoResumoFaixaVO.ClassifMedia, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida)) {

					foreach (cCriterioClassifMedia objCriterioCM in lstCriteriosCM) {
						//busca lista de faixas (pode ser 1 ou 2) para o critério do classificação de média
						lstFaixas = objCarradorFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(objAtivo.Codigo, objSetup, pobjCalculoResumoFaixaVO.ClassifMedia, objCriterioCM, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida);

						string strSQLFaixa = string.Empty;

						//para cada um dos critérios gera uma cláusula que filtra os registros verificando se o valor do critério na tabela IFR_Simulacao_Diaria 
						//entre os valores mínimo e máximo de cada uma das faixas do respectivo critério.

						foreach (cIFRSimulacaoDiariaFaixa objIFRSimulacaoDiariaFaixa in lstFaixas) {
							if (strSQLFaixa != string.Empty) {
								strSQLFaixa += " OR ";
							}

							strSQLFaixa += '\t' + " ((" + objCriterioCM.CampoBD + ") BETWEEN " + FuncoesBD.CampoFormatar(objIFRSimulacaoDiariaFaixa.ValorMinimo) + " AND " + FuncoesBD.CampoFormatar(objIFRSimulacaoDiariaFaixa.ValorMaximo) + ")" + Environment.NewLine;

						}

						if (strSQLFaixa != string.Empty) {
							strSQL = strSQL + "AND (" + strSQLFaixa + ")";
						}

						//FECHA O EXISTS
						//strSQL = strSQL & ")"

						lstFaixas.Clear();

					}

					objRS.ExecuteQuery(strSQL);

					objRetorno.NumTradesComFiltro = Convert.ToInt32(objRS.Field("NumTrades"));
					objRetorno.NumAcertosComFiltro = Convert.ToInt32(objRS.Field("NumAcertos"));

					objRS.Fechar();


				} else {
					objRetorno.NumTradesComFiltro = 0;
					objRetorno.NumAcertosComFiltro = 0;

				}

				objRetorno.Salvar(objConexao);

				return true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

			}

		}


	}
}
