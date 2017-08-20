using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Configuracao;
using Cotacao;
using prmCotacao;
using DataBase;
using pWeb;
namespace TraderWizard
{

	public partial class frmCotacao
	{


		private readonly Conexao objConexao;

		private void GridCorAjustar()
		{
		    for (var intI = 0; intI <= DataGridView1.Rows.Count - 1; intI++) {
			    decimal decOscilacao;
			    if (!decimal.TryParse((string) DataGridView1.Rows[intI].Cells["Oscilacao"].Value, out decOscilacao)) {
					decOscilacao = 0;
				}

				if (decOscilacao < 0) {
					DataGridView1.Rows[intI].DefaultCellStyle.ForeColor = Color.Red;
				} else if (decOscilacao > 0) {
					DataGridView1.Rows[intI].DefaultCellStyle.ForeColor = Color.Green;
				} else {
					DataGridView1.Rows[intI].DefaultCellStyle.ForeColor = Color.Black;
				}

			}

		}


		private void GridAtualizar(string pstrURL)
		{
			cWeb objWeb = new cWeb(objConexao);

		    DataSet objDataSet = new DataSet();

			objDataSet.Clear();


			try {
				
				string strCaminhoPadrao = null;
                strCaminhoPadrao = BuscarConfiguracao.ObtemCaminhoPadrao();

				objWeb.DownloadWithProxy(pstrURL, strCaminhoPadrao + "temp", "cotacao.xml");

				//objDataSet.ReadXml(pstrURL)
				objDataSet.ReadXml(strCaminhoPadrao + "temp\\cotacao.xml");

				DataGridView1.DataSource = objDataSet;
				DataGridView1.DataMember = objDataSet.Tables[0].TableName;

				

				//Em cada uma das linhas de dados.

			    for (var intI = 0; intI <= DataGridView1.Rows.Count - 1; intI++) {
					//Para a quarta coluna (indice 3, pois é base zero), separa a hora data colocando um espaço
				    string value = (string) DataGridView1.Rows[intI].Cells[3].Value;
					DataGridView1.Rows[intI].Cells[3].Value = value.Substring(0,10) + " " + value.Substring(10);

				}

				//Ajusta o tipo das células da quarta coluna. Parece não estar funcionando. Não está interferindo no funcionamento do grid.
				DataGridView1.Columns[3].ValueType = System.Type.GetType("DateTime");


				for (var intI = 0; intI <= DataGridView1.Columns.Count - 1; intI++)
				{

				    int width;

					if (intI == 1) {
                        width = 150;
					} else if (intI == 2) {
                        width = 60;
					} else if (intI == 3) {
                        width = 120;
					} else {
                        width = 50;

					}

				    DataGridView1.Columns[intI].Width = width;

				}


				GridCorAjustar();



			} catch {
			    MessageBox.Show("Não foi possível conectar no site www.bmfbovespa.com.br.",this.Text,MessageBoxButtons.OK,MessageBoxIcon.Exclamation );

			}

		}




		private void Atualizar()
		{
		    string strAtivo = txtAtivo.Text.Trim().ToUpper();


			if (!string.IsNullOrEmpty(strAtivo))
			{
			    string strURL = "http://www.bmfbovespa.com.br/Pregao-Online/ExecutaAcaoAjax.asp?CodigoPapel=" + strAtivo;

			    GridAtualizar(strURL);

				//Else

				//    MsgBox("Nenhum ativo foi informado.", MsgBoxStyle.Information, Me.Text)
			}

		    FundoAtualizar();

		}

        private Color CalculaCorDaOscilacao(decimal pdecOscilacao)
        {
            if (pdecOscilacao < 0)
            {
                return Color.Red;
            }
            else if (pdecOscilacao > 0)
            {
                return Color.Green;
            }
            else
            {
                return Color.Black;
            }            
        }

