using System;
using Dominio.Entidades;
using prjCandle;
using prjCandle.Desenho;
using TraderWizard.Enumeracoes;

namespace TesteSemAcessarBancoDeDados
{

	public class FuncoesGerais
	{

		public static Ativo GeraAtivoPadrao()
		{
			return new Ativo("ATIV4", "Ativo Padrão");
		}

		public static Ativo GeraAtivo(string pstrCodigo)
		{
			return new Ativo(pstrCodigo, string.Empty);
		}

		public static CotacaoDiaria GeraCotacaoPadrao()
		{
			return new CotacaoDiaria(GeraAtivoPadrao(), DateTime.Now);
		}

		private static IFRSobrevendido RetornaIFRSobrevendido()
		{
			return new IFRSobrevendido(1, 5);
		}

		public static Carteira RetornaCarteiraPadrao()
		{
			var objRetorno = new Carteira(1, "Teste", RetornaIFRSobrevendido(), true, DateTime.Now.Date);
			objRetorno.AdicionaAtivo(GeraAtivoPadrao());
			objRetorno.AdicionaAtivo(GeraAtivo("BBAS3"));
			return objRetorno;
		}

        public static AreaDeDesenho RetornaAreaDeDesenhoPadrao()
        {
            return new AreaDeDesenho(20, 150, 35M, cEnum.Escala.Logaritmica, new LimiteHorizontal(1, 20), new LimiteHorizontal(500, 700));   
        }
	}
}
