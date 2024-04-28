﻿Imports System.Collections.Specialized.BitVector32
Imports System.Globalization
Imports Google.Protobuf.WellKnownTypes

Public Class roomSchedule
    Dim room As String
    Private Sub roomSchedule_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DBCon()
        room = viewRooms.ClickRoomNumber
        Try

            Label1.Text = $"ROOM :{room}"
            cmd.Connection = con
            cmd.CommandText = "SELECT InstructorName, Section, Subject, TIME_FORMAT(StartTime, '%h:%i %p') AS FormattedStart, TIME_FORMAT(EndTime, '%h:%i %p') AS FormattedEnd, Day, RoomNumber FROM schedules WHERE RoomNumber = @roomnumber ORDER BY DAYOFWEEK(day) ASC   ;
"
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@roomnumber", room)


            Dim data As New DataTable()
            dataReader.SelectCommand = cmd
            dataReader.Fill(data)

            printingdgv.DataSource = data


            dgvRoomSched.Columns.Clear()

            Dim daysOfWeek As String() = {"MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY"}

            For Each day As String In daysOfWeek
                dgvRoomSched.Columns.Add(day, day)
            Next


            For Each row As DataRow In data.Rows


                Dim TimeDuration As String = row("FormattedStart") & " - " & row("FormattedEnd")
                Dim day As String = row("Day").ToString().Trim()
                Dim section As String = row("Section").ToString().Trim()
                Dim instructor As String = row("InstructorName").ToString().Trim()
                Dim Subject As String = row("Subject").ToString().Trim()

                Dim columnIndex As Integer = Array.IndexOf(daysOfWeek, day)


                If columnIndex <> -1 AndAlso Not String.IsNullOrEmpty(section) Then

                    Dim rowIndex As Integer = dgvRoomSched.Rows.Add()
                    Dim CellValue As String = $"Time: {TimeDuration}" & vbCrLf & $"Instructor: {instructor}" & vbCrLf & $"Section: {section}" & vbCrLf & $"Subject: {Subject}"



                    dgvRoomSched.Rows(rowIndex).Cells(columnIndex).Value = CellValue
                    dgvRoomSched.Rows(rowIndex).Cells(columnIndex).Style.BackColor = Color.Green
                End If
            Next

            ' Set DataGridView properties
            dgvRoomSched.AllowUserToAddRows = False
            dgvRoomSched.ReadOnly = True

            dgvRoomSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader
            dgvRoomSched.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            dgvRoomSched.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            dgvRoomSched.DefaultCellStyle.WrapMode = DataGridViewTriState.True
            dgvRoomSched.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            dgvRoomSched.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)



        Catch ex As Exception
            MsgBox("Error: " & ex.ToString())
        Finally
            con.Close()
        End Try
    End Sub

    Private Sub backbtn_Click(sender As Object, e As EventArgs) Handles backbtn.Click

        Me.Close()
        viewRooms.Show()
    End Sub




    Private rowIndexToPrint As Integer = 0 ' Track the index of the next row to print
    Private isNewPage As Boolean = True

    Private Sub printer_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles printer.PrintPage
        Dim StrFormat As New StringFormat()
        StrFormat.Alignment = StringAlignment.Center

        Dim image1 As Image = Image.FromFile("C:\Users\ravem\OneDrive\Desktop\ICS_SCHD\Resources\cmdtransparent.png")
        e.Graphics.DrawImage(image1, 60, 50, 100, 100)

        Dim Image2 As Image = Image.FromFile("C:\Users\ravem\OneDrive\Desktop\ICS_SCHD\Resources\icstransparent.png")
        e.Graphics.DrawImage(Image2, 930, 35, 120, 120)




        e.Graphics.DrawString("Colegio De Montalban", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 80), StrFormat)
        e.Graphics.DrawString("ICS Schedules", New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, New PointF(570, 100), StrFormat)

        e.Graphics.DrawString($"Room:", New Font("Calibri", 16, FontStyle.Bold), Brushes.Black, New PointF(50, 200), StrFormat)
        e.Graphics.DrawString("___________________________________________________________________", New Font("Calibri", 10, FontStyle.Regular), Brushes.Black, New PointF(320, 210), StrFormat)
        e.Graphics.DrawString(room, New Font("Calibri", 16, FontStyle.Bold), Brushes.Black, New PointF(120, 203), StrFormat)

        Dim Format As New StringFormat(StringFormatFlags.LineLimit)
        Format.LineAlignment = StringAlignment.Center
        Format.Trimming = StringTrimming.EllipsisCharacter
        Format.Alignment = StringAlignment.Center


        Dim y As Integer = 230
        Dim x As Integer = 0
        Dim h As Integer = 0
        Dim recta As Rectangle
        Dim row As DataGridViewRow

        If isNewPage Then
            row = printingdgv.Rows(rowIndexToPrint)
            x = 20
            'Print Header Row
            For Each cell As DataGridViewCell In row.Cells
                If cell.Visible Then

                    printingdgv.Columns(cell.ColumnIndex).Width = 152
                    recta = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)
                    e.Graphics.FillRectangle(Brushes.LightYellow, recta)
                    e.Graphics.DrawRectangle(Pens.Black, recta)

                    Dim headerText As String = ""
                    Select Case cell.ColumnIndex
                        Case 0
                            headerText = "Name"
                        Case 1
                            headerText = "Section"
                        Case 2
                            headerText = "Subject"
                        Case 3
                            headerText = "Start"
                        Case 4
                            headerText = "End"
                        Case 5
                            headerText = "Day"
                        Case 6
                            headerText = "Room"
                    End Select

                    Dim centerHeaderFormat As New StringFormat(Format)
                    centerHeaderFormat.Alignment = StringAlignment.Center

                    e.Graphics.DrawString(headerText, New Font("Calibri", 14, FontStyle.Bold), Brushes.Black, recta, centerHeaderFormat)


                    x += recta.Width
                    h = Math.Max(h, recta.Height)

                End If
            Next
            y += h
        End If

        isNewPage = False
        Dim dNow As Integer

        'Print Rows
        For dNow = rowIndexToPrint To printingdgv.RowCount - 1
            row = printingdgv.Rows(dNow)
            x = 20
            h = 0

            For Each cell As DataGridViewCell In row.Cells
                If cell.Visible Then
                    recta = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)
                    e.Graphics.DrawRectangle(Pens.Black, recta)

                    Format.Alignment = StringAlignment.Near
                    recta.Offset(5, 0)





                    e.Graphics.DrawString(cell.FormattedValue.ToString(), New Font("Calibri", 12, FontStyle.Regular), Brushes.Black, recta, Format)


                    x += recta.Width
                    h = Math.Max(h, recta.Height)
                End If
            Next

            y += h

            rowIndexToPrint = dNow + 1
            If y + h > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                isNewPage = True
                Return
            End If




        Next
    End Sub






    Private Sub Printbtn_Click(sender As Object, e As EventArgs) Handles Printbtn.Click
        printer.DefaultPageSettings.Landscape = True
        PrintPreviewDialog.WindowState = WindowState.Maximized
        PrintPreviewDialog.Document = printer
        PrintPreviewDialog.ShowDialog()
        printingdgv.DataSource = Nothing
        viewRooms.Show()
        Me.Close()



    End Sub
End Class