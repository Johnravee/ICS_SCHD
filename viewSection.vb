﻿Public Class viewSection
    Dim startPositionX As Integer = 30
    Dim startPositionY As Integer = 10
    Dim buttonWidth As Integer = 200
    Dim buttonHeight As Integer = 100
    Dim buttonSpacingX As Integer = 10
    Dim buttonSpacingY As Integer = 10
    Dim buttonCountInRow As Integer = 0
    Dim currentRow As Integer = 0
    Dim roomsTable As New DataTable()
    Public Shared Section As String = ""


    Private Sub viewSection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            DBCon()
            cmd.Connection = con
            cmd.CommandText = "SELECT CONCAT(Section_Program, '-', Year, '', Section_Code) AS Section FROM sections"
            dataReader.SelectCommand = cmd
            dataReader.Fill(roomsTable)


            For Each row As DataRow In roomsTable.Rows

                Dim btn As New Button()
                btn.Text = row("Section")
                btn.Size = New Size(buttonWidth, buttonHeight)
                btn.Font = New Font("Calibri", 20, Font.Style.Bold)


                Dim imageFilePath As String = "C:\Users\ravem\OneDrive\Desktop\ICS_SCHD\Resources\icons8-section-sign-key-80.png"
                If System.IO.File.Exists(imageFilePath) Then
                    btn.Image = Image.FromFile(imageFilePath)
                    btn.ImageAlign = ContentAlignment.MiddleCenter
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText

                End If



                Dim xCoordinate As Integer = startPositionX + (buttonWidth + buttonSpacingX) * buttonCountInRow
                Dim yCoordinate As Integer = startPositionY + (buttonHeight + buttonSpacingY) * currentRow


                btn.Location = New Point(xCoordinate, yCoordinate)
                panelbtns.Controls.Add(btn)
                buttonCountInRow += 1

                If buttonCountInRow = 6 Then
                    currentRow += 1
                    buttonCountInRow = 0
                End If

                AddHandler btn.Click, AddressOf SectionButton

            Next



        Catch ex As Exception
            MsgBox(ex.Message())
        End Try
    End Sub

    Private Sub SectionButton(sender As Object, e As EventArgs)
        Section = sender.Text
        sectionSched.Show()
        Me.Hide()

    End Sub

    Private Sub backBtn_Click(sender As Object, e As EventArgs) Handles backBtn.Click
        Dashboard.Show()
        Me.Hide()

    End Sub


End Class