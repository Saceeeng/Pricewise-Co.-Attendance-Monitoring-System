Imports System.Diagnostics.Eventing
Imports System.Globalization
Imports System.IO

Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient
Imports AForge.Video.DirectShow
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports AForge.Video
Public Class Form1
    Dim cmd As MySqlCommand
    Dim conn As MySqlConnection
    Dim cnstr As String = "data source =  localhost; user id = root; database = attendancesheet;"
    Dim reader As MySqlDataReader
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        internlist.SelectedIndex = -1
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        PictureBox1.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
        AddIntern.Show()
    End Sub
    Public Sub internlists()
        conn = New MySqlConnection(cnstr)
        conn.Open()
        Dim sql As String = "SELECT name from interns ORDER BY name ASC"
        cmd = New MySqlCommand(sql, conn)
        reader = cmd.ExecuteReader
        Dim count As Integer = 0
        While reader.Read
            internlist.Items.Add(reader.GetString("name"))
        End While
        conn.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
        Catch ex As Exception
            MsgBox("No Database in your localhost")
            conn.Close()
            Dim connectionString As String = "data source =  localhost; user id = root;"
            Dim conn1 As New MySqlConnection(connectionString)

            Dim cmd1 As New MySqlCommand()
            cmd1.Connection = conn1

            Try
                conn1.Open()
                cmd1.CommandText = "CREATE DATABASE attendancesheet;"
                cmd1.ExecuteNonQuery()
                MsgBox("Database created successfully!")
                conn1.Close()
            Catch ex11 As Exception
                MsgBox("Database creation failed: " & ex11.Message)
            End Try


            Dim conn2 As MySqlConnection
            conn2 = New MySqlConnection(cnstr)

            Dim script As String = File.ReadAllText("database/attendancesheet.sql")
            Dim cmd As New MySqlCommand(script, conn2)

            Try
                conn2.Open()
                cmd.ExecuteNonQuery()
                MsgBox("Database Created!")
            Catch exs As Exception
                MsgBox("SQL script execution failed: " & exs.Message)
            End Try

            conn2.Close()

        End Try
        conn.Close()
        Timer1.Interval = 1000 ' Set the timer interval to 1 second
        Timer1.Start() ' Start the timer
        Call internlists()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Label3.Text = DateTime.Now.ToString("hh:mm:ss tt")
    End Sub
    Dim internnames As String
    Dim internid As String
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim cmd As New MySqlCommand()
            cmd.Connection = conn
            cmd.CommandText = "SELECT COUNT(*) FROM attendance WHERE DATE(timein) = Date(now()) AND internname = '" + internlist.SelectedItem + "'"
            Dim count As Integer = CInt(cmd.ExecuteScalar())
            If count > 0 Then
                MessageBox.Show("ALREADY TIMED IN")
                Exit Sub
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim sql As String = "SELECT id,name from interns where name  ='" + internlist.SelectedItem.ToString + "' "
            cmd = New MySqlCommand(sql, conn)
            reader = cmd.ExecuteReader
            Dim count As Integer = 0
            While reader.Read
                internnames = reader.GetString("name")
                internid = reader.GetString("id")
            End While
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim sql As String = "INSERT INTO attendance (internid,internname,timein) VALUES (@internid,@internname,@timein)"
            cmd = New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@internid", internid)
            cmd.Parameters.AddWithValue("@internname", internnames)
            cmd.Parameters.AddWithValue("@timein", DateTime.Parse(Label3.Text, CultureInfo.InvariantCulture))

            Dim i As Integer = cmd.ExecuteNonQuery
            If i > 0 Then
                MsgBox("TIME IN REGISTERED", vbInformation, "Query")
            Else
                MsgBox("TIME IN NOT REGISTERED", vbInformation, "Query")
            End If
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        PictureBox1.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
        internlist.SelectedIndex = -1
    End Sub

    Private Sub internlist_SelectedIndexChanged(sender As Object, e As EventArgs) Handles internlist.SelectedIndexChanged
        conn = New MySqlConnection(cnstr)
        conn.Open()
        Dim sql As String = "SELECT * FROM interns WHERE name = '" + internlist.SelectedItem + "' "
        cmd = New MySqlCommand(sql, conn)
        reader = cmd.ExecuteReader
        Dim count As Integer = 0
        If reader.Read Then
            TextBox1.Text = reader.GetString("name")
            TextBox2.Text = reader.GetString("hours_to_render")
            TextBox3.Text = reader.GetString("hours_rendered")
            TextBox4.Text = reader.GetString("hours_left")
            Try
                Dim imageBytes As Byte() = DirectCast(reader("internprofile"), Byte())
                Dim stream As New MemoryStream(imageBytes) ' assume that imageBytes is a byte array containing the image data
                Dim img As Image = Image.FromStream(stream)
                PictureBox1.Image = img
            Catch ex As Exception
                PictureBox1.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
            End Try



        End If
        conn.Close()

    End Sub
    Dim hrsrendered As Integer
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim cmd As New MySqlCommand()
            cmd.Connection = conn
            cmd.CommandText = "SELECT COUNT(*) FROM attendance WHERE DATE(timeout) = Date(now()) AND internname = '" + internlist.SelectedItem + "'"
            Dim count As Integer = CInt(cmd.ExecuteScalar())
            If count > 0 Then
                MessageBox.Show("ALREADY TIMED OUT")
                Exit Sub
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()
            Dim sql As String = "SELECT id,name from interns where name  ='" + internlist.SelectedItem.ToString + "' "
            cmd = New MySqlCommand(sql, conn)
            reader = cmd.ExecuteReader
            Dim count As Integer = 0
            While reader.Read
                internnames = reader.GetString("name")
                internid = reader.GetString("id")
            End While
            conn.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Try
            conn = New MySqlConnection(cnstr)
            conn.Open()         '
            Dim sql As String = "UPDATE attendance SET timeout = @timeout WHERE internname = '" + internlist.SelectedItem + "' AND DATE(timein) = Date(now())"
            cmd = New MySqlCommand(sql, conn)

            cmd.Parameters.AddWithValue("@timeout", DateTime.Parse(Label3.Text, CultureInfo.InvariantCulture))

            Dim i As Integer = cmd.ExecuteNonQuery
            If i > 0 Then
                MsgBox("TIME OUT REGISTERED", vbInformation, "Query")
                conn.Close()

                Try
                    conn = New MySqlConnection(cnstr)
                    conn.Open()
                    Dim sql1 As String = "SELECT attendanceid, internname,TIMESTAMPDIFF(HOUR, timein, timeout) AS hour_diff FROM attendance WHERE internname = '" + internlist.SelectedItem + "' AND DATE(timein) = Date(now());"
                    cmd = New MySqlCommand(sql1, conn)
                    reader = cmd.ExecuteReader
                    Dim count As Integer = 0
                    While reader.Read
                        hrsrendered = reader.GetString("hour_diff")
                        MsgBox("Hours Rendered Today: " + hrsrendered.ToString)
                    End While
                    conn.Close()
                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
                Try
                    conn = New MySqlConnection(cnstr)
                    conn.Open()
                    Dim sql2 As String = "UPDATE attendance SET hours_rendered = @hours_rendered WHERE internname = '" + internlist.SelectedItem + "' AND DATE(timein) = Date(now()) "
                    cmd = New MySqlCommand(sql2, conn)
                    cmd.Parameters.AddWithValue("@hours_rendered", hrsrendered)
                    Dim i2 As Integer = cmd.ExecuteNonQuery
                    If i2 > 0 Then
                        conn.Close()
                        Try
                            conn = New MySqlConnection(cnstr)
                            conn.Open()             'UPDATE interns SET hours_rendered = hours_rendered + 2 , hours_left = hours_left - 2 WHERE name = 'John Alsace Mondonedo';
                            Dim sql21 As String = "UPDATE interns SET hours_rendered = hours_rendered+@hours_rendered ,hours_left = hours_left - @hours_rendered  WHERE name = '" + internlist.SelectedItem + "' "
                            cmd = New MySqlCommand(sql21, conn)
                            cmd.Parameters.AddWithValue("@hours_rendered", hrsrendered)
                            Dim i21 As Integer = cmd.ExecuteNonQuery
                            If i2 > 0 Then

                            End If
                            conn.Close()
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                    End If

                Catch ex As Exception
                    MsgBox(ex.Message)
                End Try
            Else
                MsgBox("TIME OUT NOT REGISTERED: BE SURE TO TIMED IN FIRST", vbInformation, "Query")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        TextBox1.Clear()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        PictureBox1.Image = Pricewise_Co.Attendance_Monitoring_System.My.Resources.noimage
        internlist.SelectedIndex = -1
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Button2_MouseHover(sender As Object, e As EventArgs) Handles Button2.MouseHover
        Button2.BackColor = Color.Teal
        Button2.ForeColor = Color.White
    End Sub
    Private Sub Button3_MouseHover(sender As Object, e As EventArgs) Handles Button3.MouseHover
        Button3.BackColor = Color.Teal
        Button3.ForeColor = Color.White
    End Sub
    Private Sub Button1_MouseHover(sender As Object, e As EventArgs) Handles Button1.MouseHover
        Button1.BackColor = Color.OrangeRed
        Button1.ForeColor = Color.White
    End Sub

    Private Sub Button1_MouseLeave(sender As Object, e As EventArgs) Handles Button1.MouseLeave
        Button1.BackColor = Color.Salmon
        Button1.ForeColor = Color.Black
    End Sub
    Private Sub Button2_MouseLeave(sender As Object, e As EventArgs) Handles Button2.MouseLeave
        Button2.BackColor = Color.CadetBlue
        Button2.ForeColor = Color.Black
    End Sub
    Private Sub Button3_MouseLeave(sender As Object, e As EventArgs) Handles Button3.MouseLeave
        Button3.BackColor = Color.CadetBlue
        Button3.ForeColor = Color.Black
    End Sub


End Class
