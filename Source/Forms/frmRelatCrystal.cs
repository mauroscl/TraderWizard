using System.Drawing;

namespace TraderWizard
{
	public partial class frmRelatCrystal
	{


		private long lngcodRelatorio;

		public frmRelatCrystal(long plngCodRelatorio)
		{
			Load += frmRelatCrystal_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.

			lngcodRelatorio = plngCodRelatorio;

		}



		private void frmRelatCrystal_Load(object sender, System.EventArgs e)
		{
			this.Location = new Point(0, 0);
			this.Height = MdiParent.ClientSize.Height - 28;
			this.Width = MdiParent.ClientSize.Width - 4;

			crvRelat.Location = new Point(0, 0);
			crvRelat.Height = this.Height - 50;
			crvRelat.Width = this.Width - 10;
			//crvRelat.DisplayGroupTree = False

			crvRelat.SelectionFormula = "{RELATORIOS_SPOOL.COD_RELATORIO} = " + lngcodRelatorio.ToString();

		}
	}
}
