using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
namespace DataBase
{
	partial class frmInformacao : System.Windows.Forms.Form
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

		private System.ComponentModel.IContainer components;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.cmdFechar = new System.Windows.Forms.Button();
			this.txtInformacao = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			//cmdFechar
			//
			this.cmdFechar.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.cmdFechar.Location = new System.Drawing.Point(325, 170);
			this.cmdFechar.Name = "cmdFechar";
			this.cmdFechar.Size = new System.Drawing.Size(98, 31);
			this.cmdFechar.TabIndex = 0;
			this.cmdFechar.Text = "Fechar";
			//
			//txtInformacao
			//
			this.txtInformacao.Location = new System.Drawing.Point(9, 11);
			this.txtInformacao.Multiline = true;
			this.txtInformacao.Name = "txtInformacao";
			this.txtInformacao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtInformacao.Size = new System.Drawing.Size(413, 153);
			this.txtInformacao.TabIndex = 1;
			//
			//frmInformacao
			//
			this.AcceptButton = this.cmdFechar;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(435, 209);
			this.Controls.Add(this.txtInformacao);
			this.Controls.Add(this.cmdFechar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmInformacao";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Informação";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		private System.Windows.Forms.Button withEventsField_cmdFechar;
		internal System.Windows.Forms.Button cmdFechar {
			get { return withEventsField_cmdFechar; }
			set {
				if (withEventsField_cmdFechar != null) {
					withEventsField_cmdFechar.Click -= cmdFechar_Click;
				}
				withEventsField_cmdFechar = value;
				if (withEventsField_cmdFechar != null) {
					withEventsField_cmdFechar.Click += cmdFechar_Click;
				}
			}
		}

		internal System.Windows.Forms.TextBox txtInformacao;
	}
}
