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

	public class cIFRSimulacaoDiariaFaixa
	{

		private long? lngID;
		private string strCodigo;
		private cClassifMedia objCM;
		private Setup objSetup;
		private cCriterioClassifMedia objCriterioCM;
		private cIFRSobrevendido objIFRSobrevendido;
		private double dblValorMinimo;
		private double dblValorMaximo;
		private int intNumTentativasMinimo;
		public System.DateTime Data { get; set; }


		public cIFRSimulacaoDiariaFaixa(string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioCM, cIFRSobrevendido pobjIFRSobrevendido, System.DateTime pdtmData, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
		{
			strCodigo = pstrCodigo;
			objCM = pobjCM;
			objSetup = pobjSetup;
			objCriterioCM = pobjCriterioCM;
			objIFRSobrevendido = pobjIFRSobrevendido;
			this.Data = pdtmData;

			dblValorMinimo = pdblValorMinimo;
			dblValorMaximo = pdblValorMaximo;

			intNumTentativasMinimo = pintNumTentativasMinimo;

		}


		public cIFRSimulacaoDiariaFaixa(long plngID, string pstrCodigo, Setup pobjSetup, cClassifMedia pobjCM, cCriterioClassifMedia pobjCriterioCM, int pintNumTentativasMinimo, double pdblValorMinimo, double pdblValorMaximo)
		{
			lngID = plngID;
			strCodigo = pstrCodigo;
			objCM = pobjCM;
			objSetup = pobjSetup;
			objCriterioCM = pobjCriterioCM;
			dblValorMinimo = pdblValorMinimo;
			dblValorMaximo = pdblValorMaximo;

			intNumTentativasMinimo = pintNumTentativasMinimo;

		}

		public long? ID {
			get { return lngID; }
		}

		public double ValorMinimo {
			get { return dblValorMinimo; }
		}

		public double ValorMaximo {
			get { return dblValorMaximo; }
		}

		public int NumTentativasMinimo {
			get { return intNumTentativasMinimo; }
		}

		public cCriterioClassifMedia CriterioCM {
			get { return objCriterioCM; }
		}

		public bool Salvar(cConexao pobjConexao)
		{
		    cCommand objCommand = new cCommand(pobjConexao);

            FuncoesBd FuncoesBd = pobjConexao.ObterFormatadorDeCampo();

			//O Campo ID da tabela IFR_Simulacao_Diaria_Faixa é do tipo IDENTITY
			string strSQL = "INSERT INTO IFR_Simulacao_Diaria_Faixa " + Environment.NewLine;
			strSQL = strSQL + "(Codigo, ID_Setup, ID_CM, ID_Criterio_CM, ID_IFR_Sobrevendido, Data, Valor_Minimo, Valor_Maximo, NumTentativas_Minimo)" + Environment.NewLine;
			strSQL = strSQL + " VALUES " + Environment.NewLine;
			strSQL = strSQL + "(" + FuncoesBd.CampoFormatar(strCodigo);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objSetup.ID);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objCM.ID);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objCriterioCM.ID);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(objIFRSobrevendido.ID);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(Data);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(dblValorMinimo);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(dblValorMaximo);
			strSQL = strSQL + ", " + FuncoesBd.CampoFormatar(intNumTentativasMinimo) + ")";

			objCommand.Execute(strSQL);

			return pobjConexao.TransStatus;

		}

	}
}
