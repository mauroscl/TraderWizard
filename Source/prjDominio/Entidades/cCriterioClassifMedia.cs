namespace prjDominio.Entidades
{

	public abstract class cCriterioClassifMedia
	{

		public cCriterioClassifMedia(int pintID, string pstrDescricao)
		{
			intID = pintID;
			strDescricao = pstrDescricao;
		}

		protected int intID;

		protected string strDescricao;
		public int ID {
			get { return intID; }
		}

		protected string Descricao {
			get { return strDescricao; }
		}

		public abstract string CampoBD { get; }

		public abstract string AliasBD { get; }

		public abstract string CampoRelatorio { get; }

		public abstract int Peso { get; }

	}

	public class cCriterioClassifMediaMM21 : cCriterioClassifMedia
	{

		public cCriterioClassifMediaMM21() : base(1, "Percentual do preço em relação a média móvel de 21 períodos")
		{
		}

		public override string CampoBD {
			get { return "Percentual_MME21"; }
		}

		public override string AliasBD {
			get { return "Percentual_MME21"; }
		}

		public override string CampoRelatorio {
			get { return "Percentual_MM21"; }
		}
		public override int Peso {
			get { return 1; }
		}

	}


	public class cCriterioClassifMediaMM49 : cCriterioClassifMedia
	{

		public cCriterioClassifMediaMM49() : base(2, "Percentual do preço em relação a média móvel de 49 períodos")
		{
		}

		public override string CampoBD {
			get { return "Percentual_MME49"; }
		}

		public override string AliasBD {
			get { return "Percentual_MME49"; }
		}

		public override string CampoRelatorio {
			get { return "Percentual_MM49"; }
		}

		public override int Peso {
			get { return 2; }
		}

	}


	public class cCriterioClassifMediaMM200 : cCriterioClassifMedia
	{

		public cCriterioClassifMediaMM200() : base(3, "Percentual do preço em relação a média móvel de 200 períodos")
		{
		}

		public override string CampoBD {
			get { return "Percentual_MME200"; }
		}

		public override string AliasBD {
			get { return "Percentual_MME200"; }
		}

		public override string CampoRelatorio {
			get { return "Percentual_MM200"; }
		}

		public override int Peso {
			get { return 4; }
		}

	}

	public class cCriterioClassifMediaDifMM200MM21 : cCriterioClassifMedia
	{

		public cCriterioClassifMediaDifMM200MM21() : base(4, "Percentual MM 200 - Percentual MM 21")
		{
		}

		public override string CampoBD {
			get { return "Percentual_MME200 - Percentual_MME21"; }
		}

		public override string AliasBD {
			get { return "Diferenca_MM200_MM21"; }
		}

		public override string CampoRelatorio {
			get { return "Diferenca_MM200_MM21"; }
		}


		public override int Peso {
			get { return 8; }
		}


	}

	public class cCriterioClassifMediaDifMM200MM49 : cCriterioClassifMedia
	{

		public cCriterioClassifMediaDifMM200MM49() : base(5, "Percentual MM 200 - Percentual MM 49")
		{
		}

		public override string CampoBD {
			get { return "Percentual_MME200 - Percentual_MME49"; }
		}

		public override string AliasBD {
			get { return "Diferenca_MM200_MM49"; }
		}

		public override string CampoRelatorio {
			get { return "Diferenca_MM200_MM49"; }
		}


		public override int Peso {
			get { return 16; }
		}

	}

}
