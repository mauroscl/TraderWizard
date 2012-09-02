using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using prjModelo.ValueObjects;

namespace prjModelo.VOBuilders
{

	public class cSimulacaoDiariaVOBuilder
	{

		public SimulacaoDiariaVO Build(cIFRSimulacaoDiariaDetalhe pobjSimulacaoDetalhe)
		{

			var objSimulacaoDiariaVO = new SimulacaoDiariaVO();

			objSimulacaoDiariaVO.Setup = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.Setup;
			objSimulacaoDiariaVO.Ativo = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.Ativo;
			objSimulacaoDiariaVO.ClassificacaoMedia = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.ClassificacaoMedia;
			objSimulacaoDiariaVO.DataEntradaEfetiva = pobjSimulacaoDetalhe.IFRSimulacaoDiaria.DataEntradaEfetiva;
			objSimulacaoDiariaVO.IFRSobrevendido = pobjSimulacaoDetalhe.IFRSobreVendido;
			objSimulacaoDiariaVO.NumTentativas = pobjSimulacaoDetalhe.NumTentativas;

			return objSimulacaoDiariaVO;

		}


	}
}
