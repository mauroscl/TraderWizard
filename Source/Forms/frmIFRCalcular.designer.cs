using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmIFRCalcular : System.Windows.Forms.Form
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
            this.pnlAtivosEscolher = new System.Windows.Forms.Panel();
            this.btnRemover = new System.Windows.Forms.Button();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.btnAdicionarTodos = new System.Windows.Forms.Button();
            this.lstAtivosEscolhidos = new System.Windows.Forms.ListBox();
            this.lstAtivosNaoEscolhidos = new System.Windows.Forms.ListBox();
            this.txtPeriodo = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Panel1.SuspendLayout();
            this.pnlAtivosEscolher.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 434);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(527, 40);
            this.Panel1.TabIndex = 3;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(446, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(371, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlAtivosEscolher
            // 
            this.pnlAtivosEscolher.Controls.Add(this.btnRemover);
            this.pnlAtivosEscolher.Controls.Add(this.btnRemoverTodos);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionar);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionarTodos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosEscolhidos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosNaoEscolhidos);
            this.pnlAtivosEscolher.Location = new System.Drawing.Point(12, 32);
            this.pnlAtivosEscolher.Name = "pnlAtivosEscolher";
            this.pnlAtivosEscolher.Size = new System.Drawing.Size(524, 399);
            this.pnlAtivosEscolher.TabIndex = 16;
            // 
            // btnRemover
            // 
            this.btnRemover.Location = new System.Drawing.Point(218, 189);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(75, 37);
            this.btnRemover.TabIndex = 4;
            this.btnRemover.Text = "Remover";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            // 
            // btnRemoverTodos
            // 
            this.btnRemoverTodos.Location = new System.Drawing.Point(219, 146);
            this.btnRemoverTodos.Name = "btnRemoverTodos";
            this.btnRemoverTodos.Size = new System.Drawing.Size(75, 37);
            this.btnRemoverTodos.TabIndex = 3;
            this.btnRemoverTodos.Text = "Remover Todos";
            this.btnRemoverTodos.UseVisualStyleBackColor = true;
            this.btnRemoverTodos.Click += new System.EventHandler(this.btnRemoverTodos_Click);
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Location = new System.Drawing.Point(219, 43);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionar.TabIndex = 2;
            this.btnAdicionar.Text = "Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // btnAdicionarTodos
            // 
            this.btnAdicionarTodos.Location = new System.Drawing.Point(218, 0);
            this.btnAdicionarTodos.Name = "btnAdicionarTodos";
            this.btnAdicionarTodos.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionarTodos.TabIndex = 1;
            this.btnAdicionarTodos.Text = "Adicionar Todos";
            this.btnAdicionarTodos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAdicionarTodos.UseVisualStyleBackColor = true;
            this.btnAdicionarTodos.Click += new System.EventHandler(this.btnAdicionarTodos_Click);
            // 
            // lstAtivosEscolhidos
            // 
            this.lstAtivosEscolhidos.FormattingEnabled = true;
            this.lstAtivosEscolhidos.Location = new System.Drawing.Point(309, 0);
            this.lstAtivosEscolhidos.Name = "lstAtivosEscolhidos";
            this.lstAtivosEscolhidos.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAtivosEscolhidos.Size = new System.Drawing.Size(198, 394);
            this.lstAtivosEscolhidos.Sorted = true;
            this.lstAtivosEscolhidos.TabIndex = 5;
            // 
            // lstAtivosNaoEscolhidos
            // 
            this.lstAtivosNaoEscolhidos.FormattingEnabled = true;
            this.lstAtivosNaoEscolhidos.Location = new System.Drawing.Point(3, 0);
            this.lstAtivosNaoEscolhidos.Name = "lstAtivosNaoEscolhidos";
            this.lstAtivosNaoEscolhidos.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAtivosNaoEscolhidos.Size = new System.Drawing.Size(198, 394);
            this.lstAtivosNaoEscolhidos.Sorted = true;
            this.lstAtivosNaoEscolhidos.TabIndex = 0;
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(78, 6);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Size = new System.Drawing.Size(135, 20);
            this.txtPeriodo.TabIndex = 17;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(9, 9);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(48, 13);
            this.Label2.TabIndex = 18;
            this.Label2.Text = "Período:";
            // 
            // frmIFRCalcular
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 474);
            this.Controls.Add(this.txtPeriodo);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.pnlAtivosEscolher);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmIFRCalcular";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Calcular IFR";
            this.Load += new System.EventHandler(this.frmIDiarioCalcular_Load);
            this.Panel1.ResumeLayout(false);
            this.pnlAtivosEscolher.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Panel pnlAtivosEscolher;
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.Button btnRemoverTodos;

		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.Button btnAdicionarTodos;
		private System.Windows.Forms.ListBox lstAtivosEscolhidos;
		private System.Windows.Forms.ListBox lstAtivosNaoEscolhidos;
		private System.Windows.Forms.TextBox txtPeriodo;
		private System.Windows.Forms.Label Label2;
	}
}
