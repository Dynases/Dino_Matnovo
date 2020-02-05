Imports Logica.AccesoLogica
Imports DevComponents.DotNetBar
Imports Janus.Windows.GridEX
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports DevComponents.DotNetBar.Controls

Public Class F1_Obras
#Region "Variables Locales"
#Region "MApas"
    Dim _Punto As Integer
    Dim _ListPuntos As List(Of PointLatLng)
    Dim _Overlay As GMapOverlay
    Dim _latitud As Double = 0
    Dim _longitud As Double = 0
#End Region
    Dim RutaGlobal As String = gs_CarpetaRaiz
    Dim RutaTemporal As String = "C:\Temporal"
    Dim Modificado As Boolean = False
    Dim nameImg As String = "Default.jpg"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public _Tipo As Integer
    Dim NumiVendedor As Integer

    Public banderaobra As Boolean = False
#End Region
#Region "Metodos Privados"

    Private Sub _prIniciarTodo()

        Me.Text = "REGISTRO OBRAS"
        _prInicarMapa()
        _prMaxLength()
        _prCargarComboLibreria(cbTipoObra, 7, 1)

        _prAsignarPermisos()
        _PMIniciarTodo()
        'SuperTabItem1.Visible = False

        Dim blah As New Bitmap(New Bitmap(My.Resources.construccion), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico

    End Sub
    Private Sub P_IniciarMap()
        Gmc_Cliente.DragButton = MouseButtons.Left
        Gmc_Cliente.CanDragMap = True
        Gmc_Cliente.MapProvider = GMapProviders.GoogleMap
        If (_latitud <> 0 And _longitud <> 0) Then
            Gmc_Cliente.Position = New PointLatLng(_latitud, _longitud)
        Else
            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-17.3931784, -66.1738852)
        End If

        Gmc_Cliente.MinZoom = 0
        Gmc_Cliente.MaxZoom = 24
        Gmc_Cliente.Zoom = 15.5
        Gmc_Cliente.AutoScroll = True

        GMapProvider.Language = LanguageType.Spanish
    End Sub
    Public Sub _prInicarMapa()
        _Punto = 0
        '_ListPuntos = New List(Of PointLatLng)
        _Overlay = New GMapOverlay("points")
        Gmc_Cliente.Overlays.Add(_Overlay)
        P_IniciarMap()
    End Sub

    Public Sub _prStyleJanus()
        GroupPanelBuscador.Style.BackColor = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.BackColor2 = Color.FromArgb(13, 71, 161)
        GroupPanelBuscador.Style.TextColor = Color.White
        JGrM_Buscador.RootTable.HeaderFormatStyle.FontBold = TriState.True
    End Sub

    Public Sub _prMaxLength()
        tbNombre.MaxLength = 300
        cbTipoObra.MaxLength = 30
        tbDireccion.MaxLength = 200
        tbContacto.MaxLength = 200
        tbTelf1.MaxLength = 50
        tbObs.MaxLength = 200
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

    Private Sub _prCrearCarpetaReportes()
        Dim rutaDestino As String = RutaGlobal + "\Reporte\Reporte ClienteDino\"

        If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte ClienteDino\") = False Then
            If System.IO.Directory.Exists(RutaGlobal + "\Reporte") = False Then
                System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte")
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte ClienteDino") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte ClienteDino")
                End If
            Else
                If System.IO.Directory.Exists(RutaGlobal + "\Reporte\Reporte ClienteDino") = False Then
                    System.IO.Directory.CreateDirectory(RutaGlobal + "\Reporte\Reporte ClienteDino")

                End If
            End If
        End If
    End Sub

