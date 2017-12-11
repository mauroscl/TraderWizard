using DataBase.Carregadores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DataBase;
using Dominio.Entidades;
using frwTela;
using ServicosDeInterface;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;
using TraderWizard.ServicosDeAplicacao;

namespace TraderWizard
{

	public partial class frmRelatVisualizacao
	{

		private readonly Conexao objConexao;

		private DateTime dtmDataDeGeracaoDoRelatorio;

		public frmRelatVisualizacao(Conexao pobjConexao)
		{
			Shown += frmRelatVisualizacao_Shown;
			Load += frmRelatVisualizacao_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			objConexao = pobjConexao;

		}


		private void CoresAtualizar()
		{
		    DataGridViewCellStyle objCorVerde = new DataGridViewCellStyle();
			objCorVerde.ForeColor = Color.Green;
			objCorVerde.SelectionForeColor = Color.Green;

			DataGridViewCellStyle objCorVermelho = new DataGridViewCellStyle();
			objCorVermelho.ForeColor = Color.Red;
			objCorVermelho.SelectionForeColor = Color.Red;

			CarregadorCriterioClassificacaoMedia objCarregadorCriterioCM = new CarregadorCriterioClassificacaoMedia();

			IList<CriterioClassifMedia> lstCriterios = objCarregadorCriterioCM.CarregaTodos();



			for (int intI = 0; intI <= objDataGridView.Rows.Count - 1; intI++) {
				int intSomatorioCriterios = Convert.ToInt32(objDataGridView.Rows[intI].Cells["SomatorioCriterios"].Value);


				if (intSomatorioCriterios == 0) {
					//todos os critérios OK.
					foreach (CriterioClassifMedia objCriterioCM in lstCriterios) {
						objDataGridView.Rows[intI].Cells[objCriterioCM.CampoRelatorio].Style = objCorVerde;
						objDataGridView.Rows[intI].Cells["Codigo"].Style = objCorVerde;
					}

				} else if (intSomatorioCriterios > 0) {

					foreach (CriterioClassifMedia objCriterioCM in lstCriterios) {
						if ((intSomatorioCriterios & objCriterioCM.Peso) == 0) {
							objDataGridView.Rows[intI].Cells[objCriterioCM.CampoRelatorio].Style = objCorVerde;
						//objDataGridView.Rows(intI).Cells.Item(objCriterioCM.CampoRelatorio).Value = objDataGridView.Rows(intI).Cells.Item(objCriterioCM.CampoRelatorio).Value & " *"
						} else {
							objDataGridView.Rows[intI].Cells[objCriterioCM.CampoRelatorio].Style = objCorVermelho;
						}

					}

				}


				if (intSomatorioCriterios >= 0) {
					if ((intSomatorioCriterios & 32) == 0) {
						objDataGridView.Rows[intI].Cells["NumTentativas"].Style = objCorVerde;
					} else {
						objDataGridView.Rows[intI].Cells["NumTentativas"].Style = objCorVermelho;
					}

					if ((intSomatorioCriterios & 64) == 0) {
						objDataGridView.Rows[intI].Cells["Aproveitamento"].Style = objCorVerde;
					} else {
						objDataGridView.Rows[intI].Cells["Aproveitamento"].Style = objCorVermelho;
					}

				}

			}


		}


