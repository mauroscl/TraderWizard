using DataBase;
using Dominio.Entidades;
using prjDominio.ValueObjects;
using prjModelo.Carregadores;

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

			var objCarregador = new cCarregadorDeResumoDoIFRDiario(_conexao);

			IFRSimulacaoDiariaFaixaResumo objResumo = objCarregador.Carregar(pobjSimulacaoDiariaVO);

		    return objResumo != null && objResumo.PercentualAcertosComFiltro >= PercentualMinimo;
		}

	}
}
