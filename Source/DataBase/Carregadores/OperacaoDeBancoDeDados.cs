using Dominio.Entidades;

namespace DataBase.Carregadores
{
	public class OperacaoDeBancoDeDados
	{

		public string Comando { get; set; }

		public Modelo Modelo { get; set; }

		public OperacaoDeBancoDeDados(Modelo pobjModelo, string pstrComando)
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
