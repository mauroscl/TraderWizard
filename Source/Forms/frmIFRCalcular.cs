using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using DataBase;
using prjServicoNegocio;
using prmCotacao;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;

namespace TraderWizard
{

	public partial class frmIFRCalcular
	{


		private readonly cConexao objConexao;

		public frmIFRCalcular(cConexao pobjConexao)
		{
			Load += frmIDiarioCalcular_Load;
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
			    MessageBox.Show("Nenhum ativo foi escolhido.", Text, MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
				return false;

			}


            if (txtPeriodo.Text.IsNumeric())
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

		    string lstAtivos = string.Empty;

		    var objCotacao = new ServicoDeCotacao(objConexao);


			foreach (object item in lstAtivosEscolhidos.Items) {
				string strItem = Convert.ToString(item);
				//strCodigoAtivo = Strings.Trim(Strings.Mid(strItem, 1, Strings.InStr(strItem, "-", CompareMethod.Text) - 1));
				string strCodigoAtivo = strItem.Substring(0,strItem.IndexOf('-')).Trim();

				lstAtivos += "#" + strCodigoAtivo;
			}

			lstAtivos += "#";

			IList<int> colPeriodos = new List<int>();
			colPeriodos.Add(Convert.ToInt32(txtPeriodo.Text));

			bool blnOK = objCotacao.IFRGeralCalcular(colPeriodos, cEnum.Periodicidade.Semanal,Constantes.DataInvalida , lstAtivos);

		    MessageBox.Show(blnOK ? "Operação realizada com sucesso." : "Ocorreram erros ao executar a operação.", Text,
		        MessageBoxButtons.OK, MessageBoxIcon.Information);

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

		private void frmIDiarioCalcular_Load(object sender, System.EventArgs e)
		{
			ListAtivosPreencher();
		}
	}
}
