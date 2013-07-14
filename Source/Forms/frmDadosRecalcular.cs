using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using DataBase;
using prjModelo.Entidades;
using prmCotacao;
using TraderWizard.Infra.Repositorio;
using System.Linq;

namespace TraderWizard
{

	public partial class frmDadosRecalcular
	{


		private readonly cConexao _conexao;

		public frmDadosRecalcular(cConexao pobjConexao)
		{
			Load += frmDadosRecalcular_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			_conexao = pobjConexao;

		}

		private void chkCotacaoDiaria_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			pnlCotacaoDiariaOpcoes.Enabled = chkCotacaoDiaria.Checked;
		    foreach (var control in pnlCotacaoDiariaOpcoes.Controls)
		    {
		        ((CheckBox) control).Checked = chkCotacaoDiaria.Checked;
		    }
		}

		private void CotacaoSemanal_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			pnlCotacaoSemanalOpcoes.Enabled = chkCotacaoSemanal.Checked;
		    foreach (var control in pnlCotacaoSemanalOpcoes.Controls)
		    {
		        ((CheckBox) control).Checked = chkCotacaoSemanal.Checked;
		    }
		}

		private void chkDataInicialUtilizar_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			txtDataInicial.Enabled = chkDataInicialUtilizar.Checked;
		}

		private void rdbAtivosEscolher_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			pnlAtivosEscolher.Enabled = rdbAtivosEscolher.Checked;
		}


		private void ListAtivosPreencher()
		{

		    var ativos = new Ativos(_conexao);

		    IList<cAtivo> ativosValidos = ativos.Validos();

			lstAtivosNaoEscolhidos.Items.Clear();

		    foreach (var ativosValido in ativosValidos)
		    {
                lstAtivosNaoEscolhidos.Items.Add(ativosValido.Codigo + " - " + ativosValido.Descricao);
		    }

		}

		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void frmDadosRecalcular_Load(object sender, System.EventArgs e)
		{
			ListAtivosPreencher();
		}


		private void btnAdicionarTodos_Click(System.Object sender, System.EventArgs e)
		{

			for (var intI = 0; intI <= lstAtivosNaoEscolhidos.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos escolhidos
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.Items[intI]);

			}

			//remove todos os itens da lista de ativos não escolhidos
			lstAtivosNaoEscolhidos.Items.Clear();

		}


		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{

			for (var intI = 0; intI <= lstAtivosEscolhidos.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos não escolhidos
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.Items[intI]);

			}

			//remove todos os itens da lista de ativos escolhidos
			lstAtivosEscolhidos.Items.Clear();

		}


		private void btnAdicionar_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			Collection colItem = new Collection();


		    for (intI = 0; intI <= lstAtivosNaoEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

			}


			foreach (object item in colItem)
			{
			    object objItem = item;
			    lstAtivosNaoEscolhidos.Items.Remove(objItem);
			}
		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			Collection colItem = new Collection();


			for (var intI = 0; intI <= lstAtivosEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosEscolhidos.SelectedItems[intI]);

			}

			foreach (object item in colItem) {
				lstAtivosEscolhidos.Items.Remove(item);

			}

		}

		/// <summary>
		/// Faz todas as consistências da operação
		/// </summary>
		/// <returns>
		/// TRUE - Se todas as consistências estiverem corretas.
		/// FALSE - Se alguma consistência falhar.
		/// </returns>
		/// <remarks></remarks>
		private bool Consistir()
		{

			//indica se há operações para executar.
			bool blnOperacoesExecutar = false;


			if (chkCotacaoDiaria.Checked) {
				//se vai ter operações 
				blnOperacoesExecutar = (chkCotacaoDiariaOscilacaoRecalcular.Checked || chkCotacaoDiariaIFRRecalcular.Checked || chkCotacaoDiariaMMExpRecalcular.Checked || chkCotacaoDiariaVolumeMedioRecalcular.Checked || chkCotacaoDiariaIFR2MedioRecalcular.Checked);

			}


			if (!blnOperacoesExecutar) {

				if (chkCotacaoSemanal.Checked) {
					blnOperacoesExecutar = (chkCotacaoSemanalDadosGeraisRecalcular.Checked || chkCotacaoSemanalIFRRecalcular.Checked || chkCotacaoSemanalMMExpRecalcular.Checked || chkCotacaoSemanalVolumeMedioRecalcular.Checked || chkCotacaoSemanalIFR2MedioRecalcular.Checked);

				}

			}


			if (!blnOperacoesExecutar) {
				//se não tem operações para executar emite mensagem  e retorna false
				Interaction.MsgBox("Não foram selecionadas operações para serem executadas.", MsgBoxStyle.Information, this.Text);

				return false;

			}


			if (chkDataInicialUtilizar.Checked) {

				if (!Information.IsDate(txtDataInicial.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Data Inicial" + Strings.Chr(34) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, this.Text);

					return false;

				}

			}


			if (rdbAtivosEscolher.Checked) {
				//se não tem ativos selecionados

				if (lstAtivosEscolhidos.Items.Count == 0) {
					Interaction.MsgBox("Nenhum ativo foi escolhido.", MsgBoxStyle.Exclamation, this.Text);

					return false;

				}

			}

			return true;

		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{

			if (Consistir()) {

				if (Interaction.MsgBox("Recalcular dados das cotações. Confirma a execução da operação?", MsgBoxStyle.YesNo, this.Text) != MsgBoxResult.Yes) {
					return;

				}

				string strAtivos = String.Empty;


			    if (rdbAtivosEscolher.Checked) {

					for (var intI = 0; intI <= lstAtivosEscolhidos.Items.Count - 1; intI++)
					{
					    string strCodigoAtivo = mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo( (string)lstAtivosEscolhidos.Items[intI]);

					    strAtivos = strAtivos + "#" + strCodigoAtivo;
					}


				    if (strAtivos != String.Empty) {
						strAtivos = strAtivos + "#";

					}

				}

				System.DateTime dtmDataInicial = frwInterface.cConst.DataInvalida;


				if (chkDataInicialUtilizar.Checked) {
					dtmDataInicial = Convert.ToDateTime(txtDataInicial.Text);

				}

				cCotacao objCotacao = new cCotacao(_conexao);

				bool blnCotacaoDiariaOscilacaoRecalcular = false;
				bool blnCotacaoDiariaIFRRecalcular = false;
				bool blnCotacaoDiariaMMExpRecalcular = false;
				bool blnCotacaoDiariaVolumeMedioRecalcular = false;
				bool blnCotacaoDiariaIFRMedioRecalcular = false;

				bool blnCotacaoSemanalDadosGeraisRecalcular = false;
				bool blnCotacaoSemanalIFRRecalcular = false;
				bool blnCotacaoSemanalMMExpRecalcular = false;
				bool blnCotacaoSemanalVolumeMedioRecalcular = false;
				bool blnCotacaoSemanalIFRMedioRecalcular = false;


				if (chkCotacaoDiaria.Checked) {
					blnCotacaoDiariaOscilacaoRecalcular = chkCotacaoDiariaOscilacaoRecalcular.Checked;
					blnCotacaoDiariaIFRRecalcular = chkCotacaoDiariaIFRRecalcular.Checked;
					blnCotacaoDiariaMMExpRecalcular = chkCotacaoDiariaMMExpRecalcular.Checked;
					blnCotacaoDiariaVolumeMedioRecalcular = chkCotacaoDiariaVolumeMedioRecalcular.Checked;
					blnCotacaoDiariaIFRMedioRecalcular = chkCotacaoDiariaIFR2MedioRecalcular.Checked;

				}


				if (chkCotacaoSemanal.Checked) {
					blnCotacaoSemanalDadosGeraisRecalcular = chkCotacaoSemanalDadosGeraisRecalcular.Checked;
					blnCotacaoSemanalIFRRecalcular = chkCotacaoSemanalIFRRecalcular.Checked;
					blnCotacaoSemanalMMExpRecalcular = chkCotacaoSemanalMMExpRecalcular.Checked;
					blnCotacaoSemanalVolumeMedioRecalcular = chkCotacaoSemanalVolumeMedioRecalcular.Checked;
					blnCotacaoSemanalIFRMedioRecalcular = chkCotacaoSemanalIFR2MedioRecalcular.Checked;

				}

				this.Cursor = Cursors.WaitCursor;

				if (objCotacao.DadosRecalcular(blnCotacaoDiariaOscilacaoRecalcular, blnCotacaoDiariaOscilacaoRecalcular, blnCotacaoDiariaIFRRecalcular, blnCotacaoDiariaMMExpRecalcular, blnCotacaoDiariaVolumeMedioRecalcular, blnCotacaoDiariaIFRMedioRecalcular, blnCotacaoSemanalDadosGeraisRecalcular, blnCotacaoSemanalIFRRecalcular, blnCotacaoSemanalMMExpRecalcular, blnCotacaoSemanalVolumeMedioRecalcular,

				blnCotacaoSemanalIFRMedioRecalcular, dtmDataInicial, strAtivos)) {
					Interaction.MsgBox("Operação executada com sucesso.", MsgBoxStyle.Information, this.Text);


				} else {
					Interaction.MsgBox("Ocorreram alguns erros ao executar a operação. Veja o log de erros.", MsgBoxStyle.Exclamation, this.Text);

				}

				this.Cursor = Cursors.Default;

			}

		}

	}
}
