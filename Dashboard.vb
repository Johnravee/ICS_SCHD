﻿Public Class Dashboard
    Private Sub closeBtn_Click(sender As Object, e As EventArgs) Handles closeBtn.Click
        Me.Close()
    End Sub

    Private Sub minimizeBtn_Click(sender As Object, e As EventArgs) Handles minimizeBtn.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub



    Private Sub btnNewSched_Click(sender As Object, e As EventArgs) Handles btnNewSched.Click
        CreateScheduleForm.Show()
        Me.Hide()
    End Sub

    Private Sub btnNewInstructor_Click(sender As Object, e As EventArgs) Handles btnNewInstructor.Click
        addNewInstructor.Show()
        Me.Hide()
    End Sub



    Private Sub btnViewFacilities_Click(sender As Object, e As EventArgs) Handles btnViewFacilities.Click
        Me.Hide()
        viewRooms.Show()
    End Sub

    Private Sub btnViewSection_Click(sender As Object, e As EventArgs) Handles btnViewSection.Click
        Me.Hide()
        viewSection.Show()
    End Sub

    Private Sub btnViewInstructor_Click(sender As Object, e As EventArgs) Handles btnViewInstructor.Click
        Me.Hide()
        viewInstructors.Show()
    End Sub

    Private Sub btnViewSubject_Click(sender As Object, e As EventArgs) Handles btnViewSubject.Click
        Me.Hide()
        SubjectForm.Show()
    End Sub


    'Logout Problem
    Private Sub LogoutBtn_Click_1(sender As Object, e As EventArgs)
        MsgBox("Clicks")
    End Sub

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
