using Dominio.Entidades;
using DTO;

namespace DataBase.Interfaces
{

	public interface ICarregadorMedia
	{

		MediaAbstract CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, MediaDTO pobjMediaDto1);

	}

}
