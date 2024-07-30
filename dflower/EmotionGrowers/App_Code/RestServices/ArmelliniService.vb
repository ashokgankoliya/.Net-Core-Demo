Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports RestSharp

Public Class ArmelliniService
    Private Const V As Boolean = False

    Public Shared Function GetToken() As String
        Try
            Dim client = New RestClient("https://armellini.auth0.com/oauth/token")
            'client.Options.
            'client.Timeout = -1
            Dim request = New RestRequest()
            request.AddHeader("Content-Type", "application/json")
            Dim body = "{""client_id"": """ + ConfigurationManager.AppSettings("ArmelliniClientID").ToString() + """, ""client_secret"": """ + ConfigurationManager.AppSettings("ArmelliniClientSecret").ToString() + """, ""audience"": ""https://aelis4api.armellini.com/api/"",""grant_type"":""client_credentials""}"
            request.AddJsonBody(body)
            Dim response As RestResponse = client.PostAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of ArmelliniTokenResponse)(response.Content)
            Return resultObj.access_token
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService GetToken")
        End Try
        Return String.Empty
    End Function


    Public Shared Function IsDayOfService(ByVal order As String) As Boolean
        Try
            Dim shipperAccountNumber As String, consigneeAccountNumber As String, orderDate As DateTime
            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetShipperConsigneeDateFromOrder", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("order", order)
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            shipperAccountNumber = dt.Rows(0)(1).ToString()
            consigneeAccountNumber = dt.Rows(0)(0).ToString()
            orderDate = dt.Rows(0)(2).ToString()
            Dim token As String = GetToken()
            If String.IsNullOrEmpty(token) Then
                Throw New Exception("Invalid Token")
            End If
            Dim client = New RestClient(ConfigurationManager.AppSettings("ArmelliniAPIUrl").ToString() + "accounts/days-of-service?shipperAccountNumber=" + shipperAccountNumber + "&consigneeAccountNumber=" + consigneeAccountNumber + "")
            Dim request = New RestRequest()
            request.AddHeader("Authorization", "Bearer " + token + "")
            Dim response As RestResponse = client.GetAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of List(Of ServiceDays))(response.Content)
            If resultObj Is Nothing Then
                Return V
            End If
            For Each availableDate In resultObj
                If availableDate.shipDate = orderDate Then
                    Return True
                End If
            Next
            Return V
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService IsDayOfService")
            Return V
        End Try

    End Function

    Public Shared Function GetDayOfService(ByVal consigneeAccountNumber As String) As List(Of ServiceDays)
        Try
            Dim shipperAccountNumber As String
            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("select top 1 ARMELLINI from constant", con)
                    cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            shipperAccountNumber = dt.Rows(0)(0).ToString()
            Dim token As String = GetToken()
            If String.IsNullOrEmpty(token) Then
                Throw New Exception("Invalid Token")
            End If
            Dim client = New RestClient(ConfigurationManager.AppSettings("ArmelliniAPIUrl").ToString() + "accounts/days-of-service?shipperAccountNumber=" + shipperAccountNumber + "&consigneeAccountNumber=" + consigneeAccountNumber + "")
            Dim request = New RestRequest()
            request.AddHeader("Authorization", "Bearer " + token + "")
            Dim response As RestResponse = client.GetAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of List(Of ServiceDays))(response.Content)
            If resultObj Is Nothing Then
                Return New List(Of ServiceDays)
            End If
            Return resultObj
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService GetDayOfService")
            Throw
        End Try
        Return New List(Of ServiceDays)
    End Function

    Public Shared Function SaveDayOfService() As List(Of ServiceDays)
        Try
            Dim shipperAccountNumber As String = GetShipper()
            If shipperAccountNumber Is "" Then
                Return New List(Of ServiceDays)
            End If
            Dim token As String = GetToken()
            If String.IsNullOrEmpty(token) Then
                Throw New Exception("Invalid Token")
            End If
            Dim client = New RestClient(ConfigurationManager.AppSettings("ArmelliniAPIUrl").ToString() + "accounts/days-of-service?shipperAccountNumber=" + shipperAccountNumber)
            Dim request = New RestRequest()
            request.AddHeader("Authorization", "Bearer " + token + "")
            Dim response As RestResponse = client.GetAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of List(Of ServiceDays))(response.Content)
            If resultObj Is Nothing Then
                Return New List(Of ServiceDays)
            End If
            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("truncate table tblDaysOfService", con)
                    cmd.ExecuteNonQuery()
                End Using
                For Each dayOfService In resultObj
                    Using cmd As New SqlCommand("spInsertDayOfService", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("shipDate", dayOfService.shipDate)
                        cmd.Parameters.AddWithValue("holiday", dayOfService.holiday)
                        cmd.Parameters.AddWithValue("consigneeAccountNumber", dayOfService.consigneeAccountNumber)
                        cmd.ExecuteNonQuery()
                    End Using
                Next
            End Using
            Return resultObj
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService SaveDayOfService")
            Throw
        End Try
        Return New List(Of ServiceDays)
    End Function

    Public Shared Function NeedUpdate() As Boolean
        Try

            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetTblDaysOfServiceTop1", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                    Return Not (dt.Rows.Count > 0 AndAlso DateDiff(DateInterval.Day, dt.Rows(0)(0), Today.Date) > 14)
                End Using
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService NeedUpdate")
            Throw
        End Try
    End Function

    Public Shared Function GetDayOfService() As List(Of ServiceDays)
        Try
            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetTblDaysOfService", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            If dt.Rows.Count > 0 Then
                Dim resultObj = New List(Of ServiceDays)
                For Each row In dt.Rows
                    Dim dayOfService As New ServiceDays With {
                        .shipDate = row(1),
                        .holiday = row(2),
                        .consigneeAccountNumber = row(3)
                    }

                    resultObj.Add(dayOfService)
                Next
                Return resultObj
            End If
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService GetDayOfService")
            Throw
        End Try

    End Function

    Public Shared Function GetShipper() As String
        Try
            Dim dt As New DataTable
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("select top 1 ARMELLINI from constant", con)
                    cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(dt)
                End Using
            End Using
            Return dt.Rows(0)(0).ToString()
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ArmelliniService GetShipper")
            Throw
        End Try
    End Function

End Class

Public Class ArmelliniTokenResponse
    Public Property access_token As String
    Public Property scope As String
    Public Property expires_in As String
    Public Property token_type As String
End Class

Public Class ServiceDays
    Public Property shipDate As String
    Public Property regionCode As String
    Public Property warehouseCode As String
    Public Property holiday As Boolean
    Public Property consigneeAccountNumber As String
End Class
