Imports System
Imports System.IO
Imports ICSharpCode.SharpZipLib.Checksums
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
    Friend WithEvents txtSourceDir As System.Windows.Forms.TextBox
    Friend WithEvents btnZipIt As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtZipFileName As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtSourceDir = New System.Windows.Forms.TextBox()
        Me.btnZipIt = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtZipFileName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'txtSourceDir
        '
        Me.txtSourceDir.Location = New System.Drawing.Point(128, 16)
        Me.txtSourceDir.Name = "txtSourceDir"
        Me.txtSourceDir.Size = New System.Drawing.Size(216, 20)
        Me.txtSourceDir.TabIndex = 0
        Me.txtSourceDir.Text = ""
        '
        'btnZipIt
        '
        Me.btnZipIt.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnZipIt.Location = New System.Drawing.Point(360, 16)
        Me.btnZipIt.Name = "btnZipIt"
        Me.btnZipIt.Size = New System.Drawing.Size(69, 22)
        Me.btnZipIt.TabIndex = 2
        Me.btnZipIt.Text = "ZipIt"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(15, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(113, 22)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Directory to put in zip:"
        '
        'txtZipFileName
        '
        Me.txtZipFileName.Location = New System.Drawing.Point(128, 40)
        Me.txtZipFileName.Name = "txtZipFileName"
        Me.txtZipFileName.Size = New System.Drawing.Size(216, 20)
        Me.txtZipFileName.TabIndex = 1
        Me.txtZipFileName.Text = ""
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(16, 40)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(91, 22)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Name of zip file:"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(448, 98)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.Label2, Me.txtZipFileName, Me.Label1, Me.btnZipIt, Me.txtSourceDir})
        Me.Name = "Form1"
        Me.Text = "Create Zip File"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "Payload Implementation"
    Public Sub btnZipIt_Click(ByVal sender As System.Object, ByVal EA As System.EventArgs) Handles btnZipIt.Click
        Dim astrFileNames() As String = Directory.GetFiles(txtSourceDir.Text)
        Dim objCrc32 As New Crc32()
        Dim strmZipOutputStream As ZipOutputStream

        strmZipOutputStream = New ZipOutputStream(File.Create(txtZipFileName.Text))
        strmZipOutputStream.SetLevel(6)

        REM Compression Level: 0-9
        REM 0: no(Compression)
        REM 9: maximum compression

        Dim strFile As String

        For Each strFile In astrFileNames
            Dim strmFile As FileStream = File.OpenRead(strFile)
            Dim abyBuffer(strmFile.Length - 1) As Byte

            strmFile.Read(abyBuffer, 0, abyBuffer.Length)
            Dim objZipEntry As ZipEntry = New ZipEntry(strFile)

            objZipEntry.DateTime = DateTime.Now
            objZipEntry.Size = strmFile.Length
            strmFile.Close()
            objCrc32.Reset()
            objCrc32.Update(abyBuffer)
            objZipEntry.Crc = objCrc32.Value
            strmZipOutputStream.PutNextEntry(objZipEntry)
            strmZipOutputStream.Write(abyBuffer, 0, abyBuffer.Length)

        Next

        strmZipOutputStream.Finish()
        strmZipOutputStream.Close()

        MessageBox.Show("Operation complete")
    End Sub

    Public Shared Sub ZipFile(ByVal strFileToZip As String, ByVal strZippedFile As String, ByVal nCompressionLevel As Integer, ByVal nBlockSize As Integer)
        If (Not System.IO.File.Exists(strFileToZip)) Then
            Throw New System.IO.FileNotFoundException("The specified file " + strFileToZip + "could not be found. Zipping aborted.")
        End If

        Dim strmStreamToZip As System.IO.FileStream
        strmStreamToZip = New System.IO.FileStream(strFileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read)

        Dim strmZipFile As System.IO.FileStream
        strmZipFile = System.IO.File.Create(strZippedFile)

        Dim strmZipStream As ZipOutputStream
        strmZipStream = New ZipOutputStream(strmZipFile)

        Dim myZipEntry As ZipEntry
        myZipEntry = New ZipEntry("ZippedFile")
        strmZipStream.PutNextEntry(myZipEntry)
        strmZipStream.SetLevel(nCompressionLevel)

        Dim abyBuffer(nBlockSize) As Byte
        Dim nSize As System.Int32
        nSize = strmStreamToZip.Read(abyBuffer, 0, abyBuffer.Length)
        strmZipStream.Write(abyBuffer, 0, nSize)

        Try
            While (nSize < strmStreamToZip.Length)
                Dim nSizeRead As Integer
                nSizeRead = strmStreamToZip.Read(abyBuffer, 0, abyBuffer.Length)
                strmZipStream.Write(abyBuffer, 0, nSizeRead)
                nSize = nSize + nSizeRead
            End While

        Catch Ex As System.Exception
            Throw Ex

        End Try

        strmZipStream.Finish()
        strmZipStream.Close()
        strmStreamToZip.Close()
    End Sub
#End Region

End Class
