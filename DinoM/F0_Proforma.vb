﻿Imports Logica.AccesoLogica
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

Public Class F0_Proforma
#Region "Variables Globales"
    Dim _CodCliente As Integer = 0
    Dim _CodObra As Integer = 0
    Dim _CodEmpleado As Integer = 0
    Public _nameButton As String
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Dim FilaSelectLote As DataRow = Nothing
    Dim Lote As Boolean = False '1=igual a mostrar las columnas de lote y fecha de Vencimiento
    Public conv As String
#End Region

#Region "Metodos Privados"
    Private Sub _IniciarTodo()
        '' L_prAbrirConexion(gs_Ip, gs_UsuarioSql, gs_ClaveSql, gs_NombreBD)
        MSuperTabControl.SelectedTabIndex = 0
        Me.WindowState = FormWindowState.Maximized
        _prValidarLote()
        _prCargarComboLibreriaSucursal(cbSucursal)
        lbTipoMoneda.Visible = False
        swMoneda.Visible = False

        _prCargarProforma()
        _prInhabiliitar()
        grVentas.Focus()
        Me.Text = "PROFORMA"
        Dim blah As New Bitmap(New Bitmap(My.Resources.ic_p), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        _prAsignarPermisos()


    End Sub
    Public Sub _prValidarLote()
        Dim dt As DataTable = L_fnPorcUtilidad()
        If (dt.Rows.Count > 0) Then
            Dim lot As Integer = dt.Rows(0).Item("VerLote")
            If (lot = 1) Then
                Lote = True
            Else
                Lote = False
            End If

        End If
    End Sub
    Private Sub _prCargarComboLibreriaSucursal(mCombo As Janus.Windows.GridEX.EditControls.MultiColumnCombo)
        Dim dt As New DataTable
        dt = L_fnListarSucursales()
        With mCombo
            .DropDownList.Columns.Clear()
            .DropDownList.Columns.Add("aanumi").Width = 60
            .DropDownList.Columns("aanumi").Caption = "COD"
            .DropDownList.Columns.Add("aabdes").Width = 200
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
        tbCodigo.ReadOnly = True
        tbCliente.ReadOnly = True
        tbObra.ReadOnly = True
        tbVendedor.ReadOnly = True
        tbObservacion.ReadOnly = True
        tbFechaVenta.IsInputReadOnly = True
        swMoneda.IsReadOnly = True


        btnModificar.Enabled = True
        btnGrabar.Enabled = False
        btnNuevo.Enabled = True
        btnEliminar.Enabled = True

        tbSubTotal.IsInputReadOnly = True
        tbIce.IsInputReadOnly = True
        tbtotal.IsInputReadOnly = True

        tbMdesc.IsInputReadOnly = True
        tbTransporte.IsInputReadOnly = True


        grVentas.Enabled = True
        PanelNavegacion.Enabled = True
        grdetalle.RootTable.Columns("img").Visible = False
        If (GPanelProductos.Visible = True) Then
            _DesHabilitarProductos()
        End If
        cbSucursal.ReadOnly = True
        FilaSelectLote = Nothing

        SwServicio.IsReadOnly = True
        tbServicio.ReadOnly = True
    End Sub
    Private Sub _prhabilitar()
        grVentas.Enabled = False
        tbCodigo.ReadOnly = False
        ''  tbCliente.ReadOnly = False  por que solo podra seleccionar Cliente
        ''  tbVendedor.ReadOnly = False
        tbObservacion.ReadOnly = False
        tbFechaVenta.IsInputReadOnly = False
        swMoneda.IsReadOnly = False
        btnGrabar.Enabled = True

        tbMdesc.IsInputReadOnly = False
        tbTransporte.IsInputReadOnly = False

        If (tbCodigo.Text.Length > 0) Then
            cbSucursal.ReadOnly = True
        Else
            cbSucursal.ReadOnly = False
        End If

        SwServicio.IsReadOnly = False
        tbServicio.ReadOnly = False
    End Sub
    Public Sub _prFiltrar()
        'cargo el buscador
        Dim _Mpos As Integer
        _prCargarProforma()
        If grVentas.RowCount > 0 Then
            _Mpos = 0
            grVentas.Row = _Mpos
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
        _CodObra = 0
        tbFechaVenta.Value = Now.Date
        _prCargarDetalleVenta(-1)
        MSuperTabControl.SelectedTabIndex = 0
        tbSubTotal.Value = 0
        tbPdesc.Value = 0
        tbMdesc.Value = 0
        tbIce.Value = 0
        tbtotal.Value = 0
        tbTransporte.Value = 0
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

        If (CType(cbSucursal.DataSource, DataTable).Rows.Count > 0) Then
            cbSucursal.SelectedIndex = 0
        End If
        FilaSelectLote = Nothing

        btCliente.Visible = True
        btObra.Visible = True

        tbServicio.Clear()
        SwServicio.Value = False
    End Sub
    Public Sub _prMostrarRegistro(_N As Integer)
        '' grVentas.Row = _N
        '      a.panumi ,a.paalm ,a.pafdoc ,a.paven ,vendedor .yddesc as vendedor,a.paclpr,
        'cliente.yddesc as cliente ,a.pamon ,IIF(pamon=1,'Boliviano','Dolar') as moneda,a.paest ,a.paobs ,
        'a.padesc, a.pafact ,a.pahact ,a.pauact,a.patotal as total

        With grVentas
            cbSucursal.Value = .GetValue("paalm")
            tbCodigo.Text = .GetValue("panumi")
            tbFechaVenta.Value = .GetValue("pafdoc")
            _CodEmpleado = .GetValue("paven")
            tbVendedor.Text = .GetValue("vendedor")
            _CodCliente = .GetValue("paclpr")
            tbCliente.Text = .GetValue("cliente")
            _CodObra = .GetValue("paobra")
            tbObra.Text = .GetValue("oanomb")
            swMoneda.Value = .GetValue("pamon")
            tbObservacion.Text = .GetValue("paobs")

            lbFecha.Text = CType(.GetValue("pafact"), Date).ToString("dd/MM/yyyy")
            lbHora.Text = .GetValue("pahact").ToString
            lbUsuario.Text = .GetValue("pauact").ToString

        End With

        _prCargarDetalleVenta(tbCodigo.Text)
        tbMdesc.Value = grVentas.GetValue("padesc")
        tbTransporte.Value = grVentas.GetValue("patransp")
        _prCalcularPrecioTotal()
        LblPaginacion.Text = Str(grVentas.Row + 1) + "/" + grVentas.RowCount.ToString

        btCliente.Visible = False
        btObra.Visible = False

        If grVentas.GetValue("paservicio") > 0 Then
            SwServicio.Value = True
            tbServicio.Text = grVentas.GetValue("paservicio")
        Else
            SwServicio.Value = False
            tbServicio.Text = ""
        End If
    End Sub

    Private Sub _prCargarDetalleVenta(_numi As String)
        Dim dt As New DataTable
        dt = L_fnDetalleProforma(_numi)
        grdetalle.DataSource = dt
        grdetalle.RetrieveStructure()
        grdetalle.AlternatingColors = True

        With grdetalle.RootTable.Columns("pbnumi")
            .Width = 100
            .Caption = "CODIGO"
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbtp1numi")
            .Width = 90
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbty5prod")
            .Caption = "COD. ORIG."
            .Width = 80
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("yfcprod")
            .Caption = "COD."
            .Width = 80
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("producto")
            .Caption = "PRODUCTOS"
            .Width = 250
            .Visible = True
        End With
        With grdetalle.RootTable.Columns("pbest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbcmin")
            .Width = 160
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Medición(m2-ml)".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbccaja")
            .Width = 110
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cant. Cj/Ba".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbcantc")
            .Width = 130
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cajas/Barras".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbcantu")
            .Width = 160
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Cantidad(m2-ml)".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbumin")
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
        With grdetalle.RootTable.Columns("pbpbas")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Precio U.".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbptot")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "Sub Total".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbporc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "P.Desc(%)".ToUpper
        End With
        With grdetalle.RootTable.Columns("pbdesc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = False
            .FormatString = "0.00"
            .Caption = "M.Desc".ToUpper
        End With

        With grdetalle.RootTable.Columns("pbtotdesc")
            .Width = 100
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .FormatString = "0.00"
            .Caption = "Total".ToUpper
        End With

        With grdetalle.RootTable.Columns("pbfact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbhact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle.RootTable.Columns("pbuact")
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

        With grdetalle.RootTable.Columns("stock")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grdetalle
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
    End Sub

    Private Sub _prCargarProforma()
        Dim dt As New DataTable
        dt = L_fnGeneralProforma()
        grVentas.DataSource = dt
        grVentas.RetrieveStructure()
        grVentas.AlternatingColors = True

        With grVentas.RootTable.Columns("panumi")
            .Width = 80
            .Caption = "CODIGO"
            .Visible = True
        End With
        With grVentas.RootTable.Columns("paalm")
            .Width = 90
            .Visible = False
        End With
        With grVentas.RootTable.Columns("pafdoc")
            .Width = 85
            .Visible = True
            .Caption = "FECHA"
        End With
        With grVentas.RootTable.Columns("paven")
            .Width = 140
            .Visible = False
        End With
        With grVentas.RootTable.Columns("vendedor")
            .Width = 140
            .Visible = True
            .Caption = "VENDEDOR".ToUpper
        End With
        With grVentas.RootTable.Columns("paclpr")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("paobra")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("oanomb")
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
        With grVentas.RootTable.Columns("empresa")
            .Width = 250
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "EMPRESA"
        End With
        With grVentas.RootTable.Columns("rsocial")
            .Width = 250
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "RAZÓN SOCIAL"
        End With
        With grVentas.RootTable.Columns("pamon")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("moneda")
            .Width = 150
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "MONEDA"
        End With
        With grVentas.RootTable.Columns("paobs")
            .Width = 200
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "OBSERVACION"
        End With
        With grVentas.RootTable.Columns("padesc")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("patransp")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("paest")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
      
        With grVentas.RootTable.Columns("pafact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("pahact")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grVentas.RootTable.Columns("pauact")
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
        With grVentas.RootTable.Columns("paservicio")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Far
            .Visible = True
            .Caption = "NRO. SERVICIO"

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


    Public Sub actualizarSaldoSinLote(ByRef dt As DataTable)
        'b.yfcdprod1 ,a.iclot ,a.icfven  ,a.iccven 

        '      a.tbnumi ,a.tbtv1numi ,a.tbty5prod ,b.yfcdprod1 as producto,a.tbest ,a.tbcmin ,a.tbumin ,Umin .ycdes3 as unidad,a.tbpbas ,a.tbptot ,a.tbobs ,
        'a.tbpcos,a.tblote ,a.tbfechaVenc , a.tbptot2, a.tbfact ,a.tbhact ,a.tbuact,1 as estado,Cast(null as Image) as img,
        'Cast (0 as decimal (18,2)) as stock
        Dim _detalle As DataTable = CType(grdetalle.DataSource, DataTable)

        For i As Integer = 0 To dt.Rows.Count - 1 Step 1
            Dim sum As Integer = 0
            Dim codProducto As Integer = dt.Rows(i).Item("yfnumi")
            For j As Integer = 0 To grdetalle.RowCount - 1 Step 1
                grdetalle.Row = j
                Dim estado As Integer = grdetalle.GetValue("estado")
                If (estado = 0) Then
                    If (codProducto = grdetalle.GetValue("pbty5prod")) Then
                        sum = sum + grdetalle.GetValue("pbcmin")
                    End If
                End If
            Next
            dt.Rows(i).Item("stock") = dt.Rows(i).Item("stock") - sum
        Next

    End Sub

    Private Sub _prCargarProductos(_cliente As String)
        If (cbSucursal.SelectedIndex < 0) Then
            Return
        End If
        Dim dtname As DataTable = L_fnNameLabel()
        Dim dt As New DataTable
        dt = L_fnListarProductosSinLoteProforma(cbSucursal.Value, _cliente, CType(grdetalle.DataSource, DataTable))
        actualizarSaldoSinLote(dt)
        grProductos.DataSource = dt
        grProductos.RetrieveStructure()
        grProductos.AlternatingColors = True

        With grProductos.RootTable.Columns("yfnumi")
            .Width = 100
            .Caption = "Código"
            .Visible = False
        End With
        With grProductos.RootTable.Columns("yfcprod")
            .Width = 120
            .Caption = "Código"
            .Visible = True
        End With
        With grProductos.RootTable.Columns("yfcdprod1")
            .Width = 320
            .Visible = True
            .Caption = "Descripción"
        End With
        With grProductos.RootTable.Columns("yfcdprod2")
            .Width = 150
            .Visible = False
            .Caption = "Descripción Corta"
        End With

        With grProductos.RootTable.Columns("yfgr1")
            .Width = 160
            .Visible = False
        End With
        If (dtname.Rows.Count > 0) Then

            With grProductos.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 1").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 2").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With

            With grProductos.RootTable.Columns("grupo3")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 3").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("grupo4")
                .Width = 120
                .Caption = dtname.Rows(0).Item("Grupo 4").ToString
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        Else
            With grProductos.RootTable.Columns("grupo1")
                .Width = 120
                .Caption = "Grupo 1"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("grupo2")
                .Width = 120
                .Caption = "Grupo 2"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("grupo3")
                .Width = 120
                .Caption = "Grupo 3"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
            With grProductos.RootTable.Columns("grupo4")
                .Width = 120
                .Caption = "Grupo 4"
                .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
                .Visible = False
            End With
        End If

        With grProductos.RootTable.Columns("yfgr2")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yfgr3")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yfgr4")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With

        With grProductos.RootTable.Columns("yfumin")
            .Width = 50
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
        End With
        With grProductos.RootTable.Columns("UnidMin")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "Unidad Min."
        End With
        With grProductos.RootTable.Columns("yhprecio")
            .Width = 110
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = True
            .Caption = "Precio"
            .FormatString = "0.00"
        End With
        With grProductos.RootTable.Columns("pcos")
            .Width = 120
            .CellStyle.TextAlignment = Janus.Windows.GridEX.TextAlignment.Near
            .Visible = False
            .Caption = "Precio Costo"
            .FormatString = "0.00"
        End With
        With grProductos.RootTable.Columns("stock")
            .Width = 120
            .FormatString = "0.00"
            .Visible = True
            .Caption = "Stock"
        End With
        With grProductos.RootTable.Columns("yfvsup")
            .Width = 120
            .FormatString = "0.00"
            .Visible = False
            .Caption = "Conv."
        End With

        With grProductos
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False
            'diseño de la grilla
            .VisualStyle = VisualStyle.Office2007
        End With
        _prAplicarCondiccionJanusSinLote()
    End Sub
    Public Sub _prAplicarCondiccionJanusSinLote()
        Dim fc As GridEXFormatCondition
        fc = New GridEXFormatCondition(grProductos.RootTable.Columns("stock"), ConditionOperator.Equal, 0)
        'fc.FormatStyle.FontBold = TriState.True
        fc.FormatStyle.ForeColor = Color.Tan
        grProductos.RootTable.FormatConditions.Add(fc)
    End Sub


    Private Sub _prAddDetalleVenta()
        '       a.pbnumi ,a.pbtp1numi ,a.pbty5prod , producto,a.pbest ,a.pbcmin ,a.pbumin , unidad,
        'a.pbpbas ,a.pbptot,a.pbporc,a.pbdesc ,a.pbtotdesc, a.pbfact ,a.pbhact ,a.pbuact, estado, img,
        '	 stock
        Dim Bin As New MemoryStream
        Dim img As New Bitmap(My.Resources.delete, 28, 28)
        img.Save(Bin, Imaging.ImageFormat.Png)
        CType(grdetalle.DataSource, DataTable).Rows.Add(_fnSiguienteNumi() + 1, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, Now.Date, "", "", 0, Bin.GetBuffer, 0)
    End Sub

    Public Function _fnSiguienteNumi()
        Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
        Dim rows() As DataRow = dt.Select("pbnumi=MAX(pbnumi)")
        If (rows.Count > 0) Then
            Return rows(rows.Count - 1).Item("pbnumi")
        End If
        Return 1
    End Function
    Public Function _fnAccesible()
        Return tbFechaVenta.IsInputReadOnly = False
    End Function
    Private Sub _HabilitarProductos()
        GPanelProductos.Height = 520
        GPanelProductos.Visible = True
        'PanelTotal.Visible = False
        'PanelInferior.Visible = False
        _prCargarProductos(Str(_CodCliente))
        grProductos.Focus()
        grProductos.MoveTo(grProductos.FilterRow)
        grProductos.Col = 1

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
            Dim _numi As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pbnumi")
            If (_numi = numi) Then
                pos = i
                Return
            End If
        Next

    End Sub

    Public Function _fnExisteProducto(idprod As Integer) As Boolean
        For i As Integer = 0 To CType(grdetalle.DataSource, DataTable).Rows.Count - 1 Step 1
            Dim _idprod As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("pbty5prod")
            Dim estado As Integer = CType(grdetalle.DataSource, DataTable).Rows(i).Item("estado")
            If (_idprod = idprod And estado >= 0) Then

                Return True
            End If
        Next
        Return False
    End Function

   
    Public Sub P_PonerTotal(rowIndex As Integer)
        If (rowIndex < grdetalle.RowCount) Then

            Dim lin As Integer = grdetalle.GetValue("pbnumi")
            Dim pos As Integer = -1
            _fnObtenerFilaDetalle(pos, lin)
            'Dim cant As Double = grdetalle.GetValue("pbcmin")
            Dim cant As Double = grdetalle.GetValue("pbcantu")
            Dim uni As Double = grdetalle.GetValue("pbpbas")
            Dim MontoDesc As Double = grdetalle.GetValue("pbdesc")
            Dim dt As DataTable = CType(grdetalle.DataSource, DataTable)
            If (pos >= 0) Then
                Dim TotalUnitario As Double = cant * uni

                'grDetalle.SetValue("lcmdes", montodesc)

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = TotalUnitario
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = TotalUnitario - MontoDesc
                grdetalle.SetValue("pbptot", TotalUnitario)
                grdetalle.SetValue("pbtotdesc", TotalUnitario - MontoDesc)


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
        Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("pbtotdesc"), AggregateFunction.Sum))
        tbPdesc.Value = pordesc
        tbSubTotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pbtotdesc"), AggregateFunction.Sum)

        tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pbtotdesc"), AggregateFunction.Sum) - montodesc + tbTransporte.Value

    End Sub
    Public Sub _prEliminarFila()
        If (grdetalle.Row >= 0) Then
            If (grdetalle.RowCount >= 2) Then
                Dim estado As Integer = grdetalle.GetValue("estado")
                Dim pos As Integer = -1
                Dim lin As Integer = grdetalle.GetValue("pbnumi")
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


    End Sub
    Public Function _ValidarCampos() As Boolean
        If (_CodCliente <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Cliente con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbCliente.Focus()
            Return False

        End If
        If (_CodEmpleado <= 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione un Vendedor con Ctrl+Enter".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            tbVendedor.Focus()
            Return False
        End If
        If (cbSucursal.SelectedIndex < 0) Then
            Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
            ToastNotification.Show(Me, "Por Favor Seleccione una Sucursal".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            cbSucursal.Focus()
            Return False
        End If
        'Validar datos de factura
       

        If (grdetalle.RowCount = 1) Then
            grdetalle.Row = grdetalle.RowCount - 1
            If (grdetalle.GetValue("pbty5prod") = 0) Then
                Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
                ToastNotification.Show(Me, "Por Favor Seleccione  un detalle de producto".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                Return False
            End If

        End If
        Return True
    End Function

    Public Sub _GuardarNuevo()
        Dim numi As String = ""

        Dim res As Boolean = L_fnGrabarProforma(numi, tbFechaVenta.Value.ToString("yyyy/MM/dd"), _CodEmpleado, _CodCliente, _CodObra, IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbTransporte.Value, tbtotal.Value, CType(grdetalle.DataSource, DataTable), cbSucursal.Value, IIf(SwServicio.Value = True, tbServicio.Text, 0))

        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Grabado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            tbCodigo.Text = numi
            _prImprimirReporte()

            _prCargarProforma()

            _Limpiar()

        Else
            Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
            ToastNotification.Show(Me, "La Proforma no pudo ser insertado".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)

        End If

    End Sub
    Private Sub _prGuardarModificado()
        Dim res As Boolean = L_fnModificarProforma(tbCodigo.Text, tbFechaVenta.Value.ToString("yyyy/MM/dd"), _CodEmpleado, _CodCliente, _CodObra, IIf(swMoneda.Value = True, 1, 0), tbObservacion.Text, tbMdesc.Value, tbTransporte.Value, tbtotal.Value, CType(grdetalle.DataSource, DataTable), cbSucursal.Value, IIf(SwServicio.Value = True, tbServicio.Text, 0))
        If res Then

            Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)
            ToastNotification.Show(Me, "Código de Proforma ".ToUpper + tbCodigo.Text + " Modificado con Exito.".ToUpper,
                                      img, 2000,
                                      eToastGlowColor.Green,
                                      eToastPosition.TopCenter
                                      )
            _prImprimirReporte()
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
            If grVentas.RowCount > 0 Then
                _prMostrarRegistro(0)
            End If
        Else
            _modulo.Select()
            _tab.Close()
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
                '              a.ydnumi, a.ydcod, a.yddesc, a.yddctnum, a.yddirec
                ',a.ydtelf1 ,a.ydfnac 

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
                listEstCeldas.Add(New Modelo.Celda("vendedor,", False, "ID", 50))
                listEstCeldas.Add(New Modelo.Celda("yddias", False, "CRED", 50))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                ef.SeleclCol = 2
                Modelo.MGlobal.SeleccionarCol = 3
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
                    tbCliente.Text = Row.Cells("yddesc").Value
                    '_dias = Row.Cells("yddias").Value

                    Dim numiVendedor As Integer = IIf(IsDBNull(Row.Cells("ydnumivend").Value), 0, Row.Cells("ydnumivend").Value)
                    If (numiVendedor > 0) Then
                        tbVendedor.Text = Row.Cells("vendedor").Value
                        _CodEmpleado = Row.Cells("ydnumivend").Value

                        'grdetalle.Select()
                        tbObra.Focus()
                    Else
                        tbVendedor.Clear()
                        _CodEmpleado = 0
                        tbObra.Focus()
                        'tbVendedor.Focus()

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
                '              a.ydnumi, a.ydcod, a.yddesc, a.yddctnum, a.yddirec
                ',a.ydtelf1 ,a.ydfnac 

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
                Modelo.MGlobal.SeleccionarCol = 2
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
                    tbObservacion.Focus()
                End If
            End If
        End If
    End Sub
  

    Private Sub grdetalle_EditingCell(sender As Object, e As EditingCellEventArgs) Handles grdetalle.EditingCell
        If (_fnAccesible()) Then
            'Habilitar solo las columnas de Precio, %, Monto y Observación
            If (e.Column.Index = grdetalle.RootTable.Columns("pbcmin").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("pbporc").Index Or
                 e.Column.Index = grdetalle.RootTable.Columns("pbporc").Index Or
                e.Column.Index = grdetalle.RootTable.Columns("pbpbas").Index) Then
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
            grdetalle.Col = 1
            grdetalle.Row = 0
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

            If (grdetalle.Col = grdetalle.RootTable.Columns("pbcmin").Index) Then
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
            If (grdetalle.Col = grdetalle.RootTable.Columns("pbpbas").Index) Then
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
    Public Sub InsertarProductosSinLote()
        Dim pos As Integer = -1
        grdetalle.Row = grdetalle.RowCount - 1
        _fnObtenerFilaDetalle(pos, grdetalle.GetValue("pbnumi"))
        Dim existe As Boolean = _fnExisteProducto(grProductos.GetValue("yfnumi"))
        If ((pos >= 0) And (Not existe)) Then
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbty5prod") = grProductos.GetValue("yfnumi")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("yfcprod") = grProductos.GetValue("yfcprod")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("producto") = grProductos.GetValue("yfcdprod1")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbumin") = grProductos.GetValue("yfumin")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("unidad") = grProductos.GetValue("UnidMin")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas") = grProductos.GetValue("yhprecio")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = grProductos.GetValue("yhprecio")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = grProductos.GetValue("yhprecio")
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbcmin") = 1
            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("stock") = grProductos.GetValue("stock")
            'conv = grProductos.GetValue("yfvsup")
            _prCalcularPrecioTotal()
            _DesHabilitarProductos()
        Else
            If (existe) Then
                Dim img As Bitmap = New Bitmap(My.Resources.Mensaje, 50, 50)
                ToastNotification.Show(Me, "El producto ya existe en el detalle".ToUpper, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        End If
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
        Dim codprod As Integer
        If (e.Column.Index = grdetalle.RootTable.Columns("pbcmin").Index) Then 'Or (e.Column.Index = grdetalle.RootTable.Columns("pbpbas").Index)
            If (Not IsNumeric(grdetalle.GetValue("pbcmin")) Or grdetalle.GetValue("pbcmin").ToString = String.Empty) Then
                Dim lin As Integer = grdetalle.GetValue("pbnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbcmin") = 1
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")

            Else
                If (grdetalle.GetValue("pbcmin") > 0) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    Dim porcdesc As Double = grdetalle.GetValue("pbporc")
                    Dim montodesc As Double = ((grdetalle.GetValue("pbpbas") * grdetalle.GetValue("pbcmin")) * (porcdesc / 100))
                    Dim lin As Integer = grdetalle.GetValue("pbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = montodesc
                    grdetalle.SetValue("pbdesc", montodesc)

                    'Saca la conversión de cada producto
                    codprod = grdetalle.GetValue("pbty5prod")
                    Dim dtconv As New DataTable
                    dtconv = L_fnConversionProd(codprod)
                    conv = dtconv.Rows(0).Item("yfvsup")

                    If dtconv.Rows(0).Item("yfgr1") = 1 Then
                        grdetalle.SetValue("pbccaja", grdetalle.GetValue("pbcmin") / conv)
                        Dim CjReal As Integer = Math.Ceiling(grdetalle.GetValue("pbccaja"))
                        grdetalle.SetValue("pbcantc", CjReal)
                        grdetalle.SetValue("pbcantu", CjReal * conv)

                        P_PonerTotal(rowIndex)
                    Else
                        grdetalle.SetValue("pbccaja", grdetalle.GetValue("pbcmin") / conv)
                        Dim CjReal As Double = grdetalle.GetValue("pbccaja")
                        grdetalle.SetValue("pbcantc", CjReal)
                        grdetalle.SetValue("pbcantu", CjReal * conv)

                        P_PonerTotal(rowIndex)
                    End If

                Else
                    Dim lin As Integer = grdetalle.GetValue("pbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbcmin") = 1
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")
                    _prCalcularPrecioTotal()
                    'grdetalle.SetValue("pbcmin", 1)
                    'grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))

                End If
            End If
        End If

        ''''''''''''''''''''''CAMBIO PRECIO UNITARIO '''''''''''''''''''''
        If (e.Column.Index = grdetalle.RootTable.Columns("pbpbas").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pbpbas")) Or grdetalle.GetValue("pbpbas").ToString = String.Empty) Then
                Dim lin As Integer = grdetalle.GetValue("pbnumi")
                Dim pos As Integer = -1
                _fnObtenerFilaDetalle(pos, lin)
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas") = 1
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")

                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
                CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")

            Else
                If (grdetalle.GetValue("pbpbas") > 0) Then
                    Dim rowIndex As Integer = grdetalle.Row
                    Dim porcdesc As Double = grdetalle.GetValue("pbporc")
                    Dim montodesc As Double = ((grdetalle.GetValue("pbpbas") * grdetalle.GetValue("pbcmin")) * (porcdesc / 100))
                    Dim lin As Integer = grdetalle.GetValue("pbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = montodesc
                    grdetalle.SetValue("pbdesc", montodesc)

                    P_PonerTotal(rowIndex)
                Else
                    Dim lin As Integer = grdetalle.GetValue("pbnumi")
                    Dim pos As Integer = -1
                    _fnObtenerFilaDetalle(pos, lin)
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas") = 1
                    CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")
                    _prCalcularPrecioTotal()
                End If
            End If
        End If


        ''''''''''''''''''''''PORCENTAJE DE DESCUENTO '''''''''''''''''''''
        'If (e.Column.Index = grdetalle.RootTable.Columns("pbporc").Index) Then
        '    If (Not IsNumeric(grdetalle.GetValue("pbporc")) Or grdetalle.GetValue("pbporc").ToString = String.Empty) Then

        '        'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
        '        '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
        '        Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '        Dim pos As Integer = -1
        '        _fnObtenerFilaDetalle(pos, lin)
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot")
        '        'grdetalle.SetValue("pbcmin", 1)
        '        'grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))
        '    Else
        '        If (grdetalle.GetValue("pbporc") > 0 And grdetalle.GetValue("pbporc") <= 100) Then

        '            Dim porcdesc As Double = grdetalle.GetValue("pbporc")
        '            Dim montodesc As Double = (grdetalle.GetValue("pbptot") * (porcdesc / 100))
        '            Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '            Dim pos As Integer = -1
        '            _fnObtenerFilaDetalle(pos, lin)
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = montodesc
        '            grdetalle.SetValue("pbdesc", montodesc)

        '            Dim rowIndex As Integer = grdetalle.Row
        '            P_PonerTotal(rowIndex)

        '        Else
        '            Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '            Dim pos As Integer = -1
        '            _fnObtenerFilaDetalle(pos, lin)
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot")
        '            grdetalle.SetValue("pbporc", 0)
        '            grdetalle.SetValue("pbdesc", 0)
        '            grdetalle.SetValue("pbtotdesc", grdetalle.GetValue("pbptot"))
        '            _prCalcularPrecioTotal()
        '            'grdetalle.SetValue("pbcmin", 1)
        '            'grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))

        '        End If
        '    End If
        'End If


        ''''''''''''''''''''''MONTO DE DESCUENTO '''''''''''''''''''''
        'If (e.Column.Index = grdetalle.RootTable.Columns("pbdesc").Index) Then
        '    If (Not IsNumeric(grdetalle.GetValue("pbdesc")) Or grdetalle.GetValue("pbdesc").ToString = String.Empty) Then

        '        'grDetalle.GetRow(rowIndex).Cells("cant").Value = 1
        '        '  grDetalle.CurrentRow.Cells.Item("cant").Value = 1
        '        Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '        Dim pos As Integer = -1
        '        _fnObtenerFilaDetalle(pos, lin)
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
        '        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot")
        '        'grdetalle.SetValue("pbcmin", 1)
        '        'grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))
        '    Else
        '        If (grdetalle.GetValue("pbdesc") > 0 And grdetalle.GetValue("pbdesc") <= grdetalle.GetValue("pbptot")) Then

        '            Dim montodesc As Double = grdetalle.GetValue("pbdesc")
        '            Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetValue("pbptot"))

        '            Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '            Dim pos As Integer = -1
        '            _fnObtenerFilaDetalle(pos, lin)
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = montodesc
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = pordesc

        '            grdetalle.SetValue("pbporc", pordesc)
        '            Dim rowIndex As Integer = grdetalle.Row
        '            P_PonerTotal(rowIndex)

        '        Else
        '            Dim lin As Integer = grdetalle.GetValue("pbnumi")
        '            Dim pos As Integer = -1
        '            _fnObtenerFilaDetalle(pos, lin)
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbporc") = 0
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbdesc") = 0
        '            CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbtotdesc") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot")
        '            grdetalle.SetValue("pbporc", 0)
        '            grdetalle.SetValue("pbdesc", 0)
        '            grdetalle.SetValue("pbtotdesc", grdetalle.GetValue("pbptot"))
        '            _prCalcularPrecioTotal()
        '            'grdetalle.SetValue("pbcmin", 1)
        '            'grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))
        '        End If
        '    End If
        'End If

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
                    Dim montodesc As Double = (grdetalle.GetTotal(grdetalle.RootTable.Columns("pbptot"), AggregateFunction.Sum) * (porcdesc / 100))
                    tbMdesc.Value = montodesc

                    tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pbptot"), AggregateFunction.Sum) - montodesc
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
                    Dim pordesc As Double = ((montodesc * 100) / grdetalle.GetTotal(grdetalle.RootTable.Columns("pbptot"), AggregateFunction.Sum))
                    tbPdesc.Value = pordesc

                    tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pbptot"), AggregateFunction.Sum) - montodesc + tbTransporte.Value

                End If

            End If

            If (tbMdesc.Text = String.Empty) Then
                tbMdesc.Value = 0

            End If
        End If

    End Sub


    Private Sub grdetalle_CellEdited(sender As Object, e As ColumnActionEventArgs) Handles grdetalle.CellEdited
        If (e.Column.Index = grdetalle.RootTable.Columns("pbcmin").Index) Then
            If (Not IsNumeric(grdetalle.GetValue("pbcmin")) Or grdetalle.GetValue("pbcmin").ToString = String.Empty) Then

                grdetalle.SetValue("pbcmin", 1)
                grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))
                grdetalle.SetValue("pbporc", 0)
                grdetalle.SetValue("pbdesc", 0)
                grdetalle.SetValue("pbtotdesc", grdetalle.GetValue("pbpbas"))
            Else
                If (grdetalle.GetValue("pbcmin") > 0) Then

                    Dim cant As Integer = grdetalle.GetValue("pbcmin")
                    Dim stock As Integer = grdetalle.GetValue("stock")
                    If (cant > stock) Then
                        Dim lin As Integer = grdetalle.GetValue("pbnumi")
                        Dim pos As Integer = -1
                        _fnObtenerFilaDetalle(pos, lin)
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbcmin") = 1
                        CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbptot") = CType(grdetalle.DataSource, DataTable).Rows(pos).Item("pbpbas")


                        Dim img As Bitmap = New Bitmap(My.Resources.mensaje, 50, 50)
                        ToastNotification.Show(Me, "La cantidad de la venta no debe ser mayor al del stock" & vbCrLf &
                        "Stock=" + Str(stock).ToUpper, img, 6000, eToastGlowColor.Red, eToastPosition.BottomCenter)
                        grdetalle.SetValue("pbcmin", 1)
                        grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))

                        _prCalcularPrecioTotal()
                    Else
                        If (cant = stock) Then
                            'grdetalle.SelectedFormatStyle.ForeColor = Color.Blue
                            'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle = New GridEXFormatStyle
                            'grdetalle.CurrentRow.Cells(e.Column).FormatStyle.BackColor = Color.Pink
                            'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.BackColor = Color.DodgerBlue
                            'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.ForeColor = Color.White
                            'grdetalle.CurrentRow.Cells.Item(e.Column).FormatStyle.FontBold = TriState.True
                        End If
                    End If
                Else
                    grdetalle.SetValue("pbcmin", 1)
                    grdetalle.SetValue("pbptot", grdetalle.GetValue("pbpbas"))
                    grdetalle.SetValue("pbporc", 0)
                    grdetalle.SetValue("pbdesc", 0)
                    grdetalle.SetValue("pbtotdesc", grdetalle.GetValue("pbpbas"))

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
        If (grVentas.RowCount > 0) Then
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
            Dim res As Boolean = L_fnEliminarProforma(tbCodigo.Text, mensajeError)
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
        If e.KeyData = Keys.Enter Then
            MSuperTabControl.SelectedTabIndex = 0
            grdetalle.Focus()

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

            grdetalle.Select()

            grdetalle.Col = 3
            grdetalle.Row = 0

            'grdetalle.Focus()
        End If
    End Sub

    Private Sub ButtonX1_Click(sender As Object, e As EventArgs)
        If (Not _fnAccesible()) Then
            P_GenerarReporte()
        End If
    End Sub
    Private Sub P_GenerarReporte()
        Dim dt As DataTable = L_fnReporteProforma(tbCodigo.Text)
        Dim dt1 As DataTable = L_fnReporteProformaCompuesta(tbCodigo.Text)
        Dim _Ds As DataSet = L_Reporte_Factura_Cia("1")

        If Not IsNothing(P_Global.Visualizador) Then
            P_Global.Visualizador.Close()
        End If

        If SwServicio.Value = False Then
            P_Global.Visualizador = New Visualizador
            Dim objrep As New R_Proforma

            objrep.SetDataSource(dt)
            objrep.SetParameterValue("usuario", gs_user)
            objrep.SetParameterValue("Direccion", _Ds.Tables(0).Rows(0).Item("scdir").ToString)
            objrep.SetParameterValue("Telefono", _Ds.Tables(0).Rows(0).Item("sctelf").ToString)
            objrep.SetParameterValue("Ciudad", _Ds.Tables(0).Rows(0).Item("scciu").ToString)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep 'Comentar
            P_Global.Visualizador.ShowDialog() 'Comentar
            P_Global.Visualizador.BringToFront() 'Comentar

        Else
            Dim Total As Double = dt.Rows(0).Item("patotal") + dt1.Rows(0).Item("pcTotal")

            P_Global.Visualizador = New Visualizador
            Dim objrep As New R_ProformaCompuesta

            objrep.SetDataSource(dt)
            objrep.Subreports.Item("R_ProformaServicios.rpt").SetDataSource(dt1)
            objrep.SetParameterValue("usuario", gs_user)
            objrep.SetParameterValue("Direccion", _Ds.Tables(0).Rows(0).Item("scdir").ToString)
            objrep.SetParameterValue("Telefono", _Ds.Tables(0).Rows(0).Item("sctelf").ToString)
            objrep.SetParameterValue("Ciudad", _Ds.Tables(0).Rows(0).Item("scciu").ToString)
            objrep.SetParameterValue("TotalBs", Total.ToString)
            P_Global.Visualizador.CrGeneral.ReportSource = objrep
            P_Global.Visualizador.ShowDialog()
            P_Global.Visualizador.BringToFront()

        End If


    End Sub

    Private Sub cbSucursal_Leave(sender As Object, e As EventArgs) Handles cbSucursal.Leave
        grdetalle.Select()
    End Sub

    Private Sub btnImprimir_Click(sender As Object, e As EventArgs) Handles btnImprimir.Click
        If (Not _fnAccesible()) Then
            P_GenerarReporte()
        End If
    End Sub

    Private Sub btCliente_Click(sender As Object, e As EventArgs) Handles btCliente.Click
        prof_venta = True
        'P_Principal.btConfFabrica_Click(sender, e)
        Dim ef = New F1_Clientes
        ef._nameButton = "btConfCliente"
        ef._Tipo = 1
        ef.ShowDialog()

        If (ef.bandera = True) Then
            _CodCliente = codcli
            tbCliente.Text = nomcli
            _CodEmpleado = codvend
            tbVendedor.Text = nomvend
            tbObra.Focus()
        End If
    End Sub

    Private Sub btObra_Click(sender As Object, e As EventArgs) Handles btObra.Click
        'P_Principal.btObras_Click(sender, e)
        prof_venta = True
        Dim ef = New F1_Obras
        ef._nameButton = "btObras"
        ef.ShowDialog()

        If (ef.banderaobra = True) Then
            _CodObra = codobra
            tbObra.Text = nomobra

            grdetalle.Select()
            grdetalle.Col = 4
            grdetalle.Row = 0

        End If
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
                Modelo.MGlobal.SeleccionarCol = 1
                ef.SeleclCol = 1
                ef.listEstCeldas = listEstCeldas
                ef.alto = 120
                ef.ancho = 320
                ef.Context = "Seleccione Obra".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                    _CodObra = Row.Cells("oanumi").Value
                    tbObra.Text = Row.Cells("oanomb").Value

                    grdetalle.Select()
                    grdetalle.Col = 4
                    grdetalle.Row = 0
                End If

            End If
        End If
    End Sub

    Private Sub tbTransporte_ValueChanged(sender As Object, e As EventArgs) Handles tbTransporte.ValueChanged
        If (tbTransporte.Focused) Then
            Dim total As Double = tbtotal.Value
            If (Not tbTransporte.Text = String.Empty) Then
                If (tbTransporte.Value = 0) Then
                    tbTransporte.Value = 0
                    _prCalcularPrecioTotal()
                Else
                    Dim transporte As Double = tbTransporte.Value
                    tbtotal.Value = grdetalle.GetTotal(grdetalle.RootTable.Columns("pbptot"), AggregateFunction.Sum) - tbMdesc.Value + transporte

                End If

            End If

            If (tbTransporte.Text = String.Empty) Then
                tbTransporte.Value = 0
            End If
        End If
    End Sub

    Private Sub tbServicio_KeyDown(sender As Object, e As KeyEventArgs) Handles tbServicio.KeyDown
        If (_fnAccesible()) Then
            If e.KeyData = Keys.Control + Keys.Enter Then
                Dim dt As DataTable
                dt = L_fnListarProformaServicio()
                Dim listEstCeldas As New List(Of Modelo.Celda)
                listEstCeldas.Add(New Modelo.Celda("pcNumi,", True, "NRO PROFORMA", 140))
                listEstCeldas.Add(New Modelo.Celda("pcFDoc", True, "FECHA", 80, "dd/MM/yyyy"))
                listEstCeldas.Add(New Modelo.Celda("pcVen", False, "", 50))
                listEstCeldas.Add(New Modelo.Celda("vendedor", True, "VENDEDOR".ToUpper, 150))
                listEstCeldas.Add(New Modelo.Celda("pcClie", False, "", 50))
                listEstCeldas.Add(New Modelo.Celda("cliente", True, "CLIENTE", 150))
                listEstCeldas.Add(New Modelo.Celda("total", True, "TOTAL".ToUpper, 120))
                listEstCeldas.Add(New Modelo.Celda("pcObra,", False, "IdObra", 120))
                listEstCeldas.Add(New Modelo.Celda("oanomb,", True, "Obra", 150))
                listEstCeldas.Add(New Modelo.Celda("pcDesc,", False, "Descuento", 120))
                Dim ef = New Efecto
                ef.tipo = 3
                ef.dt = dt
                Modelo.MGlobal.SeleccionarCol = 4
                ef.SeleclCol = 2
                ef.listEstCeldas = listEstCeldas
                ef.alto = 250
                ef.ancho = 250
                ef.Context = "SELECCIONE PROFORMA SERVICIO".ToUpper
                ef.ShowDialog()
                Dim bandera As Boolean = False
                bandera = ef.band
                If (bandera = True) Then
                    If dt.Rows.Count > 0 Then
                        Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row
                        tbServicio.Text = Row.Cells("pcNumi").Value
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub SwServicio_ValueChanged(sender As Object, e As EventArgs) Handles SwServicio.ValueChanged
        If (_fnAccesible()) Then
            If (SwServicio.Value = True) Then
                tbServicio.BackColor = Color.White
                tbServicio.ReadOnly = True
                tbServicio.Enabled = True
                tbServicio.Focus()
            Else
                tbServicio.BackColor = Color.LightGray
                tbServicio.ReadOnly = True
                tbServicio.Enabled = False
            End If
        End If
    End Sub


End Class