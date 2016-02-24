using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refactoring
{
    public class Tusc
    {
        const int FIVETIMEUSERCHECK = 5;
        const int SEVENPRODUCTCHECK = 7;
        static string _userName;
        static string _passWord;
        static List<User> _users;
        static List<Product> _products;

        public static void Start(List<User> usrs, List<Product> prods)
        {
            _users = usrs;
            _products = prods;

            // Write welcome message
            WelcomeMessage();

            // Login
            Login:
            PromptUser();


           // if valid user
           if (IsValidUser())
           {
               if (IsValidPassword())
               {
                   // Show welcome message
                   ShowLoginMessage();
                   
                   double bal = ShowBalance();
                   // Show product list
                   while (true)
                   {
                      // Prompt for user input
                       ShowTwoLines(string.Empty, "What would you like to buy?");

                      for (int i = 0; i < SEVENPRODUCTCHECK; i++)
                            {
                                Product prod = prods[i];
                                Console.WriteLine(i + 1 + ": " + prod.Name + " (" + prod.Price.ToString("C") + ")");
                            }
                            Console.WriteLine(prods.Count + 1 + ": Exit");

                            // Prompt for user input
                            Console.WriteLine("Enter a number:");
                            string answer = Console.ReadLine();
                            int num = Convert.ToInt32(answer);
                            num = num - 1; /* Subtract 1 from number
                            num = num + 1 // Add 1 to number */

                            // Check if user entered number that equals product count
                            if (num == SEVENPRODUCTCHECK)
                            {
                                // Update balance
                                foreach (var usr in usrs)
                                {
                                    // Check that name and password match
                                    if (usr.Name == _userName && usr.Pwd == _passWord)
                                    {
                                        usr.Bal = bal;
                                    }
                                }

                                // Write out new balance
                                string json = JsonConvert.SerializeObject(usrs, Formatting.Indented);
                                File.WriteAllText(@"Data/Users.json", json);

                                // Write out new quantities
                                string json2 = JsonConvert.SerializeObject(prods, Formatting.Indented);
                                File.WriteAllText(@"Data/Products.json", json2);


                                // Prevent console from closing
                                PreventClose();
                                return;
                            }
                            else
                            {
                                ShowTwoLines(string.Empty, "You want to buy: " + prods[num].Name);
                                ShowTwoLines("Your balance is " + bal.ToString("C"), "Enter amount to purchase:");

                                // Prompt for user input
                                answer = Console.ReadLine();
                                int qty = Convert.ToInt32(answer);

                                // Check if balance - quantity * price is less than 0
                                if (bal - prods[num].Price * qty < 0)
                                {
                                    ChangeColorAndMsg(ConsoleColor.Red, string.Empty, "You do not have enough money to buy that.");
                                    continue;
                                }

                                // Check if quantity is less than quantity
                                if (prods[num].Qty <= qty)
                                {
                                    ChangeColorAndMsg(ConsoleColor.Red, string.Empty,"Sorry, " + prods[num].Name + " is out of stock");
                                    continue;
                                }

                                // Check if quantity is greater than zero
                                if (qty > 0)
                                {
                                    // Balance = Balance - Price * Quantity
                                    bal = bal - prods[num].Price * qty;

                                    // Quanity = Quantity - Quantity
                                    prods[num].Qty = prods[num].Qty - qty;
                                    ChangeColorAndMsg(ConsoleColor.Green, "You bought " + qty + " " + prods[num].Name, "Your new balance is " + bal.ToString("C"));
                                }
                                else
                                {
                                    // Quantity is less than zero
                                    QuantityLessZero();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Invalid Password
                        InValidPwd();
                        goto Login;
                    }
                }
                else
                {
                    // Invalid User
                    InValidUser();
                    goto Login;
                }
            // Prevent console from closing
           // PreventClose();
        }

        private static void ShowTwoLines(string firstMsg, string secondMsg)
        {
            Console.WriteLine(firstMsg);
            Console.WriteLine(secondMsg);
        }
        private static void WelcomeMessage()
        {
            ShowTwoLines("Welcome to TUSC", "---------------");
        }
        private static void ShowLoginMessage()
        {
            ChangeColorAndMsg(ConsoleColor.Green, string.Empty, "Login successful! Welcome " + _userName + "!");
        }
        private static string PromptUser()
        {
            // Prompt for user input
            string name = string.Empty;
            Console.WriteLine();
            Console.WriteLine("Enter Username:");
            name = Console.ReadLine();
            while (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Enter Username:");
                name = Console.ReadLine();
            }
            return name;

        }
        public static bool IsValidUser()
        {
            bool ValidatedUser = false; // Is valid user?
            _userName = PromptUser();
            
            for (int i = 0; i < FIVETIMEUSERCHECK; i++)
                {
                    User user = _users[i];
                    // Check that name matches
                    if (user.Name == _userName)
                    {
                        ValidatedUser = true;
                    }
                }
            return ValidatedUser;
        }
        private static void ChangeColorAndMsg(ConsoleColor color, string firstMsg, string secondmsg)
        {
            Console.Clear();
            Console.ForegroundColor = color;
            Console.WriteLine(firstMsg);
            Console.WriteLine(secondmsg);
            Console.ResetColor();
        }
        private static void InValidUser()
        {
            ChangeColorAndMsg(ConsoleColor.Red,string.Empty, "You entered an invalid user.");
        }
        private static void InValidPwd()
        {
            ChangeColorAndMsg(ConsoleColor.Red, string.Empty, "You entered an invalid password.");
        }
        private static bool IsValidPassword()
        {
            Console.WriteLine("Enter Password:");
            _passWord = Console.ReadLine();

            // Validate Password
            bool valPwd = false; // Is valid password?
            for (int i = 0; i < FIVETIMEUSERCHECK; i++)
            {
                User user = _users[i];

                //password match
                if (user.Name == _userName && user.Pwd == _passWord)
                {
                    valPwd = true;
                }
            }
            return valPwd;
        }
        private static double ShowBalance()
        {
            double bal = 0;
            for (int i = 0; i < FIVETIMEUSERCHECK; i++)
            {
                User usr = _users[i];

                // Check that name and password match
                if (usr.Name == _userName && usr.Pwd == _passWord)
                {
                    bal = usr.Bal;

                    // Show balance 
                    Console.WriteLine();
                    Console.WriteLine("Your balance is " + usr.Bal.ToString("C"));
                }
            }
            return bal;
        }
        private static void PreventClose()
        {
            Console.WriteLine();
            Console.WriteLine("Press Enter key to exit");
            Console.ReadLine();
        }
        private static void  QuantityLessZero()
        {
            ChangeColorAndMsg(ConsoleColor.Yellow, string.Empty,"Purchase cancelled");
        }
    }
}
