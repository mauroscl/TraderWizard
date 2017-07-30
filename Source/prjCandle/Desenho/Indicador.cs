using System;
using System.Drawing;
using System.Windows.Forms;

namespace prjCandle
{


	public class Indicador
	{

		private PointF[] _arrIndicadorPonto;

	    public int NumPeriodos { get; set; }

	    public Color Cor { get; set; }

	    public PointF[] IndicadorPonto => _arrIndicadorPonto;

	    public int ArrayIndicadorValorIndice { get; private set; }

	    public int ArrayIndicadorPontoIndice { get; private set; }

	    public string Tipo { get; set; }

	    public bool ArrayPontoRedimensionar(int pintTamanho)
		{
			bool functionReturnValue;

			try {
				Array.Resize(ref _arrIndicadorPonto, pintTamanho);

				ArrayIndicadorPontoIndice = pintTamanho - 1;

				functionReturnValue = true;


			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);

				functionReturnValue = false;

			}
			return functionReturnValue;

		}

		/// <summary>
		/// Decrementa o índice do array de valores
		/// </summary>
		/// <remarks></remarks>

		public void ArrayIndicadorValorIndiceDecrementar()
		{
			ArrayIndicadorValorIndice = ArrayIndicadorValorIndice - 1;

		}

		/// <summary>
		/// Decrementa o índice do array de pontos
		/// </summary>
		/// <remarks></remarks>

		public void ArrayIndicadorPontoIndiceDecrementar()
		{
			ArrayIndicadorPontoIndice = ArrayIndicadorPontoIndice - 1;

		}

		/// <summary>
		/// Seta um ponto no indice atual do array de pontos
		/// </summary>
		/// <param name="pobjPonto">Ponto que será setado</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool ArrayIndicadorPontoSetar(PointF pobjPonto)
		{
			try {
				_arrIndicadorPonto[ArrayIndicadorPontoIndice] = pobjPonto;
                return true;            

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

			}
		}


	}
}
