using prjModelo.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorIFR
	{

		cIFR CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos1);

	}

}
