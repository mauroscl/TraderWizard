using System.Windows.Forms;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using DataBase;

namespace prjModelo.DomainServices
{

	public class cCalculadorIFRSimulacaoDiariaDetalhe
	{


		private readonly cConexao objConexao;
		public cCalculadorIFRSimulacaoDiariaDetalhe(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}


		public void CalcularDetalhes(cIFRSimulacaoDiaria pobjSimulacaoParaCalcular, IList<cIFRSobrevendido> plstIFRSobrevendido)
		{
			var lstParaCalcular = (from ifr in plstIFRSobrevendido where ifr.ValorMaximo >= pobjSimulacaoParaCalcular.ValorIFR select ifr).ToList();

			foreach (cIFRSobrevendido objIfrSobrevendido in lstParaCalcular) {
				CalcularDetalhe(pobjSimulacaoParaCalcular, objIfrSobrevendido);
			}

		}

		/// <summary>
		/// Calcula o número de tentativas e a melhor entrada todos os trades simulados de um determinado papel
		/// </summary>
		/// <param name="pobjIFRSobreVendido">objeto que contém o valor máximo do IFR Sobrevendido</param>
		/// <returns>status das inserções dos registros na tabela detalhe</returns>
		/// <remarks></remarks>
		private bool CalcularDetalhe(cIFRSimulacaoDiaria pobjSimulacaoParaCalcular, cIFRSobrevendido pobjIFRSobreVendido)
		{


			try {
				cIFRSimulacaoDiariaDetalhe objNovoDetalhe = new cIFRSimulacaoDiariaDetalhe(objConexao, pobjIFRSobreVendido, pobjSimulacaoParaCalcular);

				pobjSimulacaoParaCalcular.Detalhes.Add(objNovoDetalhe);

				return true;


			} catch (Exception ex) {
			    MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				return false;

			}

		}

	}
}
