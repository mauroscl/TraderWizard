using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DataBase.Interfaces;
using Dominio.Entidades;
using DTO;
using TraderWizard.Enumeracoes;

namespace DataBase.Carregadores
{
    public class CarregadorCotacaoSemanal  : CarregadorGenerico, ICarregadorCotacao
    {
        public CarregadorCotacaoSemanal(Conexao conexao): base(conexao)
        {
        }
        public IList<CotacaoDiaria> CarregarPorPeriodo(Ativo pobjAtivo, DateTime pdtmDataInicial, DateTime pdtmDataFinal, string pstrOrdem,
            IList<MediaDTO> psltMedias, bool pblnCarregarIFR)
        {
            throw new NotImplementedException();
        }

        public IList<CotacaoDiaria> CarregaComIFRSobrevendidoSemSimulacao(Ativo pobjAtivo, Setup pobjSetup, double pdblValorMaximoIFRSobrevendido,
            cEnum.enumMediaTipo pintMediaTipo)
        {
            throw new NotImplementedException();
        }

        public IList<CotacaoDiaria> CarregarParaIFRComFiltro(Ativo pobjAtivo, Setup pobjSetup, cEnum.enumMediaTipo pintMediaTipo, DateTime pdtmDataInicial)
        {
            throw new NotImplementedException();
        }

        public ICollection<CotacaoOscilacao> CarregarOscilacaoPorAtivo(string codigo)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Data, 1 + Oscilacao / 100 AS Oscilacao ")
                .Append("FROM Cotacao_Semanal ")
                .Append($"WHERE Codigo = {funcoesBd.CampoStringFormatar(codigo)} ")
                .Append("ORDER BY Data");

            var rs = new RS(Conexao);
            rs.ExecuteQuery(sb.ToString());

            var oscilacoes = new Collection<CotacaoOscilacao>();

            while (!rs.Eof)
            {
                var oscilacao = new CotacaoOscilacao
                {
                    Data = Convert.ToDateTime(rs.Field("Data")),
                    Oscilacao = Convert.ToDecimal(rs.Field("Oscilacao"))
                };
                oscilacoes.Add(oscilacao);
                rs.MoveNext();
            }

            rs.Fechar();

            return oscilacoes;
        }

        public IDictionary<string, List<CotacaoOscilacao>> CarregarOscilacaoAPartirDe(DateTime dataInicialDados)
        {
            var funcoesBd = Conexao.ObterFormatadorDeCampo();
            var sb = new StringBuilder();
            sb
                .Append("SELECT Codigo, Data, 1 + Oscilacao / 100 AS Oscilacao ")
                .Append("FROM Cotacao_Semanal ")
                .Append($"WHERE Data >= {funcoesBd.CampoDateFormatar(dataInicialDados)} ")
                .Append($"ORDER BY Codigo, Data");

            var rs = new RS(Conexao);
            rs.ExecuteQuery(sb.ToString());

            var oscilacoes = new Collection<CotacaoOscilacao>();

            while (!rs.Eof)
            {
                
                var oscilacao = new CotacaoOscilacao
                {
                    Codigo = rs.Field("Codigo").ToString(),
                    Data = Convert.ToDateTime(rs.Field("Data")),
                    Oscilacao = Convert.ToDecimal(rs.Field("Oscilacao"))
                };
                oscilacoes.Add(oscilacao);
                rs.MoveNext();
            }

            rs.Fechar();

            //ativos que não tiverem pelo menos 21 oscilações não pode ser calculada a volatilidade
            IDictionary<string, List<CotacaoOscilacao>> dictionary = oscilacoes.GroupBy(o => o.Codigo)
                .Where(g => g.Count() >= 21)
                .ToDictionary(x => x.Key, x => x.ToList());

            return dictionary;
        }
    }
}
