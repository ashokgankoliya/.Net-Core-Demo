Imports System.Data.SqlClient
Imports System.Data


Namespace DAO

    Public Class InvoiceHistory

        Public Shared Function SaveInvoiceHistory(ByVal ORDER As String, ByVal CUSTOMER As String, ByVal Note As String, ByVal USER As String) As String
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString

                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("spF_IHistoryInsert", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@ORDER", ORDER)
                        cmd.Parameters.AddWithValue("@CUSTOMER", CUSTOMER)
                        cmd.Parameters.AddWithValue("@PERSON", USER)
                        cmd.Parameters.AddWithValue("@Note", Note)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in SaveInvoiceHistory")
                Return "error"
            End Try
            Return Nothing
        End Function

    End Class

End Namespace