using DataBase;
namespace TesteBase
{

	public abstract class Inicializacao
	{

		public static Conexao objConexao { get; set; }
		public static void Inicializa()
		{
			objConexao = new Conexao();
            SessionManager.ConfigureDataAccess();
		}


		public static void Finaliza()
		{
			objConexao.FecharConexao();
		}

	}
}
