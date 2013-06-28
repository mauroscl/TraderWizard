using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Data.OleDb;
using System.Windows.Forms;
namespace DataBase
{

	public class cRS
	{

		//objeto da conexao

		private cConexao objConexao;

		private OleDbDataReader objOleDbDR;
		//indica se já chegou no fim do RS

	    //indica se a query retornou algum dado

	    //status da execução da consulta

		private bool blnQueryStatus;
		//armazena a última query executada

		private string strUltimaQuery;
		//Private objOleDbCommand As OleDbCommand

		//GUID gerado a cada execução de query.

	    public string UltimaQuery {

			get { return strUltimaQuery; }
		}

	    public bool EOF { get; private set; }

	    public bool DadosExistir { get; private set; }

	    public bool QueryStatus {
			get { return this.blnQueryStatus; }
		}

		public cConexao Conexao {
			get { return objConexao; }
		}

		public OleDbDataReader GetDataReader {
			get { return objOleDbDR; }
		}

		public string GetName(int pintIndice) {
			return objOleDbDR.GetName(pintIndice);
		}


		public cRS()
		{
			this.objConexao = new cConexao();

			ValorPadraoInicializar();

		}


		public cRS(cConexao pobjConexao)
		{
			this.objConexao = pobjConexao;

			ValorPadraoInicializar();

		}


		private void ValorPadraoInicializar()
		{
			//inicialização das propriedades
			this.strUltimaQuery = "";
			EOF = false;
			DadosExistir = false;
			blnQueryStatus = false;

		}


		private OleDbCommand CriarComando(string pstrComando)
		{

			VerificarConexao();

			OleDbCommand cmd = new OleDbCommand(pstrComando, this.objConexao.Conn);

			cmd.Transaction = objConexao.Transacao;

			return cmd;

		}


		public void ExecuteQuery(string pstrQuery)
		{
			OleDbCommand objOleDbCommand = null;

			bool blnExecutar = false;

			bool blnContinuarExecutando = true;

			int intNumeroDeExecucoes = 0;


			while (blnContinuarExecutando) {

				try {
					//executa a query somente se a transação está OK
					//ou ainda se não tem transação aberta.
					if (objConexao.TransAberta) {
						blnExecutar = objConexao.TransStatus;
					} else {
						blnExecutar = true;
					}


					if (blnExecutar) {

						//removido por mauro, 17/04/2010

						//GERA O GUID
						//strGuid = Guid.NewGuid().ToString()

						//INSERE O REGISTRO NA TABELA DE CONTROLE
						//GUIDInserir()

						//fim do código removido por mauro, 17/04/2010.

						intNumeroDeExecucoes = intNumeroDeExecucoes + 1;


						if (blnContinuarExecutando) {

							if (intNumeroDeExecucoes > 10) {
								blnContinuarExecutando = false;

							}

						}


						if (intNumeroDeExecucoes > 1) {
							Trace.WriteLine("Comando: " + pstrQuery + " - Tentativas: " + intNumeroDeExecucoes.ToString());

						}

						objOleDbCommand = CriarComando(pstrQuery);
						objOleDbDR = objOleDbCommand.ExecuteReader();
						blnQueryStatus = true;

						//Chama o método read para o primeiro registro ficar disponível
						//Se consegue ler, é porque tem dados
						//blnDadosExistir = objOleDbDR.Read()
						objOleDbDR.Read();

						DadosExistir = objOleDbDR.HasRows;

						//----fim do código alterado por mauro, 26/07/2009
						//quando não conseguir mais ler nenhum registro chegou ao fim (EOF).
						EOF = !DadosExistir;

						blnContinuarExecutando = false;

					}

				} 
                catch
                {
                    RollBackTrans();

                    blnQueryStatus = false;
                    DadosExistir = false;
                    EOF = true;
                    throw;

                }
                
                finally {
					strUltimaQuery = pstrQuery;

				}

			}

		}

		public object Field(string pstrCampo, object pobjRetornoErro = null)
		{
			object functionReturnValue;


		    try {
				//executa a query somente se a transação está OK
				//ou ainda se não tem transação aberta.
			    bool blnExecutar;
			    if (objConexao.TransAberta) {
					blnExecutar = objConexao.TransStatus;
				} else {
					blnExecutar = true;
				}


				if (blnExecutar)
				{
				    int intOrdinal = objOleDbDR.GetOrdinal(pstrCampo);

				    functionReturnValue = GetField(intOrdinal, pobjRetornoErro);
				}
				else {
					functionReturnValue = pobjRetornoErro;

				}

			} catch (IndexOutOfRangeException e) {
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
			bool blnExecutar = !objConexao.TransAberta || objConexao.TransStatus;

		    if (!blnExecutar)
		    {
		        return pobjRetornoErro;
		    }

		    Type tipo = objOleDbDR.GetFieldType(pintOrdinal);

		    try
		    {
		        if ( ! EOF && !objOleDbDR.IsDBNull(pintOrdinal)) {
		            if (tipo == Type.GetType("System.Boolean", true, true)) {
		                return objOleDbDR.GetBoolean(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Int16", true, true)) {
		                return objOleDbDR.GetInt16(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Int32", true, true)) {
		                return objOleDbDR.GetInt32(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.String", true, true)) {
		                return objOleDbDR.GetString(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Decimal", true, true)) {
		                return objOleDbDR.GetDecimal(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.Double", true, true)) {
		                return objOleDbDR.GetDouble(pintOrdinal);
		            }
		            if (tipo == Type.GetType("System.DateTime", true, true)) {
		                return objOleDbDR.GetDateTime(pintOrdinal);
		            }

		            return objOleDbDR.GetString(pintOrdinal);

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
			return objOleDbDR.GetValues(parrValues);
		}


		public void MoveNext()
		{

			try {

				if (objConexao.TransStatus) {
					if ((objOleDbDR != null)) {
						EOF = !objOleDbDR.Read();
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
			this.objConexao.VerificarConexao();
		}

		//Fecha a conexão
		private void FecharConexao()
		{
			this.objConexao.FecharConexao();
		}


		private void RollBackTrans()
		{
			if (this.objConexao.TransAberta) {
				objConexao.RollBackTrans();
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
			long functionReturnValue = 0;

			string strQuery = null;

			strQuery = " SELECT MAX(" + pstrCampo + ") AS ValorMaximo " + " FROM " + pstrTabela;


			if (pstrWhereAdicional != String.Empty) {
				strQuery = strQuery + " WHERE " + pstrWhereAdicional;

			}

			ExecuteQuery(strQuery);

			//soma 1 no valor encontrado.
			functionReturnValue = Convert.ToInt64(Field("ValorMaximo", 0)) + 1;

			Fechar();
			return functionReturnValue;

		}

		/// <summary>
		/// Fecha o RECORDSET. Utilizado para liberar recursos do banco
		/// </summary>
		/// <remarks></remarks>

		public void Fechar()
		{

			if ((objOleDbDR != null)) {
				if (!objOleDbDR.IsClosed) {
					objOleDbDR.Close();
					//removido por mauro, 17/04/2010
					//GUIDAtualizar()
					//fim do código removido por mauro, 17/04/2010
				}

			}

		}


		public int GetFieldCount()
		{

			return objOleDbDR.FieldCount;

		}

	}
}
