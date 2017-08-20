using System;

namespace DataBase
{
    public class FuncoesBdSqlServer : FuncoesBd
    {
        public FuncoesBdSqlServer()
        {
            OperadorDeConcatenacao = " + ";
        }
        public override string CampoDateFormatar(DateTime pdtmData)
        {
            return $"'{pdtmData:yyyy-MM-dd}'";
        }

        public override string ConvertParaString(string expressao)
        {
            return $"CONVERT(VARCHAR, {expressao})";
        }

        public override string IndiceDaSubString(string expressaoParaProcurar, string expressaoParaPesquisar)
        {
            return $"CHARINDEX({expressaoParaProcurar}, {expressaoParaPesquisar})";
        }

        public override string FormataSubSelect(string select)
        {
            return base.FormataSubSelect(select) + " AS TABELA ";
        }

        public override string ConverterParaPontoFlutuante(string nomeDaColuna)
        {
            return $"CONVERT(FLOAT, {nomeDaColuna})";
        }

        public override string Condicional(string condicao, string valorSeVerdadeiro, string valorSeFalso)
        {
            return $"CASE WHEN {condicao} THEN {valorSeVerdadeiro} ELSE {valorSeFalso} END ";
        }
    }
}