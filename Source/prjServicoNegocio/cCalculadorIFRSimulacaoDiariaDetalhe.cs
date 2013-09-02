using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DataBase;
using prjDominio.Entidades;
using prjModelo.Entidades;
using prjModelo.Regras;
using prjModelo.VOBuilders;

namespace prjServicoNegocio
{

	public class cCalculadorIFRSimulacaoDiariaDetalhe
	{

		private readonly cConexao _conexao;
	    private readonly ServicoDeCotacaoDeAtivo _servicoDeCotacaoDeAtivo;
		public cCalculadorIFRSimulacaoDiariaDetalhe(cConexao pobjConexao, ServicoDeCotacaoDeAtivo servicoDeCotacaoDeAtivo)
		{
		    _conexao = pobjConexao;
		    _servicoDeCotacaoDeAtivo = servicoDeCotacaoDeAtivo;
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
	    /// <param name="pobjSimulacaoParaCalcular"></param>
	    /// <param name="pobjIFRSobreVendido">objeto que contém o valor máximo do IFR Sobrevendido</param>
	    /// <returns>status das inserções dos registros na tabela detalhe</returns>
	    /// <remarks></remarks>
	    private void CalcularDetalhe(cIFRSimulacaoDiaria pobjSimulacaoParaCalcular, cIFRSobrevendido pobjIFRSobreVendido)
		{

			try {

                var objCalculadorTentativas = new cCalculadorDeTentativas(_servicoDeCotacaoDeAtivo);

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



        public int VerificarSeDeveGerarEntrada(cIFRSimulacaoDiaria simulacaoDiaria, cIFRSimulacaoDiariaDetalhe simulacaoDiariaDetalhe)
        {
            var objSimulacaoDiariaVOBuilder = new cSimulacaoDiariaVOBuilder();
            var objSimulacaoDiariaVO = objSimulacaoDiariaVOBuilder.Build(simulacaoDiariaDetalhe);

            var objValorCriterioClassifMediaVOBuilder = new cValorCriterioClassifMediaVOBuilder();
            var objValorCriterioClassifMediaVO = objValorCriterioClassifMediaVOBuilder.Build(simulacaoDiaria);

            var objVerifica = new cVerificaSeDeveGerarEntrada(_conexao);

            return objVerifica.Verificar(objSimulacaoDiariaVO, objValorCriterioClassifMediaVO, null);
            //blnGerouEntrada = (intSomatorioCriterios = 0)
        }


	}
}
