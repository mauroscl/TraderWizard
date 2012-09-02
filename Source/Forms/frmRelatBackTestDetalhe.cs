using Microsoft.VisualBasic;
using System;
using frwInterface;
namespace TraderWizard
{

	public partial class frmRelatBackTestDetalhe
	{

		/// <summary>
		/// Indica se a operação é de adição ou edição
		/// valores possíveis: ADICIONAR, EDITAR
		/// </summary>
		/// <remarks></remarks>

		private string strOperacao;

		private frmRelatBackTest frmGrid;

		public frmRelatBackTestDetalhe(frmRelatBackTest pfrmGrid)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			ComponentesAjustar();

			//inicializa o combo na primeirã opção
			cmbSetup.SelectedIndex = 0;

			strOperacao = "ADICIONAR";

			frmGrid = pfrmGrid;

		}


		public frmRelatBackTestDetalhe(frwInterface.cEstrutura.structBackTestSetup pstructBackTestSetup, frmRelatBackTest pfrmGrid)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			strOperacao = "EDITAR";

			cmbSetup.Text = mCotacao.SetupDescricaoGerar(pstructBackTestSetup.strCodigoSetup);


			if (pstructBackTestSetup.strCodigoSetup == "IFR2SOBREVEND" | pstructBackTestSetup.strCodigoSetup == "IFR2>MMA13") {
				chkAcimaMME49.Checked = pstructBackTestSetup.blnMME49Filtrar;

			}


			if (pstructBackTestSetup.strCodigoSetup == "IFR2SOBREVEND") {
				txtIFR2Maximo.Text = pstructBackTestSetup.dblIFR2SobrevendidoValorMaximo.ToString();

			}

			frmGrid = pfrmGrid;

