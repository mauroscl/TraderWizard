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
	partial class frmRelatVisualizacao : System.Windows.Forms.Form
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRelatVisualizacao));
            this.objDataGridView = new System.Windows.Forms.DataGridView();
            this.txtTitulosTotal = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtNegociosTotal = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtValorTotal = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.cmbSetup = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.rdbSemanal = new System.Windows.Forms.RadioButton();
            this.rdbDiario = new System.Windows.Forms.RadioButton();
            this.txtIFR2Maximo = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.txtStopGainPercentual = new System.Windows.Forms.TextBox();
            this.rdbStopGainPercentual = new System.Windows.Forms.RadioButton();
            this.rdbStopGainAlijamento = new System.Windows.Forms.RadioButton();
            this.chkAcimaMME49 = new System.Windows.Forms.CheckBox();
            this.btnRelarioGerar = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.txtValorCapital = new System.Windows.Forms.TextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.txtPercentualManejo = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.lblIFRFiltro = new System.Windows.Forms.Label();
            this.cmbIFRFiltro = new System.Windows.Forms.ComboBox();
            this.Calendario = new System.Windows.Forms.MonthCalendar();
            this.btnCalendario = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.objDataGridView)).BeginInit();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // objDataGridView
            // 
            this.objDataGridView.AllowUserToAddRows = false;
            this.objDataGridView.AllowUserToDeleteRows = false;
            this.objDataGridView.AllowUserToOrderColumns = true;
            this.objDataGridView.AllowUserToResizeRows = false;
            this.objDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.objDataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.objDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.objDataGridView.Location = new System.Drawing.Point(9, 184);
            this.objDataGridView.Name = "objDataGridView";
            this.objDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.objDataGridView.Size = new System.Drawing.Size(734, 199);
            this.objDataGridView.TabIndex = 0;
            this.objDataGridView.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.objDataGridView_CellToolTipTextNeeded);
            this.objDataGridView.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.objDataGridView_ColumnHeaderMouseClick);
            // 
            // txtTitulosTotal
            // 
            this.txtTitulosTotal.Location = new System.Drawing.Point(131, 18);
            this.txtTitulosTotal.Name = "txtTitulosTotal";
            this.txtTitulosTotal.Size = new System.Drawing.Size(122, 20);
            this.txtTitulosTotal.TabIndex = 8;
            this.txtTitulosTotal.Text = "100.000";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(19, 22);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(112, 13);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "Quantidade de ações:";
            // 
            // txtNegociosTotal
            // 
            this.txtNegociosTotal.Location = new System.Drawing.Point(131, 70);
            this.txtNegociosTotal.Name = "txtNegociosTotal";
            this.txtNegociosTotal.Size = new System.Drawing.Size(122, 20);
            this.txtNegociosTotal.TabIndex = 10;
            this.txtNegociosTotal.Text = "100";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(19, 74);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(70, 13);
            this.Label1.TabIndex = 11;
            this.Label1.Text = "Nº Negócios:";
            // 
            // txtValorTotal
            // 
            this.txtValorTotal.Location = new System.Drawing.Point(131, 44);
            this.txtValorTotal.Name = "txtValorTotal";
            this.txtValorTotal.Size = new System.Drawing.Size(122, 20);
            this.txtValorTotal.TabIndex = 12;
            this.txtValorTotal.Text = "1.000.000,00";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(19, 48);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(61, 13);
            this.Label3.TabIndex = 13;
            this.Label3.Text = "Valor Total:";
            // 
            // cmbSetup
            // 
            this.cmbSetup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSetup.FormattingEnabled = true;
            this.cmbSetup.Items.AddRange(new object[] {
            "MME 9.1",
            "MME 9.2",
            "MME 9.3",
            "IFR 2 Sem Filtro",
            "IFR 2 Sem Filtro Personalizado",
            "IFR 2 Com filtro"});
            this.cmbSetup.Location = new System.Drawing.Point(64, 6);
            this.cmbSetup.Name = "cmbSetup";
            this.cmbSetup.Size = new System.Drawing.Size(177, 21);
            this.cmbSetup.TabIndex = 14;
            this.cmbSetup.SelectedIndexChanged += new System.EventHandler(this.cmbSetup_SelectedIndexChanged);
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(12, 9);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(38, 13);
            this.Label4.TabIndex = 15;
            this.Label4.Text = "Setup:";
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.txtTitulosTotal);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.txtValorTotal);
            this.GroupBox1.Controls.Add(this.txtNegociosTotal);
            this.GroupBox1.Controls.Add(this.Label3);
            this.GroupBox1.Location = new System.Drawing.Point(252, 7);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(268, 99);
            this.GroupBox1.TabIndex = 16;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Volume Mínimo Diário";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.rdbSemanal);
            this.GroupBox2.Controls.Add(this.rdbDiario);
            this.GroupBox2.Location = new System.Drawing.Point(9, 34);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(232, 72);
            this.GroupBox2.TabIndex = 17;
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
            // txtIFR2Maximo
            // 
            this.txtIFR2Maximo.Location = new System.Drawing.Point(621, 110);
            this.txtIFR2Maximo.Name = "txtIFR2Maximo";
            this.txtIFR2Maximo.Size = new System.Drawing.Size(122, 20);
            this.txtIFR2Maximo.TabIndex = 18;
            this.txtIFR2Maximo.Text = "10";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(533, 114);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(85, 13);
            this.Label5.TabIndex = 19;
            this.Label5.Text = "IFR 2 abaixo de:";
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.txtStopGainPercentual);
            this.GroupBox3.Controls.Add(this.rdbStopGainPercentual);
            this.GroupBox3.Controls.Add(this.rdbStopGainAlijamento);
            this.GroupBox3.Location = new System.Drawing.Point(530, 7);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(261, 99);
            this.GroupBox3.TabIndex = 20;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Stop Gain";
            // 
            // txtStopGainPercentual
            // 
            this.txtStopGainPercentual.Location = new System.Drawing.Point(91, 43);
            this.txtStopGainPercentual.Name = "txtStopGainPercentual";
            this.txtStopGainPercentual.Size = new System.Drawing.Size(122, 20);
            this.txtStopGainPercentual.TabIndex = 19;
            // 
            // rdbStopGainPercentual
            // 
            this.rdbStopGainPercentual.AutoSize = true;
            this.rdbStopGainPercentual.Location = new System.Drawing.Point(6, 44);
            this.rdbStopGainPercentual.Name = "rdbStopGainPercentual";
            this.rdbStopGainPercentual.Size = new System.Drawing.Size(79, 17);
            this.rdbStopGainPercentual.TabIndex = 1;
            this.rdbStopGainPercentual.TabStop = true;
            this.rdbStopGainPercentual.Text = "Percentual:";
            this.rdbStopGainPercentual.UseVisualStyleBackColor = true;
            this.rdbStopGainPercentual.CheckedChanged += new System.EventHandler(this.rdbStopGainPercentual_CheckedChanged);
            // 
            // rdbStopGainAlijamento
            // 
            this.rdbStopGainAlijamento.AutoSize = true;
            this.rdbStopGainAlijamento.Checked = true;
            this.rdbStopGainAlijamento.Location = new System.Drawing.Point(6, 20);
            this.rdbStopGainAlijamento.Name = "rdbStopGainAlijamento";
            this.rdbStopGainAlijamento.Size = new System.Drawing.Size(138, 17);
            this.rdbStopGainAlijamento.TabIndex = 0;
            this.rdbStopGainAlijamento.TabStop = true;
            this.rdbStopGainAlijamento.Text = "Alijamento do Stop Loss";
            this.rdbStopGainAlijamento.UseVisualStyleBackColor = true;
            // 
            // chkAcimaMME49
            // 
            this.chkAcimaMME49.AutoSize = true;
            this.chkAcimaMME49.Location = new System.Drawing.Point(269, 112);
            this.chkAcimaMME49.Name = "chkAcimaMME49";
            this.chkAcimaMME49.Size = new System.Drawing.Size(101, 17);
            this.chkAcimaMME49.TabIndex = 21;
            this.chkAcimaMME49.Text = "Acima MME 49:";
            this.chkAcimaMME49.UseVisualStyleBackColor = true;
            // 
            // btnRelarioGerar
            // 
            this.btnRelarioGerar.Location = new System.Drawing.Point(12, 137);
            this.btnRelarioGerar.Name = "btnRelarioGerar";
            this.btnRelarioGerar.Size = new System.Drawing.Size(75, 37);
            this.btnRelarioGerar.TabIndex = 22;
            this.btnRelarioGerar.Text = "&Gerar Relatório";
            this.btnRelarioGerar.UseVisualStyleBackColor = true;
            this.btnRelarioGerar.Click += new System.EventHandler(this.btnRelarioGerar_Click);
            // 
            // txtData
            // 
            this.txtData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtData.Location = new System.Drawing.Point(64, 110);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(122, 20);
            this.txtData.TabIndex = 23;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(12, 114);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(33, 13);
            this.Label6.TabIndex = 24;
            this.Label6.Text = "Data:";
            // 
            // txtValorCapital
            // 
            this.txtValorCapital.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtValorCapital.Location = new System.Drawing.Point(370, 133);
            this.txtValorCapital.Name = "txtValorCapital";
            this.txtValorCapital.Size = new System.Drawing.Size(122, 20);
            this.txtValorCapital.TabIndex = 25;
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(280, 136);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(84, 13);
            this.Label13.TabIndex = 26;
            this.Label13.Text = "Valor do Capital:";
            // 
            // txtPercentualManejo
            // 
            this.txtPercentualManejo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.txtPercentualManejo.Location = new System.Drawing.Point(371, 158);
            this.txtPercentualManejo.Name = "txtPercentualManejo";
            this.txtPercentualManejo.Size = new System.Drawing.Size(122, 20);
            this.txtPercentualManejo.TabIndex = 27;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(309, 162);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(56, 13);
            this.Label14.TabIndex = 28;
            this.Label14.Text = "% Manejo:";
            // 
            // lblIFRFiltro
            // 
            this.lblIFRFiltro.AutoSize = true;
            this.lblIFRFiltro.Location = new System.Drawing.Point(551, 140);
            this.lblIFRFiltro.Name = "lblIFRFiltro";
            this.lblIFRFiltro.Size = new System.Drawing.Size(67, 13);
            this.lblIFRFiltro.TabIndex = 29;
            this.lblIFRFiltro.Text = "IFR do Filtro:";
            // 
            // cmbIFRFiltro
            // 
            this.cmbIFRFiltro.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbIFRFiltro.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbIFRFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIFRFiltro.FormattingEnabled = true;
            this.cmbIFRFiltro.Location = new System.Drawing.Point(621, 136);
            this.cmbIFRFiltro.Name = "cmbIFRFiltro";
            this.cmbIFRFiltro.Size = new System.Drawing.Size(122, 21);
            this.cmbIFRFiltro.TabIndex = 30;
            // 
            // Calendario
            // 
            this.Calendario.Location = new System.Drawing.Point(99, 133);
            this.Calendario.MaxSelectionCount = 1;
            this.Calendario.Name = "Calendario";
            this.Calendario.TabIndex = 31;
            this.Calendario.Visible = false;
            this.Calendario.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.Calendario_DateSelected);
            // 
            // btnCalendario
            // 
            this.btnCalendario.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendario.Image")));
            this.btnCalendario.Location = new System.Drawing.Point(190, 108);
            this.btnCalendario.Name = "btnCalendario";
            this.btnCalendario.Size = new System.Drawing.Size(24, 24);
            this.btnCalendario.TabIndex = 32;
            this.btnCalendario.UseVisualStyleBackColor = true;
            this.btnCalendario.Click += new System.EventHandler(this.btnCalendario_Click);
            // 
            // frmRelatVisualizacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 420);
            this.Controls.Add(this.btnCalendario);
            this.Controls.Add(this.Calendario);
            this.Controls.Add(this.cmbIFRFiltro);
            this.Controls.Add(this.lblIFRFiltro);
            this.Controls.Add(this.txtPercentualManejo);
            this.Controls.Add(this.Label14);
            this.Controls.Add(this.txtValorCapital);
            this.Controls.Add(this.Label13);
            this.Controls.Add(this.txtData);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.btnRelarioGerar);
            this.Controls.Add(this.chkAcimaMME49);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.txtIFR2Maximo);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Label4);
            this.Controls.Add(this.cmbSetup);
            this.Controls.Add(this.objDataGridView);
            this.Name = "frmRelatVisualizacao";
            this.Text = "Relatórios";
            this.Load += new System.EventHandler(this.frmRelatVisualizacao_Load);
            this.Shown += new System.EventHandler(this.frmRelatVisualizacao_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.objDataGridView)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.DataGridView objDataGridView;
		private System.Windows.Forms.TextBox txtTitulosTotal;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.TextBox txtNegociosTotal;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.TextBox txtValorTotal;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.ComboBox cmbSetup;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.GroupBox GroupBox2;
		private System.Windows.Forms.RadioButton rdbSemanal;
		private System.Windows.Forms.RadioButton rdbDiario;
		private System.Windows.Forms.TextBox txtIFR2Maximo;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.GroupBox GroupBox3;
		private System.Windows.Forms.TextBox txtStopGainPercentual;
		private System.Windows.Forms.RadioButton rdbStopGainPercentual;
		private System.Windows.Forms.RadioButton rdbStopGainAlijamento;
		private System.Windows.Forms.CheckBox chkAcimaMME49;
		private System.Windows.Forms.Button btnRelarioGerar;
		private System.Windows.Forms.TextBox txtData;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.TextBox txtValorCapital;
		private System.Windows.Forms.Label Label13;
		private System.Windows.Forms.TextBox txtPercentualManejo;
		private System.Windows.Forms.Label Label14;
		private System.Windows.Forms.Label lblIFRFiltro;
		private System.Windows.Forms.ComboBox cmbIFRFiltro;
		private System.Windows.Forms.MonthCalendar Calendario;
		private System.Windows.Forms.Button btnCalendario;
	}
}
