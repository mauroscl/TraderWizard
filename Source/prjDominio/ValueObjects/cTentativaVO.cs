namespace prjDominio.ValueObjects
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
