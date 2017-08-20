using System;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;
using TraderWizard.Enumeracoes;

namespace DataBase
{
    
	public class Command
	{

	    public int LinhasAfetadas { get; private set; }

	    public bool TransStatus => this.Conexao.TransStatus;

	    public bool TransAberta => this.Conexao.TransAberta;

	    public string UltimoComando { get; private set; }

		public Conexao Conexao { get; }

		public Command()
		{
			this.Conexao = new Conexao();

			//inicialização das propriedades
			this.UltimoComando = "";
			this.LinhasAfetadas = -1;

		}

		public Command(Conexao pobjConexao)
		{
			this.Conexao = pobjConexao;
			this.UltimoComando = "";
			this.LinhasAfetadas = -1;
		}

		//Fecha a conexão

	    private DbCommand CriarComando(string pstrComando)
		{

			VerificarConexao();

		    DbCommand cmd;

		    if (this.Conexao.BancoDeDados == cEnum.BancoDeDados.SqlServer)
		    {
                cmd = new  SqlCommand (pstrComando, (SqlConnection) this.Conexao.Conn);

		    }
		    else
		    {
                cmd = new OleDbCommand(pstrComando,(OleDbConnection)  this.Conexao.Conn);    
		    }

			

			cmd.Transaction = Conexao.Transacao;

			return cmd;

		}

		private void VerificarConexao()
		{
			this.Conexao.VerificarConexao();
		}


		public void Execute(string pstrComando)
		{
			DbCommand cmd = null;

		    if (!this.TransStatus) return;

		    try {

		        cmd = CriarComando(pstrComando);
		        LinhasAfetadas = cmd.ExecuteNonQuery();


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
		        this.UltimoComando = pstrComando;
		        cmd?.Dispose();
		    }
		}


		public void BeginTrans()
		{
			this.Conexao.BeginTrans();

		}

		public void CommitTrans()
		{
			//se a transação ainda está aberta (não ocorreu erro) então faz commit
			if (this.TransAberta) {
				Conexao.CommitTrans();
			}
		}

		public void RollBackTrans()
		{
			Conexao.RollBackTrans();
			LinhasAfetadas = -1;
		}


	}
}
