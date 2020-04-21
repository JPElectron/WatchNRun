
Public Module Log

    Private _log As New EventLog("WatchNRun")

    Sub New()
        If Not EventLog.SourceExists("WatchNRun") Then
            EventLog.CreateEventSource("WatchNRun", "Application")
        End If

        _log.Source = "WatchNRun"
        _log.Log = "Application"
    End Sub

    Public Sub WriteLine(value As String)
        _log.WriteEntry(String.Format("{0} {1}", DateTime.Now, value))
    End Sub

End Module
