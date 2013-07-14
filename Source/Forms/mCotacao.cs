using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataBase;
using Microsoft.VisualBasic;
using prjModelo.Entidades;
using TraderWizard.Infra.Repositorio;

namespace Forms
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

				if ((DateAndTime.Weekday(dtmData) > 1) && (DateAndTime.Weekday(dtmData) < 7)) {
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


		public static void ComboAtivoPreencher(ComboBox pcmbAtivo, cConexao pobjConexao, string codigoDoAtivoParaSelecionar, bool pblnSelecionarItem)
		{
		    var ativos = new Ativos(pobjConexao);
		    IList<cAtivo> ativosValidos = ativos.Validos();

			pcmbAtivo.Items.Clear();

		    int intIndicePadrao = -1;

		    foreach (var ativo in ativosValidos )
		    {
		        pcmbAtivo.Items.Add(new cAtivo(ativo.Codigo, ativo.Codigo + " - " + ativo.Descricao));

		        if (pblnSelecionarItem && ativo.Codigo == codigoDoAtivoParaSelecionar)
		        {
		            intIndicePadrao = pcmbAtivo.Items.Count - 1;
		        }
		    }

			if (pblnSelecionarItem) {

			    if (intIndicePadrao == -1)
			    {
			        intIndicePadrao = 0;
			    }
				pcmbAtivo.SelectedIndex = intIndicePadrao;

			}

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
