using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjModelo.Entidades;
using prjModelo.Regras;
namespace TesteSemAcessarBancoDeDados
{

	[TestClass()]
	public class testes_de_media_alinhada
	{

		[TestMethod()]

		public void QuandoAsMediasEstiverAlinhadasPorPeriodoEValorTemQueRetornarTrue()
		{
			var objCotacao = FuncoesGerais.GeraCotacaoPadrao();

			IList<cMediaAbstract> lstMedia = new List<cMediaAbstract>();

			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 21, 20));
			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 49, 15));
			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 200, 10));

			bool blnRetorno = cVerificadorMediasAlinhadas.Verificar(ref lstMedia);

			Assert.IsTrue(blnRetorno);

		}

		[TestMethod()]

		public void QuandoAsMediasEstiverDesalinhadasPorPeriodoEValorTemQueRetornarFalse()
		{
			var objCotacao = FuncoesGerais.GeraCotacaoPadrao();

			IList<cMediaAbstract> lstMedia = new List<cMediaAbstract>();

			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 21, 10));
			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 49, 15));
			lstMedia.Add(new cMediaDiaria(objCotacao, "MME", 200, 20));

			bool blnRetorno = cVerificadorMediasAlinhadas.Verificar(ref lstMedia);

			Assert.IsFalse(blnRetorno);

		}


	}
}
