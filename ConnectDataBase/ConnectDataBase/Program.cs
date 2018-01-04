using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;

namespace ConnectDataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Connect con = new Connect();
            while (true)
            {
            
                Console.WriteLine("****QUIT or EXIT to quit****\n" +
                    "Lookup by typing COL, ROW, or ALL. Edit by typing ADD or DELETE");

                string input = Console.ReadLine();
                Console.WriteLine("\n");
                switch (input.ToLower())
                {
                    case "col":
                        con.printByCol();
                        Console.WriteLine("\n");
                        break;
                    case "row":
                        con.printByRow();
                        Console.WriteLine("\n");
                        break;
                    case "all":
                        con.printAll();
                        Console.WriteLine("\n");
                        break;
                    case "add":
                        con.add();
                        Console.WriteLine("\n");
                        break;
                    case "delete":
                        con.delete();
                        Console.WriteLine("\n");
                        break;
                    case "quit":
                    case "exit":
                        Environment.Exit(1);
                        break;

                }
            }
        }
    }
    class Connect
    {
        public string connectionString = "Data Source=(LocalDb)\\LocalDBDemo;Initial Catalog=StudentTracker;Integrated Security=True";
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader dataReader;

        
        //Print a specific columns
        public void printByCol()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            string type, choice;
            
            Console.WriteLine("Which columns would you like to view? (Id, First, Last, DOB)");
            
            string sql = null;
            
            string[] cols = Console.ReadLine().Split(' ');
            for (int i = 0; i < cols.Length; i++) 
                {
                    if (cols[i].ToLower() != "dob")
                    {
                        string form = cols[i].ToLower();
                        cols[i] = char.ToUpper(form[0]) + form.Substring(1);
                        Console.Write(cols[i] + "\t");
                    }
                    else 
                    {
                        cols[i] = cols[i].ToUpper();
                        Console.Write(cols[i] + "\t");
                    }
                }
            Console.WriteLine();
            sql = String.Join(", ", cols);
            
            command = new SqlCommand("select " + sql + " from dbo.students", connection);
            dataReader = command.ExecuteReader();

            while(dataReader.Read()) 
            {
                sql = null;
                for (int i = 0; i < dataReader.FieldCount; i++) 
                {
                    sql += dataReader.GetValue(i) + "\t";
                }
                Console.WriteLine(sql);
            }

            command.Dispose();
            connection.Close();
            
        }

        public void printByRow()
        {
            string data = null;
            connection = new SqlConnection(connectionString);
            connection.Open();

            command = new SqlCommand("select * from students", connection);
            dataReader = command.ExecuteReader();


            Console.WriteLine("Choose a row to display");
            int row = int.Parse(Console.ReadLine());
            Console.WriteLine(getTitles(dataReader));
            if (row < dataReader.FieldCount)
            {
                for (int i = 0; i < row; i++)
                {
                    dataReader.Read();
                }
                for (int i = 0; i < dataReader.FieldCount - 1; i++)
                {
                    data += dataReader.GetValue(i) + ":\t";
                }
                data += dataReader.GetValue(dataReader.FieldCount - 1);
                Console.WriteLine(data);
            }
            command.Dispose();
            connection.Close();
        }

        //print the entire table
        public void printAll()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        
            string sql = "Select * from dbo.students";

            command = new SqlCommand(sql, connection);
            dataReader = command.ExecuteReader();

            Console.WriteLine(getTitles(dataReader));
            while (dataReader.Read())
            {
                string data = null;
                for (int i = 0; i < dataReader.FieldCount - 1; i++)
                {
                    data += dataReader.GetValue(i) + ":\t";
                }
                data += dataReader.GetValue(dataReader.FieldCount - 1);
                Console.WriteLine(data);
                
            }


            command.Dispose();
            connection.Close();
        }

        //add an entry to the table
        public void add()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            string query = "Insert into dbo.students (First, Last, DOB) Values(@First, @Last, @DOB)";
            command = new SqlCommand(query, connection);

            Console.WriteLine("Enter a First name");
            command.Parameters.AddWithValue("@First", Console.ReadLine());
            Console.WriteLine("Enter a Last name");
            command.Parameters.AddWithValue("@Last", Console.ReadLine());
            Console.WriteLine("Enter a Date of Birth (mm-dd-yyyy");
            command.Parameters.AddWithValue("@DOB", Console.ReadLine());

            command.ExecuteNonQuery();


            command.Dispose();
            connection.Close();
        } 

        //delete an entry from the table
        public void delete()
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
            string type, choice;
            Console.WriteLine("choose a data type (Id, First, Last, or DOB");
            type = Console.ReadLine();

            Console.WriteLine("which entry would you like to delete?");
            choice = Console.ReadLine();
            string query = "Delete from dbo.students Where " + type + "=" + "\'" + choice + "\'";
            command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();

            command.Dispose();
            connection.Close();
        }

        //prints the specified number of titles
        public string getTitles(SqlDataReader reader)
        {
            int width = reader.FieldCount;
            string titles = null;
            for (int i = 0; i < width - 1; i++)
            {
                titles += reader.GetName(i) + "\t";
            }

            titles += "\t" + reader.GetName(width - 1);
            return titles;
        }
    }
}
