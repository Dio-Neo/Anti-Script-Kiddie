<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        Me.TableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
        Me.Panel = New System.Windows.Forms.Panel()
        Me.BtnScan = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LvStartup = New System.Windows.Forms.ListView()
        Me.LvProcess = New System.Windows.Forms.ListView()
        Me.TableLayoutPanel.SuspendLayout()
        Me.Panel.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel
        '
        Me.TableLayoutPanel.ColumnCount = 1
        Me.TableLayoutPanel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel.Controls.Add(Me.Panel, 0, 1)
        Me.TableLayoutPanel.Controls.Add(Me.TableLayoutPanel1, 0, 0)
        Me.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel.Name = "TableLayoutPanel"
        Me.TableLayoutPanel.RowCount = 2
        Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 76.0!))
        Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel.Size = New System.Drawing.Size(1000, 562)
        Me.TableLayoutPanel.TabIndex = 0
        '
        'Panel
        '
        Me.Panel.Controls.Add(Me.BtnScan)
        Me.Panel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel.Location = New System.Drawing.Point(3, 489)
        Me.Panel.Name = "Panel"
        Me.Panel.Size = New System.Drawing.Size(994, 70)
        Me.Panel.TabIndex = 1
        '
        'BtnScan
        '
        Me.BtnScan.Location = New System.Drawing.Point(9, 20)
        Me.BtnScan.Name = "BtnScan"
        Me.BtnScan.Size = New System.Drawing.Size(106, 23)
        Me.BtnScan.TabIndex = 0
        Me.BtnScan.Text = "Scan"
        Me.BtnScan.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 1
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.40845!))
        Me.TableLayoutPanel1.Controls.Add(Me.LvStartup, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.LvProcess, 0, 1)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(3, 3)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 216.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(994, 480)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'LvStartup
        '
        Me.LvStartup.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LvStartup.Location = New System.Drawing.Point(3, 3)
        Me.LvStartup.Name = "LvStartup"
        Me.LvStartup.Size = New System.Drawing.Size(988, 258)
        Me.LvStartup.TabIndex = 0
        Me.LvStartup.UseCompatibleStateImageBehavior = False
        '
        'LvProcess
        '
        Me.LvProcess.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LvProcess.Location = New System.Drawing.Point(3, 267)
        Me.LvProcess.Name = "LvProcess"
        Me.LvProcess.Size = New System.Drawing.Size(988, 210)
        Me.LvProcess.TabIndex = 1
        Me.LvProcess.UseCompatibleStateImageBehavior = False
        '
        'FrmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1000, 562)
        Me.Controls.Add(Me.TableLayoutPanel)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FrmMain"
        Me.Text = "Anti Script Kiddie"
        Me.TableLayoutPanel.ResumeLayout(False)
        Me.Panel.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel As TableLayoutPanel
    Friend WithEvents Panel As Panel
    Friend WithEvents BtnScan As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents LvProcess As ListView
    Friend WithEvents LvStartup As ListView
End Class
