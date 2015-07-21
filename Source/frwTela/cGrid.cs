using System;
using System.Data;
using System.Data.SqlClient;
using DataBase;
using System.Windows.Forms;
namespace frwTela
{
	public class cGrid
	{
	    private readonly Conexao _conexao;

	    public cGrid(Conexao pobjConexao)
		{
			_conexao = pobjConexao;
		}

	    public string Tabela { get; set; }

	    public string Query { get; set; }

	    public bool Atualizar(DataSet ds)
		{
	        cRS objRS = new cRS(_conexao);
			cColuna objColuna = null;


	        try
	        {
	            //Executa a query

	            objRS.ExecuteQuery(Query);

	            var dbDataReader = objRS.DataReader;

	            //If Not (schemaTable Is Nothing) Then
	            //cria um novo Datatable
	            var dataTable = new DataTable {TableName = this.Tabela};

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

	            //For Each objColuna In pcolColuna


	            int intI;
	            for (intI = 0; intI <= dbDataReader.FieldCount - 1; intI++)
	            {
	                var objDataColumn = new DataColumn
	                {
	                    DataType = dbDataReader.GetFieldType(intI),
	                    ColumnName = dbDataReader.GetName(intI),
	                    ReadOnly = true,
	                    Unique = false
	                };

	                dataTable.Columns.Add(objDataColumn);

	            }

	            // inclui a tabela no DataSet
	            ds.Tables.Add(dataTable);

	            var arrData = new object[dataTable.Columns.Count];

                //var sqlDataReader = (SqlDataReader)objRS.DataReader;


	            // le todas as linhas do DataReader
	            while (!objRS.EOF)
	            {
	                // le a linha do DataReader para um array
	                objRS.GetValues(arrData);


	                //sqlDataReader.GetSqlValues(arrData);

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

	        }
	        catch (Exception e)
	        {
	            MessageBox.Show("Erro: " + e.Message, "Grid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
	            return false;
	        }
	        finally
	        {
	            objRS.Fechar();
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

