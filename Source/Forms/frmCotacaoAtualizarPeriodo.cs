using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using prmCotacao;
using DataBase;
using frwInterface;
using prjDTO;
using System.Threading;
using prjModelo.Regras;

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

		private enum enumAtualizacaoDiariaTipo
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


		private void CotacaoDiariaAtualizar(string pstrCodigo, System.DateTime pdtmDataInicial, System.DateTime pdtmDataFinal, bool pblnCalcularDados, enumAtualizacaoDiariaTipo intTipo)
		{
			AlterCursorDoMouseCallback callBack = new AlterCursorDoMouseCallback(AlterCursorDoMouse);

			BeginInvoke(callBack,  new object[]{Cursors.WaitCursor} );

		    cCotacao objCotacao = new cCotacao(objConexao);

			if (intTipo == enumAtualizacaoDiariaTipo.Online) {
				objCotacao.CotacaoPeriodoAtualizar(pdtmDataInicial, pdtmDataFinal, pstrCodigo, pblnCalcularDados);


			} else if (intTipo == enumAtualizacaoDiariaTipo.Historica) {
				objCotacao.CotacaoHistoricaPeriodoAtualizar(pdtmDataInicial, pdtmDataFinal, pblnCalcularDados);


			} else if (intTipo == enumAtualizacaoDiariaTipo.IntraDay) {
				objCotacao.CotacaoIntraDayAtualizar(pdtmDataInicial, pblnCalcularDados);

			}

			BeginInvoke(callBack, new object[] { Cursors.Default });

		}


		private void btnOK_Click(object sender, EventArgs e)
		{
			cCotacao objCotacao = null;

			cEnum.enumRetorno intRetorno = default(cEnum.enumRetorno);



			if (rdbAtualizacaoDiaria.Checked) {
				System.DateTime dtmDataInicial = default(System.DateTime);
				System.DateTime dtmDataFinal = default(System.DateTime);

				//verificação dos campos obrigatórios para a atualização diária.


				if (!Information.IsDate(txtDataInicial.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Data Inicial" + Strings.Chr(34) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, Text);

					return;

				}


				if (!Information.IsDate(txtDataFinal.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Data Final" + Strings.Chr(34) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, Text);

					return;

				}

				dtmDataInicial = mCotacao.DataFormatoConverter(txtDataInicial.Text);

				dtmDataFinal = mCotacao.DataFormatoConverter(txtDataFinal.Text);


				if (dtmDataInicial > dtmDataFinal) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Data Inicial" + Strings.Chr(34) + " não pode ter uma data superior ao campo " + Strings.Chr(34) + "Data Final" + Strings.Chr(34) + ".", MsgBoxStyle.Exclamation, Text);

					return;

				}


				if (rdbIntraday.Checked) {

					if (dtmDataInicial != dtmDataFinal) {
						Interaction.MsgBox("Para atualizações intraday as datas inicial e final devem ser as mesmas.", MsgBoxStyle.Exclamation, Text);

						return;

					}

					cCalculadorData objCalculadorData = new cCalculadorData(objConexao);

					//verifica se a data é um dia útil. Se não for, não permite a execução

					if (!objCalculadorData.DiaUtilVerificar(Convert.ToDateTime(txtDataInicial.Text))) {
						Interaction.MsgBox("Data não é um dia útil. Não é possível atualizar as cotações.", MsgBoxStyle.Exclamation, Text);

						//objCotacao = Nothing

						return;

					}

				}

				//Me.Cursor = Cursors.WaitCursor

				enumAtualizacaoDiariaTipo intAtualizacaoTipo = default(enumAtualizacaoDiariaTipo);

				if (rdbOnline.Checked) {
					intAtualizacaoTipo = enumAtualizacaoDiariaTipo.Online;
				} else if (rdbHistorica.Checked) {
					intAtualizacaoTipo = enumAtualizacaoDiariaTipo.Historica;
				} else if (rdbIntraday.Checked) {
					intAtualizacaoTipo = enumAtualizacaoDiariaTipo.IntraDay;
				}

				bool blnCalcularDados = chkCalcularDados.Checked;
				string strCodigo = mCotacao.cmbAtivoCodigoRetornar(cmbAtivo);

                //Thread thread = new Thread((ThreadStart)CotacaoDiariaAtualizar(strCodigo, dtmDataInicial, dtmDataFinal, blnCalcularDados, intAtualizacaoTipo));

                var thread = new Thread(() => CotacaoDiariaAtualizar(strCodigo, dtmDataInicial, dtmDataFinal, blnCalcularDados, intAtualizacaoTipo));

				thread.IsBackground = true;

				thread.Start();

			//Me.Cursor = Cursors.Default


			} else {
				//verificação dos campos obrigatórios para a atualização anual

				if (!Information.IsNumeric(txtAno.Text)) {
					Interaction.MsgBox("Campo " + Strings.Chr(34) + "Ano" + Strings.Chr(34) + " não preenchido ou com valor inválido.", MsgBoxStyle.Exclamation, Text);

					return;

				}

				Cursor = Cursors.WaitCursor;

				objCotacao = new cCotacao(objConexao);

				intRetorno = objCotacao.CotacaoHistoricaAnoAtualizar(Convert.ToInt32(txtAno.Text));

				Cursor = Cursors.Default;

				//RetornoOK - Operação realizada com sucesso.
				//RetornoErroInesperado - algum erro inesperado de banco de dados ou de programação
				//RetornoErro2 - Já existe cotação no ano.
				//RetornoErro3 - Não foi possível descompactar o arquivo zip ou abrir o arquivo txt.

				switch (intRetorno) {

					case cEnum.enumRetorno.RetornoOK:

						Interaction.MsgBox("Operação realizada com sucesso.", MsgBoxStyle.Information, Text);

						break;
					case cEnum.enumRetorno.RetornoErroInesperado:

						Interaction.MsgBox("Ocorreram erros ao executar a operação.", MsgBoxStyle.Exclamation, Text);

						break;
					case cEnum.enumRetorno.RetornoErro2:

						Interaction.MsgBox("Já existe cotação para o ano " + txtAno.Text + ".", MsgBoxStyle.Exclamation, Text);

						break;
					case cEnum.enumRetorno.RetornoErro3:

						Interaction.MsgBox("Não foi possível descompactar o arquivo zip ou abrir o arquivo texto com as cotações.", MsgBoxStyle.Exclamation, Text);

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
			mCotacao.cmbAtivoPreencher(cmbAtivo, objConexao, false);

			cCalculadorData objCalculadorData = new cCalculadorData(objConexao);

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
			if (Information.IsDate(txtDataInicial.Text)) {
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
			if (Information.IsDate(txtDataFinal.Text)) {
				CalendarioFinal.SetDate(Convert.ToDateTime(txtDataFinal.Text));
			}

			CalendarioFinal.Show();
			CalendarioFinal.BringToFront();

		}
	}
}
