using DataBase;
using DataBase.Carregadores;
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

		public static Ativo RetornaAtivo(string pstrCodigo)
		{
			return new Ativo(pstrCodigo, string.Empty);
		}

		public static cIFRSobrevendido CarregaIFRSobrevendido(cConexao pobjConexao, int pintId)
		{

			cCarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new cCarregadorIFRSobrevendido(pobjConexao);

			return objCarregadorIFRSobrevendido.CarregaPorID(pintId);

		}
	}
}
