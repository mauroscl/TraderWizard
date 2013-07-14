using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataBase;
using prjServicoNegocio;
using TesteBase;
namespace TestesQueAlterarBancoDeDados
{

	///<summary>
	///This is a test class for cWebTest and is intended
	///to contain all cWebTest Unit Tests
	///</summary>
	[TestClass()]
	public class teste_da_simulacao_diaria_do_ifr : Inicializacao
	{



		private TestContext testContextInstance;
		///<summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext {
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		#region "Additional test attributes"
		//
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//<ClassInitialize()> _
		//Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
		//End Sub
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//<ClassCleanup()>  _
		//Public Shared Sub MyClassCleanup()
		//End Sub
		//
		//Use TestInitialize to run code before running each test
		[TestInitialize()]
		public void MyTestInitialize()
		{
			Inicializa();
		}

		//Use TestCleanup to run code after each test has run
		[TestCleanup()]
		public void MyTestCleanup()
		{
			Finaliza();
		}
		//
		#endregion


		[TestMethod()]

		public void AtualizarValorOriginalDasSimulacoesExistentes()
		{
			cRS objRS = new cRS(objConexao);

		    string strSQL = "SELECT IFR.CODIGO, Data_Entrada_Efetiva, ID_Setup, ValorFechamento " + Environment.NewLine;
			strSQL = strSQL + " FROM IFR_Simulacao_Diaria IFR INNER JOIN Cotacao C " + Environment.NewLine;
			strSQL = strSQL + " ON IFR.Codigo = C.Codigo " + Environment.NewLine;
			strSQL = strSQL + " AND IFR.Data_Entrada_Efetiva = C.Data " + Environment.NewLine;

			objRS.ExecuteQuery(strSQL);

			cCommand objCommand = new cCommand(objConexao);

            FuncoesBd FuncoesBd = objConexao.ObterFormatadorDeCampo();

			while (!objRS.EOF) {
				strSQL = "UPDATE IFR_Simulacao_Diaria SET " + Environment.NewLine;
				strSQL = strSQL + " Valor_Entrada_Original = " + FuncoesBd.CampoFormatar(Convert.ToDecimal(objRS.Field("ValorFechamento")));
				strSQL = strSQL + " WHERE Codigo = " + FuncoesBd.CampoFormatar(Convert.ToString(objRS.Field("Codigo")));
				strSQL = strSQL + " AND Data_Entrada_Efetiva = " + FuncoesBd.CampoFormatar(Convert.ToDateTime(objRS.Field("Data_Entrada_Efetiva")));
				strSQL = strSQL + " AND ID_Setup = " + FuncoesBd.CampoFormatar(Convert.ToInt16(objRS.Field("ID_Setup")));

			    objCommand.Execute(strSQL);

				objRS.MoveNext();

			}

			objRS.Fechar();

		}


	}
}
