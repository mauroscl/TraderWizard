using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DataBase
{

	public class cRSList
	{


		private readonly Conexao objConexao;
		//Lista de queries que devem ser executadas

	    //Posição atual no lstDados.
		private int lngPosicaoAtual;
		//Lista dos resultados consultados no banco de dados

	    public cRSList(Conexao pobjConexao)
		{
			objConexao = pobjConexao;

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

				cRS objRS = new cRS(objConexao);

				//Para cada uma das queries da lista.

				foreach (string strQuery in Queries) {
					objRS.ExecuteQuery(strQuery);


					while (!objRS.EOF) {

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
				lngPosicaoAtual = 0;


			} catch (Exception ex) {
				objConexao.RollBackTrans();
				 MessageBox.Show(ex.Message, "Executar Query",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

		}

		public bool EOF {
			get { return (lngPosicaoAtual >= Dados.Count); }
		}

		public int RecordCount {
			get { return Dados.Count; }
		}

		public bool DadosExistir {
			get { return Dados.Count > 0; }
		}


		public object Field(string pstrCampo, object pobjRetornoErro = null)
		{


			try {
				if (Dados.Count > 0 && lngPosicaoAtual < Dados.Count) {
					//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
					return Dados[lngPosicaoAtual].SingleOrDefault(x => x.Key.ToLower() ==  pstrCampo.ToLower()).Value;
				} else {
					//Caso contrário retorna o erro.
					return pobjRetornoErro;
				}
			} catch (Exception) {
				return pobjRetornoErro;
			}

		}

        public Dictionary<string, object> RetornaLinhaAtual()
		{

			if (Dados.Count > 0 && lngPosicaoAtual < Dados.Count) {
				//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
				return Dados[lngPosicaoAtual];
			} else {
				//Caso contrário retorna o erro.
				return null;
			}

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
				if (Dados.Count > 0 && lngPosicaoAtual + 1 < Dados.Count) {
					//Se o List tem dados e não ultrapassou a última posição retorna o conteudo do campo
					return Dados[lngPosicaoAtual + 1][pstrCampo];
				}
			    //Caso contrário retorna o erro.
			    return pobjRetornoErro;
			} catch (Exception) {
				return pobjRetornoErro;
			}

		}

		public void MoveNext()
		{
			lngPosicaoAtual = lngPosicaoAtual + 1;
		}


	}
}
