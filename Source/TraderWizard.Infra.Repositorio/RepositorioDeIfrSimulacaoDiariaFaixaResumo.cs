using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase;
using Dominio.Entidades;

namespace TraderWizard.Infra.Repositorio
{
    public class RepositorioDeIfrSimulacaoDiariaFaixaResumo
    {
        private readonly Conexao _conexao;

        public RepositorioDeIfrSimulacaoDiariaFaixaResumo(Conexao conexao)
        {
            _conexao = conexao;
        }

        public void Salvar(IFRSimulacaoDiariaFaixaResumo resumo)
        {
            cCommand objCommand = new cCommand(_conexao);

            //Somente salva se houve algum trade com ou sem filtro.

            if (resumo.NumTradesSemFiltro > 0 || resumo.NumTradesComFiltro > 0)
            {

                FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

                string strSQL = "INSERT INTO IFR_Simulacao_Diaria_Faixa_Resumo" + Environment.NewLine;
                strSQL = strSQL + "(Codigo, ID_Setup, ID_CM, ID_IFR_Sobrevendido, Data, NumTradesSemFiltro, NumAcertosSemFiltro, PercentualAcertosSemFiltro " + Environment.NewLine;
                strSQL = strSQL + ", NumTradesComFiltro, NumAcertosComFiltro, PercentualAcertosComFiltro)" + Environment.NewLine;
                strSQL = strSQL + " VALUES ";
                strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(resumo.Ativo.Codigo);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.Setup.Id);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.ClassificacaoDaMedia.ID);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.IfrSobrevendido.Id);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.Data);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.NumTradesSemFiltro);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.NumAcertosSemFiltro);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.PercentualAcertosSemFiltro);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.NumTradesComFiltro);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.NumAcertosComFiltro);
                strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(resumo.PercentualAcertosComFiltro);
                strSQL = strSQL + ")";

                objCommand.Execute(strSQL);

            }

        }

    }
}