			switch (pstructBackTestSetup.intRealizacaoParcialTipo) {

				case cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial:

					rdbSemRealizacaoParcial.Checked = true;

					break;
				case cEnum.enumRealizacaoParcialTipo.AlijamentoRisco:

					rdbRealizacaoParcialAlijamento.Checked = true;

					break;
				case cEnum.enumRealizacaoParcialTipo.PercentualFixo:

					rdbPercentualFixo.Checked = true;

					txtPercentualFixo.Text = pstructBackTestSetup.decPercentualRealizacaoParcialFixo.ToString();

					break;
				case cEnum.enumRealizacaoParcialTipo.PrimeiroLucro:

					rdbPrimeiroFechamentoPercentualMinimo.Checked = true;

					txtPrimeiroFechamentoPercentualMinimo.Text = pstructBackTestSetup.decPrimeiroLucroPercentualMinimo.ToString();

					break;
			}

		}


		private void ComponentesRealizaoParcialAjustar()
		{
			//agora ajusta os campos de acordo com a realização parcial

			if (rdbSemRealizacaoParcial.Checked | rdbRealizacaoParcialAlijamento.Checked) {
				txtPercentualFixo.Enabled = false;
				txtPrimeiroFechamentoPercentualMinimo.Enabled = false;


			} else if (rdbPercentualFixo.Checked) {
				txtPercentualFixo.Enabled = true;
				txtPrimeiroFechamentoPercentualMinimo.Enabled = false;


			} else if (rdbPrimeiroFechamentoPercentualMinimo.Checked) {
				txtPercentualFixo.Enabled = false;
				txtPrimeiroFechamentoPercentualMinimo.Enabled = true;

			}

		}


		private void ComponentesSetupAjustar()
		{
			//verifica qual o setup foi selecionado

			if (Strings.Mid(cmbSetup.Text, 1, 5) == "IFR 2") {
				grbIFR2.Enabled = true;

				//agora testa o setup específico para saber se é o IFR 2 sobrevendido
				if (cmbSetup.Text == "IFR 2 Sobrevendido") {
					txtIFR2Maximo.Enabled = true;

				} else {
					txtIFR2Maximo.Enabled = false;
				}


			} else {
				//se não é setup de IFR 2 desabilita os campos relativos somente a estes setups
				grbIFR2.Enabled = false;

			}

		}


		private void ComponentesAjustar()
		{
			ComponentesSetupAjustar();

			ComponentesRealizaoParcialAjustar();

		}


		private void cmbSetup_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ComponentesSetupAjustar();

		}

		private void rdbSemRealizacaoParcial_CheckedChanged(object sender, System.EventArgs e)
		{
			ComponentesRealizaoParcialAjustar();
		}

		private void rdbPercentualFixo_CheckedChanged(object sender, System.EventArgs e)
		{
			ComponentesRealizaoParcialAjustar();
		}

		private void rdbPrimeiroFechamentoPercentualMinimo_CheckedChanged(object sender, System.EventArgs e)
		{
			ComponentesRealizaoParcialAjustar();
		}

		private void rdbRealizacaoParcialAlijamento_CheckedChanged(object sender, System.EventArgs e)
		{
			ComponentesRealizaoParcialAjustar();
		}

		private bool Consistir()
		{


			if (cmbSetup.Text == "IFR 2 Sobrevendido") {
				//verifica se o campo que indica o valor máximo do IFR está preenchido

				if (!Information.IsNumeric(txtIFR2Maximo.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(39) + "IFR 2 abaixo de" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Critical, this.Text);

					return false;

				}

			}


			if (rdbPercentualFixo.Checked) {

				if (!Information.IsNumeric(txtPercentualFixo.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(39) + "Percentual Fixo" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Critical, this.Text);

					return false;

				}

			}


			if (rdbPrimeiroFechamentoPercentualMinimo.Checked) {

				if (!Information.IsNumeric(txtPrimeiroFechamentoPercentualMinimo.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(39) + "Fechamento c/ Perc. Mínimo de" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Critical, this.Text);

					return false;

				}

			}

			return true;

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (Consistir()) {
				cEstrutura.structBackTestSetup objBackTestSetup = default(cEstrutura.structBackTestSetup);

				switch (cmbSetup.Text) {

					case "MME 9.1":

						objBackTestSetup.strCodigoSetup = "MME9.1";
						objBackTestSetup.blnMME49Filtrar = false;

						break;
					case "MME 9.2":

						objBackTestSetup.strCodigoSetup = "MME9.2";
						objBackTestSetup.blnMME49Filtrar = false;

						break;
					case "MME 9.3":


						objBackTestSetup.strCodigoSetup = "MME9.3";
						objBackTestSetup.blnMME49Filtrar = false;

						break;
					case "IFR 2 Sobrevendido":

						objBackTestSetup.strCodigoSetup = "IFR2SOBREVEND";
						objBackTestSetup.blnMME49Filtrar = chkAcimaMME49.Checked;
						objBackTestSetup.dblIFR2SobrevendidoValorMaximo = Convert.ToDouble(txtIFR2Maximo.Text);

						break;
					case "IFR 2 acima MMA 13":

						objBackTestSetup.strCodigoSetup = "IFR2>MMA13";
						objBackTestSetup.blnMME49Filtrar = chkAcimaMME49.Checked;

						break;
					default:

						objBackTestSetup.strCodigoSetup = String.Empty;

						break;
				}


				if (rdbSemRealizacaoParcial.Checked) {
					objBackTestSetup.intRealizacaoParcialTipo = cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial;

				}


				if (rdbRealizacaoParcialAlijamento.Checked) {
					objBackTestSetup.intRealizacaoParcialTipo = cEnum.enumRealizacaoParcialTipo.AlijamentoRisco;

				}


				if (rdbPercentualFixo.Checked) {
					objBackTestSetup.intRealizacaoParcialTipo = cEnum.enumRealizacaoParcialTipo.PercentualFixo;
					objBackTestSetup.decPercentualRealizacaoParcialFixo = Convert.ToDecimal(txtPercentualFixo.Text);

				}


				if (rdbPrimeiroFechamentoPercentualMinimo.Checked) {
					objBackTestSetup.intRealizacaoParcialTipo = cEnum.enumRealizacaoParcialTipo.PrimeiroLucro;
					objBackTestSetup.decPrimeiroLucroPercentualMinimo = Convert.ToDecimal(txtPrimeiroFechamentoPercentualMinimo.Text);

				}

				frmGrid.GridAtualizar(strOperacao, objBackTestSetup);

			}

		}
	}
}
