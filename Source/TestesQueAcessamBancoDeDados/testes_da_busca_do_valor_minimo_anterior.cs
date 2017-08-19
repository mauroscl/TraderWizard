using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Carregadores;
using Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using prjModelo.Carregadores;
using prjServicoNegocio;
using Services;
using ServicoNegocio;
using TesteBase;
namespace TestProject1
{


	///<summary>
	///This is a test class for cCalculadorDataInicialDetalheTest and is intended
	///to contain all cCalculadorDataInicialDetalheTest Unit Tests
	///</summary>
	[TestClass()]
	public class testes_da_busca_do_valor_minimo_anterior : Inicializacao
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

		public void QuandoNaoTiverSplitEntreOsCandlesTemQueRetornarValorMinimoDoPrimeiroCandleAnteriorComValorMinimoMenorDoQueOValorMinimoDaDataPassadaPorParametro()
		{
			var objCarregadorCotacao = new CarregadorCotacaoDiaria(objConexao);

		    var ativo = new Ativo("CSNA3", "");
			var objCotacao = objCarregadorCotacao.CarregarPorPeriodo(ativo, new System.DateTime(2011, 5, 13), new System.DateTime(2011, 5, 13), string.Empty, new List<MediaDTO>(), false).Single();

			//TODO: ver o que fazer com o valor 22.09. Talvez tenha que chamar o método do setup que calcula o stop loss.
		    var servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(ativo, objConexao);
		    var buscaValorMinimoAnterior = new BuscaCotacaoValorMinimoAnterior(servicoDeCotacaoDeAtivo);
            var objCotacaoDoValorMinimoAnterior = buscaValorMinimoAnterior.Buscar(objCotacao);

			Assert.AreEqual(new decimal(22.01), objCotacaoDoValorMinimoAnterior.ValorMinimo);

		}

		[TestMethod()]

		public void QuandoTiverSplitEntreOsCandlesTemQueRetornarValorMinimoDoPrimeiroCandleAnteriorComValorMinimoMenorDoQueOValorMinimoDaDataPassadaPorParametroConvertidoParaOSplitQueFoiGerado()
		{
			var objCarregadorCotacao = new CarregadorCotacaoDiaria(objConexao);

		    var ativo = new Ativo("CSNA3", "");
			var objCotacao = objCarregadorCotacao.CarregarPorPeriodo(ativo, new DateTime(2010, 3, 26), new DateTime(2010, 3, 26), string.Empty, new List<MediaDTO>(), false).Single();

			//TODO: ver o que fazer com o valor 34.21. Talvez tenha que chamar o método do setup que calcula o stop loss.
            var servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(ativo, objConexao);
            var buscaValorMinimoAnterior = new BuscaCotacaoValorMinimoAnterior(servicoDeCotacaoDeAtivo);

            var objCotacaoDoValorMinimoAnterior = buscaValorMinimoAnterior.Buscar(objCotacao);

			Assert.AreEqual(new decimal(34.1), objCotacaoDoValorMinimoAnterior.ValorMinimo);

		}

		[TestMethod()]

		public void QuandoTiverMaisDeUmSplitNoMesmoDiaEntreOsCandlesTemQueRetornarValorMinimoDoPrimeiroCandleAnteriorComValorMinimoMenorDoQueOValorMinimoDaDataPassadaPorParametroConvertidoParaOSplitQueFoiGerado()
		{
			var objCarregadorCotacao = new CarregadorCotacaoDiaria(objConexao);

		    var ativo = new Ativo("ETER3", "");
		    var objCotacao = objCarregadorCotacao.CarregarPorPeriodo(ativo, new DateTime(2011, 5, 10), new DateTime(2011, 5, 10), string.Empty, new List<MediaDTO>(), false).Single();

			//TODO: ver o que fazer com o valor 10.36. Talvez tenha que chamar o método do setup que calcula o stop loss.
            var servicoDeCotacaoDeAtivo = new ServicoDeCotacaoDeAtivo(ativo, objConexao);
            var buscaValorMinimoAnterior = new BuscaCotacaoValorMinimoAnterior(servicoDeCotacaoDeAtivo);

            var objCotacaoDoValorMinimoAnterior = buscaValorMinimoAnterior.Buscar(objCotacao);


			Assert.AreEqual(new decimal(10.24), Math.Round(objCotacaoDoValorMinimoAnterior.ValorMinimo, 2));

		}

	}
}