		private void FundoAtualizar()
		{

			Investimento objInvestimento = new Investimento(objConexao);

			decimal decPETR3MediaAtual = default(decimal);
			decimal decPETR4MediaAtual = default(decimal);

			decimal decPETR3MediaAnterior = default(decimal);
			decimal decPETR4MediaAnterior = default(decimal);

			decimal decVALE3MediaAtual = default(decimal);
			decimal decVALE5MediaAtual = default(decimal);

			decimal decVALE3MediaAnterior = default(decimal);
			decimal decVALE5MediaAnterior = default(decimal);

			decimal decBBAS3MediaAtual = default(decimal);
			decimal decBBAS3MediaAnterior = default(decimal);

			decimal decOscilacao = default(decimal);


			objInvestimento.CotacoesBuscar();


			if (objInvestimento.FundoPETROBRASCalcular(Convert.ToInt32(cmbPetrobrasFormaCalculo.SelectedIndex), ref decOscilacao, ref decPETR3MediaAtual, ref decPETR4MediaAtual, ref decPETR3MediaAnterior, ref decPETR4MediaAnterior))
			{

			    txtPetrobrasOscilacao.ForeColor = CalculaCorDaOscilacao(decOscilacao);

				txtPETR3MediaAnterior.Text = String.Format("{0:0.000}",decPETR3MediaAnterior);
                txtPETR3MediaAtual.Text = String.Format("{0:0.000}", decPETR3MediaAtual);

                txtPETR4MediaAnterior.Text = String.Format("{0:0.000}", decPETR4MediaAnterior);
                txtPETR4MediaAtual.Text = String.Format("{0:0.000}", decPETR4MediaAtual);

                txtPetrobrasOscilacao.Text = String.Format("{0:0.00}", decOscilacao); 

			}

			//decOscilacao = Math.Round((decVALE3MediaAtual / CDbl(txtVALE3MediaAnterior.Text) - 1) * 69.48 + (decVALE5MediaAtual / CDbl(txtVALE5MediaAnterior.Text) - 1) * 30.52, 3)


			if (objInvestimento.FundoVALECalcular(Convert.ToInt32(cmbValeFormaCalculo.SelectedIndex), ref decOscilacao, ref decVALE3MediaAtual, ref decVALE5MediaAtual, ref decVALE3MediaAnterior, ref decVALE5MediaAnterior))
			{

			    txtValeOscilacao.ForeColor = CalculaCorDaOscilacao(decOscilacao);

                txtVALE3MediaAnterior.Text = String.Format("{0:0.000}", decVALE3MediaAnterior); 
                txtVALE3MediaAtual.Text = String.Format("{0:0.000}", decVALE3MediaAtual); ;

                txtVALE5MediaAnterior.Text = String.Format("{0:0.000}", decVALE3MediaAnterior);
                txtVALE5MediaAtual.Text = String.Format("{0:0.000}", decVALE5MediaAtual);

                txtValeOscilacao.Text = String.Format("{0:0.00}", decOscilacao); 

			}


			if (objInvestimento.FundoBBCalcular(Convert.ToInt32(cmbBBASFormaCalculo.SelectedIndex), ref decOscilacao, ref decBBAS3MediaAtual, ref decBBAS3MediaAnterior))
			{

			    txtBBAS3Oscilacao.ForeColor = CalculaCorDaOscilacao(decOscilacao);

                txtBBAS3MediaAnterior.Text = String.Format("{0:0.000}", decBBAS3MediaAnterior); 
                txtBBAS3MediaAtual.Text = String.Format("{0:0.0.000}", decBBAS3MediaAtual);

                txtBBAS3Oscilacao.Text = String.Format("{0:0.00}", decOscilacao); 

			}

		}


		private void DataGridView1_Sorted(object sender, System.EventArgs e)
		{
			GridCorAjustar();
		}

		public frmCotacao()
		{
			Shown += frmCotacao_Shown;
			Load += frmCotacao_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.

			//inicializa conexão com banco de dados
			objConexao = new Conexao();

		}


		private void frmCotacao_Load(object sender, System.EventArgs e)
		{
			this.Location = new Point(0, 0);
			this.Height = MdiParent.ClientSize.Height - 30;
			this.Width = MdiParent.ClientSize.Width - 4;

			grbVale.Top = this.Height - 277;
			grbPetrobras.Top = this.Height - 277;
			grbBancoBrasil.Top = this.Height - 155;

			DataGridView1.Height = this.Height - 345;

		}


		private void frmCotacao_Shown(object sender, System.EventArgs e)
		{
			ComboPreencher();

			//busca o parâmetro que contém a lista de ativos
			string strValor;

			mdlGeral.ParametroConsultar(objConexao, "CotacaoAtivos", out strValor);

			txtAtivo.Text = strValor;

			Atualizar();

			GridCorAjustar();

		}


		private void tsbCotacaoAtualizar_Click(System.Object sender, System.EventArgs e)
		{
			this.Cursor = Cursors.WaitCursor;

			Atualizar();

			this.Cursor = Cursors.Default;

		}


		private void ComboPreencher()
		{
			cmbPetrobrasFormaCalculo.Items.Clear();

			cmbPetrobrasFormaCalculo.Items.Add("Fechamento");
			cmbPetrobrasFormaCalculo.Items.Add("Média");

			cmbPetrobrasFormaCalculo.SelectedIndex = 0;

			cmbValeFormaCalculo.Items.Clear();

			cmbValeFormaCalculo.Items.Add("Fechamento");
			cmbValeFormaCalculo.Items.Add("Média");

			cmbValeFormaCalculo.SelectedIndex = 0;

			cmbBBASFormaCalculo.Items.Clear();

			cmbBBASFormaCalculo.Items.Add("Fechamento");
			cmbBBASFormaCalculo.Items.Add("Média");

			cmbBBASFormaCalculo.SelectedIndex = 0;

		}

	}
}


