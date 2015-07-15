using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using prjConfiguracao;
using TraderWizard.Enumeracoes;

namespace DataBase
{

	public class Conexao
	{

	    private readonly cEnum.BancoDeDados _bancoDeDados;

        public Conexao()
	    {
	        _bancoDeDados = cBuscarConfiguracao.ObterBancoDeDados();

			try {

				ConnectionString = cBuscarConfiguracao.ObterConnectionStringPadrao(_bancoDeDados);

			    if (_bancoDeDados == cEnum.BancoDeDados.SqlServer)
			    {
			        Conn = new SqlConnection(ConnectionString);
			    }
			    else
			    {
                    Conn = new OleDbConnection(ConnectionString);    
			    }
				

				//inicialização das propriedades
				TransAberta = false;
				TransStatus = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message);

			}

		}

	    public cEnum.BancoDeDados BancoDeDados
	    {
	        get { return _bancoDeDados; }
	    }

        public string ConnectionString { get; set; }

        public bool TransStatus { get; private set; }

        public bool TransAberta { get; private set; }

        public DbConnection Conn { get; private set; }

        public DbTransaction Transacao { get; private set; }

		public void VerificarConexao()
		{
			if (Conn == null) {
			    if (_bancoDeDados == cEnum.BancoDeDados.SqlServer)
			    {
			        Conn = new SqlConnection(this.ConnectionString);
			    }
			    else
			    {
                    Conn = new OleDbConnection(this.ConnectionString);    
			    }
				
				Conn.Open();
			} else {
				//se a conexão não está aberta, só pode abrir se o TransStatus está OK.
				if (TransStatus) {
					if (Conn.State == ConnectionState.Closed) {
						Conn.Open();
					}
				}
			}

		}

		//Fecha a conexão
		public void FecharConexao()
		{
			if ((Conn != null)) {
				if (Conn.State == ConnectionState.Open) {
					Conn.Close();
				}
			}
		}


		public void BeginTrans()
		{
			//ABRE A CONEXÃO COM O BANCO. VAI FICAR ABERTA ATÉ O ROLLBACK OU COMMIT
			VerificarConexao();

			Transacao = this.Conn.BeginTransaction(IsolationLevel.ReadCommitted);
			//MARCA QUE A TRANSAÇÃO FOI ABERTA
			TransAberta = true;
			//O STATUS INICIA, JÁ QUE NÃO FOI EXECUTADO NENHUM COMANDO É TRUE
			TransStatus = true;

		}

		/// <summary>
		/// faz o commit em uma transação
		/// </summary>
		/// <remarks></remarks>
		public void CommitTrans()
		{
			//se a transação ainda está aberta (não ocorreu erro) então faz commit

			if (TransAberta) {
				Transacao.Commit();

				TransAberta = false;

			}

			//fecha a conexão com o banco de dados
			//FecharConexao()

		}


		public void RollBackTrans()
		{
			if (TransAberta) {
				Transacao.Rollback();
			}

			//quando dá rollback a transação fica fechada e seu status não está OK.
			TransAberta = false;
			TransStatus = false;

			//AO FAZER ROLLBACK NÃO PRECISA FECHAR A CONEXÃO, POIS O ROLLBACK
			//APENAS INDICA QUE OS COMANDOS EXECUTADOS SERÃO DESFEITOS
			//FecharConexao()

		}

	    public FuncoesBd ObterFormatadorDeCampo()
	    {
	        switch (_bancoDeDados)
	        {
	            case cEnum.BancoDeDados.Access:
                    return new FuncoesBdAccess();
                case cEnum.BancoDeDados.SqlServer:
                    return new FuncoesBdSqlServer();
                default:
                    return new FuncoesBd();
	        }
	    }


		/*protected override void Finalize()
		{
			//Caution()
			//Do not call Close or Dispose on an OleDbConnection, an OleDbDataReader, 
			//or any other managed object in the Finalize method of your class. In a finalizer, 
			//you should only release unmanaged resources that your class owns directly. 
			//If your class does not own any unmanaged resources, do not include a Finalize method 
			//in your class definition. For more information, see Garbage Collection.

			//FecharConexao()



			base.Finalize();

		}*/

		// This method disposes the derived object's resources.
		/*protected override void Dispose(bool disposing)
		{
			if (!this.disposed) {
				if (disposing) {
					// Insert code to free unmanaged resources.
					FecharConexao();
				}
				// Insert code to free shared resources.
			}
			base.Dispose(disposing);
		}*/

		// The derived class does not have a Finalize method
		// or a Dispose method with parameters because it inherits
		// them from the base class.



	}
}


