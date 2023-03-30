Imports MySql.Data.MySqlClient
Imports AForge.Video.DirectShow
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports AForge.Video
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar

Public Class AddIntern
    Dim cmd As MySqlCommand
    Dim conn As MySqlConnection
    Dim cnstr As String = "data source =  localhost; user id = root; database = attendancesheet;"
    Dim videoDevices As FilterInfoCollection
    Dim videoSource As VideoCaptureDevice

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ms As New MemoryStream()
        PictureBox2.Image.Save(ms, PictureBox2.Image.RawFormat)
        Dim imageBytes As Byte() = ms.ToArray()
        MsgBox(imageBytes.ToString)
        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim sql As String = "INSERT INTO interns (name,internprofile,hours_to_render,hours_rendered,hours_left) VALUES (@name,@profile,@hrstorender,@hours_rendered,@hours_left)"
            cmd = New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@name", internname.Text)
            cmd.Parameters.AddWithValue("@profile", imageBytes)
            cmd.Parameters.AddWithValue("@hrstorender", hrs.Text)
            cmd.Parameters.AddWithValue("@hours_rendered", 0)
            cmd.Parameters.AddWithValue("@hours_left", hrs.Text)

            Dim i As Integer = cmd.ExecuteNonQuery
            If i > 0 Then
                MsgBox("INTERN REGISTERED", vbInformation, "Query")
                internname.Clear()
                hrs.Clear()
                PictureBox2.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
            Else
                MsgBox("INTERN NOT REGISTERED", vbInformation, "Query")

            End If
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Call Form1.internlists()
        Form1.Show()
        Me.Close()
        If videoSource IsNot Nothing AndAlso videoSource.IsRunning Then
            videoSource.SignalToStop()
            videoSource.WaitForStop()
            videoSource = Nothing
        End If

    End Sub
    Private Sub photo_MouseHover(sender As Object, e As EventArgs) Handles photo.MouseHover
        photo.BackColor = Color.OrangeRed
        photo.ForeColor = Color.White
    End Sub
    Private Sub Button2_MouseHover(sender As Object, e As EventArgs) Handles Button2.MouseHover
        Button2.BackColor = Color.Teal
        Button2.ForeColor = Color.White
    End Sub
    Private Sub Button1_MouseHover(sender As Object, e As EventArgs) Handles Button1.MouseHover
        Button1.BackColor = Color.Teal
        Button1.ForeColor = Color.White
    End Sub

    Private Sub photo_MouseLeave(sender As Object, e As EventArgs) Handles photo.MouseLeave
        photo.BackColor = Color.Salmon
        photo.ForeColor = Color.Black
    End Sub
    Private Sub Button2_MouseLeave(sender As Object, e As EventArgs) Handles Button2.MouseLeave
        Button2.BackColor = Color.CadetBlue
        Button2.ForeColor = Color.Black
    End Sub
    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.BackColor = Color.CadetBlue
        Button1.ForeColor = Color.Black
    End Sub
    Private Sub AddIntern_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        videoDevices = New FilterInfoCollection(FilterCategory.VideoInputDevice)
        PictureBox2.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
        For Each device As FilterInfo In videoDevices
            ComboBox1.Items.Add(device.Name)
        Next
        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 0
        End If
        videoSource = New VideoCaptureDevice(videoDevices(ComboBox1.SelectedIndex).MonikerString)
        AddHandler videoSource.NewFrame, AddressOf video_NewFrame
        videoSource.Start()
        Timer1.Interval = 1000 ' Set the timer interval to 1 second
        Timer1.Start() ' Start the timer
    End Sub
    Private Sub video_NewFrame(sender As Object, eventArgs As NewFrameEventArgs)
        PictureBox1.Image = DirectCast(eventArgs.Frame.Clone(), Bitmap)
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs)
        videoSource = New VideoCaptureDevice(videoDevices(ComboBox1.SelectedIndex).MonikerString)
        AddHandler videoSource.NewFrame, AddressOf video_NewFrame
        videoSource.Start()
    End Sub



    Private Sub photo_Click(sender As Object, e As EventArgs) Handles photo.Click
        Dim bmp As Bitmap = CType(PictureBox1.Image, Bitmap)
        Dim filePath As String = "Profiles/" + internname.Text + ".png"

        Try
            bmp.Save("Profiles/" + internname.Text + ".png", System.Drawing.Imaging.ImageFormat.Png)
            Dim image As Image = Image.FromFile("Profiles/" + internname.Text + ".png")
            PictureBox2.Image = image
        Catch ex As Exception
            PictureBox2.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage

        End Try


    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label5.Text = DateTime.Now.ToString("hh:mm:ss tt")
    End Sub
End Class