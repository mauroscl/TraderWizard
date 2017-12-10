using System;
using System.Collections.Generic;
using System.Linq;

//Descrição: contém funcões auxiliares para facilitar o acesso ao banco de dados

namespace DataBase
{

    public abstract class FuncoesBd
    {
        protected string OperadorDeConcatenacao;

        public string CampoStringFormatar(string pstrValor)
        {
            //coloca aspas simples no início e no fim da string e a cada "'"  encontrada
            //substitui por "''"
            return "'" + pstrValor.Replace("'", "''") + "'";
        }

        //aqui transforma a string do campo para maiúscula
        public string CampoStringUpperFormatar(string pstrValor)
        {
            //coloca aspas simples no início e no fim da string e a cada "'"  encontrada
            //substitui por "''"
            return "'" + pstrValor.ToUpper().Replace("'", "''");
        }

        public string CampoUpperFormatar(string pstrCampo)
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
        public long CodigoChaveGerar(string pstrTabela, string pstrCampo, Command pobjCommand, string pstrWhere = "")
        {
            RS objRS = new RS(pobjCommand.Conexao);

            string strQuery = " select max( " + pstrCampo + ") as NovoCodigo " + " from " + pstrTabela;

            if (!string.IsNullOrEmpty(pstrWhere))
            {
                strQuery = strQuery + " where " + pstrWhere;

            }

            objRS.ExecuteQuery(strQuery);

            //coloca o retorno erro para 0, para caso não existe nenhum registro na tabela o primeiro
            //registro tenha código 1, já que sempre é somado 1 ao valor retornado no select.
            return Convert.ToInt64(objRS.Field("NovoCodigo", "0")) + 1;

        }

        public virtual string CampoDateFormatar(DateTime pdtmData)
        {
            return "";
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
        public string CampoFloatFormatar(double? pdblValor)
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
        public string CampoDecimalFormatar(decimal? pdecValor)
        {
            //Return Format(pdecValor, "#,##")
            //Return FormatNumber(pdecValor, 2, TriState.False, TriState.False, TriState.False)
            return pdecValor.HasValue ? Convert.ToString(pdecValor).Replace(",", ".") : "NULL";
        }

        public string CampoFormatar(int pintValor)
        {
            return Convert.ToString(pintValor);
        }

        public string CampoFormatar(long? plngValor)
        {
            return plngValor.HasValue ? plngValor.ToString() : "NULL";
        }

        public string CampoFormatar(long plngValor)
        {
            return Convert.ToString(plngValor);
        }

        public string CampoFormatar(DateTime pdtmValor)
        {
            return CampoDateFormatar(pdtmValor);
        }

        public string CampoFormatar(DateTime? pdtmValor)
        {
            return pdtmValor.HasValue ? CampoDateFormatar(pdtmValor.Value) : "NULL";
        }

        public string CampoFormatar(decimal? pdecValor)
        {
            return pdecValor.HasValue ? CampoDecimalFormatar(pdecValor) : "NULL";
        }

        public string CampoFormatar(decimal pdecValor)
        {
            return CampoDecimalFormatar(pdecValor);
        }

        public string CampoFormatar(double pdblValor)
        {
            return CampoFloatFormatar(pdblValor);
        }

        public string CampoFormatar(double? pdblValor)
        {
            return pdblValor.HasValue ? CampoFloatFormatar(pdblValor) : "NULL";
        }

        public string CampoFormatar(string pstrValor)
        {
            return !string.IsNullOrEmpty(pstrValor) ? CampoStringFormatar(pstrValor) : "NULL";
        }

        public string CampoFormatar(bool pblnValor)
        {
            return (pblnValor ? "TRUE" : "FALSE");
        }

        public string ConcatenarString(string string1, string string2)
        {
            return string1 + OperadorDeConcatenacao + string2;
        }

        public string ConcatenarStrings(string[] strings)
        {
            if (strings.Length <= 1)
            {
                throw new Exception("São necessárias pelo menos duas strings para serem concatenadas.");
            }

            string stringConcatenada = "";

            foreach (string termo in strings)
            {
                if (stringConcatenada != "")
                {
                    stringConcatenada += OperadorDeConcatenacao;
                }
                stringConcatenada += termo;
            }

            return stringConcatenada;
        }

        /// <summary>
        /// formata uma query para que possa ser usada como uma tabela em uma subquery
        /// </summary>
        /// <param name="select">query que será usada como subselect</param>
        /// <returns>subselect na sintaxe que pode ser usada como uma tabela</returns>
	    public virtual string FormataSubSelect(string select)
        {
            return "(" + select + ")";
        }

        public virtual string ConvertParaString(string expressao)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// formata a cláusula SQL que verifica o indice da primeira ocorrência de uma substring e uma string
        /// </summary>
        /// <param name="expressaoParaProcurar">termo que será  procurado (substring)</param>
        /// <param name="expressaoParaPesquisar">expressão onde o termo será procurado</param>
        /// <returns>string com a expressão SQL</returns>
        public abstract string IndiceDaSubString(string expressaoParaProcurar, string expressaoParaPesquisar);

        public abstract string ConverterParaPontoFlutuante(string nomeDaColuna);

        public abstract string Condicional(string condicao, string valorSeVerdadeiro, string valorSeFalso);

        public string GerarParametrosParaConditionIn(ICollection<string> items)
        {
            return string.Join(", ", items.Select(CampoStringFormatar));
        }


    }
}
