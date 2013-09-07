namespace DataBase
{

	public partial class frmInformacao
	{

		private void cmdFechar_Click(System.Object sender, System.EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}



		public frmInformacao(string pstrInformacao)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			txtInformacao.Text = pstrInformacao;

		}

	}
}
