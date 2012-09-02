using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frwInterface;
using prjCandle;

namespace TesteSemAcessarBancoDeDados.UI.FerramentaDeDesenho
{
    /// <summary>
    /// Summary description for TestesLinhaHorizontal
    /// </summary>
    [TestClass]
    public class TestesLinhaHorizontal
    {
        public TestesLinhaHorizontal()
        {
            _areaDeDesenho = FuncoesGerais.RetornaAreaDeDesenhoPadrao();
        }

        private readonly AreaDeDesenho _areaDeDesenho;


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void QuandoMoverSemClicarPelaPrimeiraVezTemQueRetorarUmDesenhoNulo()
        {
            var ferramenta = new FerramentaLinhaHorizontal(_areaDeDesenho);
            ferramenta.Move(new PontoDoDesenho(new Point(30, 40), 15));
            Assert.IsNull(ferramenta.DesenhoGerado);
        }


        [TestMethod]
        public void QuandoClicarPelaPrimeiraVezTemQueRetornarUmDesenhoNulo()
        {
            var ferramenta = new FerramentaLinhaHorizontal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30,40), 15));
            Assert.IsNull(ferramenta.DesenhoGerado);
        }

        [TestMethod]
        public void QuandoClicarUmaVezDepoisMoverTemQueRetornarUmaLinhaHorizontal()
        {
            var ferramenta = new FerramentaLinhaHorizontal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(100, 50), 15));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(LinhaHorizontal));
            var linhaHorizontal = (LinhaHorizontal) ferramenta.DesenhoGerado;

            Assert.AreEqual(30,linhaHorizontal.PontoInicial.Ponto.X);
            Assert.AreEqual(50, linhaHorizontal.PontoInicial.Ponto.Y);
            Assert.AreEqual(100, linhaHorizontal.PontoFinal.Ponto.X);
            Assert.AreEqual(50, linhaHorizontal.PontoFinal.Ponto.Y);

        }


        [TestMethod]
        public void QuandoClicarPelaSegundaVezTemQueRetornarUmaLinhaHorizontal()
        {
            var ferramenta = new FerramentaLinhaHorizontal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30,40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(70,35), 22));
            ferramenta.Click(new PontoDoDesenho(new Point(100, 80), 55));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(LinhaHorizontal));
            var linhaHorizontal = (LinhaHorizontal) ferramenta.DesenhoGerado;

            Assert.AreEqual(30, linhaHorizontal.PontoInicial.Ponto.X);
            Assert.AreEqual(80, linhaHorizontal.PontoInicial.Ponto.Y);
            Assert.AreEqual(100, linhaHorizontal.PontoFinal.Ponto.X);
            Assert.AreEqual(80, linhaHorizontal.PontoFinal.Ponto.Y);

        }

        [TestMethod]
        public void QuandoUmDesenhoEstiverCompletoeHouverMaisUmCliqueDeveIniciarUmNovoDesenho()
        {
            var ferramenta = new FerramentaLinhaHorizontal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(70, 35), 22));
            ferramenta.Click(new PontoDoDesenho(new Point(100, 80), 55));

            ferramenta.Click(new PontoDoDesenho(new Point(200, 30), 110));

            Assert.IsNull(ferramenta.DesenhoGerado);            
        }

       




    }
}
