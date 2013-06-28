using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjDTO;
using prjModelo.Regras;

namespace prjModelo.Entidades
{

	public abstract class Setup
	{

		protected int intID;

		protected string strDescricao;
		public int ID {
			get { return intID; }
		}

		public string Descricao {
			get { return strDescricao; }
		}

		public override bool Equals(object obj)
		{
			var objSetup = (Setup)obj;
			return (ID == objSetup.ID);
		}

		protected decimal CalcularValorMargem(decimal pdecValor)
		{

			decimal decValorMargem = Math.Round(pdecValor * 0.0025M, 2);

			if (decValorMargem < 0.01M) {
				decValorMargem = 0.01M;
			}

			return decValorMargem;

		}

		public abstract bool GeraEntradaNaCotacaoDoAcionamento { get; }
		public abstract bool TemFiltro { get; }
		public abstract bool SubirStopApenasAposRealizacaoParcial { get; }
		public abstract bool RealizarCalculosAdicionais { get; }
		public abstract decimal CalculaValorEntrada(cCotacaoAbstract pobjCotacao);
		public abstract decimal CalculaValorStopLossInicial(cCotacaoAbstract pobjCotacao);
		public abstract decimal CalculaValorRealizacaoParcial(cCotacaoAbstract pobjCotacao);

		protected decimal VerificaQualValorDeStopDeveSerUtilizado(decimal pdecNovoValor, decimal pdecValorAtual)
		{

			//O novo valor do stop nunca pode estar abaixo do valor anterior
			if (pdecNovoValor > pdecValorAtual) {
				return pdecNovoValor;
			} else {
				return pdecValorAtual;
			}

		}

		public virtual decimal CalculaValorStopLossDeSaida(cCotacaoAbstract pobjCotacao, InformacoesDoTradeDTO pobjInformacoesDoTradeDTO)
		{

			decimal decNovoValorDeStop = default(decimal);

			decNovoValorDeStop = pobjCotacao.ValorMinimo - CalcularValorMargem(pobjCotacao.ValorMinimo);

			return VerificaQualValorDeStopDeveSerUtilizado(decNovoValorDeStop, pobjInformacoesDoTradeDTO.ValorDoStopLoss);

		}


	}

	public class SetupIFR2SemFiltro : Setup
	{

		public SetupIFR2SemFiltro()
		{
			intID = 1;
			strDescricao = "IFR 2 sem filtro";
		}

		public override bool GeraEntradaNaCotacaoDoAcionamento {
			get { return true; }
		}

		public override bool TemFiltro {
			get { return false; }
		}

		public override bool SubirStopApenasAposRealizacaoParcial {
			get { return false; }
		}

		public override bool RealizarCalculosAdicionais {
			get { return false; }
		}

		public override decimal CalculaValorEntrada(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento;
		}

		public override decimal CalculaValorStopLossInicial(cCotacaoAbstract pobjCotacao)
		{

			decimal decValor = Math.Round(pobjCotacao.ValorMinimo - (pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo) * 1.3M, 2);

			if (decValor < 0) {
				decValor = 0;
			}

			return decValor;

		}

		public override decimal CalculaValorRealizacaoParcial(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento * 1.05M;
		}

	}

	public class SetupIFR2SemFiltroRealizacaoParcial : Setup
	{

		public SetupIFR2SemFiltroRealizacaoParcial()
		{
			intID = 10;
			strDescricao = "IFR 2 sem filtro - RP";
		}

		public override bool GeraEntradaNaCotacaoDoAcionamento {
			get { return true; }
		}

		public override bool TemFiltro {
			get { return false; }
		}

		public override bool SubirStopApenasAposRealizacaoParcial {
			get { return true; }
		}

		public override bool RealizarCalculosAdicionais {
			get { return true; }
		}


		public override decimal CalculaValorEntrada(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento;
		}

		public override decimal CalculaValorStopLossInicial(cCotacaoAbstract pobjCotacao)
		{

			decimal decValor = Math.Round(pobjCotacao.ValorMinimo - (pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo) * 1.3M, 2);

			if (decValor < 0) {
				decValor = 0;
			}

			return decValor;

		}

		public override decimal CalculaValorRealizacaoParcial(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento * 1.05M;
		}

		public override decimal CalculaValorStopLossDeSaida(cCotacaoAbstract pobjCotacao, InformacoesDoTradeDTO pobjInformacoesDoTradeDTO)
		{

			if (!pobjInformacoesDoTradeDTO.IFRCruzouMediaParaCima || !pobjInformacoesDoTradeDTO.PermitiuRealizarParcial) {
				//caso ainda não tenha cruzado a média para cima ou ainda não tenha permitido realização parcial retorna o mesmo valor do stop loss
				return pobjInformacoesDoTradeDTO.ValorDoStopLoss;
			}

			IList<cMediaAbstract> lstMedias = pobjCotacao.Medias.Where(x => (x.NumPeriodos == 21 || x.NumPeriodos == 49 || x.NumPeriodos == 200) && x.Tipo == "MME").ToList();

			decimal decMaiorMedia = (decimal) lstMedias.Max(x => x.Valor);

			decimal decNovoValorDoStop;


			if (pobjCotacao.ValorFechamento > decMaiorMedia || cVerificadorMediasAlinhadas.Verificar(ref lstMedias)) {
				cCotacaoAbstract objCotacaoDoValorMinimoAnterior = BuscaCotacaoValorMinimoAnterior.Buscar(pobjCotacao);
				decNovoValorDoStop = objCotacaoDoValorMinimoAnterior.ValorMinimo - CalcularValorMargem(objCotacaoDoValorMinimoAnterior.ValorMinimo);

			} else {
				decNovoValorDoStop = pobjCotacao.ValorMinimo - CalcularValorMargem(pobjCotacao.ValorMinimo);
			}

			return VerificaQualValorDeStopDeveSerUtilizado(decNovoValorDoStop, pobjInformacoesDoTradeDTO.ValorDoStopLoss);

		}

	}

	public class SetupIFR2ComFiltro : Setup
	{

		public SetupIFR2ComFiltro()
		{
			intID = 2;
			strDescricao = "IFR 2 com filtro";
		}

		public override bool GeraEntradaNaCotacaoDoAcionamento {
			get { return false; }
		}

		public override bool TemFiltro {
			get { return true; }
		}

		public override bool SubirStopApenasAposRealizacaoParcial {
			get { return false; }
		}

		public override bool RealizarCalculosAdicionais {
			get { return false; }
		}


		public override decimal CalculaValorEntrada(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorMaximo + CalcularValorMargem(pobjCotacao.ValorMaximo);
		}

		public override decimal CalculaValorStopLossInicial(cCotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorMinimo - CalcularValorMargem(pobjCotacao.ValorMinimo);
		}

		public override decimal CalculaValorRealizacaoParcial(cCotacaoAbstract pobjCotacao)
		{

			decimal decValorRP = default(decimal);

			decimal decValorEntrada = CalculaValorEntrada(pobjCotacao);

			decValorRP = decValorEntrada + pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo;

			if ((decValorRP / decValorEntrada) < 1.03M) {
				decValorRP = Math.Round(decValorEntrada * 1.03M, 2);
			}

			return decValorRP;
		}


	}

}
