Imports System
Imports System.IO
Imports ICSharpCode.SharpZipLib.BZip2

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
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents btnExecute As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents rdCompress As System.Windows.Forms.RadioButton
    Friend WithEvents rdDecompress As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnBrowseForBZ As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.btnExecute = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.rdDecompress = New System.Windows.Forms.RadioButton()
        Me.rdCompress = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnBrowseForBZ = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(72, 16)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.Size = New System.Drawing.Size(200, 20)
        Me.txtFileName.TabIndex = 0
        Me.txtFileName.Text = ""
        '
        'btnExecute
        '
        Me.btnExecute.Location = New System.Drawing.Point(160, 48)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(112, 22)
        Me.btnExecute.TabIndex = 2
        Me.btnExecute.Text = "Execute"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.AddRange(New System.Windows.Forms.Control() {Me.rdDecompress, Me.rdCompress})
        Me.GroupBox1.Location = New System.Drawing.Point(8, 40)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(144, 80)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Operation to perform"
        '
        'rdDecompress
        '
        Me.rdDecompress.Location = New System.Drawing.Point(8, 48)
        Me.rdDecompress.Name = "rdDecompress"
        Me.rdDecompress.TabIndex = 1
        Me.rdDecompress.Text = "decompress"
        '
        'rdCompress
        '
        Me.rdCompress.Checked = True
        Me.rdCompress.Location = New System.Drawing.Point(8, 24)
        Me.rdCompress.Name = "rdCompress"
        Me.rdCompress.TabIndex = 0
        Me.rdCompress.TabStop = True
        Me.rdCompress.Text = "compress"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 16)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Filename:"
        '
        'btnBrowseForBZ
        '
        Me.btnBrowseForBZ.Location = New System.Drawing.Point(280, 16)
        Me.btnBrowseForBZ.Name = "btnBrowseForBZ"
        Me.btnBrowseForBZ.Size = New System.Drawing.Size(24, 22)
        Me.btnBrowseForBZ.TabIndex = 5
        Me.btnBrowseForBZ.Text = "..."
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(306, 133)
        Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.btnBrowseForBZ, Me.Label1, Me.GroupBox1, Me.btnExecute, Me.txtFileName})
        Me.Name = "Form1"
        Me.Text = "Mini BZ2 Application"
        Me.GroupBox1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub btnExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecute.Click
        If (False = rdCompress.Checked) Then
            ' Decompression of single-file archive
            Dim fsBZ2Archive As FileStream, fsOutput As FileStream
            Dim strOutputFilename As String

            fsBZ2Archive = File.OpenRead(txtFileName.Text)
            strOutputFilename = Path.GetDirectoryName(txtFileName.Text) & _
                Path.GetFileNameWithoutExtension(txtFileName.Text)

            fsOutput = File.Create(strOutputFilename)

            BZip2.Decompress(fsBZ2Archive, fsOutput)

            fsBZ2Archive.Close()
            fsOutput.Flush()
            fsOutput.Close()
        Else
            'Compression of single-file archive
            Dim fsInputFile As FileStream, fsBZ2Archive As FileStream
            fsInputFile = File.OpenRead(txtFileName.Text)
            fsBZ2Archive = File.Create(txtFileName.Text + ".bz")

            BZip2.Compress(fsInputFile, fsBZ2Archive, 4026)

            fsInputFile.Close()
            ' fsBZ2Archive.Flush() & fsBZ2Archive.Close() are automatically called by .Compress
        End If
    End Sub

    Private Sub btnBrowseForBZ_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseForBZ.Click
        Dim ofn As New OpenFileDialog()

        ofn.InitialDirectory = "c:\"
        ofn.Filter = "BZ files (*.bz)|*.bz|All files (*.*)|*.*"

        If ofn.ShowDialog() = DialogResult.OK Then
            txtFileName.Text = ofn.FileName
        End If
    End Sub
End Class
