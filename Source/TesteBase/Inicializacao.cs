using DataBase;
namespace TesteBase
{

	public abstract class Inicializacao
	{

		public static cConexao objConexao { get; set; }
		public static void Inicializa()
		{
			objConexao = new cConexao();
            SessionManager.ConfigureDataAccess();
		}


		public static void Finaliza()
		{
			objConexao.FecharConexao();
		}

	}
}
