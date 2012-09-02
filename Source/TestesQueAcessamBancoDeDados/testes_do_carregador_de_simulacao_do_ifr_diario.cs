using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frwInterface;
using prjModelo.Carregadores;
using DataBase;
using TesteBase;
namespace TestProject1
{


	///<summary>
	///This is a test class for cCalculadorDataInicialDetalheTest and is intended
	///to contain all cCalculadorDataInicialDetalheTest Unit Tests
	///</summary>
	[TestClass()]
	public class testes_do_carregador_de_simulacao_do_ifr_diario : Inicializacao
	{


		private TestContext testContextInstance;
		///<summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		#region "Additional test attributes"
		//
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//<ClassInitialize()> _
		//Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
		//End Sub
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//<ClassCleanup()>  _
		//Public Shared Sub MyClassCleanup()
		//End Sub
		//
		//Use TestInitialize to run code before running each test
		[TestInitialize()]
		public void MyTestInitialize()
		{
			Inicializa();
		}

		//Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup()
		{
			Finaliza();
		}
		//
		#endregion

		[TestMethod()]

		public void QuandoCarregarMelhorEntradaPorAgrupadorDeTentativasTemQueRetornarNothingSeNaoExisteMelhorEntradaParaEsseAgrupador()
		{
			var objAtivo = FuncoesGerais.RetornaAtivo("BBAS3");
			cCarregadorSimulacaoIFRDiario objCarregadorSimulacaoDiaria = new cCarregadorSimulacaoIFRDiario(objConexao);

			var objIFRSobrevendido = FuncoesGerais.CarregaIFRSobrevendido(objConexao, 1);
			var objSetup = FuncoesGerais.CarregarSetup(cEnum.enumSetup.IFRSemFiltroRP);
			var objSimulacao = objCarregadorSimulacaoDiaria.CarregarMelhorEntradaPorAgrupadorDeTentativas(objAtivo, objSetup, objIFRSobrevendido, 1);

			Assert.IsNull(objSimulacao);

		}

		//TODO: fazer teste para quando existir uma simulação de melhor entrada para o agrupador de tentativas    

		[TestMethod()]

		public void QuandoCarregarMelhorEntradaPorDataDeSaidaTemQueRetornarNothingSeNaoExisteMelhorEntradaParaEsseData()
		{
			var objAtivo = FuncoesGerais.RetornaAtivo("BBAS3");
			cCarregadorSimulacaoIFRDiario objCarregadorSimulacaoDiaria = new cCarregadorSimulacaoIFRDiario(objConexao);

			var objIFRSobrevendido = FuncoesGerais.CarregaIFRSobrevendido(objConexao, 1);
			var objSetup = FuncoesGerais.CarregarSetup(cEnum.enumSetup.IFRSemFiltroRP);
			var objSimulacao = objCarregadorSimulacaoDiaria.CarregarMelhorEntradaPorDataDeSaida(objAtivo, objSetup, objIFRSobrevendido, DateAndTime.Now);

			Assert.IsNull(objSimulacao);

		}

		//TODO: fazer teste para quando existir uma simulação de melhor entrada para uma data de saida.

		[TestMethod()]

		public void QuandoCarregarUltimaSimulacaoEOAtivoNaoPossuirSimulacoesTemQueRetornarNothing()
		{
			var objAtivo = FuncoesGerais.RetornaAtivo("BBAS3");
			cCarregadorSimulacaoIFRDiario objCarregadorSimulacaoDiaria = new cCarregadorSimulacaoIFRDiario(objConexao);

			var objIFRSobrevendido = FuncoesGerais.CarregaIFRSobrevendido(objConexao, 1);
			var objSetup = FuncoesGerais.CarregarSetup(cEnum.enumSetup.IFRSemFiltroRP);
			var objSimulacao = objCarregadorSimulacaoDiaria.CarregarUltimasSimulacoesPorIFRSobrevendido(objAtivo, objSetup, objIFRSobrevendido, new DateTime(2011, 10, 20));

			Assert.IsTrue(objSimulacao.Count() == 0);

		}

		//TODO: fazer teste para quando ativo possuir uma simulação


	}
}
