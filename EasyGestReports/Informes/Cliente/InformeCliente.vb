Namespace EasyGestReports.Informes
    Partial Public Class InformeCliente
        Inherits Telerik.Reporting.Report
        Implements IEasyGestReport

        Public Sub New()
            InitializeComponent()
            EasyGestControllers.Controller.EasyGestController.ResetConnectionString(My.MySettings.Default.EasyGest)
            Using c As New EasyGestControllers.Controller.VistasController(Of EasyGestControllers.Data.Entity.VWClientes)
                DSClientes.DataSource = c.GetItems()
            End Using
        End Sub

        Private Sub InformeCliente_NeedDataSource(sender As Object, e As EventArgs) Handles Me.NeedDataSource
        End Sub

        Public Sub Configurar() Implements IEasyGestReport.Configurar
            Throw New NotImplementedException()
        End Sub


    End Class
End Namespace