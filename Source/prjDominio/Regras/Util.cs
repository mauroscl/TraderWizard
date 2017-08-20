using System;
using System.Linq;
using Dominio.Entidades;

namespace Dominio.Regras
{

	public class Util
	{

		/// <summary>
		/// Retorna o ID da operação de acordo com o alinhamento das médias móveis de 200, 49 períodos e do valor de fechamento.
		/// </summary>
		/// <param name="pobjCotacao">objeto que contem a cotação e as médias desta cotação</param>
		/// <returns></returns>
		/// <remarks></remarks>
		public static ClassifMedia ClassifMediaCalcular(CotacaoAbstract pobjCotacao)
		{

			//Public Shared Function ClassifMediaCalcular(ByVal pdecValorMM200 As Decimal, ByVal pdecValorMM49 As Decimal _
			//, ByVal pdecPreco As Decimal) As cClassifMedia

			//1)  primária e secundária de alta: preço > mme 49 > mme 200
			//2) primária de alta e secundária de baixa: mme 49 > preço > mme 200
			//3) secundário de alta e primária de baixa:  mme 200 > preço > mme 49
			//4) primária e secundária de baixa:  mme200 > mme49 > preço
			//5) mme 49 > mme 200 > preço 
			//6)  preço > mme 200 > mme 49
			//7) indefinido

			decimal decValorMM49 = (decimal) pobjCotacao.Medias.Single(x => x.NumPeriodos == 49 && x.Tipo == "MME").Valor;
			decimal decValorMM200 = (decimal) pobjCotacao.Medias.Single(x => x.NumPeriodos == 200 && x.Tipo == "MME").Valor;

			if (pobjCotacao.ValorFechamento > decValorMM49 && decValorMM49 > decValorMM200) {
				return new cClassifMediaAltaAlinhada();
			} else if (decValorMM49 > pobjCotacao.ValorFechamento && pobjCotacao.ValorFechamento > decValorMM200) {
				return new cClassifMediaPrimAltaSecBaixa();
			} else if (decValorMM200 > pobjCotacao.ValorFechamento && pobjCotacao.ValorFechamento > decValorMM49) {
				return new cClassifMediaPrimBaixaSecAlta();
			} else if (decValorMM200 > decValorMM49 && decValorMM49 > pobjCotacao.ValorFechamento) {
				return new cClassifMediaBaixaAlinhada();
			} else if (decValorMM49 > decValorMM200 && decValorMM200 > pobjCotacao.ValorFechamento) {
				return new cClassifMediaBaixaDesalinhada();
			} else if (pobjCotacao.ValorFechamento > decValorMM200 && decValorMM200 > decValorMM49) {
				return new cClassifMediaAltaDesalinhada();
			} else {
				return new cClassifMediaIndefinida();
			}

		}

		/// <summary>
		/// Calcula a faixa inferior para um determinado número. A faixa inferior sempre é um inteiro ou um decimal .5
		/// </summary>
		/// <param name="pdblValor">Valor utilizado para estabelecer a faixa</param>
		/// <returns>Um valor que fique abaixo do valor recebido por parâmetro que termine em .0 ou .5 e que a diferença máxima para
		/// o valor recebido por parâmetro fique entre 0.5 e 0.54999....
		/// </returns>
		/// <remarks></remarks>
		public static double PontoInferiorCalcular(double pdblValor)
		{
			double functionReturnValue = 0;

			int intAuxiliar = 0;
			intAuxiliar = (int) Math.Floor(pdblValor);

			if (pdblValor - intAuxiliar >= 0.55) {
				functionReturnValue = intAuxiliar + 0.5;
			} else if (pdblValor - intAuxiliar < 0.05) {
				functionReturnValue = intAuxiliar - 0.5;
			} else {
				functionReturnValue = intAuxiliar;
			}
			return functionReturnValue;

		}

		/// <summary>
		/// Calcula a faixa superior para um determinado número. A faixa superior sempre é um inteiro ou um decimal .5
		/// </summary>
		/// <param name="pdblValor">Valor utilizado para estabelecer a faixa</param>
		/// <returns>Um valor que fique acima do valor recebido por parâmetro que termine em .0 ou .5 e que a diferença máxima para
		/// o valor recebido por parâmetro fique entre 0.05 e 0.54999....
		/// </returns>
		/// <remarks></remarks>
		public static double PontoSuperiorCalcular(double pdblValor)
		{
			double functionReturnValue = 0;

			int intAuxiliar = 0;
			intAuxiliar = (int) Math.Ceiling(pdblValor);

			if (intAuxiliar - pdblValor >= 0.55) {
				functionReturnValue = intAuxiliar - 0.5;
			} else if (intAuxiliar - pdblValor < 0.05) {
				functionReturnValue = intAuxiliar + 0.5;
			} else {
				functionReturnValue = intAuxiliar;
			}
			return functionReturnValue;

		}

	}
}
