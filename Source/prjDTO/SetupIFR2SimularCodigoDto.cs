namespace DTO
{
    public class SetupIFR2SimularCodigoDto : SetupIFR2SimularDto
    {

        public string Codigo { get; set; }


        public SetupIFR2SimularCodigoDto(SetupIFR2SimularDto pobjSetupIFR2SimularDTO, string pstrCodigo)
        {
            Codigo = pstrCodigo;

            IFRTipo = pobjSetupIFR2SimularDTO.IFRTipo;
            MediaTipo = pobjSetupIFR2SimularDTO.MediaTipo;
            SubirStopApenasAposRealizacaoParcial = pobjSetupIFR2SimularDTO.SubirStopApenasAposRealizacaoParcial;
            ExcluirSimulacoesAnteriores = pobjSetupIFR2SimularDTO.ExcluirSimulacoesAnteriores;

        }

    }
}