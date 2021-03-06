﻿using Configuracao;
using DataBase;
using DataBase.Carregadores;
using DTO;
using Ionic.Zip;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using WebAccess;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ImportadorBoletimDiario : IImportadorCotacao
    {
        private readonly Web _web;
        private readonly SequencialService _sequencialService;
        private readonly CarregadorDeAtivo _carregadorDeAtivo;

        public ImportadorBoletimDiario()
        {
            var conexao = new Conexao();
            this._web = new Web(conexao);
            this._sequencialService = new SequencialService(conexao);
            this._carregadorDeAtivo = new CarregadorDeAtivo();
        }

        public IEnumerable<CotacaoImportacao> ObterCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados)
        {
            var pathLocal = $"{BuscarConfiguracao.ObtemCaminhoPadrao()}Arquivos";
            var caminhoDoArquivo = ObterArquivoDeCotacao(pathLocal, data);
            //criar cotações 
            IEnumerable<CotacaoImportacao> cotacoes = TransformarXmlEmCotacoes(data, codigoUnico, ativosDesconsiderados, caminhoDoArquivo);
            var fileService = new FileService();
            fileService.DeleteAllFiles($"{pathLocal}\\unzip");

            return cotacoes;

        }

        private string ObterArquivoDeCotacao(String pathLocal, DateTime data)
        {
            var pathPorData = $"PR{data:yyMMdd}";
            var nomeArquivoCotacoes = $"{pathPorData}.zip";
            string nomeArquivoDownload = $"pesquisa-pregao-{data:yyMMdd}.zip";
            var pathArquivoLocal = $"{pathLocal}\\{nomeArquivoDownload}";
            var pathToExtract = $"{pathLocal}\\unzip";
            var pathArquivoCotacoes = $"{pathToExtract}\\{nomeArquivoCotacoes}";
            var pathXml = $"{pathToExtract}\\{pathPorData}";
            //baixar arquivo zip
            if (!File.Exists(pathArquivoLocal))
            {
                var url = GeradorNomeArquivo.GerarUlrBoletimDiario(data);
                if (!_web.DownloadWithProxy(url, pathLocal, nomeArquivoDownload))
                {
                    throw new Exception($"Não foi possível baixar o arquivo de cotações na data {data:d}.");
                }

            }
            //extrair zip dentro do zip
            var zipFile1 = new ZipFile(pathArquivoLocal);
            zipFile1.ExtractAll(pathToExtract);

            //extrair XMLs dentro do zip
            var zipFile2 = new ZipFile(pathArquivoCotacoes);
            zipFile2.ExtractAll(pathXml);

            var ultimoArquivoCriado = new DirectoryInfo(pathXml)
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First()
                .FullName;

            return ultimoArquivoCriado;

        }

        private IEnumerable<CotacaoImportacao> TransformarXmlEmCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados, string xmlFilePath)
        {
            ICollection<SequencialAtivo> sequenciais;
            if (!string.IsNullOrEmpty(codigoUnico))
            {
                SequencialAtivo sequencial = _sequencialService.AtivoProximoSequencialCalcular(codigoUnico, data);
                sequenciais = new Collection<SequencialAtivo> { sequencial };
            }
            else
            {
                sequenciais = _sequencialService.AtivosProximoSequencialCalcular(ativosDesconsiderados);
            }

            ICollection<AtivoSelecao> ativosCadastrados = this._carregadorDeAtivo.Carregar().ToList();

            var pattern = new Regex("^[A-Z]{1}.{1}[A-Z]{2}\\d{1,2}$");
            String[] prefixosMercadoFuturo = { "WIN", "WDO", "DOL", "IND", "BGI", "CCM", "ICF", "WSP", "ISP" };
            var xmldoc = new XmlDocument();
            var cotacoes = new Collection<CotacaoImportacao>();
            using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    xmldoc.Load(fs);
                    XmlNodeList nodes = xmldoc.GetElementsByTagName("PricRpt");
                    var cultureInfo = new CultureInfo("en-US");
                    int i = 0;
                    while (i < nodes.Count && !CodigoUnicoEncontrado(codigoUnico, cotacoes))
                    {
                        XmlNode nodoPriceReport = nodes[i];

                        var codigo = nodoPriceReport.ChildNodes[1].ChildNodes[0].InnerText;
                        var nodoFinanceiro = nodoPriceReport.ChildNodes[4];

                        var childFinanceiro = nodoFinanceiro.ChildNodes.OfType<XmlElement>().AsQueryable();
                        bool temOscilacao = childFinanceiro.Any(x => x.Name == "OscnPctg");
                        bool temAjusteContrato = childFinanceiro.Any(x => x.Name == "AdjstdValCtrct");

                        var elementoQuantidadeNegocios = childFinanceiro.SingleOrDefault(x => x.Name == "RglrTxsQty");
                        var quantidadeDeNegocios = elementoQuantidadeNegocios != null
                            ? Convert.ToInt64(elementoQuantidadeNegocios.InnerText)
                            : 0;

                        if ((string.IsNullOrEmpty(codigoUnico) || codigoUnico.Equals(codigo))
                            && codigo.Length <= 6 && pattern.IsMatch(codigo) && temOscilacao
                            && (quantidadeDeNegocios > 100 || ativosCadastrados.Any(ativoCadastrado => ativoCadastrado.Codigo.Equals(codigo)))
                            && !temAjusteContrato && !ativosDesconsiderados.Contains(codigo)
                            && prefixosMercadoFuturo.All(p => !codigo.StartsWith(p))
                            && !IsFundoImobiliario(codigo, ativosCadastrados))
                        {
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
                        i++;
                    }
                }
                finally
                {
                    fs.Close();
                }
            }
            return cotacoes;
        }

        private bool CodigoUnicoEncontrado(string codigoUnico, Collection<CotacaoImportacao> cotacoes)
        {
            return !string.IsNullOrEmpty(codigoUnico) && cotacoes.Count == 1;
        }

        private bool IsFundoImobiliario(string codigo, ICollection<AtivoSelecao> ativosCadastrados)
        {
            return (codigo.EndsWith("11") || codigo.EndsWith("12"))
                && ativosCadastrados.All(ativoCadastrado => !ativoCadastrado.Codigo.Equals(codigo));
        }
    }
}




