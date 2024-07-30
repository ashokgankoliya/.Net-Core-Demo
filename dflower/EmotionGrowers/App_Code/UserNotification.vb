Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic

Public Class UserNotifications
    Public Shared Function GetUserNotifications(ByVal UserID As Int32) As UserNotification
        Dim userNotification As UserNotification = New UserNotification
        userNotification.Notifications = New List(Of Notification)
        userNotification.UserID = UserID
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim SqlCon As New SqlConnection(ConStr)
            SqlCon.Open()
            Dim ScopeID As String = ""
            Dim Cmd As New SqlCommand("sp_GetUserNotifications", SqlCon)
            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.Parameters.AddWithValue("@UserID", UserID)
            Dim da As New SqlDataAdapter(Cmd)
            Dim dt As New DataTable
            da.Fill(dt)
            For Each row In dt.Rows
                Dim notification As Notification = New Notification
                notification.IsRead = row("IsRead")
                notification.Notes = row("Notes")
                notification.DatePosted = row("DatePosted")
                userNotification.Notifications.Add(notification)
            Next
            userNotification.UnReadCount = userNotification.Notifications.Where(Function(x) x.IsRead = False).Count
            Cmd.Dispose()
            SqlCon.Close()
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetUserNotifications")
        End Try
        Return userNotification
    End Function

    Public Shared Function GetNotifications() As String
        Dim notificationBuilder As StringBuilder = New StringBuilder()
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim SqlCon As New SqlConnection(ConStr)
            SqlCon.Open()
            Dim ScopeID As String = ""
            Dim Cmd As New SqlCommand("sp_GetNotifications", SqlCon)
            Cmd.CommandType = CommandType.StoredProcedure
            Dim da As New SqlDataAdapter(Cmd)
            Dim dt As New DataTable
            da.Fill(dt)
            For Each row In dt.Rows
                notificationBuilder.Append("  ***" + row("Notes"))
            Next
            Cmd.Dispose()
            SqlCon.Close()
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in GetNotifications")
        End Try
        Return notificationBuilder.ToString()
    End Function

    Public Shared Function SaveNotification(ByVal text As String) As Boolean
        Dim notificationBuilder As StringBuilder = New StringBuilder()
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim SqlCon As New SqlConnection(ConStr)
            SqlCon.Open()
            Dim Cmd As New SqlCommand("sp_SaveNotifications", SqlCon)
            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.Parameters.AddWithValue("@Note", text)
            Cmd.ExecuteNonQuery()
            Cmd.Dispose()
            SqlCon.Close()
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in SaveNotification")
            Return False
        End Try
        Return True
    End Function

    Public Shared Function MarkUserNotificationsAsRead(ByVal UserID As Int32) As Boolean
        Try
            Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
            Dim SqlCon As New SqlConnection(ConStr)
            SqlCon.Open()
            Dim ScopeID As String = ""
            Dim Cmd As New SqlCommand("sp_MarkUserNotificationsAsRead", SqlCon)
            Cmd.CommandType = CommandType.StoredProcedure
            Cmd.Parameters.AddWithValue("@UserID", UserID)
            Cmd.ExecuteNonQuery()
            Cmd.Dispose()
            SqlCon.Close()
        Catch ex As Exception
            ErrorLogs.LogException(ex, "Error in MarkUserNotificationsAsRead")
        End Try
        Return True
    End Function
End Class

Public Class UserNotification
    Private _UserID As Int32
    Public Property UserID() As Int32
        Get
            Return _UserID
        End Get
        Set(ByVal value As Int32)
            _UserID = value
        End Set
    End Property

    Private _UnReadCount As Int32
    Public Property UnReadCount() As Int32
        Get
            Return _UnReadCount
        End Get
        Set(ByVal value As Int32)
            _UnReadCount = value
        End Set
    End Property

    Private _Notifications As List(Of Notification)
    Public Property Notifications() As List(Of Notification)
        Get
            Return _Notifications
        End Get
        Set(ByVal value As List(Of Notification))
            _Notifications = value
        End Set
    End Property
End Class

Public Class Notification

    Private _DatePosted As String
    Public Property DatePosted() As String
        Get
            Return _DatePosted
        End Get
        Set(ByVal value As String)
            _DatePosted = value
        End Set
    End Property

    Private _IsRead As Boolean
    Public Property IsRead() As Boolean
        Get
            Return _IsRead
        End Get
        Set(ByVal value As Boolean)
            _IsRead = value
        End Set
    End Property

    Private _Notes As String
    Public Property Notes() As String
        Get
            Return _Notes
        End Get
        Set(ByVal value As String)
            _Notes = value
        End Set
    End Property
End Class