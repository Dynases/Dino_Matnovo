<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F1_CategoriaContable
    Inherits Modelo.ModeloF1

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F1_CategoriaContable))
        Me.txtDescripcionCategoria = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.lbDescripcion = New DevComponents.DotNetBar.LabelX()
        Me.lbIdServicio = New DevComponents.DotNetBar.LabelX()
        Me.txtId = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.txtCuentaContable = New DevComponents.DotNetBar.Controls.TextBoxX()
        Me.LabelX1 = New DevComponents.DotNetBar.LabelX()
        Me.swEstadoS = New DevComponents.DotNetBar.Controls.SwitchButton()
        Me.lbEstado = New DevComponents.DotNetBar.LabelX()
        Me.btnCuentaContable = New DevComponents.DotNetBar.ButtonX()
        CType(Me.SuperTabPrincipal, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuperTabPrincipal.SuspendLayout()
        Me.SuperTabControlPanelRegistro.SuspendLayout()
        Me.PanelSuperior.SuspendLayout()
        Me.PanelInferior.SuspendLayout()
        CType(Me.BubbleBarUsuario, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelToolBar1.SuspendLayout()
        Me.PanelToolBar2.SuspendLayout()
        Me.MPanelSup.SuspendLayout()
        Me.PanelPrincipal.SuspendLayout()
        Me.GroupPanelBuscador.SuspendLayout()
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelUsuario.SuspendLayout()
        Me.PanelNavegacion.SuspendLayout()
        Me.MPanelUserAct.SuspendLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SuperTabPrincipal
        '
        '
        '
        '
        '
        '
        '
        Me.SuperTabPrincipal.ControlBox.CloseBox.Name = ""
        '
        '
        '
        Me.SuperTabPrincipal.ControlBox.MenuBox.Name = ""
        Me.SuperTabPrincipal.ControlBox.Name = ""
        Me.SuperTabPrincipal.ControlBox.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.SuperTabPrincipal.ControlBox.MenuBox, Me.SuperTabPrincipal.ControlBox.CloseBox})
        Me.SuperTabPrincipal.Size = New System.Drawing.Size(1072, 647)
        Me.SuperTabPrincipal.Controls.SetChildIndex(Me.SuperTabControlPanelBuscador, 0)
        Me.SuperTabPrincipal.Controls.SetChildIndex(Me.SuperTabControlPanelRegistro, 0)
        '
        'SuperTabControlPanelBuscador
        '
        Me.SuperTabControlPanelBuscador.Location = New System.Drawing.Point(0, 0)
        Me.SuperTabControlPanelBuscador.Size = New System.Drawing.Size(1037, 647)
        '
        'SuperTabControlPanelRegistro
        '
        Me.SuperTabControlPanelRegistro.Size = New System.Drawing.Size(1037, 647)
        Me.SuperTabControlPanelRegistro.Controls.SetChildIndex(Me.PanelSuperior, 0)
        Me.SuperTabControlPanelRegistro.Controls.SetChildIndex(Me.PanelInferior, 0)
        Me.SuperTabControlPanelRegistro.Controls.SetChildIndex(Me.PanelPrincipal, 0)
        '
        'PanelSuperior
        '
        Me.PanelSuperior.Size = New System.Drawing.Size(1037, 89)
        Me.PanelSuperior.Style.Alignment = System.Drawing.StringAlignment.Center
        Me.PanelSuperior.Style.BackColor1.Color = System.Drawing.Color.DarkSlateGray
        Me.PanelSuperior.Style.BackColor2.Color = System.Drawing.Color.DarkSlateGray
        Me.PanelSuperior.Style.BackgroundImage = CType(resources.GetObject("PanelSuperior.Style.BackgroundImage"), System.Drawing.Image)
        Me.PanelSuperior.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine
        Me.PanelSuperior.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.PanelSuperior.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.PanelSuperior.Style.GradientAngle = 90
        '
        'PanelInferior
        '
        Me.PanelInferior.Location = New System.Drawing.Point(0, 603)
        Me.PanelInferior.Size = New System.Drawing.Size(1037, 44)
        Me.PanelInferior.Style.Alignment = System.Drawing.StringAlignment.Center
        Me.PanelInferior.Style.BackColor1.Color = System.Drawing.Color.DarkSlateGray
        Me.PanelInferior.Style.BackColor2.Color = System.Drawing.Color.DarkSlateGray
        Me.PanelInferior.Style.BackgroundImage = CType(resources.GetObject("PanelInferior.Style.BackgroundImage"), System.Drawing.Image)
        Me.PanelInferior.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine
        Me.PanelInferior.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder
        Me.PanelInferior.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText
        Me.PanelInferior.Style.GradientAngle = 90
        '
        'BubbleBarUsuario
        '
        '
        '
        '
        Me.BubbleBarUsuario.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BackColor = System.Drawing.Color.Transparent
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BorderBottomWidth = 1
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BorderColor = System.Drawing.Color.FromArgb(CType(CType(180, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(245, Byte), Integer))
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BorderLeftWidth = 1
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BorderRightWidth = 1
        Me.BubbleBarUsuario.ButtonBackAreaStyle.BorderTopWidth = 1
        Me.BubbleBarUsuario.ButtonBackAreaStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.BubbleBarUsuario.ButtonBackAreaStyle.PaddingBottom = 3
        Me.BubbleBarUsuario.ButtonBackAreaStyle.PaddingLeft = 3
        Me.BubbleBarUsuario.ButtonBackAreaStyle.PaddingRight = 3
        Me.BubbleBarUsuario.ButtonBackAreaStyle.PaddingTop = 3
        Me.BubbleBarUsuario.MouseOverTabColors.BorderColor = System.Drawing.SystemColors.Highlight
        Me.BubbleBarUsuario.SelectedTabColors.BorderColor = System.Drawing.Color.Black
        '
        'TxtNombreUsu
        '
        Me.TxtNombreUsu.Size = New System.Drawing.Size(267, 44)
        '
        'btnSalir
        '
        '
        'PanelToolBar2
        '
        Me.PanelToolBar2.Location = New System.Drawing.Point(930, 0)
        '
        'MPanelSup
        '
        Me.MPanelSup.Controls.Add(Me.btnCuentaContable)
        Me.MPanelSup.Controls.Add(Me.swEstadoS)
        Me.MPanelSup.Controls.Add(Me.lbEstado)
        Me.MPanelSup.Controls.Add(Me.txtCuentaContable)
        Me.MPanelSup.Controls.Add(Me.LabelX1)
        Me.MPanelSup.Controls.Add(Me.txtDescripcionCategoria)
        Me.MPanelSup.Controls.Add(Me.lbDescripcion)
        Me.MPanelSup.Controls.Add(Me.lbIdServicio)
        Me.MPanelSup.Controls.Add(Me.txtId)
        Me.MPanelSup.Size = New System.Drawing.Size(1037, 226)
        Me.MPanelSup.Controls.SetChildIndex(Me.PanelUsuario, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.txtId, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.lbIdServicio, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.lbDescripcion, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.txtDescripcionCategoria, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.LabelX1, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.txtCuentaContable, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.lbEstado, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.swEstadoS, 0)
        Me.MPanelSup.Controls.SetChildIndex(Me.btnCuentaContable, 0)
        '
        'PanelPrincipal
        '
        Me.PanelPrincipal.Size = New System.Drawing.Size(1037, 514)
        '
        'GroupPanelBuscador
        '
        Me.GroupPanelBuscador.Size = New System.Drawing.Size(1037, 288)
        '
        '
        '
        Me.GroupPanelBuscador.Style.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanelBuscador.Style.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanelBuscador.Style.BackColorGradientAngle = 90
        Me.GroupPanelBuscador.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanelBuscador.Style.BorderBottomWidth = 1
        Me.GroupPanelBuscador.Style.BorderColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.GroupPanelBuscador.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanelBuscador.Style.BorderLeftWidth = 1
        Me.GroupPanelBuscador.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanelBuscador.Style.BorderRightWidth = 1
        Me.GroupPanelBuscador.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid
        Me.GroupPanelBuscador.Style.BorderTopWidth = 1
        Me.GroupPanelBuscador.Style.CornerDiameter = 4
        Me.GroupPanelBuscador.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded
        Me.GroupPanelBuscador.Style.Font = New System.Drawing.Font("Georgia", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.GroupPanelBuscador.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center
        Me.GroupPanelBuscador.Style.TextColor = System.Drawing.Color.White
        Me.GroupPanelBuscador.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near
        '
        '
        '
        Me.GroupPanelBuscador.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.GroupPanelBuscador.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'JGrM_Buscador
        '
        Me.JGrM_Buscador.HeaderFormatStyle.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.HeaderFormatStyle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(72, Byte), Integer), CType(CType(127, Byte), Integer))
        Me.JGrM_Buscador.SelectedFormatStyle.BackColor = System.Drawing.Color.DodgerBlue
        Me.JGrM_Buscador.SelectedFormatStyle.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.JGrM_Buscador.SelectedFormatStyle.ForeColor = System.Drawing.Color.White
        Me.JGrM_Buscador.Size = New System.Drawing.Size(1031, 261)
        '
        'btnUltimo
        '
        Me.btnUltimo.Location = New System.Drawing.Point(171, 0)
        '
        'MPanelUserAct
        '
        Me.MPanelUserAct.Location = New System.Drawing.Point(770, 0)
        '
        'txtDescripcionCategoria
        '
        Me.txtDescripcionCategoria.BackColor = System.Drawing.Color.White
        '
        '
        '
        Me.txtDescripcionCategoria.Border.Class = "TextBoxBorder"
        Me.txtDescripcionCategoria.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.txtDescripcionCategoria.DisabledBackColor = System.Drawing.Color.White
        Me.txtDescripcionCategoria.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDescripcionCategoria.ForeColor = System.Drawing.Color.Black
        Me.txtDescripcionCategoria.Location = New System.Drawing.Point(182, 70)
        Me.txtDescripcionCategoria.Margin = New System.Windows.Forms.Padding(4)
        Me.txtDescripcionCategoria.MaxLength = 150
        Me.txtDescripcionCategoria.Multiline = True
        Me.txtDescripcionCategoria.Name = "txtDescripcionCategoria"
        Me.txtDescripcionCategoria.PreventEnterBeep = True
        Me.txtDescripcionCategoria.Size = New System.Drawing.Size(331, 30)
        Me.txtDescripcionCategoria.TabIndex = 250
        '
        'lbDescripcion
        '
        Me.lbDescripcion.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbDescripcion.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbDescripcion.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbDescripcion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.lbDescripcion.Location = New System.Drawing.Point(22, 71)
        Me.lbDescripcion.Margin = New System.Windows.Forms.Padding(4)
        Me.lbDescripcion.Name = "lbDescripcion"
        Me.lbDescripcion.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbDescripcion.Size = New System.Drawing.Size(108, 28)
        Me.lbDescripcion.TabIndex = 253
        Me.lbDescripcion.Text = "Descripción:"
        '
        'lbIdServicio
        '
        Me.lbIdServicio.AutoSize = True
        Me.lbIdServicio.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbIdServicio.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbIdServicio.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbIdServicio.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.lbIdServicio.Location = New System.Drawing.Point(21, 39)
        Me.lbIdServicio.Margin = New System.Windows.Forms.Padding(4)
        Me.lbIdServicio.Name = "lbIdServicio"
        Me.lbIdServicio.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbIdServicio.Size = New System.Drawing.Size(103, 20)
        Me.lbIdServicio.TabIndex = 252
        Me.lbIdServicio.Text = "Id Categoria:"
        '
        'txtId
        '
        Me.txtId.BackColor = System.Drawing.Color.LightGray
        '
        '
        '
        Me.txtId.Border.Class = "TextBoxBorder"
        Me.txtId.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.txtId.DisabledBackColor = System.Drawing.Color.White
        Me.txtId.Enabled = False
        Me.txtId.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtId.ForeColor = System.Drawing.Color.Black
        Me.txtId.Location = New System.Drawing.Point(182, 36)
        Me.txtId.Margin = New System.Windows.Forms.Padding(4)
        Me.txtId.Name = "txtId"
        Me.txtId.PreventEnterBeep = True
        Me.txtId.Size = New System.Drawing.Size(93, 26)
        Me.txtId.TabIndex = 251
        '
        'txtCuentaContable
        '
        Me.txtCuentaContable.BackColor = System.Drawing.Color.White
        '
        '
        '
        Me.txtCuentaContable.Border.Class = "TextBoxBorder"
        Me.txtCuentaContable.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.txtCuentaContable.DisabledBackColor = System.Drawing.Color.White
        Me.txtCuentaContable.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCuentaContable.ForeColor = System.Drawing.Color.Black
        Me.txtCuentaContable.Location = New System.Drawing.Point(182, 116)
        Me.txtCuentaContable.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCuentaContable.MaxLength = 150
        Me.txtCuentaContable.Multiline = True
        Me.txtCuentaContable.Name = "txtCuentaContable"
        Me.txtCuentaContable.PreventEnterBeep = True
        Me.txtCuentaContable.Size = New System.Drawing.Size(331, 30)
        Me.txtCuentaContable.TabIndex = 254
        '
        'LabelX1
        '
        Me.LabelX1.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.LabelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.LabelX1.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelX1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.LabelX1.Location = New System.Drawing.Point(22, 117)
        Me.LabelX1.Margin = New System.Windows.Forms.Padding(4)
        Me.LabelX1.Name = "LabelX1"
        Me.LabelX1.SingleLineColor = System.Drawing.SystemColors.Control
        Me.LabelX1.Size = New System.Drawing.Size(149, 28)
        Me.LabelX1.TabIndex = 255
        Me.LabelX1.Text = "CuentaContable:"
        '
        'swEstadoS
        '
        Me.swEstadoS.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.swEstadoS.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.swEstadoS.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.swEstadoS.Location = New System.Drawing.Point(182, 164)
        Me.swEstadoS.Margin = New System.Windows.Forms.Padding(4)
        Me.swEstadoS.Name = "swEstadoS"
        Me.swEstadoS.OffBackColor = System.Drawing.Color.Red
        Me.swEstadoS.OffText = "INACTIVO"
        Me.swEstadoS.OffTextColor = System.Drawing.Color.White
        Me.swEstadoS.OnBackColor = System.Drawing.Color.Lime
        Me.swEstadoS.OnText = "ACTIVO"
        Me.swEstadoS.OnTextColor = System.Drawing.Color.White
        Me.swEstadoS.Size = New System.Drawing.Size(181, 27)
        Me.swEstadoS.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.swEstadoS.SwitchBackColor = System.Drawing.Color.Gainsboro
        Me.swEstadoS.SwitchBorderColor = System.Drawing.Color.Black
        Me.swEstadoS.TabIndex = 332
        Me.swEstadoS.Value = True
        Me.swEstadoS.ValueObject = "Y"
        '
        'lbEstado
        '
        Me.lbEstado.BackColor = System.Drawing.Color.Transparent
        '
        '
        '
        Me.lbEstado.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.lbEstado.Font = New System.Drawing.Font("Georgia", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbEstado.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(85, Byte), Integer), CType(CType(139, Byte), Integer))
        Me.lbEstado.Location = New System.Drawing.Point(22, 163)
        Me.lbEstado.Margin = New System.Windows.Forms.Padding(4)
        Me.lbEstado.Name = "lbEstado"
        Me.lbEstado.SingleLineColor = System.Drawing.SystemColors.Control
        Me.lbEstado.Size = New System.Drawing.Size(105, 28)
        Me.lbEstado.TabIndex = 333
        Me.lbEstado.Text = "Estado:"
        '
        'btnCuentaContable
        '
        Me.btnCuentaContable.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton
        Me.btnCuentaContable.BackColor = System.Drawing.Color.Transparent
        Me.btnCuentaContable.ColorTable = DevComponents.DotNetBar.eButtonColor.Flat
        Me.btnCuentaContable.Image = Global.DinoM.My.Resources.Resources.search
        Me.btnCuentaContable.ImageFixedSize = New System.Drawing.Size(25, 25)
        Me.btnCuentaContable.Location = New System.Drawing.Point(521, 116)
        Me.btnCuentaContable.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCuentaContable.Name = "btnCuentaContable"
        Me.btnCuentaContable.Size = New System.Drawing.Size(37, 28)
        Me.btnCuentaContable.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.btnCuentaContable.TabIndex = 377
        '
        'F1_CategoriaContable
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1072, 647)
        Me.Name = "F1_CategoriaContable"
        Me.Text = "F1_CategoriaContable"
        Me.Controls.SetChildIndex(Me.SuperTabPrincipal, 0)
        CType(Me.SuperTabPrincipal, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SuperTabPrincipal.ResumeLayout(False)
        Me.SuperTabControlPanelRegistro.ResumeLayout(False)
        Me.PanelSuperior.ResumeLayout(False)
        Me.PanelInferior.ResumeLayout(False)
        CType(Me.BubbleBarUsuario, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelToolBar1.ResumeLayout(False)
        Me.PanelToolBar2.ResumeLayout(False)
        Me.MPanelSup.ResumeLayout(False)
        Me.MPanelSup.PerformLayout()
        Me.PanelPrincipal.ResumeLayout(False)
        Me.GroupPanelBuscador.ResumeLayout(False)
        CType(Me.JGrM_Buscador, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelUsuario.ResumeLayout(False)
        Me.PanelUsuario.PerformLayout()
        Me.PanelNavegacion.ResumeLayout(False)
        Me.MPanelUserAct.ResumeLayout(False)
        Me.MPanelUserAct.PerformLayout()
        CType(Me.MEP, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents txtDescripcionCategoria As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents lbDescripcion As DevComponents.DotNetBar.LabelX
    Friend WithEvents lbIdServicio As DevComponents.DotNetBar.LabelX
    Friend WithEvents txtId As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents txtCuentaContable As DevComponents.DotNetBar.Controls.TextBoxX
    Friend WithEvents LabelX1 As DevComponents.DotNetBar.LabelX
    Friend WithEvents swEstadoS As DevComponents.DotNetBar.Controls.SwitchButton
    Friend WithEvents lbEstado As DevComponents.DotNetBar.LabelX
    Friend WithEvents btnCuentaContable As DevComponents.DotNetBar.ButtonX
End Class
