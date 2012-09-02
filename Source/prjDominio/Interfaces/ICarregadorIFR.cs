using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;

namespace prjModelo.Interfaces
{

	public interface ICarregadorIFR
	{

		cIFR CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, int pintNumPeriodos1);

	}

}