#End Region
#Region "METODOS SOBRECARGADOS"

    Public Overrides Sub _PMOHabilitar()
        tbNombre.ReadOnly = False
        cbTipoObra.ReadOnly = False
        tbDireccion.ReadOnly = False
        tbContacto.ReadOnly = False
        tbTelf1.ReadOnly = False
        tbObs.ReadOnly = False

    End Sub

    Public Overrides Sub _PMOInhabilitar()

        tbCodigoOriginal.ReadOnly = True
        tbNombre.ReadOnly = True
        cbTipoObra.ReadOnly = True
        tbDireccion.ReadOnly = True
        tbContacto.ReadOnly = True
        tbTelf1.ReadOnly = True
        tbObs.ReadOnly = True

        _prStyleJanus()
        JGrM_Buscador.Focus()

        ' SuperTabItem1.Visible = False
    End Sub

    Public Overrides Sub _PMOLimpiar()
        tbCodigoOriginal.Clear()
        tbNombre.Clear()
        tbDireccion.Clear()
        tbContacto.Clear()
        tbTelf1.Clear()
        tbObs.Clear()

        _Overlay.Markers.Clear()
        _latitud = 0
        _longitud = 0

        If (cbTipoObra.SelectedIndex < 0) Then
            If (CType(cbTipoObra.DataSource, DataTable).Rows.Count > 0) Then
                cbTipoObra.SelectedIndex = 0
            End If
        End If


    End Sub

    Public Overrides Sub _PMOLimpiarErrores()
        MEP.Clear()
        tbNombre.BackColor = Color.White
        tbDireccion.BackColor = Color.White
    End Sub

    Public Overrides Function _PMOGrabarRegistro() As Boolean
        Dim res As Boolean = L_fnGrabarObras(tbCodigoOriginal.Text, tbNombre.Text, cbTipoObra.Value, tbDireccion.Text, tbContacto.Text, tbTelf1.Text, tbObs.Text, _latitud, _longitud, 1)


        If res Then
            Modificado = False

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Obra ".ToUpper + tbCodigoOriginal.Text + " Grabado con éxito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            tbNombre.Focus()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La obra no pudo ser insertada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        If prof_venta = True Then
            codobra = tbCodigoOriginal.Text
            nomobra = tbNombre.Text
            banderaobra = True
            prof_venta = False
            Me.Close()
            Return res = False
        End If
        Return res
    End Function

    Public Overrides Function _PMOModificarRegistro() As Boolean
        Dim res As Boolean

        If (Modificado = False) Then
            res = L_fnModificarObras(tbCodigoOriginal.Text, tbNombre.Text, cbTipoObra.Value, tbDireccion.Text, tbContacto.Text, tbTelf1.Text, tbObs.Text, _latitud, _longitud, 2)

        Else
            res = L_fnModificarObras(tbCodigoOriginal.Text, tbNombre.Text, cbTipoObra.Value, tbDireccion.Text, tbContacto.Text, tbTelf1.Text, tbObs.Text, _latitud, _longitud, 2)

        End If
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Obra ".ToUpper + tbCodigoOriginal.Text + " modificado con éxito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter)
            _PMInhabilitar()
            _PMPrimerRegistro()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La obra no pudo ser modificada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res
    End Function


    Public Function _fnActionNuevo() As Boolean
        Return tbCodigoOriginal.Text = String.Empty And tbDireccion.ReadOnly = False
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
            Dim res As Boolean = L_fnEliminarObras(tbCodigoOriginal.Text, mensajeError)
            If res Then

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código de la obra ".ToUpper + tbCodigoOriginal.Text + " eliminada con éxito.".ToUpper,
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

        If tbNombre.Text = String.Empty Then
            tbNombre.BackColor = Color.Red
            MEP.SetError(tbNombre, "ingrese el nombre de la obra!".ToUpper)
            _ok = False
        Else
            tbNombre.BackColor = Color.White
            MEP.SetError(tbNombre, "")
        End If
        'If (cbTipoDoc.SelectedIndex < 0) Then

        '    If (CType(cbTipoDoc.DataSource, DataTable).Rows.Count > 0) Then
        '        cbTipoDoc.SelectedIndex = 0
        '    End If
        'End If


        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Overrides Function _PMOGetTablaBuscador() As DataTable
        Dim dtBuscador As DataTable = L_fnGeneralObras()
        Return dtBuscador
    End Function

    Public Overrides Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)
        Dim listEstCeldas As New List(Of Modelo.Celda)

        listEstCeldas.Add(New Modelo.Celda("oanumi", True, "Código".ToUpper, 80))
        listEstCeldas.Add(New Modelo.Celda("oanomb", True, "Nombre Obra".ToUpper, 400))
        listEstCeldas.Add(New Modelo.Celda("oatipo", False))
        listEstCeldas.Add(New Modelo.Celda("oadir", True, "Dirección".ToUpper, 350))
        listEstCeldas.Add(New Modelo.Celda("oacontacto", True, "Contacto".ToUpper, 200))
        listEstCeldas.Add(New Modelo.Celda("oatelf", True, "Teléfono".ToUpper, 120))
        listEstCeldas.Add(New Modelo.Celda("oaobs", False, "Observacion".ToUpper, 180))
        listEstCeldas.Add(New Modelo.Celda("oalat", False))
        listEstCeldas.Add(New Modelo.Celda("oalongi", False))
        listEstCeldas.Add(New Modelo.Celda("oaest", False))
        listEstCeldas.Add(New Modelo.Celda("oafact", False))
        listEstCeldas.Add(New Modelo.Celda("oahact", False))
        listEstCeldas.Add(New Modelo.Celda("oauact", False))

        Return listEstCeldas
    End Function

    Public Overrides Sub _PMOMostrarRegistro(_N As Integer)
        JGrM_Buscador.Row = _MPos

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Try
            tbCodigoOriginal.Text = JGrM_Buscador.GetValue("oanumi").ToString
        Catch ex As Exception
            Exit Sub
        End Try
        With JGrM_Buscador

            tbCodigoOriginal.Text = .GetValue("oanumi").ToString
            tbNombre.Text = .GetValue("oanomb").ToString
            cbTipoObra.Value = .GetValue("oatipo")
            tbDireccion.Text = .GetValue("oadir").ToString
            tbContacto.Text = .GetValue("oacontacto").ToString
            tbTelf1.Text = .GetValue("oatelf").ToString
            tbObs.Text = .GetValue("oaobs").ToString

            _latitud = .GetValue("oalat")
            _longitud = .GetValue("oalongi")

            lbFecha.Text = CType(.GetValue("oafact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("oahact").ToString
            lbUsuario.Text = .GetValue("oauact").ToString

        End With

        _dibujarUbicacion()
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub
    Public Sub _dibujarUbicacion()
        If (_latitud <> 0 And _longitud <> 0) Then
            Dim plg As PointLatLng = New PointLatLng(_latitud, _longitud)
            _Overlay.Markers.Clear()
            P_AgregarPunto(plg)
        Else
            _Overlay.Markers.Clear()
            Gmc_Cliente.Position = New PointLatLng(-17.7833101, -63.1843143)
        End If
    End Sub

    Private Sub P_AgregarPunto(pointLatLng As PointLatLng)
        If (Not IsNothing(_Overlay)) Then
            'añadir puntos
            Dim marker As New GMarkerGoogle(pointLatLng, My.Resources.markerR)
            'añadir tooltip
            Dim mode As MarkerTooltipMode = MarkerTooltipMode.OnMouseOver
            marker.ToolTip = New GMapBaloonToolTip(marker)
            marker.ToolTipMode = mode
            Dim ToolTipBackColor As New SolidBrush(Color.Blue)
            marker.ToolTip.Fill = ToolTipBackColor
            marker.ToolTip.Foreground = Brushes.White

            _Overlay.Markers.Add(marker)
            Gmc_Cliente.Position = pointLatLng
        End If
    End Sub
    Public Overrides Sub _PMOHabilitarFocus()

    End Sub

#End Region

    Private Sub F1_Clientes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub Gmc_Cliente_DoubleClick(sender As Object, e As EventArgs) Handles Gmc_Cliente.DoubleClick
        If (btnGrabar.Enabled = True) Then

            _Overlay.Markers.Clear()

            Dim gm As GMapControl = CType(sender, GMapControl)
            Dim hj As MouseEventArgs = CType(e, MouseEventArgs)
            Dim plg As PointLatLng = gm.FromLocalToLatLng(hj.X, hj.Y)
            _latitud = plg.Lat
            _longitud = plg.Lng

            P_AgregarPunto(plg)
        End If
    End Sub

    Private Sub ButtonX3_Click(sender As Object, e As EventArgs) Handles ButtonX3.Click
        If (Gmc_Cliente.Zoom >= Gmc_Cliente.MinZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom - 1
        End If
    End Sub

    Private Sub ButtonX4_Click(sender As Object, e As EventArgs) Handles ButtonX4.Click
        If (Gmc_Cliente.Zoom <= Gmc_Cliente.MaxZoom) Then
            Gmc_Cliente.Zoom = Gmc_Cliente.Zoom + 1
        End If
    End Sub

    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        If btnGrabar.Enabled = True Then
            _PMInhabilitar()
            _PMPrimerRegistro()

        Else
            '  Public _modulo As SideNavItem
            If prof_venta = True Then
                Close()
            Else
                _modulo.Select()
                _tab.Close()
            End If

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
        Return tbNombre.ReadOnly = False
    End Function

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        P_GenerarReporte()
    End Sub
    Private Sub P_GenerarReporte()
        'Dim dt As DataTable = L_fnReportecliente()

        'If Not IsNothing(P_Global.Visualizador) Then
        '    P_Global.Visualizador.Close()
        'End If

        'P_Global.Visualizador = New Visualizador

        'Dim objrep As New R_Clientes
        ''' GenerarNro(_dt)
        '''objrep.SetDataSource(Dt1Kardex)
        'objrep.SetDataSource(dt)

        'P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        'P_Global.Visualizador.Show() 'Comentar
        'P_Global.Visualizador.BringToFront() 'Comentar

    End Sub

    Private Sub cbTipoObra_ValueChanged(sender As Object, e As EventArgs) Handles cbTipoObra.ValueChanged
        If cbTipoObra.SelectedIndex < 0 And cbTipoObra.Text <> String.Empty Then
            btTipoObra.Visible = True
        Else
            btTipoObra.Visible = False
        End If
    End Sub

    Private Sub btTipoObra_Click(sender As Object, e As EventArgs) Handles btTipoObra.Click
        Dim numi As String = ""
        If L_prLibreriaGrabar(numi, "7", "1", cbTipoObra.Text, "") Then
            _prCargarComboLibreria(cbTipoObra, "7", "1")
            cbTipoObra.SelectedIndex = CType(cbTipoObra.DataSource, DataTable).Rows.Count - 1
        End If
    End Sub
End Class