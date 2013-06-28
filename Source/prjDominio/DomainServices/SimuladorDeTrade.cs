using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDTO;
using prjModelo.Carregadores;
using DataBase;
using prjModelo.Entidades;
using prjModelo.Regras;

namespace prjModelo.DomainServices
{

	public class SimuladorDeTrade
	{

		private readonly cConexao objConexao;
		private readonly Setup objSetup;
		private readonly cAtivo objAtivo;

		private readonly IList<cIFRSobrevendido> lstIFRSobrevendido;
		public SimuladorDeTrade(cConexao pobjConexao, Setup pobjSetup, cAtivo pobjAtivo, IList<cIFRSobrevendido> plstIFRSobrevendido)
		{
			objConexao = pobjConexao;

			objSetup = pobjSetup;
			objAtivo = pobjAtivo;
			lstIFRSobrevendido = plstIFRSobrevendido;

		}

		/// <summary>
		/// Recebe uma data por parâmetro e obtém o menor IFR antes das cotações voltarem a subir
		/// </summary>
		/// <param name="pdtmData"></param>
		/// <param name="pstrTabelaCotacao"></param>
		/// <param name="pstrTabelaIFR"></param>
		/// <param name="pstrTabelaMedia"></param>
		/// <param name="pdecValorFechamentoRet">Retorna o Valor de Fechamento na data da menor cotação anterior</param>
		/// <param name="pdblMME21Ret">Retorna a MME 21 na data da menor cotação anterior </param>
		/// <param name="pdblMME49Ret">Retorna a MME 49 na data da menor cotação anterior</param>
		/// <returns>Retorna o IFR de dois períodos na data da menor cotação anterior</returns>
		/// <remarks></remarks>
		private double IFRMinimoAnteriorCalcular(System.DateTime pdtmData, string pstrTabelaCotacao, string pstrTabelaIFR
            , string pstrTabelaMedia, ref decimal pdecValorFechamentoRet, ref double? pdblMME21Ret, ref double? pdblMME49Ret)
		{

			cRS objRS = new cRS(objConexao);

			string strQuery = null;

			System.DateTime dtmDataInicial = default(System.DateTime);
			System.DateTime dtmDataFinal = default(System.DateTime);
			//Dim blnEncontrouPositivo As Boolean = False
			bool blnEncontrouNegativo = false;
			double dblIFR = 0;

			string strTabelaMME21 = null;
			string strTabelaMME49 = null;
			//Dim strTabelaMME200 As String

			//primeiro dia do mês.
            dtmDataInicial = new DateTime(pdtmData.Year, pdtmData.Month, 1);
			//começa com um dia antes da data recebida por parâmetro, depois será a última data do mês.
            dtmDataFinal = pdtmData.AddDays(-1);

			//While (Not blnEncontrouPositivo) Or (Not blnEncontrouNegativo)

			while ((!blnEncontrouNegativo)) {
				//enquanto não encontrou as oscilações positivas e negativas

				strTabelaMME21 = '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + " FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(objAtivo.Codigo) + Environment.NewLine + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + " AND NumPeriodos = 21 " + Environment.NewLine + '\t' + " AND Data >= " + FuncoesBD.CampoDateFormatar(dtmDataInicial) + Environment.NewLine + '\t' + " AND Data <= " + FuncoesBD.CampoDateFormatar(dtmDataFinal) + Environment.NewLine;

				strTabelaMME49 = '\t' + "SELECT Codigo, Data, Valor " + Environment.NewLine + '\t' + " FROM " + pstrTabelaMedia + Environment.NewLine + '\t' + " WHERE Codigo = " + FuncoesBD.CampoStringFormatar(objAtivo.Codigo) + Environment.NewLine + '\t' + " AND Tipo = " + FuncoesBD.CampoStringFormatar("MME") + Environment.NewLine + '\t' + " AND NumPeriodos = 49 " + Environment.NewLine + '\t' + " AND Data >= " + FuncoesBD.CampoDateFormatar(dtmDataInicial) + Environment.NewLine + '\t' + " AND Data <= " + FuncoesBD.CampoDateFormatar(dtmDataFinal) + Environment.NewLine;

				//Busca as cotações em ordem decrescente.
				strQuery = "SELECT C.ValorFechamento, C.Oscilacao, IFR.Valor, MME21.Valor AS MME21, MME49.Valor AS MME49 " + Environment.NewLine;

				strQuery += " FROM (((" + pstrTabelaCotacao + " C INNER JOIN " + pstrTabelaIFR + " IFR" + Environment.NewLine + " ON C.Codigo = IFR.Codigo " + Environment.NewLine + " AND C.Data = IFR.Data) " + Environment.NewLine + " LEFT JOIN " + Environment.NewLine + "(" + Environment.NewLine + strTabelaMME21 + ") MME21 " + Environment.NewLine + " ON C.Codigo = MME21.Codigo " + Environment.NewLine + " AND C.Data = MME21.Data) " + Environment.NewLine + " LEFT JOIN " + Environment.NewLine + "(" + Environment.NewLine + strTabelaMME49 + ") MME49 " + Environment.NewLine + " ON C.Codigo = MME49.Codigo " + Environment.NewLine + " AND C.Data = MME49.Data " + Environment.NewLine;

				strQuery += " WHERE C.Codigo = " + FuncoesBD.CampoStringFormatar(objAtivo.Codigo) + Environment.NewLine + " AND IFR.NumPeriodos = 2 " + Environment.NewLine + " AND C.Data >= " + FuncoesBD.CampoDateFormatar(dtmDataInicial) + Environment.NewLine + " AND C.Data <= " + FuncoesBD.CampoDateFormatar(dtmDataFinal) + Environment.NewLine + " ORDER BY C.Data DESC";

				objRS.ExecuteQuery(strQuery);

				//Não podemos esquecer que estamos percorrendo o recordset em ordem decrescente de data.
				//Primeiro tem que encontrar uma oscilação negativa. Depois disso tem que encontrar uma positiva.
				//Quando encontrar a positiva, o IFR da cotação anterior, que era positiva, será o IFR mínimo.

				//While (Not objRS.EOF) And ((Not blnEncontrouPositivo) Or (Not blnEncontrouNegativo))

				while ((!objRS.EOF) && (!blnEncontrouNegativo)) {

					if (Convert.ToDouble(objRS.Field("Oscilacao")) < 0) {
						//Se encontrou uma oscilação menor do que zero, marca que a mesma foi encontrada.
						//Este é o sinal para procurar a oscilação positiva e então sair do loop.
						blnEncontrouNegativo = true;

						//se a oscilção foi menor ou igual a zero atribui o IFR na variável
						dblIFR = Convert.ToDouble(objRS.Field("Valor"));

                        double valorMedia;

						if (!double.TryParse(Convert.ToString(objRS.Field("MME21")), out valorMedia)) {
							pdblMME21Ret = null;


						} else {
							pdblMME21Ret = valorMedia;

						}

						if (!double.TryParse(Convert.ToString(objRS.Field("MME49")), out valorMedia)) {
							pdblMME49Ret = null;
						} else {
							pdblMME49Ret = valorMedia;
						}

						//If Not IsNumeric(objRS.Field("MME200")) Then
						//    pdblMME200Ret = Nothing
						//Else
						//    pdblMME200Ret = CDbl(objRS.Field("MME200"))
						//End If

						pdecValorFechamentoRet = Convert.ToDecimal(objRS.Field("ValorFechamento", -1));

						//ElseIf CDbl(objRS.Field("Oscilacao")) > 0 And blnEncontrouNegativo Then

						//se encontrou uma oscilação positiva e já havia encontra uma negativa
						//então seta a variável para sair do loop
						//    blnEncontrouPositivo = True

					}

					objRS.MoveNext();

				}

				//If objRS.EOF And ((Not blnEncontrouPositivo) Or (Not blnEncontrouNegativo)) Then

				if (objRS.EOF && (!blnEncontrouNegativo)) {
					//se chegou no fim do RecordSet e não encontrou as oscilações positivas e negativas.
					//busca as datas do primeiro e último dia do mês anterior

					//Para obter o último dia do mês anterior, subtrai um dia do primeiro dia do mês atual
                    dtmDataFinal = dtmDataInicial.AddDays(-1);

					//para obter o primeiro dia do mês anterior, subtrai um mês do primeiro dia do mês atual
					dtmDataInicial = dtmDataInicial.AddMonths(-1);

				}

				objRS.Fechar();

			}

			return dblIFR;

		}


