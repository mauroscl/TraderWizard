using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using prjDominio.ValueObjects;

namespace ServicoNegocio
{

	public class VerificaSeAtingiuPercentualMinimo

	{

		private readonly Conexao _conexao;

		private const double PercentualMinimo = 80.0;
		public VerificaSeAtingiuPercentualMinimo(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		public bool Verificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{

			var objCarregador = new CarregadorDeResumoDoIFRDiario(_conexao);

			IFRSimulacaoDiariaFaixaResumo objResumo = objCarregador.Carregar(pobjSimulacaoDiariaVO);

		    return objResumo != null && objResumo.PercentualAcertosComFiltro >= PercentualMinimo;
		}

	}
}
