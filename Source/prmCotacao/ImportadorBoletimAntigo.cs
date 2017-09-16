using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuracao;
using DataBase;
using Dominio.Regras;
using Services;
using WebAccess;

namespace TraderWizard.ServicosDeAplicacao
{
    class ImportadorBoletimAntigo
    {
        private readonly Web _web;

        public ImportadorBoletimAntigo()
        {
            this._web = new Web();
        }

        public IEnumerable<CotacaoImportacao> ObterCotacoes(DateTime data, ICollection<string> ativosDesconsiderados, string codigoUnico)
        {
            string[] tiposDeCotacao = { "0202010", "0205010" };
            const char strSeparadorDecimal = ',';

            string strCodigoAtivo = String.Empty;

            decimal decValorFechamento = default(decimal);
            decimal decValorMinimo = default(decimal);
            decimal decValorMedio = default(decimal);
            decimal decValorMaximo = default(decimal);

            decimal decOscilacao = default(decimal);

            long lngNegociosTotal = 0;
            long lngTitulosTotal = 0;
            decimal decValorTotal = default(decimal);

            if (ArquivoDataBaixar(data))
            {
                //se foi possivel baixar...
                //percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados
                var arquivoTextoService = new ArquivoTextoService();
                var linhas = arquivoTextoService.LerLinhas("", "", "");

                foreach (var linha in linhas)
                {
                    //os dois primeiros caracteres indicam o tipo de registro.
                    //o tipo de registro 02 indica que é a cotação de um papel
                    //o terceiro e quarto caracteres indicam o código BDI do papel.
                    //O código 02 indica que é um papel do lote padrão
                    //posição 70 - 73 indica o tipo  de mercado do ativo
                    //o tipo de mercado 010 é o mercado A VISTA
                    bool blnImportarLinha;
                    bool blnInserir;
                    string strOscilacao;
                    decimal decValorAbertura = default(decimal);
                    if (tiposDeCotacao.Contains(linha.Substring(0, 4) + linha.Substring(69, 3)))
                    {
                        //se é a cotação de um papel
                        //e é do mercado à vista.

                        //busca código do ativo, posicao 58-69
                        strCodigoAtivo = linha.Substring(57, 12).Trim();


                        blnImportarLinha = string.IsNullOrWhiteSpace(codigoUnico)
                            ? ativosDesconsiderados.Contains(strCodigoAtivo)
                            : strCodigoAtivo == codigoUnico;

                        //verifica se deve importar a linha
                        if (blnImportarLinha)
                        {
                            //se o ativo não foi encontrado na lista dos desconsiderados.

                            //TOTAL DE NEGÓCIOS (174-178)
                            lngNegociosTotal = Convert.ToInt64(linha.Substring(173, 5));

                            //algumas ações não tem negócios no dia. Estas cotações não serão importadas.

                            if (lngNegociosTotal > 0)
                            {
                                //busca valor de abertura do ativo: 91 - 99 (inteiro), 100-101 (decimal)
                                decValorAbertura = Convert.ToDecimal(linha.Substring(90, 9) + strSeparadorDecimal + linha.Substring(99, 2));

                                //busca o valor máximo do ativo: 102-110 (inteiro), 111-112 (decimal)
                                decValorMaximo = Convert.ToDecimal(linha.Substring(101, 9) + strSeparadorDecimal + linha.Substring(110, 2));

                                //busca o valor mínimo do ativo: 113-121 (inteiro), 122-123 (decimal)
                                decValorMinimo = Convert.ToDecimal(linha.Substring(112, 9) + strSeparadorDecimal + linha.Substring(121, 2));

                                //busca o valor médio do ativo: 124-132 (inteiro), 133-134 (decimal)
                                decValorMedio = Convert.ToDecimal(linha.Substring(123, 9) + strSeparadorDecimal + linha.Substring(132, 2));

                                //busca o valor de fechamento do ativo: 135-143 (inteiro), 144-145 (decimal)
                                decValorFechamento = Convert.ToDecimal(linha.Substring(134, 9) + strSeparadorDecimal + linha.Substring(143, 2));

                                //busca a oscilação do papel em relação ao dia anterior
                                //146 = sinal da oscilação (+ ou -)
                                //147-149 = parte inteira da oscilação
                                //150-151 = parte decimal da oscilação

                                strOscilacao = linha.Substring(146, 3) + strSeparadorDecimal + linha.Substring(149, 2);


                                if (linha.Substring(145, 1) == "-")
                                {
                                    strOscilacao = "-" + strOscilacao;

                                }

                                decOscilacao = Convert.ToDecimal(strOscilacao);

                                //TOTAL DE TÍTULOS NEGOCIADOS (179-193)
                                lngTitulosTotal = Convert.ToInt64(linha.Substring(178, 15));

                                //VALOR TOTAL NEGOCIADO: 194-208 (inteiro), 209-210 (decimal)
                                decValorTotal = Convert.ToDecimal(linha.Substring(193, 15) + strSeparadorDecimal + linha.Substring(208, 2));

                                blnInserir = true;

                            }
                            else
                            {
                                //se não teve negócios no dia
                                blnInserir = false;

                            }
                            //if lngNegocios_Total > 0 then


                        }
                        else
                        {
                            //se o ativo deve ser desconsiderado
                            blnInserir = false;

                        }


                    }
                    else if (linha.Substring(0, 12) == "0101IBOVESPA")
                    {
                        if (codigoUnico == string.Empty)
                        {
                            blnImportarLinha = true;
                        }
                        else
                        {
                            blnImportarLinha = (codigoUnico == "IBOV");
                        }


                        if (blnImportarLinha)
                        {
                            //é o indice BOVESPA
                            strCodigoAtivo = "IBOV";

                            //busca valor de abertura do ativo: 35 - 40
                            decValorAbertura = Convert.ToDecimal(linha.Substring(34, 6));

                            //busca o valor máximo do ativo: 41-46
                            decValorMinimo = Convert.ToDecimal(linha.Substring(40, 6));

                            //busca o valor mínimo do ativo: 47-52
                            decValorMaximo = Convert.ToDecimal(linha.Substring(46, 6));

                            //busca o valor médio do ativo: 53-58
                            decValorMedio = Convert.ToDecimal(linha.Substring(52, 6));

                            //busca o valor de fechamento do ativo: 93-98
                            decValorFechamento = Convert.ToDecimal(linha.Substring(92, 6));

                            //busca a oscilação do papel em relação ao dia anterior
                            //99 = sinal da oscilação (+ ou -)
                            //100-102 = parte inteira da oscilação
                            //103-104 = parte decimal da oscilação

                            strOscilacao = linha.Substring(99, 3) + strSeparadorDecimal + linha.Substring(102, 2);


                            if (linha.Substring(98, 1) == "-")
                            {
                                strOscilacao = "-" + strOscilacao;

                            }

                            decOscilacao = Convert.ToDecimal(strOscilacao);

                            //TOTAL DE NEGÓCIOS (159-164)
                            lngNegociosTotal = Convert.ToInt64(linha.Substring(158, 6));

                            //TOTAL DE TÍTULOS NEGOCIADOS (165-179)
                            lngTitulosTotal = Convert.ToInt64(linha.Substring(164, 15));

                            //VALOR TOTAL NEGOCIADO: 180-194 (inteiro), 195-196 (decimal)
                            decValorTotal = Convert.ToDecimal(linha.Substring(179, 15) + strSeparadorDecimal + linha.Substring(194, 2));

                            blnInserir = true;


                        }
                        else
                        {
                            blnInserir = false;

                        }


                    }
                    else
                    {
                        //não é um ativo do mercado à vista, nem o indice BOVESPA
                        blnInserir = false;

                    }
                    //se é uma linha do mercado à vista.


                    //if (blnInserir)
                    //{
                    //    var objCommand = new Command(this._conexao);
                    //    var funcoesBd = this._conexao.ObterFormatadorDeCampo();
                    //    //calcula o sequencial do ativo
                    //    long lngSequencial = _sequencialService.SequencialCalcular(strCodigoAtivo, "Cotacao", objCommand.Conexao);

                    //    //insere na tabela
                    //    var dataFormatada = funcoesBd.CampoDateFormatar(data);

                    //    var insertBuilder = new StringBuilder()
                    //        .Append(" insert into Cotacao ")
                    //        .Append("(Codigo, Data, DataFinal, ValorAbertura, ValorFechamento, ValorMinimo, ValorMedio, ValorMaximo, Oscilacao, Negocios_Total, Titulos_Total, Valor_Total, Sequencial) ")
                    //        .Append(" VALUES ")
                    //        .AppendFormat("({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})",
                    //            funcoesBd.CampoStringFormatar(strCodigoAtivo), dataFormatada, dataFormatada, funcoesBd.CampoDecimalFormatar(decValorAbertura),
                    //            funcoesBd.CampoDecimalFormatar(decValorFechamento), funcoesBd.CampoDecimalFormatar(decValorMinimo), funcoesBd.CampoDecimalFormatar(decValorMedio),
                    //            funcoesBd.CampoDecimalFormatar(decValorMaximo), funcoesBd.CampoDecimalFormatar(decOscilacao),
                    //            lngNegociosTotal, lngTitulosTotal, funcoesBd.CampoDecimalFormatar(decValorTotal), lngSequencial);

                    //    objCommand.Execute(insertBuilder.ToString());

                    //}

                }


            }

            return new Collection<CotacaoImportacao>();

        }

        private bool ArquivoDataBaixar(DateTime pdtmData)
        {
            string strPathZip = BuscarConfiguracao.ObtemCaminhoPadrao();

            strPathZip = strPathZip + "Arquivos";

            //Verifica se existe o diretório para armazenar os arquivos baixados

            var fileService = new FileService();
            fileService.CreateFolder(strPathZip);

            //gera o nome do arquivo que deve ser baixado
            string url = GeradorNomeArquivo.GerarUlrBoletimDiario(pdtmData);

            //Nome que será dado ao arquivo baixado :"bdi" + yyyymmdd + ".zip"
            string strArquivoZipDestino = GeradorNomeArquivo.GerarNomeArquivoLocal(pdtmData);

            if (fileService.FileExists(strPathZip + "\\" + strArquivoZipDestino)) return true;
            return _web.DownloadWithProxy(url, strPathZip, strArquivoZipDestino);
        }


    }
}
