using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjCandle;
using prjCandle.Desenho;

namespace TesteSemAcessarBancoDeDados.UI.FerramentaDeDesenho
{
    /// <summary>
    /// Summary description for TestesLinhaTendencia
    /// </summary>
    [TestClass]
    public class TestesCanal
    {
        public TestesCanal()
        {
            _areaDeDesenho = FuncoesGerais.RetornaAreaDeDesenhoPadrao();
        }

        private TestContext testContextInstance;
        private readonly AreaDeDesenho _areaDeDesenho;

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
        public void QuandoClicarUmaVezDepoisMoverTemQueRetornarUmaLinhaDeTendencia()
        {
            var ferramenta = new FerramentaCanal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(100, 50), 15));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(LinhaTendencia));
            var linhaHorizontal = (LinhaTendencia)ferramenta.DesenhoGerado;

            Assert.AreEqual(30, linhaHorizontal.PontoInicial.Ponto.X);
            Assert.AreEqual(40, linhaHorizontal.PontoInicial.Ponto.Y);
            Assert.AreEqual(100, linhaHorizontal.PontoFinal.Ponto.X);
            Assert.AreEqual(50, linhaHorizontal.PontoFinal.Ponto.Y);

        }

        [TestMethod]
        public void QuandoClicarUmaVezEMoverDuasVezesTemQueDesenharUmaLinhaDeTendencia()
        {
            var ferramenta = new FerramentaCanal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(100, 50), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(120, 60), 15));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(LinhaTendencia));
            var linhaHorizontal = (LinhaTendencia)ferramenta.DesenhoGerado;

            Assert.AreEqual(30, linhaHorizontal.PontoInicial.Ponto.X);
            Assert.AreEqual(40, linhaHorizontal.PontoInicial.Ponto.Y);
            Assert.AreEqual(120, linhaHorizontal.PontoFinal.Ponto.X);
            Assert.AreEqual(60, linhaHorizontal.PontoFinal.Ponto.Y);

        }

        [TestMethod]
        public void QuandoClicarDuasVezesTemQueConstruirUmaLinhaDeTendencia()
        {
            var ferramenta = new FerramentaCanal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(100, 50), 40));
            ferramenta.Click(new PontoDoDesenho(new Point(120, 60), 50));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(LinhaTendencia));
            var linhaDeTendencia = (LinhaTendencia)ferramenta.DesenhoGerado;

            Assert.AreEqual(30, linhaDeTendencia.PontoInicial.Ponto.X);
            Assert.AreEqual(40, linhaDeTendencia.PontoInicial.Ponto.Y);
            Assert.AreEqual(15, linhaDeTendencia.PontoInicial.Indice);
            Assert.AreEqual(120, linhaDeTendencia.PontoFinal.Ponto.X);
            Assert.AreEqual(60, linhaDeTendencia.PontoFinal.Ponto.Y);
            Assert.AreEqual(50, linhaDeTendencia.PontoFinal.Indice);
            
        }

        [TestMethod]
        public void QuandoClicarDuasVezesDepoisMoverTemQueConstruirUmCanal()
        {
            var ferramenta = new FerramentaCanal(_areaDeDesenho);
            ferramenta.Click(new PontoDoDesenho(new Point(30, 40), 15));
            ferramenta.Move(new PontoDoDesenho(new Point(100, 50), 20));
            ferramenta.Click(new PontoDoDesenho(new Point(120, 60), 20));
            ferramenta.Move(new PontoDoDesenho(new Point(130, 70), 20));
            Assert.IsInstanceOfType(ferramenta.DesenhoGerado, typeof(Canal));
            var canal = (Canal)ferramenta.DesenhoGerado;

            Assert.AreEqual(30, canal.PontoInicial.Ponto.X);
            Assert.AreEqual(40, canal.PontoInicial.Ponto.Y);
            Assert.AreEqual(15, canal.PontoInicial.Indice);
            Assert.AreEqual(130, canal.PontoFinal.Ponto.X);
            Assert.AreEqual(70, canal.PontoFinal.Ponto.Y);            
            Assert.AreEqual(20, canal.PontoFinal.Indice);
        }

    }
}