		public void GridCarregar(string pstrQuery)
		{
			var objDataSet = new DataSet();

			int intHeaderIndex = 0;

			var objGrid = new cGrid(objConexao) {Query = pstrQuery, Tabela = "Cotacao"};

		    cEnum.enumSetup intSetup = ComboSetupMapear();


			if (objGrid.Atualizar(objDataSet)) {
				objDataGridView.DataSource = objDataSet;

				objDataGridView.DataMember = "Cotacao";

				objDataGridView.Refresh();


				if (intSetup == cEnum.enumSetup.IFRSemFiltroRP) {
					//A última coluna serve apenas para controle.
					objDataGridView.Columns["SomatorioCriterios"].Visible = false;

					objDataGridView.Columns["Codigo"].HeaderText = "Código";
					objDataGridView.Columns["Valor_IFR"].HeaderText = "IFR 2";

					objDataGridView.Columns["Valor_Entrada"].HeaderText = "Entrada";
					objDataGridView.Columns["Valor_Entrada"].DefaultCellStyle.Format = "###,###0.00";

					objDataGridView.Columns["Valor_Realizacao_Parcial"].HeaderText = "Real. Parcial";
					objDataGridView.Columns["Valor_Realizacao_Parcial"].DefaultCellStyle.Format = "###,###0.00";

					objDataGridView.Columns["Valor_Realizacao_Final"].HeaderText = "Real. Final";
					objDataGridView.Columns["Valor_Realizacao_Final"].DefaultCellStyle.Format = "###,###0.00";

					objDataGridView.Columns["Valor_Stop_Loss"].HeaderText = "Stop Loss";
					objDataGridView.Columns["Valor_Stop_Loss"].DefaultCellStyle.Format = "###,###0.00";

					objDataGridView.Columns["Percentual_Stop_Loss"].HeaderText = "% Stop Loss";

					//Quantidade em lotes de 100 que pode ser comprado com todo o capital.
					objDataGridView.Columns["Quantidade_Sem_Manejo"].HeaderText = "Quant. S/Manejo";


					//valor total da compra com todo o capital
					objDataGridView.Columns["Valor_Total_Sem_Manejo"].HeaderText = "Total S/Manejo (R$)";
					objDataGridView.Columns["Valor_Total_Sem_Manejo"].DefaultCellStyle.Format = "###,###0.00";

					//percentual do risco utilizando todo o capital possível em relação ao capital total
					objDataGridView.Columns["Percentual_Risco_Sem_Manejo"].HeaderText = "% Risco S/Manejo";

					//Quantidade que pode ser comprada utilizando o manejo de risco.
					objDataGridView.Columns["Quantidade_Com_Manejo"].HeaderText = "Quant. C/Manejo";

					//valor total da compra utilizando o manejo de risco.
					objDataGridView.Columns["Valor_Total_Com_Manejo"].HeaderText = "Total C/Manejo (R$)";
					objDataGridView.Columns["Valor_Total_Com_Manejo"].DefaultCellStyle.Format = "###,###0.00";

					//percentual do risco utilizando o manejo em relação ao capital total
					objDataGridView.Columns["Percentual_Risco_Com_Manejo"].HeaderText = "% Risco C/ Manejo";

					objDataGridView.Columns["ID_CM"].HeaderText = "Classif. Média";

					objDataGridView.Columns["Percentual_MM21"].HeaderText = "% MM 21";

					objDataGridView.Columns["Percentual_MM49"].HeaderText = "% MM 49";
					objDataGridView.Columns["Percentual_MM200"].HeaderText = "% MM 200";
					objDataGridView.Columns["Diferenca_MM200_MM21"].HeaderText = "MM 200 - MM 21";
					objDataGridView.Columns["Diferenca_MM200_MM49"].HeaderText = "MM 200 - MM 49";
					objDataGridView.Columns["NumTentativas"].HeaderText = "Tentativas";

					CoresAtualizar();


				} else {
					//tem que ajustar o header das colunas depois de fazer o refresh no grid.
					//se isto for feito antes os headers ficam com o nome da coluna utilizada no select.
					objDataGridView.Columns[intHeaderIndex].HeaderText = "Código";
					intHeaderIndex = intHeaderIndex + 1;

					objDataGridView.Columns[intHeaderIndex].HeaderText = "Fechamento";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;


					if (intSetup == cEnum.enumSetup.MM91 || intSetup == cEnum.enumSetup.MM92 || intSetup == cEnum.enumSetup.MM93) {
						//setups de média móvel de 9 períodos: 
						//mostra o valor da MME 9 e o percentual do valor de fechamento em relação 
						//às mesmas.

						objDataGridView.Columns[intHeaderIndex].HeaderText = "MME 9";
						intHeaderIndex = intHeaderIndex + 1;

						objDataGridView.Columns[intHeaderIndex].HeaderText = "% MME 9";
						intHeaderIndex = intHeaderIndex + 1;


					} else {
						//setups de ifr 2: mostra o valor do ifr de 2 períodos.
						objDataGridView.Columns[intHeaderIndex].HeaderText = "IFR 2";
						intHeaderIndex = intHeaderIndex + 1;

					}


					if (intSetup == cEnum.enumSetup.IFRComFiltro) {
						objDataGridView.Columns[intHeaderIndex].HeaderText = "MME 49";
						objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
						intHeaderIndex = intHeaderIndex + 1;

						objDataGridView.Columns[intHeaderIndex].HeaderText = "% MME 49";
						intHeaderIndex = intHeaderIndex + 1;

					}

					objDataGridView.Columns[intHeaderIndex].HeaderText = "Entrada";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;


					if (intSetup != cEnum.enumSetup.IFRSemFiltro && intSetup != cEnum.enumSetup.IFRSemFiltroRP) {
						//para o IFR 2 SOBREVENDIDO não tem % de entrada, porque a entrada já é feita no fechamento.
						objDataGridView.Columns[intHeaderIndex].HeaderText = "% Entrada";
						intHeaderIndex = intHeaderIndex + 1;

					}

					objDataGridView.Columns[intHeaderIndex].HeaderText = "Stop Loss";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;

					objDataGridView.Columns[intHeaderIndex].HeaderText = "% Stop Loss";
					intHeaderIndex = intHeaderIndex + 1;

					//Quantidade em lotes de 100 que pode ser comprado com todo o capital.
					objDataGridView.Columns[intHeaderIndex].HeaderText = "Quant. Capital";
				    objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "n0";

					intHeaderIndex = intHeaderIndex + 1;

					//valor total da compra com todo o capital
					objDataGridView.Columns[intHeaderIndex].HeaderText = "Total Capital (R$)";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;

					//percentual do risco utilizando todo o capital possível em relação ao capital total
					objDataGridView.Columns[intHeaderIndex].HeaderText = "% Risco / Capital";
					intHeaderIndex = intHeaderIndex + 1;

					//Quantidade que pode ser comprada utilizando o manejo de risco.
					objDataGridView.Columns[intHeaderIndex].HeaderText = "Quant. Manejo";
                    objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "n0";
					intHeaderIndex = intHeaderIndex + 1;

					//valor total da compra utilizando o manejo de risco.
					objDataGridView.Columns[intHeaderIndex].HeaderText = "Total Manejo (R$)";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;

					//percentual do risco utilizando o manejo em relação ao capital total
					objDataGridView.Columns[intHeaderIndex].HeaderText = "% Risco / Capital";
					intHeaderIndex = intHeaderIndex + 1;

					objDataGridView.Columns[intHeaderIndex].HeaderText = "Stop Gain";
					objDataGridView.Columns[intHeaderIndex].DefaultCellStyle.Format = "###,###0.00";
					intHeaderIndex = intHeaderIndex + 1;

					objDataGridView.Columns[intHeaderIndex].HeaderText = "% MME 21";
					//intHeaderIndex = intHeaderIndex + 1

				}


				objDataGridView.AutoResizeColumns();

				objDataGridView.Refresh();

			}
			//if objGridAtualizar...

		}


