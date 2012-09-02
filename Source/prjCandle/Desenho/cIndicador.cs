using System;
using System.Drawing;
using System.Windows.Forms;
namespace prjCandle
{

	public class cIndicador
	{

		//número de períodos utilizados para calcular o indicador

		private int intNumPeriodos;
			//cor da linha que imprime a média
		private System.Drawing.Color objCor;

		//Private arrIndicadorValor As Double() 'array contendo os valores do indicador

			//array contendo os pontos de impressão do indicador.
		private PointF[] arrIndicadorPonto;

			//contem o índice atual do array de valores. Utilizado nos loops.
		private int intArrayIndicadorValorIndice;

			//contem o índice atual do array de pontos. Utilizado nos loops.
		private int intArrayIndicadorPontoIndice;

			//utilizado para médias. E = EXPONENCIAL; A = ARITMÉTICA
		private string strTipo;

		public int NumPeriodos {
			get { return intNumPeriodos; }
			set { intNumPeriodos = value; }
		}

		public Color Cor {
			get { return objCor; }
			set { objCor = value; }
		}

		//Public WriteOnly Property IndicadorValor() As Double()
		//    Set(ByVal value As Double())
		//        arrIndicadorValor = value
		//        intArrayIndicadorValorIndice = arrIndicadorValor.Length - 1
		//    End Set
		//End Property

		public PointF[] IndicadorPonto {
			get { return arrIndicadorPonto; }
		}

		public int ArrayIndicadorValorIndice {
			get { return intArrayIndicadorValorIndice; }
		}

		public int ArrayIndicadorPontoIndice {
			get { return intArrayIndicadorPontoIndice; }
		}

		public string Tipo {
			get { return strTipo; }
			set { strTipo = value; }
		}

		//Public Function ArrayValorRedimensionar(ByVal pintTamanho As Integer) As Boolean

		//    Try

		//        Array.Resize(arrIndicadorValor, pintTamanho)

		//        ArrayValorRedimensionar = True

		//    Catch ex As Exception

		//        MsgBox(ex.Message, MsgBoxStyle.Critical, "Trader Wizard")

		//        ArrayValorRedimensionar = False

		//    End Try

		//End Function

		public bool ArrayPontoRedimensionar(int pintTamanho)
		{
			bool functionReturnValue = false;


			try {
				Array.Resize(ref arrIndicadorPonto, pintTamanho);

				intArrayIndicadorPontoIndice = pintTamanho - 1;

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
			intArrayIndicadorValorIndice = intArrayIndicadorValorIndice - 1;

		}

		/// <summary>
		/// Decrementa o índice do array de pontos
		/// </summary>
		/// <remarks></remarks>

		public void ArrayIndicadorPontoIndiceDecrementar()
		{
			intArrayIndicadorPontoIndice = intArrayIndicadorPontoIndice - 1;

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
				arrIndicadorPonto[intArrayIndicadorPontoIndice] = pobjPonto;
                return true;            

			} catch (Exception ex) {
                MessageBox.Show(ex.Message, "Trader Wizard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;

			}
		}

		///' <summary>
		///' Obtém o valor na posição do índice atual do array de valores.
		///' </summary>
		///' <returns></returns>
		///' <remarks></remarks>
		//Public Function ArrayIndicadorValorBuscar() As Double

		//    Try

		//        ArrayIndicadorValorBuscar = arrIndicadorValor(intArrayIndicadorValorIndice)

		//    Catch ex As Exception

		//        MsgBox(ex.Message, MsgBoxStyle.Critical, "Trader Wizard")

		//        ArrayIndicadorValorBuscar = 0

		//    End Try

		//End Function

	}
}
