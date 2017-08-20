using System;
using System.Drawing;
using System.Windows.Forms;
using DataBase;
using Forms;
using pWeb;
using TraderWizard.Extensoes;

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
            var objComando = new Command(objConexao);

            string strProxyType = String.Empty;
            string strProxyManualHttp = String.Empty;
            string strProxyManualPorta = String.Empty;

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
                strProxyManualHttp = txtProxyManualHTTP.Text.Trim();
                strProxyManualPorta = txtProxyManualPorta.Text.Trim();
            }

            string strProxyCredencialUtilizar = chkCredencialUtilizar.Checked ? "SIM" : "NAO";

            objComando.BeginTrans();

            ParametroSalvar("ProxyTipo", strProxyType);

            if (strProxyType == "PM")
            {
                ParametroSalvar("ProxyManualHTTP", strProxyManualHttp);
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

                var objCriptografia = new cCriptografia();

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

        private bool Consistir(string periodo, string tipo)
        {
            if (!periodo.IsNumeric())
            {
                MessageBox.Show("Campo " + Convert.ToChar(34) + "Período" + Convert.ToChar(34) + " não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }


            for (int intI = 0; intI <= lstPeriodoSelecionado.Items.Count - 1; intI++)
            {
                ListViewItem listViewItem = lstPeriodoSelecionado.Items[intI];
                if (periodo == listViewItem.Text.Trim() && tipo == listViewItem.SubItems[1].Text)
                {

                    MessageBox.Show("Esta configuração já foi inserida.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return false;
                }
            }

            return true;
        }


        private void btnAdicionar_Click(Object sender, EventArgs e)
        {

            string strTipo = "";
            if (rdbExponencial.Checked)
            {
                strTipo = "MME";
            }
            else if (rdbAritmetica.Checked)
            {
                strTipo = "MMA";
            }

            if (!Consistir(txtPeriodo.Text, strTipo)) return;
            
            ListViewItem objListViewItem = lstPeriodoSelecionado.Items.Add(txtPeriodo.Text);

            objListViewItem.SubItems.Add(strTipo);

            //seta esta propriedade para false para permitir que apenas uma coluna tenha
            //a propriedade backcolor alterada.
            //Se a propriedade for true, não permite alterar a propriedade BACKCOLOR e outras
            //propriedades dos subitems.
            objListViewItem.UseItemStyleForSubItems = false;

            objListViewItem.SubItems.Add("");

            objListViewItem.SubItems[2].BackColor = pnlCor.BackColor;
        }


        private void btnRemover_Click(Object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in lstPeriodoSelecionado.SelectedItems)
            {
                lstPeriodoSelecionado.Items.Remove(listViewItem);
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

            var objDadosDB = new DadosDb(objConexao, "Configuracao");

            objDadosDB.CampoAdicionar("Parametro", true, pstrParametro);

            objDadosDB.CampoAdicionar("Valor", false, pstrValor);

            objDadosDB.DadosBDSalvar();
        }


        private void IndicadorCarregar()
        {
            var objRS = new RS(objConexao);

            var funcoesBd = objConexao.ObterFormatadorDeCampo();

            string query = " select Tipo, NumPeriodos, Cor " + " from Configuracao_Indicador " +
                               " where (Tipo  = " + funcoesBd.CampoFormatar("MME") + " OR Tipo = " + funcoesBd.CampoFormatar("MMA") + ")";

            objRS.ExecuteQuery(query);


            while (!objRS.Eof)
            {
                ListViewItem objListViewItem = lstPeriodoSelecionado.Items.Add(Convert.ToString(objRS.Field("NumPeriodos")));

                objListViewItem.SubItems.Add(Convert.ToString(objRS.Field("Tipo")));

                objListViewItem.UseItemStyleForSubItems = false;

                objListViewItem.SubItems.Add("");

                objListViewItem.SubItems[2].BackColor = Color.FromArgb(Convert.ToInt32(objRS.Field("Cor")));

                objRS.MoveNext();
            }

            objRS.Fechar();
        }


        private void IndicadorSalvar()
        {
            var objCommand = new Command(objConexao);

            var funcoesBd = objConexao.ObterFormatadorDeCampo();

            objCommand.Execute(" DELETE " + " FROM Configuracao_Indicador " +
                " WHERE (Tipo = " + funcoesBd.CampoFormatar("MME") + " OR Tipo = " + funcoesBd.CampoFormatar("MMA") + ")");


            if (chkMMExpDesenhar.Checked)
            {
                for (int i = 0; i <= lstPeriodoSelecionado.Items.Count - 1; i++)
                {
                    objCommand.Execute(" INSERT INTO Configuracao_Indicador " + "(Tipo, NumPeriodos, Cor)" + " VALUES " +
                                       "(" + funcoesBd.CampoFormatar(lstPeriodoSelecionado.Items[i].SubItems[1].Text) + ", " +
                                       lstPeriodoSelecionado.Items[i].Text + ", " +
                                       lstPeriodoSelecionado.Items[i].SubItems[2].BackColor.ToArgb() + ")");
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