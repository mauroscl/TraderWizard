using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DataBase;

namespace prjModelo.Entidades
{

	public class cIFRSimulacaoDiariaFaixaResumo
	{

		private readonly cAtivo objAtivo;
		private readonly Setup objSetup;
		private readonly cClassifMedia objCM;
		private readonly cIFRSobrevendido objIFRSobrevendido;
		private int intNumTradesSemFiltro;
		private int intNumAcertosSemFiltro;
		private double dblPercentualAcertosSemFiltro;
		private int intNumTradesComFiltro;
		private int intNumAcertosComFiltro;
		public System.DateTime Data { get; set; }
		public double PercentualAcertosComFiltro { get; set; }


		public cIFRSimulacaoDiariaFaixaResumo(cAtivo pobjAtivo, Setup pobjSetup, cClassifMedia pobjCM, cIFRSobrevendido pobjIFRSobrevendido, System.DateTime pdtmData)
		{
			objAtivo = pobjAtivo;
			objSetup = pobjSetup;
			objCM = pobjCM;
			objIFRSobrevendido = pobjIFRSobrevendido;
			Data = pdtmData;

		}
		public int NumTradesSemFiltro {
			get { return intNumTradesSemFiltro; }
			set {
				intNumTradesSemFiltro = value;
				CalcularPercentualAcertosSemFiltro();
			}
		}

		public int NumAcertosSemFiltro {
			get { return intNumAcertosSemFiltro; }
			set {
				intNumAcertosSemFiltro = value;
				CalcularPercentualAcertosSemFiltro();
			}
		}

		public int NumTradesComFiltro {
			get { return intNumTradesComFiltro; }
			set {
				intNumTradesComFiltro = value;
				CalcularPercentualAcertosComFiltro();
			}
		}

		public int NumAcertosComFiltro {
			get { return intNumAcertosComFiltro; }
			set {
				intNumAcertosComFiltro = value;
				CalcularPercentualAcertosComFiltro();
			}
		}

		//Public Property PercentualAcertosComFiltro() As Double
		//    Get
		//        Return dblPercentualAcertosComFiltro
		//    End Get
		//    Set(ByVal value As Double)
		//        dblPercentualAcertosComFiltro = value
		//    End Set
		//End Property


		//Public Sub New(ByVal pstrCodigo As String, ByVal pstrIDSetup As String, ByVal pintNumTradesSemFiltro As Integer, ByVal pintNumAcertosSemFiltro As Integer _
		//, ByVal pdblPercentualAcertosSemFiltro As Double, ByVal pintNumTradesComFiltro As Integer, ByVal pintNumAcertosComFiltro As Integer _
		//, ByVal pdblPercentualAcertosComFiltro As Double)

		//    strCodigo = pstrCodigo
		//    strIDSetup = pstrIDSetup
		//    intNumTradesSemFiltro = pintNumTradesSemFiltro
		//    intNumAcertosSemFiltro = pintNumAcertosSemFiltro
		//    dblPercentualAcertosSemFiltro = pdblPercentualAcertosSemFiltro
		//    intNumTradesComFiltro = pintNumTradesComFiltro
		//    intNumAcertosComFiltro = pintNumAcertosComFiltro
		//    dblPercentualAcertosComFiltro = pdblPercentualAcertosComFiltro

		//End Sub

		private void CalcularPercentualAcertosSemFiltro()
		{
			if (intNumTradesSemFiltro != 0) {
				dblPercentualAcertosSemFiltro = intNumAcertosSemFiltro / intNumTradesSemFiltro * 100;
			} else {
				dblPercentualAcertosSemFiltro = 0;
			}
		}

		private void CalcularPercentualAcertosComFiltro()
		{
			if (intNumTradesComFiltro != 0) {
				PercentualAcertosComFiltro = intNumAcertosComFiltro / intNumTradesComFiltro * 100;
			} else {
				PercentualAcertosComFiltro = 0;
			}
		}



		public void Salvar(cConexao pobjConexao)
		{
			cCommand objCommand = new cCommand(pobjConexao);

			string strSQL = null;

			//Somente salva se houve algum trade com ou sem filtro.

			if (intNumTradesSemFiltro > 0 | intNumTradesComFiltro > 0) {
				strSQL = "INSERT INTO IFR_Simulacao_Diaria_Faixa_Resumo" + Environment.NewLine;
				strSQL = strSQL + "(Codigo, ID_Setup, ID_CM, ID_IFR_Sobrevendido, Data, NumTradesSemFiltro, NumAcertosSemFiltro, PercentualAcertosSemFiltro " + Environment.NewLine;
				strSQL = strSQL + ", NumTradesComFiltro, NumAcertosComFiltro, PercentualAcertosComFiltro)" + Environment.NewLine;
				strSQL = strSQL + " VALUES ";
				strSQL = strSQL + "(" + FuncoesBD.CampoFormatar(objAtivo.Codigo);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objSetup.ID);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objCM.ID);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(objIFRSobrevendido.ID);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(Data);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intNumTradesSemFiltro);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intNumAcertosSemFiltro);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(dblPercentualAcertosSemFiltro);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intNumTradesComFiltro);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(intNumAcertosComFiltro);
				strSQL = strSQL + ", " + FuncoesBD.CampoFormatar(PercentualAcertosComFiltro);
				strSQL = strSQL + ")";

				objCommand.Execute(strSQL);

			}

		}

	}
}
