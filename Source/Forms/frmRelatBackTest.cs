using Forms;
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
using TraderWizard.Enumeracoes;

namespace TraderWizard
{

	public partial class frmRelatBackTest
	{

		//indica o indice da linha selecionada

		private int intLinhaSelecionada;

		private cConexao objConexao;

		public frmRelatBackTest(cConexao pobjConexao)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

			mCotacao.ComboAtivoPreencher(cmbAtivo, objConexao,"", true);

		}

		private void btnExcluir_Click(System.Object sender, System.EventArgs e)
		{

			if (lstSetup.SelectedItems.Count == 0) {
				Interaction.MsgBox("Selecione uma linha antes de excluir.", MsgBoxStyle.Information, this.Text);


			} else {
				//o comando está em loop, mas se houver linhas selecionadas, haverá
				//apenas uma linha, pois não é permitido selecionar mais do que uma
				//linha no grid. Pode ocorrer de não haver nenhuma linha selecionada.

				ListViewItem objItem = null;


				foreach (ListViewItem objItem_loopVariable in lstSetup.SelectedItems) {
					objItem = objItem_loopVariable;
					lstSetup.Items.Remove(objItem);

				}

			}

		}

		private bool SetupRepetidoVerificar(string psrCodigoSetup)
		{

			//verifica se o setup já foi adicionado no grid.
			int intI = 0;


			for (intI = 0; intI <= lstSetup.Items.Count - 1; intI++) {
				//sempre desconsidera a linha selecionada em caso de edição.
				//quando for adicão, a variável intLinhaSelecionada estará com valor = -1

				if (intI != intLinhaSelecionada) {

					if (lstSetup.Items[intI].Text == psrCodigoSetup) {
						Interaction.MsgBox("Este setup já foi adicionado.", MsgBoxStyle.Information, this.Text);

						return false;

					}

				}

			}

			return true;

		}

		private string RealizacaoParcialTipoDescricaoGerar(cEnum.enumRealizacaoParcialTipo pintRealizacaoParcialTipo)
		{

			switch (pintRealizacaoParcialTipo) {

				case cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial:


					return "Sem Realização Parcial";
				case cEnum.enumRealizacaoParcialTipo.AlijamentoRisco:


					return "Alijamento de Risco";
				case cEnum.enumRealizacaoParcialTipo.PercentualFixo:


					return "Percentual Fixo";
				case cEnum.enumRealizacaoParcialTipo.PrimeiroLucro:


					return "Fechamento c/ Perc. Mínimo";
				default:


					return String.Empty;
			}

		}


