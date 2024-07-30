Imports System.Data.SqlClient
Imports System.Data


Namespace DAO
    Public Class Pictures

        Private _TotalRows As Integer
        Public Property TotalRows() As Integer
            Get
                Return _TotalRows
            End Get
            Set(ByVal value As Integer)
                _TotalRows = value
            End Set

        End Property
        ' Modified by Anubhuti 2023_06_06
        Public Function GetPicturesListForFgrd(ByVal whereClause As String, ByVal sortExp As String, ByVal startRowIndex As Integer,
                                              ByVal numberOfRows As Integer) As List(Of BO.Variety)

            Dim uList As New List(Of BO.Variety)()
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString

                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("[spGetPictures]", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.Add(New SqlParameter("@WhereClause", whereClause))
                        cmd.Parameters.Add(New SqlParameter("@SortExpression", sortExp))
                        cmd.Parameters.Add(New SqlParameter("@RowIndex", startRowIndex))
                        cmd.Parameters.Add(New SqlParameter("@NoOfRows", numberOfRows))
                        Dim da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        For Each row In ds.Tables(0).Rows
                            Dim u As New BO.Variety()
                            u.ID = row("ID")
                            u.Category = If(row("Category") Is DBNull.Value, "", row("Category"))
                            u.Variety = If(row("Variety") Is DBNull.Value, "", row("Variety"))
                            u.Color = If(row("Color") Is DBNull.Value, "", row("Color"))
                            u.Photo = If(row("Photo") Is DBNull.Value, "", row("Photo"))
                            u.PhotoLarge = If(row("PhotoLarge") Is DBNull.Value, "", row("PhotoLarge"))
                            uList.Add(u)
                        Next
                        Me.TotalRows = ds.Tables(0).Rows.Count
                    End Using
                End Using

            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in GetPicturesListForFgrd")
                Return Nothing
            End Try
            Return uList
        End Function

        Public Shared Function AddEditDeleteSelectPictures(ByVal ID As String, ByVal Category As String, ByVal Variety As String,
                                                             ByVal Color As String, ByVal Mode As String, ByVal Photo As String, ByVal PhotoLarge As String) As String
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
                Dim SqlCon As New SqlConnection(ConStr)
                SqlCon.Open()
                Dim PicturesScopeID As String = ""
                Dim Cmd As New SqlCommand("sp_F_PICTURES", SqlCon)
                Cmd.CommandType = CommandType.StoredProcedure
                Cmd.Parameters.AddWithValue("@ID", ID)
                Cmd.Parameters.AddWithValue("@Category", Category)
                Cmd.Parameters.AddWithValue("@Variety", Variety)
                Cmd.Parameters.AddWithValue("@Color", Color)
                Cmd.Parameters.AddWithValue("@Mode", Mode)
                Cmd.Parameters.AddWithValue("@Photo", Photo)
                Cmd.Parameters.AddWithValue("@PhotoLarge", PhotoLarge)
                PicturesScopeID = Cmd.ExecuteScalar()
                Dim UserDetails As New User
                If (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                    UserDetails = HttpContext.Current.Session("LoginUserDetails")
                ElseIf (Not HttpContext.Current.Session("LoginUserDetails") Is Nothing) Then
                    UserDetails = HttpContext.Current.Session("LoginUserDetails")
                End If
                If (Not UserDetails Is Nothing) Then
                    Logs.Savelog(UserDetails.ID, "Users", "AddEditDeleteSelectPictures", IIf(ID = "0", "Added a new Picture for <b>" + Variety + "</b>", "Updated a Variety <b>" + Variety + "</b>"), PicturesScopeID.ToString())
                End If
                Return PicturesScopeID
            Catch ex As Exception
                If (Not ex.Message.Contains("UNIQUE KEY constraint")) Then
                    ErrorLogs.LogException(ex, "Error in AddEditDeleteSelectPictures")
                End If
                Return ex.Message()
            End Try
        End Function

        Public Shared Function GetPicturesByID(ByVal ID As String) As BO.Variety
            Try
                Dim ConStr As String = ConfigurationManager.ConnectionStrings.Item("BloomsConnectionString").ToString
                Dim u As New BO.Variety()
                Using con As New SqlConnection(ConStr)
                    con.Open()
                    Using cmd As New SqlCommand("SELECT * FROM F_PICTURES WHERE ID=@ID", con)
                        cmd.CommandType = CommandType.Text
                        cmd.Parameters.AddWithValue("@ID", ID)
                        Dim da As New SqlDataAdapter(cmd)
                        Dim ds As New DataSet
                        da.Fill(ds)
                        For Each row In ds.Tables(0).Rows
                            u.ID = row("ID")
                            u.Category = If(row("Category") Is DBNull.Value, "", row("Category"))
                            u.Variety = If(row("Variety") Is DBNull.Value, "", row("Variety"))
                            u.Color = If(row("Color") Is DBNull.Value, "", row("Color"))
                            u.Photo = If(row("Photo") Is DBNull.Value, "", row("Photo"))
                            u.PhotoLarge = If(row("PhotoLarge") Is DBNull.Value, "", row("PhotoLarge"))

                        Next
                    End Using
                End Using
                Return u
            Catch ex As Exception
                ErrorLogs.LogException(ex, "Error in GetPicturesByID")
                Throw ex
            End Try
        End Function

    End Class
End Namespace

Namespace BOPICTURES
    Public Class Variety
        Private _ID As String
        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        Private _Category As String
        Public Property Category() As String
            Get
                Return _Category
            End Get
            Set(ByVal value As String)
                _Category = value
            End Set
        End Property

        Private _Variety As String
        Public Property Variety() As String
            Get
                Return _Variety
            End Get
            Set(ByVal value As String)
                _Variety = value
            End Set
        End Property

        Private _Color As String
        Public Property Color() As String
            Get
                Return _Color
            End Get
            Set(ByVal value As String)
                _Color = value
            End Set
        End Property

        Private _Photo As String
        Public Property Photo() As String
            Get
                Return _Photo
            End Get
            Set(ByVal value As String)
                _Photo = value
            End Set
        End Property

        Private _PhotoLarge As String
        Public Property PhotoLarge() As String
            Get
                Return _PhotoLarge
            End Get
            Set(ByVal value As String)
                _PhotoLarge = value
            End Set
        End Property


    End Class
End Namespace
