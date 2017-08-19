using System;
using DataBase;
using DataBase.Carregadores;
using Dominio.Entidades;
using TraderWizard.Enumeracoes;

namespace TestProject1
{

	public class FuncoesGerais
	{
        [CLSCompliant(false)]
		public static Setup CarregarSetup(cEnum.enumSetup pintIDSetup)
		{

			cCarregadorSetup objCarregadorSetup = new cCarregadorSetup();

			return objCarregadorSetup.CarregaPorID(pintIDSetup);

		}

		public static Ativo RetornaAtivo(string pstrCodigo)
		{
			return new Ativo(pstrCodigo, string.Empty);
		}

		public static IFRSobrevendido CarregaIFRSobrevendido(Conexao pobjConexao, int pintId)
		{

			cCarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(pobjConexao);

			return objCarregadorIFRSobrevendido.CarregaPorID(pintId);

		}
	}
}
