using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataBase;
using Dominio.Entidades;
using prjDominio.VOBuilders;
using prjServicoNegocio;
using Services;

namespace ServicoNegocio
{

	public class CalculadorIFRSimulacaoDiariaDetalhe
	{

		private readonly Conexao _conexao;
	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;
		public CalculadorIFRSimulacaoDiariaDetalhe(Conexao pobjConexao, ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
		{
		    _conexao = pobjConexao;
		    _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
		}


	    public void CalcularDetalhes(IFRSimulacaoDiaria pobjSimulacaoParaCalcular, IList<IFRSobrevendido> plstIFRSobrevendido)
		{
			var lstParaCalcular = (from ifr in plstIFRSobrevendido where ifr.ValorMaximo >= pobjSimulacaoParaCalcular.ValorIFR select ifr).ToList();

			foreach (IFRSobrevendido objIfrSobrevendido in lstParaCalcular) {
				CalcularDetalhe(pobjSimulacaoParaCalcular, objIfrSobrevendido);
			}

		}

	    /// <summary>
	    /// Calcula o número de tentativas e a melhor entrada todos os trades simulados de um determinado papel
	    /// </summary>
	    /// <param name="pobjSimulacaoParaCalcular"></param>
	    /// <param name="pobjIFRSobreVendido">objeto que contém o valor máximo do IFR Sobrevendido</param>
	    /// <returns>status das inserções dos registros na tabela detalhe</returns>
	    /// <remarks></remarks>
	    private void CalcularDetalhe(IFRSimulacaoDiaria pobjSimulacaoParaCalcular, IFRSobrevendido pobjIFRSobreVendido)
		{

			try {

                var objCalculadorTentativas = new CalculadorDeTentativas(_servicoDeCotacaoDeAtivo);

                var objTentativaVO = objCalculadorTentativas.Calcular(pobjSimulacaoParaCalcular, pobjIFRSobreVendido);

                var objCalcularMelhorEntrada = new cCalculadorMelhorEntrada(_conexao,_servicoDeCotacaoDeAtivo);

                var objNovoDetalhe = new cIFRSimulacaoDiariaDetalhe(pobjIFRSobreVendido, objTentativaVO.NumTentativas, 
			        objTentativaVO.AgrupadorDeTentativas, pobjSimulacaoParaCalcular);

			    objCalcularMelhorEntrada.Calcular(objNovoDetalhe, objTentativaVO.GerouNovoAgrupadorDeTentativas);

			    int somatorioDeCriterios = VerificarSeDeveGerarEntrada(pobjSimulacaoParaCalcular, objNovoDetalhe);

                objNovoDetalhe.AlterarSomatorioDeCriterios(somatorioDeCriterios);

				pobjSimulacaoParaCalcular.Detalhes.Add(objNovoDetalhe);

			} catch (Exception ex)
			{
			    MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}



        public int VerificarSeDeveGerarEntrada(IFRSimulacaoDiaria simulacaoDiaria, cIFRSimulacaoDiariaDetalhe simulacaoDiariaDetalhe)
        {
            var objSimulacaoDiariaVOBuilder = new SimulacaoDiariaVOBuilder();
            var objSimulacaoDiariaVO = objSimulacaoDiariaVOBuilder.Build(simulacaoDiariaDetalhe);

            var objValorCriterioClassifMediaVOBuilder = new ValorCriterioClassifMediaVOBuilder();
            var objValorCriterioClassifMediaVO = objValorCriterioClassifMediaVOBuilder.Build(simulacaoDiaria);

            var objVerifica = new VerificaSeDeveGerarEntrada(_conexao);

            return objVerifica.Verificar(objSimulacaoDiariaVO, objValorCriterioClassifMediaVO, null);
            //blnGerouEntrada = (intSomatorioCriterios = 0)
        }


	}
}
