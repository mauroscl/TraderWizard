using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBase.Carregadores
{
    public class RepositorioMediaNegocios: CarregadorGenerico
    {
        public RepositorioMediaNegocios(Conexao conexao): base(conexao)
        {
        }

        public void ExcluirMediaNegociosDiaria(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM MediaNegociosDiaria ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
               sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }

        public void ExcluirMediaNegociosSemanal(DateTime dataInicial, ICollection<string> ativos)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("DELETE ")
                .Append("FROM MediaNegociosSemanal ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicial)} ");

            if (ativos.Any())
            {
                sb.Append($"AND Codigo IN ({string.Join(", ", ativos.Select(funcoesBd.CampoStringFormatar))})");
            }

            var command = new Command(Conexao);
            command.Execute(sb.ToString());
        }

    }
}
