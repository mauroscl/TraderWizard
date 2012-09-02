using System.Collections.Generic;
using System.Linq;
using frwInterface;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public class cCarregadorClassificacaoMedia
	{


		private readonly IList<cClassifMedia> lstTodasClassificacoes;

		public cCarregadorClassificacaoMedia()
		{
			lstTodasClassificacoes = new List<cClassifMedia>
			                             {
			                                 new cClassifMediaAltaAlinhada(),
			                                 new cClassifMediaAltaDesalinhada(),
			                                 new cClassifMediaBaixaAlinhada(),
			                                 new cClassifMediaBaixaDesalinhada(),
			                                 new cClassifMediaPrimAltaSecBaixa(),
			                                 new cClassifMediaPrimBaixaSecAlta()
			                             };
		}

		public IList<cClassifMedia> CarregaTodos()
		{
			return lstTodasClassificacoes;
		}

		public cClassifMedia CarregaPorID(cEnum.enumClassifMedia pintID)
		{
			return lstTodasClassificacoes.FirstOrDefault(x => x.ID == (decimal) pintID);
		}


		#region "Deprecated"

		//Public Function CarregaTodos() As IList(Of cClassifMedia)

		//    Dim strSQL As String
		//    Dim objRS = New cRS(objConexao)

		//    Dim lstRetorno As IList(Of cClassifMedia)
		//    lstRetorno = New List(Of cClassifMedia)

		//    strSQL = "SELECT ID " & vbNewLine
		//    strSQL = strSQL & " FROM Classificacao_Media "

		//    objRS.ExecuteQuery(strSQL)

		//    While Not objRS.EOF

		//        lstRetorno.Add(Me.CarregaPorID(CInt(objRS.Field("ID"))))

		//        objRS.MoveNext()

		//    End While

		//    objRS.Fechar()

		//    Return lstRetorno

		//End Function

		//Public Function CarregaPorID(ByVal pintID As cEnum.enumClassifMedia) As cClassifMedia

		//    Dim objRetorno As cClassifMedia = Nothing

		//    Select Case pintID

		//        Case cEnum.enumClassifMedia.AltaAlinha

		//            objRetorno = New cClassifMediaAltaAlinhada()

		//        Case cEnum.enumClassifMedia.PrimAltaSecBaixa

		//            objRetorno = New cClassifMediaPrimAltaSecBaixa()

		//        Case cEnum.enumClassifMedia.PrimBaixaSecAlta

		//            objRetorno = New cClassifMediaPrimBaixaSecAlta()

		//        Case cEnum.enumClassifMedia.BaixaAlinhada

		//            objRetorno = New cClassifMediaBaixaAlinhada

		//        Case cEnum.enumClassifMedia.BaixaDesalinhada

		//            objRetorno = New cClassifMediaBaixaDesalinhada()

		//        Case cEnum.enumClassifMedia.AltaDesalinhada

		//            objRetorno = New cClassifMediaAltaDesalinhada()

		//        Case cEnum.enumClassifMedia.Indefinida

		//            objRetorno = New cClassifMediaIndefinida()

		//    End Select

		//    Return objRetorno
		//End Function
		#endregion


	}
}
