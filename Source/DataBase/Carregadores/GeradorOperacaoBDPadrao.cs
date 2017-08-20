using System.Collections.Generic;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public abstract class GeradorOperacaoBDPadrao
	{

		public Conexao Conexao { get; set; }
		public IList<OperacaoDeBancoDeDados> Operacoes { get; set; }
		protected IList<GeradorOperacaoBDPadrao> GeradoresFilhos { get; set; }

		public GeradorOperacaoBDPadrao(Conexao pobjConexao)
		{
			Conexao = pobjConexao;
			Operacoes = new List<OperacaoDeBancoDeDados>();
			GeradoresFilhos = new List<GeradorOperacaoBDPadrao>();
		}

		public virtual void Adicionar(Modelo pobjModelo, string pstrComando)
		{
			Operacoes.Add(new OperacaoDeBancoDeDados(pobjModelo, pstrComando));
		}

		public void AdicionarGeradorFilho(GeradorOperacaoBDPadrao pobjItem)
		{
			GeradoresFilhos.Add(pobjItem);
		}

		public abstract string GeraInsert(Modelo pobjModelo);

		public abstract string GeraUpdate(Modelo pobjModelo);

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


			foreach (GeradorOperacaoBDPadrao objGerador in GeradoresFilhos) {
				objGerador.Executar();

			}

			return this.Conexao.TransStatus;

		}

	}
}
