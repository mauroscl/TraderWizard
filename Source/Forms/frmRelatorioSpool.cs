using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using frwTela;
using DataBase;
using prmCotacao;
namespace TraderWizard
{

	public partial class frmRelatorioSpool
	{

		private cConexao objConexao;

		private cGrid objGrid;

		public frmRelatorioSpool(cConexao pobjConexao)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

			mCotacao.cmbAtivoPreencher(cmbAtivo, objConexao, true);

			objGrid = new cGrid(objConexao);

			objGrid.Tabela = "RELATORIOS_SPOOL";

		}


		private void GridAtualizar()
		{
			objGrid.Query = " SELECT R.COD_RELATORIO, R.CODIGO, A.DESCRICAO AS DESCRICAOATIVO, R.PERIODO, R.DATA " + ", R.DESCRICAO AS DESCRICAORELATORIO " + " FROM RELATORIOS_SPOOL R INNER JOIN ATIVO A " + " ON R.CODIGO = A.CODIGO " + " WHERE R.CODIGO = " + FuncoesBD.CampoStringFormatar(mCotacao.cmbAtivoCodigoRetornar(cmbAtivo)) + " ORDER BY R.COD_RELATORIO";

			DataSet objDataSet = new DataSet();


			if (objGrid.Atualizar(objDataSet)) {
				dgrRelatorioSpool.DataSource = objDataSet.Tables["RELATORIOS_SPOOL"].DefaultView;

				dgrRelatorioSpool.Refresh();

				//os nomes das colunas tem que ser atualizados após o refresh do datagridview
				dgrRelatorioSpool.Columns[0].HeaderText = "Código do Relatório";
				dgrRelatorioSpool.Columns[0].Visible = false;

				dgrRelatorioSpool.Columns[1].HeaderText = "Código do Ativo";
				dgrRelatorioSpool.Columns[1].Width = 50;

				dgrRelatorioSpool.Columns[2].HeaderText = "Descrição do Ativo";
				dgrRelatorioSpool.Columns[2].Width = 150;

				dgrRelatorioSpool.Columns[3].HeaderText = "Período";
				dgrRelatorioSpool.Columns[3].Width = 50;

				dgrRelatorioSpool.Columns[4].HeaderText = "Data";
				dgrRelatorioSpool.Columns[4].Width = 100;

				dgrRelatorioSpool.Columns[5].HeaderText = "Descrição do Relatório";
				dgrRelatorioSpool.Columns[5].Width = 500;

			}

		}


		private void btnAtualizar_Click(System.Object sender, System.EventArgs e)
		{
			GridAtualizar();

		}


		private void btnRelarioVisualizar_Click(System.Object sender, System.EventArgs e)
		{

			if (dgrRelatorioSpool.SelectedRows.Count > 0) {
				DataGridViewRow objDataGridViewRow = dgrRelatorioSpool.SelectedRows[0];

				// Create a new instance of the child form.
				frmRelatCrystal frmChildForm = new frmRelatCrystal((long) objDataGridViewRow.Cells[0].Value);
				// Make it a child of this MDI form before showing it.
				frmChildForm.MdiParent = this.MdiParent;

				//m_ChildFormNumber += 1

				frmChildForm.Show();

			}

		}


		private void btnExcluir_Click(System.Object sender, System.EventArgs e)
		{

			if (dgrRelatorioSpool.SelectedRows.Count > 0) {

                if (MessageBox.Show("Confirma a exclusão do relatório de back test selecionado?",this.Text,MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
					DataGridViewRow objDataGridViewRow = dgrRelatorioSpool.SelectedRows[0];

					cRelatorio objRelatorio = new cRelatorio(objConexao);

					if (objRelatorio.RelatorioSpoolExcluir(Convert.ToInt64(objDataGridViewRow.Cells[0].Value))) {
						GridAtualizar();
					} else {
                        MessageBox.Show("Ocorreram erros ao excluir o relatório selecionado.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}

				}

			}

		}

	}
}
