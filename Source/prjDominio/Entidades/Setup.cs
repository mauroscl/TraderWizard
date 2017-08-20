using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Regras;
using DTO;

namespace Dominio.Entidades
{

	public abstract class Setup
    {

        #region Equality Members
        protected bool Equals(Setup other)
	    {
	        return Id == other.Id;
	    }

	    public override int GetHashCode()
	    {
	        return Id;
	    }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Setup)obj);
        }
        #endregion

        public int Id { get; protected set; }

	    public string Descricao { get; protected set; }


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
		public abstract decimal CalculaValorEntrada(CotacaoAbstract pobjCotacao);
		public abstract decimal CalculaValorStopLossInicial(CotacaoAbstract pobjCotacao);
		public abstract decimal CalculaValorRealizacaoParcial(CotacaoAbstract pobjCotacao);

		protected decimal VerificaQualValorDeStopDeveSerUtilizado(decimal pdecNovoValor, decimal pdecValorAtual)
		{

			//O novo valor do stop nunca pode estar abaixo do valor anterior
			if (pdecNovoValor > pdecValorAtual) {
				return pdecNovoValor;
			} else {
				return pdecValorAtual;
			}

		}

        public virtual decimal CalculaValorStopLossDeSaida(CotacaoAbstract pobjCotacao, InformacoesDoTradeDTO pobjInformacoesDoTradeDTO, CotacaoAbstract cotacaoDoValorMinimoAnterior)
        {
            decimal decNovoValorDeStop = pobjCotacao.ValorMinimo - CalcularValorMargem(pobjCotacao.ValorMinimo);

            return VerificaQualValorDeStopDeveSerUtilizado(decNovoValorDeStop, pobjInformacoesDoTradeDTO.ValorDoStopLoss);
        }
	}

	public class SetupIFR2SemFiltro : Setup
	{

		public SetupIFR2SemFiltro()
		{
			Id = 1;
			Descricao = "IFR 2 sem filtro";
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

		public override decimal CalculaValorEntrada(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento;
		}

		public override decimal CalculaValorStopLossInicial(CotacaoAbstract pobjCotacao)
		{

			decimal decValor = Math.Round(pobjCotacao.ValorMinimo - (pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo) * 1.3M, 2);

			if (decValor < 0) {
				decValor = 0;
			}

			return decValor;

		}

		public override decimal CalculaValorRealizacaoParcial(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento * 1.05M;
		}

	}

	public class SetupIFR2SemFiltroRealizacaoParcial : Setup
	{

		public SetupIFR2SemFiltroRealizacaoParcial()
		{
			Id = 10;
			Descricao = "IFR 2 sem filtro - RP";
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


		public override decimal CalculaValorEntrada(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento;
		}

		public override decimal CalculaValorStopLossInicial(CotacaoAbstract pobjCotacao)
		{

			decimal decValor = Math.Round(pobjCotacao.ValorMinimo - (pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo) * 1.3M, 2);

			if (decValor < 0) {
				decValor = 0;
			}

			return decValor;

		}

		public override decimal CalculaValorRealizacaoParcial(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorFechamento * 1.05M;
		}

		public override decimal CalculaValorStopLossDeSaida(CotacaoAbstract pobjCotacao, InformacoesDoTradeDTO pobjInformacoesDoTradeDTO, CotacaoAbstract cotacaoDoValorMinimoAnterior)
		{

			if (!pobjInformacoesDoTradeDTO.IFRCruzouMediaParaCima || !pobjInformacoesDoTradeDTO.PermitiuRealizarParcial) {
				//caso ainda não tenha cruzado a média para cima ou ainda não tenha permitido realização parcial retorna o mesmo valor do stop loss
				return pobjInformacoesDoTradeDTO.ValorDoStopLoss;
			}

			IList<MediaAbstract> lstMedias = pobjCotacao.Medias.Where(x => (x.NumPeriodos == 21 || x.NumPeriodos == 49 || x.NumPeriodos == 200) && x.Tipo == "MME").ToList();

			decimal decMaiorMedia = (decimal) lstMedias.Max(x => x.Valor);

			decimal decNovoValorDoStop;


			if (pobjCotacao.ValorFechamento > decMaiorMedia || VerificadorMediasAlinhadas.Verificar(ref lstMedias)) {
				//cCotacaoAbstract objCotacaoDoValorMinimoAnterior = BuscaCotacaoValorMinimoAnterior.Buscar(pobjCotacao);
                decNovoValorDoStop = cotacaoDoValorMinimoAnterior.ValorMinimo - CalcularValorMargem(cotacaoDoValorMinimoAnterior.ValorMinimo);

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
			Id = 2;
			Descricao = "IFR 2 com filtro";
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


		public override decimal CalculaValorEntrada(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorMaximo + CalcularValorMargem(pobjCotacao.ValorMaximo);
		}

		public override decimal CalculaValorStopLossInicial(CotacaoAbstract pobjCotacao)
		{
			return pobjCotacao.ValorMinimo - CalcularValorMargem(pobjCotacao.ValorMinimo);
		}

		public override decimal CalculaValorRealizacaoParcial(CotacaoAbstract pobjCotacao)
		{
		    decimal decValorEntrada = CalculaValorEntrada(pobjCotacao);

			decimal valorDaRealizacaoParcial = decValorEntrada + pobjCotacao.ValorMaximo - pobjCotacao.ValorMinimo;

			if ((valorDaRealizacaoParcial / decValorEntrada) < 1.03M) {
				valorDaRealizacaoParcial = Math.Round(decValorEntrada * 1.03M, 2);
			}

			return valorDaRealizacaoParcial;
		}


	}

}
