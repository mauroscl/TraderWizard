using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using Dominio.Regras;
using Dominio.ValueObjects;
using DTO;
using prjDominio.ValueObjects;
using TraderWizard.Infra.Repositorio;

namespace ServicoNegocio
{


	public class CalculadorFaixasIFRDiario
	{

		private readonly Conexao _conexao;
		private readonly Setup _setup;

		private readonly Ativo _ativo;

		private const double intTolerancia = 0.05;
		/// <summary>
		/// </summary>
		/// <param name="pobjConexao"></param>
		/// <param name="pobjAtivo"></param>
		/// <param name="pobjSetup"></param>
		/// <remarks></remarks>
		public CalculadorFaixasIFRDiario(Conexao pobjConexao, Ativo pobjAtivo, Setup pobjSetup)
		{
			_conexao = pobjConexao;
			_setup = pobjSetup;
			_ativo = pobjAtivo;
		}

		private FaixaDto FaixaCalcular(IList<TradeDto> plstTrades, IList<TradeDto> plstTradesMelhorEntrada, double pdblValorMinimo, double pdblValorMaximo)
		{

			FaixaDto objFaixaRetorno;

			//inclui a tolerância nos valores mínimo e máximo para o cálculo do número mínimo de tentativas e do número de trades com melhor entrada
			double dblValorMinimoAux = pdblValorMinimo + intTolerancia;
			double dblValorMaximoAux = pdblValorMaximo - intTolerancia;

			int intNumTradesTotal = (from itens in plstTrades where (itens.Valor >= pdblValorMinimo) && (itens.Valor <= pdblValorMaximo) select itens).Count();


			if (intNumTradesTotal != 0) {
				int intNumTradesVerdadeiro = (from itens in plstTrades where (itens.Valor >= pdblValorMinimo) && (itens.Valor <= pdblValorMaximo) && (itens.Verdadeiro) select itens ).Count();

				int intNumTradesMelhorEntrada = (from itens in plstTradesMelhorEntrada where (itens.Valor >= dblValorMinimoAux) && (itens.Valor <= dblValorMaximoAux) select itens).Count();

				int intNumTentativasMinimo = (from itens in plstTradesMelhorEntrada where (itens.Valor >= dblValorMinimoAux) && (itens.Valor <= dblValorMaximoAux) select itens.NumTentativas).Min();

				objFaixaRetorno = new FaixaDto(pdblValorMinimo, pdblValorMaximo, intNumTentativasMinimo, intNumTradesTotal, intNumTradesVerdadeiro, intNumTradesMelhorEntrada);


			} else {
				objFaixaRetorno = new FaixaDto(pdblValorMinimo, pdblValorMaximo, 0, 0, 0, 0);

			}

			return objFaixaRetorno;

		}

		private bool PontoMedioInferiorCalcular(IList<TradeDto> plstTradesMelhorEntrada, double pdblPontoMedioSuperior, ref double? pdblPontoMedioInferiorRet)
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

		private IList<FaixaDto> CalcularFaixaParaUmCriterio(IList<TradeDto> plstTrades)
		{

			var lstRetorno = new List<FaixaDto>();

			//ordena a lista de trades pelo campo do valor na ordem descendente.
			plstTrades = plstTrades.OrderByDescending(item => item.Valor).ToList();

			//obtem lista apenas com os trades de melhor entrada
			IList<TradeDto> lstTradesMelhorEntrada = plstTrades.Where(item => item.MelhorEntrada).ToList();
			int intQuantidadeMelhorEntrada = lstTradesMelhorEntrada.Count();

			//obtem o ponto mínimo da melhor entrada de todos os trades
			double dblPontoMinimo = (from itens in lstTradesMelhorEntrada select itens.Valor).Min();

			//ajusta o valor para ficar com final .0 ou .5
			dblPontoMinimo = Util.PontoInferiorCalcular(dblPontoMinimo);

			//obtem o ponto máximo  da melhor entrada de todos os trades
			double dblPontoMaximo = (from itens in lstTradesMelhorEntrada select itens.Valor).Max();

			//ajusta o valor para ficar com final .0 ou .5
			dblPontoMaximo = Util.PontoSuperiorCalcular(dblPontoMaximo);

			FaixaDto objFaixaUnicaDTO = null;
			objFaixaUnicaDTO = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMinimo, dblPontoMaximo);

