using System.Collections.Generic;
using System.Linq;
using DataBase;
using Dominio.Entidades;
using Dominio.ValueObjects;
using prjDominio.ValueObjects;

namespace ServicoNegocio
{

	public class CalculadorFaixasEResumoIFRDiario
	{

		private readonly Conexao _conexao;
		private readonly Ativo _ativo;

		private readonly Setup _setup;

		public CalculadorFaixasEResumoIFRDiario(Conexao pobjConexao, Ativo pobjAtivo, Setup pobjSetup)
		{
			_conexao = pobjConexao;
			_ativo = pobjAtivo;
			_setup = pobjSetup;
		}


		public void Calcular(CalculoFaixaResumo pobjCalculoFaixaResumo, IList<IFRSobrevendido> plstTodosIFRSobrevendido)
		{
			IList<IFRSobrevendido> lstIFRSobrevendidoParaCalcular = plstTodosIFRSobrevendido.Where(x => pobjCalculoFaixaResumo.ValorMenorIFR <= x.ValorMaximo).ToList();

			CalculadorFaixasIFRDiario objCalculadorFaixas = new CalculadorFaixasIFRDiario(_conexao, _ativo, _setup);

			CalculadorResumoIFRDiario objCalcularResumo = new CalculadorResumoIFRDiario(_conexao, _setup, _ativo);


			foreach (IFRSobrevendido objIfrSobrevendido in lstIFRSobrevendidoParaCalcular) {
				objCalculadorFaixas.CalcularFaixasParaUmaData(objIfrSobrevendido, pobjCalculoFaixaResumo);

				objCalcularResumo.Calcular(objIfrSobrevendido, pobjCalculoFaixaResumo);

			}


		}

	}
}
