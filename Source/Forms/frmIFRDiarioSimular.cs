using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using DataBase;
using prjDTO;
using prjServicoNegocio;
using prmCotacao;
using TraderWizard.Enumeracoes;

namespace TraderWizard
{

	public partial class frmIFRSimulacaoDiaria
	{


		private readonly Conexao objConexao;

		public frmIFRSimulacaoDiaria(Conexao pobjConexao)
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
                MessageBox.Show("Nenhum ativo foi escolhido.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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

			var objSetupIFR2SimularDTO = new cSetupIFR2SimularDTO();

			int intI;

		    List<string> lstAtivos = new List<string>();

			objSetupIFR2SimularDTO.IFRTipo = intIFRTipo;
			objSetupIFR2SimularDTO.MediaTipo = intMediaTipo;
			objSetupIFR2SimularDTO.SubirStopApenasAposRealizacaoParcial = chkSubirStopApenasAposRealizacaoParcial.Checked;
			//objSetupIFR2SimularDTO.ValorMaximoIFRSobrevendido = dblValorIFRMaximoSobrevendido
			objSetupIFR2SimularDTO.ExcluirSimulacoesAnteriores = chkExcluirSimulacoesAnteriores.Checked;

			var objRelatorio = new cRelatorio(objConexao);
			bool blnOK;


			for (intI = 0; intI <= lstAtivosEscolhidos.Items.Count - 1; intI += 4) {


				for (int intJ = intI; intJ <= intI + 3; intJ++) {

					if (intJ <= lstAtivosEscolhidos.Items.Count - 1)
					{
					    string strCodigoAtivo = mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo((string) lstAtivosEscolhidos.Items[intJ]);

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
                MessageBox.Show("Operação realizada com sucesso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);


			} else {
                MessageBox.Show("Ocorreram erros ao executar a operação.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

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
			var colItem = new Collection<object>();


		    for (var intI = 0; intI <= lstAtivosNaoEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosEscolhidos.Items.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosNaoEscolhidos.SelectedItems[intI]);

			}


			foreach (object item in colItem)
			{
			    lstAtivosNaoEscolhidos.Items.Remove(item);
			}
		}


		private void btnRemover_Click(System.Object sender, System.EventArgs e)
		{
			int intI;

			var colItem = new Collection<object>();


		    for (intI = 0; intI <= lstAtivosEscolhidos.SelectedItems.Count - 1; intI++) {
				lstAtivosNaoEscolhidos.Items.Add(lstAtivosEscolhidos.SelectedItems[intI]);

				colItem.Add(lstAtivosEscolhidos.SelectedItems[intI]);

			}


			foreach (object item in colItem)
			{
			    lstAtivosEscolhidos.Items.Remove(item);
			}
		}


		private void ListAtivosPreencher()
		{
			var objCalculadorData = new cCalculadorData(objConexao);

			DateTime dtmDataUltimaCotacao = objCalculadorData.ObtemDataDaUltimaCotacao();

			var objRelatorio = new cRelatorio(objConexao);

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
