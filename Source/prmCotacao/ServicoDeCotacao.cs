using System.Linq;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataBase.Carregadores;
using Ionic.Zip;
using prjDominio.Regras;
using prjServicoNegocio;
using Services;
using prjModelo.Carregadores;
using prjModelo;
using pWeb;
using DataBase;
using prjDTO;
using prjConfiguracao;
using TraderWizard.Enumeracoes;

namespace prmCotacao
{

	public class ServicoDeCotacao
	{

		/// <summary>
		/// Propriedade que contém a conexão ativa com o banco de dados, para não precisar
		/// passar por parâmetro em todas as funções
		/// </summary>
		/// <remarks></remarks>

		private readonly Conexao objConexao;
		/// <summary>
		/// contém o objeto que faz downloads na internet.
		/// </summary>
		/// <remarks></remarks>

		private readonly cWeb objWeb;
		//Private Sub ConexaoIniciar()
		//    objConexao = New cConexao
		//End Sub

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pobjConexao">Recebe a conexão da tela principal. 
		/// Toda clase que acessa o banco de dados tem que receber a conexão
		/// por parâmetro</param>
		/// <remarks></remarks>

		public ServicoDeCotacao(Conexao pobjConexao)
		{
			objConexao = pobjConexao;
			objWeb = new cWeb(pobjConexao);

		}

	    public ServicoDeCotacao()
	    {
	        
	    }

