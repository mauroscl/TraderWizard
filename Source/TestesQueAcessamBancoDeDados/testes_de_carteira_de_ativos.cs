using System;
using System.Linq;
using DataBase.Carregadores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjModelo.Carregadores;
using TesteBase;
namespace TestProject1
{

	[TestClass()]
	public class testes_de_carteira_de_ativos : Inicializacao
	{

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

		[TestMethod()]

		public void CarregadorDeCarteirasTemQueCarregarUmaUnicaCarteiraAtivaPorIFRSobrevendido()
		{
			var objCarregadorCarteira = new CarregadorCarteira(objConexao);

			var objIFRSobrevendido = FuncoesGerais.CarregaIFRSobrevendido(objConexao, 1);

			var objCarteira = objCarregadorCarteira.CarregaAtiva(objIFRSobrevendido);

			Assert.AreEqual(1, objCarteira.IdCarteira);
			Assert.AreEqual("IFR Máximo: 5", objCarteira.Descricao);
			Assert.AreEqual(1, objCarteira.IFRSobrevendido.Id);
			Assert.AreEqual(objCarteira.DataInicio, new DateTime(2011, 9, 11));
			Assert.IsNull(objCarteira.DataFim);
			Assert.IsTrue(objCarteira.Ativo);
			Assert.AreEqual(36, objCarteira.Ativos.Count());

		}

	}
}
