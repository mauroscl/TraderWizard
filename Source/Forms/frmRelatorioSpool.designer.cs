using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	[Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
	partial class frmRelatorioSpool:Form
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
            this.cmbAtivo = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.dgrRelatorioSpool = new System.Windows.Forms.DataGridView();
            this.btnRelarioVisualizar = new System.Windows.Forms.Button();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgrRelatorioSpool)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbAtivo
            // 
            this.cmbAtivo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbAtivo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAtivo.FormattingEnabled = true;
            this.cmbAtivo.Location = new System.Drawing.Point(48, 12);
            this.cmbAtivo.Name = "cmbAtivo";
            this.cmbAtivo.Size = new System.Drawing.Size(223, 21);
            this.cmbAtivo.TabIndex = 8;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(8, 15);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(34, 13);
            this.Label7.TabIndex = 7;
            this.Label7.Text = "Ativo:";
            // 
            // dgrRelatorioSpool
            // 
            this.dgrRelatorioSpool.AllowUserToAddRows = false;
            this.dgrRelatorioSpool.AllowUserToDeleteRows = false;
            this.dgrRelatorioSpool.AllowUserToResizeRows = false;
            this.dgrRelatorioSpool.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgrRelatorioSpool.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrRelatorioSpool.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgrRelatorioSpool.Location = new System.Drawing.Point(11, 39);
            this.dgrRelatorioSpool.MultiSelect = false;
            this.dgrRelatorioSpool.Name = "dgrRelatorioSpool";
            this.dgrRelatorioSpool.ReadOnly = true;
            this.dgrRelatorioSpool.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dgrRelatorioSpool.RowTemplate.ReadOnly = true;
            this.dgrRelatorioSpool.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrRelatorioSpool.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrRelatorioSpool.Size = new System.Drawing.Size(801, 339);
            this.dgrRelatorioSpool.TabIndex = 9;
            // 
            // btnRelarioVisualizar
            // 
            this.btnRelarioVisualizar.Location = new System.Drawing.Point(818, 85);
            this.btnRelarioVisualizar.Name = "btnRelarioVisualizar";
            this.btnRelarioVisualizar.Size = new System.Drawing.Size(75, 37);
            this.btnRelarioVisualizar.TabIndex = 23;
            this.btnRelarioVisualizar.Text = "&Visualizar Relatório...";
            this.btnRelarioVisualizar.UseVisualStyleBackColor = true;
            this.btnRelarioVisualizar.Click += new System.EventHandler(this.btnRelarioVisualizar_Click);
            // 
            // btnAtualizar
            // 
            this.btnAtualizar.Location = new System.Drawing.Point(818, 39);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(75, 37);
            this.btnAtualizar.TabIndex = 24;
            this.btnAtualizar.Text = "&Atualizar";
            this.btnAtualizar.UseVisualStyleBackColor = true;
            this.btnAtualizar.Click += new System.EventHandler(this.btnAtualizar_Click);
            // 
            // btnExcluir
            // 
            this.btnExcluir.Location = new System.Drawing.Point(818, 128);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(75, 37);
            this.btnExcluir.TabIndex = 25;
            this.btnExcluir.Text = "&Excluir";
            this.btnExcluir.UseVisualStyleBackColor = true;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // frmRelatorioSpool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 429);
            this.Controls.Add(this.btnExcluir);
            this.Controls.Add(this.btnAtualizar);
            this.Controls.Add(this.btnRelarioVisualizar);
            this.Controls.Add(this.dgrRelatorioSpool);
            this.Controls.Add(this.cmbAtivo);
            this.Controls.Add(this.Label7);
            this.MaximizeBox = false;
            this.Name = "frmRelatorioSpool";
            this.Text = "Spool de Relatórios";
            ((System.ComponentModel.ISupportInitialize)(this.dgrRelatorioSpool)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		internal System.Windows.Forms.ComboBox cmbAtivo;
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.DataGridView dgrRelatorioSpool;
	    internal System.Windows.Forms.Button btnRelarioVisualizar;
        internal System.Windows.Forms.Button btnAtualizar;
	    internal System.Windows.Forms.Button btnExcluir;
	}
}
