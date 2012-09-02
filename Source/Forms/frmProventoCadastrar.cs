using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using DataBase;
using prmCotacao;
using frwInterface;

namespace TraderWizard
{

	public partial class frmProventoCadastrar
	{


		private cConexao objConexao;

		public frmProventoCadastrar(cConexao pobjConexao)
		{
			Load += frmProventoCadastrar_Load;
			InitializeComponent();

			objConexao = pobjConexao;

		}


		private void frmProventoCadastrar_Load(object sender, System.EventArgs e)
		{
			mCotacao.cmbAtivoPreencher(cmbAtivo, objConexao, true);

			mdlGeral.ComboProventoTipoPreencher(cmbProventoTipo);

			txtDataEx.Text = DateAndTime.Now.Date.ToString("dd/MM/yyyy");

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}

		private bool TelaValidar()
		{


			if (Strings.Trim(cmbAtivo.Text) == String.Empty) {
				Interaction.MsgBox("É necessário escolher um ativo.", MsgBoxStyle.Information, this.Text);

				cmbAtivo.Focus();

				return false;

			}


			if (!Information.IsDate(txtDataAprovacao.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Data de Aprovação" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, this.Text);

				txtDataAprovacao.Focus();

				return false;

			}


			if (!Information.IsDate(txtDataEx.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Data de Aprovação" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, this.Text);

				txtDataEx.Focus();

				return false;

			}


			if (!Information.IsNumeric(txtValorPorAcao.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Valor por Ação" + Strings.Chr(39) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, this.Text);

				txtValorPorAcao.Focus();

				return false;

			}

			return true;

		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (!TelaValidar()) {
				return;

			}

			cCotacao objCotacao = new cCotacao(objConexao);

			cEnum.enumRetorno intRetorno = default(cEnum.enumRetorno);

			this.Cursor = Cursors.WaitCursor;


			intRetorno = objCotacao.ProventoCadastrar(mCotacao.cmbAtivoCodigoRetornar(cmbAtivo),  mdlGeral.ComboProventoTipoCodigoRetornar(cmbProventoTipo), Convert.ToDateTime(txtDataAprovacao.Text), Convert.ToDateTime(txtDataEx.Text), Convert.ToDecimal(txtValorPorAcao.Text));

			this.Cursor = Cursors.Default;

			switch (intRetorno) {

				case cEnum.enumRetorno.RetornoOK:

					Interaction.MsgBox("Operação executada com sucesso.", MsgBoxStyle.Exclamation, this.Text);

					break;
				case cEnum.enumRetorno.RetornoErroInesperado:

					Interaction.MsgBox("Ocorreram erros ao executar a operação.", MsgBoxStyle.Exclamation, this.Text);

					break;
				case cEnum.enumRetorno.RetornoErro2:

					Interaction.MsgBox("Não foi encontrada cotação na data ex do provento. Operação não pode ser executada.", MsgBoxStyle.Exclamation, this.Text);

					break;
			}

		}


		private void btnCalendarioAprovacao_Click(System.Object sender, System.EventArgs e)
		{
			if (Information.IsDate(txtDataAprovacao.Text)) {
				CalendarioAprovacao.SetDate( Convert.ToDateTime(txtDataAprovacao.Text));
			}

			CalendarioAprovacao.Show();
		}


		private void btnCalendarioEx_Click(System.Object sender, System.EventArgs e)
		{
			if (Information.IsDate(txtDataEx.Text)) {
				CalendarioEx.SetDate(Convert.ToDateTime(txtDataEx.Text));
			}

			CalendarioEx.Show();
		}

		private void CalendarioAprovacao_DateSelected(System.Object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			txtDataAprovacao.Text = CalendarioAprovacao.SelectionRange.Start.ToString("dd/MM/yyyy");
			CalendarioAprovacao.Hide();
		}

		private void CalendarioEx_DateSelected(System.Object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			txtDataEx.Text = CalendarioEx.SelectionRange.Start.ToString("dd/MM/yyyy");
			CalendarioEx.Hide();
		}
	}
}
