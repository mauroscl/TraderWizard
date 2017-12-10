using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dominio.Entidades;
using Dominio.ValueObjects;
using DTO;

namespace DataBase.Carregadores
{
    public class CarregadorVolatilidade: CarregadorGenerico
    {
        public CarregadorVolatilidade(Conexao conexao): base(conexao)
        {
        }

        public ICollection<Volatilidade> CarregarVolatilidadeDiaria(string codigo)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Data, Valor ")
                .Append("FROM VolatilidadeDiaria ")
                .Append($"WHERE Codigo = {funcoesBd.CampoStringFormatar(codigo)} ")
                .Append("ORDER BY Data");

            return BuildVolatilidadePorAtivo(sb.ToString());

        }

        public IDictionary<string, List<Volatilidade>> CarregarVolatilidadeDiaria(DateTime dataInicialDados, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Codigo, Data, Valor ")
                .Append("FROM VolatilidadeDiaria ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicialDados)} ");

            if (ativos.Any())
            {
                sb.Append($" AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar).ToArray())})");
            }

            sb.Append("ORDER BY Codigo, Data");

            return BuildVolatilidadePorData(sb.ToString());

        }

        public ICollection<Volatilidade> CarregarVolatilidadeSemanal(string codigo)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Data, Valor ")
                .Append("FROM VolatilidadeSemanal ")
                .Append($"WHERE Codigo = {funcoesBd.CampoStringFormatar(codigo)} ")
                .Append("ORDER BY Data");

            return BuildVolatilidadePorAtivo(sb.ToString());
        }

        public IDictionary<string, List<Volatilidade>> CarregarVolatilidadeSemanal(DateTime dataInicialDados, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Codigo, Data, Valor ")
                .Append("FROM VolatilidadeSemanal ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicialDados)} ");

            if (ativos.Any())
            {
                sb.Append($" AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar).ToArray())})");
            }

            sb.Append("ORDER BY Codigo, Data");

            return BuildVolatilidadePorData(sb.ToString());
        }

        public void ExcluirVolatilidadeDiaria(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM VolatilidadeDiaria ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
                sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }

        public void ExcluirMediaVolatilidadeDiaria(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM MediaVolatilidadeDiaria ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
               sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }


        public void ExcluirVolatilidadeSemanal(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM VolatilidadeSemanal ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
                sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }

        public void ExcluirMediaVolatilidadeSemanal(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM MediaVolatilidadeSemanal ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
                sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }

        private ICollection<Volatilidade> BuildVolatilidadePorAtivo(string query)
        {
            var rs = new RS(Conexao);
            rs.ExecuteQuery(query);

            var volatilidades = new Collection<Volatilidade>();

            while (!rs.Eof)
            {
                var volatilidade = new Volatilidade
                {
                    Data = Convert.ToDateTime(rs.Field("Data")),
                    Valor = Convert.ToDecimal(rs.Field("Valor"))
                };
                volatilidades.Add(volatilidade);
                rs.MoveNext();
            }

            rs.Fechar();

            return volatilidades;
        }



        private IDictionary<string, List<Volatilidade>> BuildVolatilidadePorData(string query)
        {
            var rs = new RS(Conexao);
            rs.ExecuteQuery(query);

            var volatilidades = new Collection<Volatilidade>();

            while (!rs.Eof)
            {
                var volatilidade = new Volatilidade
                {
                    Codigo = rs.Field("Codigo").ToString(),
                    Data = Convert.ToDateTime(rs.Field("Data")),
                    Valor = Convert.ToDecimal(rs.Field("Valor"))
                };
                volatilidades.Add(volatilidade);
                rs.MoveNext();
            }

            rs.Fechar();

            //ativos que não tiverem pelo menos 21 oscilações não pode ser calculada a volatilidade
            IDictionary<string, List<Volatilidade>> dictionary = volatilidades.GroupBy(o => o.Codigo)
                .Where(g => g.Count() >= 21)
                .ToDictionary(x => x.Key, x => x.ToList());

            return dictionary;
        }
    }
}
