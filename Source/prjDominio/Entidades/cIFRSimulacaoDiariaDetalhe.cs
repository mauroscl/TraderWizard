using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.DomainServices;
using prjModelo.Regras;
using prjModelo.ValueObjects;
using prjModelo.VOBuilders;

namespace prjModelo.Entidades
{

	public class cIFRSimulacaoDiariaDetalhe : cModelo
	{


		private readonly cConexao objConexao;
		private readonly cIFRSobrevendido objIFRSobrevendido;
		private readonly byte bytNumTentativas;
	    //Private blnGerouEntrada As Boolean 'Indica se gerou entrada pelos filtros adicionais
	    public UInt32 AgrupadorDeTentativas { get; set; }

		private readonly cIFRSimulacaoDiaria objIFRSimulacaoDiaria;
		/// <summary>
		/// Construtor utilizado quando é necessário criar um objeto para representar um registro já persistido
		/// </summary>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="pbytNumTentativas"></param>
		/// <param name="pblnMelhorEntrada"></param>
		/// <param name="pintSomatorioCriterios"></param>
		/// <param name="pintAgrupadorDeTentativas"></param>
		/// <param name="pobjIFRSimulacaoDiaria"></param>
		/// <remarks></remarks>

		public cIFRSimulacaoDiariaDetalhe(cIFRSobrevendido pobjIFRSobrevendido, byte pbytNumTentativas, bool pblnMelhorEntrada, int pintSomatorioCriterios, UInt32 pintAgrupadorDeTentativas, cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{

			objIFRSobrevendido = pobjIFRSobrevendido;
			bytNumTentativas = pbytNumTentativas;
			AgrupadorDeTentativas = pintAgrupadorDeTentativas;

			MelhorEntrada = pblnMelhorEntrada;
			//blnGerouEntrada = pblnGerouEntrada
			SomatorioCriterios = pintSomatorioCriterios;

			objIFRSimulacaoDiaria = pobjIFRSimulacaoDiaria;

		}

		/// <summary>
		/// Construtor utilizado para quando é calculado um novo detalhe. O próprio construtor chama os cálculos necessários
		/// </summary>
		/// <param name="pobjConexao"></param>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="pobjIFRSimulacaoDiaria"></param>
		/// <remarks></remarks>

		public cIFRSimulacaoDiariaDetalhe(cConexao pobjConexao, cIFRSobrevendido pobjIFRSobrevendido, cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{
			objConexao = pobjConexao;

			objIFRSobrevendido = pobjIFRSobrevendido;

			objIFRSimulacaoDiaria = pobjIFRSimulacaoDiaria;

			cCalculadorDeTentativas objCalculadorTentativas = new cCalculadorDeTentativas();

			var objTentativaVO = objCalculadorTentativas.Calcular(objIFRSimulacaoDiaria, objIFRSobrevendido);

			AgrupadorDeTentativas = (uint) objTentativaVO.AgrupadorDeTentativas;
			bytNumTentativas = objTentativaVO.NumTentativas;

			CalcularMelhorEntrada(objTentativaVO.GerouNovoAgrupadorDeTentativas);

			VerificarSeDeveGerarEntrada();


		}


		public cIFRSobrevendido IFRSobreVendido {
			get { return objIFRSobrevendido; }
		}

		public byte NumTentativas {
			get { return bytNumTentativas; }
		}

	    public bool MelhorEntrada { get; private set; }

	    public bool GerouEntrada {
//Return blnGerouEntrada
			get { return (SomatorioCriterios == 0); }
		}

	    public int SomatorioCriterios { get; private set; }

	    public void AlterarMelhorEntrada(bool pblnValor)
		{
			MelhorEntrada = pblnValor;
		}

		public cIFRSimulacaoDiaria IFRSimulacaoDiaria {
			get { return objIFRSimulacaoDiaria; }
		}

		private void CalcularMelhorEntrada(bool pblnGerouNovoAgrupadorDeTentativas)
		{
			cCalculadorMelhorEntrada objCalcularMelhorEntrada = new cCalculadorMelhorEntrada(objConexao);
			MelhorEntrada = objCalcularMelhorEntrada.Calcular(this, pblnGerouNovoAgrupadorDeTentativas);
		}


		public void VerificarSeDeveGerarEntrada()
		{
			cSimulacaoDiariaVOBuilder objSimulacaoDiariaVOBuilder = new cSimulacaoDiariaVOBuilder();
			var objSimulacaoDiariaVO = objSimulacaoDiariaVOBuilder.Build(this);

			cValorCriterioClassifMediaVOBuilder objValorCriterioClassifMediaVOBuilder = new cValorCriterioClassifMediaVOBuilder();
			var objValorCriterioClassifMediaVO = objValorCriterioClassifMediaVOBuilder.Build(IFRSimulacaoDiaria);

			cVerificaSeDeveGerarEntrada objVerifica = new cVerificaSeDeveGerarEntrada(objConexao);
			SomatorioCriterios = objVerifica.Verificar(objSimulacaoDiariaVO, objValorCriterioClassifMediaVO, null);
			//blnGerouEntrada = (intSomatorioCriterios = 0)
		}

	}

}
