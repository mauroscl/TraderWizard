using System.Collections.Generic;
using System.Linq;
using DataBase;
using prjDominio.Entidades;
using prjDominio.ValueObjects;

namespace prjServicoNegocio
{

	public class cCalculadorFaixasEResumoIFRDiario
	{

		private readonly cConexao objConexao;
		private readonly Ativo objAtivo;

		private readonly Setup objSetup;

		public cCalculadorFaixasEResumoIFRDiario(cConexao pobjConexao, Ativo pobjAtivo, Setup pobjSetup)
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
