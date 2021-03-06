﻿Imports System.ComponentModel
Imports System.Data.Linq
Imports System.Runtime.Serialization
Imports EasyGestControllers.Data.Entity

Namespace Data.Entity

#Region "Enums"

    Public Enum FormaOfertaEnum
        PrecioEspecial
        DescuentoPorcentual
        DescuentoFijo
        UnidadGratis
        UnidadConDescuento
        UnidadAPrecioEspecial
    End Enum

    Public Enum ModoPagoEnum As Short
        Metalico = 1
        Transferencia = 2
        Cheque = 3
        Tarjeta = 4
        Otro = 5
    End Enum

    Friend Enum TipoDatoEnum As Short
        Cadena = 0
        Numerico = 1
        Fecha = 2
        ValoLogico = 3
        Color = 4
    End Enum

    Friend Enum PrioridadTareaEnum As Short
        Baja = -1
        Normal = 0
        Alta = 1
    End Enum

    <Flags()>
    Public Enum Permiso
        SinAcceso = 0
        Visualizacion = 1
        Modificacion = 2
        Adicion = 4
        Eliminacion = 8
    End Enum
#End Region

#Region "Tablas"
    Partial Class Albaranes
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FCreacion = Now()
            _Cobrado = False
            _FAlbaran = _FCreacion
            _idModo = ModoPagoEnum.Metalico
            _Pendiente = 0.0F
            _SerieAlbaran = ""
            _idAlbaran = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L

        End Sub

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            If action = ChangeAction.Insert Then
                If _idUsuarioEmitido.HasValue() AndAlso _idUsuarioEmitido.Value = EasyGestControllers.Data.Context.EasyGestDataContext.IDUSUARIOSUPER Then
                    _idUsuarioEmitido = Nothing
                End If
            End If
        End Sub

    End Class

    Partial Class ApuntesDiario
        Inherits LINQEntityBase

        Public Property EsEntrada As Boolean = True

        Private Sub OnCreated()
            _Importe = 0.0
            _idApunte = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Concepto)
        End Function

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            If action = ChangeAction.Update Or action = ChangeAction.Insert Then
                _Importe = CType(IIf(EsEntrada, Math.Abs(_Importe), -1.0 * Math.Abs(Importe)), Single)
            End If
        End Sub
    End Class

    Partial Class CaracteristicasProducto
        Inherits LINQEntityBase
        Public Sub New(idProducto As Long)
            Me.New()
            _idProducto = idProducto
        End Sub

        Private Sub OnCreated()
            _Valor = ""
            _idCaracteristica = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

        Public ReadOnly Property NombrePropiedad() As String
            Get
                Return _Caracteristica.PascalCase()
            End Get
        End Property

        Public ReadOnly Property ValorDato As Object
            Get
                If Not String.IsNullOrEmpty(_Valor) Then
                    Try
                        Select Case _Tipo
                            Case TipoDatoEnum.Numerico
                                Return Double.Parse(_Valor)
                            Case TipoDatoEnum.Fecha
                                Return DateTime.Parse(_Valor)
                            Case TipoDatoEnum.ValoLogico
                                Return Boolean.Parse(_Valor)
                            Case TipoDatoEnum.Color
                                Return System.Drawing.ColorTranslator.FromHtml(_Valor)
                            Case Else
                                Return _Valor
                        End Select
                    Catch ex As Exception

                    End Try
                End If
                Select Case _Tipo
                    Case TipoDatoEnum.Numerico
                        Return 0D
                    Case TipoDatoEnum.Fecha
                        Return Today()
                    Case TipoDatoEnum.ValoLogico
                        Return Boolean.Parse("False")
                    Case TipoDatoEnum.Color
                        Return Color.White
                    Case Else
                        Return String.Empty
                End Select
            End Get
        End Property

        Public ReadOnly Property TipoDato() As Type
            Get
                Select Case _Tipo
                    Case TipoDatoEnum.Numerico
                        Return GetType(Double)
                    Case TipoDatoEnum.Fecha
                        Return GetType(DateTime)
                    Case TipoDatoEnum.ValoLogico
                        Return GetType(Boolean)
                    Case TipoDatoEnum.Color
                        Return GetType(Color)
                    Case Else
                        Return GetType(String)
                End Select
            End Get
        End Property
    End Class

    Partial Class Clientes
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _Activo = True
            _AplicableRE = False
            _Descuento = 0.0F
            _FCreacion = Now()
            _Pais = "ESPAÑA"
            _AplicableImpIndirecto = True
            _idCliente = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

        Public ReadOnly Property Numero() As String
            Get
                Return _FCreacion.ToString("yy") & "-" & Convert.ToString(_idCliente).PadLeft(5, "0"c)
            End Get
        End Property

        Public ReadOnly Property NombreYNombreCN() As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Nombre) And Not String.IsNullOrWhiteSpace(_Codigo)
        End Function

        Private Sub OnValidate(ByVal action As System.Data.Linq.ChangeAction)
            Select Case action
                Case ChangeAction.Insert
                    _FCreacion = Now()
                    _FModificacion = _FCreacion
                    _Nombre = _Nombre.Trim().ToUpper()
                Case ChangeAction.Update
                    _FModificacion = Now()
                    _Nombre = _Nombre.Trim().ToUpper()
            End Select
        End Sub
    End Class

    Partial Class CodigosBarra
        Inherits LINQEntityBase

        Public Sub New(idProducto As Long)
            Me.New()
            _idProducto = idProducto
        End Sub

        Private Sub OnCreated()
            _idCodigo = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

    End Class

    Partial Class Configuraciones
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idConfiguracion = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

    End Class
    Partial Class Contactos
        Inherits LINQEntityBase

        Public Const TIPO_PROPIETARIO_CLIENTE As Char = "C"c
        Public Const TIPO_PROPIETARIO_PROVEEDOR As Char = "P"c

        Private Sub OnCreated()
            _TipoPropietario = Microsoft.VisualBasic.ControlChars.NullChar
            _idPropietario = 0
            _idContacto = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnValidate(ByVal action As System.Data.Linq.ChangeAction)
            Select Case action
                Case ChangeAction.Update Or ChangeAction.Insert
                    _DatoContacto = _DatoContacto.Trim()
            End Select
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_DatoContacto) And _idPropietario > 0 And (_TipoPropietario.Equals(TIPO_PROPIETARIO_CLIENTE) Or
                _TipoPropietario.Equals(TIPO_PROPIETARIO_PROVEEDOR))
        End Function
    End Class

    Partial Class DatosBancario
        Inherits LINQEntityBase

        Public Const TIPO_PROPIETARIO_CLIENTE As Char = "C"c
        Public Const TIPO_PROPIETARIO_PROVEEDOR As Char = "P"c

        Private Sub OnCreated()
            _TipoPropietario = Microsoft.VisualBasic.ControlChars.NullChar
            _idPropietario = 0
            _idCuenta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnValidate(action As ChangeAction)
            Select Case action
                Case ChangeAction.Insert Or ChangeAction.Update
                    _Banco = _Banco.Trim()
            End Select
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Banco) And Not String.IsNullOrWhiteSpace(_CCC) And _idPropietario > 0 And (_TipoPropietario.Equals(TIPO_PROPIETARIO_CLIENTE) Or
                _TipoPropietario.Equals(TIPO_PROPIETARIO_PROVEEDOR))
        End Function

    End Class

    Partial Class Diarios
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _Apertura = Now()
            _idDiario = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now().Ticks) * -1L
        End Sub

        Public ReadOnly Property Diferencia() As Single
            Get
                Dim real As Single = 0.0F
                Dim supuesto As Single = 0.0F
                If _Cierre IsNot Nothing Then
                    If _CierreReal.HasValue() Then real = _CierreReal.Value
                    If _CierreSupuesto.HasValue() Then supuesto = _CierreSupuesto.Value
                End If
                Return real - supuesto
            End Get
        End Property

        Public ReadOnly Property Cerrado As Boolean
            Get
                Return _Cierre.HasValue()
            End Get
        End Property

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            If _idUsuario.HasValue() AndAlso _idUsuario.Value = EasyGestControllers.Data.Context.EasyGestDataContext.IDUSUARIOSUPER Then
                _idUsuario = Nothing
            End If
        End Sub
    End Class

    Partial Class DiseñosEtiqueta
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idEtiqueta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
            _XMLDiseño = Nothing
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(Nombre) And (TipoEtiqueta.Equals(Etiquetas.TIPO_ETIQUETA_ENHOJA) Or TipoEtiqueta.Equals(Etiquetas.TIPO_ETIQUETA_ENROLLO))
        End Function

        Public Property UniqueIDEtiqueta As String
            Get
                Return String.Concat(TipoEtiqueta & idEtiqueta)
            End Get
            Set(value As String)
                Dim tipo As Char = Etiquetas.TIPO_ETIQUETA_ENHOJA
                Dim id As Long = 0
                Try
                    tipo = value.ToCharArray().First()
                    id = CLng(value.Substring(1))
                Catch ex As Exception

                End Try
                idEtiqueta = id
                TipoEtiqueta = tipo
            End Set
        End Property
    End Class

    Partial Class Empresas
        Inherits LINQEntityBase

        Public Const FormatoNumeracionNumero As String = "n"
        Public Const FormatoNumeracionAño As String = "a"
        Public Const FormatoNumeracionSerie As String = "s"

        Private Sub OnCreated()
            _idEmpresa = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1
            _FormatoNumeracion = String.Format("{0}-{1}-{2}", FormatoNumeracionNumero, FormatoNumeracionAño, FormatoNumeracionSerie)
            _Series = ""
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_NIF) And Not String.IsNullOrWhiteSpace(_Empresa)
        End Function

        Public Property LogoImagen As Image
            Get
                If IsNothing(_Logo) Then
                    Return Nothing
                Else
                    Dim buffer As IO.MemoryStream = Nothing
                    Dim img As Drawing.Image = Nothing
                    Try
                        buffer = New System.IO.MemoryStream(_Logo.ToArray())
                        img = DirectCast(Image.FromStream(buffer).Clone(), Image)
                        buffer.Close()
                        buffer.Dispose()
                        buffer = Nothing
                    Catch ex As Exception

                    End Try
                    Return img
                End If
            End Get
            Set(value As Image)
                If IsNothing(value) Then
                    _Logo = Nothing
                Else
                    Dim buffer As IO.MemoryStream = New IO.MemoryStream()
                    Try
                        value.Save(buffer, value.RawFormat)
                        _Logo = New System.Data.Linq.Binary(buffer.ToArray())
                        buffer.Close()
                        buffer.Dispose()
                        buffer = Nothing
                    Catch ex As Exception

                    End Try
                End If
            End Set
        End Property

        Public ReadOnly Property SeriesArray As String()
            Get
                Dim s As String() = New String() {}
                If Not String.IsNullOrEmpty(_Series) Then
                    s = _Series.Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray()
                End If
                Return s
            End Get
        End Property
    End Class

    Partial Class Encargos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FechaEncargo = Today()
            _idEncargo = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Descripcion) And Not String.IsNullOrWhiteSpace(_Cliente) And Not String.IsNullOrWhiteSpace(_Telefono)
        End Function

    End Class

    Partial Class EtiquetasEnHoja
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idEtiqueta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return _MargenDer >= 0.0 And _MargenInf >= 0.0 And _MargenIzq >= 0.0 And
                _MargenSup >= 0.0 And _Alto > 0.0 And _Ancho > 0.0 And _Columnas > 0.0 And _Filas > 0.0 And
                _EspacioH >= 0.0 And _EspacioV >= 0.0 And Not String.IsNullOrWhiteSpace(_Referencia)
        End Function

        Public Property UnidadMediad As String
            Get
                Return "CM"
            End Get
            Private Set(value As String)

            End Set
        End Property

        Public ReadOnly Property NombreEtiqueta() As String
            Get
                Return "Ref:" & _Referencia & " " & _Filas & "X" & _Columnas & " An:" & _Ancho & UnidadMediad & " Al:" & _Alto & UnidadMediad
            End Get
        End Property

    End Class

    Partial Class EtiquetasEnRollo
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idEtiqueta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Dim unidades As String() = New String() {"CM", "INCH", "POINT"}
            Return _Alto > 0.0 And _Ancho > 0.0 And Not String.IsNullOrWhiteSpace(_Referencia) And unidades.Contains(_UnidadMedida)
        End Function

        Public ReadOnly Property NombreEtiqueta() As String
            Get
                Return "Ref:" & _Referencia & " An:" & _Ancho & _UnidadMedida & " Al:" & _Alto & _UnidadMedida
            End Get
        End Property
    End Class

    Partial Class Facturas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FCreacion = Now()
            _FFactura = _FCreacion
            _SerieFactura = ""
            _idFactura = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnValidate(action As ChangeAction)
            If action = ChangeAction.Insert Then
                If _idUsuarioEmitido.HasValue() AndAlso _idUsuarioEmitido.Value = EasyGestControllers.Data.Context.EasyGestDataContext.IDUSUARIOSUPER Then
                    _idUsuarioEmitido = Nothing
                End If
            End If
        End Sub
    End Class

    Partial Class Familias
        Inherits LINQEntityBase

        Private Sub OnCreated()
            idFamilia = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Familia)
        End Function

    End Class

    Partial Class FormasContacto
        Inherits LINQEntityBase

        Public ReadOnly Property FormaContacto() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _FormaContactoChn
                Else
                    Return _FormaContactoEsp
                End If
            End Get
        End Property

        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub
    End Class

    Partial Class Galerias
        Inherits LINQEntityBase
        Public Sub New(idProducto As Long)
            Me.New()
            _idProducto = idProducto
        End Sub

        Private Sub OnCreated()
            _idGaleria = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1
        End Sub
    End Class

    Partial Class Impuestos
        Inherits LINQEntityBase

    End Class

    Partial Class LineasAlbaran
        Inherits LINQEntityBase

        Public ReadOnly Property ImporteDescuento As Single
            Get
                Return (ImporteBase + ImporteImpuesto + ImporteRE) * (_Descuento / 100.0F)
            End Get
        End Property

        Public ReadOnly Property ImporteRE As Single
            Get
                If _Recargo.HasValue Then
                    Return _Cantidad * _Precio * (_Recargo.Value / 100.0F)
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteImpuesto As Single
            Get
                If _Impuesto.HasValue Then
                    Return _Cantidad * _Precio * (_Impuesto.Value / 100.0F)
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteBase As Single
            Get
                Return _Cantidad * _Precio
            End Get
        End Property

        Public ReadOnly Property Importe As Single
            Get
                Return _Cantidad * _PrecioFinal
            End Get
        End Property

        Private Sub OnCreated()
            _idLinea = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1
            _idProducto = Nothing
            _DescripcionProducto = Nothing
            _Descuento = 0.0F
            _Precio = 0.0F
            _Cantidad = 1.0F
        End Sub

        Private Sub RecalcularPrecioFinal()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value / 100.0F
            If _Recargo.HasValue Then re = _Recargo.Value / 100.0F
            _PrecioFinal = _Precio * ((1.0F + imp + re) * (1.0F - _Descuento / 100.0))
        End Sub
        Private Sub RecalcularPrecioNeto()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value
            If _Recargo.HasValue Then re = _Recargo.Value
            _Precio = _PrecioFinal / ((1.0F + imp + re) * (1.0F - _Descuento / 100.0))
        End Sub

        Private Sub OnPrecioNetoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnImpuestoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnRecargoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnPrecioFinalChanged()
            RecalcularPrecioNeto()
        End Sub

        Private Sub OnDescuentoChanged()
            RecalcularPrecioFinal()
        End Sub
    End Class

    Partial Class LineasFactura
        Inherits LINQEntityBase

        Public ReadOnly Property ImporteDescuento As Single
            Get
                Return (ImporteBase + ImporteImpuesto + ImporteRE) * (_Descuento / 100.0F)
            End Get
        End Property

        Public ReadOnly Property ImporteRE As Single
            Get
                If _Recargo.HasValue Then
                    Return _Cantidad * _Precio * _Recargo.Value / 100.0F
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteImpuesto As Single
            Get
                If _Impuesto.HasValue Then
                    Return _Cantidad * _Precio * _Impuesto.Value / 100.0F
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteBase As Single
            Get
                Return _Cantidad * _Precio
            End Get
        End Property

        Public ReadOnly Property Importe As Single
            Get
                Return _Cantidad * _PrecioFinal
            End Get
        End Property

        Private Sub OnCreated()
            _idLinea = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1
            _idProducto = Nothing
            _DescripcionProducto = Nothing
            _Precio = 0.0F
            _Cantidad = 1.0F
            _Referencia = ""
        End Sub

        Private Sub RecalcularPrecioFinal()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value / 100.0F
            If _Recargo.HasValue Then re = _Recargo.Value / 100.0F
            _PrecioFinal = _Precio * ((1.0F + imp + re) * (1.0F - _Descuento / 100.0F))
        End Sub

        Private Sub RecalcularPrecioNeto()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value
            If _Recargo.HasValue Then re = _Recargo.Value
            _Precio = _PrecioFinal / ((1.0F + imp + re) * (1.0F - _Descuento / 100.0))
        End Sub

        Private Sub OnPrecioFinalChanged()
            RecalcularPrecioNeto()
        End Sub

        Private Sub OnPrecioNetoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnRecargoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnImpuestoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnDescuentoChanged()
            RecalcularPrecioFinal()
        End Sub
    End Class

    Partial Class LineasPedido
        Inherits LINQEntityBase

        Public Property PrecioVenta As Single
        Public Property Referencia As String

        Public ReadOnly Property ImporteDescuento As Single
            Get
                Return (ImporteBase + ImporteRE + ImporteImpuesto) * (_Descuento / 100.0F)
            End Get
        End Property

        Public ReadOnly Property Importe As Single
            Get
                Return _Cantidad * _PrecioFinal
            End Get
        End Property

        Public ReadOnly Property ImporteImpuesto As Single
            Get
                If _Impuesto.HasValue Then
                    Return _Cantidad * _Precio * _Impuesto.Value / 100.0F
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteRE As Single
            Get
                If _Recargo.HasValue Then
                    Return _Cantidad * _Precio * _Recargo.Value / 100.0F
                Else
                    Return 0.0F
                End If
            End Get
        End Property

        Public ReadOnly Property ImporteBase As Single
            Get
                Return _Cantidad * _Precio
            End Get
        End Property

        Private Sub OnCreated()
            _idLinea = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
            _Precio = 0.0F
            _Cantidad = 1.0F
            _PrecioVenta = 0.0F
            _Referencia = ""
        End Sub

        Private Sub RecalcularPrecioFinal()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value / 100.0F
            If _Recargo.HasValue Then re = _Recargo.Value / 100.0F
            _PrecioFinal = _Precio * ((1.0F + imp + re) * (1.0F - _Descuento / 100.0F))
        End Sub

        Private Sub RecalcularPrecioNeto()
            Dim imp As Single = 0.0F, re As Single = 0.0F
            If _Impuesto.HasValue Then imp = _Impuesto.Value
            If _Recargo.HasValue Then re = _Recargo.Value
            _Precio = _PrecioFinal / ((1.0F + imp + re) * (1.0F - _Descuento / 100.0))
        End Sub

        Private Sub OnRecargoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnPrecioFinalChanged()
            RecalcularPrecioNeto()
        End Sub

        Private Sub OnImpuestoChanged()
            RecalcularPrecioFinal()
        End Sub

        Private Sub OnPrecioNetoChanged()
            RecalcularPrecioFinal()
        End Sub
    End Class

    Partial Class LineasImpuestoAlbaran
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idLineaImpuesto = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now.Ticks) * -1L
        End Sub

    End Class

    Partial Class LineasImpuestoFactura
        Inherits LINQEntityBase
        Private Sub OnCreated()
            _idLineaImpuesto = Math.Abs(Context.EasyGestDataContext.FECHAREFERENCIA.Ticks - Now.Ticks) * -1L
        End Sub
    End Class

    Partial Class ListasCompra
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FechaCreacion = Today()
            _idEncargo = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Descripcion)
        End Function

        Public ReadOnly Property EsEncargo As Boolean
            Get
                Return _idEncargo IsNot Nothing
            End Get
        End Property

    End Class

    Partial Class Marcas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idMarca = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Marca)
        End Function

    End Class

    Partial Class ModosPago
        Inherits LINQEntityBase
        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub

        Public ReadOnly Property ModoPago() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _ModoPagoChn
                Else
                    Return _ModoPagoEsp
                End If
            End Get
        End Property

    End Class
    Partial Class Morosidades
        Inherits LINQEntityBase
        Private Sub OnCreated()
            _FCreacion = Now()
            _idMorosidad = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub
    End Class

    Partial Class MovimientosMorosidad
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FMovimiento = Now()
            _idMovimiento = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnValidate(action As ChangeAction)
            If action = ChangeAction.Insert Then
                If _idUsuario.HasValue() AndAlso _idUsuario.Value = EasyGestControllers.Data.Context.EasyGestDataContext.IDUSUARIOSUPER Then
                    _idUsuario = Nothing
                End If
            End If
        End Sub
    End Class

    Partial Class MovimientosTarjeta
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FMovimiento = Now()
            _idMovimiento = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

    End Class

    Partial Class MunicipiosEspañolas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub

    End Class

    Partial Class Ofertas
        Inherits LINQEntityBase

        'Public Const CaracterSeparador As Char = ":"c
        Private Sub OnCreated()
            _FCreacion = Now()
            _FInicio = Today()
            _FFinalizacion = New Date(2079, 6, 1)
            _Forma = FormaOfertaEnum.PrecioEspecial
            _Activo = True
            _Valor1 = 0.0
            _Valor2 = 0.0
            _NumeroOferta = "00000"
            _idOferta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not (_Valor1 = 0.0 And _Valor2 = 0.0)
        End Function

        Public Property DiaDuracion As Integer
            Get
                If _FFinalizacion = Date.MaxValue Then
                    Return 0
                Else
                    Return _FFinalizacion.Subtract(_FInicio).Days
                End If
            End Get
            Set(ByVal value As Integer)
                If value <= 0 Then
                    _FFinalizacion = Date.MaxValue
                Else
                    _FFinalizacion = FInicio.AddDays(value)
                End If
            End Set

        End Property

        Public ReadOnly Property Plan As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Select Case _Forma
                        Case FormaOfertaEnum.PrecioEspecial
                            Return "特价" & FormatCurrency(_Valor1, 2)
                        Case FormaOfertaEnum.DescuentoPorcentual
                            Return "打折" & FormatPercent(_Valor1 / 100, 2)
                        Case FormaOfertaEnum.DescuentoFijo
                            Return "减价" & FormatCurrency(_Valor1, 2)
                        Case FormaOfertaEnum.UnidadGratis
                            Return String.Format("{0}送{1}", _Valor1, _Valor2)
                        Case FormaOfertaEnum.UnidadConDescuento
                            Return String.Format("第{0}个打折{1}", _Valor1, FormatPercent(_Valor2 / 100, 2))
                        Case FormaOfertaEnum.UnidadAPrecioEspecial
                            Return String.Format("第{0}个特价{1}", _Valor1, FormatCurrency(_Valor2, 2))
                        Case Else
                            Return "未确定"
                    End Select
                Else
                    Select Case _Forma
                        Case FormaOfertaEnum.PrecioEspecial
                            Return "Precio especial " & FormatCurrency(_Valor1, 2)
                        Case FormaOfertaEnum.DescuentoPorcentual
                            Return "Desc. porcentual " & FormatPercent(_Valor1 / 100, 2)
                        Case FormaOfertaEnum.DescuentoFijo
                            Return "Desc. fijo" & FormatCurrency(_Valor1, 2)
                        Case FormaOfertaEnum.UnidadGratis
                            Return String.Format("{0} por {1}", _Valor1, _Valor2)
                        Case FormaOfertaEnum.UnidadConDescuento
                            Return String.Format("La {0}ª unidad con descuento {1}", _Valor1, FormatPercent(_Valor2 / 100, 2))
                        Case FormaOfertaEnum.UnidadAPrecioEspecial
                            Return String.Format("La {0}ª unidad a precio {1}", _Valor1, FormatCurrency(_Valor2, 2))
                        Case Else
                            Return "No definido"
                    End Select
                End If
            End Get
        End Property

        Private Sub OnValidate(ByVal action As System.Data.Linq.ChangeAction)
            If action = ChangeAction.Insert Or action = ChangeAction.Update Then
                If Forma = FormaOfertaEnum.PrecioEspecial Or Forma = FormaOfertaEnum.DescuentoPorcentual Or Forma = FormaOfertaEnum.DescuentoFijo Then
                    Valor2 = 0.0
                End If
            End If
        End Sub
    End Class

    Partial Class Pedidos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _FCreacion = Now()
            _Pagado = False
            _Pago = 0.0F
            _idPedido = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            If action = ChangeAction.Insert Then
                If _idUsuarioEmitido.HasValue() AndAlso _idUsuarioEmitido.Value = EasyGestControllers.Data.Context.EasyGestDataContext.IDUSUARIOSUPER Then
                    _idUsuarioEmitido = Nothing
                End If
            End If
        End Sub
    End Class

    Partial Class Productos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _ControlStock = True
            _PrecioVariable = False
            _Descuento = 0.0F
            _PrecioVenta = 0.0F
            _Coste = 0.0F
            _FCreacion = Now()
            _Activo = True
            _UnidadVenta = 1.0F
            _UnidadXCaja = 1.0F
            _idProducto = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Private Sub OnLoaded()
            MyBase.OnLoaded()
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Referencia) And Not String.IsNullOrWhiteSpace(_Descripcion)
        End Function

        Private Sub OnValidate(ByVal action As System.Data.Linq.ChangeAction)
            Select Case action
                Case ChangeAction.Insert
                    _FCreacion = Now()
                    _FModificacion = _FCreacion
                    _Descripcion = _Descripcion.Trim().ToUpper()
                    _Referencia = _Referencia.Trim().ToUpper()
                Case ChangeAction.Update
                    _FModificacion = Now()
                    _Descripcion = _Descripcion.Trim().ToUpper()
                    _Referencia = _Referencia.Trim().ToUpper()
            End Select
        End Sub

    End Class

    Partial Class PrioridadesTarea
        Inherits LINQEntityBase

        Private Sub OnCreated()
            [ReadOnly] = True
        End Sub


        Public ReadOnly Property PrioridadID As Short
            Get
                Return _idPrioridad
            End Get
        End Property

        Public ReadOnly Property Prioridad() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _PrioridadChn
                Else
                    Return _PrioridadEsp
                End If
            End Get
        End Property

    End Class

    Partial Class Proveedores
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _Activo = True
            _FCreacion = Now()
            _Pais = "ESPAÑA"
            _idProveedor = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public ReadOnly Property NombreYNombreCN() As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

        Public ReadOnly Property Numero() As String
            Get
                Return _FCreacion.ToString("yy") & "-" & Convert.ToString(_idProveedor).PadLeft(5, "0"c)
            End Get
        End Property

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Nombre)
        End Function

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            Select Case action
                Case ChangeAction.Insert
                    _FCreacion = Now()
                    _FModificacion = _FCreacion
                    _Nombre = _Nombre.Trim().ToUpper()
                Case ChangeAction.Update
                    _FModificacion = Now()
                    _Nombre = _Nombre.Trim().ToUpper()
            End Select
        End Sub
    End Class

    Partial Class ProvinciasEspañolas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub

    End Class

    Partial Class Puestos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idPuesto = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
            _EsCajaCentral = False
            _Puesto = ""
        End Sub


        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Identificacion) And Not String.IsNullOrWhiteSpace(_Puesto)
        End Function


    End Class

    Partial Class Tareas
        Inherits LINQEntityBase

        Private _avisoAntelacion As TimeSpan = TimeSpan.Zero

        Private Sub OnCreated()
            _idTarea = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
            _Prioridad = PrioridadTareaEnum.Normal
            _FechaTarea = Today()
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Tarea)
        End Function

        Public Shared PosiblesHorasAntelacion() As Integer = New Integer() {1, 2, 3, 4, 6, 8, 10, 12, 18, 24, 48, 72}

        Public Property AvisoAntelacion As Integer
            Get
                Return _avisoAntelacion.Hours
            End Get
            Set(value As Integer)
                If Not _avisoAntelacion.Hours.Equals(value) Then
                    Me.SendPropertyChanging()
                    _avisoAntelacion = New TimeSpan(value, 0, 0)
                    CambiarAviso()
                    Me.SendPropertyChanged("AvisoAntelacion")
                End If
            End Set
        End Property

        Private Sub OnFechaTareaChanged()
            CambiarAviso()
        End Sub

        Private Sub CambiarAviso()
            Aviso = _FechaTarea.Subtract(_avisoAntelacion)
        End Sub

    End Class

    Partial Class TarjetasFidelizacion
        Inherits LINQEntityBase

        Private _FormulaSaldo As String = Nothing

        Public ReadOnly Property EnPunto As Boolean
            Get
                Return Not _EnSaldo
            End Get
        End Property

        Private Sub OnCreated()
            _idTarjeta = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
            _FCreacion = Today()
            _Activo = True
            _Saldo = 0
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return _Base > 0.0 And _BasePunto > 0.0 And _Beneficio > 0.0 And _EquivalenciaPunto > 0.0 And Not String.IsNullOrWhiteSpace(_NumeroTarjeta)
        End Function

        Public WriteOnly Property FormulaSaldo As String
            Set(value As String)
                _FormulaSaldo = value
                Dim _b1, _b2, _b3, _b4 As Single

                Try
                    Util.Comunes.InterpretarFormulaFidelizacion(_FormulaSaldo, _b1, _b2, _b3, _b4)
                    _Base = _b1
                    _Beneficio = _b2
                    _BasePunto = _b3
                    _EquivalenciaPunto = _b4
                Catch ex As Exception

                End Try
            End Set
        End Property

    End Class

    Partial Class TiposIdentificacion
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub

        Public ReadOnly Property TipoID As Short
            Get
                Return _idTipo
            End Get
        End Property

        Public ReadOnly Property TipoIdentificacion() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _TipoIdentificacionChn
                Else
                    Return _TipoIdentificacionEsp
                End If
            End Get
        End Property


    End Class

    Partial Class TiposDatoCaractProducto
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.ReadOnly = True
        End Sub

        Public ReadOnly Property TipoID As Short
            Get
                Return _idTipoDato
            End Get
        End Property

        Public ReadOnly Property TipoDato() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _TipoDatoChn
                Else
                    Return _TipoDatoEsp
                End If
            End Get
        End Property

    End Class

    Partial Class UnidadesMedida
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idUnidadMedida = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Medida) And Not String.IsNullOrWhiteSpace(_Abreviatura)
        End Function

    End Class

    Partial Class Ubicaciones
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _idUbicacion = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        'Private Sub OnLoaded()
        '    MyBase.OnLoaded()
        'End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Ubicacion)
        End Function

    End Class

    Partial Class Usuarios
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _Activo = True
            _FCreacion = Today
            _IdiomaPreferente = "ES"
            _TamañoGrid = 0
            _idUsuario = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        'Private Sub OnLoaded()
        '    MyBase.OnLoaded()
        'End Sub

        Public Overrides Function IsValid() As Boolean
            Return Not String.IsNullOrWhiteSpace(_Nombre) And Not String.IsNullOrWhiteSpace(_NombreSesion) And Not String.IsNullOrWhiteSpace(_Contraseña)
        End Function

        Public Function IsSuper() As Boolean
            Return _Nombre.Equals("Super") And _NombreSesion.Equals("Super") And _idUsuario = Context.EasyGestDataContext.IDUSUARIOSUPER
        End Function

    End Class

    Partial Class Vales
        Inherits LINQEntityBase

        Private Sub OnCreated()
            _Activo = True
            _FEmision = Now()
            _ObtenidoXFidelizacion = False
            _idVale = Math.Abs(Now().Ticks - Context.EasyGestDataContext.FECHAREFERENCIA.Ticks) * -1L
        End Sub

        <DataMember()>
        Public ReadOnly Property Caducado As Boolean
            Get
                Return _FValidez.HasValue AndAlso _FValidez.Value.CompareTo(Today()) < 0
            End Get
        End Property

        Public Overrides Function IsValid() As Boolean
            Return _Importe >= 0.0F
        End Function

        Private Sub OnValidate(action As System.Data.Linq.ChangeAction)
            If action = ChangeAction.Insert Then
                _Numero = GenerarNumeracion()
            End If
        End Sub

        Private Function GenerarNumeracion() As String
            Dim numero As String = _FEmision.ToString("yy")
            numero &= _FEmision.DayOfYear.ToString().PadLeft(3, "0"c)
            numero &= _FEmision.ToString("HHmm")
            numero &= DirectCast(IIf(_EsPunto, "1", "0"), String)
            numero &= FormatNumber(_Importe, 2).Replace(Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator, "").PadLeft(5, "0"c)
            Return numero
        End Function

        'Private Sub OnLoaded()
        '    MyBase.OnLoaded()
        'End Sub
    End Class
