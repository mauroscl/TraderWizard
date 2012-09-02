using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
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
