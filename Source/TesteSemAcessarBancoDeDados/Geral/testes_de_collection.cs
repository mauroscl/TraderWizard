using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using prjModelo.Entidades;
using prjModelo;
namespace TesteSemAcessarBancoDeDados
{

	[TestClass()]
	public class testes_de_collection
	{

		[TestMethod()]

		public void VerificaSeEPossivelAlterarObjetoDentroDaCollectionEAlteracaoSerMantida()
		{
			List<cIFRSobrevendido> lstLista = new List<cIFRSobrevendido> {
				new cIFRSobrevendido(1, 5),
				new cIFRSobrevendido(2, 10)
			};

		    var cIFRSobrevendido = lstLista.FirstOrDefault(x => x.ID == 1);
		    if (cIFRSobrevendido != null)
		        cIFRSobrevendido.ValorMaximo = 6;

		    Assert.AreEqual(6.0, lstLista[0].ValorMaximo);

		}

		[TestMethod()]
		public void VerificaSeOrdenaSemAtribuirEmOutraLista()
		{
			List<Int32> lstNumeros = new List<Int32>();

			lstNumeros.Add(3);
			lstNumeros.Add(2);
			lstNumeros.Add(1);

			Assert.AreEqual(3, lstNumeros[0]);
			Assert.AreEqual(2, lstNumeros[1]);
			Assert.AreEqual(1, lstNumeros[2]);

			lstNumeros = lstNumeros.OrderBy(x => x).ToList();

			Assert.AreEqual(1, lstNumeros[0]);
			Assert.AreEqual(2, lstNumeros[1]);
			Assert.AreEqual(3, lstNumeros[2]);

		}

	}
}
