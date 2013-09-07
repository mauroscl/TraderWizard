using Forms;
using System;
using System.Windows.Forms;
using prjServicoNegocio;
using prmCotacao;
using DataBase;
using prjDTO;
using System.Threading;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;

namespace TraderWizard
{

	public partial class frmCotacaoAtualizarPeriodo
	{


		private readonly cConexao objConexao;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pobjConexao">
		/// conexão com o banco de dados
		/// </param>
		/// <remarks></remarks>
		/// 

		public frmCotacaoAtualizarPeriodo(cConexao pobjConexao)
		{
			Load += frmCotacaoAtualizarPeriodo_Load;
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// Add any initialization after the InitializeComponent() call.
			objConexao = pobjConexao;

		}

		private enum EnumAtualizacaoDiariaTipo
		{
			Online = 1,
			Historica = 2,
			IntraDay = 3
		}

		private void AlterCursorDoMouse(Cursor pNovoCursor)
		{
			Cursor = pNovoCursor;
		}

		public delegate void AlterCursorDoMouseCallback(Cursor pNovoCursor);


		private void CotacaoDiariaAtualizar(string pstrCodigo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, bool pblnCalcularDados, EnumAtualizacaoDiariaTipo intTipo)
		{
			AlterCursorDoMouseCallback callBack = new AlterCursorDoMouseCallback(AlterCursorDoMouse);

			BeginInvoke(callBack,  new object[]{Cursors.WaitCursor} );

		    ServicoDeCotacao objCotacao = new ServicoDeCotacao(objConexao);

			if (intTipo == EnumAtualizacaoDiariaTipo.Online) {
				objCotacao.CotacaoPeriodoAtualizar(pdtmDataInicial, pdtmDataFinal, pstrCodigo, pblnCalcularDados);


			} else if (intTipo == EnumAtualizacaoDiariaTipo.Historica) {
				objCotacao.CotacaoHistoricaPeriodoAtualizar(pdtmDataInicial, pdtmDataFinal, pblnCalcularDados);


			} else if (intTipo == EnumAtualizacaoDiariaTipo.IntraDay) {
				objCotacao.CotacaoIntraDayAtualizar(pdtmDataInicial, pblnCalcularDados);

			}

			BeginInvoke(callBack, new object[] { Cursors.Default });

		}