		/// <summary>
		/// Atualiza as cotações em todas as datas em que há pregão em um determinado período.
		/// </summary>
		/// <param name="pdtmDataInicial">Data inicial de importação</param>
		/// <param name="pdtmDataFinal">Data final de importação</param>
		/// <param name="pstrCodigoUnico">Indica se é para importar o código de um único ativo. Se o parâmetro for uma string vazia deve importar todos os arquivos</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CotacaoPeriodoAtualizar(DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrCodigoUnico, bool pblnCalcularDados)
		{

			//inicializa a data com data inválida. Vai ser atribuido nesta variável o primeiro dia útil.
			var dtmDataInicialAux = Constantes.DataInvalida;

			//indica se o período informado é uma única data
			//Dim blnDataUnica As Boolean = (pdtmDataInicial = pdtmDataFinal)

			bool blnOk = true;

		    DateTime dtmDataUltimaCotacao = Constantes.DataInvalida;

			var objCalculadorData = new cCalculadorData(objConexao);

			//enquanto a data inicial for menor que a data final.

			while ((pdtmDataInicial <= pdtmDataFinal) && blnOk) {

				if (objCalculadorData.DiaUtilVerificar(pdtmDataInicial)) {
					if (dtmDataInicialAux == Constantes.DataInvalida) {
						//se a data que será utilizada nos cálculos de IFR, MÉDIA, ETC 
						//ainda não foi atribuida, atribui com o primeiro dia útil.
						dtmDataInicialAux = pdtmDataInicial;
					}

					cEnum.enumRetorno intRetorno = CotacaoDataAtualizar(pdtmDataInicial, pstrCodigoUnico);


					if (intRetorno == cEnum.enumRetorno.RetornoOK) {
						dtmDataUltimaCotacao = pdtmDataInicial;


					} else if (intRetorno == cEnum.enumRetorno.RetornoErro2) {
						//não conseguiu baixar o arquivo na data.

						//se é uma data única avisa para o usuário.
						//se é mais de uma data não faz nada e busca a próxima data

						//If blnDataUnica Then

						blnOk = false;
                        MessageBox.Show("Não foi possível baixar o arquivo de cotações na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					    //End If


					} else if (intRetorno == cEnum.enumRetorno.RetornoErro3) {
						blnOk = false;
						//já existe cotação na data
                        MessageBox.Show("Já existe cotação na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);



					} else if (intRetorno == cEnum.enumRetorno.RetornoErroInesperado) {
						//erro na transação. Sai fora porque ocorreu um erro
						blnOk = false;
                        MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					}

				}

				//incrementa um dia na data inicial
				pdtmDataInicial = pdtmDataInicial.AddDays(1);

			}

			//********MAURO, 11/05/2010
			//Atualiza a tabela de resumo.

			if (dtmDataUltimaCotacao != Constantes.DataInvalida) {
				//se alguma cotação foi atualizada com sucesso atualiza a tabela de resumo.
				TabelaResumoAtualizar(dtmDataUltimaCotacao, Constantes.DataInvalida);


				if (blnOk) {

					if (pblnCalcularDados) {
						DadosRecalcular(true, false, true, true, true, true, true, true, true, true,
						true, dtmDataInicialAux);

					}

                    MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				}


			} else {
                MessageBox.Show("Não existem cotações para serem atualizadas.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

			return blnOk;

		}


		/// <summary>
		/// "Faz download do arquivo de cotações da Bovespa de uma determinada data e salva na tabela em Access"
		/// </summary>
		/// <param name="pdtmData"></param>
		/// <param name="pstrCodigoUnico">Indica se é para importar o código de um único ativo. Se o parâmetro for uma string vazia deve importar todos os arquivos</param>
		/// <returns></returns>
		///RetornoOK = operação realizada com sucesso.
		///RetornoErro2 = não foi possível baixar o arquivo na data especificada
		///RetornoErroInesperado = Erro ao executar a operação de atualização de cotas.             
		///RetornoErro3 = Já existe cotações na data específicada    
		/// <remarks></remarks>
		private cEnum.enumRetorno CotacaoDataAtualizar(DateTime pdtmData, string pstrCodigoUnico)
		{
		    cCommand objCommand = new cCommand(objConexao);

			IList<string> colLinha;

		    string strCodigoAtivo = String.Empty;

			decimal decValorAbertura = default(decimal);
			decimal decValorFechamento = default(decimal);
			decimal decValorMinimo = default(decimal);
			decimal decValorMedio = default(decimal);
			decimal decValorMaximo = default(decimal);

			decimal decOscilacao = default(decimal);

		    long lngNegociosTotal = 0;
			long lngTitulosTotal = 0;
			decimal decValorTotal = default(decimal);

		    const char strSeparadorDecimal = ',';


			cRS objRS = new cRS(objConexao);

		    //utilizado para calcular o sequencial do ativo.

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + FuncoesBd.CampoDateFormatar(pdtmData));


			if ((int) objRS.Field("Contador") > 0) {
				DateTime[] arrData = { pdtmData };


				if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK) {
					objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    

					return cEnum.enumRetorno.RetornoErroInesperado;

				}

			}

			objRS.Fechar();

			objCommand.BeginTrans();

		    bool blnCotacaoExistir = pstrCodigoUnico == string.Empty ? CotacaoDataExistir(pdtmData, "Cotacao") : CotacaoDataExistir(pdtmData, "Cotacao", pstrCodigoUnico);

			if (blnCotacaoExistir) {
				//se já existe alguma cotação na data,
				//faz rollback e sai da função
				objCommand.RollBackTrans();

				return cEnum.enumRetorno.RetornoErro3;

			}

			//baixar as cotações do site da bovespa

			if (ArquivoDataBaixar(pdtmData, out colLinha)) {
				//busca os ativos que não devem ser consideros
				string strAtivosDesconsiderados = AtivosDesconsideradosListar();

			    //se foi possivel baixar...
				//percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados

			    int intI;
			    for (intI = 0; intI < colLinha.Count; intI++) {
					//coloca a linha na variável auxiliar
					string strLinhaAux = colLinha[intI];

					//os dois primeiros caracteres indicam o tipo de registro.
					//o tipo de registro 02 indica que é a cotação de um papel
					//o terceiro e quarto caracteres indicam o código BDI do papel.
					//O código 02 indica que é um papel do lote padrão
					//posição 70 - 73 indica o tipo  de mercado do ativo
					//o tipo de mercado 010 é o mercado A VISTA
			        bool blnImportarLinha;
			        bool blnInserir;
			        string strOscilacao;
			        if (strLinhaAux.Substring(0,4) + strLinhaAux.Substring( 69, 3) == "0202010") {
						//se é a cotação de um papel
						//e é do mercado à vista.

						//busca código do ativo, posicao 58-69
						strCodigoAtivo = strLinhaAux.Substring(57, 12).Trim();


						if (pstrCodigoUnico == string.Empty) {
							//Se é para importar todos os códigos, verifica se o mesmo não se encontra na lista de ativos desconsiderados
                            blnImportarLinha = (strAtivosDesconsiderados.IndexOf("#" + strCodigoAtivo + "#",StringComparison.InvariantCultureIgnoreCase) == -1);
                            
						} else {
							//Se é para importar um único ativo, verificar se a linha refere-se ao código que deve ser importado
							blnImportarLinha = (strCodigoAtivo == pstrCodigoUnico);
						}

						//verifica se deve importar a linha
						if (blnImportarLinha) {
							//se o ativo não foi encontrado na lista dos desconsiderados.

							//TOTAL DE NEGÓCIOS (174-178)
							lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(173, 5));

							//algumas ações não tem negócios no dia. Estas cotações não serão importadas.

							if (lngNegociosTotal > 0) {
								//busca valor de abertura do ativo: 91 - 99 (inteiro), 100-101 (decimal)
								decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring( 90, 9) + strSeparadorDecimal + strLinhaAux.Substring(99, 2));

								//busca o valor máximo do ativo: 102-110 (inteiro), 111-112 (decimal)
								decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(101, 9) + strSeparadorDecimal + strLinhaAux.Substring(110, 2));

								//busca o valor mínimo do ativo: 113-121 (inteiro), 122-123 (decimal)
								decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(112, 9) + strSeparadorDecimal + strLinhaAux.Substring(121, 2));

								//busca o valor médio do ativo: 124-132 (inteiro), 133-134 (decimal)
								decValorMedio = Convert.ToDecimal(strLinhaAux.Substring( 123, 9) + strSeparadorDecimal + strLinhaAux.Substring(132, 2));

								//busca o valor de fechamento do ativo: 135-143 (inteiro), 144-145 (decimal)
								decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(134, 9) + strSeparadorDecimal + strLinhaAux.Substring(143, 2));

								//busca a oscilação do papel em relação ao dia anterior
								//146 = sinal da oscilação (+ ou -)
								//147-149 = parte inteira da oscilação
								//150-151 = parte decimal da oscilação

								strOscilacao = strLinhaAux.Substring( 146, 3) + strSeparadorDecimal + strLinhaAux.Substring(149, 2);


								if (strLinhaAux.Substring(145, 1) == "-") {
									strOscilacao = "-" + strOscilacao;

								}

								decOscilacao = Convert.ToDecimal(strOscilacao);

								//TOTAL DE TÍTULOS NEGOCIADOS (179-193)
								lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(178, 15));

								//VALOR TOTAL NEGOCIADO: 194-208 (inteiro), 209-210 (decimal)
								decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(193, 15) + strSeparadorDecimal + strLinhaAux.Substring(208, 2));

								blnInserir = true;

							} else {
								//se não teve negócios no dia
								blnInserir = false;

							}
							//if lngNegocios_Total > 0 then


						} else {
							//se o ativo deve ser desconsiderado
							blnInserir = false;

						}


					} else if (strLinhaAux.Substring(0,12) == "0101IBOVESPA") {
						if (pstrCodigoUnico == string.Empty) {
							blnImportarLinha = true;
						} else {
							blnImportarLinha = (pstrCodigoUnico == "IBOV");
						}


						if (blnImportarLinha) {
							//é o indice BOVESPA
							strCodigoAtivo = "IBOV";

							//busca valor de abertura do ativo: 35 - 40
							decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring(34, 6));

							//busca o valor máximo do ativo: 41-46
							decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(40, 6));

							//busca o valor mínimo do ativo: 47-52
							decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(46, 6));

							//busca o valor médio do ativo: 53-58
							decValorMedio = Convert.ToDecimal(strLinhaAux.Substring(52, 6));

							//busca o valor de fechamento do ativo: 93-98
							decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(92, 6));

							//busca a oscilação do papel em relação ao dia anterior
							//99 = sinal da oscilação (+ ou -)
							//100-102 = parte inteira da oscilação
							//103-104 = parte decimal da oscilação

							strOscilacao = strLinhaAux.Substring(99, 3) + strSeparadorDecimal + strLinhaAux.Substring(102, 2);


							if (strLinhaAux.Substring(98, 1) == "-") {
								strOscilacao = "-" + strOscilacao;

							}

							decOscilacao = Convert.ToDecimal(strOscilacao);

							//TOTAL DE NEGÓCIOS (159-164)
							lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(158, 6));

							//TOTAL DE TÍTULOS NEGOCIADOS (165-179)
							lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(164, 15));

							//VALOR TOTAL NEGOCIADO: 180-194 (inteiro), 195-196 (decimal)
							decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(179, 15) + strSeparadorDecimal + strLinhaAux.Substring(194, 2));

							blnInserir = true;


						} else {
							blnInserir = false;

						}


					} else {
						//não é um ativo do mercado à vista, nem o indice BOVESPA
						blnInserir = false;

					}
					//se é uma linha do mercado à vista.


					if (blnInserir) {
						//calcula o sequencial do ativo
						long lngSequencial = SequencialCalcular(strCodigoAtivo, "Cotacao", objCommand.Conexao);

						//insere na tabela
						string strQuery = " insert into Cotacao " + "(Codigo, Data, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo, Oscilacao " + ", Negocios_Total, Titulos_Total, Valor_Total, Sequencial) " + " values " + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtivo) + "," + FuncoesBd.CampoDateFormatar(pdtmData) + "," + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + "," + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + "," + FuncoesBd.CampoDecimalFormatar(decValorMinimo) + "," + FuncoesBd.CampoDecimalFormatar(decValorMedio) + "," + FuncoesBd.CampoDecimalFormatar(decValorMaximo) + "," + FuncoesBd.CampoDecimalFormatar(decOscilacao) + "," + lngNegociosTotal.ToString() + "," + lngTitulosTotal.ToString() + "," + FuncoesBd.CampoDecimalFormatar(decValorTotal) + "," + lngSequencial.ToString() + ")";

						objCommand.Execute(strQuery);

					}

				}


			} else {
				//se não conseguiu baixar o arquivo
				objCommand.RollBackTrans();

				return cEnum.enumRetorno.RetornoErro2;

			}
			//if ArquivoDataBaixar

			objCommand.CommitTrans();

			cEnum.enumRetorno functionReturnValue = objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		    return functionReturnValue;

		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="pstrCaminho"> Pasta onde se encontra o arquivo </param>
		/// <param name="pstrArquivoZip"> Nome do arquivo que deve ser descompactado </param>
		/// <param name="pstrArquivoTexto"> Nome do arquivo contido dentro do arquivo zip  </param>
		/// <param name="pcolLinhaRet"> retorna collection com todas as linhas do arquivo texto descompactado</param>
		/// <returns> 
		/// True = arquivo zip descomptactado e arquivo texto lido com sucesso 
		/// False = algum erro na descompactação do arquivo zip ou leitura do arquivo texto
		/// </returns>
		/// <remarks></remarks>
		private bool ArquivoDescompactar(string pstrCaminho, string pstrArquivoZip, string pstrArquivoTexto, out IList<string> pcolLinhaRet)
		{
            var zipFile = new ZipFile(pstrCaminho + "\\" + pstrArquivoZip);

            zipFile.ExtractAll(pstrCaminho);

            IFileService fileService = new FileService();

            //o conteúdo do arquivo zipado é um arquivo chamado BDIN, sem extensão
            //abre o arquivo para leitura
            pcolLinhaRet = fileService.ReadAllLines(pstrCaminho + "\\" + pstrArquivoTexto);

			//apaga o arquivo BDIN que foi extraído do zip.
			fileService.Delete(pstrCaminho + "\\" + pstrArquivoTexto) ;

			return true;

		}


		private bool ArquivoDataBaixar(DateTime pdtmData, out IList<string> pcolLinhaRet)
		{
		    string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

			strPathZip = strPathZip + "Arquivos";

			//Verifica se existe o diretório para armazenar os arquivos baixados

		    var fileService = new FileService();
            fileService.CreateFolder(strPathZip);

			//gera o nome do arquivo que deve ser baixado
			string strArquivoBaixar = cGeradorNomeArquivo.GerarNomeArquivoRemoto(pdtmData);

			//Nome que será dado ao arquivo baixado :"bdi" + yyyymmdd + ".zip"
			string strArquivoZipDestino = cGeradorNomeArquivo.GerarNomeArquivoLocal(pdtmData);

			if (!fileService.FileExists(strPathZip + "\\" + strArquivoZipDestino)) 
            {
			
				if (!objWeb.DownloadWithProxy("http://www.bmfbovespa.com.br/fechamento-pregao/bdi/" + strArquivoBaixar, strPathZip, strArquivoZipDestino))
				{
				    pcolLinhaRet = new List<string>();
				    return false;
				}
            }

		    //CHAMA FUNÇÃO QUE DESCOMPACTA O ARQUIVO, ABRE O ARQUIVO TEXTO, FAZ A LEITURA E RETORNA TODAS AS LINHAS
		    //EM UMA COLLECTION.
		    bool blnDescompactar = ArquivoDescompactar(strPathZip, strArquivoZipDestino, "BDIN", out pcolLinhaRet);

		    return blnDescompactar;

		}

		public decimal UltimaMediaConsultar(string pstrCodigo)
		{
		    cRS objRS = new cRS(objConexao);

		    string strQuery = " select ValorMedio " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " order by Data desc ";

			objRS.ExecuteQuery(strQuery);

			decimal functionReturnValue = Convert.ToDecimal(objRS.Field("ValorMedio", "0"));

			objRS.Fechar();
			return functionReturnValue;

		}

	    /// <summary>
	    /// Recebe uma collection com as linhas de um arquivo
	    /// </summary>
	    /// <param name="pcolLinha">Collection contendo as linhas do arquivo de cotações</param>
	    /// <param name="pobjCommand"></param>
	    /// <returns>status da transação</returns>
	    /// <remarks></remarks>
	    private void CotacoesImportar(IList<string> pcolLinha, cCommand pobjCommand)
		{
			int intI;

		    DateTime dtmCotacaoData = default(DateTime);

	        //utilizado para calcular o sequencial do ativo.

		    const char strSeparadorDecimal = ',';

			//busca os ativos que não devem ser consideros
			string strAtivosDesconsiderados = AtivosDesconsideradosListar();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//se foi possivel baixar...
			//percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados

			for (intI = 1; intI <= pcolLinha.Count; intI++)
			{
			    //coloca a linha na variável auxiliar
			    string strLinhaAux = pcolLinha[intI];

			    //os dois primeiros caracteres indicam o tipo de registro.
				//o tipo de registro 01 indica que é a cotação de um papel do mercado a vista

				//posição 11-12 indica o código BDI do papel. O código 02 indica que é um LOTE PADRÃO

				//posição 25 - 27 indica o tipo  de mercado do ativo
				//o tipo de mercado 010 é o mercado A VISTA
				if (strLinhaAux.Substring(0, 2) + strLinhaAux.Substring(10, 2) + strLinhaAux.Substring(24, 3) == "0102010")
				{
				    //se é a cotação de um papel
					//e é do mercado à vista.

					//busca código do ativo, posicao 13-24
				    string strCodigoAtivo = strLinhaAux.Substring(12, 12).Trim();

				    //verifica se o ativo não deve ser desconsiderado
					if (strAtivosDesconsiderados.IndexOf("#" + strCodigoAtivo + "#", StringComparison.InvariantCultureIgnoreCase ) < 0)
					{
					    //se o ativo não foi encontrado na lista dos desconsiderados.

						//TOTAL DE NEGÓCIOS (148-152)
					    long lngNegociosTotal = Convert.ToInt64(strLinhaAux.Substring(147, 5));

					    //ALGUNS ATIVOS NÃO TEM NEGÓCIOS E NÃO DEVEM SER IMPORTADOS

						if (lngNegociosTotal > 0) {
							//busca a data da cotação: 3-10 no formato YYYYMMDD
							dtmCotacaoData = new DateTime(Convert.ToInt32(strLinhaAux.Substring(2, 4)), Convert.ToInt32(strLinhaAux.Substring(6, 2)), Convert.ToInt32(strLinhaAux.Substring(8, 2)));

							//busca valor de abertura do ativo: 57 - 67 (inteiro), 68-69 (decimal)
							decimal decValorAbertura = Convert.ToDecimal(strLinhaAux.Substring(56, 11) + strSeparadorDecimal + strLinhaAux.Substring(67, 2));

							//busca o valor máximo do ativo: 70-80 (inteiro), 81-82 (decimal)
							decimal decValorMaximo = Convert.ToDecimal(strLinhaAux.Substring(69, 11) + strSeparadorDecimal + strLinhaAux.Substring(80, 2));

							//busca o valor mínimo do ativo: 83-93 (inteiro), 94-95 (decimal)
							decimal decValorMinimo = Convert.ToDecimal(strLinhaAux.Substring(82, 11) + strSeparadorDecimal + strLinhaAux.Substring(93, 2));

							//busca o valor médio do ativo: 96-106 (inteiro), 107-108 (decimal)
							decimal decValorMedio = Convert.ToDecimal(strLinhaAux.Substring(95, 11) + strSeparadorDecimal + strLinhaAux.Substring(106, 2));

							//busca o valor de fechamento do ativo: 109-119 (inteiro), 120-121 (decimal)
							decimal decValorFechamento = Convert.ToDecimal(strLinhaAux.Substring(108, 11) + strSeparadorDecimal + strLinhaAux.Substring(119, 2));

							//TOTAL DE TÍTULOS NEGOCIADOS (153-170)
							long lngTitulosTotal = Convert.ToInt64(strLinhaAux.Substring(152, 18));

							//VALOR TOTAL NEGOCIADO: 171-186 (inteiro), 187-188 (decimal)
							decimal decValorTotal = Convert.ToDecimal(strLinhaAux.Substring(170, 16) + strSeparadorDecimal + strLinhaAux.Substring(186, 2));

							//calcula o sequencial do ativo
							long lngSequencial = SequencialCalcular(strCodigoAtivo, "Cotacao", pobjCommand.Conexao);

							//insere na tabela
							string strQuery = " insert into Cotacao " + "(Codigo, Data, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo " + ", Negocios_Total, Titulos_Total, Valor_Total, Sequencial) " + " values " + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtivo) + "," + FuncoesBd.CampoDateFormatar(dtmCotacaoData) + "," + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + "," + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + "," + FuncoesBd.CampoDecimalFormatar(decValorMinimo) + "," + FuncoesBd.CampoDecimalFormatar(decValorMedio) + "," + FuncoesBd.CampoDecimalFormatar(decValorMaximo) + "," + lngNegociosTotal.ToString() + "," + lngTitulosTotal.ToString() + "," + FuncoesBd.CampoDecimalFormatar(decValorTotal) + "," + lngSequencial.ToString() + ")";

							pobjCommand.Execute(strQuery);

						}
						//if lngNegocios_Total > 0 then
					}
				    //se o ativo não foi desconsiderado
				}
			    //se é uma cotação à vista.
			}

		    //atualiza a tabela de resumo
			TabelaResumoAtualizar(dtmCotacaoData,Constantes.DataInvalida);
		}

		/// <summary>
		/// Recebe uma data e procura um arquivo de cotação histórica na pasta Arquivos.
		/// Importa as cotações deste arquivo e salva na tabela.
		/// </summary>
		/// <param name="pdtmData"> Indica a data que deve ser feita a atualização </param>
		/// <returns>
		///RetornoOK = operação realizada com sucesso.
		///RetornoErro2 = não foi possível encontrar o arquivo na data especificada
		///RetornoErroInesperado = Erro ao executar a operação de atualização de cotas.             
		///RetornoErro3 = Já existe cotações na data específicada       
		/// </returns>
		/// <remarks></remarks>
		private cEnum.enumRetorno CotacaoHistoricaDataAtualizar(DateTime pdtmData)
		{

			cCommand objCommand = new cCommand(objConexao);

			IList<string> colLinha = new List<string>();

		    //nome dado ao arquivo zip que for baixado

		    string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

			strPathZip = strPathZip + "Arquivos";

			//Nome do arquivo zip baixado :"COTAHIST_D" + ddmmyyyy + ".zip"
			string strArquivoZipDestino = "COTAHIST_D" + pdtmData.ToString("ddMMyyyy") + ".ZIP";

			string strArquivoTextoDestino = "COTAHIST_D" + pdtmData.ToString("ddMMyyyy") + ".TXT";

			cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + FuncoesBd.CampoDateFormatar(pdtmData));


			if (Convert.ToInt32( objRS.Field("Contador")) > 0) {
				DateTime[] arrData = { pdtmData };


				if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK) {
					objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);

					return cEnum.enumRetorno.RetornoErroInesperado;

				}

			}

			objRS.Fechar();

			//**************** INICIO DA TRANSAÇÃO
			objCommand.BeginTrans();


			if (CotacaoDataExistir(pdtmData, "Cotacao")) {
				//se já existe cotação na data faz rollback para não sobrescrever
				objCommand.RollBackTrans();

				//retorno erro conforme descrito no cabeçalho da função
				return cEnum.enumRetorno.RetornoErro3;

			}

			//descompactar o arquivo que contém as cotações

			if (ArquivoDescompactar(strPathZip, strArquivoZipDestino, strArquivoTextoDestino, out colLinha)) {
				CotacoesImportar(colLinha, objCommand);

			}

			//**************** FIM DA TRANSAÇÃO
			objCommand.CommitTrans();

			return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;
		}

		/// <summary>
		/// Atualiza as cotações de um ano todo
		/// </summary>
		/// <param name="pintAno">Ano da cotação</param>
		/// <returns></returns>
		/// RetornoOK - Operação realizada com sucesso.
		/// RetornoErroInesperado - algum erro inesperado de banco de dados ou de programação
		/// RetornoErro2 - Já existe cotação no ano.
		/// RetornoErro3 - Não foi possível descompactar o arquivo zip ou abrir o arquivo txt.
		/// <remarks></remarks>
		public cEnum.enumRetorno CotacaoHistoricaAnoAtualizar(int pintAno)
		{

			cCommand objCommand = new cCommand(objConexao);

			//objConexao = objCommand.Conexao

			cRS objRS = new cRS(objConexao);

			IList<string> colLinha;

		    //nome dado ao arquivo zip que for baixado

		    string strPathZip = cBuscarConfiguracao.ObtemCaminhoPadrao();

			strPathZip = strPathZip + "Arquivos";

			//Nome do arquivo zip baixado :"COTAHIST_A" + YYYY + ".zip"
			string strArquivoZipDestino = "COTAHIST_A" + Convert.ToString(pintAno) + ".ZIP";

			string strArquivoTextoDestino = "COTAHIST_A" + Convert.ToString(pintAno) + ".TXT";

			//**************** INICIO DA TRANSAÇÃO
			objCommand.BeginTrans();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//verifica se tem alguma cotação no ano. Se já existir avisa para o usuário.
		    objRS.ExecuteQuery(" select count(1) as contador " + " from Cotacao " + " where Data >= " +
		                       FuncoesBd.CampoDateFormatar(new DateTime(pintAno, 1, 1)) + " and Data <= " +
		                       FuncoesBd.CampoDateFormatar(new DateTime(pintAno, 12, 31)));


			if (Convert.ToInt64(objRS.Field("Contador", 0)) > 0) {
				//se já existe cotação na data faz rollback para não sobrescrever
				objCommand.RollBackTrans();

				objRS.Fechar();

				//retorno erro conforme descrito no cabeçalho da função
				return cEnum.enumRetorno.RetornoErro2;

			}

			objRS.Fechar();

			//descompactar o arquivo que contém as cotações

			if (ArquivoDescompactar(strPathZip, strArquivoZipDestino, strArquivoTextoDestino, out colLinha)) {
				CotacoesImportar(colLinha, objCommand);


			} else {
				objCommand.RollBackTrans();
				return cEnum.enumRetorno.RetornoErro3;

			}

			//**************** FIM DA TRANSAÇÃO
			objCommand.CommitTrans();

			return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		}

		/// <summary>
		/// Verifica se existe alguma cotação em uma determinada data
		/// </summary>
		/// <param name="pdtmData"> Data que deve ser verificada se existe cotação </param>
		/// <returns>
		/// True = Existe cotação na data
		/// False = Não existe cotação na data
		/// </returns>
		/// <remarks></remarks>
		public bool CotacaoDataExistir(DateTime pdtmData, string pstrTabela)
		{
		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strQuery = " select 1 " + " from " + pstrTabela + " where Data = " + FuncoesBd.CampoDateFormatar(pdtmData);

			objRS.ExecuteQuery(strQuery);

			bool functionReturnValue = objRS.DadosExistir;

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Verifica se existe alguma cotação em uma determinada data para um determinado ativo
		/// </summary>
		/// <param name="pdtmData"> Data que deve ser verificada se existe cotação </param>
		/// <returns>
		/// True = Existe cotação na data
		/// False = Não existe cotação na data
		/// </returns>
		/// <remarks></remarks>
		public bool CotacaoDataExistir(DateTime pdtmData, string pstrTabela, string pstrCodigoAtivo)
		{
		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strQuery = " select 1 " + " FROM " + pstrTabela + " WHERE Data = " + FuncoesBd.CampoFormatar(pdtmData) + " AND Codigo = " + FuncoesBd.CampoFormatar(pstrCodigoAtivo);

			objRS.ExecuteQuery(strQuery);

			bool functionReturnValue = objRS.DadosExistir;

			objRS.Fechar();
			return functionReturnValue;

		}


		/// <summary>
		/// Consulta alguns dados da tabela de cotação ou cotação semanal
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmData">data em que deve ser feita a consulta</param>
		/// <param name="pstrTabela">
		/// COTACAO
		/// COTACAO_SEMANAL
		/// </param>
		/// <param name="pdecValorFechamentoRet"></param>
		/// <param name="pdecValorAberturaRet"></param>
		/// <param name="pobjConexao"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CotacaoConsultar(string pstrCodigo, DateTime pdtmData, string pstrTabela, ref decimal pdecValorFechamentoRet, ref decimal pdecValorAberturaRet, Conexao pobjConexao = null)
		{

		    cRS objRS = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strQuery = " select ValorAbertura, ValorFechamento " + " from " + pstrTabela + " where Data = " + FuncoesBd.CampoDateFormatar(pdtmData) + " and Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo);

			objRS.ExecuteQuery(strQuery);

			pdecValorAberturaRet = Convert.ToDecimal(objRS.Field("ValorAbertura", "0"));
			pdecValorFechamentoRet = Convert.ToDecimal(objRS.Field("ValorFechamento", "0"));

			bool functionReturnValue = objRS.DadosExistir;

			objRS.Fechar();
			return functionReturnValue;

		}


		/// <summary>
		/// Atualiza as cotações em todas as datas em que há pregão em um determinado período.
		/// Os arquivos já devem estar baixados
		/// </summary>
		/// <param name="pdtmDataInicial">Data inicial do cálculo</param>
		/// <param name="pdtmDataFinal">Data final do cálculo</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CotacaoHistoricaPeriodoAtualizar(DateTime pdtmDataInicial, DateTime pdtmDataFinal, bool pblnCalcularDados)
		{

			//inicializa a data com data inválida. Vai ser atribuido nesta variável o primeiro dia útil.
			DateTime dtmDataInicialAux = Constantes.DataInvalida;

			//indica se o período informado é uma única data
			bool blnDataUnica = (pdtmDataInicial == pdtmDataFinal);

			bool blnOK = true;

		    DateTime dtmData_Ultima_Cotacao = Constantes.DataInvalida;

			cCalculadorData objCalculadorData = new cCalculadorData(objConexao);

			//enquanto a data inicial for menor que a data final.

			while ((pdtmDataInicial <= pdtmDataFinal) && blnOK) {

				if (objCalculadorData.DiaUtilVerificar(pdtmDataInicial)) {
					if (dtmDataInicialAux == Constantes.DataInvalida) {
						//se a data que será utilizada nos cálculos de IFR, MÉDIA, ETC 
						//ainda não foi atribuida, atribui com o primeiro dia útil.
						dtmDataInicialAux = pdtmDataInicial;
					}

					cEnum.enumRetorno intRetorno = CotacaoHistoricaDataAtualizar(pdtmDataInicial);


					if (intRetorno == cEnum.enumRetorno.RetornoOK) {
						dtmData_Ultima_Cotacao = pdtmDataInicial;


					} else if (intRetorno == cEnum.enumRetorno.RetornoErro2) {
						//não conseguiu baixar o arquivo na data.

						//se é uma data única avisa para o usuário.
						//se é mais de uma data não faz nada e busca a próxima data
						if (blnDataUnica) {
							blnOK = false;
                            MessageBox.Show("Não foi possível baixar o arquivo de cotações na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						}


					} else if (intRetorno == cEnum.enumRetorno.RetornoErro3) {
						blnOK = false;
						//já existe cotação na data
                        MessageBox.Show("Já existe cotação na data " + pdtmDataInicial.ToString("dd/MM/yyyy") + ".", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);



					} else if (intRetorno == cEnum.enumRetorno.RetornoErroInesperado) {
						//erro na transação. Sai fora porque ocorreu um erro
						blnOK = false;
                        MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					}

				}

				//incrementa um dia na data inicial
				pdtmDataInicial = pdtmDataInicial.AddDays(1);

			}

			//********MAURO, 11/05/2010
			//Atualiza a tabela de resumo.

			if (dtmData_Ultima_Cotacao != Constantes.DataInvalida) {
				//se alguma cotação foi atualizada com sucesso atualiza a tabela de resumo.
				TabelaResumoAtualizar(dtmData_Ultima_Cotacao,Constantes.DataInvalida);

			}

			if (blnOK) {
				if (pblnCalcularDados) {
					DadosRecalcular(true, true, true, true, true, true, true, true, true, true,
					true, dtmDataInicialAux);
				}

			    MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK,MessageBoxIcon.Exclamation);

			}

			return blnOK;

		}

		/// <summary>
		/// Busca a data da cotação imediatamente anterior a uma data recebida por parâmetro
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">Data base utilizada para buscar a cotação anterior</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <returns></returns>
		/// <remarks></remarks>
		private DateTime AtivoCotacaoAnteriorDataConsultar(string pstrCodigo, DateTime pdtmDataBase, string pstrTabela, Conexao pobjConexao = null)
		{
			DateTime functionReturnValue;

			cRS objRsData;

			if (pobjConexao == null) {
				objRsData = new cRS(objConexao);
			} else {
				objRsData = new cRS(pobjConexao);
			}

			string strPeriodo;

			pstrTabela = pstrTabela.ToUpper();

			switch (pstrTabela)
			{
			    case "COTACAO":
			        strPeriodo = "DIARIO";
			        break;
			    case "COTACAO_SEMANAL":
			        strPeriodo = "SEMANAL";
			        break;
			    default:
			        strPeriodo = String.Empty;
			        break;
            }

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			objRsData.ExecuteQuery("SELECT Data_Anterior" + " FROM Cotacao_Anterior" + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " AND Data = " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " AND Periodo = " + FuncoesBd.CampoStringFormatar(strPeriodo));


			if (objRsData.DadosExistir) {
				functionReturnValue = Convert.ToDateTime(objRsData.Field("Data_Anterior"));


			} else {
				objRsData.Fechar();

				//Busca a data imediatamente anterior que tem uma cotação para o ativo recebido por parâmetro.
				objRsData.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data < " + FuncoesBd.CampoDateFormatar(pdtmDataBase));

				functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

			}

			objRsData.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a cotação imediatamente posterior a uma data para um determinado ativo.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pdtmDataBase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private DateTime AtivoCotacaoPosteriorDataConsultar(string pstrCodigo, DateTime pdtmDataBase)
		{
		    cRS objRsData = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			objRsData.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data > " + FuncoesBd.CampoDateFormatar(pdtmDataBase));

			DateTime functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

			objRsData.Fechar();
			return functionReturnValue;

		}

		private DateTime AtivoPrimeiraCotacaoDataConsultar(string pstrCodigo)
		{
			DateTime functionReturnValue = default(DateTime);

			cRS objRS = new cRS(objConexao);

			objRS.ExecuteQuery("SELECT MIN(Data) AS Data " + Environment.NewLine + "FROM Cotacao " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

			functionReturnValue = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Busca a data da cotação imediatamente anterior a uma data recebida por parâmetro
		/// </summary>
		/// <param name="pdtmDataBase">Data base utilizada para buscar a cotação anterior</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime CotacaoAnteriorDataConsultar(DateTime pdtmDataBase, string pstrTabela, Conexao pobjConexao = null)
		{
		    cRS objRsData = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//Busca a data imediatamente anterior que tem uma cotação para o ativo recebido por parâmetro.
			objRsData.ExecuteQuery(" select max(Data) as Data " + " from " + pstrTabela + " where Data < " + FuncoesBd.CampoDateFormatar(pdtmDataBase));

			DateTime functionReturnValue = Convert.ToDateTime(objRsData.Field("Data", Constantes.DataInvalida));

			objRsData.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a oscilação de todas as cotações
		/// </summary>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer os cálculos 
		/// apenas nas datas em que houver split. Esta opção é utilizada no recálculo quando
		/// há importação de proventos. 
		/// ATENÇÃO: quando este parâmetro for TRUE, é obrigatório passar o parâmetro
		/// "pdtmDataInicial" com uma data válida</param>
		/// <param name="pblnPercentualCalcular">Indica se é para calcular o percentual. 
		/// A diferença sempre é calculada</param>
		/// <param name="pdtmDataInicial">Data inicial para o início dos cálculos</param>
		/// <param name="pstrAtivos">Lista dos ativos para os quais deve ser feito
		/// os cálculos de oscilação separados por "#[código]#"
		/// </param>
		/// <returns>status da transação</returns>
		/// <remarks></remarks>
		private bool OscilacaoGeralCalcular(bool pblnPercentualCalcular, DateTime pdtmDataInicial, string pstrAtivos = "", bool pblnConsiderarApenasDataSplit = false)
		{

			if (pblnConsiderarApenasDataSplit && pdtmDataInicial == Constantes.DataInvalida) {
				//esta mensagem dificilmente será disparada. Só vai acontecer se houver erro de programação,
				//pois o desenvolvedor tem que saber que se quiser calcular apenas nas datas em que há split
				//tem que passar a data inicial como parâmetro. Este controle é apenas uma segurança para 
				//evitar erros na base sem que sejam percebidos.
                MessageBox.Show("Não é possivel calcular as oscilações apenas nas datas em que há splits " + "se não for recebida uma data inicial válida", "Trader Wizard",MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

			}

			cCommand objCommand = null;

			//não passa a conexão global para criar uma nova conexão
			cRS objRSAtivo = new cRS(objConexao);
			cRS objRSCotacao = null;

			cRSList objRSSplit = null;

			//Dim blnSplitExistir As Boolean

			Conexao objConnAux = new Conexao();


		    decimal decOscilacao = default(decimal);
		    decimal decCotacaoAnterior = default(decimal);

		    //contador do número de iterações por papel

		    DateTime dtmCalculoDataInicial = Constantes.DataInvalida;

			string strWhere = String.Empty;

			bool blnRetorno = true;

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

			//busca todos os ativos do período.
			//utiliza o group by que parece ser mais eficiente que o distinct

			string strQuery = " select Codigo " + " from Cotacao ";

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pdtmDataInicial != Constantes.DataInvalida) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";

				//se passou uma data inicial busca as cotações a partir de uma data.
				strWhere = strWhere + " Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";

				strWhere = strWhere + "INSTR(" + FuncoesBd.CampoStringFormatar(pstrAtivos) + ", '#' & Codigo & '#') > 0";

			}

			//'*************************************************
			//'UTILIZADO PARA DEBUG. COLOCAR O CÓDIGO DE UM PAPEL
			//If strWhere <> "" Then strWhere = strWhere & " and "

			//strWhere = strWhere _
			//& " Codigo = " & FuncoesBD.CampoStringFormatar("GGBR4")
			//'*************************************************


			if (!string.IsNullOrEmpty(strWhere)) {
				strQuery = strQuery + " where " + strWhere;

			}

			strQuery = strQuery + " group by Codigo ";

			objRSAtivo.ExecuteQuery(strQuery);

			//loop para percorrer todos os códigos de ativos

			while ((!objRSAtivo.EOF) && (blnRetorno)) {
				//utiliza uma variável para não precisar chamar a função field várias vezes
				var strCodigo = (string) objRSAtivo.Field("Codigo");


				if (pblnConsiderarApenasDataSplit) {
					//chama função que calcula a oscilação apenas nas datas em que há split
					blnRetorno = OscilacaoUnitCalcularApenasSplit(strCodigo, pdtmDataInicial);


				} else {
					if (objRSCotacao == null) {
						objRSCotacao = new cRS(objConnAux);
					}

					if (objCommand == null) {
						objCommand = new cCommand(objConnAux);
					}

					//inicializa o contador para cada ativo
					int intContador = 1;

					objCommand.BeginTrans();


					if (pdtmDataInicial != Constantes.DataInvalida) {
						dtmCalculoDataInicial = AtivoCotacaoAnteriorDataConsultar(strCodigo, pdtmDataInicial, "Cotacao", objConnAux);

					}

					//busca as cotações do ativo ordenado por data crescente
					//a partir da cotação anterior à data inicial recebida por parâmetro 
					//(se for recebida uma data válida)
					strQuery = " select Data, ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo);


					if (dtmCalculoDataInicial != Constantes.DataInvalida) {
						strQuery = strQuery + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmCalculoDataInicial);

					}

					strQuery = strQuery + " order by Data ";

					objRSCotacao.ExecuteQuery(strQuery);

					//verifica se tem splits. Tem que verificar se tem um dia após a primeira data. 
					//A primeira data não tem que ser considerada, pois a oscilação é calculada
					//apenas a partir da segunda data.
					objCarregadorSplit.SplitConsultar(strCodigo, Convert.ToDateTime(objRSCotacao.Field("Data")).AddDays(1), "A", ref objRSSplit,Constantes.DataInvalida);


					while ((!objRSCotacao.EOF) && (objCommand.TransStatus)) {
						decimal decCotacaoAtual = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento"));


						if (intContador > 1) {
							//só pode colocar a oscilação depois da segunda cotação pois precisa de duas para
							//fazer a relação de uma para outra.


							if (!objRSSplit.EOF) {
								//*****IMPORTANTE: Alterado de IF para WHILE porque podem ocorrer casos em que na mesma data haja um desdobramento e uma bonificação.
								//Neste caso o RS de splits vai retornar mais de um registro para a mesma data
								bool blnContinuarLoop = (Convert.ToDateTime(objRSCotacao.Field("Data")) == Convert.ToDateTime(objRSSplit.Field("Data")));


								while (blnContinuarLoop) {
									//se tem split na data, tem que multiplicar a cotação da data anterior pelo split
									decCotacaoAnterior = decCotacaoAnterior * Convert.ToDecimal(objRSSplit.Field("Razao"));

									//move para o próximo registro de split.
									objRSSplit.MoveNext();

									if (objRSSplit.EOF) {
										blnContinuarLoop = false;
									} else {

										if (Convert.ToDateTime(objRSCotacao.Field("Data")) != Convert.ToDateTime(objRSSplit.Field("Data"))) {
											blnContinuarLoop = false;

										}

									}

								}

							}


							if (pblnPercentualCalcular) {
								decOscilacao = decCotacaoAnterior > 0 ? Math.Round((decCotacaoAtual / decCotacaoAnterior - 1) * 100, 2) : 0;
							}


							//atualiza o registro com a oscilacao
							strQuery = " UPDATE Cotacao SET " + " Diferenca = " + FuncoesBd.CampoDecimalFormatar(decCotacaoAtual - decCotacaoAnterior);


							if (pblnPercentualCalcular) {
								strQuery = strQuery + ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao);

							}

							strQuery = strQuery + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo) + " AND Data = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRSCotacao.Field("Data")));

							objCommand.Execute(strQuery);

						}
						//if intContador > 1

						//cotação anterior recebe a cotação atual para ser usada na próxima iteração.
						decCotacaoAnterior = decCotacaoAtual;

						intContador = intContador + 1;

						objRSCotacao.MoveNext();

					}
					//fim do while que retorna as cotações de um ativo específico

					objCommand.CommitTrans();

					objRSCotacao.Fechar();


					if (!objCommand.TransStatus) {
						//se ocorrer erro na transação realizada para um dos ativos retorna FALSE
						blnRetorno = false;

					}

					//Alterado por mauro, 12/05/2010 - removido porque não faz sentido fechar a conexão dentro do loop;
					//objConnAux.FecharConexao()

					//objCommand = Nothing

				}
				//if pblnConsiderarApenasDataSplit

				objRSAtivo.MoveNext();

			}
			//fim do while do RS que contém todos os arquivos.

		    objConnAux.FecharConexao();
		    objRSAtivo.Fechar();

			return blnRetorno;

		}

		/// <summary>
		/// Calcula a oscilação apenas nas datas em que há split. Esta é uma função unitária,
		/// ou seja, calcula a oscilação para apenas um ativo.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo para o qual será feito o cálculo.</param>
		/// <param name="pdtmDataInicial">Data inicial dos cálculos</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool OscilacaoUnitCalcularApenasSplit(string pstrCodigo, DateTime pdtmDataInicial)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRSCotacao = new cRS(objConnAux);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

		    cRSList objRssListSplit = null;

		    double dblSplitAcumulado = 1;

			objCommand.BeginTrans();

			objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial.AddDays(1), "A", ref objRssListSplit, Constantes.DataInvalida);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			while (!objRssListSplit.EOF) {
				//fica multiplicando o split. pode haver casos em que tenho mais de um registro para a mesma data
				dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRssListSplit.Field("Razao"));

				DateTime dtmDataAtual = Convert.ToDateTime(objRssListSplit.Field("Data"));


				if (dtmDataAtual != Convert.ToDateTime(objRssListSplit.NextField("Data", Constantes.DataInvalida))) {
					//Quando o próximo registro tiver data diferente tem que atualizar a oscilação e a diferença
					// da data atual, pois na próxima iteração os cálculos já serão referentes à outra data.

					DateTime dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataAtual, "Cotacao", objConnAux);

					//busca a cotação anterior do ativo.
					string strQuery = " select ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData);

					objRSCotacao.ExecuteQuery(strQuery);

					//multiplica a cotação anterior ao split
					decimal decCotacaoAnterior = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento")) * (decimal) dblSplitAcumulado;

					//fecha o RS para ser utilizado novamente.
					objRSCotacao.Fechar();

					//consulta a cotação atual. Esta não deve ser multiplicada pelo split
					strQuery = " select ValorFechamento " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmDataAtual);

					objRSCotacao.ExecuteQuery(strQuery);

					decimal decCotacaoAtual = Convert.ToDecimal(objRSCotacao.Field("ValorFechamento"));

					objRSCotacao.Fechar();

				    decimal decOscilacao = decCotacaoAnterior > 0 ? Math.Round((decCotacaoAtual / decCotacaoAnterior - 1) * 100, 2) : 0;

					//atualiza o registro com a oscilacao
					strQuery = " UPDATE Cotacao SET " + Environment.NewLine + " Diferenca = " + FuncoesBd.CampoDecimalFormatar(decCotacaoAtual - decCotacaoAnterior) + Environment.NewLine + ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " AND Data = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRssListSplit.Field("Data")));

					objCommand.Execute(strQuery);

					//Inicializa a variável com 1 novamente, pois a próxima iteração irá se referir a uma data diferente
					dblSplitAcumulado = 1;

				}
				//If dtmDataAtual <> CDate(objRSSListSplit.NextField("Data", DataInvalida)) Then

				objRssListSplit.MoveNext();

			}

			objCommand.CommitTrans();

		    objConnAux.FecharConexao();
            return objCommand.TransStatus;

		}

		/// <summary>
		/// Verifica se o número de períodos em um determinado intervalo de datas em que há cotações é o número de períodos que se deseja
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataInicial">data inicial do intervalo</param>
		/// <param name="pdtmDataFinal">data final do intervalo</param>
		/// <param name="pintNumPeriodos">número de períodos do intervalo que devem ter cotações</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <returns>
		/// TRUE - O número de cotações é igual ao número esperado
		/// FALSE - O número de cotações é diferente do número esperado.
		/// </returns>
		/// <remarks></remarks>
		private bool IntervaloNumPeriodosVerificar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, int pintNumPeriodos, string pstrTabela, int pintNumPeriodosTabelaDados = -1, Conexao pobjConexao = null)
		{
		    cRS objRs = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//calcula o número de períodos em que há cotações para o papel no intervalo de datas recebido.
			string strQuery = " Select count(1) as Contador " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);

			if (pintNumPeriodosTabelaDados != -1) {
				strQuery = strQuery + " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();
			}

			objRs.ExecuteQuery(strQuery);

			bool functionReturnValue = Convert.ToInt32(objRs.Field("Contador", "0")) == pintNumPeriodos;

			objRs.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Para um determinado ativo retorna a data inicial e a data final em que existem os primeiros
		/// "pintNumPeriodos" com cotação.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pintNumPeriodos">Número de períodos desejado</param>
		/// <param name="pdtmDataInicialRet">Retorna a data inicial do número de períodos desejado</param>
		/// <param name="pdtmDataFinalRet">Retorna a data final do número de períodos desejado</param>
		/// <returns>    
		/// TRUE - Foi possível encontrar o número de períodos desejados
		/// FALSE - Não foi possível encontrar o número de períodos desejados 
		/// <param name="pblnPrimeiraDataConsiderar">Indica se deve ser considerada a primeira cotação. 
		/// Em cálculos em que é necessário utilizar a Oscilação ou a Diferença só pode ser utilizado a partir 
		/// do segundo período, pois é só a partir do segundo período que esta informaçãoexiste.
		/// TRUE - considera a primeira data
		/// FALSE - não considera a primeira data.
		/// </param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <param name="pintNumPeriodoTabelaDados"></param>
		/// Quando pstrTabelaDados for uma tabela de IFR, indica qual o período do IFR que deve ser utilizado
		/// para calcular a média.
		/// </returns>
		/// <remarks></remarks>
		private bool NumPeriodosDataInicialCalcular(string pstrCodigo, int pintNumPeriodos, bool pblnPrimeiraDataConsiderar, ref DateTime pdtmDataInicialRet, ref DateTime pdtmDataFinalRet, string pstrTabela, int pintNumPeriodosTabelaDados = -1, Conexao pobjConexao = null)
		{
		    string strQuery;

			cRS objRS = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			string strCodigoFormatado = FuncoesBd.CampoStringFormatar(pstrCodigo);

			int intNumPeriodosFinal;

		    string subQuery;

			if (pblnPrimeiraDataConsiderar) {
				//se é para considerar a primeira cotação, para calcular a data inicial 
				//basta pegar um min(Data) das cotações do papel

				strQuery = " select min(Data) as DataInicial " + " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;


				if (pintNumPeriodosTabelaDados != -1) {
					strQuery = strQuery + " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();

				}

				objRS.ExecuteQuery(strQuery);

				pdtmDataInicialRet = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

				//Se é para considerar a primeira data a última cotação que deve ser buscada 
				//é a cotação do número de periodos
				intNumPeriodosFinal = pintNumPeriodos;


			} else {
				//se não é para considerar a primeira data, pega a segunda data.

                subQuery = " select top 2 Data " + " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;

				if (pintNumPeriodosTabelaDados != -1) {
					subQuery += " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();

				}

				subQuery += " order by Data " ;

                strQuery = " select max(Data) as DataInicial " + " from " + FuncoesBd.FormataSubSelect(subQuery) ;

				objRS.ExecuteQuery(strQuery);

				pdtmDataInicialRet = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

				//Se não é para considerar a primeira data a última cotação que deve ser buscada 
				//é a cotação do número de periodos + 1
				intNumPeriodosFinal = pintNumPeriodos + 1;

			}

			objRS.Fechar();

			if (pdtmDataInicialRet == Constantes.DataInvalida) {
				//se não encontrou data inicial significa que não existem cotações para o papel
				return false;

			}

			//para obter a data final da cotação que contém o número do periodos desejados tem que fazer os seguintes passos:
			//1) Ordenar as cotações em ordem crescente
			//2) Pegar as "pintNumPeriodos" primeiras
			//3) Pegar a data máxima entre "pintNumPeriodos" primeiras
			subQuery = " select top " + intNumPeriodosFinal + " Data " +
            " from " + pstrTabela + " where Codigo = " + strCodigoFormatado;


			if (pintNumPeriodosTabelaDados != -1) {
				subQuery += " and NumPeriodos = " + pintNumPeriodosTabelaDados.ToString();

			}

			subQuery += " order by Data ";

            strQuery = " select max(Data) as DataMaxima FROM " + FuncoesBd.FormataSubSelect(subQuery);

			objRS.ExecuteQuery(strQuery);

			pdtmDataFinalRet = Convert.ToDateTime(objRS.Field("DataMaxima", Constantes.DataInvalida));

			//retorna resultado da função que verifica se o intervalo realmente tem o número de períodos informado.
			bool functionReturnValue = IntervaloNumPeriodosVerificar(pstrCodigo, pdtmDataInicialRet, pdtmDataFinalRet, pintNumPeriodos, pstrTabela, pintNumPeriodosTabelaDados, objRS.Conexao);

			objRS.Fechar();
			return functionReturnValue;

		}

		public decimal IFRCalcular(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, int pintNumPeriodos, string pstrTabela, 
            out double pdblMediaAltaAnteriorRet, out double pdblMediaBaixaAnteriorRet, Conexao pobjConexao = null)
		{
		    cRS objRS = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

			cRSList objRSListSplit = null;

		    //contém a razão acumulada de todos os splits do período.
			double dblSplitAcumulado = 1;
			double dblDiferencaPositivaAcumulada = 0;
			double dblDiferencaNegativaAcumulada = 0;

		    double dblIFR;

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objRS.Conexao);

			//busca os splits do ativo no período, ordenado pelo último split.
			//Tem que começar a buscar um dia após 
			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial.AddDays(1), "D", ref objRSListSplit, pdtmDataFinal);


			if (blnSplitExistir) {
				DateTime dtmDataFinalSplit = pdtmDataFinal;

				//quando tem script todas as cotaçoes tem que ser convertidas de acordo com o split

			    DateTime dtmDataInicialSplit;
			    while (!objRSListSplit.EOF) {

					if (Convert.ToDateTime(objRSListSplit.Field("Data")) != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida))) {
						dtmDataInicialSplit = pstrTabela.ToUpper() == "COTACAO" ? Convert.ToDateTime(objRSListSplit.Field("Data")) : PrimeiraSemanaDataCalcular(Convert.ToDateTime(objRSListSplit.Field("Data")));

						//acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
						dblDiferencaPositivaAcumulada = dblDiferencaPositivaAcumulada + (double)  CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "P", pstrTabela) * dblSplitAcumulado;

						//acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
						dblDiferencaNegativaAcumulada = dblDiferencaNegativaAcumulada + (double) CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "N", pstrTabela) * dblSplitAcumulado;

						//a data final para o próximo período e um dia antes ao periodo anterior
						dtmDataFinalSplit = dtmDataInicialSplit.AddDays(-1);

					}

					//multiplica o split anterior pelo split da data para o próximo periodo
					dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("Razao"));

					objRSListSplit.MoveNext();

				}

				//após o loop de splits ainda tem que somar o acumulado entre a data inicial e o primeiro split.
				dtmDataInicialSplit = pdtmDataInicial;

				//acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
				dblDiferencaPositivaAcumulada = dblDiferencaPositivaAcumulada + (double) CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "P", pstrTabela) * dblSplitAcumulado;

				//acumula as diferenças positivas entre as datas do split multiplicando pelo split acumulado
				dblDiferencaNegativaAcumulada = dblDiferencaNegativaAcumulada + (double) CotacaoPeriodoDiferencaSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, "N", pstrTabela) * dblSplitAcumulado;

				//calcula a média das diferenças acumuladas
				pdblMediaAltaAnteriorRet = dblDiferencaPositivaAcumulada / pintNumPeriodos;
				pdblMediaBaixaAnteriorRet = dblDiferencaNegativaAcumulada / pintNumPeriodos;


			} else {
				//SE NÃO TEM SPLIT FAZ O CÁLCULO NORMAL.
                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

				//calcula a média do valor de fechamento nos ultimos pintNumPeriodos com cotação positiva
				string strQuery = " select ROUND(SUM(Diferenca)/" + pintNumPeriodos + ",6) as MediaPositiva " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + " and Diferenca > 0 ";

				objRS.ExecuteQuery(strQuery);

				pdblMediaAltaAnteriorRet = Convert.ToDouble(objRS.Field("MediaPositiva", 0));

				objRS.Fechar();

				//calcula a média do valor de fechamento nos ultimos pintNumPeriodos com cotação negativa.
				//ANTES DE SOMAR CONVERTE O VALOR PARA O VALOR ABSOLUTO, POIS A DIFERENÇA NESTES CASOS ESTÁ COM SINAL NEGATIVO
				strQuery = " select ROUND(SUM(ABS(Diferenca)) / " + pintNumPeriodos + ",6) as MediaNegativa " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + " and Diferenca < 0 ";

				objRS.ExecuteQuery(strQuery);

				pdblMediaBaixaAnteriorRet = Convert.ToDouble(objRS.Field("MediaNegativa", 0));

				objRS.Fechar();

			}

			//calcula o INDICE DE FORÇA RELATIVA
			//IFR = 100 - (100 / (1 + U / D))
			//U = média das cotações dos últimos N dias em que a cotação subiu
			//D = média das cotações dos últimos N dias em que a cotação desceu 


			if (pdblMediaBaixaAnteriorRet != 0) {
				dblIFR = 100 - (100 / (1 + pdblMediaAltaAnteriorRet / pdblMediaBaixaAnteriorRet));
			} else {
				dblIFR = 100;
			}

            return Convert.ToDecimal(dblIFR);

		}
		/// <summary>
		/// Calcula o IFR para um determinado número de períodos tendo a informação de médias do último
		/// período e uma diferença entre de cotação em relação ao último período.
		/// </summary>
		/// <param name="pintNumPeriodos">Número de períodos utilizados no cálculo</param>
		/// <param name="pdblMediaAltaAnterior">Média de altas dos últimos pintNumPeriodos</param>
		/// <param name="pdblMediaBaixaAnterior">Média de baixas dos últimos pintNumPeriodos</param>
		/// <param name="pdecDiferenca">diferença entre o período que será calculado o IFR e o período imediatamente anterior</param>
		/// <param name="pdblMediaAltaAtualRet">Retorna a média de alta já considerando o período para o qual o IFR está sendo calculado</param>
		/// <param name="pdblMediaBaixaAtualRet">Retorna a média de baixa já considerando o período para o qual o IFR está sendo calculado</param>
		/// <returns>O indice de força relativa calculado</returns>
		/// <remarks></remarks>
		private double IFRCalcular(int pintNumPeriodos, double pdblMediaAltaAnterior, double pdblMediaBaixaAnterior, decimal pdecDiferenca, ref double pdblMediaAltaAtualRet, ref double pdblMediaBaixaAtualRet)
		{

			double dblIFR;

			//Mup = ((Mup anterior)*(pd-1)+M) / pd;
			pdblMediaAltaAtualRet = (pdblMediaAltaAnterior * (pintNumPeriodos - 1) + Convert.ToDouble((pdecDiferenca > 0 ? pdecDiferenca : 0))) / pintNumPeriodos;

			//Mdown = ((Mdown anterior)*(pd-1)+M) / pd;
			pdblMediaBaixaAtualRet = (pdblMediaBaixaAnterior * (pintNumPeriodos - 1) + Convert.ToDouble((pdecDiferenca < 0 ? Math.Abs(pdecDiferenca) : 0))) / pintNumPeriodos;

			//calcula o INDICE DE FORÇA RELATIVA
			//IFR = 100 - (100 / (1 + U / D))
			//U = média das cotações dos últimos N dias em que a cotação subiu
			//D = média das cotações dos últimos N dias em que a cotação desceu 


			if (pdblMediaBaixaAtualRet != 0) {
				dblIFR = 100 - (100 / (1 + pdblMediaAltaAtualRet / pdblMediaBaixaAtualRet));
			} else {
				//se a média de baixa está zerada, ou seja, não tem cotação em baixa,
				//o IFR é 100.
				dblIFR = 100;
			}

			return dblIFR;

		}

		/// <summary>
		/// Calcula o IFR de 14 períodos para uma diferença recebida por parâmetro em relação 
		/// ao último IFR calculado
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pstrTabela"></param>
		/// <param name="pdecDiferenca"></param>
		/// <returns></returns>
		/// <remarks></remarks>

		public double IFRSimuladoCalcular(string pstrCodigo, string pstrTabela, decimal pdecDiferenca)
		{

			cRS objRS = new cRS(objConexao);

			cCalculadorData objCalculadorData = new cCalculadorData(this.objConexao);

			DateTime dtmDataMaxima = objCalculadorData.CotacaoDataMaximaConsultar(pstrCodigo, pstrTabela);

		    string strTabelaIFR = pstrTabela.ToUpper() == "COTACAO" ? "IFR_DIARIO" : "IFR_SEMANAL";

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			objRS.ExecuteQuery(" select MediaBaixa, MediaAlta " + " from " + strTabelaIFR + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmDataMaxima) + " and NumPeriodos = 14 ");

		    //dblIFR = IFRCalcular(14, CDbl(objRS.Field("IFR14MediaAlta")), CDbl(objRS.Field("IFR14MediaBaixa")), pdecDiferenca, 0, 0)

		    double pdblMediaBaixaAtualRet = 0.0;
		    double pdblMediaAltaAtualRet = 0.0;
		    double dblIFR = IFRCalcular(14, Convert.ToDouble(objRS.Field("MediaAlta")), Convert.ToDouble(objRS.Field("MediaBaixa")), pdecDiferenca, ref pdblMediaAltaAtualRet, ref pdblMediaBaixaAtualRet);

			objRS.Fechar();

			return dblIFR;

		}

	    /// <summary>
	    /// Calcula o IFR a partir de uma data base até a cotação mais existente
	    /// </summary>
	    /// <param name="pstrCodigo">Código do ativo</param>
	    /// <param name="pdtmDataBase">data inicial utilizada no cálculo</param>
	    /// <param name="pintNumPeriodos">número de períodos utilizado no cálculo do IFR</param>
	    /// <param name="pstrTabela">
	    /// Cotacao
	    /// Cotacao_Semanal
	    /// </param>
	    /// <param name="pdtmCotacaoAnteriorData"></param>
	    /// <returns>
	    /// RETORNOOK = OPERAÇÃO REALIZADA COM SUCESSO
	    /// RETORNOERROINESPERADO = ALGUM ERRO DE BANCO DE DADOS OU PROGRAMAÇÃO
	    /// RETORNOERRO2 = NÃO EXISTE UM NÚMERO SUFICIENTES DE PERÍODO PARA FAZER O CÁLCULO.
	    /// </returns>
	    /// <remarks></remarks>
	    public cEnum.enumRetorno IFRRetroativoUnitCalcular(string pstrCodigo, DateTime pdtmDataBase, int pintNumPeriodos, string pstrTabela, DateTime pdtmCotacaoAnteriorData)
		{
	        Conexao objConnAux = new Conexao();

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRS = new cRS(objConnAux);
			cRSList objRSSplit = null;

		    double dblIFR;
			double dblMediaAltaAnterior = 0;
			double dblMediaBaixaAnterior = 0;

			double dblMediaAlta = 0;
			double dblMediaBaixa = 0;

			DateTime dtmDataInicial = default(DateTime);
			DateTime dtmDataFinal = default(DateTime);

		    bool blnPeriodoCalcular = false;

			//tabela onde devem ser buscados / armazenados os dados do IFR
	        pstrTabela = pstrTabela.ToUpper();

		    string strTabelaIfr = pstrTabela == "COTACAO" ? "IFR_DIARIO" : "IFR_SEMANAL";

			//**********************inicia transação
			objCommand.BeginTrans();

			DateTime dtmCotacaoAnteriorData = pdtmCotacaoAnteriorData;

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (dtmCotacaoAnteriorData != Constantes.DataInvalida) {
				//se tem busca as médias do IFR na data
				objRS.ExecuteQuery(" select MediaAlta, MediaBaixa " + " from " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData) + " AND NumPeriodos = " + pintNumPeriodos.ToString());

				//verifica se os campos estão preenchidos e estão com valor maior do que zero.

				if (Convert.ToDouble(objRS.Field("MediaAlta", 0)) > 0 && Convert.ToDouble(objRS.Field("MediaBaixa", 0)) > 0) {
					dblMediaAltaAnterior = Convert.ToDouble(objRS.Field("MediaAlta"));
					dblMediaBaixaAnterior = Convert.ToDouble(objRS.Field("MediaBaixa"));
				} else {
					//se tem cotação mas não tem as médias calculadas, tem que calcular o período inicial.
					blnPeriodoCalcular = true;
				}

				objRS.Fechar();


			} else {
				//se não encontrou cotação tem que calcular o período inicial
				blnPeriodoCalcular = true;

			}


			if (blnPeriodoCalcular) {
				//verifica se existe o número de períodos necessários para fazer pelo menos um cálculo e retorna o
				//periodo para calcular o IFR inicial.
				//Para calcular o periodo inicial não pode considerar o primeiro dia, pois o primeiro dia não tem a diferença.

				if (NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, false, ref dtmDataInicial, ref dtmDataFinal, pstrTabela,-1 , objConnAux)) {
					//calcula o IFR inicial no período retornado pela função
					dblIFR = Convert.ToDouble( IFRCalcular(pstrCodigo, dtmDataInicial, dtmDataFinal, pintNumPeriodos, pstrTabela, out dblMediaAltaAnterior, out dblMediaBaixaAnterior, objConnAux));

					//tem que excluir os registros caso já existam
					objCommand.Execute(" DELETE " + " FROM " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataFinal) + " and NumPeriodos = " + pintNumPeriodos.ToString());

					//atualiza o IFR na tabela 
					IFRAtualizar(pstrCodigo, dtmDataFinal, pintNumPeriodos, strTabelaIfr, dblIFR, dblMediaAltaAnterior, dblMediaBaixaAnterior, objConnAux);

					//neste caso a data base tem que ser um dia posterior à data final da data calculada
					//pdtmDataBase = DateAdd(DateInterval.Day, 1, dtmDataFinal)
					var objCalculadorData = new cCalculadorData(objConnAux);
					pdtmDataBase = objCalculadorData.CalcularDataProximoPeriodo(pstrCodigo, dtmDataFinal, pstrTabela);


				} else {

					//se não encontrou um intervalo de datas que também tenha o mesmo número de periodos 
					//sai da função retornando o erro.
					//Antes de sair tem que fazer rollback para não deixar a transação aberta.
					objCommand.RollBackTrans();

					return cEnum.enumRetorno.RetornoErro2;
				}

			}

			if (pdtmDataBase != Constantes.DataInvalida) {

				if (!blnPeriodoCalcular) {
					//tem que excluir os registros caso já existam e não tenha sido necessário calcular o período.
					//se o período foi calculado, os registros já foram excluidos
					objCommand.Execute(" DELETE " + " FROM " + strTabelaIfr + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " and NumPeriodos = " + pintNumPeriodos.ToString());

				}

                //busca todos os splits a partir da data base em ordem ascendente
                objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataBase, "A", ref objRSSplit, Constantes.DataInvalida);

				//busca todas as cotações a partir da data base. Busca a diferença, pois o IFR é calculado em cima deste valor.

				string strQuery = " select Data, Diferenca";

				if (pstrTabela == "COTACAO_SEMANAL") {
					//SE É COTAÇÃO SEMANAL TEM QUE BUSCAR A DATA FINAL PARA USAR NO CÁLCULO DOS SPLITS
					strQuery = strQuery + ", DataFinal";

				}

				strQuery = strQuery + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " order by Data ";

				objRS.ExecuteQuery(strQuery);

			    var cotacoes = new List<CotacaoDiferencaDto>();

			    while (!objRS.EOF)
			    {
			        var cotacaoDiferencaDto = new CotacaoDiferencaDto
			        {
			            DataInicial = Convert.ToDateTime(objRS.Field("Data")),
			            Diferenca = Convert.ToDecimal(objRS.Field("Diferenca"))
			        };
			        if (pstrTabela == "COTACAO_SEMANAL")
			        {
			            cotacaoDiferencaDto.DataFinal = Convert.ToDateTime(objRS.Field("DataFinal")) ;
			        }
                    cotacoes.Add(cotacaoDiferencaDto);

			        objRS.MoveNext();
			    }
                objRS.Fechar();
				//loop para calcular a média exponencial em todas as datas subsequentes

				foreach (var cotacaoDiferencaDto in cotacoes) {

					if (!objRSSplit.EOF) {
						bool blnContinuarLoop;


						if (pstrTabela == "COTACAO") {
							//********TRATAMENTO PARA A COTAÇÃO DIÁRIA

							blnContinuarLoop = (cotacaoDiferencaDto.DataInicial == Convert.ToDateTime(objRSSplit.Field("Data")));


							while (blnContinuarLoop) {
								//se a data do split é a mesma data do cálculo da média

								//multiplica a média anterior pelo split
								dblMediaAltaAnterior = dblMediaAltaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));
								dblMediaBaixaAnterior = dblMediaBaixaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));

								//passa para o próximo registro
								objRSSplit.MoveNext();

								if (objRSSplit.EOF) {
									blnContinuarLoop = false;
								} else {

									if (cotacaoDiferencaDto.DataInicial != Convert.ToDateTime(objRSSplit.Field("Data"))) {
										blnContinuarLoop = false;

									}

								}

							}

						//End If



						} else {
							//para aplicar o split na cotação semanal, a data do split tem que estar entre 
							//o primeiro e ó último dia da semana.

							blnContinuarLoop = (Convert.ToDateTime(objRSSplit.Field("Data")) >= cotacaoDiferencaDto.DataInicial && Convert.ToDateTime(objRSSplit.Field("Data")) <= cotacaoDiferencaDto.DataFinal);


							while (blnContinuarLoop) {
								//multiplica a média anterior pelo split
								dblMediaAltaAnterior = dblMediaAltaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));
								dblMediaBaixaAnterior = dblMediaBaixaAnterior * Convert.ToDouble(objRSSplit.Field("Razao"));

								//passa para o próximo registro
								objRSSplit.MoveNext();

								if (objRSSplit.EOF) {
									blnContinuarLoop = false;

								} else {
									blnContinuarLoop = (Convert.ToDateTime(objRSSplit.Field("Data")) >= cotacaoDiferencaDto.DataInicial && Convert.ToDateTime(objRSSplit.Field("Data")) <= cotacaoDiferencaDto.DataFinal);

								}

							}

						}

					}

					//calcula o IFR
					dblIFR = IFRCalcular(pintNumPeriodos, dblMediaAltaAnterior, dblMediaBaixaAnterior, cotacaoDiferencaDto.Diferenca, ref dblMediaAlta, ref dblMediaBaixa);

					//atualiza na tabela o IFR e as médias dos períodos de alta e baixa
					IFRAtualizar(pstrCodigo, cotacaoDiferencaDto.DataInicial, pintNumPeriodos, strTabelaIfr, dblIFR, dblMediaAlta, dblMediaBaixa, objConnAux);

					//atribui a média calculada como média anterior para a próxima iteração;
					dblMediaAltaAnterior = dblMediaAlta;
					dblMediaBaixaAnterior = dblMediaBaixa;

				}


			}
			// pdtmDataBase <> DataInvalida Then

			//******************ENCERRA TRANSAÇÃO
			objCommand.CommitTrans();

			//retorna de acordo com o status da transação.
			cEnum.enumRetorno functionReturnValue = objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		    objConnAux.FecharConexao();

			return functionReturnValue;

		}

	    /// <summary>
	    /// Calcula o IFR para todos os ativos a partir da data inicial
	    /// </summary>
	    /// <param name="pcolPeriodos">Collection contendo o número de períodos para os quais deve ser feito o cálculo</param>
	    /// <param name="pPeriodicidade"></param>
	    /// <param name="pdtmDataInicial"></param>
	    /// <param name="pstrAtivos"></param>
	    /// <returns>
	    /// TRUE - Cálculo correto para todos os ativos
	    /// FALSE - Cálculo errado para pelo menos um dos ativos
	    /// </returns>
	    /// <remarks></remarks>
	    public bool IFRGeralCalcular(IList<int> pcolPeriodos, cEnum.Periodicidade pPeriodicidade, DateTime pdtmDataInicial, string pstrAtivos = "")
		{
			bool functionReturnValue = false;

			//**********PARA BUSCAR OS ATIVOS NÃO PODE USAR A MESMA CONEXÃO DA TRANSAÇÃO,
			//**********POIS SE A TRANSAÇÃO FIZER ROLLBACK PARA UM ATIVO O RECORDSET NÃO IRÁ FUNCIONAR MAIS.
			cRS objRSAtivo = new cRS();

			//Dim objRSCotacao As cRS = New cRS(objConexao)

		    bool blnRetorno = true;

		    string strWhere = String.Empty;

			string strLog = "";

			//indica o nome da tabela de cotações, de acordo com a duração do período das cotações
			string strTabela = null;

			switch (pPeriodicidade)
			{
			    case cEnum.Periodicidade.Diario:
			        strTabela = "Cotacao";
			        break;
			    case cEnum.Periodicidade.Semanal:
			        strTabela = "Cotacao_Semanal";
			        break;
			}

			//busca todos os ativos do período e a menor data para ser utilizada como data base.
			string strQuery = " select Codigo, min(Data) as DataInicial " + " from " + strTabela;

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pdtmDataInicial != Constantes.DataInvalida) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";


				if (pPeriodicidade == cEnum.Periodicidade.Diario) {
					//se passou uma data inicial busca as cotações a partir de uma data.
					strWhere = strWhere + " Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);


				} else if (pPeriodicidade == cEnum.Periodicidade.Semanal) {
					//se é uma cotação semanal, a data recebida por parâmetro tem que estar entre 
					//a data inicial e a data final da semana,
					//ou então tem que estar na próxima semana, caso a data informada seja uma data de final de 
					//semana ou feriado que esteja ligado com final de semana 
					strWhere = strWhere + " ((Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + ")" + " or Data > " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + ")";

				}

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " And ";

				strWhere = strWhere + "INSTR(" + FuncoesBd.CampoStringFormatar(pstrAtivos) + ", '#' & Codigo & '#') > 0";

			}


			//'*************************************************
			//'UTILIZADO PARA DEBUG. COLOCAR O CÓDIGO DE UM PAPEL
			//If strWhere <> "" Then strWhere = strWhere & " and "

			//strWhere = strWhere _
			//& " Codigo = " & FuncoesBD.CampoStringFormatar("GGBR4")
			//'*************************************************


			if (!string.IsNullOrEmpty(strWhere)) {
				strQuery = strQuery + " where " + strWhere;

			}

			strQuery = strQuery + " group by Codigo ";

			objRSAtivo.ExecuteQuery(strQuery);

		    var ativos = new List<CotacaoDataDto>();

		    while ((!objRSAtivo.EOF))
		    {
                ativos.Add(new CotacaoDataDto { 
                    Codigo = (string)objRSAtivo.Field("Codigo"), 
                    Data = Convert.ToDateTime(objRSAtivo.Field("DataInicial")) 
                });
                objRSAtivo.MoveNext();
		    }

            objRSAtivo.Fechar();

		    foreach (var cotacaoDataDto in ativos) {

				DateTime dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(cotacaoDataDto.Codigo, cotacaoDataDto.Data, strTabela);

				foreach (int intNumPeriodos in pcolPeriodos) {
					cEnum.enumRetorno intRetorno = IFRRetroativoUnitCalcular(cotacaoDataDto.Codigo, cotacaoDataDto.Data, intNumPeriodos, strTabela, dtmCotacaoAnteriorData);

					if (intRetorno != cEnum.enumRetorno.RetornoOK) {
						blnRetorno = false;

						if (!string.IsNullOrEmpty(strLog)) {
							//coloca um ENTER PARA QUEBRAR A LINHA
							strLog = strLog + Environment.NewLine;
						}
						strLog = strLog + " Código = " + cotacaoDataDto.Codigo + " - Período: " + intNumPeriodos + " - Data Inicial: " + cotacaoDataDto.Data;
					}
				}
			}


			if (!string.IsNullOrEmpty(strLog)) {
				string strArquivoNome = null;

				if (pPeriodicidade == cEnum.Periodicidade.Diario) {
					strArquivoNome = "Log_IFR_Diario.txt";
				} else if (pPeriodicidade == cEnum.Periodicidade.Semanal) {
					strArquivoNome = "Log_IFR_Semanal.txt";
				}

			    var fileService = new FileService();
                fileService.Save(cBuscarConfiguracao.ObtemCaminhoPadrao() + strArquivoNome, strLog);

			}


			if (objRSAtivo.QueryStatus) {
				functionReturnValue = blnRetorno;
			}

			return functionReturnValue;

		}



		/// <summary>
		/// Soma o campo de uma determinada tabela no período indicado.
		/// </summary>
        /// <param name="pstrCodigo"></param>
        /// <param name="pdtmDataInicial"></param>
        /// <param name="pdtmDataFinal"></param>
        /// <param name="pstrTabela">Cotacao ou Cotacao_Semanal</param>
        /// <param name="pstrCampo">Volume ou ValorFechamento ou outro campo que for necessário</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private decimal CotacaoPeriodoCampoSomar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrTabela, string pstrCampo)
		{
		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			objRS.ExecuteQuery(" select sum(" + pstrCampo + ") as total " + " from " + pstrTabela + " where codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal));

			decimal functionReturnValue = Convert.ToDecimal(objRS.Field("total"));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Soma o valor absoluto das diferenças de cotações em um período.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <param name="pstrSinal">
		/// Indica se deve considerar somente cotações positivas ou somente cotações negativas
		/// P - POSITIVA
		/// N - NEGATIVA
		/// </param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <returns>Soma da diferença das cotações do período, conforme o sinal recebido por parâmetro</returns>
		/// <remarks></remarks>
		private decimal CotacaoPeriodoDiferencaSomar(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrSinal, string pstrTabela)
		{
		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    //TEM QUE SER O VALOR ABSOLUTO, POIS NO CÁLCULO OS VALORES TEM QUE SER SEMPRE POSITIVOS.
			string strQuery = " select sum(ABS(Diferenca)) as Total " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);


			if (pstrSinal == "P") {
				strQuery = strQuery + " and Diferenca > 0 ";


			} else if (pstrSinal == "N") {
				strQuery = strQuery + " and Diferenca < 0 ";

			}

			objRS.ExecuteQuery(strQuery);

			decimal functionReturnValue = Convert.ToDecimal(objRS.Field("Total"));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a média móvel aritmética para um determinado número de períodos
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo que será calcula a média móvel simples</param>
		/// <param name="pdtmDataInicial"> Data inicial do cálculo da média</param>
		/// <param name="pdtmDataFinal">Data final do cálculo da média. Quando o período for semanal, 
		/// esta data equivale ao primeiro dia da última semana.</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// <param name="pstrDado">"VALOR" ou "VOLUME"</param>
		/// <param name="pintNumPeriodos">número de períodos utilizado no cálculo da média</param>
		/// <param name="pobjConexao">objeto de conexão com o banco de dados</param>
		/// <returns>
		/// Retorna a média calculada.
		/// </returns>
		/// <remarks></remarks>
		private double MMAritmeticaCalcular(string pstrCodigo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, int pintNumPeriodos, string pstrTabela, string pstrDado, Conexao pobjConexao = null)
		{
			double functionReturnValue;

		    cRS objRS = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

			cRSList objRSListSplit = null;

			bool blnSplitExistir = false;

		    //contém a razão acumulada de todos os splits do período.
			double dblSplitAcumulado = 1;
			double dblDadoAcumulado = 0;

			string strCampo;

			int intIFRNumPeriodos = -1;

			string strSplitTipo = String.Empty;

			if (pstrDado == "VALOR") {
				strCampo = "ValorFechamento";

			} else if (pstrDado == "VOLUME") {
				strCampo = "Titulos_Total";
				//Quando é média de volume considera apenas os desdobramentos.
				strSplitTipo = "DESD";

			} else {
				//IFR
				strCampo = "Valor";

				//quando é IFR o dado está sempre no formato IFRxx, onde xx é o IFR de xx períodos, para
				//o qual deve ser calculada a média.
				//com isso, para saber o número de períodos busca a substring a partir do 4º caracter,
				//pois os três primeiros são: "IFR"
                intIFRNumPeriodos = Convert.ToInt16(pstrDado.Substring(3));

			}


			if (intIFRNumPeriodos == -1) {
			    DateTime dtmDataFinalBuscaSplit;
			    if (pstrTabela.ToUpper() == "COTACAO_SEMANAL") {
					//quando o período é semanal busca a data da última cotação da semana 
					//para utilizar como data máxima de busca do splitporque o mesmo pode ocorrer
					//em qualquer dia da semana.
					dtmDataFinalBuscaSplit = AtivoCotacaoSemanalUltimoDiaSemanaCalcular(pstrCodigo, pdtmDataFinal);
				} else {
					dtmDataFinalBuscaSplit = pdtmDataFinal;
				}

				cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objRS.Conexao);

				//busca os splits do ativo no período, ordenado pelo último split.
				//Tem que começar a buscar um dia após 
				blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataInicial.AddDays(1), "D", ref objRSListSplit, dtmDataFinalBuscaSplit, strSplitTipo);

			}


		    if (blnSplitExistir) {
				DateTime dtmDataFinalSplit = pdtmDataFinal;

				//quando tem split todas as cotaçoes tem que ser convertidas de acordo com o split

		        DateTime dtmDataInicialSplit;
		        while (!objRSListSplit.EOF) {

					if (Convert.ToDateTime(objRSListSplit.Field("Data")) != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida))) {
						if (pstrTabela.ToUpper() == "COTACAO") {
							//obtém a data do split.
							dtmDataInicialSplit = Convert.ToDateTime(objRSListSplit.Field("Data"));
						} else {
							//PARA AS COTAÇÕES SEMANAIS TEM QUE TRANSFORMAR NO PRIMEIRO DIA DA SEMANA
							dtmDataInicialSplit = PrimeiraSemanaDataCalcular(Convert.ToDateTime(objRSListSplit.Field("Data")));
						}

						dblDadoAcumulado = dblDadoAcumulado + (double) CotacaoPeriodoCampoSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, pstrTabela, strCampo) * dblSplitAcumulado;

						//a data final para o próximo período e um dia antes ao periodo anterior
						dtmDataFinalSplit = dtmDataInicialSplit.AddDays(-1);

					}


					if (pstrDado == "VALOR") {
						//acumula as cotações entre as datas do split multiplicando pelo split acumulado
						//multiplica o split anterior pelo split da data para o próximo periodo
						dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("Razao"));



					} else {
						//TRATA O VOLUME
						dblSplitAcumulado = dblSplitAcumulado * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));

					}


					objRSListSplit.MoveNext();

				}

				//após o loop de splits ainda tem que somar o acumulado entre a data inicial e o primeiro split.
				dtmDataInicialSplit = pdtmDataInicial;

				dblDadoAcumulado = dblDadoAcumulado + (double) CotacaoPeriodoCampoSomar(pstrCodigo, dtmDataInicialSplit, dtmDataFinalSplit, pstrTabela, strCampo) * dblSplitAcumulado;

				//divide a cotação acumulada pelo número de periodos
				functionReturnValue = dblDadoAcumulado / pintNumPeriodos;


			} else {

                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

				//se não tem split calcula a média no próprio banco.
				string strQuery = " select avg(" + FuncoesBd.ConverterParaPontoFlutuante(strCampo)  + ") as Media " + 
                    " from " + pstrTabela + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + 
                    " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + 
                    " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);

				if (intIFRNumPeriodos != -1) {
					//se está calculando média do IFR, coloca no where o número de periodos
					//do IFR que está sendo calculado a média.
					strQuery = strQuery + " AND NumPeriodos = " + intIFRNumPeriodos;

				}

				objRS.ExecuteQuery(strQuery);

				functionReturnValue = Convert.ToDouble(objRS.Field("Media", "0"));

			}

			objRS.Fechar();

			return functionReturnValue;

		}

	    private double MMExponencialCalcular(decimal pdecValorFechamento, int pintPeriodo, decimal pdecMMExpAnterior)
	    {
	        //MMEx = ME(x-1) + K x {Fech(x) – ME(x-1)} 

			//* MMEx representa a média móvel exponencial no dia x
			//* ME(x-1) representa a média móvel exponencial no dia x-1
			//* N é o número de dias para os quais se quer o cálculo
			//* Constante K = {2 ÷ (N+1)} 

			// calcula a constante K
	        decimal decConstanteK = new decimal(2) / (pintPeriodo + 1);

	        return Convert.ToDouble(pdecMMExpAnterior + decConstanteK * (pdecValorFechamento - pdecMMExpAnterior));
	    }


	    /// <summary>
	    /// Atualiza a média no banco de dados na tabela de média diária e média semanal
	    /// </summary>
	    /// <param name="pstrCodigo">Código do ativo</param>
	    /// <param name="pdtmData">data da média</param>
	    /// <param name="pintNumPeriodos"></param>
	    /// <param name="pstrTabela">
	    ///     Cotacao
	    ///     Cotacao_Semanal
	    /// </param>
	    /// <param name="pdblMedia">Valor da média que será atualizada
	    /// </param>
	    /// <param name="pstrMediaTipo"></param>
	    /// <param name="pobjConexao"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private void MMAtualizar(string pstrCodigo, DateTime pdtmData, int pintNumPeriodos, string pstrTabela, double pdblMedia, string pstrMediaTipo, Conexao pobjConexao = null)
		{
		    cCommand objCommand = pobjConexao == null ? new cCommand(objConexao) : new cCommand(pobjConexao);

		    pstrTabela = pstrTabela.ToUpper();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			string strQuery = " INSERT INTO " + pstrTabela + "(Codigo, Data, NumPeriodos, Tipo, Valor)" + " VALUES " + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + FuncoesBd.CampoDateFormatar(pdtmData) + ", " + pintNumPeriodos.ToString() + ", " + FuncoesBd.CampoStringFormatar(pstrMediaTipo) + ", " + FuncoesBd.CampoFloatFormatar(pdblMedia) + ")";

			objCommand.Execute(strQuery);
		}

	    /// <summary>
	    /// Atualiza o valor do IFR no banco de dados
	    /// </summary>
	    /// <param name="pstrCodigo">Código do papel</param>
	    /// <param name="pdtmData"></param>
	    /// <param name="pintPeriodo"></param>
	    /// <param name="pstrTabela"></param>
	    /// <param name="pdblIFR"></param>
	    /// <param name="pdblMediaAlta"></param>
	    /// <param name="pdblMediaBaixa"></param>
	    /// <param name="pobjConexao"></param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private void IFRAtualizar(string pstrCodigo, DateTime pdtmData, int pintPeriodo, string pstrTabela, double pdblIFR, double pdblMediaAlta, double pdblMediaBaixa, Conexao pobjConexao = null)
		{
	        cCommand objCommand = pobjConexao == null ? new cCommand(objConexao) : new cCommand(pobjConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

	        //******ALTERADO POR MAURO, 19/12/2009
			//******ARMAZENAMENTO DO IFR EM TABELA PRÓPRIA PARA O IFR DIÁRIO E PARA O IFR SEMANAL
			string strQuery = " INSERT INTO " + pstrTabela + "(Codigo, Data, NumPeriodos, MediaBaixa, MediaAlta, Valor) " + " VALUES " + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + FuncoesBd.CampoDateFormatar(pdtmData) + ", " + pintPeriodo.ToString() + ", " + FuncoesBd.CampoDecimalFormatar((decimal?) pdblMediaBaixa) + ", " + FuncoesBd.CampoDecimalFormatar((decimal?) pdblMediaAlta) + ", " + FuncoesBd.CampoDecimalFormatar((decimal?) pdblIFR) + ")";

			objCommand.Execute(strQuery);
		}


		/// <summary>
		/// Calcula a média móvel exponencial para um determinado ativo desde a data inicial até a data da cotação 
		/// mais recente encontrada
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">data inicial para cálculo </param>
		/// <param name="pintNumPeriodos">número de dias utilizado no cálculo da média</param>
		/// <param name="pstrTabela">
		/// Cotacao
		/// Cotacao_Semanal
		/// </param>
		/// DATABASE = SALVA OS VALORES CALCULADOS NO DATABASE
		/// MEMORIA = SALVA OS VALORES CALCULADOS EM UMA ESTRUTURA DE MEMÓRIA
		/// </param>
		/// <param name="pdtmCotacaoAnteriorData">Data da cotação anterior. Só é passado um valor válido para este parâmetro
		///  quando o parâmetro pdtmDataBase não é uma DATAINVALIDA. Este parâmetro é utilizado para otimizar o código,
		/// já que a função CotacaoAnteriorDataConsultar pode ser utilizada várias vezes se a função de cálculo de média 
		/// for chamada várias vezes.</param>
		/// <returns>
		/// RetornoOK = operação realizada com sucesso
		/// RetornoErroInesperado = algum erro de banco de dados ou de programação.
		/// RetornoErro2 = erro na execução da operação em um ou mais dias
		/// RetornoErro3 = Não existe o número de cotações suficientes para fazer o cálculo.
		/// </returns>
		/// <remarks></remarks>
		public cEnum.enumRetorno MediaMovelMExponencialRetroativaUnitCalcular(string pstrCodigo, DateTime pdtmDataBase, int pintNumPeriodos, string pstrTabela
            , DateTime pdtmCotacaoAnteriorData)
		{
			Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRS = new cRS(objConnAux);
			cRSList objRsSplit = null;

		    pstrTabela = pstrTabela.ToUpper();

		    string strTabelaMedia = pstrTabela == "COTACAO" ? "Media_Diaria" : "Media_Semanal";

			double dblMmExpAnterior = 0;
		    DateTime dtmDataInicial = default(DateTime);
			DateTime dtmDataFinal = default(DateTime);
			//Dim dtmDataFinalSplit As Date = DataInvalida

		    bool blnPeriodoCalcular = false;

		    //número de registros para os quais a média será calculada
			int intArrayIndice = 0;
			//controla o array do indice

			//**********************inicia transação
			objCommand.BeginTrans();

			//If pintNumPeriodos = 21 Then

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pdtmDataBase != Constantes.DataInvalida)
			{
			    //verifica se existe uma média móvel em uma data anterior.
				//para isso busca a cotação anterior a esta

				//--04/01/2010
				//alterado para receber a data da cotação anterior por parâmetro.
				//dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataBase, pstrTabela, objConnAux)

			    DateTime dtmCotacaoAnteriorData = pdtmCotacaoAnteriorData;
			    //--fim da alteração do dia 04/01/2010


				if (dtmCotacaoAnteriorData != Constantes.DataInvalida) {
					//se tem busca a média móvel de 21 dias na data

					objRS.ExecuteQuery(" select Valor " + " from " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData) + " and NumPeriodos = " + pintNumPeriodos.ToString() + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME"));

					//verifica se o campo está preenchido e está com valor maior que zero.
					//If CDec(objRS.Field("MMExp21", 0)) > 0 Then
					if (Convert.ToDecimal(objRS.Field("Valor", 0)) > 0) {
						dblMmExpAnterior = Convert.ToDouble(objRS.Field("Valor"));
					} else {
						//se tem cotação mas não tem média calculada, tem que calcular o período inicial.
						blnPeriodoCalcular = true;
					}

					objRS.Fechar();


				} else {
					//se não encontrou cotação tem que calcular o período inicial
					blnPeriodoCalcular = true;

				}
			}
			else {
				//se não recebeu uma data válida para utilizar como data base, então tem que calcular o período
				blnPeriodoCalcular = true;

			}


			if (blnPeriodoCalcular) {
				//verifica se existe o número de períodos necessários para fazer pelo menos um cálculo e retorna o
				//periodo para calcular a primeira média, que é a média simples.

				if (NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true, ref dtmDataInicial, ref dtmDataFinal, pstrTabela,-1 , objConnAux)) {
					//calcula a média simples no periodo, que será usado como primeira média
					dblMmExpAnterior = MMAritmeticaCalcular(pstrCodigo, dtmDataInicial, dtmDataFinal, pintNumPeriodos, pstrTabela, "VALOR", objConnAux);

					objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataFinal) + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME") + " and NumPeriodos = " + pintNumPeriodos.ToString());

					//atualiza a média na tabela 
					MMAtualizar(pstrCodigo, dtmDataFinal, pintNumPeriodos, strTabelaMedia, dblMmExpAnterior, "MME", objConnAux);

					//Calcula a data da próxima cotação conforme a periodicidade do cálculo.
					cCalculadorData objCalculadorData = new cCalculadorData(objConnAux);

					//Se a data base retornar DATAINVALIDA significa que a média aritmética que foi calculada anteriormente é exata a do número de periodos
					//que estamos calculando e nesse caso não tem mais nenhuma média para calcular.
					pdtmDataBase = objCalculadorData.CalcularDataProximoPeriodo(pstrCodigo, dtmDataFinal, pstrTabela);

				} else {
					//se não encontrou um intervalo de datas que também tenha o mesmo número de periodos sai da função retornando o erro.
					objCommand.RollBackTrans();
					return cEnum.enumRetorno.RetornoErro3;
				}

			}


			if (pdtmDataBase != Constantes.DataInvalida) {

				if (!blnPeriodoCalcular) {
					//EXCLUI OS DADOS A PARTIR DA DATA BASE DO CÁLCULO, POIS ESTES DADOS SERÃO RECALCULADOS.
					//NÃO EXCLUI SE O PERÍODO FOI CALCULADO. NESTE CASO JÁ FOI EXCLUÍDO ANTERIORMENTE.
					objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " and Tipo = " + FuncoesBd.CampoStringFormatar("MME") + " and NumPeriodos = " + pintNumPeriodos.ToString());

				}

				//busca todas as cotações a partir da data base. Busca o valor de fechamento, pois a média é calculada em cima deste valor.
				string strQuery = " select Data, ValorFechamento ";


				if (pstrTabela.ToUpper() == "COTACAO_SEMANAL") {
					//SE É COTAÇÃO SEMANAL TEM QUE BUSCAR A DATA FINAL PARA USAR NO CÁLCULO DOS SPLITS
					strQuery = strQuery + ", DataFinal";

				}

				strQuery = strQuery + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataBase) + " order by Data ";

				objRS.ExecuteQuery(strQuery);

			    var cotacoes = new List<CotacaoFechamentoDto>();

			    while (!objRS.EOF)
			    {
			        var cotacaoFechamentoDto = new CotacaoFechamentoDto
			        {
			            DataInicial = Convert.ToDateTime(objRS.Field("Data")),
			            ValorDeFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"))
			        };
			        if (pstrTabela == "COTACAO_SEMANAL")
			        {
			            cotacaoFechamentoDto.DataFinal = Convert.ToDateTime(objRS.Field("DataFinal"));
			        }
                    
                    cotacoes.Add(cotacaoFechamentoDto);
			        objRS.MoveNext();
			    }
                objRS.Fechar();

				cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

				//busca todos os splits a partir da data base em ordem ascendente
				objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataBase, "A", ref objRsSplit, Constantes.DataInvalida);

				//loop para calcular a média exponencial em todas as datas subsequentes

				foreach (var cotacaoFechamentoDto in cotacoes) {

					if (!objRsSplit.EOF) {
						bool blnContinuarLoop;


						if (pstrTabela == "COTACAO") {
							//********TRATAMENTO PARA A COTAÇÃO DIÁRIA
							blnContinuarLoop = (cotacaoFechamentoDto.DataInicial == Convert.ToDateTime(objRsSplit.Field("Data")));

							//verifica se tem split na data.

							while (blnContinuarLoop) {
								//se a data do split é a mesma data do cálculo da média

								//multiplica a média anterior pelo split
								dblMmExpAnterior = dblMmExpAnterior * Convert.ToDouble(objRsSplit.Field("Razao"));

								//passa para o próximo registro
								objRsSplit.MoveNext();

								if (objRsSplit.EOF) {
									blnContinuarLoop = false;
								} else {

									if (cotacaoFechamentoDto.DataInicial != Convert.ToDateTime(objRsSplit.Field("Data"))) {
										blnContinuarLoop = false;

									}

								}

							}


						} else {
							//*********TRATAMENTO PARA A COTAÇÃO SEMANAL
							//para aplicar o split na cotação semanal, a data do split tem que estar entre 
							//o primeiro e ó último dia da semana.

							blnContinuarLoop = (Convert.ToDateTime(objRsSplit.Field("Data")) >= cotacaoFechamentoDto.DataInicial 
                                && Convert.ToDateTime(objRsSplit.Field("Data")) <= cotacaoFechamentoDto.DataFinal);


							while (blnContinuarLoop) {
								//multiplica a média anterior pelo split
								dblMmExpAnterior = dblMmExpAnterior * Convert.ToDouble(objRsSplit.Field("Razao"));

								//passa para o próximo registro
								objRsSplit.MoveNext();

								if (objRsSplit.EOF) {
									blnContinuarLoop = false;

								} else {
									blnContinuarLoop = (Convert.ToDateTime(objRsSplit.Field("Data")) >= cotacaoFechamentoDto.DataInicial 
                                        && Convert.ToDateTime(objRsSplit.Field("Data")) <= cotacaoFechamentoDto.DataFinal);

								}

							}

						}

					}

					//calcula a média 
					double dblMediaMovelMExponencial = MMExponencialCalcular(cotacaoFechamentoDto.ValorDeFechamento, pintNumPeriodos, (decimal) dblMmExpAnterior);

                    MMAtualizar(pstrCodigo, cotacaoFechamentoDto.DataInicial, pintNumPeriodos, strTabelaMedia, dblMediaMovelMExponencial, "MME", objConnAux);

					//atribui a média calculada como média anterior para a próxima iteração;
					dblMmExpAnterior = dblMediaMovelMExponencial;

				}

			}

            objCommand.CommitTrans();

			objConnAux.FecharConexao();
            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		}

	    ///  <summary>
	    ///  Calcula a médida móvel exponencial de todos os papéis.
	    ///  </summary>
	    ///  <param name="pdtmDataInicial">DAta inicial para fazer o cálculo. 
	    ///  Se não for passsada uma data inicial calcula desde a primeira cotação</param>
	    /// <param name="pstrPeriodoDuracao">
	    ///  Período utilizado no cálculo: "DIARIO" ou "SEMANAL"
	    ///  </param>
	    ///  <param name="plstMedias">lista contendo as médias que devem ser calculadas</param>
	    ///  * DATABASE = SALVA OS VALORES CALCULADOS NO DATABASE
	    ///  * MEMORIA = SALVA OS VALORES CALCULADOS EM UMA ESTRUTURA DE MEMÓRIA
	    ///  </param>
	    ///  <param name="pstrAtivos">
	    ///  Código dos ativos separados pelo caracter "#"    
	    ///  </param>    
	    ///  <returns>
	    ///  TRUE - A MÉDIA MÓVEL EXPONENCIAL FOI CALCULADA PARA TODOS OS ATIVOS SEM NENHUM ERRO.
	    ///  FALSE - OCORREU ERRO NA EXECUÇÃO DA MÉDIA MÓVEL PARA PELO MENOS UM DOS ATIVOS.
	    ///  </returns>
	    ///  <remarks></remarks>
	    public bool MediaMovelGeralCalcular(string pstrPeriodoDuracao, List<cMediaDTO> plstMedias, DateTime pdtmDataInicial, string pstrAtivos)
		{
			bool functionReturnValue = false;

			//**********PARA BUSCAR OS ATIVOS NÃO PODE USAR A MESMA CONEXÃO DA TRANSAÇÃO,
			//**********POIS SE A TRANSAÇÃO FIZER ROLLBACK PARA UM ATIVO O RECORDSET NÃO IRÁ FUNCIONAR MAIS.
			cRS objRSAtivo = new cRS();

		    bool blnRetorno = true;

		    string strWhere = String.Empty;

			string strLog = "";

			//indica o nome da tabela de cotações, de acordo com a duração do período das cotações

		    string strTabela = pstrPeriodoDuracao == "DIARIO" ? "Cotacao" : "Cotacao_Semanal";


			//busca todos os ativos do período e a menor data para ser utilizada como data base.
			string strQuery = " select Codigo";

			if (pdtmDataInicial != Constantes.DataInvalida) {
				//se foi passado uma data inicial válida, calcula a data inicial a partir desta data.
				strQuery = strQuery + ", min(Data) as DataInicial ";
			}

			strQuery = strQuery + " from " + strTabela;

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pdtmDataInicial != Constantes.DataInvalida) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " and ";


				if (pstrPeriodoDuracao == "DIARIO") {
					//se passou uma data inicial busca as cotações a partir de uma data.
					strWhere = strWhere + " Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);


				} else {
					//se é uma cotação semanal, a data recebida por parâmetro tem que estar entre 
					//a data inicial e a data final da semana,
					//ou então tem que estar na próxima semana, caso a data informada seja uma data de final de 
					//semana ou feriado que esteja ligado com final de semana 
					strWhere = strWhere + " ((Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + ")" + " or Data > " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + ")";

				}

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " And ";

			    string sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
                strWhere += FuncoesBd.IndiceDaSubString(FuncoesBd.ConcatenarStrings(new []{sustenidoFormatado,"Codigo",sustenidoFormatado}),FuncoesBd.CampoStringFormatar(pstrAtivos)) + " > 0";

			}


			//'*************************************************
			//'UTILIZADO PARA DEBUG. COLOCAR O CÓDIGO DE UM PAPEL
			//If strWhere <> "" Then strWhere = strWhere & " and "

			//strWhere = strWhere _
			//& " Codigo = " & FuncoesBD.CampoStringFormatar("VNET3")
			//'*************************************************


			if (!string.IsNullOrEmpty(strWhere)) {
				strQuery = strQuery + " where " + strWhere;

			}

			strQuery = strQuery + " group by Codigo ";

			objRSAtivo.ExecuteQuery(strQuery);

		    DateTime dtmCotacaoAnteriorData = Constantes.DataInvalida;

		    var ativos = new List<CotacaoDataDto>();

		    while (!objRSAtivo.EOF)
		    {

		        var cotacaoDataDto = new CotacaoDataDto
		        {
		            Codigo = (string) objRSAtivo.Field("Codigo")
		        };

                if (pdtmDataInicial != Constantes.DataInvalida)
                {
                    cotacaoDataDto.Data = Convert.ToDateTime(objRSAtivo.Field("DataInicial"));
                }

                ativos.Add(cotacaoDataDto);

                objRSAtivo.MoveNext();
		    }

            objRSAtivo.Fechar();

			foreach (var cotacaoDataDto in ativos) {
			    DateTime dtmDataInicialAux;
			    if (pdtmDataInicial == Constantes.DataInvalida) {
					//se não recebeu uma data inicial válida, não faz sentido buscar a menor data
					//porque estas datas não serão utilizadas pelas funções de cálculo de média, 
					//já que as funções terão que calcular o período inicial. Neste caso, a query
					//retorna a data inválida.
					dtmDataInicialAux = Constantes.DataInvalida;


				} else {
					dtmDataInicialAux = cotacaoDataDto.Data ;

					//If Not pcolMedia Is Nothing Then

					if ((plstMedias != null)) {
						dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(cotacaoDataDto.Codigo, dtmDataInicialAux, strTabela);

					}

				}

				//For Each objStructMedia In pcolMedia

				foreach (cMediaDTO objMediaDTO in plstMedias)
				{
				    //If objStructMedia.strTipo = "E" Then

				    cEnum.enumRetorno intRetorno = objMediaDTO.Tipo == "E" ? 
				        MediaMovelMExponencialRetroativaUnitCalcular(cotacaoDataDto.Codigo, dtmDataInicialAux, objMediaDTO.NumPeriodos, strTabela, dtmCotacaoAnteriorData) :
                        MediaMovelAritmeticaRetroativoUnitCalcular(cotacaoDataDto.Codigo, objMediaDTO.Dado, pstrPeriodoDuracao, objMediaDTO.NumPeriodos, dtmDataInicialAux);


				    if (intRetorno != cEnum.enumRetorno.RetornoOK) {
						blnRetorno = false;

						if (!string.IsNullOrEmpty(strLog)) {
							//coloca um ENTER PARA QUEBRAR A LINHA
							strLog = strLog + Environment.NewLine;
						}

						strLog = strLog + " Código: " + cotacaoDataDto.Codigo + " - Tabela: " + strTabela + " - Período: " + objMediaDTO.NumPeriodos + " - Tipo: " + objMediaDTO.Tipo;


						if (dtmDataInicialAux != Constantes.DataInvalida) {
							strLog = strLog + " - Data Inicial: " + dtmDataInicialAux;

						}

					}
				}

			}


			if (!string.IsNullOrEmpty(strLog)) {
			    string strArquivoNome = pstrPeriodoDuracao == "DIARIO" ? "Log_MMExp_Diario.txt" : "Log_MMExp_Semanal.txt";

			    var fileService = new FileService();
                fileService.Save(cBuscarConfiguracao.ObtemCaminhoPadrao() + strArquivoNome, strLog);

			}

			if (objRSAtivo.QueryStatus) {
				functionReturnValue = blnRetorno;
			}

			return functionReturnValue;

		}

		/// <summary>
		/// Lita todos os ativos que não devem ser considerados separados pelo simbolo "#"
		/// </summary>
		/// <returns>String contendo a lista de ativos não considerados</returns>
		/// <remarks></remarks>
		private string AtivosDesconsideradosListar()
		{

			cRS objRS = new cRS(objConexao);

			string strLista = String.Empty;

			objRS.ExecuteQuery(" select Codigo " + " from Ativos_Desconsiderados");


			while (!objRS.EOF) {
				strLista = strLista + "#" + objRS.Field("Codigo");

				objRS.MoveNext();

			}

			objRS.Fechar();

			//tem que colocar o # para não ocorrer erros do tipo ter um código desconsiderado VALE50
			//e o ativo VALE5 NÃO SER IMPORTADO
			strLista = strLista + "#";

			return strLista;

		}

	    /// <summary>
	    /// Calcula, para um ativo, qual é a primeira semana que este ativo tem cotação a semana completa
	    /// </summary>
	    /// <param name="pstrCodigo">Código do ativo</param>
	    /// <param name="pobjConexao"></param>
	    /// <returns>a data da segunda-feira da primeira semana em que existe uma cotação completa</returns>
	    /// <remarks></remarks>
	    private DateTime PrimeiraSemanaDataCalcular(string pstrCodigo, Conexao pobjConexao = null)
		{
		    cRS objRs = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

	        objRs.ExecuteQuery(" select min(Data) as Data " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

			DateTime dtmDataAux = Convert.ToDateTime(objRs.Field("Data"));

            objRs.Fechar();

			return PrimeiraSemanaDataCalcular(dtmDataAux);

		}

		/// <summary>
		/// Calcula, para um ativo, qual é a primeira semana que este ativo tem cotação a semana completa
		/// </summary>
		/// <param name="pdtmDataBase">Data recebida para iniciar os cálculos. 
		/// A primeira semana de cálculo tem que compreender
		/// esta data ou ser uma semana seguinte a esta data</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private DateTime PrimeiraSemanaDataCalcular(DateTime pdtmDataBase)
		{

			//se a data é uma segunda-feira retorna a própria data
			DateTime dtmDataAux = pdtmDataBase;

            DayOfWeek diaDaSemana = dtmDataAux.DayOfWeek;

		    if (diaDaSemana == DayOfWeek.Monday)
		    {
		        return dtmDataAux;
		    }

		    double dblIntervalo;
		    if (diaDaSemana ==  DayOfWeek.Saturday || diaDaSemana == DayOfWeek.Sunday) {
		        //se o dia da semana é sábado ou domingo tem que buscar a próxima segunda-feira,
		        //então o incremento tem que ser positivo para a data ir para uma data posterior
		        dblIntervalo = 1;


		    } else {
		        //se a data está entre terça e sexta, o incremento tem que ser negativo para a data
		        //voltar até a segunda-feira anterior
		        dblIntervalo = -1;

		    }


		    while (diaDaSemana != DayOfWeek.Monday) {
		        dtmDataAux = dtmDataAux.AddDays(dblIntervalo);

		        //tem que descontar 1 para bater com o enum.
		        diaDaSemana = dtmDataAux.DayOfWeek;

		    }

		    return dtmDataAux;

		}

		/// <summary>
		/// Recebe uma data e busca o primeiro dia da semana para a qual esta data pertence.
		/// Se a data recebida for um dia sem cotação, como um sábado ou domingo, irá buscar a primeira data da
		/// semana anterior.
		/// Se for um feriado no meio da semana irá buscar a primeira data da mesma semana do feriado
		/// </summary>
		/// <param name="pdtmDataBase"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime CotacaoSemanalPrimeiroDiaSemanaCalcular(DateTime pdtmDataBase)
		{

			cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    //primeiro busca a primeira data na cotação diária com data menor ou igual à data recebida por parâmetro.
			objRS.ExecuteQuery("SELECT MAX(Data) as Data " + "FROM Cotacao " + "WHERE Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataBase));

			DateTime dtmDataAux = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

			objRS.Fechar();


			if (dtmDataAux != Constantes.DataInvalida) {
				//se existe uma data menor ou igual à data informada

				//busca a menor data inicial de semana na qual esta cotação está contida
				objRS.ExecuteQuery("SELECT MIN(Data) as Data " + "FROM Cotacao_Semanal " + "WHERE Data <= " + FuncoesBd.CampoDateFormatar(dtmDataAux) + " AND DataFinal >= " + FuncoesBd.CampoDateFormatar(dtmDataAux));

				dtmDataAux = Convert.ToDateTime(objRS.Field("Data"));

				objRS.Fechar();

			}

			return dtmDataAux;

		}

		/// <summary>
		/// Consulta o primeiro dia de cotação  de uma semana para um determinado ativo.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">Data para o qual deve ser buscada o primeiro dia da semana</param>
		/// <returns>A data do primeiro dia da semana</returns>
		/// <remarks></remarks>
		public DateTime AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(string pstrCodigo, DateTime pdtmDataBase)
		{

		    if (pdtmDataBase == Constantes.DataInvalida)
		    {
		        return Constantes.DataInvalida;
		    }

		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    objRS.ExecuteQuery(" select max(Data) as Data " + Environment.NewLine + " from Cotacao_Semanal" +
		                       Environment.NewLine + " where Codigo = " + FuncoesBd.CampoFormatar(pstrCodigo) +
		                       Environment.NewLine + " and Data <= " + FuncoesBd.CampoFormatar(pdtmDataBase));

			DateTime functionReturnValue = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

			objRS.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Consulta o último dia de cotação de uma semana.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmPrimeiroDiaSemana">Primeiro dia de cotação de uma semana</param>
		/// <returns>Data do último dia de cotação de uma semana.</returns>
		/// <remarks></remarks>
		private DateTime AtivoCotacaoSemanalUltimoDiaSemanaCalcular(string pstrCodigo, DateTime pdtmPrimeiroDiaSemana)
		{
		    cRS objRS = new cRS(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

            objRS.ExecuteQuery("SELECT DataFinal " + Environment.NewLine + " FROM Cotacao_Semanal " + Environment.NewLine +
		                       " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine +
		                       " AND Data = " + FuncoesBd.CampoDateFormatar(pdtmPrimeiroDiaSemana));

			DateTime dataDoUltimoDiaDaSemana = (DateTime) objRS.Field("DataFinal");

			objRS.Fechar();

		    return dataDoUltimoDiaDaSemana;

		}


		private void CotacaoSemanalDadosGerar(string pstrCodigo, DateTime pdtmDataSegundaFeira, DateTime pdtmDataSextaFeira, string pstrOperacaoBD, Conexao pobjConnAux, ref decimal pdecCotacaoAnteriorRet)
		{
			cCommand objCommand = new cCommand(pobjConnAux);
			cRS objRS = new cRS(pobjConnAux);
			cRSList objRsSplit = null;

		    double dblSplitRazaoCotacaoAbertura = 0;
			//Dim dblSplitRazaoInvertida As Double
			//Dim dtmSplitData As Date

			//indica se tem splits em todo o período calculado

		    //indica se tem splits na semana do cálculo
			bool blnSplitSemana = false;

		    cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(pobjConnAux);

			//busca os splits. 
			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigo, pdtmDataSegundaFeira, "D", ref objRsSplit, pdtmDataSextaFeira);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (blnSplitExistir) {
				//busca a primeira data da semana que tem cotação
				objRS.ExecuteQuery(" select min(Data) as DataInicial " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataSegundaFeira) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataSextaFeira));

				DateTime dtmDataPrimeiraCotacaoSemana = Convert.ToDateTime(objRS.Field("DataInicial"));

				objRS.Fechar();

				dblSplitRazaoCotacaoAbertura = 1;
				double dblSplitRazaoCotacaoAnterior = 1;
				//dblSplitRazaoInvertida = 1


				while (!objRsSplit.EOF) {

					if (Convert.ToDateTime(objRsSplit.Field("Data")) != dtmDataPrimeiraCotacaoSemana) {
						//se a data  de pelo menos um dos splits não é a data da primeira cotação da semana, tem que fazer as conversões.
						blnSplitSemana = true;

						//dtmSplitData = CDate(objRSSplit.Field("Data"))
						dblSplitRazaoCotacaoAbertura *= Convert.ToDouble(objRsSplit.Field("Razao"));

					}

					dblSplitRazaoCotacaoAnterior *= Convert.ToDouble(objRsSplit.Field("Razao"));

					//se encontrou passa para o próximo split.
					objRsSplit.MoveNext();

				}

				//se tem split tem que multiplicar a cotação anterior pela razão do split,
				//independentemente se é no primeiro dia da semana ou não, porque a cotação da semana 
				//anterior estava sem o split.
				pdecCotacaoAnteriorRet = Convert.ToDecimal(pdecCotacaoAnteriorRet * (decimal) dblSplitRazaoCotacaoAnterior);

			}


			if (blnSplitSemana) {
			    List<string> lstQueries = ConsultaQueriesGerar(pstrCodigo, pdtmDataSegundaFeira, pdtmDataSextaFeira, "DIARIO", "COTACAO", pdtmDataSextaFeira, true, true);

				string strFrom = String.Empty;


				foreach (string strSQL in lstQueries) {

					if (strFrom != String.Empty) {
						strFrom = strFrom + " UNION " + Environment.NewLine;

					}

					strFrom = strFrom + strSQL;

				}

				//se tem split as data anteriores ao split tem que ser convertidas
				//a primeira parte da query traz as cotações anteriores ao split. 
				//Estas cotações tem que ser multiplicadas pela razão do split
				//A segunda parte traz as cotações já com o split e não precisam ser convertidas
				objRS.ExecuteQuery(" select min(Data) as DataInicial, max(Data) as DataFinal, min(ValorMinimo) as ValorMinimo " + ", max(ValorMaximo) as ValorMaximo, sum(Negocios_Total) as Negocios_Total " + ", sum(Titulos_Total) as Titulos_Total, sum(Valor_Total) as Valor_Total " + ", count(1) as Contador " + " from (" + strFrom + ")");

			} else {
				//se não tem split faz a busca normal

				//busca o resumo das cotações para o ativo na semana.
				objRS.ExecuteQuery(" select min(Data) as DataInicial, max(Data) as DataFinal, min(ValorMinimo) as ValorMinimo " + ", max(ValorMaximo) as ValorMaximo, sum(Negocios_Total) as Negocios_Total " + ", sum(Titulos_Total) as Titulos_Total, sum(Valor_Total) as Valor_Total " + ", count(1) as Contador " + " from Cotacao " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataSegundaFeira) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataSextaFeira));

			}

			if (Convert.ToInt32(objRS.Field("Contador")) > 0) {
				decimal decValorAbertura = default(decimal);
				decimal decValorFechamento = default(decimal);
				decimal decOscilacao;
				decimal decDiferenca;

				//busca o valor de abertura da primeira data da semana
			    decimal pdecValorFechamentoRet = -1M;
			    CotacaoConsultar(pstrCodigo, Convert.ToDateTime(objRS.Field("DataInicial")), "Cotacao", ref pdecValorFechamentoRet , ref decValorAbertura, pobjConnAux);


				if (blnSplitSemana) {
					//se tem split na semana, a cotação de abertura é anterior 
					//ao split e tem que fazer a multiplicaçao pela razão
					decValorAbertura = Convert.ToDecimal(decValorAbertura * (decimal) dblSplitRazaoCotacaoAbertura);

				}

				//busca o valor de fechamento da última data da semana.
			    decimal pdecValorAberturaRet = -1M;
			    CotacaoConsultar(pstrCodigo, Convert.ToDateTime(objRS.Field("DataFinal")), "Cotacao", ref decValorFechamento, ref pdecValorAberturaRet , pobjConnAux);


				if (pdecCotacaoAnteriorRet > 0) {
					//calcula a diferença entre o valor de fechamento e o valor de abertura
					decDiferenca = decValorFechamento - pdecCotacaoAnteriorRet;
					decOscilacao = Math.Round((decValorFechamento / pdecCotacaoAnteriorRet - 1) * 100, 2);
				} else {
					decDiferenca = 0;
					decOscilacao = 0;
				}

				if (pstrOperacaoBD == "INSERT")
				{
				    //calcula o sequencial do ativo
				    long lngSequencial = SequencialCalcular(pstrCodigo, "Cotacao_Semanal", objCommand.Conexao);

				    objCommand.Execute(" INSERT INTO Cotacao_Semanal " + "(Codigo, Data, DataFinal, ValorAbertura, ValorMinimo, ValorMaximo, ValorFechamento " + ", Diferenca, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial) " + " values " + "( " + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("DataInicial"))) + ", " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("DataFinal"))) + ", " + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + ", " + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRS.Field("ValorMinimo"))) + ", " + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRS.Field("ValorMaximo"))) + ", " + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + ", " + FuncoesBd.CampoDecimalFormatar(decDiferenca) + ", " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Negocios_Total"))) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Titulos_Total"))) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Valor_Total"))) + ", " + lngSequencial.ToString() + ")");
				}
				else {
					objCommand.Execute(" UPDATE Cotacao_Semanal SET " + " DataFinal = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("DataFinal"))) + ", ValorAbertura = " + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + ", ValorMinimo = " + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRS.Field("ValorMinimo"))) + ", ValorMaximo = " + FuncoesBd.CampoDecimalFormatar(Convert.ToDecimal(objRS.Field("ValorMaximo"))) + ", ValorFechamento = " + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + ", Diferenca = " + FuncoesBd.CampoDecimalFormatar(decDiferenca) + ", Oscilacao = " + FuncoesBd.CampoDecimalFormatar(decOscilacao) + ", Negocios_Total = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Negocios_Total"))) + ", Titulos_Total = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Titulos_Total"))) + ", Valor_Total = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Valor_Total"))) + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("DataInicial"))));

				}

				//ajuste de variáveis para a próxima iteração
				pdecCotacaoAnteriorRet = decValorFechamento;

			}
			//se tem dados no período.

			objRS.Fechar();

		}

		/// <summary>
		/// Calcula os dados da cotação semanal para um ativo recebido por parâmetro
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataBase">Data inicial para começar o cálculo. Quando este parâmetro for igual 
		/// a "DataInvalida" calcula a partir da última cotação</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private bool CotacaoSemanalUnitRetroativoCalcular(string pstrCodigo, DateTime pdtmDataBase)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);
			cRS objRS = new cRS(objConnAux);

			cCalculadorData objCalculadorData = new cCalculadorData(objConnAux);

			objCommand.BeginTrans();

			//busca a maior data em que há cotação para o ativo recebido por parâmetro
			DateTime dtmDataMaxima = objCalculadorData.CotacaoDataMaximaConsultar(pstrCodigo, "Cotacao");

			DateTime dtmDataSegundaFeira;

			if (pdtmDataBase != Constantes.DataInvalida) {
				dtmDataSegundaFeira = PrimeiraSemanaDataCalcular(pdtmDataBase);
			} else {
				dtmDataSegundaFeira = PrimeiraSemanaDataCalcular(pstrCodigo, objConnAux);
			}

		    //a data maior da semana é sempre a data de sexta-feira, que são quatro dias após a segunda
			DateTime dtmDataSextaFeira = dtmDataSegundaFeira.AddDays(4);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//EXCLUI OS DADOS A PARTIR DA PRIMEIRA SEGUNDA-FEIRA, POIS ESTES DADOS SERÃO RECALCULADOS.
			objCommand.Execute(" DELETE " + " FROM Cotacao_Semanal " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataSegundaFeira));

			//Dim decValorAbertura As Decimal
			//Dim decValorFechamento As Decimal
			//Dim decOscilacao As Decimal
			//Dim decDiferenca As Decimal
			decimal decCotacaoAnterior = default(decimal);


			if (pdtmDataBase != Constantes.DataInvalida) {
			    //consulta a primeira data anterior que tem cotação.

				//dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataBase _
				//, "Cotacao_Semanal", objConnAux)

				DateTime dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataSegundaFeira, "Cotacao_Semanal", objConnAux);

                decimal valorDeAbertura = 0;
				//busca o valor de fechamento nesta data.
				CotacaoConsultar(pstrCodigo, dtmCotacaoAnteriorData, "Cotacao_Semanal", ref decCotacaoAnterior, ref valorDeAbertura , objConnAux);

			} else {
				//se não passou uma data base, então vai calcular tudo desde o começo.
				//Neste caso a cotação anterior é 0.
				decCotacaoAnterior = 0;
			}

		    objRS.ExecuteQuery(" select count(1) as contador " + " from Cotacao_Semanal " + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataSegundaFeira) + " and Data <= " + FuncoesBd.CampoDateFormatar(dtmDataSextaFeira));

            //indica se a operação no banco de dados deve ser um INSERT ou um UPDATE.
            string strOperacaoBd = Convert.ToInt32(objRS.Field("Contador")) > 0 ? "UPDATE" : "INSERT";

			objRS.Fechar();

			while ((dtmDataSegundaFeira <= dtmDataMaxima) && (objCommand.TransStatus)) {
				CotacaoSemanalDadosGerar(pstrCodigo, dtmDataSegundaFeira, dtmDataSextaFeira, strOperacaoBd, objConnAux, ref decCotacaoAnterior);

				//incrementa a data de segunda-feira em uma semana.
				dtmDataSegundaFeira = dtmDataSegundaFeira.AddDays(7);

				//incrementa a data de sexta-feira em uma semana.
				dtmDataSextaFeira = dtmDataSextaFeira.AddDays(7);

				//depois da primeira iteração a operação é sempre INSERT
				strOperacaoBd = "INSERT";

				objRS.Fechar();

			}

			objCommand.CommitTrans();

			bool functionReturnValue = objCommand.TransStatus;

			objConnAux.FecharConexao();
			return functionReturnValue;

		}


		private bool CotacaoSemanalUnitRetroativoCalcularSplit(string pstrCodigo, DateTime pdtmDataBase)
		{

			Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);
			cRSList objRSSplit = null;

		    decimal decCotacaoAnterior = default(decimal);

		    objCommand.BeginTrans();

			DateTime dtmDataSegundaFeira = PrimeiraSemanaDataCalcular(pdtmDataBase);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConnAux);

			//busca os splits
			objCarregadorSplit.SplitConsultar(pstrCodigo, dtmDataSegundaFeira, "A", ref objRSSplit,Constantes.DataInvalida);

			//PARA CADA UM DOS SPLITS

			while (!objRSSplit.EOF) {
				//CALCULA A DATA DA SEGUNDA-FEIRA E DA SEXTA-FEIRA DA SEMANA EM QUE HÁ SPLIT.
				dtmDataSegundaFeira = PrimeiraSemanaDataCalcular(Convert.ToDateTime(objRSSplit.Field("Data")));

				//a data maior da semana é sempre a data de sexta-feira, que são quatro dias após a segunda
				DateTime dtmDataSextaFeira = dtmDataSegundaFeira.AddDays(4);

				DateTime dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, dtmDataSegundaFeira, "Cotacao_Semanal", objConnAux);

				//busca o valor de fechamento nesta data.
			    decimal pdecValorAberturaRet = -1M;
			    CotacaoConsultar(pstrCodigo, dtmCotacaoAnteriorData, "Cotacao_Semanal", ref decCotacaoAnterior, ref pdecValorAberturaRet , objConnAux);

				CotacaoSemanalDadosGerar(pstrCodigo, dtmDataSegundaFeira, dtmDataSextaFeira, "UPDATE", objConnAux, ref decCotacaoAnterior);


				objRSSplit.MoveNext();

			}

			objCommand.CommitTrans();

			objConnAux.FecharConexao();

			return objCommand.TransStatus;

		}


	    /// <summary>
	    /// Calcula os dados da cotação semanal para todos os ativos
	    /// </summary>
	    /// <param name="pdtmData">
	    /// Quando este parâmetro é uma data válida calcula os dados a partir desta data, inclusive.
	    /// Quando este parâmetro é uma data inválida calcula os dados desde a primeira data em que existe cotação diária.
	    /// </param>
	    /// '''
	    /// <param name="pstrAtivos">Lista dos ativos para os quais será calculada a cotação</param>
	    /// <param name="pblnCalcularApenasEmSplit">Indica se é para fazer o recálculo das cotações semanais apenas 
	    /// nas semanas que houver split. Neste caso também tem que calcular na semana subsequente, pois esta
	    /// é dependente da primeira para calcular os campos "Diferenca" e "Oscilacao"</param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private bool CotacaoSemanalRetroativoGeralCalcular(DateTime pdtmData, string pstrAtivos = "", bool pblnCalcularApenasEmSplit = false)
		{

			cRS objRS = new cRS();

			string strLog = "";

		    string strQuery = " select Codigo " + " from Cotacao " + " where not exists " + "(" + " select 1 " + " from Ativos_Desconsiderados " + " where Cotacao.Codigo = Ativos_Desconsiderados.Codigo " + ")";


			if (pdtmData != Constantes.DataInvalida) {
                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();
                strQuery = strQuery + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmData);
			}

			if (pstrAtivos != String.Empty) {
				strQuery = strQuery + " and INSTR(" + FuncoesBd.CampoStringFormatar(pstrAtivos) + ", '#' & Codigo & '#') > 0";
			}

			strQuery = strQuery + " group by Codigo ";

			objRS.ExecuteQuery(strQuery);


			while (!objRS.EOF) {
			    bool blnRetorno = pblnCalcularApenasEmSplit ?  
                    CotacaoSemanalUnitRetroativoCalcularSplit((string) objRS.Field("Codigo"), pdtmData) : 
                    CotacaoSemanalUnitRetroativoCalcular((string) objRS.Field("Codigo"), pdtmData);


				if (!blnRetorno) {
					if (!string.IsNullOrEmpty(strLog)) {
						//coloca um ENTER PARA QUEBRAR A LINHA
						strLog = strLog + Environment.NewLine;
					}

					strLog = strLog + " Código = " + objRS.Field("Codigo");

				}

				objRS.MoveNext();

			}


			if (!string.IsNullOrEmpty(strLog)) {
			    var fileService = new FileService();
                fileService.Save(cBuscarConfiguracao.ObtemCaminhoPadrao() + "Log_Cotacao_Semanal.txt",strLog);
			}

			objRS.Fechar();

			return (string.IsNullOrEmpty(strLog));

		}

		/// <summary>
		/// Calcula todos os dados da cotação diária a partir de uma determinada data ou desde o início das cotações.
		/// </summary>
		/// <param name="pblnOscilacaoCalcular">se for TRUE indica que é para calcular a oscilação das cotações</param>
		/// <param name="pblnOscilacaoPercentualCalcular">quando for para calcular a oscilação indica se é para calcular o percentualou apenas a diferença</param>
		/// <param name="pblnIFRCalcular">se for TRUE indica que é para calcular o indice de força relativa</param>
		/// <param name="pblnMMExpCalcular">se for TRUE indica que é para calcular a média móvel exponencial</param>
		/// <param name="pdtmDataBase"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer os cálculos 
		/// apenas nas datas em que houver split. Esta opção é utilizada no recálculo quando
		/// há importação de proventos. 
		/// ATENÇÃO: quando este parâmetro for TRUE, é obrigatório passar o parâmetro
		/// "pdtmDataInicial" com uma data válida
		/// </param>
		/// <param name="pblnCotacaoAnteriorInicializar">Indica se é para inicializar a tabela de cotações anteriores
		/// para não ser necessário buscar o valor da cotação anterior em todos os cálculos de indicadores. Esta busca
		/// é um pouco demorada e esta opção foi criada para otimizar o tempo de atualização das cotações. </param>
		/// <param name="pblnIFRMedioCalcular">Indica se é para calcular o IFR médio das cotações</param>
		/// <param name="pblnVolumeMedioCalcular">Indica se é para calcular o volume médio das cotações</param>
		/// <param name="pstrAtivos">Lista de ativos para os quais deve ser feito o cálculo</param>
		/// <returns>
		/// ''' RetornoOK = todos os cálculos foram realizados com sucesso.
		/// RetornoErro2 = erro ao transportar os dados das cotações diárias para a cotação semanal
		/// RetornoErro3 = erro ao calcular o índice de força relativa
		/// RetornoErro4 = erro ao calcular a média móvel exponencial.
		///</returns>
		/// <remarks></remarks>
		private bool CotacaoDiariaDadosAtualizar(bool pblnOscilacaoCalcular, bool pblnOscilacaoPercentualCalcular, bool pblnIFRCalcular, bool pblnMMExpCalcular, bool pblnVolumeMedioCalcular, bool pblnIFRMedioCalcular, DateTime pdtmDataBase, string pstrAtivos = "", bool pblnCotacaoAnteriorInicializar = true, bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOK = true;
			bool blnIFROK = true;
			bool blnMMExpOK = true;


			if ((pdtmDataBase != Constantes.DataInvalida) && pblnCotacaoAnteriorInicializar) {
				blnOK = CotacaoAnteriorInicializar("DIARIO", pdtmDataBase, pstrAtivos);


				if (!blnOK) {
                    MessageBox.Show("Ocorreram erros ao inicializar os dados das cotações anteriores.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return false;

				}

			}


			if (pblnOscilacaoCalcular) {
				//atualiza os dados "Oscilacao" e "Diferenca" da tabela "Cotacao"
				blnOK = OscilacaoGeralCalcular(pblnOscilacaoPercentualCalcular, pdtmDataBase, pstrAtivos, pblnConsiderarApenasDataSplit);

			}


			if (blnOK) {

				if (pblnIFRCalcular) {
					IList<int> colPeriodos = new List<int>();

					colPeriodos.Add(2);

					blnIFROK = IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Diario, pdtmDataBase, pstrAtivos);

				}


			} else {
                MessageBox.Show("Ocorreram erros ao calcular os dados das cotações diárias.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

			//Dim objstructMediaEscolha As structIndicadorEscolha

			//Dim colMediaEscolha As Collection
			List<cMediaDTO> lstMediasSelecionadas = null;


			if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular) {
				//colMediaEscolha = New Collections
				lstMediasSelecionadas = new List<cMediaDTO>();

			}


			if (pblnMMExpCalcular) {

				lstMediasSelecionadas.Add(new cMediaDTO("E", 21, "VALOR"));
				lstMediasSelecionadas.Add(new cMediaDTO("E", 200, "VALOR"));
				lstMediasSelecionadas.Add(new cMediaDTO("E", 49, "VALOR"));

			}


			if (pblnIFRMedioCalcular) {

                lstMediasSelecionadas.Add(new cMediaDTO("A", 13, "IFR2"));
			}


			if (pblnVolumeMedioCalcular) {
				lstMediasSelecionadas.Add(new cMediaDTO("A", 21, "VOLUME"));
			}


			if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular)
			{
			    blnMMExpOK = MediaMovelGeralCalcular("DIARIO", lstMediasSelecionadas, pdtmDataBase, pstrAtivos);

			}

		    return (blnOK && blnIFROK && blnMMExpOK);

		}


		/// <summary>
		/// Calcula todos os dados da cotação semanal a partir de uma determinada data ou desde o início das cotações.
		/// </summary>
		/// <param name="pblnDadosCalcular">se for TRUE indica que é para calcular os dados da cotação utilizando como base a tabela de cotações diárias</param>
		/// <param name="pblnIFRCalcular">se for TRUE indica que é para calcular o indice de força relativa</param>
		/// <param name="pblnMMExpCalcular">se for TRUE indica que é para calcular a média móvel exponencial</param>
		/// <param name="pdtmDataBase"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica se é para fazer o recálculo das cotações semanais apenas 
		/// nas semanas que houver split. Neste caso também tem que calcular na semana subsequente, pois esta
		/// é dependente da primeira para calcular os campos "Diferenca" e "Oscilacao"
		/// </param>
		/// <returns>
		/// ''' RetornoOK = todos os cálculos foram realizados com sucesso.
		/// RetornoErro2 = erro ao transportar os dados das cotações diárias para a cotação semanal
		/// RetornoErro3 = erro ao calcular o índice de força relativa
		/// RetornoErro4 = erro ao calcular a média móvel exponencial.
		///</returns>
		/// <remarks></remarks>
		private bool CotacaoSemanalDadosAtualizar(bool pblnDadosCalcular, bool pblnIFRCalcular, bool pblnMMExpCalcular, bool pblnVolumeMedioCalcular, bool pblnIFRMedioCalcular, DateTime pdtmDataBase, string pstrAtivos = "", bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOK = true;


			if ((pdtmDataBase != Constantes.DataInvalida)) {
				//INICIALIZA AS COTAÇÕES DE DATAS ANTERIORES ANTES DE TODAS AS OPERAÇÕES.
				blnOK = CotacaoAnteriorInicializar("SEMANAL", pdtmDataBase, pstrAtivos);


				if (!blnOK) {
                    MessageBox.Show("Ocorreram erros ao inicializar os dados das cotações anteriores.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return false;

				}

			}


			if (pblnDadosCalcular) {
				//atualiza os dados da tabela de cotação semanal buscando da tabela de cotação diária
				blnOK = CotacaoSemanalRetroativoGeralCalcular(pdtmDataBase, pstrAtivos, pblnConsiderarApenasDataSplit);

			}


			if (blnOK) {

				if (pblnIFRCalcular) {
					IList<int> colPeriodos = new List<int>();

					//colPeriodos.Add(14)

					colPeriodos.Add(2);

					//If Not IFRGeralCalcular(14, "SEMANAL", pdtmDataBase, pstrAtivos) Then


					if (!IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Semanal, pdtmDataBase, pstrAtivos)) {
						//MsgBox("Ocorreram erros ao calcular o Indice de Força Relativa para as cotações semanais." _
						//, MsgBoxStyle.Exclamation, "Atualizar Cotações")

						blnOK = false;

					}

				}

				//Dim objstructMediaEscolha As structIndicadorEscolha

				//Dim colMediaEscolha As Collection
				List<cMediaDTO> lstMediasSelecionadas = null;


				if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular) {
					//colMediaEscolha = New Collection
					lstMediasSelecionadas = new List<cMediaDTO>();

				}


				if (pblnMMExpCalcular) {

					lstMediasSelecionadas.Add(new cMediaDTO("E", 21, "VALOR"));

					lstMediasSelecionadas.Add(new cMediaDTO("E", 200, "VALOR"));

					lstMediasSelecionadas.Add(new cMediaDTO("E", 49, "VALOR"));

				}


				if (pblnIFRMedioCalcular) {
					lstMediasSelecionadas.Add(new cMediaDTO("A", 13, "IFR2"));

				}


				if (pblnVolumeMedioCalcular) {
					lstMediasSelecionadas.Add(new cMediaDTO("A", 21, "VOLUME"));

				}


				if (pblnMMExpCalcular || pblnVolumeMedioCalcular || pblnIFRMedioCalcular)
				{
				    if (!MediaMovelGeralCalcular("SEMANAL", lstMediasSelecionadas, pdtmDataBase, pstrAtivos)) {
						blnOK = false;

					}
				}
			} else {
			    MessageBox.Show("Ocorreram erros ao calcular os dados das cotações semanais.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

			return blnOK;

		}

		/// <summary>
		/// Função que centraliza a chamada de recálculo de todos os indicadores.
		/// </summary>
		/// <param name="pblnCotacaoDiariaOscilacaoCalcular"></param>
		/// <param name="pblnCotacaoDiariaOscilacaoPercentualCalcular"></param>
		/// <param name="pblnCotacaoDiariaIFRCalcular"></param>
		/// <param name="pblnCotacaoDiariaMMExpCalcular"></param>
		/// <param name="pblnCotacaoDiariaVolumeMedioCalcular"></param>
		/// <param name="pblnCotacaoDiariaIFRMedioCalcular"></param>
		/// <param name="pblnCotacaoSemanalDadosCalcular"></param>
		/// <param name="pblnCotacaoSemanalIFRCalcular"></param>
		/// <param name="pblnCotacaoSemanalMMExpCalcular"></param>
		/// <param name="pblnCotacaoSemanalVolumeMedioCalcular"></param>
		/// <param name="pblnCotacaoSemanalIFRMedioCalcular"></param>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pstrAtivos"></param>
		/// <param name="pblnCotacaoAnteriorInicializar"></param>
		/// <param name="pblnConsiderarApenasDataSplit">Indica para fazer cálculos apenas 
		/// nas datas em que há splits. É utilizado no cálculo da oscilação na cotação diária
		/// e no cálculo de todos os dados da tabela COTACAO_SEMANAL</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool DadosRecalcular(bool pblnCotacaoDiariaOscilacaoCalcular, bool pblnCotacaoDiariaOscilacaoPercentualCalcular, bool pblnCotacaoDiariaIFRCalcular, bool pblnCotacaoDiariaMMExpCalcular, bool pblnCotacaoDiariaVolumeMedioCalcular, bool pblnCotacaoDiariaIFRMedioCalcular, bool pblnCotacaoSemanalDadosCalcular, bool pblnCotacaoSemanalIFRCalcular, bool pblnCotacaoSemanalMMExpCalcular, bool pblnCotacaoSemanalVolumeMedioCalcular,
		bool pblnCotacaoSemanalIFRMedioCalcular, DateTime pdtmDataInicial, string pstrAtivos = "", bool pblnCotacaoAnteriorInicializar = true, bool pblnConsiderarApenasDataSplit = false)
		{

			bool blnOkCotacaoDiaria = true;
			bool blnOkCotacaoSemanal = true;


			if (pblnCotacaoDiariaOscilacaoCalcular || pblnCotacaoDiariaIFRCalcular || pblnCotacaoDiariaMMExpCalcular || pblnCotacaoDiariaVolumeMedioCalcular || pblnCotacaoDiariaIFRMedioCalcular) {
				blnOkCotacaoDiaria = CotacaoDiariaDadosAtualizar(pblnCotacaoDiariaOscilacaoCalcular, pblnCotacaoDiariaOscilacaoPercentualCalcular, pblnCotacaoDiariaIFRCalcular, pblnCotacaoDiariaMMExpCalcular, pblnCotacaoDiariaVolumeMedioCalcular, pblnCotacaoDiariaIFRMedioCalcular, pdtmDataInicial, pstrAtivos, pblnCotacaoAnteriorInicializar, pblnConsiderarApenasDataSplit);

			}


			if (pblnCotacaoSemanalDadosCalcular || pblnCotacaoSemanalIFRCalcular || pblnCotacaoSemanalMMExpCalcular || pblnCotacaoSemanalVolumeMedioCalcular || pblnCotacaoSemanalIFRMedioCalcular) {
				blnOkCotacaoSemanal = CotacaoSemanalDadosAtualizar(pblnCotacaoSemanalDadosCalcular, pblnCotacaoSemanalIFRCalcular, pblnCotacaoSemanalMMExpCalcular, pblnCotacaoSemanalVolumeMedioCalcular, pblnCotacaoSemanalIFRMedioCalcular, pdtmDataInicial, pstrAtivos, pblnConsiderarApenasDataSplit);

			}

			return blnOkCotacaoDiaria && blnOkCotacaoSemanal;

		}

		/// <summary>
		/// Calcula o número de registro que existe na tabela de cotações recebida por parâmetro para um determinado 
		/// ativo a partir de uma data inicial. Se existir uma cotação na data inicial esta também será incluída na contabilização.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pdtmDataInicial">data inicial do cálculo</param>
		/// <param name="pstrTabela">
		/// Cotacao ou Cotacao_Semanal
		/// </param>
		/// <param name="pobjConexaoAux">conexão auxiliar de banco de dados, caso não deva utilizar a conexão principal</param>
		/// <returns>O número de cotações existentes no período</returns>
		/// <remarks></remarks>
		private int NumCotacoesCalcular(string pstrCodigo, DateTime pdtmDataInicial, string pstrTabela, Conexao pobjConexaoAux = null)
		{
		    cRS objRs = pobjConexaoAux == null ? new cRS(objConexao) : new cRS(pobjConexaoAux);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//calcula o número de períodos em que há cotações para o papel no intervalo de datas recebido.
			objRs.ExecuteQuery(" Select count(1) as Contador " + " from " + pstrTabela + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial));

			int numeroDeCotacoes = Convert.ToInt32(objRs.Field("Contador", 0));

			objRs.Fechar();
			return numeroDeCotacoes;

		}

		public bool MediaAtualizar(string pstrCodigo, List<cMediaDTO> plstMediasSelecionadas, string pstrPeriodoDuracao)
		{

			string strTabelaMedia;
			string strTabelaCotacao;


			if (pstrPeriodoDuracao == "DIARIO") {
				strTabelaCotacao = "Cotacao";
				strTabelaMedia = "Media_Diaria";


			} else {
				strTabelaCotacao = "Cotacao_Semanal";
				strTabelaMedia = "Media_Semanal";

			}

			bool blnRetorno = true;

			cRS objRS = new cRS(objConexao);

		    //Dim objstructMediaEscolha As structIndicadorEscolha

			//Dim colMediaEscolhaAux As Collection = New Collection
			var lstMediasSelecionadasAux = new List<cMediaDTO>();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			foreach (cMediaDTO objMediaDTO in plstMediasSelecionadas) {
				//BUSCA A MAIOR DATA EM QUE JÁ EXISTE MÉDIA PARA O ATIVO RECEBIDO POR PARÂMETRO
				string strQuery = " select MAX(Data) as Data " + " FROM " + strTabelaMedia + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and NumPeriodos = " + objMediaDTO.NumPeriodos.ToString() + " and Tipo = " + FuncoesBd.CampoStringFormatar((objMediaDTO.Tipo == "E" ? "MME" : "MMA"));

				objRS.ExecuteQuery(strQuery);

				DateTime dtmDataInicial = Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida));

				objRS.Fechar();

				if (dtmDataInicial != Constantes.DataInvalida) {

					//se encontrou uma data, verifica se  tem cotação após esta data. 
					//Só precisa calcular a média, se possuir cotação
					objRS.ExecuteQuery(" select max(Data) as Data " + " from " + strTabelaCotacao + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data > " + FuncoesBd.CampoDateFormatar(dtmDataInicial));


					if (Convert.ToDateTime(objRS.Field("Data", Constantes.DataInvalida)) != Constantes.DataInvalida) {
						//colMediaEscolhaAux.Clear()
						lstMediasSelecionadasAux.Clear();

						//colMediaEscolhaAux.Add(objstructMediaEscolha)
						lstMediasSelecionadasAux.Add(objMediaDTO);

					    blnRetorno = MediaMovelGeralCalcular(pstrPeriodoDuracao, lstMediasSelecionadasAux, Convert.ToDateTime(objRS.Field("Data")), "#" + pstrCodigo + "#");

					}

					objRS.Fechar();


				} else {
					//colMediaEscolhaAux.Clear()
					lstMediasSelecionadasAux.Clear();

					//colMediaEscolhaAux.Add(objstructMediaEscolha)
					lstMediasSelecionadasAux.Add(objMediaDTO);

					//se não tem média calculada, tem que calcular para todo o período calculado
				    blnRetorno = MediaMovelGeralCalcular(pstrPeriodoDuracao, lstMediasSelecionadasAux,Constantes.DataInvalida , "#" + pstrCodigo + "#");

				}

			}

			return blnRetorno;

		}


		/// <summary>
		/// Calcula a média aritmética de uma cotação ou de um volume.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="pstrDado">
		/// VALOR = CALCULA A MÉDIA DE COTAÇÕES
		/// VOLUME = CALCULA A MÉDIA DE VOLUME
		/// </param>
		/// <param name="pstrPeriodoDuracao"></param>
		/// <param name="pintNumPeriodos"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <returns></returns>
		/// retornook = operação realizada com sucesso
		/// retornoerroinesperado = algum erro de banco de dados ou de programação.
		/// retornoerro2 = não existe o número de cotações suficientes para fazer o cálculo.
		/// <remarks></remarks>
		private cEnum.enumRetorno MediaMovelAritmeticaRetroativoUnitCalcular(string pstrCodigo, string pstrDado, string pstrPeriodoDuracao, int pintNumPeriodos, DateTime pdtmDataFinal)
		{
		    Conexao objConnAux = new Conexao();

			cCommand objCommand = new cCommand(objConnAux);

			cRS objRS = new cRS(objConnAux);

			string strQuery;

			DateTime dtmDataInicial = default(DateTime);

		    string strTabelaDados;
			//TABELA ONDE SERÃO BUSCADOS OS DADOS PARA CALCULAR A MÉDIA
			string strTabelaMedia;
			//TABELA ONDE O CÁLCULO DA MÉDIA SERÁ ARMAZENADO

			//INDICA O NÚMERO DE PERÍODOS QUE DEVE SER UTILIZADO NA BUSCA DOS DADOS, QUANDO OS DADOS FOREM O IFR, POR EXEMPLO,
			//QUE É UMA TABELA QUE PODE CONTER DADOS DE VÁRIOS PERÍODOS. 

			//INICIALIZA COM -1, QUE É O VALOR PADRÃO PARA NÃO SER CONSIDERADO
			int intNumPeriodosTabelaDados = -1;

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pstrPeriodoDuracao == "DIARIO") {
				if (pstrDado == "VALOR" || pstrDado == "VOLUME") {
					//SE A MÉDIA É CALCULADA SOBRE O VALOR OU SOBRE O VOLUME
					//BUSCA OS DADOS NA TABELA DE COTAÇÃO
					strTabelaDados = "Cotacao";
				} else {
					//SE A MÉDIA É CALCULADA SOBRE O IFR, BUSCA OS DADOS NA TABELA IFR_DIARIO
					strTabelaDados = "IFR_DIARIO";
					//POR ENQUANTO QUANDO A TABELA DE DADOS FOR O IFR, SEMPRE SERÁ O IFR DE 2 PERÍODOS.
					intNumPeriodosTabelaDados = 2;

				}

				strTabelaMedia = "Media_Diaria";


			} else {
				if (pstrDado == "VALOR" || pstrDado == "VOLUME") {
					//SE A MÉDIA É CALCULADA SOBRE O VALOR OU SOBRE O VOLUME
					//BUSCA OS DADOS NA TABELA DE COTAÇÃO
					strTabelaDados = "Cotacao_Semanal";

				} else {
					//SE A MÉDIA É CALCULADA SOBRE O IFR, BUSCA OS DADOS NA TABELA IFR_DIARIO
					strTabelaDados = "IFR_SEMANAL";

					//POR ENQUANTO QUANDO A TABELA DE DADOS FOR O IFR, SEMPRE SERÁ O IFR DE 2 PERÍODOS.
					intNumPeriodosTabelaDados = 2;

				}

				strTabelaMedia = "Media_Semanal";

			}

			string strMediaTipo;

			switch (pstrDado)
			{
			    case "VALOR":
			        strMediaTipo = "MMA";
			        break;
			    case "VOLUME":
			        strMediaTipo = "VMA";
			        break;
			    default:
			        strMediaTipo = "IFR2";
			        break;
			}

			objCommand.BeginTrans();

			if (pdtmDataFinal == Constantes.DataInvalida) {
				//se não recebeu uma data final, que é a data da primeira média que deve ser calculada, então tem
				//que calcular desde o inicio das cotações. Para descobrir o primeiro período chamada a função

				if (!NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true, ref dtmDataInicial,ref pdtmDataFinal, strTabelaDados, intNumPeriodosTabelaDados)) {
					//se não foi possível calcular o período, provavelmente porque não tem cotações
					//suficientes faz rollback.
					objCommand.RollBackTrans();

					return cEnum.enumRetorno.RetornoErro2;

				}


			} else {
				//se já recebeu a data final por parâmetro tem apenas que calcular a data inicial.
				//Para isso:
				//Buscar os registros anteriores à data final, considerando na busca a data final, 
				//ordenando por data decrescente crescente. Em cima destes dados, buscar 
				//os top intNumPeriodos registros e dentro destes top registros buscar 
				//o registro com menor data. Esta será a data inicial.

                var subQuery = " select top " + pintNumPeriodos + " Data " + 
                    "FROM " + strTabelaDados + 
                    " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);

				if (intNumPeriodosTabelaDados != -1) {

                    subQuery += " and NumPeriodos = " + intNumPeriodosTabelaDados;

				}

				subQuery += " order by Data desc " ;

                strQuery = "select min(Data) as DataInicial " + " from " + FuncoesBd.FormataSubSelect(subQuery);

				objRS.ExecuteQuery(strQuery);

				dtmDataInicial = Convert.ToDateTime(objRS.Field("DataInicial", Constantes.DataInvalida));

				objRS.Fechar();

				//verifica se o período entre as datas calculas é o mesmo recebido por parâmetro

				if (!IntervaloNumPeriodosVerificar(pstrCodigo, dtmDataInicial, pdtmDataFinal, pintNumPeriodos, strTabelaDados, intNumPeriodosTabelaDados, objConnAux)) {
					//se o intervalo calculado não contém o número de períodos necessários,
					//então tenta buscar um intervalo desde o início das cotações

					if (!NumPeriodosDataInicialCalcular(pstrCodigo, pintNumPeriodos, true,ref dtmDataInicial, ref pdtmDataFinal, strTabelaDados, intNumPeriodosTabelaDados)) {
						//se não foi possível calcular o período, provavelmente porque não tem cotações
						//suficientes faz rollback.
						objCommand.RollBackTrans();

						return cEnum.enumRetorno.RetornoErro2;

					}


				} /*else {
					//busca o último dia da semana da data final para caso haja split 
					//exatamente na semana da data final considerar até o último dia da semana.
					dtmDataFinalSplit = AtivoCotacaoSemanalUltimoDiaSemanaCalcular(pstrCodigo, pdtmDataFinal);

				}*/

			}

			//Criar dois recordsets.  O primeiro (RS1) deve buscar todas as datas de cotação com 
			//data maior ou igual à data inicial da primeira média, ordenados pela data.  
			//O segundo (RS2) deve buscar todas as datas de cotação com data maior ou igual à data final 
			//da primeira media, ordenados por data. 

			//Os dois recordsets devem andar juntos em um loop com a finalidade de informar 
			//a cada iteração a data inicial e a data final do cálculo da média.  Os recordsets 
			//devem ser percorridos até que o RS2 chegue ao final, pois este tem data maior 
			//e terminará primeiro. A cada iteração do recordset deve ser feita uma query 
			//que calcula a média entre a data inicial e a data final utilizando a função AVG, 
			//nativa do MS Access. Utilizar a função já existente na classe cCotacao: MMAritmeticaCalcular.  
			//Atualizar a média calculada na tabela de média, conforme calculado no item b, 
			//sempre utilizando como data da média a data do RS2.

			//se chegou até este ponto, então significa que conseguiu calcular as datas inicial e final.
			cRS objRsDataInicial = new cRS(objConnAux);
			cRS objRsDataFinal = new cRS(objConnAux);

		    //executa o recordset de datas iniciais

			strQuery = "select Data " + " from " + strTabelaDados + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(dtmDataInicial);


			if (intNumPeriodosTabelaDados != -1) {

				strQuery = strQuery + " and NumPeriodos = " + intNumPeriodosTabelaDados;

			}

			strQuery = strQuery + " order by Data ";

			objRsDataInicial.ExecuteQuery(strQuery);

		    var datasIniciais = new List<DateTime>() ;

            while (!objRsDataInicial.EOF)
            {
                datasIniciais.Add(Convert.ToDateTime(objRsDataInicial.Field("Data")));
                objRsDataInicial.MoveNext();
	        }

            objRsDataInicial.Fechar();

			//executa o recordset de datas iniciais
			strQuery = "select Data " + " from " + strTabelaDados + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);


			if (intNumPeriodosTabelaDados != -1) {

				strQuery = strQuery + " and NumPeriodos = " + intNumPeriodosTabelaDados;

			}

			strQuery = strQuery + " order by Data ";

			objRsDataFinal.ExecuteQuery(strQuery);

		    var datasFinais = new List<DateTime>();

		    while (!objRsDataFinal.EOF)
		    {
                datasFinais.Add(Convert.ToDateTime(objRsDataFinal.Field("Data")));
		        objRsDataFinal.MoveNext();
		    }
            objRsDataFinal.Fechar();

			//exclui as médias já existentes a partir da primeira data que será calculada,
			//caso exista
			objCommand.Execute(" DELETE " + " FROM " + strTabelaMedia + " where Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " and Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + " AND Tipo = " + FuncoesBd.CampoStringFormatar(strMediaTipo) + " AND NumPeriodos = " + pintNumPeriodos);

			for (int i= 0; i < datasFinais.Count; i++)
			{
			    var dataInicial = datasIniciais[i];
			    var dataFinal = datasFinais[i];
				//calcula a média entre os dois recordsets
				double dblMedia = MMAritmeticaCalcular(pstrCodigo, dataInicial, dataFinal, pintNumPeriodos, strTabelaDados, pstrDado, objConnAux);

				//atualiza a média no banco de dados
				MMAtualizar(pstrCodigo, dataFinal, pintNumPeriodos, strTabelaMedia, dblMedia, strMediaTipo, objConnAux);

			}

			objCommand.CommitTrans();

			objConnAux.FecharConexao();
            return objCommand.TransStatus ? cEnum.enumRetorno.RetornoOK : cEnum.enumRetorno.RetornoErroInesperado;

		}

		/// <summary>
		/// Busca as cotações direto do site da bovespa, baixa o xml para a pasta temporária,
		/// lê o xml e atualiza as cotações.
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool CotacaoIntraDayAtualizar(DateTime pdtmData, bool pblnCalcularDados)
		{
		    string strAtivos = String.Empty;

		    cCommand objCommand = new cCommand(objConexao);

			cRS objRS = new cRS(objConexao);

			IList<string> colAtivos = new List<string>();

			int intContador = 0;

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();
            
			objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data = " + FuncoesBd.CampoDateFormatar(pdtmData));

			if ((int) objRS.Field("Contador") > 0) {
				//blnCotacaoIntradayExistir = True

				DateTime[] arrData = { pdtmData };


				if (CotacaoExcluir(arrData, false) != cEnum.enumRetorno.RetornoOK) {
					objRS.Fechar();

                    MessageBox.Show("Erro ao excluir as cotações intraday.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					return false;

				}
			}

			objRS.Fechar();

			//inicializa os dados das cotações anteriores, pois serão utilizados depois para buscar o volume médio

			if (!CotacaoAnteriorInicializar("DIARIO", pdtmData)) {
                MessageBox.Show("Ocorreram erros ao calcular os dados das cotações anteriores.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;

			}

			objCommand.BeginTrans();

			//busca os ativos para os quais devem ser buscadas as cotações.
			//DEVE buscar todos os ativos que não foram desconsiderados.
			//Os ativos que ainda não foram cadastrados na tabela Ativo não serão considerados

			objRS.ExecuteQuery(" select Codigo " + " from Ativo " + " where Codigo not in " + "(" + " select Codigo " + " from Ativos_Desconsiderados" + ")");

			int intNumeroDeAtivosCotacaoIntraday = cBuscarConfiguracao.NumeroDeAtivosCotacaoIntraday();


			while (!objRS.EOF) {
				//Gera os códigos separados por "|"

				if (strAtivos != String.Empty) {
					//teste para gerar o pipe a partir do segundo e não gerar para o último.
					strAtivos = strAtivos + "|";

				}

				strAtivos = strAtivos + objRS.Field("Codigo");

				intContador = intContador + 1;


				if (intContador == intNumeroDeAtivosCotacaoIntraday) {
					colAtivos.Add(strAtivos);

					intContador = 0;

					strAtivos = String.Empty;

				}

				objRS.MoveNext();

			}


			if (strAtivos != String.Empty) {
				colAtivos.Add(strAtivos);

			}

			objRS.Fechar();

			cWeb objWebLocal = new cWeb(objConexao);

		    DateTime dtmData = pdtmData;
			//Dim strDataAux As String

		    double dblNegociosTotal = 0;
			double dblTitulosTotal = 0;
			double dblValorTotal = 0;

		    string strCaminhoPadrao = cBuscarConfiguracao.ObtemCaminhoPadrao();

		    foreach (string strAtivosLoop in colAtivos) {
			    string strUrl = "http://www.bmfbovespa.com.br/Pregao-Online/ExecutaAcaoAjax.asp?CodigoPapel=" + strAtivosLoop;


				if (objWebLocal.DownloadWithProxy(strUrl, strCaminhoPadrao + "temp", "cotacao.xml")) {
					var objArquivoXml = new cArquivoXML(strCaminhoPadrao + "temp\\cotacao.xml");


					if (!objArquivoXml.Abrir()) {
						objCommand.RollBackTrans();

						return false;

					}


					while ((!objArquivoXml.EOF()) && (objCommand.TransStatus)) {
						decimal decValorAbertura = Convert.ToDecimal(objArquivoXml.LerAtributo("Abertura", 0));


						if (decValorAbertura != 0) {
							var strCodigo = (string) objArquivoXml.LerAtributo("Codigo");

							//strDataAux = objArquivoXML.LerAtributo("Data")

							//dtmData = CDate(Left(strDataAux, 10))

							decimal decValorFechamento = Convert.ToDecimal(objArquivoXml.LerAtributo("Ultimo", 0));

							decimal decValorMinimo = Convert.ToDecimal(objArquivoXml.LerAtributo("Minimo", 0));

							decimal decValorMaximo = Convert.ToDecimal(objArquivoXml.LerAtributo("Maximo", 0));

							decimal decValorMedio = Convert.ToDecimal(objArquivoXml.LerAtributo("Medio", 0));

							decimal decOscilacao = Convert.ToDecimal(objArquivoXml.LerAtributo("Oscilacao", 0));

							//CONSULTA A COTAÇÃO ANTERIOR PARA O ATIVO
							DateTime dtmCotacaoAnteriorData = AtivoCotacaoAnteriorDataConsultar(strCodigo, pdtmData, "Cotacao");


							if (dtmCotacaoAnteriorData != Constantes.DataInvalida) {
								//QUANDO A COTAÇAO É INTRADAY, COMO NÃO TEMOS DADOS DE VOLUME,
								//UTILIZA OS DADOS DA COTAÇÃO ANTERIOR.
								objRS.ExecuteQuery("SELECT Titulos_Total, Negocios_Total, Valor_Total " + " FROM Cotacao " + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigo) + " AND Data = " + FuncoesBd.CampoDateFormatar(dtmCotacaoAnteriorData));

								dblNegociosTotal = Convert.ToDouble(objRS.Field("Negocios_Total"));

								dblTitulosTotal = Convert.ToDouble(objRS.Field("Titulos_Total"));

								dblValorTotal = Convert.ToDouble(objRS.Field("Valor_Total"));

								objRS.Fechar();

							}

						    //calcula o sequencial do ativo
							long lngSequencial = SequencialCalcular(strCodigo, "Cotacao", objCommand.Conexao);

							string strQuery = " insert into Cotacao " + "(Codigo, Data, ValorAbertura, ValorFechamento " + ", ValorMinimo, ValorMedio, ValorMaximo, Oscilacao " + ", Titulos_Total, Negocios_Total, Valor_Total, Sequencial) " + " values " + "(" + FuncoesBd.CampoStringFormatar(strCodigo) + "," + FuncoesBd.CampoDateFormatar(dtmData) + "," + FuncoesBd.CampoDecimalFormatar(decValorAbertura) + "," + FuncoesBd.CampoDecimalFormatar(decValorFechamento) + "," + FuncoesBd.CampoDecimalFormatar(decValorMinimo) + "," + FuncoesBd.CampoDecimalFormatar(decValorMedio) + "," + FuncoesBd.CampoDecimalFormatar(decValorMaximo) + "," + FuncoesBd.CampoDecimalFormatar(decOscilacao) + "," + FuncoesBd.CampoFloatFormatar(dblTitulosTotal) + "," + FuncoesBd.CampoFloatFormatar(dblNegociosTotal) + "," + FuncoesBd.CampoFloatFormatar(dblValorTotal) + "," + lngSequencial.ToString() + ")";

							objCommand.Execute(strQuery);

						}

						objArquivoXml.LerNodo();

					}

					objArquivoXml.Fechar();


				} else {
					objCommand.RollBackTrans();

				}

			}

			//inserir registro na tabela COTACAO_INTRADAY, para saber que esta cotação
			//foi salva com dados intraday e não com dados oficiais de fechamento

			//primeiro verifica se já existe registro na tabela, porque esta cotação pode ser baixada
			//várias vezes durante o dia.

			//ALTERADO EM 12/03/2010 PARA SEMPRE INSERIR O REGISTRO NA TABELA COTACAO_INTRADAY
			//POIS O REGISTRO É EXCLUIDO PELA FUNÇÃO COTACAOEXCLUIR QUE É CHAMADA ANTERIORMENTE
			//NESTA FUNÇÃO.

			//If Not blnCotacaoIntradayExistir Then

			objCommand.Execute(" INSERT INTO Cotacao_Intraday " + " (Data)" + " VALUES " + "(" + FuncoesBd.CampoDateFormatar(dtmData) + ")");

			//End If

			objCommand.CommitTrans();


			if (objCommand.TransStatus) {
				if (pblnCalcularDados) {
					DadosRecalcular(true, false, true, true, true, true, true, true, true, true,
					true, dtmData);
				}


			    MessageBox.Show("Atualização das cotações realizada com sucesso.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Information);


			} else {
                MessageBox.Show("Ocorreram erros ao executar a operação.", "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

			return objCommand.TransStatus;

		}

	    /// <summary>
	    /// Exclui as cotações das datas recebidas por parâmetros e recalcula os dados semanais se
	    /// for necessário
	    /// </summary>
	    /// <param name="parrData">Array contendo a data das cotações que devem ser excluídas.
	    /// O array está ordenado em ordem crescente.</param>
	    /// <param name="pblnDadosSemanaisRecalcular"></param>
	    /// <returns>
	    /// RetornoOK = operação executada com sucesso.
	    /// RetornoErroInesperado = algum erro de banco de dados ou de programação.
	    /// RetornoErro2 = existem cotações posteriores às datas recebidas por parâmetro.
	    /// </returns>
	    /// <remarks></remarks>
	    public cEnum.enumRetorno CotacaoExcluir(DateTime[] parrData, bool pblnDadosSemanaisRecalcular)
		{
			cEnum.enumRetorno functionReturnValue;

			cCommand objCommand = new cCommand(objConexao);

			cRS objRS = new cRS(objConexao);

		    string strAtivos = String.Empty;

			objCommand.BeginTrans();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//consistência para evitar buracos nas cotações.
			//Para isso verificar se existe alguma cotação maior do que a primeira do array
			//e diferente de todas as outras do array.

			string strQuery = " SELECT COUNT(1) AS Contador " + " FROM Cotacao " + " WHERE Data > " + FuncoesBd.CampoDateFormatar(parrData[0]);


			for (int intI = 1; intI <= parrData.Length - 1; intI++) {
				strQuery = strQuery + " AND Data <> " + FuncoesBd.CampoDateFormatar(parrData[intI]);

			}

			objRS.ExecuteQuery(strQuery);


			if (Convert.ToInt32(objRS.Field("Contador")) > 0) {
				//se existem alguma data intermediária não pode deixar buraco.
				objRS.Fechar();

				objCommand.RollBackTrans();

				return cEnum.enumRetorno.RetornoErro2;

			}

			objRS.Fechar();

			//COTACAO
			objCommand.Execute(" DELETE " + " FROM Cotacao " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

			//MEDIA_DIARIA

			objCommand.Execute(" DELETE " + " FROM Media_Diaria " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

			//IFR_DIARIO
			objCommand.Execute(" DELETE " + " FROM IFR_Diario " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));


			if (pblnDadosSemanaisRecalcular) {
				//DAS COTAÇÕES SEMANAIS DEVE EXCLUIR OS REGISTROS ONDE A PRIMEIRA DATA DA SEMANA
				//É MAIOR OU IGUAL A PRIMEIRA DATA DO ARRAY

				//COTACAO_SEMANAL
				objCommand.Execute(" DELETE " + " FROM Cotacao_Semanal " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

				//MEDIA_SEMANAL
				objCommand.Execute(" DELETE " + " FROM Media_Semanal " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

				//IFR_SEMANAL
				objCommand.Execute(" DELETE " + " FROM IFR_Semanal " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

				//verifica se existem cotações intraday para todos os ativos ou somente para ativos específicos
				objRS.ExecuteQuery(" SELECT COUNT(1) AS Contador " + " FROM Cotacao_Intraday " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));


				if (Convert.ToInt32(objRS.Field("Contador")) == 0) {
					objRS.Fechar();

					//se não tem cotações intraday para todos os ativos
					//lista apenas os ativos que tiveram cotação intraday para 
					//fazer o recálculo dos dados semanais.
					objRS.ExecuteQuery(" SELECT Codigo " + " FROM Cotacao_Intraday_Ativo " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]) + " GROUP BY Codigo");


					strAtivos = "#";


					while (!objRS.EOF) {
						strAtivos = strAtivos + objRS.Field("Codigo") + "#";

						objRS.MoveNext();

					}

				}

				objRS.Fechar();

			}

			//EXCLUI REGISTROS DA TABELA COTACAO_INTRADAY
			objCommand.Execute(" DELETE " + " FROM Cotacao_Intraday " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

			//EXCLUI REGISTROS DA TABELA COTACAO_INTRADAY_ATIVO
			objCommand.Execute(" DELETE " + " FROM Cotacao_Intraday_Ativo " + " WHERE Data >= " + FuncoesBd.CampoDateFormatar(parrData[0]));

			objCommand.CommitTrans();


			if (objCommand.TransStatus) {

				if (pblnDadosSemanaisRecalcular) {
					//CHAMA A FUNÇÃO DE RECÁLCULO PARA OS DADOS SEMANAIS.
					//SE NÃO HOUVER REGISTROS NÃO CALCULARÁ
					CotacaoSemanalDadosAtualizar(true, true, true, true, true, parrData[0], strAtivos);

				}

				functionReturnValue = cEnum.enumRetorno.RetornoOK;


			} else {
				functionReturnValue = cEnum.enumRetorno.RetornoErroInesperado;

			}
			return functionReturnValue;

		}

		private bool CotacaoAnteriorInicializar(string pstrPeriodo, DateTime pdtmDataInicial, string pstrAtivos = "")
		{

			Conexao objConexaoAux = new Conexao();

			cCommand objCommand = new cCommand(objConexaoAux);

			cRS objRS = new cRS(objConexaoAux);

			cRS objRsSemanal = new cRS(objConexaoAux);

		    //indica o nome da tabela de cotações, de acordo com a duração do período das cotações

		    string strTabela = pstrPeriodo == "DIARIO" ? "Cotacao" : "Cotacao_Semanal";

			string strWhere = String.Empty;

			objCommand.BeginTrans();

			//exclui todos os registros da tabela de cotação antes de inserir os novos
			//a decisão de excluir todos e não apenas os registros da data "pdtmDataInicial"
			//é para a tabela não ficar muito populada, pois o objetivo desta tabela é consultar
			//rapidamente a data anterior
			objCommand.Execute("DELETE " + " FROM Cotacao_Anterior ");

			//busca todos os ativos do período e a menor data para ser utilizada como data base.
			string strQuery = " select Codigo, max(Data) as DataAnterior " + " from " + strTabela;

			if (!string.IsNullOrEmpty(strWhere))
				strWhere = strWhere + " and ";

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pstrPeriodo == "DIARIO") {
				//se passou uma data inicial busca as cotações a partir de uma data.
				strWhere = strWhere + " Data < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);


			} else {
				//se é uma cotação semanal, a data recebida por parâmetro tem que ser menor do que 
				//a data inicial e a data final da semana
				strWhere = strWhere + " Data < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " and DataFinal < " + FuncoesBd.CampoDateFormatar(pdtmDataInicial);

			}


			if (pstrAtivos != String.Empty) {
				if (!string.IsNullOrEmpty(strWhere))
					strWhere = strWhere + " And ";

			    var sustenidoFormatado = FuncoesBd.CampoStringFormatar("#");
			    strWhere +=
			        FuncoesBd.IndiceDaSubString(
			            FuncoesBd.ConcatenarStrings(new[] {sustenidoFormatado, "Codigo", sustenidoFormatado}),
			            FuncoesBd.CampoStringFormatar(pstrAtivos)) + "  > 0 ";

			}

			strQuery = strQuery + " WHERE " + strWhere;

			strQuery = strQuery + " GROUP BY Codigo ";

			objRS.ExecuteQuery(strQuery);

		    var cotacoesAnteriores = new List<CotacaoDataDto>();

		    while (!objRS.EOF)
		    {
                cotacoesAnteriores.Add(new CotacaoDataDto
                {
                    Codigo = Convert.ToString(objRS.Field("Codigo")),
                    Data = Convert.ToDateTime(objRS.Field("DataAnterior")) 
                });
                objRS.MoveNext();
		    }


            objRS.Fechar();

		    foreach (var cotacaoAnteriorDto in cotacoesAnteriores)
		    {
			    DateTime dtmData;
			    if (pstrPeriodo == "DIARIO") {
					dtmData = pdtmDataInicial;

				} else {
					//busca da primeira data da semana, pois pdtmDataInicial recebido por parâmetro pode 
					//uma data de meio de semana
				    objRsSemanal.ExecuteQuery(" SELECT Data " + "FROM Cotacao_Semanal " + "WHERE Codigo = " +
				                              FuncoesBd.CampoStringFormatar(cotacaoAnteriorDto.Codigo) + " AND Data <= " +
				                              FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " AND DataFinal >= " +
				                              FuncoesBd.CampoDateFormatar(pdtmDataInicial));

					dtmData = Convert.ToDateTime(objRsSemanal.Field("Data",Constantes.DataInvalida));

					objRsSemanal.Fechar();

				}

				//Para cada ativo do loop, faz a inserção na tabela Cotacao_Anterior
		        objCommand.Execute("INSERT INTO Cotacao_Anterior " + "(Codigo, Data, Periodo, Data_Anterior) " +
		                           "VALUES " +
		                           "(" + FuncoesBd.CampoStringFormatar(cotacaoAnteriorDto.Codigo) + ", " +
		                           FuncoesBd.CampoDateFormatar(dtmData) + ", " + FuncoesBd.CampoStringFormatar(pstrPeriodo) +
		                           ", " + FuncoesBd.CampoDateFormatar(cotacaoAnteriorDto.Data) + ")");

		    }

			objCommand.CommitTrans();

			objConexaoAux.FecharConexao();

		    return objCommand.TransStatus;

		}


		/// <summary>
		/// Preenche o campo SEQUENCIAL para um determinado ativo em uma determinada tabela
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
		/// <returns>status da transação</returns>
		/// <remarks></remarks>
		private bool SequencialAtivoPreencher(string pstrCodigo, string pstrTabela)
		{

			cCommand objCommand = new cCommand(objConexao);
			cRS objRS = new cRS(objConexao);
			objCommand.BeginTrans();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//BUSCA AS COTAÇÕES ORDENADAS POR DATA
			objRS.ExecuteQuery(" SELECT Data " + " FROM " + pstrTabela + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " ORDER BY Data ");

			//INICIALIZA O SEQUENCIAL EM 1
			long lngSequencial = 1;

            //PARA CADA DATA ONDE HÁ COTACAO  
			while (!objRS.EOF) {
				//ATUALIZA O SEQUENCIAL
				objCommand.Execute(" UPDATE " + pstrTabela + " SET " + "Sequencial = " + lngSequencial.ToString() + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " AND Data = " + FuncoesBd.CampoDateFormatar(Convert.ToDateTime(objRS.Field("Data"))));

				//INCREMENTA PARA A PRÓXIMA ITERAÇÃO
				lngSequencial = lngSequencial + 1;

				objRS.MoveNext();

			}

			objRS.Fechar();

			objCommand.CommitTrans();

			return objCommand.TransStatus;

		}

		private string SequencialQueryDivergenciaGerar(string pstrTabelaCotacao)
		{

			return " SELECT Codigo " + Environment.NewLine + " FROM Ativo A " + Environment.NewLine + " WHERE " + Environment.NewLine + "(" + Environment.NewLine + '\t' + "SELECT Max(sequencial) " + Environment.NewLine + '\t' + " FROM " + pstrTabelaCotacao + " C " + Environment.NewLine + '\t' + " WHERE A.Codigo = C.Codigo " + Environment.NewLine + ") <> " + "(" + Environment.NewLine + '\t' + " SELECT Count(1) " + Environment.NewLine + '\t' + " FROM " + pstrTabelaCotacao + " C " + Environment.NewLine + '\t' + " WHERE A.Codigo = C.Codigo" + Environment.NewLine + ")";

		}

		private bool SequencialPeriodicidadePreencher(string pstrPeriodicidade)
		{

			cRS objRS = new cRS(objConexao);

			bool blnOK = true;

		    string strTabelaCotacao;

			switch (pstrPeriodicidade)
			{
			    case "DIARIO":
			        strTabelaCotacao = "Cotacao";
			        break;
			    case "SEMANAL":
			        strTabelaCotacao = "Cotacao_Semanal";
			        break;
			    default:
			        strTabelaCotacao = String.Empty;
			        break;
			}

			string strQuery = SequencialQueryDivergenciaGerar(strTabelaCotacao);

			//BUSCA TODOS OS ATIVOS
			objRS.ExecuteQuery(strQuery);

			//PARA CADA ATIVO...

			while ((!objRS.EOF) && blnOK) {
				//CHAMA FUNÇÃO PARA ATUALIZAR SEQUENCIAL NAS COTAÇÕES DIÁRIAS

				if (!SequencialAtivoPreencher((string) objRS.Field("Codigo"), strTabelaCotacao)) {
					blnOK = false;

				}

				objRS.MoveNext();

			}

			objRS.Fechar();

			return blnOK;

		}

		/// <summary>
		/// Preenche o campo sequencial para todos os ativos nas tabelas COTACAO e COTACAO_SEMANAL.
		/// Serão considerados apenas os ativos cadastrados na tabeal ATIVO.
		/// </summary>
		/// <returns>STATUS DA TRANSAÇÃO</returns>
		/// <remarks></remarks>
		public bool SequencialPreencher()
		{
		    bool blnOk = SequencialPeriodicidadePreencher("DIARIO");

			if (blnOk) {
				blnOk = SequencialPeriodicidadePreencher("SEMANAL");

			}

			return blnOk;

		}

		/// <summary>
		/// Calcula o valor máximo da coluna sequencial para um determinado ativo em uma determinada periodicidade 
		/// (DIÁRIO ou SEMANAL).
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
		/// <param name="pobjConexao"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public long AtivoSequencialMaximoBuscar(string pstrCodigo, string pstrTabela, Conexao pobjConexao = null)
		{

			cRS objRs = pobjConexao == null ? new cRS(objConexao) : new cRS(pobjConexao);

			//busca o maior sequencial utilizado
			objRs.ExecuteQuery(" SELECT MAX(Sequencial) AS Sequencial " + " FROM " + pstrTabela + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo));

			long functionReturnValue = Convert.ToInt64(objRs.Field("Sequencial", 0));

			objRs.Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula o novo sequencial para um determinado ativo, de acordo com o último sequencial utilizado
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pstrTabela">Tabela de cotações. Valores possíveis: COTACAO, COTACAO_SEMANAL</param>
		/// <param name="pobjConexao">Conexão para fazer a consulta. Obrigado a receber a mesma conexão
		/// do objCommand que controla a transação da função principal que chamada esta,
		///  pois muitas vezes é feito delete dos dados e para não deixarmos espaços em branco 
		/// no sequencial precisamos saber que os dados já foram excluídos e podemos utilizar 
		/// um número que foi utilizado antes, mas foi excluído. Conseguimos fazer isso apenas utilizando 
		/// a mesma conexão</param>
		/// <returns></returns>
		/// <remarks></remarks>
		private long SequencialCalcular(string pstrCodigo, string pstrTabela, Conexao pobjConexao)
		{

			//incrementa 1 no último sequencial utiliazado.
			//retorno erro é 0, caso o ativo ainda não tenha registro inserido (primeiro dia de negociação)
			return AtivoSequencialMaximoBuscar(pstrCodigo, pstrTabela, pobjConexao) + 1;

		}

		/// <summary>
		/// consulta o valor de fechamento da última cotação de um ativo na periodicidade diária
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <returns>O valor de fechamento do ativo na última cotação</returns>
		/// <remarks></remarks>
		public decimal CotacaoUltimaValorFechamentoConsultar(string pstrCodigo)
		{
		    cRS objRS = new cRS(objConexao);

			objRS.ExecuteQuery("SELECT TOP 1 ValorFechamento " + "FROM Cotacao " + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + " ORDER BY Sequencial DESC");

			decimal valorFechamento = Convert.ToDecimal(objRS.Field("ValorFechamento"));

			objRS.Fechar();
			return valorFechamento;

		}


	    /// <summary>
	    /// 'Atualiza o registro da tabela Resumo. Esta tabela contém apenas sempre 1 registro.
	    /// </summary>
	    /// <param name="pdtmDataUltimaCotacao">Data em que foi buscada a última cotação das ações.
	    ///     Não devem ser consideradas as cotações intraday </param>
	    /// <param name="pdtmDataUltimoProvento">Data em que foi atualizado o último arquivo de proventos</param>
	    /// <returns></returns>
	    /// <remarks></remarks>
	    private void TabelaResumoAtualizar(DateTime pdtmDataUltimaCotacao, DateTime pdtmDataUltimoProvento)
		{

			cCommand objCommand = new cCommand(objConexao);

			string strQuery = String.Empty;

		    bool blnTransacaoInterna = !objConexao.TransAberta;

			if (blnTransacaoInterna)
				objCommand.BeginTrans();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (pdtmDataUltimaCotacao != Constantes.DataInvalida) {
				strQuery = "Data_Ultima_Cotacao = " + FuncoesBd.CampoDateFormatar(pdtmDataUltimaCotacao) + Environment.NewLine;

			}


			if (pdtmDataUltimoProvento != Constantes.DataInvalida) {

				if (strQuery != String.Empty) {
					strQuery = strQuery + ", ";

				}

				strQuery = strQuery + "Data_Ultimo_Provento = " + FuncoesBd.CampoDateFormatar(pdtmDataUltimoProvento) + Environment.NewLine;

			}

			strQuery = "UPDATE Resumo SET " + Environment.NewLine + strQuery;

			//quando estiver atualizando a data da última cotaçao atualiza somente se a data for maior do que a data
			//já existente na tabela

			if (pdtmDataUltimaCotacao != Constantes.DataInvalida) {
				strQuery = strQuery + Environment.NewLine + " WHERE Data_Ultima_Cotacao < " + FuncoesBd.CampoDateFormatar(pdtmDataUltimaCotacao);

			}

			objCommand.Execute(strQuery);

			if (blnTransacaoInterna)
				objCommand.CommitTrans();

		}



		/// <summary>
		/// Busca os registros de proventos (dividendos e juros) na tabela de Proventos e atualiza na tabela Split
		/// </summary>
		/// <param name="pdtmDataFinal">última data para a busca dos proventos</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool ProventoAtualizar(DateTime pdtmDataFinal)
		{
			cCommand objCommand = new cCommand(objConexao);

			cRS objRS = new cRS(objConexao);

			cRS objRSAtivo = new cRS(objConexao);

		    string strWhere = String.Empty;

		    //Dim dtmData_Ultimo_Provento As Date

			string strCodigoAtual = String.Empty;
			DateTime dtmDataInicioRecalculo = Constantes.DataInvalida;

			//código no sistema de cotações equivalente ao conjunto Nome_Pregao + Tipo_Acao
			//Dim strCodigoProvento As String

			string strNomePregaoAtual = String.Empty;
			string strTipoAcaoAtual = String.Empty;

			string strNomePregaoNaoEncontrado = String.Empty;
			string strTipoAcaoNaoEncontrado = String.Empty;

			string strUltimoNomePregao = String.Empty;
			string strUltimoTipoAcao = String.Empty;

		    //indica se o provento foi importado, ou seja, foi encontrado o ativo na tabela de ativos 
			//para importá-lo.
			bool blnImportado = false;

		    //usado para o cálculo da primeira dat ex-provento

		    //contém a data da primeira cotação para o papel

		    string strTipoProvento = null;

			var objCalculadorData = new cCalculadorData(objConexao);

			//prepara os dados da tabela temporária
			objCommand.BeginTrans();

			//Exclui os registros em branco do final da planilha do Excel que também são importados
			objCommand.Execute("DELETE " + Environment.NewLine + " FROM Proventos_Temp " + Environment.NewLine + " WHERE Nome_Pregao Is Null");

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//Atualiza campo Data_Aprovacao para uma formato de data. O campo é do tipo string na tabela
			//Temporária e na hora de importar fica com um formato numérico contando os dias que se passaram
			//desde 30/12/1899. Não faz isso se o campo já possuir 10 caracteres, pois o formato dd/MM/yyyy ocupa 
			//10 caracteres e se já for encontrado o separados das datas ("/") também.

			objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine + " DATA_APROVACAO = " + FuncoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_APROVACAO) " + Environment.NewLine + " WHERE DATA_APROVACAO <> " + FuncoesBd.CampoStringFormatar("ESTATUTÁRIO") + Environment.NewLine + " And len(DATA_APROVACAO) <> 10 " + Environment.NewLine + " And instr(DATA_APROVACAO, '/') = 0 ");


			objCommand.Execute("UPDATE PROVENTOS_TEMP SET " + Environment.NewLine + "DATA_ULTIMO_PRECO_COM = " + FuncoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(DATA_ULTIMO_PRECO_COM) " + Environment.NewLine + "WHERE DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar("PREÇO TEÓRICO") + Environment.NewLine + " And len(DATA_ULTIMO_PRECO_COM) <> 10 " + Environment.NewLine + " And DATA_ULTIMO_PRECO_COM <> " + FuncoesBd.CampoStringFormatar(" ") + Environment.NewLine + " And instr(DATA_ULTIMO_PRECO_COM, '/') = 0 ");

			objCommand.Execute(" UPDATE PROVENTOS_TEMP SET " + " ULTIMO_DIA_COM = " + FuncoesBd.CampoDateFormatar(new DateTime(1899, 12, 30)) + " + cDate(ULTIMO_DIA_COM) " + Environment.NewLine + " WHERE(Len(ULTIMO_DIA_COM) <> 10) " + " And instr(ULTIMO_DIA_COM,'/') = 0 ");

			objCommand.CommitTrans();


			if (!objCommand.TransStatus) {
				return false;

			}

			//consulta a data da primeira cotação e do último provento na tabela Resumo
			objRS.ExecuteQuery("SELECT Data_Primeira_Cotacao " + Environment.NewLine + "FROM Resumo ");

			DateTime dtmData_Primeira_Cotacao = Convert.ToDateTime(objRS.Field("Data_Primeira_Cotacao", Constantes.DataInvalida));
			//dtmData_Ultimo_Provento = CDate(objRS.Field("Data_Ultimo_Provento", DataInvalida))

			objRS.Fechar();

			//busca todos os proventos da tabela de proventos após a data do último provento, 
			//caso a mesma esteja preenchida.

			string strQuery = "SELECT Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + ", SUM(Valor_Provento) AS Valor_Provento " + "FROM Proventos_Temp PT " + Environment.NewLine + " WHERE ";


			strWhere = "CDBL(Ultimo_Preco_com) > 0 " + Environment.NewLine;


			if (dtmData_Primeira_Cotacao != Constantes.DataInvalida) {
				//busca os proventos a partir da data da primeira cotação.
				strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) >= " + FuncoesBd.CampoDateFormatar(dtmData_Primeira_Cotacao) + Environment.NewLine;

			}


			if (pdtmDataFinal != Constantes.DataInvalida) {
				//a data final é a maior data para qual deve ser buscada os proventos.
				//geralmente é utilizada somente quando é necessário atualizar proventos 
				//anteriores aos proventos já cadastrados.
				strWhere = strWhere + " AND CDATE(Ultimo_Dia_Com) <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal) + Environment.NewLine;

			}

			//WHERE que verifica se o registro já foi processado.
			strWhere = strWhere + " AND NOT EXISTS ( " + Environment.NewLine + " SELECT 1 " + Environment.NewLine + " FROM Proventos P " + Environment.NewLine + " WHERE PT.Nome_Pregao = P.Nome_Pregao " + Environment.NewLine + " AND PT.Tipo_Acao = P.Tipo_Acao " + Environment.NewLine + " AND PT.Tipo_Provento = P.Tipo_Provento " + Environment.NewLine + " AND PT.Ultimo_Dia_Com = P.Ultimo_Dia_Com " + Environment.NewLine + ")";

			strQuery = strQuery + strWhere + " GROUP BY Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com " + ", Ultimo_Preco_Com" + Environment.NewLine + " ORDER BY Nome_Pregao, Tipo_Acao, CDATE(Ultimo_Dia_Com)";

			objRS.ExecuteQuery(strQuery);

		    string conteudoDoArquivoDeLog = "";

			//No loop a seguir podem existir vários registros seguidos para a mesma ação
			//(Nome_Pregao + Tipo_Acao). Por isso a cada iteração tem que verificar se o ativo mudou
			while ((!objRS.EOF) && (objCommand.TransStatus))
			{

			    var strNomePregao = (string) objRS.Field("Nome_Pregao");
			    var strTipoAcao = (string) objRS.Field("Tipo_Acao");
				if (strNomePregaoNaoEncontrado != strNomePregao || strTipoAcaoNaoEncontrado != strTipoAcao) {
					//se a ação não está marcada como não encontrada na tabela de ativos prossegue.
					//Caso contrário o programa vai para o próximo registro pois não entrará neste IF.


				    if (strNomePregaoAtual != strNomePregao || strTipoAcaoAtual != strTipoAcao) {
						//inicia transação para trabalhar com os dados deste ativo.
						objCommand.BeginTrans();

						//inicializa o campo de data de início do recálculo
						dtmDataInicioRecalculo = Constantes.DataInvalida;

						//se é um ativo novo comparado com o ativo anterior
						objRSAtivo.ExecuteQuery("SELECT Codigo " + Environment.NewLine + " FROM Ativo " + Environment.NewLine + " WHERE Descricao = " + FuncoesBd.CampoStringFormatar(objRS.Field("Nome_Pregao") + " " + objRS.Field("Tipo_Acao")) + Environment.NewLine + " OR Descricao = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Nome_Pregao") + (string) objRS.Field("Tipo_Acao")));


						if (objRSAtivo.DadosExistir) {
							//ativo encontrado
							strCodigoAtual = (string) objRSAtivo.Field("Codigo");

							strNomePregaoAtual = (string) objRS.Field("Nome_Pregao");
							strTipoAcaoAtual = (string) objRS.Field("Tipo_Acao");



						} else {
							//seta o código para vazio para neste caso não fazer insert na tabela de split
							strCodigoAtual = String.Empty;

							strNomePregaoNaoEncontrado = (string) objRS.Field("Nome_Pregao");
							strTipoAcaoNaoEncontrado = (string) objRS.Field("Tipo_Acao");

							//neste caso com certeza não vai importar os proventos.
							blnImportado = false;

							//lança log informando que não encontrou o ativo.
                            conteudoDoArquivoDeLog += "Ativo não encontrado - Nome: " + strNomePregaoNaoEncontrado + " - Tipo de ação: " + strTipoAcaoNaoEncontrado;

						}

						objRSAtivo.Fechar();

						strUltimoNomePregao = (string) objRS.Field("Nome_Pregao");
						strUltimoTipoAcao = (string) objRS.Field("Tipo_Acao");

					}
					//fim do if que testa se é um código novo de ativo.


					if (strCodigoAtual != String.Empty) {

                        //busca a data da primeira cotação para o papel
                        DateTime dtmDataPrimeiraCotacao = AtivoPrimeiraCotacaoDataConsultar(strCodigoAtual);

						if (Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao) {

							if (dtmDataInicioRecalculo == Constantes.DataInvalida) {
								dtmDataInicioRecalculo = Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com"));

							}

							//calcula a data ex-provento, que é a próxima data em que houver cotação para o ativo
							//após o "último dia com"
							DateTime dtmDataExProvento = AtivoCotacaoPosteriorDataConsultar(strCodigoAtual, Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));


							if (dtmDataExProvento == Constantes.DataInvalida) {
                                conteudoDoArquivoDeLog += "Não foram encontradas cotações após o dia ex-provento." + "Verificar a data cadastrada na tabela Split - Código: " + strCodigoAtual + " - Data ex-provento: " + Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")).ToString("dd/MM/yyyy");

								//se não encontrou a data da próxima cotação é porque a atualização de proventos 
								//veio no dia da última cotação. Neste caso busca o próximo dia útil como sendo data-ex.
								//Tem que cuida para caso haja algum feriado ainda não cadastrado na tabela "Feriado"
								//que pode fazer com que a data do próximo dia útil fique calculada errada e por 
								//consequencia também a data do último dia com provento.
								//Para alertar o usuário lança log informando que não encontrou cotação após o
								//dia ex-dividendo.
								dtmDataExProvento = objCalculadorData.DiaUtilSeguinteCalcular(Convert.ToDateTime(objRS.Field("Ultimo_Dia_Com")));

							}

						    var strTipoDeProventoPorExtenso = (string) objRS.Field("Tipo_Provento");

                            if (strTipoDeProventoPorExtenso == "DIVIDENDO")
                            {
								strTipoProvento = "DIV";
                            }
                            else if ((strTipoDeProventoPorExtenso == "JRS CAP PRÓPRIO") || (strTipoDeProventoPorExtenso == "JRS CAP PROPRIO"))
                            {
								strTipoProvento = "JCP";
                            }
                            else if (strTipoDeProventoPorExtenso == "RENDIMENTO")
                            {
								strTipoProvento = "REND";
                            }
                            else if (strTipoDeProventoPorExtenso == "REST CAP DIN")
                            {
								strTipoProvento = "RCDIN";
							} else {
								strTipoProvento = String.Empty;

							}

							//Verifica se o provento já está cadastrado
							objRSAtivo.ExecuteQuery("SELECT COUNT(1) AS Contador " + Environment.NewLine + " FROM Split " + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigoAtual) + Environment.NewLine + " AND Data = " + FuncoesBd.CampoDateFormatar(dtmDataExProvento) + Environment.NewLine + " AND Tipo = " + FuncoesBd.CampoStringFormatar(strTipoProvento));


							if (Convert.ToInt32(objRSAtivo.Field("Contador")) == 0) {
								//Se o provento ainda não está cadastrado, cadastra-o.
								strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strCodigoAtual) + ", " + FuncoesBd.CampoDateFormatar(dtmDataExProvento) + ", " + FuncoesBd.CampoStringFormatar(strTipoProvento);

								double dblQuantidadeAnterior = Convert.ToDouble(objRS.Field("Ultimo_Preco_Com")) - Convert.ToDouble(objRS.Field("Valor_Provento"));

								strQuery = strQuery + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))) + ")";

								objCommand.Execute(strQuery);

								//marca que o ativo será importado para depois carregar da tabela
								//Proventos_Temp para a tabela Proventos
								blnImportado = true;


							} else {
								//lança log informando que já existe provento cadastrado
                                conteudoDoArquivoDeLog += "Já existe provento cadastrado - Codigo: " + strCodigoAtual + " - Data: " + dtmDataExProvento.ToString("dd/MM/yyyy") + " - Tipo de Provento: " + objRS.Field("Tipo_Provento");

								blnImportado = false;

							}


						} else {
							//Se a data do provento é anterior à data da primeira cotação do ativo não importa
							blnImportado = false;

						}
						//If CDate(objRS.Field("Ultimo_Dia_Com")) >= dtmDataPrimeiraCotacao Then

						//copia os dados do provento da tabela temporária para a tabela definitiva.
						//copia somente para os registros que estão sendo processados no momento utilizando
						//como campos chaves: Nome_Pregao, Tipo_Acao, Tipo_Provento, Ultimo_Dia_Com, Ultimo_Preco_Com
						objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco" + ", " + (blnImportado ? "1" : "0") + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine + " AND Tipo_Provento = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Tipo_Provento")) + Environment.NewLine + " AND Ultimo_Dia_Com = " + FuncoesBd.CampoStringFormatar((string) objRS.Field("Ultimo_Dia_Com")) + " AND Ultimo_Preco_Com = " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(objRS.Field("Ultimo_Preco_Com"))));

					}
					//If strCodigoAtual <> vbNullString Then

				}
				//fim do if que testa se é um código não encontrado

				objRS.MoveNext();

			    bool blnExecutar;
			    if (objRS.EOF) {
					//assinala que tem que executar para que o último registro não fique sem execução
					blnExecutar = true;

				} else {
					if (strUltimoNomePregao != (string) objRS.Field("Nome_Pregao") || strUltimoTipoAcao != (string) objRS.Field("Tipo_Acao")) {
						//se vai trocar de ativo na próxima iteração tem que executar
						blnExecutar = true;
					} else {
						blnExecutar = false;
					}

				}


				if (blnExecutar) {

					if (strUltimoNomePregao == strNomePregaoNaoEncontrado && strUltimoTipoAcao == strTipoAcaoNaoEncontrado) {
						//se o último papel é um papel não encontrado copia todos os registros deste papel
						//da tabela Proventos_Temp para a tabela Proventos com o campo Importado = 0.
						objCommand.Execute("INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " SELECT Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com " + ", Ultimo_Preco_Com, Preco_1_1000, Perc_Provento_Preco, 0" + Environment.NewLine + " FROM Proventos_Temp PT " + Environment.NewLine + " WHERE " + strWhere + " AND Nome_Pregao = " + FuncoesBd.CampoStringFormatar(strUltimoNomePregao) + Environment.NewLine + " AND Tipo_Acao = " + FuncoesBd.CampoStringFormatar(strUltimoTipoAcao) + Environment.NewLine);

					}

					//quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
					//em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
					objCommand.CommitTrans();

					//Este teste do IF indica que o ativo anterior da tabela Proventos_Temp
					//foi encontrado na tabela Ativo e importado, portanto deve ser feito recálculo.

					if (blnImportado) {
                        conteudoDoArquivoDeLog += "Atualizando indicadores - Código: " + strCodigoAtual + " - Data Início: " + dtmDataInicioRecalculo.ToString("dd/MM/yyyy") + "- Tipo de Provento: " + strTipoProvento;

						//chamar função para fazer recálculo dos dados do ativo anterior....
						//Não é necessário recalcular o volume médio nas periodicidades diário e semanal
						//porque os proventos não alteram o volume de ações. Apenas os desdobramentos
						//que alteram
						DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
						true, dtmDataInicioRecalculo, "#" + strCodigoAtual + "#",true , true);

					}

				}

			}

			objRS.Fechar();

            string caminhoDoArquivo = cBuscarConfiguracao.ObtemCaminhoPadrao() + "Log_Proventos\\Log_Proventos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            IFileService fileService = new FileService();
            fileService.Save(caminhoDoArquivo, conteudoDoArquivoDeLog);

			Process.Start("c:\\windows\\notepad.exe", caminhoDoArquivo);
            return objCommand.TransStatus;

		}

		/// <summary>
		/// Busca a data de uma cotação a partir de um número sequencial.
		/// </summary>
		/// <param name="pstrCodigo"></param>
		/// <param name="plngSequencial"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public DateTime AtivoSequencialDataBuscar(string pstrCodigo, long plngSequencial, string pstrTabelaCotacao)
		{
		    cRS objRS = new cRS(objConexao);

			objRS.ExecuteQuery("SELECT Data " + Environment.NewLine + " FROM " + pstrTabelaCotacao + Environment.NewLine + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo) + Environment.NewLine + " AND Sequencial = " + plngSequencial);

			DateTime data = Convert.ToDateTime(objRS.Field("Data"));

			objRS.Fechar();

		    return data;

		}

		/// <summary>
		/// Cadastra o provento na tabela "Proventos", cadastra o provento na tabela "Split", atualiza as cotações a
		/// partir da data Ex do provento.
		/// </summary>
		/// <param name="pstrCodigo">Código do ativo</param>
		/// <param name="pintProventoTipo">Tipo de provento que foi pago: dividendo, juros, etc</param>
		/// <param name="pdtmDataAprovacao">Data em que foi aprovando o pagamento do provento</param>
		/// <param name="pdtmDataEx">Data ex-provento</param>
		/// <param name="pdecValorPorAcao">valor do provento pago por ação</param>
		/// <returns></returns>
		/// RetornoOK = operação executada com sucesso.
		/// RetonoErroInesperado = erro de banco de dados ou de programação
		/// RetornoErro2 = não existe cotação na data Ex do provento
		/// <remarks></remarks>
		public cEnum.enumRetorno ProventoCadastrar(string pstrCodigo, cEnum.enumProventoTipo pintProventoTipo, DateTime pdtmDataAprovacao, DateTime pdtmDataEx, decimal pdecValorPorAcao)
		{
			cEnum.enumRetorno functionReturnValue;

		    decimal decUltimoPrecoCom = default(decimal);
		    const string strTabelaCotacao = "COTACAO";
			string strProventoTipoAbreviatura;
			string strProventoTipoDescricao;

		    cCommand objCommand = new cCommand(objConexao);

		    cCalculadorData objCalculadorData = new cCalculadorData(objConexao);


			objCommand.BeginTrans();

			//Verifica se existe cotação na data Ex.

			if (!CotacaoDataExistir(pdtmDataEx, strTabelaCotacao)) {
				objCommand.RollBackTrans();

				return cEnum.enumRetorno.RetornoErro2;

			}

			//busca a última data em que houve cotação antes da data ex-provento
			DateTime dtmDataUltimoPrecoCom = AtivoCotacaoAnteriorDataConsultar(pstrCodigo, pdtmDataEx, strTabelaCotacao);

			//busca a cotação na última data com
		    decimal pdecValorAberturaRet = -1M;
		    CotacaoConsultar(pstrCodigo, dtmDataUltimoPrecoCom, strTabelaCotacao, ref decUltimoPrecoCom,ref pdecValorAberturaRet);

			//busca o último dia útil antes da data ex-provento
			DateTime dtmUltimoDiaCom = objCalculadorData.DiaUtilAnteriorCalcular(pdtmDataEx);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			switch (pintProventoTipo) {

				case cEnum.enumProventoTipo.Dividendo:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("DIV");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("DIVIDENDO");

					break;
				case cEnum.enumProventoTipo.JurosCapitalProprio:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("JCP");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("JRS CAP PRÓPRIO");

					break;
				case cEnum.enumProventoTipo.Rendimento:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("REND");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("RENDIMENTO");

					break;
				case cEnum.enumProventoTipo.RestCapDin:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar("RCDIN");
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar("REST CAP DIN");

					break;
				default:

					strProventoTipoAbreviatura = FuncoesBd.CampoStringFormatar(String.Empty);
					strProventoTipoDescricao = FuncoesBd.CampoStringFormatar(String.Empty);

					break;
			}

			double dblQuantidadeAnterior = Convert.ToDouble(decUltimoPrecoCom) - Convert.ToDouble(pdecValorPorAcao);

			string strQuery = "INSERT INTO Split " + Environment.NewLine + "(Codigo, Data, Tipo, QuantidadeAnterior, QuantidadePosterior)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(pstrCodigo) + ", " + FuncoesBd.CampoDateFormatar(pdtmDataEx) + ", " + strProventoTipoAbreviatura + ", " + FuncoesBd.CampoFloatFormatar(dblQuantidadeAnterior) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ")";

			objCommand.Execute(strQuery);

			cRS objRS = new cRS(objConexao);

			//consulta a descrição do ativo
			strQuery = "SELECT Descricao " + Environment.NewLine + "FROM Ativo " + Environment.NewLine + "WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigo);

			objRS.ExecuteQuery(strQuery);

			string strNomePregao = Convert.ToString(objRS.Field("Descricao")).Substring(0,Convert.ToString(objRS.Field("Descricao")).Length- 3).Trim();

			string strTipoAcao = Convert.ToString(objRS.Field("Descricao")).Substring(Convert.ToString(objRS.Field("Descricao")).Length -3).Trim();

			objRS.Fechar();

		    strQuery = "INSERT INTO Proventos " + Environment.NewLine + "(Nome_Pregao, Tipo_Acao, Data_Aprovacao, Valor_Provento, Provento_1_1000 " + ", Tipo_Provento, Ultimo_Dia_Com, Data_Ultimo_Preco_Com, Ultimo_Preco_Com " + ", Preco_1_1000, Perc_Provento_Preco, Importado) " + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBd.CampoStringFormatar(strNomePregao) + ", " + FuncoesBd.CampoStringFormatar(strTipoAcao) + ", " + FuncoesBd.CampoStringFormatar(pdtmDataAprovacao.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(pdecValorPorAcao)) + ", 1, " + strProventoTipoDescricao + ", " + FuncoesBd.CampoStringFormatar(dtmUltimoDiaCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoStringFormatar(dtmDataUltimoPrecoCom.ToString("dd/MM/yyyy")) + ", " + FuncoesBd.CampoFloatFormatar(Convert.ToDouble(decUltimoPrecoCom)) + ", 1" + ", " + FuncoesBd.CampoStringFormatar((pdecValorPorAcao / decUltimoPrecoCom * 100M).ToString("0##.##")) + ", 1)";

			objCommand.Execute(strQuery);


			//quando vai trocar de ativo precisa fazer commit para que o recálculo que será feito
			//em outra transação já tenho os dados dos splits que foram inseridos por esta operação.
			objCommand.CommitTrans();



			if (objCommand.TransStatus) {
				functionReturnValue = cEnum.enumRetorno.RetornoOK;

				//chamar função para fazer recálculo dos dados do ativo anterior....
				//Não é necessário recalcular o volume médio nas periodicidades diário e semanal
				//porque os proventos não alteram o volume de ações. Apenas os desdobramentos
				//que alteram
				DadosRecalcular(true, true, true, true, false, true, true, true, true, false,
				true, dtmUltimoDiaCom, "#" + pstrCodigo + "#",true , true);


			} else {
				functionReturnValue = cEnum.enumRetorno.RetornoErroInesperado;

			}
			return functionReturnValue;

		}

	    /// <summary>
	    /// Gera as cláusulas FROM e WHERE para serem utilizadas com as queries que buscam os dados para gerar os gráficos.
	    /// </summary>
	    /// <param name="pstrCodigoAtivo"></param>
	    /// <param name="pdtmDataMinima">Data inicial de busca dos dados</param>
	    /// <param name="pdtmDataMaxima">Data final de busca dos dados</param>
	    /// <param name="pstrTabela">
	    /// Indica a tabela que será utilizada para buscar os dados do gráfico.
	    /// Os valores possíveis são: COTACAO, COTACAO_SEMANAL, MEDIA_DIARIA, MEDIA_SEMANAL.
	    /// </param>
	    /// <param name="pdblOperador">Quando a cotação possuir splits, contém todas as operações para transformar
	    /// as cotações de períodos anteriores ao split na mesma razão da última cotação</param>
	    /// <param name="pdblOperadorInvertido">Mesma função do parâmetro pstrOperador, porém com as operações invertidas,
	    /// caso seja necessário obter os valores do split invertido. Este operador é utilizado geralmente para calcular o volume
	    /// </param>
	    /// <param name="pintNumPeriodos">Número de períodos da média que dever ser buscada. Necessário informar apenas quando a função é utilizada
	    /// para calcular médias</param>
	    /// <param name="pstrDado">VALOR OU VOLUME. Necessário informar apenas quando a função é utilizada
	    /// para calcular médias</param>
	    /// <param name="pstrFinalidade">Indica se esta função está sendo chamada para obter todos os registros de um determinado período
	    /// ou apenas os valores mínimos e máximos
	    /// Valores possíveis: "TODOS", "EXTREMOS"</param>
	    /// <param name="pblnCotacaoBuscar">Indica se devem ser buscadas dados relativos ao preço do ativo</param>
	    /// <param name="pblnVolumeBuscar">Indica se devem ser buscados dados relativos ao volume do ativo</param>
	    /// <param name="pstrMediaTipo">Tipo da média que deve ser consultada. Valores possíveis: "E" (exponencial), "A" (aritmética). 
	    /// Necessário informar apenas quando a função é utilizada para calcular médias</param>
	    /// <param name="pstrOrderBy">Indica o ordenamento da consulta. Deve ser informado apenas quando o parâmetro "pstrFinalidade" estiver
	    /// com o valor "TODOS", mas não é obrigatório informar neste caso.</param>
	    /// <returns>string contendo as cláusulas from e where</returns>
	    /// <remarks></remarks>
	    private string ConsultaUnitariaGerar(string pstrCodigoAtivo, DateTime pdtmDataMinima, DateTime pdtmDataMaxima, string pstrTabela, double pdblOperador, double pdblOperadorInvertido, string pstrFinalidade, string pstrMediaTipo = "", int pintNumPeriodos = -1, string pstrDado = "",
		bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrOrderBy = "")
		{

			string strSQL = String.Empty;

			string strColunas = String.Empty;

			string strTabelaAux = pstrTabela.ToUpper();

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			if (strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL") {

				if (pstrFinalidade == "TODOS") {

					if (pblnCotacaoBuscar) {
						strColunas = " data, valorabertura * " + FuncoesBd.CampoFormatar(pdblOperador) + " as valorabertura " + Environment.NewLine + ", valorfechamento * " + FuncoesBd.CampoFormatar(pdblOperador) + " as valorfechamento " + Environment.NewLine + ", valorminimo * " + FuncoesBd.CampoFormatar(pdblOperador) + " as valorminimo " + Environment.NewLine + ", valormaximo * " + FuncoesBd.CampoFormatar(pdblOperador) + " as valormaximo " + Environment.NewLine + ", Oscilacao ";


						if (strTabelaAux == "COTACAO_SEMANAL") {
							strColunas = strColunas + ", DataFinal";

						}

					}


				} else if (pstrFinalidade == "EXTREMOS") {
					strSQL = " SELECT MIN(ValorMinimo * " + FuncoesBd.CampoFormatar(pdblOperador) + ") AS ValorMinimo " + ", MAX(ValorMaximo * " + FuncoesBd.CampoFormatar(pdblOperador) + ") AS ValorMaximo ";

				}


			} else {
				//se é a tabela de médias
				if (pstrDado == "VALOR") {
					//SE É MEDIA DE VALOR

					if (pstrFinalidade == "TODOS") {
						strSQL = " SELECT Data, Valor * " + FuncoesBd.CampoFormatar(pdblOperador) + " as Valor " + Environment.NewLine;


					} else if (pstrFinalidade == "EXTREMOS") {
						strSQL = " SELECT MIN(Valor * " + FuncoesBd.CampoFormatar(pdblOperador) + ") AS ValorMinimo " + Environment.NewLine + ", MAX(Valor * " + FuncoesBd.CampoFormatar(pdblOperador) + ") AS ValorMaximo " + Environment.NewLine + ", COUNT(1) as NumRegistros " + Environment.NewLine;

					}
				} else {
					//SE É MEDIA DE VOLUME

					if (pstrFinalidade == "TODOS") {
						strSQL = '\t' + " select Data, Valor * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + " as Valor " + Environment.NewLine;


					} else if (pstrFinalidade == "EXTREMOS") {
						strSQL = " SELECT MIN(Valor * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + ") AS ValorMinimo " + Environment.NewLine + ", MAX(Valor * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + ") AS ValorMaximo " + Environment.NewLine + ", COUNT(1) as NumRegistros " + Environment.NewLine;

					}

				}

			}

			//Quando é uma das tabelas de cotações, calcula o volume também.

			if (strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL") {

				if (pstrFinalidade == "TODOS") {

					if (pblnVolumeBuscar) {
						if (strColunas != String.Empty) {
							strColunas = strColunas + ", ";
						}

						strColunas = strColunas + "titulos_total * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + " as Titulos_Total " + Environment.NewLine + ", Negocios_Total, Valor_Total " + Environment.NewLine;

					}


				} else {
					strSQL = strSQL + ", MIN(titulos_total * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + ") as Volume_Minimo " + Environment.NewLine + ", MAX(titulos_total * " + FuncoesBd.CampoFormatar(pdblOperadorInvertido) + ") as Volume_Maximo " + Environment.NewLine + ", COUNT(1) AS ContadorVolumeMedio " + Environment.NewLine;

				}

			}


			if ((strTabelaAux == "COTACAO" || strTabelaAux == "COTACAO_SEMANAL") && pstrFinalidade == "TODOS") {
				strSQL = "SELECT " + strColunas;

			}

			//********INICIO DO TRATAMENTO DO FROM, WHERE e ORDER BY 
			strSQL = strSQL + " from " + pstrTabela + Environment.NewLine;

			strSQL = strSQL + " where codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigoAtivo) + Environment.NewLine + " and data >= " + FuncoesBd.CampoDateFormatar(pdtmDataMinima) + Environment.NewLine + " and data <= " + FuncoesBd.CampoDateFormatar(pdtmDataMaxima) + Environment.NewLine;


			if (strTabelaAux == "MEDIA_DIARIA" || strTabelaAux == "MEDIA_SEMANAL") {
				string strMediaTipoAux;

				if (pstrDado == "VALOR") {
					if (pstrMediaTipo == "E") {
						strMediaTipoAux = "MME";
					} else if (pstrMediaTipo == "A") {
						strMediaTipoAux = "MMA";
					} else {
						strMediaTipoAux = String.Empty;
					}
				} else if (pstrDado == "VOLUME") {
					//volume não verifica qual o tipo de média. É sempre aritmética
					strMediaTipoAux = "VMA";
				} else {
					strMediaTipoAux = String.Empty;
				}

				strSQL = strSQL + " and Tipo = " + FuncoesBd.CampoStringFormatar(strMediaTipoAux) + Environment.NewLine + " and NumPeriodos = " + pintNumPeriodos.ToString() + Environment.NewLine;

			}


			if (pstrFinalidade == "TODOS" && pstrOrderBy != String.Empty) {
				//se é para listar todos os registros e foi passado um ORDER BY
				strSQL = strSQL + " ORDER BY " + pstrOrderBy;

			}

			return strSQL;

		}

		/// <summary>
		/// Realiza as consultas necessárias dos dados que serão mostrados no gráfico e retorna um objeto cRSList com estes dados
		/// </summary>
		/// <param name="pdtmDataInicial"></param>
		/// <param name="pdtmDataFinal"></param>
		/// <param name="pstrOrigemDado">
		/// Indica de onde serão buscados os dados.
		/// Valores possíveis: "COTACAO", "MEDIA"
		/// </param>
		/// <param name="pstrMediaTipo">Tipo da média que deve ser consultada. Necessário informar apenas quando a função é utilizada</param>
		/// <param name="pintNumPeriodos">Número de períodos da média que deve ser consultada. Necessário informar apenas quando a função é utilizada</param>
		/// <param name="pstrDado">VALOR OU VOLUME. Necessário informar apenas quando a função é utilizada para calcular médias</param>
		/// <param name="pblnCotacaoBuscar">Indica se é para buscar dados das cotações</param>
		/// <param name="pblnVolumeBuscar">Indica se é para buscar dados do volume</param>
		/// <param name="pdtmDataMaximaSplit">Data máxima (pdtmDataFinal) que deve ser utilizada na busca do split. Caso seja necessários consultar
		/// todos os splits até a data mais recente, parâmetro não deve ser passado. </param>
		/// <returns></returns>
		/// <remarks></remarks>
		private List<string> ConsultaQueriesGerar(string pstrCodigoAtivo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrPeriodicidade, string pstrOrigemDado, DateTime pdtmDataMaximaSplit, bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrMediaTipo = "", int pintNumPeriodos = -1,
		string pstrDado = "", string pstrOrderBy = "")
		{

			cRSList objRSListSplit = null;
			var lstQueries = new List<string>();

			string strSQL;

			string strTabela = string.Empty;
			string strTabelaCotacao = string.Empty;
			string strTabelaMedia = string.Empty;
		    string strTabelaIfr = string.Empty;

			cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIfr);

			if (pstrOrigemDado == "COTACAO") {
				strTabela = strTabelaCotacao;
			} else if (pstrOrigemDado == "MEDIA") {
				strTabela = strTabelaMedia;
			}

			double dblOperador = 1;
			double dblOperadorInvertido = 1;

			//inicializa os operadores com 1 para que a multiplicação até o primeiro split não altere os valores
			double dblOperadorAux = 1;
			double dblOperadorInvertidoAux = 1;

			//Se o dado que será consultado for o volume, considera apenas os splits de desdobramento. 
			//Caso contrário considera todos os tipos.
			string strSplitTipo = (pstrDado == "VOLUME" ? "DESD" : string.Empty);

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this.objConexao);

			//consulta os splits
			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigoAtivo, pdtmDataInicial, "D", ref objRSListSplit, pdtmDataMaximaSplit, strSplitTipo);


			if (blnSplitExistir) {
				bool blnRealizarConsultaUnitaria = false;
				bool blnAtribuirOperador = false;

				DateTime dtmDataMinima = Convert.ToDateTime(objRSListSplit.Field("Data"));

				//Inicializa a data máxima com a data final porque na primeira iteração, se já for necessário calcular valores (dtmDataMinima < pdtmDataFinal)
				//os valores têm que ser calculados da data do primeiro split até a data final do gráfico.
				DateTime dtmDataMaxima = pdtmDataFinal;

				//Fica em loop buscando os registros até que a data máxima fique menor do que a data mínima.
				//Isto vai acontecer logo após a iteração referente as datas entre a data inicial (pdtmDataInicial) 
				//e o Split com menor data, que no caso será o último split do RS, pois o mesmo está em ordem 
				//decrescente de data.

				//Tem que colocar um OR pelo EOF do RS porque quando a área de desenho vai mostrar os dados que não são as últimas cotações
				//pode acontecer de inicialmente a data máxima ser menor do que a data mínima. Isto vai acontecer se a data máximo for menor 
				//do que a data do split mais recente.

				while ((dtmDataMaxima >= dtmDataMinima) || (!objRSListSplit.EOF)) {

					if (!objRSListSplit.EOF) {

						if (pstrPeriodicidade == "SEMANAL") {
							//quando a cotação é semanal, a data inicial tem que ser a data menor ou igual a data do split,
							//pois se ocorrer um split em uma data que não for a primeira data da semana, toda as cotações
							//desta semana já foram convertidas para a razão do split
							dtmDataMinima = AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, dtmDataMinima);

						}

					}


					if (pstrPeriodicidade == "DIARIO") {
						blnRealizarConsultaUnitaria = dtmDataMinima <= pdtmDataFinal && dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));
					} else if (pstrPeriodicidade == "SEMANAL")
					{
					    DateTime dataDoProximoSplit = Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));
                        DateTime primeiraDataDaSemana = dataDoProximoSplit == Constantes.DataInvalida ? Constantes.DataInvalida : AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo,dataDoProximoSplit);
						blnRealizarConsultaUnitaria = (dtmDataMinima <= pdtmDataFinal) && (dtmDataMinima != primeiraDataDaSemana);
					}


					if (blnRealizarConsultaUnitaria) {
						//tem que gerar a tabela somente se o split estiver dentro da área de dados,
						//senão tem que ficar calculando apenas o operador para fazer as multiplicações depois.

						//Regra do parâmetro "pdtmDataFinal": se a data final de busca (pdtmDataFinal) for menor que a data maxima da iteração
						//tem que utilizar a data final, senão utiliza a data máxima
						strSQL = ConsultaUnitariaGerar(pstrCodigoAtivo, dtmDataMinima, dtmDataMaxima, strTabela, dblOperador, dblOperadorInvertido, "TODOS", pstrMediaTipo, pintNumPeriodos, pstrDado,
						pblnCotacaoBuscar, pblnVolumeBuscar, pstrOrderBy);

						lstQueries.Add(strSQL);

						//para a próxima iteração a data máxima é a data anterior ao split que está acabando
						dtmDataMaxima = dtmDataMinima.AddDays(-1);

					}
					//If dtmDataMinima <= pdtmDataFinal Then


					if (!objRSListSplit.EOF) {
						dblOperadorAux = dblOperadorAux * Convert.ToDouble(objRSListSplit.Field("Razao"));

						//A razão invertida só é utilizada no cálculo do volume. 
						//O volume só deve utilizar os splits de desdobramento (tipo = 'DESD').
						//Por isso, só multiplica o operador invertido pela razão invertida neste caso.
						if ((string) objRSListSplit.Field("Tipo") == "DESD") {
							dblOperadorInvertidoAux = dblOperadorInvertidoAux * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));
						}

						//Ajusta o operador quando a próxima data for diferente.
						//Tem que colocar este IF antes do MOVENEXT, pois caso contrário vai alterar o operador antes de chamar
						//a função "ConsultaUnitariaGerar", fazendo com que os valores sejam multiplicados em um intervalo 
						//imediatamente anterior ao que deve ser multiplicado


						if (pstrPeriodicidade == "DIARIO") {
							blnAtribuirOperador = dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


						} else if (pstrPeriodicidade == "SEMANAL") {
							blnAtribuirOperador = dtmDataMinima != AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));

						}


						if (blnAtribuirOperador) {
							dblOperador = dblOperadorAux;
							dblOperadorInvertido = dblOperadorInvertidoAux;

						}

						objRSListSplit.MoveNext();



						if (objRSListSplit.EOF) {
							//se chegou ao fim do RS de splits, marca como data mínima para próxima iteração
							dtmDataMinima = pdtmDataInicial;

						} else {
							//a data mínima é a data do split
							dtmDataMinima = Convert.ToDateTime(objRSListSplit.Field("Data"));

						}

					}

				}
				//If Not objRSSplit.EOF Then


			} else {
				//Não tem split: consulta única.
				strSQL = ConsultaUnitariaGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, strTabela, dblOperador, dblOperadorInvertido, "TODOS", pstrMediaTipo, pintNumPeriodos, pstrDado,
				pblnCotacaoBuscar, pblnVolumeBuscar, pstrOrderBy);

				lstQueries.Add(strSQL);


			}

			return lstQueries;

		}

		public cRSList ConsultaExecutar(string pstrCodigoAtivo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrPeriodicidade, string pstrOrigemDado, DateTime pdtmDataMaximaSplit, bool pblnCotacaoBuscar = false, bool pblnVolumeBuscar = false, string pstrMediaTipo = "", int pintNumPeriodos = -1,
		string pstrDado = "")
		{

			cRSList objRSList = new cRSList(objConexao);

			objRSList.Queries = ConsultaQueriesGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, pstrPeriodicidade, pstrOrigemDado, pdtmDataMaximaSplit, pblnCotacaoBuscar, pblnVolumeBuscar, pstrMediaTipo, pintNumPeriodos,
			pstrDado, "DATA DESC");

			//Executa as queries
			objRSList.ExecuteQuery();

			//Retorna o List.
			return objRSList;

		}


	    /// <summary>
	    /// Calcula os valores máximo e mínimo das áreas de desenho do gráfico  (cotação, volume, ifr). 
	    /// Função unitária que não considera splits.
	    /// </summary>
	    /// <param name="pdtmDataInicial">Data do primeiro candle que será mostrado na área de visualização do gráfico</param>
	    /// <param name="pdtmDataFinal">Data do último candle que será mostrado na área de visualização do gráfico</param>
	    /// <param name="pdblOperador">Operador que será utilizando para fazer as conversão dos preços. 
	    /// Quando a área de dados não possuir splits, o valor é "1" para não alterar o valor dos dados </param>
	    /// <param name="pdblOperadorInvertido">Operador que será utilizando para fazer as conversão dos volumes. 
	    /// Quando a área de dados não possuir splits, o valor é "1" para não alterar o valor dos dados </param>
	    /// <param name="pdecValorMinimoRet">Valor mínimo que será mostrado na área de preços</param>
	    /// <param name="pdecValorMaximoRet">Valor máximo que será mostrado na área de preços</param>
	    /// <param name="plstMediasRet">lista que contém o número de registros que serão desenhadas para cada uma das médias do gráfico</param>
	    /// <param name="pdblVolumeMinimoRet">Menor valor que será mostrado na área de volume</param>
	    /// <param name="pdblVolumeMaximoRet">Maior valor que será mostrado na área de volume</param>
	    /// <param name="pintContadorIFRMedioRet">Número de registros de média do IFR que serão mostrados</param>
	    /// <param name="pintContadorIFRRet">Núumero de registros do IFR que serão mostrados</param>
	    /// <param name="pintVolumeMedioNumRegistrosRet">Número de registros do volume que serão mostrados</param>
	    /// <param name="pblnVolumeDesenhar"></param>
	    /// <param name="pblnIFRDesenhar"></param>
	    /// <param name="pstrCodigoAtivo"></param>
	    /// <param name="pstrPeriodicidade"></param>
	    /// <param name="pblnMediaMovelDesenhar"></param>
	    /// <remarks></remarks>
	    private bool ValoresExtremosUnitarioCalcular(string pstrCodigoAtivo, string pstrPeriodicidade, bool pblnMediaMovelDesenhar, List<cMediaDTO> plstMediasSelecionadas, 
        bool pblnVolumeDesenhar, bool pblnIFRDesenhar, int pintIFRNumPeriodos, DateTime pdtmDataInicial, DateTime pdtmDataFinal, double pdblOperador,
		double pdblOperadorInvertido, ref decimal pdecValorMinimoRet, ref decimal pdecValorMaximoRet, ref double pdblVolumeMinimoRet, ref double pdblVolumeMaximoRet, 
        ref int pintContadorIFRRet, ref int pintContadorIFRMedioRet, ref int pintVolumeMedioNumRegistrosRet, ref List<cMediaDTO> plstMediasRet)
		{
			cRS objRS = new cRS(objConexao);

		    string strTabelaCotacao = string.Empty;
			string strTabelaMedia = string.Empty;
			string strTabelaIFR = string.Empty;

			cCalculadorTabelas.TabelasCalcular(pstrPeriodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

			string strQuery = ConsultaUnitariaGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, strTabelaCotacao, pdblOperador, pdblOperadorInvertido, "EXTREMOS");

			objRS.ExecuteQuery(strQuery);

            if (Convert.ToInt32(objRS.Field("ContadorVolumeMedio")) == 0)
		    {
		        return false;
		    }

			pdecValorMinimoRet = Convert.ToDecimal(objRS.Field("ValorMinimo"));
			pdecValorMaximoRet = Convert.ToDecimal(objRS.Field("ValorMaximo"));


			if (pblnVolumeDesenhar) {
				pdblVolumeMinimoRet = Convert.ToDouble(objRS.Field("Volume_Minimo"));

				pdblVolumeMaximoRet = Convert.ToDouble(objRS.Field("Volume_Maximo"));

			}

			objRS.Fechar();

			//******************INICIO DO TRATAMENTO PARA O IFR******************

			if (pblnIFRDesenhar) {

                FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

                //Calcula o número de registros que serão desenhados do IFR de 2 períodos 
				strQuery = " SELECT COUNT(1) AS ContadorIFR" + " FROM " + strTabelaIFR + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigoAtivo) + " AND NumPeriodos = " + pintIFRNumPeriodos + " AND Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " AND Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);

				objRS.ExecuteQuery(strQuery);

				pintContadorIFRRet = Convert.ToInt16(objRS.Field("ContadorIFR"));

				objRS.Fechar();


				if (pintIFRNumPeriodos == 2) {
					//Calcula o número de registros que serão desenhados da média de 13 períodos do IFR de 2 períodos 
					strQuery = " SELECT COUNT(1) AS ContadorIFRMedio" + " FROM " + strTabelaMedia + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(pstrCodigoAtivo) + " AND Tipo = " + FuncoesBd.CampoStringFormatar("IFR2") + " AND NumPeriodos = 13" + " AND Data >= " + FuncoesBd.CampoDateFormatar(pdtmDataInicial) + " AND Data <= " + FuncoesBd.CampoDateFormatar(pdtmDataFinal);

					objRS.ExecuteQuery(strQuery);

					pintContadorIFRMedioRet = Convert.ToInt16(objRS.Field("ContadorIFRMedio"));

					objRS.Fechar();

				} else {
					pintContadorIFRMedioRet = 0;
				}

			}
			//********************FIM DO TRATAMENTO PARA O IFR*******************

			//********************INICIO DO TRATAMENTO PARA O VOLUME MÉDIO*******************


			if (pblnVolumeDesenhar) {
				//busca os valores máximo e mínimo da média de 21 períodos do volume
				strQuery = ConsultaUnitariaGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, strTabelaMedia, pdblOperador, pdblOperadorInvertido, "EXTREMOS", "A", 21, "VOLUME");

				objRS.ExecuteQuery(strQuery);


				if (Convert.ToDouble(objRS.Field("ValorMinimo")) < pdblVolumeMinimoRet) {
					pdblVolumeMinimoRet = Convert.ToDouble(objRS.Field("ValorMinimo"));

				}


				if (Convert.ToDouble(objRS.Field("ValorMaximo")) > pdblVolumeMaximoRet) {
					pdblVolumeMaximoRet = Convert.ToDouble(objRS.Field("ValorMaximo"));

				}

				pintVolumeMedioNumRegistrosRet = Convert.ToInt16(objRS.Field("NumRegistros"));

				objRS.Fechar();

			}

			//********************FIM DO TRATAMENTO PARA O VOLUME MÉDIO*******************

			//********************INICIO DO TRATAMENTO PARA AS MÉDIAS MÓVEIS*******************


			if (pblnMediaMovelDesenhar) {
				plstMediasRet = new List<cMediaDTO>();

				//percorre a collection que contém todas as médias que serão desenhadas e calcula
				//os valores máximo e mínimo no período em que os dados serão desenhados.


				foreach (cMediaDTO objMediaDTO in plstMediasSelecionadas) {
					//calcula a tabela para as médias
					strQuery = ConsultaUnitariaGerar(pstrCodigoAtivo, pdtmDataInicial, pdtmDataFinal, strTabelaMedia, pdblOperador, pdblOperadorInvertido, "EXTREMOS", objMediaDTO.Tipo, objMediaDTO.NumPeriodos, "VALOR");

					objRS.ExecuteQuery(strQuery);


					if (Convert.ToDecimal(objRS.Field("ValorMinimo", 0)) > 0) {

						if (Convert.ToDecimal(objRS.Field("ValorMinimo")) < pdecValorMinimoRet) {
							pdecValorMinimoRet = Convert.ToDecimal(objRS.Field("ValorMinimo"));

						}

					}


					if (Convert.ToDecimal(objRS.Field("ValorMaximo", 0)) > 0) {

						if (Convert.ToDecimal(objRS.Field("ValorMaximo")) > pdecValorMaximoRet) {
							pdecValorMaximoRet = Convert.ToDecimal(objRS.Field("ValorMaximo"));

						}

					}

					//atribui na estrutura de médias o número de registros encontrados.
					//será utilizada posteriormente para redimensionar os arrays de médias
					plstMediasRet.Add(new cMediaDTO(objMediaDTO.Tipo, objMediaDTO.NumPeriodos, "VALOR", Convert.ToInt16(objRS.Field("NumRegistros"))));

					objRS.Fechar();

				}

			}
			//If blnMMExpDesenhar Then...

			//********************FIM DO TRATAMENTO PARA AS MÉDIAS MÓVEIS EXPONENCIAIS*******************

		    return true;
		}


		/// <summary>
		/// Calcula os valores máximo e mínimo das áreas de desenho do gráfico  (cotação, volume, ifr).
		/// Função que calcula para toda a área de dados e faz tratamento para os splits.
		/// </summary>
		/// <param name="pdtmDataInicial">Data do primeiro candle que será mostrado na área de visualização do gráfico</param>
		/// <param name="pdtmDataFinal">Data do último candle que será mostrado na área de visualização do gráfico</param>
		/// <param name="pdecValorMinimoRet">Valor mínimo que será mostrado na área de preços</param>
		/// <param name="pdecValorMaximoRet">Valor máximo que será mostrado na área de preços</param>
		/// <param name="plstMediasRet">Lista que contém o número de registros que serão desenhadas para cada uma das médias do gráfico</param>
		/// <param name="pdblVolumeMinimoRet">Menor valor que será mostrado na área de volume</param>
		/// <param name="pdblVolumeMaximoRet">Maior valor que será mostrado na área de volume</param>
		/// <param name="pintContadorIFRMedioRet">Número de registros de média do IFR que serão mostrados</param>
		/// <param name="pintContadorIFRRet">Núumero de registros do IFR que serão mostrados</param>
		/// <param name="pintVolumeMedioNumRegistrosRet">Número de registros do volume que serão mostrados</param>
		/// <remarks></remarks>
		public void ValoresExtremosCalcular(string pstrCodigoAtivo, string pstrPeriodicidade, bool pblnMMDesenhar, List<cMediaDTO> plstMediasSelecionadas, bool pblnVolumeDesenhar, bool pblnIFRDesenhar, int pintIFRNumPeriodos, DateTime pdtmDataInicial, DateTime pdtmDataFinal, ref decimal pdecValorMinimoRet,

		ref decimal pdecValorMaximoRet, ref double pdblVolumeMinimoRet, ref double pdblVolumeMaximoRet, ref int pintContadorIFRRet, ref List<cMediaDTO> plstMediasRet, ref int pintVolumeMedioNumRegistrosRet, ref int pintContadorIFRMedioRet)
		{
			cRSList objRSListSplit = null;

			double dblOperador = 1;
			double dblOperadorInvertido = 1;

			//inicializa os operadores com 1 para que a multiplicação até o primeiro split não altere os valores
			double dblOperadorAux = 1;
			double dblOperadorInvertidoAux = 1;

			//sempre considera a data inicial de geração do gráfico mais um, pois se tiver um split na data 
			//do primeiro dia de gráfico não tem valores para serem convertidos.
			//***Ordena os splits em ordem DECRESCENTE de data.

			cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(objConexao);

			bool blnSplitExistir = objCarregadorSplit.SplitConsultar(pstrCodigoAtivo, pdtmDataInicial.AddDays(1), "D", ref objRSListSplit,Constantes.DataInvalida);


			if (blnSplitExistir) {
				//Tem split. Necessário percorrer o RS de splits e gerar queries para cada dado a cada split.

				//As variávies com sufixo Aux são passadas por referência para a função "ValoresExtremosUnitarioCalcular"
				decimal decValorMinimoAux = default(decimal);
				decimal decValorMaximoAux = default(decimal);
				double dblVolumeMinimoAux = 0;
				double dblVolumeMaximoAux = 0;
				int intContadorIFRAux = 0;
				List<cMediaDTO> lstMediasAux = null;
				int intVolumeMedioNumRegistrosAux = 0;
				int intContadorIFRMedioAux = 0;

				DateTime dtmDataMinima = Convert.ToDateTime(objRSListSplit.Field("Data"));
				//Inicializa a data máxima com a data final porque na primeira iteração, se já for necessário calcular valores (dtmDataMinima < pdtmDataFinal)
				//os valores têm que ser calculados da data do primeiro split até a data final do gráfico.
				DateTime dtmDataMaxima = pdtmDataFinal;

				//Indica se é a primeira iteração da busca de valores extremos quando há extremos.
				//Serve para saber se deve atribuir os valores de retorno da função "ValoresExtremosUnitarioCalcular"
				//diretamente ou se deve comparar com algum valor das iterações anteriores.
				bool blnPrimeiraIteracao = true;

				//Utilizada para saber se encontrou um item na collection de médias.

			    bool blnRealizarConsultaUnitaria = false;
				bool blnAtribuirOperador = false;

				//Fica em loop calculando os valores extremos até que a data máxima fique menor do que a data mínima.
				//Isto vai acontecer logo após a iteração referente as datas entre a data inicial (pdtmDataInicial) 
				//e o Split com menor data, que no caso será o último split do RS, pois o mesmo está em ordem 
				//decrescente de data.
				//Tem que colocar um OR pelo EOF do RS porque quando a área de desenho vai mostrar os dados que não são as últimas cotações
				//pode acontecer de inicialmente a data máxima ser menor do que a data mínima. Isto vai acontecer se a data máximo for menor 
				//do que a data do split mais recente.

				while ((dtmDataMaxima >= dtmDataMinima) || (!objRSListSplit.EOF)) {

					if (!objRSListSplit.EOF) {

						if (pstrPeriodicidade == "SEMANAL") {
							//quando a cotação é semanal, a data inicial tem que ser a data menor ou igual a data do split,
							//pois se ocorrer um split em uma data que não for a primeira data da semana, toda as cotações
							//desta semana já foram convertidas para a razão do split
							dtmDataMinima = AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, dtmDataMinima);

						}

					}


					if (pstrPeriodicidade == "DIARIO") {
						blnRealizarConsultaUnitaria = dtmDataMinima <= pdtmDataFinal && dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


					} else if (pstrPeriodicidade == "SEMANAL") {

						blnRealizarConsultaUnitaria = dtmDataMinima <= pdtmDataFinal && dtmDataMinima != AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));

					}



					if (blnRealizarConsultaUnitaria) {
						//tem que gerar a tabela somente se o split estiver dentro da área de dados,
						//senão tem que ficar calculando apenas o operador para fazer as multiplicações depois.

						//Regra do parâmetro "pdtmDataFinal": se a data final de busca (pdtmDataFinal) for menor que a data maxima da iteração
						//tem que utilizar a data final, senão utiliza a data máxima
						if (ValoresExtremosUnitarioCalcular(pstrCodigoAtivo, pstrPeriodicidade, pblnMMDesenhar, plstMediasSelecionadas, pblnVolumeDesenhar, pblnIFRDesenhar, pintIFRNumPeriodos, 
                            dtmDataMinima, dtmDataMaxima, dblOperador,dblOperadorInvertido, ref decValorMinimoAux, ref decValorMaximoAux, ref dblVolumeMinimoAux, ref dblVolumeMaximoAux, 
                            ref intContadorIFRAux, ref intContadorIFRMedioAux, ref intVolumeMedioNumRegistrosAux, ref lstMediasAux))
						{
						    if (blnPrimeiraIteracao) {
						        //Se é a primeira iteração atribui diretamente nas variáveis de retorno sem comparação
						        pdecValorMinimoRet = decValorMinimoAux;
						        pdecValorMaximoRet = decValorMaximoAux;

						        pdblVolumeMinimoRet = dblVolumeMinimoAux;
						        pdblVolumeMaximoRet = dblVolumeMaximoAux;

						        pintContadorIFRRet = intContadorIFRAux;
						        pintContadorIFRMedioRet = intContadorIFRMedioAux;

						        pintVolumeMedioNumRegistrosRet = intVolumeMedioNumRegistrosAux;

						        plstMediasRet = lstMediasAux;


						    } else {

						        if (decValorMinimoAux < pdecValorMinimoRet) {
						            pdecValorMinimoRet = decValorMinimoAux;

						        }


						        if (decValorMaximoAux > pdecValorMaximoRet) {
						            pdecValorMaximoRet = decValorMaximoAux;

						        }


						        if (dblVolumeMinimoAux < pdblVolumeMinimoRet) {
						            pdblVolumeMinimoRet = dblVolumeMinimoAux;

						        }


						        if (dblVolumeMaximoAux > pdblVolumeMaximoRet) {
						            pdblVolumeMaximoRet = dblVolumeMaximoAux;

						        }

						        pintContadorIFRRet = pintContadorIFRRet + intContadorIFRAux;

						        pintContadorIFRMedioRet = pintContadorIFRMedioRet + intContadorIFRMedioAux;

						        pintVolumeMedioNumRegistrosRet = pintVolumeMedioNumRegistrosRet + intVolumeMedioNumRegistrosAux;

						        //Percorre cada um dos items da collection retorna pela função "ValoresExtremosUnitarioCalcular".

						        foreach (cMediaDTO objMediaAux in lstMediasAux) {
						            //marca como item não encontrado
						            bool blnItemEncontrado = false;


						            for (int intI = 0; intI <= plstMediasRet.Count - 1; intI++) {

						                if (plstMediasRet[intI].Equals(objMediaAux)) {
						                    blnItemEncontrado = true;

						                    //Incrementa o número de registros do item atual na collection geral.
						                    plstMediasRet[intI].IncrementaNumRegistros(objMediaAux.NumRegistros);

						                    //No momento que  encontrou o item pode sair do for
						                    break; // TODO: might not be correct. Was : Exit For

						                }

						            }


						            if (!blnItemEncontrado) {
						                //Se o item não foi encontrado, adiciona na collection geral
						                plstMediasRet.Add(objMediaAux);

						            }

						        }

						    }
						    
						}						// If blnPrimeiraIteracao Then

						//Marca como false, para nas próximas iterações comparar com os valores já atribuidos nas variáveis.
						blnPrimeiraIteracao = false;

						//para a próxima iteração a data máxima é a data anterior ao split que está acabando
						dtmDataMaxima = dtmDataMinima.AddDays(-1);

					}
					//If dtmDataMinima <= pdtmDataFinal Then



					if (!objRSListSplit.EOF) {
						dblOperadorAux = dblOperadorAux * Convert.ToDouble(objRSListSplit.Field("Razao"));

						//A razão invertida só é utilizada no cálculo do volume. 
						//O volume só deve utilizar os splits de desdobramento (tipo = 'DESD').
						//Por isso, só multiplica o operador invertido pela razão invertida neste caso.
						if ((string) objRSListSplit.Field("Tipo") == "DESD") {
							dblOperadorInvertidoAux = dblOperadorInvertidoAux * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));
						}

						//Ajusta o operador quando a próxima data for diferente.
						//Tem que colocar este IF antes do MOVENEXT, pois caso contrário vai alterar o operador antes de chamar
						//a função "ConsultaUnitariaGerar", fazendo com que os valores sejam multiplicados em um intervalo 
						//imediatamente anterior ao que deve ser multiplicado



						if (pstrPeriodicidade == "DIARIO") {
							blnAtribuirOperador = dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


						} else if (pstrPeriodicidade == "SEMANAL") {
							blnAtribuirOperador = dtmDataMinima != AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(pstrCodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));

						}


						if (blnAtribuirOperador) {
							dblOperador = dblOperadorAux;
							dblOperadorInvertido = dblOperadorInvertidoAux;

						}


						objRSListSplit.MoveNext();

						dtmDataMinima = objRSListSplit.EOF ? pdtmDataInicial : Convert.ToDateTime(objRSListSplit.Field("Data"));

					}

				}


			} else {
				//Não tem split. Tem que chamar a função ValoresExtremosUnitarioCalcular uma única vez para todo o período que será desenhado o gráfico
				ValoresExtremosUnitarioCalcular(pstrCodigoAtivo, pstrPeriodicidade, pblnMMDesenhar, plstMediasSelecionadas, pblnVolumeDesenhar, pblnIFRDesenhar, pintIFRNumPeriodos, pdtmDataInicial, pdtmDataFinal, dblOperador,
				dblOperadorInvertido, ref pdecValorMinimoRet, ref pdecValorMaximoRet, ref pdblVolumeMinimoRet, ref pdblVolumeMaximoRet, ref pintContadorIFRRet, ref pintContadorIFRMedioRet, ref pintVolumeMedioNumRegistrosRet, ref plstMediasRet);

			}
		}


		public void RecalcularIndicadores()
		{
			cRSList objRS = new cRSList(objConexao);

		    string strSQL = " SELECT CODIGO, Min(DATA)  As DATA " + "FROM " + "(" + " SELECT CODIGO, DATA " + " FROM Split " + " WHERE TIPO Not In('DESD', 'CISAO')" + " And codigo Not In ('BBAS3', 'BBDC4', 'BRAP4', 'CIEL3', 'COCE5', 'CSNA3', 'ELET3', 'FFTL4', 'ITSA4', 'ITUB3', 'ITUB4', 'POMO4', 'TNLP4', 'USIM5', 'VALE3', 'VALE5', 'BVMF3' " + ", 'PINE4', 'TMAR5', 'BEES3', 'PSSA3', 'ITSA3', 'POSI3', 'USIM3', 'PETR3', 'PETR4', 'ETER3', 'TLPP3', 'TLPP4', 'NATU3', 'GETI3', 'GETI4', 'AMAR3', 'VIVO4') " + " And data <= #2011-05-02# " + " GROUP BY CODIGO, DATA " + " HAVING(Count(1) > 1) " + ") " + "GROUP BY CODIGO " + "ORDER BY Min(data) ";

			objRS.AdicionarQuery(strSQL);
			objRS.ExecuteQuery();


			while (!objRS.EOF) {
				DadosRecalcular(true, true, true, true, true, true, true, true, true, true,
				true, Convert.ToDateTime(objRS.Field("Data")), "#" + Convert.ToString(objRS.Field("Codigo")) + "#");

				objRS.MoveNext();

			}

		}

	}
}
