using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using prjDominio.ValueObjects;
using TraderWizard.Infra.Repositorio;

namespace ServicoNegocio
{

	public class CalculadorResumoIFRDiario
    {

		private readonly Conexao _conexao;
		private readonly Ativo _ativo;

		private readonly Setup _setup;
		public CalculadorResumoIFRDiario(Conexao pobjConexao, Setup pobjSetup, Ativo pobjAtivo)
		{
			_conexao = pobjConexao;
			_ativo = pobjAtivo;
			_setup = pobjSetup;
		}

		public bool Calcular(IFRSobrevendido pobjIFRSobrevendido, CalculoFaixaResumoVO pobjCalculoResumoFaixaVO)
		{


			try {
				RS objRS = new RS(_conexao);

			    IFRSimulacaoDiariaFaixaResumo objRetorno = new IFRSimulacaoDiariaFaixaResumo(_ativo, _setup, pobjCalculoResumoFaixaVO.ClassifMedia, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida);


				CarregadorCriterioClassificacaoMedia objCarregadorCriterioClassifMedia = new CarregadorCriterioClassificacaoMedia();

				IList<CriterioClassifMedia> criteriosDeClassificacao = objCarregadorCriterioClassifMedia.CarregaTodos();

			    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

			    string strWherePadrao = " WHERE Codigo = " + funcoesBd.CampoFormatar(_ativo.Codigo) + Environment.NewLine;
				strWherePadrao += " AND ID_Setup = " + funcoesBd.CampoFormatar(_setup.Id) + Environment.NewLine;
				strWherePadrao += " AND ID_CM = " + funcoesBd.CampoFormatar(pobjCalculoResumoFaixaVO.ClassifMedia.ID) + Environment.NewLine;
				strWherePadrao += " AND Valor_IFR_Minimo <= " + pobjIFRSobrevendido.ValorMaximo + Environment.NewLine;
				strWherePadrao += " AND Data_Saida <= " + funcoesBd.CampoFormatar(pobjCalculoResumoFaixaVO.DataSaida) + Environment.NewLine;


				//Calcula o Número de Trades total e certos, sem utilizar filtro
				string strSQL = "SELECT COUNT(1) AS NumTrades ";
				strSQL = strSQL + " , SUM(IIF(Verdadeiro = " + funcoesBd.CampoFormatar(true) + " , 1, 0 )) AS NumAcertos " + Environment.NewLine;
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

			    CarregadorIFRDiarioFaixa objCarradorFaixa = new CarregadorIFRDiarioFaixa(_conexao);

				//Verifica se já existe faixa para o critério. Vai existir quando já houver alguma entrada que é MELHOR ENTRADA

				if (objCarradorFaixa.ExisteFaixaParaCriterio(_ativo.Codigo, _setup, pobjCalculoResumoFaixaVO.ClassifMedia, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida)) {

					foreach (CriterioClassifMedia objCriterioCM in criteriosDeClassificacao) {
						//busca lista de faixas (pode ser 1 ou 2) para o critério do classificação de média
						IList<IFRSimulacaoDiariaFaixa> lstFaixas = objCarradorFaixa.CarregaUltimaFaixaAteDataPorCriterioClassificacaoMedia(_ativo.Codigo, _setup, pobjCalculoResumoFaixaVO.ClassifMedia, objCriterioCM, pobjIFRSobrevendido, pobjCalculoResumoFaixaVO.DataSaida);

						string strSQLFaixa = string.Empty;

						//para cada um dos critérios gera uma cláusula que filtra os registros verificando se o valor do critério na tabela IFR_Simulacao_Diaria 
						//entre os valores mínimo e máximo de cada uma das faixas do respectivo critério.

						foreach (IFRSimulacaoDiariaFaixa objIFRSimulacaoDiariaFaixa in lstFaixas) {
							if (strSQLFaixa != string.Empty) {
								strSQLFaixa += " OR ";
							}

							strSQLFaixa += '\t' + " ((" + objCriterioCM.CampoBD + ") BETWEEN " + funcoesBd.CampoFormatar(objIFRSimulacaoDiariaFaixa.ValorMinimo) +
                                " AND " + funcoesBd.CampoFormatar(objIFRSimulacaoDiariaFaixa.ValorMaximo) + ")" + Environment.NewLine;

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

			    var repositorio = new RepositorioDeIfrSimulacaoDiariaFaixaResumo(_conexao);
				repositorio.Salvar(objRetorno);

				return true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

			}

		}


	}
}
