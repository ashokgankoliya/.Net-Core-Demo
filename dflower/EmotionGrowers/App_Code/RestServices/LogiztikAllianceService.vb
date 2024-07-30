Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json

Public Class LogiztikAllianceService
    Public Shared Async Function SendAsync(ByVal orders As String) As Threading.Tasks.Task
        Try
            For Each OrderNo In orders.Split(",")
                Await SendOneAsync(OrderNo)
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Function
    Public Shared Async Function SendOneAsync(ByVal order As String) As Threading.Tasks.Task
        Try
            Dim message = CreateMessage(order)

            Dim gngShipper As String = ConfigurationManager.AppSettings("GNGShipper").ToString()
            Dim gngToken As String = ConfigurationManager.AppSettings("GNGAPIToken").ToString()
            Dim gngUrl As String = ConfigurationManager.AppSettings("GNGAPIURL").ToString()
            Dim gngPrefix As String = ConfigurationManager.AppSettings("GNGPrefix").ToString()
            Dim gngMethod As String = "OrdenesLocales/InsertarEdiOrdenesLocalesQueue/"

            Dim http As New Http.HttpClient()
            Dim content As New Http.StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json")
            Dim response = Await http.PostAsync(gngUrl + gngMethod + gngToken, content)
            Dim result = Await response.Content.ReadAsStringAsync()

            Console.WriteLine(result)
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Function
    Public Shared Function SendOne(ByVal order As String) As String
        Try
            Dim message = CreateMessage(order)

            Dim gngShipper As String = ConfigurationManager.AppSettings("GNGShipper").ToString()
            Dim gngToken As String = ConfigurationManager.AppSettings("GNGAPIToken").ToString()
            Dim gngUrl As String = ConfigurationManager.AppSettings("GNGAPIURL").ToString()
            Dim gngPrefix As String = ConfigurationManager.AppSettings("GNGPrefix").ToString()
            Dim gngMethod As String = "OrdenesLocales/InsertarEdiOrdenesLocalesQueue/"

            Dim webRequest As HttpWebRequest = CType(webRequest.Create(gngUrl + gngMethod + gngToken), HttpWebRequest)
            webRequest.ContentType = "application/json"
            webRequest.Accept = "application/json"
            webRequest.Method = "POST"

            'webRequest.Timeout = 4000
            Dim result As String = ""

            Dim bytes As Byte() = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))

            Dim requeststream As Stream = webRequest.GetRequestStream()
            requeststream.Write(bytes, 0, bytes.Length)
            requeststream.Close()


            Dim myWebResponse = CType(webRequest.GetResponse(), HttpWebResponse)

            Dim myStreamReader = New StreamReader(myWebResponse.GetResponseStream())
            result = myStreamReader.ReadToEnd
            myStreamReader.Close()

            Dim resultObj = JsonConvert.DeserializeObject(Of List(Of Response))(result)
            Dim responseMessage As String = ""
            Dim StatusType As String = IIf(resultObj(0).isSuccess, "success", "error")
            If Not String.IsNullOrEmpty(resultObj(0).errorMessage) Then
                responseMessage = resultObj(0).errorMessage
            End If

            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("insert into tblFloridaXMLLogs (RequestXML,RequestXMLFileName,ResponseXML,ResponseMessage,XMLStatus,StatusType,Shipdate,InvoiceNo,TotalBoxesSend,TotalBoxesUploaded,WebOrWindows,CreatedDateTime,Consignee,BoxNo)values(@RequestXML, @RequestXMLFileName, @ResponseXML, @ResponseMessage, @XMLStatus, @StatusType, @Shipdate, @InvoiceNo, @TotalBoxesSend, @TotalBoxesUploaded, @WebOrWindows, @CreatedDateTime,@Consignee,@BoxNo)", con)
                    cmd.CommandType = CommandType.Text
                    cmd.Parameters.Add(New SqlParameter("@RequestXML", JsonConvert.SerializeObject(message)))
                    cmd.Parameters.Add(New SqlParameter("@RequestXMLFileName", ""))
                    cmd.Parameters.Add(New SqlParameter("@ResponseXML", result))
                    cmd.Parameters.Add(New SqlParameter("@ResponseMessage", responseMessage))
                    cmd.Parameters.Add(New SqlParameter("@XMLStatus", resultObj(0).isSuccess))
                    cmd.Parameters.Add(New SqlParameter("@StatusType", StatusType))
                    cmd.Parameters.Add(New SqlParameter("@Shipdate", message.OrderHeader.ShipdateOriginal))
                    cmd.Parameters.Add(New SqlParameter("@InvoiceNo", order))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesSend", 0))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesUploaded", 0))
                    cmd.Parameters.Add(New SqlParameter("@WebOrWindows", "Web"))
                    cmd.Parameters.Add(New SqlParameter("@CreatedDateTime", DateTime.Now))
                    cmd.Parameters.Add(New SqlParameter("@Consignee", ""))
                    cmd.Parameters.Add(New SqlParameter("@BoxNo", ""))
                    cmd.ExecuteNonQuery()
                End Using
            End Using


            Return result
        Catch ex As Exception
            Return ex.Message
        End Try

    End Function
    Public Shared Async Function Send(ByVal orders As String) As Threading.Tasks.Task(Of List(Of String))
        Try
            Dim result As New List(Of String)
            For Each OrderNo In orders.Split(",")
                result.Add(SendOne(OrderNo))
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return Nothing
    End Function

    Public Shared Function CreateMessage(ByVal order As String) As LogiztikAllianceMessage
        Dim dt As New DataTable
        Dim ds As New DataSet

        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spGetGnGOrderInfo", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.AddWithValue("@order", order)
                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(ds)
                End Using
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In InsertShippingLabelDetails")
            Return Nothing
        End Try


        Dim message = New LogiztikAllianceMessage()
        message.OrderHeader = New LogiztikAllianceHeader()

        Dim gngShipper As String = ConfigurationManager.AppSettings("GNGShipper").ToString()
        Dim gngToken As String = ConfigurationManager.AppSettings("GNGAPIToken").ToString()
        Dim gngUrl As String = ConfigurationManager.AppSettings("GNGAPIURL").ToString()
        Dim gngPrefix As String = ConfigurationManager.AppSettings("GNGPrefix").ToString()
        Dim gngMethod As String = "OrdenesLocales/InsertarEdiOrdenesLocalesQueue/"

        message.OrderHeader.Shipper = gngShipper
        message.OrderHeader.BarcodeAccount = "DGG"
        message.OrderHeader.Shipdate = Convert.ToDateTime(ds.Tables(0).Rows(0).Item("ShippingDate")).ToString("MM/dd/yy")
        message.OrderHeader.ShipdateOriginal = ds.Tables(0).Rows(0).Item("ShippingDate")

        message.OrderDetail = New List(Of LogiztikAllianceDetail)()

        For i = 0 To ds.Tables(0).Rows.Count - 1
            Dim detail As New LogiztikAllianceDetail()
            detail.UnitID = gngPrefix + ds.Tables(0).Rows(i).Item("ARBoxNo")
            detail.UnitOfMeasure = "box"
            detail.Measure = "inch"
            detail.AWB = ds.Tables(0).Rows(0).Item("awb")
            detail.Consignee = ds.Tables(0).Rows(0).Item("Consignee")
            detail.Length = ds.Tables(0).Rows(0).Item("L")
            detail.Width = ds.Tables(0).Rows(0).Item("W")
            detail.Height = ds.Tables(0).Rows(0).Item("H")
            'detail.Product = ds.Tables(0).Rows(0).Item("FCF")
            detail.Product = "FCF"
            detail.ProductDescription = ds.Tables(0).Rows(0).Item("Product")
            detail.PO = ds.Tables(0).Rows(0).Item("PONumber")
            detail.PaymentType = ds.Tables(0).Rows(0).Item("Prepaid")
            detail.Command = "A"
            detail.Invoice = order
            message.OrderDetail.Add(detail)
        Next
        Return message
    End Function

End Class
Public Class Response
    Public Property isSuccess As Boolean
    Public Property unitID As String
    Public Property errorMessage As Object
End Class

