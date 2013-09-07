using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjDominio.Entidades;
using prjDTO;
using prjModelo.Carregadores;
using TesteBase;
using TraderWizard.Enumeracoes;

namespace TestProject1
{

	///<summary>
	///This is a test class for cCalculadorDataInicialDetalheTest and is intended
	///to contain all cCalculadorDataInicialDetalheTest Unit Tests
	///</summary>
	[TestClass()]
	public class testes_do_carregador_de_cotacao_diaria : Inicializacao
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

		public void QuandoCarregarCotacoesPorPeriodoTemQueRetornarListaComAsCotacoesDeCadaUmDosDiasDoPeriodo()
		{
			cCarregadorCotacaoDiaria objCarregadorCotacaoDiaria = new cCarregadorCotacaoDiaria(objConexao);

			var lstCotacoes = objCarregadorCotacaoDiaria.CarregarPorPeriodo(new Ativo("CSNA3", string.Empty), new System.DateTime(2011, 1, 8), new System.DateTime(2011, 1, 13), "DATA ASC", new List<cMediaDTO>(), false);

			Assert.AreEqual(4, lstCotacoes.Count);

		}

		[TestMethod()]

		public void QuandoCarregarPorIFRSobrevendidoSemSimulacaoTemQueRetornarAListaDeAtivosQueAtendemEstesCriterios()
		{
			var objSetup = FuncoesGerais.CarregarSetup(cEnum.enumSetup.IFRSemFiltroRP);
			var objAtivo = FuncoesGerais.RetornaAtivo("BBAS3");

			cCarregadorCotacaoDiaria objCarregadorCotacao = new cCarregadorCotacaoDiaria(objConexao);

			var lstCotacoes = objCarregadorCotacao.CarregaComIFRSobrevendidoSemSimulacao(objAtivo, objSetup, 10, cEnum.enumMediaTipo.Exponencial);

			Assert.IsTrue(lstCotacoes.Count > 0);

		}

		[TestMethod()]

		public void QuandoCarregarCotacaoComMediasDeveRetornarUmaCotacaoComUmaListaDeMediasContendoAsMediasSolicitadas()
		{
			List<cMediaDTO> lstMediasParaCarregar = new List<cMediaDTO>();

			lstMediasParaCarregar.Add(new cMediaDTO("E", 21, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("E", 49, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("E", 200, "VALOR"));
			lstMediasParaCarregar.Add(new cMediaDTO("A", 13, "IFR2"));

			cCarregadorCotacaoDiaria objCarregadorDeCotacoes = new cCarregadorCotacaoDiaria(objConexao);

			var objAtivo = FuncoesGerais.RetornaAtivo("BBAS3");
			var objCotacao = objCarregadorDeCotacoes.CarregarPorPeriodo(objAtivo, new System.DateTime(2011, 8, 3), new System.DateTime(2011, 8, 3), string.Empty, lstMediasParaCarregar, false).Single();

			Assert.AreEqual(48.3343228670924, objCotacao.Medias.Single(x => x.Tipo == "IFR2" && x.NumPeriodos == 13).Valor);
			Assert.AreEqual(26.2214200591533, objCotacao.Medias.Single(x => x.Tipo == "MME" && x.NumPeriodos == 21).Valor);
			Assert.AreEqual(26.7531931064397, objCotacao.Medias.Single(x => x.Tipo == "MME" && x.NumPeriodos == 49).Valor);
			Assert.AreEqual(27.8102979102056, objCotacao.Medias.Single(x => x.Tipo == "MME" && x.NumPeriodos == 200).Valor);

		}

	}
}
