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
Public Class F0_VentasServicio
#Region "Variables Globales"
    Dim _CodCliente As Integer = 0
    Dim _CodEmpleado As Integer = 0
    Dim _CodObra As Integer = 0
    Dim OcultarFact As Integer = 0
    Dim _codeBar As Integer = 1
    Dim _dias As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Dim FilaSelectLote As DataRow = Nothing
    Dim Table_Producto As DataTable
    Dim G_Lote As Boolean = False '1=igual a mostrar las columnas de lote y fecha de Vencimiento
    Dim prod As String
    Dim grup1 As String = " "
    Dim grup2 As String = " "

#End Region

#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        MSuperTabControl.SelectedTabIndex = 0
        Me.WindowState = FormWindowState.Maximized
        lbTipoMoneda.Visible = False
        swMoneda.Visible = False
        P_prCargarVariablesIndispensables()
        _prCargarVenta()
        _prInhabiliitar()
        grVentas.Focus()
        Me.Text = "DESPACHOS"
        Dim blah As New Bitmap(New Bitmap(My.Resources.compra), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _prAsignarPermisos()
        P_prCargarParametro()
        _prValidadFactura()
        _prCargarNameLabel()

        'Ocultar paneles de Facturación
        'GroupPanelFactura2.Visible = False
        'GroupPanelFactura.Visible = False
    End Sub
    Public Sub _prCargarNameLabel()
        Dim dt As DataTable = L_fnNameLabel()
        If (dt.Rows.Count > 0) Then
            _codeBar = 1 'dt.Rows(0).Item("codeBar")
        End If
    End Sub
    Sub _prValidadFactura()
        'If (OcultarFact = 1) Then
        '    GroupPanelFactura2.Visible = False
        '    GroupPanelFactura.Visible = False
        'Else
        '    GroupPanelFactura2.Visible = True
        '    GroupPanelFactura.Visible = True
        'End If

    End Sub

    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 500
            .DropDownList.Columns("aabdes").Caption = "SUCURSAL"
            .ValueMember = "aanumi"
            .DisplayMember = "aabdes"
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
    Private Sub _prInhabiliitar()
        SwProforma.IsReadOnly = True
        tbCodigo.ReadOnly = True
        tbCliente.ReadOnly = True
        tbVendedor.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFechaVenta.IsInputReadOnly = True
        tbFechaVenc.IsInputReadOnly = True
        swMoneda.IsReadOnly = True
        swTipoVenta.IsReadOnly = True
        tbObra.ReadOnly = True
        'Datos facturacion
        tbNroAutoriz.ReadOnly = True
        tbNroFactura.ReadOnly = True
        tbCodigoControl.ReadOnly = True
        dtiFechaFactura.IsInputReadOnly = True
        dtiFechaFactura.ButtonDropDown.Enabled = False

        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True

        tbSubTotal.IsInputReadOnly = True
        tbIce.IsInputReadOnly = True
        tbtotal.IsInputReadOnly = True

        grVentas.Enabled = True
        PanelNavegacion.Enabled = True
        grdetalle.RootTable.Columns("img").Visible = False
        If (GPanelProductos.Visible = True) Then
            _DesHabilitarProductos()
        End If

        TbNit.ReadOnly = True
        TbNombre1.ReadOnly = True
        TbNombre2.ReadOnly = True

        FilaSelectLote = Nothing
    End Sub
    Private Sub _prhabilitar()
        SwProforma.IsReadOnly = False
        grVentas.Enabled = False
        tbCodigo.ReadOnly = False
        ''  tbCliente.ReadOnly = False  por que solo podra seleccionar Cliente
        ''  tbVendedor.ReadOnly = False
        tbObservacion.ReadOnly = False
        tbObra.ReadOnly = False
        tbFechaVenta.IsInputReadOnly = False
        tbFechaVenc.IsInputReadOnly = False
        swMoneda.IsReadOnly = False
        swTipoVenta.IsReadOnly = False
        btnGrabar.Enabled = True

        TbNit.ReadOnly = False
        TbNombre1.ReadOnly = False
        TbNombre2.ReadOnly = False

        'Datos facturacion
        tbNroAutoriz.ReadOnly = False
        tbNroFactura.ReadOnly = False
        tbCodigoControl.ReadOnly = False
        dtiFechaFactura.IsInputReadOnly = False
    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarVenta()
        If grVentas.RowCount > 0 Then
            _Mpos = 0
            grVentas.Row = _Mpos
        Else
            _Limpiar()
            LblPaginacion.Text = "0/0"
        End If
    End Sub
    Private Sub _Limpiar()
        tbProforma.Clear()
        tbCodigo.Clear()
        tbCliente.Clear()
        tbVendedor.Clear()
        tbObservacion.Clear()
        tbObra.Clear()
        swMoneda.Value = True
        swTipoVenta.Value = True
        _CodCliente = 0
        _CodEmpleado = 0
        _CodObra = 0
        tbFechaVenta.Value = Now.Date
        tbFechaVenc.Visible = False
        lbCredito.Visible = False
        _prCargarDetalleVenta(-1)
        MSuperTabControl.SelectedTabIndex = 0
        tbSubTotal.Value = 0
        tbPdesc.Value = 0
        tbMdesc.Value = 0
        tbIce.Value = 0
        tbtotal.Value = 0
        With grdetalle.RootTable.Columns("img")
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

        TbNit.Clear()
        TbNombre1.Clear()
        TbNombre2.Clear()

        tbNroAutoriz.Clear()
        tbNroFactura.Clear()
        tbCodigoControl.Clear()
        dtiFechaFactura.Value = Now.Date

        FilaSelectLote = Nothing
        SwProforma.Value = False
        tbCliente.Focus()
        Table_Producto = Nothing
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)


        With grVentas

            tbCodigo.Text = .GetValue("tgnumi")
            tbFechaVenta.Value = .GetValue("tgFdoc")
            _CodEmpleado = .GetValue("tgVen")
            _CodObra = .GetValue("tgObra")
            tbObra.Text = .GetValue("Obra")
            tbVendedor.Text = .GetValue("vendedor")
            swTipoVenta.Value = .GetValue("tgTipoVen")
            _CodCliente = .GetValue("tgCliente")
            tbCliente.Text = .GetValue("cliente")
            swMoneda.Value = .GetValue("tgMone")
            tbObservacion.Text = .GetValue("tgobs")
            swEmision.Value = .GetValue("tgemision")

            Dim proforma As Integer = IIf(IsDBNull(.GetValue("tgProforma")), 0, .GetValue("tgProforma"))
            If (proforma = 0) Then
                SwProforma.Value = False
                tbProforma.Clear()

            Else
                tbProforma.Text = proforma
                SwProforma.Value = True
            End If
            tbFechaVenc.Value = .GetValue("tgFvcr")


            'If (gb_FacturaEmite) Then
            Dim dt As DataTable = L_fnObtenerTabla("TFV001", "fvanitcli, fvadescli1, fvadescli2, fvaautoriz, fvanfac, fvaccont, fvafec", "fvanumi=" + tbCodigo.Text.Trim)
            If (dt.Rows.Count = 1) Then
                TbNit.Text = dt.Rows(0).Item("fvanitcli").ToString
                TbNombre1.Text = dt.Rows(0).Item("fvadescli1").ToString
                TbNombre2.Text = dt.Rows(0).Item("fvadescli2").ToString

                tbNroAutoriz.Text = dt.Rows(0).Item("fvaautoriz").ToString
                tbNroFactura.Text = dt.Rows(0).Item("fvanfac").ToString
                tbCodigoControl.Text = dt.Rows(0).Item("fvaccont").ToString
                dtiFechaFactura.Value = dt.Rows(0).Item("fvafec")
            Else
                TbNit.Clear()
                TbNombre1.Clear()
                TbNombre2.Clear()

                tbNroAutoriz.Clear()
                tbNroFactura.Clear()
                tbCodigoControl.Clear()
                dtiFechaFactura.Value = "2000/01/01"
            End If
            'End If

            lbFecha.Text = CType(.GetValue("tgFact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("tgHact").ToString
            lbUsuario.Text = .GetValue("tgUact").ToString

        End With

        _prCargarDetalleVenta(tbCodigo.Text)
        tbMdesc.Value = grVentas.GetValue("tgDesc")
        _prCalcularPrecioTotal()
        LblPaginacion.Text = Str(grVentas.Row + 1) + "/" + grVentas.RowCount.ToString

    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = L_fnDetalleVentaServicio(_numi)
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True
        ' a.thnumi ,a.thtv4numi ,a.thty8prod, b.yiDesc as producto,a.thEst ,a.thCant ,a.thUni ,Umin .ycdes3 as unidad,
        ' a.thPrec , a.thSubTotal, a.thPorc, a.thDesc, a.thTotal, a.thObs,
        ' a.thCosto, a.thPtot2, a.thFact, a.thHact, a.thUact, 1 As estado, Cast(null As Image) as img
        With grdetalle.RootTable.Columns("thnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thtv4numi")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thty8prod")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thEst")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("producto")
            .Caption = "Productos"
            .Width = 250
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("thCant")
            .Width = 160
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad".ToUpper
        End With
        With grdetalle.RootTable.Columns("thUni")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("unidad")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "Unidad".ToUpper
        End With
        With grdetalle.RootTable.Columns("thPrec")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio U.".ToUpper
        End With
        ' a.thPrec , a.thSubTotal, a.thPorc, a.thDesc, a.thTotal, a.thObs,
        ' a.thCosto, a.thPtot2, a.thFact, a.thHact, a.thUact, 1 As estado, Cast(null As Image) as img
        With grdetalle.RootTable.Columns("thSubTotal")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Sub Total".ToUpper
        End With
        With grdetalle.RootTable.Columns("thPorc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "P.Desc(%)".ToUpper
        End With
        With grdetalle.RootTable.Columns("thDesc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "M.Desc".ToUpper
        End With
        With grdetalle.RootTable.Columns("thTotal")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
        End With
        With grdetalle.RootTable.Columns("thObs")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thCosto")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        ' a.thCosto, a.thPtot2, a.thFact, a.thHact, a.thUact, 1 As estado, Cast(null As Image) as img
        With grdetalle.RootTable.Columns("thPtot2")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thFact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thHact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("thUact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("estado")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("img")
            .Width = 80
            .Caption = "Eliminar".ToUpper
            .CellStyle.ImageHorizontalAlignment = ImageHorizontalAlignment.Center
            .Visible = False
        End With
        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Private Sub _prCargarVenta()
        Dim dt As New DataTable
        dt = L_fnGeneralVentaServicio()
        grVentas.DataSource = dt
        grVentas.RetrieveStructure()
        grVentas.AlternatingColors = True

        With grVentas.RootTable.Columns("tgnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = True

        End With
        With grVentas.RootTable.Columns("tgProforma")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tgObra")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tgfdoc")
            .Width = 90
            .Visible = True
            .Caption = "FECHA"
        End With

        With grVentas.RootTable.Columns("tgVen")
            .Width = 160
            .Visible = False
        End With
        With grVentas.RootTable.Columns("vendedor")
            .Width = 250
            .Visible = True
            .Caption = "VENDEDOR".ToUpper
        End With


        With grVentas.RootTable.Columns("tgTipoVen")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grVentas.RootTable.Columns("tgFvcr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tgCliente")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("cliente")
            .Width = 250
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "CLIENTE"
        End With

        With grVentas.RootTable.Columns("tgMone")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("moneda")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "MONEDA"
        End With
        With grVentas.RootTable.Columns("tgEst")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "tgEst"
        End With
        With grVentas.RootTable.Columns("tgobs")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "OBSERVACION"
        End With

        With grVentas.RootTable.Columns("tgDesc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With


        With grVentas.RootTable.Columns("tgFact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tgHact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("tgUact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("total")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "TOTAL"
            .FormatString = "0.00"
        End With
        With grVentas.RootTable.Columns("tgEmision")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas
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
        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True
        With grProductos.RootTable.Columns("yiId")
            .Width = 150
            .Caption = "Código"
            .Visible = True
        End With
        With grProductos.RootTable.Columns("yiDesc")
            .Width = 400
            .Caption = "Descripcionn"
            .Visible = True
        End With
        With grProductos.RootTable.Columns("yiEst")
            .Width = 270
            .Visible = False
            .Caption = "yiEst"
        End With
        With grProductos.RootTable.Columns("Estado")
            .Width = 150
            .Visible = False
            .Caption = "Estado"
        End With

        With grProductos.RootTable.Columns("yiUnidad")
            .Width = 160
            .Visible = False
        End With


        With grProductos.RootTable.Columns("Unidad")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Caption = "Unidad"
            .Visible = True
        End With

        With grProductos.RootTable.Columns("yiFecha")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yiHora")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yiUsuario")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductos
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
        CType(grdetalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, Now.Date, "", "", 0, Bin.GetBuffer)
    End Sub

    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("thnumi=MAX(thnumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("thnumi")
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
        grProductos.Focus()
        grProductos.MoveTo(grProductos.FilterRow)
        grProductos.Col = 2
    End Sub

    Private Sub _DesHabilitarProductos()
        GPanelProductos.Visible = False
        PanelTotal.Visible = True
        PanelInferior.Visible = True

        grdetalle.Select()
        grdetalle.Col = 5
        grdetalle.Row = grdetalle.RowCount - 1

    End Sub
    Public Sub _fnObtenerFilaDetalle(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("thnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next
    End Sub

    Public Sub _fnObtenerFilaDetalleProducto(ByRef pos As Integer, numi As Integer)
        For i As Integer = 0 To CType(grProductos.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _numi As Integer = CType(grProductos.DataSource, DataTable).Rows(i).Item("yiId")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("thty8prod")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grdetalle.RowCount) Then

            Dim lin As Integer = grdetalle.GetValue("thnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            Dim cant As Double = grdetalle.GetValue("thCant")
            Dim uni As Double = grdetalle.GetValue("thPrec")
            Dim cos As Double = grdetalle.GetValue("thCosto")
            Dim MontoDesc As Double = grdetalle.GetValue("thDesc")
            Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni
                Dim TotalCosto As Double = cant * cos
                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = TotalUnitario
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = TotalUnitario - MontoDesc
                grdetalle.SetValue("thSubTotal", TotalUnitario)
                grdetalle.SetValue("thTotal", TotalUnitario - MontoDesc)

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPtot2") = TotalCosto
                grdetalle.SetValue("thPtot2", TotalCosto)

                Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado")
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = 2
                End If
            End If
            _prCalcularPrecioTotal()
        End If



    End Sub
    Public Sub _prCalcularPrecioTotal()
        Dim montodesc As Double = tbMdesc.Value
        Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum))
        tbPdesc.Value = pordesc
        tbSubTotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum)
        tbIce.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thPtot2"), AggregateFunction.Sum) * (gi_ICE / 100)
        If (gb_FacturaIncluirICE = True) Then
            tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc + tbIce.Value
        Else
            tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc
        End If
    End Sub
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 2) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("thnumi")
                _fnObtenerFilaDetalle(pos, lin)
                If (estado = 0) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -2
                End If
                If (estado = 1) Then
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("estado") = -1
                End If
                grdetalle.RootTable.ApplyFilter(New Janus.Windows.GridEX.GridEXFilterCondition(grdetalle.RootTable.Columns("estado"), Janus.Windows.GridEX.ConditionOperator.GreaterThanOrEqualTo, 0))
                _prCalcularPrecioTotal()
                grdetalle.Select()
                grdetalle.Col = 5
                grdetalle.Row = grdetalle.RowCount - 1
            End If
        End If
        grdetalle.Refetch()
        grdetalle.Refresh()

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
            tbObra.Focus()
            Return False
        End If
        'Validar datos de factura
        If (TbNit.Text = String.Empty) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor ponga el nit del cliente.".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False
        End If

        If (TbNombre1.Text = String.Empty) Then
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor ponga la razon social del cliente.".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False
        End If

        If (grdetalle.RowCount = 1) Then
            grdetalle.Row = grdetalle.RowCount - 1
            If (grdetalle.GetValue("thty8prod") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return False
            End If

        End If
        Return True
    End Function

    Public Sub _GuardarNuevo()
        Dim numi As String = ""

        Dim res As Boolean = L_fnGrabarVentaServicio(numi, IIf(SwProforma.Value = True, tbProforma.Text, 0), _CodObra, tbFechaVenta.Value.ToString("yyyy/MM/dd"), _CodEmpleado, IIf(swTipoVenta.Value = True, 1, 0),
                                                     IIf(swTipoVenta.Value = True, Now.Date.ToString("yyyy/MM/dd"), tbFechaVenc.Value.ToString("yyyy/MM/dd")), _CodCliente, IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbtotal.Value, CType(grdetalle.DataSource, DataTable), IIf(swEmision.Value = True, 1, 0))
        If res Then
            If (gb_FacturaEmite) Then
                P_fnGenerarFactura(numi)
            End If

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Venta ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            _prImiprimirNotaVenta(numi)
            _prCargarVenta()
            _Limpiar()
            Table_Producto = Nothing
        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If
    End Sub
    Private Sub _prGuardarModificado()
        Dim res As Boolean = L_fnModificarVentaServicio(tbCodigo.Text, IIf(SwProforma.Value = True, tbProforma.Text, 0), _CodObra, tbFechaVenta.Value.ToString("yyyy/MM/dd"), _CodEmpleado, IIf(swTipoVenta.Value = True, 1, 0),
                                                     IIf(swTipoVenta.Value = True, Now.Date.ToString("yyyy/MM/dd"), tbFechaVenc.Value.ToString("yyyy/MM/dd")), _CodCliente, IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbtotal.Value, CType(grdetalle.DataSource, DataTable), IIf(swEmision.Value = True, 1, 0))
        If res Then

            If (gb_FacturaEmite) Then
                L_fnEliminarDatos("TFV001", "fvanumi=" + tbCodigo.Text.Trim)
                L_fnEliminarDatos("TFV0011", "fvbnumi=" + tbCodigo.Text.Trim)
                P_fnGenerarFactura(tbCodigo.Text.Trim)
            End If

            _prImiprimirNotaVenta(tbCodigo.Text)

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Venta ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            _prCargarVenta()
            _prSalir()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no pudo ser Modificada".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
        End If
    End Sub
    Public Sub _prImiprimirNotaVenta(numi As String)
        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "MENSAJE PRINCIPAL".ToUpper
        ef.Header = "¿desea imprimir la nota de venta?".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporte(numi)
        End If
    End Sub
    Public Sub _prImiprimirFacturaPreimpresa(numi As String)
        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "MENSAJE PRINCIPAL".ToUpper
        ef.Header = "¿desea imprimir la factura Preimpresa?".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            P_GenerarReporteFactura(numi)
        End If
    End Sub

    Private Sub _prSalir()
        If btnGrabar.Enabled = True Then
            _prInhabiliitar()
            If grVentas.RowCount > 0 Then
                _prMostrarRegistro(0)
            End If
        Else
            _tab.Close()
            _modulo.Select()
        End If
    End Sub
    Public Sub _prCargarIconELiminar()
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim Bin As New MemoryStream
            Dim img As New Bitmap(My.Resources.delete, 28, 28)
            img.Save(Bin, Imaging.ImageFormat.Png)
            CType(grdetalle.DataSource, DataTable).Rows(i).Item("img") = Bin.GetBuffer
            grdetalle.RootTable.Columns("img").Visible = True
        Next

    End Sub
    Public Sub _PrimerRegistro()
        Dim _MPos As Integer
        If grVentas.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            grVentas.Row = _MPos
        End If
    End Sub
    Public Sub InsertarProductosSinLote()
        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, grdetalle.GetValue("thnumi"))
        Dim existe As Boolean = _fnExisteProducto(grProductos.GetValue("yiId"))
        If ((pos >= 0) And (Not existe)) Then
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thty8prod") = grProductos.GetValue("yiId")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = grProductos.GetValue("yiDesc")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thUni") = grProductos.GetValue("yiUnidad")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("unidad") = grProductos.GetValue("Unidad")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCant") = 1
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec") = 1
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = 1
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = 1
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCosto") = 0
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPtot2") = 0
            _prCalcularPrecioTotal()
            _DesHabilitarProductos()
        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If
    End Sub
    Private Function P_fnGenerarFactura(numi As String) As Boolean
        Dim res As Boolean = False
        res = P_fnGrabarFacturarTFV001(numi) ' Grabar en la TFV001
        If (res) Then
            If (P_fnValidarFactura()) Then
                'Validar para facturar
                P_prImprimirFacturar(numi, True, True) '_Codigo de a tabla TV001
            Else
                'Volver todo al estada anterior
                ToastNotification.Show(Me, "No es posible facturar, vuelva a ingresar a la mesa he intente nuevamente!!!".ToUpper,
                                       My.Resources.OK,
                                       5 * 1000,
                                       eToastGlowColor.Red,
                                       eToastPosition.MiddleCenter)
            End If

            If (Not TbNit.Text.Trim.Equals("0")) Then
                L_Grabar_Nit(TbNit.Text.Trim, TbNombre1.Text.Trim, TbNombre2.Text.Trim)
            Else
                L_Grabar_Nit(TbNit.Text, "S/N", "")
            End If
        End If

        Return res
    End Function

    Private Function P_fnGrabarFacturarTFV001(numi As String) As Boolean
        Dim a As Double = CDbl(tbtotal.Value + tbMdesc.Value)
        'Dim b As Double = CDbl(IIf(IsDBNull(tbIce.Value), 0, tbIce.Value)) 'Ya esta calculado el 55% del ICE
        Dim b As Double = CDbl("0")
        Dim c As Double = CDbl("0")
        Dim d As Double = CDbl("0")
        Dim e As Double = a - b - c - d
        Dim f As Double = CDbl(tbMdesc.Value)
        Dim g As Double = e - f
        Dim h As Double = g * (gi_IVA / 100)

        Dim res As Boolean = False
        'Grabado de Cabesera Factura
        L_Grabar_Factura(numi,
                        dtiFechaFactura.Value.ToString("yyyy/MM/dd"),
                        IIf(Val(tbNroFactura.Text) = 0, "0", tbNroFactura.Text),
                        IIf(Val(tbNroAutoriz.Text) = 0, "0", tbNroAutoriz.Text),
                        "1",
                        TbNit.Text.Trim,
                        "0",
                        TbNombre1.Text,
                        "",
                        CStr(Format(a, "####0.00")),
                        CStr(Format(b, "####0.00")),
                        CStr(Format(c, "####0.00")),
                        CStr(Format(d, "####0.00")),
                        CStr(Format(e, "####0.00")),
                        CStr(Format(f, "####0.00")),
                        CStr(Format(g, "####0.00")),
                        CStr(Format(h, "####0.00")),
                        "",
                        Now.Date.ToString("yyyy/MM/dd"),
                        "''",
                        0,
                        numi)

        'Grabado de Detalle de Factura
        grProductos.Update()

        'Dim s As String = ""
        For Each fil As GridEXRow In grdetalle.GetRows
            If (Not fil.Cells("thCant").Value.ToString.Trim.Equals("") And
                Not fil.Cells("thty8prod").Value.ToString.Trim.Equals("0")) Then
                's = fil.Cells("codP").Value
                's = fil.Cells("des").Value
                's = fil.Cells("can").Value
                's = fil.Cells("imp").Value
                L_Grabar_Factura_Detalle(numi.ToString,
                                        fil.Cells("thty8prod").Value.ToString.Trim,
                                        fil.Cells("producto").Value.ToString.Trim,
                                        fil.Cells("thCant").Value.ToString.Trim,
                                        fil.Cells("thPrec").Value.ToString.Trim,
                                        numi)
                res = True
            End If
        Next
        Return res
    End Function

    Private Function P_fnValidarFactura() As Boolean
        Return True
    End Function

    Private Sub P_prImprimirFacturar(numi As String, impFactura As Boolean, grabarPDF As Boolean)
        Dim _Fecha, _FechaAl As Date
        Dim _Ds, _Ds1, _Ds2, _Ds3 As New DataSet
        Dim _Autorizacion, _Nit, _Fechainv, _Total, _Key, _Cod_Control, _Hora,
            _Literal, _TotalDecimal, _TotalDecimal2 As String
        Dim I, _NumFac, _numidosif, _TotalCC As Integer
        Dim ice, _Desc, _TotalLi As Decimal
        Dim _VistaPrevia As Integer = 0


        _Desc = CDbl(tbMdesc.Value)
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        _Fecha = Now.Date '.ToString("dd/MM/yyyy")
        _Hora = Now.Hour.ToString + ":" + Now.Minute.ToString
        _Ds1 = L_Dosificacion("1", "1", _Fecha)

        _Ds = L_Reporte_Factura(numi, numi)
        _Autorizacion = _Ds1.Tables(0).Rows(0).Item("sbautoriz").ToString
        _NumFac = CInt(_Ds1.Tables(0).Rows(0).Item("sbnfac")) + 1
        _Nit = _Ds.Tables(0).Rows(0).Item("fvanitcli").ToString
        _Fechainv = Microsoft.VisualBasic.Right(_Fecha.ToShortDateString, 4) +
                    Microsoft.VisualBasic.Right(Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 5), 2) +
                    Microsoft.VisualBasic.Left(_Fecha.ToShortDateString, 2)
        _Total = _Ds.Tables(0).Rows(0).Item("fvatotal").ToString
        ice = _Ds.Tables(0).Rows(0).Item("fvaimpsi")
        _numidosif = _Ds1.Tables(0).Rows(0).Item("sbnumi").ToString
        _Key = _Ds1.Tables(0).Rows(0).Item("sbkey")
        _FechaAl = _Ds1.Tables(0).Rows(0).Item("sbfal")

        Dim maxNFac As Integer = L_fnObtenerMaxIdTabla("TFV001", "fvanfac", "fvaautoriz = " + _Autorizacion)
        _NumFac = maxNFac + 1

        _TotalCC = Math.Round(CDbl(_Total), MidpointRounding.AwayFromZero)
        _Cod_Control = ControlCode.generateControlCode(_Autorizacion, _NumFac, _Nit, _Fechainv, CStr(_TotalCC), _Key)

        'Literal 
        _TotalLi = _Ds.Tables(0).Rows(0).Item("fvastot") - _Ds.Tables(0).Rows(0).Item("fvadesc")
        _TotalDecimal = _TotalLi - Math.Truncate(_TotalLi)
        _TotalDecimal2 = CDbl(_TotalDecimal) * 100

        'Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_Total) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Literal = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(_TotalLi) - CDbl(_TotalDecimal)) + " con " + IIf(_TotalDecimal2.Equals("0"), "00", _TotalDecimal2) + "/100 Bolivianos"
        _Ds2 = L_Reporte_Factura_Cia("1")
        QrFactura.Text = _Ds2.Tables(0).Rows(0).Item("scnit").ToString + "|" + Str(_NumFac).Trim + "|" + _Autorizacion + "|" + _Fecha + "|" + _Total + "|" + _TotalLi.ToString + "|" + _Cod_Control + "|" + TbNit.Text.Trim + "|" + ice.ToString + "|0|0|" + Str(_Desc).Trim

        L_Modificar_Factura("fvanumi = " + CStr(numi),
                            "",
                            CStr(_NumFac),
                            CStr(_Autorizacion),
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            _Cod_Control,
                            _FechaAl.ToString("yyyy/MM/dd"),
                            "",
                            "",
                            CStr(numi))

        _Ds = L_Reporte_Factura(numi, numi)

        For I = 0 To _Ds.Tables(0).Rows.Count - 1
            _Ds.Tables(0).Rows(I).Item("fvaimgqr") = P_fnImageToByteArray(QrFactura.Image)
        Next
        If (impFactura) Then
            _Ds3 = L_ObtenerRutaImpresora("1") ' Datos de Impresion de Facturación
            If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
                P_Global.Visualizador = New Visualizador 'Comentar
            End If
            Dim objrep As Object = Nothing
            If (gi_FacturaTipo = 1) Then
                'objrep = New R_FacturaG
            ElseIf (gi_FacturaTipo = 2) Then
                objrep = New R_FacturaCarta
                If (Not _Ds.Tables(0).Rows.Count = gi_FacturaCantidadItems) Then
                    For index = _Ds.Tables(0).Rows.Count To gi_FacturaCantidadItems - 1
                        'Insertamos la primera fila con el saldo Inicial
                        Dim f As DataRow = _Ds.Tables(0).NewRow
                        f.ItemArray() = _Ds.Tables(0).Rows(0).ItemArray
                        f.Item("fvbcant") = -1
                        _Ds.Tables(0).Rows.Add(f)
                    Next
                End If
            End If

            objrep.SetDataSource(_Ds.Tables(0))
            objrep.SetParameterValue("Hora", _Hora)
            objrep.SetParameterValue("Direccionpr", _Ds2.Tables(0).Rows(0).Item("scdir").ToString)
            objrep.SetParameterValue("Telefonopr", _Ds2.Tables(0).Rows(0).Item("sctelf").ToString)
            objrep.SetParameterValue("Literal1", _Literal)
            objrep.SetParameterValue("Literal2", " ")
            objrep.SetParameterValue("Literal3", " ")
            objrep.SetParameterValue("NroFactura", _NumFac)
            objrep.SetParameterValue("NroAutoriz", _Autorizacion)
            objrep.SetParameterValue("ENombre", _Ds2.Tables(0).Rows(0).Item("scneg").ToString) '?
            objrep.SetParameterValue("ECasaMatriz", _Ds2.Tables(0).Rows(0).Item("scsuc").ToString)
            objrep.SetParameterValue("ECiudadPais", _Ds2.Tables(0).Rows(0).Item("scpai").ToString)
            objrep.SetParameterValue("ESFC", _Ds1.Tables(0).Rows(0).Item("sbsfc").ToString)
            objrep.SetParameterValue("ENit", _Ds2.Tables(0).Rows(0).Item("scnit").ToString)
            objrep.SetParameterValue("EActividad", _Ds2.Tables(0).Rows(0).Item("scact").ToString)
            objrep.SetParameterValue("ESms", "''" + _Ds1.Tables(0).Rows(0).Item("sbnota").ToString + "''")
            objrep.SetParameterValue("ESms2", "''" + _Ds1.Tables(0).Rows(0).Item("sbnota2").ToString + "''")
            objrep.SetParameterValue("EDuenho", _Ds2.Tables(0).Rows(0).Item("scnom").ToString) '?
            objrep.SetParameterValue("URLImageLogo", gs_CarpetaRaiz + "\LogoFactura.jpg")
            objrep.SetParameterValue("URLImageMarcaAgua", gs_CarpetaRaiz + "\MarcaAguaFactura.jpg")

            If (_Ds3.Tables(0).Rows(0).Item("cbvp")) Then 'Vista Previa de la Ventana de Vizualización 1 = True 0 = False
                P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
                P_Global.Visualizador.ShowDialog() 'Comentar
                P_Global.Visualizador.BringToFront() 'Comentar
            End If

            Dim pd As New PrintDocument()
            pd.PrinterSettings.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString
            If (Not pd.PrinterSettings.IsValid) Then
                ToastNotification.Show(Me, "La Impresora ".ToUpper + _Ds3.Tables(0).Rows(0).Item("cbrut").ToString + Chr(13) + "No Existe".ToUpper,
                                       My.Resources.WARNING, 5 * 1000,
                                       eToastGlowColor.Blue, eToastPosition.BottomRight)
            Else
                objrep.PrintOptions.PrinterName = _Ds3.Tables(0).Rows(0).Item("cbrut").ToString '"EPSON TM-T20II Receipt5 (1)"
                objrep.PrintToPrinter(1, False, 1, 1)
            End If

            If (grabarPDF) Then
                'Copia de Factura en PDF
                If (Not Directory.Exists(gs_CarpetaRaiz + "\Facturas")) Then
                    Directory.CreateDirectory(gs_CarpetaRaiz + "\Facturas")
                End If
                objrep.ExportToDisk(ExportFormatType.PortableDocFormat, gs_CarpetaRaiz + "\Facturas\" + CStr(_NumFac) + "_" + CStr(_Autorizacion) + ".pdf")

            End If
        End If
        L_Actualiza_Dosificacion(_numidosif, _NumFac, numi)
    End Sub

    Public Function P_fnImageToByteArray(ByVal imageIn As Image) As Byte()
        Dim ms As New System.IO.MemoryStream()
        imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
        Return ms.ToArray()
    End Function


    Private Function P_fnValidarFacturaVigente() As Boolean
        Dim est As String = L_fnObtenerDatoTabla("TFV001", "fvaest", "fvanumi=" + tbCodigo.Text.Trim)
        Return (est.Equals("True") Or est = String.Empty)
    End Function

    Private Sub P_prCargarVariablesIndispensables()
        If (gb_FacturaEmite) Then
            gi_IVA = CDbl(IIf(L_fnGetIVA().Rows(0).Item("scdebfis").ToString.Equals(""), gi_IVA, L_fnGetIVA().Rows(0).Item("scdebfis").ToString))
            gi_ICE = CDbl(IIf(L_fnGetICE().Rows(0).Item("scice").ToString.Equals(""), gi_ICE, L_fnGetICE().Rows(0).Item("scice").ToString))
        End If

    End Sub

    Private Sub P_prCargarParametro()
        'El sistema factura?
        GroupPanelFactura.Visible = True 'gb_FacturaEmite

        'Si factura, preguntar si, Se incluye el Importe ICE / IEHD / TASAS?
        If (gb_FacturaEmite) Then
            lbIce.Visible = gb_FacturaIncluirICE
            tbIce.Visible = gb_FacturaIncluirICE
        Else
            lbIce.Visible = False
            tbIce.Visible = False
        End If

    End Sub
    Private Sub P_GenerarReporte(numi As String)
        Dim dt As DataTable = L_fnVentaNotaDeVenta(numi)
        If (gb_DetalleProducto) Then
            ponerDescripcionProducto(dt)
        End If
        Dim total As Decimal = dt.Compute("SUM(Total)", "") - dt.Rows(0).Item("Descuento")
        Dim totald As Double = (total / 6.96)
        Dim fechaven As String = dt.Rows(0).Item("fechaventa")
        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If
        Dim ParteEntera As Long
        Dim ParteDecimal As Decimal
        ParteEntera = Int(total)
        ParteDecimal = Math.Round(total - ParteEntera, 2)
        Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Bolivianos"

        ParteEntera = Int(totald)
        ParteDecimal = Math.Round(totald - ParteEntera, 2)
        Dim lid As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Dolares"

        Dim dt2 As DataTable = L_fnNameReporte()
        Dim ParEmp1 As String = ""
        Dim ParEmp2 As String = ""
        Dim ParEmp3 As String = ""
        Dim ParEmp4 As String = ""
        If (dt2.Rows.Count > 0) Then
            ParEmp1 = dt2.Rows(0).Item("Empresa1").ToString
            ParEmp2 = dt2.Rows(0).Item("Empresa2").ToString
            ParEmp3 = dt2.Rows(0).Item("Empresa3").ToString
            ParEmp4 = dt2.Rows(0).Item("Empresa4").ToString
        End If

        P_Global.Visualizador = New Visualizador
        Dim _FechaAct As String
        Dim _FechaPar As String
        Dim _Fecha() As String
        Dim _Meses() As String = {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"}
        _FechaAct = fechaven
        _Fecha = Split(_FechaAct, "-")
        _FechaPar = "Cochabamba, " + _Fecha(0).Trim + " De " + _Meses(_Fecha(1) - 1).Trim + " Del " + _Fecha(2).Trim
        If (G_Lote = False) Then
            Dim objrep As New R_NotaDeVenta
            '' GenerarNro(_dt)
            ''objrep.SetDataSource(Dt1Kardex)

            objrep.SetDataSource(dt)
            objrep.SetParameterValue("TotalBs", li)
            objrep.SetParameterValue("TotalDo", lid)
            objrep.SetParameterValue("TotalDoN", totald)
            objrep.SetParameterValue("usuario", gs_user)
            objrep.SetParameterValue("estado", 1)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.Show() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        Else
            Dim objrep As New R_NotaDeVenta
            'Dim objrep As New R_NotaDeVentaSinLote
            'GenerarNro(_dt)
            'objrep.SetDataSource(Dt1Kardex)
            'totald = Math.Round(totald, 2)
            objrep.SetDataSource(dt)
            objrep.SetParameterValue("Total", total)
            objrep.SetParameterValue("TotalBs", li)
            objrep.SetParameterValue("TotalDo", lid)
            objrep.SetParameterValue("TotalDoN", totald)
            'objrep.SetParameterValue("P_Fecha", _FechaPar)
            'objrep.SetParameterValue("P_Empresa", ParEmp1)
            'objrep.SetParameterValue("P_Empresa1", ParEmp2)
            'objrep.SetParameterValue("P_Empresa2", ParEmp3)
            'objrep.SetParameterValue("P_Empresa3", ParEmp4)
            objrep.SetParameterValue("usuario", gs_user)
            objrep.SetParameterValue("estado", 1)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.Show() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar
        End If

    End Sub

    Private Sub ponerDescripcionProducto(ByRef dt As DataTable)
        For Each fila As DataRow In dt.Rows
            Dim numi As Integer = fila.Item("codProducto")
            Dim dtDP As DataTable = L_fnDetalleProducto(numi)
            Dim des As String = fila.Item("producto") + vbNewLine + vbNewLine
            For Each fila2 As DataRow In dtDP.Rows
                des = des + fila2.Item("yfadesc").ToString + vbNewLine
            Next
            fila.Item("producto") = des
        Next
    End Sub

    Private Sub P_GenerarReporteFactura(numi As String)
        Dim dt As DataTable = L_fnVentaFactura(numi)
        Dim total As Double = dt.Compute("SUM(Total)", "")

        Dim ParteEntera As Long
        Dim ParteDecimal As Double
        ParteEntera = Int(total)
        ParteDecimal = total - ParteEntera
        Dim li As String = Facturacion.ConvertirLiteral.A_fnConvertirLiteral(CDbl(ParteEntera)) + " con " +
        IIf(ParteDecimal.ToString.Equals("0"), "00", ParteDecimal.ToString) + "/100 Bolivianos"
        Dim objrep As New R_FacturaFarmacia
        '' GenerarNro(_dt)
        ''objrep.SetDataSource(Dt1Kardex)
        'imprimir
        If PrintDialog1.ShowDialog = DialogResult.OK Then
            objrep.SetDataSource(dt)
            objrep.SetParameterValue("TotalEscrito", li)
            objrep.SetParameterValue("nit", TbNit.Text)
            objrep.SetParameterValue("Total", total)
            objrep.SetParameterValue("cliente", TbNombre1.Text + " " + TbNombre2.Text)
            objrep.PrintOptions.PrinterName = PrintDialog1.PrinterSettings.PrinterName
            objrep.PrintToPrinter(1, False, 1, 1)
            objrep.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize
        End If
    End Sub

    Sub _prCargarProductoDeLaProforma(numiProforma As Integer)
        Dim dt As DataTable = L_fnListarProformaServicioDetalle(numiProforma)
        If (dt.Rows.Count > 0) Then
            CType(grdetalle.DataSource, DataTable).Rows.Clear()
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim numiServicio As Integer = dt.Rows(i).Item("pdNumiServ")
                Dim nameServicio As String = dt.Rows(i).Item("Servicio")
                Dim cant As Double = dt.Rows(i).Item("pdCantidad")
                _prAddDetalleVenta()
                grdetalle.Row = grdetalle.RowCount - 1
                grdetalle.SetValue("thty8prod", numiServicio)
                grdetalle.SetValue("Producto", nameServicio)
                grdetalle.SetValue("thCant", dt.Rows(i).Item("pdCantidad"))
                grdetalle.SetValue("thUni", dt.Rows(i).Item("pdNumiUnidad"))
                grdetalle.SetValue("unidad", dt.Rows(i).Item("unidad"))
                grdetalle.SetValue("thPrec", dt.Rows(i).Item("pdPrecio"))
                grdetalle.SetValue("thSubTotal", dt.Rows(i).Item("pdSubTotal"))
                grdetalle.SetValue("thTotal", dt.Rows(i).Item("pdTotal"))
                grdetalle.SetValue("thDesc", dt.Rows(i).Item("pdDesc"))
                grdetalle.SetValue("thCosto", 0)
            Next
            grdetalle.Select()
            _prCalcularPrecioTotal()
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

        'btnNuevo.Enabled = False
        'btnModificar.Enabled = False
        'btnEliminar.Enabled = False
        'GPanelProductos.Visible = False
        '_prhabilitar()

        '_Limpiar()
    End Sub
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _prSalir()

    End Sub



    Private Sub tbCliente_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCliente.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarClientes()
                If dt.Rows.Count > 0 Then
                    Dim listEstCeldas As New List(Of Modelo.Celda)
                    listEstCeldas.Add(New Modelo.Celda("ydnumi,", True, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("ydcod", False, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("ydrazonsocial", True, "RAZON SOCIAL", 180))
                    listEstCeldas.Add(New Modelo.Celda("yddesc", True, "NOMBRE", 280))
                    listEstCeldas.Add(New Modelo.Celda("yddctnum", True, "N. Documento".ToUpper, 150))
                    listEstCeldas.Add(New Modelo.Celda("yddirec", True, "DIRECCION", 220))
                    listEstCeldas.Add(New Modelo.Celda("ydtelf1", True, "Telefono".ToUpper, 200))
                    listEstCeldas.Add(New Modelo.Celda("ydfnac", True, "F.Nacimiento".ToUpper, 150, "MM/dd,YYYY"))
                    listEstCeldas.Add(New Modelo.Celda("ydnumivend,", False, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("vendedor,", False, "ID", 50))
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
                        _dias = Row.Cells("yddias").Value

                        Dim numiVendedor As Integer = IIf(IsDBNull(Row.Cells("ydnumivend").Value), 0, Row.Cells("ydnumivend").Value)
                        If (numiVendedor > 0) Then
                            tbVendedor.Text = Row.Cells("vendedor").Value
                            _CodEmpleado = Row.Cells("ydnumivend").Value

                            grdetalle.Select()
                            Table_Producto = Nothing
                        Else
                            tbVendedor.Clear()
                            _CodEmpleado = 0
                            tbVendedor.Focus()
                            Table_Producto = Nothing
                        End If
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
                If dt.Rows.Count > 0 Then
                    Dim listEstCeldas As New List(Of Modelo.Celda)
                    listEstCeldas.Add(New Modelo.Celda("ydnumi,", True, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("ydcod", False, "ID", 50))
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
                        _CodEmpleado = Row.Cells("ydnumi").Value
                        tbVendedor.Text = Row.Cells("yddesc").Value
                        grdetalle.Select()
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub swTipoVenta_ValueChanged(sender As Object, e As EventArgs) Handles swTipoVenta.ValueChanged
        If (swTipoVenta.Value = False) Then
            lbCredito.Visible = True
            tbFechaVenc.Visible = True
            tbFechaVenc.Value = DateAdd(DateInterval.Day, _dias, Now.Date)
        Else
            lbCredito.Visible = False
            tbFechaVenc.Visible = False
        End If
    End Sub

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            'If (e.Column.Index = grdetalle.RootTable.Columns("yfcbarra").Index Or
            If (
                e.Column.Index = grdetalle.RootTable.Columns("thCant").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("thPorc").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("thPrec").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("thDesc").Index) Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else
            e.Cancel = True
        End If

    End Sub

    Private Sub grdetalle_Enter(sender As Object, e As EventArgs) Handles grdetalle.Enter

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

            grdetalle.Select()
            If _codeBar = 1 Then
                If gb_CodigoBarra Then
                    grdetalle.Col = 3
                    grdetalle.Row = 0
                Else
                    grdetalle.Col = 5
                    grdetalle.Row = 0
                End If
            End If
        End If


    End Sub
    Private Sub grdetalle_KeyDown(sender As Object, e As KeyEventArgs) Handles grdetalle.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grdetalle.Col
            f = grdetalle.Row
            If (grdetalle.Col = grdetalle.RootTable.Columns("thCant").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    _prAddDetalleVenta()
                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            End If
            If (grdetalle.Col = grdetalle.RootTable.Columns("thPrec").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    _prAddDetalleVenta()
                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            End If
            If (grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
                If (grdetalle.GetValue("producto") <> String.Empty) Then
                    _prAddDetalleVenta()
                    _HabilitarProductos()
                Else
                    ToastNotification.Show(Me, "Seleccione un Producto Por Favor", My.Resources.WARNING, 3000, eToastGlowColor.Red, eToastPosition.TopCenter)
                End If
            End If
salirIf:
        End If

        If (e.KeyData = Keys.Control + Keys.Enter And grdetalle.Row >= 0 And
            grdetalle.Col = grdetalle.RootTable.Columns("producto").Index) Then
            Dim indexfil As Integer = grdetalle.Row
            Dim indexcol As Integer = grdetalle.Col
            _HabilitarProductos()
        End If
        If (e.KeyData = Keys.Escape And grdetalle.Row >= 0) Then
            _prEliminarFila()
        End If
    End Sub
    Private Function existeProducto(codigo As String) As Boolean
        Return (Table_Producto.Select("yfcbarra='" + codigo.Trim() + "'", "").Count > 0)
    End Function

    Private Function verificarExistenciaUnica(codigo As String) As Boolean
        Dim cont As Integer = 0
        For Each fila As GridEXRow In grdetalle.GetRows()
            If (fila.Cells("yfcbarra").Value.ToString.Trim = codigo.Trim) Then
                cont += 1
            End If
        Next
        Return (cont >= 1)
    End Function

    Private Sub ponerProducto(codigo As String)
        grdetalle.DataChanged = True
        CType(grdetalle.DataSource, DataTable).AcceptChanges()
        Dim fila As DataRow() = Table_Producto.Select("yfcbarra='" + codigo.Trim + "'", "")
        If (fila.Count > 0) Then
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, grdetalle.GetValue("thnumi"))
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thty8prod") = fila(0).ItemArray(0)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("codigo") = fila(0).ItemArray(1)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("yfcbarra") = fila(0).ItemArray(2)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = fila(0).ItemArray(3)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thUni") = fila(0).ItemArray(13)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("unidad") = fila(0).ItemArray(14)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec") = fila(0).ItemArray(15)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = fila(0).ItemArray(15)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = fila(0).ItemArray(15)
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCant") = 1
            If (gb_FacturaIncluirICE) Then
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCosto") = fila(0).ItemArray(17)
            Else
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCosto") = 0
            End If
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPtot2") = fila(0).ItemArray(17)

            'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("tblote") = grProductos.GetValue("iclot")
            'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("tbfechaVenc") = grProductos.GetValue("icfven")
            'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("iccven")
            _prCalcularPrecioTotal()
        End If
    End Sub

    Private Sub sumarCantidad(codigo As String)
        Dim fila As DataRow() = CType(grdetalle.DataSource, DataTable).Select("yfcbarra='" + codigo.Trim + "'", "")
        If (fila.Count > 0) Then
            Dim pos1 As Integer = -1
            _fnObtenerFilaDetalle(pos1, fila(0).Item("thnumi"))

            Dim cant As Integer = grdetalle.GetRow(pos1).Cells("thCant").Value + 1
            Dim stock As Integer = grdetalle.GetRow(pos1).Cells("stock").Value
            'If (cant > stock) Then
            Dim lin As Integer = grdetalle.GetRow(pos1).Cells("thnumi").Value
            Dim pos2 As Integer = -1
            _fnObtenerFilaDetalle(pos2, lin)
            CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thCant") = cant
            CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thSubTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thPrec") * cant
            CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thPtot2") = grdetalle.GetRow(pos1).Cells("thCosto").Value * cant
            CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos2).Item("thPrec") * cant
            Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
            'ToastNotification.Show(Me, "La cantidad de la venta no debe ser mayor al del stock" & vbCrLf &
            '        "Stock=" + Str(stock).ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            grdetalle.SetValue("yfcbarra", "")
            grdetalle.SetValue("thCant", 0)
            grdetalle.SetValue("thSubTotal", 0)
            grdetalle.SetValue("thPtot2", 0)
            grdetalle.DataChanged = True
            'grdetalle.Refetch()
            grdetalle.Refresh()
            '_prCalcularPrecioTotal()
            'Else
            '    If (cant = stock) Then
            '        'grdetalle.SelectedFormatStyle.ForeColor = Color.Blue
            '        'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle = New GridEXFormatStyle
            '        'grdetalle.CurrentRow.Cells(e.Column).FormatStyle.BackColor = Color.Pink
            '        'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.BackColor = Color.DodgerBlue
            '        'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.ForeColor = Color.White
            '        'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.FontBold = TriState.True
            '    End If
            'End If

            _prCalcularPrecioTotal()
        End If
    End Sub

    Private Sub _buscarRegistro(cbarra As String)
        Dim _t As DataTable
        _t = L_fnListarProductosC(cbarra)
        If _t.Rows.Count > 0 Then
            CType(grdetalle.DataSource, DataTable).Rows(0).Item("producto") = _t.Rows(0).Item("yfcdprod1")
            CType(grdetalle.DataSource, DataTable).Rows(0).Item("thCant") = 1
            CType(grdetalle.DataSource, DataTable).Rows(0).Item("unidad") = _t.Rows(0).Item("uni")

        Else
            MsgBox("Codigo de Producto No Exite")
        End If
        'CType(grdetalle.DataSource, DataTable).Rows(0).Item("thPrec") =
        'CType(grdetalle.DataSource, DataTable).Rows(0).Item("thUni") = 1
        'CType(grdetalle.DataSource, DataTable).Rows(0).Item("thPtot2") = grdetalle.GetValue("thCosto") * 1
        'ojo 'Dim pos, lin As Integer
        'pos = grdetalle.Row
        'lin = grdetalle.Col

        'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCant") = 1
        'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec")
        'CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPtot2") = grdetalle.GetValue("thCosto") * 1


    End Sub
    Private Sub grProductos_KeyDown(sender As Object, e As KeyEventArgs) Handles grProductos.KeyDown
        If (Not _fnAccesible()) Then
            Return
        End If
        If (e.KeyData = Keys.Enter) Then
            Dim f, c As Integer
            c = grProductos.Col
            f = grProductos.Row
            If (f >= 0) Then
                InsertarProductosSinLote()
            End If
        End If
        If e.KeyData = Keys.Escape Then
            _DesHabilitarProductos()
            FilaSelectLote = Nothing
        End If
    End Sub
    Private Sub grdetalle_CellValueChanged(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellValueChanged
        If (e.Column.Index = grdetalle.RootTable.Columns("thCant").Index) Or (e.Column.Index = grdetalle.RootTable.Columns("thPrec").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("thCant")) Or grdetalle.GetValue("thCant").ToString = String.Empty) Then

                Dim lin As Integer = grdetalle.GetValue("thnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCant") = 1
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec")
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPtot2") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCosto")
            Else
                If (grdetalle.GetValue("thCant") > 0) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    Dim porcdesc As Double = grdetalle.GetValue("thPorc")
                    Dim montodesc As Double = ((grdetalle.GetValue("thPrec") * grdetalle.GetValue("thCant")) * (porcdesc / 100))
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = montodesc
                    grdetalle.SetValue("thDesc", montodesc)
                    P_PonerTotal(rowIndex)
                Else
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thCant") = 1
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPrec")
                    _prCalcularPrecioTotal()
                End If
            End If
        End If
        '''''''''''''''''''''PORCENTAJE DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("thPorc").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("thPorc")) Or grdetalle.GetValue("thPorc").ToString = String.Empty) Then
                Dim lin As Integer = grdetalle.GetValue("thnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal")

            Else
                If (grdetalle.GetValue("thPorc") > 0 And grdetalle.GetValue("thPorc") <= 100) Then
                    Dim porcdesc As Double = grdetalle.GetValue("thPorc")
                    Dim montodesc As Double = (grdetalle.GetValue("thSubTotal") * (porcdesc / 100))
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = montodesc
                    grdetalle.SetValue("thDesc", montodesc)

                    Dim rowIndex As Integer = grdetalle.Row
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal")
                    grdetalle.SetValue("thPorc", 0)
                    grdetalle.SetValue("thDesc", 0)
                    grdetalle.SetValue("thTotal", grdetalle.GetValue("thSubTotal"))
                    _prCalcularPrecioTotal()
                End If
            End If
        End If


        '''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("thDesc").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("thDesc")) Or grdetalle.GetValue("thDesc").ToString = String.Empty) Then
                Dim lin As Integer = grdetalle.GetValue("thnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal")
            Else
                If (grdetalle.GetValue("thDesc") > 0 And grdetalle.GetValue("thDesc") <= grdetalle.GetValue("thSubTotal")) Then

                    Dim montodesc As Double = grdetalle.GetValue("thDesc")
                    Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetValue("thSubTotal"))
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = montodesc
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = pordesc
                    grdetalle.SetValue("thPorc", pordesc)
                    Dim rowIndex As Integer = grdetalle.Row
                    P_PonerTotal(rowIndex)

                Else
                    Dim lin As Integer = grdetalle.GetValue("thnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thPorc") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thDesc") = 0
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thTotal") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("thSubTotal")
                    grdetalle.SetValue("thPorc", 0)
                    grdetalle.SetValue("thDesc", 0)
                    grdetalle.SetValue("thTotal", grdetalle.GetValue("thSubTotal"))
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
                    Dim montodesc As Double = (grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) * (porcdesc / 100))
                    tbMdesc.Value = montodesc
                    tbIce.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thPtot2"), AggregateFunction.Sum) * (gi_ICE / 100)
                    If (gb_FacturaIncluirICE = True) Then
                        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc + tbIce.Value
                    Else
                        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc
                    End If
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
                    Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum))
                    tbPdesc.Value = pordesc
                    tbIce.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thPtot2"), AggregateFunction.Sum) * (gi_ICE / 100)
                    If (gb_FacturaIncluirICE = True) Then
                        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc + tbIce.Value
                    Else
                        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("thTotal"), AggregateFunction.Sum) - montodesc
                    End If

                End If

            End If

            If (tbMdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If

    End Sub

    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("thCant").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("thCant")) Or grdetalle.GetValue("thCant").ToString = String.Empty) Then
                grdetalle.SetValue("thCant", 1)
                grdetalle.SetValue("thSubTotal", grdetalle.GetValue("thPrec"))
                grdetalle.SetValue("thPorc", 0)
                grdetalle.SetValue("thDesc", 0)
                grdetalle.SetValue("thTotal", grdetalle.GetValue("thPrec"))
            Else
                If (grdetalle.GetValue("thCant") < 0) Then
                    grdetalle.SetValue("thCant", 1)
                    grdetalle.SetValue("thSubTotal", grdetalle.GetValue("thPrec"))
                    grdetalle.SetValue("thPorc", 0)
                    grdetalle.SetValue("thDesc", 0)
                    grdetalle.SetValue("thTotal", grdetalle.GetValue("thPrec"))
                End If
            End If
        End If
        If (e.Column.Index = grdetalle.RootTable.Columns("thPrec").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("thPrec")) Or grdetalle.GetValue("thPrec").ToString = String.Empty) Then
                grdetalle.SetValue("thPrec", 1)
                grdetalle.SetValue("thSubTotal", grdetalle.GetValue("thPrec"))
                grdetalle.SetValue("thPorc", 0)
                grdetalle.SetValue("thDesc", 0)
                grdetalle.SetValue("thTotal", grdetalle.GetValue("thPrec"))
            Else
                If (grdetalle.GetValue("thPrec") < 0) Then
                    grdetalle.SetValue("thPrec", 1)
                    grdetalle.SetValue("thSubTotal", grdetalle.GetValue("thPrec"))
                    grdetalle.SetValue("thPorc", 0)
                    grdetalle.SetValue("thDesc", 0)
                    grdetalle.SetValue("thTotal", grdetalle.GetValue("thPrec"))
                End If
            End If
        End If
    End Sub
    Private Sub grdetalle_MouseClick(sender As Object, e As MouseEventArgs) Handles grdetalle.MouseClick
        If (Not _fnAccesible()) Then
            Return
        End If
        If (grdetalle.RowCount >= 2) Then
            If (grdetalle.CurrentColumn.Index = grdetalle.RootTable.Columns("img").Index) Then
                _prEliminarFila()
            End If
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
                ''    _prInhabiliitar() RODRIGO RLA

            End If
        End If

    End Sub

    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        If (grVentas.RowCount > 0) Then
            If (gb_FacturaEmite) Then
                If (Not P_fnValidarFacturaVigente()) Then
                    Dim img As Bitmap = New Bitmap(My.Resources.WARNING, 50, 50)

                    ToastNotification.Show(Me, "No se puede modificar la venta con codigo ".ToUpper + tbCodigo.Text + ", su factura esta anulada.".ToUpper,
                                              img, 2000,
                                              eToastGlowColor.Green,
                                              eToastPosition.TopCenter)
                    Exit Sub
                End If
            End If
            Dim res As Boolean = L_fnVerificarSiSeContabilizoVenta(tbCodigo.Text)
            If res Then
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, "La Venta no puede ser Modificada porque ya fue contabilizada".ToUpper, img, 3500, eToastGlowColor.Red, eToastPosition.TopCenter)
            Else
                _prhabilitar()
                btnNuevo.Enabled = False
                btnModificar.Enabled = False
                btnEliminar.Enabled = False
                btnGrabar.Enabled = True

                PanelNavegacion.Enabled = False
                _prCargarIconELiminar()
            End If
        End If
    End Sub
    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        If (gb_FacturaEmite) Then
            If (P_fnValidarFacturaVigente()) Then
                Dim img As Bitmap = New Bitmap(My.Resources.WARNING, 50, 50)

                ToastNotification.Show(Me, "No se puede eliminar la venta con codigo ".ToUpper + tbCodigo.Text + ", su factura esta vigente, por favor primero anule la factura".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)
                Exit Sub
            End If
        End If
        If (swTipoVenta.Value = False) Then
            Dim res1 As Boolean = L_fnVerificarPagosVentas(tbCodigo.Text)
            If res1 Then
                Dim img As Bitmap = New Bitmap(My.Resources.WARNING, 50, 50)
                ToastNotification.Show(Me, "No se puede eliminar la venta con código ".ToUpper + tbCodigo.Text + ", porque tiene pagos realizados, por favor primero elimine los pagos correspondientes a esta venta".ToUpper,
                                          img, 5000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)
                Exit Sub
            End If
        End If

        Dim result As Boolean = L_fnVerificarSiSeContabilizoVenta(tbCodigo.Text)
        If result Then
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Venta no puede ser eliminada porque ya fue contabilizada".ToUpper, img, 4500, eToastGlowColor.Red, eToastPosition.TopCenter)
        End If

        Dim ef = New Efecto
        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarVenta(tbCodigo.Text, mensajeError)
            If res Then
                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Venta ".ToUpper + tbCodigo.Text + " eliminado con Exito.".ToUpper,
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

    Private Sub grVentas_SelectionChanged(sender As Object, e As EventArgs) Handles grVentas.SelectionChanged
        If (grVentas.RowCount >= 0 And grVentas.Row >= 0) Then
            _prMostrarRegistro(grVentas.Row)
        End If
    End Sub

    Private Sub btnSiguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = grVentas.Row
        If _pos < grVentas.RowCount - 1 And _pos >= 0 Then
            _pos = grVentas.Row + 1
            '' _prMostrarRegistro(_pos)
            grVentas.Row = _pos
        End If
    End Sub

    Private Sub btnUltimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = grVentas.Row
        If grVentas.RowCount > 0 Then
            _pos = grVentas.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            grVentas.Row = _pos
        End If
    End Sub

    Private Sub btnAnterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = grVentas.Row
        If _MPos > 0 And grVentas.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            grVentas.Row = _MPos
        End If
    End Sub

    Private Sub btnPrimero_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click
        _PrimerRegistro()
    End Sub
    Private Sub grVentas_KeyDown(sender As Object, e As KeyEventArgs) Handles grVentas.KeyDown

    End Sub

    Private Sub TbNit_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles TbNit.Validating
        Dim nom1, nom2 As String
        nom1 = ""
        nom2 = ""
        If (TbNit.Text.Trim = String.Empty) Then
            TbNit.Text = "0"
        End If
        L_Validar_Nit(TbNit.Text.Trim, nom1, nom2)
        TbNombre1.Text = nom1
        TbNombre2.Text = nom2
    End Sub
    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If (Not _fnAccesible()) Then
            P_GenerarReporte(tbCodigo.Text)

        End If
    End Sub

    Private Sub TbNit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TbNit.KeyPress
        g_prValidarTextBox(1, e)
    End Sub

    Private Sub swTipoVenta_KeyDown(sender As Object, e As KeyEventArgs) Handles swTipoVenta.KeyDown

    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs) Handles ButtonX1.Click

    End Sub

    Private Sub swTipoVenta_Leave(sender As Object, e As EventArgs) Handles swTipoVenta.Leave
        grdetalle.Select()
    End Sub

    Private Sub tbProforma_KeyDown(sender As Object, e As KeyEventArgs) Handles tbProforma.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarProformaServicio()
                Dim listEstCeldas As New List(Of Modelo.Celda)
                listEstCeldas.Add(New Modelo.Celda("pcNumi,", True, "NRO PROFORMA", 120))
                listEstCeldas.Add(New Modelo.Celda("pcFDoc", True, "FECHA", 80, "dd/MM/yyyy"))
                listEstCeldas.Add(New Modelo.Celda("pcVen", False, "", 50))
                listEstCeldas.Add(New Modelo.Celda("vendedor", True, "VENDEDOR".ToUpper, 150))
                listEstCeldas.Add(New Modelo.Celda("pcClie", False, "", 50))
                listEstCeldas.Add(New Modelo.Celda("cliente", True, "CLIENTE", 150))
                listEstCeldas.Add(New Modelo.Celda("total", True, "TOTAL".ToUpper, 120, "0.00"))
                listEstCeldas.Add(New Modelo.Celda("pcObra,", False, "IdObra", 120))
                listEstCeldas.Add(New Modelo.Celda("oanomb,", True, "Obra", 120))
                listEstCeldas.Add(New Modelo.Celda("pcDesc,", True, "Descuento", 120))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 50
                ef.ancho = 350
                ef.Context = "SELECCIONE PROFORMA".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
                    _CodEmpleado = Row.Cells("pcVen").Value
                    _CodCliente = Row.Cells("pcClie").Value
                    tbCliente.Text = Row.Cells("cliente").Value
                    tbVendedor.Text = Row.Cells("vendedor").Value
                    tbProforma.Text = Row.Cells("pcNumi").Value
                    _CodObra = Row.Cells("pcObra").Value
                    tbObra.Text = Row.Cells("oanomb").Value
                    tbProforma.Text = Row.Cells("pcNumi").Value
                    tbMdesc.Text = Row.Cells("pcDesc").Value
                    _prCargarProductoDeLaProforma(Row.Cells("pcNumi").Value)
                End If
            End If
        End If

    End Sub

    Private Sub SwProforma_ValueChanged(sender As Object, e As EventArgs) Handles SwProforma.ValueChanged
        If (_fnAccesible()) Then
            If (SwProforma.Value = True) Then
                tbProforma.BackColor = Color.White
                tbProforma.ReadOnly = True
                tbProforma.Enabled = True
                tbProforma.Focus()

            Else
                tbProforma.BackColor = Color.LightGray
                tbProforma.ReadOnly = True
                tbProforma.Enabled = False
            End If
        End If
    End Sub

    Private Sub grProductos_FormattingRow(sender As Object, e As RowLoadEventArgs)

    End Sub

    Private Sub cbSucursal_ValueChanged(sender As Object, e As EventArgs)
        Table_Producto = Nothing
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
                    tbObra.Text = Row.Cells("oanomb").Value
                    grdetalle.Select()
                    grdetalle.Col = 4
                    grdetalle.Row = 0
                End If
            End If
        End If
    End Sub
    Private Sub btObra_Click(sender As Object, e As EventArgs) Handles btObra.Click
        P_Principal.btObras_Click(sender, e)
    End Sub
#End Region
End Class