using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using prjDTO;
namespace TraderWizard
{

	public partial class frmIndicadorEscolha
	{

		//Private colStructIndicadorEscolha As Collection

		private readonly List<cMediaDTO> lstMediasSelecionadas;
		//Indica o item do grid que está selecionado

		private ListViewItem ItemSelecionado;
		//Public Sub New(ByVal pstrText As String, ByVal pcolStructPeriodoEscolha As Collection)

		public frmIndicadorEscolha(string pstrText, List<cMediaDTO> plstMediasSelecionadas)
		{
			KeyUp += frmIndicadorEscolha_KeyUp;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.

			ItemSelecionado = null;

			this.Text = pstrText;

			//colStructIndicadorEscolha = pcolStructPeriodoEscolha
			lstMediasSelecionadas = plstMediasSelecionadas;

			//Dim objStructIndicadorEscolha As structIndicadorEscolha

			ListViewItem objListViewItem = null;

			//If colStructIndicadorEscolha Is Nothing Then
			//    colStructIndicadorEscolha = New Collection
			//End If

			if (lstMediasSelecionadas == null) {
				lstMediasSelecionadas = new List<cMediaDTO>();
			}

			//MONTA OS ITENS RECEBIDOS POR PARÂMETRO.

			foreach (cMediaDTO objMediaDTO in lstMediasSelecionadas) {
				//período
				objListViewItem = lstPeriodoSelecionado.Items.Add(objMediaDTO.NumPeriodos.ToString());

				//tipo
				objListViewItem.SubItems.Add((objMediaDTO.Tipo == "E" ? "Exponencial" : "Aritmética"));

				objListViewItem.UseItemStyleForSubItems = false;

				//cor
				objListViewItem.SubItems.Add("");

				objListViewItem.SubItems[2].BackColor = objMediaDTO.Cor;

			}

			ToolTipCancelar.SetToolTip(btnAdicionar, "Pressione ESC para cancelar a edição do item selecionado");

		}

		public List<cMediaDTO> MediasSelecionadas {
			get { return lstMediasSelecionadas; }
		}

		private string ObtemTipoDaMedia()
		{
			if (rdbExponencial.Checked) {
				return "Exponencial";
			} else if (rdbAritmetica.Checked) {
				return "Aritmética";
			} else {
				return string.Empty;
			}

		}

		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{
			lstPeriodoSelecionado.Items.Clear();
			LimparItemSelecionado();
		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			ListViewItem objItem = null;


			foreach (ListViewItem objItem_loopVariable in lstPeriodoSelecionado.SelectedItems) {
				objItem = objItem_loopVariable;
				lstPeriodoSelecionado.Items.Remove(objItem);

			}

			LimparItemSelecionado();

		}

		private bool Consistir()
		{


			if (!Information.IsNumeric(txtPeriodo.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(34) + "Período" + Strings.Chr(34) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, this.Text);
				return false;

			}


			//verifica se o período já foi inserido
			int intI = 0;


			string strNovoPeriodo = Strings.Trim(txtPeriodo.Text);
			string strNovoTipo = ObtemTipoDaMedia();


			for (intI = 0; intI <= lstPeriodoSelecionado.Items.Count - 1; intI++) {
				if ((ItemSelecionado != null) && ItemSelecionado.Index == intI) {
					continue;
				}


				if (strNovoPeriodo == Strings.Trim(lstPeriodoSelecionado.Items[intI].Text) && strNovoTipo == Strings.Trim(lstPeriodoSelecionado.Items[intI].SubItems[1].Text)) {
					Interaction.MsgBox("A Média Móvel " + strNovoTipo + " de " + strNovoPeriodo + " já foi inserida.", MsgBoxStyle.Exclamation, this.Text);

					return false;

				}

			}

			return true;

		}


		private void btnAdicionar_Click(System.Object sender, System.EventArgs e)
		{

			if (Consistir()) {
				string strNovoTipo = ObtemTipoDaMedia();


				if (ItemSelecionado == null) {
					ListViewItem objListViewItem = lstPeriodoSelecionado.Items.Add(txtPeriodo.Text);

					objListViewItem.SubItems.Add(strNovoTipo);

					//seta esta propriedade para false para permitir que apenas uma coluna tenha
					//a propriedade backcolor alterada.
					//Se a propriedade for true, não permite alterar a propriedade BACKCOLOR e outras
					//propriedades dos subitems.
					objListViewItem.UseItemStyleForSubItems = false;

					objListViewItem.SubItems.Add("").BackColor = pnlCor.BackColor;

				} else {
					ItemSelecionado.SubItems[0].Text = txtPeriodo.Text;
					ItemSelecionado.SubItems[1].Text = strNovoTipo;
					ItemSelecionado.SubItems[2].BackColor = pnlCor.BackColor;
				}



			}

		}


		private void pnlCor_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try {
				objColorDialog.ShowDialog();
				pnlCor.BackColor = objColorDialog.Color;
			} catch (Exception ex) {
				//tem que ter sempre um exception para quando o usuário cancelar a tela
			}

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{
			//LIMPA A COLLECTION
			//colStructIndicadorEscolha.Clear()
			lstMediasSelecionadas.Clear();

			//ADICIONA OS NOVOS ITENS NA COLLECTION CASO EXISTAM ITENS NO LISTVIEW
			int intI = 0;

			//Dim objStructIndicadorEscolha As structIndicadorEscolha


			for (intI = 0; intI <= lstPeriodoSelecionado.Items.Count - 1; intI++) {
				lstMediasSelecionadas.Add(new cMediaDTO((lstPeriodoSelecionado.Items[intI].SubItems[1].Text == "Exponencial" ? "E" : "A"), Convert.ToInt32(lstPeriodoSelecionado.Items[intI].Text), "VALOR", lstPeriodoSelecionado.Items[intI].SubItems[2].BackColor));

			}

			this.DialogResult = System.Windows.Forms.DialogResult.OK;

		}

		private void lstPeriodoSelecionado_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			ListView listView = (ListView)sender;
			if (listView.SelectedItems.Count == 0) {
				return;
			}
			ListViewItem selectedItem = listView.SelectedItems[0];
			txtPeriodo.Text = selectedItem.Text;
			string strTipo = selectedItem.SubItems[1].Text;

			if (strTipo == "Exponencial") {
				rdbExponencial.Checked = true;
			} else if (strTipo == "Aritmética") {
				rdbAritmetica.Checked = true;
			}

			pnlCor.BackColor = selectedItem.SubItems[2].BackColor;

			ItemSelecionado = selectedItem;
			btnAdicionar.Text = "Atualizar";
			ToolTipCancelar.Active = true;

		}

		private void frmIndicadorEscolha_KeyUp(System.Object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode.Equals(Keys.Escape)) {
				LimparItemSelecionado();
			}
		}

		private void LimparItemSelecionado()
		{
			ItemSelecionado = null;
			btnAdicionar.Text = "Adicionar";
			ToolTipCancelar.Active = false;

		}
	}
}