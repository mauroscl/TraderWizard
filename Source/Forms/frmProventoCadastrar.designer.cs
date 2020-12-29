using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmProventoCadastrar : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProventoCadastrar));
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbAtivo = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.txtDataAprovacao = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtDataEx = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.cmbProventoTipo = new System.Windows.Forms.ComboBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.txtValorPorAcao = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.btnCalendarioAprovacao = new System.Windows.Forms.Button();
            this.btnCalendarioEx = new System.Windows.Forms.Button();
            this.CalendarioAprovacao = new System.Windows.Forms.MonthCalendar();
            this.CalendarioEx = new System.Windows.Forms.MonthCalendar();
            this.Panel1.SuspendLayout();
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
            this.Panel1.TabIndex = 5;
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
            // cmbAtivo
            // 
            this.cmbAtivo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbAtivo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAtivo.FormattingEnabled = true;
            this.cmbAtivo.Location = new System.Drawing.Point(123, 12);
            this.cmbAtivo.Name = "cmbAtivo";
            this.cmbAtivo.Size = new System.Drawing.Size(179, 21);
            this.cmbAtivo.TabIndex = 0;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(11, 16);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(34, 13);
            this.Label7.TabIndex = 9;
            this.Label7.Text = "Ativo:";
            // 
            // txtDataAprovacao
            // 
            this.txtDataAprovacao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDataAprovacao.Location = new System.Drawing.Point(123, 72);
            this.txtDataAprovacao.Name = "txtDataAprovacao";
            this.txtDataAprovacao.Size = new System.Drawing.Size(122, 20);
            this.txtDataAprovacao.TabIndex = 2;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(11, 74);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(103, 13);
            this.Label1.TabIndex = 12;
            this.Label1.Text = "Data de Aprovação:";
            // 
            // txtDataEx
            // 
            this.txtDataEx.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtDataEx.Location = new System.Drawing.Point(123, 101);
            this.txtDataEx.Name = "txtDataEx";
            this.txtDataEx.Size = new System.Drawing.Size(122, 20);
            this.txtDataEx.TabIndex = 3;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(11, 103);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(48, 13);
            this.Label2.TabIndex = 14;
            this.Label2.Text = "Data Ex:";
            // 
            // cmbProventoTipo
            // 
            this.cmbProventoTipo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbProventoTipo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbProventoTipo.FormattingEnabled = true;
            this.cmbProventoTipo.Location = new System.Drawing.Point(123, 42);
            this.cmbProventoTipo.Name = "cmbProventoTipo";
            this.cmbProventoTipo.Size = new System.Drawing.Size(179, 21);
            this.cmbProventoTipo.TabIndex = 1;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(11, 45);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(103, 13);
            this.Label3.TabIndex = 15;
            this.Label3.Text = "Tipo de Pagamento:";
            // 
            // txtValorPorAcao
            // 
            this.txtValorPorAcao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtValorPorAcao.Location = new System.Drawing.Point(123, 130);
            this.txtValorPorAcao.Name = "txtValorPorAcao";
            this.txtValorPorAcao.Size = new System.Drawing.Size(122, 20);
            this.txtValorPorAcao.TabIndex = 4;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(11, 132);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(34, 13);
            this.Label9.TabIndex = 18;
            this.Label9.Text = "Valor:";
            // 
            // btnCalendarioAprovacao
            // 
            this.btnCalendarioAprovacao.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendarioAprovacao.Image")));
            this.btnCalendarioAprovacao.Location = new System.Drawing.Point(246, 70);
            this.btnCalendarioAprovacao.Name = "btnCalendarioAprovacao";
            this.btnCalendarioAprovacao.Size = new System.Drawing.Size(24, 24);
            this.btnCalendarioAprovacao.TabIndex = 19;
            this.btnCalendarioAprovacao.UseVisualStyleBackColor = true;
            this.btnCalendarioAprovacao.Click += new System.EventHandler(this.btnCalendarioAprovacao_Click);
            // 
            // btnCalendarioEx
            // 
            this.btnCalendarioEx.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendarioEx.Image")));
            this.btnCalendarioEx.Location = new System.Drawing.Point(246, 99);
            this.btnCalendarioEx.Name = "btnCalendarioEx";
            this.btnCalendarioEx.Size = new System.Drawing.Size(24, 24);
            this.btnCalendarioEx.TabIndex = 20;
            this.btnCalendarioEx.UseVisualStyleBackColor = true;
            this.btnCalendarioEx.Click += new System.EventHandler(this.btnCalendarioEx_Click);
            // 
            // CalendarioAprovacao
            // 
            this.CalendarioAprovacao.Location = new System.Drawing.Point(168, 12);
            this.CalendarioAprovacao.MaxSelectionCount = 1;
            this.CalendarioAprovacao.Name = "CalendarioAprovacao";
            this.CalendarioAprovacao.TabIndex = 21;
            this.CalendarioAprovacao.Visible = false;
            this.CalendarioAprovacao.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.CalendarioAprovacao_DateSelected);
            // 
            // CalendarioEx
            // 
            this.CalendarioEx.Location = new System.Drawing.Point(168, 12);
            this.CalendarioEx.MaxSelectionCount = 1;
            this.CalendarioEx.Name = "CalendarioEx";
            this.CalendarioEx.TabIndex = 22;
            this.CalendarioEx.Visible = false;
            this.CalendarioEx.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.CalendarioEx_DateSelected);
            // 
            // frmProventoCadastrar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 300);
            this.Controls.Add(this.CalendarioEx);
            this.Controls.Add(this.CalendarioAprovacao);
            this.Controls.Add(this.btnCalendarioEx);
            this.Controls.Add(this.btnCalendarioAprovacao);
            this.Controls.Add(this.txtValorPorAcao);
            this.Controls.Add(this.Label9);
            this.Controls.Add(this.cmbProventoTipo);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.txtDataEx);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.txtDataAprovacao);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.cmbAtivo);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmProventoCadastrar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cadastrar Proventos";
            this.Load += new System.EventHandler(this.frmProventoCadastrar_Load);
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ComboBox cmbAtivo;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.TextBox txtDataAprovacao;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.TextBox txtDataEx;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.ComboBox cmbProventoTipo;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.TextBox txtValorPorAcao;
		private System.Windows.Forms.Label Label9;
		private System.Windows.Forms.Button btnCalendarioAprovacao;
		private System.Windows.Forms.Button btnCalendarioEx;
		private System.Windows.Forms.MonthCalendar CalendarioAprovacao;
		private System.Windows.Forms.MonthCalendar CalendarioEx;
	}
}
