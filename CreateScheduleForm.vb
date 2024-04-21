﻿Imports System.IO
Imports MySql.Data.MySqlClient

Public Class CreateScheduleForm
    Dim timeDuration As String
    Dim dset As New DataSet

    Private Sub CreateScheduleForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartTime.Format = DateTimePickerFormat.Time
        StartTime.CustomFormat = "hh:mm"

        EndTIme.Format = DateTimePickerFormat.Time
        EndTIme.CustomFormat = "hh:mm"

        Try
            getSchedules()
            GetInstructor()
            GetSection()
            GetSubject()
            GetRoom()
        Catch ex As Exception
            MsgBox(ex.ToString())
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            If String.IsNullOrEmpty(cb_building.SelectedItem) OrElse
                String.IsNullOrEmpty(cb_section.SelectedItem) OrElse
                String.IsNullOrEmpty(cb_subject.SelectedItem) OrElse
                String.IsNullOrEmpty(cb_day.SelectedItem) OrElse
                String.IsNullOrEmpty(cb_room.SelectedItem) Then
                MessageBox.Show("Please fill up all the fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' Check if start time and end time are the same
            If StartTime.Value.ToString("hh:mm") = EndTIme.Value.ToString("hh:mm") Then
                MessageBox.Show("Same Start and End time is not applicable", "Invalid Time", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Check if the schedule already exists
            If ScheduleExists(cb_day.SelectedItem, cb_room.SelectedItem, StartTime.Value.ToString("HH:mm"), EndTIme.Value.ToString("HH:mm")) Then
                MessageBox.Show("Schedule is not available. Another schedule already exists for the same day and room.", "Schedule Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Insert the schedule into the database
            cmd.Connection = con
            cmd.CommandText = "INSERT INTO schedules(`InstructorName`, `Section`, `Subject`, `StartTime`, `EndTime`, `Day`, `RoomNumber`, `Building`) VALUES (@InstructorName, @Section, @Subject, @StartTime, @EndTime, @Day, @RoomNumber, @Building)"

            ' Clear the parameters collection before adding new parameters
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@InstructorName", cb_instructor.SelectedItem)
            cmd.Parameters.AddWithValue("@Section", cb_section.SelectedItem)
            cmd.Parameters.AddWithValue("@Subject", cb_subject.SelectedItem)
            cmd.Parameters.AddWithValue("@StartTime", StartTime.Value)
            cmd.Parameters.AddWithValue("@EndTime", EndTIme.Value)
            cmd.Parameters.AddWithValue("@Day", cb_day.SelectedItem)
            cmd.Parameters.AddWithValue("@RoomNumber", cb_room.SelectedItem)
            cmd.Parameters.AddWithValue("@Building", cb_building.SelectedItem)

            DBCon()
            cmd.ExecuteNonQuery()
            getSchedules()
            con.Close()

            cb_instructor.Text = ""
            cb_section.Text = ""
            cb_subject.Text = ""
            cb_day.Text = ""
            cb_building.Text = ""
            cb_room.Text = ""
        Catch ex As Exception
            MsgBox(ex.ToString)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub

    Private Sub getSchedules()
        Try
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM schedules ORDER BY ScheduleID DESC"


            table.Clear()


            dataReader.SelectCommand = cmd
            dataReader.Fill(table)

            ' Add new columns only if they don't already exist
            If Not table.Columns.Contains("Start Time") Then
                table.Columns.Add("Start Time", GetType(String))
            End If

            If Not table.Columns.Contains("End Time") Then
                table.Columns.Add("End Time", GetType(String))
            End If


            For Each row As DataRow In table.Rows
                Dim startTime As TimeSpan = DirectCast(row("StartTime"), TimeSpan)
                Dim endTime As TimeSpan = DirectCast(row("EndTime"), TimeSpan)

                Dim startDateTime As DateTime = DateTime.Today.Add(startTime)
                Dim endDateTime As DateTime = DateTime.Today.Add(endTime)


                row("Start Time") = startDateTime.ToString("hh:mm tt")
                row("End Time") = endDateTime.ToString("hh:mm tt")
            Next

            dgvSchedule.DataSource = table

            ' Optionally, hide the ScheduleID column
            dgvSchedule.Columns("ScheduleID").Visible = False
            dgvSchedule.Columns("StartTime").Visible = False
            dgvSchedule.Columns("EndTime").Visible = False

            ' Set auto-sizing and wrap mode for better display
            dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
            dgvSchedule.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgvSchedule.DefaultCellStyle.WrapMode = DataGridViewTriState.True
        Catch ex As Exception
            MsgBox(ex.ToString())
        Finally
            con.Close()
        End Try
    End Sub

    Private Function ScheduleExists(day As String, room As String, StartTime As String, EndTime As String) As Boolean
        Dim exists As Boolean = False

        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            cmd.Connection = con
            cmd.CommandText = "SELECT * FROM schedules WHERE RoomNumber = @RoomNumber AND Day = @Day AND ((StartTime >= @starttime AND StartTime < @endtime) OR (EndTime > @starttime AND EndTime <= @endtime) OR (StartTime <= @starttime AND EndTime >= @endtime))"

            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@RoomNumber", room)
            cmd.Parameters.AddWithValue("@Day", day)
            cmd.Parameters.AddWithValue("@starttime", StartTime)
            cmd.Parameters.AddWithValue("@endtime", EndTime)

            Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
            If count > 0 Then
                exists = True
            End If
        Catch ex As Exception
            MsgBox(ex.ToString())
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try

        Return exists
    End Function

    Private Sub txt_search_TextChanged(sender As Object, e As EventArgs) Handles txt_search.TextChanged

        Dim searchQuery As String = txt_search.Text.Trim()

        ' Filter the data in the table based on the search query
        If Not String.IsNullOrEmpty(searchQuery) Then
            Dim filteredData As New DataTable()
            For Each column As DataColumn In table.Columns
                filteredData.Columns.Add(column.ColumnName, column.DataType)
            Next

            For Each row As DataRow In table.Rows
                For Each column As DataColumn In table.Columns
                    ' Check if the cell value contains the search query
                    If row(column.ColumnName).ToString().ToLower().Contains(searchQuery.ToLower()) Then
                        filteredData.Rows.Add(row.ItemArray)
                        Exit For ' No need to check other columns once a match is found
                    End If
                Next
            Next


            dgvSchedule.DataSource = filteredData
        Else

            dgvSchedule.DataSource = table
        End If
    End Sub

    Private Sub GetInstructor()
        Try
            Dim newtable As New DataTable()
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "SELECT CONCAT(Firstname, ' ', MiddleName, ' ', Surname) AS FullName FROM instructor"
            dataReader.SelectCommand = cmd
            dataReader.Fill(newtable)

            ' Clear existing items in the ComboBox
            cb_instructor.Items.Clear()

            ' Add each FullName value to the ComboBox
            For Each row As DataRow In newtable.Rows
                cb_instructor.Items.Add(row("FullName").ToString())
            Next
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub GetSection()
        Try
            Dim newtable As New DataTable()
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "Select CONCAT(Section_Program, '-', Year, '', Section_Code) AS Section from sections"
            dataReader.SelectCommand = cmd
            dataReader.Fill(newtable)

            ' Clear existing items in the ComboBox
            cb_section.Items.Clear()

            ' Add each FullName value to the ComboBox
            For Each row As DataRow In newtable.Rows
                cb_section.Items.Add(row("Section").ToString())
            Next
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub GetSubject()
        Try
            Dim newtable As New DataTable()
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "Select CONCAT(subject_name, ' ', subject_code) AS Subject from subjects"
            dataReader.SelectCommand = cmd
            dataReader.Fill(newtable)

            ' Clear existing items in the ComboBox
            cb_subject.Items.Clear()

            ' Add each FullName value to the ComboBox
            For Each row As DataRow In newtable.Rows
                cb_subject.Items.Add(row("Subject").ToString())
            Next
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub GetRoom()
        Try
            Dim newtable As New DataTable()
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "Select room_number from rooms"
            dataReader.SelectCommand = cmd
            dataReader.Fill(newtable)

            ' Clear existing items in the ComboBox
            cb_room.Items.Clear()

            ' Add each FullName value to the ComboBox
            For Each row As DataRow In newtable.Rows
                cb_room.Items.Add(row("room_number").ToString())
            Next
        Catch ex As Exception
            MsgBox(ex.ToString())
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dashboard.Show()
        Me.Hide()
    End Sub
End Class
