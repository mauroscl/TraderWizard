using System.Windows.Forms;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.DomainServices;
using prjModelo.Carregadores;
using DataBase;
using prjModelo.Entidades;
using prjModelo;
using prjDTO;
using prjModelo.ValueObjects;
namespace prjServicoNegocio
{


	public class cCalculadorFaixasIFRDiario
	{

		private readonly cConexao objConexao;
		private readonly Setup objSetup;

		private readonly cAtivo objAtivo;

		private const double intTolerancia = 0.05;
		/// <summary>
		/// </summary>
		/// <param name="pobjConexao"></param>
		/// <param name="pobjAtivo"></param>
		/// <param name="pobjSetup"></param>
		/// <remarks></remarks>
		public cCalculadorFaixasIFRDiario(cConexao pobjConexao, cAtivo pobjAtivo, Setup pobjSetup)
		{
			objConexao = pobjConexao;
			objSetup = pobjSetup;
			objAtivo = pobjAtivo;
		}

		private cFaixaDTO FaixaCalcular(IList<cTradeDTO> plstTrades, IList<cTradeDTO> plstTradesMelhorEntrada, double pdblValorMinimo, double pdblValorMaximo)
		{

			cFaixaDTO objFaixaRetorno = null;

			//inclui a tolerância nos valores mínimo e máximo para o cálculo do número mínimo de tentativas e do número de trades com melhor entrada
			double dblValorMinimoAux = pdblValorMinimo + intTolerancia;
			double dblValorMaximoAux = pdblValorMaximo - intTolerancia;

			int intNumTradesTotal = (from itens in plstTrades where (itens.Valor >= pdblValorMinimo) && (itens.Valor <= pdblValorMaximo) select itens).Count();


			if (intNumTradesTotal != 0) {
				int intNumTradesVerdadeiro = (from itens in plstTrades where (itens.Valor >= pdblValorMinimo) & (itens.Valor <= pdblValorMaximo) && (itens.Verdadeiro) select itens ).Count();

				int intNumTradesMelhorEntrada = (from itens in plstTradesMelhorEntrada where (itens.Valor >= dblValorMinimoAux) && (itens.Valor <= dblValorMaximoAux) select itens).Count();

				int intNumTentativasMinimo = (from itens in plstTradesMelhorEntrada where (itens.Valor >= dblValorMinimoAux) && (itens.Valor <= dblValorMaximoAux) select itens.NumTentativas).Min();

				objFaixaRetorno = new cFaixaDTO(pdblValorMinimo, pdblValorMaximo, intNumTentativasMinimo, intNumTradesTotal, intNumTradesVerdadeiro, intNumTradesMelhorEntrada);


			} else {
				objFaixaRetorno = new cFaixaDTO(pdblValorMinimo, pdblValorMaximo, 0, 0, 0, 0);

			}

			return objFaixaRetorno;

		}

		private bool PontoMedioInferiorCalcular(IList<cTradeDTO> plstTradesMelhorEntrada, double pdblPontoMedioSuperior, ref double? pdblPontoMedioInferiorRet)
		{

			int intContador = plstTradesMelhorEntrada.Count(item => item.Valor < pdblPontoMedioSuperior);


			if (intContador > 0) {
				pdblPontoMedioInferiorRet = (from itens in (plstTradesMelhorEntrada.Where(item => item.Valor < pdblPontoMedioSuperior)) select itens.Valor).Max();

			}

			return (intContador > 0);

		}

		/// <summary>
		/// '
		/// </summary>
		/// <param name="plstTrades">Lista de todos os trades que deram entrada</param>
		/// <returns>lista com as faixas</returns>
		/// <remarks></remarks>
		//Private Function CalcularFaixaParaUmCriterio(ByVal plstTrades As IList(Of cTradeDTO), ByVal pintQuantidadeMelhorEntrada As Integer _
		//, ByVal pobjClassifMedia As cClassifMedia, ByVal pobjCriterioCM As cCriterioClassifMedia, pobjIFRSobrevendido As cIFRSobrevendido _
		//, pdtmData As DateTime) As IList(Of cIFRSimulacaoDiariaFaixa)

