using System;
using System.Drawing;
using System.Windows.Forms;
using DataBase;
using Forms;
using pWeb;

namespace TraderWizard
{
    public partial class frmConfiguracao
    {
        private readonly Conexao objConexao;

        public frmConfiguracao()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            objConexao = new Conexao();

            DadosCarregar();
        }


        private void DadosSalvar()
        {
            var objComando = new cCommand(objConexao);

            //sse não tem nenhum erro salva os parâmetros no banco de dados.
            string strProxyType = String.Empty;
            string strProxyManualHTTP = String.Empty;
            string strProxyManualPorta = String.Empty;
            //Dim strProxyConfigAutEndereco As String = vbNullString
            string strProxyCredencialUtilizar = String.Empty;

            if (rdbSemProxy.Checked)
            {
                strProxyType = "SP";
            }
            else if (rdbProxyAutomatico.Checked)
            {
                strProxyType = "PA";
            }
            else if (rdbProxyManual.Checked)
            {
                strProxyType = "PM";
                strProxyManualHTTP = txtProxyManualHTTP.Text.Trim();
                strProxyManualPorta = txtProxyManualPorta.Text.Trim();
            }

            strProxyCredencialUtilizar = chkCredencialUtilizar.Checked ? "SIM" : "NAO";

            objComando.BeginTrans();

            ParametroSalvar("ProxyTipo", strProxyType);

            if (strProxyType == "PM")
            {
                ParametroSalvar("ProxyManualHTTP", strProxyManualHTTP);
                ParametroSalvar("ProxyManualPorta", strProxyManualPorta);
            }

            //If strProxyType = "PE" Then
            //    ParametroSalvar("ProxyConfigAutEndereco", strProxyConfigAutEndereco)
            //End If

            ParametroSalvar("ProxyCredencialUtilizar", strProxyCredencialUtilizar);


            if (strProxyCredencialUtilizar == "SIM")
            {
                ParametroSalvar("ProxyCredencialDominio", txtDominio.Text.Trim());

                ParametroSalvar("ProxyCredencialUsuario", txtUsuario.Text.Trim());

                cCriptografia objCriptografia = new cCriptografia();

                ParametroSalvar("ProxyCredencialSenha", objCriptografia.Criptografar(txtSenha.Text.Trim()));
                //ParametroSalvar("ProxyCredencialSenha", Trim(txtSenha.Text))
            }

            //***********ATIVOPADRAO***********
            //string strValor = Strings.Mid(cmbAtivo.Text, 1, Strings.InStr(cmbAtivo.Text, "-") - 2);
            string strValor = mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo(cmbAtivo.Text);
            ParametroSalvar("AtivoPadrao", strValor);
            //***********ATIVOPADRAO***********

            //***********MMEXPDESENHAR***********
            strValor = (chkMMExpDesenhar.Checked ? "SIM" : "NAO");

            ParametroSalvar("MMExpDesenhar", strValor);

            IndicadorSalvar();
            //***********MMEXPDESENHAR***********

            //***********IFRDESENHAR***********
            strValor = (chkIFRDesenhar.Checked ? "SIM" : "NAO");

            ParametroSalvar("IFRDesenhar", strValor);
            //***********IFRDESENHAR***********

            //***********VOLUMEDESENHAR***********
            strValor = (chkVolumeDesenhar.Checked ? "SIM" : "NAO");

            ParametroSalvar("VolumeDesenhar", strValor);
            //***********VOLUMEDESENHAR***********

            strValor = txtCotacaoAtivos.Text.Trim();

            ParametroSalvar("CotacaoAtivos", strValor);

            strValor = txtValorCapital.Text.Trim();

            ParametroSalvar("ValorCapital", strValor);

            strValor = txtPercentualManejo.Text.Trim();

            ParametroSalvar("PercentualManejo", strValor);

            ParametroSalvar("HoraAberturaPregao", txtHoraAberturaPregao.Text.Trim());

            ParametroSalvar("HoraFechamentoPregao", txtHoraFechamentoPregao.Text.Trim());

            objComando.CommitTrans();


            if (objComando.TransStatus)
            {
                Close();
            }
            else
            {
                MessageBox.Show("Ocorreram erros ao salvar as configurações.", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);

            }
        }


