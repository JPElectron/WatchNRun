Imports System.IO
Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports System.Collections.Concurrent

Public Class FileModifiedHandler

    Public Property Path As String

    Public Property FileNameFilter As String

    Public Property PatternToMatch As String

    Public Property SkipEmptyLines As Boolean

    Public Property Delay As Integer

    Public Property RunProcess As String

    'Private _lastModified As DateTime

    Private _lastReadSizes As New Dictionary(Of String, Long)

    Private Shared _readLock As New Object

    Public Sub Run()
        Dim watcher As New FileSystemWatcher
        watcher.Filter = FileNameFilter
        watcher.Path = Path
        watcher.InternalBufferSize = 65536
        watcher.IncludeSubdirectories = False
        watcher.NotifyFilter = NotifyFilters.LastWrite

        AddHandler watcher.Changed, Sub(sender As Object, e As FileSystemEventArgs)
                                        Log.WriteLine(String.Format("File Modified: {0} Raised.", e.FullPath))
                                        'Dim lastModified = _lastModified
                                        '_lastModified = DateTime.Now
                                        'A single write can trigger multiple changed events, so only respond if a second has passed since the last change
                                        'If _lastModified - lastModified > New TimeSpan(0, 0, 1) Then

                                        Task.Factory.StartNew(Sub()
                                                                  Log.WriteLine(String.Format("File Modified: {0} Attempting to read.", e.FullPath))

                                                                  SyncLock _readLock

                                                                      Dim retry = True

                                                                      While retry
                                                                          Try
                                                                              Using changedFile = File.OpenRead(e.FullPath)
                                                                                  Using fileReader = New StreamReader(changedFile)
                                                                                      Dim lastReadSize = 0
                                                                                      Dim isFirstRead = False
                                                                                      If Not _lastReadSizes.TryGetValue(e.FullPath, lastReadSize) Then
                                                                                          _lastReadSizes.Add(e.FullPath, 0)
                                                                                          isFirstRead = True
                                                                                      End If
                                                                                      If lastReadSize = changedFile.Length Then
                                                                                          Log.WriteLine(String.Format("File Modified: {0} File contents not modified since last read.", e.FullPath))
                                                                                          Exit While
                                                                                      End If
                                                                                      If lastReadSize > changedFile.Length Then
                                                                                          'File contents have been deleted
                                                                                          lastReadSize = 0
                                                                                          isFirstRead = True
                                                                                      End If
                                                                                      changedFile.Seek(lastReadSize, SeekOrigin.Begin)
                                                                                      Dim lastLines = fileReader.ReadToEnd().Split(Environment.NewLine).Select(Function(l) (l.Trim())).Where(Function(l) (Not SkipEmptyLines OrElse Not String.IsNullOrWhiteSpace(l)))
                                                                                      If isFirstRead Then
                                                                                          lastLines = lastLines.Skip(lastLines.Count() - 1)
                                                                                      End If
                                                                                      _lastReadSizes(e.FullPath) = changedFile.Length
                                                                                      For Each lastLine In lastLines
                                                                                          If Regex.IsMatch(lastLine, PatternToMatch) Then
                                                                                              Log.WriteLine(String.Format("File Modified: {0} Matched pattern '{1}' in last line: {2}", e.FullPath, PatternToMatch, lastLine))
                                                                                              If Delay > 0 Then
                                                                                                  Log.WriteLine(String.Format("File Modified: {0} Waiting {1} seconds.", e.FullPath, Delay))
                                                                                                  Thread.Sleep(Delay * 1000)
                                                                                              End If
                                                                                              Log.WriteLine(String.Format("File Modified: {0} so process: {1} started.", e.FullPath, RunProcess))
                                                                                              Process.Start(RunProcess)
                                                                                          Else
                                                                                              Log.WriteLine(String.Format("File Modified: {0} Pattern '{1}' not matched in last line: {2}", e.FullPath, PatternToMatch, lastLine))
                                                                                          End If
                                                                                      Next

                                                                                      retry = False
                                                                                  End Using
                                                                              End Using
                                                                          Catch ex As Exception
                                                                              Log.WriteLine(String.Format("File Modified: {0} Could not be read, retrying...", e.FullPath))
                                                                              Thread.Sleep(Delay * 500)
                                                                              retry = True
                                                                          End Try
                                                                      End While

                                                                  End SyncLock
                                                              End Sub)
                                        'End If
                                    End Sub

        watcher.EnableRaisingEvents = True

        Log.WriteLine(String.Format("File Modified has started watching for {0}\{1}", Path, FileNameFilter))
    End Sub
End Class
