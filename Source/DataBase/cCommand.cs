using System;
using System.Data.OleDb;
using System.Windows.Forms;
namespace DataBase
{


	public class cCommand
	{

		//objeto da conexao

		private cConexao objConexao;
		//objeto da transação
		//Private objTransacao As OracleTransaction

		//Recordset
		//Private objOracleDR As OracleDataReader

		//indica se já chegou no fim do RS
		//Private blnEOF As Boolean

		//indica se a query retornou algum dado
		//Private blnDadosExistir As Boolean

		//status da execução da consulta
		//Private blnQueryStatus As Boolean

		//Número de linhas afetadas pela execução do último comando

		private int intLinhasAfetadas;
		//armazena a última query executada

		private string strUltimoComando;
		//Private blnTransStatus As Boolean

		//Private blnTransAberta As Boolean

		//String de conexão com o BD
		//Protected strConnectionString As String

		//Indica se é para manter a conexão aberta
		//Private blnManterConexao As Boolean



		//Property Get e Set da ConnectionString
		//Public Property ConnectionString() As String
		//    Get
		//        Return Me.strConnectionString
		//    End Get
		//    Set(ByVal Value As String)
		//        Me.strConnectionString = Value
		//    End Set
		//End Property

		public int LinhasAfetadas {
			get { return intLinhasAfetadas; }
		}

		public bool TransStatus {
			get { return this.objConexao.TransStatus; }
		}

		public bool TransAberta {
			get { return this.objConexao.TransAberta; }
		}

		public string UltimoComando {
			get { return strUltimoComando; }
		}

		//propriedade muito importante. Retorna a conexao
		public cConexao Conexao {
			get { return this.objConexao; }
		}

		//Public ReadOnly Property EOF() As Boolean
		//    Get
		//        Return blnEOF
		//    End Get
		//End Property

		//Public ReadOnly Property DadosExistir()
		//    Get
		//        Return blnDadosExistir
		//    End Get
		//End Property

		//Public ReadOnly Property QueryStatus() As Boolean
		//Get
		//    Return Me.blnQueryStatus
		//End Get
		//End Property

		//Property Get e Set ManterConexao
		//Public Property ManterConexao() As Boolean
		//    Get
		//        Return Me.blnManterConexao
		//    End Get
		//    Set(ByVal Value As Boolean)
		//        Me.blnManterConexao = Value
		//    End Set
		//End Property

		//constrututor passando somente a connectionStrin
		public cCommand()
		{
			//Me.ConnectionString = "user id=mscl;data source=ORCL;password=mscl"

			this.objConexao = new cConexao();

			//inicialização das propriedades
			this.strUltimoComando = "";
			this.intLinhasAfetadas = -1;

			//aqui pode inicializa com os valores padrões, mesmo sem buscar as propriedades da conexão
			//Me.blnTransAberta = False
			//Me.blnTransStatus = True
			//Me.ManterConexao = False

		}

		public cCommand(cConexao pobjConexao)
		{
			this.objConexao = pobjConexao;
			this.strUltimoComando = "";
			this.intLinhasAfetadas = -1;
		}

		//Fecha a conexão
		private void FecharConexao()
		{
			//If Not (objConexao Is Nothing) Then
			//    If objConexao.State = ConnectionState.Open Then
			//        objConexao.Close()
			//    End If
			//End If
			this.objConexao.FecharConexao();
		}

		private OleDbCommand CriarComando(string pstrComando)
		{

			VerificarConexao();

			OleDbCommand cmd = new OleDbCommand(pstrComando, this.objConexao.Conn);

			cmd.Transaction = objConexao.Transacao;

			return cmd;

		}

		private void VerificarConexao()
		{
			//If objConexao Is Nothing Then
			//    objConexao = New OracleConnection(Me.ConnectionString)
			//    objConexao.Open()
			//Else
			//    If objConexao.State <> ConnectionState.Open Then
			//        objConexao.Open()
			//    End If
			//End If
			this.objConexao.VerificarConexao();
		}


		public void Execute(string pstrComando)
		{
			OleDbCommand cmd = null;

		    if (!this.TransStatus) return;

		    try {

		        cmd = CriarComando(pstrComando);
		        intLinhasAfetadas = cmd.ExecuteNonQuery();


		    } catch (InvalidOperationException ex) {
		        RollBackTrans();

		        MessageBox.Show(ex.Message, "Executar Comando", MessageBoxButtons.OK,MessageBoxIcon.Error);


		    } catch (OleDbException ex) {

		        RollBackTrans();

		        var objfrmInformacao = new frmInformacao("Erro - Código: " + ex.ErrorCode + 
                    " - Descrição: " + ex.Message + Environment.NewLine + 
                    " - Query: " + pstrComando);

		        objfrmInformacao.ShowDialog();


		    } catch (Exception ex) {
		        RollBackTrans();

		        MessageBox.Show(ex.Message, "Executar Comando", MessageBoxButtons.OK, MessageBoxIcon.Error);


		    } finally
		    {
		        //último comando executado
		        this.strUltimoComando = pstrComando;
		        if (cmd != null) cmd.Dispose();
		    }
		}


		public void BeginTrans()
		{
			this.objConexao.BeginTrans();

		}

		public void CommitTrans()
		{
			//se a transação ainda está aberta (não ocorreu erro) então faz commit
			if (this.TransAberta) {
				objConexao.CommitTrans();
				//quando dá commit a transação está fechada
				//blnTransAberta = False

			}

			//marca a conexão para ser mantida até que a transação seja encerrada
			//Me.ManterConexao = False

			//fecha a conexão com o banco de dados
			//FecharConexao()

		}


		public void RollBackTrans()
		{
			objConexao.RollBackTrans();
			//quando dá rollback a transação fica fechada e seu status não está OK.
			//blnTransAberta = False
			//blnTransStatus = False
			intLinhasAfetadas = -1;

		}


	}
}
