using System;
using System.Collections.Generic;

namespace TraderWizard.ServicosDeAplicacao
{
    public interface IImportadorCotacao
    {
        IEnumerable<CotacaoImportacao> ObterCotacoes(DateTime data, string codigoUnico, ICollection<string> ativosDesconsiderados);
    }
}