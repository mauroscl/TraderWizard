using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmIFRSimulacaoDiaria : System.Windows.Forms.Form
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
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbMediaAritmetica = new System.Windows.Forms.RadioButton();
            this.rdbMediaExponencial = new System.Windows.Forms.RadioButton();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.rdbComFiltro = new System.Windows.Forms.RadioButton();
            this.rdbSemFiltro = new System.Windows.Forms.RadioButton();
            this.chkSubirStopApenasAposRealizacaoParcial = new System.Windows.Forms.CheckBox();
            this.chkExcluirSimulacoesAnteriores = new System.Windows.Forms.CheckBox();
            this.pnlAtivosEscolher = new System.Windows.Forms.Panel();
            this.btnRemover = new System.Windows.Forms.Button();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.btnAdicionarTodos = new System.Windows.Forms.Button();
            this.lstAtivosEscolhidos = new System.Windows.Forms.ListBox();
            this.lstAtivosNaoEscolhidos = new System.Windows.Forms.ListBox();
            this.Panel1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
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
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.rdbMediaAritmetica);
            this.GroupBox2.Controls.Add(this.rdbMediaExponencial);
            this.GroupBox2.Location = new System.Drawing.Point(12, 71);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(507, 64);
            this.GroupBox2.TabIndex = 3;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Tipo de Média";
            // 
            // rdbMediaAritmetica
            // 
            this.rdbMediaAritmetica.AutoSize = true;
            this.rdbMediaAritmetica.Location = new System.Drawing.Point(6, 43);
            this.rdbMediaAritmetica.Name = "rdbMediaAritmetica";
            this.rdbMediaAritmetica.Size = new System.Drawing.Size(71, 17);
            this.rdbMediaAritmetica.TabIndex = 1;
            this.rdbMediaAritmetica.Text = "Aritmética";
            this.rdbMediaAritmetica.UseVisualStyleBackColor = true;
            // 
            // rdbMediaExponencial
            // 
            this.rdbMediaExponencial.AutoSize = true;
            this.rdbMediaExponencial.Checked = true;
            this.rdbMediaExponencial.Location = new System.Drawing.Point(7, 20);
            this.rdbMediaExponencial.Name = "rdbMediaExponencial";
            this.rdbMediaExponencial.Size = new System.Drawing.Size(83, 17);
            this.rdbMediaExponencial.TabIndex = 0;
            this.rdbMediaExponencial.TabStop = true;
            this.rdbMediaExponencial.Text = "Exponencial";
            this.rdbMediaExponencial.UseVisualStyleBackColor = true;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.rdbComFiltro);
            this.GroupBox3.Controls.Add(this.rdbSemFiltro);
            this.GroupBox3.Location = new System.Drawing.Point(12, 3);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(507, 64);
            this.GroupBox3.TabIndex = 2;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Setup";
            // 
            // rdbComFiltro
            // 
            this.rdbComFiltro.AutoSize = true;
            this.rdbComFiltro.Location = new System.Drawing.Point(6, 43);
            this.rdbComFiltro.Name = "rdbComFiltro";
            this.rdbComFiltro.Size = new System.Drawing.Size(71, 17);
            this.rdbComFiltro.TabIndex = 1;
            this.rdbComFiltro.Text = "Com Filtro";
            this.rdbComFiltro.UseVisualStyleBackColor = true;
            // 
            // rdbSemFiltro
            // 
            this.rdbSemFiltro.AutoSize = true;
            this.rdbSemFiltro.Checked = true;
            this.rdbSemFiltro.Location = new System.Drawing.Point(7, 20);
            this.rdbSemFiltro.Name = "rdbSemFiltro";
            this.rdbSemFiltro.Size = new System.Drawing.Size(71, 17);
            this.rdbSemFiltro.TabIndex = 0;
            this.rdbSemFiltro.TabStop = true;
            this.rdbSemFiltro.Text = "Sem Filtro";
            this.rdbSemFiltro.UseVisualStyleBackColor = true;
            // 
            // chkSubirStopApenasAposRealizacaoParcial
            // 
            this.chkSubirStopApenasAposRealizacaoParcial.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSubirStopApenasAposRealizacaoParcial.Checked = true;
            this.chkSubirStopApenasAposRealizacaoParcial.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSubirStopApenasAposRealizacaoParcial.Location = new System.Drawing.Point(12, 141);
            this.chkSubirStopApenasAposRealizacaoParcial.Name = "chkSubirStopApenasAposRealizacaoParcial";
            this.chkSubirStopApenasAposRealizacaoParcial.Size = new System.Drawing.Size(257, 23);
            this.chkSubirStopApenasAposRealizacaoParcial.TabIndex = 8;
            this.chkSubirStopApenasAposRealizacaoParcial.Text = "Subir stop apenas após a realização parcial";
            this.chkSubirStopApenasAposRealizacaoParcial.UseVisualStyleBackColor = true;
            // 
            // chkExcluirSimulacoesAnteriores
            // 
            this.chkExcluirSimulacoesAnteriores.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkExcluirSimulacoesAnteriores.Location = new System.Drawing.Point(12, 170);
            this.chkExcluirSimulacoesAnteriores.Name = "chkExcluirSimulacoesAnteriores";
            this.chkExcluirSimulacoesAnteriores.Size = new System.Drawing.Size(257, 23);
            this.chkExcluirSimulacoesAnteriores.TabIndex = 9;
            this.chkExcluirSimulacoesAnteriores.Text = "Excluir Simulações Anteriores";
            this.chkExcluirSimulacoesAnteriores.UseVisualStyleBackColor = true;
            // 
            // pnlAtivosEscolher
            // 
            this.pnlAtivosEscolher.Controls.Add(this.btnRemover);
            this.pnlAtivosEscolher.Controls.Add(this.btnRemoverTodos);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionar);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionarTodos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosEscolhidos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosNaoEscolhidos);
            this.pnlAtivosEscolher.Location = new System.Drawing.Point(12, 199);
            this.pnlAtivosEscolher.Name = "pnlAtivosEscolher";
            this.pnlAtivosEscolher.Size = new System.Drawing.Size(524, 232);
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
            this.lstAtivosEscolhidos.Size = new System.Drawing.Size(198, 225);
            this.lstAtivosEscolhidos.Sorted = true;
            this.lstAtivosEscolhidos.TabIndex = 5;
            // 
            // lstAtivosNaoEscolhidos
            // 
            this.lstAtivosNaoEscolhidos.FormattingEnabled = true;
            this.lstAtivosNaoEscolhidos.Location = new System.Drawing.Point(3, 0);
            this.lstAtivosNaoEscolhidos.Name = "lstAtivosNaoEscolhidos";
            this.lstAtivosNaoEscolhidos.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAtivosNaoEscolhidos.Size = new System.Drawing.Size(198, 225);
            this.lstAtivosNaoEscolhidos.Sorted = true;
            this.lstAtivosNaoEscolhidos.TabIndex = 0;
            // 
            // frmIFRSimulacaoDiaria
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 474);
            this.Controls.Add(this.pnlAtivosEscolher);
            this.Controls.Add(this.chkExcluirSimulacoesAnteriores);
            this.Controls.Add(this.chkSubirStopApenasAposRealizacaoParcial);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmIFRSimulacaoDiaria";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simular IFR Diário";
            this.Load += new System.EventHandler(this.frmIFRSimulacaoDiaria_Load);
            this.Panel1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.pnlAtivosEscolher.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.GroupBox GroupBox2;
		private System.Windows.Forms.RadioButton rdbMediaAritmetica;
		private System.Windows.Forms.RadioButton rdbMediaExponencial;
		private System.Windows.Forms.GroupBox GroupBox3;
		private System.Windows.Forms.RadioButton rdbComFiltro;
		private System.Windows.Forms.RadioButton rdbSemFiltro;
		private System.Windows.Forms.CheckBox chkSubirStopApenasAposRealizacaoParcial;
		private System.Windows.Forms.CheckBox chkExcluirSimulacoesAnteriores;
		private System.Windows.Forms.Panel pnlAtivosEscolher;
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.Button btnRemoverTodos;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.Button btnAdicionarTodos;
    	private System.Windows.Forms.ListBox lstAtivosEscolhidos;
		private System.Windows.Forms.ListBox lstAtivosNaoEscolhidos;
	}
}
