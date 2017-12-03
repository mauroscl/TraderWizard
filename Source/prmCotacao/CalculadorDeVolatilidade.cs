using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataBase;
using DataBase.Carregadores;
using DataBase.Interfaces;
using Dominio.Regras;
using DTO;

namespace TraderWizard.ServicosDeAplicacao
{
    public class CalculadorDeVolatilidade
    {
        private readonly CalculoService _calculoService;
        private readonly CotacaoDataService _cotacaoDataService;
        private readonly CarregadorVolatilidade _carregadorVolatilidade;
        private readonly Conexao _conexao;
        private const string CodigoPadrao = "PETR4";
        private const int Periodo = 21;

        public CalculadorDeVolatilidade()
        {
            _calculoService = new CalculoService();
            _cotacaoDataService = new CotacaoDataService();
            _conexao = new Conexao();
            _carregadorVolatilidade = new CarregadorVolatilidade(_conexao);
        }

        public void CalcularVolatilidadeDiaria(string codigo)
        {

            try
            {
                _conexao.BeginTrans();

                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoDiaria(_conexao);

                ICollection<CotacaoOscilacao> oscilacoes = carregadorCotacao.CarregarOscilacaoPorAtivo(codigo);

                var volatilidades = CalcularVolatilidades(codigo, oscilacoes);

                InserirVolatilidadeDiaria(volatilidades);

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }

        }

