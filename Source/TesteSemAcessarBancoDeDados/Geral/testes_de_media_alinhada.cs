using System.Collections.Generic;
using Dominio.Entidades;
using Dominio.Regras;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TesteSemAcessarBancoDeDados.Geral;

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

			bool blnRetorno = VerificadorMediasAlinhadas.Verificar(ref lstMedia);

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

			bool blnRetorno = VerificadorMediasAlinhadas.Verificar(ref lstMedia);

			Assert.IsFalse(blnRetorno);

		}


	}
}
