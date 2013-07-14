using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using DataBase;
using frwInterface;
using prjDTO;
using prjModelo.Regras;
using prmCotacao;
namespace TraderWizard
{

	public partial class frmIFRSimulacaoDiaria
	{


		private readonly cConexao objConexao;

		public frmIFRSimulacaoDiaria(cConexao pobjConexao)
		{
			Load += frmIFRSimulacaoDiaria_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

		}


		private void btnCancelar_Click(System.Object sender, System.EventArgs e)
		{
			this.Close();

		}

		private bool CamposValidar()
		{

			//se não tem ativos selecionados

			if (lstAtivosEscolhidos.Items.Count == 0) {
				Interaction.MsgBox("Nenhum ativo foi escolhido.", MsgBoxStyle.Exclamation, this.Text);

				return false;

			}

			return true;

		}


		private void btnOK_Click(System.Object sender, System.EventArgs e)
		{
			if (!CamposValidar()) {
				return;
			}

			cEnum.enumMediaTipo intMediaTipo = default(cEnum.enumMediaTipo);

			if (rdbMediaAritmetica.Checked) {
				intMediaTipo = cEnum.enumMediaTipo.Aritmetica;
			} else if (rdbMediaExponencial.Checked) {
				intMediaTipo = cEnum.enumMediaTipo.Exponencial;
			}

			cEnum.enumIFRTipo intIFRTipo = default(cEnum.enumIFRTipo);

			if (rdbComFiltro.Checked) {
				intIFRTipo = cEnum.enumIFRTipo.ComFiltro;
			} else if (rdbSemFiltro.Checked) {
				intIFRTipo = cEnum.enumIFRTipo.SemFiltro;
			}

			Cursor = Cursors.WaitCursor;

			cSetupIFR2SimularDTO objSetupIFR2SimularDTO = new cSetupIFR2SimularDTO();

			int intI = 0;
			string strCodigoAtivo = null;

			List<string> lstAtivos = new List<string>();

			objSetupIFR2SimularDTO.IFRTipo = intIFRTipo;
			objSetupIFR2SimularDTO.MediaTipo = intMediaTipo;
			objSetupIFR2SimularDTO.SubirStopApenasAposRealizacaoParcial = chkSubirStopApenasAposRealizacaoParcial.Checked;
			//objSetupIFR2SimularDTO.ValorMaximoIFRSobrevendido = dblValorIFRMaximoSobrevendido
			objSetupIFR2SimularDTO.ExcluirSimulacoesAnteriores = chkExcluirSimulacoesAnteriores.Checked;

			cRelatorio objRelatorio = new cRelatorio(objConexao);
			bool blnOK = false;


			for (intI = 0; intI <= lstAtivosEscolhidos.Items.Count - 1; intI += 4) {


				for (int intJ = intI; intJ <= intI + 3; intJ++) {

					if (intJ <= lstAtivosEscolhidos.Items.Count - 1) {
						strCodigoAtivo = mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo((string) lstAtivosEscolhidos.Items[intJ]);

						lstAtivos.Add(strCodigoAtivo);

					}

				}

				blnOK = objRelatorio.SimularIFRDiarioParaListaDeAtivos(lstAtivos, objSetupIFR2SimularDTO);

				if (!blnOK) {
					break; // TODO: might not be correct. Was : Exit For
				}

				lstAtivos.Clear();

			}

			blnOK = objRelatorio.SimularIFRDiarioParaListaDeAtivos(lstAtivos, objSetupIFR2SimularDTO);


			if (blnOK) {
				Interaction.MsgBox("Operação realizada com sucesso.", MsgBoxStyle.Information, Text);


			} else {
				Interaction.MsgBox("Ocorreram erros ao executar a operação.", MsgBoxStyle.Information, Text);

			}

			Cursor = Cursors.Default;

		}

		private void btnAdicionarTodos_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			//percorre a lista de ativos não escolhidos

			for (intI = 0; intI <= lstAtivosNaoEscolhidos.Items.Count - 1; intI++) {
				//adiciona o item na lista de ativos escolhidos
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.Items[intI]);

			}

