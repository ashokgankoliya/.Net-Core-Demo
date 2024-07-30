Imports Microsoft.VisualBasic

Public Class LogiztikAllianceMessage
    Public Property OrderHeader As LogiztikAllianceHeader
    Public Property OrderDetail As List(Of LogiztikAllianceDetail)
End Class

Public Class LogiztikAllianceHeader
    Public Property Shipdate As String
    Public Property Shipper As String
    Public Property BarcodeAccount As String
    Public Property ShipdateOriginal As Date
End Class

Public Class LogiztikAllianceDetail
    Public Property UnitID As String
    Public Property UnitOfMeasure As String
    Public Property Measure As String
    Public Property FarmName As String
    Public Property AWB As String
    Public Property Consignee As String
    Public Property ClienteFinal As String
    Public Property Length As Integer
    Public Property Width As Integer
    Public Property Height As Integer
    Public Property Product As String
    Public Property ProductDescription As String
    Public Property PalletID As String
    Public Property PO As String
    Public Property PaymentType As String
    Public Property Command As String
    Public Property ShipperAccount As Object
    Public Property Remarks As String
    Public Property IdClienteFinal As String
    Public Property Invoice As String
    Public Property CreationDateManifiesto As String
    Public Property IdProgCarrier As String
End Class
