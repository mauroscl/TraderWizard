using DataBase;

namespace prjModelo.Carregadores
{

	public abstract class cCarregadorGenerico
	{

		public cConexao Conexao;

		public bool ConexaoLocal;
		public cCarregadorGenerico()
		{
			Conexao = new cConexao();
			ConexaoLocal = true;
		}

		public cCarregadorGenerico(cConexao pobjConexao)
		{
			Conexao = pobjConexao;
		}

		public void VerificaSeDeveFecharConexao()
		{
			if (ConexaoLocal) {
				Conexao.FecharConexao();
			}
		}

	}
}
