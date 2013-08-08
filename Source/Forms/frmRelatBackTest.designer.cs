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
	partial class frmRelatBackTest : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRelatBackTest));
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbAtivo = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtNumAcoesLote = new System.Windows.Forms.TextBox();
            this.txtCapitalIncial = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtDataInicio = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnExcluir = new System.Windows.Forms.Button();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbSemanal = new System.Windows.Forms.RadioButton();
            this.rdbDiario = new System.Windows.Forms.RadioButton();
            this.lstSetup = new System.Windows.Forms.ListView();
            this.colSetup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSetupDescricao = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFiltrarMME49 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colIFRSobrevendidoMaximo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRealizacaoParcialTipo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRealizacaoParcialTipoDescricao = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRealizacaoParcialPercentualFixo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRealizacaoParcialFechamentoPercMinimo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkRealizacaoParcialPermitirDayTrade = new System.Windows.Forms.CheckBox();
            this.txtDescricao = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.btnAbaixo = new System.Windows.Forms.Button();
            this.btnAcima = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbMediaAritmetica = new System.Windows.Forms.RadioButton();
            this.rdbMediaExponencial = new System.Windows.Forms.RadioButton();
            this.Panel1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 407);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(815, 40);
            this.Panel1.TabIndex = 14;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(737, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(662, 0);
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
            this.cmbAtivo.Location = new System.Drawing.Point(90, 12);
            this.cmbAtivo.Name = "cmbAtivo";
            this.cmbAtivo.Size = new System.Drawing.Size(179, 21);
            this.cmbAtivo.TabIndex = 0;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(9, 15);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(34, 13);
            this.Label7.TabIndex = 5;
            this.Label7.Text = "Ativo:";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(303, 15);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(82, 13);
            this.Label1.TabIndex = 7;
            this.Label1.Text = "Ações por Lote:";
            // 
            // txtNumAcoesLote
            // 
            this.txtNumAcoesLote.Location = new System.Drawing.Point(406, 12);
            this.txtNumAcoesLote.Name = "txtNumAcoesLote";
            this.txtNumAcoesLote.Size = new System.Drawing.Size(114, 20);
            this.txtNumAcoesLote.TabIndex = 2;
            this.txtNumAcoesLote.Text = "100";
            // 
            // txtCapitalIncial
            // 
            this.txtCapitalIncial.Location = new System.Drawing.Point(90, 38);
            this.txtCapitalIncial.Name = "txtCapitalIncial";
            this.txtCapitalIncial.Size = new System.Drawing.Size(179, 20);
            this.txtCapitalIncial.TabIndex = 1;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(9, 41);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 13);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "Capital Inicial:";
            // 
            // txtDataInicio
            // 
            this.txtDataInicio.Location = new System.Drawing.Point(406, 38);
            this.txtDataInicio.Name = "txtDataInicio";
            this.txtDataInicio.Size = new System.Drawing.Size(114, 20);
            this.txtDataInicio.TabIndex = 3;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(303, 41);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(78, 13);
            this.Label3.TabIndex = 11;
            this.Label3.Text = "Data de Início:";
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Location = new System.Drawing.Point(727, 165);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(76, 35);
            this.btnAdicionar.TabIndex = 9;
            this.btnAdicionar.Text = "&Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(727, 206);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(76, 35);
            this.btnEditar.TabIndex = 10;
            this.btnEditar.Text = "&Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnExcluir
            // 
            this.btnExcluir.Location = new System.Drawing.Point(727, 247);
            this.btnExcluir.Name = "btnExcluir";
            this.btnExcluir.Size = new System.Drawing.Size(76, 35);
            this.btnExcluir.TabIndex = 11;
            this.btnExcluir.Text = "E&xcluir";
            this.btnExcluir.UseVisualStyleBackColor = true;
            this.btnExcluir.Click += new System.EventHandler(this.btnExcluir_Click);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.rdbSemanal);
            this.GroupBox2.Controls.Add(this.rdbDiario);
            this.GroupBox2.Location = new System.Drawing.Point(12, 64);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(253, 64);
            this.GroupBox2.TabIndex = 5;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Periodicidade";
            // 
            // rdbSemanal
            // 
            this.rdbSemanal.AutoSize = true;
            this.rdbSemanal.Location = new System.Drawing.Point(6, 40);
            this.rdbSemanal.Name = "rdbSemanal";
            this.rdbSemanal.Size = new System.Drawing.Size(66, 17);
            this.rdbSemanal.TabIndex = 1;
            this.rdbSemanal.Text = "Semanal";
            this.rdbSemanal.UseVisualStyleBackColor = true;
            // 
            // rdbDiario
            // 
            this.rdbDiario.AutoSize = true;
            this.rdbDiario.Checked = true;
            this.rdbDiario.Location = new System.Drawing.Point(6, 17);
            this.rdbDiario.Name = "rdbDiario";
            this.rdbDiario.Size = new System.Drawing.Size(52, 17);
            this.rdbDiario.TabIndex = 0;
            this.rdbDiario.TabStop = true;
            this.rdbDiario.Text = "Diário";
            this.rdbDiario.UseVisualStyleBackColor = true;
            // 
            // lstSetup
            // 
            this.lstSetup.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSetup,
            this.colSetupDescricao,
            this.colFiltrarMME49,
            this.colIFRSobrevendidoMaximo,
            this.colRealizacaoParcialTipo,
            this.colRealizacaoParcialTipoDescricao,
            this.colRealizacaoParcialPercentualFixo,
            this.colRealizacaoParcialFechamentoPercMinimo});
            this.lstSetup.FullRowSelect = true;
            this.lstSetup.GridLines = true;
            this.lstSetup.Location = new System.Drawing.Point(12, 165);
            this.lstSetup.Name = "lstSetup";
            this.lstSetup.Size = new System.Drawing.Size(709, 222);
            this.lstSetup.TabIndex = 8;
            this.lstSetup.UseCompatibleStateImageBehavior = false;
            this.lstSetup.View = System.Windows.Forms.View.Details;
            // 
            // colSetup
            // 
            this.colSetup.Text = "Setup";
            this.colSetup.Width = 0;
            // 
            // colSetupDescricao
            // 
            this.colSetupDescricao.Text = "Setup";
            this.colSetupDescricao.Width = 110;
            // 
            // colFiltrarMME49
            // 
            this.colFiltrarMME49.Text = "Filtrar MME 49";
            this.colFiltrarMME49.Width = 82;
            // 
            // colIFRSobrevendidoMaximo
            // 
            this.colIFRSobrevendidoMaximo.Text = "Valor Max. IFR Sobr.";
            this.colIFRSobrevendidoMaximo.Width = 115;
            // 
            // colRealizacaoParcialTipo
            // 
            this.colRealizacaoParcialTipo.Text = "Tipo de Realização Parcial";
            this.colRealizacaoParcialTipo.Width = 0;
            // 
            // colRealizacaoParcialTipoDescricao
            // 
            this.colRealizacaoParcialTipoDescricao.Text = "Tipo Real. Parcial";
            this.colRealizacaoParcialTipoDescricao.Width = 105;
            // 
            // colRealizacaoParcialPercentualFixo
            // 
            this.colRealizacaoParcialPercentualFixo.Text = "Real. Parc. Perc. Fixo";
            this.colRealizacaoParcialPercentualFixo.Width = 125;
            // 
            // colRealizacaoParcialFechamentoPercMinimo
            // 
            this.colRealizacaoParcialFechamentoPercMinimo.Text = "Real. Parc. Fech. Perc. Mínimo";
            this.colRealizacaoParcialFechamentoPercMinimo.Width = 179;
            // 
            // chkRealizacaoParcialPermitirDayTrade
            // 
            this.chkRealizacaoParcialPermitirDayTrade.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRealizacaoParcialPermitirDayTrade.Location = new System.Drawing.Point(549, 14);
            this.chkRealizacaoParcialPermitirDayTrade.Name = "chkRealizacaoParcialPermitirDayTrade";
            this.chkRealizacaoParcialPermitirDayTrade.Size = new System.Drawing.Size(254, 17);
            this.chkRealizacaoParcialPermitirDayTrade.TabIndex = 4;
            this.chkRealizacaoParcialPermitirDayTrade.Text = "Permitir Day Trade na Realização Parcial:";
            this.chkRealizacaoParcialPermitirDayTrade.UseVisualStyleBackColor = true;
            // 
            // txtDescricao
            // 
            this.txtDescricao.Location = new System.Drawing.Point(73, 134);
            this.txtDescricao.Name = "txtDescricao";
            this.txtDescricao.Size = new System.Drawing.Size(648, 20);
            this.txtDescricao.TabIndex = 7;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(9, 137);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(58, 13);
            this.Label4.TabIndex = 32;
            this.Label4.Text = "Descrição:";
            // 
            // btnAbaixo
            // 
            this.btnAbaixo.Location = new System.Drawing.Point(727, 353);
            this.btnAbaixo.Name = "btnAbaixo";
            this.btnAbaixo.Size = new System.Drawing.Size(76, 35);
            this.btnAbaixo.TabIndex = 13;
            this.btnAbaixo.UseVisualStyleBackColor = true;
            this.btnAbaixo.Click += new System.EventHandler(this.btnAbaixo_Click);
            // 
            // btnAcima
            // 
            this.btnAcima.Image = ((System.Drawing.Image)(resources.GetObject("btnAcima.Image")));
            this.btnAcima.Location = new System.Drawing.Point(727, 307);
            this.btnAcima.Name = "btnAcima";
            this.btnAcima.Size = new System.Drawing.Size(76, 40);
            this.btnAcima.TabIndex = 12;
            this.btnAcima.UseVisualStyleBackColor = true;
            this.btnAcima.Click += new System.EventHandler(this.btnAcima_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.rdbMediaAritmetica);
            this.GroupBox1.Controls.Add(this.rdbMediaExponencial);
            this.GroupBox1.Location = new System.Drawing.Point(306, 64);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(214, 64);
            this.GroupBox1.TabIndex = 6;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Tipo de Média";
            // 
            // rdbMediaAritmetica
            // 
            this.rdbMediaAritmetica.AutoSize = true;
            this.rdbMediaAritmetica.Location = new System.Drawing.Point(7, 40);
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
            this.rdbMediaExponencial.Location = new System.Drawing.Point(7, 17);
            this.rdbMediaExponencial.Name = "rdbMediaExponencial";
            this.rdbMediaExponencial.Size = new System.Drawing.Size(83, 17);
            this.rdbMediaExponencial.TabIndex = 0;
            this.rdbMediaExponencial.TabStop = true;
            this.rdbMediaExponencial.Text = "Exponencial";
            this.rdbMediaExponencial.UseVisualStyleBackColor = true;
            // 
            // frmRelatBackTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 447);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.txtDescricao);
            this.Controls.Add(this.chkRealizacaoParcialPermitirDayTrade);
            this.Controls.Add(this.btnAbaixo);
            this.Controls.Add(this.btnAcima);
            this.Controls.Add(this.lstSetup);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.btnExcluir);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnAdicionar);
            this.Controls.Add(this.txtDataInicio);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.txtCapitalIncial);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.txtNumAcoesLote);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.cmbAtivo);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmRelatBackTest";
            this.Text = "Relatórios de Back Test";
            this.Panel1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.ComboBox cmbAtivo;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.TextBox txtNumAcoesLote;
		private System.Windows.Forms.TextBox txtCapitalIncial;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.TextBox txtDataInicio;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.Button btnEditar;
		private System.Windows.Forms.Button btnExcluir;
		private System.Windows.Forms.GroupBox GroupBox2;
		private System.Windows.Forms.RadioButton rdbSemanal;
		private System.Windows.Forms.RadioButton rdbDiario;
		private System.Windows.Forms.ListView lstSetup;
		private System.Windows.Forms.ColumnHeader colSetup;
		private System.Windows.Forms.ColumnHeader colFiltrarMME49;
		private System.Windows.Forms.ColumnHeader colIFRSobrevendidoMaximo;
		private System.Windows.Forms.ColumnHeader colRealizacaoParcialTipoDescricao;
		private System.Windows.Forms.ColumnHeader colRealizacaoParcialPercentualFixo;
		private System.Windows.Forms.ColumnHeader colRealizacaoParcialFechamentoPercMinimo;
		private System.Windows.Forms.ColumnHeader colRealizacaoParcialTipo;
		private System.Windows.Forms.ColumnHeader colSetupDescricao;
		private System.Windows.Forms.Button btnAbaixo;
		private System.Windows.Forms.Button btnAcima;
		private System.Windows.Forms.CheckBox chkRealizacaoParcialPermitirDayTrade;
		private System.Windows.Forms.TextBox txtDescricao;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton rdbMediaAritmetica;
		private System.Windows.Forms.RadioButton rdbMediaExponencial;
	}
}
