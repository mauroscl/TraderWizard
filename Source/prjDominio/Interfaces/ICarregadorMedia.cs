using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using prjDTO;

namespace prjModelo.Interfaces
{

	public interface ICarregadorMedia
	{

		cMediaAbstract CarregarPorData(cCotacaoDiaria pobjCotacaoDiaria, cMediaDTO pobjMediaDto1);

	}

}
