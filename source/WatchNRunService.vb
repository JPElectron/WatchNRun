Imports System.ServiceProcess
Imports System.Threading
Imports System.Xml.Serialization
Imports System.IO

Public Class WatchNRun

    'Private _exit As New ManualResetEvent(False)

    Shared Sub Main()
        System.ServiceProcess.ServiceBase.Run(New WatchNRun())
    End Sub

    Protected Overrides Sub OnStart(ByVal args() As String)
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory)

        Dim handlers As FileChangeHandlers
        Dim configSerializer = New XmlSerializer(GetType(FileChangeHandlers))
        Using configStream = File.OpenRead("WatchNRunConfig.xml")
            handlers = configSerializer.Deserialize(configStream)
        End Using

        For Each handler In handlers.FileCreatedHandlers
            handler.Run()
        Next

        For Each handler In handlers.FileModifiedHandlers
            handler.Run()
        Next

        '_exit.WaitOne()
    End Sub

    Protected Overrides Sub OnStop()
        '_exit.Set()
    End Sub

End Class
