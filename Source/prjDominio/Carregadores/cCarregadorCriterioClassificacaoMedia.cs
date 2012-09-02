using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using frwInterface;
using prjModelo.Entidades;
//Imports DataBase

namespace prjModelo.Carregadores
{

	public class cCarregadorCriterioClassificacaoMedia
	{

		//Private ReadOnly objConexao As cConexao


		private readonly IList<cCriterioClassifMedia> lstTodosCriterios;

		//Public Sub New(ByVal pobjConexao As cConexao)

		//objConexao = pobjConexao


		public cCarregadorCriterioClassificacaoMedia()
		{
			lstTodosCriterios = new List<cCriterioClassifMedia>();

			lstTodosCriterios.Add(new cCriterioClassifMediaMM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM49());
			lstTodosCriterios.Add(new cCriterioClassifMediaMM200());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM21());
			lstTodosCriterios.Add(new cCriterioClassifMediaDifMM200MM49());

		}

		public IList<cCriterioClassifMedia> CarregaTodos()
		{
			return lstTodosCriterios;
		}

		public cCriterioClassifMedia CarregaPorID(cEnum.enumCriterioClassificacaoMedia pintID)
		{
			return lstTodosCriterios.FirstOrDefault(x => x.ID == (decimal) pintID);
		}


		#region "Deprecated"

		//Public Function CarregaTodos() As IList(Of cCriterioClassifMedia)

		//    Dim strSQL As String
		//    Dim objRS = New cRS(objConexao)

		//    Dim lstRetorno As IList(Of cCriterioClassifMedia)
		//    lstRetorno = New List(Of cCriterioClassifMedia)

		//    strSQL = "SELECT ID, Descricao " & vbNewLine
		//    strSQL = strSQL & " FROM Criterio_Classificacao_Media "
		//    'strSQL = strSQL & " WHERE ID = 1 "


		//    objRS.ExecuteQuery(strSQL)

		//    While Not objRS.EOF

		//        lstRetorno.Add(Me.CarregaPorID(CInt(objRS.Field("ID")), objRS.Field("Descricao")))

		//        objRS.MoveNext()

		//    End While

		//    objRS.Fechar()

		//    Return lstRetorno

		//End Function

		//Public Function CarregaPorID(pintID As cEnum.enumCriterioClassificacaoMedia) As cCriterioClassifMedia

		//    Dim strSQL As String
		//    Dim objRS = New cRS(objConexao)

		//    Dim objRetorno As cCriterioClassifMedia = Nothing

		//    strSQL = "SELECT ID, Descricao " & vbNewLine
		//    strSQL = strSQL & " FROM Criterio_Classificacao_Media "
		//    strSQL = strSQL & " WHERE ID = " & FuncoesBD.CampoFormatar(pintID)


		//    objRS.ExecuteQuery(strSQL)

		//    If Not objRS.EOF Then

		//        objRetorno = Me.CarregaPorID(CInt(objRS.Field("ID")), objRS.Field("Descricao"))

		//    End If

		//    objRS.Fechar()

		//    Return objRetorno

		//End Function

		//Private Function CarregaPorID(ByVal pintID As cEnum.enumCriterioClassificacaoMedia, ByVal pstrDescricao As String) As cCriterioClassifMedia
		//    Select Case pintID
		//        Case cEnum.enumCriterioClassificacaoMedia.PercentualMM21
		//            Return New cCriterioClassifMediaMM21(pintID, pstrDescricao)

		//        Case cEnum.enumCriterioClassificacaoMedia.PercentualMM49
		//            Return New cCriterioClassifMediaMM49(pintID, pstrDescricao)

		//        Case cEnum.enumCriterioClassificacaoMedia.PercentualMM200
		//            Return New cCriterioClassifMediaMM200(pintID, pstrDescricao)

		//        Case cEnum.enumCriterioClassificacaoMedia.PercentualDiferencaMM200MM21
		//            Return New cCriterioClassifMediaDifMM200MM21(pintID, pstrDescricao)
		//        Case cEnum.enumCriterioClassificacaoMedia.PercentualDiferencaMM200MM49
		//            Return New cCriterioClassifMediaDifMM200MM49(pintID, pstrDescricao)
		//        Case Else
		//            Return Nothing
		//    End Select


		//End Function

		#endregion

	}
}
