using System;
using prjDominio.Entidades;
using prjModelo.Entidades;
using Services;

namespace prjServicoNegocio
{

	public class cAjustarCotacao
	{

	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;

	    public cAjustarCotacao(ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
	    {
	        _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
	    }

	    public CotacaoDiaria ConverterCotacaoParaData(CotacaoDiaria pobjCotacao, DateTime pdtmDataDestino)
		{

			DateTime dtmDataInicial = default(DateTime);
			DateTime dtmDataFinal = default(DateTime);

			if (pobjCotacao.Data < pdtmDataDestino) {
				//Quando a data de origem é menor do que a data inicial tem que pegar um dia depois, 
				//pois se houver desdobramentos nesta data, o valor de origem já está ajustado para esse desdobramento
				dtmDataInicial = pobjCotacao.Data.AddDays(1);
				dtmDataFinal = pdtmDataDestino;
			} else if (pobjCotacao.Data > pdtmDataDestino) {
				//Quando a data de destino é menor do que a data de origem, tem que pegar um dia depois,
				//pois se houver desdobramentos nesta data, o valor de destino já está ajustado para esse desdobramento
				dtmDataInicial = pdtmDataDestino.AddDays(1);
				dtmDataFinal = pobjCotacao.Data;
			} else {
                throw new Exception("Não existe necessidade de converter cotações pois as datas origem e destino são iguais: " + pobjCotacao.Data.ToString("dd/MM/yyyy"));
			    
			}

			var lstDesdobramentos = _servicoDeCotacaoDeAtivo.RetornaListaParcialDeDesdobramentos(dtmDataInicial, dtmDataFinal);

			if (lstDesdobramentos.Count == 0) {
				//Se não tem desdobramentos retorna a mesma cotação
				return pobjCotacao;
			} else {
				var objCotacaoConvertida = pobjCotacao.Clonar();
				foreach (Desdobramento objDesdobramento in lstDesdobramentos) {
					objDesdobramento.ConverterCotacao(objCotacaoConvertida);
				}

				return objCotacaoConvertida;

			}

		}


	}
}