		private IList<cFaixaDTO> CalcularFaixaParaUmCriterio(IList<cTradeDTO> plstTrades)
		{

			var lstRetorno = new List<cFaixaDTO>();

			//ordena a lista de trades pelo campo do valor na ordem descendente.
			plstTrades = plstTrades.OrderByDescending(item => item.Valor).ToList();

			//obtem lista apenas com os trades de melhor entrada
			IList<cTradeDTO> lstTradesMelhorEntrada = plstTrades.Where(item => item.MelhorEntrada).ToList();
			int intQuantidadeMelhorEntrada = lstTradesMelhorEntrada.Count();

			//obtem o ponto mínimo da melhor entrada de todos os trades
			double dblPontoMinimo = (from itens in lstTradesMelhorEntrada select itens.Valor).Min();

			//ajusta o valor para ficar com final .0 ou .5
			dblPontoMinimo = cUtil.PontoInferiorCalcular(dblPontoMinimo);

			//obtem o ponto máximo  da melhor entrada de todos os trades
			double dblPontoMaximo = (from itens in lstTradesMelhorEntrada select itens.Valor).Max();

			//ajusta o valor para ficar com final .0 ou .5
			dblPontoMaximo = cUtil.PontoSuperiorCalcular(dblPontoMaximo);

			cFaixaDTO objFaixaUnicaDTO = null;
			objFaixaUnicaDTO = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMinimo, dblPontoMaximo);

			bool blnUtilizarFaixaUnica = false;

			cFaixaDTO objFaixaInferiorDTORetorno = null;
			cFaixaDTO objFaixaSuperiorDTORetorno = null;


