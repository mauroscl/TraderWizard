using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using DataBase;
using DataBase.Carregadores;
using DataBase.Interfaces;
using prjDTO;
using prmCotacao;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;

namespace Forms
{

	public partial class frmIFRCalcular
	{


		public frmIFRCalcular()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.

		}


		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();

		}

		private bool CamposValidar()
		{

			//se não tem ativos selecionados

			if (lstAtivosEscolhidos.Items.Count == 0) {
			    MessageBox.Show("Nenhum ativo foi escolhido.", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				return false;

			}


            if (!txtPeriodo.Text.IsNumeric())
            {

                MessageBox.Show("Período não preenchido ou inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;

			}

			return true;

		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{
			if (!CamposValidar()) {
				return;
			}

			Cursor = Cursors.WaitCursor;


		    var ativosSelecionados = "";
            ativosSelecionados = lstAtivosEscolhidos.Items.Cast<AtivoSelecao>().Aggregate(ativosSelecionados, (current, ativoSelecionado) => current + ("#" + ativoSelecionado.Codigo));

            ativosSelecionados += "#";

			IList<int> colPeriodos = new List<int>();
			colPeriodos.Add(Convert.ToInt32(txtPeriodo.Text));

		    bool blnOkDiario = true;
		    bool blnOkSemanal = true;

            var conexao = new Conexao();
		    try
		    {
		        var objCotacao = new ServicoDeCotacao(conexao);

		        if (periodicidadeDiaria.Checked)
		        {
		            blnOkDiario = objCotacao.IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Diario, Constantes.DataInvalida,
		                ativosSelecionados);
		        }

		        if (periodicidadeSemanal.Checked)
		        {
		            blnOkSemanal = objCotacao.IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Semanal,
		                Constantes.DataInvalida, ativosSelecionados);
		        }
		    }
		    catch (Exception exception)
		    {
                MessageBox.Show(string.Format("Ocorreram erros ao calcular o IFR. Detalhes: {0}", exception.Message), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
		    }
		    finally
		    {
		        conexao.FecharConexao();
		    }

		    if (blnOkDiario && blnOkSemanal)
		    {
		        MessageBox.Show("Cálculo do IFR realizado com sucesso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
		    }
            else if (!blnOkDiario)
            {
                MessageBox.Show("Ocorreram erros ao calcular o IFR diário.", Text, MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Ocorreram erros ao calcular o IFR semanal.", Text, MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

		    Cursor = Cursors.Default;

		}


		private void btnAdicionarTodos_Click(System.Object sender, System.EventArgs e)
		{
			//percorre a lista de ativos não escolhidos

			for (var intI = 0; intI <= lstAtivosNaoEscolhidos.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos escolhidos
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.Items[intI]);

			}

			//remove todos os itens da lista de ativos não escolhidos
			lstAtivosNaoEscolhidos.Items.Clear();

		}


		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{
			//percorre a lista de ativos 

			for (int i = 0; i <= lstAtivosEscolhidos.Items.Count - 1; i++) {
				//adiciona o item na lista de ativos não escolhidos
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.Items[i]);

			}

			//remove todos os itens da lista de ativos escolhidos
			lstAtivosEscolhidos.Items.Clear();

		}


		private void btnAdicionar_Click(System.Object sender, System.EventArgs e)
		{
			var colItem = new Collection<object>();

			for (var intI = 0; intI <= lstAtivosNaoEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

			}


			foreach (object item in colItem) {
				lstAtivosNaoEscolhidos.Items.Remove(item);

			}

		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			var colItem = new Collection<object>();

			for (var intI = 0; intI <= lstAtivosEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosEscolhidos.SelectedItems[intI]);

			}


			foreach (object item in colItem) {
				lstAtivosEscolhidos.Items.Remove(item);
			}

		}


		private void ListAtivosPreencher()
		{
		    using (ICarregadorDeAtivo carregadorDeAtivo = new CarregadorDeAtivo())
		    {
		        
		        IEnumerable<AtivoSelecao> ativos = carregadorDeAtivo.Carregar();

		        foreach (var ativoSelecao in ativos)
		        {
		            lstAtivosNaoEscolhidos.Items.Add(ativoSelecao);

		        }

		    }
		}

		private void frmIDiarioCalcular_Load(object sender, System.EventArgs e)
		{
			ListAtivosPreencher();
		}
	}
}
