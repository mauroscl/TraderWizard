using System;
using Dominio.Entidades;
using prjDominio.ValueObjects;

namespace DataBase.Carregadores
{

	public class CarregadorDeResumoDoIFRDiario
	{


		private readonly Conexao _conexao;
		public CarregadorDeResumoDoIFRDiario(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

		public IFRSimulacaoDiariaFaixaResumo Carregar(SimulacaoDiariaVO pobjSimulacaoDiariaVO)
		{

			cRS objRS = new cRS(_conexao);

		    FuncoesBd funcoesBd = _conexao.ObterFormatadorDeCampo();

		    string strSql = " SELECT NumTradesComFiltro, NumAcertosComFiltro, PercentualAcertosComFiltro " + Environment.NewLine;
			strSql += " FROM IFR_Simulacao_Diaria_Faixa_Resumo R1 " + Environment.NewLine;
			strSql += " WHERE Codigo = " + funcoesBd.CampoFormatar(pobjSimulacaoDiariaVO.Ativo.Codigo) + Environment.NewLine;
			strSql += " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjSimulacaoDiariaVO.Setup.Id) + Environment.NewLine;
			strSql += " AND ID_CM = " + funcoesBd.CampoFormatar(pobjSimulacaoDiariaVO.ClassificacaoMedia.ID) + Environment.NewLine;
			strSql += " AND ID_IFR_Sobrevendido = " + funcoesBd.CampoFormatar(pobjSimulacaoDiariaVO.IFRSobrevendido.Id) + Environment.NewLine;
			strSql += " AND Data = ";
			strSql += "( " + Environment.NewLine;
			strSql += '\t' + " SELECT MAX(Data) " + Environment.NewLine;
			strSql += '\t' + " FROM IFR_Simulacao_Diaria_Faixa_Resumo R2 " + Environment.NewLine;
			strSql += '\t' + " WHERE R1.Codigo = R2.Codigo " + Environment.NewLine;
			strSql += '\t' + " AND R1.ID_Setup = R2.ID_Setup " + Environment.NewLine;
			strSql += '\t' + " AND R1.ID_CM = R2.ID_CM " + Environment.NewLine;
			strSql += '\t' + " AND R1.ID_IFR_Sobrevendido = R2.ID_IFR_Sobrevendido " + Environment.NewLine;
			strSql += '\t' + " AND R2.Data <= " + funcoesBd.CampoFormatar(pobjSimulacaoDiariaVO.DataEntradaEfetiva) + Environment.NewLine;
			strSql += ")";


			objRS.ExecuteQuery(strSql);

			IFRSimulacaoDiariaFaixaResumo objRetorno = null;


			if (objRS.DadosExistir) {
			    objRetorno = new IFRSimulacaoDiariaFaixaResumo(pobjSimulacaoDiariaVO.Ativo, pobjSimulacaoDiariaVO.Setup,
			        pobjSimulacaoDiariaVO.ClassificacaoMedia, pobjSimulacaoDiariaVO.IFRSobrevendido,
			        pobjSimulacaoDiariaVO.DataEntradaEfetiva)
			    {
			        NumTradesComFiltro = Convert.ToInt32(objRS.Field("NumTradesComFiltro")),
			        NumAcertosComFiltro = Convert.ToInt32(objRS.Field("NumAcertosComFiltro")),
			        PercentualAcertosComFiltro = Convert.ToDouble(objRS.Field("PercentualAcertosComFiltro"))
			    };


			}

			objRS.Fechar();

			return objRetorno;

		}

	}
}
