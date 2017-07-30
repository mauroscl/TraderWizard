using System.Windows.Forms;
using System;
using System.Data;
using DataBase;
using pWeb;
using prjConfiguracao;

namespace prmCotacao
{

	public class cInvestimento
	{

		private DataTable dtbCotacao;

		private readonly Conexao _conexao;
		public cInvestimento(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		public bool CotacoesBuscar()
		{
			bool functionReturnValue = false;

			cWeb objWeb = new cWeb(_conexao);

			DataSet dtsCotacao = new DataSet();

			const string strAtivo = "PETR3|PETR4|VALE3|VALE5|BBAS3";


			try {

				string strCaminhoPadrao = null;

                strCaminhoPadrao = cBuscarConfiguracao.ObtemCaminhoPadrao();

				objWeb.DownloadWithProxy("http://www.bmfbovespa.com.br/Pregao-Online/ExecutaAcaoAjax.asp?CodigoPapel=" + strAtivo, strCaminhoPadrao + "temp", "cotacao.xml");

				//dtsCotacao.ReadXml("http://www.bovespa.com.br/Mercado/RendaVariavel/InfoPregao/ExecutaAcaoAjax.asp?CodigoPapel=" & strAtivo)

				dtsCotacao.ReadXml(strCaminhoPadrao + "temp\\cotacao.xml");

				dtbCotacao = dtsCotacao.Tables[0];

				functionReturnValue = true;


			} catch (Exception) {
                MessageBox.Show("Não foi possível conectar no site www.bovespa.com.br.", "Atualizar Cotações",MessageBoxButtons.OK, MessageBoxIcon.Information);
				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		public bool FundoVALECalcular(int pintFormaCalculo, ref decimal pdecOscilacaoRet, ref decimal pdecVALE3MediaAtualRet, ref decimal pdecVALE5MediaAtualRet, ref decimal pdecVALE3MediaAnteriorRet, ref decimal pdecVALE5MediaAnteriorRet)
		{
			bool functionReturnValue;

			ServicoDeCotacao objCotacao = new ServicoDeCotacao(_conexao);


		    try {
				//medias da vale nas posições 2 e 3 do datatable
			    decimal decValorMedio;
			    pdecVALE3MediaAtualRet = decimal.TryParse((string) dtbCotacao.Rows[2]["Medio"], out decValorMedio) ? decValorMedio : 0;

				pdecVALE5MediaAtualRet = decimal.TryParse((string) dtbCotacao.Rows[3]["Medio"],out decValorMedio) ? Convert.ToDecimal(dtbCotacao.Rows[3]["Medio"]) : 0;

				pdecVALE3MediaAnteriorRet = objCotacao.UltimaMediaConsultar("VALE3");
				pdecVALE5MediaAnteriorRet = objCotacao.UltimaMediaConsultar("VALE5");



				if (pintFormaCalculo == 0) {
				    decimal decOscilacao;
				    var decVale3Oscilacao = decimal.TryParse((string) dtbCotacao.Rows[2]["Oscilacao"], out decOscilacao) ? decOscilacao : 0;

				    var decVale5Oscilacao = decimal.TryParse((string)dtbCotacao.Rows[3]["Oscilacao"],out decOscilacao) ? decOscilacao : 0;

					pdecOscilacaoRet = Math.Round(decVale3Oscilacao * 0.6948M + decVale5Oscilacao * 0.3052M, 3);


				} else {
					pdecOscilacaoRet = Math.Round((pdecVALE3MediaAtualRet / pdecVALE3MediaAnteriorRet - 1) * 69.48M + (pdecVALE5MediaAtualRet / pdecVALE5MediaAnteriorRet - 1) * 30.52M, 3);

				}

				functionReturnValue = true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message, "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Information);

				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a cotação do fundo de ações da Petrobras
		/// </summary>
		/// <param name="pintFormaCalculo">
		///     0 = calcula a oscilação pelo fechamento 
		///     1 = calcula a oscilação pela média
		///     </param>
		/// <param name="pdecOscilacaoRet"></param>
		/// <param name="pdecPETR3MediaAtualRet"></param>
		/// <param name="pdecPETR4MediaAtualRet"></param>
		/// <param name="pdecPETR3MediaAnteriorRet"></param>
		/// <param name="pdecPETR4MediaAnteriorRet"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool FundoPETROBRASCalcular(int pintFormaCalculo, ref decimal pdecOscilacaoRet, ref decimal pdecPETR3MediaAtualRet, ref decimal pdecPETR4MediaAtualRet, ref decimal pdecPETR3MediaAnteriorRet, ref decimal pdecPETR4MediaAnteriorRet)
		{
			bool functionReturnValue;

			ServicoDeCotacao objCotacao = new ServicoDeCotacao(_conexao);


		    try {
				//medias da PETROBRAS nas posições 0 e 1 do datatable
			    decimal decValorMedio;
			    if (decimal.TryParse((string)dtbCotacao.Rows[0]["Medio"], out decValorMedio))
                {
					pdecPETR3MediaAtualRet = decValorMedio;
				} else {
					pdecPETR3MediaAtualRet = 0;
				}

				if (decimal.TryParse((string)dtbCotacao.Rows[1]["Medio"], out decValorMedio)) {
					pdecPETR4MediaAtualRet = decValorMedio;
				} else {
					pdecPETR4MediaAtualRet = 0;
				}

				pdecPETR3MediaAnteriorRet = objCotacao.UltimaMediaConsultar("PETR3");
				pdecPETR4MediaAnteriorRet = objCotacao.UltimaMediaConsultar("PETR4");


				if (pintFormaCalculo == 0) {
					//forma de cálculo pelo fechamento

				    decimal decOscilacao;
				    decimal decPETR3Oscilacao = default(decimal);
				    if (decimal.TryParse((string) dtbCotacao.Rows[0]["Oscilacao"], out decOscilacao)) {
						decPETR3Oscilacao = decOscilacao;
					} else {
						decPETR3Oscilacao = 0;
					}

				    decimal decPETR4Oscilacao = default(decimal);
				    if (decimal.TryParse((string)dtbCotacao.Rows[1]["Oscilacao"], out decOscilacao)) {
						decPETR4Oscilacao = decOscilacao;
					} else {
						decPETR4Oscilacao = 0;
					}

					pdecOscilacaoRet = Math.Round(decPETR3Oscilacao * 0.7M + decPETR4Oscilacao * 0.3M, 3);


				} else {
					//forma de cálculo pela média


					pdecOscilacaoRet = Math.Round((pdecPETR3MediaAtualRet / pdecPETR3MediaAnteriorRet - 1) * 70 + (pdecPETR4MediaAtualRet / pdecPETR4MediaAnteriorRet - 1) * 30, 3);

				}

				functionReturnValue = true;


			} catch (Exception ex)
			{
			    MessageBox.Show(ex.Message, "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Information);


				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		public bool FundoBBCalcular(int pintFormaCalculo, ref decimal pdecOscilacaoRet, ref decimal pdecBBAS3MediaAtualRet, ref decimal pdecBBAS3MediaAnteriorRet)
		{
			bool functionReturnValue = false;

			ServicoDeCotacao objCotacao = new ServicoDeCotacao(_conexao);


			try {
				//medias do BB  na posição 4
			    decimal decValorMedio;
			    if (decimal.TryParse((string) dtbCotacao.Rows[4]["Medio"], out decValorMedio)) {
					pdecBBAS3MediaAtualRet = decValorMedio;
				} else {
					pdecBBAS3MediaAtualRet = 0;
				}

				pdecBBAS3MediaAnteriorRet = objCotacao.UltimaMediaConsultar("BBAS3");


				if (pintFormaCalculo == 0)
				{
				    decimal decOscilacao;
				    if (decimal.TryParse((string)dtbCotacao.Rows[4]["Oscilacao"],out decOscilacao)) {
						pdecOscilacaoRet = decOscilacao;
					} else {
						pdecOscilacaoRet = 0;
					}
				}
				else {
					pdecOscilacaoRet = Math.Round((pdecBBAS3MediaAtualRet / pdecBBAS3MediaAnteriorRet - 1) * 100, 3);

				}

				functionReturnValue = true;


			} catch (Exception ex)
			{
			    MessageBox.Show(ex.Message, "Atualizar Cotações", MessageBoxButtons.OK, MessageBoxIcon.Information);

				functionReturnValue = false;

			}
			return functionReturnValue;

		}

	}
}
