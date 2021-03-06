using System;
using System.Windows.Forms;
using DataBase;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace Forms
{

	public static class mdlGeral
	{

		public static bool ParametroConsultar(Conexao pobjConexao, string pstrParametro, out string pstrValorRet)
		{

			DadosDb objDadosDB = new DadosDb(pobjConexao, "Configuracao");

			objDadosDB.CampoAdicionar("Parametro", true, pstrParametro);

			objDadosDB.CampoAdicionar("Valor", false, "");

			objDadosDB.DadosBDConsultar();

			pstrValorRet = objDadosDB.CampoConsultar("Valor");

			return true;

		}

		/// <summary>
		/// Preenche o combo de tipos de proventos baseado no enum frwConfiguracao.enumProventoTipo
		/// </summary>
		/// <param name="pcmbProventoTipo">ComboBox que será preenchido</param>
		/// <remarks></remarks>

		public static void ComboProventoTipoPreencher(ComboBox pcmbProventoTipo)
		{

			try {
				pcmbProventoTipo.Items.Clear();

				pcmbProventoTipo.Items.Add(new ProventoTipo((int) cEnum.enumProventoTipo.Dividendo, "Dividendo"));

				pcmbProventoTipo.Items.Add(new ProventoTipo((int) cEnum.enumProventoTipo.JurosCapitalProprio, "Juros sobre Capital Próprio"));

				pcmbProventoTipo.Items.Add(new ProventoTipo((int) cEnum.enumProventoTipo.Rendimento, "Rendimento"));

				pcmbProventoTipo.Items.Add(new ProventoTipo((int) cEnum.enumProventoTipo.RestCapDin, "Rest. Cap. Din."));

				pcmbProventoTipo.SelectedIndex = 0;


			} catch (Exception ex) {
				MessageBox.Show("Erro ao preencher combo de tipo de provento. " + Environment.NewLine + "Descrição do erro:" + Environment.NewLine + ex.Message);

			}

		}

		public static cEnum.enumProventoTipo ComboProventoTipoCodigoRetornar(ComboBox pcmbProventoTipo)
		{

			var objProventoTipo = (ProventoTipo)pcmbProventoTipo.SelectedItem;
			return objProventoTipo.GetEnumProventoTipo;

		}

        public  static string ObtemCodigoDoAtivoSelecionadoNoCombo(string pstrCodigoComDescricao)
        {
            return pstrCodigoComDescricao.Substring(0, pstrCodigoComDescricao.IndexOf('-')).Trim();
        }

	}
}
