using System;
using TraderWizard.Enumeracoes;

namespace prjDominio.Entidades
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
