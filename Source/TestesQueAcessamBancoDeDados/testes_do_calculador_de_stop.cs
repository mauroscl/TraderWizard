using System.Collections.Generic;
using DataBase.Carregadores;
using Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DTO;
using TesteBase;
using TraderWizard.Enumeracoes;

namespace TestProject1
{

	///<summary>
	///This is a test class for cCalculadorDataInicialDetalheTest and is intended
	///to contain all cCalculadorDataInicialDetalheTest Unit Tests
	///</summary>
	[TestClass()]
	public class testes_do_calculador_de_stop : Inicializacao
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

		private IList<MediaDTO> RetornaListaDeMediasUtilizadasNaClassificacao()
		{

			List<MediaDTO> lstMediasDTO = new List<MediaDTO>();

			lstMediasDTO.Add(new MediaDTO("E", 21, "VALOR"));
			lstMediasDTO.Add(new MediaDTO("E", 49, "VALOR"));
			lstMediasDTO.Add(new MediaDTO("E", 200, "VALOR"));

			return lstMediasDTO;

		}

		private InformacoesDoTradeDTO RetornaInformacoesDoTradeDTOPadrao()
		{

			InformacoesDoTradeDTO objInformacoesDoTradeDTO = new InformacoesDoTradeDTO();
			objInformacoesDoTradeDTO.IFRCruzouMediaParaCima = true;
			//valor de entrada e valor máximo não refletem os valores reais. foram colocados apenas para indicar que foi permitido realizar parcial
            objInformacoesDoTradeDTO.ValorRealizacaoParcial = 1.05M ;
			objInformacoesDoTradeDTO.ValorMaximo = 1.05M;

			return objInformacoesDoTradeDTO;

		}

		[TestMethod()]

		public void QuandoMediaEstiveremDesalinhadasOValorDoStopTemQueFicarAbaixoDoValorMinimoDaUltimaCotacaoSeONovoValorDoStopForMaiorDoQueOValorDoStopAnterior()
		{
			var lstMediasDTO = RetornaListaDeMediasUtilizadasNaClassificacao();
			CarregadorCotacaoDiaria objCarregadorCotacao = new CarregadorCotacaoDiaria();

			var objCotacao = objCarregadorCotacao.CarregarPorPeriodo(new Ativo("CSNA3", string.Empty), new System.DateTime(2011, 5, 11), new System.DateTime(2011, 5, 11), string.Empty, lstMediasDTO, true);

			CarregadorSetup objCarregadorSetup = new CarregadorSetup();
			var objSetup = objCarregadorSetup.CarregaPorId(cEnum.enumSetup.IFRSemFiltroRP);

			var objInformacoesDoTradeDTO = RetornaInformacoesDoTradeDTOPadrao();
			objInformacoesDoTradeDTO.ValorDoStopLoss = 22.45M;
			//TODO: verificar se o valor do stop loss anterior era mesmo 22.35

			decimal decValorStop = objSetup.CalculaValorStopLossDeSaida((CotacaoAbstract) objCotacao, objInformacoesDoTradeDTO,null);

			Assert.AreEqual(new decimal(22.29), decValorStop);

		}

		[TestMethod()]

		public void QuandoMediaEstiveremDesalinhadasOValorDoStopTemQueFicarIgualAoAnteriorSeONovoValorDoStopForMenorDoQueOValorDoStopAnterior()
		{
			//TODO: Refazer teste
			//Dim objCalculadorStop As New cCalculadorStop(Me.objConexao)

			//Dim decValorStop As Decimal = objCalculadorStop.Calcular(New cAtivo("CSNA3"), New Date(2011, 5, 11), 23, 22.35, 22.35)

			//Assert.AreEqual(New Decimal(23), decValorStop)

		}

		[TestMethod()]

		public void QuandoMediaEstiveremAlinhadasOValorDoStopTemQueFicarAbaixoDoValorMinimoDaCotacaoAnteriorSeONovoValorDoStopForMaiorDoQueOValorDoStopAnterior()
		{
			//TODO: Refazer teste
			//Dim objCalculadorStop As New cCalculadorStop(Me.objConexao)

			//Dim decValorStop As Decimal = objCalculadorStop.Calcular(New cAtivo("CSNA3"), New Date(2011, 1, 27), 21, 28.37, 28.18)

			//'Pegou abaixo da mínima do dia 20/01/2011, que foi 28.04
			//Assert.AreEqual(New Decimal(27.97), decValorStop)

		}

		[TestMethod()]

		public void QuandoMediaEstiveremDesalinhadasMasOFechamentoEstiverAcimaDasMediasOValorDoStopTemQueFicarAbaixoDoValorMinimoDaCotacaoAnteriorSeONovoValorDoStopForMaiorDoQueOValorDoStopAnterior()
		{
			//TODO: Refazer teste
			//Dim objCalculadorStop As New cCalculadorStop(Me.objConexao)

			//Dim decValorStop As Decimal = objCalculadorStop.Calcular(New cAtivo("CSNA3"), New Date(2011, 1, 13), 21, 29.7, 29.61)

			//'Pegou abaixo da mínima do dia 11/01/2011, que foi 29.04
			//Assert.AreEqual(New Decimal(28.97), decValorStop)

		}



	}
}
