using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmDadosRecalcular : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDadosRecalcular));
            this.Panel1 = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkCotacaoDiaria = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanal = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanalDadosGeraisRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanalIFRRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanalMMExpRecalcular = new System.Windows.Forms.CheckBox();
            this.chkDataInicialUtilizar = new System.Windows.Forms.CheckBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.rdbAtivosEscolher = new System.Windows.Forms.RadioButton();
            this.rdbTodosAtivos = new System.Windows.Forms.RadioButton();
            this.pnlAtivosEscolher = new System.Windows.Forms.Panel();
            this.btnRemover = new System.Windows.Forms.Button();
            this.btnRemoverTodos = new System.Windows.Forms.Button();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.btnAdicionarTodos = new System.Windows.Forms.Button();
            this.lstAtivosEscolhidos = new System.Windows.Forms.ListBox();
            this.lstAtivosNaoEscolhidos = new System.Windows.Forms.ListBox();
            this.pnlCotacaoSemanalOpcoes = new System.Windows.Forms.Panel();
            this.chkVolatilidadeSemanal = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanalIFR2MedioRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoSemanalVolumeMedioRecalcular = new System.Windows.Forms.CheckBox();
            this.pnlCotacaoDiariaOpcoes = new System.Windows.Forms.Panel();
            this.chkVolatilidadeDiaria = new System.Windows.Forms.CheckBox();
            this.chkCotacaoDiariaIFR2MedioRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoDiariaVolumeMedioRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoDiariaMMExpRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoDiariaIFRRecalcular = new System.Windows.Forms.CheckBox();
            this.chkCotacaoDiariaOscilacaoRecalcular = new System.Windows.Forms.CheckBox();
            this.txtDataInicial = new System.Windows.Forms.TextBox();
            this.btnCalendario = new System.Windows.Forms.Button();
            this.Calendario = new System.Windows.Forms.MonthCalendar();
            this.chkMediaNegociosSemanal = new System.Windows.Forms.CheckBox();
            this.chkMediaNegociosDiaria = new System.Windows.Forms.CheckBox();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.pnlAtivosEscolher.SuspendLayout();
            this.pnlCotacaoSemanalOpcoes.SuspendLayout();
            this.pnlCotacaoDiariaOpcoes.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.btnCancelar);
            this.Panel1.Controls.Add(this.btnOK);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 537);
            this.Panel1.Name = "Panel1";
            this.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Panel1.Size = new System.Drawing.Size(536, 40);
            this.Panel1.TabIndex = 4;
            // 
            // btnCancelar
            // 
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.Location = new System.Drawing.Point(454, 0);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(76, 35);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(379, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 35);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // chkCotacaoDiaria
            // 
            this.chkCotacaoDiaria.AutoSize = true;
            this.chkCotacaoDiaria.Location = new System.Drawing.Point(12, 12);
            this.chkCotacaoDiaria.Name = "chkCotacaoDiaria";
            this.chkCotacaoDiaria.Size = new System.Drawing.Size(161, 17);
            this.chkCotacaoDiaria.TabIndex = 0;
            this.chkCotacaoDiaria.Text = "Recalcular Cotações diárias:";
            this.chkCotacaoDiaria.UseVisualStyleBackColor = true;
            this.chkCotacaoDiaria.CheckedChanged += new System.EventHandler(this.chkCotacaoDiaria_CheckedChanged);
            // 
            // chkCotacaoSemanal
            // 
            this.chkCotacaoSemanal.AutoSize = true;
            this.chkCotacaoSemanal.Location = new System.Drawing.Point(11, 117);
            this.chkCotacaoSemanal.Name = "chkCotacaoSemanal";
            this.chkCotacaoSemanal.Size = new System.Drawing.Size(177, 17);
            this.chkCotacaoSemanal.TabIndex = 1;
            this.chkCotacaoSemanal.Text = "Recalcular Cotações Semanais:";
            this.chkCotacaoSemanal.UseVisualStyleBackColor = true;
            this.chkCotacaoSemanal.CheckedChanged += new System.EventHandler(this.CotacaoSemanal_CheckedChanged);
            // 
            // chkCotacaoSemanalDadosGeraisRecalcular
            // 
            this.chkCotacaoSemanalDadosGeraisRecalcular.AutoSize = true;
            this.chkCotacaoSemanalDadosGeraisRecalcular.Location = new System.Drawing.Point(3, 3);
            this.chkCotacaoSemanalDadosGeraisRecalcular.Name = "chkCotacaoSemanalDadosGeraisRecalcular";
            this.chkCotacaoSemanalDadosGeraisRecalcular.Size = new System.Drawing.Size(93, 17);
            this.chkCotacaoSemanalDadosGeraisRecalcular.TabIndex = 0;
            this.chkCotacaoSemanalDadosGeraisRecalcular.Text = "Dados Gerais:";
            this.chkCotacaoSemanalDadosGeraisRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoSemanalIFRRecalcular
            // 
            this.chkCotacaoSemanalIFRRecalcular.AutoSize = true;
            this.chkCotacaoSemanalIFRRecalcular.Location = new System.Drawing.Point(3, 26);
            this.chkCotacaoSemanalIFRRecalcular.Name = "chkCotacaoSemanalIFRRecalcular";
            this.chkCotacaoSemanalIFRRecalcular.Size = new System.Drawing.Size(145, 17);
            this.chkCotacaoSemanalIFRRecalcular.TabIndex = 1;
            this.chkCotacaoSemanalIFRRecalcular.Text = "Indice de Força Relativa:";
            this.chkCotacaoSemanalIFRRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoSemanalMMExpRecalcular
            // 
            this.chkCotacaoSemanalMMExpRecalcular.AutoSize = true;
            this.chkCotacaoSemanalMMExpRecalcular.Location = new System.Drawing.Point(163, 3);
            this.chkCotacaoSemanalMMExpRecalcular.Name = "chkCotacaoSemanalMMExpRecalcular";
            this.chkCotacaoSemanalMMExpRecalcular.Size = new System.Drawing.Size(151, 17);
            this.chkCotacaoSemanalMMExpRecalcular.TabIndex = 2;
            this.chkCotacaoSemanalMMExpRecalcular.Text = "Média Móvel Exponencial:";
            this.chkCotacaoSemanalMMExpRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkDataInicialUtilizar
            // 
            this.chkDataInicialUtilizar.AutoSize = true;
            this.chkDataInicialUtilizar.Location = new System.Drawing.Point(9, 218);
            this.chkDataInicialUtilizar.Name = "chkDataInicialUtilizar";
            this.chkDataInicialUtilizar.Size = new System.Drawing.Size(82, 17);
            this.chkDataInicialUtilizar.TabIndex = 2;
            this.chkDataInicialUtilizar.Text = "Data Inicial:";
            this.chkDataInicialUtilizar.UseVisualStyleBackColor = true;
            this.chkDataInicialUtilizar.CheckedChanged += new System.EventHandler(this.chkDataInicialUtilizar_CheckedChanged);
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.rdbAtivosEscolher);
            this.Panel2.Controls.Add(this.rdbTodosAtivos);
            this.Panel2.Location = new System.Drawing.Point(9, 248);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(524, 48);
            this.Panel2.TabIndex = 2;
            // 
            // rdbAtivosEscolher
            // 
            this.rdbAtivosEscolher.AutoSize = true;
            this.rdbAtivosEscolher.Location = new System.Drawing.Point(3, 26);
            this.rdbAtivosEscolher.Name = "rdbAtivosEscolher";
            this.rdbAtivosEscolher.Size = new System.Drawing.Size(101, 17);
            this.rdbAtivosEscolher.TabIndex = 1;
            this.rdbAtivosEscolher.Text = "Escolher Ativos:";
            this.rdbAtivosEscolher.UseVisualStyleBackColor = true;
            this.rdbAtivosEscolher.CheckedChanged += new System.EventHandler(this.rdbAtivosEscolher_CheckedChanged);
            // 
            // rdbTodosAtivos
            // 
            this.rdbTodosAtivos.AutoSize = true;
            this.rdbTodosAtivos.Checked = true;
            this.rdbTodosAtivos.Location = new System.Drawing.Point(3, 3);
            this.rdbTodosAtivos.Name = "rdbTodosAtivos";
            this.rdbTodosAtivos.Size = new System.Drawing.Size(87, 17);
            this.rdbTodosAtivos.TabIndex = 0;
            this.rdbTodosAtivos.TabStop = true;
            this.rdbTodosAtivos.Text = "Todos Ativos";
            this.rdbTodosAtivos.UseVisualStyleBackColor = true;
            // 
            // pnlAtivosEscolher
            // 
            this.pnlAtivosEscolher.Controls.Add(this.btnRemover);
            this.pnlAtivosEscolher.Controls.Add(this.btnRemoverTodos);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionar);
            this.pnlAtivosEscolher.Controls.Add(this.btnAdicionarTodos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosEscolhidos);
            this.pnlAtivosEscolher.Controls.Add(this.lstAtivosNaoEscolhidos);
            this.pnlAtivosEscolher.Enabled = false;
            this.pnlAtivosEscolher.Location = new System.Drawing.Point(9, 297);
            this.pnlAtivosEscolher.Name = "pnlAtivosEscolher";
            this.pnlAtivosEscolher.Size = new System.Drawing.Size(524, 232);
            this.pnlAtivosEscolher.TabIndex = 15;
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
            this.lstAtivosEscolhidos.DoubleClick += new System.EventHandler(this.lstAtivosEscolhidos_DoubleClick);
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
            this.lstAtivosNaoEscolhidos.DoubleClick += new System.EventHandler(this.lstAtivosNaoEscolhidos_DoubleClick);
            // 
            // pnlCotacaoSemanalOpcoes
            // 
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkMediaNegociosSemanal);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkVolatilidadeSemanal);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkCotacaoSemanalIFR2MedioRecalcular);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkCotacaoSemanalVolumeMedioRecalcular);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkCotacaoSemanalMMExpRecalcular);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkCotacaoSemanalDadosGeraisRecalcular);
            this.pnlCotacaoSemanalOpcoes.Controls.Add(this.chkCotacaoSemanalIFRRecalcular);
            this.pnlCotacaoSemanalOpcoes.Enabled = false;
            this.pnlCotacaoSemanalOpcoes.Location = new System.Drawing.Point(38, 134);
            this.pnlCotacaoSemanalOpcoes.Name = "pnlCotacaoSemanalOpcoes";
            this.pnlCotacaoSemanalOpcoes.Size = new System.Drawing.Size(493, 73);
            this.pnlCotacaoSemanalOpcoes.TabIndex = 1;
            // 
            // chkVolatilidadeSemanal
            // 
            this.chkVolatilidadeSemanal.AutoSize = true;
            this.chkVolatilidadeSemanal.Location = new System.Drawing.Point(342, 26);
            this.chkVolatilidadeSemanal.Name = "chkVolatilidadeSemanal";
            this.chkVolatilidadeSemanal.Size = new System.Drawing.Size(83, 17);
            this.chkVolatilidadeSemanal.TabIndex = 6;
            this.chkVolatilidadeSemanal.Text = "Volatilidade:";
            this.chkVolatilidadeSemanal.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoSemanalIFR2MedioRecalcular
            // 
            this.chkCotacaoSemanalIFR2MedioRecalcular.AutoSize = true;
            this.chkCotacaoSemanalIFR2MedioRecalcular.Location = new System.Drawing.Point(342, 3);
            this.chkCotacaoSemanalIFR2MedioRecalcular.Name = "chkCotacaoSemanalIFR2MedioRecalcular";
            this.chkCotacaoSemanalIFR2MedioRecalcular.Size = new System.Drawing.Size(141, 17);
            this.chkCotacaoSemanalIFR2MedioRecalcular.TabIndex = 5;
            this.chkCotacaoSemanalIFR2MedioRecalcular.Text = "IFR 2 Média (aritmética):";
            this.chkCotacaoSemanalIFR2MedioRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoSemanalVolumeMedioRecalcular
            // 
            this.chkCotacaoSemanalVolumeMedioRecalcular.AutoSize = true;
            this.chkCotacaoSemanalVolumeMedioRecalcular.Location = new System.Drawing.Point(163, 26);
            this.chkCotacaoSemanalVolumeMedioRecalcular.Name = "chkCotacaoSemanalVolumeMedioRecalcular";
            this.chkCotacaoSemanalVolumeMedioRecalcular.Size = new System.Drawing.Size(151, 17);
            this.chkCotacaoSemanalVolumeMedioRecalcular.TabIndex = 4;
            this.chkCotacaoSemanalVolumeMedioRecalcular.Text = "Volume Médio (Aritmética):";
            this.chkCotacaoSemanalVolumeMedioRecalcular.UseVisualStyleBackColor = true;
            // 
            // pnlCotacaoDiariaOpcoes
            // 
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkMediaNegociosDiaria);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkVolatilidadeDiaria);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkCotacaoDiariaIFR2MedioRecalcular);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkCotacaoDiariaVolumeMedioRecalcular);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkCotacaoDiariaMMExpRecalcular);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkCotacaoDiariaIFRRecalcular);
            this.pnlCotacaoDiariaOpcoes.Controls.Add(this.chkCotacaoDiariaOscilacaoRecalcular);
            this.pnlCotacaoDiariaOpcoes.Enabled = false;
            this.pnlCotacaoDiariaOpcoes.Location = new System.Drawing.Point(39, 29);
            this.pnlCotacaoDiariaOpcoes.Name = "pnlCotacaoDiariaOpcoes";
            this.pnlCotacaoDiariaOpcoes.Size = new System.Drawing.Size(493, 73);
            this.pnlCotacaoDiariaOpcoes.TabIndex = 0;
            // 
            // chkVolatilidadeDiaria
            // 
            this.chkVolatilidadeDiaria.AutoSize = true;
            this.chkVolatilidadeDiaria.Location = new System.Drawing.Point(342, 26);
            this.chkVolatilidadeDiaria.Name = "chkVolatilidadeDiaria";
            this.chkVolatilidadeDiaria.Size = new System.Drawing.Size(83, 17);
            this.chkVolatilidadeDiaria.TabIndex = 7;
            this.chkVolatilidadeDiaria.Text = "Volatilidade:";
            this.chkVolatilidadeDiaria.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoDiariaIFR2MedioRecalcular
            // 
            this.chkCotacaoDiariaIFR2MedioRecalcular.AutoSize = true;
            this.chkCotacaoDiariaIFR2MedioRecalcular.Location = new System.Drawing.Point(342, 3);
            this.chkCotacaoDiariaIFR2MedioRecalcular.Name = "chkCotacaoDiariaIFR2MedioRecalcular";
            this.chkCotacaoDiariaIFR2MedioRecalcular.Size = new System.Drawing.Size(141, 17);
            this.chkCotacaoDiariaIFR2MedioRecalcular.TabIndex = 4;
            this.chkCotacaoDiariaIFR2MedioRecalcular.Text = "IFR 2 Média (aritmética):";
            this.chkCotacaoDiariaIFR2MedioRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoDiariaVolumeMedioRecalcular
            // 
            this.chkCotacaoDiariaVolumeMedioRecalcular.AutoSize = true;
            this.chkCotacaoDiariaVolumeMedioRecalcular.Location = new System.Drawing.Point(163, 26);
            this.chkCotacaoDiariaVolumeMedioRecalcular.Name = "chkCotacaoDiariaVolumeMedioRecalcular";
            this.chkCotacaoDiariaVolumeMedioRecalcular.Size = new System.Drawing.Size(151, 17);
            this.chkCotacaoDiariaVolumeMedioRecalcular.TabIndex = 3;
            this.chkCotacaoDiariaVolumeMedioRecalcular.Text = "Volume Médio (Aritmética):";
            this.chkCotacaoDiariaVolumeMedioRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoDiariaMMExpRecalcular
            // 
            this.chkCotacaoDiariaMMExpRecalcular.AutoSize = true;
            this.chkCotacaoDiariaMMExpRecalcular.Location = new System.Drawing.Point(163, 3);
            this.chkCotacaoDiariaMMExpRecalcular.Name = "chkCotacaoDiariaMMExpRecalcular";
            this.chkCotacaoDiariaMMExpRecalcular.Size = new System.Drawing.Size(151, 17);
            this.chkCotacaoDiariaMMExpRecalcular.TabIndex = 2;
            this.chkCotacaoDiariaMMExpRecalcular.Text = "Média Móvel Exponencial:";
            this.chkCotacaoDiariaMMExpRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoDiariaIFRRecalcular
            // 
            this.chkCotacaoDiariaIFRRecalcular.AutoSize = true;
            this.chkCotacaoDiariaIFRRecalcular.Location = new System.Drawing.Point(3, 26);
            this.chkCotacaoDiariaIFRRecalcular.Name = "chkCotacaoDiariaIFRRecalcular";
            this.chkCotacaoDiariaIFRRecalcular.Size = new System.Drawing.Size(145, 17);
            this.chkCotacaoDiariaIFRRecalcular.TabIndex = 1;
            this.chkCotacaoDiariaIFRRecalcular.Text = "Indice de Força Relativa:";
            this.chkCotacaoDiariaIFRRecalcular.UseVisualStyleBackColor = true;
            // 
            // chkCotacaoDiariaOscilacaoRecalcular
            // 
            this.chkCotacaoDiariaOscilacaoRecalcular.AutoSize = true;
            this.chkCotacaoDiariaOscilacaoRecalcular.Location = new System.Drawing.Point(3, 3);
            this.chkCotacaoDiariaOscilacaoRecalcular.Name = "chkCotacaoDiariaOscilacaoRecalcular";
            this.chkCotacaoDiariaOscilacaoRecalcular.Size = new System.Drawing.Size(76, 17);
            this.chkCotacaoDiariaOscilacaoRecalcular.TabIndex = 0;
            this.chkCotacaoDiariaOscilacaoRecalcular.Text = "Oscilação:";
            this.chkCotacaoDiariaOscilacaoRecalcular.UseVisualStyleBackColor = true;
            // 
            // txtDataInicial
            // 
            this.txtDataInicial.Enabled = false;
            this.txtDataInicial.Location = new System.Drawing.Point(95, 216);
            this.txtDataInicial.Name = "txtDataInicial";
            this.txtDataInicial.Size = new System.Drawing.Size(89, 20);
            this.txtDataInicial.TabIndex = 3;
            // 
            // btnCalendario
            // 
            this.btnCalendario.Enabled = false;
            this.btnCalendario.Image = ((System.Drawing.Image)(resources.GetObject("btnCalendario.Image")));
            this.btnCalendario.Location = new System.Drawing.Point(186, 213);
            this.btnCalendario.Name = "btnCalendario";
            this.btnCalendario.Size = new System.Drawing.Size(24, 24);
            this.btnCalendario.TabIndex = 16;
            this.btnCalendario.UseVisualStyleBackColor = true;
            this.btnCalendario.Click += new System.EventHandler(this.btnCalendario_Click);
            // 
            // Calendario
            // 
            this.Calendario.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Calendario.Location = new System.Drawing.Point(222, 215);
            this.Calendario.MaxSelectionCount = 1;
            this.Calendario.Name = "Calendario";
            this.Calendario.TabIndex = 17;
            this.Calendario.Visible = false;
            this.Calendario.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.Calendario_DateSelected);
            // 
            // chkMediaNegociosSemanal
            // 
            this.chkMediaNegociosSemanal.AutoSize = true;
            this.chkMediaNegociosSemanal.Location = new System.Drawing.Point(3, 49);
            this.chkMediaNegociosSemanal.Name = "chkMediaNegociosSemanal";
            this.chkMediaNegociosSemanal.Size = new System.Drawing.Size(121, 17);
            this.chkMediaNegociosSemanal.TabIndex = 7;
            this.chkMediaNegociosSemanal.Text = "Média de Negócios:";
            this.chkMediaNegociosSemanal.UseVisualStyleBackColor = true;
            // 
            // chkMediaNegociosDiaria
            // 
            this.chkMediaNegociosDiaria.AutoSize = true;
            this.chkMediaNegociosDiaria.Location = new System.Drawing.Point(3, 49);
            this.chkMediaNegociosDiaria.Name = "chkMediaNegociosDiaria";
            this.chkMediaNegociosDiaria.Size = new System.Drawing.Size(121, 17);
            this.chkMediaNegociosDiaria.TabIndex = 8;
            this.chkMediaNegociosDiaria.Text = "Média de Negócios:";
            this.chkMediaNegociosDiaria.UseVisualStyleBackColor = true;
            // 
            // frmDadosRecalcular
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(536, 577);
            this.Controls.Add(this.Calendario);
            this.Controls.Add(this.btnCalendario);
            this.Controls.Add(this.txtDataInicial);
            this.Controls.Add(this.pnlCotacaoDiariaOpcoes);
            this.Controls.Add(this.pnlCotacaoSemanalOpcoes);
            this.Controls.Add(this.pnlAtivosEscolher);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.chkDataInicialUtilizar);
            this.Controls.Add(this.chkCotacaoSemanal);
            this.Controls.Add(this.chkCotacaoDiaria);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmDadosRecalcular";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recalcular Dados";
            this.Load += new System.EventHandler(this.frmDadosRecalcular_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            this.pnlAtivosEscolher.ResumeLayout(false);
            this.pnlCotacaoSemanalOpcoes.ResumeLayout(false);
            this.pnlCotacaoSemanalOpcoes.PerformLayout();
            this.pnlCotacaoDiariaOpcoes.ResumeLayout(false);
            this.pnlCotacaoDiariaOpcoes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.Panel Panel1;
		private System.Windows.Forms.Button btnCancelar;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.CheckBox chkCotacaoDiaria;
		private System.Windows.Forms.CheckBox chkCotacaoSemanal;
		private System.Windows.Forms.CheckBox chkCotacaoSemanalDadosGeraisRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoSemanalIFRRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoSemanalMMExpRecalcular;
		private System.Windows.Forms.CheckBox chkDataInicialUtilizar;
		private System.Windows.Forms.Panel Panel2;
		private System.Windows.Forms.RadioButton rdbAtivosEscolher;
		private System.Windows.Forms.RadioButton rdbTodosAtivos;
		private System.Windows.Forms.Panel pnlAtivosEscolher;
		private System.Windows.Forms.ListBox lstAtivosEscolhidos;
		private System.Windows.Forms.ListBox lstAtivosNaoEscolhidos;
		private System.Windows.Forms.Button btnRemover;
		private System.Windows.Forms.Button btnRemoverTodos;
		private System.Windows.Forms.Button btnAdicionar;
		private System.Windows.Forms.Button btnAdicionarTodos;
		private System.Windows.Forms.Panel pnlCotacaoSemanalOpcoes;
		private System.Windows.Forms.Panel pnlCotacaoDiariaOpcoes;
		private System.Windows.Forms.CheckBox chkCotacaoDiariaMMExpRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoDiariaIFRRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoDiariaOscilacaoRecalcular;
		private System.Windows.Forms.TextBox txtDataInicial;
		private System.Windows.Forms.CheckBox chkCotacaoDiariaVolumeMedioRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoSemanalVolumeMedioRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoSemanalIFR2MedioRecalcular;
		private System.Windows.Forms.CheckBox chkCotacaoDiariaIFR2MedioRecalcular;
        private Button btnCalendario;
        private MonthCalendar Calendario;
        private CheckBox chkVolatilidadeSemanal;
        private CheckBox chkVolatilidadeDiaria;
        private CheckBox chkMediaNegociosSemanal;
        private CheckBox chkMediaNegociosDiaria;
    }
}
