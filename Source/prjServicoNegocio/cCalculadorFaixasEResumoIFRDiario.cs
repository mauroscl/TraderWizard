using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;
using prjModelo.ValueObjects;
namespace prjServicoNegocio
{

	public class cCalculadorFaixasEResumoIFRDiario
	{

		private readonly cConexao objConexao;
		private readonly cAtivo objAtivo;

		private readonly Setup objSetup;

		public cCalculadorFaixasEResumoIFRDiario(cConexao pobjConexao, cAtivo pobjAtivo, Setup pobjSetup)
		{
			objConexao = pobjConexao;
			objAtivo = pobjAtivo;
			objSetup = pobjSetup;
		}


		public void Calcular(cCalculoFaixaResumoVO pobjCalculoFaixaResumoVO, IList<cIFRSobrevendido> plstTodosIFRSobrevendido)
		{
			IList<cIFRSobrevendido> lstIFRSobrevendidoParaCalcular = plstTodosIFRSobrevendido.Where(x => pobjCalculoFaixaResumoVO.ValorMenorIFR <= x.ValorMaximo).ToList();

			cCalculadorFaixasIFRDiario objCalculadorFaixas = new cCalculadorFaixasIFRDiario(objConexao, objAtivo, objSetup);

			cCalculadorResumoIFRDiario objCalcularResumo = new cCalculadorResumoIFRDiario(objConexao, objSetup, objAtivo);


			foreach (cIFRSobrevendido objIfrSobrevendido in lstIFRSobrevendidoParaCalcular) {
				objCalculadorFaixas.CalcularFaixasParaUmaData(objIfrSobrevendido, pobjCalculoFaixaResumoVO);

				objCalcularResumo.Calcular(objIfrSobrevendido, pobjCalculoFaixaResumoVO);

			}


		}

	}
}
