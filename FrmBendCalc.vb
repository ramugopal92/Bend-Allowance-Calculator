Public Class FrmBendCalc

    Private validationShown As Boolean = False

    Private Sub FrmBendCalc_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Clear outputs when form starts
        ClearOutputs()
    End Sub

    '==========================
    ' On-type triggers
    '==========================
    Private Sub txtThickness_TextChanged(sender As Object, e As EventArgs) Handles txtThickness.TextChanged
        CalculateValues()
    End Sub

    Private Sub txtRadius_TextChanged(sender As Object, e As EventArgs) Handles txtRadius.TextChanged
        CalculateValues()
    End Sub

    Private Sub txtAngle_TextChanged(sender As Object, e As EventArgs) Handles txtAngle.TextChanged
        CalculateValues()
    End Sub

    Private Sub txtK_TextChanged(sender As Object, e As EventArgs) Handles txtK.TextChanged
        CalculateValues()
    End Sub

    Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) _
        Handles txtThickness.KeyPress, txtRadius.KeyPress, txtAngle.KeyPress, txtK.KeyPress

        Dim tb As TextBox = DirectCast(sender, TextBox)

        ' Allow control keys like Backspace
        If Char.IsControl(e.KeyChar) Then Exit Sub

        ' Allow digits
        If Char.IsDigit(e.KeyChar) Then Exit Sub

        ' Allow one decimal point
        If e.KeyChar = "."c AndAlso Not tb.Text.Contains(".") Then Exit Sub

        ' Block everything else
        e.Handled = True
    End Sub

    '==========================
    ' Main calculation routine
    '==========================
    Private Sub CalculateValues()
        Dim T As Double, R As Double, A As Double, K As Double
        Dim BA As Double, SB As Double, BD As Double
        Dim PI As Double = Math.PI

        ' If any input is blank or non-numeric → clear output and exit silently
        If Not GetNumber(txtThickness, T) Then ClearOutputs() : Exit Sub
        If Not GetNumber(txtRadius, R) Then ClearOutputs() : Exit Sub
        If Not GetNumber(txtAngle, A) Then ClearOutputs() : Exit Sub
        If Not GetNumber(txtK, K) Then ClearOutputs() : Exit Sub

        ' Engineering validation
        If T <= 0 Then
            ClearOutputs()
            ShowValidationMessage("Sheet Thickness must be greater than zero.", txtThickness)
            Exit Sub
        End If

        If R <= 0 Then
            ClearOutputs()
            ShowValidationMessage("Bend Radius must be greater than zero.", txtRadius)
            Exit Sub
        End If

        If A <= 0 Or A > 180 Then
            ClearOutputs()
            ShowValidationMessage("Bend Angle must be between 0 and 180 degrees.", txtAngle)
            Exit Sub
        End If

        If K <= 0 Or K > 0.5 Then
            ClearOutputs()
            ShowValidationMessage("K-Factor must be between 0 and 0.5.", txtK)
            Exit Sub
        End If

        '=== FORMULAS ===
        Dim Arad As Double = A * PI / 180.0

        ' Setback
        SB = (R + T) * Math.Tan((A / 2.0) * PI / 180.0)

        ' Bend Allowance
        BA = Arad * (R + K * T)

        ' Bend Deduction
        BD = 2.0 * SB - BA

        '=== DISPLAY ===
        txtsetback.Text = FixZero(SB).ToString("0.000")
        txtBA.Text = FixZero(BA).ToString("0.000")
        txtBD.Text = FixZero(BD).ToString("0.000")
    End Sub

    '==========================
    ' Helpers
    '==========================
    Private Function GetNumber(tb As TextBox, ByRef outVal As Double) As Boolean
        Dim s As String = Trim(tb.Text)

        If s = "" Then Return False

        Return Double.TryParse(s, outVal)
    End Function

    Private Sub ClearOutputs()
        txtsetback.Text = ""
        txtBA.Text = ""
        txtBD.Text = ""
    End Sub

    Private Function FixZero(v As Double) As Double
        If Math.Abs(v) < 0.0000001 Then v = 0
        Return v
    End Function

    Private Sub ShowValidationMessage(msg As String, tb As TextBox)
        If validationShown Then Exit Sub

        validationShown = True
        MessageBox.Show(msg, "Bend Allowance Calculator", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        tb.Focus()
        tb.SelectAll()
        validationShown = False
    End Sub

End Class