        private void DadosCarregar()
        {
            var objWebConfiguracao = new cWebConfiguracao(true, objConexao);

            string strValor = objWebConfiguracao.ProxyTipo;


            if (strValor == String.Empty)
            {
                //se o parâmetro não está cadastrado o padrão é sem PROXY.
                strValor = "SP";
            }

            switch (strValor)
            {
                case "SP":

                    rdbSemProxy.Checked = true;

                    break;
                case "PM":

                    rdbProxyManual.Checked = true;

                    txtProxyManualHTTP.Text = objWebConfiguracao.ProxyManualHTTP;

                    txtProxyManualPorta.Text = objWebConfiguracao.ProxyManualPorta.ToString();

                    break;
                case "PA":

                    rdbProxyAutomatico.Checked = true;

                    break;

            }


            if (objWebConfiguracao.CredencialUtilizar)
            {
                chkCredencialUtilizar.Checked = true;

                txtDominio.Text = objWebConfiguracao.Dominio;

                txtUsuario.Text = objWebConfiguracao.Usuario;

                txtSenha.Text = objWebConfiguracao.Senha;
            }

            //consulta o ativo padrão
            mdlGeral.ParametroConsultar(objConexao, "AtivoPadrao", out strValor);

            //chama função para preencher o combo
            cmbAtivoPreencher(strValor);

            mdlGeral.ParametroConsultar(objConexao, "MMExpDesenhar", out strValor);


            if (strValor == "SIM")
            {
                chkMMExpDesenhar.Checked = true;

                IndicadorCarregar();
            }

            mdlGeral.ParametroConsultar(objConexao, "IFRDesenhar", out strValor);


            if (strValor == "SIM")
            {
                chkIFRDesenhar.Checked = true;
            }

            mdlGeral.ParametroConsultar(objConexao, "VolumeDesenhar", out strValor);


            if (strValor == "SIM")
            {
                chkVolumeDesenhar.Checked = true;
            }

            mdlGeral.ParametroConsultar(objConexao, "CotacaoAtivos", out strValor);

            txtCotacaoAtivos.Text = strValor;

            mdlGeral.ParametroConsultar(objConexao, "ValorCapital", out strValor);

            txtValorCapital.Text = strValor;

            mdlGeral.ParametroConsultar(objConexao, "PercentualManejo", out strValor);

            txtPercentualManejo.Text = strValor;

            mdlGeral.ParametroConsultar(objConexao, "HoraAberturaPregao", out strValor);
            txtHoraAberturaPregao.Text = strValor;

            mdlGeral.ParametroConsultar(objConexao, "HoraFechamentoPregao", out strValor);
            txtHoraFechamentoPregao.Text = strValor;
        }

        private void rdbProxyManual_CheckedChanged(Object sender, EventArgs e)
        {
            pnlProxyManual.Enabled = rdbProxyManual.Checked;
        }

        private void rdbProxyEnderecoAutomatico_CheckedChanged(Object sender, EventArgs e)
        {
            pnlProxyAutEndereco.Enabled = rdbProxyEnderecoAutomatico.Checked;
        }

        private void chkCredencialUtilizar_CheckedChanged(Object sender, EventArgs e)
        {
            pnlCredencial.Enabled = chkCredencialUtilizar.Checked;
        }

        private void chkMMExpDesenhar_CheckedChanged(Object sender, EventArgs e)
        {
            pnlMMExp.Enabled = chkMMExpDesenhar.Checked;
        }

