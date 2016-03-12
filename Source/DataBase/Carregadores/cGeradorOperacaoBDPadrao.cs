using System.Collections.Generic;
using prjDominio.Entidades;
using prjModelo.Carregadores;

namespace DataBase.Carregadores
{

	public abstract class cGeradorOperacaoBDPadrao
	{

		public Conexao Conexao { get; set; }
		public IList<OperacaoDeBancoDeDados> Operacoes { get; set; }
		protected IList<cGeradorOperacaoBDPadrao> GeradoresFilhos { get; set; }

		public cGeradorOperacaoBDPadrao(Conexao pobjConexao)
		{
			Conexao = pobjConexao;
			Operacoes = new List<OperacaoDeBancoDeDados>();
			GeradoresFilhos = new List<cGeradorOperacaoBDPadrao>();
		}

		public virtual void Adicionar(cModelo pobjModelo, string pstrComando)
		{
			Operacoes.Add(new OperacaoDeBancoDeDados(pobjModelo, pstrComando));
		}

		public void AdicionarGeradorFilho(cGeradorOperacaoBDPadrao pobjItem)
		{
			GeradoresFilhos.Add(pobjItem);
		}

		public abstract string GeraInsert(cModelo pobjModelo);

		public abstract string GeraUpdate(cModelo pobjModelo);

		public bool Executar()
		{
		    cCommand objCommand = new cCommand(this.Conexao);

			foreach (OperacaoDeBancoDeDados item in this.Operacoes) {
			    string strComando;
			    if (item.Comando.ToUpper() == "INSERT") {
					strComando = GeraInsert(item.Modelo);
				} else if (item.Comando.ToUpper() == "UPDATE") {
					strComando = GeraUpdate(item.Modelo);
				} else {
					strComando = string.Empty;
				}


				if (strComando != string.Empty) {
					objCommand.Execute(strComando);

				}

				//Trace.WriteLine(strComando)

			}


			foreach (cGeradorOperacaoBDPadrao objGerador in GeradoresFilhos) {
				objGerador.Executar();

			}

			return this.Conexao.TransStatus;

		}

	}
}
