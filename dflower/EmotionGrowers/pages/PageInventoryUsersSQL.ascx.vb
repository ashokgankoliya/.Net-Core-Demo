﻿Imports System.IO
Imports System.Data
Partial Class pages_PageInventoryUserSQL
    Inherits System.Web.UI.UserControl

    Private Sub pages_PageInventoryUserSQL_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim UserFeatures As List(Of Feature) = Session("LoginUserFeatures")
        Dim LoginUserDetails As New User
        If (Not Session("LoginUserDetails") Is Nothing) Then
            LoginUserDetails = Session("LoginUserDetails")
            LoggedInUserID_InvenUser.InnerText = LoginUserDetails.ID


        End If
    End Sub
End Class
