using System;
using System.Collections.Generic;
using System.Linq;
using DTO;

namespace Dominio.Entidades
{
	public abstract class CotacaoAbstract
    {
        #region Equality Members
        protected bool Equals(CotacaoAbstract other)
	    {
	        return Equals(Ativo, other.Ativo) && Data.Equals(other.Data);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            return ((Ativo != null ? Ativo.GetHashCode() : 0)*397) ^ Data.GetHashCode();
	        }
	    }

#endregion

	    public Ativo Ativo { get; set; }
		public System.DateTime Data { get; set; }
		public int Sequencial { get; set; }
		public decimal ValorAbertura { get; set; }
		public decimal ValorFechamento { get; set; }
		public decimal ValorMinimo { get; set; }
		public decimal ValorMaximo { get; set; }
		public IList<MediaAbstract> Medias { get; set; }
		public cIFR IFR { get; set; }


	    protected CotacaoAbstract(Ativo pobjAtivo, DateTime pdtmData)
		{
			Ativo = pobjAtivo;
			Data = pdtmData;
			Medias = new List<MediaAbstract>();

		}

		public decimal Amplitude => ValorMaximo - ValorMinimo;

        public override bool Equals(object obj)
		{
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((CotacaoAbstract) obj);
		}

		public decimal CalculaPercentualDoFechamentoEmRelacaoAMedia(MediaDTO pobjMediaDTO)
		{

			decimal decValorMedia = (from x in Medias where x.Tipo == pobjMediaDTO.CampoTipoBd && x.NumPeriodos == pobjMediaDTO.NumPeriodos select Convert.ToDecimal (x.Valor)).Single();

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


			foreach (MediaAbstract objMedia in Medias) {
				objMedia.Valor *= pdblRazao;

			}
			//IFR não precisa converter.

		}

		public IList<MediaDTO> ObtemListaDeMediasDTO()
		{
			return (from m in Medias select m.ObtemDTO()).ToList();
		}

	}

	public class CotacaoDiaria : CotacaoAbstract
	{

		public CotacaoDiaria(Ativo pobjAtivo, System.DateTime pdtmData) : base(pobjAtivo, pdtmData)
		{
		}

		public CotacaoDiaria Clonar()
		{

			var retorno = new CotacaoDiaria(Ativo, Data)
			{
			    Sequencial = Sequencial,
			    ValorAbertura = ValorAbertura,
			    ValorFechamento = ValorFechamento,
			    ValorMinimo = ValorMinimo,
			    ValorMaximo = ValorMaximo
			};

		    retorno.IFR = new cIFRDiario(retorno, IFR.NumPeriodos, IFR.Valor);


			foreach (var media in Medias)
			{
			    var mediaDiaria = (MediaDiaria) media;
			    retorno.Medias.Add(new MediaDiaria(retorno, mediaDiaria.Tipo, mediaDiaria.NumPeriodos, mediaDiaria.Valor));
			}

			return retorno;

		}

	}
}
