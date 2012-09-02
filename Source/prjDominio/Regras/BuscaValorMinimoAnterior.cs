using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;

namespace prjModelo.Regras
{

	public class BuscaCotacaoValorMinimoAnterior
	{

		public static cCotacaoAbstract Buscar(cCotacaoAbstract pobjCotacao)
		{

			cAjustarCotacao objAjustarCotacao = new cAjustarCotacao();

			cCotacaoAbstract objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria)  pobjCotacao.CotacaoAnterior(), pobjCotacao.Data);

			//Procura uma cotação com valor anterior com valor mínimo menor que o da cotação atual.

			while (objCotacaoDoValorMinimoAnterior.ValorMinimo >= pobjCotacao.ValorMinimo) {
				objCotacaoDoValorMinimoAnterior = objAjustarCotacao.ConverterCotacaoParaData((cCotacaoDiaria) objCotacaoDoValorMinimoAnterior.CotacaoAnterior(), pobjCotacao.Data);

			}

			return objCotacaoDoValorMinimoAnterior;

		}

	}
}
