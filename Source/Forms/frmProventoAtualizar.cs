using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using prmCotacao;
using DataBase;
using frwInterface;
namespace TraderWizard
{

	public partial class frmProventoAtualizar
	{

		private bool DadosConsistir()
		{
			if (String.IsNullOrEmpty(txtDataFinal.Text.Trim())) {

				if (!Information.IsDate(txtDataFinal.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Data Final" + " não preenchido ou com valor inválido.", MsgBoxStyle.Information, this.Text);

					return false;

				}

			}

			return true;

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{
		    if (!DadosConsistir()) return;
		    if (MessageBox.Show("Confirma a execução da operação de atualização de Proventos?", this.Text
                , MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            cConexao objConexao = new cConexao();

		    cCotacao objCotacao = new cCotacao(objConexao);

		    DateTime dtmDataFinal;
		    if (!DateTime.TryParse(txtDataFinal.Text, out dtmDataFinal) ) {
		        dtmDataFinal = cConst.DataInvalida;
		    }

		    this.Cursor = Cursors.WaitCursor;

		    //If objCotacao.ProventoAtualizar(CDate(txtDataUltimAtualizacao.Text), dtmDataFinal) Then
		    if (objCotacao.ProventoAtualizar(dtmDataFinal)) {
		        MessageBox.Show("Operação realizada com sucesso.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		    } else {
		        MessageBox.Show("Ocorreram erros ao executar a operação.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		    }

		    this.Cursor = Cursors.Default;

		    objConexao.FecharConexao();
		}

	}
}
