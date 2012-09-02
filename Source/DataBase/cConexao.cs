using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Data.OleDb;
using System.Windows.Forms;
using prjConfiguracao;
namespace DataBase
{

	public class cConexao
	{

		//objeto da conexao

		private OleDbConnection objConn;
		//objeto da transação

		private OleDbTransaction objTransacao;
		/// <summary>
		/// Quando uma transação está aberta, indica se o status da transação está OK ou não
		/// </summary>
		/// <remarks></remarks>

		private bool blnTransStatus;
		/// <summary>
		/// Indica se a transação está aberta ou fechada.
		/// </summary>
		/// <remarks></remarks>

		private bool blnTransAberta;
		//Private intConexaoEstado As enumConexaoEstado

		//String de conexão com o BD

		private string strConnectionString;
		//Property Get e Set da ConnectionString
		public string ConnectionString {
			get { return this.strConnectionString; }
			set { this.strConnectionString = value; }
		}

		public bool TransStatus {
			get { return blnTransStatus; }
		}

		public bool TransAberta {
			get { return blnTransAberta; }
		}

		public OleDbConnection Conn {
			get { return this.objConn; }
		}

		public OleDbTransaction Transacao {
			get { return objTransacao; }
		}


		public cConexao()
		{

			try {
				//Dim strCaminhoBD As String
				//strCaminhoBD = cBuscarConfiguracao.ObtemCaminhoPadrao()
				//ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strCaminhoBD & "Bolsa.mdb"

				ConnectionString = cBuscarConfiguracao.ObterConnectionStringPadrao();

				objConn = new OleDbConnection(ConnectionString);

				//inicialização das propriedades
				blnTransAberta = false;
				blnTransStatus = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message);

			}

		}

		/// <summary>
		/// Utilizar somente para testes, pois nos testes o arquivo de configuração não existe.
		/// </summary>
		/// <param name="pstrCaminhoBD"></param>
		/// <remarks></remarks>
		//Public Sub New(pstrCaminhoBD As String)

		//    Me.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & pstrCaminhoBD & "Bolsa.mdb"

		//    Me.objConn = New OleDbConnection(Me.ConnectionString)

		//    'inicialização das propriedades
		//    Me.blnTransAberta = False
		//    Me.blnTransStatus = True

		//End Sub


		public void VerificarConexao()
		{

			//try {
				if (objConn == null) {
					objConn = new OleDbConnection(this.ConnectionString);
					objConn.Open();
				} else {
					//se a conexão não está aberta, só pode abrir se o TransStatus está OK.
					if (blnTransStatus) {
						if (objConn.State != ConnectionState.Open) {
							objConn.Open();
						}
					}
				}


			/*} catch (Exception ex) {
				frmInformacao objfrmInformacao = new frmInformacao(ex.Message);
				objfrmInformacao.ShowDialog();

			}*/

		}

		//Fecha a conexão
		public void FecharConexao()
		{
			if ((objConn != null)) {
				if (objConn.State == ConnectionState.Open) {
					objConn.Close();
				}
			}
		}


		public void BeginTrans()
		{
			//ABRE A CONEXÃO COM O BANCO. VAI FICAR ABERTA ATÉ O ROLLBACK OU COMMIT
			VerificarConexao();

			objTransacao = this.objConn.BeginTransaction(IsolationLevel.ReadCommitted);
			//MARCA QUE A TRANSAÇÃO FOI ABERTA
			blnTransAberta = true;
			//O STATUS INICIA, JÁ QUE NÃO FOI EXECUTADO NENHUM COMANDO É TRUE
			blnTransStatus = true;

		}

		/// <summary>
		/// faz o commit em uma transação
		/// </summary>
		/// <remarks></remarks>
		public void CommitTrans()
		{
			//se a transação ainda está aberta (não ocorreu erro) então faz commit

			if (blnTransAberta) {
				objTransacao.Commit();

				blnTransAberta = false;

			}

			//fecha a conexão com o banco de dados
			//FecharConexao()

		}


		public void RollBackTrans()
		{
			if (TransAberta) {
				objTransacao.Rollback();
			}

			//quando dá rollback a transação fica fechada e seu status não está OK.
			blnTransAberta = false;
			blnTransStatus = false;

			//AO FAZER ROLLBACK NÃO PRECISA FECHAR A CONEXÃO, POIS O ROLLBACK
			//APENAS INDICA QUE OS COMANDOS EXECUTADOS SERÃO DESFEITOS
			//FecharConexao()

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


