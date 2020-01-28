Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Public Class F1_Servicio
#Region "Variables Locales"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public _Tipo As Integer
    Dim NumiVendedor As Integer
    Public Limpiar As Boolean = False
#End Region
#Region "Metodos Privados"

    Private Sub _prIniciarTodo()
        Me.Text = "REGISTRO SERVICIOS"
        _prAsignarPermisos()
        _PMIniciarTodo()
        _prCargarComboLibreria(cb_Unidad, 9, 1)
        Dim blah As New Bitmap(New Bitmap(My.Resources.Services), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
    End Sub


    Public Sub _prStyleJanus()
        GroupPanelBuscador.Style.BackColor = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.BackColor2 = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.TextColor = Color.White
        JGrM_Buscador.RootTable.HeaderFormatStyle.FontBold = TriState.True
    End Sub

    Private Sub _prAsignarPermisos()

        Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        If add = False Then
            btnNuevo.Visible = False
        End If
        If modif = False Then
            btnModificar.Visible = False
        End If
        If del = False Then
            btnEliminar.Visible = False
        End If
    End Sub
    Private Sub _prCargarComboLibreria(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo, cod1 As String, cod2 As String)
        Dim dt As New DataTable
        dt = L_prLibreriaClienteLGeneral(cod1, cod2)
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("yccod3").Width = 70
            .DropDownList.Columns("yccod3").Caption = "COD"
            .DropDownList.Columns.Add("ycdes3").Width = 200
            .DropDownList.Columns("ycdes3").Caption = "DESCRIPCION"
            .ValueMember = "yccod3"
            .DisplayMember = "ycdes3"
            .DataSource = dt
            .Refresh()
        End With
    End Sub
#End Region
#Region "METODOS SOBRECARGADOS"

    Public Overrides Sub _PMOHabilitar()
        txtDescripcion.ReadOnly = False
        txtIdServicio.ReadOnly = False
        swEstadoS.Enabled = True
        swEstadoS.Value = 1
        cb_Unidad.Enabled = True
        Limpiar = False
    End Sub

    Public Overrides Sub _PMOInhabilitar()
        txtDescripcion.ReadOnly = True
        txtIdServicio.ReadOnly = True
        swEstadoS.Enabled = False
        cb_Unidad.Enabled = False
        _prStyleJanus()
        JGrM_Buscador.Focus()
    End Sub

    Public Overrides Sub _PMOLimpiar()
        txtDescripcion.Clear()
        txtIdServicio.Clear()
        If (Limpiar = False) Then
            _prSeleccionarCombo(cb_Unidad)
            swEstadoS.Value = 0
        End If
    End Sub

    Public Overrides Sub _PMOLimpiarErrores()
        MEP.Clear()
        txtDescripcion.BackColor = Color.White
    End Sub

    Public Overrides Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean = L_fnGrabarServicio(txtIdServicio.Text, txtDescripcion.Text, IIf(swEstadoS.Value, 1, 2), cb_Unidad.Value)


        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de servicio ".ToUpper + txtIdServicio.Text + " Grabado con éxito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            txtDescripcion.Focus()
            Limpiar = True
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El servicio no pudo ser insertada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res

    End Function

    Public Overrides Function _PMOModificarRegistro() As Boolean
        Dim res As Boolean
        res = L_fnModificarServicio(txtIdServicio.Text, txtDescripcion.Text, IIf(swEstadoS.Value, 1, 2), cb_Unidad.Value)
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Servicio ".ToUpper + txtIdServicio.Text + " modificado con éxito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter)
            _PMInhabilitar()
            _PMPrimerRegistro()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "El Servicio no pudo ser modificada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res
    End Function


    Public Function _fnActionNuevo() As Boolean
        Return txtIdServicio.Text = String.Empty And txtDescripcion.ReadOnly = False
    End Function

    Public Overrides Sub _PMOEliminarRegistro()

        Dim ef = New Efecto

        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarServicio(txtIdServicio.Text, mensajeError)
            If res Then

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código del servicio ".ToUpper + txtIdServicio.Text + " eliminada con éxito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _PMFiltrar()
            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If


    End Sub
    Public Overrides Function _PMOValidarCampos() As Boolean
        Dim _ok As Boolean = True
        MEP.Clear()

        If txtDescripcion.Text = String.Empty Then
            txtDescripcion.BackColor = Color.Red
            MEP.SetError(txtDescripcion, "ingrese el nombre del servicio!".ToUpper)
            _ok = False
        Else
            txtDescripcion.BackColor = Color.White
            MEP.SetError(txtDescripcion, "")
        End If

        If cb_Unidad.SelectedIndex < 0 Then
            cb_Unidad.BackColor = Color.Red
            MEP.SetError(cb_Unidad, "Seleccione una Unidad de servicio!".ToUpper)
            _ok = False
        Else
            cb_Unidad.BackColor = Color.White
            MEP.SetError(cb_Unidad, "")
        End If
        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Overrides Function _PMOGetTablaBuscador() As DataTable
        Dim dtBuscador As DataTable = L_fnMostrarServicio()
        Return dtBuscador
    End Function
    'yiId, yiDesc, Estado, yiFecha, yiHora, yiUsuario
    Public Overrides Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)
        Dim listEstCeldas As New List(Of Modelo.Celda)
        listEstCeldas.Add(New Modelo.Celda("yiId", True, "Código".ToUpper, 120))
        listEstCeldas.Add(New Modelo.Celda("yiDesc", True, "Descripción".ToUpper, 400))
        listEstCeldas.Add(New Modelo.Celda("yiEst", False))
        listEstCeldas.Add(New Modelo.Celda("yiUnidad", False))
        listEstCeldas.Add(New Modelo.Celda("Unidad", True, "Unidad".ToUpper, 200))
        listEstCeldas.Add(New Modelo.Celda("Estado", True, "Estado".ToUpper, 300))
        listEstCeldas.Add(New Modelo.Celda("yiFecha", False))
        listEstCeldas.Add(New Modelo.Celda("yiHora", False))
        listEstCeldas.Add(New Modelo.Celda("yiUsuario", False))
        Return listEstCeldas
    End Function

    Public Overrides Sub _PMOMostrarRegistro(_N As Integer)
        JGrM_Buscador.Row = _MPos

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Try
            txtIdServicio.Text = JGrM_Buscador.GetValue("yiId").ToString
        Catch ex As Exception
            Exit Sub
        End Try
        With JGrM_Buscador

            txtIdServicio.Text = .GetValue("yiId").ToString
            txtDescripcion.Text = .GetValue("yiDesc").ToString
            swEstadoS.Value = IIf(.GetValue("yiEst") = 1, True, False)
            cb_Unidad.Value = .GetValue("yiUnidad")
            lbFecha.Text = CType(.GetValue("yiFecha"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("yiHora").ToString
            lbUsuario.Text = .GetValue("yiUsuario").ToString

        End With
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

#End Region
#Region "Eventos"
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            _modulo.Select()
            _tab.Close()
        End If
    End Sub

    Private Sub JGrM_Buscador_KeyDown(sender As Object, e As KeyEventArgs) Handles JGrM_Buscador.KeyDown
        If e.KeyData = Keys.Enter Then
            If (MPanelSup.Visible = True) Then
                JGrM_Buscador.GroupByBoxVisible = True
                MPanelSup.Visible = False
                JGrM_Buscador.UseGroupRowSelector = True

            Else
                JGrM_Buscador.GroupByBoxVisible = False
                JGrM_Buscador.UseGroupRowSelector = True
                MPanelSup.Visible = True
            End If
        End If
    End Sub
    Function _fnAccesible() As Boolean
        Return txtDescripcion.ReadOnly = False
    End Function

    Private Sub F1_Servicio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub cb_Unidad_ValueChanged(sender As Object, e As EventArgs) Handles cb_Unidad.ValueChanged
        If cb_Unidad.SelectedIndex < 0 And cb_Unidad.Text <> String.Empty Then
            btn_Unidad.Visible = True
        Else
            btn_Unidad.Visible = False
        End If
    End Sub

    Private Sub btn_Unidad_Click(sender As Object, e As EventArgs) Handles btn_Unidad.Click
        Dim numi As String = ""

        If L_prLibreriaGrabar(numi, "9", "1", cb_Unidad.Text, "") Then
            _prCargarComboLibreria(cb_Unidad, "9", "1")
            cb_Unidad.SelectedIndex = CType(cb_Unidad.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub
    Public Sub _prSeleccionarCombo(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        If (CType(mCombo.DataSource, DataTable).Rows.Count > 0) Then
            mCombo.SelectedIndex = 0
        Else
            mCombo.SelectedIndex = -1
        End If
    End Sub
#End Region
End Class