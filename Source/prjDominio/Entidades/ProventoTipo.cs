using System;
using TraderWizard.Enumeracoes;

namespace Dominio.Entidades
{
	public class ProventoTipo
	{

		public ProventoTipo(int pintCodigo, string pstrDescricao)
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
