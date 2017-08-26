Imports System.Data.SqlClient
Imports System.Data
Imports DevExpress.XtraEditors.Repository
Imports System.Data.OleDb
Imports Microsoft.Office.Interop

Module Funciones
    Public Conexion = "Data Source='192.168.10.223';Initial Catalog=Ventas;Persist Security Info=True;User ID=sa;Password=SO.DEBDC"

    Public Function Gestionar_Formularios(control1 As DevExpress.XtraTabbedMdi.XtraTabbedMdiManager, control2 As DevExpress.XtraEditors.PanelControl) As Boolean
        Try
            Gestionar_Formularios = True
            If control1.Pages.Count = 0 Then
                control2.Visible = True
            Else
                control2.Visible = False
            End If
        Catch ex As Exception
            Gestionar_Formularios = False
            MsgBox(ex.Message)
        End Try
        Return Gestionar_Formularios
    End Function
    Public Function Cargar_Formularios(padre As Form, hijo As Form) As Boolean
        Try
            Cargar_Formularios = True
            With hijo
                .MdiParent = padre
                .Show()
                .Focus()
            End With
        Catch ex As Exception
            Cargar_Formularios = False
            MsgBox(ex.Message)
        End Try
        Return Cargar_Formularios
    End Function
    Public Function Llenar_Combobox(Consulta As String, Campo As String, Control As DevExpress.XtraBars.BarEditItem) As Boolean
        Try
            Llenar_Combobox = True
            Using cnn As New SqlConnection(Conexion)
                cnn.Open()
                Dim enunciado = New SqlCommand(Consulta, cnn)
                Dim respuesta As SqlDataReader
                respuesta = enunciado.ExecuteReader
                While respuesta.Read
                    CType(Control.Edit, RepositoryItemComboBox).Items.Add(respuesta.Item(Campo))
                End While
                respuesta.Close()
            End Using
        Catch ex As Exception
            MsgBox(ex.Message)
            Llenar_Combobox = False
        End Try
        Return Llenar_Combobox
    End Function
    Public Function Obtener_Nombre_Libro(ByVal Ubicacion As String) As String
        Dim app As Excel.Application = Nothing
        Try
            app = New Excel.Application()
            Dim wb As Excel.Workbook = app.Workbooks.Open(Ubicacion)
            Dim ws As Excel.Worksheet = CType(wb.Worksheets.Item(1), Excel.Worksheet)
            Dim name As String = ws.Name
            ws = Nothing
            wb.Close()
            wb = Nothing
            Return name
        Catch ex As Exception
            Throw
        Finally
            If (Not app Is Nothing) Then _
                app.Quit()
            Runtime.InteropServices.Marshal.ReleaseComObject(app)
            app = Nothing
        End Try
    End Function
    Public Function Importar_Doc(Tabla As String, Grid As DataGridView) As Boolean
        Try
            Importar_Doc = True
            Dim myFileDialog As New OpenFileDialog()
            Dim xSheet As String = ""

            With myFileDialog
                .Filter = “Archivos Excel(*.xls;*.xlsx)|*.xls;*xlsx|Todos los archivos(*.*)|*.*”
                .Title = "Open File"
                .ShowDialog()
            End With
            If myFileDialog.FileName.ToString <> "" Then
                Dim ExcelFile As String = myFileDialog.FileName.ToString
                xSheet = Obtener_Nombre_Libro(ExcelFile)
                Dim cadena As String = ("Provider=Microsoft.ACE.OLEDB.12.0;" & ("Data Source=" & (ExcelFile & ";Extended Properties=""Excel 12.0;Xml;HDR=YES;IMEX=2"";")))
                Dim cadenaorigen As OleDbConnection
                cadenaorigen = New OleDbConnection(cadena)
                Dim cmdselect As OleDbCommand
                cmdselect = New OleDbCommand("select * from [" & xSheet & "$]", cadenaorigen)
                Dim adaptador As New OleDbDataAdapter
                adaptador.SelectCommand = cmdselect
                Dim ds As DataSet
                ds = New DataSet
                adaptador.Fill(ds)
                Grid.DataSource = ds.Tables(0)
                cadenaorigen.Close()
                Dim cadenadestino As New SqlConnection
                cadenadestino.ConnectionString = (Conexion)
                cadenadestino.Open()
                Dim importar As SqlBulkCopy
                importar = New SqlBulkCopy(cadenadestino)
                importar.DestinationTableName = "" + Tabla + ""
                importar.WriteToServer(ds.Tables(0))
                cadenadestino.Close()
            End If
        Catch ex As Exception
            Importar_Doc = False
            MsgBox(ex.Message)
        End Try
        Return Importar_Doc
    End Function
End Module
