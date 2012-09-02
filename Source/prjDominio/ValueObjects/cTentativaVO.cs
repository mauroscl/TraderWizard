using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
namespace prjModelo.ValueObjects
{
	public class cTentativaVO
	{
	    public cTentativaVO(uint pintAgrupadorDeTentativas, byte pbytNumTentativas, bool pblnGerouNovoAgrupadorDeTentativas)
		{
			AgrupadorDeTentativas = pintAgrupadorDeTentativas;
			NumTentativas = pbytNumTentativas;
			GerouNovoAgrupadorDeTentativas = pblnGerouNovoAgrupadorDeTentativas;

		}

	    public bool GerouNovoAgrupadorDeTentativas { get; private set; }

	    public byte NumTentativas { get; private set; }

	    public uint AgrupadorDeTentativas { get; private set; }
	}

}