		private void btnOK_Click(object sender, EventArgs e)
		{
		    if (rdbAtualizacaoDiaria.Checked) {
			    //verificação dos campos obrigatórios para a atualização diária.


				if (!txtDataInicial.Text.IsDate()) {
                    MessageBox.Show("Campo \"Data Inicial\" não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

					return;

				}


				if (!txtDataFinal.Text.IsDate()) {
                    MessageBox.Show("Campo \"Data Final\" não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

					return;

				}

				DateTime dtmDataInicial = Convert.ToDateTime(txtDataInicial.Text);

                DateTime dtmDataFinal = Convert.ToDateTime(txtDataFinal.Text);


				if (dtmDataInicial > dtmDataFinal) {
					MessageBox.Show("Campo \"Data Inicial\" não pode ter uma data superior ao campo \"Data Final\".",
                        Text,MessageBoxButtons.OK, MessageBoxIcon.Information);

					return;

				}


				if (rdbIntraday.Checked) {

					if (dtmDataInicial != dtmDataFinal) {
                        MessageBox.Show("Para atualizações intraday as datas inicial e final devem ser as mesmas.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

						return;

					}

					var objCalculadorData = new cCalculadorData(objConexao);

					//verifica se a data é um dia útil. Se não for, não permite a execução

					if (!objCalculadorData.DiaUtilVerificar(Convert.ToDateTime(txtDataInicial.Text))) {

                        MessageBox.Show("Data não é um dia útil. Não é possível atualizar as cotações.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

						return;

					}

				}

				//Me.Cursor = Cursors.WaitCursor

				EnumAtualizacaoDiariaTipo intAtualizacaoTipo = default(EnumAtualizacaoDiariaTipo);

				if (rdbOnline.Checked) {
					intAtualizacaoTipo = EnumAtualizacaoDiariaTipo.Online;
				} else if (rdbHistorica.Checked) {
					intAtualizacaoTipo = EnumAtualizacaoDiariaTipo.Historica;
				} else if (rdbIntraday.Checked) {
					intAtualizacaoTipo = EnumAtualizacaoDiariaTipo.IntraDay;
				}

				bool blnCalcularDados = chkCalcularDados.Checked;
				string strCodigo = mCotacao.ObterCodigoDoAtivoSelecionado(cmbAtivo);

                //Thread thread = new Thread((ThreadStart)CotacaoDiariaAtualizar(strCodigo, dtmDataInicial, dtmDataFinal, blnCalcularDados, intAtualizacaoTipo));

                var thread = new Thread(() => CotacaoDiariaAtualizar(strCodigo, dtmDataInicial, dtmDataFinal, blnCalcularDados, intAtualizacaoTipo));

				thread.IsBackground = true;

				thread.Start();

			//Me.Cursor = Cursors.Default


			} else {
				//verificação dos campos obrigatórios para a atualização anual

				if (txtAno.Text.IsNumeric()) {
                    MessageBox.Show("Campo \"Ano\" não preenchido ou com valor inválido.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

					return;

				}

				Cursor = Cursors.WaitCursor;

				var objCotacao = new ServicoDeCotacao(objConexao);

				cEnum.enumRetorno intRetorno = objCotacao.CotacaoHistoricaAnoAtualizar(Convert.ToInt32(txtAno.Text));

				Cursor = Cursors.Default;

				//RetornoOK - Operação realizada com sucesso.
				//RetornoErroInesperado - algum erro inesperado de banco de dados ou de programação
				//RetornoErro2 - Já existe cotação no ano.
				//RetornoErro3 - Não foi possível descompactar o arquivo zip ou abrir o arquivo txt.

				switch (intRetorno) {

					case cEnum.enumRetorno.RetornoOK:

                        MessageBox.Show("Operação realizada com sucesso.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

						break;
					case cEnum.enumRetorno.RetornoErroInesperado:

                        MessageBox.Show("Ocorreram erros ao executar a operação.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

						break;
					case cEnum.enumRetorno.RetornoErro2:

                        MessageBox.Show("Já existe cotação para o ano " + txtAno.Text + ".", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

						break;
					case cEnum.enumRetorno.RetornoErro3:

                        MessageBox.Show("Não foi possível descompactar o arquivo zip ou abrir o arquivo texto com as cotações.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

						break;
				}

			}


		}

		private void btnCancelar_Click(System.Object sender, EventArgs e)
		{
			Close();
		}

		private void rdbAtualizacaoDiaria_CheckedChanged(System.Object sender, EventArgs e)
		{
			pnlAtualizacaoDiaria.Enabled = rdbAtualizacaoDiaria.Checked;
		}

		private void rdbAtualizacaoAnual_CheckedChanged(System.Object sender, EventArgs e)
		{
			pnlAtualizacaoAnual.Enabled = rdbAtualizacaoAnual.Checked;
		}


		private void frmCotacaoAtualizarPeriodo_Load(object sender, EventArgs e)
		{
			mCotacao.ComboAtivoPreencher(cmbAtivo, objConexao,"", false);

			var objCalculadorData = new cCalculadorData(objConexao);

			cSugerirAtualizacaoCotacaoDTO objRetornoDTO = objCalculadorData.SugerirAtualizarCotacao();


			if ((objRetornoDTO != null)) {
				txtDataInicial.Text = objRetornoDTO.DataInicial.ToString("dd/MM/yyyy");
				txtDataFinal.Text = objRetornoDTO.DataFinal.ToString("dd/MM/yyyy");

				if (objRetornoDTO.Tipo == "online") {
					rdbOnline.Checked = true;
				} else {
					rdbIntraday.Checked = true;
				}

			}

		}

		private void rdbOnline_CheckedChanged(System.Object sender, EventArgs e)
		{
			lblAtivo.Visible = rdbOnline.Checked;
			cmbAtivo.Visible = rdbOnline.Checked;
		}


		private void btnCalendarioInicial_Click(System.Object sender, EventArgs e)
		{
			if (txtDataInicial.Text.IsDate()) {
				CalendarioInicial.SetDate(Convert.ToDateTime(txtDataInicial.Text));
			}

			CalendarioInicial.Show();
		}


		private void CalendarioInicial_DateSelected(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			txtDataInicial.Text = CalendarioInicial.SelectionRange.Start.ToString("dd/MM/yyyy");
			CalendarioInicial.Hide();

		}

		private void CalendarioFinal_DateSelected(System.Object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			txtDataFinal.Text = CalendarioFinal.SelectionRange.Start.ToString("dd/MM/yyyy");
			CalendarioFinal.Hide();
		}


		private void btnCalendarioFinal_Click(System.Object sender, EventArgs e)
		{
			if (txtDataFinal.Text.IsDate()) {
				CalendarioFinal.SetDate(Convert.ToDateTime(txtDataFinal.Text));
			}

			CalendarioFinal.Show();
			CalendarioFinal.BringToFront();

		}
	}
}