			if (objFaixaUnicaDTO.NumTradesTotal > intQuantidadeMelhorEntrada) {
				int intI;

			    double? dblPontoMedioInferior = 0;

			    cFaixaDTO objFaixaSuperiorDTOAux = null;

				//for do primeiro ao penúltimo item.
				//o último não precisa, porque se for verdadeiro já está cadastrado no ponto mínimo.

				for (intI = 1; intI <= plstTrades.Count - 2; intI++)
				{
				    bool blnTestar = false;

				    //dblPontoMedioInferior = Nothing
					//dblPontoMedioSuperior = Nothing

					if (plstTrades[intI].MelhorEntrada) {
						//Se é melhor entrada...

						double dblPontoMedioSuperior = cUtil.PontoInferiorCalcular(plstTrades[intI].Valor);


						if ((!plstTrades[intI + 1].MelhorEntrada)) {

							if ((objFaixaSuperiorDTOAux != null)) {
								if (dblPontoMedioSuperior < objFaixaSuperiorDTOAux.ValorMinimo) {
									//Só faz sentido testar uma nova faixa se o valor for inferior ao ponto médio da faixa anterior
									blnTestar = true;
								}


							} else {
								//Se o próximo trade não é melhor entrada e ainda não existe uma faixa calculada com certeza tem que testar
								blnTestar = true;

							}


						}
						//If (Not plstTrades(intI + 1).MelhorEntrada) Then


						if (blnTestar) {
							//Tenta calcular o ponto médio inferior.
							//Se não conseguir significa que todas as faixas possíveis já foram calculadas e sai do loop.

							if (!PontoMedioInferiorCalcular(lstTradesMelhorEntrada, dblPontoMedioSuperior, ref dblPontoMedioInferior)) {
								break; // TODO: might not be correct. Was : Exit For

							}


							dblPontoMedioInferior = cUtil.PontoSuperiorCalcular(dblPontoMedioInferior.Value);


							if (dblPontoMedioInferior != dblPontoMedioSuperior) {
								cFaixaDTO objFaixaInferiorDTOAux = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMinimo, dblPontoMedioInferior.Value);

								if (objFaixaInferiorDTOAux.NumTradesTotal == 0) {
									//Se ao calcular as faixas a faixa inferior ficou sem trades, esta faixa é inválida e tem que sair do loop
									break; // TODO: might not be correct. Was : Exit For
								}

								objFaixaSuperiorDTOAux = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMedioSuperior, dblPontoMaximo);

								//Este if serve para evitar os casos em que algum valor acaba ficando de fora das duas faixas por causa da tolerância.
								//Neste caso não considera a faixa calculada.

								if ((objFaixaInferiorDTOAux.NumTradesMelhorEntrada + objFaixaSuperiorDTOAux.NumTradesMelhorEntrada) == intQuantidadeMelhorEntrada) {


									if (objFaixaInferiorDTORetorno == null | objFaixaSuperiorDTORetorno == null) {
										//Primeira faixa calculada 
										objFaixaInferiorDTORetorno = objFaixaInferiorDTOAux;
										objFaixaSuperiorDTORetorno = objFaixaSuperiorDTOAux;


									} else {
										//Se já tem uma faixa calculada, verifica se a atual é melhor do que a anterior.


										if (((objFaixaInferiorDTOAux.NumTradesTotal + objFaixaSuperiorDTOAux.NumTradesTotal) < (objFaixaInferiorDTORetorno.NumTradesTotal + objFaixaSuperiorDTORetorno.NumTradesTotal))) {
											objFaixaInferiorDTORetorno = objFaixaInferiorDTOAux;
											objFaixaSuperiorDTORetorno = objFaixaSuperiorDTOAux;


										} else if (((objFaixaInferiorDTOAux.NumTradesTotal + objFaixaSuperiorDTOAux.NumTradesTotal) == (objFaixaInferiorDTORetorno.NumTradesTotal + objFaixaSuperiorDTORetorno.NumTradesTotal))) {


											if (((objFaixaInferiorDTOAux.NumTradesVerdadeiro + objFaixaSuperiorDTOAux.NumTradesVerdadeiro) > (objFaixaInferiorDTORetorno.NumTradesVerdadeiro + objFaixaSuperiorDTORetorno.NumTradesVerdadeiro))) {
												objFaixaInferiorDTORetorno = objFaixaInferiorDTOAux;
												objFaixaSuperiorDTORetorno = objFaixaSuperiorDTOAux;

											}

										}

									}
									//If objFaixaInferiorDTORetorno Is Nothing Or objFaixaSuperiorDTORetorno Is Nothing Then


									if (((objFaixaInferiorDTOAux.NumTradesTotal + objFaixaSuperiorDTOAux.NumTradesTotal) == intQuantidadeMelhorEntrada) | (objFaixaInferiorDTOAux.NumTradesMelhorEntrada == 1) | (objFaixaInferiorDTOAux.ValorMaximo - objFaixaInferiorDTOAux.ValorMinimo == 0.5) | (objFaixaInferiorDTOAux.NumTradesTotal == objFaixaInferiorDTOAux.NumTradesMelhorEntrada)) {
										//Se a faixa inferior atual já tem o número de trades igual ao número de trades com melhor entrada
										//ou se tem apenas 1 trade de melhor entrada
										//ou se a distância entre seus valores máximos e mínimos é 0.5, que é a menor diferença que pode haver numa faixa
										//ou se todos os trades da faixa são melhor entrada
										//não tem mais como melhor esta faixa e portanto pode sair do loop desde que as faixas de retorno já estejam calculadas

										break; // TODO: might not be correct. Was : Exit For

									}

								}
								//If (objFaixaInferiorDTOAux.NumTradesTotal + objFaixaSuperiorDTOAux.NumTradesTotal) = plstTrades.Count Then


							}
							//If dblPontoMedioInferior <> dblPontoMedioSuperior Then

						}
						//If blnTestar then...


					}
					//If plstTrades(intI).MelhorEntrada Then
				}


