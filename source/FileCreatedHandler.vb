Imports System.IO
Imports System.Threading
Imports System.Threading.Tasks

Public Class FileCreatedHandler

    Public Property Path As String

    Public Property FileNameFilter As String

    Public Property Delay As Integer

    Public Property RunProcess As String

    Public Sub Run()
        Dim watcher As New FileSystemWatcher
        watcher.Filter = FileNameFilter
        watcher.Path = Path
        watcher.InternalBufferSize = 65536
        watcher.IncludeSubdirectories = False

        AddHandler watcher.Created, Sub(sender As Object, e As FileSystemEventArgs)
                                        Task.Factory.StartNew(Sub()
                                                                  If Delay > 0 Then
                                                                      Log.WriteLine(String.Format("File Created: {0} Waiting {1} seconds.", e.FullPath, Delay))
                                                                      Thread.Sleep(Delay * 1000)
                                                                  End If
                                                                  Log.WriteLine(String.Format("File Created: {0} so process: {1} started.", e.FullPath, RunProcess))
                                                                  Process.Start(RunProcess)
                                                              End Sub)
                                    End Sub

        watcher.EnableRaisingEvents = True

        Log.WriteLine(String.Format("File Created has started watching for {0}\{1}", Path, FileNameFilter))

    End Sub

End Class
