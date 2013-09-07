using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmCotacaoExcluir : System.Windows.Forms.Form
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
            this.btnRemover = new System.Windows.Forms.Button();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.lstDataEscolhida = new System.Windows.Forms.ListBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnAdicionarTodos = new System.Windows.Forms.Button();
            this.lstDataNaoEscolhida = new System.Windows.Forms.ListBox();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRemover
            // 
            this.btnRemover.Location = new System.Drawing.Point(220, 201);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(75, 37);
            this.btnRemover.TabIndex = 11;
            this.btnRemover.Text = "Remover";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            // 
            // btnRemoverTodos
            // 
            this.btnRemoverTodos.Location = new System.Drawing.Point(221, 158);
            this.btnRemoverTodos.Name = "btnRemoverTodos";
            this.btnRemoverTodos.Size = new System.Drawing.Size(75, 37);
            this.btnRemoverTodos.TabIndex = 9;
            this.btnRemoverTodos.Text = "Remover Todos";
            this.btnRemoverTodos.UseVisualStyleBackColor = true;
            this.btnRemoverTodos.Click += new System.EventHandler(this.btnRemoverTodos_Click);
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Location = new System.Drawing.Point(221, 55);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionar.TabIndex = 8;
            this.btnAdicionar.Text = "Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // lstDataEscolhida
            // 
            this.lstDataEscolhida.FormattingEnabled = true;
            this.lstDataEscolhida.Location = new System.Drawing.Point(301, 12);
            this.lstDataEscolhida.Name = "lstDataEscolhida";
            this.lstDataEscolhida.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDataEscolhida.Size = new System.Drawing.Size(198, 225);
            this.lstDataEscolhida.TabIndex = 12;
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 246);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(511, 40);
            this.Panel1.TabIndex = 10;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(432, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(356, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnAdicionarTodos
            // 
            this.btnAdicionarTodos.Location = new System.Drawing.Point(220, 12);
            this.btnAdicionarTodos.Name = "btnAdicionarTodos";
            this.btnAdicionarTodos.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionarTodos.TabIndex = 7;
            this.btnAdicionarTodos.Text = "Adicionar Todos";
            this.btnAdicionarTodos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAdicionarTodos.UseVisualStyleBackColor = true;
            this.btnAdicionarTodos.Click += new System.EventHandler(this.btnAdicionarTodos_Click);
            // 
            // lstDataNaoEscolhida
            // 
            this.lstDataNaoEscolhida.FormattingEnabled = true;
            this.lstDataNaoEscolhida.Location = new System.Drawing.Point(12, 12);
            this.lstDataNaoEscolhida.Name = "lstDataNaoEscolhida";
            this.lstDataNaoEscolhida.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstDataNaoEscolhida.Size = new System.Drawing.Size(198, 225);
            this.lstDataNaoEscolhida.TabIndex = 6;
            // 
            // frmCotacaoExcluir
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 286);
            this.Controls.Add(this.btnRemover);
            this.Controls.Add(this.btnRemoverTodos);
            this.Controls.Add(this.btnAdicionar);
            this.Controls.Add(this.lstDataEscolhida);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.btnAdicionarTodos);
            this.Controls.Add(this.lstDataNaoEscolhida);
            this.MaximizeBox = false;
            this.Name = "frmCotacaoExcluir";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Excluir Cotações";
            this.Load += new System.EventHandler(this.frmCotacaoExcluir_Load);
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.Button btnRemoverTodos;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.ListBox lstDataEscolhida;
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnAdicionarTodos;
		private System.Windows.Forms.ListBox lstDataNaoEscolhida;
	}
}
