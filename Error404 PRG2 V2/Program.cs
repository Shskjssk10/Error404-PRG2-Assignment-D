using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Error404_PRG2_V2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Dictionary<int, Customer> customerDict = InitData();
            while (true)
            {
                //Menu interface containing options for user to enter
                MenuInterface();
                string option = Console.ReadLine();
                if (option == "1")
                {
                    Console.WriteLine();
                    Option1(customerDict);
                    Console.WriteLine();
                }
                else if (option == "2")
                {
                    Console.WriteLine();
                    Option2(customerDict);
                    Console.WriteLine();
                }
                else if (option == "3")
                {
                    Console.WriteLine();
                    Option3(customerDict);
                    Console.WriteLine();
                }
                else if (option == "4")
                {
                    Console.WriteLine();
                    Option4(customerDict);
                    Console.WriteLine();
                }
                else if (option == "5")
                {
                    Console.WriteLine();
                    Option5(customerDict);
                    Console.WriteLine();
                }
                else if (option == "6")
                {
                    Console.WriteLine();
                    Option6(customerDict);
                    Console.WriteLine();
                }
                else if (option == "7")
                {
                    Console.WriteLine();
                    Option7(customerDict);
                    Console.WriteLine();
                }
                else if (option == "8")
                {
                    Console.WriteLine();
                    Option8(customerDict);
                    Console.WriteLine();
                }
                else if (option == "0")
                {
                    Console.WriteLine();
                    Console.WriteLine("Thank you! Goodbye :D");
                    break;
                }
                // Test case for Caden -- help delete if this is still here ================================================================
                else if (option == "caden")
                {
                    List<Flavour> flavourList = new List<Flavour>();
                    flavourList.Add(new Flavour("Strawberry", false, 2));
                    flavourList.Add(new Flavour("Vanilla", false, 1));
                    string flavour = "Vanilla";

                    foreach (Flavour flavourInList in flavourList)
                    {
                        if (flavourInList.Type == flavour)
                        {
                            Console.WriteLine(true);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Please return a valid input. An integer between 0-6 inclusive!");
                }
            }
        }

        // Method for menu interface
        static void MenuInterface()
        {
            Console.Write("================= MENU INTERFACE =================\n[1] List all customers\n[2] List all current orders\n[3] Register" +
                " a new customer\n[4] Create a customer's order\n[5] Display order details of a customer\n[6] Modify order details\n[7] Process an Order and Checkout" +
                "\n[8] Display Financial Details\n[0] Exit\n==================================================\nEnter option: ");
        }

        // Method for initiating data
        static Dictionary<int, Customer> InitData()
        {
            // Creates customer objects and apends all PointCard relevant data into it. 
            // At the moment does not append orderHistory
            Dictionary<int, Customer> customerDict = new Dictionary<int, Customer>();

            // Reading customer data from customers.csv to create Customer objects 
            using (StreamReader sr = new StreamReader("customers.csv"))
            {
                string header = sr.ReadLine();
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] contents = line.Split(',');
                    if (int.TryParse(contents[1], out int memId) &&
                        DateTime.TryParse(contents[2], out DateTime dob))
                    {
                        Customer customer = new Customer(contents[0], memId, dob);
                        customer.Rewards.Tier = contents[3];
                        customer.Rewards.Points = int.Parse(contents[4]);
                        customer.Rewards.PunchCard = int.Parse(contents[5]);
                        customerDict.Add(memId, customer);
                    }
                }
            }

            string orderPath = "orders.csv";
            string[] temp = File.ReadAllLines(orderPath);

            //Skips the header
            string[] orderContents = temp.Skip(1).ToArray();

            string format = "dd/MM/yyyy HH:mm";

            // MemberID and OrderID respectively, used for referencing later.
            Dictionary<int, int> orderCustomerPairs = new Dictionary<int, int>();
            foreach(string line in orderContents)
            {
                var selectedLine = line.Split(',');
                try
                {
                    orderCustomerPairs.Add(Convert.ToInt32(selectedLine[0]), Convert.ToInt32(selectedLine[1]));
                }
                catch (Exception ex) 
                { 
                }
            }

            // Temp dictionary for storing orders, used for referencing later.
            Dictionary<int, Order> tempOrderList = new Dictionary<int, Order>();

            // foreach loop to create order objects 
            foreach (string line in orderContents)
            {
                // Appends member ID attributed to it, time received and fulfilled.
                string[] selectedLine = line.Split(',');

                // Adds first order as there are no orders in dictionary
                if (tempOrderList.Count == 0)
                {
                    Order order = new Order(Convert.ToInt32(selectedLine[0]), DateTime.Parse(selectedLine[2]));
                    order.TimeFulfilled = DateTime.Parse(selectedLine[3]);
                    tempOrderList.Add(Convert.ToInt32(selectedLine[0]), order) ;
                }
                // Checks for duplicates after first order is added
                else
                {
                    if (tempOrderList.Keys.Contains(Convert.ToInt32(selectedLine[0])) == false)
                    {
                        //Console.WriteLine(selectedLine[2]);
                        Order order1 = new Order(Convert.ToInt32(selectedLine[0]), DateTime.Parse(selectedLine[2]));
                        try
                        {
                            order1.TimeFulfilled = DateTime.Parse(selectedLine[3]);
                        }
                        catch (Exception e)
                        {
                            order1.TimeFulfilled = null;
                        }
                        tempOrderList.Add(Convert.ToInt32(selectedLine[0]), order1);

                    }
                }
            }

            // foreach loop to append respective data to each order object
            foreach (var order in tempOrderList)
            {
                foreach(string line in orderContents)
                {
                    List<Flavour> flavourList = new List<Flavour>();
                    List<Topping> toppingList = new List<Topping>();

                    string[] selectedLine = line.Split(',');
                    if (Convert.ToInt32(selectedLine[0]) == order.Key)
                    {
                        int scoops = Convert.ToInt32(selectedLine[5]);

                        flavourList.Clear();
                        toppingList.Clear();

                        for (int i = 8; i < 8 + scoops; i++)
                        {
                            // Check if the flavour is not empty
                            if (!string.IsNullOrEmpty(selectedLine[i]))
                            {
                                // Check if the flavour already exists in the list
                                Flavour existingFlavour = flavourList.Find(f => f.Type == selectedLine[i]);

                                if (existingFlavour != null)
                                {
                                    // If the flavour exists, increment the quantity
                                    existingFlavour.Quantity++;
                                }
                                else
                                {
                                    // If the flavour does not exist, add a new Flavour object to the list
                                    bool isPremium = (selectedLine[i] != "Vanilla") && (selectedLine[i] != "Chocolate") && (selectedLine[i] != "Strawberry");
                                    flavourList.Add(new Flavour(selectedLine[i], isPremium, 1));
                                }
                            }
                        }

                        // Prepare toppings list
                        for (int i = 11; i < 15; i++)
                        {
                            // Check 
                            if (!string.IsNullOrEmpty(selectedLine[i]))
                            {
                                toppingList.Add(new Topping(selectedLine[i]));
                            }
                        }

                        // Checks if Icecream is cup, cone or waffle 
                        // Adds to order as well 
                        if (selectedLine[4] == "Cup")
                        {
                            order.Value.AddIceCream(new Cup(selectedLine[4], scoops, flavourList, toppingList));
                        }
                        else if (selectedLine[4] == "Cone")
                        {
                            if (selectedLine[6] == "TRUE")
                            {
                                order.Value.AddIceCream(new Cone(selectedLine[4], scoops, flavourList, toppingList, true));
                            }
                            else
                            {
                                order.Value.AddIceCream(new Cone(selectedLine[4], scoops, flavourList, toppingList, false));
                            }
                        }
                        else
                        {
                            if (selectedLine[7] != "Original")
                            {
                                order.Value.AddIceCream(new Waffle(selectedLine[4], scoops, flavourList, toppingList, selectedLine[7]));
                            }
                            else
                            {
                                order.Value.AddIceCream(new Waffle(selectedLine[4], scoops, flavourList, toppingList, "Original"));
                            }
                        }
                    }
                }
            }

            List<Order> orderList = tempOrderList.Values.ToList();

            //Appends orders to respective customer
            foreach (Order order in orderList)
            {
                foreach (var pair in orderCustomerPairs)
                {
                    if (order.Id == pair.Key)
                    {
                        if (order.TimeFulfilled != null)
                        {
                            customerDict[pair.Value].OrderHistory.Add(order);
                        }
                        else
                        {
                            customerDict[pair.Value].CurrentOrder = order;
                        }
                    }
                }
            }

            return customerDict;
        }

        // Completed 
        static void Option1(Dictionary<int, Customer> customerDict)
        {
            // Display the information of all customers 
            Console.WriteLine("{0,-11}{1,-14}{2}", "Name", "Member Id", "DOB");
            foreach (var customer in customerDict)
            {
                Console.WriteLine(customer.Value);
            }
        }

        // Option2 Logic is complete, requires option 4 to be complete.
        static void Option2(Dictionary<int, Customer> customerDict)
        {
            // Display information of all CURRENT orders in both gold and regular queues
            Queue<Customer> goldQueue = new Queue<Customer>();
            Queue<Customer> regularQueue = new Queue<Customer>();

            List<Customer> filteredCustomerList = new List<Customer>();
            
            // Filter customers with current orders 
            foreach (Customer customer in customerDict.Values)
            {
                if (customer.CurrentOrder != null)
                {
                    filteredCustomerList.Add(customer);
                }
            }

            foreach (Customer customer in filteredCustomerList)
            {
                if (customer.Rewards.Tier == "Gold" && customer.CurrentOrder != null)
                {
                    goldQueue.Enqueue(customer);
                }
                else if (customer.CurrentOrder != null)
                {
                    regularQueue.Enqueue(customer);
                    Console.WriteLine("Enqueued!");
                }
            }

            Console.WriteLine("================= GOLD QUEUE =================\n");
            if (goldQueue.Count == 0)
            {
                Console.WriteLine("(There are no current orders)\n");
            }
            else
            {
                foreach (Customer customer in goldQueue)
                {
                    Console.WriteLine(customer.CurrentOrder.ToString());
                    foreach (IceCream iceCream in customer.CurrentOrder.IceCreamList)
                    {
                        Console.WriteLine(iceCream.ToString());
                    }
                }
            }

            Console.WriteLine("=============== REGULAR QUEUE ===============\n");
            if (regularQueue.Count == 0)
            {
                Console.WriteLine("(There are no current orders)");
            }
            else
            {
                foreach (Customer customer in regularQueue)
                {
                    Console.WriteLine(customer.CurrentOrder.ToString());
                    foreach (IceCream iceCream in customer.CurrentOrder.IceCreamList)
                    {
                        Console.WriteLine(iceCream.ToString());
                    }
                }
            }
        }

        // Option3 Customer is not appended to customer.csv 
        static void Option3(Dictionary<int, Customer> customerDict)
        {
            var name = "";
            while (true)
            {
                Console.Write("Enter customer name: ");
                name = Console.ReadLine().Trim();
                bool isString = true;
                foreach (char character in name)
                {
                    if (!char.IsLetter(character))
                    {
                        Console.WriteLine("Please ensure that name entered only consists of alphabets!");
                        isString = false;
                        break;
                    }
                }

                if (isString)
                {
                    break;
                }
            }

            int id = 0;

            // ID data validation. Checks that it must be a 5-digit integer.
            while (true)
            {
                try
                {
                    Console.Write("Enter customer ID: ");
                    id = Convert.ToInt32(Console.ReadLine().Trim());
                    if (Convert.ToString(id).Length != 6)
                    {
                        Console.WriteLine("Input is not 6 digits longs. Please ensure 6 integers are entered.\n");
                    }
                    else if (customerDict.Keys.Contains(id) == true)
                    {
                        Console.WriteLine("Current ID entered clashes with another customer. Please enter another set of 6 digits.\n");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Please enter a 6-digit integer\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("How did you do this?\n");
                }
            }

            // DOB data validation. Checks that it is entered in the correct format.
            DateTime dob = DateTime.Today;
            string format = "dd/MM/yyyy";

            while (true)
            {
                try
                {
                    Console.Write("Enter customer DOB [DD/MM/YYYY]: ");
                    dob = DateTime.ParseExact(Console.ReadLine(), format, null);
                    break;
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Please ensure that your birthday is entered in the following format: [DD/MM/YYYY]\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("How did you do this?\n");
                }

            }

            Customer customer = new Customer(name, id, dob);
            customerDict.Add(id, customer);

            string customerFilePath = "customers.csv";
            try
            {
                using (StreamWriter sw = new StreamWriter(customerFilePath, true))
                {
                    sw.WriteLine($"{customer.Name},{customer.MemberID},{customer.Dob.ToShortDateString()}," +
                        $"{customer.Rewards.Tier},{customer.Rewards.Points},{customer.Rewards.PunchCard}");
                }
                Console.WriteLine("Customer successfully appended to customers.csv");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.Message); 
            }
        }
        
        // Option4 Logic completed. Need hendrik to test test this 
        static void Option4(Dictionary<int, Customer> customerDict)
        {
            Customer selectedCustomer = selectCustomer(customerDict);

            if (selectedCustomer.CurrentOrder == null)
            {
                string[] contents = File.ReadAllLines("orders.csv");

                // Parse the formatted string back to DateTimee
                DateTime formattedDateAndTime = DateTime.ParseExact(DateTime.Today.ToString("dd/MM/yyyy HH:mm"),
                    "dd/MM/yyyy HH:mm", null);

                // Checks last orderID of orders.csv and gives new ID of +1
                selectedCustomer.CurrentOrder = new Order(Convert.ToInt16(contents.Last().Split(',').First()) + 1,
                    formattedDateAndTime);

                IceCream iceCream = makeIceCream();
                selectedCustomer.CurrentOrder.AddIceCream(iceCream);
                Console.WriteLine($"Ice Cream has been succesfully added to {selectedCustomer.Name} current order!");

                string option;
                while (true)
                {
                    Console.Write("Would you like to add another ice cream [y/n]: ");
                    option = Console.ReadLine();
                    if (option == "y")
                    {
                        IceCream newIceCream = makeIceCream();
                        selectedCustomer.CurrentOrder.AddIceCream(newIceCream);
                        Console.WriteLine(
                            $"Ice Cream has been succesfully added to {selectedCustomer.Name} current order!");
                    }
                    else if (option == "n")
                    {
                        Console.WriteLine("Order has been succesfully created!");

                        // Preparing data to append to orders.csv

                        foreach (IceCream item in selectedCustomer.CurrentOrder.IceCreamList)
                        {
                            string finalAppendLine = csvLineFormatter(item, selectedCustomer);

                            using (StreamWriter sw = new StreamWriter("orders.csv", true))
                            {
                                sw.WriteLine(finalAppendLine);
                            }
                        }

                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter 'y' or 'n'.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"{selectedCustomer.Name} already have a current order.");
            }
        }

        // Completed 
        static void Option5(Dictionary<int, Customer> customerDict)
        {
            int counter = 0;
            Customer selectedCustomer = selectCustomer(customerDict);

            // Returns current order
            Console.WriteLine("\nCurrent order\n===================================");
            if (selectedCustomer.CurrentOrder == null)
            {
                Console.WriteLine($"({selectedCustomer.Name}) has no orders at the moment.\n");
            }
            else
            {
                Console.WriteLine(selectedCustomer.CurrentOrder);
            }

            Console.WriteLine("\nPast orders\n===================================\n");
            counter = 1;
            if (selectedCustomer.OrderHistory.Count == 0)
            {
                Console.WriteLine($"({selectedCustomer.Name}) has no orders in history.\n");
            }
            else
            {
                foreach (Order order in selectedCustomer.OrderHistory)
                {
                    Console.WriteLine($"=========\nOrder {counter}\n=========");
                    Console.WriteLine(order.ToString());
                    foreach (IceCream icecream in order.IceCreamList)
                    {
                        Console.WriteLine(icecream.ToString());
                    }
                    counter++;
                    Console.WriteLine();
                }
            }
        }

        // Logic completed. Requires testing with option4 
        static void Option6(Dictionary<int, Customer> customerDict)
        {
            // Method that returns valid customer
            Customer selectedCustomer = selectCustomer(customerDict);

            string[] contents = File.ReadAllLines("orders.csv").Skip(1).ToArray();

            // index is which line of csv.
            List<int> indexLine = new List<int>();
            int indexx = 0;
            foreach (string line in contents)
            {
                string[] parsedLine = line.Split(',');
                //Console.WriteLine(parsedLine[0]);
                if (parsedLine[3] == "" && Convert.ToInt16(parsedLine[0]) == selectedCustomer.CurrentOrder.Id)
                {
                    indexLine.Add(indexx);
                }
                indexx++;
            }

            Console.WriteLine($"{selectedCustomer.Name}'s orders:\n==================================================\n");
            int counter = 0;
            if (selectedCustomer.CurrentOrder == null)
            {
                Console.WriteLine($"({selectedCustomer.Name}) has no current orders.");
            }
            else
            {
                foreach (IceCream iceCream in selectedCustomer.CurrentOrder.IceCreamList)
                {
                    Console.WriteLine($"Ice Cream ({counter+1})\n==============" + iceCream.ToString()); 
                    counter++;
                }
                Console.WriteLine("\n==================== MODIFICATION ====================\n[1] Modify existing Ice Cream\n[2] Add an Ice Cream\n" +
                    "[3] Delete existing Ice Cream\n");
                int option = integerValidator("option", 3);

                // Option for modifying existing ice cream
                if (option == 1)
                {
                    IceCream selectedIceCream;
                    int index = 0;
                    if (counter > 1)
                    {
                        Console.WriteLine("\nWhich ice cream would you like to modify");
                        index = integerValidator("option", counter) - 1;
                    }
                    else
                    {
                        index = 0;
                    }
                    selectedIceCream = selectedCustomer.CurrentOrder.IceCreamList[index];
                    IceCream modifiedIceCream = modifyIceCream(selectedIceCream);
                    selectedCustomer.CurrentOrder.IceCreamList[index] = modifiedIceCream;

                    List<string> updatedContents = new List<string>();
                    using (StreamReader sr = new StreamReader("orders.csv"))
                    {
                        for (int i = 0; i < contents.Length; i++)
                        {
                            string line = sr.ReadLine();
                            if (i == indexLine[index])
                            {
                                updatedContents.Add(csvLineFormatter(modifiedIceCream, selectedCustomer));
                            }
                            else
                            {
                                updatedContents.Add(line);
                            }
                        }
                    }

                    //foreach (var updatedContent in updatedContents)
                    //{
                    //    Console.WriteLine(updatedContent);
                    //}
                }
                else if (option == 2)
                {
                    IceCream iceCream = makeIceCream();
                    selectedCustomer.CurrentOrder.AddIceCream(iceCream);
                    string finalAppendedLine = csvLineFormatter(iceCream, selectedCustomer);

                    List<string> updatedContent = new List<string>();
                    updatedContent = contents.ToList();
                    updatedContent.Add(finalAppendedLine);

                    // To delete
                    foreach (var line in updatedContent)
                    {
                        Console.WriteLine(line);
                    }
                    //File.Append("orders.csv", updatedContent);


                    

                    Console.WriteLine("Ice Cream has been successfully added!");
                }
                else
                {
                    IceCream selectedIceCream;
                    int index = 0;
                    if (counter > 1)
                    {
                        Console.WriteLine("\nWhich ice cream would you like to delete");
                        index = integerValidator("option", counter) - 1;
                        selectedCustomer.CurrentOrder.IceCreamList.RemoveAt(index);

                        List<string> existingLines = contents.ToList();
                        existingLines.RemoveAt(indexLine[index]);

                        // To delete
                        foreach (var line in existingLines)
                        {
                            Console.WriteLine(line);
                        }

                        File.WriteAllLines("orders.csv", existingLines);
                        Console.WriteLine("Ice Cream has been successfully deleted!");
                    }
                    else
                    {
                        Console.WriteLine("Cannot remove ice cream. Orders may not have 0 orders.");
                    }
                }
            }

            
        }

        //Incomplete
        static void Option7(Dictionary<int, Customer> customerDict)
        {
            try
            {
                Queue<Customer> goldQueue = new Queue<Customer>();
                Queue<Customer> regularQueue = new Queue<Customer>();

                string[] contents = File.ReadAllLines("orders.csv");

                List<Customer> filteredCustomerList = new List<Customer>();

                // Filter customers with current orders 
                foreach (Customer customer in customerDict.Values)
                {
                    if (customer.CurrentOrder != null)
                    {
                        filteredCustomerList.Add(customer);
                    }
                }

                foreach (Customer customer in filteredCustomerList)
                {
                    if (customer.Rewards.Tier == "Gold")
                    {
                        goldQueue.Enqueue(customer);
                    }
                    else
                    {
                        regularQueue.Enqueue(customer);
                    }
                }

                Customer dequeuedCustomer = null;
                if (goldQueue.Count > 0)
                {
                    dequeuedCustomer = goldQueue.Dequeue();
                }
                else
                {
                    dequeuedCustomer = regularQueue.Dequeue();
                }

                int index = 0;
                foreach (string line in contents)
                {
                    string[] parsedLine = line.Split(',');
                    if (parsedLine[3] == "" && Convert.ToInt16(parsedLine[0]) == dequeuedCustomer.CurrentOrder.Id)
                    {
                        break;
                    }
                    index++;
                }

                Console.WriteLine("============= DEQUEUED ORDER =============");
                IceCream mostExpensiveIceCream = null;
                double mostCost = 0;
                double totalCost = 0;

                List<IceCream> iceCreamOrders = dequeuedCustomer.CurrentOrder.IceCreamList;

                foreach (IceCream iceCream in iceCreamOrders)
                {
                    Console.WriteLine(iceCream.ToString() + $"\n\nCost: ${iceCream.CalculatePrice().ToString("0.00")}");
                    if (iceCream.CalculatePrice() > mostCost)
                    {
                        mostCost = iceCream.CalculatePrice();
                        mostExpensiveIceCream = iceCream;
                    }
                    totalCost += iceCream.CalculatePrice();
                }

                Console.WriteLine(dequeuedCustomer.Rewards.ToString());
                if (dequeuedCustomer.Dob == DateTime.Today)
                {
                    totalCost -= mostExpensiveIceCream.CalculatePrice();
                }

                if (dequeuedCustomer.Rewards.PunchCard == 10)
                {
                    if (iceCreamOrders[0] != mostExpensiveIceCream)
                    {
                        totalCost -= iceCreamOrders[0].CalculatePrice();
                    }
                }

                if (dequeuedCustomer.Rewards.Tier != "Ordinary")
                {
                    Console.WriteLine($"\nYou have {dequeuedCustomer.Rewards.Points} points. How many would you like to use?");
                    int pointsUsed = integerValidator("number of points", dequeuedCustomer.Rewards.Points);
                    double costOffset = pointsUsed * 0.02;
                    totalCost -= costOffset;
                }

                Console.WriteLine($"Final Cost: ${totalCost.ToString("0.00")}");

                Console.Write("Press any key to proceed");
                Console.ReadLine();

                dequeuedCustomer.Rewards.Punch();

                // Need to do Math.Floor()
                dequeuedCustomer.Rewards.AddPoints(Convert.ToInt32((int)Math.Ceiling(totalCost * 0.72)));

                string format = "dd/MM/yyyy HH:mm";
                string dateFulfilled = Convert.ToString(DateTime.Now);
                dequeuedCustomer.CurrentOrder.TimeFulfilled = DateTime.Parse(dateFulfilled);

                List<string> existingLines = contents.ToList();
                existingLines.RemoveAt(index);
                File.WriteAllLines("orders.csv", existingLines);

                dequeuedCustomer.OrderHistory.Add(dequeuedCustomer.CurrentOrder);
                dequeuedCustomer.CurrentOrder = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Queue empty. no orders.");
            }
        }

        // Incomplete
        static void Option8(Dictionary<int, Customer> customerDict)
        {
            int inputtedYear = 0;
            List<String> months = new List<String>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Oct", "Nov", "Dec" };
            // Validate year
            while (true)
            {
                try
                {
                    Console.Write("Enter the year: ");
                    inputtedYear = Convert.ToInt32(Console.ReadLine());
                    if (inputtedYear.ToString().Length != 4) 
                    {
                        Console.WriteLine("Please enter a year that has 4 digits");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            double monthlyTotal = 0;
            double grandTotal = 0;

            List<double> monthlyTotalList = new List<double>();

            foreach (string month in months)
            {
                monthlyTotal = 0;
                int monthlyIndex = months.IndexOf(month);
                foreach (Customer customer in customerDict.Values)
                {
                    foreach (Order order in customer.OrderHistory)
                    {
                        if (order.TimeReceived.Month == monthlyIndex && order.TimeReceived.Year == inputtedYear)
                        foreach(IceCream iceCream in order.IceCreamList)
                        {
                            monthlyTotal += iceCream.CalculatePrice();
                        }
                    }
                }
                monthlyTotalList.Add(monthlyTotal);
                grandTotal += monthlyTotal;
            }

            int counter = 0;
            foreach (double total in monthlyTotalList)
            {
                Console.WriteLine($"{months[counter]} {inputtedYear}:  ${total.ToString("0.00")}");
                counter++;
            }
            Console.WriteLine($"\nTotal:     ${grandTotal.ToString("0.00")}");
        }
        
        // Method that validates and returns customer
        static Customer selectCustomer(Dictionary<int, Customer> customerDict)
        {
            Console.WriteLine("List of customers\n========================================");
            int counter = 1;
            foreach (Customer customer in customerDict.Values)
            {
                Console.WriteLine($"[{counter}] {customer.Name}");
                counter++;
            }

            // Initialises list
            List<string> allCustomerNames = new List<string>();
            foreach (Customer customer in customerDict.Values)
            {
                allCustomerNames.Add(customer.Name.ToLower());
            }

            //Validates that customer name entered is under the list
            Customer selectedCustomer = null;
            while (true)
            {
                Console.Write("Please enter customer: ");
                string option = Console.ReadLine().Trim().ToLower();
                if (allCustomerNames.Contains(option) == false)
                {
                    Console.WriteLine("Please enter a name that is in the list\n");
                }
                else
                {
                    foreach (Customer customer in customerDict.Values)
                    {
                        if (customer.Name == option.Substring(0, 1).ToUpper() + option.Substring(1))
                        {
                            selectedCustomer = customer;
                        }
                    }
                    break;
                }
            }
            return selectedCustomer;
        }

        // Method that validates integers, limit represents choices from 1 to limit. 
        // Prompt to cater to different inputs
        static int integerValidator(string prompt, int limit)
        {
            int option = 0;
            while (true)
            {
                try
                {
                    Console.Write($"Enter {prompt}: ");
                    option = Convert.ToInt32(Console.ReadLine());
                    if (option < 1 || option > limit)
                    {
                        Console.WriteLine($"Please enter an option between 1-{limit}\n");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Please enter an option between 1-{limit}\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Please enter an option between 1-{limit}\n");
                }
            }
            return option;
        }

        static List<Flavour> choosingFlavours(int scoops)
        {
            List<string> flavours = new List<string>() { "Vanilla", "Chocolate", "Strawberry", "Durian", "Ube", "Sea salt" };
            List<Flavour> userFlavourList = new List<Flavour>();
            int counter;
            int inputtedFlavours = 0;
            while (inputtedFlavours != scoops)
            {
                counter = 1;
                Console.WriteLine();
                Console.WriteLine("Flavour Options");
                foreach (var flavour in flavours)
                {
                    Console.WriteLine($"[{counter++}] {flavour}");
                }
                //Choose flavour 
                int userFlavour = integerValidator("flavour number from 1-6", 6);

                //Number of selected flavour 
                var userFlavourNo = 0;

                //Check that inputted flavours do not exceed number of scoops 
                while (true)
                {
                    //Validate input type
                    userFlavourNo = integerValidator($"No. of {flavours[userFlavour - 1]}", 10);

                    //Validate if inputted number is within range
                    if (userFlavourNo == 0)
                    {
                        Console.WriteLine("Number of flavour cannot be 0.");
                    }
                    else if ((inputtedFlavours + userFlavourNo) > scoops)
                    {
                        Console.WriteLine($"Exceeded number of flavours, please make sure its less than {scoops}");
                    }
                    else
                    {
                        inputtedFlavours += userFlavourNo;
                        //Apending flavour to flavourList 
                        bool isPremium = userFlavour > 3;
                        userFlavourList.Add(new Flavour(flavours[userFlavour - 1], isPremium, userFlavourNo));
                        break;
                    }
                }
            }
            return userFlavourList;
        }

        static List<Topping> choosingToppings()
        {
            List<string> toppings = new List<string>() { "Sprinkles", "Mochi", "Sago", "Oreos" };
            List<Topping> userToppingList = new List<Topping>();
            string wantTopping;
            for (int m = 1; m <= 4; m++)
            {
                //Display topping options 
                Console.WriteLine();
                Console.WriteLine("Choose Topping");
                Console.WriteLine($"[1] {toppings[0]}");
                Console.WriteLine($"[2] {toppings[1]}");
                Console.WriteLine($"[3] {toppings[2]}");
                Console.WriteLine($"[4] {toppings[3]}");

                //Validation for choosing of topping
                while (true)
                {
                    var userTopping = integerValidator("topping", 4);
                    userToppingList.Add(new Topping(toppings[userTopping - 1]));

                    // Validate whether user want another topping
                    while (true)
                    {
                        Console.Write("Do you want another topping[y/n]: ");
                        wantTopping = Console.ReadLine().Trim().ToLower();
                        if (wantTopping != "y" && wantTopping != "n")
                        {
                            Console.WriteLine("Please enter either a 'y' or 'n'");
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (wantTopping == "n")
                    {
                        break;
                    }
                }
                if (wantTopping == "n")
                {
                    break;
                }
            }
            return userToppingList;
        }

        static IceCream modifyIceCream(IceCream modifyIceCream)
        {
            string optionsFormat = "\nWhat would you like to modify?\n[1] Option\n[2] No. of scoops\n[3] Flavour\n[4] Topping";
            int selectedOption = 0;
            if (modifyIceCream.Option == "Cup")
            {
                Console.WriteLine(optionsFormat);
                selectedOption = integerValidator("option", 4);
            }
            else if (modifyIceCream.Option == "Cone")
            {
                Console.WriteLine(optionsFormat + "\n[5] Dipped in Chocolate");
                selectedOption = integerValidator("option", 5);
            }
            else
            {
                Console.WriteLine(optionsFormat + "\n[5] Waffle flavour");
                selectedOption = integerValidator("option", 5);
            }

            // Modifying No. of scoops
            if (selectedOption == 1)
            {
                List<string> options = new List<string>() { "Cup", "Cone", "Waffle", "Exit"};

                Console.WriteLine("Select Ice Cream Option");
                int i = 1;
                foreach (var option in options)
                {
                    Console.WriteLine($"[{i++}] {option}");
                }

                // Validate user choosing of option
                string differentOption;
                while (true)
                {
                    int optionIndex = integerValidator("option", 4);
                    if (modifyIceCream.Option == options[optionIndex - 1])
                    {
                        Console.WriteLine("You selected the same option.\n Exit if you do not want to change.\n");
                    }
                    else
                    {
                        differentOption = options[optionIndex - 1];
                        break;
                    }
                }

                if (differentOption == "Cup" && (modifyIceCream.Option == "Cone" || modifyIceCream.Option == "Waffle"))
                {
                    modifyIceCream = new Cup("Cup", modifyIceCream.Scoops, modifyIceCream.Flavours, modifyIceCream.Toppings);
                    Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
                }
                else if ((modifyIceCream.Option == "Cone" || modifyIceCream.Option == "Cup") && differentOption == "Waffle")
                {
                    modifyIceCream = makeWaffle(modifyIceCream.Scoops, modifyIceCream.Flavours, modifyIceCream.Toppings);
                    Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
                }
                else if ((modifyIceCream.Option == "Waffle" || modifyIceCream.Option == "Cup") && differentOption == "Cone")
                {
                    modifyIceCream = makeCone(modifyIceCream.Scoops, modifyIceCream.Flavours, modifyIceCream.Toppings);
                    Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
                }


            }
            else if (selectedOption == 2)
            {
                modifyIceCream.Scoops = integerValidator("no. of scoops (1-3)", 3);
                modifyIceCream.Flavours = choosingFlavours(modifyIceCream.Scoops);
                Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
            }
            else if (selectedOption == 3)
            {
                modifyIceCream.Flavours = choosingFlavours(modifyIceCream.Scoops);
                Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
            }
            else if (selectedOption == 4)
            {
                modifyIceCream.Toppings = choosingToppings();
                Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
            }
            else if (selectedOption == 5 && modifyIceCream.Option == "Cone")
            {
                modifyIceCream = makeCone(modifyIceCream.Scoops, modifyIceCream.Flavours, modifyIceCream.Toppings);
                Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
            }
            else if (selectedOption == 5 && modifyIceCream.Option == "Waffle")
            {
                modifyIceCream = makeWaffle(modifyIceCream.Scoops, modifyIceCream.Flavours, modifyIceCream.Toppings);
                Console.WriteLine($"\n{modifyIceCream.Option} has been successfully modified!\n{modifyIceCream.ToString()}");
            }

            return modifyIceCream;
        }

        static Cone makeCone(int scoops, List<Flavour> userFlavourList, List<Topping> userToppingList)
        {
            string option;
            Cone cone = null;
            while (true)
            {
                Console.Write("Would you like your cone to be dipped in chocolate [y/n]: ");
                option = Console.ReadLine();
                if (option != "y" && option != "n")
                {
                    Console.WriteLine("Please enter either a 'y' or 'n'.\n");
                }
                else
                {
                    break;
                }
            }
            if (option == "y")
            {
                cone = new Cone("Cone", scoops, userFlavourList, userToppingList, true);
            }
            else
            {
                cone = new Cone("Cone", scoops, userFlavourList, userToppingList, false);
            }
            return cone;
        }

        static Waffle makeWaffle(int scoops, List<Flavour> userFlavourList, List<Topping> userToppingList)
        {
            List<string> specialFlavour = new List<string>() { "Red velvet", "Charcoal", "Pandan Waffle" };
            Waffle waffle = null;

            string option;
            int counter = 1;
            while (true)
            {
                Console.Write("Would you like your waffle to be a special flavour [y/n]: ");
                option = Console.ReadLine();
                if (option != "y" && option != "n")
                {
                    Console.WriteLine("Please enter either a 'y' or 'n'.\n");
                }
                else
                {
                    break;
                }
            }
            if (option == "y")
            {
                foreach (string flavour in specialFlavour)
                {
                    Console.WriteLine($"[{counter}] {flavour}");
                    counter++;
                }

                // Data validation for special flavour
                int specialFlavourOption = integerValidator("special flavour number", 3);

                waffle = new Waffle("Waffle", scoops, userFlavourList, userToppingList, specialFlavour[specialFlavourOption - 1]);
            }
            else
            {
                waffle = new Waffle("Waffle", scoops, userFlavourList, userToppingList, "Original");
            }
            return waffle;
        }

        static IceCream makeIceCream()
        {
            List<string> options = new List<string>() { "Cup", "Cone", "Waffle" };
            IceCream icecream = null;

            Console.WriteLine();
            Console.WriteLine("Select Ice Cream Option");
            int i = 1;
            foreach (var option in options)
            {
                Console.WriteLine($"[{i++}] {option}");
            }
            //Choose Cup, Cone or Waffle.
            int selectedOption = integerValidator("option", 3);

            //Scoops, flavour and topping

            //Initialise flavour and topping Lists
            List<Flavour> userFlavourList = new List<Flavour>();
            List<Topping> userToppingList = new List<Topping>();

            //Choose no. of scoops 
            var scoops = integerValidator("No. of scoops from 1-3", 3);

            //List containing flavours chosen by user 
            userFlavourList = choosingFlavours(scoops);

            Console.WriteLine();
            //Ask if user want topping 
            var wantTopping = "";

            // Validate whether user wants toppings
            while (true)
            {
                Console.Write("Do you want toppings[y/n]: ");
                wantTopping = Console.ReadLine().Trim().ToLower();
                if (wantTopping == "y" || wantTopping == "n")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter either a 'y' or 'n'");
                }
            }

            // User wants topping 
            if (wantTopping == "y")
            {
                userToppingList = choosingToppings();
            }

            //If Cup is chosen 
            if (selectedOption == 1)
            {
                return icecream = new Cup("Cup", scoops, userFlavourList, userToppingList);
            }
            //If Cone is chosen
            else if (selectedOption == 2)
            {
                return icecream = makeCone(scoops, userFlavourList, userToppingList);
            }
            //If Waffle is chosen
            else
            {
                return icecream = makeWaffle(scoops, userFlavourList, userToppingList);
            }
        }

        static string csvLineFormatter(IceCream item, Customer selectedCustomer)
        {
            string flavourString = "";
            string toppingString = "";
            int counter = 0;

            // Prepare flavour string
            foreach (Flavour flavour in item.Flavours)
            {
                for (int f = 0; f < flavour.Quantity; f++)
                {
                    flavourString += Convert.ToString(flavour.Type) + $",";
                    counter++;
                }
            }
            for (int i = 0; i < 3 - counter; i++)
            {
                flavourString += ",";
            }
            //Console.WriteLine(flavourString);

            // Prepare topping string 
            counter = 1;
            foreach (Topping topping in item.Toppings)
            {
                toppingString += Convert.ToString(topping.Type) + $",";
                counter++;
            }
            for (int i = 0; i < 4 - counter; i++)
            {
                toppingString += ",";
            }
            //Console.WriteLine(toppingString);
            Console.WriteLine(selectedCustomer.CurrentOrder.TimeFulfilled);

            string finalAppendLine = "";
            string appendLineFormat = $"{selectedCustomer.CurrentOrder.Id},{selectedCustomer.MemberID},{selectedCustomer.CurrentOrder.TimeReceived}," +
                $"{selectedCustomer.CurrentOrder.TimeFulfilled},{item.Option},{item.Scoops},";

            if (item is Cone coneItem)
            {
                return finalAppendLine += appendLineFormat += $"{Convert.ToString(coneItem.Dipped).ToUpper()},,{flavourString}{toppingString}";
            }
            else if (item is Cup cupItem)
            {
                return finalAppendLine += appendLineFormat += $",,{flavourString}{toppingString}";
            }
            else if (item is Waffle waffleItem)
            {
                return finalAppendLine += appendLineFormat += $",{waffleItem.WaffleFlavour},{flavourString}{toppingString}";
            }
            else
            {
                return "";
            }
        }
    }
}