		/// <summary>
		/// Atualiza o grid de setups incluindo um novo setup ou atualizando a linha do setup que foi editado
		/// </summary>
		/// <param name="pstrOperacao">valores possiveis: ADICIONAR, EDITAR</param>
		/// <param name="pstructBackTestSetup">estrutura contendo todos os atributos do SETUP</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool GridAtualizar(string pstrOperacao, cEstrutura.structBackTestSetup pstructBackTestSetup)
		{

			Collection colSubItems = null;
			string strCamposAux = String.Empty;

			ListViewItem objListViewItem = null;


			if (pstrOperacao == "ADICIONAR") {

				if (!SetupRepetidoVerificar(pstructBackTestSetup.strCodigoSetup)) {
					return false;

				}

				//código do setup é adicionado como item. Demais campos são considerados subitems

				objListViewItem = lstSetup.Items.Add(pstructBackTestSetup.strCodigoSetup);

				objListViewItem.SubItems.Add(mCotacao.SetupDescricaoGerar(pstructBackTestSetup.strCodigoSetup));

				colSubItems = new Collection();


				if (Strings.Mid(pstructBackTestSetup.strCodigoSetup, 1, 4) == "IFR2") {
					//COLUNA "FILTRAR MME 49"

					if (pstructBackTestSetup.blnMME49Filtrar) {
						colSubItems.Add("SIM");


					} else {
						colSubItems.Add("NÃO");

					}

					//COLUNA "Valor Máximo IFR Sobrevendido"

					if (pstructBackTestSetup.strCodigoSetup == "IFR2SOBREVEND") {
						colSubItems.Add(pstructBackTestSetup.dblIFR2SobrevendidoValorMaximo.ToString());


					} else {
						//NÃO APLICADO
						colSubItems.Add("N/A");

					}


				} else {
					//caso o setup não seja de IFR as duas primeiras colunas não se aplicam
					colSubItems.Add("N/A");
					colSubItems.Add("N/A");

				}

				//INSERE O CÓDIGO DO TIPO DE REALIZAÇÃO PARCIAL, DE ACORDO COM O ENUM
				colSubItems.Add(Convert.ToInt32(pstructBackTestSetup.intRealizacaoParcialTipo).ToString());

				string strRealizacaoParcialDescricao = null;

				strRealizacaoParcialDescricao = RealizacaoParcialTipoDescricaoGerar(pstructBackTestSetup.intRealizacaoParcialTipo);

				colSubItems.Add(strRealizacaoParcialDescricao);


				if (pstructBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial || pstructBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.AlijamentoRisco) {
					//se não houver realização parcial, ou for alijamento de risco não preenche as colunas de percentual
					//fixo ou percentual mínimo
					colSubItems.Add("N/A");
					colSubItems.Add("N/A");


				} else if (pstructBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PercentualFixo) {
					colSubItems.Add(pstructBackTestSetup.decPercentualRealizacaoParcialFixo.ToString());
					colSubItems.Add("N/A");


				} else if (pstructBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PrimeiroLucro) {
					colSubItems.Add("N/A");
					colSubItems.Add(pstructBackTestSetup.decPrimeiroLucroPercentualMinimo.ToString());

				}

				//percorre collection adicionando todos os campos no grid

				foreach (string strCamposAux_loopVariable in colSubItems) {
					strCamposAux = strCamposAux_loopVariable;
					objListViewItem.SubItems.Add(strCamposAux);

				}
			} else if (pstrOperacao == "EDITAR") {

				if (!SetupRepetidoVerificar(pstructBackTestSetup.strCodigoSetup)) {
					return false;

				}

				int intIndice = 1;

				objListViewItem = lstSetup.Items[intLinhaSelecionada];

				objListViewItem.Text = pstructBackTestSetup.strCodigoSetup;

				objListViewItem.SubItems[intIndice].Text = mCotacao.SetupDescricaoGerar(pstructBackTestSetup.strCodigoSetup);

				intIndice = intIndice + 1;


				if (Strings.Mid(pstructBackTestSetup.strCodigoSetup, 1, 4) == "IFR2") {
					objListViewItem.SubItems[intIndice].Text = (pstructBackTestSetup.blnMME49Filtrar ? "SIM" : "NÃO");

					intIndice = intIndice + 1;


					if (pstructBackTestSetup.strCodigoSetup == "IFR2SOBREVEND") {
						objListViewItem.SubItems[intIndice].Text = pstructBackTestSetup.dblIFR2SobrevendidoValorMaximo.ToString();


					} else {
						objListViewItem.SubItems[intIndice].Text = "N/A";

					}

					intIndice = intIndice + 1;


				} else {
					objListViewItem.SubItems[intIndice].Text = "N/A";

					intIndice = intIndice + 1;

					objListViewItem.SubItems[intIndice].Text = "N/A";

					intIndice = intIndice + 1;

				}

				objListViewItem.SubItems[intIndice].Text = Convert.ToInt32(pstructBackTestSetup.intRealizacaoParcialTipo).ToString();

				intIndice = intIndice + 1;

				objListViewItem.SubItems[intIndice].Text = RealizacaoParcialTipoDescricaoGerar(pstructBackTestSetup.intRealizacaoParcialTipo);

				intIndice = intIndice + 1;

				switch (pstructBackTestSetup.intRealizacaoParcialTipo) {

					case cEnum.enumRealizacaoParcialTipo.SemRealizacaoParcial:
					case cEnum.enumRealizacaoParcialTipo.AlijamentoRisco:

						objListViewItem.SubItems[intIndice].Text = "N/A";

						intIndice = intIndice + 1;

						objListViewItem.SubItems[intIndice].Text = "N/A";

				        break;
					case cEnum.enumRealizacaoParcialTipo.PercentualFixo:

						objListViewItem.SubItems[intIndice].Text = pstructBackTestSetup.decPercentualRealizacaoParcialFixo.ToString();

						intIndice = intIndice + 1;

						objListViewItem.SubItems[intIndice].Text = "N/A";

				        break;
					case cEnum.enumRealizacaoParcialTipo.PrimeiroLucro:

						objListViewItem.SubItems[intIndice].Text = "N/A";

						intIndice = intIndice + 1;

						objListViewItem.SubItems[intIndice].Text = pstructBackTestSetup.decPrimeiroLucroPercentualMinimo.ToString();

				        break;
				}

			}

			return true;

		}

