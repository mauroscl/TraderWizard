using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesteBase;
using DataBase;

namespace TestProject1
{
    [TestClass]
    public class TestesDeFuncoesBD : Inicializacao
    {

        //Use TestInitialize to run code before running each test
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Inicializa();
        }

        //Use TestCleanup to run code after each test has run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            Finaliza();
        }
        [TestMethod]
        public void ConsigoConsultarOAtivoPadrao()
        {
            var dadosDB = new DadosDb(objConexao, "Configuracao");

            dadosDB.CampoAdicionar("Parametro", true, "AtivoPadrao");

            dadosDB.CampoAdicionar("Negocios", false, "");

            dadosDB.DadosBDConsultar();
            string ativoPadrao = dadosDB.CampoConsultar("Negocios");            
            Assert.AreEqual("ELPL4", ativoPadrao);
        }
    }
}
