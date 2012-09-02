using Microsoft.VisualBasic;
using System;
using System.Windows.Forms;
using prjModelo.Entidades;
using DataBase;

namespace TraderWizard
{

	static class mCotacao
	{

		public static System.DateTime DiaUtilAnteriorCalcular(System.DateTime pdtmData)
		{
		    bool blnOK = false;

			DateTime dtmData = pdtmData;

			do {
				//busca o dia anterior
				dtmData = DateAndTime.DateAdd(DateInterval.Day, -1, dtmData);

				if ((DateAndTime.Weekday(dtmData) > 1) & (DateAndTime.Weekday(dtmData) < 7)) {
					blnOK = true;
				}

			} while (!(blnOK));

			return dtmData;

		}

		public static DateTime DataFormatoConverter(string pstrData)
		{
		    int intPosicaoInicial = 1;

			int intPosicaoBarra = Strings.InStr(intPosicaoInicial, pstrData, "/", CompareMethod.Text);

			string strDia = Strings.Mid(pstrData, intPosicaoInicial, intPosicaoBarra - intPosicaoInicial);

			intPosicaoInicial = intPosicaoBarra + 1;

			intPosicaoBarra = Strings.InStr(intPosicaoInicial, pstrData, "/", CompareMethod.Text);

			string strMes = Strings.Mid(pstrData, intPosicaoInicial, intPosicaoBarra - intPosicaoInicial);

			intPosicaoInicial = intPosicaoBarra + 1;

			string strAno = Strings.Mid(pstrData, intPosicaoInicial);

			return DateAndTime.DateSerial(Convert.ToInt32(strAno), Convert.ToInt32(strMes), Convert.ToInt32(strDia));

		}

		public static string SetupDescricaoGerar(string pstrCodigoSetup)
		{

			switch (pstrCodigoSetup) {

				case "MME9.1":


					return "MME 9.1";
				case "MME9.2":


					return "MME9.2";
				case "MME9.3":


					return "MME9.3";
				case "IFR2SOBREVEND":


					return "IFR 2 Sobrevendido";
				case "IFR2>MMA13":


					return "IFR 2 acima MMA 13";
				default:


					return String.Empty;
			}

		}


		public static void cmbAtivoPreencher(ComboBox pcmbAtivo, cConexao pobjConexao, bool pblnSelecionarItem)
		{
			cRS objRS = new cRS(pobjConexao);

			//busca os ativos da tabela ativo
			objRS.ExecuteQuery(" select Codigo, Descricao " + " from Ativo " + " WHERE NOT EXISTS " + "(" + " SELECT 1 " + " FROM ATIVOS_DESCONSIDERADOS " + " WHERE ATIVO.CODIGO = ATIVOS_DESCONSIDERADOS.CODIGO " + ")" + " order by Codigo");

			pcmbAtivo.Items.Clear();


			while (!objRS.EOF) {
				pcmbAtivo.Items.Add(new cAtivo(Convert.ToString(objRS.Field("Codigo")), Convert.ToString(objRS.Field("Descricao"))));

				objRS.MoveNext();

			}


			if (pblnSelecionarItem) {
				pcmbAtivo.SelectedIndex = 0;

			}

			objRS.Fechar();

		}

		public static string cmbAtivoCodigoRetornar(ComboBox pcmbAtivo)
		{
		    if (pcmbAtivo.Text == string.Empty) {
				return string.Empty;
			}
		    cAtivo objAtivo = (cAtivo)pcmbAtivo.SelectedItem;
		    return objAtivo.Codigo;
		}


	}
}
