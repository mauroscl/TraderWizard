using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	public class testes_do_carregador_de_ifr_sobrevendido : Inicializacao
	{

		private TestContext testContextInstance;

		private cConexao objConexao;
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

		public void QuandoCarregarPorIdTemQueRetornarIFRSobrevendidoComMesmoId()
		{
			var objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(objConexao);

			var objIFRSobrevendido = objCarregadorIFRSobrevendido.CarregaPorID(1);

			Assert.AreEqual(1, objIFRSobrevendido.ID);

		}

		[TestMethod()]

		public void QuandoCarregarTodosTemQueRetornarUmaListaComTodosIFRSobrevendido()
		{
			var objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(objConexao);

			var lstIFrSobrevendido = objCarregadorIFRSobrevendido.CarregarTodos();

			Assert.AreEqual(2, lstIFrSobrevendido.Count());

		}

		[TestMethod()]

		public void QuandoCarregarPorValorMaximoTemQueRetornarOIFRSobrevendidoReferenteAoValorMaximoPassado()
		{
			var objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(objConexao);

			var objIFRSobrevendido = objCarregadorIFRSobrevendido.CarregaPorValorMaximo(10);

			Assert.AreEqual(Convert.ToDouble(10), objIFRSobrevendido.ValorMaximo);

		}

		[TestMethod()]

		public void QuandoCarregarPorValorTemQueRetornarUmaListaComTodosIFRSobrevendidoQueSeEncaixamNoValor()
		{
			var objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(objConexao);

			var lstIFRSobrevendido = objCarregadorIFRSobrevendido.CarregaPorValor(7.5);

			Assert.AreEqual(1, lstIFRSobrevendido.Count());

		}




	}
}
