using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo.Entidades
{
	public class cCarteira
	{

		private readonly int intIdCarteira;
		private readonly string strDescricao;
		private readonly cIFRSobrevendido objIFRSobrevendido;
		private readonly bool blnAtivo;
		private readonly System.DateTime dtmDataInicio;
		private readonly System.DateTime? dtmDataFim;

		private readonly IList<cCarteiraAtivo> lstCarteiraAtivos;

        public cCarteira(int pintIdCarteira, string pstrDescricao, cIFRSobrevendido pobjIFRSobrevendido, bool pblnAtivo, DateTime pdtmDataInicio)
        {
            intIdCarteira = pintIdCarteira;
            strDescricao = pstrDescricao;
            objIFRSobrevendido = pobjIFRSobrevendido;
            blnAtivo = pblnAtivo;
            dtmDataInicio = pdtmDataInicio;

            lstCarteiraAtivos = new List<cCarteiraAtivo>();

        }


		public cCarteira(int pintIdCarteira, string pstrDescricao, cIFRSobrevendido pobjIFRSobrevendido, bool pblnAtivo, DateTime pdtmDataInicio, DateTime pdtmDataFim)
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

		public cIFRSobrevendido IFRSobrevendido {
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

		public void AdicionaAtivo(cAtivo pobjAtivo)
		{
			var objCarteiraAtivo = new cCarteiraAtivo(this, pobjAtivo);
			lstCarteiraAtivos.Add(objCarteiraAtivo);
		}

		public bool AtivoEstaNaCarteira(cAtivo pobjAtivo)
		{

			return lstCarteiraAtivos.Where(a => a.Ativo.Equals(pobjAtivo)).Count() > 0;

		}

	}

	public class cCarteiraAtivo
	{

		private readonly cCarteira objCarteira;

		private readonly cAtivo objAtivo;
		public cCarteiraAtivo(cCarteira pobjCarteira, cAtivo pobjAtivo)
		{
			objCarteira = pobjCarteira;
			objAtivo = pobjAtivo;
		}

		public cAtivo Ativo {
			get { return objAtivo; }
		}

		public cCarteira Carteira {
			get { return objCarteira; }
		}
	}

}
