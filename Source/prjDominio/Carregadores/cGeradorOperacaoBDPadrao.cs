using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public abstract class cGeradorOperacaoBDPadrao
	{

		public cConexao Conexao { get; set; }
		public IList<cOperacaoBD> Operacoes { get; set; }
		protected IList<cGeradorOperacaoBDPadrao> GeradoresFilhos { get; set; }

		public cGeradorOperacaoBDPadrao(cConexao pobjConexao)
		{
			Conexao = pobjConexao;
			Operacoes = new List<cOperacaoBD>();
			GeradoresFilhos = new List<cGeradorOperacaoBDPadrao>();
		}

		public virtual void Adicionar(cModelo pobjModelo, string pstrComando)
		{
			Operacoes.Add(new cOperacaoBD(pobjModelo, pstrComando));
		}

		public void AdicionarGeradorFilho(cGeradorOperacaoBDPadrao pobjItem)
		{
			GeradoresFilhos.Add(pobjItem);
		}

		public abstract string GeraInsert(cModelo pobjModelo);

		public abstract string GeraUpdate(cModelo pobjModelo);

		public bool Executar()
		{

			string strComando = null;

			cCommand objCommand = new cCommand(this.Conexao);

			foreach (cOperacaoBD item in this.Operacoes) {
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
