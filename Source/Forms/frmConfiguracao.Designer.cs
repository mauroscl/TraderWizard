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
	partial class frmConfiguracao : System.Windows.Forms.Form
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
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabInternet = new System.Windows.Forms.TabPage();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.chkCredencialUtilizar = new System.Windows.Forms.CheckBox();
            this.pnlCredencial = new System.Windows.Forms.Panel();
            this.txtSenha = new System.Windows.Forms.MaskedTextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.txtDominio = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.pnlProxyAutEndereco = new System.Windows.Forms.Panel();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.pnlProxyManual = new System.Windows.Forms.Panel();
            this.txtProxyManualHTTP = new System.Windows.Forms.TextBox();
            this.txtProxyManualPorta = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.rdbProxyEnderecoAutomatico = new System.Windows.Forms.RadioButton();
            this.rdbProxyManual = new System.Windows.Forms.RadioButton();
            this.rdbProxyAutomatico = new System.Windows.Forms.RadioButton();
            this.rdbSemProxy = new System.Windows.Forms.RadioButton();
            this.tabGrafico = new System.Windows.Forms.TabPage();
            this.pnlMMExp = new System.Windows.Forms.Panel();
            this.pnlCor = new System.Windows.Forms.Panel();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.txtPeriodo = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.lblPeriodo = new System.Windows.Forms.Label();
            this.btnRemover = new System.Windows.Forms.Button();
            this.lstPeriodoSelecionado = new System.Windows.Forms.ListView();
            this.colPeriodo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkVolumeDesenhar = new System.Windows.Forms.CheckBox();
            this.chkIFRDesenhar = new System.Windows.Forms.CheckBox();
            this.chkMMExpDesenhar = new System.Windows.Forms.CheckBox();
            this.cmbAtivo = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.tabOutros = new System.Windows.Forms.TabPage();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.txtHoraFechamentoPregao = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.txtHoraAberturaPregao = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPercentualManejo = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.txtValorCapital = new System.Windows.Forms.TextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.txtCotacaoAtivos = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.objColorDialog = new System.Windows.Forms.ColorDialog();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.TabControl1.SuspendLayout();
            this.tabInternet.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.pnlCredencial.SuspendLayout();
            this.pnlProxyAutEndereco.SuspendLayout();
            this.pnlProxyManual.SuspendLayout();
            this.tabGrafico.SuspendLayout();
            this.pnlMMExp.SuspendLayout();
            this.tabOutros.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabInternet);
            this.TabControl1.Controls.Add(this.tabGrafico);
            this.TabControl1.Controls.Add(this.tabOutros);
            this.TabControl1.Location = new System.Drawing.Point(3, 2);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(615, 366);
            this.TabControl1.TabIndex = 0;
            // 
            // tabInternet
            // 
            this.tabInternet.Controls.Add(this.GroupBox1);
            this.tabInternet.Location = new System.Drawing.Point(4, 22);
            this.tabInternet.Name = "tabInternet";
            this.tabInternet.Padding = new System.Windows.Forms.Padding(3);
            this.tabInternet.Size = new System.Drawing.Size(607, 340);
            this.tabInternet.TabIndex = 0;
            this.tabInternet.Text = "Internet";
            this.tabInternet.UseVisualStyleBackColor = true;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.chkCredencialUtilizar);
            this.GroupBox1.Controls.Add(this.pnlCredencial);
            this.GroupBox1.Controls.Add(this.pnlProxyAutEndereco);
            this.GroupBox1.Controls.Add(this.pnlProxyManual);
            this.GroupBox1.Controls.Add(this.rdbProxyEnderecoAutomatico);
            this.GroupBox1.Controls.Add(this.rdbProxyManual);
            this.GroupBox1.Controls.Add(this.rdbProxyAutomatico);
            this.GroupBox1.Controls.Add(this.rdbSemProxy);
            this.GroupBox1.Location = new System.Drawing.Point(8, 8);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(596, 325);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Acesso a Internet";
            // 
            // chkCredencialUtilizar
            // 
            this.chkCredencialUtilizar.AutoSize = true;
            this.chkCredencialUtilizar.Location = new System.Drawing.Point(18, 148);
            this.chkCredencialUtilizar.Name = "chkCredencialUtilizar";
            this.chkCredencialUtilizar.Size = new System.Drawing.Size(118, 17);
            this.chkCredencialUtilizar.TabIndex = 8;
            this.chkCredencialUtilizar.Text = "Utilizar Credenciais:";
            this.chkCredencialUtilizar.UseVisualStyleBackColor = true;
            this.chkCredencialUtilizar.CheckedChanged += new System.EventHandler(this.chkCredencialUtilizar_CheckedChanged);
            // 
            // pnlCredencial
            // 
            this.pnlCredencial.Controls.Add(this.txtSenha);
            this.pnlCredencial.Controls.Add(this.Label6);
            this.pnlCredencial.Controls.Add(this.txtUsuario);
            this.pnlCredencial.Controls.Add(this.Label5);
            this.pnlCredencial.Controls.Add(this.txtDominio);
            this.pnlCredencial.Controls.Add(this.Label3);
            this.pnlCredencial.Enabled = false;
            this.pnlCredencial.Location = new System.Drawing.Point(17, 170);
            this.pnlCredencial.Name = "pnlCredencial";
            this.pnlCredencial.Size = new System.Drawing.Size(571, 100);
            this.pnlCredencial.TabIndex = 7;
            // 
            // txtSenha
            // 
            this.txtSenha.Location = new System.Drawing.Point(61, 61);
            this.txtSenha.Name = "txtSenha";
            this.txtSenha.Size = new System.Drawing.Size(367, 20);
            this.txtSenha.TabIndex = 19;
            this.txtSenha.UseSystemPasswordChar = true;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(4, 64);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(41, 13);
            this.Label6.TabIndex = 18;
            this.Label6.Text = "Senha:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(61, 37);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(367, 20);
            this.txtUsuario.TabIndex = 17;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(4, 40);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(46, 13);
            this.Label5.TabIndex = 16;
            this.Label5.Text = "Usuário:";
            // 
            // txtDominio
            // 
            this.txtDominio.Location = new System.Drawing.Point(61, 12);
            this.txtDominio.Name = "txtDominio";
            this.txtDominio.Size = new System.Drawing.Size(367, 20);
            this.txtDominio.TabIndex = 15;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(4, 15);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(50, 13);
            this.Label3.TabIndex = 14;
            this.Label3.Text = "Domínio:";
            // 
            // pnlProxyAutEndereco
            // 
            this.pnlProxyAutEndereco.Controls.Add(this.TextBox1);
            this.pnlProxyAutEndereco.Controls.Add(this.Label4);
            this.pnlProxyAutEndereco.Location = new System.Drawing.Point(18, 286);
            this.pnlProxyAutEndereco.Name = "pnlProxyAutEndereco";
            this.pnlProxyAutEndereco.Size = new System.Drawing.Size(573, 42);
            this.pnlProxyAutEndereco.TabIndex = 6;
            this.pnlProxyAutEndereco.Visible = false;
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(48, 7);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(379, 20);
            this.TextBox1.TabIndex = 7;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(3, 10);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(39, 13);
            this.Label4.TabIndex = 4;
            this.Label4.Text = "HTTP:";
            // 
            // pnlProxyManual
            // 
            this.pnlProxyManual.Controls.Add(this.txtProxyManualHTTP);
            this.pnlProxyManual.Controls.Add(this.txtProxyManualPorta);
            this.pnlProxyManual.Controls.Add(this.Label2);
            this.pnlProxyManual.Controls.Add(this.Label1);
            this.pnlProxyManual.Enabled = false;
            this.pnlProxyManual.Location = new System.Drawing.Point(18, 100);
            this.pnlProxyManual.Name = "pnlProxyManual";
            this.pnlProxyManual.Size = new System.Drawing.Size(573, 42);
            this.pnlProxyManual.TabIndex = 5;
            // 
            // txtProxyManualHTTP
            // 
            this.txtProxyManualHTTP.Location = new System.Drawing.Point(60, 7);
            this.txtProxyManualHTTP.Name = "txtProxyManualHTTP";
            this.txtProxyManualHTTP.Size = new System.Drawing.Size(367, 20);
            this.txtProxyManualHTTP.TabIndex = 7;
            // 
            // txtProxyManualPorta
            // 
            this.txtProxyManualPorta.Location = new System.Drawing.Point(487, 7);
            this.txtProxyManualPorta.Name = "txtProxyManualPorta";
            this.txtProxyManualPorta.Size = new System.Drawing.Size(83, 20);
            this.txtProxyManualPorta.TabIndex = 6;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(446, 10);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(35, 13);
            this.Label2.TabIndex = 5;
            this.Label2.Text = "Porta:";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(3, 10);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(39, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "HTTP:";
            // 
            // rdbProxyEnderecoAutomatico
            // 
            this.rdbProxyEnderecoAutomatico.AutoSize = true;
            this.rdbProxyEnderecoAutomatico.Location = new System.Drawing.Point(21, 276);
            this.rdbProxyEnderecoAutomatico.Name = "rdbProxyEnderecoAutomatico";
            this.rdbProxyEnderecoAutomatico.Size = new System.Drawing.Size(258, 17);
            this.rdbProxyEnderecoAutomatico.TabIndex = 3;
            this.rdbProxyEnderecoAutomatico.TabStop = true;
            this.rdbProxyEnderecoAutomatico.Text = "Endereço para configuração automática de proxy";
            this.rdbProxyEnderecoAutomatico.UseVisualStyleBackColor = true;
            this.rdbProxyEnderecoAutomatico.Visible = false;
            this.rdbProxyEnderecoAutomatico.CheckedChanged += new System.EventHandler(this.rdbProxyEnderecoAutomatico_CheckedChanged);
            // 
            // rdbProxyManual
            // 
            this.rdbProxyManual.AutoSize = true;
            this.rdbProxyManual.Location = new System.Drawing.Point(18, 77);
            this.rdbProxyManual.Name = "rdbProxyManual";
            this.rdbProxyManual.Size = new System.Drawing.Size(168, 17);
            this.rdbProxyManual.TabIndex = 2;
            this.rdbProxyManual.TabStop = true;
            this.rdbProxyManual.Text = "Configuração manual de proxy";
            this.rdbProxyManual.UseVisualStyleBackColor = true;
            this.rdbProxyManual.CheckedChanged += new System.EventHandler(this.rdbProxyManual_CheckedChanged);
            // 
            // rdbProxyAutomatico
            // 
            this.rdbProxyAutomatico.AutoSize = true;
            this.rdbProxyAutomatico.Location = new System.Drawing.Point(18, 54);
            this.rdbProxyAutomatico.Name = "rdbProxyAutomatico";
            this.rdbProxyAutomatico.Size = new System.Drawing.Size(213, 17);
            this.rdbProxyAutomatico.TabIndex = 1;
            this.rdbProxyAutomatico.TabStop = true;
            this.rdbProxyAutomatico.Text = "Autodetectar as configurações de proxy";
            this.rdbProxyAutomatico.UseVisualStyleBackColor = true;
            // 
            // rdbSemProxy
            // 
            this.rdbSemProxy.AutoSize = true;
            this.rdbSemProxy.Location = new System.Drawing.Point(17, 31);
            this.rdbSemProxy.Name = "rdbSemProxy";
            this.rdbSemProxy.Size = new System.Drawing.Size(74, 17);
            this.rdbSemProxy.TabIndex = 0;
            this.rdbSemProxy.TabStop = true;
            this.rdbSemProxy.Text = "Sem proxy";
            this.rdbSemProxy.UseVisualStyleBackColor = true;
            // 
            // tabGrafico
            // 
            this.tabGrafico.Controls.Add(this.pnlMMExp);
            this.tabGrafico.Controls.Add(this.chkVolumeDesenhar);
            this.tabGrafico.Controls.Add(this.chkIFRDesenhar);
            this.tabGrafico.Controls.Add(this.chkMMExpDesenhar);
            this.tabGrafico.Controls.Add(this.cmbAtivo);
            this.tabGrafico.Controls.Add(this.Label7);
            this.tabGrafico.Location = new System.Drawing.Point(4, 22);
            this.tabGrafico.Name = "tabGrafico";
            this.tabGrafico.Padding = new System.Windows.Forms.Padding(3);
            this.tabGrafico.Size = new System.Drawing.Size(607, 340);
            this.tabGrafico.TabIndex = 1;
            this.tabGrafico.Text = "Gráfico";
            this.tabGrafico.UseVisualStyleBackColor = true;
            // 
            // pnlMMExp
            // 
            this.pnlMMExp.Controls.Add(this.pnlCor);
            this.pnlMMExp.Controls.Add(this.btnRemoverTodos);
            this.pnlMMExp.Controls.Add(this.btnAdicionar);
            this.pnlMMExp.Controls.Add(this.txtPeriodo);
            this.pnlMMExp.Controls.Add(this.Label8);
            this.pnlMMExp.Controls.Add(this.lblPeriodo);
            this.pnlMMExp.Controls.Add(this.btnRemover);
            this.pnlMMExp.Controls.Add(this.lstPeriodoSelecionado);
            this.pnlMMExp.Enabled = false;
            this.pnlMMExp.Location = new System.Drawing.Point(6, 60);
            this.pnlMMExp.Name = "pnlMMExp";
            this.pnlMMExp.Size = new System.Drawing.Size(525, 216);
            this.pnlMMExp.TabIndex = 5;
            // 
            // pnlCor
            // 
            this.pnlCor.BackColor = System.Drawing.Color.Red;
            this.pnlCor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCor.Location = new System.Drawing.Point(186, 8);
            this.pnlCor.Name = "pnlCor";
            this.pnlCor.Size = new System.Drawing.Size(44, 19);
            this.pnlCor.TabIndex = 7;
            this.pnlCor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlCor_MouseClick);
            // 
            // btnRemoverTodos
            // 
            this.btnRemoverTodos.Location = new System.Drawing.Point(246, 106);
            this.btnRemoverTodos.Name = "btnRemoverTodos";
            this.btnRemoverTodos.Size = new System.Drawing.Size(75, 37);
            this.btnRemoverTodos.TabIndex = 13;
            this.btnRemoverTodos.Text = "Remover &Todos";
            this.btnRemoverTodos.UseVisualStyleBackColor = true;
            this.btnRemoverTodos.Click += new System.EventHandler(this.btnRemoverTodos_Click);
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.Location = new System.Drawing.Point(246, 7);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(75, 37);
            this.btnAdicionar.TabIndex = 9;
            this.btnAdicionar.Text = "&Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = true;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // txtPeriodo
            // 
            this.txtPeriodo.Location = new System.Drawing.Point(57, 7);
            this.txtPeriodo.Name = "txtPeriodo";
            this.txtPeriodo.Size = new System.Drawing.Size(91, 20);
            this.txtPeriodo.TabIndex = 6;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(154, 10);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(26, 13);
            this.Label8.TabIndex = 11;
            this.Label8.Text = "Cor:";
            // 
            // lblPeriodo
            // 
            this.lblPeriodo.AutoSize = true;
            this.lblPeriodo.Location = new System.Drawing.Point(3, 10);
            this.lblPeriodo.Name = "lblPeriodo";
            this.lblPeriodo.Size = new System.Drawing.Size(48, 13);
            this.lblPeriodo.TabIndex = 8;
            this.lblPeriodo.Text = "Período:";
            // 
            // btnRemover
            // 
            this.btnRemover.Location = new System.Drawing.Point(246, 63);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(75, 37);
            this.btnRemover.TabIndex = 12;
            this.btnRemover.Text = "&Remover";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            // 
            // lstPeriodoSelecionado
            // 
            this.lstPeriodoSelecionado.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colPeriodo,
            this.colCor});
            this.lstPeriodoSelecionado.FullRowSelect = true;
            this.lstPeriodoSelecionado.GridLines = true;
            this.lstPeriodoSelecionado.Location = new System.Drawing.Point(6, 63);
            this.lstPeriodoSelecionado.Name = "lstPeriodoSelecionado";
            this.lstPeriodoSelecionado.Size = new System.Drawing.Size(224, 144);
            this.lstPeriodoSelecionado.TabIndex = 10;
            this.lstPeriodoSelecionado.UseCompatibleStateImageBehavior = false;
            this.lstPeriodoSelecionado.View = System.Windows.Forms.View.Details;
            // 
            // colPeriodo
            // 
            this.colPeriodo.Text = "Períodos";
            this.colPeriodo.Width = 140;
            // 
            // colCor
            // 
            this.colCor.Text = "Cor";
            // 
            // chkVolumeDesenhar
            // 
            this.chkVolumeDesenhar.AutoSize = true;
            this.chkVolumeDesenhar.Location = new System.Drawing.Point(10, 305);
            this.chkVolumeDesenhar.Name = "chkVolumeDesenhar";
            this.chkVolumeDesenhar.Size = new System.Drawing.Size(113, 17);
            this.chkVolumeDesenhar.TabIndex = 4;
            this.chkVolumeDesenhar.Text = "Desenhar Volume:";
            this.chkVolumeDesenhar.UseVisualStyleBackColor = true;
            // 
            // chkIFRDesenhar
            // 
            this.chkIFRDesenhar.AutoSize = true;
            this.chkIFRDesenhar.Location = new System.Drawing.Point(10, 282);
            this.chkIFRDesenhar.Name = "chkIFRDesenhar";
            this.chkIFRDesenhar.Size = new System.Drawing.Size(194, 17);
            this.chkIFRDesenhar.TabIndex = 3;
            this.chkIFRDesenhar.Text = "Desenhar Indice de Força Relativa:";
            this.chkIFRDesenhar.UseVisualStyleBackColor = true;
            // 
            // chkMMExpDesenhar
            // 
            this.chkMMExpDesenhar.AutoSize = true;
            this.chkMMExpDesenhar.Location = new System.Drawing.Point(10, 37);
            this.chkMMExpDesenhar.Name = "chkMMExpDesenhar";
            this.chkMMExpDesenhar.Size = new System.Drawing.Size(200, 17);
            this.chkMMExpDesenhar.TabIndex = 2;
            this.chkMMExpDesenhar.Text = "Desenhar Média Móvel Exponencial:";
            this.chkMMExpDesenhar.UseVisualStyleBackColor = true;
            this.chkMMExpDesenhar.CheckedChanged += new System.EventHandler(this.chkMMExpDesenhar_CheckedChanged);
            // 
            // cmbAtivo
            // 
            this.cmbAtivo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbAtivo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbAtivo.FormattingEnabled = true;
            this.cmbAtivo.Location = new System.Drawing.Point(91, 6);
            this.cmbAtivo.Name = "cmbAtivo";
            this.cmbAtivo.Size = new System.Drawing.Size(179, 21);
            this.cmbAtivo.TabIndex = 1;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(10, 9);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(79, 13);
            this.Label7.TabIndex = 0;
            this.Label7.Text = "Ativo Preferido:";
            // 
            // tabOutros
            // 
            this.tabOutros.Controls.Add(this.GroupBox3);
            this.tabOutros.Controls.Add(this.GroupBox2);
            this.tabOutros.Controls.Add(this.txtCotacaoAtivos);
            this.tabOutros.Controls.Add(this.Label9);
            this.tabOutros.Location = new System.Drawing.Point(4, 22);
            this.tabOutros.Name = "tabOutros";
            this.tabOutros.Size = new System.Drawing.Size(607, 340);
            this.tabOutros.TabIndex = 2;
            this.tabOutros.Text = "Outros";
            this.tabOutros.UseVisualStyleBackColor = true;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.txtHoraFechamentoPregao);
            this.GroupBox3.Controls.Add(this.Label10);
            this.GroupBox3.Controls.Add(this.txtHoraAberturaPregao);
            this.GroupBox3.Controls.Add(this.Label11);
            this.GroupBox3.Location = new System.Drawing.Point(5, 129);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(596, 83);
            this.GroupBox3.TabIndex = 10;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Horário do Pregão";
            // 
            // txtHoraFechamentoPregao
            // 
            this.txtHoraFechamentoPregao.BackColor = System.Drawing.Color.White;
            this.txtHoraFechamentoPregao.Location = new System.Drawing.Point(127, 48);
            this.txtHoraFechamentoPregao.Name = "txtHoraFechamentoPregao";
            this.txtHoraFechamentoPregao.Size = new System.Drawing.Size(122, 20);
            this.txtHoraFechamentoPregao.TabIndex = 31;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(6, 50);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(69, 13);
            this.Label10.TabIndex = 32;
            this.Label10.Text = "Fechamento:";
            // 
            // txtHoraAberturaPregao
            // 
            this.txtHoraAberturaPregao.BackColor = System.Drawing.Color.White;
            this.txtHoraAberturaPregao.Location = new System.Drawing.Point(127, 22);
            this.txtHoraAberturaPregao.Name = "txtHoraAberturaPregao";
            this.txtHoraAberturaPregao.Size = new System.Drawing.Size(122, 20);
            this.txtHoraAberturaPregao.TabIndex = 29;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(6, 25);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(50, 13);
            this.Label11.TabIndex = 30;
            this.Label11.Text = "Abertura:";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.txtPercentualManejo);
            this.GroupBox2.Controls.Add(this.Label14);
            this.GroupBox2.Controls.Add(this.txtValorCapital);
            this.GroupBox2.Controls.Add(this.Label13);
            this.GroupBox2.Location = new System.Drawing.Point(3, 32);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(596, 83);
            this.GroupBox2.TabIndex = 6;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Setups de Entrada";
            // 
            // txtPercentualManejo
            // 
            this.txtPercentualManejo.BackColor = System.Drawing.Color.White;
            this.txtPercentualManejo.Location = new System.Drawing.Point(127, 48);
            this.txtPercentualManejo.Name = "txtPercentualManejo";
            this.txtPercentualManejo.Size = new System.Drawing.Size(122, 20);
            this.txtPercentualManejo.TabIndex = 31;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(6, 50);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(56, 13);
            this.Label14.TabIndex = 32;
            this.Label14.Text = "% Manejo:";
            // 
            // txtValorCapital
            // 
            this.txtValorCapital.BackColor = System.Drawing.Color.White;
            this.txtValorCapital.Location = new System.Drawing.Point(127, 22);
            this.txtValorCapital.Name = "txtValorCapital";
            this.txtValorCapital.Size = new System.Drawing.Size(122, 20);
            this.txtValorCapital.TabIndex = 29;
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(6, 25);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(84, 13);
            this.Label13.TabIndex = 30;
            this.Label13.Text = "Valor do Capital:";
            // 
            // txtCotacaoAtivos
            // 
            this.txtCotacaoAtivos.Location = new System.Drawing.Point(130, 6);
            this.txtCotacaoAtivos.Name = "txtCotacaoAtivos";
            this.txtCotacaoAtivos.Size = new System.Drawing.Size(469, 20);
            this.txtCotacaoAtivos.TabIndex = 9;
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(5, 8);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(134, 17);
            this.Label9.TabIndex = 8;
            this.Label9.Text = "Ativos (Cotações Online):";
            // 
            // objColorDialog
            // 
            this.objColorDialog.AnyColor = true;
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 372);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(618, 40);
            this.Panel1.TabIndex = 5;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(537, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(462, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmConfiguracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 412);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.TabControl1);
            this.MaximizeBox = false;
            this.Name = "frmConfiguracao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuracões";
            this.TabControl1.ResumeLayout(false);
            this.tabInternet.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.pnlCredencial.ResumeLayout(false);
            this.pnlCredencial.PerformLayout();
            this.pnlProxyAutEndereco.ResumeLayout(false);
            this.pnlProxyAutEndereco.PerformLayout();
            this.pnlProxyManual.ResumeLayout(false);
            this.pnlProxyManual.PerformLayout();
            this.tabGrafico.ResumeLayout(false);
            this.tabGrafico.PerformLayout();
            this.pnlMMExp.ResumeLayout(false);
            this.pnlMMExp.PerformLayout();
            this.tabOutros.ResumeLayout(false);
            this.tabOutros.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.TabControl TabControl1;
		private System.Windows.Forms.TabPage tabInternet;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton rdbProxyEnderecoAutomatico;
		private System.Windows.Forms.RadioButton rdbProxyManual;
		private System.Windows.Forms.RadioButton rdbProxyAutomatico;
		private System.Windows.Forms.RadioButton rdbSemProxy;
		private System.Windows.Forms.Panel pnlProxyManual;
		private System.Windows.Forms.Label Label2;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.TextBox txtProxyManualHTTP;
		private System.Windows.Forms.TextBox txtProxyManualPorta;
		private System.Windows.Forms.Panel pnlProxyAutEndereco;
		private System.Windows.Forms.TextBox TextBox1;
		private System.Windows.Forms.Label Label4;
		private System.Windows.Forms.Panel pnlCredencial;
		private System.Windows.Forms.Label Label6;
		private System.Windows.Forms.TextBox txtUsuario;
		private System.Windows.Forms.Label Label5;
		private System.Windows.Forms.TextBox txtDominio;
		private System.Windows.Forms.Label Label3;
		private System.Windows.Forms.CheckBox chkCredencialUtilizar;
		private System.Windows.Forms.MaskedTextBox txtSenha;
		private System.Windows.Forms.TabPage tabGrafico;
		private System.Windows.Forms.CheckBox chkVolumeDesenhar;
		private System.Windows.Forms.CheckBox chkIFRDesenhar;
		private System.Windows.Forms.CheckBox chkMMExpDesenhar;
		private System.Windows.Forms.ComboBox cmbAtivo;
		private System.Windows.Forms.Label Label7;
		private System.Windows.Forms.Panel pnlMMExp;
		private System.Windows.Forms.Panel pnlCor;
		private System.Windows.Forms.Button btnRemoverTodos;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.TextBox txtPeriodo;
		private System.Windows.Forms.Label Label8;
		private System.Windows.Forms.Label lblPeriodo;
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.ListView lstPeriodoSelecionado;
		private System.Windows.Forms.ColumnHeader colPeriodo;
		private System.Windows.Forms.ColumnHeader colCor;
		private System.Windows.Forms.ColorDialog objColorDialog;
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TabPage tabOutros;
		private System.Windows.Forms.TextBox txtCotacaoAtivos;
		private System.Windows.Forms.Label Label9;
		private System.Windows.Forms.GroupBox GroupBox2;
		private System.Windows.Forms.TextBox txtPercentualManejo;
		private System.Windows.Forms.Label Label14;
		private System.Windows.Forms.TextBox txtValorCapital;
		private System.Windows.Forms.Label Label13;
		private System.Windows.Forms.GroupBox GroupBox3;
		private System.Windows.Forms.TextBox txtHoraFechamentoPregao;
		private System.Windows.Forms.Label Label10;
		private System.Windows.Forms.TextBox txtHoraAberturaPregao;
		private System.Windows.Forms.Label Label11;
	}
}
