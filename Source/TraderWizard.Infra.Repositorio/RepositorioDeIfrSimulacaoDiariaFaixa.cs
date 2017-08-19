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
            var objCommand = new cCommand(_conexao);

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            //O Campo ID da tabela IFR_Simulacao_Diaria_Faixa é do tipo IDENTITY
            string strSQL = "INSERT INTO IFR_Simulacao_Diaria_Faixa " + Environment.NewLine;
            strSQL = strSQL + "(Codigo, ID_Setup, ID_CM, ID_Criterio_CM, ID_IFR_Sobrevendido, Data, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo)" + Environment.NewLine;
            strSQL = strSQL + " VALUES " + Environment.NewLine;
            strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Codigo);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Setup.Id);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ClassificacaoDaMedia.ID);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.CriterioDeClassificacaoDaMedia.ID);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.IfrSobrevendido.Id);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.Data);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ValorMinimo);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.ValorMaximo);
            strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(ifrSimulacaoDiariaFaixa.NumTentativasMinimo) + ")";

            objCommand.Execute(strSQL);

            return _conexao.TransStatus;

        }


    }
}
