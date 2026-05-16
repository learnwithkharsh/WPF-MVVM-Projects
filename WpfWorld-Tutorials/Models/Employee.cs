using SQLite;


public class Employee 
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string Company { get; set; }
    public string Role { get; set; }
    public string Country { get; set; }
    public int Experience { get; set; }
}

public class EmployeeSqlite : Employee
{
    [PrimaryKey, AutoIncrement]
    public new int Id
    {
        get => base.Id;
        set => base.Id = value;
    }
}

