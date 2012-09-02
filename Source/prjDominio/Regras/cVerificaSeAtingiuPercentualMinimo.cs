using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using prjModelo.Entidades;
using prjModelo.Carregadores;
using DataBase;
using prjModelo.ValueObjects;

namespace prjModelo.Regras
{

	public class cVerificaSeAtingiuPercentualMinimo
	{

		private readonly cConexao objConexao;

		private const double PercentualMinimo = 80.0;
		public cVerificaSeAtingiuPercentualMinimo(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public bool Verificar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{

			cCarregadorDeResumoDoIFRDiario objCarregador = new cCarregadorDeResumoDoIFRDiario(objConexao);

			cIFRSimulacaoDiariaFaixaResumo objResumo = objCarregador.Carregar(pobjSimulacaoDiariaVO);

			if ((objResumo != null)) {
				return (objResumo.PercentualAcertosComFiltro >= PercentualMinimo);
			} else {
				return false;
			}

		}

	}
}
