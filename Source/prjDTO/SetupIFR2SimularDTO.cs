using TraderWizard.Enumeracoes;

namespace DTO
{

	public class SetupIFR2SimularDto
	{

		public cEnum.enumIFRTipo IFRTipo { get; set; }
        public cEnum.enumMediaTipo MediaTipo { get; set; }
		public bool SubirStopApenasAposRealizacaoParcial { get; set; }
		public bool ExcluirSimulacoesAnteriores { get; set; }

	}
}
namespace DTO
{
}
