Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports RestSharp

Public Class EliteService
    Public Shared Function SendDV(user As User) As String
        Dim dt As New DataTable
        Dim ds As New DataSet

        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetDvOrderInfo", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(ds)
                End Using
                dt = ds.Tables(0)
                If dt.Rows.Count = 0 OrElse dt.Rows(0)("invoice") = 0 Then
                    Return "edierror~Could not do EDI"
                End If
                Return Send(dt, dt.Rows(0)("invoice"), user, False, "EL")
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In Eliteservice.vb SendDV")
            Return "edierror~" & ex.Message
        End Try

    End Function

    Public Shared Function SendNewDV(user As User) As String
        Dim dt As New DataTable
        Dim ds As New DataSet

        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetNewDvOrderInfo", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(ds)
                End Using
                dt = ds.Tables(0)
                If dt.Rows.Count = 0 OrElse dt.Rows(0)("invoice") = 0 Then
                    Return "edierror~Could not do EDI"
                End If
                Return Send(dt, dt.Rows(0)("invoice"), user, False, "EL")
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In Eliteservice.vb SendNewDV")
            Return "edierror~" & ex.Message
        End Try

    End Function

    Public Shared Function SendAsync(order As String, user As User) As String
        Dim dt As New DataTable
        Dim ds As New DataSet

        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetEliteOrderInfo", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@order", order)
                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(ds)
                End Using
                dt = ds.Tables(0)
                Dim carrier As String = dt.Rows(0)("carrier")
                Return Send(dt, order, user, True, carrier)
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In Eliteservice.vb SendAsync")
            Return "edierror~" & ex.Message
        End Try
    End Function
    Public Shared Function Send(table As DataTable, invoice As String, user As User, includeShipper As Boolean, carrier As String) As String
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim ds As New DataSet, apiUrl As String, token As String, shipper As String

            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetCarrierApiInfo", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@carrier", carrier)
                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(ds)
                End Using
                apiUrl = ds.Tables(0).Rows(0)("apiurl")
                token = ds.Tables(0).Rows(0)("token")
                shipper = ds.Tables(0).Rows(0)("shipper")
            End Using
            Dim client = New RestClient(New RestClientOptions With {
                .MaxTimeout = 10000,
                .BaseUrl = New Uri(apiUrl)
                })
            Dim request = New RestRequest()
            Dim shipDate As Date = table.Rows(0)("ShippingDate")
            request.AddHeader("authorizationToken", token)
            Dim eliteRequest As New EliteRequest With {
                .OrderHeader = New Orderheader With {
                    .ShipDate = shipDate.ToString("MM/dd/yy"),
                    .Shipper = shipper
                },
                .OrderDetail = New List(Of Orderdetail)()
            }
            Dim arbox As String = table.Rows(0)("arBoxNo")
            Dim consignee As String = table.Rows(0)("consignee")
            Dim company As String = ConfigurationManager.AppSettings("CompanyName")
            Dim unitShipper As String = ""
            If includeShipper Then
                unitShipper = shipper
            End If
            For Each row In table.Rows
                eliteRequest.OrderDetail.Add(New Orderdetail With {
                    .UnitID = unitShipper & row("arBoxNo"),
                    .UnitOfMeasure = "Box",
                    .FarmName = row("FarmName"),
                    .AWB = "",
                    .Consignee = row("consignee"),
                    .Length = row("l"),
                    .Width = row("w"),
                    .Height = row("h"),
                    .Product_Description = row("Product"),
                    .PO = row("PONumber")
                })

            Next
            request.AddJsonBody(eliteRequest)
            Dim success As Boolean = False
            Dim content As String = String.Empty
            Dim responseMessage As String = String.Empty
            Dim errorMessage As String = String.Empty

            Try
                Dim response As RestResponse = client.Post(request)
                Dim result As EliteResponse = JsonConvert.DeserializeObject(Of EliteResponse)(response.Content)
                success = True
                content = response.Content
                success = ((result.ResponseHeader.totalUnitsInFile - result.ResponseHeader.totalUnitsSuccess - result.ResponseHeader.totalUnitsDuplicates) = 0)
                If result.ResponseDetail Is Nothing Then
                    errorMessage = result.ResponseDetails(0).message
                Else
                    errorMessage = result.ResponseDetail(0).message
                End If
                responseMessage = String.Format("Ticket: {4} - Total: {0} - Success: {1} - Duplicates: {2} - Message: {3}", result.ResponseHeader.totalUnitsInFile, result.ResponseHeader.totalUnitsSuccess, result.ResponseHeader.totalUnitsDuplicates, errorMessage, result.ticket)

                Dim subject As String = String.Format("{2} - Order: {0} - Ticket: {1}", invoice, result.ticket, company)

                Logs.SendMail(ConfigurationManager.AppSettings("VendorFromEmails").ToString(), user.Email, responseMessage & Environment.NewLine & JsonConvert.SerializeObject(eliteRequest) & Environment.NewLine & response.Content, "", subject)
            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error In Eliteservice.vb Send")
                responseMessage = "Server error"
                errorMessage = "Server error - Please try again"
            End Try
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("insert into tblFloridaXMLLogs (RequestXML,RequestXMLFileName,ResponseXML,ResponseMessage,XMLStatus,StatusType,Shipdate,InvoiceNo,TotalBoxesSend,TotalBoxesUploaded,WebOrWindows,CreatedDateTime,Consignee,BoxNo)values(@RequestXML, @RequestXMLFileName, @ResponseXML, @ResponseMessage, @XMLStatus, @StatusType, @Shipdate, @InvoiceNo, @TotalBoxesSend, @TotalBoxesUploaded, @WebOrWindows, @CreatedDateTime,@Consignee,@BoxNo)", con)
                    cmd.CommandType = CommandType.Text
                    cmd.Parameters.Add(New SqlParameter("@RequestXML", JsonConvert.SerializeObject(eliteRequest)))
                    cmd.Parameters.Add(New SqlParameter("@RequestXMLFileName", ""))
                    cmd.Parameters.Add(New SqlParameter("@ResponseXML", content))
                    cmd.Parameters.Add(New SqlParameter("@ResponseMessage", responseMessage))
                    cmd.Parameters.Add(New SqlParameter("@XMLStatus", success))
                    cmd.Parameters.Add(New SqlParameter("@StatusType", IIf(success, "success", "error")))
                    cmd.Parameters.Add(New SqlParameter("@Shipdate", shipDate))
                    cmd.Parameters.Add(New SqlParameter("@InvoiceNo", invoice))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesSend", 0))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesUploaded", 0))
                    cmd.Parameters.Add(New SqlParameter("@WebOrWindows", "Web"))
                    cmd.Parameters.Add(New SqlParameter("@CreatedDateTime", DateTime.Now))
                    cmd.Parameters.Add(New SqlParameter("@Consignee", consignee))
                    cmd.Parameters.Add(New SqlParameter("@BoxNo", arbox))
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            If success Then
                Return "success~"
            Else
                Return "edierror~" & errorMessage
            End If
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In Eliteservice.vb Send")
            Return "edierror~Error In EliteService.vd Send"
        End Try
    End Function

