using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Configuracao;
using DataBase;
using WebAccess;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ImportadorDadosHistoricos : IImportadorCotacao
    {

        private readonly Web _web;
        private readonly SequencialService _sequencialService;


        public ImportadorDadosHistoricos()
        {
            var conexao = new Conexao();
            this._web = new Web(conexao);
            this._sequencialService = new SequencialService(conexao);
        }

        public IEnumerable<CotacaoImportacao> ObterCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados)
        {
            //nome dado ao arquivo zip que for baixado

            string strPathZip = BuscarConfiguracao.ObtemCaminhoPadrao();

            strPathZip = strPathZip + "Arquivos";

            var url = BuscarConfiguracao.ObterUrlCotacaoHistorica();

            string strArquivoZipDestino = "COTAHIST_D" + data.ToString("ddMMyyyy") + ".ZIP";
            string strArquivoTextoDestino = "COTAHIST_D" + data.ToString("ddMMyyyy") + ".TXT";

            if (!_web.DownloadWithProxy(url + strArquivoZipDestino, strPathZip, strArquivoZipDestino))
            {
                throw new Exception("Não foi possível baixar o arquivo de cotações históricas.");
            }

            var arquivoTextoService = new ArquivoTextoService();
            var colLinha = arquivoTextoService.LerLinhas(strPathZip, strArquivoZipDestino, strArquivoTextoDestino);
            return CotacoesImportar(colLinha, ativosDesconsiderados);
        }


        /// <summary>
        /// Recebe uma collection com as linhas de um arquivo
        /// </summary>
        /// <returns>status da transação</returns>
        /// <remarks></remarks>
        private ICollection<CotacaoImportacao> CotacoesImportar(ICollection<string> linhas , ICollection<string> ativosDesconsiderados)
        {
            //utilizado para calcular o sequencial do ativo.

            const char strSeparadorDecimal = ',';

            var cotacoes = new Collection<CotacaoImportacao>();
            ICollection<SequencialAtivo> sequenciais = _sequencialService.AtivosProximoSequencialCalcular();

            //se foi possivel baixar...
            //percorre todas as linhas da collection e nas linhas que forem cotações de ativos insere no banco de dados

            foreach (var linha in linhas)
            {

                //busca código do ativo, posicao 13-24
                string codigoAtivo = linha.Substring(12, 12).Trim();
                //TOTAL DE NEGÓCIOS (148-152)
                long lngNegociosTotal = Convert.ToInt64(linha.Substring(147, 5));

                //os dois primeiros caracteres indicam o tipo de registro.
                //o tipo de registro 01 indica que é a cotação de um papel do mercado a vista
                //posição 11-12 indica o código BDI do papel. O código 02 indica que é um LOTE PADRÃO
                //posição 25 - 27 indica o tipo  de mercado do ativo
                //o tipo de mercado 010 é o mercado A VISTA
                if (linha.Substring(0, 2) + linha.Substring(10, 2) + linha.Substring(24, 3) == "0102010" 
                    && !ativosDesconsiderados.Contains(codigoAtivo)
                    && lngNegociosTotal > 0)
                {



                    //busca a data da cotação: 3-10 no formato YYYYMMDD
                    var dtmCotacaoData = new DateTime(Convert.ToInt32(linha.Substring(2, 4)), Convert.ToInt32(linha.Substring(6, 2)), Convert.ToInt32(linha.Substring(8, 2)));

                    //busca valor de abertura do ativo: 57 - 67 (inteiro), 68-69 (decimal)
                    decimal decValorAbertura = Convert.ToDecimal(linha.Substring(56, 11) + strSeparadorDecimal + linha.Substring(67, 2));

                    //busca o valor máximo do ativo: 70-80 (inteiro), 81-82 (decimal)
                    decimal decValorMaximo = Convert.ToDecimal(linha.Substring(69, 11) + strSeparadorDecimal + linha.Substring(80, 2));

                    //busca o valor mínimo do ativo: 83-93 (inteiro), 94-95 (decimal)
                    decimal decValorMinimo = Convert.ToDecimal(linha.Substring(82, 11) + strSeparadorDecimal + linha.Substring(93, 2));

                    //busca o valor médio do ativo: 96-106 (inteiro), 107-108 (decimal)
                    decimal decValorMedio = Convert.ToDecimal(linha.Substring(95, 11) + strSeparadorDecimal + linha.Substring(106, 2));

                    //busca o valor de fechamento do ativo: 109-119 (inteiro), 120-121 (decimal)
                    decimal decValorFechamento = Convert.ToDecimal(linha.Substring(108, 11) + strSeparadorDecimal + linha.Substring(119, 2));

                    //TOTAL DE TÍTULOS NEGOCIADOS (153-170)
                    long lngTitulosTotal = Convert.ToInt64(linha.Substring(152, 18));

                    //VALOR TOTAL NEGOCIADO: 171-186 (inteiro), 187-188 (decimal)
                    decimal decValorTotal = Convert.ToDecimal(linha.Substring(170, 16) + strSeparadorDecimal + linha.Substring(186, 2));

                    //calcula o sequencial do ativo
                    var sequencialAtivo = sequenciais.SingleOrDefault(s => s.Codigo.Equals(codigoAtivo));
                    long sequencial = sequencialAtivo?.Sequencial ?? 1;

                    var cotacao = new CotacaoImportacao
                    {
                        Codigo = codigoAtivo,
                        Sequencial = sequencial,
                        Data = dtmCotacaoData,
                        QuantidadeNegociada = lngTitulosTotal,
                        VolumeFinanceiro = decValorTotal,
                        ValorMinimo = decValorMinimo,
                        ValorMaximo = decValorMaximo,
                        ValorAbertura = decValorAbertura,
                        ValorFechamento = decValorFechamento,
                        PrecoMedio = decValorMedio
                    };

                    cotacoes.Add(cotacao);

                }
                //se é uma cotação à vista.
            }

            return cotacoes;
        }


    }
}
