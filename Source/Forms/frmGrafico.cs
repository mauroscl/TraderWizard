using DataBase;
using DataBase.Carregadores;
using Forms.Properties;
using prjCandle;
using DTO;
using prmCotacao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dominio.Entidades;
using prjCandle.Desenho;
using TraderWizard.Enumeracoes;
using TraderWizard.Extensoes;
using TraderWizard.Infra.Repositorio;

namespace TraderWizard
{
    public partial class frmGrafico
    {

        private const int intMargemSuperior = 3;

        //tamanho da margem inferior e superior para as áreas de indicadores como IFR e VOLUME

        private const int intIndicadoresMargem = 2;
        //altura total de cada área de indicadores

        private int intIndicadoresAreaHeight;

        //largura total entre candles (o padrão é 4 do corpo + 3 de espaçamento)
        private int intLarguraTotal;

        //largura do corpo do candle (o padrão é 4 de corpo)
        private int intCandleWidth;

        //Indica se os dados sobre as cotações e os indicadores devem ser buscados no banco de dados

        private bool blnDadosAtualizar = true;
        //Indica se as coordenadas dos pontos de desenhos dos gráficos devem ser recalculados.

        private bool blnPontosAtualizar = true;

        //collection contendos os labels
        private readonly Collection<Control> _labelsVerticais = new Collection<Control>();

        //código do ativo que está impresso na tela
        private string strCodigoAtivo;

        private Candle[] arrCandle = { };

        private PointF[] arrIFRPonto = { };
        private Rectangle[] arrVolumeRectangle = { };
        private PointF[] arrVolumeMedio = { };

        //collection contendo as médias que serão impressas. A collection será preenchida com itens da estrutura structMedia

        IList<Indicador> colMedia = new List<Indicador>();
        //retângulo contendo a área de impressão do gráfico. 
        //Considera como área do gráfico também a área de estudos como volume e IFR.

        private Rectangle objGraficoArea;

        //-------------------------------------------------------------------------------------------
        //largura total da área do gráfico
        int intAreaWidth;

        //coordenada Y do ponto mais alto da área de impressão do gráfico
        int intAreaTop;

        //coordenada Y do ponto mais alto da área de impressão do gráfico 
        int intAreaBottom;

        //coordenada X do ponto mais à esquerda da área de impressão do gráfico
        int intAreaLeft;

        //coordenada X do ponto mais à direita da área de impressão do gráfico
        int intAreaRight;

        //altura da área de impreessão do gráfico
        int intAreaTotalHeight;
        //-------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------
        //altura da área que são impressas as cotações. Não considera volume, IFR e outros indicadores
        int intAreaCotacaoHeight;

        //coordenada Y da parte inferior da área de cotação
        int intAreaCotacaoBottom;
        //-------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------
        //dados utilizados para desenhar os labels verticais, com os valores das cotações do gráfico
        //menor valor para o qual será impresso um label de referência de valor
        decimal decValorInicialReferencia;

        //intervalo de valor que será impresso cada valor de referência
        decimal decIntervaloReferencia;

        //maior valor para o qual será impresso um label de referência de valor
        decimal decValorMaximoPeriodoReferencia;
        //-------------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------------
        //dados utilizados para desenhar os labels verticais, com os volumes das cotações do gráfico
        //menor volume para o qual será impresso um label de referência de volume
        double dblVolumeInicialReferencia;

        //intervalo de volume que será impresso cada volume de referência
        double dblVolumeIntervaloReferencia;

        //maior volume para o qual será impresso um label de referência de volume
        double dblVolumeMaximoPeriodoReferencia;
        //-------------------------------------------------------------------------------------------

        //valor máximo que será impresso no gráfico considerando os candles e a média móvel

        decimal decValorMaximoPeriodo;
        //indica quantos pixes são utilizados para imprimir 1 real

        double dblPixelPorReal;
        //indica o número de pixels que corresponde a cada ponto do IFR

        double dblPixelPorIFR;
        //indica o número de pixels que cada titulo negociado representa na área de impressão de volumes

        double dblPixelPorNegocio;

        //quanto o maior volume impresso no gráfico de períodos, quando este for impresso.
        double dblVolumeMaximoPeriodo;


        bool blnVolumeDesenhar;
        //INDICA SE É PARA DESENHAR O IFR. FICA ARMAZENADO  EM VARIÁVEL PARA QUANDO FOR ATUALIZAR
        //O GRÁFICO VERIFICAR SE HOUVE ALTERAÇÕES E VERIFICAR SE É NECESSÁRIO BUSCAR NOVAMENTE OS DADOS

        bool blnIFRDesenhar;
        //CONTÉM O NÚMERO DE PERÍODOS SELECIONADO PARA APRESENTAR O GRÁFICO DO IFR

        int intIFRNumPeriodos = -1;

        bool blnMMExpDesenhar;
        int intIFRAreaTop;

        int intVolumeAreaTop;
        //indica se os dados estão sendo atualizados, para evitar loops

        bool blnAtualizandoDados;
        //indica se os pontos estão sendo atualizados, para evitar loops

        bool blnAtualizandoPontos; //Indica se o gráfico está sendo desenhado, para evitar loops.

        bool blnGraficoDesenhando;

        bool blnGraficoGerando;

        int intVolumePosicaoSelecionada = -1;


        private readonly Conexao _conexao;

        List<MediaDTO> _mediasSelecionadas;

        private struct structSplit
        {
            //data do split
            //ex: 1/2
            public string strRazao;

            //posiçao de desenho
            public int intPosicaoX;
        }

        //contém os splits do ativo que está desenhado.
        //a collection contém itens do tipo structSplit

        IList<structSplit> colSplit;

        private AreaDeDesenho _areaDeDesenho;
        private FerramentaDeDesenho _ferramentaSelecionada;

        //lista que contém os objetos que são desenhadas na tela 
        //pelo usuário: linha horizontal, linha de tendência, canal, fibonacci
        private readonly IList<Desenho> _desenhosCriados;
        //posição X do primeiro candle da esquerda para a direita.

        int intCandle1X;
        //indica o zoom da tela. Existem sete tipos de zoom. 
        //o zoom padrão é o 4, que é o maior deles.

        int intZoom = 4;

        //PERIODO DE DURAÇÃO DOS DADOS: DIÁRIO, SEMANAL
        string strPeriodoDuracao;

        //TABELA UTILIZADA PARA BUSCAR OS DADOS DAS COTAÇÕES
        string strTabelaCotacao;

        //TABELA UTILIZADA PARA BUSCAR O IFR DAS COTAÇÕES
        string strTabelaIFR;

        //posições inicial e final do array de candles dos quais os dados estão sendo desenhados.
        int intArrayCandlePosicaoInicial;

        int intArrayCandlePosicaoFinal;
        //data mínima de todos os candles que estão carregados no array de dados (arrCandle).

        DateTime dtmDataMinimaAreaDados;
        //data máxima de todos os candles que estão carregados no array de dados (arrCandle).

        DateTime dtmDataMaximaAreaDados;
        //data da última cotação do papel independentemente se está ou não na área de dados.

        //Quando TRUE indica se o botão do mouse foi preessionado e ainda não foi solto e o mouse
        //ainda está na área de desenho.
        bool blnMouseDown;
        //ponto em que o mouse foi clicado.

        Point objMouseDownPonto;
        //contém o tamanho máximo dos arrays de dados.

        int intAreaDadosTamanhoMaximo;

        //contém os sequeciais inicial que estão carregados nos array de dados: arrCandle, arrVolume, etc.
        //inicializa estas variáveis com "-1" porque neste caso é utilizado um controle 
        //que calcula estas variáveis na primeira vez em que são buscados dados para um
        //determinado ativo em uma determinada periodicidade.
        long lngAreaDadosSequencialInicial = -1;

        long lngAreaDadosSequencialFinal = -1;
        //valor máximo do Sequencial do ativo na tabela de Cotacao. Isto indica o número de registros
        //que existe na tabela de cotações, pois o Sequencial começa em 1.

        long lngSequencialMaximo = -1;

        //contém o sequencial inicial e o sequencial final da área de desenho do gráfico
        long lngAreaDesenhoSequencialInicial;

        long lngAreaDesenhoSequencialFinal;


        public frmGrafico(Conexao pobjConexao)
        {
            /*MouseLeave += frmGrafico_MouseLeave;
            MouseUp += frmGrafico_MouseUp;
            MouseDown += frmGrafico_MouseDown;
            MouseMove += frmGrafico_MouseMove;
            MouseClick += frmGrafico_MouseClick;
            Paint += frmGrafico_Paint;
            Load += frmGrafico_Load;*/
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            _conexao = pobjConexao;
            _desenhosCriados = new List<Desenho>();

        }


        private void frmGrafico_Load(System.Object sender, System.EventArgs e)
        {
            //PREENCHE O COMBO OS PAPÉIS EXISTENTES E SETA O ATIVO PADRÃO DE ACORDO COM A CONFIGURAÇÃO.
            ToolStripcmbAtivoPreencher();

            //PREENCHE O COMBO PARA ESCOLHER SE O GRÁFICO É SEMANAL OU DIÁRIO
            ToolStripcmbPeriodoDuracaoPreencher();

            //BUSCA parâmetro indicando se deve imprimir as médias
            string strValor;

            //***************MMExpDesenhar***************
            ParametroConsultar("MMExpDesenhar", out strValor);


            if (strValor == "SIM")
            {
                TSmnuMMExibir.Checked = true;

                IndicadorCarregar();

            }
            //***************MMExpDesenhar***************

            //***************IFRDesenhar***************
            ParametroConsultar("IFRDesenhar", out strValor);


            if (strValor == "SIM")
            {
                ToolStripbtnIFR14.Checked = true;

                ToolStripcmbIFRNumPeriodos.Enabled = true;

                ToolStripcmbIFRNumPeriodos.Text = "2";

            }
            //***************IFRDesenhar***************

            //***************IFRDesenhar***************
            ParametroConsultar("VolumeDesenhar", out strValor);

            if (strValor == "SIM")
            {
                ToolStripbtnVolume.Checked = true;

            }
            //***************IFRDesenhar***************

            //ajusta localização e tamanho do form
            this.Location = new Point(0, 0);
            this.Height = MdiParent.ClientSize.Height - 30;
            this.Width = MdiParent.ClientSize.Width - 4;

            //ajusta propriedades que indicam a área de impressão 
            intAreaWidth = this.Width - 105;
            intAreaTop = ToolStripPrincipal.Height + 10;
            intAreaLeft = 5;
            intAreaRight = this.Width - 100;

            intAreaTotalHeight = (this.ClientSize.Height - intAreaTop - StatusStrip.Height - 30);

            intAreaBottom = intAreaTop + intAreaTotalHeight;

            //cálculo da altura da área de indicadores: IFR, MÉDIA.
            //utiliza 15% da área total de impressão do gráfico.
            intIndicadoresAreaHeight = Convert.ToInt32(intAreaTotalHeight * 0.15);

            //calcula a largura total e a largura do corpo de cada um dos candles em função do zoom.
            ZoomMedidasCandleCalcular();

            //calcula o tamanho máximo do array de dados. O tamanho é sempre 4 vezes o
            //número de candles que pode ser desenhado em uma janela de dados porque 
            //o zoom mínimo é 4 vezes o tamanho da área de dados.
            intAreaDadosTamanhoMaximo = Convert.ToInt32((intAreaWidth - intMargemSuperior * 3) / intLarguraTotal) * 4;

        }


        private void ToolStripcmbAtivoPreencher()
        {

            string strCodigoAtivoPadrao;

            //consulta o ativo padrão
            ParametroConsultar("AtivoPadrao", out strCodigoAtivoPadrao);

            int intIndicePadrao = 0;

            ToolStripcmbAtivo.Items.Clear();

            var ativos = new Ativos(_conexao);

            IList<Ativo> ativosValidos = ativos.Validos();

            foreach (var ativoValido in ativosValidos)
            {
                ToolStripcmbAtivo.Items.Add(ativoValido.Codigo + " - " + ativoValido.Descricao);
                if (ativoValido.Codigo == strCodigoAtivoPadrao)
                {
                    intIndicePadrao = ToolStripcmbAtivo.Items.Count - 1;

                }

            }

            if (intIndicePadrao > 0)
            {
                ToolStripcmbAtivo.SelectedIndex = intIndicePadrao;
            }

        }


        private void ToolStripcmbPeriodoDuracaoPreencher()
        {
            ToolStripcmbPeriodoDuracao.Items.Clear();

            ToolStripcmbPeriodoDuracao.Items.Add("Diário");

            ToolStripcmbPeriodoDuracao.Items.Add("Semanal");

            ToolStripcmbPeriodoDuracao.SelectedIndex = 0;

        }

        /// <summary>
        /// Verifica se um ponto pertence à area de dados. A área de dados não é toda a área do gráfico e assim
        /// a área que possui candles, ou seja, que possui dados
        /// </summary>
        /// <param name="pobjPonto"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool AreaDadosPontoPertencer(Point pobjPonto)
        {
            bool functionReturnValue = pobjPonto.X >= intCandle1X && pobjPonto.X <= intAreaRight - 3 &&
                                       pobjPonto.Y >= intAreaTop + 3 && pobjPonto.Y <= intAreaCotacaoBottom - 6;


            return functionReturnValue;

        }

