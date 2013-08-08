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
	partial class frmCotacaoAtualizarPeriodo : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCotacaoAtualizarPeriodo));
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlAtualizacaoDiaria = new System.Windows.Forms.Panel();
            this.CalendarioInicial = new System.Windows.Forms.MonthCalendar();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbAtivo = new System.Windows.Forms.ComboBox();
            this.lblAtivo = new System.Windows.Forms.Label();
            this.rdbIntraday = new System.Windows.Forms.RadioButton();
            this.rdbHistorica = new System.Windows.Forms.RadioButton();
            this.rdbOnline = new System.Windows.Forms.RadioButton();
            this.btnCalendarioFinal = new System.Windows.Forms.Button();
            this.btnCalendarioInicial = new System.Windows.Forms.Button();
            this.txtDataFinal = new System.Windows.Forms.TextBox();
            this.txtDataInicial = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.rdbAtualizacaoDiaria = new System.Windows.Forms.RadioButton();
            this.rdbAtualizacaoAnual = new System.Windows.Forms.RadioButton();
            this.pnlAtualizacaoAnual = new System.Windows.Forms.Panel();
            this.txtAno = new System.Windows.Forms.TextBox();
            this.lblAno = new System.Windows.Forms.Label();
            this.chkCalcularDados = new System.Windows.Forms.CheckBox();
            this.CalendarioFinal = new System.Windows.Forms.MonthCalendar();
            this.Panel1.SuspendLayout();
            this.pnlAtualizacaoDiaria.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.pnlAtualizacaoAnual.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 327);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(472, 40);
            this.Panel1.TabIndex = 3;
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
            // pnlAtualizacaoDiaria
            // 
            this.pnlAtualizacaoDiaria.Controls.Add(this.CalendarioInicial);
            this.pnlAtualizacaoDiaria.Controls.Add(this.CalendarioFinal);
            this.pnlAtualizacaoDiaria.Controls.Add(this.GroupBox1);
            this.pnlAtualizacaoDiaria.Controls.Add(this.btnCalendarioFinal);
            this.pnlAtualizacaoDiaria.Controls.Add(this.btnCalendarioInicial);
            this.pnlAtualizacaoDiaria.Controls.Add(this.txtDataFinal);
            this.pnlAtualizacaoDiaria.Controls.Add(this.txtDataInicial);
            this.pnlAtualizacaoDiaria.Controls.Add(this.Label2);
            this.pnlAtualizacaoDiaria.Controls.Add(this.Label1);
            this.pnlAtualizacaoDiaria.Location = new System.Drawing.Point(0, 28);
            this.pnlAtualizacaoDiaria.Name = "pnlAtualizacaoDiaria";
            this.pnlAtualizacaoDiaria.Size = new System.Drawing.Size(472, 196);
            this.pnlAtualizacaoDiaria.TabIndex = 1;
            // 
            // CalendarioInicial
            // 
            this.CalendarioInicial.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CalendarioInicial.Location = new System.Drawing.Point(236, 0);
            this.CalendarioInicial.MaxSelectionCount = 1;
            this.CalendarioInicial.Name = "CalendarioInicial";
            this.CalendarioInicial.TabIndex = 10;
            this.CalendarioInicial.Visible = false;
            this.CalendarioInicial.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.CalendarioInicial_DateSelected);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.cmbAtivo);
            this.GroupBox1.Controls.Add(this.lblAtivo);
            this.GroupBox1.Controls.Add(this.rdbIntraday);
            this.GroupBox1.Controls.Add(this.rdbHistorica);
            this.GroupBox1.Controls.Add(this.rdbOnline);
            this.GroupBox1.Location = new System.Drawing.Point(6, 59);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(457, 134);
            this.GroupBox1.TabIndex = 4;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Tipo de Atualização";
            // 
            // cmbAtivo
            // 
            this.cmbAtivo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbAtivo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAtivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAtivo.FormattingEnabled = true;
            this.cmbAtivo.Location = new System.Drawing.Point(269, 19);
            this.cmbAtivo.Name = "cmbAtivo";
            this.cmbAtivo.Size = new System.Drawing.Size(179, 21);
            this.cmbAtivo.TabIndex = 10;
            // 
            // lblAtivo
            // 
            this.lblAtivo.AutoSize = true;
            this.lblAtivo.Location = new System.Drawing.Point(180, 22);
            this.lblAtivo.Name = "lblAtivo";
            this.lblAtivo.Size = new System.Drawing.Size(83, 13);
            this.lblAtivo.TabIndex = 11;
            this.lblAtivo.Text = "Ativo (opcional):";
            // 
            // rdbIntraday
            // 
            this.rdbIntraday.AutoSize = true;
            this.rdbIntraday.Location = new System.Drawing.Point(15, 63);
            this.rdbIntraday.Name = "rdbIntraday";
            this.rdbIntraday.Size = new System.Drawing.Size(63, 17);
            this.rdbIntraday.TabIndex = 2;
            this.rdbIntraday.Text = "Intraday";
            this.rdbIntraday.UseVisualStyleBackColor = true;
            // 
            // rdbHistorica
            // 
            this.rdbHistorica.AutoSize = true;
            this.rdbHistorica.Location = new System.Drawing.Point(15, 43);
            this.rdbHistorica.Name = "rdbHistorica";
            this.rdbHistorica.Size = new System.Drawing.Size(179, 17);
            this.rdbHistorica.TabIndex = 1;
            this.rdbHistorica.Text = "Offline - Arquivos estão baixados";
            this.rdbHistorica.UseVisualStyleBackColor = true;
            // 
            // rdbOnline
            // 
            this.rdbOnline.AutoSize = true;
            this.rdbOnline.Checked = true;
            this.rdbOnline.Location = new System.Drawing.Point(15, 20);
            this.rdbOnline.Name = "rdbOnline";
            this.rdbOnline.Size = new System.Drawing.Size(136, 17);
            this.rdbOnline.TabIndex = 0;
            this.rdbOnline.TabStop = true;
            this.rdbOnline.Text = "Online - Baixar arquivos";
            this.rdbOnline.UseVisualStyleBackColor = true;
            this.rdbOnline.CheckedChanged += new System.EventHandler(this.rdbOnline_CheckedChanged);
            // 
            // btnCalendarioFinal
            // 
            this.btnCalendarioFinal.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendarioFinal.Image")));
            this.btnCalendarioFinal.Location = new System.Drawing.Point(205, 31);
            this.btnCalendarioFinal.Name = "btnCalendarioFinal";
            this.btnCalendarioFinal.Size = new System.Drawing.Size(24, 24);
            this.btnCalendarioFinal.TabIndex = 9;
            this.btnCalendarioFinal.UseVisualStyleBackColor = true;
            this.btnCalendarioFinal.Click += new System.EventHandler(this.btnCalendarioFinal_Click);
            // 
            // btnCalendarioInicial
            // 
            this.btnCalendarioInicial.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendarioInicial.Image")));
            this.btnCalendarioInicial.Location = new System.Drawing.Point(205, 0);
            this.btnCalendarioInicial.Name = "btnCalendarioInicial";
            this.btnCalendarioInicial.Size = new System.Drawing.Size(24, 24);
            this.btnCalendarioInicial.TabIndex = 8;
            this.btnCalendarioInicial.UseVisualStyleBackColor = true;
            this.btnCalendarioInicial.Click += new System.EventHandler(this.btnCalendarioInicial_Click);
            // 
            // txtDataFinal
            // 
            this.txtDataFinal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDataFinal.Location = new System.Drawing.Point(82, 33);
            this.txtDataFinal.Name = "txtDataFinal";
            this.txtDataFinal.Size = new System.Drawing.Size(122, 20);
            this.txtDataFinal.TabIndex = 1;
            // 
            // txtDataInicial
            // 
            this.txtDataInicial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDataInicial.Location = new System.Drawing.Point(82, 3);
            this.txtDataInicial.Name = "txtDataInicial";
            this.txtDataInicial.Size = new System.Drawing.Size(122, 20);
            this.txtDataInicial.TabIndex = 0;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(12, 37);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(58, 13);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "Data Final:";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(13, 7);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(63, 13);
            this.Label1.TabIndex = 5;
            this.Label1.Text = "Data Inicial:";
            // 
            // rdbAtualizacaoDiaria
            // 
            this.rdbAtualizacaoDiaria.AutoSize = true;
            this.rdbAtualizacaoDiaria.Checked = true;
            this.rdbAtualizacaoDiaria.Location = new System.Drawing.Point(16, 5);
            this.rdbAtualizacaoDiaria.Name = "rdbAtualizacaoDiaria";
            this.rdbAtualizacaoDiaria.Size = new System.Drawing.Size(108, 17);
            this.rdbAtualizacaoDiaria.TabIndex = 0;
            this.rdbAtualizacaoDiaria.TabStop = true;
            this.rdbAtualizacaoDiaria.Text = "Atualização diária";
            this.rdbAtualizacaoDiaria.UseVisualStyleBackColor = true;
            this.rdbAtualizacaoDiaria.CheckedChanged += new System.EventHandler(this.rdbAtualizacaoDiaria_CheckedChanged);
            // 
            // rdbAtualizacaoAnual
            // 
            this.rdbAtualizacaoAnual.AutoSize = true;
            this.rdbAtualizacaoAnual.Location = new System.Drawing.Point(12, 230);
            this.rdbAtualizacaoAnual.Name = "rdbAtualizacaoAnual";
            this.rdbAtualizacaoAnual.Size = new System.Drawing.Size(109, 17);
            this.rdbAtualizacaoAnual.TabIndex = 2;
            this.rdbAtualizacaoAnual.Text = "Atualização anual";
            this.rdbAtualizacaoAnual.UseVisualStyleBackColor = true;
            this.rdbAtualizacaoAnual.CheckedChanged += new System.EventHandler(this.rdbAtualizacaoAnual_CheckedChanged);
            // 
            // pnlAtualizacaoAnual
            // 
            this.pnlAtualizacaoAnual.Controls.Add(this.txtAno);
            this.pnlAtualizacaoAnual.Controls.Add(this.lblAno);
            this.pnlAtualizacaoAnual.Enabled = false;
            this.pnlAtualizacaoAnual.Location = new System.Drawing.Point(-4, 253);
            this.pnlAtualizacaoAnual.Name = "pnlAtualizacaoAnual";
            this.pnlAtualizacaoAnual.Size = new System.Drawing.Size(364, 46);
            this.pnlAtualizacaoAnual.TabIndex = 3;
            // 
            // txtAno
            // 
            this.txtAno.Location = new System.Drawing.Point(82, 11);
            this.txtAno.Name = "txtAno";
            this.txtAno.Size = new System.Drawing.Size(122, 20);
            this.txtAno.TabIndex = 0;
            // 
            // lblAno
            // 
            this.lblAno.AutoSize = true;
            this.lblAno.Location = new System.Drawing.Point(13, 14);
            this.lblAno.Name = "lblAno";
            this.lblAno.Size = new System.Drawing.Size(29, 13);
            this.lblAno.TabIndex = 0;
            this.lblAno.Text = "Ano:";
            // 
            // chkCalcularDados
            // 
            this.chkCalcularDados.AutoSize = true;
            this.chkCalcularDados.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCalcularDados.Checked = true;
            this.chkCalcularDados.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCalcularDados.Location = new System.Drawing.Point(8, 305);
            this.chkCalcularDados.Name = "chkCalcularDados";
            this.chkCalcularDados.Size = new System.Drawing.Size(98, 17);
            this.chkCalcularDados.TabIndex = 4;
            this.chkCalcularDados.Text = "Calcular Dados";
            this.chkCalcularDados.UseVisualStyleBackColor = true;
            // 
            // CalendarioFinal
            // 
            this.CalendarioFinal.Location = new System.Drawing.Point(236, 31);
            this.CalendarioFinal.Name = "CalendarioFinal";
            this.CalendarioFinal.TabIndex = 13;
            this.CalendarioFinal.Visible = false;
            this.CalendarioFinal.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.CalendarioFinal_DateSelected);
            // 
            // frmCotacaoAtualizarPeriodo
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(472, 367);
            this.Controls.Add(this.chkCalcularDados);
            this.Controls.Add(this.pnlAtualizacaoAnual);
            this.Controls.Add(this.rdbAtualizacaoAnual);
            this.Controls.Add(this.rdbAtualizacaoDiaria);
            this.Controls.Add(this.pnlAtualizacaoDiaria);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmCotacaoAtualizarPeriodo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Atualizar Cotações";
            this.Load += new System.EventHandler(this.frmCotacaoAtualizarPeriodo_Load);
            this.Panel1.ResumeLayout(false);
            this.pnlAtualizacaoDiaria.ResumeLayout(false);
            this.pnlAtualizacaoDiaria.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.pnlAtualizacaoAnual.ResumeLayout(false);
            this.pnlAtualizacaoAnual.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Panel pnlAtualizacaoDiaria;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton rdbHistorica;
		private System.Windows.Forms.RadioButton rdbOnline;
		private System.Windows.Forms.TextBox txtDataFinal;
		private  System.Windows.Forms.TextBox txtDataInicial;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.RadioButton rdbAtualizacaoDiaria;
		private System.Windows.Forms.RadioButton rdbAtualizacaoAnual;
		private System.Windows.Forms.Panel pnlAtualizacaoAnual;
		private System.Windows.Forms.TextBox txtAno;
		private System.Windows.Forms.Label lblAno;
		private System.Windows.Forms.RadioButton rdbIntraday;
		private System.Windows.Forms.ComboBox cmbAtivo;
        private System.Windows.Forms.Label lblAtivo;
		private System.Windows.Forms.MonthCalendar CalendarioInicial;
		private System.Windows.Forms.Button btnCalendarioFinal;
		private System.Windows.Forms.Button btnCalendarioInicial;
		internal System.Windows.Forms.CheckBox chkCalcularDados;
        private MonthCalendar CalendarioFinal;
	}
}
