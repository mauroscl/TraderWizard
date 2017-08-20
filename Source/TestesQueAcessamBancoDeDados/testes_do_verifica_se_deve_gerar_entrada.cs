using System;
using System.Linq;
using DataBase.Carregadores;
using Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjDominio.VOBuilders;
using ServicoNegocio;
using TesteBase;
using TraderWizard.Enumeracoes;

namespace TestProject1
{


	///<summary>
	///This is a test class for cCalculadorDataInicialDetalheTest and is intended
	///to contain all cCalculadorDataInicialDetalheTest Unit Tests
	///</summary>
	[TestClass()]
	public class testes_do_verifica_se_deve_gerar_entrada : Inicializacao
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

		public void QuandoDeveGerarEntradaTemQueRetornarSomatorioDeCriteriosIgualAZero()
		{
			CarregadorSimulacaoIFRDiario objCarregadorSimulacaoDiaria = new CarregadorSimulacaoIFRDiario(objConexao);
			IFRSimulacaoDiaria objSimulacao = objCarregadorSimulacaoDiaria.CarregaPorDataEntradaEfetiva(FuncoesGerais.RetornaAtivo("RENT3"), FuncoesGerais.CarregarSetup(cEnum.enumSetup.IFRSemFiltroRP), new DateTime(2011, 5, 23));

			var objIFRSobrevendido = FuncoesGerais.CarregaIFRSobrevendido(objConexao, 1);
			var objDetalhe = objSimulacao.Detalhes.Where(d => d.IFRSobreVendido.Equals(objIFRSobrevendido)).Single();

			SimulacaoDiariaVOBuilder objSimulacaoDiariaVOBuilder = new SimulacaoDiariaVOBuilder();
			var objSimulacaoDiariaVO = objSimulacaoDiariaVOBuilder.Build(objDetalhe);

			ValorCriterioClassifMediaVOBuilder objValorCriterioClassifMediaVOBuilder = new ValorCriterioClassifMediaVOBuilder();
			var objValorCriterioClassifMediaVO = objValorCriterioClassifMediaVOBuilder.Build(objDetalhe.IFRSimulacaoDiaria);

			var objVerifica = new VerificaSeDeveGerarEntrada(objConexao);

			var intRetorno = objVerifica.Verificar(objSimulacaoDiariaVO, objValorCriterioClassifMediaVO, null);

			Assert.IsTrue(intRetorno == 0);

		}


	}
}
