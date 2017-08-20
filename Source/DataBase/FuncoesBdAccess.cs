using System;

namespace DataBase
{
    public class FuncoesBdAccess : FuncoesBd
    {
        public FuncoesBdAccess()
        {
            OperadorDeConcatenacao = " & ";
        }

        public override string CampoDateFormatar(DateTime pdtmData)
        {
            return "#" + pdtmData.ToString("MM/dd/yyyy") + "#";
        }

        public override string ConvertParaString(string expressao)
        {
            return "CSTR(" + expressao + ")";
        }

        public override string IndiceDaSubString(string expressaoParaProcurar, string expressaoParaPesquisar)
        {
            return "INSTR(" + expressaoParaPesquisar + ","+  expressaoParaProcurar  +  ")";
        }

        public override string ConverterParaPontoFlutuante(string nomeDaColuna)
        {
            return "CDBL(" + nomeDaColuna + ")";
        }

        public override string Condicional(string condicao, string valorSeVerdadeiro, string valorSeFalso)
        {
            return $"IIF ({condicao}, {valorSeVerdadeiro}, {valorSeFalso})";
        }
    }
}