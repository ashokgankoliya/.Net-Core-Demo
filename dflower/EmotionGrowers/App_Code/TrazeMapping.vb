Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Security.Cryptography
Imports System.IO

Public Class TrazeMapping
    Public Function GetSystemTrazeMapping(ByVal whereClause As String, ByVal sortExp As String, ByVal startRowIndex As Integer, ByVal numberOfRows As Integer) As List(Of TRAZEVALUES)

        Dim uList As New List(Of TRAZEVALUES)()
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString

            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetTrazeMapping", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add(New SqlParameter("@WhereClause", whereClause))
                    cmd.Parameters.Add(New SqlParameter("@SortExpression", sortExp))
                    cmd.Parameters.Add(New SqlParameter("@RowIndex", startRowIndex))
                    cmd.Parameters.Add(New SqlParameter("@NoOfRows", numberOfRows))
                    Dim da As New SqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    da.Fill(ds)
                    For Each row In ds.Tables(0).Rows
                        Dim u As New TRAZEVALUES()
                        u.ID = row("ID")
                        u.FLOWER = row("FLOWER")
                        u.UOM = row("UOM")
                        u.PACK = row("PACK")
                        u.PACKUNIT = row("PACKUNIT")
                        u.INNERPACK = row("INNERPACK")
                        u.INNERPACKUNIT = row("INNERPACKUNIT")
                        uList.Add(u)
                    Next
                    Me.TotalRows = ds.Tables(1).Rows(0)(0)
                End Using

            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetSystemTrazeMapping")
            Return Nothing
        End Try
        Return uList
    End Function

    Public Shared Function SaveUpdateTRAZEVALUES(ByVal ID As Integer, ByVal FLOWER As String, ByVal UOM As String, ByVal PACK As Integer, ByVal PACKUNIT As String, ByVal INNERPACK As Integer,
      ByVal INNERPACKUNIT As String) As String
        Try
            Dim UserDetails As New User
            If (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                UserDetails = HttpContext.Current.Session("LoginUserDetails")
            ElseIf (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                UserDetails = HttpContext.Current.Session("LoginUserDetails")
            End If
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim SqlCon As New SqlConnection(ConStr)
            SqlCon.Open()
            Dim TrazeScopeID As String = ""
            Dim Cmd As New SqlCommand("spSaveUpdateTrazeValues", SqlCon)
            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.Parameters.AddWithValue("@ID", ID)
            Cmd.Parameters.AddWithValue("@FLOWER", FLOWER)
            Cmd.Parameters.AddWithValue("@UOM", UOM)
            Cmd.Parameters.AddWithValue("@PACK", PACK)
            Cmd.Parameters.AddWithValue("@PACKUNIT", PACKUNIT)
            Cmd.Parameters.AddWithValue("@INNERPACK", INNERPACK)
            Cmd.Parameters.AddWithValue("@INNERPACKUNIT", INNERPACKUNIT)
            TrazeScopeID = Cmd.ExecuteScalar()

            Cmd.Dispose()
            SqlCon.Close()
            Return TrazeScopeID
        Catch ex As Exception
            If (Not ex.Message.Contains("UNIQUE KEY constraint")) Then
                ErrorLogs.LogException(ex, "Error in SaveUpdateTRAZEVALUES")
            End If
            Return ex.Message.ToString()
        End Try
    End Function

    Public Shared Function DeleteTRAZEVALUESbyID(ByVal ID As String) As String
        Try
            Dim UserDetails As New User
            If (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                UserDetails = HttpContext.Current.Session("LoginUserDetails")
            ElseIf (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                UserDetails = HttpContext.Current.Session("LoginUserDetails")
            End If
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Dim Cmd As New SqlCommand("spDeleteTrazeValues", con)
                Cmd.CommandType = CommandType.StoredProcedure
                Cmd.Parameters.AddWithValue("@ID", ID)
                Cmd.ExecuteNonQuery()
                Return "Success"
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in DeleteTRAZEVALUESbyID")
            Return "error"
        End Try
    End Function

    Public Shared Function GetTRAZEVALUESbyID(ByVal ID As String) As TRAZEVALUES
        Try
            Dim u As New TRAZEVALUES
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetTrazeValuesbyID", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@ID", ID)
                    Dim da As New SqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    da.Fill(ds)
                    For Each row In ds.Tables(0).Rows
                        u.ID = row("ID")
                        u.FLOWER = If(row("FLOWER") Is DBNull.Value, "", row("FLOWER"))
                        u.FLOWERNAME = If(row("FLOWERNAME") Is DBNull.Value, "", row("FLOWERNAME"))
                        u.UOM = If(row("UOM") Is DBNull.Value, "", row("UOM"))
                        u.PACK = If(row("PACK") Is DBNull.Value, "", row("PACK"))
                        u.PACKUNIT = If(row("PACKUNIT") Is DBNull.Value, "", row("PACKUNIT"))
                        u.INNERPACK = If(row("INNERPACK") Is DBNull.Value, "", row("INNERPACK"))
                        u.INNERPACKUNIT = If(row("INNERPACKUNIT") Is DBNull.Value, 0, row("INNERPACKUNIT"))
                    Next
                End Using
            End Using
            Return u
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetTRAZEVALUESbyID")
            Return Nothing
        End Try
    End Function
    Private _TotalRows As Integer
    Public Property TotalRows() As Integer
        Get
            Return _TotalRows
        End Get
        Set(ByVal value As Integer)
            _TotalRows = value
        End Set
    End Property
End Class

Public Class TRAZEVALUES

    Private _ID As String
    Public Property ID() As String
        Get
            Return _ID
        End Get
        Set(value As String)
            _ID = value

        End Set
    End Property


    Private _FLOWER As String
    Public Property FLOWER() As String
        Get
            Return _FLOWER
        End Get
        Set(value As String)
            _FLOWER = value

        End Set
    End Property

    Private _FLOWERNAME As String
    Public Property FLOWERNAME() As String
        Get
            Return _FLOWERNAME
        End Get
        Set(value As String)
            _FLOWERNAME = value
        End Set
    End Property

    Private _UOM As String
    Public Property UOM() As String
        Get
            Return _UOM
        End Get
        Set(value As String)
            _UOM = value

        End Set
    End Property


    Private _PACK As String
    Public Property PACK() As String
        Get
            Return _PACK
        End Get
        Set(value As String)
            _PACK = value

        End Set
    End Property


    Private _PACKUNIT As String
    Public Property PACKUNIT() As String
        Get
            Return _PACKUNIT
        End Get
        Set(value As String)
            _PACKUNIT = value

        End Set
    End Property

    Private _INNERPACK As String
    Public Property INNERPACK() As String
        Get
            Return _INNERPACK
        End Get
        Set(value As String)
            _INNERPACK = value

        End Set
    End Property


    Private _INNERPACKUNIT As String
    Public Property INNERPACKUNIT() As String
        Get
            Return _INNERPACKUNIT
        End Get
        Set(value As String)
            _INNERPACKUNIT = value

        End Set
    End Property
End Class
