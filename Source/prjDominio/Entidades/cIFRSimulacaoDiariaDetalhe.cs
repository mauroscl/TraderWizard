using System;
using prjDominio.Entidades;

namespace prjModelo.Entidades
{

	public class cIFRSimulacaoDiariaDetalhe : cModelo
	{


		private readonly cIFRSobrevendido _ifrSobrevendido;
		private readonly byte bytNumTentativas;
	    //Private blnGerouEntrada As Boolean 'Indica se gerou entrada pelos filtros adicionais
	    public UInt32 AgrupadorDeTentativas { get; set; }

		private readonly cIFRSimulacaoDiaria objIFRSimulacaoDiaria;
		/// <summary>
		/// Construtor utilizado quando é necessário criar um objeto para representar um registro já persistido
		/// </summary>
		/// <param name="pobjIFRSobrevendido"></param>
		/// <param name="pbytNumTentativas"></param>
		/// <param name="pblnMelhorEntrada"></param>
		/// <param name="pintSomatorioCriterios"></param>
		/// <param name="pintAgrupadorDeTentativas"></param>
		/// <param name="pobjIFRSimulacaoDiaria"></param>
		/// <remarks></remarks>

		public cIFRSimulacaoDiariaDetalhe(cIFRSobrevendido pobjIFRSobrevendido, byte pbytNumTentativas, bool pblnMelhorEntrada, int pintSomatorioCriterios, UInt32 pintAgrupadorDeTentativas, cIFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{

			_ifrSobrevendido = pobjIFRSobrevendido;
			bytNumTentativas = pbytNumTentativas;
			AgrupadorDeTentativas = pintAgrupadorDeTentativas;

			MelhorEntrada = pblnMelhorEntrada;
			//blnGerouEntrada = pblnGerouEntrada
			SomatorioCriterios = pintSomatorioCriterios;

			objIFRSimulacaoDiaria = pobjIFRSimulacaoDiaria;

		}

	    public cIFRSimulacaoDiariaDetalhe(cIFRSobrevendido ifrSobreVendido, byte numTentativas, uint agrupadorDeTentativas, cIFRSimulacaoDiaria simulacaoDiaria)
	    {
	        _ifrSobrevendido = ifrSobreVendido;
	        bytNumTentativas = numTentativas;
	        AgrupadorDeTentativas = agrupadorDeTentativas;
            objIFRSimulacaoDiaria = simulacaoDiaria;

	    }


	    public cIFRSobrevendido IFRSobreVendido {
			get { return _ifrSobrevendido; }
		}

		public byte NumTentativas {
			get { return bytNumTentativas; }
		}

	    public bool MelhorEntrada { get; private set; }

	    public bool GerouEntrada {
			get { return (SomatorioCriterios == 0); }
		}

	    public int SomatorioCriterios { get; private set; }

	    public void AlterarMelhorEntrada(bool pblnValor)
		{
			MelhorEntrada = pblnValor;
		}

	    public void AlterarSomatorioDeCriterios(int somatorioDeCriterios)
	    {
	        SomatorioCriterios = somatorioDeCriterios;
	    }

		public cIFRSimulacaoDiaria IFRSimulacaoDiaria {
			get { return objIFRSimulacaoDiaria; }
		}


	}

}
