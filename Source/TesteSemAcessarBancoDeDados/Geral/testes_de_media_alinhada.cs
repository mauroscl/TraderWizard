using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjDominio.Entidades;
using prjDominio.Regras;

namespace TesteSemAcessarBancoDeDados
{

	[TestClass()]
	public class testes_de_media_alinhada
	{

		[TestMethod()]

		public void QuandoAsMediasEstiverAlinhadasPorPeriodoEValorTemQueRetornarTrue()
		{
			var objCotacao = FuncoesGerais.GeraCotacaoPadrao();

			IList<MediaAbstract> lstMedia = new List<MediaAbstract>();

			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 21, 20));
			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 49, 15));
			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 200, 10));

			bool blnRetorno = cVerificadorMediasAlinhadas.Verificar(ref lstMedia);

			Assert.IsTrue(blnRetorno);

		}

		[TestMethod()]

		public void QuandoAsMediasEstiverDesalinhadasPorPeriodoEValorTemQueRetornarFalse()
		{
			var objCotacao = FuncoesGerais.GeraCotacaoPadrao();

			IList<MediaAbstract> lstMedia = new List<MediaAbstract>();

			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 21, 10));
			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 49, 15));
			lstMedia.Add(new MediaDiaria(objCotacao, "MME", 200, 20));

			bool blnRetorno = cVerificadorMediasAlinhadas.Verificar(ref lstMedia);

			Assert.IsFalse(blnRetorno);

		}


	}
}