#End Region

#Region "Vistas"


    Partial Class VWAlbaranes
        Inherits LINQEntityBase

        Public Property Cambio As Single
        Public Property Entregado As Single

        Public ReadOnly Property ModoPago() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _ModoPagoChn
                Else
                    Return _ModoPagoEsp
                End If
            End Get
        End Property

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWFacturas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

    End Class

    Partial Class VWClientes
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property Numero() As String
            Get
                Return _FCreacion.ToString("yy") & "-" & Convert.ToString(_idCliente).PadLeft(5, "0"c)
            End Get
        End Property

        Public ReadOnly Property TipoIdentificacion() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _TipoIdentificacionChn
                Else
                    Return _TipoIdentificacionEsp
                End If
            End Get
        End Property

        Public ReadOnly Property NombreYNombreCN() As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

    End Class

    Partial Class VWCobros
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
        Public ReadOnly Property ModoPago() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _ModoPagoChn
                Else
                    Return _ModoPagoEsp
                End If
            End Get
        End Property

    End Class

    Partial Class VWContactos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
        Public ReadOnly Property IconTipoPropietario() As Drawing.Image
            Get
                If _TipoPropietario = Data.Entity.Contactos.TIPO_PROPIETARIO_CLIENTE Then
                    Return My.Resources.user_green
                End If
                If _TipoPropietario = Data.Entity.Contactos.TIPO_PROPIETARIO_PROVEEDOR Then
                    Return My.Resources.user_red
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Propietario As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

        Public ReadOnly Property FormaContacto() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _FormaContactoChn
                Else
                    Return _FormaContactoEsp
                End If
            End Get
        End Property
    End Class

    Partial Class VWCreditosCliente
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property DiaTardanza() As Integer
            Get
                Return Now().Subtract(_FCreacion).Days
            End Get
        End Property

        Public ReadOnly Property CantidadPendiente() As Single
            Get
                Return _Importe - _ImporteGestionado
            End Get
        End Property

    End Class

    Partial Class VWDatosBancario
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property IconTipoPropietario() As Drawing.Image
            Get
                If _TipoPropietario = Data.Entity.DatosBancario.TIPO_PROPIETARIO_CLIENTE Then
                    Return My.Resources.user_green
                End If
                If _TipoPropietario = Data.Entity.DatosBancario.TIPO_PROPIETARIO_PROVEEDOR Then
                    Return My.Resources.user_red
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property Propietario As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

    End Class

    Partial Class VWDeberesProveedor
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property DiaTardanza() As Integer
            Get
                Return Now().Subtract(_FCreacion).Days
            End Get
        End Property

        Public ReadOnly Property CantidadPendiente() As Single
            Get
                Return _Importe - _ImporteGestionado
            End Get
        End Property

    End Class

    Partial Class VWDiarios
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWFacturacion
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWLineasAlbaran
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

    End Class

    Partial Class VWLineasPedido
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWLineasFactura
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWTareas
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property PrioridadTexto() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _PrioridadChn
                Else
                    Return _PrioridadEsp
                End If
            End Get
        End Property

        Public ReadOnly Property ImagenPrioridad As Image
            Get
                Dim imagen As Image = Nothing
                Select Case _Prioridad
                    Case PrioridadTareaEnum.Alta
                        imagen = My.Resources.bullet_red
                    Case PrioridadTareaEnum.Baja
                        imagen = My.Resources.bullet_blue
                    Case PrioridadTareaEnum.Normal
                        imagen = My.Resources.bullet_black
                End Select
                Return imagen
            End Get
        End Property

        Public ReadOnly Property ColorPrioridad As Color
            Get
                If _Prioridad = PrioridadTareaEnum.Baja Then
                    Return Color.Blue
                ElseIf _Prioridad = PrioridadTareaEnum.Alta Then
                    Return Color.Red
                Else
                    Return Color.Black
                End If
            End Get
        End Property

        Public ReadOnly Property HoraTarea As DateTime
            Get
                Return _FechaTarea
            End Get
        End Property
    End Class

    Partial Class VWTarjetasFidelizacion
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
        Public ReadOnly Property EnPunto As Boolean
            Get
                Return Not _EnSaldo
            End Get
        End Property

        Public ReadOnly Property Condicion As String
            Get
                Dim _cadena As String
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    _cadena = CStr(IIf(_EnSaldo, "每购买{0}欧元得到{1}金额。", "每购买{0}欧元得到{1}点数， 而每{2}点相当于{3}欧元。"))
                Else
                    _cadena = CStr(IIf(_EnSaldo, "Por cada {0} € de compra obtiene {1} saldo/s.", "Por cada {0}€ de compra obtiene {1} punto/s, y por cada {2} punto/s equivale {3} €"))
                End If

                Return String.Format(_cadena, _Base, _Beneficio, _BasePunto, _EquivalenciaPunto)
            End Get
        End Property

        Public ReadOnly Property Cliente() As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property
    End Class

    Partial Class VWPagos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
        Public ReadOnly Property ModoPago() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _ModoPagoChn
                Else
                    Return _ModoPagoEsp
                End If
            End Get
        End Property

    End Class

    Partial Class VWPedidos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
    End Class

    Partial Class VWProductos
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property PlanOferta As String
            Get
                If _FormaOferta.HasValue() And _OfertaFormula1.HasValue() And _OfertaFormula2.HasValue() Then
                    If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                        Select Case _FormaOferta.Value
                            Case Data.Entity.FormaOfertaEnum.PrecioEspecial
                                Return "特价" & FormatCurrency(_OfertaFormula1.Value, 2)
                            Case Data.Entity.FormaOfertaEnum.DescuentoPorcentual
                                Return "打折" & FormatPercent(_OfertaFormula1.Value / 100, 2)
                            Case Data.Entity.FormaOfertaEnum.DescuentoFijo
                                Return "减价" & FormatCurrency(_OfertaFormula1.Value, 2)
                            Case Data.Entity.FormaOfertaEnum.UnidadGratis
                                Return String.Format("{0}送{1}", _OfertaFormula1.Value, _OfertaFormula2.Value)
                            Case Data.Entity.FormaOfertaEnum.UnidadConDescuento
                                Return String.Format("第{0}个打折{1}", _OfertaFormula1.Value, FormatPercent(_OfertaFormula2.Value / 100, 2))
                            Case Data.Entity.FormaOfertaEnum.UnidadAPrecioEspecial
                                Return String.Format("第{0}个特价{1}", _OfertaFormula1.Value, FormatCurrency(_OfertaFormula2.Value, 2))
                            Case Else
                                Return "未确定"
                        End Select
                    Else
                        Select Case _FormaOferta.Value
                            Case Data.Entity.FormaOfertaEnum.PrecioEspecial
                                Return "Precio especial " & FormatCurrency(_OfertaFormula1.Value, 2)
                            Case Data.Entity.FormaOfertaEnum.DescuentoPorcentual
                                Return "Desc. porcentual " & FormatPercent(_OfertaFormula1.Value / 100, 2)
                            Case Data.Entity.FormaOfertaEnum.DescuentoFijo
                                Return "Desc. fijo" & FormatCurrency(_OfertaFormula1.Value, 2)
                            Case Data.Entity.FormaOfertaEnum.UnidadGratis
                                Return String.Format("{0} por {1}", _OfertaFormula1.Value, _OfertaFormula2.Value)
                            Case Data.Entity.FormaOfertaEnum.UnidadConDescuento
                                Return String.Format("La {0}ª unidad con descuento {1}", _OfertaFormula1.Value, FormatPercent(_OfertaFormula2.Value / 100, 2))
                            Case Data.Entity.FormaOfertaEnum.UnidadAPrecioEspecial
                                Return String.Format("La {0}ª unidad a precio {1}", _OfertaFormula1.Value, FormatCurrency(_OfertaFormula2.Value, 2))
                            Case Else
                                Return "No definido"
                        End Select
                    End If
                Else
                    Return ""
                End If
            End Get
        End Property

    End Class

    Partial Class VWProveedores
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub
        Public ReadOnly Property Numero() As String
            Get
                Return _FCreacion.ToString("yy") & "-" & Convert.ToString(_idProveedor).PadLeft(5, "0"c)
            End Get
        End Property

        Public ReadOnly Property NombreYNombreCN() As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property

        Public ReadOnly Property TipoIdentificacion() As String
            Get
                If Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.Equals("zh") Then
                    Return _TipoIdentificacionChn
                Else
                    Return _TipoIdentificacionEsp
                End If
            End Get
        End Property

    End Class

    Partial Class VWVales
        Inherits LINQEntityBase

        Private Sub OnCreated()
            Me.[ReadOnly] = True
        End Sub

        Public ReadOnly Property Cliente As String
            Get
                If String.IsNullOrEmpty(_NombreCN) Then
                    Return _Nombre
                Else
                    Return _Nombre & " (" & _NombreCN & ")"
                End If
            End Get
        End Property
    End Class
