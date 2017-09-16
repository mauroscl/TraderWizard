using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Configuracao;
using DataBase;
using Ionic.Zip;
using Services;
using WebAccess;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ImportadorBoletimDiario : IImportadorCotacao
    {
        private readonly Web _web;
        private readonly SequencialService _sequencialService;

        public ImportadorBoletimDiario()
        {
            var conexao = new Conexao();
            this._web = new Web(conexao);
            this._sequencialService = new SequencialService(conexao);
        }

        public IEnumerable<CotacaoImportacao> ObterCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados)
        {
            var pathLocal = $"{BuscarConfiguracao.ObtemCaminhoPadrao()}Arquivos";
            var caminhoDoArquivo = ObterArquivoDeCotacao(pathLocal, data);
            //criar cotações 
            IEnumerable<CotacaoImportacao> cotacoes = TransformarXmlEmCotacoes(data, codigoUnico, ativosDesconsiderados, caminhoDoArquivo);
            var fileService = new FileService();
            fileService.DeleteAllFiles($"{pathLocal}\\unzip");

            Console.Write(cotacoes);
            //salvar cotações
            return cotacoes;

        }

        private string ObterArquivoDeCotacao(String pathLocal, DateTime data)
        {
            var nomeArquivoCotacoes = $"PR{data:yyMMdd}.zip";
            string nomeArquivoDownload = $"pesquisa-pregao-{data:yyMMdd}.zip";
            var pathArquivoLocal = $"{pathLocal}\\{nomeArquivoDownload}";
            var pathToExtract = $"{pathLocal}\\unzip";
            var pathArquivoCotacoes = $"{pathToExtract}\\{nomeArquivoCotacoes}";
            //baixar arquivo zip
            var url = $"http://www.bmfbovespa.com.br/pesquisapregao/download?filelist={nomeArquivoCotacoes}";
            if (!File.Exists(pathArquivoLocal))
            {
                if (!_web.DownloadWithProxy(url, pathLocal, nomeArquivoDownload))
                {
                    throw new Exception($"Não foi possível baixar o arquivo de cotações na data {data:d}.");
                }

            }
            if (!Directory.GetFiles(pathToExtract, "*.xml").Any())
            {
                //extrair primeiro arquivo
                var zipFile1 = new ZipFile(pathArquivoLocal);
                zipFile1.ExtractAll(pathToExtract);

                //extrair segundo arquivo
                var zipFile2 = new ZipFile(pathArquivoCotacoes);
                zipFile2.ExtractAll(pathToExtract);
            }

            //selecionar arquivo
            string fileToRead = Directory.GetFiles(pathToExtract, "*.xml").First();

            return fileToRead;

        }

        private IEnumerable<CotacaoImportacao> TransformarXmlEmCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados, string xmlFilePath)
        {

            var xmldoc = new XmlDocument();
            var cotacoes = new Collection<CotacaoImportacao>();
            FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read);
            try
            {
                
                xmldoc.Load(fs);
                XmlNodeList nodes = xmldoc.GetElementsByTagName("PricRpt");
                ICollection<SequencialAtivo> sequenciais = _sequencialService.AtivosProximoSequencialCalcular();
                for (int i = 0; i < nodes.Count; i++)
                {
                    XmlNode nodoPriceReport = nodes[i];

                    var codigo = nodoPriceReport.ChildNodes[1].ChildNodes[0].InnerText;
                    var pattern = new Regex("^[A-Z]{4}\\d{1,2}$");
                    var nodoFinanceiro = nodoPriceReport.ChildNodes[4];

                    var childFinanceiro = nodoFinanceiro.ChildNodes.OfType<XmlElement>().AsQueryable();
                    bool temOscilacao = childFinanceiro.Any(x => x.Name == "OscnPctg");
                    bool temAjusteContrato = childFinanceiro.Any(x => x.Name == "AdjstdValCtrct");

                    var elementoQuantidadeNegocios = childFinanceiro.SingleOrDefault(x => x.Name == "RglrTxsQty");
                    var quantidadeDeNegocios = elementoQuantidadeNegocios != null
                        ? Convert.ToInt64(elementoQuantidadeNegocios.InnerText)
                        : 0;

                    if ((string.IsNullOrEmpty(codigoUnico) || codigoUnico.Equals(codigo) ) 
                        && codigo.Length <= 6 && pattern.IsMatch(codigo) && temOscilacao && quantidadeDeNegocios > 100
                        && !temAjusteContrato && !ativosDesconsiderados.Contains(codigo))
                    {
                        var cultureInfo = new CultureInfo("en-US");
                        var quantidadeNegociada = Convert.ToInt64(childFinanceiro.Single(x => x.Name == "RglrTraddCtrcts").InnerText);
                        var volumeFinanceiro = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "NtlRglrVol").InnerText, cultureInfo);
                        var valorAbertura = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "FrstPric").InnerText, cultureInfo);
                        var valorMinimo = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "MinPric").InnerText, cultureInfo);
                        var valorMaximo = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "MaxPric").InnerText, cultureInfo);
                        var valorFechamento = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "LastPric").InnerText, cultureInfo);
                        var precoMedio = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "TradAvrgPric").InnerText, cultureInfo);
                        var oscilacao = Convert.ToDecimal(childFinanceiro.Single(x => x.Name == "OscnPctg").InnerText, cultureInfo);

                        var sequencialAtivo = sequenciais.FirstOrDefault(s => s.Codigo.Equals(codigo));
                        long sequencial = sequencialAtivo?.Sequencial ?? 1;
                        var cotacao = new CotacaoImportacao
                        {
                            Codigo = codigo,
                            Sequencial = sequencial,
                            Data = data,
                            QuantidadeNegocios = quantidadeDeNegocios,
                            QuantidadeNegociada = quantidadeNegociada,
                            VolumeFinanceiro = volumeFinanceiro,
                            ValorMinimo = valorMinimo,
                            ValorMaximo = valorMaximo,
                            ValorAbertura = valorAbertura,
                            ValorFechamento = valorFechamento,
                            PrecoMedio = precoMedio,
                            Oscilacao = oscilacao
                        };

                        cotacoes.Add(cotacao);

                    }

                }


            }
            finally
            {
                fs.Close();
            }
            

            return cotacoes;

        }

    }
}




