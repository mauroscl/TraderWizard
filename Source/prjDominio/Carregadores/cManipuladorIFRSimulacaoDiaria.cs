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

	public class cManipuladorIFRSimulacaoDiaria : cGeradorOperacaoBDPadrao
	{

		public cManipuladorIFRSimulacaoDiaria(cConexao pobjConexao) : base(pobjConexao)
		{
		}


		public override void Adicionar(cModelo pobjModelo, string pstrComando)
		{
			Operacoes.Add(new cOperacaoBD(pobjModelo, pstrComando));

			cManipuladorIFRSimulacaoDiariaDetalhe objManipuladorDetalhe = new cManipuladorIFRSimulacaoDiariaDetalhe(Conexao);

			var objSimulacao = (cIFRSimulacaoDiaria)pobjModelo;


			foreach (cIFRSimulacaoDiariaDetalhe objDetalhe in objSimulacao.Detalhes) {
				objManipuladorDetalhe.Adicionar(objDetalhe, "INSERT");

			}

			AdicionarGeradorFilho(objManipuladorDetalhe);

		}

		public override string GeraInsert(cModelo pobjModelo)
		{
		    cIFRSimulacaoDiaria objItem = (cIFRSimulacaoDiaria)pobjModelo;

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
		                    FuncoesBd.CampoFormatar(objItem.Ativo.Codigo) + ", " + FuncoesBd.CampoFormatar(objItem.Setup.ID) +
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


			//Valor original de entrada
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objItem.ValorEntradaOriginal);

			strSQL = strSQL + ")" + Environment.NewLine;

			return strSQL;

		}

		public override string GeraUpdate(cModelo pobjModelo)
		{

			throw new NotImplementedException();

		}


	}
}
