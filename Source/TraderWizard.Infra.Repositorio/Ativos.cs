using System;
using System.Collections.Generic;
using DataBase;
using Dominio.Entidades;

namespace TraderWizard.Infra.Repositorio
{
    public class Ativos
    {
        private readonly Conexao _conexao;

        public Ativos(Conexao conexao)
        {
            _conexao = conexao;
        }

        public IList<Ativo> Validos()
        {
            cRS rs = new cRS(_conexao);

            //busca os ativos da tabela ativo
            rs.ExecuteQuery(" select Codigo, Descricao " + " from Ativo " + " WHERE NOT EXISTS " + "(" + " SELECT 1 " +
                            " FROM ATIVOS_DESCONSIDERADOS " + " WHERE ATIVO.CODIGO = ATIVOS_DESCONSIDERADOS.CODIGO " +
                            ")" + " order by Codigo");

            var ativos = new List<Ativo>();

            while (! rs.EOF)
            {
                ativos.Add(new Ativo(Convert.ToString(rs.Field("Codigo")), Convert.ToString(rs.Field("Descricao"))));
                rs.MoveNext();
            }

            rs.Fechar();

            return ativos;

        }
    }
}
