using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;

namespace prjModelo.Regras
{

	public class cVerificadorMediasAlinhadas
	{

		public static bool Verificar(ref IList<cMediaAbstract> plstMedias)
		{

			IList<cMediaAbstract> lstAlinhadaPorNumPeriodos = (from x in plstMedias orderby x.NumPeriodos descending select x).ToList();
			IList<cMediaAbstract> lstAlinhadaPorValor = (from x in plstMedias orderby x.Valor select x).ToList();

			return lstAlinhadaPorNumPeriodos.SequenceEqual(lstAlinhadaPorValor);
		}

	}
}
