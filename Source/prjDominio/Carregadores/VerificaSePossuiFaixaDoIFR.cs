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

	public class VerificaSePossuiFaixaDoIFR
	{

		private cConexao Conexao { get; set; }


		public VerificaSePossuiFaixaDoIFR(cConexao pobjConexao)
		{
			Conexao = pobjConexao;

		}

		public bool VerificaPorClassificacaoMedia(string pstrCodigo, cClassifMedia pobjCM, cIFRSobrevendido pobjIFRSobrevendido)
		{
			bool functionReturnValue = false;

			cRS objRS = new cRS(Conexao);

			//TODO: Falta fazer teste de unidade

			string strSQL = null;

			strSQL = " SELECT COUNT(1) AS Contador " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA " + Environment.NewLine;
			strSQL = strSQL + " WHERE CODIGO = " + FuncoesBD.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBD.CampoFormatar(pobjCM.ID);
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBD.CampoFormatar(pobjIFRSobrevendido.ID);

			objRS.ExecuteQuery(strSQL);

			functionReturnValue = (Convert.ToInt32(objRS.Field("Contador")) > 0);

			objRS.Fechar();
			return functionReturnValue;

		}

	}
}
