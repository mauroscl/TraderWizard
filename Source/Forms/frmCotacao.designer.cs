using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
partial class frmCotacao : System.Windows.Forms.Form
	{

		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) {
				components.Dispose();
			}

			objConexao.FecharConexao();

			base.Dispose(disposing);
		}

		//Required by the Windows Form Designer

		private System.ComponentModel.IContainer components;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.  
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCotacao));
            this.DataGridView1 = new System.Windows.Forms.DataGridView();
            this.DataSet1 = new System.Data.DataSet();
            this.Label1 = new System.Windows.Forms.Label();
            this.txtAtivo = new System.Windows.Forms.TextBox();
            this.grbPetrobras = new System.Windows.Forms.GroupBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.cmbPetrobrasFormaCalculo = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtPetrobrasOscilacao = new System.Windows.Forms.TextBox();
            this.txtPETR4MediaAtual = new System.Windows.Forms.TextBox();
            this.txtPETR3MediaAtual = new System.Windows.Forms.TextBox();
            this.txtPETR4MediaAnterior = new System.Windows.Forms.TextBox();
            this.txtPETR3MediaAnterior = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.grbVale = new System.Windows.Forms.GroupBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.cmbValeFormaCalculo = new System.Windows.Forms.ComboBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtValeOscilacao = new System.Windows.Forms.TextBox();
            this.txtVALE5MediaAtual = new System.Windows.Forms.TextBox();
            this.txtVALE3MediaAtual = new System.Windows.Forms.TextBox();
            this.txtVALE5MediaAnterior = new System.Windows.Forms.TextBox();
            this.txtVALE3MediaAnterior = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.grbBancoBrasil = new System.Windows.Forms.GroupBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.cmbBBASFormaCalculo = new System.Windows.Forms.ComboBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.txtBBAS3Oscilacao = new System.Windows.Forms.TextBox();
            this.txtBBAS3MediaAtual = new System.Windows.Forms.TextBox();
            this.txtBBAS3MediaAnterior = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.Label18 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbCotacaoAtualizar = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.txtZip = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).BeginInit();
            this.grbPetrobras.SuspendLayout();
            this.grbVale.SuspendLayout();
            this.grbBancoBrasil.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView1
            // 
            this.DataGridView1.AllowUserToResizeRows = false;
            this.DataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView1.Location = new System.Drawing.Point(12, 64);
            this.DataGridView1.Name = "DataGridView1";
            this.DataGridView1.ReadOnly = true;
            this.DataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.DataGridView1.RowTemplate.ReadOnly = true;
            this.DataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView1.Size = new System.Drawing.Size(896, 350);
            this.DataGridView1.TabIndex = 0;
            this.DataGridView1.Sorted += new System.EventHandler(this.DataGridView1_Sorted);
            // 
            // DataSet1
            // 
            this.DataSet1.DataSetName = "NewDataSet";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(12, 38);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(130, 13);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Ativos (separados por \"|\"):";
            // 
            // txtAtivo
            // 
            this.txtAtivo.Location = new System.Drawing.Point(148, 34);
            this.txtAtivo.Name = "txtAtivo";
            this.txtAtivo.Size = new System.Drawing.Size(760, 20);
            this.txtAtivo.TabIndex = 3;
            // 
            // grbPetrobras
            // 
            this.grbPetrobras.Controls.Add(this.Label14);
            this.grbPetrobras.Controls.Add(this.cmbPetrobrasFormaCalculo);
            this.grbPetrobras.Controls.Add(this.Label7);
            this.grbPetrobras.Controls.Add(this.Label6);
            this.grbPetrobras.Controls.Add(this.Label5);
            this.grbPetrobras.Controls.Add(this.txtPetrobrasOscilacao);
            this.grbPetrobras.Controls.Add(this.txtPETR4MediaAtual);
            this.grbPetrobras.Controls.Add(this.txtPETR3MediaAtual);
            this.grbPetrobras.Controls.Add(this.txtPETR4MediaAnterior);
            this.grbPetrobras.Controls.Add(this.txtPETR3MediaAnterior);
            this.grbPetrobras.Controls.Add(this.Label4);
            this.grbPetrobras.Controls.Add(this.Label3);
            this.grbPetrobras.Controls.Add(this.Label2);
            this.grbPetrobras.Location = new System.Drawing.Point(15, 420);
            this.grbPetrobras.Name = "grbPetrobras";
            this.grbPetrobras.Size = new System.Drawing.Size(355, 116);
            this.grbPetrobras.TabIndex = 4;
            this.grbPetrobras.TabStop = false;
            this.grbPetrobras.Text = "Petrobras";
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label14.Location = new System.Drawing.Point(9, 19);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(92, 13);
            this.Label14.TabIndex = 17;
            this.Label14.Text = "Forma de Cálculo:";
            // 
            // cmbPetrobrasFormaCalculo
            // 
            this.cmbPetrobrasFormaCalculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPetrobrasFormaCalculo.FormattingEnabled = true;
            this.cmbPetrobrasFormaCalculo.Location = new System.Drawing.Point(107, 15);
            this.cmbPetrobrasFormaCalculo.Name = "cmbPetrobrasFormaCalculo";
            this.cmbPetrobrasFormaCalculo.Size = new System.Drawing.Size(139, 21);
            this.cmbPetrobrasFormaCalculo.TabIndex = 16;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(9, 85);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(42, 13);
            this.Label7.TabIndex = 15;
            this.Label7.Text = "PETR4";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(9, 62);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(42, 13);
            this.Label6.TabIndex = 14;
            this.Label6.Text = "PETR3";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(9, 40);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(36, 13);
            this.Label5.TabIndex = 13;
            this.Label5.Text = "Ativo";
            // 
            // txtPetrobrasOscilacao
            // 
            this.txtPetrobrasOscilacao.Location = new System.Drawing.Point(268, 59);
            this.txtPetrobrasOscilacao.Name = "txtPetrobrasOscilacao";
            this.txtPetrobrasOscilacao.Size = new System.Drawing.Size(72, 20);
            this.txtPetrobrasOscilacao.TabIndex = 12;
            // 
            // txtPETR4MediaAtual
            // 
            this.txtPETR4MediaAtual.Location = new System.Drawing.Point(174, 85);
            this.txtPETR4MediaAtual.Name = "txtPETR4MediaAtual";
            this.txtPETR4MediaAtual.Size = new System.Drawing.Size(72, 20);
            this.txtPETR4MediaAtual.TabIndex = 11;
            // 
            // txtPETR3MediaAtual
            // 
            this.txtPETR3MediaAtual.Location = new System.Drawing.Point(174, 59);
            this.txtPETR3MediaAtual.Name = "txtPETR3MediaAtual";
            this.txtPETR3MediaAtual.Size = new System.Drawing.Size(72, 20);
            this.txtPETR3MediaAtual.TabIndex = 10;
            // 
            // txtPETR4MediaAnterior
            // 
            this.txtPETR4MediaAnterior.Location = new System.Drawing.Point(65, 85);
            this.txtPETR4MediaAnterior.Name = "txtPETR4MediaAnterior";
            this.txtPETR4MediaAnterior.Size = new System.Drawing.Size(72, 20);
            this.txtPETR4MediaAnterior.TabIndex = 9;
            // 
            // txtPETR3MediaAnterior
            // 
            this.txtPETR3MediaAnterior.Location = new System.Drawing.Point(65, 59);
            this.txtPETR3MediaAnterior.Name = "txtPETR3MediaAnterior";
            this.txtPETR3MediaAnterior.Size = new System.Drawing.Size(72, 20);
            this.txtPETR3MediaAnterior.TabIndex = 8;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(268, 40);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(63, 13);
            this.Label4.TabIndex = 2;
            this.Label4.Text = "Oscilação";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(174, 40);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(74, 13);
            this.Label3.TabIndex = 1;
            this.Label3.Text = "Média Atual";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(65, 40);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(89, 13);
            this.Label2.TabIndex = 0;
            this.Label2.Text = "Média Anterior";
            // 
            // grbVale
            // 
            this.grbVale.Controls.Add(this.Label20);
            this.grbVale.Controls.Add(this.cmbValeFormaCalculo);
            this.grbVale.Controls.Add(this.Label8);
            this.grbVale.Controls.Add(this.Label9);
            this.grbVale.Controls.Add(this.Label10);
            this.grbVale.Controls.Add(this.txtValeOscilacao);
            this.grbVale.Controls.Add(this.txtVALE5MediaAtual);
            this.grbVale.Controls.Add(this.txtVALE3MediaAtual);
            this.grbVale.Controls.Add(this.txtVALE5MediaAnterior);
            this.grbVale.Controls.Add(this.txtVALE3MediaAnterior);
            this.grbVale.Controls.Add(this.Label11);
            this.grbVale.Controls.Add(this.Label12);
            this.grbVale.Controls.Add(this.Label13);
            this.grbVale.Location = new System.Drawing.Point(376, 420);
            this.grbVale.Name = "grbVale";
            this.grbVale.Size = new System.Drawing.Size(355, 116);
            this.grbVale.TabIndex = 5;
            this.grbVale.TabStop = false;
            this.grbVale.Text = "Vale";
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label20.Location = new System.Drawing.Point(9, 19);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(92, 13);
            this.Label20.TabIndex = 19;
            this.Label20.Text = "Forma de Cálculo:";
            // 
            // cmbValeFormaCalculo
            // 
            this.cmbValeFormaCalculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbValeFormaCalculo.FormattingEnabled = true;
            this.cmbValeFormaCalculo.Location = new System.Drawing.Point(107, 15);
            this.cmbValeFormaCalculo.Name = "cmbValeFormaCalculo";
            this.cmbValeFormaCalculo.Size = new System.Drawing.Size(139, 21);
            this.cmbValeFormaCalculo.TabIndex = 18;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(9, 89);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(40, 13);
            this.Label8.TabIndex = 15;
            this.Label8.Text = "VALE5";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(9, 62);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(40, 13);
            this.Label9.TabIndex = 14;
            this.Label9.Text = "VALE3";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(9, 40);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(36, 13);
            this.Label10.TabIndex = 13;
            this.Label10.Text = "Ativo";
            // 
            // txtValeOscilacao
            // 
            this.txtValeOscilacao.Location = new System.Drawing.Point(268, 58);
            this.txtValeOscilacao.Name = "txtValeOscilacao";
            this.txtValeOscilacao.Size = new System.Drawing.Size(72, 20);
            this.txtValeOscilacao.TabIndex = 12;
            // 
            // txtVALE5MediaAtual
            // 
            this.txtVALE5MediaAtual.Location = new System.Drawing.Point(174, 85);
            this.txtVALE5MediaAtual.Name = "txtVALE5MediaAtual";
            this.txtVALE5MediaAtual.Size = new System.Drawing.Size(72, 20);
            this.txtVALE5MediaAtual.TabIndex = 11;
            // 
            // txtVALE3MediaAtual
            // 
            this.txtVALE3MediaAtual.Location = new System.Drawing.Point(174, 58);
            this.txtVALE3MediaAtual.Name = "txtVALE3MediaAtual";
            this.txtVALE3MediaAtual.Size = new System.Drawing.Size(72, 20);
            this.txtVALE3MediaAtual.TabIndex = 10;
            // 
            // txtVALE5MediaAnterior
            // 
            this.txtVALE5MediaAnterior.Location = new System.Drawing.Point(65, 85);
            this.txtVALE5MediaAnterior.Name = "txtVALE5MediaAnterior";
            this.txtVALE5MediaAnterior.Size = new System.Drawing.Size(72, 20);
            this.txtVALE5MediaAnterior.TabIndex = 9;
            // 
            // txtVALE3MediaAnterior
            // 
            this.txtVALE3MediaAnterior.Location = new System.Drawing.Point(65, 58);
            this.txtVALE3MediaAnterior.Name = "txtVALE3MediaAnterior";
            this.txtVALE3MediaAnterior.Size = new System.Drawing.Size(72, 20);
            this.txtVALE3MediaAnterior.TabIndex = 8;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(268, 40);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(63, 13);
            this.Label11.TabIndex = 2;
            this.Label11.Text = "Oscilação";
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(174, 40);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(74, 13);
            this.Label12.TabIndex = 1;
            this.Label12.Text = "Média Atual";
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(65, 40);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(89, 13);
            this.Label13.TabIndex = 0;
            this.Label13.Text = "Média Anterior";
            // 
            // grbBancoBrasil
            // 
            this.grbBancoBrasil.Controls.Add(this.Label21);
            this.grbBancoBrasil.Controls.Add(this.cmbBBASFormaCalculo);
            this.grbBancoBrasil.Controls.Add(this.Label15);
            this.grbBancoBrasil.Controls.Add(this.Label16);
            this.grbBancoBrasil.Controls.Add(this.txtBBAS3Oscilacao);
            this.grbBancoBrasil.Controls.Add(this.txtBBAS3MediaAtual);
            this.grbBancoBrasil.Controls.Add(this.txtBBAS3MediaAnterior);
            this.grbBancoBrasil.Controls.Add(this.Label17);
            this.grbBancoBrasil.Controls.Add(this.Label18);
            this.grbBancoBrasil.Controls.Add(this.Label19);
            this.grbBancoBrasil.Location = new System.Drawing.Point(15, 542);
            this.grbBancoBrasil.Name = "grbBancoBrasil";
            this.grbBancoBrasil.Size = new System.Drawing.Size(355, 116);
            this.grbBancoBrasil.TabIndex = 6;
            this.grbBancoBrasil.TabStop = false;
            this.grbBancoBrasil.Text = "Banco do Brasil";
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(9, 19);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(92, 13);
            this.Label21.TabIndex = 19;
            this.Label21.Text = "Forma de Cálculo:";
            // 
            // cmbBBASFormaCalculo
            // 
            this.cmbBBASFormaCalculo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBBASFormaCalculo.FormattingEnabled = true;
            this.cmbBBASFormaCalculo.Location = new System.Drawing.Point(107, 15);
            this.cmbBBASFormaCalculo.Name = "cmbBBASFormaCalculo";
            this.cmbBBASFormaCalculo.Size = new System.Drawing.Size(139, 21);
            this.cmbBBASFormaCalculo.TabIndex = 18;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(9, 68);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(41, 13);
            this.Label15.TabIndex = 14;
            this.Label15.Text = "BBAS3";
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label16.Location = new System.Drawing.Point(9, 46);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(36, 13);
            this.Label16.TabIndex = 13;
            this.Label16.Text = "Ativo";
            // 
            // txtBBAS3Oscilacao
            // 
            this.txtBBAS3Oscilacao.Location = new System.Drawing.Point(268, 65);
            this.txtBBAS3Oscilacao.Name = "txtBBAS3Oscilacao";
            this.txtBBAS3Oscilacao.Size = new System.Drawing.Size(72, 20);
            this.txtBBAS3Oscilacao.TabIndex = 12;
            // 
            // txtBBAS3MediaAtual
            // 
            this.txtBBAS3MediaAtual.Location = new System.Drawing.Point(174, 65);
            this.txtBBAS3MediaAtual.Name = "txtBBAS3MediaAtual";
            this.txtBBAS3MediaAtual.Size = new System.Drawing.Size(72, 20);
            this.txtBBAS3MediaAtual.TabIndex = 10;
            // 
            // txtBBAS3MediaAnterior
            // 
            this.txtBBAS3MediaAnterior.Location = new System.Drawing.Point(65, 65);
            this.txtBBAS3MediaAnterior.Name = "txtBBAS3MediaAnterior";
            this.txtBBAS3MediaAnterior.Size = new System.Drawing.Size(72, 20);
            this.txtBBAS3MediaAnterior.TabIndex = 8;
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.Location = new System.Drawing.Point(268, 46);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(63, 13);
            this.Label17.TabIndex = 2;
            this.Label17.Text = "Oscilação";
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label18.Location = new System.Drawing.Point(174, 46);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(74, 13);
            this.Label18.TabIndex = 1;
            this.Label18.Text = "Média Atual";
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label19.Location = new System.Drawing.Point(65, 46);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(89, 13);
            this.Label19.TabIndex = 0;
            this.Label19.Text = "Média Anterior";
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCotacaoAtualizar,
            this.ToolStripSeparator1});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(914, 25);
            this.ToolStrip1.TabIndex = 7;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // tsbCotacaoAtualizar
            // 
            this.tsbCotacaoAtualizar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCotacaoAtualizar.Image = ((System.Drawing.Image)(resources.GetObject("tsbCotacaoAtualizar.Image")));
            this.tsbCotacaoAtualizar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCotacaoAtualizar.Name = "tsbCotacaoAtualizar";
            this.tsbCotacaoAtualizar.Size = new System.Drawing.Size(57, 22);
            this.tsbCotacaoAtualizar.Text = "&Atualizar";
            this.tsbCotacaoAtualizar.Click += new System.EventHandler(this.tsbCotacaoAtualizar_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // txtZip
            // 
            this.txtZip.Location = new System.Drawing.Point(433, 588);
            this.txtZip.Name = "txtZip";
            this.txtZip.Size = new System.Drawing.Size(67, 20);
            this.txtZip.TabIndex = 8;
            this.txtZip.Visible = false;
            // 
            // frmCotacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 663);
            this.Controls.Add(this.txtZip);
            this.Controls.Add(this.ToolStrip1);
            this.Controls.Add(this.grbBancoBrasil);
            this.Controls.Add(this.grbVale);
            this.Controls.Add(this.grbPetrobras);
            this.Controls.Add(this.txtAtivo);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.DataGridView1);
            this.Name = "frmCotacao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cotações da Bovespa";
            this.Load += new System.EventHandler(this.frmCotacao_Load);
            this.Shown += new System.EventHandler(this.frmCotacao_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).EndInit();
            this.grbPetrobras.ResumeLayout(false);
            this.grbPetrobras.PerformLayout();
            this.grbVale.ResumeLayout(false);
            this.grbVale.PerformLayout();
            this.grbBancoBrasil.ResumeLayout(false);
            this.grbBancoBrasil.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.DataGridView DataGridView1;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.TextBox txtAtivo;
		private System.Windows.Forms.GroupBox grbPetrobras;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.TextBox txtPetrobrasOscilacao;
		private System.Windows.Forms.TextBox txtPETR4MediaAtual;
		private System.Windows.Forms.TextBox txtPETR3MediaAtual;
		private System.Windows.Forms.TextBox txtPETR4MediaAnterior;
		private System.Windows.Forms.TextBox txtPETR3MediaAnterior;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.GroupBox grbVale;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Label Label9;
		private System.Windows.Forms.Label Label10;
		private System.Windows.Forms.TextBox txtValeOscilacao;
		private System.Windows.Forms.TextBox txtVALE5MediaAtual;
		private System.Windows.Forms.TextBox txtVALE3MediaAtual;
		private System.Windows.Forms.TextBox txtVALE5MediaAnterior;
		private System.Windows.Forms.TextBox txtVALE3MediaAnterior;
		private System.Windows.Forms.Label Label11;
		private System.Windows.Forms.Label Label12;
		private System.Windows.Forms.Label Label13;
		private System.Windows.Forms.GroupBox grbBancoBrasil;
		private System.Windows.Forms.Label Label15;
		private System.Windows.Forms.Label Label16;
		private System.Windows.Forms.TextBox txtBBAS3Oscilacao;
		private System.Windows.Forms.TextBox txtBBAS3MediaAtual;
		private System.Windows.Forms.TextBox txtBBAS3MediaAnterior;
		private System.Windows.Forms.Label Label17;
		private System.Windows.Forms.Label Label18;
		private System.Windows.Forms.Label Label19;
		private System.Data.DataSet DataSet1;
		private System.Windows.Forms.ToolStrip ToolStrip1;
		private System.Windows.Forms.ToolStripButton tsbCotacaoAtualizar;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		private System.Windows.Forms.TextBox txtZip;
		private System.Windows.Forms.Label Label14;
		private System.Windows.Forms.ComboBox cmbPetrobrasFormaCalculo;
		private System.Windows.Forms.Label Label20;
		private System.Windows.Forms.ComboBox cmbValeFormaCalculo;
		private System.Windows.Forms.Label Label21;
		private System.Windows.Forms.ComboBox cmbBBASFormaCalculo;
	}
}
