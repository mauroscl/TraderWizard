using System;

//Descrição: contém funcões auxiliares para facilitar o acesso ao banco de dados

namespace DataBase
{

	public class FuncoesBd
	{

		public static string CampoStringFormatar(string pstrValor)
		{
			//coloca aspas simples no início e no fim da string e a cada "'"  encontrada
			//substitui por "''"
			return "'" + pstrValor.Replace("'", "''") + "'";
		}

		//aqui transforma a string do campo para maiúscula
		public static string CampoStringUpperFormatar(string pstrValor)
		{
			//coloca aspas simples no início e no fim da string e a cada "'"  encontrada
			//substitui por "''"
			return "'" + pstrValor.ToUpper().Replace("'", "''") ;
		}

		public static string CampoUpperFormatar(string pstrCampo)
		{

			//aqui formata para o oracle converter o campo para maiúscula
			return "UPPER(" + pstrCampo + ")";

		}

		//Descrição: Gera o código chave para uma tabela, adicionando 1 ao código máximo já existente
		//Parâmetros:

		//pstrTabela = tabela do campo chave
		//pstrCampo = campo para o qual vai ser gerada a chave

		//pstrWhere = utilizado quando a chave da tabela é composto por mais de um campo e
		//deseja-se usar os outros campos com valor fixo

		//Retorno: A nova chave para a tabela
		public static long CodigoChaveGerar(string pstrTabela, string pstrCampo, cCommand pobjCommand, string pstrWhere = "")
		{
		    cRS objRS = new cRS(pobjCommand.Conexao);

		    string strQuery = " select max( " + pstrCampo + ") as NovoCodigo " + " from " + pstrTabela;

			if (!string.IsNullOrEmpty(pstrWhere)) {
				strQuery = strQuery + " where " + pstrWhere;

			}

			objRS.ExecuteQuery(strQuery);

			//coloca o retorno erro para 0, para caso não existe nenhum registro na tabela o primeiro
			//registro tenha código 1, já que sempre é somado 1 ao valor retornado no select.
			return Convert.ToInt64(objRS.Field("NovoCodigo", "0")) + 1;

		}

		public static string CampoDateFormatar(System.DateTime pdtmData)
		{
			return "#" + pdtmData.Month + "/" + pdtmData.Day + "/" + pdtmData.Year + "#";

		}


		/// <summary>
		/// Formata um campo em ponto flutuante
		/// </summary>
		/// <param name="pdblValor">conteúdo que deve ser formatado</param>
		/// <returns>campo formatado com o separador decimal</returns>
		/// <remarks></remarks>
		/// <alteracoes>
		/// 17/12/2010 - Inclusão da possibilidade de receber parâmetro NULO    
		/// </alteracoes>    
		public static string CampoFloatFormatar(double? pdblValor)
		{
		    if (!pdblValor.HasValue) return "NULL";
		    string strAux = Convert.ToString(pdblValor);
		    strAux = strAux.Replace(",", ".");

		    return strAux;
		}

	    /// <summary>
		/// Formata um campo decimal
		/// </summary>
		/// <param name="pdecValor">conteúdo que deve ser formatado</param>
		/// <returns>campo formatado com o separador decimal</returns>
		/// <remarks></remarks>
		/// <alteracoes>
		/// 17/12/2010 - Inclusão da possibilidade de receber parâmetro NULO    
		/// </alteracoes>    
		public static string CampoDecimalFormatar(decimal? pdecValor)
		{
		    //Return Format(pdecValor, "#,##")
			//Return FormatNumber(pdecValor, 2, TriState.False, TriState.False, TriState.False)
		    return pdecValor.HasValue ? Convert.ToString(pdecValor).Replace( ",", ".") : "NULL";
		}

	    public static string CampoFormatar(int pintValor)
		{
            return Convert.ToString(pintValor);
		}

		public static string CampoFormatar(long? plngValor)
		{
		    return plngValor.HasValue ? plngValor.ToString() : "NULL";
		}

	    public static string CampoFormatar(long plngValor)
		{
            return Convert.ToString(plngValor);
		}

		public static string CampoFormatar(DateTime pdtmValor)
		{
			return CampoDateFormatar(pdtmValor);
		}

		public static string CampoFormatar(DateTime? pdtmValor)
		{
		    return pdtmValor.HasValue ? CampoDateFormatar(pdtmValor.Value) : "NULL";
		}

	    public static string CampoFormatar(decimal? pdecValor)
		{
		    return pdecValor.HasValue ? CampoDecimalFormatar(pdecValor) : "NULL";
		}

	    public static string CampoFormatar(decimal pdecValor)
		{
			return CampoDecimalFormatar(pdecValor);
		}

		public static string CampoFormatar(double pdblValor)
		{
			return CampoFloatFormatar(pdblValor);
		}

		public static string CampoFormatar(double? pdblValor)
		{
		    return pdblValor.HasValue ? CampoFloatFormatar(pdblValor) : "NULL";
		}

	    public static string CampoFormatar(string pstrValor)
	    {
	        return !string.IsNullOrEmpty(pstrValor) ? CampoStringFormatar(pstrValor) : "NULL";
	    }

	    public static string CampoFormatar(bool pblnValor)
		{
			return (pblnValor ? "TRUE" : "FALSE");
		}


	}
}
