Imports System
Imports System.Text
Imports System.Collections
Imports System.IO
Imports System.Windows.Forms
Imports ICSharpCode.SharpZipLib.Zip

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
    Friend WithEvents btnTest As System.Windows.Forms.Button
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblListName As System.Windows.Forms.Label
    Friend WithEvents chdrRawSize As System.Windows.Forms.ColumnHeader
    Friend WithEvents chdrSize As System.Windows.Forms.ColumnHeader
    Friend WithEvents hdrDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents hdrTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chdrName As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents lvZipContents As System.Windows.Forms.ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.btnTest = New System.Windows.Forms.Button()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblListName = New System.Windows.Forms.Label()
        Me.lvZipContents = New System.Windows.Forms.ListView()
        Me.chdrRawSize = New System.Windows.Forms.ColumnHeader()
        Me.chdrSize = New System.Windows.Forms.ColumnHeader()
        Me.hdrDate = New System.Windows.Forms.ColumnHeader()
        Me.hdrTime = New System.Windows.Forms.ColumnHeader()
        Me.chdrName = New System.Windows.Forms.ColumnHeader()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnTest
        '
        Me.btnTest.Location = New System.Drawing.Point(344, 23)
        Me.btnTest.Name = "btnTest"
        Me.btnTest.Size = New System.Drawing.Size(68, 21)
        Me.btnTest.TabIndex = 1
        Me.btnTest.Text = "View"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(117, 23)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(197, 20)
        Me.txtFileName.TabIndex = 0
        Me.txtFileName.Text = ""
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(16, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(91, 21)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Zip File Name:"
        '
        'lblListName
        '
        Me.lblListName.Location = New System.Drawing.Point(16, 53)
        Me.lblListName.Name = "lblListName"
        Me.lblListName.Size = New System.Drawing.Size(256, 22)
        Me.lblListName.TabIndex = 5
        Me.lblListName.Text = "(no file)"
        '
        'lvZipContents
        '
        Me.lvZipContents.Anchor = (((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right)
        Me.lvZipContents.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chdrName, Me.chdrRawSize, Me.chdrSize, Me.hdrDate, Me.hdrTime})
        Me.lvZipContents.FullRowSelect = True
        Me.lvZipContents.GridLines = True
        Me.lvZipContents.Location = New System.Drawing.Point(16, 80)
        Me.lvZipContents.Name = "lvZipContents"
        Me.lvZipContents.Size = New System.Drawing.Size(400, 200)
        Me.lvZipContents.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvZipContents.TabIndex = 6
        Me.lvZipContents.View = System.Windows.Forms.View.Details
        '
        'chdrRawSize
        '
        Me.chdrRawSize.Text = "RawSize"
        Me.chdrRawSize.Width = 67
        '
        'chdrSize
        '
        Me.chdrSize.Text = "Size"
        Me.chdrSize.Width = 52
        '
        'hdrDate
        '
        Me.hdrDate.Text = "Date"
        Me.hdrDate.Width = 71
        '
        'hdrTime
        '
        Me.hdrTime.Text = "Time"
        Me.hdrTime.Width = 58
        '
        'chdrName
        '
        Me.chdrName.Text = "Name"
        Me.chdrName.Width = 127
        '
        'btnBrowse
        '
        Me.btnBrowse.Location = New System.Drawing.Point(320, 23)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(20, 21)
        Me.btnBrowse.TabIndex = 7
        Me.btnBrowse.Text = "..."
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(420, 283)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.btnBrowse, Me.lvZipContents, Me.lblListName, Me.Label1, Me.txtFileName, Me.btnTest})
        Me.Name = "Form1"
        Me.Text = "Test Zip File"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Payload Implementation"

    Public Sub btnTest_Click(ByVal Sender As Object, ByVal EA As EventArgs) Handles btnTest.Click
        Dim objEntry As ZipEntry
        Dim dtStamp As DateTime
        Dim objItem As ListViewItem
        Dim zFile As ZipFile

        ' Really simple error handling here (catch all)
        Try
            zFile = New ZipFile(txtFileName.Text)
        Catch Ex As System.Exception
            MessageBox.Show(Ex.Message)
            Exit Sub
        End Try

        lblListName.Text = "Listing of : " + zFile.Name.ToString
        lvZipContents.BeginUpdate()
        lvZipContents.Items.Clear()

        For Each objEntry In zFile
            objItem = New ListViewItem(objEntry.Name)
            dtStamp = objEntry.DateTime
            objItem.SubItems.Add(objEntry.Size.ToString)
            objItem.SubItems.Add(objEntry.CompressedSize.ToString)
            objItem.SubItems.Add(dtStamp.ToString("dd-MM-yy"))
            objItem.SubItems.Add(dtStamp.ToString("t"))
            objItem.SubItems.Add(objEntry.Name.ToString)
            lvZipContents.Items.Add(objItem)
        Next

        lvZipContents.EndUpdate()
    End Sub

#End Region

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim ofn As New OpenFileDialog()

        ofn.InitialDirectory = "c:\"
        ofn.Filter = "Zip Files (*.zip)|*.zip|All files (*.*)|*.*"

        If ofn.ShowDialog() = DialogResult.OK Then
            txtFileName.Text = ofn.FileName
        End If
    End Sub
End Class
