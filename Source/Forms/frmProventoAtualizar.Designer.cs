using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmProventoAtualizar : System.Windows.Forms.Form
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
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.grbFiltro = new System.Windows.Forms.GroupBox();
            this.txtDataFinal = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.grbDados = new System.Windows.Forms.GroupBox();
            this.txtDataUltimAtualizacao = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel1.SuspendLayout();
            this.grbFiltro.SuspendLayout();
            this.grbDados.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 260);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(472, 40);
            this.Panel1.TabIndex = 2;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(392, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(317, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grbFiltro
            // 
            this.grbFiltro.Controls.Add(this.txtDataFinal);
            this.grbFiltro.Controls.Add(this.Label2);
            this.grbFiltro.Location = new System.Drawing.Point(5, 5);
            this.grbFiltro.Name = "grbFiltro";
            this.grbFiltro.Size = new System.Drawing.Size(458, 249);
            this.grbFiltro.TabIndex = 0;
            this.grbFiltro.TabStop = false;
            this.grbFiltro.Text = "Filtros";
            // 
            // txtDataFinal
            // 
            this.txtDataFinal.Location = new System.Drawing.Point(13, 39);
            this.txtDataFinal.Name = "txtDataFinal";
            this.txtDataFinal.Size = new System.Drawing.Size(135, 20);
            this.txtDataFinal.TabIndex = 8;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(10, 23);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(58, 13);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "Data Final:";
            // 
            // grbDados
            // 
            this.grbDados.Controls.Add(this.txtDataUltimAtualizacao);
            this.grbDados.Controls.Add(this.Label1);
            this.grbDados.Location = new System.Drawing.Point(6, 134);
            this.grbDados.Name = "grbDados";
            this.grbDados.Size = new System.Drawing.Size(457, 115);
            this.grbDados.TabIndex = 1;
            this.grbDados.TabStop = false;
            this.grbDados.Text = "Dados";
            this.grbDados.Visible = false;
            // 
            // txtDataUltimAtualizacao
            // 
            this.txtDataUltimAtualizacao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDataUltimAtualizacao.Location = new System.Drawing.Point(13, 57);
            this.txtDataUltimAtualizacao.Name = "txtDataUltimAtualizacao";
            this.txtDataUltimAtualizacao.Size = new System.Drawing.Size(135, 20);
            this.txtDataUltimAtualizacao.TabIndex = 10;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(10, 41);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(138, 13);
            this.Label1.TabIndex = 11;
            this.Label1.Text = "Data da Última Atualização:";
            // 
            // frmProventoAtualizar
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(472, 300);
            this.Controls.Add(this.grbFiltro);
            this.Controls.Add(this.grbDados);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmProventoAtualizar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar Proventos";
            this.Panel1.ResumeLayout(false);
            this.grbFiltro.ResumeLayout(false);
            this.grbFiltro.PerformLayout();
            this.grbDados.ResumeLayout(false);
            this.grbDados.PerformLayout();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Panel Panel1;
	    private System.Windows.Forms.Button btnCancelar;
	    private System.Windows.Forms.Button btnOK;
		/*private System.Windows.Forms.Button withEventsField_btnCancelar;
		internal System.Windows.Forms.Button btnCancelar {
			get { return withEventsField_btnCancelar; }
			set {
				if (withEventsField_btnCancelar != null) {
					withEventsField_btnCancelar.Click -= btnCancelar_Click;
				}
				withEventsField_btnCancelar = value;
				if (withEventsField_btnCancelar != null) {
					withEventsField_btnCancelar.Click += btnCancelar_Click;
				}
			}
		}
		private System.Windows.Forms.Button withEventsField_btnOK;
		internal System.Windows.Forms.Button btnOK {
			get { return withEventsField_btnOK; }
			set {
				if (withEventsField_btnOK != null) {
					withEventsField_btnOK.Click -= btnOK_Click;
				}
				withEventsField_btnOK = value;
				if (withEventsField_btnOK != null) {
					withEventsField_btnOK.Click += btnOK_Click;
				}
			}
		}*/
		private System.Windows.Forms.GroupBox grbFiltro;
		private System.Windows.Forms.TextBox txtDataFinal;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.GroupBox grbDados;
		private System.Windows.Forms.TextBox txtDataUltimAtualizacao;
		private System.Windows.Forms.Label Label1;
		/*public frmProventoAtualizar()
		{
			InitializeComponent();
		}*/
	}
}