End Class


Public Class EliteRequest
    Public Property OrderHeader As Orderheader
    Public Property OrderDetail() As List(Of Orderdetail)
End Class

Public Class Orderheader
    Public Property ShipDate As String
    Public Property Shipper As String
End Class

Public Class Orderdetail
    Public Property UnitID As String
    Public Property UnitOfMeasure As String
    Public Property FarmName As String
    Public Property AWB As String
    Public Property Consignee As String
    Public Property Length As String
    Public Property Width As String
    Public Property Height As String
    Public Property Product_Description As String
    Public Property PO As String
End Class


Public Class EliteResponse
    Public Property ticket As String
    Public Property ResponseHeader As Responseheader
    Public Property ResponseDetail As List(Of Responsedetail)
    Public Property ResponseDetails As List(Of Responsedetail)
End Class

Public Class Responseheader
    Public Property Shipdate As String
    Public Property Shipper As String
    Public Property totalUnitsInFile As Integer
    Public Property totalUnitsSuccess As Integer
    Public Property totalUnitsDuplicates As Integer
    Public Property totalUnitsDiscrepancy As Integer
    Public Property orderNumber As String
End Class

Public Class Responsedetail
    Public Property consignee As String
    Public Property consigneename As String
    Public Property unitsSucces As Integer
    Public Property unitsFailed As Integer
    Public Property message As String
    'Public Property unitFailedDetail As Unitfaileddetail
End Class

Public Class Unitfaileddetail
    Public Property unitId As List(Of String)
    Public Property errorMessage As String
End Class
