using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using prjCandle;
using prjDominio.Entidades;
using prjModelo.Entidades;
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
			return new CotacaoDiaria(GeraAtivoPadrao(), DateAndTime.Now);
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
