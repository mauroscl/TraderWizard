using prjDTO;
using prjModelo.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorMedia
	{

		cMediaAbstract CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDto1);

	}

}
