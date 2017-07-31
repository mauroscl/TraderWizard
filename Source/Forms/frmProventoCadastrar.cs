using Forms;
using System;
using System.Windows.Forms;
using DataBase;
using prmCotacao;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;

namespace TraderWizard
{

	public partial class frmProventoCadastrar
	{


		private Conexao objConexao;

		public frmProventoCadastrar(Conexao pobjConexao)
		{
			Load += frmProventoCadastrar_Load;
			InitializeComponent();

			objConexao = pobjConexao;

		}


		private void frmProventoCadastrar_Load(object sender, System.EventArgs e)
		{
			mCotacao.ComboAtivoPreencher(cmbAtivo, objConexao,"", true);

			mdlGeral.ComboProventoTipoPreencher(cmbProventoTipo);

			txtDataEx.Text = DateTime.Now.Date.ToString("dd/MM/yyyy");

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}

		private bool TelaValidar()
		{


            if (string.IsNullOrEmpty(cmbAtivo.Text.Trim()))
            {
                MessageBox.Show("É necessário escolher um ativo.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

				cmbAtivo.Focus();

				return false;

			}


			if (!txtDataAprovacao.Text.IsDate()) {
                MessageBox.Show("Campo \"Data de Aprovação\" não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				txtDataAprovacao.Focus();

				return false;

			}


			if (!txtDataEx.Text.IsDate()) {
                MessageBox.Show("Campo \"Data Ex\" não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				txtDataEx.Focus();

				return false;

			}


            if (txtValorPorAcao.Text.IsNumeric())
            {
                MessageBox.Show("Campo \"Valor por Ação\" não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

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

			var proventoService = new ProventoService();

		    this.Cursor = Cursors.WaitCursor;

			cEnum.enumRetorno intRetorno = proventoService.ProventoCadastrar(mCotacao.ObterCodigoDoAtivoSelecionado(cmbAtivo),  
                mdlGeral.ComboProventoTipoCodigoRetornar(cmbProventoTipo), Convert.ToDateTime(txtDataAprovacao.Text), 
                Convert.ToDateTime(txtDataEx.Text), Convert.ToDecimal(txtValorPorAcao.Text));

			this.Cursor = Cursors.Default;

			switch (intRetorno) {

				case cEnum.enumRetorno.RetornoOK:

                    MessageBox.Show("Operação executada com sucesso.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					break;
				case cEnum.enumRetorno.RetornoErroInesperado:

                    MessageBox.Show("Ocorreram erros ao executar a operação.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					break;
				case cEnum.enumRetorno.RetornoErro2:

                    MessageBox.Show("Não foi encontrada cotação na data ex do provento. Operação não pode ser executada.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					break;
			}

		}


		private void btnCalendarioAprovacao_Click(System.Object sender, System.EventArgs e)
		{
            if (txtDataAprovacao.Text.IsDate())
            {
				CalendarioAprovacao.SetDate( Convert.ToDateTime(txtDataAprovacao.Text));
			}

			CalendarioAprovacao.Show();
		}


		private void btnCalendarioEx_Click(System.Object sender, System.EventArgs e)
		{
            if (txtDataEx.Text.IsDate())
            {
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
