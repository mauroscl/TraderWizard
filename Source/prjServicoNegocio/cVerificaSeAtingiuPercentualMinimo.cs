using DataBase;
using prjModelo.Carregadores;
using prjModelo.Entidades;
using prjModelo.ValueObjects;

namespace prjServicoNegocio
{

	public class cVerificaSeAtingiuPercentualMinimo
	{

		private readonly cConexao objConexao;

		private const double PercentualMinimo = 80.0;
		public cVerificaSeAtingiuPercentualMinimo(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public bool Verificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{

			var objCarregador = new cCarregadorDeResumoDoIFRDiario(objConexao);

			cIFRSimulacaoDiariaFaixaResumo objResumo = objCarregador.Carregar(pobjSimulacaoDiariaVO);

		    return objResumo != null && objResumo.PercentualAcertosComFiltro >= PercentualMinimo;
		}

	}
}
