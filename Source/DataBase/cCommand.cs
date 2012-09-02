using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
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

			bool blnContinuarExecutando = true;

			int intNumeroDeExecucoes = 0;


			while (blnContinuarExecutando) {
				//só executa o comando se o status da transação está OK.

				if (this.TransStatus) {

					try {
						intNumeroDeExecucoes = intNumeroDeExecucoes + 1;


						if (blnContinuarExecutando) {

							if (intNumeroDeExecucoes > 10) {
								blnContinuarExecutando = false;

							}

						}


						if (intNumeroDeExecucoes > 1) {
							Trace.WriteLine("Comando: " + pstrComando + " - Tentativas: " + intNumeroDeExecucoes.ToString());

						}

						cmd = CriarComando(pstrComando);
						intLinhasAfetadas = cmd.ExecuteNonQuery();
						blnContinuarExecutando = false;


					} catch (InvalidOperationException ex) {
						RollBackTrans();

                        MessageBox.Show(ex.Message, "Executar Comando", MessageBoxButtons.OK,MessageBoxIcon.Error);


					} catch (OleDbException ex) {

						if (ex.ErrorCode == -2147467259 & blnContinuarExecutando) {
							//Erro de objeto bloqueado
							System.Threading.Thread.Sleep(1000);


						} else {
							RollBackTrans();

							frmInformacao objfrmInformacao = new frmInformacao("Erro - Código: " + ex.ErrorCode.ToString() + " - Descrição: " + ex.Message + Environment.NewLine + " - Query: " + pstrComando);

							objfrmInformacao.ShowDialog();

						}


					} catch (Exception ex) {
						RollBackTrans();

                        MessageBox.Show(ex.Message, "Executar Comando", MessageBoxButtons.OK, MessageBoxIcon.Error);


					} finally {
						//último comando executado
						this.strUltimoComando = pstrComando;
						cmd.Dispose();
					}

				}

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

			//marca a conexão para ser mantida até que a transação seja encerrada
			//Me.ManterConexao = False

			//ALTERADO POR MAURO, 11/08/2009
			//NÃO PRECISA FECHAR A CONEXÃO AO FAZER ROLLBACK
			//fecha a conexão com o banco de dados
			//FecharConexao()

		}


		//Public Sub ExecuteQuery(ByVal pstrQuery As String)

		//    Dim cmd As OracleCommand

		//    Try

		//        'executa a query somente se a transação está OK.
		//        'Caso esta query esteja sendo executada fora de uma transação, 
		//        'a propriedade blnTransStatus vai estar sempre True e não haverá problema
		//        If Me.TransStatus Then

		//            cmd = CriarComando(pstrQuery)
		//            objOracleDR = cmd.ExecuteReader()
		//            blnQueryStatus = True

		//            'Chama o método read para o primeiro registro ficar disponível
		//            'Se consegue ler, é porque tem dados
		//            blnDadosExistir = objOracleDR.Read()
		//            'quando não conseguir mais ler nenhum registro chegou ao fim (EOF).
		//            blnEOF = Not blnDadosExistir

		//        End If

		//    Catch ex As OracleException

		//        If Me.TransAberta Then
		//            RollBackTrans()
		//        End If

		//        blnQueryStatus = False
		//        blnDadosExistir = False
		//        blnEOF = True

		//        MsgBox("Erro: " & ex.Number & " - Descrição: " & ex.Message & " - Query: " & pstrQuery _
		//        , MsgBoxStyle.Critical, "Erro de Banco de Dados")

		//    Finally

		//        strUltimoComando = pstrQuery

		//    End Try

		//End Sub

		//Public Sub MoveNext()

		//    blnEOF = Not objOracleDR.Read()

		//End Sub

		//Public Function Field(ByVal pstrCampo As String, Optional ByVal pstrRetornoErro As String = "") As String

		//    Dim intOrdinal As Integer

		//    Dim Tipo As Type

		//    Try
		//        If blnTransStatus Then
		//            intOrdinal = objOracleDR.GetOrdinal(pstrCampo)
		//        End If
		//    Catch e As IndexOutOfRangeException
		//        MsgBox("Erro ao consultar Campo: " & e.Message, MsgBoxStyle.Critical, "Executar Query")
		//        Return pstrRetornoErro
		//        Exit Function

		//    Catch e As System.NullReferenceException
		//        MsgBox("Erro ao consultar Campo: " & e.Message, MsgBoxStyle.Critical, "Executar Query")
		//        Return pstrRetornoErro
		//        Exit Function

		//    End Try

		//    If blnTransStatus Then

		//        Tipo = objOracleDR.GetFieldType(intOrdinal)

		//        If Not objOracleDR.IsDBNull(intOrdinal) Then

		//            If Tipo.Equals(Type.GetType("System.Boolean", True, True)) Then
		//                Return CStr(objOracleDR.GetBoolean(intOrdinal))
		//            ElseIf Tipo.Equals(Type.GetType("System.Int16", True, True)) Then
		//                Return CStr(objOracleDR.GetInt16(intOrdinal))
		//            ElseIf Tipo.Equals(Type.GetType("System.String", True, True)) Then
		//                Return CStr(objOracleDR.GetString(intOrdinal))
		//            ElseIf Tipo.Equals(Type.GetType("System.Decimal", True, True)) Then
		//                Return CStr(objOracleDR.GetDecimal(intOrdinal))
		//            ElseIf Tipo.Equals(Type.GetType("System.DateTime", True, True)) Then
		//                Return CStr(objOracleDR.GetDateTime(intOrdinal))
		//            End If

		//            Return objOracleDR.GetString(intOrdinal)
		//        Else
		//            Return pstrRetornoErro
		//        End If

		//    Else
		//        Return pstrRetornoErro
		//    End If

		//End Function


		/*protected override void Finalize()
		{
			//FecharConexao()
			base.Finalize();
		}*/
	}
}
