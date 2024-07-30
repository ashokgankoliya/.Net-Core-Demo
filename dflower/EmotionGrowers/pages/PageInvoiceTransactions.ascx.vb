
Partial Class pages_PageInvoiceTransactions
    Inherits System.Web.UI.UserControl
    Implements ICustomParams_InvoiceNumber

    Dim m_InoviceNo As String

    Public Property InvoiceNo As String Implements ICustomParams_InvoiceNumber.Invoiceno
        Get
            Return m_InoviceNo
        End Get
        Set(ByVal value As String)
            m_InoviceNo = value
            divInvoiceNo_InvTrans.InnerText = m_InoviceNo
        End Set
    End Property


End Class

Partial Class pages_PageInvoiceTransactions
    Inherits System.Web.UI.UserControl
    Implements ICustomParams_IsARINVHistory

    Dim m_IsARINVHistory As String
    Public Property IsARINVHistory As String Implements ICustomParams_IsARINVHistory.IsARINVHistory
        Get
            Return m_IsARINVHistory
        End Get
        Set(ByVal value As String)
            m_IsARINVHistory = value
            divInvoiceNo_InvTrans.InnerText = m_IsARINVHistory
        End Set
    End Property
End Class
