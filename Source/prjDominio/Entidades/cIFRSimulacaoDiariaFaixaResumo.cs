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

		    //Somente salva se houve algum trade com ou sem filtro.

			if (intNumTradesSemFiltro > 0 || intNumTradesComFiltro > 0) {

                FuncoesBd FuncoesBd = pobjConexao.ObterFormatadorDeCampo();

				string strSQL = "INSERT INTO IFR_Simulacao_Diaria_Faixa_Resumo" + Environment.NewLine;
				strSQL = strSQL + "(Codigo, ID_Setup, ID_CM, ID_IFR_Sobrevendido, Data, NumTradesSemFiltro, NumAcertosSemFiltro, PercentualAcertosSemFiltro " + Environment.NewLine;
				strSQL = strSQL + ", NumTradesComFiltro, NumAcertosComFiltro, PercentualAcertosComFiltro)" + Environment.NewLine;
				strSQL = strSQL + " VALUES ";
				strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(objAtivo.Codigo);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objSetup.ID);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objCM.ID);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objIFRSobrevendido.ID);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Data);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumTradesSemFiltro);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumAcertosSemFiltro);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(dblPercentualAcertosSemFiltro);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumTradesComFiltro);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumAcertosComFiltro);
				strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(PercentualAcertosComFiltro);
				strSQL = strSQL + ")";

				objCommand.Execute(strSQL);

			}

		}

	}
}
