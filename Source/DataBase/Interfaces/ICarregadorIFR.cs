using prjDominio.Entidades;
using prjModelo.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorIFR
	{

		cIFR CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos1);

	}

}
