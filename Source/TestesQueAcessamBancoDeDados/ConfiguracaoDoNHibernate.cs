using System;
using System.Linq;
using Dominio.Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Linq;
using StructureMap;
using TesteBase;

namespace TestProject1
{
    [TestClass]
    public class ConfiguracaoDoNHibernate: Inicializacao
    {
        [ClassInitialize]
        public static void Inicializar(TestContext testContext)
        {
            Inicializa();
        }

        [ClassCleanup]
        public static void Finalizar()
        {
            Finaliza();
        }

        [TestMethod]
        public void ConsigoConfigurarUmaSessao()
        {
            var session = ObjectFactory.GetInstance<ISession>();
            Assert.IsTrue(session.IsConnected);
            Assert.IsTrue(session.IsOpen);

            var ativo = session.Query<Ativo>().SingleOrDefault(x => x.Codigo == "PETR4");

            Assert.IsNotNull(ativo);
            Assert.AreEqual("PETR4", ativo.Codigo);
            Assert.AreEqual("PETROBRAS PN", ativo.Descricao);
            
        }

        [TestMethod]
        public void ConsigoAdicionarUmAtivoSemIniciarUmaTransacao()
        {
            var ativo = new Ativo("TEST4", "TESTE PN");

            var session = ObjectFactory.GetInstance<ISession>();

            session.Save(ativo);

            session.Flush();

            //session.Close();
        }
    }
}
