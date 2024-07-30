Imports System.Data
Imports System.Data.SqlClient
Imports System.Net
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports RestSharp

Public Class FloridaBeautyService
    Public Shared Function GetDayOfService(ByVal consigneeAccountNumber As String, ByVal shipDate As String) As FBResponse
        Try
            'Dim client = New RestClient(ConfigurationManager.AppSettings("FloridaBeautyRestAPIURL").ToString() + "day_of_service/validate?shipDate=" + shipDate + "&consigneeId=" + consigneeAccountNumber + "&shipperId=" + ConfigurationManager.AppSettings("FloridaBeautyShipper").ToString() + "")
            'Dim request = New RestRequest(Method.[Get])
            'Dim svcCredentials As String = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings("FloridaBeautyUserName").ToString() + ":" + ConfigurationManager.AppSettings("FloridaBeautyPassword").ToString()))
            'request.AddHeader("Authorization", "Basic " + svcCredentials + "")
            'request.AddHeader("Content-Type", "application/json")
            'System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            'System.Net.ServicePointManager.ServerCertificateValidationCallback = Function() True
            'Dim response As RestResponse = client.Execute(request)
            Dim client = New RestClient(ConfigurationManager.AppSettings("FloridaBeautyRestAPIURL").ToString() + "day_of_service/validate?shipDate=" + shipDate + "&consigneeId=" + consigneeAccountNumber + "&shipperId=" + ConfigurationManager.AppSettings("FloridaBeautyShipper").ToString() + "")
            Dim request = New RestRequest()
            Dim svcCredentials As String = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings("FloridaBeautyUserName").ToString() + ":" + ConfigurationManager.AppSettings("FloridaBeautyPassword").ToString()))
            request.AddHeader("Authorization", "Basic " + svcCredentials + "")
            Dim response As RestResponse = client.GetAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of FBResponse)(response.Content)
            If resultObj Is Nothing Then
                Return New FBResponse
            End If
            Return resultObj
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In FloridaBeautyService GetDayOfService")
            'Throw
        End Try
        Return New FBResponse
    End Function
    Public Shared Function GetDayOfService() As List(Of FBResponse)
        Try
            Dim client = New RestClient(ConfigurationManager.AppSettings("FloridaBeautyRestAPIURL").ToString() + "day_of_service/shipper/consignees?shipDate=" + DateTime.Today.ToString("yyyy-MM-dd") + "&shipperId=" + ConfigurationManager.AppSettings("FloridaBeautyShipper").ToString() + "")
            Dim request = New RestRequest()
            Dim svcCredentials As String = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(ConfigurationManager.AppSettings("FloridaBeautyUserName").ToString() + ":" + ConfigurationManager.AppSettings("FloridaBeautyPassword").ToString()))
            request.AddHeader("Authorization", "Basic " + svcCredentials + "")
            Dim response As RestResponse = client.GetAsync(request).Result
            Dim resultObj = JsonConvert.DeserializeObject(Of List(Of FBResponse))(response.Content)
            If resultObj Is Nothing Then
                Return New List(Of FBResponse)
            End If
            Return resultObj
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In FloridaBeautyService GetDayOfService")
            'Throw
        End Try
        Return New List(Of FBResponse)
    End Function

    Public Shared Function SaveDayOfService(values As List(Of FBResponse)) As Boolean
        Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
        Using con As New SqlConnection(ConStr)
            con.Open()
            For Each consignee In values
                Dim results = (From c In consignee.serviceDays Where c.hasService Select c.dayOfWeek).ToList()
                Dim shipDate As DateTime = DateTime.Parse(consignee.shipDate)
                For x = 0 To 20
                    If results.Contains(shipDate.ToString("dddd").ToUpper()) Then
                        Using cmd As New SqlCommand("spInsertDayOfService", con)
                            cmd.CommandType = CommandType.StoredProcedure
                            cmd.Parameters.AddWithValue("shipDate", shipDate)
                            cmd.Parameters.AddWithValue("holiday", False)
                            cmd.Parameters.AddWithValue("consigneeAccountNumber", consignee.consigneeId)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                    shipDate = shipDate.AddDays(1)
                Next
            Next
        End Using

    End Function
End Class

Public Class FBResponse

    Public Property consigneeId As String
    Public Property shipDate As String
    Public Property hasService As Boolean
    Public Property serviceDays As List(Of FBServiceDays)
    Public Property messages As Message()
End Class

Public Class FBServiceDays
    Public Property dayOfWeek As String
    Public Property hasService As Boolean
End Class

Public Class Message
    Public Property severity As String
    Public Property message As String
End Class


