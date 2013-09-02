using prjDominio.Entidades;
using prjModelo.Entidades;

namespace prjServicoNegocio
{

	public class BuscaCotacaoValorMinimoAnterior
	{

	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;

	    public BuscaCotacaoValorMinimoAnterior(ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
	    {
	        _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
	    }

	    public cCotacaoAbstract Buscar(cCotacaoAbstract pobjCotacao)
		{

            var objAjustarCotacao = new cAjustarCotacao(_servicoDeCotacaoDeAtivo);

            cCotacaoAbstract objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria)_servicoDeCotacaoDeAtivo.CotacaoAnterior(pobjCotacao), pobjCotacao.Data);

			//Procura uma cotação com valor anterior com valor mínimo menor que o da cotação atual.

			while (objCotacaoDoValorMinimoAnterior.ValorMinimo >= pobjCotacao.ValorMinimo) {
				objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria) _servicoDeCotacaoDeAtivo.CotacaoAnterior(objCotacaoDoValorMinimoAnterior), pobjCotacao.Data);
			}

			return objCotacaoDoValorMinimoAnterior;

		}

	}
}
