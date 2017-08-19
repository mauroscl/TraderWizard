using Dominio.Entidades;
using Services;

namespace ServicoNegocio
{

	public class BuscaCotacaoValorMinimoAnterior
	{

	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;

	    public BuscaCotacaoValorMinimoAnterior(ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
	    {
	        _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
	    }

	    public CotacaoAbstract Buscar(CotacaoAbstract pobjCotacao)
		{

            var objAjustarCotacao = new AjustarCotacao(_servicoDeCotacaoDeAtivo);

            CotacaoAbstract objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((CotacaoDiaria)_servicoDeCotacaoDeAtivo.CotacaoAnterior(pobjCotacao), pobjCotacao.Data);

			//Procura uma cotação com valor anterior com valor mínimo menor que o da cotação atual.

			while (objCotacaoDoValorMinimoAnterior.ValorMinimo >= pobjCotacao.ValorMinimo) {
				objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((CotacaoDiaria) _servicoDeCotacaoDeAtivo.CotacaoAnterior(objCotacaoDoValorMinimoAnterior), pobjCotacao.Data);
			}

			return objCotacaoDoValorMinimoAnterior;

		}

	}
}