        private bool Consistir()
        {
            int periodo;
            if (!int.TryParse(txtPeriodo.Text, out periodo))
            {
                MessageBox.Show("Campo " + Convert.ToChar(34) + "Período" + Convert.ToChar(34) + " não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }


            //verifica se o período já foi inserido
            string strNovoPeriodo = txtPeriodo.Text.Trim();


            for (int intI = 0; intI <= lstPeriodoSelecionado.Items.Count - 1; intI++)
            {
                if (strNovoPeriodo == lstPeriodoSelecionado.Items[intI].Text.Trim())
                {

                    MessageBox.Show("Período " + Convert.ToChar(34) + strNovoPeriodo + Convert.ToChar(34) + " já foi inserido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return false;
                }
            }

            return true;
        }


        private void btnAdicionar_Click(Object sender, EventArgs e)
        {
            if (Consistir())
            {
                ListViewItem objListViewItem = null;

                objListViewItem = lstPeriodoSelecionado.Items.Add(txtPeriodo.Text);

                //seta esta propriedade para false para permitir que apenas uma coluna tenha
                //a propriedade backcolor alterada.
                //Se a propriedade for true, não permite alterar a propriedade BACKCOLOR e outras
                //propriedades dos subitems.
                objListViewItem.UseItemStyleForSubItems = false;

                objListViewItem.SubItems.Add("");

                objListViewItem.SubItems[1].BackColor = pnlCor.BackColor;
            }
        }


        private void btnRemover_Click(Object sender, EventArgs e)
        {
            ListViewItem objItem = null;


            foreach (ListViewItem objItem_loopVariable in lstPeriodoSelecionado.SelectedItems)
            {
                objItem = objItem_loopVariable;
                lstPeriodoSelecionado.Items.Remove(objItem);
            }
        }


        private void btnRemoverTodos_Click(Object sender, EventArgs e)
        {
            lstPeriodoSelecionado.Clear();
        }


        private void cmbAtivoPreencher(string pstrCodigoAtivoPadrao)
        {
            mCotacao.ComboAtivoPreencher(cmbAtivo,objConexao,pstrCodigoAtivoPadrao, true);
        }

        private void ParametroSalvar(string pstrParametro, string pstrValor)
        {

            var objDadosDB = new cDadosDB(objConexao, "Configuracao");

            objDadosDB.CampoAdicionar("Parametro", true, pstrParametro);

            objDadosDB.CampoAdicionar("Valor", false, pstrValor);

            objDadosDB.DadosBDSalvar();
        }


        private void IndicadorCarregar()
        {
            var objRS = new cRS(objConexao);

            ListViewItem objListViewItem;

            objRS.ExecuteQuery(" select NumPeriodos, Cor " + " from Configuracao_Indicador " + " where Tipo = " +
                               FuncoesBd.CampoStringFormatar("MME"));


            while (!objRS.EOF)
            {
                objListViewItem = lstPeriodoSelecionado.Items.Add(Convert.ToString(objRS.Field("NumPeriodos")));

                objListViewItem.UseItemStyleForSubItems = false;

                objListViewItem.SubItems.Add("");

                objListViewItem.SubItems[1].BackColor = Color.FromArgb(Convert.ToInt32(objRS.Field("Cor")));

                objRS.MoveNext();
            }

            objRS.Fechar();
        }


        private void IndicadorSalvar()
        {
            var objCommand = new cCommand(objConexao);


            objCommand.Execute(" DELETE " + " FROM Configuracao_Indicador " + " WHERE Tipo = " +
                               FuncoesBd.CampoStringFormatar("MME"));

            int intI = 0;


            if (chkMMExpDesenhar.Checked)
            {
                for (intI = 0; intI <= lstPeriodoSelecionado.Items.Count - 1; intI++)
                {
                    objCommand.Execute(" INSERT INTO Configuracao_Indicador " + "(Tipo, NumPeriodos, Cor)" + " VALUES " +
                                       "(" + FuncoesBd.CampoStringFormatar("MME") + ", " +
                                       lstPeriodoSelecionado.Items[intI].Text + ", " +
                                       lstPeriodoSelecionado.Items[intI].SubItems[1].BackColor.ToArgb().ToString() + ")");
                }
            }
        }


        private void pnlCor_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                objColorDialog.ShowDialog(this);
                pnlCor.BackColor = objColorDialog.Color;
            }
            catch (Exception)
            {
                //tem que ter sempre um exception para quando o usuário cancelar a tela
            }
        }

        private void ExibirMensagemDeCampoNaoPreenchido(string pstrNomeDoCampo)
        {
            MessageBox.Show("Preencha o campo " + pstrNomeDoCampo + ".",Text,MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnOK_Click(Object sender, EventArgs e)
        {
            if (rdbProxyManual.Checked)
            {
                //verifica se o campo de endereço e porta do proxy manual está preenchido

                if (txtProxyManualHTTP.Text.Trim() == String.Empty)
                {
                    ExibirMensagemDeCampoNaoPreenchido("HTTP");
                    
                    txtProxyManualHTTP.Focus();

                    return;
                }


                if (txtProxyManualPorta.Text.Trim() == String.Empty)
                {
                    ExibirMensagemDeCampoNaoPreenchido("Porta");
                    txtProxyManualPorta.Focus();

                    return;
                }
            }


            if (chkCredencialUtilizar.Checked)
            {
                if (txtUsuario.Text.Trim() == String.Empty)
                {
                    ExibirMensagemDeCampoNaoPreenchido("Usuário");
                    txtUsuario.Focus();

                    return;
                }


                if (txtSenha.Text.Trim() == String.Empty)
                {
                    ExibirMensagemDeCampoNaoPreenchido("Senha");
                    txtSenha.Focus();

                    return;
                }
            }

            //se não tem erros chama procedure para salvar os dados
            DadosSalvar();
        }


        private void btnCancelar_Click(Object sender, EventArgs e)
        {
            Close();
        }

    }
}