using System;
using System.Data.OleDb;
using System.Windows.Forms;
namespace DataBase
{

	public class cRS
	{

	    public string UltimaQuery { get; private set; }

	    public bool EOF { get; private set; }

	    public bool DadosExistir { get; private set; }

	    public bool QueryStatus { get; private set; }

	    public cConexao Conexao { get; private set; }

	    public OleDbDataReader GetDataReader { get; private set; }

	    public string GetName(int pintIndice) {
			return GetDataReader.GetName(pintIndice);
		}


		public cRS()
		{
			this.Conexao = new cConexao();

			ValorPadraoInicializar();

		}


		public cRS(cConexao pobjConexao)
		{
			this.Conexao = pobjConexao;

			ValorPadraoInicializar();

		}

		private void ValorPadraoInicializar()
		{
			//inicialização das propriedades
			this.UltimaQuery = "";
			EOF = false;
			DadosExistir = false;
			QueryStatus = false;

		}


		private OleDbCommand CriarComando(string pstrComando)
		{

			VerificarConexao();

			var cmd = new OleDbCommand(pstrComando, this.Conexao.Conn);

			cmd.Transaction = Conexao.Transacao;

			return cmd;

		}


		public void ExecuteQuery(string pstrQuery)
		{
		    bool blnContinuarExecutando = true;

			int intNumeroDeExecucoes = 0;

			while (blnContinuarExecutando) {

				try
				{
				    //executa a query somente se a transação está OK
					//ou ainda se não tem transação aberta.
				    bool blnExecutar = !Conexao.TransAberta || Conexao.TransStatus;


				    if (blnExecutar) {
						intNumeroDeExecucoes = intNumeroDeExecucoes + 1;

						OleDbCommand objOleDbCommand = CriarComando(pstrQuery);
						GetDataReader = objOleDbCommand.ExecuteReader();
						QueryStatus = true;

						//Chama o método read para o primeiro registro ficar disponível
						//Se consegue ler, é porque tem dados
						//blnDadosExistir = objOleDbDR.Read()
						GetDataReader.Read();

						DadosExistir = GetDataReader.HasRows;

						//----fim do código alterado por mauro, 26/07/2009
						//quando não conseguir mais ler nenhum registro chegou ao fim (EOF).
						EOF = !DadosExistir;

						blnContinuarExecutando = false;

					}
				}
				catch
                {
                    RollBackTrans();

                    QueryStatus = false;
                    DadosExistir = false;
                    EOF = true;
                    throw;

                }
                
                finally {
					UltimaQuery = pstrQuery;

				}

			}

		}

		public object Field(string pstrCampo, object pobjRetornoErro = null)
		{
			object functionReturnValue;


