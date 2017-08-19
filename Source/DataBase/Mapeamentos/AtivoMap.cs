using Dominio.Entidades;
using FluentNHibernate.Mapping;

namespace DataBase.Mapeamentos
{
    public class AtivoMap: ClassMap<Ativo>
    {
        public AtivoMap()
        {
            Table("Ativo");
            Id(x => x.Codigo);
            Map(x => x.Descricao);
        }
    }
}
