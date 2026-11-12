
/*Name: Jhon Michael
 Matricula: 2025-1861*/


using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

class Program
{
    static int Main(string[] args)
    {
        string connectionString = "Server=localhost;Port=3306;Database=employee_system;Uid=root;Pwd=1234;";

        string selectEmployees = "SELECT e.id, e.name, e.lastname, e.idcard, d.id, d.name, p.id, p.name " +
                                 "FROM employees e " +
                                 "INNER JOIN departments d ON e.department_id = d.id " +
                                 "INNER JOIN positions p ON e.position_id = p.id";

        string opcion = "";
        string exit = "no";

        List<Department> listdepartments = new List<Department>();
        List<Position> listpositions = new List<Position>();

        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();

            using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM departments ORDER BY id", connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    listdepartments.Add(new Department { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                }
            }

            using (MySqlCommand cmd = new MySqlCommand("SELECT id, name FROM positions ORDER BY id", connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    listpositions.Add(new Position { Id = reader.GetInt32(0), Name = reader.GetString(1) });
                }
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"Could not connect to the database: {ex.Message}");
            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
            return 1;
        }

        do
        {
            Console.WriteLine("--------Welcome to the Employee System----------");
            Console.WriteLine("============================");
            Console.WriteLine("Select An Option");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Employee List");
            Console.WriteLine("3. Search Employee");
            Console.WriteLine("4. Modify Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. List by Department");
            Console.WriteLine("7. Add Department");
            Console.WriteLine("8. Add Position");
            Console.WriteLine("9. Exit");
            Console.WriteLine("============================");

            opcion = (Console.ReadLine() ?? "").Trim();

            try
            {
                switch (opcion)
                {
                    case "1":
                        {
                            exit = "yes";

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("--------Adding New Employee----------");

                                Console.WriteLine("Please enter the employee's name:");
                                string name = (Console.ReadLine() ?? "").Trim();

                                while (name == "")
                                {
                                    Console.WriteLine("ERROR: Please enter a name:");
                                    name = (Console.ReadLine() ?? "").Trim();
                                }

                                Console.WriteLine("Please enter the employee's last name:");
                                string lastname = (Console.ReadLine() ?? "").Trim();

                                while (lastname == "")
                                {
                                    Console.WriteLine("ERROR: Please enter the last name:");
                                    lastname = (Console.ReadLine() ?? "").Trim();
                                }

                                string idcard;

                                while (true)
                                {
                                    Console.WriteLine("Please enter the employee's ID card (11 digits):");
                                    idcard = (Console.ReadLine() ?? "").Trim();

                                    if (idcard.Length != 11 || !idcard.All(char.IsDigit))
                                    {
                                        Console.WriteLine("ERROR: The ID card must have exactly 11 digits.");
                                        continue;
                                    }

                                    bool idcardExists;

                                    using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM employees WHERE idcard = @idcard", connection))
                                    {
                                        checkCmd.Parameters.AddWithValue("@idcard", idcard);
                                        idcardExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                                    }

                                    if (idcardExists)
                                    {
                                        Console.WriteLine("This ID card already exists.");
                                        continue;
                                    }

                                    break;
                                }

                                Department? selectedDepartment = null;

                                while (selectedDepartment == null)
                                {
                                    Console.WriteLine("Select the department:");

                                    foreach (Department dep in listdepartments)
                                    {
                                        Console.WriteLine($"{dep.Id}. {dep.Name}");
                                    }

                                    if (int.TryParse(Console.ReadLine(), out int depId))
                                    {
                                        selectedDepartment = listdepartments.Find(d => d.Id == depId);
                                    }

                                    if (selectedDepartment == null)
                                    {
                                        Console.WriteLine("Invalid selection.");
                                    }
                                }

                                Position? selectedPosition = null;

                                while (selectedPosition == null)
                                {
                                    Console.WriteLine("Select the position:");

                                    foreach (Position pos in listpositions)
                                    {
                                        Console.WriteLine($"{pos.Id}. {pos.Name}");
                                    }

                                    if (int.TryParse(Console.ReadLine(), out int posId))
                                    {
                                        selectedPosition = listpositions.Find(p => p.Id == posId);
                                    }

                                    if (selectedPosition == null)
                                    {
                                        Console.WriteLine("Invalid selection.");
                                    }
                                }

                                using (MySqlCommand insertCmd = new MySqlCommand("INSERT INTO employees (name, lastname, idcard, department_id, position_id) VALUES (@name, @lastname, @idcard, @department, @position)", connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@name", name);
                                    insertCmd.Parameters.AddWithValue("@lastname", lastname);
                                    insertCmd.Parameters.AddWithValue("@idcard", idcard);
                                    insertCmd.Parameters.AddWithValue("@department", selectedDepartment.Id);
                                    insertCmd.Parameters.AddWithValue("@position", selectedPosition.Id);
                                    insertCmd.ExecuteNonQuery();

                                    Console.WriteLine($"Employee added successfully with ID {insertCmd.LastInsertedId}.");
                                }

                                Console.WriteLine("Would you like to continue? (yes/no)");
                                exit = (Console.ReadLine() ?? "").Trim().ToLower();

                            } while (exit == "yes" || exit == "y");

                            Console.Clear();
                            break;
                        }

                    case "2":
                        {
                            Console.Clear();
                            Console.WriteLine("--------Employee List----------");

                            List<Employee> allEmployees = new List<Employee>();

                            using (MySqlCommand listCmd = new MySqlCommand(selectEmployees + " ORDER BY e.id", connection))
                            using (MySqlDataReader reader = listCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    allEmployees.Add(new Employee(reader));
                                }
                            }

                            if (allEmployees.Count == 0)
                            {
                                Console.WriteLine("No employees registered yet.");
                            }
                            else
                            {
                                foreach (Employee emp in allEmployees)
                                {
                                    Console.WriteLine(emp);
                                }
                            }

                            Console.WriteLine("Press ENTER to return to the menu...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }

                    case "3":
                        {
                            exit = "no";

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("--------Search Employee--------");
                                Console.WriteLine("1. Search by ID");
                                Console.WriteLine("2. Search by ID Card");
                                Console.WriteLine("3. Search by Name or Last Name");
                                Console.WriteLine("4. Search by Department");

                                string searchOpcion = (Console.ReadLine() ?? "").Trim();
                                string whereClause = "";
                                object paramValue = "";
                                bool validSearch = true;

                                if (searchOpcion == "1")
                                {
                                    Console.WriteLine("Enter the employee ID:");

                                    if (int.TryParse(Console.ReadLine(), out int searchId) && searchId > 0)
                                    {
                                        whereClause = " WHERE e.id = @value";
                                        paramValue = searchId;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid ID format.");
                                        validSearch = false;
                                    }
                                }
                                else if (searchOpcion == "2")
                                {
                                    Console.WriteLine("Enter the ID card:");
                                    string searchIdCard = (Console.ReadLine() ?? "").Trim();

                                    if (searchIdCard == "")
                                    {
                                        Console.WriteLine("ID card cannot be empty.");
                                        validSearch = false;
                                    }
                                    else
                                    {
                                        whereClause = " WHERE e.idcard = @value";
                                        paramValue = searchIdCard;
                                    }
                                }
                                else if (searchOpcion == "3")
                                {
                                    Console.WriteLine("Enter the name or last name:");
                                    string searchName = (Console.ReadLine() ?? "").Trim();

                                    if (searchName == "")
                                    {
                                        Console.WriteLine("The text cannot be empty.");
                                        validSearch = false;
                                    }
                                    else
                                    {
                                        whereClause = " WHERE e.name LIKE @value OR e.lastname LIKE @value";
                                        paramValue = "%" + searchName + "%";
                                    }
                                }
                                else if (searchOpcion == "4")
                                {
                                    Department? searchDepartment = null;

                                    while (searchDepartment == null)
                                    {
                                        Console.WriteLine("Select the department:");

                                        foreach (Department dep in listdepartments)
                                        {
                                            Console.WriteLine($"{dep.Id}. {dep.Name}");
                                        }

                                        if (int.TryParse(Console.ReadLine(), out int searchDepId))
                                        {
                                            searchDepartment = listdepartments.Find(d => d.Id == searchDepId);
                                        }

                                        if (searchDepartment == null)
                                        {
                                            Console.WriteLine("Invalid selection.");
                                        }
                                    }

                                    whereClause = " WHERE e.department_id = @value";
                                    paramValue = searchDepartment.Id;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid Option.");
                                    validSearch = false;
                                }

                                if (validSearch)
                                {
                                    List<Employee> foundEmployees = new List<Employee>();

                                    using (MySqlCommand searchCmd = new MySqlCommand(selectEmployees + whereClause + " ORDER BY e.id", connection))
                                    {
                                        searchCmd.Parameters.AddWithValue("@value", paramValue);

                                        using (MySqlDataReader reader = searchCmd.ExecuteReader())
                                        {
                                            while (reader.Read())
                                            {
                                                foundEmployees.Add(new Employee(reader));
                                            }
                                        }
                                    }

                                    if (foundEmployees.Count == 0)
                                    {
                                        Console.WriteLine("No employees found.");
                                    }
                                    else
                                    {
                                        foreach (Employee emp in foundEmployees)
                                        {
                                            Console.WriteLine(emp);
                                        }
                                    }
                                }

                                Console.WriteLine("Do you want to return to the main menu? (yes/no)");
                                exit = (Console.ReadLine() ?? "").Trim().ToLower();

                            } while (exit == "no" || exit == "n");

                            Console.Clear();
                            break;
                        }

                    case "4":
                        {
                            exit = "no";

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("--------Modifying Employee----------");
                                Console.WriteLine("Please enter the employee ID to modify:");

                                if (int.TryParse(Console.ReadLine(), out int modifyId) && modifyId > 0)
                                {
                                    Employee? employeeToModify = null;

                                    using (MySqlCommand findCmd = new MySqlCommand(selectEmployees + " WHERE e.id = @id", connection))
                                    {
                                        findCmd.Parameters.AddWithValue("@id", modifyId);

                                        using (MySqlDataReader reader = findCmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                employeeToModify = new Employee(reader);
                                            }
                                        }
                                    }

                                    if (employeeToModify != null)
                                    {
                                        Console.WriteLine("============================");
                                        Console.WriteLine($"Current information for employee ID {modifyId}:");
                                        Console.WriteLine(employeeToModify);
                                        Console.WriteLine("============================");

                                        Console.WriteLine("Enter the new name (ENTER to keep current):");
                                        string newName = (Console.ReadLine() ?? "").Trim();

                                        if (newName != "")
                                        {
                                            employeeToModify.Name = newName;
                                        }

                                        Console.WriteLine("Enter the new last name (ENTER to keep current):");
                                        string newLastname = (Console.ReadLine() ?? "").Trim();

                                        if (newLastname != "")
                                        {
                                            employeeToModify.Lastname = newLastname;
                                        }

                                        while (true)
                                        {
                                            Console.WriteLine("Enter the new ID card, 11 digits (ENTER to keep current):");
                                            string newIdcard = (Console.ReadLine() ?? "").Trim();

                                            if (newIdcard == "")
                                            {
                                                break;
                                            }

                                            if (newIdcard.Length != 11 || !newIdcard.All(char.IsDigit))
                                            {
                                                Console.WriteLine("ERROR: The ID card must have exactly 11 digits.");
                                                continue;
                                            }

                                            bool idcardTaken;

                                            using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM employees WHERE idcard = @idcard AND id <> @id", connection))
                                            {
                                                checkCmd.Parameters.AddWithValue("@idcard", newIdcard);
                                                checkCmd.Parameters.AddWithValue("@id", modifyId);
                                                idcardTaken = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                                            }

                                            if (idcardTaken)
                                            {
                                                Console.WriteLine("This ID card already exists.");
                                                continue;
                                            }

                                            employeeToModify.Idcard = newIdcard;
                                            break;
                                        }

                                        while (true)
                                        {
                                            Console.WriteLine("Select the new department (ENTER to keep current):");

                                            foreach (Department dep in listdepartments)
                                            {
                                                Console.WriteLine($"{dep.Id}. {dep.Name}");
                                            }

                                            string depInput = (Console.ReadLine() ?? "").Trim();

                                            if (depInput == "")
                                            {
                                                break;
                                            }

                                            if (int.TryParse(depInput, out int newDepId))
                                            {
                                                Department? newDepartment = listdepartments.Find(d => d.Id == newDepId);

                                                if (newDepartment != null)
                                                {
                                                    employeeToModify.Department = newDepartment;
                                                    break;
                                                }
                                            }

                                            Console.WriteLine("Invalid selection.");
                                        }

                                        while (true)
                                        {
                                            Console.WriteLine("Select the new position (ENTER to keep current):");

                                            foreach (Position pos in listpositions)
                                            {
                                                Console.WriteLine($"{pos.Id}. {pos.Name}");
                                            }

                                            string posInput = (Console.ReadLine() ?? "").Trim();

                                            if (posInput == "")
                                            {
                                                break;
                                            }

                                            if (int.TryParse(posInput, out int newPosId))
                                            {
                                                Position? newPosition = listpositions.Find(p => p.Id == newPosId);

                                                if (newPosition != null)
                                                {
                                                    employeeToModify.Position = newPosition;
                                                    break;
                                                }
                                            }

                                            Console.WriteLine("Invalid selection.");
                                        }

                                        using (MySqlCommand updateCmd = new MySqlCommand("UPDATE employees SET name = @name, lastname = @lastname, idcard = @idcard, department_id = @department, position_id = @position WHERE id = @id", connection))
                                        {
                                            updateCmd.Parameters.AddWithValue("@name", employeeToModify.Name);
                                            updateCmd.Parameters.AddWithValue("@lastname", employeeToModify.Lastname);
                                            updateCmd.Parameters.AddWithValue("@idcard", employeeToModify.Idcard);
                                            updateCmd.Parameters.AddWithValue("@department", employeeToModify.Department.Id);
                                            updateCmd.Parameters.AddWithValue("@position", employeeToModify.Position.Id);
                                            updateCmd.Parameters.AddWithValue("@id", modifyId);
                                            updateCmd.ExecuteNonQuery();
                                        }

                                        Console.WriteLine("Employee information updated successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Employee not found.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid ID format.");
                                }

                                Console.WriteLine("Do you want to return to the main menu? (yes/no)");
                                exit = (Console.ReadLine() ?? "").Trim().ToLower();

                            } while (exit == "no" || exit == "n");

                            Console.Clear();
                            break;
                        }

                    case "5":
                        {
                            exit = "no";

                            do
                            {
                                Console.Clear();
                                Console.WriteLine("--------Deleting Employee----------");
                                Console.WriteLine("Please enter the employee ID to delete:");

                                if (int.TryParse(Console.ReadLine(), out int deleteId) && deleteId > 0)
                                {
                                    Employee? employeeToDelete = null;

                                    using (MySqlCommand findCmd = new MySqlCommand(selectEmployees + " WHERE e.id = @id", connection))
                                    {
                                        findCmd.Parameters.AddWithValue("@id", deleteId);

                                        using (MySqlDataReader reader = findCmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                employeeToDelete = new Employee(reader);
                                            }
                                        }
                                    }

                                    if (employeeToDelete != null)
                                    {
                                        Console.WriteLine("============================");
                                        Console.WriteLine($"Current information for employee ID {deleteId}:");
                                        Console.WriteLine(employeeToDelete);
                                        Console.WriteLine("============================");

                                        Console.WriteLine("Are you sure you want to delete this employee? (yes/no)");
                                        string confirm = (Console.ReadLine() ?? "").Trim().ToLower();

                                        if (confirm == "yes" || confirm == "y")
                                        {
                                            using (MySqlCommand deleteCmd = new MySqlCommand("DELETE FROM employees WHERE id = @id", connection))
                                            {
                                                deleteCmd.Parameters.AddWithValue("@id", deleteId);
                                                deleteCmd.ExecuteNonQuery();
                                            }

                                            Console.WriteLine("Employee deleted successfully.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Deletion cancelled.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Employee not found.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid ID format.");
                                }

                                Console.WriteLine("Do you want to return to the main menu? (yes/no)");
                                exit = (Console.ReadLine() ?? "").Trim().ToLower();

                            } while (exit == "no" || exit == "n");

                            Console.Clear();
                            break;
                        }

                    case "6":
                        {
                            Console.Clear();
                            Console.WriteLine("--------List by Department----------");

                            foreach (Department dep in listdepartments)
                            {
                                List<Employee> inDepartment = new List<Employee>();

                                using (MySqlCommand depCmd = new MySqlCommand(selectEmployees + " WHERE e.department_id = @dep ORDER BY e.id", connection))
                                {
                                    depCmd.Parameters.AddWithValue("@dep", dep.Id);

                                    using (MySqlDataReader reader = depCmd.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            inDepartment.Add(new Employee(reader));
                                        }
                                    }
                                }

                                Console.WriteLine($"== {dep.Name} ({inDepartment.Count} employees) ==");

                                if (inDepartment.Count == 0)
                                {
                                    Console.WriteLine("No employees in this department.");
                                }
                                else
                                {
                                    foreach (Employee emp in inDepartment)
                                    {
                                        Console.WriteLine(emp);
                                    }
                                }

                                Console.WriteLine();
                            }

                            Console.WriteLine("Press ENTER to return to the menu...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }

                    case "7":
                        {
                            Console.Clear();
                            Console.WriteLine("--------Add Department----------");
                            Console.WriteLine("Enter the department name:");

                            string departmentName = (Console.ReadLine() ?? "").Trim();

                            while (departmentName == "")
                            {
                                Console.WriteLine("Department name cannot be empty:");
                                departmentName = (Console.ReadLine() ?? "").Trim();
                            }

                            bool departmentExists;

                            using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM departments WHERE name = @name", connection))
                            {
                                checkCmd.Parameters.AddWithValue("@name", departmentName);
                                departmentExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                            }

                            if (departmentExists)
                            {
                                Console.WriteLine("Department already exists.");
                            }
                            else
                            {
                                using (MySqlCommand insertCmd = new MySqlCommand("INSERT INTO departments (name) VALUES (@name)", connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@name", departmentName);
                                    insertCmd.ExecuteNonQuery();

                                    listdepartments.Add(new Department
                                    {
                                        Id = (int)insertCmd.LastInsertedId,
                                        Name = departmentName
                                    });
                                }

                                Console.WriteLine("Department added successfully.");
                            }

                            Console.WriteLine("Press ENTER to return to the menu...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }

                    case "8":
                        {
                            Console.Clear();
                            Console.WriteLine("--------Add Position----------");
                            Console.WriteLine("Enter the position name:");

                            string positionName = (Console.ReadLine() ?? "").Trim();

                            while (positionName == "")
                            {
                                Console.WriteLine("Position name cannot be empty:");
                                positionName = (Console.ReadLine() ?? "").Trim();
                            }

                            bool positionExists;

                            using (MySqlCommand checkCmd = new MySqlCommand("SELECT COUNT(*) FROM positions WHERE name = @name", connection))
                            {
                                checkCmd.Parameters.AddWithValue("@name", positionName);
                                positionExists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                            }

                            if (positionExists)
                            {
                                Console.WriteLine("Position already exists.");
                            }
                            else
                            {
                                using (MySqlCommand insertCmd = new MySqlCommand("INSERT INTO positions (name) VALUES (@name)", connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@name", positionName);
                                    insertCmd.ExecuteNonQuery();

                                    listpositions.Add(new Position
                                    {
                                        Id = (int)insertCmd.LastInsertedId,
                                        Name = positionName
                                    });
                                }

                                Console.WriteLine("Position added successfully.");
                            }

                            Console.WriteLine("Press ENTER to return to the menu...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }

                    case "9":
                        {
                            Console.WriteLine("Exiting the program...");
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("Invalid option. Please try again.");
                            Console.WriteLine("Press ENTER to continue...");
                            Console.ReadLine();
                            Console.Clear();
                            break;
                        }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                Console.WriteLine("Press ENTER to return to the menu...");
                Console.ReadLine();
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine("Press ENTER to return to the menu...");
                Console.ReadLine();
                Console.Clear();
            }

        } while (opcion != "9");

        connection.Close();

        Console.WriteLine("Thank you for using the Employee System.");

        return 0;
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Lastname { get; set; } = "";
    public string Idcard { get; set; } = "";
    public Department Department { get; set; } = new Department();
    public Position Position { get; set; } = new Position();

    public Employee()
    {
    }

    public Employee(MySqlDataReader reader)
    {
        Id = reader.GetInt32(0);
        Name = reader.GetString(1);
        Lastname = reader.GetString(2);
        Idcard = reader.GetString(3);
        Department = new Department { Id = reader.GetInt32(4), Name = reader.GetString(5) };
        Position = new Position { Id = reader.GetInt32(6), Name = reader.GetString(7) };
    }

    public override string ToString()
    {
        return $"ID: {Id} | {Name} {Lastname} | ID Card: {Idcard} | Department: {Department.Name} | Position: {Position.Name}";
    }
}

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Position
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}