		    try
		    {
		        //executa a query somente se a transação está OK
				//ou ainda se não tem transação aberta.
		        bool blnExecutar = !Conexao.TransAberta || Conexao.TransStatus;


		        if (blnExecutar)
				{
				    int intOrdinal = GetDataReader.GetOrdinal(pstrCampo);

				    functionReturnValue = GetField(intOrdinal, pobjRetornoErro);
				}
				else {
					functionReturnValue = pobjRetornoErro;

				}
		    }
		    catch (IndexOutOfRangeException e) {
				RollBackTrans();
                MessageBox.Show("Erro ao consultar o campo " + pstrCampo + " - " + e.Message, "Executar Query", MessageBoxButtons.OK, MessageBoxIcon.Error);


				return pobjRetornoErro;


			} catch (NullReferenceException e) {
				RollBackTrans();
                MessageBox.Show("Erro ao consultar o campo " + pstrCampo + " - " + e.Message, "Executar Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return pobjRetornoErro;


			} catch (InvalidOperationException e) {
				RollBackTrans();
                MessageBox.Show(e.Message, "Executar Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return pobjRetornoErro;

			}
			return functionReturnValue;

		}

		public object Field(int pintOrdinal, object pobjRetornoErro = null)
		{

			return GetField(pintOrdinal, pobjRetornoErro);

		}

		private object GetField(int pintOrdinal, object pobjRetornoErro = null)
		{
		    //executa a query somente se a transação está OK
			//ou ainda se não tem transação aberta.
			bool blnExecutar = !Conexao.TransAberta || Conexao.TransStatus;

		    if (!blnExecutar)
		    {
		        return pobjRetornoErro;
		    }

		    Type tipo = GetDataReader.GetFieldType(pintOrdinal);

		    try
		    {
		        if ( ! EOF && !GetDataReader.IsDBNull(pintOrdinal)) {
		            if (tipo == Type.GetType("System.Boolean", true, true)) {
		                return GetDataReader.GetBoolean(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Int16", true, true)) {
		                return GetDataReader.GetInt16(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Int32", true, true)) {
		                return GetDataReader.GetInt32(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.String", true, true)) {
		                return GetDataReader.GetString(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Decimal", true, true)) {
		                return GetDataReader.GetDecimal(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Double", true, true)) {
		                return GetDataReader.GetDouble(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.DateTime", true, true)) {
		                return GetDataReader.GetDateTime(pintOrdinal);
		            }

		            return GetDataReader.GetString(pintOrdinal);

		        }
		        return pobjRetornoErro;
		    }
		    catch (InvalidOperationException) {
		        //Esta exceção geralmente acontece quando a query não retornou dados e alguma 
		        //coluna está sendo consultada
		        return pobjRetornoErro;

		    }
		}

		public int GetValues(object[] parrValues)
		{
			return GetDataReader.GetValues(parrValues);
		}


		public void MoveNext()
		{

			try {

				if (Conexao.TransStatus) {
					if ((GetDataReader != null)) {
						EOF = !GetDataReader.Read();
					} else {
						//se o RS não está preenchido, então considera como se tivesse chegado no fim deste
						//para que a aplicação não fique em loop
						EOF = true;
					}


				} else {
					//Se ocorre algum erro de o transstatus estar com erro então considera que chegou ao fim do RS
					EOF = true;

				}

			} catch (InvalidOperationException) {
                MessageBox.Show("Não foi possível ler o próximo registro.", "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} catch (Exception ex) {
                MessageBox.Show("Não foi possível ler o próximo registro. Erro: " + ex.Message, "Banco de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

		}

		private void VerificarConexao()
		{
			this.Conexao.VerificarConexao();
		}

		private void RollBackTrans()
		{
			if (this.Conexao.TransAberta) {
				Conexao.RollBackTrans();
			}

			//comentado por mauro, 29/04/2010
			//não precisa fechar a conexão quando faz rollback.

			//fecha a conexão com o banco de dados
			//FecharConexao()

		}

		/// <summary>
		/// Calcula o código sequencial seguinte para uma tabela
		/// </summary>
		/// <param name="pstrTabela">Nome da tabela</param>
		/// <param name="pstrCampo">Nome do campo</param>
		/// <param name="pstrWhereAdicional">WHERE adicional utilizado na busca do próximo código.
		/// Geralmente é utilizado quando a chave é composta de mais de um campo e já temos os outros
		/// campos calculados</param>
		/// <returns>retorna o valor máximo do campo recebido por parâmetro adicionado de um</returns>
		/// <remarks></remarks>
		public long CodigoSequencialCalcular(string pstrTabela, string pstrCampo, string pstrWhereAdicional)
		{
		    string strQuery = " SELECT MAX(" + pstrCampo + ") AS ValorMaximo " + " FROM " + pstrTabela;


			if (pstrWhereAdicional != String.Empty) {
				strQuery = strQuery + " WHERE " + pstrWhereAdicional;

			}

			ExecuteQuery(strQuery);

			//soma 1 no valor encontrado.
			long sequencial = Convert.ToInt64(Field("ValorMaximo", 0)) + 1;

			Fechar();
			return sequencial;

		}

		/// <summary>
		/// Fecha o RECORDSET. Utilizado para liberar recursos do banco
		/// </summary>
		/// <remarks></remarks>

		public void Fechar()
		{

			if ((GetDataReader != null)) {
				if (!GetDataReader.IsClosed) {
					GetDataReader.Close();
					//removido por mauro, 17/04/2010
					//GUIDAtualizar()
					//fim do código removido por mauro, 17/04/2010
				}

			}

		}


		public int GetFieldCount()
		{

			return GetDataReader.FieldCount;

		}

	}
}
