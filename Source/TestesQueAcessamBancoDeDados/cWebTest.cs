using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBase;
using pWeb;
using TesteBase;
namespace TestProject1
{


	///<summary>
	///This is a test class for cWebTest and is intended
	///to contain all cWebTest Unit Tests
	///</summary>
	[TestClass()]
	public class cWebTest : Inicializacao
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


		///<summary>
		///A test for ArquivoBaixar
		///</summary>
		[TestMethod()]

		public void QuandoPassarLinkValidoFuncaoTemQueRetornarQueOLinkEhValido()
		{
			Conexao pobjConexao = objConexao;
			cWeb target = new cWeb(pobjConexao);
			string pstrURL = "http://www.bmfbovespa.com.br/fechamento-pregao/bdi/bdi0325.zip";
			bool expected = false;
			expected = target.VerificarLink(pstrURL);
			Assert.IsTrue(expected);
		}

		[TestMethod()]

		public void QuandoPassarLinkInvalidoFuncaoTemQueRetornarQueOLinkEhInvalido()
		{
			Conexao pobjConexao = objConexao;
			cWeb target = new cWeb(pobjConexao);
			string pstrURL = "http://www.bmfbovespa.com.br/fechamento-pregao/bdi/bdi0326.zip";
			bool expected = false;
			expected = target.VerificarLink(pstrURL);
			Assert.IsFalse(expected);
		}

	}
}
