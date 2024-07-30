Imports Microsoft.VisualBasic

<Serializable>
Public Class QuickBookBillIncoming

    Public Property Id As Integer
    Public Property TnxDate As DateTime
    Public Property AWB As String
    Public Property FarmName As String
    Public Property PoAmount As Decimal
    Public Property Invoice As String
    Public Property Credits As Decimal
    Public Property AmountToPay As Decimal
    Public Property PendingPayment As Decimal

End Class