		private void cmbSetup_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			cEnum.enumSetup intSetup = ComboSetupMapear();

			if (intSetup == cEnum.enumSetup.IFRSemFiltro || intSetup == cEnum.enumSetup.IFRSemFiltroRP) {
				//setup ifr 2 sobrevendido
				txtIFR2Maximo.Enabled = true;
			} else {
				//demais setups.
				txtIFR2Maximo.Enabled = false;
			}

			if (intSetup == cEnum.enumSetup.IFRSemFiltro || intSetup == cEnum.enumSetup.IFRComFiltro) {
				chkAcimaMME49.Enabled = true;
			} else {
				chkAcimaMME49.Enabled = false;
			}


			if (intSetup == cEnum.enumSetup.IFRSemFiltroRP) {
				rdbDiario.Checked = true;

			}

			if (intSetup == cEnum.enumSetup.IFRSemFiltroRP) {
				lblIFRFiltro.Visible = true;
				cmbIFRFiltro.Visible = true;
			} else {
				lblIFRFiltro.Visible = false;
				cmbIFRFiltro.Visible = false;
			}

		}

		private void rdbStopGainPercentual_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rdbStopGainPercentual.Checked) {
				txtStopGainPercentual.Enabled = true;
			} else {
				txtStopGainPercentual.Enabled = false;
			}
		}

		private bool Consistir()
		{


			if (!txtData.Text.IsDate()) {
                MessageBox.Show("Campo \"Data\"  não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				txtData.Focus();

				return false;

			}

			cEnum.enumSetup intSetup = ComboSetupMapear();


			if (intSetup == cEnum.enumSetup.IFRSemFiltro || intSetup == cEnum.enumSetup.IFRSemFiltroRP) {

                if (!txtIFR2Maximo.Text.IsNumeric())
                {
					MessageBox.Show("Campo \"IFR 2 abaixo de\"  não preenchido ou com valor inválido.", this.Text,MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					txtIFR2Maximo.Focus();

					return false;

				}

			}


			if (rdbStopGainPercentual.Checked) {

				if (txtStopGainPercentual.Text.IsNumeric()) {
                    MessageBox.Show("Campo \"Percentual\" do Stop Gain não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					txtStopGainPercentual.Focus();

					return false;

				}

			}


            if (!txtValorCapital.Text.IsNumeric())
            {
                MessageBox.Show("Campo \"Negocios do Capital\" não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				txtValorCapital.Focus();

				return false;

			}


            if (!txtPercentualManejo.Text.IsNumeric())
            {
                MessageBox.Show("Campo \"% Manejo\"  não preenchido ou com valor inválido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

				txtPercentualManejo.Focus();

				return false;

			}

			return true;

		}

		private cEnum.enumSetup ComboSetupMapear()
		{

			cEnum.enumSetup intSetup = default(cEnum.enumSetup);
			switch (cmbSetup.SelectedIndex) {

				case 0:

					intSetup = cEnum.enumSetup.MM91;

					break;
				case 1:
					intSetup = cEnum.enumSetup.MM92;
					break;
				case 2:
					intSetup = cEnum.enumSetup.MM93;
					break;
				case 3:
					intSetup = cEnum.enumSetup.IFRSemFiltro;
					break;
				case 4:
					intSetup = cEnum.enumSetup.IFRSemFiltroRP;
					break;
				case 5:
					intSetup = cEnum.enumSetup.IFRComFiltro;

					break;

			}

			return intSetup;

		}


		private void btnRelarioGerar_Click(System.Object sender, System.EventArgs e)
		{

			if (!Consistir()) {
				return;

			}

			dtmDataDeGeracaoDoRelatorio = Convert.ToDateTime(txtData.Text);

		    int intNegociosTotal = txtNegociosTotal.Text.IsNumeric() ? Convert.ToInt32(txtNegociosTotal.Text) : -1;

		    double dblTitulosTotal = txtTitulosTotal.Text.IsNumeric() ? Convert.ToDouble(txtTitulosTotal.Text) : -1;

		    decimal decValorTotal = txtValorTotal.Text.IsNumeric() ? Convert.ToDecimal(txtValorTotal.Text) : -1;

            string strPeriodo = String.Empty;

            if (rdbDiario.Checked)
            {
				strPeriodo = "DIARIO";
			} else if (rdbSemanal.Checked) {
				strPeriodo = "SEMANAL";
			} 

		    decimal decPercentualStopGain = rdbStopGainPercentual.Checked ? Convert.ToDecimal(txtStopGainPercentual.Text) : -1;

		    var objRelatorio = new cRelatorio(objConexao);

			cEnum.enumSetup intSetup = ComboSetupMapear();

			this.Cursor = Cursors.WaitCursor;

			IFRSobrevendido objIFRSobrevendido = null;


			if (intSetup == cEnum.enumSetup.IFRSemFiltroRP) {
				objIFRSobrevendido = (IFRSobrevendido)cmbIFRFiltro.SelectedItem;

			}

			string strQuery = objRelatorio.RelatListagemCalcular(dtmDataDeGeracaoDoRelatorio, intSetup, strPeriodo, rdbStopGainAlijamento.Checked, Convert.ToDecimal(txtValorCapital.Text), Convert.ToDecimal(txtPercentualManejo.Text), decPercentualStopGain, Convert.ToDouble(txtIFR2Maximo.Text), chkAcimaMME49.Checked, dblTitulosTotal,
			    intNegociosTotal, decValorTotal, objIFRSobrevendido);

			GridCarregar(strQuery);

			this.Cursor = Cursors.Default;

		}


		private void frmRelatVisualizacao_Load(object sender, System.EventArgs e)
		{
			CarregadorIFRSobrevendido objCarregador = new CarregadorIFRSobrevendido(this.objConexao);

			IList<IFRSobrevendido> lstLista = objCarregador.CarregarTodos();

			cmbIFRFiltro.Items.Clear();


			foreach (IFRSobrevendido objItem in lstLista) {
				cmbIFRFiltro.Items.Add(objItem);

			}

			cmbIFRFiltro.SelectedIndex = 0;

		}



		private void frmRelatVisualizacao_Shown(object sender, System.EventArgs e)
		{
			//ajusta localização e tamanho do form
			this.Location = new Point(0, 0);
			this.Height = MdiParent.ClientSize.Height - 30;
			this.Width = MdiParent.ClientSize.Width - 4;

			objDataGridView.Height = this.Height - 220;
			objDataGridView.Width = this.Width - 20;
			objDataGridView.Location = new Point(5, 180);

			//seta o combo de setups para começar com a opção "IFR Sobrevendido Sem Filtro"
			cmbSetup.SelectedIndex = 3;

			txtData.Text = DateTime.Now.ToString("dd/MM/yyyy");

			string strValor;

			//consulta valores da tabela configuração para preencher os campos
			mdlGeral.ParametroConsultar(objConexao, "ValorCapital", out strValor);

			txtValorCapital.Text = strValor;

			mdlGeral.ParametroConsultar(objConexao, "PercentualManejo", out strValor);

			txtPercentualManejo.Text = strValor;

		}


		private void objDataGridView_ColumnHeaderMouseClick(System.Object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
		{

			if (ComboSetupMapear() == cEnum.enumSetup.IFRSemFiltroRP) {
				CoresAtualizar();

			}

		}



		private void objDataGridView_CellToolTipTextNeeded(System.Object sender, System.Windows.Forms.DataGridViewCellToolTipTextNeededEventArgs e)
		{
			cEnum.enumSetup intSetup = ComboSetupMapear();


			if (intSetup == cEnum.enumSetup.IFRSemFiltroRP && e.RowIndex >= 0) {
				cEnum.enumCriterioClassificacaoMedia intIDCriterioCM = default(cEnum.enumCriterioClassificacaoMedia);


				if (objDataGridView.Columns["Percentual_MM21"].Index == e.ColumnIndex) {
					intIDCriterioCM = cEnum.enumCriterioClassificacaoMedia.PercentualMM21;


				} else if (objDataGridView.Columns["Percentual_MM49"].Index == e.ColumnIndex) {
					intIDCriterioCM = cEnum.enumCriterioClassificacaoMedia.PercentualMM49;


				} else if (objDataGridView.Columns["Percentual_MM200"].Index == e.ColumnIndex) {
					intIDCriterioCM = cEnum.enumCriterioClassificacaoMedia.PercentualMM200;


				} else if (objDataGridView.Columns["Diferenca_MM200_MM21"].Index == e.ColumnIndex) {
					intIDCriterioCM = cEnum.enumCriterioClassificacaoMedia.PercentualDiferencaMM200MM21;


				} else if (objDataGridView.Columns["Diferenca_MM200_MM49"].Index == e.ColumnIndex) {
					intIDCriterioCM = cEnum.enumCriterioClassificacaoMedia.PercentualDiferencaMM200MM49;


				} else {
					return;

				}

				string strCodigo = objDataGridView.Rows[e.RowIndex].Cells["Codigo"].Value.ToString();

				int intIndex = strCodigo.IndexOf(" *");

				if (intIndex >= 0) {
					strCodigo = strCodigo.Substring(0, intIndex + 1);
				}

				e.ToolTipText = GeradorToolTip.GerarToolTipRelatorioSetupEntrada(strCodigo, cEnum.enumSetup.IFRSemFiltroRP, (IFRSobrevendido)cmbIFRFiltro.SelectedItem
                    , (cEnum.enumClassifMedia) Enum.Parse(typeof(cEnum.enumClassifMedia),(string) objDataGridView.Rows[e.RowIndex].Cells["ID_CM"].Value) , intIDCriterioCM, dtmDataDeGeracaoDoRelatorio);

			}

		}


		private void btnCalendario_Click(System.Object sender, System.EventArgs e)
		{
			if (txtData.Text.IsDate()) {
				Calendario.SetDate(Convert.ToDateTime(txtData.Text));
			}

			Calendario.Show();
		}


		private void Calendario_DateSelected(System.Object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			txtData.Text = Calendario.SelectionRange.Start.ToString("dd/MM/yyyy");
			Calendario.Hide();

        }

        private void Calendario_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue != (int) Keys.Escape) return;

            var calendario = sender as MonthCalendar;
            if (calendario != null)
            {
                calendario.Hide();
            }
        }

        private void Calendario_MouseHover(object sender, EventArgs e)
        {
            var calendario = sender as MonthCalendar;
            if (calendario != null)
            {
                calendario.Focus();
            }
        }
	}
}
