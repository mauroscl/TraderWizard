using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDTO;

namespace prjModelo.Entidades
{
	public abstract class cCotacaoAbstract
	{

		public cAtivo Ativo { get; set; }
		public System.DateTime Data { get; set; }
		public int Sequencial { get; set; }
		public decimal ValorAbertura { get; set; }
		public decimal ValorFechamento { get; set; }
		public decimal ValorMinimo { get; set; }
		public decimal ValorMaximo { get; set; }
		public IList<cMediaAbstract> Medias { get; set; }
		public cIFR IFR { get; set; }


		public cCotacaoAbstract(cAtivo pobjAtivo, System.DateTime pdtmData)
		{
			Ativo = pobjAtivo;
			Data = pdtmData;
			Medias = new List<cMediaAbstract>();

		}

		public decimal Amplitude {
			get { return ValorMaximo - ValorMinimo; }
		}

		public override bool Equals(object obj)
		{
            cCotacaoAbstract objAux = (cCotacaoAbstract)obj;
			if (Ativo.Codigo == objAux.Ativo.Codigo && Data == objAux.Data) {
				return true;
			} else {
				return false;
			}
		}

		public decimal CalculaPercentualDoFechamentoEmRelacaoAMedia(cMediaDTO pobjMediaDTO)
		{

			decimal decValorMedia = (from x in Medias where x.Tipo == pobjMediaDTO.CampoTipoBD && x.NumPeriodos == pobjMediaDTO.NumPeriodos select Convert.ToDecimal (x.Valor)).Single();

			return (ValorFechamento / decValorMedia - 1) * 100;

		}

		/// <summary>
		/// utilizado para conveter a média em função de um split.
		/// </summary>
		/// <param name="pdblRazao"></param>
		/// <remarks></remarks>

		public void Converter(double pdblRazao)
		{
            decimal decRazao = Convert.ToDecimal(pdblRazao);
            ValorAbertura *= decRazao;
            ValorFechamento *= decRazao;
            ValorMinimo *= decRazao;
            ValorMaximo *= decRazao;


			foreach (cMediaAbstract objMedia in Medias) {
				objMedia.Valor *= pdblRazao;

			}
			//IFR não precisa converter.

		}

		public IList<cMediaDTO> ObtemListaDeMediasDTO()
		{
			return (from m in Medias select m.ObtemDTO()).ToList();
		}


		private void CarregarCotacoesAnteriores()
		{
			//obtem o último dia anterior a este em que há cotações para o ativo
			//Dim dtmDataDaUltimaCotacaoAnterior As DateTime? = Ativo.CotacoesDiarias.Where(Function(x) x.Data < Data).Max(Of DateTime)(Function(y) y.Data)
			var objUltimaCotacaoAnterior = (from c in Ativo.CotacoesDiarias where c.Data < Data select c).LastOrDefault();

			DateTime dtmDataInicial = Data;

			var lstMediasDTO = ObtemListaDeMediasDTO();

			var blnEncontrouCotacoes = false;


			while (!blnEncontrouCotacoes) {
				DateTime dtmDataFinal = dtmDataInicial.AddDays(-1);
				DateTime dtmDataDoPrimeiroDiaDoMes = new DateTime(dtmDataFinal.Year, dtmDataFinal.Month, 1);

				if ((objUltimaCotacaoAnterior == null) || dtmDataDoPrimeiroDiaDoMes >= objUltimaCotacaoAnterior.Data.AddDays(1)) {
					//se não tem cotações anteriores a data atual ou se a data do primeiro dia do mês é maior ou igual a data da última cotação
					//anterior utiliza a primeira data do mês para buscar todas as cotações do mês.
					dtmDataInicial = dtmDataDoPrimeiroDiaDoMes;
				} else {
					//caso contrário busca a partir do primeiro dia após a última cotação
					dtmDataInicial = objUltimaCotacaoAnterior.Data.AddDays(1);
				}

				blnEncontrouCotacoes = Ativo.CarregarCotacoes(dtmDataInicial, dtmDataFinal, lstMediasDTO, true);

			}

		}

		public cCotacaoAbstract CotacaoAnterior()
		{

			cCotacaoAbstract objCotacaoAnterior = null;

			objCotacaoAnterior = Ativo.CotacoesDiarias.SingleOrDefault(x => x.Sequencial == Sequencial - 1);


			if ((objCotacaoAnterior == null) && Sequencial > 1) {
				CarregarCotacoesAnteriores();

			}

			//retorna a cotação cujo sequencial é uma posição anterior ao sequencial da cotação atual.
			return Ativo.CotacoesDiarias.SingleOrDefault(x => x.Sequencial == Sequencial - 1);

		}

	}

	public class cCotacaoDiaria : cCotacaoAbstract
	{

		public cCotacaoDiaria(cAtivo pobjAtivo, System.DateTime pdtmData) : base(pobjAtivo, pdtmData)
		{
		}

		public cCotacaoDiaria Clonar()
		{

			cCotacaoDiaria objRetorno = new cCotacaoDiaria(Ativo, Data);

			objRetorno.Sequencial = Sequencial;
			objRetorno.ValorAbertura = ValorAbertura;
			objRetorno.ValorFechamento = ValorFechamento;
			objRetorno.ValorMinimo = ValorMinimo;
			objRetorno.ValorMaximo = ValorMaximo;

			objRetorno.IFR = new cIFRDiario(objRetorno, IFR.NumPeriodos, IFR.Valor);


			foreach (cMediaDiaria objMedia in Medias) {
				objRetorno.Medias.Add(new cMediaDiaria(objRetorno, objMedia.Tipo, objMedia.NumPeriodos, objMedia.Valor));

			}

			return objRetorno;

		}

	}
}
