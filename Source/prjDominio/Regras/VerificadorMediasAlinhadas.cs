using System.Collections.Generic;
using System.Linq;
using Dominio.Entidades;

namespace Dominio.Regras
{

	public class VerificadorMediasAlinhadas
	{

		public static bool Verificar(ref IList<MediaAbstract> plstMedias)
		{

			IList<MediaAbstract> lstAlinhadaPorNumPeriodos = (from x in plstMedias orderby x.NumPeriodos descending select x).ToList();
			IList<MediaAbstract> lstAlinhadaPorValor = (from x in plstMedias orderby x.Valor select x).ToList();

			return lstAlinhadaPorNumPeriodos.SequenceEqual(lstAlinhadaPorValor);
		}

	}
}
