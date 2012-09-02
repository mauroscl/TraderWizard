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
	partial class frmIndicadorEscolha : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            this.lstPeriodoSelecionado = new System.Windows.Forms.ListView();
            this.colPeriodo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRemover = new System.Windows.Forms.Button();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtPeriodo = new System.Windows.Forms.TextBox();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.objColorDialog = new System.Windows.Forms.ColorDialog();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.pnlCor = new System.Windows.Forms.Panel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbAritmetica = new System.Windows.Forms.RadioButton();
            this.rdbExponencial = new System.Windows.Forms.RadioButton();
            this.ToolTipCancelar = new System.Windows.Forms.ToolTip(this.components);
            this.Panel1.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstPeriodoSelecionado
            // 
            this.lstPeriodoSelecionado.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPeriodo,
            this.colTipo,
            this.colCor});
            this.lstPeriodoSelecionado.FullRowSelect = true;
            this.lstPeriodoSelecionado.GridLines = true;
            this.lstPeriodoSelecionado.Location = new System.Drawing.Point(12, 74);
            this.lstPeriodoSelecionado.MultiSelect = false;
            this.lstPeriodoSelecionado.Name = "lstPeriodoSelecionado";
            this.lstPeriodoSelecionado.Size = new System.Drawing.Size(224, 144);
            this.lstPeriodoSelecionado.TabIndex = 3;
            this.lstPeriodoSelecionado.UseCompatibleStateImageBehavior = false;
            this.lstPeriodoSelecionado.View = System.Windows.Forms.View.Details;
            this.lstPeriodoSelecionado.SelectedIndexChanged += new System.EventHandler(this.lstPeriodoSelecionado_SelectedIndexChanged);
            // 
            // colPeriodo
            // 
            this.colPeriodo.Text = "Períodos";
            this.colPeriodo.Width = 94;
            // 
            // colTipo
            // 
            this.colTipo.Text = "Tipo";
            // 
            // colCor
            // 
            this.colCor.Text = "Cor";
            // 
            // btnRemover
            // 
            this.btnRemover.Location = new System.Drawing.Point(252, 74);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(75, 37);
            this.btnRemover.TabIndex = 4;
            this.btnRemover.Text = "&Remover";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(9, 15);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(48, 13);
            this.lblPeriodo.TabIndex = 2;
            this.lblPeriodo.Text = "Período:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(160, 15);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(26, 13);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Cor:";
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(63, 12);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Size = new System.Drawing.Size(91, 20);
            this.txtPeriodo.TabIndex = 0;
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Location = new System.Drawing.Point(252, 12);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionar.TabIndex = 2;
            this.btnAdicionar.Text = "&Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 235);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(334, 40);
            this.Panel1.TabIndex = 7;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(254, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(178, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // objColorDialog
            // 
            this.objColorDialog.AnyColor = true;
            // 
            // btnRemoverTodos
            // 
            this.btnRemoverTodos.Location = new System.Drawing.Point(252, 117);
            this.btnRemoverTodos.Name = "btnRemoverTodos";
            this.btnRemoverTodos.Size = new System.Drawing.Size(75, 37);
            this.btnRemoverTodos.TabIndex = 5;
            this.btnRemoverTodos.Text = "Remover &Todos";
            this.btnRemoverTodos.UseVisualStyleBackColor = true;
            this.btnRemoverTodos.Click += new System.EventHandler(this.btnRemoverTodos_Click);
            // 
            // pnlCor
            // 
            this.pnlCor.BackColor = System.Drawing.Color.Red;
            this.pnlCor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCor.Location = new System.Drawing.Point(192, 13);
            this.pnlCor.Name = "pnlCor";
            this.pnlCor.Size = new System.Drawing.Size(44, 19);
            this.pnlCor.TabIndex = 1;
            this.pnlCor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlCor_MouseClick);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.rdbAritmetica);
            this.GroupBox1.Controls.Add(this.rdbExponencial);
            this.GroupBox1.Location = new System.Drawing.Point(12, 34);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(223, 34);
            this.GroupBox1.TabIndex = 8;
            this.GroupBox1.TabStop = false;
            // 
            // rdbAritmetica
            // 
            this.rdbAritmetica.AutoSize = true;
            this.rdbAritmetica.Location = new System.Drawing.Point(146, 11);
            this.rdbAritmetica.Name = "rdbAritmetica";
            this.rdbAritmetica.Size = new System.Drawing.Size(71, 17);
            this.rdbAritmetica.TabIndex = 1;
            this.rdbAritmetica.Text = "Aritmética";
            this.rdbAritmetica.UseVisualStyleBackColor = true;
            // 
            // rdbExponencial
            // 
            this.rdbExponencial.AutoSize = true;
            this.rdbExponencial.Checked = true;
            this.rdbExponencial.Location = new System.Drawing.Point(6, 11);
            this.rdbExponencial.Name = "rdbExponencial";
            this.rdbExponencial.Size = new System.Drawing.Size(83, 17);
            this.rdbExponencial.TabIndex = 0;
            this.rdbExponencial.TabStop = true;
            this.rdbExponencial.Text = "Exponencial";
            this.rdbExponencial.UseVisualStyleBackColor = true;
            // 
            // ToolTipCancelar
            // 
            this.ToolTipCancelar.Active = false;
            this.ToolTipCancelar.IsBalloon = true;
            this.ToolTipCancelar.ShowAlways = true;
            // 
            // frmIndicadorEscolha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 275);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.pnlCor);
            this.Controls.Add(this.btnRemoverTodos);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.btnAdicionar);
            this.Controls.Add(this.txtPeriodo);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.lblPeriodo);
            this.Controls.Add(this.btnRemover);
            this.Controls.Add(this.lstPeriodoSelecionado);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmIndicadorEscolha";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmIndicadorEscolha";
            this.Panel1.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ListView lstPeriodoSelecionado;
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.ColumnHeader colPeriodo;
		private System.Windows.Forms.ColumnHeader colCor;
		private System.Windows.Forms.Label lblPeriodo;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.TextBox txtPeriodo;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ColorDialog objColorDialog;
		private System.Windows.Forms.Button btnRemoverTodos;
		private System.Windows.Forms.Panel pnlCor;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton rdbAritmetica;
		private System.Windows.Forms.RadioButton rdbExponencial;
		private System.Windows.Forms.ColumnHeader colTipo;
		private System.Windows.Forms.ToolTip ToolTipCancelar;
	}
}
