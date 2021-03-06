using System;
using Dominio.Entidades;

namespace DataBase.Carregadores
{

	public class ManipuladorIFRSimulacaoDiaria : GeradorOperacaoBDPadrao
	{

		public ManipuladorIFRSimulacaoDiaria(Conexao pobjConexao) : base(pobjConexao)
		{
		}


		public override void Adicionar(Modelo pobjModelo, string pstrComando)
		{
			Operacoes.Add(new OperacaoDeBancoDeDados(pobjModelo, pstrComando));

			ManipuladorIFRSimulacaoDiariaDetalhe objManipuladorDetalhe = new ManipuladorIFRSimulacaoDiariaDetalhe(Conexao);

			var objSimulacao = (IFRSimulacaoDiaria)pobjModelo;


			foreach (cIFRSimulacaoDiariaDetalhe objDetalhe in objSimulacao.Detalhes) {
				objManipuladorDetalhe.Adicionar(objDetalhe, "INSERT");

			}

			AdicionarGeradorFilho(objManipuladorDetalhe);

		}

		public override string GeraInsert(Modelo pobjModelo)
		{
		    IFRSimulacaoDiaria objItem = (IFRSimulacaoDiaria)pobjModelo;

            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

			//INSERE REGISTRO NA TABELA
		    string strSQL = "INSERT INTO IFR_Simulacao_Diaria" + Environment.NewLine +
		                    "(Codigo, ID_Setup, ID_CM, Data_Entrada_Efetiva, Sequencial, Valor_Entrada_Ajustado " +
		                    Environment.NewLine +
		                    ", Valor_IFR_Minimo, Data_Cruzamento_Media, Valor_Realizacao_Parcial, Valor_Amplitude " +
		                    Environment.NewLine +
		                    ", Valor_Maximo, Percentual_Maximo, Data_Saida, Valor_Saida, Percentual_Saida " +
		                    Environment.NewLine +
		                    ", Percentual_MME200, Percentual_MME49, Percentual_MME21, Media_IFR , ValorFechamento_Minimo " +
		                    Environment.NewLine +
		                    ", MME21_Minima, MME49_Minima, Verdadeiro, Valor_Stop_Loss_Inicial, Valor_Entrada_Original)" +
		                    Environment.NewLine + " VALUES " + Environment.NewLine + "(" +
		                    FuncoesBd.CampoFormatar(objItem.Ativo.Codigo) + ", " + FuncoesBd.CampoFormatar(objItem.Setup.Id) +
		                    ", " + FuncoesBd.CampoFormatar(objItem.ClassificacaoMedia.ID) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.DataEntradaEfetiva) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.Sequencial) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.ValorEntradaAjustado) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.ValorIFR) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.DataCruzamentoMedia) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.ValorRealizacaoParcial) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.ValorAmplitude) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.ValorMaximo) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.PercentualMaximo) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.DataSaida) + ", " + FuncoesBd.CampoFormatar(objItem.ValorSaida) +
		                    ", " + FuncoesBd.CampoFormatar(objItem.PercentualSaida) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.PercentualMME200) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.PercentualMME49) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.PercentualMME21) + ", " +
		                    FuncoesBd.CampoFormatar(objItem.MediaIFR);

			//VALOR MÍNIMO DOS PREÇOS E DAS MÉDIAS DE 21 E 49 antes do IFR cruzar a média.
		    strSQL = strSQL + "," + FuncoesBd.CampoFormatar(objItem.ValorFechamentoMinimo) + ", " +
		             FuncoesBd.CampoFloatFormatar(objItem.ValorMME21Minima) + ", " +
		             FuncoesBd.CampoFloatFormatar(objItem.ValorMME49Minima) + ", " +
		             FuncoesBd.CampoFormatar(objItem.Verdadeiro) + ", " +
		             FuncoesBd.CampoFormatar(objItem.ValorStopLossInicial);


			//Negocios original de entrada
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.ValorEntradaOriginal);

			strSQL = strSQL + ")" + Environment.NewLine;

			return strSQL;

		}

		public override string GeraUpdate(Modelo pobjModelo)
		{

			throw new NotImplementedException();

		}


	}
}
