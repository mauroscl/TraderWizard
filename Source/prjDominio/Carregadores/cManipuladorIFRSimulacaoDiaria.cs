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

			string strSQL = null;

			cIFRSimulacaoDiaria objItem = (cIFRSimulacaoDiaria)pobjModelo;

			//INSERE REGISTRO NA TABELA
			strSQL = "INSERT INTO IFR_Simulacao_Diaria" + Environment.NewLine + "(Codigo, ID_Setup, ID_CM, Data_Entrada_Efetiva, Sequencial, Valor_Entrada_Ajustado " + Environment.NewLine + ", Valor_IFR_Minimo, Data_Cruzamento_Media, Valor_Realizacao_Parcial, Valor_Amplitude " + Environment.NewLine + ", Valor_Maximo, Percentual_Maximo, Data_Saida, Valor_Saida, Percentual_Saida " + Environment.NewLine + ", Percentual_MME200, Percentual_MME49, Percentual_MME21, Media_IFR , ValorFechamento_Minimo " + Environment.NewLine + ", MME21_Minima, MME49_Minima, Verdadeiro, Valor_Stop_Loss_Inicial, Valor_Entrada_Original)" + Environment.NewLine + " VALUES " + Environment.NewLine + "(" + FuncoesBD.CampoFormatar(objItem.Ativo.Codigo) + ", " + FuncoesBD.CampoFormatar(objItem.Setup.ID) + ", " + FuncoesBD.CampoFormatar(objItem.ClassificacaoMedia.ID) + ", " + FuncoesBD.CampoFormatar(objItem.DataEntradaEfetiva) + ", " + FuncoesBD.CampoFormatar(objItem.Sequencial) + ", " + FuncoesBD.CampoFormatar(objItem.ValorEntradaAjustado) + ", " + FuncoesBD.CampoFormatar(objItem.ValorIFR) + ", " + FuncoesBD.CampoFormatar(objItem.DataCruzamentoMedia) + ", " + FuncoesBD.CampoFormatar(objItem.ValorRealizacaoParcial) + ", " + FuncoesBD.CampoFormatar(objItem.ValorAmplitude) + ", " + FuncoesBD.CampoFormatar(objItem.ValorMaximo) + ", " + FuncoesBD.CampoFormatar(objItem.PercentualMaximo) + ", " + FuncoesBD.CampoFormatar(objItem.DataSaida) + ", " + FuncoesBD.CampoFormatar(objItem.ValorSaida) + ", " + FuncoesBD.CampoFormatar(objItem.PercentualSaida) + ", " + FuncoesBD.CampoFormatar(objItem.PercentualMME200) + ", " + FuncoesBD.CampoFormatar(objItem.PercentualMME49) + ", " + FuncoesBD.CampoFormatar(objItem.PercentualMME21) + ", " + FuncoesBD.CampoFormatar(objItem.MediaIFR);

			//VALOR MÍNIMO DOS PREÇOS E DAS MÉDIAS DE 21 E 49 antes do IFR cruzar a média.
			strSQL = strSQL + "," + FuncoesBD.CampoFormatar(objItem.ValorFechamentoMinimo) + ", " + FuncoesBD.CampoFloatFormatar(objItem.ValorMME21Minima) + ", " + FuncoesBD.CampoFloatFormatar(objItem.ValorMME49Minima) + ", " + FuncoesBD.CampoFormatar(objItem.Verdadeiro) + ", " + FuncoesBD.CampoFormatar(objItem.ValorStopLossInicial);

			//Valor original de entrada
			strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objItem.ValorEntradaOriginal);

			strSQL = strSQL + ")" + Environment.NewLine;

			return strSQL;

		}

		public override string GeraUpdate(cModelo pobjModelo)
		{

			throw new NotImplementedException();

		}


	}
}
