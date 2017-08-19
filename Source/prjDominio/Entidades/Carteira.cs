using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades
{
	public class Carteira
	{

		private readonly int intIdCarteira;
		private readonly string strDescricao;
		private readonly IFRSobrevendido objIFRSobrevendido;
		private readonly bool blnAtivo;
		private readonly System.DateTime dtmDataInicio;
		private readonly System.DateTime? dtmDataFim;

		private readonly IList<cCarteiraAtivo> lstCarteiraAtivos;

        public Carteira(int pintIdCarteira, string pstrDescricao, IFRSobrevendido pobjIFRSobrevendido, bool pblnAtivo, DateTime pdtmDataInicio)
        {
            intIdCarteira = pintIdCarteira;
            strDescricao = pstrDescricao;
            objIFRSobrevendido = pobjIFRSobrevendido;
            blnAtivo = pblnAtivo;
            dtmDataInicio = pdtmDataInicio;

            lstCarteiraAtivos = new List<cCarteiraAtivo>();

        }


		public Carteira(int pintIdCarteira, string pstrDescricao, IFRSobrevendido pobjIFRSobrevendido, bool pblnAtivo, DateTime pdtmDataInicio, DateTime pdtmDataFim)
            :this(pintIdCarteira,pstrDescricao,pobjIFRSobrevendido,pblnAtivo,pdtmDataInicio)
		{
			dtmDataFim = pdtmDataFim;
		}

		public System.DateTime? DataFim {
			get { return dtmDataFim; }
		}

		public System.DateTime DataInicio {
			get { return dtmDataInicio; }
		}

		public bool Ativo {
			get { return blnAtivo; }
		}

		public IFRSobrevendido IFRSobrevendido {
			get { return objIFRSobrevendido; }
		}

		public string Descricao {
			get { return strDescricao; }
		}

		public int IdCarteira {
			get { return intIdCarteira; }
		}

		public IList<cCarteiraAtivo> Ativos {
			get { return lstCarteiraAtivos; }
		}

		public void AdicionaAtivo(Ativo pobjAtivo)
		{
			var objCarteiraAtivo = new cCarteiraAtivo(this, pobjAtivo);
			lstCarteiraAtivos.Add(objCarteiraAtivo);
		}

		public bool AtivoEstaNaCarteira(Ativo pobjAtivo)
		{

			return lstCarteiraAtivos.Where(a => a.Ativo.Equals(pobjAtivo)).Count() > 0;

		}

	}

	public class cCarteiraAtivo
	{

		private readonly Carteira objCarteira;

		private readonly Ativo objAtivo;
		public cCarteiraAtivo(Carteira pobjCarteira, Ativo pobjAtivo)
		{
			objCarteira = pobjCarteira;
			objAtivo = pobjAtivo;
		}

		public Ativo Ativo {
			get { return objAtivo; }
		}

		public Carteira Carteira {
			get { return objCarteira; }
		}
	}

}
