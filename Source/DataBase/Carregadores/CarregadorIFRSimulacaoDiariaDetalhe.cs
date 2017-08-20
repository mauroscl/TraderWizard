using System;
using System.Collections.Generic;
using Dominio.Entidades;

namespace DataBase.Carregadores
{
	public class CarregadorIFRSimulacaoDiariaDetalhe
	{

		private Conexao Conexao { get; }

		public CarregadorIFRSimulacaoDiariaDetalhe(Conexao pobjConexao)
		{
			Conexao = pobjConexao;
		}
		public IList<cIFRSimulacaoDiariaDetalhe> CarregarTodosDeUmaSimulacao(IFRSimulacaoDiaria pobjIFRSimulacaoDiaria)
		{
            FuncoesBd funcoesBd = Conexao.ObterFormatadorDeCampo();

		    string strSql = "SELECT ID_IFR_Sobrevendido, NumTentativas, MelhorEntrada, Data_Entrada_Efetiva, SomatorioCriterios, AgrupadorTentativas " + Environment.NewLine;
			strSql = strSql + " FROM IFR_Simulacao_Diaria_Detalhe " + Environment.NewLine;
			strSql = strSql + " WHERE Codigo = " + funcoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.Ativo.Codigo) + Environment.NewLine;
			strSql = strSql + " AND ID_Setup = " + funcoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.Setup.Id) + Environment.NewLine;
			strSql = strSql + " AND Data_Entrada_Efetiva = " + funcoesBd.CampoFormatar(pobjIFRSimulacaoDiaria.DataEntradaEfetiva);

			RS objRS = new RS(Conexao);

			List<cIFRSimulacaoDiariaDetalhe> lstRetorno = new List<cIFRSimulacaoDiariaDetalhe>();

			objRS.ExecuteQuery(strSql);

			CarregadorIFRSobrevendido objCarregadorIFRSobrevendido = new CarregadorIFRSobrevendido(Conexao);


			while (!objRS.Eof) {
				lstRetorno.Add(new cIFRSimulacaoDiariaDetalhe(objCarregadorIFRSobrevendido.CarregaPorID(Convert.ToInt16(objRS.Field("ID_IFR_Sobrevendido"))), Convert.ToByte(objRS.Field("NumTentativas")), Convert.ToBoolean(objRS.Field("MelhorEntrada")), Convert.ToInt16(objRS.Field("SomatorioCriterios")), Convert.ToUInt32(objRS.Field("AgrupadorTentativas")), pobjIFRSimulacaoDiaria));

				objRS.MoveNext();

			}

			objRS.Fechar();

			return lstRetorno;

		}

	}
}
