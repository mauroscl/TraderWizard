using Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesteSemAcessarBancoDeDados.Geral;

namespace TesteSemAcessarBancoDeDados
{

	[TestClass()]
	public class testes_de_carteira_de_ativos
	{
	    ///<summary>
	    ///Gets or sets the test context which provides
	    ///information about and functionality for the current test run.
	    ///</summary>
	    public TestContext TestContext { get; set; }

	    #region "Additional test attributes"
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// <ClassInitialize()> Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
		// End Sub
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// <ClassCleanup()> Public Shared Sub MyClassCleanup()
		// End Sub
		//
		// Use TestInitialize to run code before running each test
		// <TestInitialize()> Public Sub MyTestInitialize()
		// End Sub
		//
		// Use TestCleanup to run code after each test has run
		// <TestCleanup()> Public Sub MyTestCleanup()
		// End Sub
		//
		#endregion

		[TestMethod()]
		public void QuandoAtivoEstaNaCarteiraDeveRetornaTRUE()
		{
			Carteira objCarteira = FuncoesGerais.RetornaCarteiraPadrao();
			var objAtivoProcurado = FuncoesGerais.GeraAtivo("BBAS3");
			Assert.IsTrue(objCarteira.AtivoEstaNaCarteira(objAtivoProcurado));
		}

		[TestMethod()]
		public void QuandoAtivoNaoEstaNaCarteiraDeveRetornaFALSE()
		{
			Carteira objCarteira = FuncoesGerais.RetornaCarteiraPadrao();
			var objAtivoProcurado = FuncoesGerais.GeraAtivo("MMXM3");
			Assert.IsFalse(objCarteira.AtivoEstaNaCarteira(objAtivoProcurado));
		}

	}
}
