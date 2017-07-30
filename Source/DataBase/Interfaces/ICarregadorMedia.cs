using prjDominio.Entidades;
using prjDTO;
using prjModelo.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorMedia
	{

		MediaAbstract CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, MediaDTO pobjMediaDto1);

	}

}