        public void CalcularVolatilidadeDiaria(DateTime dataInicialCalculo)
        {
            try
            {
                _conexao.BeginTrans();
                _carregadorVolatilidade.ExcluirVolatilidadeDiaria(dataInicialCalculo);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "COTACAO");
                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoDiaria(_conexao);

                IDictionary<string, List<CotacaoOscilacao>> dados =
                    carregadorCotacao.CarregarOscilacaoAPartirDe(dataInicialDados);

                foreach (var dado in dados)
                {
                    var volatilidades = CalcularVolatilidades(dado.Key, dado.Value);
                    InserirVolatilidadeDiaria(volatilidades);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        public void CalcularVolatilidadeSemanal(string codigo)
        {

            try
            {
                _conexao.BeginTrans();

                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoSemanal(_conexao);

                ICollection<CotacaoOscilacao> oscilacoes = carregadorCotacao.CarregarOscilacaoPorAtivo(codigo);

                var volatilidades = CalcularVolatilidades(codigo, oscilacoes);

                InserirVolatilidadeSemanal(volatilidades);

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        public void CalcularVolatilidadeSemanal(DateTime dataInicialCalculo)
        {
            try
            {
                _conexao.BeginTrans();
                _carregadorVolatilidade.ExcluirVolatilidadeSemanal(dataInicialCalculo);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "COTACAO_SEMANAL");
                ICarregadorCotacao carregadorCotacao = new CarregadorCotacaoSemanal(_conexao);

                IDictionary<string, List<CotacaoOscilacao>> dados = carregadorCotacao.CarregarOscilacaoAPartirDe(dataInicialDados);

                foreach (var dado in dados)
                {
                    var volatilidades = CalcularVolatilidades(dado.Key, dado.Value);
                    InserirVolatilidadeSemanal(volatilidades);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }


        public void CalcularMediaVolatilidadeDiaria(string codigo)
        {

            try
            {
                _conexao.BeginTrans();

                ICollection<Volatilidade> volatilidades = _carregadorVolatilidade.CarregarVolatilidadeDiaria(codigo);

                var medias = CalcularMedias(codigo, volatilidades);

                InserirMediaVolatilidadeDiaria(medias);

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }

        }

        public void CalcularMediaVolatilidadeDiaria(DateTime dataInicialCalculo)
        {

            try
            {
                _conexao.BeginTrans();
                _carregadorVolatilidade.ExcluirMediaVolatilidadeDiaria(dataInicialCalculo);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "VolatilidadeDiaria");
                var carregadorCotacao = new CarregadorVolatilidade(_conexao);

                IDictionary<string, List<Volatilidade>> dados = carregadorCotacao.CarregarVolatilidadeDiaria(dataInicialDados);

                foreach (var dado in dados)
                {
                    var medias = CalcularMedias(dado.Key, dado.Value);
                    InserirMediaVolatilidadeDiaria(medias);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        public void CalcularMediaVolatilidadeSemanal(string codigo)
        {

            try
            {
                _conexao.BeginTrans();

                var carregadorCotacao = new CarregadorVolatilidade(_conexao);

                ICollection<Volatilidade> volatilidades = carregadorCotacao.CarregarVolatilidadeSemanal(codigo);

                var medias = CalcularMedias(codigo, volatilidades);

                InserirMediaVolatilidadeSemanal(medias);

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }

        public void CalcularMediaVolatilidadeSemanal(DateTime dataInicialCalculo)
        {

            try
            {
                _conexao.BeginTrans();
                _carregadorVolatilidade.ExcluirMediaVolatilidadeSemanal(dataInicialCalculo);
                DateTime dataInicialDados = _cotacaoDataService.CalcularDataInicial(CodigoPadrao, Periodo, dataInicialCalculo, "VolatilidadeSemanal");
                var carregadorCotacao = new CarregadorVolatilidade(_conexao);

                IDictionary<string, List<Volatilidade>> dados = carregadorCotacao.CarregarVolatilidadeSemanal(dataInicialDados);

                foreach (var dado in dados)
                {
                    var medias = CalcularMedias(dado.Key, dado.Value);
                    InserirMediaVolatilidadeSemanal(medias);
                }

                _conexao.CommitTrans();

            }
            catch
            {
                _conexao.RollBackTrans();
                throw;
            }
        }


        private void InserirMediaVolatilidadeSemanal(Collection<MediaVolatilidade> medias)
        {
            var command = new Command(_conexao);

            var funcoesBd = _conexao.ObterFormatadorDeCampo();

            var sb = new StringBuilder();
            foreach (var media in medias)
            {
                sb
                    .Append("INSERT INTO MediaVolatilidadeSemanal ")
                    .Append("(Codigo, Data, Valor) ")
                    .Append("VALUES ")
                    .Append($"({funcoesBd.CampoStringFormatar(media.Codigo)}, ")
                    .Append($"{funcoesBd.CampoDateFormatar(media.Data)}, ")
                    .Append($"{funcoesBd.CampoDecimalFormatar(media.Valor)})");

                command.Execute(sb.ToString());
                sb.Clear();
            }
        }

        private void InserirVolatilidadeDiaria(Collection<CotacaoVolatilidade> volatilidades)
        {
            var command = new Command(_conexao);
            var funcoesBd = _conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            foreach (var volatilidade in volatilidades)
            {
                sb
                    .Append("INSERT INTO VolatilidadeDiaria ")
                    .Append("(Codigo, Data, Valor) ")
                    .Append("VALUES ")
                    .Append($"({funcoesBd.CampoStringFormatar(volatilidade.Codigo)}, ")
                    .Append($"{funcoesBd.CampoDateFormatar(volatilidade.Data)}, ")
                    .Append($"{funcoesBd.CampoDecimalFormatar(volatilidade.Valor)})");

                command.Execute(sb.ToString());

                sb.Clear();
            }
        }

        private Collection<CotacaoVolatilidade> CalcularVolatilidades(string codigo, ICollection<CotacaoOscilacao> oscilacoes)
        {
            int skip = 0;
            var valores = oscilacoes.Skip(skip).Take(Periodo).ToArray();

            var volatilidades = new Collection<CotacaoVolatilidade>();

            while (valores.Length == Periodo)
            {
                decimal valor =
                    _calculoService.CalcularVolatilidadeHistorica(valores.Select(x => Convert.ToDouble(x.Oscilacao)));
                var data = valores.Last().Data;
                var cotacaoVolatilidade = new CotacaoVolatilidade
                {
                    Codigo = codigo,
                    Data = data,
                    Valor = valor
                };

                volatilidades.Add(cotacaoVolatilidade);

                valores = oscilacoes.Skip(++skip).Take(Periodo).ToArray();
            }
            return volatilidades;
        }

        private Collection<MediaVolatilidade> CalcularMedias(string codigo, ICollection<Volatilidade> volatilidades)
        {
            int skip = 0;
            var medias = new Collection<MediaVolatilidade>();
            var valores = volatilidades.Skip(skip).Take(Periodo).ToArray();

            while (valores.Length == Periodo)
            {
                decimal media = valores.Average(x => x.Valor);
                var data = valores.Last().Data;
                var mediaVolatilidade = new MediaVolatilidade
                {
                    Codigo = codigo,
                    Data = data,
                    Valor = media
                };

                medias.Add(mediaVolatilidade);

                valores = volatilidades.Skip(++skip).Take(Periodo).ToArray();
            }
            return medias;
        }

        private void InserirVolatilidadeSemanal(Collection<CotacaoVolatilidade> volatilidades)
        {
            var command = new Command(_conexao);

            var funcoesBd = _conexao.ObterFormatadorDeCampo();

            var sb = new StringBuilder();
            foreach (var volatilidade in volatilidades)
            {
                sb
                    .Append("INSERT INTO VolatilidadeSemanal ")
                    .Append("(Codigo, Data, Valor) ")
                    .Append("VALUES ")
                    .Append($"({funcoesBd.CampoStringFormatar(volatilidade.Codigo)}, ")
                    .Append($"{funcoesBd.CampoDateFormatar(volatilidade.Data)}, ")
                    .Append($"{funcoesBd.CampoDecimalFormatar(volatilidade.Valor)})");


                command.Execute(sb.ToString());

                sb.Clear();
            }
        }

        private void InserirMediaVolatilidadeDiaria(Collection<MediaVolatilidade> medias)
        {
            var command = new Command(_conexao);

            var funcoesBd = _conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();

            foreach (var media in medias)
            {
                sb
                    .Append("INSERT INTO MediaVolatilidadeDiaria ")
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
