using System;

namespace Dominio.Entidades
{

	public class cIFRSimulacaoDiariaDetalhe : Modelo
	{


		private readonly IFRSobrevendido _ifrSobrevendido;
		private readonly byte bytNumTentativas;
	    //Private blnGerouEntrada As Boolean 'Indica se gerou entrada pelos filtros adicionais
	    public UInt32 AgrupadorDeTentativas { get; set; }

		private readonly IFRSimulacaoDiaria objIFRSimulacaoDiaria;
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

		public cIFRSimulacaoDiariaDetalhe(IFRSobrevendido pobjIFRSobrevendido, byte pbytNumTentativas, bool pblnMelhorEntrada, int pintSomatorioCriterios, UInt32 pintAgrupadorDeTentativas, IFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{

			_ifrSobrevendido = pobjIFRSobrevendido;
			bytNumTentativas = pbytNumTentativas;
			AgrupadorDeTentativas = pintAgrupadorDeTentativas;

			MelhorEntrada = pblnMelhorEntrada;
			//blnGerouEntrada = pblnGerouEntrada
			SomatorioCriterios = pintSomatorioCriterios;

			objIFRSimulacaoDiaria = pobjIFRSimulacaoDiaria;

		}

	    public cIFRSimulacaoDiariaDetalhe(IFRSobrevendido ifrSobreVendido, byte numTentativas, uint agrupadorDeTentativas, IFRSimulacaoDiaria simulacaoDiaria)
	    {
	        _ifrSobrevendido = ifrSobreVendido;
	        bytNumTentativas = numTentativas;
	        AgrupadorDeTentativas = agrupadorDeTentativas;
            objIFRSimulacaoDiaria = simulacaoDiaria;

	    }


	    public IFRSobrevendido IFRSobreVendido {
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

		public IFRSimulacaoDiaria IFRSimulacaoDiaria {
			get { return objIFRSimulacaoDiaria; }
		}


	}

}
