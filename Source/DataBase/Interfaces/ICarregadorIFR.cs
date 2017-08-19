using Dominio.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorIFR
	{

		cIFR CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos1);

	}

}
