using System;
using System.Windows.Forms;
using DataBase;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;
using TraderWizard.ServicosDeAplicacao;

namespace Forms
{

	public partial class frmProventoAtualizar
	{

		private bool DadosConsistir()
		{
			if (String.IsNullOrEmpty(txtDataFinal.Text.Trim())) {

				if (!txtDataFinal.Text.IsDate()) {
                    MessageBox.Show("Campo \"Data Final\" não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            Conexao objConexao = new Conexao();

		    var proventoService = new ProventoService();

		    if (!DateTime.TryParse(txtDataFinal.Text, out var dataFinal) ) {
		        dataFinal = Constantes.DataInvalida;
		    }

		    this.Cursor = Cursors.WaitCursor;

		    //If objCotacao.ProventoAtualizar(CDate(txtDataUltimAtualizacao.Text), dtmDataFinal) Then
		    if (proventoService.ProventoAtualizar(dataFinal)) {
		        MessageBox.Show("Operação realizada com sucesso.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		    } else {
		        MessageBox.Show("Ocorreram erros ao executar a operação.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		    }

		    this.Cursor = Cursors.Default;

		    objConexao.FecharConexao();
		}

	}
}
