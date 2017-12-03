using System;
using System.Collections.Generic;
using DataBase;
using DataBase.Carregadores;
using Dominio.Regras;
using DTO;
using TraderWizard.Enumeracoes;

namespace TraderWizard.ServicosDeAplicacao
{
    public class ValoresExtremosService
    {

        private readonly Conexao _conexao;
        private readonly CotacaoDataService _cotacaoData;
        private readonly GeradorQuery _geradorQuery;

        public ValoresExtremosService()
        {
            _conexao = new Conexao();
            _cotacaoData = new CotacaoDataService();
            _geradorQuery = new GeradorQuery(_conexao.ObterFormatadorDeCampo());
        }

        //~ValoresExtremosService()
        //{
        //    this._conexao.FecharConexao();
        //}
        /// <summary>
        /// Calcula os valores máximo e mínimo das áreas de desenho do gráfico  (cotação, volume, ifr). 
        /// Função unitária que não considera splits.
        /// </summary>
        private ValoresExtremos ValoresExtremosUnitarioCalcular(ConfiguracaoDeVisualizacao configuracaoDeVisualizacao, double pdblOperador,
            double pdblOperadorInvertido)
        {
            RS objRS = new RS(_conexao);

            string strTabelaCotacao = string.Empty;
            string strTabelaMedia = string.Empty;
            string strTabelaIFR = string.Empty;

            CalculadorTabelas.TabelasCalcular(configuracaoDeVisualizacao.Periodicidade, ref strTabelaCotacao, ref strTabelaMedia, ref strTabelaIFR);

            string strQuery = _geradorQuery.ConsultaUnitariaGerar(configuracaoDeVisualizacao.CodigoAtivo, configuracaoDeVisualizacao.DataInicial, configuracaoDeVisualizacao.DataFinal
                , strTabelaCotacao, pdblOperador, pdblOperadorInvertido, "EXTREMOS");

            objRS.ExecuteQuery(strQuery);

            if (Convert.ToInt32(objRS.Field("ContadorVolumeMedio")) == 0)
            {
                return null;
            }

            decimal pdecValorMinimoRet = Convert.ToDecimal(objRS.Field("ValorMinimo"));
            decimal pdecValorMaximoRet = Convert.ToDecimal(objRS.Field("ValorMaximo"));
            double pdblVolumeMinimoRet = 0;
            double pdblVolumeMaximoRet = 0;
            int pintContadorIFRRet = 0;
            int pintVolumeMedioNumRegistrosRet = 0;

            if (configuracaoDeVisualizacao.VolumeDesenhar)
            {
                pdblVolumeMinimoRet = Convert.ToDouble(objRS.Field("Volume_Minimo"));

                pdblVolumeMaximoRet = Convert.ToDouble(objRS.Field("Volume_Maximo"));

            }

            objRS.Fechar();

            //******************INICIO DO TRATAMENTO PARA O IFR******************

            if (configuracaoDeVisualizacao.IFRDesenhar)
            {

                FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

                //Calcula o número de registros que serão desenhados do IFR de 2 períodos 
                strQuery = " SELECT COUNT(1) AS ContadorIFR" +
                           " FROM " + strTabelaIFR +
                           " WHERE Codigo = " + funcoesBd.CampoStringFormatar(configuracaoDeVisualizacao.CodigoAtivo) +
                           " AND NumPeriodos = " + configuracaoDeVisualizacao.IFRNumPeriodos +
                           " AND Data >= " + funcoesBd.CampoDateFormatar(configuracaoDeVisualizacao.DataInicial) +
                           " AND Data <= " + funcoesBd.CampoDateFormatar(configuracaoDeVisualizacao.DataFinal);

                objRS.ExecuteQuery(strQuery);

                pintContadorIFRRet = Convert.ToInt16(objRS.Field("ContadorIFR"));

                objRS.Fechar();


            }
            //********************FIM DO TRATAMENTO PARA O IFR*******************

            //********************INICIO DO TRATAMENTO PARA O VOLUME MÉDIO*******************


            if (configuracaoDeVisualizacao.VolumeDesenhar)
            {
                //busca os valores máximo e mínimo da média de 21 períodos do volume
                strQuery = _geradorQuery.ConsultaUnitariaGerar(configuracaoDeVisualizacao.CodigoAtivo, configuracaoDeVisualizacao.DataInicial, configuracaoDeVisualizacao.DataFinal, strTabelaMedia, pdblOperador, pdblOperadorInvertido, "EXTREMOS", "A", 21, "VOLUME");

                objRS.ExecuteQuery(strQuery);


                if (Convert.ToDouble(objRS.Field("ValorMinimo")) < pdblVolumeMinimoRet)
                {
                    pdblVolumeMinimoRet = Convert.ToDouble(objRS.Field("ValorMinimo"));

                }


                if (Convert.ToDouble(objRS.Field("ValorMaximo")) > pdblVolumeMaximoRet)
                {
                    pdblVolumeMaximoRet = Convert.ToDouble(objRS.Field("ValorMaximo"));

                }

                pintVolumeMedioNumRegistrosRet = Convert.ToInt16(objRS.Field("NumRegistros"));

                objRS.Fechar();

            }

            //********************FIM DO TRATAMENTO PARA O VOLUME MÉDIO*******************

            //********************INICIO DO TRATAMENTO PARA AS MÉDIAS MÓVEIS*******************


            var plstMediasRet = new List<MediaDTO>();
            if (configuracaoDeVisualizacao.MediaDesenhar)
            {


                //percorre a collection que contém todas as médias que serão desenhadas e calcula
                //os valores máximo e mínimo no período em que os dados serão desenhados.


                foreach (MediaDTO mediaDto in configuracaoDeVisualizacao.MediasSelecionadas)
                {
                    //calcula a tabela para as médias
                    strQuery = _geradorQuery.ConsultaUnitariaGerar(configuracaoDeVisualizacao.CodigoAtivo, configuracaoDeVisualizacao.DataInicial, configuracaoDeVisualizacao.DataFinal, strTabelaMedia, pdblOperador, pdblOperadorInvertido, "EXTREMOS", mediaDto.Tipo, mediaDto.NumPeriodos, "VALOR");

                    objRS.ExecuteQuery(strQuery);


                    if (Convert.ToDecimal(objRS.Field("ValorMinimo", 0)) > 0)
                    {

                        if (Convert.ToDecimal(objRS.Field("ValorMinimo")) < pdecValorMinimoRet)
                        {
                            pdecValorMinimoRet = Convert.ToDecimal(objRS.Field("ValorMinimo"));

                        }

                    }


                    if (Convert.ToDecimal(objRS.Field("ValorMaximo", 0)) > 0)
                    {

                        if (Convert.ToDecimal(objRS.Field("ValorMaximo")) > pdecValorMaximoRet)
                        {
                            pdecValorMaximoRet = Convert.ToDecimal(objRS.Field("ValorMaximo"));

                        }

                    }

                    //atribui na estrutura de médias o número de registros encontrados.
                    //será utilizada posteriormente para redimensionar os arrays de médias
                    plstMediasRet.Add(new MediaDTO(mediaDto.Tipo, mediaDto.NumPeriodos, "VALOR", Convert.ToInt16(objRS.Field("NumRegistros"))));

                    objRS.Fechar();

                }

            }
            //If blnMMExpDesenhar Then...

            //********************FIM DO TRATAMENTO PARA AS MÉDIAS MÓVEIS EXPONENCIAIS*******************

            return new ValoresExtremos(pdecValorMinimoRet, pdecValorMaximoRet, pdblVolumeMinimoRet, pdblVolumeMaximoRet, pintContadorIFRRet, plstMediasRet, pintVolumeMedioNumRegistrosRet);
        }


