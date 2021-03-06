using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DataBase;
using Dominio.Entidades;
using TraderWizard.Infra.Repositorio;

namespace Forms
{

	static class mCotacao
	{


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


		public static void ComboAtivoPreencher(ComboBox pcmbAtivo, Conexao pobjConexao, string codigoDoAtivoParaSelecionar, bool pblnSelecionarItem)
		{
		    var ativos = new Ativos(pobjConexao);
		    IList<Ativo> ativosValidos = ativos.Validos();

			pcmbAtivo.Items.Clear();

		    int intIndicePadrao = -1;

		    foreach (var ativo in ativosValidos )
		    {
		        pcmbAtivo.Items.Add(ativo);

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

		public static string ObterCodigoDoAtivoSelecionado(ComboBox pcmbAtivo)
		{
		    if (pcmbAtivo.Text == string.Empty) {
				return string.Empty;
			}
		    var objAtivo = (Ativo)pcmbAtivo.SelectedItem;
		    return objAtivo.Codigo;
		}


	}
}