		public cIFRSimulacaoDiaria Simular(cCotacaoDiaria pobjCotacaoDeInicioDaSimulacao)
		{

			//Dim objRSSplit As cRS = Nothing
			//Dim objCarregadorSplit As New cCarregadorSplit(objConexao)
			IList<cDesdobramento> lstDesdobramentos = null;

			System.DateTime dtmDataInicial = default(System.DateTime);
			System.DateTime dtmDataFinal = default(System.DateTime);

			System.DateTime dtmDataAux = default(System.DateTime);

			//Indica se houve entrada efetiva na operação. Somente utilizada nas operações com filtro.
			//Nas operações sem filtro a entrada sempre ocorre, pois não há necessidade de confirmação
			//no próximo período
			bool blnEntradaEfetiva = false;

			//Indica se a operação foi encerrada. Pode ser encerrada após ter gerada a entrada efetiva
			//ou ter encerrada sem gerar a entrada efetiva.
			bool blnOperacaoEncerrada = false;

			decimal decValorEntradaAjustado = default(decimal);
			InformacoesDoTradeDTO objInformacoesDoTradeDTO = new InformacoesDoTradeDTO();

			//Dim dtmDataEntradaEfetiva As Date
			double dblValorIFRMinimo = 0;
			decimal decValorFechamentoMinimo = default(decimal);
			double? dblMME21Minima = 0;
			double? dblMME49Minima = 0;


			cCotacaoDiaria objCotacaoDeAcionamentoDoSetup = null;
			cCotacaoDiaria objCotacaoDeEntrada = null;
			cCotacaoDiaria objCotacaoDoValorMaximo = null;
			cCotacaoDiaria objCotacaoDeSaida = null;

			string strTabelaCotacao = String.Empty;
			string strTabelaMedia = String.Empty;
			string strTabelaIFR = String.Empty;

			cCalculadorTabelas.TabelasCalcular("DIARIO", ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

			List<cMediaDTO> lstMediasParaCarregar = new List<cMediaDTO>();

			lstMediasParaCarregar.Add(new cMediaDTO("E", 21, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("E", 49, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("E", 200, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("A", 13, "IFR2"));

			IList<cCotacaoDiaria> lstCotacoes = null;

			cCalculadorData objCalculadorData = new cCalculadorData(objConexao);

			objCotacaoDeAcionamentoDoSetup = pobjCotacaoDeInicioDaSimulacao.Clonar();


			if (objSetup.GeraEntradaNaCotacaoDoAcionamento) {
				objCotacaoDeEntrada = pobjCotacaoDeInicioDaSimulacao.Clonar();

			}

			objInformacoesDoTradeDTO.ValorDeEntradaOriginal = objSetup.CalculaValorEntrada(objCotacaoDeAcionamentoDoSetup);
			//Inicialmente o valor de entrada ajustado é o mesmo do valor de entrada original
			decValorEntradaAjustado = objInformacoesDoTradeDTO.ValorDeEntradaOriginal;

			//valor do stop loss de saída é dado pelo candle de acionamento do setup.
			objInformacoesDoTradeDTO.ValorDoStopLoss = objSetup.CalculaValorStopLossInicial(objCotacaoDeAcionamentoDoSetup);

			objInformacoesDoTradeDTO.ValorRealizacaoParcial = objSetup.CalculaValorRealizacaoParcial(objCotacaoDeAcionamentoDoSetup);

			//marca a operação como não encerrada antes de entrar no loop
			blnOperacaoEncerrada = false;


			if (objSetup.TemFiltro) {
				//nas operações com filtro tem que verificar se houve entrada efetiva ou se apenas
				//o setup foi acionado sem gerar entrada.
				blnEntradaEfetiva = false;

				//busca o valor mínimo anterior do IFR, MME21, MME49.
				dblValorIFRMinimo = IFRMinimoAnteriorCalcular(objCotacaoDeAcionamentoDoSetup.Data, strTabelaCotacao, strTabelaIFR, strTabelaMedia
                    , ref decValorFechamentoMinimo, ref dblMME21Minima, ref dblMME49Minima);

				objInformacoesDoTradeDTO.ValorIFRMinimo = dblValorIFRMinimo;
				objInformacoesDoTradeDTO.ValorFechamentoMinimo = decValorFechamentoMinimo;
				objInformacoesDoTradeDTO.MME21Minima = dblMME21Minima;
				objInformacoesDoTradeDTO.MME49Minima = dblMME49Minima;


				//objClassifMedia = cUtil.ClassifMediaCalcular(dblMME200Minima, dblMME49Minima, decValorFechamentoMinimo)

				//Marca variável que indica se o IFR cruzou a média como TRUE, porque o cruzamento é o própsrio sinal de entrada do setup.
				objInformacoesDoTradeDTO.IFRCruzouMediaParaCima = true;

			} else {
				//nas operações sem filtro a entrada sempre é gerada, pois entramos no fechamento.
				blnEntradaEfetiva = true;

				objInformacoesDoTradeDTO.ValorIFRMinimo = objCotacaoDeAcionamentoDoSetup.IFR.Valor;

				//Inicializa como false. Só irá marcar true quando ocorrer um cruzamento do IFR com a média
				objInformacoesDoTradeDTO.IFRCruzouMediaParaCima = false;

			}

			//começa 1 dia após a data de entrada
			//dtmDataInicial = DateAdd(DateInterval.Day, 1, CDate(objRSEntrada.Field("DATA_ENTRADA")))

			//dtmDataInicial = objCalculadorData.DiaUtilSeguinteCalcular(objCotacaoDeAcionamentoDoSetup.Data)
			dtmDataInicial = objCalculadorData.CalcularDataProximoPeriodo(objAtivo.Codigo, objCotacaoDeAcionamentoDoSetup.Data, strTabelaCotacao);


			while (!blnOperacaoEncerrada) {
				//O período é sempre calculado mês a mês para não gerar um overhead muito grande.
				//Como o gráfico é diário dificilmente a operação vai ficar mais do que um mês
				//sendo executada. O que pode acontecer com mais frequencia é de operação iniciar
				//em um mês e terminar no mês seguinte.

				//último dia do mês

				//soma 1 mês na data inicial
                dtmDataAux = dtmDataInicial.AddMonths(1);

				//obtém o primeiro dia do próximo mês
                dtmDataAux = new DateTime(dtmDataAux.Year, dtmDataAux.Month, 1);

				//último dia do mês atual é um dia antes do primeiro dia do próximo mês
                dtmDataFinal = dtmDataAux.AddDays(-1);

				objAtivo.CarregarCotacoes(dtmDataInicial, dtmDataFinal, lstMediasParaCarregar, true);

				lstCotacoes = objAtivo.CotacoesDiarias.Where(x => x.Data >= dtmDataInicial && x.Data <= dtmDataFinal).ToList();


				if (!lstCotacoes.Any()) {
					return null;

				}

				//busca os splits dentro do período que executou 
				//objCarregadorSplit.SplitConsultar(objAtivo.Codigo, dtmDataInicial, "A", objRSSplit, dtmDataFinal)

				cMediaDiaria objMediaDoIFR = null;


				foreach (cCotacaoDiaria objCotacaoDoFluxoDaSimulacao in lstCotacoes) {

					//********INICIO DO TRATAMENTO DOS DESDOBRAMENTOS

					lstDesdobramentos = objAtivo.RetornaDesdobramentosDeUmaData(objCotacaoDoFluxoDaSimulacao.Data);


					foreach (cDesdobramento objDesdobramento in lstDesdobramentos) {
						if ((objCotacaoDeEntrada != null)) {
							objCotacaoDeEntrada.Converter(objDesdobramento.Razao);
						}

						if ((objCotacaoDoValorMaximo != null)) {
							objCotacaoDoValorMaximo.Converter(objDesdobramento.Razao);
						}

						objCotacaoDeAcionamentoDoSetup.Converter(objDesdobramento.Razao);

						objInformacoesDoTradeDTO.ValorDoStopLoss *= Convert.ToDecimal( objDesdobramento.Razao);


					}


					if (lstDesdobramentos.Count() > 0) {
						//Se houve desdobramentos e todas as cotações já tiveram seu valores convertidos de acordo
						//temos que ajustar as variáveis utilizadas para o controle da operação
						objInformacoesDoTradeDTO.ValorRealizacaoParcial = objSetup.CalculaValorRealizacaoParcial(objCotacaoDeAcionamentoDoSetup);

						decValorEntradaAjustado = objSetup.CalculaValorEntrada(objCotacaoDeAcionamentoDoSetup);

					}

					//********FIM DO TRATAMENTO DOS DESDOBRAMENTOS

					objMediaDoIFR =  (cMediaDiaria) objCotacaoDoFluxoDaSimulacao.Medias.Single(x => x.Tipo == "IFR2" && x.NumPeriodos == 13);


					if (objCotacaoDeEntrada == null) {
						//entrada efetiva ainda não ocorreu

						//Verifica se nesta iteraçao o preço vai atingir o valor de entrada

						//Para isso o valor de entrada tem que estar entre o valor máximo e valor mínimo 
						//do período.

						if (decValorEntradaAjustado >= objCotacaoDoFluxoDaSimulacao.ValorMinimo && decValorEntradaAjustado <= objCotacaoDoFluxoDaSimulacao.ValorMaximo) {
							//marca na variável que ocorreu a entrada
							blnEntradaEfetiva = true;

							objCotacaoDeEntrada = objCotacaoDoFluxoDaSimulacao.Clonar();


						} else if (decValorEntradaAjustado < objCotacaoDoFluxoDaSimulacao.ValorMinimo || objCotacaoDoFluxoDaSimulacao.IFR.Valor < objMediaDoIFR.Valor) {
							//se o valor de entrada ficou abaixo do valor mínimo então ocorreu um gap de alta.
							//neste caso sai da operação.
							//ou se o valor do IFR cruzou a média para baixo então a entrada foi abortada
							//e também marca na variável que a operação está encerrada

							blnOperacaoEncerrada = true;
							objCotacaoDeSaida = objCotacaoDoFluxoDaSimulacao;


						} else {
							//se não ocorreu a entrada efetiva, mas também não ocorreu a saida da operação,
							//calcula os novos valor de entrada, stop loss e realização parcial.

							//O valor de entrada é 0,25% do valor máximo somado ao próprio valor máximo
							//Se este valor for menor que 0,01 (1 centavo) será somado 0,01 (1 centavo) ao valor máximo

							objCotacaoDeAcionamentoDoSetup = objCotacaoDoFluxoDaSimulacao.Clonar();
							decValorEntradaAjustado = objSetup.CalculaValorEntrada(objCotacaoDeAcionamentoDoSetup);

							//O valor de stop loss é 0,25% do valor mínimo subtraído ao próprio valor minimo
							//Se este valor for menor que 0,01 (1 centavo) será subtraído 0,01 (1 centavo) ao valor minimo

							objInformacoesDoTradeDTO.ValorDoStopLoss = objSetup.CalculaValorStopLossInicial(objCotacaoDeAcionamentoDoSetup);

							objInformacoesDoTradeDTO.ValorRealizacaoParcial = objSetup.CalculaValorRealizacaoParcial(objCotacaoDeAcionamentoDoSetup);

						}

					}

					//Se a entrada ocorreu nesta iteração ou em uma entrada inferior

					if ((objCotacaoDeEntrada != null)) {

						if (objCotacaoDoFluxoDaSimulacao.ValorMinimo <= objInformacoesDoTradeDTO.ValorDoStopLoss) {
							//verifica se a data de entrada é a data atual

							if (objCotacaoDeEntrada.Data == objCotacaoDoFluxoDaSimulacao.Data) {
								//se a data de entrada é a mesma da posição atual do RS significa que atingiu
								//os valores de entrada e stop loss no mesmo dia.
								//Neste caso não estopa se as seguintes condições forem satisfeitas:
								//1) ValorFechamento > ValorAbertura
								//2) ValorFechamento > Valor do Stop Loss
								//3) Valor de Abertura < Valor de Entrada
								//Se estas três condições forem satisfeitas provavelmente o período
								//abriu em baixa e depois superou a máxima do período anterior gerando entrada
								//Caso contrário deve ter gerado uma entrada e depois estopado.
								//Não há como ter certeza disso. Para ter certeza só se tivessemos acesso a um gráfico
								//de menor periodicidade;

								if (!(objCotacaoDoFluxoDaSimulacao.ValorFechamento > objCotacaoDoFluxoDaSimulacao.ValorAbertura 
                                    && objCotacaoDoFluxoDaSimulacao.ValorFechamento > objInformacoesDoTradeDTO.ValorDoStopLoss 
                                    && objCotacaoDoFluxoDaSimulacao.ValorAbertura < decValorEntradaAjustado)) {
									//nega as três afirmações citadas. Se pelo menos uma delas retornar FALSE, vai entrar aqui
									//e encerrar a operação. Caso contrário a operação continua.
									blnOperacaoEncerrada = true;
									objCotacaoDeSaida = objCotacaoDoFluxoDaSimulacao;
								}

							} else {
								//se a data de entrada na operação não é a data atual do RS então sempre estopa a operação.
								blnOperacaoEncerrada = true;
								objCotacaoDeSaida = objCotacaoDoFluxoDaSimulacao;

							}



						} else if (objCotacaoDoFluxoDaSimulacao.IFR.Valor < objMediaDoIFR.Valor) {
							//se o valor do IFR está abaixo da média então calcula um novo valor
							//de stop loss na mínima do período, desde que o IFR já tenha cruzado a média
							//para cima alguma vez. Quando for IFR com filtro isto sempre será verdade, pois 
							//o cruzamento é o próprio sinal de entrada no setup.

							//adicionado novo parâmetro em 13/01/2011
							//subir apenas stop apenas se já realizou parcial


							if ((objCotacaoDoValorMaximo == null) || objCotacaoDoFluxoDaSimulacao.ValorMaximo > objCotacaoDoValorMaximo.ValorMaximo) {
								//se a cotação do valor máximo ainda não foi encontrada ou a cotação atual é maior do que a do valor máximo
								objInformacoesDoTradeDTO.ValorMaximo = objCotacaoDoFluxoDaSimulacao.ValorMaximo;
							} else {
								objInformacoesDoTradeDTO.ValorMaximo = objCotacaoDoValorMaximo.ValorMaximo;
							}

							//'O valor de stop loss é 0,25% do valor mínimo subtraído ao próprio valor minimo
							//'Se este valor for menor que 0,01 (1 centavo) será subtraído 0,01 (1 centavo) ao valor minimo

							objInformacoesDoTradeDTO.ValorDoStopLoss = objSetup.CalculaValorStopLossDeSaida(objCotacaoDoFluxoDaSimulacao, objInformacoesDoTradeDTO);



						} else if (objCotacaoDoFluxoDaSimulacao.IFR.Valor > objMediaDoIFR.Valor) {
							//marca que o IFR cruzou a média para cima. Com isto, na próxima iteração em que cruzar a média para baixo
							//pode subir o stop.
							objInformacoesDoTradeDTO.IFRCruzouMediaParaCima = true;

						}

						if (objCotacaoDeSaida == null) {

							if (((objCotacaoDoValorMaximo == null) || objCotacaoDoFluxoDaSimulacao.ValorMaximo > objCotacaoDoValorMaximo.ValorMaximo)) {

								//Se o valor máximo desta iteração supera o valor máximo calculado até agora,
								//então este é o novo valor máximo
								objCotacaoDoValorMaximo = objCotacaoDoFluxoDaSimulacao.Clonar();

							}


						} else {
							//se encontrou a cotação de saída sai do for each objCotacaoDoFluxoDaSimulacao
							break; // TODO: might not be correct. Was : Exit For

						}

					}
					//If Not objCotacaoDeEntrada Is Nothing Then

				}

				//objRSSplit.Fechar()


				if (!blnOperacaoEncerrada) {
					//soma 1 mes e diminui 1 um dia para ficar um dia no mes anterior e calcular o 
					//dtmDataInicial = DateAdd(DateInterval.Month, 1, dtmDataInicial)

					//pega o primeiro dia do mês.
					//dtmDataInicial = DateSerial(Year(dtmDataInicial), Month(dtmDataInicial), 1)

					//soma um dia na última data do mes anterior
                    dtmDataInicial = dtmDataFinal.AddDays(1);


				}

			}
			//fim do loop "While not blnOperacaoEncerrada"


			if (blnEntradaEfetiva) {

				var objRetorno = new cIFRSimulacaoDiaria(objConexao, objAtivo, objSetup, objCotacaoDeAcionamentoDoSetup, objCotacaoDeEntrada, objCotacaoDoValorMaximo, objCotacaoDeSaida, objInformacoesDoTradeDTO, lstIFRSobrevendido);

				objAtivo.AdicionarSimulacao(objRetorno);

				cManipuladorIFRSimulacaoDiaria objManipuladorSimulacao = new cManipuladorIFRSimulacaoDiaria(objConexao);

				objManipuladorSimulacao.Adicionar(objRetorno, "INSERT");

				objManipuladorSimulacao.Executar();

				return objRetorno;


			} else {
				return null;

			}

		}

	}
}
