Imports DevComponents.DotNetBar.Controls
Imports DevComponents.DotNetBar
Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX

Public Class F1_CategoriaContable
#Region "Variables Locales"
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public _Tipo As Integer
    Dim NumiVendedor As Integer
    Public Limpiar As Boolean = False
    Public CuentaContableId As Integer
#End Region
#Region "Metodos Privados"

    Private Sub _prIniciarTodo()
        Me.Text = "REGISTRO CATEGORIAS"
        _prAsignarPermisos()

        _PMIniciarTodo()

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

#End Region
#Region "METODOS SOBRECARGADOS"

    Public Overrides Sub _PMOHabilitar()
        txtDescripcionCategoria.ReadOnly = False

        swEstadoS.IsReadOnly = False

        btnCuentaContable.Visible = True

        Limpiar = False
    End Sub

    Public Overrides Sub _PMOInhabilitar()
        txtDescripcionCategoria.ReadOnly = True
        txtCuentaContable.ReadOnly = True
        swEstadoS.IsReadOnly = True
        btnCuentaContable.Visible = False
        _prStyleJanus()
        JGrM_Buscador.Focus()
    End Sub

    Public Overrides Sub _PMOLimpiar()
        txtDescripcionCategoria.Clear()
        txtCuentaContable.Clear()
        txtId.Clear()
        CuentaContableId = 0

        swEstadoS.Value = True

        txtDescripcionCategoria.Focus()
    End Sub

    Public Overrides Sub _PMOLimpiarErrores()
        MEP.Clear()
        txtDescripcionCategoria.BackColor = Color.White
    End Sub

    Public Overrides Function _PMOGrabarRegistro() As Boolean

        Dim res As Boolean = L_fnGrabarCategoriasContables(txtId.Text, txtDescripcionCategoria.Text, "",
                                                           CuentaContableId, IIf(swEstadoS.Value = True, 1, 0))


        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de categoria ".ToUpper + txtId.Text + " Grabado con éxito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            txtDescripcionCategoria.Focus()
            Limpiar = True
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Categoria no pudo ser insertada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
        Return res

    End Function

    Public Overrides Function _PMOModificarRegistro() As Boolean
        Dim res As Boolean
        res = L_fnModificarCategoriasContables(txtId.Text, txtDescripcionCategoria.Text, "",
                                                           CuentaContableId, IIf(swEstadoS.Value = True, 1, 0))
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Servicio ".ToUpper + txtId.Text + " modificado con éxito.".ToUpper,
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
        Return txtId.Text = String.Empty And txtDescripcionCategoria.ReadOnly = False
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
            Dim res As Boolean = L_fnEliminarCategoriaContable(txtId.Text, mensajeError)
            If res Then

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
                ToastNotification.Show(Me, "Código del categoria ".ToUpper + txtId.Text + " eliminada con éxito.".ToUpper,
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

        If txtDescripcionCategoria.Text = String.Empty Then
            txtDescripcionCategoria.BackColor = Color.Red
            MEP.SetError(txtDescripcionCategoria, "ingrese el nombre de la categoria!".ToUpper)
            _ok = False
        Else
            txtDescripcionCategoria.BackColor = Color.White
            MEP.SetError(txtDescripcionCategoria, "")
        End If
        If CuentaContableId = 0 Then
            txtCuentaContable.BackColor = Color.Red
            MEP.SetError(txtCuentaContable, "seleccione una cuenta contable!".ToUpper)
            _ok = False
        Else
            txtCuentaContable.BackColor = Color.White
            MEP.SetError(txtCuentaContable, "")
        End If

        MHighlighterFocus.UpdateHighlights()
        Return _ok
    End Function

    Public Overrides Function _PMOGetTablaBuscador() As DataTable
        Dim dtBuscador As DataTable = L_fnListarCategoriasContables()
        Return dtBuscador
    End Function
    'yiId, yiDesc, Estado, yiFecha, yiHora, yiUsuario
    Public Overrides Function _PMOGetListEstructuraBuscador() As List(Of Modelo.Celda)
        Dim listEstCeldas As New List(Of Modelo.Celda)
        'a.Id , a.NombreCategoria, a.DescripcionContable, isnull(a.CuentaContableId, 0) As CuentaContableId,
        ' isnull(cuenta.cadesc,'') as NombreContable,Estado
        listEstCeldas.Add(New Modelo.Celda("Id", True, "Id".ToUpper, 120))
        listEstCeldas.Add(New Modelo.Celda("NombreCategoria", True, "Categoria".ToUpper, 300))
        listEstCeldas.Add(New Modelo.Celda("DescripcionContable", False))
        listEstCeldas.Add(New Modelo.Celda("CuentaContableId", False))
        listEstCeldas.Add(New Modelo.Celda("NombreContable", True, "NroCuenta".ToUpper, 400))
        listEstCeldas.Add(New Modelo.Celda("Estado", True, "Estado".ToUpper, 100))
        Return listEstCeldas
    End Function

    Public Overrides Sub _PMOMostrarRegistro(_N As Integer)
        JGrM_Buscador.Row = _MPos

        Dim dt As DataTable = CType(JGrM_Buscador.DataSource, DataTable)
        Try
            txtId.Text = JGrM_Buscador.GetValue("Id").ToString
        Catch ex As Exception
            Exit Sub
        End Try
        With JGrM_Buscador
            'a.Id , a.NombreCategoria, a.DescripcionContable, isnull(a.CuentaContableId, 0) As CuentaContableId,
            ' isnull(cuenta.cadesc,'') as NombreContable,Estado
            txtId.Text = .GetValue("Id").ToString
            txtDescripcionCategoria.Text = .GetValue("NombreCategoria").ToString
            swEstadoS.Value = .GetValue("Estado")
            CuentaContableId = .GetValue("CuentaContableId")
            txtCuentaContable.Text = .GetValue("NombreContable").ToString

        End With
        LblPaginacion.Text = Str(_MPos + 1) + "/" + JGrM_Buscador.RowCount.ToString

    End Sub

#End Region
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
        Return txtDescripcionCategoria.ReadOnly = False
    End Function

    Private Sub F1_Servicio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _prIniciarTodo()
    End Sub

    Private Sub txtCuentaContable_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCuentaContable.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarCuentaContables()

                Dim listEstCeldas As New List(Of Modelo.Celda)
                'cuenta.canumi , cuenta.cacta, cuenta.cadesc 
                listEstCeldas.Add(New Modelo.Celda("canumi,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("cacta", True, "Nro Cuenta", 180))
                listEstCeldas.Add(New Modelo.Celda("cadesc", True, "Descripción", 280))

                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                Modelo.MGlobal.SeleccionarCol = 3
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "Seleccione Cuenta Contable".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                    CuentaContableId = Row.Cells("canumi").Value
                    'tbCliente.Text = Row.Cells("ydrazonsocial").Value
                    txtCuentaContable.Text = Row.Cells("cacta").Value.ToString + "  " + Row.Cells("cadesc").Value.ToString

                End If

            End If

        End If
    End Sub

    Private Sub btCliente_Click(sender As Object, e As EventArgs) Handles btnCuentaContable.Click
        If (_fnAccesible()) Then

            Dim dt As DataTable
                dt = L_fnListarCuentaContables()

                Dim listEstCeldas As New List(Of Modelo.Celda)
                'cuenta.canumi , cuenta.cacta, cuenta.cadesc 
                listEstCeldas.Add(New Modelo.Celda("canumi,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("cacta", True, "Nro Cuenta", 180))
                listEstCeldas.Add(New Modelo.Celda("cadesc", True, "Descripción", 280))

                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                Modelo.MGlobal.SeleccionarCol = 3
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "Seleccione Cuenta Contable".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                    CuentaContableId = Row.Cells("canumi").Value
                'tbCliente.Text = Row.Cells("ydrazonsocial").Value
                txtCuentaContable.Text = Row.Cells("cacta").Value.ToString + "  " + Row.Cells("cadesc").Value.ToString


            End If

        End If
    End Sub
End Class