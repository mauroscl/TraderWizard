using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DataBase
{

	public class RSList
	{


		private readonly Conexao _conexao;
		//Lista de queries que devem ser executadas

	    //Posição atual no lstDados.
		private int _posicaoAtual;
		//Lista dos resultados consultados no banco de dados

	    public RSList(Conexao pobjConexao)
		{
			_conexao = pobjConexao;

			Queries = new List<string>();

		}

	    public List<string> Queries { get; set; }

        public IList<Dictionary<string, object>> Dados { get; private set; }

	    public void AdicionarQuery(string pstrQuery)
		{
			Queries.Add(pstrQuery);
		}


		public void ExecuteQuery()
		{

			try {
                Dados = new List<Dictionary<string, object>>();

				RS objRS = new RS(_conexao);

				//Para cada uma das queries da lista.

				foreach (string strQuery in Queries) {
					objRS.ExecuteQuery(strQuery);


					while (!objRS.Eof) {

                        Dictionary<string, object> colunas = new Dictionary<string, object>();

						for (int intI = 0; intI <= objRS.GetFieldCount() - 1; intI++) {
							//Adiciona cada uma das colunas na collection e coloca como chave do item o nome da coluna
                            colunas.Add(objRS.GetName(intI), objRS.Field(intI));
						}

                        Dados.Add(colunas);

                        objRS.MoveNext();
					}

					objRS.Fechar();

				}

				//Inicializa variável que indica a posição atual da lista para o primeiro item.
				_posicaoAtual = 0;


			} catch (Exception ex) {
				_conexao.RollBackTrans();
				 MessageBox.Show(ex.Message, "Executar Query",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

		}

		public bool Eof => (_posicaoAtual >= Dados.Count);

	    public int RecordCount => Dados.Count;

	    public bool DadosExistir => Dados.Count > 0;

	    public object Field(string pstrCampo, object pobjRetornoErro = null)
		{
			try {
				if (Dados.Count > 0 && _posicaoAtual < Dados.Count) {
					//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
					return Dados[_posicaoAtual].SingleOrDefault(x => x.Key.ToLower() ==  pstrCampo.ToLower()).Value;
				}
			    //Caso contrário retorna o erro.
			    return pobjRetornoErro;
			} catch (Exception) {
				return pobjRetornoErro;
			}

		}

        public Dictionary<string, object> RetornaLinhaAtual()
		{

			if (Dados.Count > 0 && _posicaoAtual < Dados.Count) {
				//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
				return Dados[_posicaoAtual];
			}
		    //Caso contrário retorna o erro.
		    return null;
		}

		/// <summary>
		/// Obtém o próximo registro, caso exista, sem mover o recordset para a posição seguinte
		/// </summary>
		/// <param name="pstrCampo"></param>
		/// <param name="pobjRetornoErro"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public object NextField(string pstrCampo, object pobjRetornoErro = null)
		{

			try {
				if (Dados.Count > 0 && _posicaoAtual + 1 < Dados.Count) {
					//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
					return Dados[_posicaoAtual + 1][pstrCampo];
				}
			    //Caso contrário retorna o erro.
			    return pobjRetornoErro;
			} catch (Exception) {
				return pobjRetornoErro;
			}

		}

		public void MoveNext()
		{
			_posicaoAtual = _posicaoAtual + 1;
		}


	}
}
