namespace TraderWizard.ServicosDeAplicacao
{
    public class ConversorDeListaDeAtivos
    {
        public static string[] ConverterParaArray(string ativos)
        {
            if (ativos.Length == 0)
            {
                return new string[]{};
            }
            //remove sustenido do inicio
            string ativosAux = ativos.Remove(0, 1);

            //remove ultimo sustenido
            ativosAux = ativosAux.Remove(ativosAux.Length - 1);

            //faz split pelo sustenido
            string[] ativosSelecionados = ativosAux.Split('#');
            return ativosSelecionados;
        }
    }
}
