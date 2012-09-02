using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using frwInterface;
using prjCandle;
using prjModelo.Entidades;
namespace TesteSemAcessarBancoDeDados
{

	public class FuncoesGerais
	{

		public static cAtivo GeraAtivoPadrao()
		{
			return new cAtivo("ATIV4", "Ativo Padrão");
		}

		public static cAtivo GeraAtivo(string pstrCodigo)
		{
			return new cAtivo(pstrCodigo, string.Empty);
		}

		public static cCotacaoDiaria GeraCotacaoPadrao()
		{
			return new cCotacaoDiaria(GeraAtivoPadrao(), DateAndTime.Now);
		}

		private static cIFRSobrevendido RetornaIFRSobrevendido()
		{
			return new cIFRSobrevendido(1, 5);
		}

		public static cCarteira RetornaCarteiraPadrao()
		{
			var objRetorno = new cCarteira(1, "Teste", RetornaIFRSobrevendido(), true, DateAndTime.Now.Date);
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
