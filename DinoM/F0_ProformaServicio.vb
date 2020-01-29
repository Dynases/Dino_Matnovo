Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports System.IO
Imports DevComponents.DotNetBar.SuperGrid
Imports GMap.NET.MapProviders
Imports GMap.NET
Imports GMap.NET.WindowsForms.Markers
Imports GMap.NET.WindowsForms
Imports GMap.NET.WindowsForms.ToolTips
Imports System.Drawing
Imports DevComponents.DotNetBar.Controls
Imports System.Threading
Imports System.Drawing.Text
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Drawing.Printing
Imports CrystalDecisions.Shared
Imports Facturacion
Public Class F0_ProformaServicio
    Private Sub F0_ProformaServicio_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
    End Sub
#Region "Variables Globales"
    Dim _CodCliente As Integer = 0
    Dim _CodObra As Integer = 0
    Dim _CodEmpleado As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
#End Region

#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        MSuperTabControl.SelectedTabIndex = 0
        Me.WindowState = FormWindowState.Maximized
        _prCargarProforma()
        _prInhabiliitar()
        Gr_Busqueda.Focus()
        Me.Text = "PROFORMA SERVICIOS"
        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_p), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _prAsignarPermisos()
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
    Private Sub _prInhabiliitar()
        tbCodigo.ReadOnly = True
        tbCliente.ReadOnly = True
        tbObra.ReadOnly = True
        tbVendedor.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFechaVenta.IsInputReadOnly = True
        swMoneda.IsReadOnly = True
        tbDireccion.ReadOnly = True
        tbContacto.ReadOnly = True
        tbTelefono.ReadOnly = True
        tbObra.ReadOnly = False
        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True
        tbSubTotal.IsInputReadOnly = True
        tbtotal.IsInputReadOnly = True
        tbMdesc.IsInputReadOnly = True
        Gr_Busqueda.Enabled = True
        PanelNavegacion.Enabled = True
        Gr_Detalle.RootTable.Columns("img").Visible = False
        If (GPanelProductos.Visible = True) Then
            _DesHabilitarProductos()
        End If

    End Sub

    Private Sub _prhabilitar()
        Gr_Busqueda.Enabled = False
        tbCodigo.ReadOnly = False
        ''  tbCliente.ReadOnly = False  por que solo podra seleccionar Cliente
        ''  tbVendedor.ReadOnly = False
        tbObservacion.ReadOnly = False
        tbObra.ReadOnly = True
        tbFechaVenta.IsInputReadOnly = False
        swMoneda.IsReadOnly = False
        btnGrabar.Enabled = True
        tbMdesc.IsInputReadOnly = False

    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarProforma()
        If Gr_Busqueda.RowCount > 0 Then
            _Mpos = 0
            Gr_Busqueda.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        tbCodigo.Clear()
        tbCliente.Clear()
        tbObra.Clear()
        tbVendedor.Clear()
        tbObservacion.Clear()
        swMoneda.Value = True
        _CodCliente = 0
        _CodEmpleado = 0
        tbFechaVenta.Value = Now.Date
        _prCargarDetalleVenta(-1)
        MSuperTabControl.SelectedTabIndex = 0
        tbSubTotal.Value = 0
        tbPdesc.Value = 0
        tbMdesc.Value = 0
        tbtotal.Value = 0
        tbObra.Clear()
        tbContacto.Clear()
        tbDireccion.Clear()
        tbTelefono.Clear()
        With Gr_Detalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar"
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = True
        End With
        _prAddDetalleVenta()
        If (GPanelProductos.Visible = True) Then
            GPanelProductos.Visible = False
            PanelTotal.Visible = True
            PanelInferior.Visible = True
        End If
        tbCliente.Focus()
        btCliente.Visible = True
        btObra.Visible = True
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)
        With Gr_Busqueda
            tbCodigo.Text = .GetValue("pcnumi")
            tbFechaVenta.Value = .GetValue("pcFDoc")
            _CodEmpleado = .GetValue("pcVen")
            tbVendedor.Text = .GetValue("Vendedor")
            _CodCliente = .GetValue("pcClie")
            tbCliente.Text = .GetValue("Cliente")
            _CodObra = .GetValue("pcObra")
            tbObra.Text = .GetValue("oanomb")
            swMoneda.Value = .GetValue("pcMone")
            tbObservacion.Text = .GetValue("pcObs")
            tbContacto.Text = .GetValue("oacontacto")
            tbDireccion.Text = .GetValue("oadir")
            tbTelefono.Text = .GetValue("oatelf")

            lbFecha.Text = CType(.GetValue("pcfact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("pchact").ToString
            lbUsuario.Text = .GetValue("pcuact").ToString
        End With
        _prCargarDetalleVenta(tbCodigo.Text)
        tbMdesc.Value = Gr_Busqueda.GetValue("pcDesc")
        _prCalcularPrecioTotal()
        LblPaginacion.Text = Str(Gr_Busqueda.Row + 1) + "/" + Gr_Busqueda.RowCount.ToString
        btCliente.Visible = False
        btObra.Visible = False
    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = L_fnDetalle_ProformaServicio(_numi)
        Gr_Detalle.DataSource = dt
        Gr_Detalle.RetrieveStructure()
        Gr_Detalle.AlternatingColors = True

        With Gr_Detalle.RootTable.Columns("pdnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With

        With Gr_Detalle.RootTable.Columns("pdEst")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Detalle.RootTable.Columns("pdNumiProf")
            .Width = 90
            .Visible = False
        End With
        With Gr_Detalle.RootTable.Columns("pdNumiServ")
            .Caption = "COD. ORIG."
            .Width = 80
            .Visible = False
        End With
        With Gr_Detalle.RootTable.Columns("Servicio")
            .Caption = "PRODUCTOS"
            .Width = 250
            .Visible = True
        End With


        With Gr_Detalle.RootTable.Columns("pdCantidad")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With

        With Gr_Detalle.RootTable.Columns("pdNumiUnidad")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With Gr_Detalle.RootTable.Columns("Unidad")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "Unidad".ToUpper
        End With

        With Gr_Detalle.RootTable.Columns("pdPrecio")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio U.".ToUpper
        End With
        With Gr_Detalle.RootTable.Columns("pdSubTotal")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Sub Total".ToUpper
        End With
        With Gr_Detalle.RootTable.Columns("pdPorc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Desc(%)".ToUpper
        End With
        With Gr_Detalle.RootTable.Columns("pdDesc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Desc".ToUpper
        End With

        With Gr_Detalle.RootTable.Columns("pdTotal")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
        End With
        With Gr_Detalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Detalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        With Gr_Detalle
            .GroupByBoxVisible = False
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub
    Private Sub _prCargarProforma()
        Dim dt As New DataTable
        dt = L_fnEncabezado_ProformaServicio()
        Gr_Busqueda.DataSource = dt
        Gr_Busqueda.RetrieveStructure()
        Gr_Busqueda.AlternatingColors = True
        With Gr_Busqueda.RootTable.Columns("pcnumi")
            .Width = 90
            .Caption = "CODIGO"
            .Visible = True
        End With
        With Gr_Busqueda.RootTable.Columns("pcClie")
            .Width = 90
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("Cliente")
            .Width = 150
            .Caption = "CLIENTE"
            .Visible = True
        End With
        With Gr_Busqueda.RootTable.Columns("pcVen")
            .Width = 250
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("Vendedor")
            .Width = 150
            .Caption = "VENDEDOR".ToUpper
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pcObra")
            .Width = 50
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("oanomb")
            .Width = 170
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "OBRA".ToUpper
        End With
        With Gr_Busqueda.RootTable.Columns("oacontacto")
            .Width = 130
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "CONTACTO"
        End With
        With Gr_Busqueda.RootTable.Columns("oadir")
            .Width = 50
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("oatelf")
            .Width = 150
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pcFDoc")
            .Width = 130
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "FECHA"
        End With
        With Gr_Busqueda.RootTable.Columns("pcObs")
            .Width = 50
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pcMone")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pcDesc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With Gr_Busqueda.RootTable.Columns("pctotal")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "TOTAL"
            .FormatString = "0.00"
        End With
        With Gr_Busqueda.RootTable.Columns("pcfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pchact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Busqueda.RootTable.Columns("pcuact")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
        End With
        With Gr_Busqueda
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
        End With
        If (dt.Rows.Count <= 0) Then
            _prCargarDetalleVenta(-1)
        End If
    End Sub
    Private Sub _prCargarProductos()
        Dim dt As New DataTable
        dt = L_fnMostrarServicio()
        Gr_Servicios.DataSource = dt
        Gr_Servicios.RetrieveStructure()
        Gr_Servicios.AlternatingColors = True
        With Gr_Servicios.RootTable.Columns("yiId")
            .Width = 150
            .Caption = "Código"
            .Visible = True
        End With
        With Gr_Servicios.RootTable.Columns("yiDesc")
            .Width = 400
            .Caption = "Descripcionn"
            .Visible = True
        End With
        With Gr_Servicios.RootTable.Columns("yiEst")
            .Width = 270
            .Visible = False
            .Caption = "yiEst"
        End With
        With Gr_Servicios.RootTable.Columns("Estado")
            .Width = 150
            .Visible = False
            .Caption = "Estado"
        End With

        With Gr_Servicios.RootTable.Columns("yiUnidad")
            .Width = 160
            .Visible = False
        End With


        With Gr_Servicios.RootTable.Columns("Unidad")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Caption = "Unidad"
            .Visible = True
        End With

        With Gr_Servicios.RootTable.Columns("yiFecha")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With Gr_Servicios.RootTable.Columns("yiHora")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With Gr_Servicios.RootTable.Columns("yiUsuario")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With Gr_Servicios
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub
    Private Sub _prAddDetalleVenta()
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(Gr_Detalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 1, 0, 0, "", 0, 0, "", 0, 0, 0, 0, 0, 0, Bin.GetBuffer)
    End Sub
    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(Gr_Detalle.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("pdnumi=MAX(pdnumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("pdnumi")
        End If
        Return 1
    End Function
    Public Function _fnAccesible()
        Return tbFechaVenta.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        GPanelProductos.Height = 530
        GPanelProductos.Visible = True
        _prCargarProductos()
        Gr_Servicios.Focus()
        Gr_Servicios.MoveTo(Gr_Servicios.FilterRow)
        Gr_Servicios.Col = 2
    End Sub
    Private Sub _DesHabilitarProductos()
        GPanelProductos.Visible = False
        PanelTotal.Visible = True
        PanelInferior.Visible = True
        Gr_Detalle.Select()
        Gr_Detalle.Col = 5
        Gr_Detalle.Row = Gr_Detalle.RowCount - 1
    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(Gr_Detalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(Gr_Detalle.DataSource, DataTable).Rows(i).Item("pdnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next
    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(Gr_Detalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(Gr_Detalle.DataSource, DataTable).Rows(i).Item("pdNumiServ")
            Dim estado As Integer = CType(Gr_Detalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < Gr_Detalle.RowCount) Then

            Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            'Dim cant As Double = Gr_Detalle.GetValue("pdCantidad")
            Dim cant As Double = Gr_Detalle.GetValue("pdCantidad")
            Dim uni As Double = Gr_Detalle.GetValue("pdPrecio")
            Dim MontoDesc As Double = Gr_Detalle.GetValue("pdDesc")
            Dim dt As DataTable = CType(Gr_Detalle.DataSource, DataTable)
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni

                'grDetalle.SetValue("lcmdes", montodesc)

                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal") = TotalUnitario
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = TotalUnitario - MontoDesc
                Gr_Detalle.SetValue("pdSubTotal", TotalUnitario)
                Gr_Detalle.SetValue("pdTotal", TotalUnitario - MontoDesc)


                Dim estado As Integer = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("estado")
                If (estado = 1) Then
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
            End If
            _prCalcularPrecioTotal()
        End If



    End Sub
    Public Sub _prCalcularPrecioTotal()
        Dim montodesc As Double = tbMdesc.Value
        Dim pordesc As Double = ((montodesc * 100) / Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdTotal"), AggregateFunction.Sum))
        tbPdesc.Value = pordesc
        tbSubTotal.Value = Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdTotal"), AggregateFunction.Sum)

        tbtotal.Value = Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdTotal"), AggregateFunction.Sum) - montodesc

    End Sub
    Public Sub _prEliminarFila()
        If (Gr_Detalle.Row >= 0) Then
            If (Gr_Detalle.RowCount >= 2) Then
                Dim estado As Integer = Gr_Detalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                _fnObtenerFilaDetalle(pos, lin)
                If (estado = 0) Then
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("estado") = -2

                End If
                If (estado = 1) Then
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                Gr_Detalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(Gr_Detalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                _prCalcularPrecioTotal()
                Gr_Detalle.Select()
                Gr_Detalle.Col = 5
                Gr_Detalle.Row = Gr_Detalle.RowCount - 1
            End If
        End If


    End Sub
    Public Function _ValidarCampos() As Boolean
        If (_CodCliente <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Cliente con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbCliente.Focus()
            Return False

        End If
        If (_CodEmpleado <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Vendedor con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False
        End If
        If (_CodObra <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione una Obra con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False
        End If
        'Validar datos de factura
        If (Gr_Detalle.RowCount = 1) Then
            Gr_Detalle.Row = Gr_Detalle.RowCount - 1
            If (Gr_Detalle.GetValue("pdNumiServ") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de Servicio".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return False
            End If

        End If
        Return True
    End Function

    Public Sub _GuardarNuevo()
        Dim res As Boolean = L_fnGrabar_ProformaServicios(tbCodigo.Text, _CodCliente, _CodEmpleado, _CodObra, tbFechaVenta.Value.ToString("yyyy/MM/dd"), IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbtotal.Value, CType(Gr_Detalle.DataSource, DataTable))
        If res Then
            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            '_prImprimirReporte()
            _prCargarProforma()
            _Limpiar()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Proforma no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If

    End Sub
    Private Sub _prGuardarModificado()
        Dim res As Boolean = L_fnModificar_ProformaServicio(tbCodigo.Text, _CodCliente, _CodEmpleado, _CodObra, tbFechaVenta.Value.ToString("yyyy/MM/dd"), IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbtotal.Value, CType(Gr_Detalle.DataSource, DataTable))
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            ' _prImprimirReporte()
            _prCargarProforma()
            _prSalir()
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser Modificada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If
    End Sub
    Private Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _prInhabiliitar()
            If Gr_Busqueda.RowCount > 0 Then
                _prMostrarRegistro(0)
            End If
        Else
            _modulo.Select()
            _tab.Close()
        End If
    End Sub
    Public Sub _prCargarIconELiminar()
        For i As Integer = 0 To CType(Gr_Detalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.delete, 28, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(Gr_Detalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
            Gr_Detalle.RootTable.Columns("img").Visible = True
        Next

    End Sub
    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If Gr_Busqueda.RowCount > 0 Then
            _MPos = 0
            Gr_Busqueda.Row = _MPos
        End If
    End Sub
#End Region
#Region "Eventos Formulario"
    Private Sub F0_Ventas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _IniciarTodo()
    End Sub
    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _Limpiar()
        _prhabilitar()
        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
        PanelNavegacion.Enabled = False
    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()

    End Sub

    Private Sub tbCliente_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCliente.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarClientes()
                Dim listEstCeldas As New List(Of Modelo.Celda)
                listEstCeldas.Add(New Modelo.Celda("ydnumi,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("ydcod", True, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("ydrazonsocial", True, "RAZON SOCIAL", 180))
                listEstCeldas.Add(New Modelo.Celda("yddesc", True, "NOMBRE", 280))
                listEstCeldas.Add(New Modelo.Celda("yddctnum", True, "N. Documento".ToUpper, 150))
                listEstCeldas.Add(New Modelo.Celda("yddirec", True, "DIRECCION", 220))
                listEstCeldas.Add(New Modelo.Celda("ydtelf1", True, "Telefono".ToUpper, 200))
                listEstCeldas.Add(New Modelo.Celda("ydfnac", True, "F.Nacimiento".ToUpper, 150, "MM/dd,YYYY"))
                listEstCeldas.Add(New Modelo.Celda("ydnumivend,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("pcVen,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("yddias", False, "CRED", 50))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "Seleccione Cliente".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                    _CodCliente = Row.Cells("ydnumi").Value
                    tbCliente.Text = Row.Cells("ydrazonsocial").Value
                    '_dias = Row.Cells("yddias").Value

                    Dim numiVendedor As Integer = IIf(IsDBNull(Row.Cells("ydnumivend").Value), 0, Row.Cells("ydnumivend").Value)
                    If (numiVendedor > 0) Then
                        _CodEmpleado = Row.Cells("ydnumivend").Value
                        tbVendedor.Text = Row.Cells("Vendedor").Value
                        tbObra.Focus()
                    Else
                        tbVendedor.Clear()
                        _CodEmpleado = 0
                        tbObra.Focus()
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub tbVendedor_KeyDown(sender As Object, e As KeyEventArgs) Handles tbVendedor.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarEmpleado()
                Dim listEstCeldas As New List(Of Modelo.Celda)
                listEstCeldas.Add(New Modelo.Celda("ydnumi,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("ydcod", True, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("yddesc", True, "NOMBRE", 280))
                listEstCeldas.Add(New Modelo.Celda("yddctnum", True, "N. Documento".ToUpper, 150))
                listEstCeldas.Add(New Modelo.Celda("yddirec", True, "DIRECCION", 220))
                listEstCeldas.Add(New Modelo.Celda("ydtelf1", True, "Telefono".ToUpper, 200))
                listEstCeldas.Add(New Modelo.Celda("ydfnac", True, "F.Nacimiento".ToUpper, 150, "MM/dd,YYYY"))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                ef.SeleclCol = 1
                ef.listEstCeldas = listEstCeldas
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "Seleccione Vendedor".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
                    If (IsNothing(Row)) Then
                        tbVendedor.Focus()
                        Return
                    End If
                    _CodEmpleado = Row.Cells("pcVen").Value
                    tbVendedor.Text = Row.Cells("Vendedor").Value
                    tbObservacion.Focus()

                End If

            End If

        End If
    End Sub


    Private Sub Gr_Detalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles Gr_Detalle.EditingCell
        If (_fnAccesible()) Then
            If (e.Column.Index = Gr_Detalle.RootTable.Columns("pdCantidad").Index Or
                    e.Column.Index = Gr_Detalle.RootTable.Columns("pdPrecio").Index Or
                e.Column.Index = Gr_Detalle.RootTable.Columns("pdPorc").Index Or
                e.Column.Index = Gr_Detalle.RootTable.Columns("pdDesc").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If

    End Sub

    Private Sub Gr_Detalle_Enter(sender As Object, e As EventArgs) Handles Gr_Detalle.Enter

        If (_fnAccesible()) Then
            If (_CodCliente <= 0) Then
                ToastNotification.Show(Me, "           Antes de Continuar Por favor Seleccione un Cliente!!             ", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbCliente.Focus()

                Return
            End If
            If (_CodEmpleado <= 0) Then

                ToastNotification.Show(Me, "           Antes de Continuar Por favor Seleccione un Vendedor!!             ", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbVendedor.Focus()
                Return

            End If
            If (_CodObra <= 0) Then

                ToastNotification.Show(Me, "           Antes de Continuar Por favor Seleccione una Obra!!             ", My.Resources.WARNING, 4000, eToastGlowColor.Red, eToastPosition.TopCenter)
                tbVendedor.Focus()
                Return

            End If

            Gr_Detalle.Select()
            Gr_Detalle.Col = 1
            Gr_Detalle.Row = 0
        End If


    End Sub
    Private Sub Gr_Detalle_KeyDown(sender As Object, e As KeyEventArgs) Handles Gr_Detalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = Gr_Detalle.Col
            f = Gr_Detalle.Row

            If (Gr_Detalle.Col = Gr_Detalle.RootTable.Columns("pdCantidad").Index) Then
                If (Gr_Detalle.GetValue("Servicio") <> String.Empty) Then
                    _prAddDetalleVenta()
                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
            If (Gr_Detalle.Col = Gr_Detalle.RootTable.Columns("Servicio").Index) Then
                If (Gr_Detalle.GetValue("Servicio") <> String.Empty) Then
                    _prAddDetalleVenta()
                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If

            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And Gr_Detalle.Row >= 0 And
            Gr_Detalle.Col = Gr_Detalle.RootTable.Columns("Servicio").Index) Then
            Dim indexfil As Integer = Gr_Detalle.Row
            Dim indexcol As Integer = Gr_Detalle.Col
            _HabilitarProductos()

        End If
        If (e.KeyData = Keys.Escape And Gr_Detalle.Row >= 0) Then

            _prEliminarFila()


        End If


    End Sub
    Public Sub InsertarProductosSinLote()
        Dim pos As Integer = -1
        Gr_Detalle.Row = Gr_Detalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, Gr_Detalle.GetValue("pdnumi"))
        Dim existe As Boolean = _fnExisteProducto(Gr_Servicios.GetValue("yiId"))
        If ((pos >= 0) And (Not existe)) Then
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdNumiServ") = Gr_Servicios.GetValue("yiId")
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("Servicio") = Gr_Servicios.GetValue("yiDesc")
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdNumiUnidad") = Gr_Servicios.GetValue("yiUnidad")
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("Unidad") = Gr_Servicios.GetValue("Unidad")
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdCantidad") = 1
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPrecio") = 1
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal") = 1
            CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = 1
            _prCalcularPrecioTotal()
            _DesHabilitarProductos()
        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El Servicio ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If
    End Sub

    Private Sub Gr_Servicios_KeyDown(sender As Object, e As KeyEventArgs) Handles Gr_Servicios.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = Gr_Servicios.Col
            f = Gr_Servicios.Row
            If (f >= 0) Then
                InsertarProductosSinLote()
            End If
        End If
        If e.KeyData = Keys.Escape Then
            _DesHabilitarProductos()
        End If
    End Sub
    Private Sub Gr_Detalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles Gr_Detalle.CellValueChanged
        Dim codprod As Integer
        If (e.Column.Index = Gr_Detalle.RootTable.Columns("pdCantidad").Index) Then
            If (Not IsNumeric(Gr_Detalle.GetValue("pdCantidad")) Or Gr_Detalle.GetValue("pdCantidad").ToString = String.Empty) Then

                Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdCantidad") = 1
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPrecio")

                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPrecio")
                'Gr_Detalle.SetValue("pdCantidad", 1)
                'Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))
            Else
                If (Gr_Detalle.GetValue("pdCantidad") > 0) Then
                    Dim rowIndex As Integer = Gr_Detalle.Row
                    Dim porcdesc As Double = Gr_Detalle.GetValue("pdPorc")
                    Dim montodesc As Double = ((Gr_Detalle.GetValue("pdPrecio") * Gr_Detalle.GetValue("pdCantidad")) * (porcdesc / 100))
                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = montodesc
                    Gr_Detalle.SetValue("pdDesc", montodesc)

                    'Saca la conversión de cada Servicio
                    codprod = Gr_Detalle.GetValue("pdNumiServ")
                    Dim dtconv As New DataTable
                    dtconv = L_fnConversionProd(codprod)
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdCantidad") = 1
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPrecio")
                    _prCalcularPrecioTotal()
                    'Gr_Detalle.SetValue("pdCantidad", 1)
                    'Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))

                End If
            End If
        End If
        '''''''''''''''''''''PORCENTAJE DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = Gr_Detalle.RootTable.Columns("pdPorc").Index) Then
            If (Not IsNumeric(Gr_Detalle.GetValue("pdPorc")) Or Gr_Detalle.GetValue("pdPorc").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal")
                'Gr_Detalle.SetValue("pdCantidad", 1)
                'Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))
            Else
                If (Gr_Detalle.GetValue("pdPorc") > 0 And Gr_Detalle.GetValue("pdPorc") <= 100) Then

                    Dim porcdesc As Double = Gr_Detalle.GetValue("pdPorc")
                    Dim montodesc As Double = (Gr_Detalle.GetValue("pdSubTotal") * (porcdesc / 100))
                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = montodesc
                    Gr_Detalle.SetValue("pdDesc", montodesc)

                    Dim rowIndex As Integer = Gr_Detalle.Row
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = 0
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = 0
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal")
                    Gr_Detalle.SetValue("pdPorc", 0)
                    Gr_Detalle.SetValue("pdDesc", 0)
                    Gr_Detalle.SetValue("pdTotal", Gr_Detalle.GetValue("pdSubTotal"))
                    _prCalcularPrecioTotal()
                    'Gr_Detalle.SetValue("pdCantidad", 1)
                    'Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))

                End If
            End If
        End If


        '''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = Gr_Detalle.RootTable.Columns("pdDesc").Index) Then
            If (Not IsNumeric(Gr_Detalle.GetValue("pdDesc")) Or Gr_Detalle.GetValue("pdDesc").ToString = String.Empty) Then

                'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
                '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
                Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = 0
                CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal")
                'Gr_Detalle.SetValue("pdCantidad", 1)
                'Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))
            Else
                If (Gr_Detalle.GetValue("pdDesc") > 0 And Gr_Detalle.GetValue("pdDesc") <= Gr_Detalle.GetValue("pdSubTotal")) Then

                    Dim montodesc As Double = Gr_Detalle.GetValue("pdDesc")
                    Dim pordesc As Double = ((montodesc * 100) / Gr_Detalle.GetValue("pdSubTotal"))

                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = montodesc
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = pordesc

                    Gr_Detalle.SetValue("pdPorc", pordesc)
                    Dim rowIndex As Integer = Gr_Detalle.Row
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPorc") = 0
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdDesc") = 0
                    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal")
                    Gr_Detalle.SetValue("pdPorc", 0)
                    Gr_Detalle.SetValue("pdDesc", 0)
                    Gr_Detalle.SetValue("pdTotal", Gr_Detalle.GetValue("pdSubTotal"))
                    _prCalcularPrecioTotal()
                End If
            End If
        End If

    End Sub
    Private Sub tbPdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbPdesc.ValueChanged
        If (tbPdesc.Focused) Then
            If (Not tbPdesc.Text = String.Empty And Not tbtotal.Text = String.Empty) Then
                If (tbPdesc.Value = 0 Or tbPdesc.Value > 100) Then
                    tbPdesc.Value = 0
                    tbMdesc.Value = 0

                    _prCalcularPrecioTotal()

                Else

                    Dim porcdesc As Double = tbPdesc.Value
                    Dim montodesc As Double = (Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdSubTotal"), AggregateFunction.Sum) * (porcdesc / 100))
                    tbMdesc.Value = montodesc

                    tbtotal.Value = Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdSubTotal"), AggregateFunction.Sum) - montodesc
                End If


            End If
            If (tbPdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If
    End Sub

    Private Sub tbMdesc_ValueChanged(sender As Object, e As EventArgs) Handles tbMdesc.ValueChanged
        If (tbMdesc.Focused) Then

            Dim total As Double = tbtotal.Value
            If (Not tbMdesc.Text = String.Empty And Not tbMdesc.Text = String.Empty) Then
                If (tbMdesc.Value = 0 Or tbMdesc.Value > total) Then
                    tbMdesc.Value = 0
                    tbPdesc.Value = 0
                    _prCalcularPrecioTotal()
                Else
                    Dim montodesc As Double = tbMdesc.Value
                    Dim pordesc As Double = ((montodesc * 100) / Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdSubTotal"), AggregateFunction.Sum))
                    tbPdesc.Value = pordesc

                    tbtotal.Value = Gr_Detalle.GetTotal(Gr_Detalle.RootTable.Columns("pdSubTotal"), AggregateFunction.Sum) - montodesc

                End If

            End If

            If (tbMdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If

    End Sub


    Private Sub Gr_Detalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles Gr_Detalle.CellEdited
        If (e.Column.Index = Gr_Detalle.RootTable.Columns("pdCantidad").Index) Then
            If (Not IsNumeric(Gr_Detalle.GetValue("pdCantidad")) Or Gr_Detalle.GetValue("pdCantidad").ToString = String.Empty) Then

                Gr_Detalle.SetValue("pdCantidad", 1)
                Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))
                Gr_Detalle.SetValue("pdPorc", 0)
                Gr_Detalle.SetValue("pdDesc", 0)
                Gr_Detalle.SetValue("pdTotal", Gr_Detalle.GetValue("pdPrecio"))


            Else
                If (Gr_Detalle.GetValue("pdCantidad") > 0) Then

                    Dim cant As Integer = Gr_Detalle.GetValue("pdCantidad")

                    'If (cant > stock) Then
                    '    Dim lin As Integer = Gr_Detalle.GetValue("pdnumi")
                    '    Dim pos As Integer = -1
                    '    _fnObtenerFilaDetalle(pos, lin)
                    '    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdCantidad") = 1
                    '    CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdSubTotal") = CType(Gr_Detalle.DataSource, DataTable).Rows(pos).Item("pdPrecio")


                    '    Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
                    '    ToastNotification.Show(Me, "La cantidad de la venta no debe ser mayor al del stock" & vbCrLf &
                    '    "Stock=" + Str(stock).ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                    '    Gr_Detalle.SetValue("pdCantidad", 1)
                    '    Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))

                    '    _prCalcularPrecioTotal()
                    'Else
                    '    If (cant = stock) Then


                    '        'Gr_Detalle.SelectedFormatStyle.ForeColor = Color.Blue
                    '        'Gr_Detalle.CurrentRow.Cells.Item(e.Column).FormatStyle = New GridEXFormatStyle
                    '        'Gr_Detalle.CurrentRow.Cells(e.Column).FormatStyle.BackColor = Color.Pink
                    '        'Gr_Detalle.CurrentRow.Cells.Item(e.Column).FormatStyle.BackColor = Color.DodgerBlue
                    '        'Gr_Detalle.CurrentRow.Cells.Item(e.Column).FormatStyle.ForeColor = Color.White
                    '        'Gr_Detalle.CurrentRow.Cells.Item(e.Column).FormatStyle.FontBold = TriState.True
                    '    End If
                    'End If

                Else

                    Gr_Detalle.SetValue("pdCantidad", 1)
                    Gr_Detalle.SetValue("pdSubTotal", Gr_Detalle.GetValue("pdPrecio"))
                    Gr_Detalle.SetValue("pdPorc", 0)
                    Gr_Detalle.SetValue("pdDesc", 0)
                    Gr_Detalle.SetValue("pdTotal", Gr_Detalle.GetValue("pdPrecio"))

                End If
            End If
        End If
    End Sub
    Private Sub Gr_Detalle_MouseClick(sender As Object, e As MouseEventArgs) Handles Gr_Detalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If
        If (Gr_Detalle.RowCount >= 2) Then
            If (Gr_Detalle.CurrentColumn.Index = Gr_Detalle.RootTable.Columns("img").Index) Then
                _prEliminarFila()
            End If
        End If


    End Sub

    Sub _prImprimirReporte()
        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "mensaje principal".ToUpper
        ef.Header = "¿DESEA IMPRIMIR REPORTE DE LA PROFORMA INSERTADA?".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporte()
        End If
    End Sub
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click

        If _ValidarCampos() = False Then
            Exit Sub
        End If

        If (tbCodigo.Text = String.Empty) Then
            _GuardarNuevo()
        Else
            If (tbCodigo.Text <> String.Empty) Then
                _prGuardarModificado()
                ''    _prInhabiliitar()

            End If
        End If

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If (Gr_Busqueda.RowCount > 0) Then
            _prhabilitar()
            btnNuevo.Enabled = False
            btnModificar.Enabled = False
            btnEliminar.Enabled = False
            btnGrabar.Enabled = True

            PanelNavegacion.Enabled = False
            _prCargarIconELiminar()
        End If
    End Sub
    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminar_ProformaServicio(tbCodigo.Text, mensajeError)
            If res Then
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

                _prFiltrar()

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If

    End Sub

    Private Sub Gr_Busqueda_SelectionChanged(sender As Object, e As EventArgs) Handles Gr_Busqueda.SelectionChanged
        If (Gr_Busqueda.RowCount >= 0 And Gr_Busqueda.Row >= 0) Then
            _prMostrarRegistro(Gr_Busqueda.Row)
        End If
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = Gr_Busqueda.Row
        If _pos < Gr_Busqueda.RowCount - 1 And _pos >= 0 Then
            _pos = Gr_Busqueda.Row + 1
            '' _prMostrarRegistro(_pos)
            Gr_Busqueda.Row = _pos
        End If
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = Gr_Busqueda.Row
        If Gr_Busqueda.RowCount > 0 Then
            _pos = Gr_Busqueda.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            Gr_Busqueda.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = Gr_Busqueda.Row
        If _MPos > 0 And Gr_Busqueda.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            Gr_Busqueda.Row = _MPos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub
    Private Sub Gr_Busqueda_KeyDown(sender As Object, e As KeyEventArgs) Handles Gr_Busqueda.KeyDown
        If e.KeyData = Keys.Enter Then
            MSuperTabControl.SelectedTabIndex = 0
            Gr_Detalle.Focus()

        End If
    End Sub
#End Region



    Public Function P_fnImageToByteArray(ByVal imageIn As Image) As Byte()
        Dim ms As New System.IO.MemoryStream()
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
        Return ms.ToArray()
    End Function



    Private Sub swTipoVenta_KeyDown(sender As Object, e As KeyEventArgs)
        If (e.KeyData = Keys.Enter) Then

            Gr_Detalle.Select()

            Gr_Detalle.Col = 3
            Gr_Detalle.Row = 0

            'Gr_Detalle.Focus()
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs)
        If (Not _fnAccesible()) Then
            P_GenerarReporte()

        End If
    End Sub
    Private Sub P_GenerarReporte()
        Dim dt As DataTable = L_fnReporteProforma(tbCodigo.Text)

        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        P_Global.Visualizador = New Visualizador

        Dim objrep As New R_Proforma
        '' GenerarNro(_dt)
        ''objrep.SetDataSource(Dt1Kardex)
        objrep.SetDataSource(dt)
        objrep.SetParameterValue("usuario", gs_user)
        P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
        P_Global.Visualizador.Show() 'Comentar
        P_Global.Visualizador.BringToFront() 'Comentar
    End Sub

    Private Sub cbSucursal_Leave(sender As Object, e As EventArgs)
        Gr_Detalle.Select()
    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If (Not _fnAccesible()) Then
            P_GenerarReporte()

        End If
    End Sub

    Private Sub btCliente_Click(sender As Object, e As EventArgs) Handles btCliente.Click
        P_Principal.btConfFabrica_Click(sender, e)
    End Sub

    Private Sub btObra_Click(sender As Object, e As EventArgs) Handles btObra.Click
        P_Principal.btObras_Click(sender, e)
    End Sub
    Private Sub tbObra_KeyDown(sender As Object, e As KeyEventArgs) Handles tbObra.KeyDown

        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dtObra As DataTable
                dtObra = L_fnListarObras()

                Dim listEstCeldas As New List(Of Modelo.Celda)
                listEstCeldas.Add(New Modelo.Celda("oanumi,", True, "Id", 70))
                listEstCeldas.Add(New Modelo.Celda("oanomb", True, "Obra", 250))
                listEstCeldas.Add(New Modelo.Celda("oatipo", False, "Tipo", 180))
                listEstCeldas.Add(New Modelo.Celda("oadir", False, "Dirección", 280))
                listEstCeldas.Add(New Modelo.Celda("oacontacto", True, "Contacto", 150))
                listEstCeldas.Add(New Modelo.Celda("oatelf", False, "Teléfono", 220))
                listEstCeldas.Add(New Modelo.Celda("oaobs", False, "Obs", 220))
                listEstCeldas.Add(New Modelo.Celda("oalat", False, "Latitud", 200))
                listEstCeldas.Add(New Modelo.Celda("oalongi", False, "Longitud", 150, "MM/dd,YYYY"))
                listEstCeldas.Add(New Modelo.Celda("oaest,", False, "Estado", 50))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dtObra
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 120
                ef.ancho = 280
                ef.Context = "Seleccione Obra".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
                    _CodObra = Row.Cells("oanumi").Value
                    tbObra.Text = Row.Cells("oanomb").Value
                    tbContacto.Text = Row.Cells("oacontacto").Value
                    tbDireccion.Text = Row.Cells("oadir").Value
                    tbTelefono.Text = Row.Cells("oatelf").Value
                    Gr_Detalle.Select()
                    Gr_Detalle.Col = 4
                    Gr_Detalle.Row = 0
                End If

            End If

        End If
    End Sub
End Class