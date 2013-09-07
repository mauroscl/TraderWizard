using System;
using System.Windows.Forms;
using DataBase;
using Forms.Properties;
using prmCotacao;

namespace TraderWizard
{
    public partial class frmPrincipal
    {
        public static string strBancoTipo;
        private int m_ChildFormNumber;
        private cConexao objConexao;

        public frmPrincipal()
        {
            Load += frmPrincipal_Load;
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.


            //Dim objCommand As cCommand = New cCommand(objConexao)


            //********INÍCIO DO CÓDIGO COMENTADO POR MAURO, 12/05/2010
            //Não está mais utilizando a tabela Controle.

            //EXCLUI TODOS OS REGISTROS DA TABELA CONTROLE
            //objCommand.Execute( _
            //"DELETE " _
            //& "FROM CONTROLE")

            //********FIM DO CÓDIGO COMENTADO POR MAURO, 12/05/2010
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var ChildForm = new Form();
            // Make it a child of this MDI form before showing it.
            ChildForm.MdiParent = this;

            m_ChildFormNumber += 1;
            ChildForm.Text = "Window " + m_ChildFormNumber;

            ChildForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            var OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            OpenFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if ((OpenFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                string FileName = OpenFileDialog.FileName;
                // TODO: Add code here to open the file.
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            SaveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if ((SaveFileDialog.ShowDialog(this) == DialogResult.OK))
            {
                string FileName = SaveFileDialog.FileName;
                // TODO: Add code here to save the current contents of the form to a file.
            }
        }


        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Use My.Computer.Clipboard to insert the selected text or images into the clipboard
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Use My.Computer.Clipboard to insert the selected text or images into the clipboard
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Use My.Computer.Clipboard.GetText() or My.Computer.Clipboard.GetData to retrieve information from the clipboard.
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStrip.Visible = ToolBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Close all child forms of the parent.
            foreach (Form ChildForm in MdiChildren)
            {
                ChildForm.Close();
            }
        }


        private void frmPrincipal_Load(Object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }


        private void mniCotacaoOnline_Click(Object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var frmChildForm = new frmCotacao();
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniGrafico_Click(Object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var frmChildForm = new frmGrafico(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniConfiguracoes_Click(Object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var frmChildForm = new frmConfiguracao();
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void CalcularMédiaMóvelExponencialToolStripMenuItem_Click(Object sender, EventArgs e)
        {
        }


        private void AtualizarCotaçõesHistóricasToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            //Me.Cursor = Cursors.WaitCursor

            // Create a new instance of the child form.
            var frmChildForm = new frmCotacaoAtualizarPeriodo(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();

            //Me.Cursor = Cursors.Default
        }


        private void AtualizarCotaçõesSemanaisToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            //Dim blnRetorno As Boolean

            //If MsgBox("Atualizar Cotações semanais para todos os ativos. Confirma a execução da operação?" _
            //, MsgBoxStyle.YesNoCancel, Me.Text) = MsgBoxResult.Yes Then

            //    Dim objCotacao As cCotacao = New cCotacao(objConexao)

            //    blnRetorno = objCotacao.CotacaoSemanalRetroativoGeralCalcular()

            //    If blnRetorno Then
            //        MsgBox("Operação executada com sucesso.", MsgBoxStyle.Information, Me.Text)
            //    Else
            //        MsgBox("Ocorreram erros ao executar a operação. Veja o log de erros.", MsgBoxStyle.Exclamation, Me.Text)
            //    End If

            //End If

            //Dim intRetorno As enumRetorno

            //Dim objCotacao As cCotacao = New cCotacao(objConexao)

            //blnRetorno = objCotacao.CotacaoSemanalDadosAtualizar()

            //If blnRetorno Then

            //    MsgBox("Operação realizada com sucesso.", MsgBoxStyle.Information, Me.Text)

            //End If
        }


        private void mniDadosRecalcular_Click(Object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var frmChildForm = new frmDadosRecalcular(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }

        private void mnuitemCotacaoExcluir_Click(Object sender, EventArgs e)
        {
            // Create a new instance of the child form.
            var frmChildForm = new frmCotacaoExcluir(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mnuItmRelatGeracaoVisualizacao_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmRelatVisualizacao(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniBackTestUnitTest_Click(Object sender, EventArgs e)
        {
            var objCotacao = new ServicoDeCotacao(objConexao);

            if (
                MessageBox.Show("Confirma o recálculo das cotações?", Text, MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                objCotacao.RecalcularIndicadores();
            }
        }


        private void mnuitmRelatSpool_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmRelatorioSpool(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mnuItmRelatBackTest_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmRelatBackTest(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniProventoAtualizar_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmProventoAtualizar();
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mnuSequencialCalcular_Click(Object sender, EventArgs e)
        {
            if (MessageBox.Show("Recalcular os Sequenciais dos ativos em que o valor do sequencial máximo for diferente do número de cotações?"
                , Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            
            var objCotacao = new ServicoDeCotacao(objConexao);

            Cursor = Cursors.WaitCursor;

            bool blnRetorno = objCotacao.SequencialPreencher();

            Cursor = Cursors.Default;


            if (blnRetorno)
            {
                MessageBox.Show("Operação realizada com sucesso.", Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ocorreu algum erro ao executar a operação.", Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }


        private void mniIFRDiarioSimular_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmIFRSimulacaoDiaria(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniProventoCadastrar_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmProventoCadastrar(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }


        private void mniIFRCalcular_Click(Object sender, EventArgs e)
        {
            var frmChildForm = new frmIFRCalcular(objConexao);
            // Make it a child of this MDI form before showing it.
            frmChildForm.MdiParent = this;

            m_ChildFormNumber += 1;

            frmChildForm.Show();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmPrincipal_Shown(object sender, EventArgs e)
        {
            try
            {
                objConexao = new cConexao();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.ApplicationMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
    }
}