#End Region

End Namespace

Namespace Data.Context

    Partial Class EasyGestDataContext

        'Private Shared _context As EasyGestDataContext

        Friend Const IDUSUARIOSUPER As Long = 0L
        Friend Const FECHAREFERENCIA As Date = #2016/01/01#
        Private Shared ReadOnlyEntities As Type() = New Type() {GetType(Entity.FormasContacto),
                                                                GetType(Entity.ModosPago),
                                                                GetType(Entity.MunicipiosEspañolas),
                                                                GetType(Entity.PrioridadesTarea),
                                                                GetType(Entity.ProvinciasEspañolas),
                                                                GetType(Entity.TiposDatoCaractProducto),
                                                                GetType(Entity.TiposIdentificacion)}

        'Protected Friend Shared Function GetTablesPrimaryKeys() As Dictionary(Of String, List(Of String))
        '    Dim tablesPrimaryKeys As New Dictionary(Of String, List(Of String))()
        '    Dim _context As EasyGestDataContext = New EasyGestDataContext()
        '    For Each model As System.Data.Linq.Mapping.MetaTable In _context.Mapping.GetTables()
        '        Dim members As Collections.ObjectModel.ReadOnlyCollection(Of System.Data.Linq.Mapping.MetaDataMember) = model.RowType.IdentityMembers
        '        Dim primaryKeys As New List(Of String)
        '        For Each member As System.Data.Linq.Mapping.MetaDataMember In members
        '            primaryKeys.Add(member.Name)
        '        Next
        '        tablesPrimaryKeys.Add(model.RowType.Name, primaryKeys)
        '    Next
        '    _context.Dispose()
        '    Return tablesPrimaryKeys
        'End Function

        Protected Friend Shared Function IsReadOnlyEntity(entityType As Type) As Boolean
            Return ReadOnlyEntities.Contains(entityType)
        End Function

        Protected Friend Shared Function DataBaseAccesible(cs As String) As Boolean
            If String.IsNullOrWhiteSpace(cs) Then Throw New ArgumentNullException()
            Dim dbExiste As Boolean
            Dim ctx As New EasyGestDataContext(cs)
            dbExiste = ctx.DatabaseExists()
            ctx.Dispose()
            Return dbExiste
        End Function

        Private Sub OnCreated()
            Me.DeferredLoadingEnabled = True
        End Sub

    End Class


End Namespace

