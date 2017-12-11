using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataBase;
using DataBase.Carregadores;
using DataBase.Interfaces;
using Dominio.ValueObjects;
using DTO;

namespace TraderWizard.ServicosDeAplicacao
{
    public class CalculadorDeMediaDeNegocios
    {
        private readonly CotacaoDataService _cotacaoDataService;
        private readonly RepositorioMediaNegocios _repositorioMediaNegocios;
        private readonly Conexao _conexao;
        private const string CodigoPadrao = "PETR4";
        private const int Periodo = 21;

        public CalculadorDeMediaDeNegocios()
        {
            _cotacaoDataService = new CotacaoDataService();
            _conexao = new Conexao();
            _repositorioMediaNegocios = new RepositorioMediaNegocios(_conexao);
        }

        public void CalcularMediaNegociosDiaria(DateTime dataInicialCalculo, ICollection<string> ativos)
        {

            try
            {
                _conexao.BeginTrans();

                _repositorioMediaNegocios.ExcluirMediaNegociosDiaria(dataInicialCalculo, ativos);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "Cotacao");

                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoDiaria(_conexao);

                IDictionary<string, List<CotacaoNegocios>> dados = carregadorCotacao.CarregarNegociosAPartirDe(dataInicialDados, ativos);

                foreach (var dado in dados)
                {
                    var medias = CalcularMedias(dado.Key, dado.Value);
                    InserirMediaNegociosDiaria(medias);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        public void CalcularMediaNegociosSemanal(DateTime dataInicialCalculo, ICollection<string> ativos)
        {

            try
            {
                _conexao.BeginTrans();

                _repositorioMediaNegocios.ExcluirMediaNegociosSemanal(dataInicialCalculo, ativos);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "Cotacao_Semanal");

                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoSemanal(_conexao);

                IDictionary<string, List<CotacaoNegocios>> dados = carregadorCotacao.CarregarNegociosAPartirDe(dataInicialDados, ativos);

                foreach (var dado in dados)
                {
                    var medias = CalcularMedias(dado.Key, dado.Value);
                    InserirMediaNegociosSemanal(medias);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        private Collection<MediaNegocios> CalcularMedias(string codigo, ICollection<CotacaoNegocios> negocios)
        {
            int skip = 0;
            var medias = new Collection<MediaNegocios>();
            var valores = negocios.Skip(skip).Take(Periodo).ToArray();

            while (valores.Length == Periodo)
            {
                decimal media = valores.Average(x => x.Negocios);
                var data = valores.Last().Data;
                var mediaNegocio = new MediaNegocios
                {
                    Codigo = codigo,
                    Data = data,
                    Valor = media
                };

                medias.Add(mediaNegocio);

                valores = negocios.Skip(++skip).Take(Periodo).ToArray();
            }
            return medias;
        }

        private void InserirMediaNegociosDiaria(Collection<MediaNegocios> medias)
        {
            var command = new Command(_conexao);

            var funcoesBd = _conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();

            foreach (var media in medias)
            {
                sb
                    .Append("INSERT INTO MediaNegociosDiaria ")
                    .Append("(Codigo, Data, Valor) ")
                    .Append("VALUES ")
                    .Append($"({funcoesBd.CampoStringFormatar(media.Codigo)}, ")
                    .Append($"{funcoesBd.CampoDateFormatar(media.Data)}, ")
                    .Append($"{funcoesBd.CampoDecimalFormatar(media.Valor)})");

                command.Execute(sb.ToString());
                sb.Clear();
            }
        }

        private void InserirMediaNegociosSemanal(Collection<MediaNegocios> medias)
        {
            var command = new Command(_conexao);

            var funcoesBd = _conexao.ObterFormatadorDeCampo();

            var sb = new StringBuilder();
            foreach (var media in medias)
            {
                sb
                    .Append("INSERT INTO MediaNegociosSemanal ")
                    .Append("(Codigo, Data, Valor) ")
                    .Append("VALUES ")
                    .Append($"({funcoesBd.CampoStringFormatar(media.Codigo)}, ")
                    .Append($"{funcoesBd.CampoDateFormatar(media.Data)}, ")
                    .Append($"{funcoesBd.CampoDecimalFormatar(media.Valor)})");

                command.Execute(sb.ToString());
                sb.Clear();
            }
        }

    }
}
