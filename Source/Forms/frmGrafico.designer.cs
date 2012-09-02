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
	partial class frmGrafico : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGrafico));
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripPrincipal = new System.Windows.Forms.ToolStrip();
            this.ToolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.ToolStripcmbAtivo = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripbtnAtualizar = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripbtnEscalaLogaritmica = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnEscalaAritmetica = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripcmbPeriodoDuracao = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.TSmnuMMExibir = new System.Windows.Forms.ToolStripMenuItem();
            this.TSmnuMMConfigurar = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripbtnIFR14 = new System.Windows.Forms.ToolStripButton();
            this.ToolStripcmbIFRNumPeriodos = new System.Windows.Forms.ToolStripComboBox();
            this.ToolStripbtnVolume = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripbtnRetaHorizontal = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnLimparFerramentas = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnCanal = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnLinhaTendencia = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnFibonacciRetracement = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripbtnZoomAumentar = new System.Windows.Forms.ToolStripButton();
            this.ToolStripbtnZoomDiminuir = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip.SuspendLayout();
            this.ToolStripPrincipal.SuspendLayout();
            this.SuspendLayout();
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1});
            this.StatusStrip.Location = new System.Drawing.Point(0, 521);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(839, 22);
            this.StatusStrip.TabIndex = 8;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(121, 17);
            this.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1";
            // 
            // ToolStripStatusLabel
            // 
            this.ToolStripStatusLabel.Name = "ToolStripStatusLabel";
            this.ToolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.ToolStripStatusLabel.Text = "Status";
            // 
            // ToolStripPrincipal
            // 
            this.ToolStripPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripLabel1,
            this.ToolStripcmbAtivo,
            this.ToolStripbtnAtualizar,
            this.ToolStripSeparator1,
            this.ToolStripbtnEscalaLogaritmica,
            this.ToolStripbtnEscalaAritmetica,
            this.ToolStripSeparator2,
            this.ToolStripcmbPeriodoDuracao,
            this.ToolStripSeparator3,
            this.ToolStripDropDownButton1,
            this.ToolStripbtnIFR14,
            this.ToolStripcmbIFRNumPeriodos,
            this.ToolStripbtnVolume,
            this.ToolStripSeparator4,
            this.ToolStripbtnRetaHorizontal,
            this.ToolStripbtnLimparFerramentas,
            this.ToolStripbtnCanal,
            this.ToolStripbtnLinhaTendencia,
            this.ToolStripbtnFibonacciRetracement,
            this.ToolStripSeparator5,
            this.ToolStripbtnZoomAumentar,
            this.ToolStripbtnZoomDiminuir});
            this.ToolStripPrincipal.Location = new System.Drawing.Point(0, 0);
            this.ToolStripPrincipal.Name = "ToolStripPrincipal";
            this.ToolStripPrincipal.Size = new System.Drawing.Size(839, 30);
            this.ToolStripPrincipal.TabIndex = 9;
            this.ToolStripPrincipal.Text = "ToolStrip1";
            // 
            // ToolStripLabel1
            // 
            this.ToolStripLabel1.Name = "ToolStripLabel1";
            this.ToolStripLabel1.Size = new System.Drawing.Size(38, 27);
            this.ToolStripLabel1.Text = "Ativo:";
            // 
            // ToolStripcmbAtivo
            // 
            this.ToolStripcmbAtivo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ToolStripcmbAtivo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ToolStripcmbAtivo.Name = "ToolStripcmbAtivo";
            this.ToolStripcmbAtivo.Size = new System.Drawing.Size(180, 30);
            // 
            // ToolStripbtnAtualizar
            // 
            this.ToolStripbtnAtualizar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnAtualizar.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnAtualizar.Image")));
            this.ToolStripbtnAtualizar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnAtualizar.Name = "ToolStripbtnAtualizar";
            this.ToolStripbtnAtualizar.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnAtualizar.Text = "ToolStripButton3";
            this.ToolStripbtnAtualizar.ToolTipText = "Atualizar";
            this.ToolStripbtnAtualizar.Click += new System.EventHandler(this.ToolStripbtnAtualizar_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // ToolStripbtnEscalaLogaritmica
            // 
            this.ToolStripbtnEscalaLogaritmica.Checked = true;
            this.ToolStripbtnEscalaLogaritmica.CheckOnClick = true;
            this.ToolStripbtnEscalaLogaritmica.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ToolStripbtnEscalaLogaritmica.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnEscalaLogaritmica.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnEscalaLogaritmica.Image")));
            this.ToolStripbtnEscalaLogaritmica.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnEscalaLogaritmica.Name = "ToolStripbtnEscalaLogaritmica";
            this.ToolStripbtnEscalaLogaritmica.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnEscalaLogaritmica.Text = "L";
            this.ToolStripbtnEscalaLogaritmica.ToolTipText = "Escala Logarítmica";
            this.ToolStripbtnEscalaLogaritmica.Click += new System.EventHandler(this.ToolStripbtnEscalaLogaritmica_Click);
            // 
            // ToolStripbtnEscalaAritmetica
            // 
            this.ToolStripbtnEscalaAritmetica.CheckOnClick = true;
            this.ToolStripbtnEscalaAritmetica.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnEscalaAritmetica.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnEscalaAritmetica.Image")));
            this.ToolStripbtnEscalaAritmetica.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnEscalaAritmetica.Name = "ToolStripbtnEscalaAritmetica";
            this.ToolStripbtnEscalaAritmetica.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnEscalaAritmetica.Text = "A";
            this.ToolStripbtnEscalaAritmetica.ToolTipText = "Escala Aritmética";
            this.ToolStripbtnEscalaAritmetica.Click += new System.EventHandler(this.ToolStripbtnEscalaAritmetica_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 30);
            // 
            // ToolStripcmbPeriodoDuracao
            // 
            this.ToolStripcmbPeriodoDuracao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripcmbPeriodoDuracao.Name = "ToolStripcmbPeriodoDuracao";
            this.ToolStripcmbPeriodoDuracao.Size = new System.Drawing.Size(121, 30);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 30);
            // 
            // ToolStripDropDownButton1
            // 
            this.ToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSmnuMMExibir,
            this.TSmnuMMConfigurar});
            this.ToolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripDropDownButton1.Image")));
            this.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1";
            this.ToolStripDropDownButton1.Size = new System.Drawing.Size(42, 27);
            this.ToolStripDropDownButton1.Text = "MM";
            this.ToolStripDropDownButton1.ToolTipText = "Média Móvel Exponencial ou Aritmética";
            // 
            // TSmnuMMExibir
            // 
            this.TSmnuMMExibir.CheckOnClick = true;
            this.TSmnuMMExibir.Name = "TSmnuMMExibir";
            this.TSmnuMMExibir.Size = new System.Drawing.Size(140, 22);
            this.TSmnuMMExibir.Text = "Exibir";
            // 
            // TSmnuMMConfigurar
            // 
            this.TSmnuMMConfigurar.Name = "TSmnuMMConfigurar";
            this.TSmnuMMConfigurar.Size = new System.Drawing.Size(140, 22);
            this.TSmnuMMConfigurar.Text = "Configurar...";
            this.TSmnuMMConfigurar.Click += new System.EventHandler(this.TSmnuMMConfigurar_Click);
            // 
            // ToolStripbtnIFR14
            // 
            this.ToolStripbtnIFR14.CheckOnClick = true;
            this.ToolStripbtnIFR14.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnIFR14.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnIFR14.Image")));
            this.ToolStripbtnIFR14.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnIFR14.Name = "ToolStripbtnIFR14";
            this.ToolStripbtnIFR14.Size = new System.Drawing.Size(27, 27);
            this.ToolStripbtnIFR14.Text = "IFR";
            this.ToolStripbtnIFR14.ToolTipText = "Indice de Força Relativa - 14 dias";
            this.ToolStripbtnIFR14.Click += new System.EventHandler(this.ToolStripbtnIFR14_Click);
            // 
            // ToolStripcmbIFRNumPeriodos
            // 
            this.ToolStripcmbIFRNumPeriodos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ToolStripcmbIFRNumPeriodos.Enabled = false;
            this.ToolStripcmbIFRNumPeriodos.Items.AddRange(new object[] {
            "2",
            "14"});
            this.ToolStripcmbIFRNumPeriodos.Name = "ToolStripcmbIFRNumPeriodos";
            this.ToolStripcmbIFRNumPeriodos.Size = new System.Drawing.Size(75, 30);
            this.ToolStripcmbIFRNumPeriodos.Visible = false;
            // 
            // ToolStripbtnVolume
            // 
            this.ToolStripbtnVolume.CheckOnClick = true;
            this.ToolStripbtnVolume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnVolume.DoubleClickEnabled = true;
            this.ToolStripbtnVolume.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnVolume.Image")));
            this.ToolStripbtnVolume.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnVolume.Name = "ToolStripbtnVolume";
            this.ToolStripbtnVolume.Size = new System.Drawing.Size(33, 27);
            this.ToolStripbtnVolume.Text = "VOL";
            this.ToolStripbtnVolume.ToolTipText = "Volume";
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 30);
            // 
            // ToolStripbtnRetaHorizontal
            // 
            this.ToolStripbtnRetaHorizontal.CheckOnClick = true;
            this.ToolStripbtnRetaHorizontal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnRetaHorizontal.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnRetaHorizontal.Image")));
            this.ToolStripbtnRetaHorizontal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnRetaHorizontal.Name = "ToolStripbtnRetaHorizontal";
            this.ToolStripbtnRetaHorizontal.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnRetaHorizontal.Text = "ToolStripButton1";
            this.ToolStripbtnRetaHorizontal.ToolTipText = "Desenha Linha Horizontal";
            this.ToolStripbtnRetaHorizontal.Click += new System.EventHandler(this.ToolStripbtnRetaHorizontal_Click);
            // 
            // ToolStripbtnLimparFerramentas
            // 
            this.ToolStripbtnLimparFerramentas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnLimparFerramentas.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStripbtnLimparFerramentas.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnLimparFerramentas.Image")));
            this.ToolStripbtnLimparFerramentas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnLimparFerramentas.Name = "ToolStripbtnLimparFerramentas";
            this.ToolStripbtnLimparFerramentas.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnLimparFerramentas.Text = "L F";
            this.ToolStripbtnLimparFerramentas.ToolTipText = "Limpar Ferramentas";
            this.ToolStripbtnLimparFerramentas.Click += new System.EventHandler(this.ToolStripbtnLimparFerramentas_Click);
            // 
            // ToolStripbtnCanal
            // 
            this.ToolStripbtnCanal.CheckOnClick = true;
            this.ToolStripbtnCanal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnCanal.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnCanal.Image")));
            this.ToolStripbtnCanal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnCanal.Name = "ToolStripbtnCanal";
            this.ToolStripbtnCanal.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnCanal.Text = "ToolStripButton1";
            this.ToolStripbtnCanal.ToolTipText = "Desenhar Canal";
            this.ToolStripbtnCanal.Click += new System.EventHandler(this.ToolStripbtnCanal_Click);
            // 
            // ToolStripbtnLinhaTendencia
            // 
            this.ToolStripbtnLinhaTendencia.CheckOnClick = true;
            this.ToolStripbtnLinhaTendencia.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnLinhaTendencia.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnLinhaTendencia.Image")));
            this.ToolStripbtnLinhaTendencia.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnLinhaTendencia.Name = "ToolStripbtnLinhaTendencia";
            this.ToolStripbtnLinhaTendencia.Size = new System.Drawing.Size(24, 27);
            this.ToolStripbtnLinhaTendencia.Text = "LT";
            this.ToolStripbtnLinhaTendencia.ToolTipText = "Desenhar Linha";
            this.ToolStripbtnLinhaTendencia.Click += new System.EventHandler(this.ToolStripbtnLinhaTendencia_Click);
            // 
            // ToolStripbtnFibonacciRetracement
            // 
            this.ToolStripbtnFibonacciRetracement.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.ToolStripbtnFibonacciRetracement.CheckOnClick = true;
            this.ToolStripbtnFibonacciRetracement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ToolStripbtnFibonacciRetracement.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStripbtnFibonacciRetracement.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnFibonacciRetracement.Image")));
            this.ToolStripbtnFibonacciRetracement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnFibonacciRetracement.Name = "ToolStripbtnFibonacciRetracement";
            this.ToolStripbtnFibonacciRetracement.Size = new System.Drawing.Size(24, 27);
            this.ToolStripbtnFibonacciRetracement.Text = "F";
            this.ToolStripbtnFibonacciRetracement.ToolTipText = "Desenhar Fibonacci";
            this.ToolStripbtnFibonacciRetracement.Click += new System.EventHandler(this.ToolStripbtnFibonacciRetracement_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(6, 30);
            // 
            // ToolStripbtnZoomAumentar
            // 
            this.ToolStripbtnZoomAumentar.CheckOnClick = true;
            this.ToolStripbtnZoomAumentar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnZoomAumentar.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnZoomAumentar.Image")));
            this.ToolStripbtnZoomAumentar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnZoomAumentar.Name = "ToolStripbtnZoomAumentar";
            this.ToolStripbtnZoomAumentar.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnZoomAumentar.Text = "ToolStripDropDownButton1";
            this.ToolStripbtnZoomAumentar.ToolTipText = "Aumentar Zoom";
            this.ToolStripbtnZoomAumentar.Click += new System.EventHandler(this.ToolStripbtnZoomAumentar_Click);
            // 
            // ToolStripbtnZoomDiminuir
            // 
            this.ToolStripbtnZoomDiminuir.CheckOnClick = true;
            this.ToolStripbtnZoomDiminuir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripbtnZoomDiminuir.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripbtnZoomDiminuir.Image")));
            this.ToolStripbtnZoomDiminuir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripbtnZoomDiminuir.Name = "ToolStripbtnZoomDiminuir";
            this.ToolStripbtnZoomDiminuir.Size = new System.Drawing.Size(23, 27);
            this.ToolStripbtnZoomDiminuir.Text = "ToolStripButton1";
            this.ToolStripbtnZoomDiminuir.ToolTipText = "Diminuir Zoom";
            this.ToolStripbtnZoomDiminuir.Click += new System.EventHandler(this.ToolStripbtnZoomDiminuir_Click);
            // 
            // frmGrafico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 543);
            this.Controls.Add(this.ToolStripPrincipal);
            this.Controls.Add(this.StatusStrip);
            this.DoubleBuffered = true;
            this.Name = "frmGrafico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Grafico";
            this.Load += new System.EventHandler(this.frmGrafico_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmGrafico_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmGrafico_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmGrafico_MouseDown);
            this.MouseLeave += new System.EventHandler(this.frmGrafico_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmGrafico_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmGrafico_MouseUp);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ToolStripPrincipal.ResumeLayout(false);
            this.ToolStripPrincipal.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		private System.Windows.Forms.StatusStrip StatusStrip;
		private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel;
		private System.Windows.Forms.ToolStrip ToolStripPrincipal;
		private System.Windows.Forms.ToolStripLabel ToolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox ToolStripcmbAtivo;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
		private System.Windows.Forms.ToolStripButton ToolStripbtnEscalaLogaritmica;
		private System.Windows.Forms.ToolStripButton ToolStripbtnEscalaAritmetica;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
		private System.Windows.Forms.ToolStripComboBox ToolStripcmbPeriodoDuracao;
		private System.Windows.Forms.ToolStripButton ToolStripbtnAtualizar;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
		private System.Windows.Forms.ToolStripButton ToolStripbtnVolume;
		private System.Windows.Forms.ToolStripButton ToolStripbtnIFR14;
		private System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
		private System.Windows.Forms.ToolStripButton ToolStripbtnRetaHorizontal;
		private System.Windows.Forms.ToolStripButton ToolStripbtnCanal;
		private System.Windows.Forms.ToolStripButton ToolStripbtnFibonacciRetracement;
		private System.Windows.Forms.ToolStripButton ToolStripbtnLimparFerramentas;
		private System.Windows.Forms.ToolStripButton ToolStripbtnLinhaTendencia;
		private System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
		private System.Windows.Forms.ToolStripButton ToolStripbtnZoomDiminuir;
		private System.Windows.Forms.ToolStripButton ToolStripbtnZoomAumentar;
		private System.Windows.Forms.ToolStripComboBox ToolStripcmbIFRNumPeriodos;
		private System.Windows.Forms.ToolStripDropDownButton ToolStripDropDownButton1;
		private System.Windows.Forms.ToolStripMenuItem TSmnuMMConfigurar;
		private System.Windows.Forms.ToolStripMenuItem TSmnuMMExibir;
	}
}
