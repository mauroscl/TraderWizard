using System;

namespace DataBase.Carregadores
{

	public abstract class CarregadorGenerico: IDisposable
	{

		public Conexao Conexao;

		private readonly bool _conexaoLocal;

	    protected CarregadorGenerico()
		{
			Conexao = new Conexao();
			_conexaoLocal = true;
		}

	    protected CarregadorGenerico(Conexao pobjConexao)
		{
			Conexao = pobjConexao;
		}


        public void Dispose()
        {
            VerificaSeDeveFecharConexao();
        }

        public void VerificaSeDeveFecharConexao()
        {
            if (_conexaoLocal)
            {
                Conexao.FecharConexao();
            }
        }


	}
}
