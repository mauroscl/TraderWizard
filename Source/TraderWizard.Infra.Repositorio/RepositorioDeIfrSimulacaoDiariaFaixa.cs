using System;
using DataBase;
using Dominio.Entidades;

namespace TraderWizard.Infra.Repositorio
{
    public class RepositorioDeIfrSimulacaoDiariaFaixa
    {
        private readonly Conexao _conexao;

        public RepositorioDeIfrSimulacaoDiariaFaixa(Conexao conexao)
        {
            _conexao = conexao;
        }

        public bool Salvar(IFRSimulacaoDiariaFaixa  ifrSimulacaoDiariaFaixa)
        {
            var objCommand = new Command(_conexao);

            FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

            //O Campo ID da tabela IFR_Simulacao_Diaria_Faixa é do tipo IDENTITY
            string strSql = "INSERT INTO IFR_Simulacao_Diaria_Faixa " + Environment.NewLine;
            strSql = strSql + "(Codigo, ID_Setup, ID_CM, ID_Criterio_CM, ID_IFR_Sobrevendido, Data, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo)" + Environment.NewLine;
            strSql = strSql + " VALUES " + Environment.NewLine;
            strSql = strSql + "(" + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Codigo);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Setup.Id);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ClassificacaoDaMedia.ID);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.CriterioDeClassificacaoDaMedia.ID);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.IfrSobrevendido.Id);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Data);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ValorMinimo);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ValorMaximo);
            strSql = strSql + ", " + funcoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.NumTentativasMinimo) + ")";

            objCommand.Execute(strSql);

            return _conexao.TransStatus;

        }


    }
}
