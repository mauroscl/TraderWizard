using System.Collections.Generic;
using System.Linq;
using DataBase;
using Dominio.Entidades;
using prjDominio.ValueObjects;
using prjServicoNegocio;

namespace ServicoNegocio
{

	public class CalculadorFaixasEResumoIFRDiario
	{

		private readonly Conexao objConexao;
		private readonly Ativo objAtivo;

		private readonly Setup objSetup;

		public CalculadorFaixasEResumoIFRDiario(Conexao pobjConexao, Ativo pobjAtivo, Setup pobjSetup)
		{
			objConexao = pobjConexao;
			objAtivo = pobjAtivo;
			objSetup = pobjSetup;
		}


		public void Calcular(CalculoFaixaResumoVO pobjCalculoFaixaResumoVO, IList<IFRSobrevendido> plstTodosIFRSobrevendido)
		{
			IList<IFRSobrevendido> lstIFRSobrevendidoParaCalcular = plstTodosIFRSobrevendido.Where(x => pobjCalculoFaixaResumoVO.ValorMenorIFR <= x.ValorMaximo).ToList();

			CalculadorFaixasIFRDiario objCalculadorFaixas = new CalculadorFaixasIFRDiario(objConexao, objAtivo, objSetup);

			CalculadorResumoIFRDiario objCalcularResumo = new CalculadorResumoIFRDiario(objConexao, objSetup, objAtivo);


			foreach (IFRSobrevendido objIfrSobrevendido in lstIFRSobrevendidoParaCalcular) {
				objCalculadorFaixas.CalcularFaixasParaUmaData(objIfrSobrevendido, pobjCalculoFaixaResumoVO);

				objCalcularResumo.Calcular(objIfrSobrevendido, pobjCalculoFaixaResumoVO);

			}


		}

	}
}
