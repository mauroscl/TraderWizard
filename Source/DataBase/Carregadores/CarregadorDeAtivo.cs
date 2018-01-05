using System;
using System.Collections.Generic;
using DataBase.Interfaces;
using DTO;

namespace DataBase.Carregadores
{
    public class CarregadorDeAtivo : CarregadorGenerico, ICarregadorDeAtivo
    {

        public IEnumerable<AtivoSelecao> Carregar()
        {
            const string subQueryAtivosDesconsiderados = "SELECT CODIGO FROM ATIVOS_DESCONSIDERADOS ";
            const string query = "SELECT CODIGO, DESCRICAO " +
                                 "FROM ATIVO " +
                                 "WHERE CODIGO NOT IN (" + subQueryAtivosDesconsiderados + ")";

            var rs = new RS(this.Conexao);
            rs.ExecuteQuery(query);

            var ativos = new List<AtivoSelecao>();

            while (!rs.Eof)
            {
                string codigo = Convert.ToString(rs.Field("Codigo"));
                ativos.Add(new AtivoSelecao
                {
                    Codigo = codigo,
                    Descricao = $"{codigo} - {Convert.ToString(rs.Field("Descricao"))}"
                });

                rs.MoveNext();
            }

            rs.Fechar();

            return ativos;

        }


    }
}