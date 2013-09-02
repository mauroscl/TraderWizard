using TraderWizard.Enumeracoes;

namespace prjDTO
{



	public class cSetupIFR2SimularDTO
	{

		public cEnum.enumIFRTipo IFRTipo { get; set; }
        public cEnum.enumMediaTipo MediaTipo { get; set; }
		public bool SubirStopApenasAposRealizacaoParcial { get; set; }
		public bool ExcluirSimulacoesAnteriores { get; set; }

	}
}
namespace prjDTO
{

	public class cSetupIFR2SimularCodigoDTO : cSetupIFR2SimularDTO
	{

		public string Codigo { get; set; }


		public cSetupIFR2SimularCodigoDTO(cSetupIFR2SimularDTO pobjSetupIFR2SimularDTO, string pstrCodigo)
		{
			Codigo = pstrCodigo;

			IFRTipo = pobjSetupIFR2SimularDTO.IFRTipo;
			MediaTipo = pobjSetupIFR2SimularDTO.MediaTipo;
			SubirStopApenasAposRealizacaoParcial = pobjSetupIFR2SimularDTO.SubirStopApenasAposRealizacaoParcial;
			ExcluirSimulacoesAnteriores = pobjSetupIFR2SimularDTO.ExcluirSimulacoesAnteriores;

		}

	}
}
