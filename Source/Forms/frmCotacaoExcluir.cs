using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using DataBase;
using prmCotacao;
using frwInterface;
namespace TraderWizard
{

	public partial class frmCotacaoExcluir
	{


		private cConexao objConexao;

		public frmCotacaoExcluir(cConexao pobjConexao)
		{
			Load += frmCotacaoExcluir_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

		}


		private void ListDatasPreencher()
		{
			cRS objRS = new cRS(objConexao);

			//busca os ativos da tabela ativo
			objRS.ExecuteQuery(" SELECT Data " + " FROM Cotacao_Intraday " + " UNION " + " SELECT Data " + " FROM Cotacao_Intraday_Ativo " + " ORDER BY Data ");

			lstDataNaoEscolhida.Items.Clear();


			while (!objRS.EOF) {
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
			int intI = 0;

			Collection colItem = new Collection();

			object objItem = null;


			for (intI = 0; intI <= lstDataNaoEscolhida.SelectedItems.Count - 1; intI++) {
				lstDataEscolhida.Items.Add(lstDataNaoEscolhida.SelectedItems[intI]);

				colItem.Add(lstDataNaoEscolhida.SelectedItems[intI]);

			}


			foreach (object objItem_loopVariable in colItem) {
				objItem = objItem_loopVariable;
				lstDataNaoEscolhida.Items.Remove(objItem);

			}


		}


		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			//percorre a lista de ativos 

			for (intI = 0; intI <= lstDataEscolhida.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos não escolhidos
				lstDataNaoEscolhida.Items.Add(lstDataEscolhida.Items[intI]);

			}

			//remove todos os itens da lista de ativos escolhidos
			lstDataEscolhida.Items.Clear();

		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			Collection colItem = new Collection();

			object objItem = null;


			for (intI = 0; intI <= lstDataEscolhida.SelectedItems.Count - 1; intI++) {
				lstDataNaoEscolhida.Items.Add(lstDataEscolhida.SelectedItems[intI]);

				colItem.Add(lstDataEscolhida.SelectedItems[intI]);

			}


			foreach (object objItem_loopVariable in colItem) {
				objItem = objItem_loopVariable;
				lstDataEscolhida.Items.Remove(objItem);

			}

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (lstDataEscolhida.Items.Count == 0) {
				Interaction.MsgBox("É necessário escolher pelo menos uma data.", MsgBoxStyle.Information, this.Text);

				return;

			}


			if (MessageBox.Show("Confirma a exclusão das cotações na(s) data(s) escolhida(s)?", this.Text, MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes) {
				return;

			}

			System.DateTime[] arrData = {
				
			};

			Array.Resize(ref arrData, lstDataEscolhida.Items.Count);

			int intI = 0;


			for (intI = 0; intI <= lstDataEscolhida.Items.Count - 1; intI++) {
				arrData[intI] = Convert.ToDateTime(lstDataEscolhida.Items[intI]);

			}

			Array.Sort(arrData);

			cEnum.enumRetorno intRetorno = default(cEnum.enumRetorno);


			ServicoDeCotacao objCotacao = new ServicoDeCotacao(objConexao);

			intRetorno = objCotacao.CotacaoExcluir(arrData, true);

			switch (intRetorno) {

				case cEnum.enumRetorno.RetornoOK:

					Interaction.MsgBox("Operação executada com sucesso.", MsgBoxStyle.Information, this.Text);

					break;
				case cEnum.enumRetorno.RetornoErroInesperado:

					Interaction.MsgBox("Ocorreram erros ao executar a operação.", MsgBoxStyle.Exclamation, this.Text);

					break;
				case cEnum.enumRetorno.RetornoErro2:

					Interaction.MsgBox("Existem cotações posteriores às datas escolhidas para serem excluídas.", MsgBoxStyle.Exclamation, this.Text);

					break;
			}
		}


	}
}
