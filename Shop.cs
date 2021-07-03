using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Practice
{
    internal static class Shop
    {
        const string ConnectionString = @"Data Source=LAPTOP-0NI53OGU\SQLEXPRESS;Initial Catalog=MySecondDB;Pooling=true;Integrated Security=SSPI";

        static void Main(string[] args)
        {
            string want, wantname, wantcity, wantpname, wantprice, productname, name;
            bool status = true;
            Console.WriteLine("Enter command; write |help| if you want to know all the commands");
            want = Console.ReadLine();

            if (want == "help")
            {

                Console.WriteLine("List of commands: ");
                Console.WriteLine("#################### ");
                Console.WriteLine("selectCustomers ");
                Console.WriteLine("selectOrders ");
                Console.WriteLine("enterCustomer ");
                Console.WriteLine("enterOrder ");
                Console.WriteLine("RemoveCustomer");
                Console.WriteLine("RemoveOrder");
                Console.WriteLine("#################### ");
            }
            else if (want == "selectCustomers")
            {
                var customers = SelectCustomers();
                foreach (var customer in customers) Console.WriteLine(customer.Name);
            }
            else if (want == "selectOrders")
            {
                var orders = SelectOrders();
                foreach (var order in orders) Console.WriteLine($"{order.NameProduct} - {order.Price}");
            }
            else if (want == "enterCustomer")
            {
                Console.WriteLine("Enter name of customer: ");
                wantname = Console.ReadLine();
                Console.WriteLine("Enter city of customer: ");
                wantcity = Console.ReadLine();
                var createdCustomerId = EnterCustomer(wantname, wantcity);
                Console.WriteLine($"Created customer id: {createdCustomerId}");
            }
            else if (want == "enterOrder")
            {
                Console.WriteLine("Enter name of product: ");
                wantpname = Console.ReadLine();
                Console.WriteLine("Enter price: ");
                wantprice = Console.ReadLine();
                var createdOrderId = EnterOrder(wantpname, wantprice);
                Console.WriteLine($"Created order id: {createdOrderId}");
            }
            else if (want == "RemoveOrder")
            {
                Console.WriteLine("Enter product name of order: ");
                productname = Console.ReadLine();
                RemoveOrder(productname);
            }
            else if (want == "RemoveCustomer")
            {
                Console.WriteLine("Enter name of Customer: ");
                name = Console.ReadLine();
                RemoveCustomer(name);
            }

            static List<Customer> SelectCustomers()
            {
                var customers = new List<Customer>();
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"SELECT [Id], [Name], [City]
                                            FROM [Customer]";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customer = new Customer
                                {
                                    CustomerId = Convert.ToInt32(reader["Id"]),
                                    Name = Convert.ToString(reader["Name"]),
                                    City = Convert.ToString(reader["City"])
                                };
                                customers.Add(customer);
                            }
                        }
                    }
                }

                return customers;
            }

            static List<Order> SelectOrders()
            {
                var orders = new List<Order>();
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"SELECT [Id], [ProductName], [Price]
                                            FROM [Orders]";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var order = new Order
                                {
                                    OrderId = Convert.ToInt32(reader["Id"]),
                                    NameProduct = Convert.ToString(reader["ProductName"]),
                                    Price = Convert.ToInt32(reader["Price"]),
                                };
                                orders.Add(order);
                            }
                        }
                    }
                }

                return orders;
            }

            static int EnterCustomer(string name, string city)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO [Customer] (
                                        [Name],
                                        [City])
                                        VALUES (
                                        @name, 
                                        @city)
                                        SELECT SCOPE_IDENTITY()";

                        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
                        cmd.Parameters.Add("@city", SqlDbType.NVarChar).Value = city;

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }

            static int EnterOrder(string productname, string price)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = @"INSERT INTO [Orders] (
                                        [ProductName],
                                        [Price])
                                        VALUES (
                                        @productname, 
                                        @price)
                                        SELECT SCOPE_IDENTITY()";

                        cmd.Parameters.Add("@productname", SqlDbType.NVarChar).Value = productname;
                        cmd.Parameters.Add("@price", SqlDbType.NVarChar).Value = price;

                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }


            static void RemoveOrder(string productname)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    string sql = "DELETE FROM [Orders] WHERE ProductName = @productname";
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@productname", productname);
                    cmd.ExecuteNonQuery();
                }
            }

            static void RemoveCustomer(string name)
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    string sql = "DELETE FROM [Customer] WHERE Name = @name";
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                }
            }        

        }
    }
}
