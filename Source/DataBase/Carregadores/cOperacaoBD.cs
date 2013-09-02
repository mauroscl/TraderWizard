
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{
	public class cOperacaoBD
	{

		public string Comando { get; set; }

		public cModelo Modelo { get; set; }

		public cOperacaoBD(cModelo pobjModelo, string pstrComando)
		{
			Modelo = pobjModelo;
			Comando = pstrComando;
		}

		public void AlterarComando(string pstrNovoComando)
		{
			this.Comando = pstrNovoComando;
		}

	}
}
