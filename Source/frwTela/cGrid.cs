using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;

//Imports Oracle.DataAccess.Client
//Imports System.Data.OracleClient
using System.Data.OleDb;
using DataBase;
using System.Reflection;
using System.Windows.Forms;
namespace frwTela
{
	public class cGrid
	{

		private string strTabela;
		private string strQuery;

		private cConexao objConexao;
		public cGrid(cConexao pobjConexao)
		{
			objConexao = pobjConexao;
		}

		public string Tabela {
			get { return this.strTabela; }
			set { this.strTabela = value; }
		}

		public string Query {
			get { return this.strQuery; }
			set { this.strQuery = value; }
		}

		//Public Function Atualizar(ByVal ds As DataSet, ByVal pcolColuna As Collection) As Boolean

		public bool Atualizar(DataSet ds)
		{

			int intI = 0;

			cRS objRS = new cRS(objConexao);
			cColuna objColuna = null;

			OleDbDataReader objOleDbDataReader = null;


			try {
				//Executa a query

				objRS.ExecuteQuery(strQuery);

				objOleDbDataReader = objRS.GetDataReader;

				// percorre o resultado gerado pelo DataReader

				//Do
				//usa o método GetSchemaTable para retornar informações da tabela (metadados)
				//Dim schemaTable As DataTable = dr.GetSchemaTable()
				DataTable schemaTable = new DataTable(strTabela);

				//If Not (schemaTable Is Nothing) Then
				//cria um novo Datatable
				DataTable dataTable = new DataTable();
				dataTable.TableName = this.Tabela;

				//'cria um DataRow
				//Dim schemaRow As DataRow

				//intI = 1
				//'percorre cada linha do DataTable(schemaTable)
				//For Each schemaRow In schemaTable.Rows
				//    Dim col As New DataColumn
				//    col.ColumnName = pcolColumnName(intI)
				//    col.DataType = CType(schemaRow("DataType"), Type)
				//    ' define o tamanho do campo para strings
				//    If schemaRow("DataType").ToString() = "System.String" Then
				//        col.MaxLength = CType(schemaRow("ColumnSize"), Int32)
				//    End If
				//    'col.Unique = CBool(schemaRow("IsUnique"))
				//    'col.AllowDBNull = CBool(schemaRow("AllowDBNull"))

				//    dataTable.Columns.Add(col)

				//    intI = intI + 1
				//Next schemaRow

				DataColumn objDataColumn = null;

				//For Each objColuna In pcolColuna


				for (intI = 0; intI <= objOleDbDataReader.FieldCount - 1; intI++) {
					objDataColumn = new DataColumn();

					//objDataColumn.DataType = objColuna.Tipo
					//objDataColumn.ColumnName = objColuna.Nome
					//objDataColumn.Caption = objColuna.Caption

					objDataColumn.DataType = objOleDbDataReader.GetFieldType(intI);
					objDataColumn.ColumnName = objOleDbDataReader.GetName(intI);
					//objDataColumn.Caption = pcolColuna(intI + 1)

					objDataColumn.ReadOnly = true;
					objDataColumn.Unique = false;

					dataTable.Columns.Add(objDataColumn);

				}

				// inclui a tabela no DataSet
				ds.Tables.Add(dataTable);

				object[] arrData = new object[dataTable.Columns.Count];

				// le todas as linhas do DataReader
				while (!objRS.EOF) {
					// le a linha do DataReader para um array
					objRS.GetValues(arrData);
					// inclui a linha do array para o DataTable
					dataTable.Rows.Add(arrData);

					objRS.MoveNext();

				}
				//End If
				//Loop While dr.NextResult()

				//conn.Close()

				// nome das tabelas incluidas no DataSet
				ds.Tables[0].TableName = this.Tabela;

				return true;

			} catch (Exception e) {
                MessageBox.Show("Erro: " + e.Message, "Grid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

		}


		public int LinhasContador(DataGrid objDataGrid)
		{

			DataView objDataView = null;
			//Transforma o source do grid em um DataView
			objDataView = (DataView)objDataGrid.DataSource;

			//Retorna o número de registro do DataView, que é o número de linhas do grid.
			return objDataView.Count;

		}

		//-------------------------------------------------------------------------------------------------------
		//Private Sub PreencheDataSetDeDataReader(ByVal ds As DataSet, ByVal dr As IDataReader)

		//    ' Cria um  xxxDataAdapter do mesmo tipo de um DataReader
		//    Dim tipoDataReader As Type = CObj(dr).GetType
		//    Dim nomeTipo As String = tipoDataReader.FullName.Replace("DataReader", "DataAdapter")
		//    Dim tipoDataAdapter As Type = tipoDataReader.Assembly.GetType(nomeTipo)
		//    Dim da As Object = Activator.CreateInstance(tipoDataAdapter)

		//    Dim args() As Object = {ds, Me.Tabela, dr, 0, 999999}

		//    ' invoca o método protegido Fill que toma um objeto IDataReader
		//    tipoDataAdapter.InvokeMember("Fill", BindingFlags.InvokeMethod Or BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, da, args)

		//    ' fecha o DataReader
		//    dr.Close()

		//End Sub

		//Public Sub Preencher(ByRef pobjDataSet As DataSet)

		//    Try

		//        Dim cn As OracleConnection = New OracleConnection("user id=mscl;data source=ORCL;password=mscl")
		//        cn.Open()


		//        Dim cm As New OracleCommand(Me.Query, cn)
		//        Dim dr As OracleDataReader = cm.ExecuteReader()

		//        If pobjDataSet Is Nothing Then
		//            pobjDataSet = New DataSet
		//        End If

		//        PreencheDataSetDeDataReader(pobjDataSet, dr)

		//    Catch e As OracleException
		//        MsgBox(e.Message, MsgBoxStyle.Critical)
		//    Catch e As Exception
		//        MsgBox(e.Message, MsgBoxStyle.Critical)
		//    End Try

		//End Sub

	}
}