			bool blnUtilizarFaixaUnica = false;

			FaixaDto objFaixaInferiorDTORetorno = null;
			FaixaDto objFaixaSuperiorDTORetorno = null;


			if (objFaixaUnicaDTO.NumTradesTotal > intQuantidadeMelhorEntrada) {
				int intI;

			    double? dblPontoMedioInferior = 0;

			    FaixaDto objFaixaSuperiorDTOAux = null;

				//for do primeiro ao penúltimo item.
				//o último não precisa, porque se for verdadeiro já está cadastrado no ponto mínimo.

				for (intI = 1; intI <= plstTrades.Count - 2; intI++)
				{
				    bool blnTestar = false;

				    //dblPontoMedioInferior = Nothing
					//dblPontoMedioSuperior = Nothing

					if (plstTrades[intI].MelhorEntrada) {
						//Se é melhor entrada...

						double dblPontoMedioSuperior = Util.PontoInferiorCalcular(plstTrades[intI].Valor);


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


							dblPontoMedioInferior = Util.PontoSuperiorCalcular(dblPontoMedioInferior.Value);


							if (dblPontoMedioInferior != dblPontoMedioSuperior) {
								FaixaDto objFaixaInferiorDTOAux = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMinimo, dblPontoMedioInferior.Value);

								if (objFaixaInferiorDTOAux.NumTradesTotal == 0) {
									//Se ao calcular as faixas a faixa inferior ficou sem trades, esta faixa é inválida e tem que sair do loop
									break; // TODO: might not be correct. Was : Exit For
								}

								objFaixaSuperiorDTOAux = FaixaCalcular(plstTrades, lstTradesMelhorEntrada, dblPontoMedioSuperior, dblPontoMaximo);

								//Este if serve para evitar os casos em que algum valor acaba ficando de fora das duas faixas por causa da tolerância.
								//Neste caso não considera a faixa calculada.

								if ((objFaixaInferiorDTOAux.NumTradesMelhorEntrada + objFaixaSuperiorDTOAux.NumTradesMelhorEntrada) == intQuantidadeMelhorEntrada) {


									if (objFaixaInferiorDTORetorno == null || objFaixaSuperiorDTORetorno == null) {
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


									if (((objFaixaInferiorDTOAux.NumTradesTotal + objFaixaSuperiorDTOAux.NumTradesTotal) == intQuantidadeMelhorEntrada) || (objFaixaInferiorDTOAux.NumTradesMelhorEntrada == 1) || (objFaixaInferiorDTOAux.ValorMaximo - objFaixaInferiorDTOAux.ValorMinimo == 0.5) || (objFaixaInferiorDTOAux.NumTradesTotal == objFaixaInferiorDTOAux.NumTradesMelhorEntrada)) {
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


			    if ((objFaixaInferiorDTORetorno != null) && (objFaixaSuperiorDTORetorno != null)) {
					int intNumTradesDuasFaixas = objFaixaInferiorDTORetorno.NumTradesTotal + objFaixaSuperiorDTORetorno.NumTradesTotal;

					//verifica qual a melhor faixa: faixa única ou a melhor com duas faixas. Se terminarem empatados, a preferência é da faixa única.
					if (objFaixaUnicaDTO.NumTradesTotal < intNumTradesDuasFaixas) {
						blnUtilizarFaixaUnica = true;
					} else if (objFaixaUnicaDTO.NumTradesTotal == intNumTradesDuasFaixas) {
						if (objFaixaUnicaDTO.NumTradesVerdadeiro >= (objFaixaInferiorDTORetorno.NumTradesVerdadeiro + objFaixaSuperiorDTORetorno.NumTradesVerdadeiro)) {
							blnUtilizarFaixaUnica = true;
						}
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
		public bool CalcularFaixasParaUmaData(IFRSobrevendido pobjIFRSobrevendido, CalculoFaixaResumo pobjCalcularFaixaResumo)
		{

			try {
			    IList<IFRSimulacaoDiariaFaixa> lstFaixasTotal = new List<IFRSimulacaoDiariaFaixa>();

				var objCarregadorCriterioCM = new CarregadorCriterioClassificacaoMedia();

			    IList<CriterioClassifMedia> lstCriterioCM = objCarregadorCriterioCM.CarregaTodos();

				RSList objRSTrades = new RSList(_conexao);

				//Busca todos os trades do papel para fazer um único acesso ao banco.
			    string strSQL = lstCriterioCM.Aggregate(
			        "SELECT ID_CM, NumTentativas, Verdadeiro, MelhorEntrada " + Environment.NewLine,
			        (current, objCriterioCM) => current + ", " + objCriterioCM.CampoBD + " AS " + objCriterioCM.AliasBD);


				//Gera os campos referentes a cada um dos critérios de classificação de média
                FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

				strSQL = strSQL + " FROM IFR_Simulacao_Diaria SD INNER JOIN IFR_Simulacao_Diaria_Detalhe D " + Environment.NewLine;
				strSQL = strSQL + " ON SD.Codigo = D.Codigo " + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_Setup = D.ID_Setup " + Environment.NewLine;
				strSQL = strSQL + " AND SD.Data_Entrada_Efetiva = D.Data_Entrada_Efetiva " + Environment.NewLine;
				strSQL = strSQL + " WHERE SD.Codigo = " + FuncoesBd.CampoFormatar(_ativo.Codigo) + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_Setup = " + FuncoesBd.CampoFormatar(_setup.Id) + Environment.NewLine;
				strSQL = strSQL + " AND SD.ID_CM = " + FuncoesBd.CampoFormatar(pobjCalcularFaixaResumo.ClassifMedia.ID) + Environment.NewLine;
				strSQL = strSQL + " AND Data_Saida <= " + FuncoesBd.CampoFormatar(pobjCalcularFaixaResumo.DataSaida) + Environment.NewLine;
				strSQL = strSQL + " AND Valor_IFR_Minimo <= " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ValorMaximo) + Environment.NewLine;
				strSQL = strSQL + " AND D.ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.Id) + Environment.NewLine;

				objRSTrades.AdicionarQuery(strSQL);

				objRSTrades.ExecuteQuery();


			    if (objRSTrades.Dados.Count > 0) {
					//Só é necessário calcular as faixas se existem simulações para a classificação de média desta iteração.

					//Calcula o número de trades que são melhor entrada.
					var intQuantidadeMelhorEntrada = objRSTrades.Dados.Count(linha => Convert.ToBoolean(linha["MelhorEntrada"]));


					if (intQuantidadeMelhorEntrada > 0) {
						//Para cada uma das classificações e dos critérios de classificação, calcula as faixas.

						foreach (CriterioClassifMedia objCriterioCM in lstCriterioCM) {
							string strCampo = objCriterioCM.AliasBD;

							//transforma a consulta SQL em um objeto da classe cTradeDTO
							IList<TradeDto> lstTrades = (from linha in objRSTrades.Dados select new TradeDto( (double) linha[strCampo], Convert.ToBoolean(linha["Verdadeiro"]), Convert.ToBoolean(linha["MelhorEntrada"]), Convert.ToInt32(linha["NumTentativas"]))).ToList();

							IList<FaixaDto> lstFaixasDTO = CalcularFaixaParaUmCriterio(lstTrades);


							foreach (FaixaDto item in lstFaixasDTO) {
								lstFaixasTotal.Add(new IFRSimulacaoDiariaFaixa(_ativo.Codigo, _setup, pobjCalcularFaixaResumo.ClassifMedia, objCriterioCM, pobjIFRSobrevendido, pobjCalcularFaixaResumo.DataSaida, item.NumTentativasMinimo, item.ValorMinimo, item.ValorMaximo));

							}

							//limpa collectin para a próxima iteração
							lstTrades.Clear();

						}

					}

				}

			    var repositorio = new RepositorioDeIfrSimulacaoDiariaFaixa(_conexao);

				//Faz a persistência de todas as faixas no banco de dados.
				foreach (IFRSimulacaoDiariaFaixa objFaixa in lstFaixasTotal) {
                    repositorio.Salvar(objFaixa);
				}

				return true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message,"Trader Wizard", MessageBoxButtons.OK,MessageBoxIcon.Error);

				return false;

			}

		}

	}
}
