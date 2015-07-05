using DataBase;
using prjDominio.ValueObjects;
using prjModelo.Carregadores;
using prjModelo.Entidades;

namespace prjServicoNegocio
{

	public class cVerificaSeAtingiuPercentualMinimo
	{

		private readonly Conexao objConexao;

		private const double PercentualMinimo = 80.0;
		public cVerificaSeAtingiuPercentualMinimo(Conexao pobjConexao)
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