			//remove todos os itens da lista de ativos não escolhidos
			lstAtivosNaoEscolhidos.Items.Clear();

		}


		private void btnRemoverTodos_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			//percorre a lista de ativos 

			for (intI = 0; intI <= lstAtivosEscolhidos.Items.Count - 1; intI++) {
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

			object objItem = null;


			for (intI = 0; intI <= lstAtivosNaoEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

			}


			foreach (object objItem_loopVariable in colItem) {
				objItem = objItem_loopVariable;
				lstAtivosNaoEscolhidos.Items.Remove(objItem);

			}

		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			int intI = 0;

			Collection colItem = new Collection();

			object objItem = null;


			for (intI = 0; intI <= lstAtivosEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosEscolhidos.SelectedItems[intI]);

			}


			foreach (object objItem_loopVariable in colItem) {
				objItem = objItem_loopVariable;
				lstAtivosEscolhidos.Items.Remove(objItem);

			}

		}


		private void ListAtivosPreencher()
		{
			cCalculadorData objCalculadorData = new cCalculadorData(objConexao);

			DateTime dtmDataUltimaCotacao = objCalculadorData.ObtemDataDaUltimaCotacao();

			cRelatorio objRelatorio = new cRelatorio(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			//busca os ativos da tabela ativo
		    string strSQL = " select Codigo, Codigo & ' - ' & Descricao as Descr " + Environment.NewLine;
			strSQL += " from Ativo A " + Environment.NewLine;
			strSQL += " WHERE EXISTS " + Environment.NewLine;
			strSQL += "(";
			strSQL += '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + " FROM Cotacao C " + Environment.NewLine;
			strSQL += '\t' + " WHERE A.Codigo = C.Codigo " + Environment.NewLine;
			strSQL += '\t' + " AND C.Data = " + FuncoesBd.CampoFormatar(dtmDataUltimaCotacao) + Environment.NewLine;
			strSQL += '\t' + " AND C.Sequencial >= 200 " + Environment.NewLine;
			strSQL += '\t' + " AND " + objRelatorio.FiltroVolumeFinanceiroGerar("C", "Cotacao", 1000000);
			strSQL += '\t' + " AND " + objRelatorio.FiltroVolumeNegociosGerar("C", "Cotacao", 100);

			strSQL += ")" + Environment.NewLine;
			strSQL += " AND NOT EXISTS " + Environment.NewLine;
			strSQL += "(" + Environment.NewLine;
			strSQL += '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + " FROM ATIVOS_DESCONSIDERADOS " + Environment.NewLine;
			strSQL += '\t' + " WHERE A.CODIGO = ATIVOS_DESCONSIDERADOS.CODIGO " + Environment.NewLine;
			strSQL += ")" + Environment.NewLine;

			strSQL += " AND EXISTS " + Environment.NewLine;
			strSQL += "(" + Environment.NewLine;
			strSQL += '\t' + " SELECT 1 " + Environment.NewLine;
			strSQL += '\t' + " FROM Media_Diaria MVT " + Environment.NewLine;
			strSQL += '\t' + " WHERE A.Codigo = MVT.Codigo " + Environment.NewLine;
			strSQL += '\t' + " AND MVT.Data = " + FuncoesBd.CampoFormatar(dtmDataUltimaCotacao) + Environment.NewLine;
			strSQL += '\t' + " AND MVT.NumPeriodos = 21 " + Environment.NewLine;
			strSQL += '\t' + " AND MVT.Tipo = " + FuncoesBd.CampoFormatar("VMA");
			strSQL += '\t' + " AND MVT.Valor >= 100000";
			//volume maior ou igual a 100.000 (cem mil) titulos negociados
			strSQL += ")" + Environment.NewLine;


			strSQL += " order by Codigo";

			lstAtivosNaoEscolhidos.Items.Clear();

			cRS objRS = new cRS();

			objRS.ExecuteQuery(strSQL);


			while (!objRS.EOF) {
				lstAtivosNaoEscolhidos.Items.Add(objRS.Field("Descr"));

				objRS.MoveNext();

			}

			objRS.Fechar();

			objRS.Conexao.FecharConexao();

		}

		private void frmIFRSimulacaoDiaria_Load(object sender, System.EventArgs e)
		{
			ListAtivosPreencher();
		}
	}
}