		private bool Consistir()
		{


			if (!Information.IsNumeric(txtNumAcoesLote.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Ações por Lote" + Strings.Chr(39) + " não preenchido ou com valor inválido");

				return false;

			}


			if (Strings.Trim(txtDataInicio.Text) != String.Empty) {

				if (!Information.IsDate(txtDataInicio.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(39) + "Data de Início" + Strings.Chr(39) + " com valor inválido.", MsgBoxStyle.Information, this.Text);

					return false;

				}

			}


			if (!Information.IsNumeric(txtCapitalIncial.Text)) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Capital Inicial" + Strings.Chr(39) + " não preenchido ou com valor inválido");

				return false;

			}


			if (Strings.Trim(txtDescricao.Text) == String.Empty) {
				Interaction.MsgBox("Campo " + Strings.Chr(39) + "Descrição" + " não preenchido.", MsgBoxStyle.Information, this.Text);

				return false;

			}

			return true;

		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (lstSetup.Items.Count > 0) {

				if (Consistir()) {
					cEstrutura.structBackTestSetup[] arrSetup = {
						
					};

					Array.Resize(ref arrSetup, lstSetup.Items.Count);

					int intI = 0;


					for (intI = 0; intI <= lstSetup.Items.Count - 1; intI++) {
						arrSetup[intI] = LinhaEstruturaTransformar(intI);

					}

					cRelatorio objRelatorio = new cRelatorio(objConexao);

					string strPeriodicidade = String.Empty;


					if (rdbDiario.Checked) {
						strPeriodicidade = "DIARIO";

					}


					if (rdbSemanal.Checked) {
						strPeriodicidade = "SEMANAL";

					}

					long lngCodRelatorio = 0;

					cEnum.enumMediaTipo intMediaTipo = default(cEnum.enumMediaTipo);

					if (rdbMediaAritmetica.Checked) {
						intMediaTipo = cEnum.enumMediaTipo.Aritmetica;
					} else if (rdbMediaExponencial.Checked) {
						intMediaTipo = cEnum.enumMediaTipo.Exponencial;
					}


				    DateTime dataInicio;
					if (objRelatorio.RelatBackTestOperacoesExecutar(mCotacao.ObterCodigoDoAtivoSelecionado(cmbAtivo), arrSetup, strPeriodicidade
                        , Convert.ToDecimal(txtCapitalIncial.Text), chkRealizacaoParcialPermitirDayTrade.Checked, txtDescricao.Text.Trim()
                        , intMediaTipo, Convert.ToInt32(txtNumAcoesLote.Text)
                        , DateTime.TryParse(txtDataInicio.Text, out dataInicio) ? dataInicio : cConst.DataInvalida, ref lngCodRelatorio)) {
						
                        MessageBox.Show("Relatório gerado com sucesso.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    } 
                    else
					{
					    MessageBox.Show("Ocorreram erros ao gerar o relatório.", this.Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);

					}

				}


			} else {
                MessageBox.Show("É necessário escolher pelo menos um setup.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

			}

		}


		private void btnAdicionar_Click(System.Object sender, System.EventArgs e)
		{
			frmRelatBackTestDetalhe objForm = new frmRelatBackTestDetalhe(this);

			//seta a variável para -1 para quando for salvar o item no grid 
			//não desconsiderar nenhuma linha na consistência que verifica se 
			//o setup já foi inserido, já que estamos inserindo um novo registro.
			intLinhaSelecionada = -1;

			objForm.ShowDialog();

		}


		private void btnAcima_Click(System.Object sender, System.EventArgs e)
		{

			if (lstSetup.SelectedItems.Count > 0) {
				int intIndex = lstSetup.SelectedItems[0].Index;
				ListViewItem objListViewItem = lstSetup.SelectedItems[0];


				if (objListViewItem.Index > 0) {
					//remove o item da posição atual
					lstSetup.Items.Remove(objListViewItem);

					//adiciona o item 1 indice antes
					lstSetup.Items.Insert(intIndex - 1, objListViewItem);

					//seleciona o item novamente.
					lstSetup.Focus();
					lstSetup.Items[intIndex - 1].Selected = true;


				} else {
					Interaction.MsgBox("Item já está na posição superior máxima.", MsgBoxStyle.Information, this.Text);

					lstSetup.Focus();
					lstSetup.Items[intIndex].Selected = true;

				}


			} else {
				Interaction.MsgBox("É necessário selecionar um item.", MsgBoxStyle.Information, this.Text);

			}

		}


		private void btnAbaixo_Click(System.Object sender, System.EventArgs e)
		{


			if (lstSetup.SelectedItems.Count > 0) {
				int intIndex = lstSetup.SelectedItems[0].Index;

				ListViewItem objListViewItem = lstSetup.SelectedItems[0];


				if (objListViewItem.Index < lstSetup.Items.Count - 1) {
					//remove o item da posição atual
					lstSetup.Items.Remove(objListViewItem);

					//adiciona o item 1 indice antes
					lstSetup.Items.Insert(intIndex + 1, objListViewItem);

					//seleciona o item novamente.
					lstSetup.Focus();
					lstSetup.Items[intIndex + 1].Selected = true;


				} else {
					Interaction.MsgBox("Item já está na posição superior máxima.", MsgBoxStyle.Information, this.Text);
					lstSetup.Focus();
					lstSetup.Items[intIndex].Selected = true;

				}


			} else {
				Interaction.MsgBox("É necessário selecionar um item.", MsgBoxStyle.Information, this.Text);

			}

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}

		private cEstrutura.structBackTestSetup LinhaEstruturaTransformar(int pintIndex)
		{

			ListViewItem objListViewItem = lstSetup.Items[pintIndex];

			cEstrutura.structBackTestSetup objBackTestSetup = default(cEstrutura.structBackTestSetup);

			objBackTestSetup.strCodigoSetup = objListViewItem.Text;

			int intIndice = 2;


			if (Strings.Mid(objBackTestSetup.strCodigoSetup, 1, 4) == "IFR2") {
				objBackTestSetup.blnMME49Filtrar = (objListViewItem.SubItems[intIndice].Text == "SIM" ? true : false);

				intIndice = intIndice + 1;


				if (objBackTestSetup.strCodigoSetup == "IFR2SOBREVEND") {
					objBackTestSetup.dblIFR2SobrevendidoValorMaximo = Convert.ToDouble(objListViewItem.SubItems[intIndice].Text);

				}

				intIndice = intIndice + 1;


			} else {
				intIndice = intIndice + 3;

			}

			objBackTestSetup.intRealizacaoParcialTipo = (cEnum.enumRealizacaoParcialTipo) Enum.Parse(typeof(cEnum.enumRealizacaoParcialTipo), objListViewItem.SubItems[intIndice].Text);

			//soma 2 no índice porque o indice da descrição da realização parcial não é utilizado
			intIndice = intIndice + 2;


			if (objBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PercentualFixo) {
				objBackTestSetup.decPercentualRealizacaoParcialFixo = Convert.ToDecimal(objListViewItem.SubItems[intIndice].Text);

			}

			intIndice = intIndice + 1;


			if (objBackTestSetup.intRealizacaoParcialTipo == cEnum.enumRealizacaoParcialTipo.PrimeiroLucro) {
				objBackTestSetup.decPrimeiroLucroPercentualMinimo = Convert.ToDecimal(objListViewItem.SubItems[intIndice].Text);

			}

			return objBackTestSetup;

		}


		private void btnEditar_Click(System.Object sender, System.EventArgs e)
		{

			if (lstSetup.SelectedItems.Count > 0) {
				cEstrutura.structBackTestSetup objBackTestSetup = default(cEstrutura.structBackTestSetup);

				objBackTestSetup = LinhaEstruturaTransformar(lstSetup.SelectedItems[0].Index);

				intLinhaSelecionada = lstSetup.SelectedItems[0].Index;

				frmRelatBackTestDetalhe objForm = new frmRelatBackTestDetalhe(objBackTestSetup, this);

				objForm.ShowDialog();

				intLinhaSelecionada = -1;


			} else {
				Interaction.MsgBox("É necessário selecionar um item.", MsgBoxStyle.Information, this.Text);

			}


		}

	}
}
