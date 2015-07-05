using System;
using DataBase;
using prjDominio.ValueObjects;
using prjModelo.Entidades;

namespace prjModelo.Carregadores
{

	public class cCarregadorDeResumoDoIFRDiario
	{


		private readonly Conexao objConexao;
		public cCarregadorDeResumoDoIFRDiario(Conexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public cIFRSimulacaoDiariaFaixaResumo Carregar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{

			cRS objRS = new cRS(objConexao);

		    FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

		    string strSQL = " SELECT NumTradesComFiltro, NumAcertosComFiltro, PercentualAcertosComFiltro " + Environment.NewLine;
			strSQL += " FROM IFR_Simulacao_Diaria_Faixa_Resumo R1 " + Environment.NewLine;
			strSQL += " WHERE Codigo = " + FuncoesBd.CampoFormatar(pobjSimulacaoDiariaVO.Ativo.Codigo) + Environment.NewLine;
			strSQL += " AND ID_Setup = " + FuncoesBd.CampoFormatar(pobjSimulacaoDiariaVO.Setup.Id) + Environment.NewLine;
			strSQL += " AND ID_CM = " + FuncoesBd.CampoFormatar(pobjSimulacaoDiariaVO.ClassificacaoMedia.ID) + Environment.NewLine;
			strSQL += " AND ID_IFR_Sobrevendido = " + FuncoesBd.CampoFormatar(pobjSimulacaoDiariaVO.IFRSobrevendido.Id) + Environment.NewLine;
			strSQL += " AND Data = ";
			strSQL += "( " + Environment.NewLine;
			strSQL += '\t' + " SELECT MAX(Data) " + Environment.NewLine;
			strSQL += '\t' + " FROM IFR_Simulacao_Diaria_Faixa_Resumo R2 " + Environment.NewLine;
			strSQL += '\t' + " WHERE R1.Codigo = R2.Codigo " + Environment.NewLine;
			strSQL += '\t' + " AND R1.ID_Setup = R2.ID_Setup " + Environment.NewLine;
			strSQL += '\t' + " AND R1.ID_CM = R2.ID_CM " + Environment.NewLine;
			strSQL += '\t' + " AND R1.ID_IFR_Sobrevendido = R2.ID_IFR_Sobrevendido " + Environment.NewLine;
			strSQL += '\t' + " AND R2.Data <= " + FuncoesBd.CampoFormatar(pobjSimulacaoDiariaVO.DataEntradaEfetiva) + Environment.NewLine;
			strSQL += ")";


			objRS.ExecuteQuery(strSQL);

			cIFRSimulacaoDiariaFaixaResumo objRetorno = null;


			if (objRS.DadosExistir) {
				objRetorno = new cIFRSimulacaoDiariaFaixaResumo(pobjSimulacaoDiariaVO.Ativo, pobjSimulacaoDiariaVO.Setup, pobjSimulacaoDiariaVO.ClassificacaoMedia, pobjSimulacaoDiariaVO.IFRSobrevendido, pobjSimulacaoDiariaVO.DataEntradaEfetiva);

				objRetorno.NumTradesComFiltro = Convert.ToInt32(objRS.Field("NumTradesComFiltro"));
				objRetorno.NumAcertosComFiltro = Convert.ToInt32(objRS.Field("NumAcertosComFiltro"));
				objRetorno.PercentualAcertosComFiltro = Convert.ToDouble(objRS.Field("PercentualAcertosComFiltro"));

			}

			objRS.Fechar();

			return objRetorno;

		}

	}
}
