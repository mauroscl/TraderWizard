using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using TraderWizard.Enumeracoes;

namespace prjModelo.Entidades
{
	public class cProventoTipo
	{

		public cProventoTipo(int pintCodigo, string pstrDescricao)
		{
			Codigo = pintCodigo;
			strDescricao = pstrDescricao;
		}

	    private readonly string strDescricao;
	    public int Codigo { get; private set; }
        public cEnum.enumProventoTipo GetEnumProventoTipo { get {return (cEnum.enumProventoTipo) Enum.Parse(typeof (cEnum.enumProventoTipo), Convert.ToString(Codigo)); } }

	    public override string ToString()
		{
			return strDescricao;
		}

	}
}
