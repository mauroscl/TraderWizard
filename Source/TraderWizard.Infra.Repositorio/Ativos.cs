using System;
using System.Collections.Generic;
using DataBase;
using prjDominio.Entidades;
using prjModelo.Entidades;

namespace TraderWizard.Infra.Repositorio
{
    public class Ativos
    {
        private readonly cConexao _conexao;

        public Ativos(cConexao conexao)
        {
            _conexao = conexao;
        }

        public IList<cAtivo> Validos()
        {
            cRS rs = new cRS(_conexao);

            //busca os ativos da tabela ativo
            rs.ExecuteQuery(" select Codigo, Descricao " + " from Ativo " + " WHERE NOT EXISTS " + "(" + " SELECT 1 " +
                            " FROM ATIVOS_DESCONSIDERADOS " + " WHERE ATIVO.CODIGO = ATIVOS_DESCONSIDERADOS.CODIGO " +
                            ")" + " order by Codigo");

            var ativos = new List<cAtivo>();

            while (! rs.EOF)
            {
                ativos.Add(new cAtivo(Convert.ToString(rs.Field("Codigo")), Convert.ToString(rs.Field("Descricao"))));
                rs.MoveNext();
            }

            rs.Fechar();

            return ativos;

        }
    }
}
