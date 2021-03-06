﻿Imports Microsoft.Win32
Imports System.Net.NetworkInformation
Imports System.Net

Namespace Util
    Public Class Comunes

        Public Shared Function Nothing2EmptyText(ByVal obj As Object) As Object
            If obj Is Nothing Then
                Return String.Empty
            Else
                Return obj
            End If
        End Function

        Public Shared Function Nothing2Zero(ByVal obj As Object) As Object
            If obj Is Nothing Then
                Return 0
            Else
                Return obj
            End If
        End Function

        Protected Friend Shared Sub InterpretarFormulaFidelizacion(ByVal formula As String, ByRef base As Single, ByRef beneficio As Single, ByRef basePunto As Single, ByRef equivalente As Single)
            Dim partes1, partes2a, partes2b As String()
            Dim _separador1 As Char = ";"c, _separador2 As Char = "/"c
            partes1 = formula.Split(_separador1)
            partes2a = partes1(0).Split(_separador2)
            partes2b = partes1(1).Split(_separador2)
            base = Convert.ToSingle(partes2a(0), My.Application.Culture.NumberFormat)
            beneficio = Convert.ToSingle(partes2a(1), My.Application.Culture.NumberFormat)
            basePunto = Convert.ToSingle(partes2b(0), My.Application.Culture.NumberFormat)
            equivalente = Convert.ToSingle(partes2b(1), My.Application.Culture.NumberFormat)
        End Sub


        Public Shared Function EjecutarComandoBD(ByVal comando As String, ByVal tipo As System.Data.CommandType, Optional ByVal params() As SqlClient.SqlParameter = Nothing) As Integer
            Dim _con As New System.Data.SqlClient.SqlConnection(Data.BaseDataContext.CadenaConexion)
            Dim _com As New System.Data.SqlClient.SqlCommand(comando, _con)
            Dim returnvalue As Integer = -1
            _com.CommandType = tipo
            If params IsNot Nothing AndAlso params.Length > 0 Then _
                _com.Parameters.AddRange(params)
            Try
                If (_con.State And ConnectionState.Closed) = ConnectionState.Closed Then
                    _con.Open()
                End If
                returnvalue = _com.ExecuteNonQuery
            Catch ex As Exception
                Throw ex
            Finally
                If (_con.State And ConnectionState.Open) = ConnectionState.Open Then
                    _con.Close()
                End If
                _con = Nothing
                _com = Nothing
            End Try
            Return returnvalue
        End Function

        Public Shared Function Encriptar(ByVal dato As String) As String
            Dim hashed As Byte() = Nothing
            Dim hash As System.Security.Cryptography.MD5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create()
            Dim md5Provider As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim encode As New System.Text.UTF8Encoding()
            If dato = Nothing Then dato = ""
            hashed = md5Provider.ComputeHash(encode.GetBytes(dato))
            Dim hexString As New System.Text.StringBuilder(hashed.Length)
            Dim i As Integer
            For i = 0 To hashed.Length - 1
                hexString.Append(hashed(i).ToString("X2"))
            Next
            Return hexString.ToString
        End Function

        Public Shared Sub SerializarToFile(ByVal obj As Object, ByVal filename As String)
            Try
                Dim serializer As System.Xml.Serialization.XmlSerializer
                Dim smString As IO.StreamWriter
                If Not IsNothing(obj) Then
                    serializer = New System.Xml.Serialization.XmlSerializer(obj.GetType)
                    smString = New IO.StreamWriter(filename)
                    serializer.Serialize(smString, obj)
                    serializer = Nothing
                    smString.Close()
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Shared Function DeserializarFromFile(Of T)(ByVal filename As String) As T
            Dim serializer As System.Xml.Serialization.XmlSerializer
            Dim xmlReader As Xml.XmlTextReader
            Dim returnObjects As T = Nothing
            Try
                xmlReader = New Xml.XmlTextReader(filename)
                serializer = New System.Xml.Serialization.XmlSerializer(GetType(T))
                returnObjects = CType(serializer.Deserialize(xmlReader), T)
            Catch ex As Exception
                Throw ex
            End Try
            Return returnObjects
        End Function

        Public Shared Function Serializar(ByVal obj As Object) As String
            Try
                Dim _xmlSerializado As String = ""
                Dim serializer As System.Xml.Serialization.XmlSerializer
                Dim smString As IO.StringWriter
                If Not IsNothing(obj) Then
                    serializer = New System.Xml.Serialization.XmlSerializer(obj.GetType)
                    smString = New IO.StringWriter()
                    serializer.Serialize(smString, obj)
                    _xmlSerializado = smString.ToString
                    serializer = Nothing
                    smString.Close()
                End If
                Return _xmlSerializado
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Shared Function Deserializar(Of T)(ByVal xml As String) As T
            Dim serializer As System.Xml.Serialization.XmlSerializer
            Dim smString As System.IO.StringReader
            Dim tipo As Type = GetType(T)
            Dim _returnObjects As T = Nothing
            If String.IsNullOrEmpty(xml) Then Return Nothing
            If tipo Is Nothing Then Return Nothing

            serializer = New System.Xml.Serialization.XmlSerializer(tipo)
            smString = New IO.StringReader(xml)
            Try
                _returnObjects = CType(serializer.Deserialize(smString), T)
            Catch ex As Exception
                _returnObjects = Nothing
            End Try

            smString.Close()
            serializer = Nothing
            Return _returnObjects
        End Function


        Public Shared Function GetRegistryValue(ByVal key As String) As String
            Dim registro As RegistryKey
            Dim value As Object

            registro = CreateRegistryKeyPath()
            value = registro.GetValue(key)
            If IsNothing(value) Then
                Return Nothing
            Else
                Return value.ToString()
            End If
        End Function

        Public Shared Sub SetRegistryValue(ByVal key As String, ByVal value As String)
            Dim registro As RegistryKey

            registro = CreateRegistryKeyPath()
            registro.SetValue(key, value, RegistryValueKind.String)
        End Sub

        Private Shared Function CreateRegistryKeyPath() As RegistryKey
            Dim registro As RegistryKey = Nothing
            Dim rutaClave As String = "Software\ChenSoft\EasyGest"

            registro = Registry.LocalMachine.OpenSubKey(rutaClave, True)
            If registro Is Nothing Then
                registro = Registry.LocalMachine.CreateSubKey(rutaClave, RegistryKeyPermissionCheck.ReadWriteSubTree)
            End If

            Return registro
        End Function

        Public Shared Sub GetInformacionRed(ByRef mac As String, ByRef ip As String)

            Dim nic As NetworkInterface
            Dim direccion As PhysicalAddress
            Dim propiedades As IPInterfaceProperties
            Dim bytes As Byte()

            mac = "00:00:00:00:00:00"
            ip = "0.0.0.0"
            Try
                nic = GetNicValido()
                If nic IsNot Nothing Then
                    direccion = nic.GetPhysicalAddress()
                    propiedades = nic.GetIPProperties()
                    bytes = direccion.GetAddressBytes
                    For m As Integer = 0 To bytes.Length - 1
                        mac = mac & bytes(m).ToString("X2")
                        If m <> bytes.Length - 1 Then
                            mac = mac & ":"
                        End If
                    Next

                    Dim uniCast As UnicastIPAddressInformationCollection = propiedades.UnicastAddresses
                    ip = "0.0.0.0"
                    If uniCast IsNot Nothing Then
                        For Each uni As UnicastIPAddressInformation In uniCast
                            If uni.Address.AddressFamily = Sockets.AddressFamily.InterNetwork Then
                                ip = uni.Address.ToString
                            End If
                        Next
                    End If
                Else
                    mac = "00:00:00:00:00:00"
                    ip = "0.0.0.0"
                End If
            Catch ex As Exception
                mac = "00:00:00:00:00:00"
                ip = "0.0.0.0"
            End Try

        End Sub

        Private Shared Function GetNicValido() As NetworkInterface
            Dim nics() As NetworkInterface = Nothing
            Try
                'computerProperties = IPGlobalProperties.GetIPGlobalProperties
                If NetworkInterface.GetIsNetworkAvailable Then
                    nics = NetworkInterface.GetAllNetworkInterfaces
                End If
                If nics IsNot Nothing AndAlso nics.Length > 0 Then
                    For Each nic In nics
                        If nic.NetworkInterfaceType = NetworkInterfaceType.Ethernet Then
                            Return nic
                        End If
                    Next
                    For Each nic In nics
                        If nic.NetworkInterfaceType = NetworkInterfaceType.Wireless80211 Then
                            Return nic
                        End If
                    Next
                    For Each nic In nics
                        If nic.NetworkInterfaceType = NetworkInterfaceType.GigabitEthernet Then
                            Return nic
                        End If
                    Next
                    Return nics.First()
                End If
                Return Nothing
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

    End Class


End Namespace

