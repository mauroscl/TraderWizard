using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;

namespace prjModelo.Regras
{

	public class cAjustarCotacao
	{

		public cCotacaoDiaria ConverterCotacaoParaData(cCotacaoDiaria pobjCotacao, DateTime pdtmDataDestino)
		{

			DateTime dtmDataInicial = default(DateTime);
			DateTime dtmDataFinal = default(DateTime);

			if (pobjCotacao.Data < pdtmDataDestino) {
				//Quando a data de origem é menor do que a data inicial tem que pegar um dia depois, 
				//pois se houver desdobramentos nesta data, o valor de origem já está ajustado para esse desdobramento
				dtmDataInicial = pobjCotacao.Data.AddDays(1);
				dtmDataFinal = pdtmDataDestino;
			} else if (pobjCotacao.Data > pdtmDataDestino) {
				//Quando a data de destino é menor do que a data de origem, tem que pegar um dia depois,
				//pois se houver desdobramentos nesta data, o valor de destino já está ajustado para esse desdobramento
				dtmDataInicial = pdtmDataDestino.AddDays(1);
				dtmDataFinal = pobjCotacao.Data;
			} else {
                throw new Exception("Não existe necessidade de converter cotações pois as datas origem e destino são iguais: " + pobjCotacao.Data.ToString("dd/MM/yyyy"));
			    
			}

			var lstDesdobramentos = pobjCotacao.Ativo.RetornaListaParcialDeDesdobramentos(dtmDataInicial, dtmDataFinal);

			if (lstDesdobramentos.Count() == 0) {
				//Se não tem desdobramentos retorna a mesma cotação
				return pobjCotacao;
			} else {
				var objCotacaoConvertida = pobjCotacao.Clonar();
				foreach (cDesdobramento objDesdobramento in lstDesdobramentos) {
					objDesdobramento.ConverterCotacao(objCotacaoConvertida);
				}

				return objCotacaoConvertida;

			}

		}

		#region "Deprecated"

		//Imports prjModelo.Carregadores
		//Imports DataBase

		//Private ReadOnly objConexao As cConexao

		//Public Sub New(pobjConexao As cConexao)
		//    objConexao = pobjConexao
		//End Sub

		//Public Function ConverterCotacaoParaData(pstrCodigo As String, pdtmDataOrigem As DateTime, ByVal pdecValorOrigem As System.Decimal, pdtmDataDestino As DateTime) As System.Decimal

		//    Dim objCarregadorSplit As New cCarregadorSplit(objConexao)
		//    Dim objRSSplit As cRS = Nothing

		//    Dim dtmDataInicial As DateTime
		//    Dim dtmDataFinal As DateTime
		//    Dim strOrdem As String
		//    Dim strFieldRazao As String

		//    If pdtmDataOrigem < pdtmDataDestino Then
		//        'Quando a data de origem é menor do que a data inicial tem que pegar um dia depois, 
		//        'pois se houver desdobramentos nesta data, o valor de origem já está ajustado para esse desdobramento
		//        dtmDataInicial = pdtmDataOrigem.AddDays(1)
		//        dtmDataFinal = pdtmDataDestino
		//        strOrdem = "A"
		//        strFieldRazao = "Razao"
		//    ElseIf pdtmDataOrigem > pdtmDataDestino Then
		//        'Quando a data de destino é menor do que a data de origem, tem que pegar um dia depois,
		//        'pois se houver desdobramentos nesta data, o valor de destino já está ajustado para esse desdobramento
		//        dtmDataInicial = pdtmDataDestino.AddDays(1)
		//        dtmDataFinal = pdtmDataOrigem
		//        strOrdem = "D"
		//        strFieldRazao = "RazaoInvertida"
		//    Else
		//        Throw New Exception("Não existe necessidade de converter cotações pois as datas origem e destino são iguais: " & Format(pdtmDataOrigem, "dd/MM/yyyy"))
		//    End If

		//    objCarregadorSplit.SplitConsultar(pstrCodigo, dtmDataInicial, strOrdem, objRSSplit, dtmDataFinal)

		//    If objRSSplit.DadosExistir Then

		//        While Not objRSSplit.EOF

		//            pdecValorOrigem *= Convert.ToDouble(objRSSplit.Field(strFieldRazao))

		//            objRSSplit.MoveNext()

		//        End While

		//    End If

		//    objRSSplit.Fechar()

		//    Return pdecValorOrigem

		//End Function

		#endregion

	}
}
