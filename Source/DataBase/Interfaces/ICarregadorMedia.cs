using prjDominio.Entidades;
using prjDTO;
using prjModelo.Entidades;

namespace DataBase.Interfaces
{

	public interface ICarregadorMedia
	{

		MediaAbstract CarregarPorData(CotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDto1);

	}

}
