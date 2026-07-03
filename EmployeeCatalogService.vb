Public Class EmployeeCatalogService
    Private ReadOnly _sqlRepository As SqlProjectRepository

    Public Sub New(Optional sqlRepository As SqlProjectRepository = Nothing)
        _sqlRepository = sqlRepository
    End Sub

    Public Function LoadEmployees() As List(Of String)
        If _sqlRepository IsNot Nothing Then
            Try
                Dim employeesFromSql = _sqlRepository.LoadEmployees()
                If employeesFromSql.Count > 0 Then
                    Return employeesFromSql
                End If
            Catch
            End Try
        End If

        Return New List(Of String) From {
            "Devarajan",
            "Mahaboob Basha",
            "Aashiq Aliuddin",
            "Mohammed Bilal",
            "Aboo Dalha",
            "Mugaitheen Alla Pichai",
            "Rafath Rehman",
            "Mohammed Faizal",
            "Abdur Rahman",
            "Tanveer Aafaque",
            "Abubacker Siddque",
            "Mohammed Harris",
            "Ubaidur Rahman",
            "Mohamed Nauman",
            "Mohammed Saif Qurashi",
            "Mohammed Imran A",
            "Abul Kasim Junaithul Bakthath",
            "Fazrul Rahman",
            "Syed Dasthagir",
            "Mohammed Hameed",
            "Shariq Hassan",
            "Mohammed Ateeq Ul Huq",
            "Irfan Ahamed",
            "Aamir Ahmed",
            "Abdul Basith M",
            "Abdul Shakeel",
            "Afzal Khan",
            "Mohammed Lukmaan",
            "Maqthum Sheriff",
            "Mohamed Samsudeen",
            "Mohammed Sheik Ahsan",
            "Mohammed Abbas",
            "Mohamed Ibrahim",
            "Abdul Rahman A R",
            "Inamul Hasan",
            "Mohamed Sameem Salman",
            "Mohammed Inamul Hasan",
            "Nisar Ahamed",
            "Syed Anwar",
            "Ihthishamul Haq"
        }
    End Function
End Class
