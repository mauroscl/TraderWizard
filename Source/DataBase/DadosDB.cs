using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DataBase
{


	public class DadosDb
	{


		private readonly Conexao _conexao;
		//tabela em que os dados serão salvos ou consultados

		private readonly string _tabela;
		//collection que contém cada uma das operações que serão feitas no banco de dados.
		//cada operação pode ter um ou mais itens para serem salvos
		//Private colRegistro As Collection

		//collection de item a serem salvos no banco de dados.
		//cada item é uma estrutura do tipo frwInterface.structDadosDB

		private Dictionary<string, CampoDb> _campos;

		public DadosDb(Conexao pobjConexao, string pstrTabela)
		{
			_conexao = pobjConexao;

			_tabela = pstrTabela;

			//colRegistro = New Collection

            _campos = new Dictionary<string, CampoDb>();

		}

		public bool CamposLimpar()
		{

			try {
				_campos.Clear();
                return true;
			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
			}

		}

		public bool CampoAdicionar(string pstrCampo, bool pblnChave, string pstrValor)
		{
			bool retorno;


		    try {
				var campoDb = new CampoDb(pstrCampo, pblnChave, pstrValor);

                _campos.Add(pstrCampo, campoDb);

				retorno = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				retorno = false;

			}
			return retorno;

		}

		private bool RegistroExistir(string pstrWhere)
		{
		    RS objRS = new RS(_conexao);

			objRS.ExecuteQuery(" SELECT 1 " + " FROM " + _tabela + " WHERE " + pstrWhere);

			var existemDados = objRS.DadosExistir;

			objRS.Fechar();
			return existemDados;

		}

		public bool DadosBDSalvar()
		{
			bool functionReturnValue;

			Command objCommand = new Command(_conexao);

		    string strQuery = String.Empty;

            string strCampos = String.Empty;

            string strValoresINSERT = String.Empty;

            string strValoresUPDATE = String.Empty;

            string strWhere = String.Empty;

            string strOperacao = String.Empty;

			try
			{
			    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

				foreach (CampoDb objCampoDB_loopVariable in _campos.Values) {
					var objCampoDB = objCampoDB_loopVariable;

					if (strValoresINSERT != String.Empty) {
						strValoresINSERT = strValoresINSERT + ", ";

					}

					//TRATAMENTO PARA OS CAMPOS É ÚNICO, POIS O UPDATE NÃO UTILIZARÁ ESTA VARIÁVEL

					if (strCampos != String.Empty) {
						strCampos = strCampos + ", ";

					}

					strCampos = strCampos + objCampoDB.Campo;


					strValoresINSERT = strValoresINSERT + funcoesBd.CampoStringFormatar(objCampoDB.Valor);

					//TRATAMENTO PARA O UPDATE. TEM QUE SEPARAR OS CAMPOS EM CHAVE
					//E OS CAMPOS QUE SERÃO ATUALIZADOS.

					if (objCampoDB.Chave) {

						if (strWhere != String.Empty) {
							strWhere = strWhere + " and ";

						}

						strWhere = strWhere + objCampoDB.Campo + " = " + funcoesBd.CampoStringFormatar(objCampoDB.Valor);


					} else {

						if (strValoresUPDATE != String.Empty) {
							strValoresUPDATE = strValoresUPDATE + ", ";

						}

						strValoresUPDATE = strValoresUPDATE + objCampoDB.Campo + " = " + funcoesBd.CampoStringFormatar(objCampoDB.Valor);

					}
					//FIM DO TRATAMENTO PARA O UPDATE

				}
				//fim da collection de campos

				//verifica se o registro já existe
				if (RegistroExistir(strWhere)) {
					//se o registro já existe tem que fazer UPDATE
					strOperacao = "UPDATE";
				} else {
					//se o registro ainda não existe tem que fazer INSERT
					strOperacao = "INSERT";
				}

				//monta o comando de acordo com a operação.

				if (strOperacao == "INSERT") {
					strQuery = " INSERT INTO " + _tabela + "(" + strCampos + ")" + " VALUES " + "(" + strValoresINSERT + ")";


				} else {
					//se é um UPDATE
					strQuery = " UPDATE " + _tabela + " SET " + strValoresUPDATE + " WHERE " + strWhere;

				}

				//executa o comando no banco de dados
				objCommand.Execute(strQuery);

				functionReturnValue = objCommand.TransStatus;

			} catch (Exception) {
				functionReturnValue = false;


			} finally {
				//limpa a collection de campo, pois depois de salvar o campo pode ser usado novamente
				_campos = null;

			}
			return functionReturnValue;

			//objCommand.CommitTrans()


			//DadosBDSalvar = objCommand.TransStatus

		}

		public bool DadosBDConsultar()
		{
			bool functionReturnValue;

			string strCampo = String.Empty;

			string strWhere = String.Empty;

			RS objRS = new RS(_conexao);

		    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

		    try {

				foreach (CampoDb objCampoDB in _campos.Values) {

					if (objCampoDB.Chave) {
						//campos chave vão para o WHERE
						if (strWhere != String.Empty) {
							strWhere = strWhere + " and ";
						}

						strWhere = strWhere + objCampoDB.Campo + " = " + funcoesBd.CampoStringFormatar(objCampoDB.Valor);


					} else {
						//campos que não são chave vão para o SELECT

						if (strCampo != String.Empty) {
							strCampo = strCampo + ", ";

						}

						strCampo = strCampo + objCampoDB.Campo;

					}

				}

				//consulta o valor do campo no banco de dados
				objRS.ExecuteQuery(" SELECT " + strCampo + " FROM " + _tabela + " WHERE " + strWhere);

				//atualiza o valor do campo na collection de dados.

                foreach (KeyValuePair<string, CampoDb> objCampoDB in _campos)
                {

					if (!objCampoDB.Value.Chave) {
						objCampoDB.Value.Valor = Convert.ToString( objRS.Field(objCampoDB.Value.Campo, String.Empty));

					}

                    

				}

				functionReturnValue = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;


			} finally {

				objRS.Fechar();

			}
			return functionReturnValue;

		}

		public string CampoConsultar(string pstrCampo)
		{
			try 
            {
				return _campos.SingleOrDefault(x => x.Key == pstrCampo).Value.Valor;
			} 
            catch (Exception) 
            {
				return String.Empty;

			}
		}

	}
}
