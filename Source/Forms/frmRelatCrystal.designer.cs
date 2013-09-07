using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmRelatCrystal : System.Windows.Forms.Form
	{

		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing && components != null) {
					components.Dispose();
				}
			} finally {
				base.Dispose(disposing);
			}
		}

		//Required by the Windows Form Designer

		private System.ComponentModel.IContainer components = null;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            this.crvRelat = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.RelatBackTest1 = new TraderWizard.RelatBackTest();
            this.RelatBackTest2 = new TraderWizard.RelatBackTest();
            this.SuspendLayout();
            // 
            // crvRelat
            // 
            this.crvRelat.ActiveViewIndex = 0;
            this.crvRelat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvRelat.Cursor = System.Windows.Forms.Cursors.Default;
            this.crvRelat.Location = new System.Drawing.Point(2, 0);
            this.crvRelat.Name = "crvRelat";
            this.crvRelat.ReportSource = this.RelatBackTest1;
            this.crvRelat.Size = new System.Drawing.Size(236, 161);
            this.crvRelat.TabIndex = 34;
            // 
            // frmRelatCrystal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 240);
            this.Controls.Add(this.crvRelat);
            this.Name = "frmRelatCrystal";
            this.Text = "Visualizador de Relatórios";
            this.Load += new System.EventHandler(this.frmRelatCrystal_Load);
            this.ResumeLayout(false);

		}
		private CrystalDecisions.Windows.Forms.CrystalReportViewer crvRelat;
		private TraderWizard.RelatBackTest RelatBackTest2;
		private TraderWizard.RelatBackTest RelatBackTest1;
	}
}