			    if ((objFaixaInferiorDTORetorno != null) & (objFaixaSuperiorDTORetorno != null)) {
					int intNumTradesDuasFaixas = objFaixaInferiorDTORetorno.NumTradesTotal + objFaixaSuperiorDTORetorno.NumTradesTotal;

					//verifica qual a melhor faixa: faixa única ou a melhor com duas faixas. Se terminarem empatados, a preferência é da faixa única.
					if (objFaixaUnicaDTO.NumTradesTotal < intNumTradesDuasFaixas) {
						blnUtilizarFaixaUnica = true;
					} else if (objFaixaUnicaDTO.NumTradesTotal == intNumTradesDuasFaixas) {
						if (objFaixaUnicaDTO.NumTradesVerdadeiro >= (objFaixaInferiorDTORetorno.NumTradesVerdadeiro + objFaixaSuperiorDTORetorno.NumTradesVerdadeiro)) {
							blnUtilizarFaixaUnica = true;
						} else {
							blnUtilizarFaixaUnica = false;
						}
					} else {
						blnUtilizarFaixaUnica = false;
					}


				} else {
					//Se não conseguiu encontrar faixas, utiliza a faixa única.
					blnUtilizarFaixaUnica = true;

				}


			} else {
				//a faixa única é a melhor opção
				blnUtilizarFaixaUnica = true;

			}
			//If objFaixaUnica.NumTradesTotal > pintQuantidadeMelhorEntrada Then


			if (blnUtilizarFaixaUnica) {
				//lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjClassifMedia, pobjCriterioCM, pobjIFRSobrevendido _
				//, DateAndTime.Now, objFaixaUnicaDTO.NumTentativasMinimo, objFaixaUnicaDTO.ValorMinimo, objFaixaUnicaDTO.ValorMaximo))

				lstRetorno.Add(objFaixaUnicaDTO);


			} else {
				lstRetorno.Add(objFaixaInferiorDTORetorno);
				lstRetorno.Add(objFaixaSuperiorDTORetorno);

				//lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjClassifMedia, pobjCriterioCM, pobjIFRSobrevendido _
				//, DateAndTime.Now, objFaixaInferiorDTORetorno.NumTentativasMinimo, objFaixaInferiorDTORetorno.ValorMinimo, objFaixaInferiorDTORetorno.ValorMaximo))

				//lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjClassifMedia, pobjCriterioCM, pobjIFRSobrevendido _
				//, DateAndTime.Now, objFaixaSuperiorDTORetorno.NumTentativasMinimo, objFaixaSuperiorDTORetorno.ValorMinimo, objFaixaSuperiorDTORetorno.ValorMaximo))

			}

			return lstRetorno;

		}


		/// <summary>
		/// Calcula as faixas de acordo com a operação executada e salva no banco de dados
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CalcularFaixasParaUmaData(cIFRSobrevendido pobjIFRSobrevendido, cCalculoFaixaResumoVO pobjCalcularFaixaResumoVO)
		{


			try {
				string strSQL = null;

				IList<cIFRSimulacaoDiariaFaixa> lstFaixasTotal = null;

				IList<cFaixaDTO> lstFaixasDTO = null;

				lstFaixasTotal = new List<cIFRSimulacaoDiariaFaixa>();

				cCarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new cCarregadorCriterioClassificacaoMedia();

				IList<cCriterioClassifMedia> lstCriterioCM = null;
				lstCriterioCM = objCarregadorCriterioCM.CarregaTodos();

				cRSList objRSTrades = new cRSList(objConexao);

				//Busca todos os trades do papel para fazer um único acesso ao banco.
				strSQL = lstCriterioCM.Aggregate("SELECT ID_CM, NumTentativas, Verdadeiro, MelhorEntrada " + Environment.NewLine, (current, objCriterioCM) => current + ", " + objCriterioCM.CampoBD + " AS " + objCriterioCM.AliasBD);

				//Gera os campos referentes a cada um dos critérios de classificação de média

				strSQL = strSQL + " FROM IFR_Simulacao_Diaria SD INNER JOIN IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
				strSQL = strSQL + " ON SD.Codigo = D.Codigo " + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_Setup = D.ID_Setup " + Environment.NewLine;
				strSQL = strSQL + " AND SD.Data_Entrada_Efetiva = D.Data_Entrada_Efetiva " + Environment.NewLine;
				strSQL = strSQL + " WHERE SD.Codigo = " + FuncoesBD.CampoFormatar(objAtivo.Codigo) + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_Setup = " + FuncoesBD.CampoFormatar(objSetup.ID) + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_CM = " + FuncoesBD.CampoFormatar(pobjCalcularFaixaResumoVO.ClassifMedia.ID) + Environment.NewLine;
				strSQL = strSQL + " AND Data_Saida <= " + FuncoesBD.CampoFormatar(pobjCalcularFaixaResumoVO.DataSaida) + Environment.NewLine;
				strSQL = strSQL + " AND Valor_IFR_Minimo <= " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ValorMaximo) + Environment.NewLine;
				strSQL = strSQL + " AND D.ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID) + Environment.NewLine;

				objRSTrades.AdicionarQuery(strSQL);

				objRSTrades.ExecuteQuery();

				IList<cTradeDTO> lstTrades = null;


				if (objRSTrades.Dados.Count > 0) {
					//Só é necessário calcular as faixas se existem simulações para a classificação de média desta iteração.

					//Calcula o número de trades que são melhor entrada.
					var intQuantidadeMelhorEntrada = objRSTrades.Dados.Where(linha => Convert.ToBoolean(linha["MelhorEntrada"])).Count();


					if (intQuantidadeMelhorEntrada > 0) {
						//Para cada uma das classificações e dos critérios de classificação, calcula as faixas.

						foreach (cCriterioClassifMedia objCriterioCM in lstCriterioCM) {
							string strCampo = objCriterioCM.AliasBD;

							//transforma a consulta SQL em um objeto da classe cTradeDTO
							lstTrades = (from linha in objRSTrades.Dados select new cTradeDTO( (double) linha[strCampo], Convert.ToBoolean(linha["Verdadeiro"]), Convert.ToBoolean(linha["MelhorEntrada"]), Convert.ToInt32(linha["NumTentativas"]))).ToList();

							lstFaixasDTO = CalcularFaixaParaUmCriterio(lstTrades);


							foreach (cFaixaDTO item in lstFaixasDTO) {
								lstFaixasTotal.Add(new cIFRSimulacaoDiariaFaixa(objAtivo.Codigo, objSetup, pobjCalcularFaixaResumoVO.ClassifMedia, objCriterioCM, pobjIFRSobrevendido, pobjCalcularFaixaResumoVO.DataSaida, item.NumTentativasMinimo, item.ValorMinimo, item.ValorMaximo));

							}

							//limpa collectin para a próxima iteração
							lstTrades.Clear();

						}

					}

				}

				//Faz a persistência de todas as faixas no banco de dados.
				foreach (cIFRSimulacaoDiariaFaixa objFaixa in lstFaixasTotal) {
					objFaixa.Salvar(objConexao);
				}

				return true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message,"Trader Wizard", MessageBoxButtons.OK,MessageBoxIcon.Error);

				//Debug.Print("Codigo: " & strCodigo)

				return false;

			}

		}


		#region "Deprecated"

		/// <summary>
		/// Comentada por Mauro, 17/07/2011. Foi criada uma nova função para calcular as faixas
		/// Calcula a(s) faixa(s) de um dos critérios de classificação de média
		/// </summary>
		/// <param name="pobjCM">objeto que indica a classificação das média, ou seja o alinhamento das médias e dos preços entre si</param>
		/// <param name="pobjCriterioCM">objeto que indica o criterio que deve ser calculado</param>
		/// <param name="pintQuantidadeMelhorEntrada">quantidade total de trades com melhor entrada</param>
		/// <param name="pdtmData">Data para a qual será calculada a faixa. Considera os trades realizados até esta faixa</param>
		/// <returns>Uma ou duas faixas contendo o(s) melhor(es) valores para o critério recebido por parâmetro</returns>
		/// <remarks></remarks>
		//Private Function CalcularFaixasParaUmCriterio(ByVal pobjCM As cClassifMedia, ByVal pobjCriterioCM As cCriterioClassifMedia _
		//, ByVal pintQuantidadeMelhorEntrada As Integer, pobjIFRSobrevendido As cIFRSobrevendido) As IList(Of cIFRSimulacaoDiariaFaixa)

		//    Dim lstRetorno As IList(Of cIFRSimulacaoDiariaFaixa)
		//    lstRetorno = New List(Of cIFRSimulacaoDiariaFaixa)

		//    Dim strRetorno As String = vbNullString

		//    Dim structFaixaUnica As structFaixa
		//    Dim structFaixaInferiorMedia As structFaixa
		//    Dim structFaixaSuperiorMedia As structFaixa
		//    Dim structFaixaInferiorPontoMedio As structFaixa
		//    Dim structFaixaSuperiorPontoMedio As structFaixa

		//    Dim dblMedia As Double
		//    Dim dblPontoMedio As Double

		//    'Calcula a faixa única
		//    structFaixaUnica = FaixaCalcular(pobjCM, pobjCriterioCM, , , dblMedia, dblPontoMedio)

		//    If structFaixaUnica.intNumTrades = pintQuantidadeMelhorEntrada Then

		//        'indica que o retorno será a faixa única.
		//        strRetorno = "U"

		//    Else

		//        'calcula as duas faixas geradas pela divisão pelo ponto médio
		//        structFaixaInferiorPontoMedio = FaixaCalcular(pobjCM, pobjCriterioCM, structFaixaUnica.dblValorMinimo, dblPontoMedio)

		//        structFaixaSuperiorPontoMedio = FaixaCalcular(pobjCM, pobjCriterioCM, dblPontoMedio, structFaixaUnica.dblValorMaximo)

		//        If (structFaixaInferiorPontoMedio.intNumTrades + structFaixaSuperiorPontoMedio.intNumTrades) = pintQuantidadeMelhorEntrada Then
		//            strRetorno = "PM"
		//        Else

		//            If (dblMedia >= structFaixaInferiorPontoMedio.dblValorMinimo And dblMedia <= structFaixaInferiorPontoMedio.dblValorMaximo) _
		//            Or (dblMedia >= structFaixaSuperiorPontoMedio.dblValorMinimo And dblMedia <= structFaixaSuperiorPontoMedio.dblValorMaximo) Then

		//                'só é necessário calcular as faixas pela média se a média estiver contida em uma das faixas calculadas pelo ponto médio.
		//                'Caso a média estiver fora das duas faixas, a divisão de faixas será igual e não é necessário calcular novamente.
		//                structFaixaInferiorMedia = FaixaCalcular(pobjCM, pobjCriterioCM, structFaixaUnica.dblValorMinimo, dblMedia)
		//                structFaixaSuperiorMedia = FaixaCalcular(pobjCM, pobjCriterioCM, dblMedia, structFaixaUnica.dblValorMaximo)
		//                If (structFaixaInferiorMedia.intNumTrades + structFaixaSuperiorMedia.intNumTrades) = pintQuantidadeMelhorEntrada Then
		//                    strRetorno = "M"
		//                End If

		//            Else

		//                'para não ocorrer erros de controle, se a faixa estabelecida pela média for igual a faixa estabelecida pelo ponto médio
		//                'vamos atribuir as faixas do ponto médio nas faixas da média.
		//                structFaixaInferiorMedia = structFaixaInferiorPontoMedio
		//                structFaixaSuperiorMedia = structFaixaSuperiorPontoMedio

		//            End If

		//        End If

		//    End If


		//    If strRetorno = vbNullString Then

		//        'Se o controle do retorno ainda não foi setado, significa que nenhuma das estratégias conseguiu retornar somente as melhores entradas.
		//        If (structFaixaInferiorMedia.intNumTrades + structFaixaSuperiorMedia.intNumTrades) _
		//        < (structFaixaInferiorPontoMedio.intNumTrades + structFaixaSuperiorPontoMedio.intNumTrades) _
		//        And (structFaixaInferiorMedia.intNumTrades + structFaixaSuperiorMedia.intNumTrades) < structFaixaUnica.intNumTrades Then
		//            strRetorno = "M"
		//        ElseIf (structFaixaInferiorPontoMedio.intNumTrades + structFaixaSuperiorPontoMedio.intNumTrades) < structFaixaUnica.intNumTrades Then
		//            strRetorno = "PM"
		//        Else
		//            strRetorno = "U"
		//        End If

		//    End If

		//    Select Case strRetorno

		//        Case "U"

		//            'Se a faixa única já traz apenas os trades de melhor entrada, retorna esta faixa
		//            lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido _
		//            , structFaixaUnica.intNumTentativasMinimo, structFaixaUnica.dblValorMinimo, structFaixaUnica.dblValorMaximo))

		//        Case "PM"

		//            'Retorna as faixas do ponto médio
		//            lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido _
		//            , structFaixaInferiorPontoMedio.intNumTentativasMinimo, structFaixaInferiorPontoMedio.dblValorMinimo _
		//            , structFaixaInferiorPontoMedio.dblValorMaximo))

		//            lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido _
		//            , structFaixaSuperiorPontoMedio.intNumTentativasMinimo, structFaixaSuperiorPontoMedio.dblValorMinimo _
		//            , structFaixaSuperiorPontoMedio.dblValorMaximo))

		//        Case "M"

		//            'Retorna as faixas da média
		//            lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido _
		//            , structFaixaInferiorMedia.intNumTentativasMinimo, structFaixaInferiorMedia.dblValorMinimo _
		//            , structFaixaInferiorMedia.dblValorMaximo))

		//            lstRetorno.Add(New cIFRSimulacaoDiariaFaixa(strCodigo, objSetup, pobjCM, pobjCriterioCM, pobjIFRSobrevendido _
		//            , structFaixaSuperiorMedia.intNumTentativasMinimo, structFaixaSuperiorMedia.dblValorMinimo _
		//            , structFaixaSuperiorMedia.dblValorMaximo))

		//    End Select

		//    Return lstRetorno

		//End Function

		//Private Function FaixaCalcular(ByVal pobjCM As cClassifMedia, ByVal pobjCriterioCM As cCriterioClassifMedia, Optional ByVal pdblPontoInferior As Double = -1 _
		//, Optional ByVal pdblPontoSuperior As Double = -1, Optional ByRef pdblMediaRet As Double = -1, Optional ByRef pdblPontoMedioRet As Double = -1) As structFaixa

		//    Dim structFaixaRet As structFaixa
		//    Dim objRS As cRS = New cRS(objConexao)

		//    Dim strSQL As String

		//    strSQL = " SELECT MIN(" & pobjCriterioCM.CampoBD & ") AS Valor_Minimo, MAX(" & pobjCriterioCM.CampoBD & ") AS Valor_Maximo" & vbNewLine
		//    strSQL = strSQL & ", MIN(NumTentativas) AS NumTentativas_Minimo " & vbNewLine

		//    If pdblPontoInferior = -1 _
		//    And pdblPontoSuperior = -1 Then
		//        'Só a necessidade de calcular média é ponto médio quando não são passados pontos superiores e inferiores
		//        strSQL = strSQL & ", AVG(CDBL(" & pobjCriterioCM.CampoBD & ")) AS Media" & vbNewLine
		//        strSQL = strSQL & ", (MIN(" & pobjCriterioCM.CampoBD & ") +  MAX(" & pobjCriterioCM.CampoBD & "))/2 AS Ponto_Medio " & vbNewLine

		//    End If

		//    strSQL = strSQL & "FROM IFR_Simulacao_Diaria " & vbNewLine
		//    strSQL = strSQL & "WHERE Codigo = " & FuncoesBD.CampoFormatar(objAtivo.Codigo) & vbNewLine
		//    strSQL = strSQL & " AND ID_Setup = " & FuncoesBD.CampoFormatar(objSetup.ID) & vbNewLine
		//    strSQL = strSQL & " AND ID_CM = " & FuncoesBD.CampoFormatar(pobjCM.ID) & vbNewLine
		//    strSQL = strSQL & " AND MelhorEntrada = " & FuncoesBD.CampoFormatar(True) & vbNewLine

		//    If pdblPontoInferior <> -1 Then

		//        strSQL = strSQL & " AND " & pobjCriterioCM.CampoBD & " >= " & FuncoesBD.CampoFormatar(pdblPontoInferior) & vbNewLine

		//    End If

		//    If pdblPontoSuperior <> -1 Then

		//        strSQL = strSQL & " AND " & pobjCriterioCM.CampoBD & " <= " & FuncoesBD.CampoFormatar(pdblPontoSuperior) & vbNewLine

		//    End If

		//    objRS.ExecuteQuery(strSQL)

		//    structFaixaRet.dblValorMinimo = cUtil.PontoInferiorCalcular(CDbl(objRS.Field("Valor_Minimo")))
		//    structFaixaRet.dblValorMaximo = cUtil.PontoSuperiorCalcular(CDbl(objRS.Field("Valor_Maximo")))
		//    structFaixaRet.intNumTentativasMinimo = CInt(objRS.Field("NumTentativas_Minimo"))

		//    If pdblPontoInferior = -1 _
		//    And pdblPontoSuperior = -1 Then

		//        'Só a necessidade de retornar média é ponto médio quando não são passados pontos superiores e inferiores
		//        pdblMediaRet = CDbl(objRS.Field("Media"))
		//        pdblPontoMedioRet = CDbl(objRS.Field("Ponto_Medio"))

		//    End If

		//    objRS.Fechar()

		//    'Calcula o número de trades que será realizado por esta faixa. 
		//    NumTradesFaixaCalcular(pobjCM, pobjCriterioCM, structFaixaRet)

		//    Return structFaixaRet

		//End Function

		//Private Sub NumTradesFaixaCalcular(ByVal pobjCM As cClassifMedia, ByVal pobjCriterioCM As cCriterioClassifMedia, ByRef pstructFaixaRet As structFaixa)

		//    Dim objRS As cRS = New cRS(objConexao)

		//    Dim strSQL As String

		//    strSQL = "SELECT COUNT(1) AS NumTrades " & vbNewLine
		//    strSQL = strSQL & "FROM IFR_Simulacao_Diaria " & vbNewLine
		//    strSQL = strSQL & "WHERE Codigo = " & FuncoesBD.CampoFormatar(objAtivo.Codigo) & vbNewLine
		//    strSQL = strSQL & " AND ID_Setup = " & FuncoesBD.CampoFormatar(objSetup.ID) & vbNewLine
		//    strSQL = strSQL & " AND ID_CM = " & FuncoesBD.CampoFormatar(pobjCM.ID) & vbNewLine
		//    strSQL = strSQL & " AND " & pobjCriterioCM.CampoBD & " >= " & FuncoesBD.CampoFormatar(pstructFaixaRet.dblValorMinimo) & vbNewLine
		//    strSQL = strSQL & " AND " & pobjCriterioCM.CampoBD & " <= " & FuncoesBD.CampoFormatar(pstructFaixaRet.dblValorMaximo) & vbNewLine
		//    strSQL = strSQL & " AND NumTentativas >= " & pstructFaixaRet.intNumTentativasMinimo

		//    objRS.ExecuteQuery(strSQL)

		//    pstructFaixaRet.intNumTrades = CInt(objRS.Field("NumTrades"))

		//    objRS.Fechar()

		//End Sub

		//Private Structure structFaixa
		//    Dim dblValorMinimo As Double
		//    Dim dblValorMaximo As Double
		//    Dim intNumTentativasMinimo As Integer
		//    Dim intNumTrades As Integer
		//    Dim intNumTradesVerdadeiro As Integer
		//End Structure

		#endregion




	}
}
