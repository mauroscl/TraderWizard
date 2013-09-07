using System.Text.RegularExpressions;

namespace TraderWizard.Extensoes
{
    public static class NumericExtension
    {
        public static bool IsNumeric(this string numero)
        {
            //aceita número com separador de milhares (",") e separador decimal ("."). Ambos são opcionais.
            var expressaoRegular = new Regex(@"^(\+|\-)?((\d{1,3}(,\d{3})+)|(\d+))(\.\d+)?$");
             
            return !string.IsNullOrEmpty(numero) && expressaoRegular.IsMatch(numero.Trim());
        }
    }
}