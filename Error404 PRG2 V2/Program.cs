using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        static void MenuInterface()
        {
            Console.Write("================= MENU INTERFACE =================\n[1] List all customers\n[2] List all current orders\n[3] Register" +
                " a new customer\n[4] Create a customer's order\n[5] Display order details of a customer\n[6] Modify order details\n" +
                "[0] Exit\n==================================================\nEnter option: ");
        }

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
                    Order order = new Order(Convert.ToInt32(selectedLine[0]), DateTime.ParseExact(selectedLine[2], format, null));
                    order.TimeFulfilled = DateTime.ParseExact(selectedLine[3], format, null);
                    tempOrderList.Add(Convert.ToInt32(selectedLine[0]), order) ;
                }
                // Checks for duplicates after first order is added
                else
                {
                    if (tempOrderList.Keys.Contains(Convert.ToInt32(selectedLine[0])) == false)
                    {
                        Order order1 = new Order(Convert.ToInt32(selectedLine[0]), DateTime.ParseExact(selectedLine[2], format, null));
                        order1.TimeFulfilled = DateTime.ParseExact(selectedLine[3], format, null);
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
                            if (selectedLine[7] != null)
                            {
                                order.Value.AddIceCream(new Waffle(selectedLine[4], scoops, flavourList, toppingList, selectedLine[7]));
                            }
                            else
                            {
                                order.Value.AddIceCream(new Waffle(selectedLine[4], scoops, flavourList, toppingList, null));
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
                        customerDict[pair.Value].OrderHistory.Add(order);
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

        // Option2 Incomplete, requires option 4 to be complete.
        static void Option2(Dictionary<int, Customer> customerDict)
        {
            // Display information of all CURRENT orders in both gold and regular queues
            Queue<string> goldQueue = new Queue<string>();
            Queue<string> regularQueue = new Queue<string>();



            //using (StreamReader sr = new StreamReader("orders.csv"))
            //{
            //    string header = sr.ReadLine();
            //    string line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        string[] parts = line.Split(',');
            //        if (int.TryParse(parts[1], out int memberId))
            //        {
            //            if (customerDict.ContainsKey(memberId))
            //            {
            //                Customer customer = customerDict[memberId];
            //                if (customer.Rewards.Tier == "Gold")
            //                {
            //                    goldQueue.Enqueue(line);
            //                }
            //                else
            //                {
            //                    regularQueue.Enqueue(line);
            //                }
            //            }
            //        }
            //    }
            //}

            //Console.WriteLine("Gold Member's Queue");
            //Console.WriteLine("{0,-3}{1,-10}{2,-18}{3,-18}{4,-8}{5,-8}{6,-8}{7,-15}{8,-12}{9,-12}{10,-12}{11,-12}{12,-12}{13,-12}{14}"
            //        , "Id", "MemberId", "Time received", "Time fulfilled", "Option", "Scoops", "Dipped",
            //        "WaffleFlavour", "Flavour1", "Flavour2", "Flavour3", "Topping1", "Topping2", "Topping3", "Topping4");
            //foreach (var order in goldQueue)
            //{
            //    string[] contents = order.Split(',');
            //    Console.WriteLine("{0,-3}{1,-10}{2,-18}{3,-18}{4,-8}{5,-8}{6,-8}{7,-15}{8,-12}{9,-12}{10,-12}{11,-12}{12,-12}{13,-12}{14}",
            //        contents[0], contents[1], contents[2], contents[3], contents[4], contents[5], contents[6], contents[7], contents[8], contents[9],
            //        contents[10], contents[11], contents[12], contents[13], contents[14]);
            //}

            //Console.WriteLine();
            //Console.WriteLine("Regular Member's Queue");
            //Console.WriteLine("{0,-3}{1,-10}{2,-18}{3,-18}{4,-8}{5,-8}{6,-8}{7,-15}{8,-12}{9,-12}{10,-12}{11,-12}{12,-12}{13,-12}{14}"
            //    , "Id", "MemberId", "Time received", "Time fulfilled", "Option", "Scoops", "Dipped",
            //    "WaffleFlavour", "Flavour1", "Flavour2", "Flavour3", "Topping1", "Topping2", "Topping3", "Topping4");
            //foreach (var order in regularQueue)
            //{
            //    string[] contents = order.Split(',');
            //    Console.WriteLine("{0,-3}{1,-10}{2,-18}{3,-18}{4,-8}{5,-8}{6,-8}{7,-15}{8,-12}{9,-12}{10,-12}{11,-12}{12,-12}{13,-12}{14}",
            //        contents[0], contents[1], contents[2], contents[3], contents[4], contents[5], contents[6], contents[7], contents[8], contents[9],
            //        contents[10], contents[11], contents[12], contents[13], contents[14]);
            //}
        }

        // Option3 Customer is not appended to customer.csv 
        static void Option3(Dictionary<int, Customer> customerDict)
        {
            Console.Write("Enter customer name: ");
            var name = Console.ReadLine().Trim();

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
            //customer.Rewards.Tier = "Ordinary"; 
            customerDict.Add(id, customer);

            string customerFilePath = "customers.csv";
            try
            {
                using (StreamWriter sw = new StreamWriter(customerFilePath, true))
                {
                    sw.WriteLine($"{customer.Name},{customer.MemberID},{customer.Dob.ToShortDateString()}," +
                        $"{customer.Rewards.Tier},{customer.Rewards.Points},{customer.Rewards.PunchCard}\n");
                }
                Console.WriteLine("Customer successfully appended to customers.csv");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message); 
            }
        }
        
        // Option4 Incomplete. Order not appended to orders.csv
        static void Option4(Dictionary<int, Customer> customerDict)
        {
            List<string> options = new List<string>() { "Cup", "Cone", "Waffle" };
            List<string> flavours = new List<string>() { "Vanilla", "Chocolate", "Strawberry", "Durian", "Ube", "Sea salt" };
            List<string> toppings = new List<string>() { "Sprinkles", "Mochi", "Sago", "Oreos" };
            List<string> specialFlavour = new List<string>() { "Red velvet", "Charcoal", "Pandan Waffle" };
            List<string> customerOptions = new List<string>();
            
            // Method that returns valid customer
            Customer selectedCustomer = selectCustomer(customerDict);

            Order order = new Order(selectedCustomer.MemberID, DateTime.Now);

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
                        var userTopping = integerValidator("Enter topping", 4);
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
            }

            //If Cup is chosen 
            if (selectedOption == 1)
            {
                IceCream cup = new Cup("Cup", scoops, userFlavourList, userToppingList);
                Console.WriteLine(cup.ToString());
            }
            //If Cone is chosen
            else if (selectedOption == 2)
            {
                // Data valiation whether user wants cone dipped in chocolate
                string option;
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
                    IceCream cone = new Cone("Cone", scoops, userFlavourList, userToppingList, true);
                }
                else
                {
                    IceCream cone = new Cone("Cone", scoops, userFlavourList, userToppingList, false);
                }
            }
            //If Waffle is chosen
            else if (selectedOption == 3)
            {
                string option;
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
                        counter = 1;
                        Console.WriteLine($"[{counter}] {flavour}");
                    }

                    // Data validation for special flavour
                    int specialFlavourOption = integerValidator("special flavour number", 3);

                    IceCream waffle = new Waffle("Waffle", scoops, userFlavourList, userToppingList, specialFlavour[specialFlavourOption - 1]);
                }
                else
                {
                    IceCream waffle = new Waffle("Waffle", scoops, userFlavourList, userToppingList, "Original");
                }
            }
        }

        // Completed 
        static void Option5(Dictionary<int, Customer> customerDict)
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
                allCustomerNames.Add(customer.Name);
            }

            //Validates that customer name entered is under the list
            Customer selectedCustomer = null;
            while (true)
            {
                Console.Write("Please enter customer: ");
                string customerName = Console.ReadLine();
                if (allCustomerNames.Contains(customerName) == false)
                {
                    Console.WriteLine("Please enter a name that is in the list\n");
                }
                else
                {
                    foreach (Customer customer in customerDict.Values)
                    {
                        if (customer.Name == customerName)
                        {
                            selectedCustomer = customer;
                        }
                    }
                    break;
                }
            }

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

        // Option 6 incomplete, require option 4 to begin with.
        static void Option6(Dictionary<int, Customer> customerDict)
        {
            // Method that returns valid customer
            //Customer selectedCustomer = selectCustomer(customerDict);

            //Test case
            string format = "dd/MM/yyyy HH:mm";
            string date = "20/01/2024 17:07";
            List<Flavour> cupFlavourList= new List<Flavour>() { new Flavour("Vanilla", false, 1) , new Flavour("Chocolate", false, 1) };
            List<Topping> cupToppingList = new List<Topping>() { new Topping("Sprinkles"), new Topping("Mochi") };
            List<Flavour> coneFlavourList = new List<Flavour>() { new Flavour("Vanilla", false, 2), new Flavour("Sea salt", true, 1)};
            List<Topping> coneToppingList = new List<Topping>() { new Topping("Oreos"), new Topping("Sago")};

            Customer testSubject = new Customer("Test Subject", 111111, new DateTime(1111,1,1));
            testSubject.CurrentOrder = new Order(6, DateTime.ParseExact(date, format, null));
            testSubject.CurrentOrder.AddIceCream(new Cup("Cup", 2, cupFlavourList, cupToppingList));
            testSubject.CurrentOrder.AddIceCream(new Cone("Cone", 2, coneFlavourList, coneToppingList, true));

            Customer selectedCustomer = testSubject;

            // Listing of selected customer's order(s)
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
            }

            Console.WriteLine("\n==================== MODIFICATION ====================\n[1] Modify existing Ice Cream\n[2] Add an Ice Cream\n" +
                "[3] Delete existing Ice Cream\n");
            int option = integerValidator("option", 3);

            // Option for modifying existing ice cream
            if (option == 1)
            {
                // Selects ice cream to modify
                if (counter > 1)
                {
                    Console.WriteLine("Which ice cream would you like to modify");
                    int index = integerValidator("option", counter) - 1;
                    IceCream modifyIceCream = selectedCustomer.CurrentOrder.IceCreamList[index];
                }
                else
                {
                    IceCream modifyIceCream = selectedCustomer.CurrentOrder.IceCreamList[0];
                }
                //Console.WriteLine("What would you like to modify?\n[1] Option\n[2] No. of scoops\n[3] Flavour\n[4] Topping");
            }
            
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
                allCustomerNames.Add(customer.Name);
            }

            //Validates that customer name entered is under the list
            Customer selectedCustomer = null;
            while (true)
            {
                Console.Write("Please enter customer: ");
                string option = Console.ReadLine();
                if (allCustomerNames.Contains(option) == false)
                {
                    Console.WriteLine("Please enter a name that is in the list\n");
                }
                else
                {
                    foreach (Customer customer in customerDict.Values)
                    {
                        if (customer.Name == option)
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
        // Work in progress
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
    }
}
