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
		    cRS objRS = new cRS(Conexao);

            FuncoesBd FuncoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSQL = " SELECT COUNT(1) AS Contador " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_SIMULACAO_DIARIA_FAIXA " + Environment.NewLine;
			strSQL = strSQL + " WHERE CODIGO = " + FuncoesBd.CampoFormatar(pstrCodigo) + Environment.NewLine;
			strSQL = strSQL + " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjCM.ID);
			strSQL = strSQL + " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjIFRSobrevendido.ID);

			objRS.ExecuteQuery(strSQL);

			bool functionReturnValue = (Convert.ToInt32(objRS.Field("Contador")) > 0);

			objRS.Fechar();
			return functionReturnValue;

		}

	}
}