        /// <summary>
        /// Calcula os valores máximo e mínimo das áreas de desenho do gráfico  (cotação, volume, ifr).
        /// Função que calcula para toda a área de dados e faz tratamento para os splits.
        /// </summary>
        public ValoresExtremos ValoresExtremosCalcular(ConfiguracaoDeVisualizacao configuracaoDeVisualizacao)
        {
            RSList objRSListSplit = null;

            double dblOperador = 1;
            double dblOperadorInvertido = 1;

            //inicializa os operadores com 1 para que a multiplicação até o primeiro split não altere os valores
            double dblOperadorAux = 1;
            double dblOperadorInvertidoAux = 1;

            ValoresExtremos retorno = null;

            //sempre considera a data inicial de geração do gráfico mais um, pois se tiver um split na data 
            //do primeiro dia de gráfico não tem valores para serem convertidos.
            //***Ordena os splits em ordem DECRESCENTE de data.

            CarregadorSplit objCarregadorSplit = new CarregadorSplit(_conexao);

            bool blnSplitExistir = objCarregadorSplit.SplitConsultar(configuracaoDeVisualizacao.CodigoAtivo, configuracaoDeVisualizacao.DataInicial.AddDays(1), "D", ref objRSListSplit, Constantes.DataInvalida);

            if (!blnSplitExistir)
                return ValoresExtremosUnitarioCalcular(configuracaoDeVisualizacao, dblOperador, dblOperadorInvertido);

            //Tem split. Necessário percorrer o RS de splits e gerar queries para cada dado a cada split.

            DateTime dtmDataMinima = Convert.ToDateTime(objRSListSplit.Field("Data"));
            //Inicializa a data máxima com a data final porque na primeira iteração, se já for necessário calcular valores (dtmDataMinima < pdtmDataFinal)
            //os valores têm que ser calculados da data do primeiro split até a data final do gráfico.
            DateTime dtmDataMaxima = configuracaoDeVisualizacao.DataFinal;

            //Utilizada para saber se encontrou um item na collection de médias.

            bool blnRealizarConsultaUnitaria = false;
            bool blnAtribuirOperador = false;

            //Fica em loop calculando os valores extremos até que a data máxima fique menor do que a data mínima.
            //Isto vai acontecer logo após a iteração referente as datas entre a data inicial (pdtmDataInicial) 
            //e o Split com menor data, que no caso será o último split do RS, pois o mesmo está em ordem 
            //decrescente de data.
            //Tem que colocar um OR pelo EOF do RS porque quando a área de desenho vai mostrar os dados que não são as últimas cotações
            //pode acontecer de inicialmente a data máxima ser menor do que a data mínima. Isto vai acontecer se a data máximo for menor 
            //do que a data do split mais recente.

            while (dtmDataMaxima >= dtmDataMinima || !objRSListSplit.Eof)
            {

                if (!objRSListSplit.Eof)
                {

                    if (configuracaoDeVisualizacao.Periodicidade == "SEMANAL")
                    {
                        //quando a cotação é semanal, a data inicial tem que ser a data menor ou igual a data do split,
                        //pois se ocorrer um split em uma data que não for a primeira data da semana, toda as cotações
                        //desta semana já foram convertidas para a razão do split
                        dtmDataMinima = _cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(configuracaoDeVisualizacao.CodigoAtivo, dtmDataMinima);

                    }

                }


                if (configuracaoDeVisualizacao.Periodicidade == "DIARIO")
                {
                    blnRealizarConsultaUnitaria = dtmDataMinima <= configuracaoDeVisualizacao.DataFinal && dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


                }
                else if (configuracaoDeVisualizacao.Periodicidade == "SEMANAL")
                {
                    blnRealizarConsultaUnitaria = dtmDataMinima <= configuracaoDeVisualizacao.DataFinal && dtmDataMinima != _cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(configuracaoDeVisualizacao.CodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));
                }

                if (blnRealizarConsultaUnitaria)
                {
                    //tem que gerar a tabela somente se o split estiver dentro da área de dados,
                    //senão tem que ficar calculando apenas o operador para fazer as multiplicações depois.

                    //Regra do parâmetro "pdtmDataFinal": se a data final de busca (pdtmDataFinal) for menor que a data maxima da iteração
                    //tem que utilizar a data final, senão utiliza a data máxima
                    ValoresExtremos valoresExtremos = ValoresExtremosUnitarioCalcular(configuracaoDeVisualizacao.AlterarPeriodo(dtmDataMinima, dtmDataMaxima), dblOperador, dblOperadorInvertido);
                    if (valoresExtremos != null)
                    {
                        retorno = retorno == null ? valoresExtremos : retorno.AgruparCom(valoresExtremos);
                    }                     

                    //para a próxima iteração a data máxima é a data anterior ao split que está acabando
                    dtmDataMaxima = dtmDataMinima.AddDays(-1);

                }

                if (objRSListSplit.Eof) continue;

                dblOperadorAux = dblOperadorAux * Convert.ToDouble(objRSListSplit.Field("Razao"));

                //A razão invertida só é utilizada no cálculo do volume. 
                //O volume só deve utilizar os splits de desdobramento (tipo = 'DESD').
                //Por isso, só multiplica o operador invertido pela razão invertida neste caso.
                if ((string)objRSListSplit.Field("Tipo") == "DESD")
                {
                    dblOperadorInvertidoAux = dblOperadorInvertidoAux * Convert.ToDouble(objRSListSplit.Field("RazaoInvertida"));
                }

                //Ajusta o operador quando a próxima data for diferente.
                //Tem que colocar este IF antes do MOVENEXT, pois caso contrário vai alterar o operador antes de chamar
                //a função "ConsultaUnitariaGerar", fazendo com que os valores sejam multiplicados em um intervalo 
                //imediatamente anterior ao que deve ser multiplicado



                if (configuracaoDeVisualizacao.Periodicidade == "DIARIO")
                {
                    blnAtribuirOperador = dtmDataMinima != Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida));


                }
                else if (configuracaoDeVisualizacao.Periodicidade == "SEMANAL")
                {
                    blnAtribuirOperador = dtmDataMinima != _cotacaoData.AtivoCotacaoSemanalPrimeiroDiaSemanaCalcular(configuracaoDeVisualizacao.CodigoAtivo, Convert.ToDateTime(objRSListSplit.NextField("Data", Constantes.DataInvalida)));

                }


                if (blnAtribuirOperador)
                {
                    dblOperador = dblOperadorAux;
                    dblOperadorInvertido = dblOperadorInvertidoAux;

                }


                objRSListSplit.MoveNext();

                dtmDataMinima = objRSListSplit.Eof ? configuracaoDeVisualizacao.DataInicial : Convert.ToDateTime(objRSListSplit.Field("Data"));
            }

            return retorno;
        }

    }
}
