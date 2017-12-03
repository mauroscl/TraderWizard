using System;
using System.Windows.Forms;
using DataBase;
using Forms;
using Forms.Properties;
using ServicoNegocio;
using TraderWizard.ServicosDeAplicacao;

namespace TraderWizard
{
    public partial class frmPrincipal
    {
        private int m_ChildFormNumber;
        private Conexao objConexao;

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
            var frmChildForm = new frmConfiguracao {MdiParent = this};
            // Make it a child of this MDI form before showing it.

            m_ChildFormNumber += 1;

            frmChildForm.Show();
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
            
            Cursor = Cursors.WaitCursor;

            var sequencialService = new SequencialService();

            bool blnRetorno = sequencialService.SequencialPreencher();

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
            var frmChildForm = new frmIFRCalcular();
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
                objConexao = new Conexao();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.ApplicationMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void toolStripMenuSetupDeEntrada_Click(object sender, EventArgs e)
        {
            var frmChildForm = new frmRelatVisualizacao(objConexao) {MdiParent = this};
            // Make it a child of this MDI form before showing it.

            m_ChildFormNumber += 1;

            frmChildForm.Show();

        }

        private void calcularVolatilidadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var codigos = new[]
            //{
            //    "AALR3",
            //    "ABCB4",
            //    "ABEV3",
            //    "AGRO3",
            //    "ALSC3",
            //    "ALUP11",
            //    "AMAR3",
            //    "ANIM3",
            //    "ARZZ3",
            //    "AZUL4",
            //    "BBAS3",
            //    "BBDC3",
            //    "BBDC4",
            //    "BBRK3",
            //    "BBSE3",
            //    "BEEF3",
            //    "BOVA11",
            //    "BPAC11",
            //    "BPAN4",
            //    "BPHA3",
            //    "BRAP3",
            //    "BRAP4",
            //    "BRFS3",
            //    "BRKM5",
            //    "BRML3",
            //    "BRPR3",
            //    "BRSR6",
            //    "BSEV3",
            //    "BTOW3",
            //    "BVMF3",
            //    "CARD3",
            //    "CCRO3",
            //    "CESP3",
            //    "CESP6",
            //    "CGAS5",
            //    "CIEL3",
            //    "CMIG3",
            //    "CMIG4",
            //    "CPFE3",
            //    "CPLE3",
            //    "CPLE6",
            //    "CRFB3",
            //    "CSAN3",
            //    "CSMG3",
            //    "CSNA3",
            //    "CVCB3",
            //    "CYRE3",
            //    "CZLT33",
            //    "DAGB33",
            //    "DIRR3",
            //    "DTEX3",
            //    "ECOR3",
            //    "EGIE3",
            //    "ELET3",
            //    "ELET6",
            //    "ELPL4",
            //    "EMBR3",
            //    "ENBR3",
            //    "ENGI11",
            //    "EQTL3",
            //    "ESTC3",
            //    "ETER3",
            //    "EVEN3",
            //    "EZTC3",
            //    "FESA4",
            //    "FHER3",
            //    "FIBR3",
            //    "FJTA4",
            //    "FLRY3",
            //    "GBIO33",
            //    "GFSA3",
            //    "GGBR3",
            //    "GGBR4",
            //    "GOAU3",
            //    "GOAU4",
            //    "GOLL4",
            //    "GRND3",
            //    "GUAR3",
            //    "HBOR3",
            //    "HGTX3",
            //    "HYPE3",
            //    "IGTA3",
            //    "IRBR3",
            //    "ITSA3",
            //    "ITSA4",
            //    "ITUB3",
            //    "ITUB4",
            //    "JBSS3",
            //    "JFEN3",
            //    "JHSF3",
            //    "JSLG3",
            //    "KLBN11",
            //    "KLBN4",
            //    "KROT3",
            //    "LAME3",
            //    "LAME4",
            //    "LIGT3",
            //    "LINX3",
            //    "LPSB3",
            //    "LREN3",
            //    "MAGG3",
            //    "MDIA3",
            //    "MEAL3",
            //    "MGLU3",
            //    "MILS3",
            //    "MOVI3",
            //    "MPLU3",
            //    "MRFG3",
            //    "MRVE3",
            //    "MULT3",
            //    "MYPK3",
            //    "NATU3",
            //    "ODPV3",
            //    "OGSA3",
            //    "OIBR3",
            //    "OIBR4",
            //    "OMGE3",
            //    "PARD3",
            //    "PCAR4",
            //    "PDGR3",
            //    "PETR3",
            //    "PFRM3",
            //    "PIBB11",
            //    "PMAM3",
            //    "POMO3",
            //    "POMO4",
            //    "POSI3",
            //    "PRIO3",
            //    "PRML3",
            //    "PSSA3",
            //    "PTBL3",
            //    "QGEP3",
            //    "QUAL3",
            //    "RADL3",
            //    "RAIL3",
            //    "RAPT4",
            //    "RCSL4",
            //    "RENT3",
            //    "RLOG3",
            //    "RNEW11",
            //    "RNEW3",
            //    "RNEW4",
            //    "ROMI3",
            //    "RSID3",
            //    "SANB11",
            //    "SAPR4",
            //    "SBSP3",
            //    "SEER3",
            //    "SGPS3",
            //    "SHOW3",
            //    "SHUL4",
            //    "SLCE3",
            //    "SLED4",
            //    "SMLE3",
            //    "SMTO3",
            //    "SSBR3",
            //    "STBP3",
            //    "SULA11",
            //    "SUZB5",
            //    "TAEE11",
            //    "TCSA3",
            //    "TECN3",
            //    "TGMA3",
            //    "TIET11",
            //    "TIET3",
            //    "TIET4",
            //    "TIMP3",
            //    "TOTS3",
            //    "TPIS3",
            //    "TRPL4",
            //    "TUPY3",
            //    "UCAS3",
            //    "UGPA3",
            //    "UNIP6",
            //    "USIM5",
            //    "VALE3",
            //    "VALE5",
            //    "VIVT4",
            //    "VLID3",
            //    "VVAR11",
            //    "VVAR3",
            //    "VVAR4",
            //    "WEGE3",
            //    "WIZS3",
            //};

            var codigos = new[] {"PETR4"};
            var calculadorDeVolatilidade = new CalculadorDeVolatilidade();
            try
            {
                foreach (var codigo in codigos)
                {
                    calculadorDeVolatilidade.CalcularVolatilidadeDiaria(codigo);
                    calculadorDeVolatilidade.CalcularMediaVolatilidadeDiaria(codigo);
                    calculadorDeVolatilidade.CalcularVolatilidadeSemanal(codigo);
                    calculadorDeVolatilidade.CalcularMediaVolatilidadeSemanal(codigo);
                    Console.WriteLine($"Volatilidade calculada com sucesso: {codigo}");
                }
                MessageBox.Show("Volatilidade calculada com sucesso.", Resources.ApplicationMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.ApplicationMessageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}