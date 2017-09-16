using System.Collections.ObjectModel;
using System;
using System.Windows.Forms;
using DataBase;
using TraderWizard.Enumeracoes;
using TraderWizard.ServicosDeAplicacao;

namespace TraderWizard
{

	public partial class frmCotacaoExcluir
	{


		private Conexao objConexao;

		public frmCotacaoExcluir(Conexao pobjConexao)
		{
			Load += frmCotacaoExcluir_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

		}


		private void ListDatasPreencher()
		{
			RS objRS = new RS(objConexao);

			//busca os ativos da tabela ativo
			objRS.ExecuteQuery(" SELECT Data " + " FROM Cotacao_Intraday " + " UNION " + " SELECT Data " + " FROM Cotacao_Intraday_Ativo " + " ORDER BY Data ");

			lstDataNaoEscolhida.Items.Clear();


			while (!objRS.Eof) {
				lstDataNaoEscolhida.Items.Add(objRS.Field("Data"));

				objRS.MoveNext();

			}

			objRS.Fechar();

		}


		private void frmCotacaoExcluir_Load(object sender, System.EventArgs e)
		{
			ListDatasPreencher();
		}


		private void btnAdicionarTodos_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			//percorre a lista de ativos não escolhidos

			for (intI = 0; intI <= lstDataNaoEscolhida.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos escolhidos
				lstDataEscolhida.Items.Add(lstDataNaoEscolhida.Items[intI]);

			}

			//remove todos os itens da lista de ativos não escolhidos
			lstDataNaoEscolhida.Items.Clear();

		}


		private void btnAdicionar_Click(System.Object sender, System.EventArgs e)
		{
			var colItem = new Collection<object>();


		    for (var intI = 0; intI <= lstDataNaoEscolhida.SelectedItems.Count - 1; intI++) {
				lstDataEscolhida.Items.Add(lstDataNaoEscolhida.SelectedItems[intI]);

				colItem.Add(lstDataNaoEscolhida.SelectedItems[intI]);

			}

			foreach (object item in colItem)
			{
			    lstDataNaoEscolhida.Items.Remove(item);
			}
		}


		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{
			//percorre a lista de ativos 

			for (var intI = 0; intI <= lstDataEscolhida.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos não escolhidos
				lstDataNaoEscolhida.Items.Add(lstDataEscolhida.Items[intI]);

			}

			//remove todos os itens da lista de ativos escolhidos
			lstDataEscolhida.Items.Clear();

		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			var colItem = new Collection<object>();



		    for (var intI = 0; intI <= lstDataEscolhida.SelectedItems.Count - 1; intI++) {
				lstDataNaoEscolhida.Items.Add(lstDataEscolhida.SelectedItems[intI]);

				colItem.Add(lstDataEscolhida.SelectedItems[intI]);

			}


			foreach (object item in colItem)
			{
			    lstDataEscolhida.Items.Remove(item);}
		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (lstDataEscolhida.Items.Count == 0) {
                MessageBox.Show("É necessário escolher pelo menos uma data.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;

			}


			if (MessageBox.Show("Confirma a exclusão das cotações na(s) data(s) escolhida(s)?", this.Text, MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes) {
				return;

			}

			System.DateTime[] arrData = {
				
			};

			Array.Resize(ref arrData, lstDataEscolhida.Items.Count);


			for (var intI = 0; intI <= lstDataEscolhida.Items.Count - 1; intI++) {
				arrData[intI] = Convert.ToDateTime(lstDataEscolhida.Items[intI]);

			}

			Array.Sort(arrData);


		    var atualizadorDeCotacao = new AtualizadorDeCotacao();

			cEnum.enumRetorno intRetorno = atualizadorDeCotacao.CotacaoExcluir(arrData, true);

			switch (intRetorno) {

				case cEnum.enumRetorno.RetornoOK:

                    MessageBox.Show("Operação executada com sucesso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

					break;
				case cEnum.enumRetorno.RetornoErroInesperado:

                    MessageBox.Show("Ocorreram erros ao executar a operação.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					break;
				case cEnum.enumRetorno.RetornoErro2:

                    MessageBox.Show("Existem cotações posteriores às datas escolhidas para serem excluídas.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					break;
			}
		}


	}
}
