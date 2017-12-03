using System;

namespace Configuracao
{
	public static class GeradorNomeArquivo
	{

		public static string GerarUlrBoletimDiario(DateTime data)
		{
		    var url = BuscarConfiguracao.ObterUrlBoletimDiario();
		    return $"{url}?filelist=PR{data:yyMMdd}.zip";
		}

	    public static string GerarUrlCotacaoHistorica(DateTime data)
	    {
	        var urlBase = BuscarConfiguracao.ObterUrlCotacaoHistorica();
	        return $"{urlBase}COTAHIST_D{data:ddMMyyyy}.ZIP";
	    }


        public static string GerarNomeArquivoLocal(DateTime pdtmData)
		{
			return $"bdi{pdtmData:yyyyMMdd}.zip";

		}



	}
}
