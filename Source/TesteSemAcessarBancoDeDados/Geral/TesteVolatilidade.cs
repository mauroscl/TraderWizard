using System;
using System.Linq;
using Dominio.Regras;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TesteSemAcessarBancoDeDados.Geral
{
    [TestClass]
    public class TesteVolatilidade
    {
        [TestMethod]
        public void CalculoEstaCorreto()
        {
            var dados = new []
            {
                1.0000000000D,
                1.0006000000D,
                1.0107000000D,
                0.9920000000D,
                1.0190000000D,
                0.9794000000D,
                1.0423000000D,
                1.0169000000D,
                1.0106000000D,
                1.0271000000D,
                1.0148000000D,
                0.9712000000D,
                0.9986000000D,
                0.9993000000D,
                1.0057000000D,
                1.0029000000D,
                0.9979000000D,
                1.0337000000D,
                0.9809000000D,
                1.0421000000D,
                0.9940000000D
            };
            var service = new CalculoService();
            Assert.AreEqual(0.302M, service.CalcularVolatilidadeHistorica(dados));
        }
    }
}
