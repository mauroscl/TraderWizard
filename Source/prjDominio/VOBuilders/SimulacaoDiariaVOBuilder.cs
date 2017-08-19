using Dominio.Entidades;
using prjDominio.ValueObjects;

namespace prjDominio.VOBuilders
{

	public class SimulacaoDiariaVOBuilder
	{

		public SimulacaoDiariaVO Build(cIFRSimulacaoDiariaDetalhe pobjSimulacaoDetalhe)
		{

			var objSimulacaoDiariaVO = new SimulacaoDiariaVO();

			objSimulacaoDiariaVO.Setup = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.Setup;
			objSimulacaoDiariaVO.Ativo = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.Ativo;
			objSimulacaoDiariaVO.ClassificacaoMedia = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.ClassificacaoMedia;
			objSimulacaoDiariaVO.DataEntradaEfetiva = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.DataEntradaEfetiva;
			objSimulacaoDiariaVO.IFRSobrevendido = pobjSimulacaoDetalhe.IFRSobreVendido;
			objSimulacaoDiariaVO.NumTentativas = pobjSimulacaoDetalhe.NumTentativas;

			return objSimulacaoDiariaVO;

		}


	}
}
