using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace TraderWizard
{
	partial class frmPrincipal : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPrincipal));
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.PrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrintPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrintSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.EditMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RedoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.CutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.SelectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniConfiguracoes = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCotacaoOnline = new System.Windows.Forms.ToolStripMenuItem();
            this.mniGrafico = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuSetupDeEntrada = new System.Windows.Forms.ToolStripMenuItem();
            this.OperaçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AtualizarCotaçõesHistóricasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mniDadosRecalcular = new System.Windows.Forms.ToolStripMenuItem();
            this.mniProventoAtualizar = new System.Windows.Forms.ToolStripMenuItem();
            this.mniProventoCadastrar = new System.Windows.Forms.ToolStripMenuItem();
            this.mniIFRCalcular = new System.Windows.Forms.ToolStripMenuItem();
            this.mniIFRDiarioSimular = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSequencialCalcular = new System.Windows.Forms.ToolStripMenuItem();
            this.AtualizarCotaçõesSemanaisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOutrasOperacoes = new System.Windows.Forms.ToolStripMenuItem();
            this.mniBackTestUnitTest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuitemCotacaoExcluir = new System.Windows.Forms.ToolStripMenuItem();
            this.WindowsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.NewWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CascadeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TileVerticalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TileHorizontalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ArrangeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IndexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.SaveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.PrintToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.PrintPreviewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.HelpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.MenuStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.EditMenu,
            this.ViewMenu,
            this.ToolsMenu,
            this.WindowsMenu,
            this.HelpMenu});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.MdiWindowListItem = this.WindowsMenu;
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(632, 24);
            this.MenuStrip.TabIndex = 5;
            this.MenuStrip.Text = "MenuStrip";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripMenuItem,
            this.OpenToolStripMenuItem,
            this.ToolStripSeparator3,
            this.SaveToolStripMenuItem,
            this.SaveAsToolStripMenuItem,
            this.ToolStripSeparator4,
            this.PrintToolStripMenuItem,
            this.PrintPreviewToolStripMenuItem,
            this.PrintSetupToolStripMenuItem,
            this.ToolStripSeparator5,
            this.ExitToolStripMenuItem});
            this.FileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(61, 20);
            this.FileMenu.Text = "&Arquivo";
            // 
            // NewToolStripMenuItem
            // 
            this.NewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("NewToolStripMenuItem.Image")));
            this.NewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            this.NewToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.NewToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.NewToolStripMenuItem.Text = "&New";
            this.NewToolStripMenuItem.Visible = false;
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolStripMenuItem.Image")));
            this.OpenToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.OpenToolStripMenuItem.Text = "&Open";
            this.OpenToolStripMenuItem.Visible = false;
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(143, 6);
            this.ToolStripSeparator3.Visible = false;
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SaveToolStripMenuItem.Image")));
            this.SaveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.SaveToolStripMenuItem.Text = "&Save";
            this.SaveToolStripMenuItem.Visible = false;
            // 
            // SaveAsToolStripMenuItem
            // 
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.SaveAsToolStripMenuItem.Text = "Save &As";
            this.SaveAsToolStripMenuItem.Visible = false;
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(143, 6);
            this.ToolStripSeparator4.Visible = false;
            // 
            // PrintToolStripMenuItem
            // 
            this.PrintToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("PrintToolStripMenuItem.Image")));
            this.PrintToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem";
            this.PrintToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.PrintToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.PrintToolStripMenuItem.Text = "&Print";
            this.PrintToolStripMenuItem.Visible = false;
            // 
            // PrintPreviewToolStripMenuItem
            // 
            this.PrintPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("PrintPreviewToolStripMenuItem.Image")));
            this.PrintPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.PrintPreviewToolStripMenuItem.Name = "PrintPreviewToolStripMenuItem";
            this.PrintPreviewToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.PrintPreviewToolStripMenuItem.Text = "Print Pre&view";
            this.PrintPreviewToolStripMenuItem.Visible = false;
            // 
            // PrintSetupToolStripMenuItem
            // 
            this.PrintSetupToolStripMenuItem.Name = "PrintSetupToolStripMenuItem";
            this.PrintSetupToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.PrintSetupToolStripMenuItem.Text = "Print Setup";
            this.PrintSetupToolStripMenuItem.Visible = false;
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(143, 6);
            this.ToolStripSeparator5.Visible = false;
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.ExitToolStripMenuItem.Text = "&Sair";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // EditMenu
            // 
            this.EditMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UndoToolStripMenuItem,
            this.RedoToolStripMenuItem,
            this.ToolStripSeparator6,
            this.CutToolStripMenuItem,
            this.CopyToolStripMenuItem,
            this.PasteToolStripMenuItem,
            this.ToolStripSeparator7,
            this.SelectAllToolStripMenuItem,
            this.mniConfiguracoes});
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Size = new System.Drawing.Size(49, 20);
            this.EditMenu.Text = "&Editar";
            // 
            // UndoToolStripMenuItem
            // 
            this.UndoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("UndoToolStripMenuItem.Image")));
            this.UndoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.UndoToolStripMenuItem.Name = "UndoToolStripMenuItem";
            this.UndoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.UndoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.UndoToolStripMenuItem.Text = "&Undo";
            this.UndoToolStripMenuItem.Visible = false;
            // 
            // RedoToolStripMenuItem
            // 
            this.RedoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("RedoToolStripMenuItem.Image")));
            this.RedoToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.RedoToolStripMenuItem.Name = "RedoToolStripMenuItem";
            this.RedoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.RedoToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.RedoToolStripMenuItem.Text = "&Redo";
            this.RedoToolStripMenuItem.Visible = false;
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(161, 6);
            this.ToolStripSeparator6.Visible = false;
            // 
            // CutToolStripMenuItem
            // 
            this.CutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CutToolStripMenuItem.Image")));
            this.CutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.CutToolStripMenuItem.Name = "CutToolStripMenuItem";
            this.CutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.CutToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.CutToolStripMenuItem.Text = "Cu&t";
            this.CutToolStripMenuItem.Visible = false;
            // 
            // CopyToolStripMenuItem
            // 
            this.CopyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("CopyToolStripMenuItem.Image")));
            this.CopyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem";
            this.CopyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.CopyToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.CopyToolStripMenuItem.Text = "&Copy";
            this.CopyToolStripMenuItem.Visible = false;
            // 
            // PasteToolStripMenuItem
            // 
            this.PasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("PasteToolStripMenuItem.Image")));
            this.PasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.PasteToolStripMenuItem.Name = "PasteToolStripMenuItem";
            this.PasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.PasteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.PasteToolStripMenuItem.Text = "&Paste";
            this.PasteToolStripMenuItem.Visible = false;
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(161, 6);
            this.ToolStripSeparator7.Visible = false;
            // 
            // SelectAllToolStripMenuItem
            // 
            this.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem";
            this.SelectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.SelectAllToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.SelectAllToolStripMenuItem.Text = "Select &All";
            this.SelectAllToolStripMenuItem.Visible = false;
            // 
            // mniConfiguracoes
            // 
            this.mniConfiguracoes.Name = "mniConfiguracoes";
            this.mniConfiguracoes.Size = new System.Drawing.Size(164, 22);
            this.mniConfiguracoes.Text = "&Configurações...";
            this.mniConfiguracoes.Click += new System.EventHandler(this.mniConfiguracoes_Click);
            // 
            // ViewMenu
            // 
            this.ViewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolBarToolStripMenuItem,
            this.StatusBarToolStripMenuItem});
            this.ViewMenu.Name = "ViewMenu";
            this.ViewMenu.Size = new System.Drawing.Size(44, 20);
            this.ViewMenu.Text = "&View";
            this.ViewMenu.Visible = false;
            // 
            // ToolBarToolStripMenuItem
            // 
            this.ToolBarToolStripMenuItem.Checked = true;
            this.ToolBarToolStripMenuItem.CheckOnClick = true;
            this.ToolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolBarToolStripMenuItem.Name = "ToolBarToolStripMenuItem";
            this.ToolBarToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.ToolBarToolStripMenuItem.Text = "&Toolbar";
            // 
            // StatusBarToolStripMenuItem
            // 
            this.StatusBarToolStripMenuItem.Checked = true;
            this.StatusBarToolStripMenuItem.CheckOnClick = true;
            this.StatusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StatusBarToolStripMenuItem.Name = "StatusBarToolStripMenuItem";
            this.StatusBarToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.StatusBarToolStripMenuItem.Text = "&Status Bar";
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCotacaoOnline,
            this.mniGrafico,
            this.toolStripMenuSetupDeEntrada,
            this.OperaçõesToolStripMenuItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(84, 20);
            this.ToolsMenu.Text = "&Ferramentas";
            // 
            // mniCotacaoOnline
            // 
            this.mniCotacaoOnline.Name = "mniCotacaoOnline";
            this.mniCotacaoOnline.Size = new System.Drawing.Size(163, 22);
            this.mniCotacaoOnline.Text = "Cotação &Online";
            this.mniCotacaoOnline.Visible = false;
            this.mniCotacaoOnline.Click += new System.EventHandler(this.mniCotacaoOnline_Click);
            // 
            // mniGrafico
            // 
            this.mniGrafico.Name = "mniGrafico";
            this.mniGrafico.Size = new System.Drawing.Size(163, 22);
            this.mniGrafico.Text = "&Gráficos";
            this.mniGrafico.Click += new System.EventHandler(this.mniGrafico_Click);
            // 
            // toolStripMenuSetupDeEntrada
            // 
            this.toolStripMenuSetupDeEntrada.Name = "toolStripMenuSetupDeEntrada";
            this.toolStripMenuSetupDeEntrada.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuSetupDeEntrada.Text = "&Setup de Entrada";
            this.toolStripMenuSetupDeEntrada.Click += new System.EventHandler(this.toolStripMenuSetupDeEntrada_Click);
            // 
            // OperaçõesToolStripMenuItem
            // 
            this.OperaçõesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AtualizarCotaçõesHistóricasToolStripMenuItem,
            this.mniDadosRecalcular,
            this.mniProventoAtualizar,
            this.mniProventoCadastrar,
            this.mniIFRCalcular,
            this.mniIFRDiarioSimular,
            this.mnuSequencialCalcular,
            this.AtualizarCotaçõesSemanaisToolStripMenuItem,
            this.mnuOutrasOperacoes,
            this.mnuitemCotacaoExcluir});
            this.OperaçõesToolStripMenuItem.Name = "OperaçõesToolStripMenuItem";
            this.OperaçõesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.OperaçõesToolStripMenuItem.Text = "&Operações";
            // 
            // AtualizarCotaçõesHistóricasToolStripMenuItem
            // 
            this.AtualizarCotaçõesHistóricasToolStripMenuItem.Name = "AtualizarCotaçõesHistóricasToolStripMenuItem";
            this.AtualizarCotaçõesHistóricasToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.AtualizarCotaçõesHistóricasToolStripMenuItem.Text = "Atualizar &Cotações...";
            this.AtualizarCotaçõesHistóricasToolStripMenuItem.Click += new System.EventHandler(this.AtualizarCotaçõesHistóricasToolStripMenuItem_Click);
            // 
            // mniDadosRecalcular
            // 
            this.mniDadosRecalcular.Name = "mniDadosRecalcular";
            this.mniDadosRecalcular.Size = new System.Drawing.Size(225, 22);
            this.mniDadosRecalcular.Text = "Recalcular Dados...";
            this.mniDadosRecalcular.Click += new System.EventHandler(this.mniDadosRecalcular_Click);
            // 
            // mniProventoAtualizar
            // 
            this.mniProventoAtualizar.Name = "mniProventoAtualizar";
            this.mniProventoAtualizar.Size = new System.Drawing.Size(225, 22);
            this.mniProventoAtualizar.Text = "&Importar Proventos...";
            this.mniProventoAtualizar.Click += new System.EventHandler(this.mniProventoAtualizar_Click);
            // 
            // mniProventoCadastrar
            // 
            this.mniProventoCadastrar.Name = "mniProventoCadastrar";
            this.mniProventoCadastrar.Size = new System.Drawing.Size(225, 22);
            this.mniProventoCadastrar.Text = "&Cadastrar Proventos...";
            this.mniProventoCadastrar.Click += new System.EventHandler(this.mniProventoCadastrar_Click);
            // 
            // mniIFRCalcular
            // 
            this.mniIFRCalcular.Name = "mniIFRCalcular";
            this.mniIFRCalcular.Size = new System.Drawing.Size(225, 22);
            this.mniIFRCalcular.Text = "Calcular IFR...";
            this.mniIFRCalcular.Click += new System.EventHandler(this.mniIFRCalcular_Click);
            // 
            // mniIFRDiarioSimular
            // 
            this.mniIFRDiarioSimular.Name = "mniIFRDiarioSimular";
            this.mniIFRDiarioSimular.Size = new System.Drawing.Size(225, 22);
            this.mniIFRDiarioSimular.Text = "Simular IFR Diário...";
            this.mniIFRDiarioSimular.Click += new System.EventHandler(this.mniIFRDiarioSimular_Click);
            // 
            // mnuSequencialCalcular
            // 
            this.mnuSequencialCalcular.Name = "mnuSequencialCalcular";
            this.mnuSequencialCalcular.Size = new System.Drawing.Size(225, 22);
            this.mnuSequencialCalcular.Text = "Calcular &Sequenciais";
            this.mnuSequencialCalcular.Click += new System.EventHandler(this.mnuSequencialCalcular_Click);
            // 
            // AtualizarCotaçõesSemanaisToolStripMenuItem
            // 
            this.AtualizarCotaçõesSemanaisToolStripMenuItem.Name = "AtualizarCotaçõesSemanaisToolStripMenuItem";
            this.AtualizarCotaçõesSemanaisToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.AtualizarCotaçõesSemanaisToolStripMenuItem.Text = "Atualizar Cotações &Semanais";
            this.AtualizarCotaçõesSemanaisToolStripMenuItem.Visible = false;
            this.AtualizarCotaçõesSemanaisToolStripMenuItem.Click += new System.EventHandler(this.AtualizarCotaçõesSemanaisToolStripMenuItem_Click);
            // 
            // mnuOutrasOperacoes
            // 
            this.mnuOutrasOperacoes.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniBackTestUnitTest});
            this.mnuOutrasOperacoes.Name = "mnuOutrasOperacoes";
            this.mnuOutrasOperacoes.Size = new System.Drawing.Size(225, 22);
            this.mnuOutrasOperacoes.Text = "Outras Operações";
            // 
            // mniBackTestUnitTest
            // 
            this.mniBackTestUnitTest.Name = "mniBackTestUnitTest";
            this.mniBackTestUnitTest.Size = new System.Drawing.Size(180, 22);
            this.mniBackTestUnitTest.Text = "Recalcular Cotações";
            this.mniBackTestUnitTest.Click += new System.EventHandler(this.mniBackTestUnitTest_Click);
            // 
            // mnuitemCotacaoExcluir
            // 
            this.mnuitemCotacaoExcluir.Name = "mnuitemCotacaoExcluir";
            this.mnuitemCotacaoExcluir.Size = new System.Drawing.Size(225, 22);
            this.mnuitemCotacaoExcluir.Text = "Excluir Cotações...";
            this.mnuitemCotacaoExcluir.Click += new System.EventHandler(this.mnuitemCotacaoExcluir_Click);
            // 
            // WindowsMenu
            // 
            this.WindowsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewWindowToolStripMenuItem,
            this.CascadeToolStripMenuItem,
            this.TileVerticalToolStripMenuItem,
            this.TileHorizontalToolStripMenuItem,
            this.CloseAllToolStripMenuItem,
            this.ArrangeIconsToolStripMenuItem});
            this.WindowsMenu.Name = "WindowsMenu";
            this.WindowsMenu.Size = new System.Drawing.Size(56, 20);
            this.WindowsMenu.Text = "&Janelas";
            // 
            // NewWindowToolStripMenuItem
            // 
            this.NewWindowToolStripMenuItem.Name = "NewWindowToolStripMenuItem";
            this.NewWindowToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.NewWindowToolStripMenuItem.Text = "&New Window";
            this.NewWindowToolStripMenuItem.Visible = false;
            // 
            // CascadeToolStripMenuItem
            // 
            this.CascadeToolStripMenuItem.Name = "CascadeToolStripMenuItem";
            this.CascadeToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.CascadeToolStripMenuItem.Text = "&Cascata";
            this.CascadeToolStripMenuItem.Click += new System.EventHandler(this.CascadeToolStripMenuItem_Click);
            // 
            // TileVerticalToolStripMenuItem
            // 
            this.TileVerticalToolStripMenuItem.Name = "TileVerticalToolStripMenuItem";
            this.TileVerticalToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.TileVerticalToolStripMenuItem.Text = "Lado a Lado &Verticalmente";
            this.TileVerticalToolStripMenuItem.Click += new System.EventHandler(this.TileVerticleToolStripMenuItem_Click);
            // 
            // TileHorizontalToolStripMenuItem
            // 
            this.TileHorizontalToolStripMenuItem.Name = "TileHorizontalToolStripMenuItem";
            this.TileHorizontalToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.TileHorizontalToolStripMenuItem.Text = "Lado a Lado &Horizontalmente";
            this.TileHorizontalToolStripMenuItem.Click += new System.EventHandler(this.TileHorizontalToolStripMenuItem_Click);
            // 
            // CloseAllToolStripMenuItem
            // 
            this.CloseAllToolStripMenuItem.Name = "CloseAllToolStripMenuItem";
            this.CloseAllToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.CloseAllToolStripMenuItem.Text = "Fechar &Todas";
            this.CloseAllToolStripMenuItem.Click += new System.EventHandler(this.CloseAllToolStripMenuItem_Click);
            // 
            // ArrangeIconsToolStripMenuItem
            // 
            this.ArrangeIconsToolStripMenuItem.Name = "ArrangeIconsToolStripMenuItem";
            this.ArrangeIconsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.ArrangeIconsToolStripMenuItem.Text = "&Arrange Icons";
            this.ArrangeIconsToolStripMenuItem.Visible = false;
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ContentsToolStripMenuItem,
            this.IndexToolStripMenuItem,
            this.SearchToolStripMenuItem,
            this.ToolStripSeparator8,
            this.AboutToolStripMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(44, 20);
            this.HelpMenu.Text = "&Help";
            this.HelpMenu.Visible = false;
            // 
            // ContentsToolStripMenuItem
            // 
            this.ContentsToolStripMenuItem.Name = "ContentsToolStripMenuItem";
            this.ContentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.ContentsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.ContentsToolStripMenuItem.Text = "&Contents";
            // 
            // IndexToolStripMenuItem
            // 
            this.IndexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("IndexToolStripMenuItem.Image")));
            this.IndexToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.IndexToolStripMenuItem.Name = "IndexToolStripMenuItem";
            this.IndexToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.IndexToolStripMenuItem.Text = "&Index";
            // 
            // SearchToolStripMenuItem
            // 
            this.SearchToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("SearchToolStripMenuItem.Image")));
            this.SearchToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.SearchToolStripMenuItem.Name = "SearchToolStripMenuItem";
            this.SearchToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.SearchToolStripMenuItem.Text = "&Search";
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(165, 6);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.AboutToolStripMenuItem.Text = "&About ...";
            // 
            // ToolStrip
            // 
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripButton,
            this.OpenToolStripButton,
            this.SaveToolStripButton,
            this.ToolStripSeparator1,
            this.PrintToolStripButton,
            this.PrintPreviewToolStripButton,
            this.ToolStripSeparator2,
            this.HelpToolStripButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 24);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(632, 25);
            this.ToolStrip.TabIndex = 6;
            this.ToolStrip.Text = "ToolStrip";
            this.ToolStrip.Visible = false;
            // 
            // NewToolStripButton
            // 
            this.NewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("NewToolStripButton.Image")));
            this.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.NewToolStripButton.Name = "NewToolStripButton";
            this.NewToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.NewToolStripButton.Text = "New";
            // 
            // OpenToolStripButton
            // 
            this.OpenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.OpenToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenToolStripButton.Image")));
            this.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.OpenToolStripButton.Name = "OpenToolStripButton";
            this.OpenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.OpenToolStripButton.Text = "Open";
            // 
            // SaveToolStripButton
            // 
            this.SaveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SaveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveToolStripButton.Image")));
            this.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.SaveToolStripButton.Name = "SaveToolStripButton";
            this.SaveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.SaveToolStripButton.Text = "Save";
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // PrintToolStripButton
            // 
            this.PrintToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PrintToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PrintToolStripButton.Image")));
            this.PrintToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.PrintToolStripButton.Name = "PrintToolStripButton";
            this.PrintToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.PrintToolStripButton.Text = "Print";
            // 
            // PrintPreviewToolStripButton
            // 
            this.PrintPreviewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PrintPreviewToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("PrintPreviewToolStripButton.Image")));
            this.PrintPreviewToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.PrintPreviewToolStripButton.Name = "PrintPreviewToolStripButton";
            this.PrintPreviewToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.PrintPreviewToolStripButton.Text = "Print Preview";
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // HelpToolStripButton
            // 
            this.HelpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.HelpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("HelpToolStripButton.Image")));
            this.HelpToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
            this.HelpToolStripButton.Name = "HelpToolStripButton";
            this.HelpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.HelpToolStripButton.Text = "Help";
            // 
            // frmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "frmPrincipal";
            this.Text = "Trader Wizard";
            this.Load += new System.EventHandler(this.frmPrincipal_Load);
            this.Shown += new System.EventHandler(this.frmPrincipal_Shown);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.ToolStripMenuItem ContentsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem HelpMenu;
		private System.Windows.Forms.ToolStripMenuItem IndexToolStripMenuItem;
		private  System.Windows.Forms.ToolStripMenuItem SearchToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
		private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ArrangeIconsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CloseAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem NewWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem WindowsMenu;
		private System.Windows.Forms.ToolStripMenuItem CascadeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem TileVerticalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem TileHorizontalToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mniCotacaoOnline;
		private System.Windows.Forms.ToolStripButton HelpToolStripButton;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		private System.Windows.Forms.ToolStripButton PrintPreviewToolStripButton;
		private System.Windows.Forms.ToolTip ToolTip;
		private System.Windows.Forms.ToolStripButton PrintToolStripButton;
		private System.Windows.Forms.ToolStripButton NewToolStripButton;
		private System.Windows.Forms.ToolStrip ToolStrip;
		private System.Windows.Forms.ToolStripButton OpenToolStripButton;
		private System.Windows.Forms.ToolStripButton SaveToolStripButton;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem PrintPreviewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PrintToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem PrintSetupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem NewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem FileMenu;
		private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
		private System.Windows.Forms.MenuStrip MenuStrip;
		private System.Windows.Forms.ToolStripMenuItem EditMenu;
		private System.Windows.Forms.ToolStripMenuItem UndoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem RedoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem CutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem CopyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem PasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem SelectAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ViewMenu;
		private System.Windows.Forms.ToolStripMenuItem ToolBarToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem StatusBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
		private System.Windows.Forms.ToolStripMenuItem mniGrafico;
		private System.Windows.Forms.ToolStripMenuItem mniConfiguracoes;
		private System.Windows.Forms.ToolStripMenuItem OperaçõesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AtualizarCotaçõesHistóricasToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mniProventoAtualizar;
		private System.Windows.Forms.ToolStripMenuItem mnuSequencialCalcular;
		private System.Windows.Forms.ToolStripMenuItem AtualizarCotaçõesSemanaisToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mniDadosRecalcular;
		private System.Windows.Forms.ToolStripMenuItem mnuOutrasOperacoes;
		private System.Windows.Forms.ToolStripMenuItem mnuitemCotacaoExcluir;
		private System.Windows.Forms.ToolStripMenuItem mniBackTestUnitTest;
		private System.Windows.Forms.ToolStripMenuItem mniIFRDiarioSimular;
		private System.Windows.Forms.ToolStripMenuItem mniProventoCadastrar;
		private System.Windows.Forms.ToolStripMenuItem mniIFRCalcular;
        private ToolStripMenuItem toolStripMenuSetupDeEntrada;
    }
}
