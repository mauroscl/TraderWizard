using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraderWizard.Extensoes;

namespace Extensoes
{
    [TestClass]
    public class ExtensoesDeString
    {
        [TestMethod]
        public void UmNumeroInteiroValidoDeveSerValido()
        {
            Assert.IsTrue("123".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroDecimalValidoDeveSerValido()
        {
            Assert.IsTrue("123.15".IsNumeric());
            
        }

        [TestMethod]
        public void UmNumeroComDoisSeparadoresDecimaisDeVeSerInvalido()
        {
            Assert.IsFalse("123..15".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComDoisSeparadoresDeMilharesSeguidosDeVeSerInvalido()
        {
            Assert.IsFalse("123,,15".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComUmSeparadorDeMilharesNaPosicaoCorretaDeveSerValido()
        {
            Assert.IsTrue("12,315".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComSeparadorDeMilharesNaPosicaoIncorretaDeveSerInvalido()
        {
            Assert.IsFalse("123,15".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComDoisSeparadoresDeMilharesDeVeSerValido()
        {
            Assert.IsTrue("1,000,000".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComSeparadorDeMilharesESeparadorDecimalDeveSerValido()
        {
            Assert.IsTrue("1,000.00".IsNumeric());
        }

        [TestMethod]
        public void UmaNumeroValidoContendoEspacoNoInicioOuNoFimDeveSerValido()
        {
            Assert.IsTrue(" 123 ".IsNumeric());
        }

        [TestMethod]
        public void UmaStringVaziaDeveSerInvalida()
        {
            Assert.IsFalse("".IsNumeric());
        }

        [TestMethod]
        public void UmaStringNulaDeveSerInvalida()
        {
            string teste = null;
            Assert.IsFalse(teste.IsNumeric());
        }

        [TestMethod]
        public void UmNumeroNegativoDeveSerValido()
        {
            Assert.IsTrue("-1,000.00".IsNumeric());
        }

        [TestMethod]
        public void UmNumeroComDoisSinaisNegativosDeveSerInvalido()
        {
            Assert.IsFalse("--1,000.00".IsNumeric());
        }


    }
}
