Imports System
Imports System.IO
Imports System.Text
Imports System.Collections
Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.GZip

Public Class Form1
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnView As System.Windows.Forms.Button
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents chkShowEntry As System.Windows.Forms.CheckBox
    Friend WithEvents txtContent As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnView = New System.Windows.Forms.Button()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkShowEntry = New System.Windows.Forms.CheckBox()
        Me.txtContent = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'btnView
        '
        Me.btnView.Location = New System.Drawing.Point(360, 16)
        Me.btnView.Name = "btnView"
        Me.btnView.TabIndex = 2
        Me.btnView.Text = "View"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(144, 16)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(200, 22)
        Me.txtFileName.TabIndex = 0
        Me.txtFileName.Text = ""
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(24, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Zip File Name:"
        '
        'chkShowEntry
        '
        Me.chkShowEntry.Location = New System.Drawing.Point(24, 56)
        Me.chkShowEntry.Name = "chkShowEntry"
        Me.chkShowEntry.TabIndex = 1
        Me.chkShowEntry.Text = "Show Entry"
        '
        'txtContent
        '
        Me.txtContent.Location = New System.Drawing.Point(24, 96)
        Me.txtContent.Multiline = True
        Me.txtContent.Name = "txtContent"
        Me.txtContent.Size = New System.Drawing.Size(408, 184)
        Me.txtContent.TabIndex = 5
        Me.txtContent.Text = ""
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(6, 15)
        Me.ClientSize = New System.Drawing.Size(448, 296)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.txtContent, Me.chkShowEntry, Me.Label1, Me.txtFileName, Me.btnView})
        Me.Name = "Form1"
        Me.Text = "View Zip file"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Payload Implementation"
    Private Sub btnView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnView.Click
        Dim strmZipInputStream As ZipInputStream = New ZipInputStream(File.OpenRead(txtFileName.Text))
        Dim objEntry As ZipEntry
        Dim strOutput As String
        Dim strBuilder As StringBuilder = New StringBuilder(strOutput)

        objEntry = strmZipInputStream.GetNextEntry()

        While IsNothing(objEntry) = False
            strBuilder.Append("Name: " + objEntry.Name.ToString + vbCrLf)
            strBuilder.Append("Date: " + objEntry.DateTime.ToString + vbCrLf)
            strBuilder.Append("Size: (-1, if the size information is in the footer)" + vbCrLf)
            strBuilder.Append(vbTab + "Uncompressed: " + objEntry.Size.ToString + vbCrLf)
            strBuilder.Append(vbTab + "Compressed: " + objEntry.CompressedSize.ToString + vbCrLf)

            Dim nSize As Integer = 2048
            Dim abyData(2048) As Byte

            If (True = chkShowEntry.Checked) Then
                While True
                    nSize = strmZipInputStream.Read(abyData, 0, abyData.Length)

                    If nSize > 0 Then
                        '    strBuilder.Append(New ASCIIEncoding().GetString(abyData, 0, nSize) + vbCrLf)
                    Else
                        Exit While
                    End If
                End While
            End If

            objEntry = strmZipInputStream.GetNextEntry()
        End While

        txtContent.Text = strBuilder.ToString
        strmZipInputStream.Close()

    End Sub
#End Region
End Class