        /// <summary>
        /// Busca os dados do banco de dados e carrega para a memória
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool DadosAtualizar()
        {

            if (blnAtualizandoDados)
            {
                //se já está atualizando sai da função para evitar loops
                return false;

            }


            //*************************CONTROLE PRECISA SER COLOCADO BEM NO COMEÇO DA PROCEDURE
            //*************************NÃO PODE SER EMITIDA NENHUMA MENSAGEM ANTES DE MARCAR
            //*************************A VARIÁVEL BLNATUALIZANDO = TRUE.
            //marca que está atualizando os dados para não entrar em loop.
            //se não houver esta e for gerada alguma mensagem pelo programa,
            //o evento PAINT é chamado várias vezes e o programa entra em LOOP.
            blnAtualizandoDados = true;

            _ferramentaSelecionada = null;

            bool blnCotacaoBuscar;
            bool blnIfrBuscar;
            bool blnVolumeBuscar;

            long lngSequencialBuscaInicial = 0;
            long lngSequencialBuscaFinal = 0;

            string strPeriodoDuracaoAux = (string) ToolStripcmbPeriodoDuracao.SelectedItem == "Diário"
                ? "DIARIO"
                : "SEMANAL";


            if ((string) ToolStripcmbPeriodoDuracao.SelectedItem == "Diário")
            {
                strTabelaCotacao = "Cotacao";
                strTabelaIFR = "IFR_DIARIO";


            }
            else
            {
                strTabelaCotacao = "Cotacao_Semanal";
                strTabelaIFR = "IFR_SEMANAL";

            }

            FuncoesBd FuncoesBd = _conexao.ObterFormatadorDeCampo();

            cRS objRS = new cRS(_conexao);

            ServicoDeCotacao objCotacao = new ServicoDeCotacao(_conexao);

            //código do ativo atual.

            string strCodigoAtivoAux =
                mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo((string) ToolStripcmbAtivo.SelectedItem);

            if (strCodigoAtivo != strCodigoAtivoAux || strPeriodoDuracao != strPeriodoDuracaoAux)
            {
                //se trocou o período do gráfico ou o ativo, tem que buscar as cotações
                blnCotacaoBuscar = true;
                _areaDeDesenho = null;

                //calcula o sequencial inicial e o final para impressão

                //1º) busca o sequencial e a data da última cotação do papel
                objRS.ExecuteQuery("SELECT TOP 1 Sequencial " + Environment.NewLine
                                   + " FROM " + strTabelaCotacao + Environment.NewLine
                                   + " WHERE Codigo = " + FuncoesBd.CampoStringFormatar(strCodigoAtivoAux) +
                                   Environment.NewLine
                                   + " ORDER BY Sequencial DESC");

                lngSequencialMaximo = Convert.ToInt64(objRS.Field("Sequencial"));

                objRS.Fechar();

                //2º) sempre começamos mostrando os dados mais recentes, por isso o sequencial final é o sequencial máximo
                lngAreaDadosSequencialFinal = lngSequencialMaximo;


                if (lngAreaDadosSequencialFinal > intAreaDadosTamanhoMaximo)
                {
                    //se tem mais candles do que a capacidade da área de dados, a posição inicial
                    //é o sequencial máximo subtraído da área de dados e somado 1.
                    lngAreaDadosSequencialInicial = lngAreaDadosSequencialFinal - intAreaDadosTamanhoMaximo + 1;


                }
                else
                {
                    //se tem menos candles que o número máximo permitido pela janela de dados, então começa
                    //em 1 e mostra todos os candles existentes.
                    lngAreaDadosSequencialInicial = 1;

                }

                //nestes casos tem que buscar toda a área de dados, então tem que atualizar as variáveis de 
                //controle que indicarão efetivamente o range de busca de dados igual ao tamanho da ára de dados
                lngSequencialBuscaInicial = lngAreaDadosSequencialInicial;
                lngSequencialBuscaFinal = lngAreaDadosSequencialFinal;

                //calcula o sequencial inicial e final da área de desenho.
                int intNumCandlesDesenhar = AreaDesenhoNumCandlesCalcular();


                if ((lngSequencialMaximo - intNumCandlesDesenhar + 1) >= 1)
                {
                    lngAreaDesenhoSequencialInicial = lngSequencialMaximo - intNumCandlesDesenhar + 1;


                }
                else
                {
                    lngAreaDesenhoSequencialInicial = 1;

                }

                //começa desenhando da esquerda para a direita, ou seja, mostrando os dados mais recentes.
                lngAreaDesenhoSequencialFinal = lngSequencialMaximo;

                //redimensiona o array de candles conforme o tamanho necessário para armazenar os dados

                if (lngSequencialMaximo >= intAreaDadosTamanhoMaximo)
                {
                    //caso o número de candles de toda a base seja maior do que a área de dados
                    //o tamanho do array fica restringido ao tamanho máximo da área de dados.
                    Array.Resize(ref arrCandle, intAreaDadosTamanhoMaximo);


                }
                else
                {
                    //caso o número de candles de toda a base seja menor do que o tamanho máximo da área de dados
                    //o tamanho do array é o tamanho do número de candles.
                    Array.Resize(ref arrCandle, (int) lngSequencialMaximo);

                }



            }
            else
            {
                //Entra neste trecho de código caso não tenha trocado de ativo ou de periodicidade.

                //verifica se a área de impressão está fora da área de dados.

                if (lngAreaDesenhoSequencialInicial < lngAreaDadosSequencialInicial)
                {
                    //caso a área de desenho necessite de dados mais para a esquerda da área de dados

                    //os dados que devem ser buscados por esta função começam no sequencial inicial da área de desenho
                    //e terminam uma posição antes do sequencial inicial da área de dados.
                    lngSequencialBuscaInicial = lngAreaDesenhoSequencialInicial;
                    lngSequencialBuscaFinal = lngAreaDadosSequencialInicial - 1;

                    //move os dados do array da direita para a esquerda para poder colocar os novos dados que serão buscados.
                    Array.ConstrainedCopy(arrCandle, 0, arrCandle,
                        (int) (lngSequencialBuscaFinal - lngSequencialBuscaInicial + 1),
                        (int) (arrCandle.Length - lngSequencialBuscaFinal + lngSequencialBuscaInicial - 1));

                    //ajusta a área de dados
                    lngAreaDadosSequencialInicial = lngAreaDesenhoSequencialInicial;
                    lngAreaDadosSequencialFinal = lngAreaDadosSequencialInicial + intAreaDadosTamanhoMaximo - 1;

                    blnCotacaoBuscar = true;


                }
                else if (lngAreaDesenhoSequencialFinal > lngAreaDadosSequencialFinal)
                {
                    //caso a área de desenho necessite de dados mais para a direita da área de dados

                    //os dados que devem ser buscados por esta função começam uma posição 
                    //após o sequencial final da área de dados
                    //e terminam no sequencial final da área de desenho.
                    lngSequencialBuscaInicial = lngAreaDadosSequencialFinal + 1;
                    lngSequencialBuscaFinal = lngAreaDesenhoSequencialFinal;

                    //move os dados do array da direita para a esquerda para poder colocar os novos dados que serão buscados.
                    Array.ConstrainedCopy(arrCandle, (int) (lngSequencialBuscaFinal - lngSequencialBuscaInicial + 1),
                        arrCandle, 0,
                        (int) (arrCandle.Length - lngSequencialBuscaFinal + lngSequencialBuscaInicial - 1));

                    //ajusta a área de dados
                    lngAreaDadosSequencialFinal = lngAreaDesenhoSequencialFinal;
                    lngAreaDadosSequencialInicial = lngAreaDesenhoSequencialFinal - intAreaDadosTamanhoMaximo + 1;

                    blnCotacaoBuscar = true;


                }
                else
                {
                    //caso  a área de desenho esteja contida na área de dados não precisa fazer a busca das cotações.
                    blnCotacaoBuscar = false;

                }

            }

            //a troca do período de duração só pode ser feita após a comparação do IF anterior
            //(strPeriodoDuracao <> strPeriodoDuracaoAux). 

            strPeriodoDuracao = (string) ToolStripcmbPeriodoDuracao.SelectedItem == "Diário" ? "DIARIO" : "SEMANAL";


            if (ToolStripbtnIFR14.Checked)
            {

                if (blnCotacaoBuscar)
                {
                    //SE É PARA BUSCAR AS COTAÇÕES NOVAMENTE OU
                    //TEM QUE BUSCAR OS DADOS DO IFR
                    blnIfrBuscar = true;


                }
                else
                {

                    if (blnIFRDesenhar)
                    {
                        //SE O IFR JÁ ESTAVA SENDO DESENHADO, VERIFICA SE OS PERÍODOS DO IFR
                        //SÃO IGUAIS OU DIFERENTES
                        if (intIFRNumPeriodos == Convert.ToInt32(ToolStripcmbIFRNumPeriodos.Text))
                        {
                            //SE OS PERÍODOS SÃO OS MESMOS NÃO PRECISA BUSCAR OS DADOS DO IFR
                            blnIfrBuscar = false;
                        }
                        else
                        {
                            //SE OS PERÍODOS SÃO DIFERENTES PRECISA BUSCAR OS DADOS DO IFR
                            blnIfrBuscar = true;
                        }


                    }
                    else
                    {
                        //SE ANTES DESTA ATUALIZAÇAO O IFR NÃO ESTAVA SENDO DESENHADO,
                        //TEM QUE BUSCAR OS DADOS DO IFR
                        blnIfrBuscar = true;

                    }

                }

            }
            else
            {
                //não está marcado para buscar o IFR.
                blnIfrBuscar = false;
            }


            if (ToolStripbtnVolume.Checked)
            {
                if (blnCotacaoBuscar || !blnVolumeDesenhar)
                {
                    blnVolumeBuscar = true;
                }
                else
                {
                    blnVolumeBuscar = false;
                }
            }
            else
            {
                blnVolumeBuscar = false;
            }

            //limpa conteúdo de array globais da tela.
            //o objetivo é liberar memória.
            int intI = 0;

            int intIndiceInicial = -1;
            int intIndiceFinal = -1;



            if (blnCotacaoBuscar)
            {
                //calcula os indices inicial e final do array de candles para os quais serão 
                //buscadas novas cotações
                ArrayDadosSequencialIndicesCalcular(lngSequencialBuscaInicial, lngSequencialBuscaFinal,
                    ref intIndiceInicial, ref intIndiceFinal);

                //For intI = 0 To arrCandle.Length - 1

                for (intI = intIndiceInicial; intI <= intIndiceFinal; intI++)
                {
                    arrCandle[intI] = null;

                }

            }

            //marca variáveis que marcam se os indicadores devem ser desenhados
            blnVolumeDesenhar = ToolStripbtnVolume.Checked;

            blnIFRDesenhar = ToolStripbtnIFR14.Checked;

            if (blnIFRDesenhar)
            {
                intIFRNumPeriodos = Convert.ToInt32(ToolStripcmbIFRNumPeriodos.Text);
            }

            blnMMExpDesenhar = (TSmnuMMExibir.Checked) && ((_mediasSelecionadas != null));

            if (strCodigoAtivo !=
                mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo((string) ToolStripcmbAtivo.SelectedItem))
            {

                _desenhosCriados.Clear();

            }


            strCodigoAtivo = mdlGeral.ObtemCodigoDoAtivoSelecionadoNoCombo((string) ToolStripcmbAtivo.SelectedItem);

            this.Text = "Gráfico " + ToolStripcmbPeriodoDuracao.SelectedItem + " - " + ToolStripcmbAtivo.SelectedItem;


            //inicializa a variável com o indice final dos dados que devem ser buscados,
            //pois a atualização do array é feita do final para o início.
            int intArrayCandleIndice = intIndiceFinal;

            //busca a data equivalente ao sequencial mínimo
            dtmDataMinimaAreaDados = objCotacao.AtivoSequencialDataBuscar(strCodigoAtivo, lngAreaDadosSequencialInicial, strTabelaCotacao);

            //busca a data equivalente ao sequencial máximo
            dtmDataMaximaAreaDados = objCotacao.AtivoSequencialDataBuscar(strCodigoAtivo, lngAreaDadosSequencialFinal, strTabelaCotacao);

            //conterá a data mínima e data máxima dos dados que serão buscados
            var dtmDataMinimaBusca = default(DateTime);
            var dtmDataMaximaBusca = default(DateTime);

            if (blnCotacaoBuscar)
            {
                dtmDataMinimaBusca = lngSequencialBuscaInicial == lngAreaDadosSequencialInicial
                    ? dtmDataMinimaAreaDados
                    : objCotacao.AtivoSequencialDataBuscar(strCodigoAtivo, lngSequencialBuscaInicial, strTabelaCotacao);

                dtmDataMaximaBusca = lngSequencialBuscaFinal == lngAreaDadosSequencialFinal
                    ? dtmDataMaximaAreaDados
                    : objCotacao.AtivoSequencialDataBuscar(strCodigoAtivo, lngSequencialBuscaFinal, strTabelaCotacao);
            }

            //verifica se existem cotações com valor mínimo zerado. Se existir não permite gerar o gráfico para evitar erros.
            objRS.ExecuteQuery("SELECT COUNT(1) AS Contador " + Environment.NewLine + " FROM " + strTabelaCotacao +
                               Environment.NewLine + " WHERE Codigo = " +
                               FuncoesBd.CampoStringFormatar(strCodigoAtivo) +
                               Environment.NewLine + " AND Data BETWEEN " +
                               FuncoesBd.CampoDateFormatar(dtmDataMinimaAreaDados) + " AND " +
                               FuncoesBd.CampoDateFormatar(dtmDataMaximaAreaDados) + " AND ValorMinimo = 0 ");

            int quantidadeDeCotacaoComValorZerado = Convert.ToInt32(objRS.Field("Contador"));

            objRS.Fechar();

            if (quantidadeDeCotacaoComValorZerado > 0)
            {

                if (quantidadeDeCotacaoComValorZerado == 1)
                {
                    MessageBox.Show(
                        "Existe 1 cotação com Valor Mínimo zerado. " + Environment.NewLine + "Verifique a cotação.",
                        this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
                else
                {
                    MessageBox.Show(
                        "Existem " + quantidadeDeCotacaoComValorZerado + " cotações com Valor Mínimo zerado. " +
                        Environment.NewLine + "Verifique as cotações.", this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }

                //marca que está encerrando a função.
                blnAtualizandoDados = false;

                return false;

            }

            cRSList objRSList;

            if (blnCotacaoBuscar || blnVolumeBuscar)
            {

                try
                {
                    objRSList = objCotacao.ConsultaExecutar(strCodigoAtivo, dtmDataMinimaBusca, dtmDataMaximaBusca,
                        strPeriodoDuracao, "COTACAO", Constantes.DataInvalida, blnCotacaoBuscar, blnVolumeBuscar);


                    while (!objRSList.EOF)
                    {

                        if (blnCotacaoBuscar)
                        {
                            //LEITURA DOS VALORES DO RECORDSET
                            //If objRSList.Field("ValorMinimo") <> 0 Then

                            //valores em R$
                            decimal decValorMaximo = Convert.ToDecimal(objRSList.Field("ValorMaximo"));
                            decimal decValorMinimo = Convert.ToDecimal(objRSList.Field("ValorMinimo"));
                            decimal decValorAbertura = Convert.ToDecimal(objRSList.Field("ValorAbertura"));
                            decimal decValorFechamento = Convert.ToDecimal(objRSList.Field("ValorFechamento"));

                            //instância o candle com as propriedades em escala decimal, antes de passar para a escala logarítmica,
                            //caso o gráfico seja impresso em escala logarítmica.

                            Candle objCandle = new Candle(Convert.ToDateTime(objRSList.Field("Data")),
                                decValorAbertura, decValorFechamento, decValorMaximo, decValorMinimo,
                                Convert.ToDecimal(objRSList.Field("Oscilacao")), blnMMExpDesenhar);


                            if (strPeriodoDuracao == "SEMANAL")
                            {
                                objCandle.DataFinal = Convert.ToDateTime(objRSList.Field("DataFinal"));

                            }

                            arrCandle.SetValue(objCandle, intArrayCandleIndice);


                            if (blnVolumeBuscar)
                            {
                                //objCandle.Volume = dblVolume
                                arrCandle[intArrayCandleIndice].Volume =
                                    Convert.ToDouble(objRSList.Field("Titulos_Total"));

                            }

                        }

                        intArrayCandleIndice = intArrayCandleIndice - 1;

                        objRSList.MoveNext();

                    }


                }
                catch (Exception ex)
                {
                    //mostra mensagem de erro.
                    frmInformacao objfrmInformacao =
                        new frmInformacao("Erro: " + ex.Message + Environment.NewLine + " - Detalhes: " +
                                          ex.StackTrace);

                    objfrmInformacao.ShowDialog();

                    //marca que está encerrando a função.
                    blnAtualizandoDados = false;

                    //retorna com erro
                    return false;

                }


            }

            //*********************************INICIO DO TRATAMENTO PARA O IFR*********************************

            if (blnIfrBuscar)
            {
                //****************VER SE ESTÁ FUNCIONANDO QUANDO O PERÍODO DO IFR FOR TROCADO.

                string strQuery = " SELECT Valor " + Environment.NewLine + " FROM " + strTabelaIFR +
                                  Environment.NewLine + " WHERE Codigo = " +
                                  FuncoesBd.CampoStringFormatar(strCodigoAtivo) + Environment.NewLine +
                                  " AND NumPeriodos = " + intIFRNumPeriodos + Environment.NewLine + " AND Data >= " +
                                  FuncoesBd.CampoDateFormatar(dtmDataMinimaBusca) + Environment.NewLine +
                                  " AND Data <= " + FuncoesBd.CampoDateFormatar(dtmDataMaximaBusca) +
                                  Environment.NewLine + " ORDER BY Data DESC ";

                objRS.ExecuteQuery(strQuery);

                intArrayCandleIndice = intIndiceFinal;

                while (!objRS.EOF)
                {
                    try
                    {
                        arrCandle[intArrayCandleIndice].IFR14 = Convert.ToDouble(objRS.Field("Valor"));
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Erro ao atribuir o IFR no índice: " + intArrayCandleIndice +
                            " - Tamanho total do array: " + arrCandle.Length, this.Text, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    intArrayCandleIndice = intArrayCandleIndice - 1;

                    objRS.MoveNext();

                }

                objRS.Fechar();

                //INSERIDO POR MAURO, 21/12/2009
                //INICIALIZA DEMAIS POSIÇOES DO ARRAY DE CANDLE ATÉ O INICIO DO ARRAY.
                //ESTE TRATAMENTO SERVE PARA O CASO DE PRIMEIRO TER GERADO UTILIZADO UM IFR MENOR
                //(2 PERÍODOS POR EXEMPLO) E DEPOIS UM IFR MAIOR (14 PERÍDOS, POR EXEMPLO).
                //SE ISTO NÃO FOR FEITO AS PRIMEIRAS POSIÇÕES IRÃO FICAR COM OS VALORES DO IFR 
                //DE 2 PERÍODOS.
                //OBS: ISTO DEVE SER FEITO SOMENTE QUANDO ESTAMOS BUSCANDO A PARTE MAIS ESQUERDA DA ÁREA DE DADOS
                //PORQUE SÃO JUSTAMENTE AS COTAÇÕES MAIS ANTIGAS QUE PODEM NÃO TER DADOS DO IFR.
                if (lngSequencialBuscaInicial == lngAreaDadosSequencialInicial)
                {
                    for (intI = 0; intI <= intArrayCandleIndice; intI++)
                    {
                        //PERCORRE DA POSIÇÃO INICIAL DO ARRAY ATÉ A POSIÇÃO ANTERIOR 
                        //À QUE FOI ATRIBUIDA A ULTIMA INFORMAÇÃO DE IFR.

                        //o valor -1 é o valor para não ser impresso, que indica que o IFR não foi calculado
                        arrCandle[intI].IFR14 = -1;

                    }

                }

            }
            //*********************************FIM DO TRATAMENTO PARA O IFR*********************************


            if (blnVolumeBuscar)
            {
                objRSList = objCotacao.ConsultaExecutar(strCodigoAtivo, dtmDataMinimaBusca, dtmDataMaximaBusca,
                    strPeriodoDuracao, "MEDIA", Constantes.DataInvalida, false, false, String.Empty, 21,
                    "VOLUME");

                //intArrayCandleIndice = arrCandle.Length - 1
                intArrayCandleIndice = intIndiceFinal;


                while (!objRSList.EOF)
                {
                    arrCandle[intArrayCandleIndice].VolumeMedio = Convert.ToDouble(objRSList.Field("Valor"));

                    intArrayCandleIndice = intArrayCandleIndice - 1;

                    objRSList.MoveNext();

                }
            }


            //***********************************FIM DO TRATAMENTO PARA O VOLUME MÉDIO**********************************


            //*********************************INICIO DO TRATAMENTO PARA AS MÉDIAS*********************************

            if (blnMMExpDesenhar)
            {
                //1) remover as médias que não precisam mais ser calculadas.
                //Isto deve ser feito somente quando não for para buscar as cotações,
                //pois quando as cotações forem buscadas novos candles serão gerados.
                //Para isso tem que pegar as médias do último candle e verificar 
                //quais delas não estão mais entre as médias escolhidas pelo usuário

                cEstrutura.structMediaValor objstructMediaValor = default(cEstrutura.structMediaValor);

                //busca a collection de média do último candle preenchido.
                Dictionary<string, cEstrutura.structMediaValor> colMediaValorAux =
                    UltimoCandlePreenchidoCollectionMediaBuscar(blnCotacaoBuscar, intIndiceInicial, intIndiceFinal);

                //só precisa limpar a collection de médias quando não estiver vazia

                if ((colMediaValorAux != null))
                {
                    //REMOVE AS MÉDIA QUE NÃO SERÃO MAIS UTILIZADAS MESMO QUE TENHA QUE BUSCAR COTAÇÕES,
                    //PORQUE A BASE DE DADOS NÃO É MAIS CARREGADA TOTALMENTE PARA A MEMÓRIA. ENTÃO ALGUMAS
                    //VEZES É BUSCADO APENAS UMA PARTE DA BASE E A OUTRA PERMANECE. NESTE CASO TEM QUE EXCLUIR
                    //AS MÉDIAS QUE NÃO SERÃO MAIS UTILIZADAS DA BASE QUE IRÁ PERMANECER.

                    //If Not blnCotacaoBuscar Then

                    //AFIRMAÇÃO A SEGUIR NÃO É MAIS VÁLIDA: só precisa limpar a collection de médias se não for 
                    //para buscar as cotações. Se buscar todas as cotações novos candles serão criados.

                    foreach (KeyValuePair<string, cEstrutura.structMediaValor> mediaValor in colMediaValorAux)
                    {
                        objstructMediaValor = mediaValor.Value;
                        //para cada uma das médias encontradas, percorre a estrutura de médias escolhidas pelo usuário
                        //para ver se a média ainda está selecionada

                        //inicializa marcando que tem que remover. Se encontrar a média na estrutura, desmarca
                        bool blnMediaRemover = true;

                        //For Each objstructMediaEscolha In colStructIndicadorMMExp

                        foreach (MediaDTO objMediaDTO in _mediasSelecionadas)
                        {

                            if (objstructMediaValor.intPeriodo == objMediaDTO.NumPeriodos &&
                                objstructMediaValor.strTipo == objMediaDTO.Tipo)
                            {
                                blnMediaRemover = false;

                                break; // TODO: might not be correct. Was : Exit For

                            }

                        }


                        if (blnMediaRemover)
                        {
                            //se for para remover monta a chave da collection e faz um for removendo as médias
                            string strCollectionKey = objstructMediaValor.intPeriodo + objstructMediaValor.strTipo;


                            for (intI = 0; intI <= arrCandle.Length - 1; intI++)
                            {
                                arrCandle[intI].MediaRemover(strCollectionKey);

                            }

                        }

                    }

                    //End If 'if not blnCotacoesBuscar

                }
                //if not colMediaValorAux Is Nothing Then...

                //2) Buscar as médias necessárias

                //collection de objetos da estrutura structIndicadorEscolha que conterá
                //todas as médias que precisam ser calculadas.
                List<MediaDTO> lstMediasSelecionadasAux = new List<MediaDTO>();

                if (blnCotacaoBuscar)
                {
                    //se tiver que buscar as cotações tem que buscar todas as médias
                    lstMediasSelecionadasAux = _mediasSelecionadas;
                }
                else
                {
                    //obtém collection de médias do candle de data mais recente.
                    colMediaValorAux = arrCandle[arrCandle.Length - 1].GetMedia();

                    if (colMediaValorAux == null)
                    {
                        //se os candles ainda não tem nenhuma média calculada, então tem que buscar todas
                        //colMediaEscolhaAux = colStructIndicadorMMExp
                        lstMediasSelecionadasAux = _mediasSelecionadas;

                    }
                    else
                    {

                        //percorre todas as médias escolhidas pelos usuários e verifica quais já estão calculadas no último candle
                        //As médias que já estiverem calculadas não precisa buscar novamente.

                        foreach (MediaDTO mediaDto in _mediasSelecionadas)
                        {
                            //If Not UltimoCandleMediaExistir(objstructMediaEscolha, blnCotacaoBuscar, intIndiceInicial, intIndiceFinal) Then

                            if (!UltimoCandleMediaExistir(mediaDto, blnCotacaoBuscar, intIndiceInicial,
                                intIndiceFinal))
                            {
                                //se não encontrou a média tem que calculá-la, então adiciona na collection
                                //colMediaEscolhaAux.Add(objstructMediaEscolha)
                                lstMediasSelecionadasAux.Add(mediaDto);

                            }

                        }

                    }
                    //if colMediaValor is Nothing Then...

                }
                //if blnCotacoesBuscar then...

                //chamar função que verifica se a média já foi calculada. 
                //Se ainda não foi tem que calcular. O tratamento para saber 
                //se o cálculo é necessário é feito internamente.

                //objCotacao.MediaAtualizar(strCodigoAtivo, colMediaEscolhaAux, strPeriodoDuracao)
                objCotacao.MediaAtualizar(strCodigoAtivo, lstMediasSelecionadasAux, strPeriodoDuracao);

                //Percorre a collection de médias que devem ser calculadas 
                //For Each objstructMediaEscolha In colMediaEscolhaAux

                foreach (MediaDTO objMediaDTO in lstMediasSelecionadasAux)
                {
                    //verifica se a média já existe no último candle

                    if (UltimoCandleMediaExistir(objMediaDTO, blnCotacaoBuscar, intIndiceInicial, intIndiceFinal))
                    {
                        //Se o último candle já contém a média, tem que calcular apenas para o intervalo
                        //que foram buscadas as cotações nesta execução.

                        //gera a tabela considerando os splits
                        objRSList = objCotacao.ConsultaExecutar(strCodigoAtivo, dtmDataMinimaBusca, dtmDataMaximaBusca,
                            strPeriodoDuracao, "MEDIA", Constantes.DataInvalida, false, false, objMediaDTO.Tipo,
                            objMediaDTO.NumPeriodos,
                            "VALOR");

                        intArrayCandleIndice = intIndiceFinal;


                    }
                    else
                    {
                        //Se o último candle não contém a média, tem que calcular a média para toda a área de dados.

                        //gera a tabela considerando os splits
                        objRSList = objCotacao.ConsultaExecutar(strCodigoAtivo, dtmDataMinimaAreaDados,
                            dtmDataMaximaAreaDados, strPeriodoDuracao
                            , "MEDIA", Constantes.DataInvalida, false, false, objMediaDTO.Tipo, objMediaDTO.NumPeriodos,
                            "VALOR");

                        //neste caso tem que começar desde o final do array a atualizar as médias e tem que colocar
                        //o índice para a última posição do array
                        intArrayCandleIndice = arrCandle.Length - 1;

                    }


                    while (!objRSList.EOF)
                    {
                        //preenche estrutura com os dados da média
                        objstructMediaValor.intPeriodo = objMediaDTO.NumPeriodos;
                        objstructMediaValor.strTipo = objMediaDTO.Tipo;
                        objstructMediaValor.dblValor = Convert.ToDouble(objRSList.Field("Valor"));

                        //adiciona a média no array de candle
                        arrCandle[intArrayCandleIndice].MediaAtribuir(objstructMediaValor);

                        //decrementa o inndice do array
                        intArrayCandleIndice = intArrayCandleIndice - 1;

                        objRSList.MoveNext();

                    }

                    //fecha o record set para economizar memória.
                }


            }
            else
            {
                //se não é para desenhar médias

                if (!blnCotacaoBuscar)
                {
                    //se não vai buscar as cotações e não tem que desenhar média
                    //então tem que remover todas as médias dos candles.


                    for (intI = 0; intI <= arrCandle.Length - 1; intI++)
                    {
                        arrCandle[intI].MediaRemoverTodas();

                    }

                }

            }
            //fim do IF BLNMEDIADESENHAR

            //***********************************FIM DO TRATAMENTO PARA AS MÉDIAS**********************************

            //marca que está encerrando a função.
            blnAtualizandoDados = false;

            //retorna sem erro
            return true;

        }


        /// <summary>
        /// Calcula os pontos principais de valores para serem inseridos na área do gráfico e orientar o gráfico
        /// </summary>
        /// <param name="pdecValorMinimo">Menor valor que será impresso no gráfico</param>
        /// <param name="pdecValorMaximo">Maior valor que será impresso no gráfico</param>
        /// <param name="pintNumPontos"></param>
        /// <param name="pdecValorInicialRet">Retorna o maior valor em que será impresso uma reta horizontal de orientação</param>
        /// <param name="pdecIntervaloRet">Retorna o intervalo a qual deve ser impresso novas retas</param>
        /// <remarks></remarks>
        private void PontosPrincipaisCalcular(decimal pdecValorMinimo, decimal pdecValorMaximo, int pintNumPontos,
            out decimal pdecValorInicialRet, out decimal pdecIntervaloRet)
        {
            //divide a diferença entre o maior e o menor valores pelo número de retas que será impressa, 
            //subtraindo de 1, pois tem que desconsiderar umas das retas para fechar o cálculo
            pdecIntervaloRet = (pdecValorMaximo - pdecValorMinimo) / (pintNumPontos - 1);


            if (pdecIntervaloRet >= 1)
            {
                //se o intervalo está na casa de unidades inteiras, arredonda sem casas decimais para ficar um número inteiro
                pdecIntervaloRet = Math.Round(pdecIntervaloRet, 0);


            }
            else if (pdecIntervaloRet >= 0.1M)
            {
                //se está na intervalo está entre 0,10 e 0,99 arredonda para 1 casa para ficar um intervalo nas dezenas de centavos
                pdecIntervaloRet = Math.Round(pdecIntervaloRet, 1);


            }
            else
            {
                //se o intervalo está entre 0,09 e 0,01 arredonda para duas casas para ficar um intervalo nas dezenas de centavos
                pdecIntervaloRet = Math.Round(pdecIntervaloRet, 2);

            }

            if (pdecIntervaloRet > 0)
            {
                //o primeiro valor que deve ser impresso deve ser maior ou igual ao valor mínimo

                //divide o valor mínimo pelo intervalo utilizando apenas a parte inteira
                var intRazaoInteira = (int) Math.Round(pdecValorMinimo / pdecIntervaloRet, 0);

                //agora multiplica a razão inteira pelo intervalo e verifica se retorna um número maior ou igual ao valor mínimo

                if (intRazaoInteira * pdecIntervaloRet >= pdecValorMinimo)
                {
                    //se é maior ou igual então este é o valor inicial
                    pdecValorInicialRet = intRazaoInteira * pdecIntervaloRet;

                }
                else
                {
                    //se não é maior ou igual tem que pegar a próxima razão inteira e multiplicar pelo 
                    //intervalo para obter o valor inicial
                    pdecValorInicialRet = (intRazaoInteira + 1) * pdecIntervaloRet;

                }


                if (pdecValorInicialRet <= 0)
                {
                    MessageBox.Show(
                        "intRazaoInteira: " + intRazaoInteira + " pdecIntervaloRet" + pdecIntervaloRet +
                        " pdecValorMaximo: " + pdecValorMaximo + " pintNumPontos: " + pintNumPontos, this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }


            }
            else
            {
                //tratamento para o caso em que os valores máximo e mínimo são iguais, geralmente
                //quando há apenas um negócio no dia.
                pdecValorInicialRet = pdecValorMinimo;


                if (pdecValorInicialRet <= 0)
                {
                    MessageBox.Show("pdecValorMinimo: " + pdecValorMinimo, this.Text, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                }

                //valor para que não fique preso em loop.
                pdecIntervaloRet = 0.1M;

            }

        }



        private void PontosPrincipaisCalcular(double pdblValorMinimo, double pdblValorMaximo, int pintNumPontos,
            ref double pdblValorInicialRet, ref double pdblIntervaloRet)
        {
            if (pdblValorMinimo != pdblValorMaximo)
            {
                //divide a diferença entre o maior e o menor valores pelo número de retas que será impressa, 
                //subtraindo de 1, pois tem que desconsiderar umas das retas para fechar o cálculo
                pdblIntervaloRet = (pdblValorMaximo - pdblValorMinimo) / (pintNumPontos - 1);


                if (pdblIntervaloRet >= 1)
                {
                    //se o intervalo está na casa de unidades inteiras, arredonda sem casas decimais para ficar um número inteiro
                    pdblIntervaloRet = Math.Round(pdblIntervaloRet, 0);


                }
                else if (pdblIntervaloRet >= 0.1)
                {
                    //se está na intervalo está entre 0,10 e 0,99 arredonda para 1 casa para ficar um intervalo nas dezenas de centavos
                    pdblIntervaloRet = Math.Round(pdblIntervaloRet, 1);


                }
                else
                {
                    //se o intervalo está entre 0,09 e 0,01 arredonda para duas casas para ficar um intervalo nas dezenas de centavos
                    pdblIntervaloRet = Math.Round(pdblIntervaloRet, 2);

                }

                //o primeiro valor que deve ser impresso deve ser maior ou igual ao valor mínimo

                //divide o valor mínimo pelo intervalo utilizando apenas a parte inteira
                int intRazaoInteira = (int) Math.Round(pdblValorMinimo / pdblIntervaloRet, 0);

                //agora multiplica a razão inteira pelo intervalo e verifica se retorna um número maior ou igual ao valor mínimo

                if (intRazaoInteira * pdblIntervaloRet >= pdblValorMinimo)
                {
                    //se é maior ou igual então este é o valor inicial
                    pdblValorInicialRet = intRazaoInteira * pdblIntervaloRet;

                }
                else
                {
                    //se não é maior ou igual tem que pegar a próxima razão inteira e multiplicar pelo 
                    //intervalo para obter o valor inicial
                    pdblValorInicialRet = (intRazaoInteira + 1) * pdblIntervaloRet;

                }


            }
            else
            {
                //quando o volume mínimo e o máximo são iguais, provavelmente só existe 1 período de movimento.
                //Neste caso o valor inicial é o único volume apresentado e o intervalo é 1, para não ficar
                //preso no loop no momento em que for imprimir os labels com volume
                pdblValorInicialRet = pdblValorMaximo;

                pdblIntervaloRet = 1;

            }

        }

        /// <summary>
        /// Remove os labels existentes de data e valor existentes no form e depois limpa a collection dos labels
        /// </summary>
        /// <remarks></remarks>

        private void ControlesRemover()
        {
            //percorre collection de índices

            foreach (Control control in _labelsVerticais)
            {
                //remove o componente do índice específico.
                this.Controls.Remove(control);
            }

            _labelsVerticais.Clear();

        }


        private void LabelVerticalAdicionar(string pstrText, int pintAreaRight, int pintPosicaoY)
        {
            Label objLabel = new Label();

            objLabel.AutoSize = true;

            objLabel.Text = pstrText;

            pintPosicaoY = pintPosicaoY - objLabel.Height / 3;

            objLabel.Location = new Point(pintAreaRight + 3, pintPosicaoY);

            this.Controls.Add(objLabel);

            //adiciona na collection de componentes
            _labelsVerticais.Add(objLabel);

        }

        private void LabelHorizontalAdicionar(string pstrText, int pintAreaBottom, int pintAreaLeft, int pintPosicaoX,
            Color pobjColor)
        {
            Label objLabel = new Label();

            objLabel.AutoSize = true;

            objLabel.ForeColor = pobjColor;

            objLabel.Text = pstrText;

            pintPosicaoX = pintPosicaoX - objLabel.Width / 3;


            if (pintPosicaoX < pintAreaLeft)
            {
                pintPosicaoX = pintAreaLeft;

            }

            objLabel.Location = new Point(pintPosicaoX, pintAreaBottom + 4);

            this.Controls.Add(objLabel);

            //MAURO, 28/02/2010
            //REMOVIDO O INDICE PELO TEXTO DO LABEL PORQUE PODE ACONTECER POR EXEMPLO DE O PREÇO E O IFR
            //POSSUIREM O MESMO VALOR PARA UM DETERMINADO GRÁFICO E GERAR ERRO DE CHAVE DUPLICADA.
            //POR EXEMPLO, MOSTRAR O IFR DE 30 E NA ESCALA DE PREÇOS TAMBÉM POSSUIR O VALOR 30.

            //adiciona na collection de componentes
            //colLabelVertical.Add(objLabel, objLabel.Text)
            _labelsVerticais.Add(objLabel);

        }


        private void frmGrafico_Paint(object sender, PaintEventArgs e)
        {
            if (Convert.ToString(ToolStripcmbAtivo.SelectedItem).Trim() == String.Empty) return;
            GraficoGerar(e);
        }

        private void GraficoGerar(PaintEventArgs e)
        {

            if (blnGraficoGerando)
            {
                return;

            }

            blnGraficoGerando = true;

            bool blnOK = true;


            if (blnDadosAtualizar)
            {
                blnOK = DadosAtualizar();
                IniciarFerramentaDeDesenho();
            }

            if (blnPontosAtualizar && blnOK)
            {
                PontosAtualizar();

            }

            if (blnOK)
            {
                //Debug.Print("Chamando GraficoDesenhar")
                GraficoDesenhar(e);
            }

            if (blnOK)
            {
                //Só
                blnGraficoGerando = false;
            }



        }

        private void IniciarFerramentaDeDesenho()
        {
            if (ToolStripbtnRetaHorizontal.Checked)
            {
                _ferramentaSelecionada = new FerramentaLinhaHorizontal(_areaDeDesenho);
            }
            if (ToolStripbtnLinhaTendencia.Checked)
            {
                _ferramentaSelecionada = new FerramentaLinhaTendencia(_areaDeDesenho);
            }
            if (ToolStripbtnCanal.Checked)
            {
                _ferramentaSelecionada = new FerramentaCanal(_areaDeDesenho);
            }
            if (ToolStripbtnFibonacciRetracement.Checked)
            {
                _ferramentaSelecionada = new FerramentaFibonacci(_areaDeDesenho);
            }

        }


        private void DesenharLinhaDoIfr(PaintEventArgs e, int valorDoIfr)
        {
            //IFR = 10
            var intReferenciaY = intIFRAreaTop + intIndicadoresMargem +
                                 Convert.ToInt32((100 - valorDoIfr) * dblPixelPorIFR);

            e.Graphics.DrawLine(Pens.LightSeaGreen, intAreaLeft, intReferenciaY, intAreaRight, intReferenciaY);

            if (blnPontosAtualizar)
            {
                LabelVerticalAdicionar(Convert.ToString(valorDoIfr), intAreaRight, intReferenciaY);
            }

        }

        private void GraficoDesenhar(PaintEventArgs e)
        {

            if (blnGraficoDesenhando)
            {
                return;

            }

            blnGraficoDesenhando = true;

            //utilizado em loops
            int intI = 0;

            //desenha o retângulo que é a área do gráfico
            objGraficoArea = new Rectangle(intAreaLeft, intAreaTop, intAreaWidth, intAreaTotalHeight);
            e.Graphics.FillRectangle(Brushes.LightYellow, objGraficoArea);
            e.Graphics.DrawRectangle(Pens.Black, objGraficoArea);

            int intReferenciaY = 0;
            //coordenada Y do ponto de referência

            //If blnDadosAtualizar Then

            if (blnPontosAtualizar)
            {
                //remove os labels inseridos anteriormente
                ControlesRemover();

                //limpa as informações da barra de status
                StatusStrip.Items.Clear();

            }

            //inicializa indicando que não tem nenhum candle de volume selecionado.
            intVolumePosicaoSelecionada = -1;

            //----------------------------------------------------------------------
            decimal decValorInicialReferenciaAux = decValorInicialReferencia;

            Pen objPen = new Pen(Color.LightSeaGreen);

            objPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            //faz um loop inserindo as retas horizontais e os labels para as cotações

            while (decValorInicialReferenciaAux <= decValorMaximoPeriodoReferencia)
            {
                double dblValorReferencia = ToolStripbtnEscalaLogaritmica.Checked
                    ? Math.Log(Convert.ToDouble(decValorInicialReferenciaAux))
                    : Convert.ToDouble(decValorInicialReferenciaAux);

                try
                {
                    //calcula a coordena y da reta a ser impressa
                    intReferenciaY = intAreaTop + intMargemSuperior +
                                     Convert.ToInt32(((double) decValorMaximoPeriodo - dblValorReferencia) *
                                                     dblPixelPorReal);


                }
                catch
                {
                    MessageBox.Show(
                        "Erro ************** decValorMaximoPeriodo: " + decValorMaximoPeriodo +
                        " dblValorReferencia: " + dblValorReferencia + " decValorInicialReferenciaAux: " +
                        decValorInicialReferenciaAux + " dblpixelporreal: " + dblPixelPorReal, this.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

                //imprime a reta na horizontal
                e.Graphics.DrawLine(objPen, intAreaLeft, intReferenciaY, intAreaRight, intReferenciaY);


                //If blnDadosAtualizar Then

                if (blnPontosAtualizar)
                {
                    //imprime o label informando o valor da reta
                    //LabelVerticalAdicionar(Strings.FormatNumber(Convert.ToString(decValorInicialReferenciaAux), 2), intAreaRight, intReferenciaY);
                    LabelVerticalAdicionar(
                        decValorInicialReferenciaAux.ToString(Constantes.FormatoComDuasCasasDecimais), intAreaRight,
                        intReferenciaY);

                }

                //incrementa variável do loop
                decValorInicialReferenciaAux += decIntervaloReferencia;

            }

            //----------------------------------------------------------------------


            //----------------------------------------------------------------------
            double dblVolumeInicialReferenciaAux = dblVolumeInicialReferencia;

            double dblVolumeReferenciaAnterior = 0;


            if (blnVolumeDesenhar)
            {
                //faz um loop inserindo as retas horizontais e os labels para as cotações

                while (dblVolumeInicialReferenciaAux <= dblVolumeMaximoPeriodoReferencia)
                {
                    double dblVolumeReferencia = 0;
                    if (dblVolumeInicialReferenciaAux >= 1000000)
                    {
                        dblVolumeReferencia = Math.Round(dblVolumeInicialReferenciaAux / 1000000, 0) * 1000000;


                        if (dblVolumeReferencia > dblVolumeMaximoPeriodoReferencia)
                        {
                            dblVolumeReferencia = Math.Truncate(dblVolumeInicialReferenciaAux / 1000000) * 1000000;

                            if (dblVolumeReferencia == dblVolumeReferenciaAnterior)
                            {
                                dblVolumeReferencia = dblVolumeInicialReferenciaAux;
                            }

                        }


                    }
                    else if (dblVolumeInicialReferenciaAux >= 1000)
                    {
                        dblVolumeReferencia = Math.Round(dblVolumeInicialReferenciaAux / 1000, 0) * 1000;


                        if (dblVolumeReferencia > dblVolumeMaximoPeriodoReferencia)
                        {
                            dblVolumeReferencia = Math.Truncate(dblVolumeInicialReferenciaAux / 1000) * 1000;

                            if (dblVolumeReferencia == dblVolumeReferenciaAnterior)
                            {
                                dblVolumeReferencia = dblVolumeInicialReferenciaAux;
                            }

                        }

                    }
                    else
                    {
                        dblVolumeReferencia = Math.Truncate(dblVolumeInicialReferenciaAux);
                    }

                    //verifica se o valor não extrapolou o valor máximo

                    //calcula a coordena y da reta a ser impressa
                    intReferenciaY = intVolumeAreaTop + intIndicadoresMargem +
                                     Convert.ToInt32(
                                         (dblVolumeMaximoPeriodo - dblVolumeReferencia) * dblPixelPorNegocio);

                    //imprime a reta na horizontal
                    e.Graphics.DrawLine(Pens.DarkGreen, intAreaLeft, intReferenciaY, intAreaRight, intReferenciaY);

                    //If blnDadosAtualizar Then

                    if (blnPontosAtualizar)
                    {
                        //imprime o label informando o valor da reta
                        LabelVerticalAdicionar(ValorAbreviar(dblVolumeReferencia), intAreaRight, intReferenciaY);

                    }

                    dblVolumeReferenciaAnterior = dblVolumeReferencia;

                    //incrementa variável do loop
                    dblVolumeInicialReferenciaAux += dblVolumeIntervaloReferencia;

                }

                //desenha uma linha no top, para separar o volume das cotações
                e.Graphics.DrawLine(Pens.Black, intAreaLeft, intVolumeAreaTop, intAreaRight, intVolumeAreaTop);

            }
            //----------------------------------------------------------------------

            //*************DESENHA OS SPLITS
            structSplit objSplit = default(structSplit);


            if ((colSplit != null))
            {

                foreach (structSplit split in colSplit)
                {
                    objSplit = split;
                    e.Graphics.DrawLine(Pens.LightSalmon, objSplit.intPosicaoX, intAreaTop, objSplit.intPosicaoX,
                        intAreaBottom);

                    //If blnDadosAtualizar Then

                    if (blnPontosAtualizar)
                    {
                        //imprime o label referente à linha
                        LabelHorizontalAdicionar("Split: " + objSplit.strRazao, intAreaBottom + 13, intAreaLeft,
                            objSplit.intPosicaoX, Color.Red);

                    }
                    //if blnDadosAtualizar then

                }

            }

            //utilizado para controle de labels de referência

            //indica o número de mêses que deve ter o intervalo de cada label.
            //Inicializa com 1 porque tanto no diário quanto no semanal o primeiro label 
            //é inserido na primeira troca de mês.
            int intNumDiasIntervaloLabels = 0;

            switch (intZoom)
            {

                case 1:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 105 : 315;

                    break;
                case 2:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 70 : 210;

                    break;
                case 3:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 42 : 126;

                    break;
                case 4:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 30 : 90;

                    break;
                case 5:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 25 : 75;

                    break;
                case 6:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 20 : 60;

                    break;
                case 7:

                    intNumDiasIntervaloLabels = strPeriodoDuracao == "DIARIO" ? 15 : 45;

                    break;
            }

            //pega o mês do primeiro candle para quando trocar de mês imprimir a última data do mês.
            //e adiciona um mês.

            //pega a primeira data do próximo mês
            DateTime dtmDataAtual = arrCandle[intArrayCandlePosicaoInicial].Data.AddMonths(1).FirstDayOfMonth();

            int intArrayVolume = 0;


            for (intI = intArrayCandlePosicaoInicial; intI <= intArrayCandlePosicaoFinal; intI++)
            {
                //busca a data do candle no array
                DateTime dtmData = arrCandle[intI].Data;

                //busca o ponto x da cauda do candle no array
                int intCaudaX = arrCandle[intI].LinhaCaudaX;

                //verifica se tem que imprimir a linha vertical de referência de mês
                //If DateDiff(DateInterval.Month, dtmDataAtual, dtmData) = intNumMesesIntervaloLabels Then


                if (dtmData >= dtmDataAtual)
                {
                    //imprime a linha vertical
                    e.Graphics.DrawLine(objPen, intCaudaX, intAreaTop, intCaudaX, intAreaBottom);

                    //If blnDadosAtualizar Then

                    if (blnPontosAtualizar)
                    {
                        //imprime o label referente à linha
                        LabelHorizontalAdicionar(dtmData.ToShortDateString(), intAreaBottom, intAreaLeft, intCaudaX,
                            Color.Black);

                    }

                    //atualiza a data para a próxima iteração
                    dtmDataAtual = dtmDataAtual.AddDays(intNumDiasIntervaloLabels);

                }

                //imprime o candle: corpo e cauda
                arrCandle[intI].Desenhar(e.Graphics);



                if (blnVolumeDesenhar)
                {
                    //preenche os retângulos do volume com a  cor
                    e.Graphics.FillRectangle(Brushes.Orange, arrVolumeRectangle[intArrayVolume]);

                    //desenha o retângulo do volume na tela
                    e.Graphics.DrawRectangle(Pens.Black, arrVolumeRectangle[intArrayVolume]);

                    intArrayVolume = intArrayVolume + 1;

                }

            }


            if (blnMMExpDesenhar)
            {
                Rectangle objRectAreaLegenda = new Rectangle(intAreaRight + 45, intAreaTop, 42, colMedia.Count * 20);

                int intRectItemLegendaTop = intAreaTop + 5;

                e.Graphics.FillRectangle(Brushes.LightYellow, objRectAreaLegenda);

                e.Graphics.DrawRectangle(Pens.Black, objRectAreaLegenda);


                foreach (Indicador objMedia_loopVariable in colMedia)
                {
                    Indicador objMedia = objMedia_loopVariable;
                    //desenha todas as médias móveis.

                    if (objMedia.IndicadorPonto.Length > 1)
                    {
                        //Só desenha se tiver pelo 2 pontos. Caso contrário não é possível desenhar uma reta. 
                        //Não foi encontrada função para desenhar apenas um ponto.
                        e.Graphics.DrawLines(new Pen(objMedia.Cor), objMedia.IndicadorPonto);

                    }

                    //desenha a legenda no canto superior direito da tela - INICIO
                    var objRectItemLegenda = new Rectangle(intAreaRight + 47, intRectItemLegendaTop, 10, 10);

                    e.Graphics.FillRectangle(new SolidBrush(objMedia.Cor), objRectItemLegenda);

                    e.Graphics.DrawRectangle(Pens.Black, objRectItemLegenda);

                    e.Graphics.DrawString(objMedia.NumPeriodos + objMedia.Tipo, SystemFonts.DefaultFont, Brushes.Black,
                        intAreaRight + 59, intRectItemLegendaTop);

                    //incrementa a posição top do retângulo para desenhar o próximo
                    intRectItemLegendaTop = intRectItemLegendaTop + 15;

                    //desenha a legenda no canto superior direito da tela - FIM

                }

            }


            if (blnIFRDesenhar)
            {
                //desenha uma linha no top, para separar o IFR das cotações
                e.Graphics.DrawLine(Pens.Black, intAreaLeft, intIFRAreaTop, intAreaRight, intIFRAreaTop);

                //adiciona as linhas e os labels de referência do IFR

                if (intIFRNumPeriodos == 2)
                {
                    DesenharLinhaDoIfr(e, 10);
                }

                if (intIFRNumPeriodos != 2)
                {
                    DesenharLinhaDoIfr(e, 20);
                }

                //imprime a reta na horizontal
                e.Graphics.DrawLine(Pens.LightSeaGreen, intAreaLeft, intReferenciaY, intAreaRight, intReferenciaY);

                DesenharLinhaDoIfr(e, 30);

                DesenharLinhaDoIfr(e, 50);

                DesenharLinhaDoIfr(e, 70);

                if (intIFRNumPeriodos != 2)
                {
                    DesenharLinhaDoIfr(e, 80);
                }


                if (intIFRNumPeriodos == 2)
                {
                    DesenharLinhaDoIfr(e, 90);
                }
                //desenha os pontos do IFR

                if (arrIFRPonto.Length > 1)
                {
                    e.Graphics.DrawLines(Pens.Blue, arrIFRPonto);


                }
                else if (arrIFRPonto.Length == 1)
                {
                    //quando só tem um ponto.
                    e.Graphics.DrawLine(Pens.Blue, arrIFRPonto[0], arrIFRPonto[0]);

                }


            }

            //**************INICIO DO DESENHO DO VOLUME MÉDIO**************

            if (arrVolumeMedio.Length > 1)
            {
                e.Graphics.DrawLines(Pens.Blue, arrVolumeMedio);


            }
            else if (arrVolumeMedio.Length == 1)
            {
                //quando só tem um ponto.
                e.Graphics.DrawLine(Pens.Blue, arrVolumeMedio[0], arrVolumeMedio[0]);

            }


            //****************FIM DO DESENHO DO VOLUME MÉDIO***************


            //****************INICIO DO TRATAMENTO PARA OS OBJETOS DESENHADOS NA TELA****************
            if (_ferramentaSelecionada != null && _ferramentaSelecionada.DesenhoGerado != null)
            {
                _ferramentaSelecionada.DesenhoGerado.Desenhar(e.Graphics);
            }


            foreach (Desenho objFerramenta in _desenhosCriados)
            {
                objFerramenta.Desenhar(e.Graphics);
            }

            //****************FIM DO TRATAMENTO PARA OS OBJETOS DESENHADOS NA TELA****************

            objPen.Dispose();

            blnDadosAtualizar = false;

            blnPontosAtualizar = false;

            blnGraficoDesenhando = false;

        }

        /// <summary>
        ///Recebe uma coordenada X do mouse e retorna a posição que esta coordenada equivale no array de candles
        /// </summary>
        /// <param name="pobjPonto">ponto para o qual deve ser calculado o indice respectivo no array de candles</param>
        /// <param name="pintIndiceRet">Retorna o indice do array de candles ao qual o ponto recebido por parâmetro se refere</param>
        /// <returns>
        /// TRUE = ponto recebido por parâmetro pertence à area de impressão do gráfico
        /// FALSE = ponto recebido por parâmetro NÃO pertence à area de impressão do gráfico
        /// </returns>
        /// <remarks></remarks>
        private bool MousePositionArrayCandleIndiceCalcular(Point pobjPonto, ref int pintIndiceRet)
        {
            bool functionReturnValue;


            if (AreaDadosPontoPertencer(pobjPonto))
            {
                int intX = pobjPonto.X;

                int intNumCandles;

                //se está na área de impressão do gráfico, verifica se está sobre um candle
                //para fazer isso, primeiro verifica qual posição do array de candles corresponde a posição X do mouse

                //a impressão dos candles começa da direita para esquerda com uma posição intLargura sendo descontada
                //Com isso o corpo do candle é impresso e ainda sobre um espaçamento de 3 pixels para o candle mais 
                //da direita não ficar sobre a borda. No lado esquerdo sobra dois espaçamentos, ou seja, 6 pixels

                //para encontrar a posição no array tem que descontar da posição X do mouse 
                //a parte não impressa do lado esquerda (intAreaLeft) e os 6 pixels, dividindo o resultado 
                //pelo largura dos candles (corpo + espaçamento), que é indicando pela propriedade intLargura
                //Tem que descontar um do resultado porque o array é base 0.

                //divisão inteira

                if (intX != intCandle1X)
                {
                    intNumCandles = (intX - intCandle1X) / intLarguraTotal;


                    if ((intX - intCandle1X) % intLarguraTotal != 0)
                    {
                        //se a divisão não é exata tem que somar 1
                        intNumCandles = intNumCandles + 1;

                    }

                }
                else
                {
                    //se a corrdenada X do ponto que foi clicado com o mouse é igual à posicão X do primeiro candle então
                    //o número do candle é = 1.
                    intNumCandles = 1;
                }


                //decrementa um da posição final porque o array é base 0 
                int intPosicaoArray = intArrayCandlePosicaoInicial + intNumCandles - 1;

                pintIndiceRet = intPosicaoArray;

                functionReturnValue = true;


            }
            else
            {
                functionReturnValue = false;

            }
            return functionReturnValue;

        }


        private void CandleDadosAtualizar(System.Windows.Forms.MouseEventArgs e)
        {
            int intX = e.X;
            int intY = e.Y;

            int intPosicaoArray = 0;


            //verifica se a posição do mouse está na área de impressão do gráfico
            //If AreaDadosPontoPertencer(e.Location) Then


            if (MousePositionArrayCandleIndiceCalcular(e.Location, ref intPosicaoArray))
            {
                //se está na área de impressão do gráfico, verifica se está sobre um candle
                //para fazer isso, primeiro verifica qual posição do array de candles corresponde a posição X do mouse

                //a impressão dos candles começa da direita para esquerda com uma posição intLargura sendo descontada
                //Com isso o corpo do candle é impresso e ainda sobre um espaçamento de 3 pixels para o candle mais 
                //da direita não ficar sobre a borda. No lado esquerdo sobra dois espaçamentos, ou seja, 6 pixels

                //para encontrar a posição no array tem que descontar da posição X do mouse 
                //a parte não impressa do lado esquerda (intAreaLeft) e os 6 pixels, dividindo o resultado 
                //pelo largura dos candles (corpo + espaçamento), que é indicando pela propriedade intLargura
                //Tem que descontar um do resultado porque o array é base 0.

                Candle candle = arrCandle[intPosicaoArray];

                if (candle.RectAreaTotal.Contains(intX, intY))
                {
                    StatusStrip.Items.Clear();

                    StatusStrip.Items.Add("DATA: " + candle.Data.ToShortDateString() + "  ABE: " +
                                          candle.ValorAbertura.ToString(Constantes.FormatoComDuasCasasDecimais) +
                                          "  MIN: " +
                                          candle.ValorMinimo.ToString(Constantes.FormatoComDuasCasasDecimais) +
                                          "  MAX: " + candle.ValorMaximo.ToString(Constantes
                                              .FormatoComDuasCasasDecimais) +
                                          "  FECH: " +
                                          candle.ValorFechamento.ToString(Constantes.FormatoComDuasCasasDecimais) +
                                          "  OSC: " + candle.Oscilacao.ToString(Constantes
                                              .FormatoComDuasCasasDecimais));

                    if (blnMMExpDesenhar)
                    {
                        string strTexto = String.Empty;

                        foreach (MediaDTO objMediaDTO in _mediasSelecionadas)
                        {
                            double dblMedia = candle.MediaBuscar(objMediaDTO.NumPeriodos, objMediaDTO.Tipo);


                            if (dblMedia > 0)
                            {
                                if (strTexto != String.Empty)
                                {
                                    strTexto = strTexto + "  ";
                                }

                                strTexto = strTexto + "[" + objMediaDTO.NumPeriodos + objMediaDTO.Tipo + "] " +
                                           dblMedia.ToString(Constantes.FormatoComDuasCasasDecimais);

                            }
                        }


                        if (strTexto != String.Empty)
                        {
                            StatusStrip.Items.Add(new ToolStripSeparator());

                            strTexto = "Médias:  " + strTexto;

                            StatusStrip.Items.Add(strTexto);

                        }

                    }


                    if (blnIFRDesenhar)
                    {
                        StatusStrip.Items[0].Text = StatusStrip.Items[0].Text + "  IFR " + intIFRNumPeriodos + ": " +
                                                    candle.IFR14.ToString(Constantes.FormatoComDuasCasasDecimais);

                    }


                    if (!blnVolumeDesenhar) return;
                    StatusStrip.Items[0].Text = StatusStrip.Items[0].Text + "  VOL: " +
                                                ValorAbreviar(candle.Volume);

                    //se é para desenhar o volume, então evidencia o candle do volume para 
                    //saber em qual está clicando
                    Graphics objGraphics = this.CreateGraphics();


                    if (intVolumePosicaoSelecionada > -1)
                    {
                        //se já tem uma barra de volume selecionada tem que colocar ela na cor normal
                        objGraphics.FillRectangle(Brushes.Orange, arrVolumeRectangle[intVolumePosicaoSelecionada]);

                        objGraphics.DrawRectangle(Pens.Black, arrVolumeRectangle[intVolumePosicaoSelecionada]);

                    }

                    //marca a nova posição selecionada: subtrai a posição inicial porque o array de volumes
                    //contém apenas os volumes da área de dados que está sendo desenhada. Ao contrário dos
                    //candles que contêm toda a área de dados.
                    intVolumePosicaoSelecionada = intPosicaoArray - intArrayCandlePosicaoInicial;

                    objGraphics.FillRectangle(Brushes.Blue, arrVolumeRectangle[intVolumePosicaoSelecionada]);

                    objGraphics.DrawRectangle(Pens.Black, arrVolumeRectangle[intVolumePosicaoSelecionada]);

                    objGraphics.Dispose();
                }
            }
        }

        private bool TemFerramentaDeDesenhoSelecionada
        {
            get
            {
                return ToolStripbtnRetaHorizontal.Checked || ToolStripbtnLinhaTendencia.Checked ||
                       ToolStripbtnCanal.Checked || ToolStripbtnFibonacciRetracement.Checked;
            }

        }

        private void ToolStripbtnEscalaLogaritmica_Click(System.Object sender, System.EventArgs e)
        {
            //verifica se o botão da escala aritmética está marcado. Se estiver marcado, desmarca
            ToolStripbtnEscalaAritmetica.Checked = false;

            //se o botão de escala logaritmica já está marcada tem que manter marcado, pois o evento click irá desmarcar
            ToolStripbtnEscalaLogaritmica.Checked = true;

        }


        private void ToolStripbtnEscalaAritmetica_Click(System.Object sender, System.EventArgs e)
        {
            //verifica se o botão da escala logarítmica está marcado. Se estiver marcado, desmarca
            ToolStripbtnEscalaLogaritmica.Checked = false;

            //se o botão de escala aritmética já está marcada tem que manter marcado, pois o evento click irá desmarcar
            ToolStripbtnEscalaAritmetica.Checked = true;

        }


        private void ToolStripbtnAtualizar_Click(System.Object sender, System.EventArgs e)
        {
            //se não foi escolhido um valor válido no combo,emite mensagem e sai da função

            if (ToolStripcmbAtivo.SelectedIndex < 0)
            {
                MessageBox.Show("Escolha um ativo válido.", this.Text, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;

            }

            blnGraficoGerando = false;
            blnDadosAtualizar = true;
            blnPontosAtualizar = true;

            this.Refresh();

        }

        /// <summary>
        /// Recebe um valor e abrevia removendo os zeros e colocando letras para abreviar os milhares
        /// </summary>
        /// <param name="pdblValor"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string ValorAbreviar(double pdblValor)
        {
            string functionReturnValue;

            if (pdblValor >= 1000000)
            {
                functionReturnValue = (pdblValor / 1000000).ToString(Constantes.FormatoComDuasCasasDecimais) + " M";
            }
            else if (pdblValor >= 1000)
            {
                functionReturnValue = (pdblValor / 1000).ToString(Constantes.FormatoComDuasCasasDecimais) + " K";
            }
            else
            {
                functionReturnValue = pdblValor.ToString();
            }
            return functionReturnValue;

        }

        private void ParametroConsultar(string pstrParametro, out string pstrValorRet)
        {

            var objDadosDB = new cDadosDB(_conexao, "Configuracao");

            objDadosDB.CampoAdicionar("Parametro", true, pstrParametro);

            objDadosDB.CampoAdicionar("Valor", false, "");

            objDadosDB.DadosBDConsultar();

            pstrValorRet = objDadosDB.CampoConsultar("Valor");


        }

        private void IndicadorCarregar()
        {
            cRS objRS = new cRS(_conexao);

            objRS.ExecuteQuery(" select Tipo, NumPeriodos, Cor " + Environment.NewLine +
                               " from Configuracao_Indicador " + Environment.NewLine + " where Tipo = " +
                               FuncoesBd.CampoStringFormatar("MME") + Environment.NewLine + " or Tipo = " +
                               FuncoesBd.CampoStringFormatar("MMA") + Environment.NewLine +
                               " ORDER BY NumPeriodos, Tipo");

            if (objRS.DadosExistir)
            {
                //Inicializa lista global da tela que contem as médias que devem ser desenhadas no gráfico.
                _mediasSelecionadas = new List<MediaDTO>();
            }

            while (!objRS.EOF)
            {

                _mediasSelecionadas.Add(new MediaDTO(((string) objRS.Field("Tipo") == "MME" ? "E" : "A"),
                    Convert.ToInt32(objRS.Field("NumPeriodos")), "VALOR",
                    Color.FromArgb(Convert.ToInt32(objRS.Field("Cor")))));

                objRS.MoveNext();

            }

            objRS.Fechar();

        }


        private void frmGrafico_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //AJUSTA O PONTEIRO DO MOUSE DE ACORDO COM O QUE O PROGRAMA ESTÁ FAZENDO.

            if (blnAtualizandoDados)
            {
                //se está na operação de atualização o mouse fica no estado de espera
                this.Cursor = Cursors.WaitCursor;


            }
            else if (objGraficoArea.Contains(e.Location))
            {

                if ((ToolStripbtnRetaHorizontal.Checked || ToolStripbtnFibonacciRetracement.Checked ||
                     ToolStripbtnCanal.Checked || ToolStripbtnLinhaTendencia.Checked || ToolStripbtnCanal.Checked))
                {
                    //se o cursor do mouse está na área de desenho do gráfico, fica uma cruz
                    this.Cursor = Cursors.Cross;


                }
                else if (ToolStripbtnZoomAumentar.Checked)
                {
                    this.Cursor = new Cursor(Resources.zoom_in.GetHicon());


                }
                else if (ToolStripbtnZoomDiminuir.Checked)
                {
                    this.Cursor = new Cursor(Resources.zoom_out.GetHicon());


                }
                else
                {
                    this.Cursor = Cursors.Default;

                }


            }
            else
            {
                this.Cursor = Cursors.Default;

            }


            if (!AreaDadosPontoPertencer(e.Location))
            {
                return;

            }


            if (TemFerramentaDeDesenhoSelecionada && _ferramentaSelecionada != null)
            {

                int intIndice = 0;
                MousePositionArrayCandleIndiceCalcular(e.Location, ref intIndice);

                _ferramentaSelecionada.Move(new PontoDoDesenho(e.Location, intIndice));

                var desenhoGerado = _ferramentaSelecionada.DesenhoGerado;

                if (desenhoGerado != null)
                {
                    Refresh();
                }


            }
            else if (blnMouseDown)
            {
                AreaDadosMover(e.Location);

            }

        }

        private void ToolStripbtnFibonacciRetracement_Click(System.Object sender, System.EventArgs e)
        {
            _ferramentaSelecionada = ToolStripbtnFibonacciRetracement.Checked
                ? new FerramentaFibonacci(_areaDeDesenho)
                : null;
            if (ToolStripbtnFibonacciRetracement.Checked)
            {
                ToolStripbtnRetaHorizontal.Checked = false;
                ToolStripbtnCanal.Checked = false;
                ToolStripbtnLinhaTendencia.Checked = false;
                ToolStripbtnZoomDiminuir.Checked = false;
                ToolStripbtnZoomAumentar.Checked = false;

            }

        }


        private void ToolStripbtnRetaHorizontal_Click(System.Object sender, System.EventArgs e)
        {

            _ferramentaSelecionada = ToolStripbtnRetaHorizontal.Checked
                ? new FerramentaLinhaHorizontal(_areaDeDesenho)
                : null;
            if (ToolStripbtnRetaHorizontal.Checked)
            {
                ToolStripbtnFibonacciRetracement.Checked = false;
                ToolStripbtnCanal.Checked = false;
                ToolStripbtnLinhaTendencia.Checked = false;
                ToolStripbtnZoomDiminuir.Checked = false;
                ToolStripbtnZoomAumentar.Checked = false;

            }

        }


        private void ToolStripbtnCanal_Click(System.Object sender, System.EventArgs e)
        {
            _ferramentaSelecionada = ToolStripbtnCanal.Checked ? new FerramentaCanal(_areaDeDesenho) : null;
            if (ToolStripbtnCanal.Checked)
            {
                ToolStripbtnFibonacciRetracement.Checked = false;
                ToolStripbtnRetaHorizontal.Checked = false;
                ToolStripbtnLinhaTendencia.Checked = false;
                ToolStripbtnZoomDiminuir.Checked = false;
                ToolStripbtnZoomAumentar.Checked = false;

            }

        }


        private void ToolStripbtnLimparFerramentas_Click(System.Object sender, System.EventArgs e)
        {
            _desenhosCriados.Clear();
            this.Refresh();
        }


        private void ToolStripbtnLinhaTendencia_Click(System.Object sender, System.EventArgs e)
        {
            _ferramentaSelecionada = ToolStripbtnLinhaTendencia.Checked
                ? new FerramentaLinhaTendencia(_areaDeDesenho)
                : null;
            if (ToolStripbtnLinhaTendencia.Checked)
            {
                ToolStripbtnFibonacciRetracement.Checked = false;
                ToolStripbtnRetaHorizontal.Checked = false;
                ToolStripbtnCanal.Checked = false;
                ToolStripbtnZoomDiminuir.Checked = false;
                ToolStripbtnZoomAumentar.Checked = false;

            }

        }


        private void ToolStripbtnZoomAumentar_Click(System.Object sender, System.EventArgs e)
        {

            if (ToolStripbtnZoomAumentar.Checked)
            {
                ToolStripbtnFibonacciRetracement.Checked = false;
                ToolStripbtnRetaHorizontal.Checked = false;
                ToolStripbtnCanal.Checked = false;
                ToolStripbtnZoomDiminuir.Checked = false;
                ToolStripbtnLinhaTendencia.Checked = false;

            }

        }


        private void ToolStripbtnZoomDiminuir_Click(System.Object sender, System.EventArgs e)
        {

            if (ToolStripbtnZoomDiminuir.Checked)
            {
                ToolStripbtnFibonacciRetracement.Checked = false;
                ToolStripbtnRetaHorizontal.Checked = false;
                ToolStripbtnCanal.Checked = false;
                ToolStripbtnZoomAumentar.Checked = false;
                ToolStripbtnLinhaTendencia.Checked = false;

            }

        }


        private void PontosAtualizar()
        {

            if (blnAtualizandoPontos)
            {
                //Se já está atualizando os pontos, enttão sai fora para não entrar em loop.
                return;

            }

            //marca que está atualizando os pontos. Esta variável tem que ficar setada como true até o final da procedure.
            blnAtualizandoPontos = true;

            //calcula a área da cotação, verificando se devem ser impresso indicadores como volume e IFR
            //Se não for impresso nenhum outro indicador, utiliza o tamanho da área total
            intAreaCotacaoHeight = intAreaTotalHeight;


            if (blnVolumeDesenhar)
            {
                //se tem que imprimir o volume desconta 80 pixels que são utilizados para desenhar o gráfico de barras do volume
                intAreaCotacaoHeight = intAreaCotacaoHeight - intIndicadoresAreaHeight;

                //se tem que desenhar o volume calcula o top da área do volume
                intVolumeAreaTop = intAreaBottom - intIndicadoresAreaHeight;

            }


            if (blnIFRDesenhar)
            {
                //se tem que imprimir o IFR desconta 80 pixels que são utilizados para desenhar o gráfico de linhas do IFR
                intAreaCotacaoHeight = intAreaCotacaoHeight - intIndicadoresAreaHeight;

                intIFRAreaTop = intAreaBottom - Convert.ToInt32((blnVolumeDesenhar ? intIndicadoresAreaHeight : 0)) -
                                intIndicadoresAreaHeight;

            }

            intAreaCotacaoBottom = intAreaTop + intAreaCotacaoHeight;

            //CALCULA A POSIÇÃO NO ARRAY DE CANDLES EM FUNÇÃO DOS SEQUENCIAS
            ArrayDadosSequencialIndicesCalcular(lngAreaDesenhoSequencialInicial, lngAreaDesenhoSequencialFinal,
                ref intArrayCandlePosicaoInicial, ref intArrayCandlePosicaoFinal);

            //menor e maior data dos dados que serão impressos.
            DateTime dtmDataMinima = arrCandle[intArrayCandlePosicaoInicial].Data;
            var dtmDataMaxima = arrCandle[intArrayCandlePosicaoFinal].Data;

            DateTime dtmDataMaximaSplit = strPeriodoDuracao == "DIARIO"
                ? arrCandle[intArrayCandlePosicaoFinal].Data
                : arrCandle[intArrayCandlePosicaoFinal].DataFinal;

            int intI;

            //***********************TRATAMENTO PARA OS VALORES MÍNIMO E MÁXIMO DO GRÁFICO***************************

            var valoresExtremosService = new ValoresExtremosService();

            //CALCULA O VALOR MÍNIMO E MÁXIMO DA ÁREA DE COTAÇÕES. LEVA EM CONSIDERAÇÃO
            //AS COTAÇÕES E AS MÉDIAS QUE SERÃO DESENHADAS.

            //também calcula o número de pontos que serão impressos no IFR, no ifr médio, no volume médio
            //, nas médias móveis
            var valoresExtremos = valoresExtremosService.ValoresExtremosCalcular(new ConfiguracaoDeVisualizacao(strCodigoAtivo, strPeriodoDuracao, blnMMExpDesenhar, _mediasSelecionadas, blnVolumeDesenhar, blnIFRDesenhar, intIFRNumPeriodos, dtmDataMinima, dtmDataMaxima));
            var decValorMinimoPeriodo = valoresExtremos.ValorMinimo;
            decValorMaximoPeriodo = valoresExtremos.ValorMaximo;
            var dblVolumeMinimoPeriodo = valoresExtremos.VolumeMinimo;
            dblVolumeMaximoPeriodo = valoresExtremos.VolumeMaximo;

            //Debug.Print("Retornou de ValoresExtremos")

            int intArrayVolumeIndice = 0;
            int intArrayVolumeMedioIndice = 0;


            if (blnVolumeDesenhar)
            {
                //redimensiona também o array de retângulos do volume. 
                //Este terá o mesmo número de registros da área de cotação
                //calcula-se o número de registros do array de volume atráves das posições iniciais e finais
                //do array de dados que serão desenhados na tela
                Array.Resize(ref arrVolumeRectangle, intArrayCandlePosicaoFinal - intArrayCandlePosicaoInicial + 1);

                //para calcular o indice não precisa somar 1 porque o array é base 0.
                intArrayVolumeIndice = intArrayCandlePosicaoFinal - intArrayCandlePosicaoInicial;

                Array.Resize(ref arrVolumeMedio, valoresExtremos.VolumeMedioNumRegistros);

                intArrayVolumeMedioIndice = arrVolumeMedio.Length - 1;

            }

            int intArrayIFRIndice = 0;

            if (blnIFRDesenhar)
            {
                Array.Resize(ref arrIFRPonto, valoresExtremos.ContadorIFR);

                intArrayIFRIndice = arrIFRPonto.Length - 1;


            }

            //redimensiona o array
            Indicador objMedia;

            colMedia.Clear();


            if (blnMMExpDesenhar)
            {

                foreach (MediaDTO mediaSelecionada in _mediasSelecionadas)
                {

                    foreach (MediaDTO mediaExtremos in valoresExtremos.Medias)
                    {
                        if (!mediaExtremos.Equals(mediaSelecionada)) continue;
                        if (mediaExtremos.NumRegistros <= 0) continue;

                        objMedia = new Indicador
                        {
                            NumPeriodos = mediaSelecionada.NumPeriodos,
                            Tipo = mediaSelecionada.Tipo,
                            Cor = mediaSelecionada.Cor
                        };

                        objMedia.ArrayPontoRedimensionar(mediaExtremos.NumRegistros);

                        colMedia.Add(objMedia);
                    }

                }

            }

            //VOLUME MINIMO E MÁXIMO

            if (blnVolumeDesenhar)
            {
                dblVolumeMaximoPeriodoReferencia = dblVolumeMaximoPeriodo;

                //calcula os pontos principais para depois imprimir os labels de referência do volume.
                PontosPrincipaisCalcular(dblVolumeMinimoPeriodo, dblVolumeMaximoPeriodo, 4,
                    ref dblVolumeInicialReferencia, ref dblVolumeIntervaloReferencia);


                if ((dblVolumeMaximoPeriodo - (dblVolumeMaximoPeriodo - dblVolumeMinimoPeriodo) / 0.75) > 0)
                {
                    //tem que deixar 25% da área para o menor volume, para que o menor volume não fique sem aperecer
                    dblVolumeMinimoPeriodo = dblVolumeMaximoPeriodo -
                                             (dblVolumeMaximoPeriodo - dblVolumeMinimoPeriodo) / 0.75;


                }
                else
                {
                    dblVolumeMinimoPeriodo = dblVolumeMinimoPeriodo * 0.1;

                }


                if (dblVolumeMaximoPeriodo != dblVolumeMinimoPeriodo)
                {
                    //calcula o número de pixels por negócio na área de impressão do volume. 
                    //a área útil é a área de indicadores - 2 pixels de margem superior e 2 pixels de margem inferior
                    dblPixelPorNegocio = (intIndicadoresAreaHeight - intIndicadoresMargem * 2) /
                                         (dblVolumeMaximoPeriodo - dblVolumeMinimoPeriodo);


                }
                else
                {
                    dblPixelPorNegocio = (intIndicadoresAreaHeight - intIndicadoresMargem * 2);

                }

            }

            //CALCULA O NÚMERO DE PIXEL POR IFR.
            //O IFR SEMPRE TEM UMA ÁREA DE 100 PONTOS, POIS SEMPRE OSCILA ENTRE 0 E 100
            //TEM QUE DESCONTAR AS MARGENS SUPERIOR E INFERIOR
            if (blnIFRDesenhar)
            {
                dblPixelPorIFR = (intIndicadoresAreaHeight - intIndicadoresMargem * 2) / 100.0;
            }

            //CALCULA OS PONTOS DE REFERÊNCIA DE VALOR QUE DEVEM SER IMPRESSOS.
            //OS PONTOS TEM QUE SER CALCULADOS ANTES DO VALOR MÍNIMO E DO VALOR 
            //MÁXIMO SEREM PASSADOS PARA A ESCALA LOGARITMICA, CASO SEJA NECESSÁRIO
            decValorMaximoPeriodoReferencia = decValorMaximoPeriodo;

            PontosPrincipaisCalcular(decValorMinimoPeriodo, decValorMaximoPeriodoReferencia, 8,
                out decValorInicialReferencia, out decIntervaloReferencia);

            if (ToolStripbtnEscalaLogaritmica.Checked)
            {
                decValorMaximoPeriodo = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorMaximoPeriodo)));
                decValorMinimoPeriodo = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorMinimoPeriodo)));

            }

            cRSList objRsSplit = null;
            structSplit objSplit = default(structSplit);

            if (colSplit == null)
            {
                colSplit = new List<structSplit>();
            }
            else
            {
                colSplit.Clear();
            }

            cCarregadorSplit objCarregadorSplit = new cCarregadorSplit(this._conexao);

            //busca os splits do papel em ordem decrescente
            objCarregadorSplit.SplitConsultar(strCodigoAtivo, dtmDataMinima, "D", ref objRsSplit, dtmDataMaximaSplit,
                "DESD");

            //calcula quantos pixes cada um real vale para poder imprimir cada pixel posteriormente
            //remove duas bordas para os candles inferior e superior não ficar muito grudados, da mesma forma
            //que foi feito para a largura

            //para calcular divide o total de pixels pela diferença em reais entre o valor máximo e o valor mínimo
            //do período.
            if ((decValorMaximoPeriodo - decValorMinimoPeriodo) > 0)
            {
                dblPixelPorReal = (double) ((intAreaCotacaoHeight - intMargemSuperior * 3) /
                                            (decValorMaximoPeriodo - decValorMinimoPeriodo));
            }
            else
            {
                dblPixelPorReal = (intAreaCotacaoHeight - intMargemSuperior * 3);
            }

            //****FIM DO TRECHO CRIADO PARA ENCONTRAR ERROS

            //*********************CALCULA AS MÉDIAS MÓVEIS ESCOLHIDAS PELO USUÁRIO***********************

            //utilizado no loop

            Rectangle objRectCandleCorpo = new Rectangle();

            int intCorpoX = intAreaRight - intCandleWidth / 2;

            //IMPRIME OS CANDLES DA DIREITA PARA A ESQUERDA, PARA OS CASOS EM QUE NÃO TEM CANDLES SUFICIENTES
            //PARA TODO O PERÍODO

            //CALCULA A POSIÇÃO X DA PRIMEIRA CAUDA. A CAUDA TEM QUE SER IMPRESSA NO MEIO DO CANDLE
            //ADICIONA UMA BORDA EM RELAÇÃO AO TAMANHO TOTAL
            //DA ÁREA DE IMPRESSÃO PORQUE DENTRO DO LOOP ESTE VALOR SERÁ SUBTRAIDO DA LARGURA DO CANDLE JÁ PARA
            //A PRIMEIRA CAUDA
            int intCaudaX = intAreaRight;
            //+ intBorda

            //contém o maior valor entre o valor de abertura e valor de fechamento


            for (intI = intArrayCandlePosicaoFinal; intI >= intArrayCandlePosicaoInicial; intI += -1)
            {
                intCaudaX = intCaudaX - intLarguraTotal;
                intCorpoX = intCorpoX - intLarguraTotal;

                //valores em R$
                decimal decValorMaximo = arrCandle[intI].ValorMaximo;
                decimal decValorMinimo = arrCandle[intI].ValorMinimo;
                decimal decValorAbertura = arrCandle[intI].ValorAbertura;
                decimal decValorFechamento = arrCandle[intI].ValorFechamento;


                if (ToolStripbtnEscalaLogaritmica.Checked)
                {
                    decValorMaximo = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorMaximo)));
                    decValorMinimo = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorMinimo)));
                    decValorAbertura = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorAbertura)));
                    decValorFechamento = Convert.ToDecimal(Math.Log(Convert.ToDouble(decValorFechamento)));

                }


                PointF objPonto;
                if (blnMMExpDesenhar)
                {

                    foreach (Indicador objMedia_loopVariable in colMedia)
                    {
                        objMedia = objMedia_loopVariable;

                        if (objMedia.ArrayIndicadorPontoIndice >= 0)
                        {
                            //busca a média do candle
                            double dblMMExp = arrCandle[intI].MediaBuscar(objMedia.NumPeriodos, objMedia.Tipo);


                            if (dblMMExp > 0)
                            {

                                if (ToolStripbtnEscalaLogaritmica.Checked)
                                {
                                    dblMMExp = Math.Log(dblMMExp);

                                }

                                //CALCULA O PONTO PARA IMPRIMIR A MÉDIA MÓVEL EXPONENCIAL
                                objPonto = new Point(intCaudaX,
                                    intAreaTop + intMargemSuperior +
                                    (int) (((double) decValorMaximoPeriodo - dblMMExp) * dblPixelPorReal));
                                objMedia.ArrayIndicadorPontoSetar(objPonto);
                                objMedia.ArrayIndicadorPontoIndiceDecrementar();

                            }
                        }
                        //If objMedia.ArrayIndicadorPontoIndice >= 0 Then

                    }

                }
                //If ToolStripbtnMMExp.Checked Then


                if (blnVolumeDesenhar)
                {
                    //************INICIO DO TRATAMENTO PARA A BARRA DO VOLUME*************
                    double dblVolume = arrCandle[intI].Volume;

                    int intVolumeTop = intVolumeAreaTop + intIndicadoresMargem +
                                       Convert.ToInt32((dblVolumeMaximoPeriodo - dblVolume) * dblPixelPorNegocio);

                    //tem que descontar a margem inferior
                    int intVolumeHeight = intAreaBottom - intIndicadoresMargem - intVolumeTop;

                    Rectangle objVolumeRect = new Rectangle(intCorpoX, intVolumeTop, intCandleWidth, intVolumeHeight);

                    arrVolumeRectangle[intArrayVolumeIndice] = objVolumeRect;

                    intArrayVolumeIndice = intArrayVolumeIndice - 1;

                    //************FIM DO TRATAMENTO PARA A BARRA DO VOLUME*************

                    //************INICIO DO TRATAMENTO PARA A MÉDIA DO VOLUME*************


                    if (intArrayVolumeMedioIndice >= 0)
                    {
                        objPonto = new Point(intCaudaX,
                            intVolumeAreaTop + intIndicadoresMargem +
                            (int) ((dblVolumeMaximoPeriodo - arrCandle[intI].VolumeMedio) * dblPixelPorNegocio));

                        arrVolumeMedio[intArrayVolumeMedioIndice] = objPonto;

                        intArrayVolumeMedioIndice = intArrayVolumeMedioIndice - 1;

                    }

                    //************FIM DO TRATAMENTO PARA A MÉDIA DO VOLUME*************

                }



                if (blnIFRDesenhar)
                {
                    double dblIFR14 = arrCandle[intI].IFR14;


                    if (dblIFR14 > -1)
                    {
                        int intIFRY = intIFRAreaTop + intIndicadoresMargem +
                                      Convert.ToInt32((100 - dblIFR14) * dblPixelPorIFR);

                        objPonto = new Point(intCaudaX, intIFRY);

                        arrIFRPonto[intArrayIFRIndice] = objPonto;

                        intArrayIFRIndice = intArrayIFRIndice - 1;

                    }
                }

                //CALCULA OS PONTOS DA CAUDA. A CAUDA SERÁ DESENHADA DO VALOR MÁXIMO ATÉ O VALOR MÍNIMO
                int intCaudaY1 = intAreaTop + intMargemSuperior +
                                 Convert.ToInt32((decValorMaximoPeriodo - decValorMaximo) * (decimal) dblPixelPorReal);
                int intCaudaY2 = intAreaTop + intMargemSuperior +
                                 Convert.ToInt32((decValorMaximoPeriodo - decValorMinimo) * (decimal) dblPixelPorReal);

                //seta nos objetos do candle as propriedades da cauda
                arrCandle[intI].LinhaCaudaX = intCaudaX;
                arrCandle[intI].LinhaCaudaY1 = intCaudaY1;
                arrCandle[intI].LinhaCaudaY2 = intCaudaY2;

                //seta retângulo que contém a área total do candle incluindo o corpo e a cauda.
                //a altura deste retângulo vai do valor mínimo até o valor máximo e a largura é a largura do candle
                arrCandle[intI].RectAreaTotal =
                    new Rectangle(intCorpoX, intCaudaY1, intLarguraTotal, intCaudaY2 - intCaudaY1);

                //If decValorAbertura <> decValorFechamento Then

                int intCorpoY = 0;
                if (Convert.ToInt32(Math.Abs(decValorAbertura - decValorFechamento) * (decimal) dblPixelPorReal) > 0)
                {
                    //TRATAMENTO DE VARIÁVEIS UTILIZADAS NO CORPO E NA CAUDA

                    decimal decValorMaior;
                    if (decValorAbertura > decValorFechamento)
                    {
                        decValorMaior = decValorAbertura;

                        //SE A ABERTURA É MAIOR QUE O FECHAMENTO O CANDLE É PRETO
                        arrCandle[intI].CorpoCor = Brushes.Black;


                    }
                    else
                    {
                        decValorMaior = decValorFechamento;
                        //decValorMenor = decValorAbertura

                        arrCandle[intI].CorpoCor = Brushes.White;

                    }


                    intCorpoY = intAreaTop + intMargemSuperior +
                                Convert.ToInt32((decValorMaximoPeriodo - decValorMaior) * (decimal) dblPixelPorReal);
                    int intCorpoHeight = Convert.ToInt32(Math.Abs(decValorAbertura - decValorFechamento) *
                                                         (decimal) dblPixelPorReal);

                    objRectCandleCorpo.X = intCorpoX;
                    objRectCandleCorpo.Y = intCorpoY;
                    objRectCandleCorpo.Height = intCorpoHeight;
                    objRectCandleCorpo.Width = intCandleWidth;

                    //seta o tipo do corpo do candle para RETANGULO
                    arrCandle[intI].CorpoTipo = "R";

                    //seta no objeto do candle o retângulo do corpo do candle
                    arrCandle[intI].RectCorpo = objRectCandleCorpo;


                }
                else
                {
                    intCorpoY = intAreaTop + intMargemSuperior +
                                Convert.ToInt32((decValorMaximoPeriodo - decValorAbertura) * (decimal) dblPixelPorReal);

                    //seta o tipo do corpo do candle para LINHA
                    arrCandle[intI].CorpoTipo = "L";

                    //SE O VALOR DE ABERTURA E DE FECHAMENTO SÃO IGUAIS DESENHA APENAS UMA LINHA HORIZONTAL
                    arrCandle[intI].LinhaCorpoX1 = intCorpoX;
                    arrCandle[intI].LinhaCorpoX2 = intCorpoX + intCandleWidth;
                    arrCandle[intI].LinhaCorpoY = intCorpoY;

                }

                var limiteHorizontalEsquerdo = new LimiteHorizontal(intArrayCandlePosicaoInicial,
                    arrCandle[intArrayCandlePosicaoInicial].CoordenadaX1);
                var limiteHorizontalDireito = new LimiteHorizontal(intArrayCandlePosicaoFinal,
                    arrCandle[intArrayCandlePosicaoFinal].CoordenadaX1);

                if (_areaDeDesenho == null)
                {
                    _areaDeDesenho = new AreaDeDesenho(dblPixelPorReal, intAreaTop + intMargemSuperior,
                        decValorMaximoPeriodo
                        , ToolStripbtnEscalaLogaritmica.Checked ? cEnum.Escala.Logaritmica : cEnum.Escala.Aritmetica
                        , limiteHorizontalEsquerdo, limiteHorizontalDireito);

                }
                else
                {
                    _areaDeDesenho.AlterarValores(decValorMaximoPeriodo, dblPixelPorReal, limiteHorizontalEsquerdo,
                        limiteHorizontalDireito);
                }

                //*************TRATAMENTO DE SPLITS*************

                if (!objRsSplit.EOF)
                {
                    bool blnSplitAdicionar;

                    //só imprime splits quando forem de desdobramento.

                    if ((string) ToolStripcmbPeriodoDuracao.SelectedItem == "Semanal")
                    {
                        if (Convert.ToDateTime(objRsSplit.Field("Data")) >= arrCandle[intI].Data &&
                            Convert.ToDateTime(objRsSplit.Field("Data")) <= arrCandle[intI].DataFinal)
                        {
                            blnSplitAdicionar = true;
                        }
                        else
                        {
                            blnSplitAdicionar = false;
                        }


                    }
                    else
                    {
                        blnSplitAdicionar = Convert.ToDateTime(objRsSplit.Field("Data")) == arrCandle[intI].Data;

                    }

                    //verifica se a data é igual a data da cotação

                    if (blnSplitAdicionar)
                    {
                        //se as datas são iguais criar uma estrutura
                        objSplit.strRazao = (string) objRsSplit.Field("Razao2");
                        objSplit.intPosicaoX = intCaudaX;

                        colSplit.Add(objSplit);

                        objRsSplit.MoveNext();

                    }

                }
                //**********FIM DO TRATAMENTO DE SPLITS**********

            }

            intCandle1X = arrCandle[intArrayCandlePosicaoInicial].CoordenadaX1;

            //****************INICIO DO TRATAMENTO PARA OS OBJETOS DESENHADOS NA TELA****************

            foreach (Desenho desenho in _desenhosCriados)
            {
                desenho.AtualizarPontos(arrCandle[desenho.PontoInicial.Indice].CoordenadaX1,
                    arrCandle[desenho.PontoFinal.Indice].CoordenadaX1);
            }


            //****************FIM DO TRATAMENTO PARA OS OBJETOS DESENHADOS NA TELA****************

            blnAtualizandoDados = false;

            blnAtualizandoPontos = false;

        }

        private void frmGrafico_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (AreaDadosPontoPertencer(e.Location))
            {
                blnMouseDown = true;

                objMouseDownPonto = e.Location;

            }

        }


        private void AreaDadosMover(Point pobjPonto2)
        {
            if (AreaDadosPontoPertencer(pobjPonto2))
            {

                if (objMouseDownPonto.X != pobjPonto2.X)
                {
                    int intPixelsDistancia = Math.Abs(objMouseDownPonto.X - pobjPonto2.X);

                    //calcula o número de candles do movimento em função da distância do movimento
                    int intNumCandlesMover = (int) Math.Truncate((decimal) intPixelsDistancia / intLarguraTotal);


                    if (intNumCandlesMover >= 1)
                    {
                        //se as posições na horizontal são diferentes.

                        if (objMouseDownPonto.X < pobjPonto2.X)
                        {
                            //move para a esquerda


                            if ((lngAreaDesenhoSequencialInicial - intNumCandlesMover) < 1)
                            {
                                lngAreaDesenhoSequencialFinal =
                                    lngAreaDesenhoSequencialFinal - lngAreaDesenhoSequencialInicial + 1;

                                lngAreaDesenhoSequencialInicial = 1;


                            }
                            else
                            {
                                //tem espaço para mover integralmente.
                                lngAreaDesenhoSequencialInicial = lngAreaDesenhoSequencialInicial - intNumCandlesMover;
                                lngAreaDesenhoSequencialFinal = lngAreaDesenhoSequencialFinal - intNumCandlesMover;

                            }


                        }
                        else
                        {
                            //move para a direita
                            if ((lngAreaDadosSequencialFinal + intNumCandlesMover) > lngSequencialMaximo)
                            {
                                lngAreaDesenhoSequencialInicial =
                                    lngSequencialMaximo - lngAreaDesenhoSequencialFinal +
                                    lngAreaDesenhoSequencialInicial;
                                lngAreaDesenhoSequencialFinal = lngSequencialMaximo;


                            }
                            else
                            {
                                lngAreaDesenhoSequencialInicial = lngAreaDesenhoSequencialInicial + intNumCandlesMover;
                                lngAreaDesenhoSequencialFinal = lngAreaDesenhoSequencialFinal + intNumCandlesMover;

                            }


                        }

                        blnPontosAtualizar = true;

                        blnDadosAtualizar = true;

                        this.Refresh();

                        //atribui o ponto 2 como o ponto de mouse down, para o próximo movimento.
                        objMouseDownPonto = pobjPonto2;

                    }
                    //if intNumCandlesMover > 0 Then ...


                }

            }

        }


        private void frmGrafico_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (blnMouseDown)
            {
                AreaDadosMover(e.Location);

                //marca para nao desenhar mais.
                blnMouseDown = false;

            }

        }

        private void frmGrafico_MouseLeave(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }


        private void ToolStripbtnIFR14_Click(System.Object sender, System.EventArgs e)
        {

            if (ToolStripbtnIFR14.Checked)
            {
                ToolStripcmbIFRNumPeriodos.Enabled = true;

                if (ToolStripcmbIFRNumPeriodos.Text.IsNumeric())
                {
                    ToolStripcmbIFRNumPeriodos.Text = "2";
                }
            }
            else
            {
                ToolStripcmbIFRNumPeriodos.Enabled = false;
            }

        }


        private void TSmnuMMConfigurar_Click(System.Object sender, System.EventArgs e)
        {
            frmIndicadorEscolha frmForm = new frmIndicadorEscolha("Escolha as Médias Móveis", _mediasSelecionadas);

            frmForm.ShowDialog();


            if (frmForm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //se retornou OK tem que atualizar a collection
                _mediasSelecionadas = frmForm.MediasSelecionadas;

            }

            frmForm = null;

        }

        /// <summary>
        /// O Array 2 está contido no array 1. A função calcula os índices do array 1 que contém 
        /// a posição inicial e final do array 2.
        /// </summary>
        /// <param name="plngArray2SequencialInicial"></param>
        /// <param name="plngArray2SequencialFinal"></param>
        /// <param name="pintIndiceInicialRet"></param>
        /// <param name="pintIndiceFinalRet"></param>
        /// <remarks></remarks>

        private void ArrayDadosSequencialIndicesCalcular(long plngArray2SequencialInicial,
            long plngArray2SequencialFinal, ref int pintIndiceInicialRet, ref int pintIndiceFinalRet)
        {
            pintIndiceInicialRet = (int) (plngArray2SequencialInicial - lngAreaDadosSequencialInicial);

            pintIndiceFinalRet = (int) (plngArray2SequencialFinal - lngAreaDadosSequencialInicial);

        }

        /// <summary>
        /// Busca a collection de médias do último candle preenchido.
        /// </summary>
        /// <param name="pblnCotacoesBuscou">Indica se buscou novas cotações. Caso não tenha buscado pode
        /// estar buscando apenas as médias.</param>
        /// <param name="pintBuscaIndiceInicial">Caso tenha buscado cotações, indica o indice inicial do array de candles que foi buscado</param>
        /// <param name="pintBuscaIndiceFinal">Caso tenha buscado cotações, indica o indice final do array de candles que foi buscado</param>
        /// <returns>collection com estruturas do tipo structMediaValor</returns>
        /// <remarks></remarks>
        private Dictionary<string, cEstrutura.structMediaValor> UltimoCandlePreenchidoCollectionMediaBuscar(
            bool pblnCotacoesBuscou, int pintBuscaIndiceInicial, int pintBuscaIndiceFinal)
        {
            Dictionary<string, cEstrutura.structMediaValor> functionReturnValue;

            //obtém a collection de médias do último candle, ou seja, do candle de data mais recente.

            if (pblnCotacoesBuscou)
            {
                //Quando está buscando novos candles para a parte mais alta do array

                if (pintBuscaIndiceFinal != arrCandle.Length - 1)
                {
                    //se não buscou candle novo para a última posição do array, então pode pegar
                    //a collection de médias do último candle.
                    functionReturnValue = arrCandle[arrCandle.Length - 1].GetMedia();

                }
                else
                {
                    //se buscou candle novo para a última posição, então pega a collection de médias
                    //do candle anterior ao  indice inicial que está sendo buscado, desde que 
                    //este indice seja maior do que zero
                    functionReturnValue = pintBuscaIndiceInicial > 0
                        ? arrCandle[pintBuscaIndiceInicial - 1].GetMedia()
                        : null;

                }

            }
            else
            {
                //se não buscou cotações, pega a collection de médias do último candle do array
                functionReturnValue = arrCandle[arrCandle.Length - 1].GetMedia();

            }
            return functionReturnValue;


        }


        /// <summary>
        /// Verifica se uma determinada média está presente no último candle preenchido do array 
        /// </summary>
        /// <param name="pobjMediaDTO">estrutura que representa a média</param>
        /// <param name="pblnCotacoesBuscou">Indica se buscou novas cotações. Caso não tenha buscado pode
        /// estar buscando apenas as médias.</param>
        /// <param name="pintBuscaIndiceInicial">Caso tenha buscado cotações, indica o indice inicial do array de candles que foi buscado</param>
        /// <param name="pintBuscaIndiceFinal">Caso tenha buscado cotações, indica o indice final do array de candles que foi buscado</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool UltimoCandleMediaExistir(MediaDTO pobjMediaDTO, bool pblnCotacoesBuscou,
            int pintBuscaIndiceInicial, int pintBuscaIndiceFinal)
        {
            //inicializa marcando que a média não existe.
            //Caso encontre a média no candle, desmarca
            bool blnMediaExistir = false;

            //busca a collection de média do último candle.
            Dictionary<string, cEstrutura.structMediaValor> colstructMediaValor =
                UltimoCandlePreenchidoCollectionMediaBuscar(pblnCotacoesBuscou, pintBuscaIndiceInicial,
                    pintBuscaIndiceFinal);

            if (colstructMediaValor != null)
            {

                blnMediaExistir =
                    colstructMediaValor.Select(mediaValor => mediaValor.Value)
                        .Any(
                            objstructMediaValor =>
                                pobjMediaDTO.NumPeriodos == objstructMediaValor.intPeriodo &&
                                pobjMediaDTO.Tipo == objstructMediaValor.strTipo);
            }

            return blnMediaExistir;

        }

        /// <summary>
        /// Calcula a largura total e a largura do corpo do candle em funçao do seu tamanho
        /// </summary>
        /// <remarks></remarks>

        private void ZoomMedidasCandleCalcular()
        {
            switch (intZoom)
            {

                case 7:

                    //largura total = 13
                    intLarguraTotal = 10;

                    //largura do candle = 7; espaçamento = 3
                    intCandleWidth = 7;

                    break;

                case 6:

                    //largura total = 11
                    intLarguraTotal = 9;

                    //largura do candle = 6; espaçamento = 3
                    intCandleWidth = 6;

                    break;

                case 5:

                    //largura total = 9
                    intLarguraTotal = 8;

                    //largura do candle = 5; espaçamento = 3
                    intCandleWidth = 5;

                    break;

                case 4:

                    //largura total = 7
                    intLarguraTotal = 7;

                    //largura do candle = 4; espaçamento = 3
                    intCandleWidth = 4;

                    break;
                case 3:

                    //largura do candle = 3; espaçamento = 2
                    intLarguraTotal = 5;

                    intCandleWidth = 3;

                    break;
                case 2:

                    //largura do candle = 2; espaçamento = 1
                    intLarguraTotal = 3;

                    intCandleWidth = 2;

                    break;
                case 1:

                    //largura do candle = 1; espaçamento = 1
                    intLarguraTotal = 2;

                    intCandleWidth = 1;

                    break;
            }

        }

        /// <summary>
        /// Calcula o número de candles que é possível desenhar na área de desenhos
        /// </summary>
        /// <returns>o número de candles que pode ser desenhado</returns>
        /// <remarks></remarks>
        private int AreaDesenhoNumCandlesCalcular()
        {

            //no tamanho padrão cada candle ocupa 7 pixes de largura. São 4 pixels para a largura do corpo do candle e mais 
            //3 pixels de espaçamento entre candles
            return Convert.ToInt32((intAreaWidth - intMargemSuperior * 3) / intLarguraTotal);

        }

        private void frmGrafico_MouseClick(object sender, MouseEventArgs e)
        {
            if (!AreaDadosPontoPertencer(e.Location))
            {
                return;
            }

            int intIndice = 0;

            //número máximo de candles da área de desenho.
            int intAreaDesenhoNumCandles;

            MousePositionArrayCandleIndiceCalcular(e.Location, ref intIndice);


            if (TemFerramentaDeDesenhoSelecionada && _ferramentaSelecionada != null)
            {
                _ferramentaSelecionada.Click(new PontoDoDesenho(e.Location, intIndice));

                var desenho = _ferramentaSelecionada.DesenhoGerado;

                if (desenho != null)
                {
                    _desenhosCriados.Add(desenho);
                }


            }
            else if (ToolStripbtnZoomAumentar.Checked)
            {
                if (intZoom >= 7) return;

                intZoom = intZoom + 1;

                //calcula a largura total e do corpo dos candles em função zoom
                ZoomMedidasCandleCalcular();

                //calcula o número de candles que é possível desenhar
                intAreaDesenhoNumCandles = AreaDesenhoNumCandlesCalcular();

                //quando o zoom está aumentando fixamos a área à esquerda e movemos a parte direita da janela

                if ((lngAreaDesenhoSequencialInicial + intAreaDesenhoNumCandles - 1) <= lngSequencialMaximo)
                {
                    //caso tenha espaço aumenta a área de desenho para à direita
                    lngAreaDesenhoSequencialFinal = lngAreaDesenhoSequencialInicial + intAreaDesenhoNumCandles - 1;


                }
                else
                {
                    //caso nao tenha espaço para aumentar toda a área aumenta até o máximo
                    lngAreaDesenhoSequencialFinal = lngSequencialMaximo;


                    if ((lngAreaDesenhoSequencialFinal - intAreaDesenhoNumCandles + 1) >= 1)
                    {
                        //se tem espaço para mover a área de desenho para a esquerda
                        lngAreaDesenhoSequencialInicial = lngAreaDesenhoSequencialFinal - intAreaDesenhoNumCandles + 1;


                    }
                    else
                    {
                        lngAreaDesenhoSequencialInicial = 1;

                    }

                }

                blnDadosAtualizar = true;
                blnPontosAtualizar = true;

                this.Refresh();
            }
            else if (ToolStripbtnZoomDiminuir.Checked)
            {
                if (intZoom <= 1) return;
                intZoom = intZoom - 1;

                //calcula a largura total e do corpo dos candles em função zoom
                ZoomMedidasCandleCalcular();

                //calcula o número de candles que é possível desenhar
                intAreaDesenhoNumCandles = AreaDesenhoNumCandlesCalcular();

                //quando o zoom está diminuindo fixamos a área à direita e movemos a parte esquerda da janela

                if ((lngAreaDesenhoSequencialFinal - intAreaDesenhoNumCandles + 1) >= 1)
                {
                    //caso tenha espaço aumenta a área de desenho para à direita
                    lngAreaDesenhoSequencialInicial = lngAreaDesenhoSequencialFinal - intAreaDesenhoNumCandles + 1;


                }
                else
                {
                    //caso nao tenha espaço para aumentar toda a área aumenta até a mínimo
                    lngAreaDesenhoSequencialInicial = 1;


                    if ((lngAreaDesenhoSequencialInicial + intAreaDesenhoNumCandles - 1) <= lngSequencialMaximo)
                    {
                        //se tem espaço para mover a área de desenho para a esquerda
                        lngAreaDesenhoSequencialFinal = lngAreaDesenhoSequencialInicial + intAreaDesenhoNumCandles - 1;


                    }
                    else
                    {
                        lngAreaDesenhoSequencialFinal = lngSequencialMaximo;

                    }

                }

                blnDadosAtualizar = true;
                blnPontosAtualizar = true;

                this.Refresh();
            }
            else
            {
                CandleDadosAtualizar(e);

            }

        }

    }
}



