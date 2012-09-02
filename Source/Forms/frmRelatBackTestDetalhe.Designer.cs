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
	partial class frmRelatBackTestDetalhe : System.Windows.Forms.Form
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
            this.Label4 = new System.Windows.Forms.Label();
            this.cmbSetup = new System.Windows.Forms.ComboBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPrimeiroFechamentoPercentualMinimo = new System.Windows.Forms.TextBox();
            this.rdbPrimeiroFechamentoPercentualMinimo = new System.Windows.Forms.RadioButton();
            this.rdbRealizacaoParcialAlijamento = new System.Windows.Forms.RadioButton();
            this.txtPercentualFixo = new System.Windows.Forms.TextBox();
            this.rdbPercentualFixo = new System.Windows.Forms.RadioButton();
            this.rdbSemRealizacaoParcial = new System.Windows.Forms.RadioButton();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtIFR2Maximo = new System.Windows.Forms.TextBox();
            this.grbIFR2 = new System.Windows.Forms.GroupBox();
            this.chkAcimaMME49 = new System.Windows.Forms.CheckBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.GroupBox3.SuspendLayout();
            this.grbIFR2.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(11, 15);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(38, 13);
            this.Label4.TabIndex = 17;
            this.Label4.Text = "Setup:";
            // 
            // cmbSetup
            // 
            this.cmbSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetup.FormattingEnabled = true;
            this.cmbSetup.Items.AddRange(new object[] {
            "MME 9.1",
            "MME 9.2",
            "MME 9.3",
            "IFR 2 Sobrevendido",
            "IFR 2 acima MMA 13"});
            this.cmbSetup.Location = new System.Drawing.Point(63, 12);
            this.cmbSetup.Name = "cmbSetup";
            this.cmbSetup.Size = new System.Drawing.Size(165, 21);
            this.cmbSetup.TabIndex = 16;
            this.cmbSetup.SelectedIndexChanged += new System.EventHandler(this.cmbSetup_SelectedIndexChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.txtPrimeiroFechamentoPercentualMinimo);
            this.GroupBox3.Controls.Add(this.rdbPrimeiroFechamentoPercentualMinimo);
            this.GroupBox3.Controls.Add(this.rdbRealizacaoParcialAlijamento);
            this.GroupBox3.Controls.Add(this.txtPercentualFixo);
            this.GroupBox3.Controls.Add(this.rdbPercentualFixo);
            this.GroupBox3.Controls.Add(this.rdbSemRealizacaoParcial);
            this.GroupBox3.Location = new System.Drawing.Point(14, 39);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(349, 129);
            this.GroupBox3.TabIndex = 21;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Realização Parcial";
            // 
            // txtPrimeiroFechamentoPercentualMinimo
            // 
            this.txtPrimeiroFechamentoPercentualMinimo.Location = new System.Drawing.Point(194, 89);
            this.txtPrimeiroFechamentoPercentualMinimo.Name = "txtPrimeiroFechamentoPercentualMinimo";
            this.txtPrimeiroFechamentoPercentualMinimo.Size = new System.Drawing.Size(122, 20);
            this.txtPrimeiroFechamentoPercentualMinimo.TabIndex = 22;
            // 
            // rdbPrimeiroFechamentoPercentualMinimo
            // 
            this.rdbPrimeiroFechamentoPercentualMinimo.AutoSize = true;
            this.rdbPrimeiroFechamentoPercentualMinimo.Location = new System.Drawing.Point(6, 89);
            this.rdbPrimeiroFechamentoPercentualMinimo.Name = "rdbPrimeiroFechamentoPercentualMinimo";
            this.rdbPrimeiroFechamentoPercentualMinimo.Size = new System.Drawing.Size(182, 17);
            this.rdbPrimeiroFechamentoPercentualMinimo.TabIndex = 21;
            this.rdbPrimeiroFechamentoPercentualMinimo.Text = "Fechamento c/ Perc. Mínimo de:";
            this.rdbPrimeiroFechamentoPercentualMinimo.UseVisualStyleBackColor = true;
            this.rdbPrimeiroFechamentoPercentualMinimo.CheckedChanged += new System.EventHandler(this.rdbPrimeiroFechamentoPercentualMinimo_CheckedChanged);
            // 
            // rdbRealizacaoParcialAlijamento
            // 
            this.rdbRealizacaoParcialAlijamento.AutoSize = true;
            this.rdbRealizacaoParcialAlijamento.Location = new System.Drawing.Point(6, 43);
            this.rdbRealizacaoParcialAlijamento.Name = "rdbRealizacaoParcialAlijamento";
            this.rdbRealizacaoParcialAlijamento.Size = new System.Drawing.Size(138, 17);
            this.rdbRealizacaoParcialAlijamento.TabIndex = 20;
            this.rdbRealizacaoParcialAlijamento.Text = "Alijamento do Stop Loss";
            this.rdbRealizacaoParcialAlijamento.UseVisualStyleBackColor = true;
            this.rdbRealizacaoParcialAlijamento.CheckedChanged += new System.EventHandler(this.rdbRealizacaoParcialAlijamento_CheckedChanged);
            // 
            // txtPercentualFixo
            // 
            this.txtPercentualFixo.Location = new System.Drawing.Point(194, 66);
            this.txtPercentualFixo.Name = "txtPercentualFixo";
            this.txtPercentualFixo.Size = new System.Drawing.Size(122, 20);
            this.txtPercentualFixo.TabIndex = 19;
            // 
            // rdbPercentualFixo
            // 
            this.rdbPercentualFixo.AutoSize = true;
            this.rdbPercentualFixo.Location = new System.Drawing.Point(6, 66);
            this.rdbPercentualFixo.Name = "rdbPercentualFixo";
            this.rdbPercentualFixo.Size = new System.Drawing.Size(101, 17);
            this.rdbPercentualFixo.TabIndex = 1;
            this.rdbPercentualFixo.Text = "Percentual Fixo:";
            this.rdbPercentualFixo.UseVisualStyleBackColor = true;
            this.rdbPercentualFixo.CheckedChanged += new System.EventHandler(this.rdbPercentualFixo_CheckedChanged);
            // 
            // rdbSemRealizacaoParcial
            // 
            this.rdbSemRealizacaoParcial.AutoSize = true;
            this.rdbSemRealizacaoParcial.Checked = true;
            this.rdbSemRealizacaoParcial.Location = new System.Drawing.Point(6, 20);
            this.rdbSemRealizacaoParcial.Name = "rdbSemRealizacaoParcial";
            this.rdbSemRealizacaoParcial.Size = new System.Drawing.Size(137, 17);
            this.rdbSemRealizacaoParcial.TabIndex = 0;
            this.rdbSemRealizacaoParcial.TabStop = true;
            this.rdbSemRealizacaoParcial.Text = "Sem Realização Parcial";
            this.rdbSemRealizacaoParcial.UseVisualStyleBackColor = true;
            this.rdbSemRealizacaoParcial.CheckedChanged += new System.EventHandler(this.rdbSemRealizacaoParcial_CheckedChanged);
            // 
            // Label5
            // 
            this.Label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(3, 39);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(85, 13);
            this.Label5.TabIndex = 36;
            this.Label5.Text = "IFR 2 abaixo de:";
            // 
            // txtIFR2Maximo
            // 
            this.txtIFR2Maximo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtIFR2Maximo.Location = new System.Drawing.Point(194, 32);
            this.txtIFR2Maximo.Name = "txtIFR2Maximo";
            this.txtIFR2Maximo.Size = new System.Drawing.Size(122, 20);
            this.txtIFR2Maximo.TabIndex = 37;
            this.txtIFR2Maximo.Text = "5";
            // 
            // grbIFR2
            // 
            this.grbIFR2.Controls.Add(this.chkAcimaMME49);
            this.grbIFR2.Controls.Add(this.txtIFR2Maximo);
            this.grbIFR2.Controls.Add(this.Label5);
            this.grbIFR2.Location = new System.Drawing.Point(14, 174);
            this.grbIFR2.Name = "grbIFR2";
            this.grbIFR2.Size = new System.Drawing.Size(349, 63);
            this.grbIFR2.TabIndex = 38;
            this.grbIFR2.TabStop = false;
            this.grbIFR2.Text = "Específico - IFR 2";
            // 
            // chkAcimaMME49
            // 
            this.chkAcimaMME49.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkAcimaMME49.AutoSize = true;
            this.chkAcimaMME49.Checked = true;
            this.chkAcimaMME49.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAcimaMME49.Location = new System.Drawing.Point(6, 19);
            this.chkAcimaMME49.Name = "chkAcimaMME49";
            this.chkAcimaMME49.Size = new System.Drawing.Size(101, 17);
            this.chkAcimaMME49.TabIndex = 36;
            this.chkAcimaMME49.Text = "Acima MME 49:";
            this.chkAcimaMME49.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 255);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(474, 40);
            this.Panel1.TabIndex = 39;
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
            // frmRelatBackTestDetalhe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 295);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.grbIFR2);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.cmbSetup);
            this.MaximizeBox = false;
            this.Name = "frmRelatBackTestDetalhe";
            this.Text = "Relatório de Back Teste - Setup";
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.grbIFR2.ResumeLayout(false);
            this.grbIFR2.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.ComboBox cmbSetup;
		private System.Windows.Forms.GroupBox GroupBox3;
		private System.Windows.Forms.TextBox txtPercentualFixo;
		private System.Windows.Forms.RadioButton rdbPercentualFixo;
		private System.Windows.Forms.RadioButton rdbSemRealizacaoParcial;
		private System.Windows.Forms.RadioButton rdbRealizacaoParcialAlijamento;
		private System.Windows.Forms.TextBox txtPrimeiroFechamentoPercentualMinimo;
		private System.Windows.Forms.RadioButton rdbPrimeiroFechamentoPercentualMinimo;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.TextBox txtIFR2Maximo;
		private System.Windows.Forms.GroupBox grbIFR2;
		private System.Windows.Forms.CheckBox chkAcimaMME49;
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
	}
}
