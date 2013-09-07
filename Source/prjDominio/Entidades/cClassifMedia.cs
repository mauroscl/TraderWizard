namespace prjDominio.Entidades
{
	public abstract class cClassifMedia
	{

		protected int intID;

		protected string strDescricao;
		public int ID {
			get { return intID; }
		}

		protected string Descricao {
			get { return strDescricao; }
		}

		public override bool Equals(object obj)
		{
			var objClassifMedia = (cClassifMedia)obj;
			return (intID == objClassifMedia.intID);
		}

	}

	//1) primária e secundária de alta alinhada: preço > mme 49 > mme 200
	//2) primária de alta e secundária de baixa: mme 49 > preço > mme 200
	//3) primária de baixa e secundário de alta:  mme 200 > preço > mme 49
	//4) primária e secundária de baixa:  mme200 > mme49 > preço
	//5) primária e secundária de baixa desalinhada: mme 49 > mme 200 > preço 
	//6) primária e secundária de alta desalinhada: preço > mme 200 > mme 49
	//7) indefinida
	public class cClassifMediaAltaAlinhada : cClassifMedia
	{

		public cClassifMediaAltaAlinhada()
		{
			intID = 1;
			strDescricao = "primária e secundária de alta alinhada: preço > mme 49 > mme 200";
		}

	}

	public class cClassifMediaPrimAltaSecBaixa : cClassifMedia
	{

		public cClassifMediaPrimAltaSecBaixa()
		{
			intID = 2;
			strDescricao = "primária de alta e secundária de baixa: mme 49 > preço > mme 200";
		}

	}

	public class cClassifMediaPrimBaixaSecAlta : cClassifMedia
	{

		public cClassifMediaPrimBaixaSecAlta()
		{
			intID = 3;
			strDescricao = "primária de baixa e secundário de alta:  mme 200 > preço > mme 49";
		}

	}

	public class cClassifMediaBaixaAlinhada : cClassifMedia
	{

		public cClassifMediaBaixaAlinhada()
		{
			intID = 4;
			strDescricao = "primária e secundária de baixa:  mme200 > mme49 > preço";
		}

	}

	public class cClassifMediaBaixaDesalinhada : cClassifMedia
	{

		public cClassifMediaBaixaDesalinhada()
		{
			intID = 5;
			strDescricao = "primária e secundária de baixa desalinhada: mme 49 > mme 200 > preço";
		}

	}

	public class cClassifMediaAltaDesalinhada : cClassifMedia
	{

		public cClassifMediaAltaDesalinhada()
		{
			intID = 6;
			strDescricao = "primária e secundária de alta desalinhada: preço > mme 200 > mme 49";
		}

	}


	public class cClassifMediaIndefinida : cClassifMedia
	{

		public cClassifMediaIndefinida()
		{
			intID = 7;
			strDescricao = "Indefinida";
		}

	}
}
