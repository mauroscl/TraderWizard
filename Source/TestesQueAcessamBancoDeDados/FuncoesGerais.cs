using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBase;
using prjDominio.Entidades;
using prjModelo.Carregadores;
using prjModelo.Entidades;
using TraderWizard.Enumeracoes;

namespace TestProject1
{

	public class FuncoesGerais
	{

		public static Setup CarregarSetup(cEnum.enumSetup pintIDSetup)
		{

			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();

			return objCarregadorSetup.CarregaPorID(pintIDSetup);

		}

		public static cAtivo RetornaAtivo(string pstrCodigo)
		{
			return new cAtivo(pstrCodigo, string.Empty);
		}

		public static cIFRSobrevendido CarregaIFRSobrevendido(cConexao pobjConexao, int pintId)
		{

			cCarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(pobjConexao);

			return objCarregadorIFRSobrevendido.CarregaPorID(pintId);

		}
	}
}
