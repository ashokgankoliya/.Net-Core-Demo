﻿Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.Security.Cryptography
Imports System.IO
Imports System.Xml

Public Class ArmelliniXML

    Public Shared Function GetArmelliniXML() As List(Of BO.ArmelliniXML)
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim uList As New List(Of BO.ArmelliniXML)
            Using Con As New SqlConnection(ConStr)
                Con.Open()
                Using Cmd As New SqlCommand(String.Format("select * from vwArmelliniXML where isnull(IsSent,0)=0"), Con)
                    Cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(Cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)
                    For Each row In dt.Rows
                        Dim u As New BO.ArmelliniXML
                        u.ID = row("ID")
                        u.RequestXML = row("RequestXML")
                        u.AREmailIds = row("AREmailIds")
                        u.OrderNo = row("OrderNo")
                        uList.Add(u)
                    Next
                End Using
            End Using
            Return uList
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetArmelliniXML")
            Return Nothing
        End Try
    End Function



    Public Shared Function GetArmelliniXMLByID(ByVal ID As String) As BO.ArmelliniXML
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim u As New BO.ArmelliniXML
            Using Con As New SqlConnection(ConStr)
                Con.Open()
                Using Cmd As New SqlCommand("select * from tblArmelliniXml where ID=@ID", Con)
                    Cmd.Parameters.AddWithValue("@ID", ID)
                    Cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(Cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)
                    For Each row In dt.Rows
                        u.ID = row("ID")
                        Dim RequestXML As String = Convert.ToString(row("RequestXML")).Replace("<?xml version='1.0' encoding='utf-8'?>", "")
                        If RequestXML <> "" Then
                            u.RequestXML = ManualPODetails.PrettyXML(RequestXML)
                        End If
                        u.ResponseXML = Convert.ToString(row("ResponseXML")).Replace("<?xml version='1.0' encoding='utf-8'?>", "")
                        u.IsSent = IIf(row("IsSent") Is DBNull.Value, "0", row("IsSent"))
                    Next
                End Using
            End Using
            Return u
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetArmelliniXML")
            Return Nothing
        End Try
    End Function


    Public Shared Function UpdateArmelliniXMLResponse(ByVal Id As Integer, ByVal ResponseXML As String) As String
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using Con As New SqlConnection(ConStr)
                Con.Open()
                Dim qry As String = ""
                qry = "Update tblArmelliniXml set ResponseXML=@ResponseXML,IsSent=1 where ID=@ID"
                Using Cmd As New SqlCommand(qry, Con)
                    Cmd.Parameters.AddWithValue("@ID", Id)
                    Cmd.Parameters.AddWithValue("@ResponseXML", ResponseXML)
                    Cmd.CommandType = CommandType.Text
                    Cmd.ExecuteNonQuery()
                End Using
            End Using
            Return "success"
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in UpdateArmelliniXMLResponse")
            Return Nothing
        End Try
    End Function

    Public Shared Function CheckArmelliniLogXMLStatus(ByVal OrderNo As String, ByVal Carrier As String) As String
        Try
            Dim tablename As String = ""
            If Carrier = "FB" Or Carrier = "GG" Or Carrier = "G&G" Or Carrier = "EL" Or Carrier = "FT" Or Carrier = "AF" Then
                tablename = "tblFloridaXMLLogs"
            Else
                tablename = "tblArmelliniXMLLogs"
            End If

            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using Con As New SqlConnection(ConStr)
                Con.Open()
                Using Cmd As New SqlCommand(String.Format("select top 1 XMLStatus from " + tablename + " where InvoiceNo=@OrderNo order by createddatetime desc "), Con)
                    Cmd.Parameters.AddWithValue("@OrderNo", OrderNo)
                    Cmd.CommandType = CommandType.Text
                    Dim da As New SqlDataAdapter(Cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        If dt.Rows(0)(0).ToString() = True Then
                            Return "success"
                        Else
                            Return "error"
                        End If
                    Else
                        Return "not found"
                    End If

                End Using
            End Using
            Return False
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in CheckArmelliniLogStatus")
            Return False
        End Try
    End Function

    Public Shared Function InsertArmelliniXMLLogs(RequestXML As String, RequestXMLFileName As String, ResponseXML As String, ResponseMessage As String, XMLStatus As [Boolean], StatusType As String, Shipdate As String, InvoiceNo As String, TotalBoxesSend As Integer, TotalBoxesUploaded As Integer, Consignee As String, BoxNo As String, ByVal Username As String) As String
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using cmd As New SqlCommand("spInsertArmelliniXMLLogs", con)
                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.Parameters.Add(New SqlParameter("@RequestXML", RequestXML))
                    cmd.Parameters.Add(New SqlParameter("@RequestXMLFileName", RequestXMLFileName))
                    cmd.Parameters.Add(New SqlParameter("@ResponseXML", ResponseXML))
                    cmd.Parameters.Add(New SqlParameter("@ResponseMessage", ResponseMessage))
                    cmd.Parameters.Add(New SqlParameter("@XMLStatus", XMLStatus))
                    cmd.Parameters.Add(New SqlParameter("@StatusType", StatusType))
                    cmd.Parameters.Add(New SqlParameter("@Shipdate", Shipdate))
                    cmd.Parameters.Add(New SqlParameter("@InvoiceNo", InvoiceNo))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesSend", TotalBoxesSend))
                    cmd.Parameters.Add(New SqlParameter("@TotalBoxesUploaded", TotalBoxesUploaded))
                    cmd.Parameters.Add(New SqlParameter("@WebOrWindows", "Web"))
                    cmd.Parameters.Add(New SqlParameter("@CreatedDateTime", DateTime.Now))
                    cmd.Parameters.Add(New SqlParameter("@Consignee", Consignee))
                    cmd.Parameters.Add(New SqlParameter("@BoxNo", BoxNo))
                    cmd.Parameters.Add(New SqlParameter("@UserName", Username))
                    cmd.ExecuteNonQuery()
                End Using

                Try
                    Dim LoginUserDetails As New User
                    If (Not HttpContext.Current.Session("LoginAdminDetails") Is Nothing) Then
                        LoginUserDetails = HttpContext.Current.Session("LoginAdminDetails")
                    ElseIf (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                        LoginUserDetails = HttpContext.Current.Session("LoginUserDetails")
                    End If
                    Dim qry As String = ""
                Catch ex As Exception
                    ErrorLogs.LogException(ex, "Error in InsertArmelliniXMLLogs-Submethod")
                End Try
            End Using
            Return "valid"
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in InsertArmelliniXMLLogs")
            Return "Error"
        End Try
    End Function

#Region "New Armellini"
    Public Shared Function GetOrderDetailsForXML(ByVal OrderNo As String) As DataTable
        Try
            Dim arxml As New ArmelliniXML
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using Cmd As New SqlCommand("spGetOrderDetailsForXMLByOrderNo", con)
                    Cmd.Parameters.AddWithValue("@OrderNo", OrderNo)
                    Cmd.CommandType = CommandType.StoredProcedure
                    Dim da As New SqlDataAdapter(Cmd)
                    Dim dt As New DataTable
                    da.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetOrderDetailsForXML")
            Return Nothing
        End Try
    End Function
#End Region
#Region "New Armellini resend xml"
    Public Shared Function ResendARXMLFile(ByVal OrderNo As String, ByVal UserId As String, ByVal NewShipdate As String,
                                    ByVal NewConsignee As String) As Boolean
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Using con As New SqlConnection(ConStr)
                con.Open()
                Using Cmd As New SqlCommand("spInsertArmelliniXmlForResend", con)
                    Cmd.Parameters.AddWithValue("@OrderNo", OrderNo)
                    Cmd.Parameters.AddWithValue("@UserId", UserId)
                    Cmd.Parameters.AddWithValue("@shipperdate", NewShipdate)
                    Cmd.Parameters.AddWithValue("@consignee", NewConsignee)
                    Cmd.CommandType = CommandType.StoredProcedure
                    Cmd.ExecuteNonQuery()
                    Return True
                End Using
            End Using
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error In ResendARXMLFile")
            Throw ex
            Return False
        End Try
    End Function
#End Region


    Public Class GetArmelliniXMLDetails

        Private _TotalRows As Integer
        Public Property TotalRows() As Integer
            Get
                Return _TotalRows
            End Get
            Set(ByVal value As Integer)
                _TotalRows = value
            End Set
        End Property

        ''' <summary>
        ''' MANI::05-NOV-2015::get ARXML list for grid
        ''' </summary>
        ''' <param name="whereClause"></param>
        ''' <param name="sortExp"></param>
        ''' <param name="startRowIndex"></param>
        ''' <param name="numberOfRows"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetARXMLListForFgrd(ByVal whereClause As String, ByVal sortExp As String, ByVal startRowIndex As Integer,
                                              ByVal numberOfRows As Integer) As List(Of BO.ArmelliniXML)

            Dim uList As New List(Of BO.ArmelliniXML)()
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString

                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("[spGetARXML]", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.Add(New SqlParameter("@WhereClause", whereClause))
                        cmd.Parameters.Add(New SqlParameter("@SortExpression", sortExp))
                        cmd.Parameters.Add(New SqlParameter("@RowIndex", startRowIndex))
                        cmd.Parameters.Add(New SqlParameter("@NoOfRows", numberOfRows))
                        Dim da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        For Each row In ds.Tables(0).Rows
                            Dim u As New BO.ArmelliniXML()
                            u.ID = row("ID")
                            u.RequestXML = Convert.ToString(row("RequestXML"))
                            u.ResponseXML = Convert.ToString(row("ResponseXML"))
                            u.IsSent = row("IsSent")
                            u.CreatedDateTime = row("CreatedDateTime")
                            u.CreatedUserName = Convert.ToString(row("CreatedUserName"))
                            u.OrderNo = Convert.ToString(row("OrderNo"))
                            uList.Add(u)
                        Next
                        Me.TotalRows = ds.Tables(1).Rows(0)(0)
                    End Using
                End Using

            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in GetARXMLListForFgrd")
                Return Nothing
            End Try
            Return uList
        End Function


        Public Function GetArmelliniDayOfServiceByConsignee(ByVal consignee As String) As List(Of String)
            Dim uList As New List(Of String)()
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("SELECT shipDate FROM tblDaysOfService a WHERE consigneeAccountNumber=@consignee ORDER BY shipDate", con)
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.Add(New SqlParameter("@consignee", consignee))
                        Dim da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)

                        For Each row In ds.Tables(0).Rows
                            uList.Add(row(0))
                        Next
                    End Using
                End Using
                Return uList

            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in GetARXMLListForFgrd")
                Return Nothing
            End Try
            Return uList
        End Function


        Public Function GetArmelliniXMLLogsByOrder(ByVal Order As String, ByVal Carrier As String) As List(Of BO.ArmelliniXML)
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
                Dim uList As New List(Of BO.ArmelliniXML)()
                Dim tableName As String = ""
                If Carrier = "FB" Or Carrier = "EL" Or Carrier = "FT" Or Carrier = "AF" Or Carrier.Substring(0, 1) = "G" Then
                    tableName = "tblFloridaXMLLogs"
                Else
                    tableName = "tblArmelliniXMLLogs"
                End If

                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("SELECT * FROM " + tableName + "  WHERE InvoiceNo=@Order ORDER BY CREATEDDATETIME DESC", con)
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.Add(New SqlParameter("@Order", Order))
                        Dim da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)


                        For Each row In ds.Tables(0).Rows
                            Dim u As New BO.ArmelliniXML()
                            u.ID = row("ID")
                            u.ShipDate = Convert.ToDateTime(row("ShipDate")).ToString("MM/dd/yyyy")
                            u.XMLFileName = Convert.ToString(row("RequestXMLFileName"))
                            u.ResponseMessage = IIf(row("ResponseMessage") Is DBNull.Value, "", row("ResponseMessage"))
                            u.Status = Convert.ToString(row("StatusType"))
                            u.CreatedDateTime = row("CreatedDateTime")
                            u.Consignee = IIf(row("Consignee") Is DBNull.Value, "", row("Consignee"))
                            u.ArBoxNo = IIf(row("BoxNo") Is DBNull.Value, "", row("BoxNo"))
                            uList.Add(u)
                        Next
                        Me.TotalRows = ds.Tables(0).Rows.Count
                    End Using
                End Using
                Return uList
            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in GetArmelliniXMLLogsByOrder")
                Throw ex
            End Try
        End Function

    End Class


End Class

Namespace BO


    Public Class ArmelliniXML

        Private _ID As Integer
        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Private _RequestXML As String
        Public Property RequestXML() As String
            Get
                Return _RequestXML
            End Get
            Set(ByVal value As String)
                _RequestXML = value
            End Set
        End Property

        Private _ResponseXML As String
        Public Property ResponseXML() As String
            Get
                Return _ResponseXML
            End Get
            Set(ByVal value As String)
                _ResponseXML = value
            End Set
        End Property

        Private _IsSent As Boolean
        Public Property IsSent() As Boolean
            Get
                Return _IsSent
            End Get
            Set(ByVal value As Boolean)
                _IsSent = value
            End Set
        End Property

        Private _CreatedDateTime As String
        Public Property CreatedDateTime() As String
            Get
                Return _CreatedDateTime
            End Get
            Set(ByVal value As String)
                _CreatedDateTime = value
            End Set
        End Property

        Private _CreatedUserName As String
        Public Property CreatedUserName() As String
            Get
                Return _CreatedUserName
            End Get
            Set(ByVal value As String)
                _CreatedUserName = value
            End Set
        End Property

        Private _OrderNo As String
        Public Property OrderNo() As String
            Get
                Return _OrderNo
            End Get
            Set(ByVal value As String)
                _OrderNo = value
            End Set
        End Property

        Private _ArBoxNo As String
        Public Property ArBoxNo() As String
            Get
                Return _ArBoxNo
            End Get
            Set(ByVal value As String)
                _ArBoxNo = value
            End Set
        End Property

        Private _Consignee As String
        Public Property Consignee() As String
            Get
                Return _Consignee
            End Get
            Set(ByVal value As String)
                _Consignee = value
            End Set
        End Property

        Private _Product As String
        Public Property Product() As String
            Get
                Return _Product
            End Get
            Set(ByVal value As String)
                _Product = value
            End Set
        End Property


        Private _ShipDate As String
        Public Property ShipDate() As String
            Get
                Return _ShipDate
            End Get
            Set(ByVal value As String)
                _ShipDate = value
            End Set
        End Property

        Private _XMLFileName As String
        Public Property XMLFileName() As String
            Get
                Return _XMLFileName
            End Get
            Set(ByVal value As String)
                _XMLFileName = value
            End Set
        End Property

        Private _ResponseMessage As String
        Public Property ResponseMessage() As String
            Get
                Return _ResponseMessage
            End Get
            Set(ByVal value As String)
                _ResponseMessage = value
            End Set
        End Property

        Private _Status As String
        Public Property Status() As String
            Get
                Return _Status
            End Get
            Set(ByVal value As String)
                _Status = value
            End Set
        End Property

        Private _AREmailIds As String
        Public Property AREmailIds() As String
            Get
                Return _AREmailIds
            End Get
            Set(ByVal value As String)
                _AREmailIds = value
            End Set
        End Property

    End Class


End Namespace