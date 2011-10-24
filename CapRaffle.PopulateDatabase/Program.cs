using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapRaffle.PopulateDatabase
{
    class Program
    {
        static CapRaffleContext repository;
        static Random random = new Random();

        static void Main(string[] args)
        {
            repository = new CapRaffleContext();
            //Console.WriteLine("Please type number of events you want to populate");
            string inputnumber = "1";
            string inputname = "test";
             var number = 0;
             if (args.Count() > 0)
             {
                 if (args[0].Equals("u"))
                 {
                     inputnumber = args[1];
                     if (int.TryParse(inputnumber, out number))
                     {
                         SaveUsers(number, args[2]);
                         repository.SaveChanges();

                     }
                 }
                 else if (args[0].Equals("e"))
                 {
                     inputnumber = args[1]; //Console.ReadLine();
                     inputname = args[2] ?? "test";
                     if (int.TryParse(inputnumber, out number))
                     {
                         for (int num = 0; num < number; num++)
                         {
                             var name = inputname + " " + num;
                             SaveEvents(num, name);
                             repository.SaveChanges();
                             SaveParticipants(name);
                             repository.SaveChanges();
                         }
                     }
                 }
                 else
                 {
                     Console.Write("nothing to populate");
                     return;
                 }
             }
             else
             {
                 Console.Write("You have to set argumets in order to run this program");
                 Console.ReadKey();
                 return;
             }
            Console.Write("Done populating database");
            Console.ReadKey();
        }

        private static void SaveUsers(int num, string email)
        {
            for (int i = 0; i < num; i++)
            {
                var user = new User
                {
                    Name = i + " " + GetNameFromEmail(email) ,
                    Email = i + " " + email,
                    Password = "testesen"
                };
                repository.AddToUsers(user);
            }
        }

        private static string GetNameFromEmail(string email)
        {
            var name = ".";
            if (email.Contains(".") || email.Contains("-") || email.Contains("."))
            {
                email = email.Replace("capgemini.com", "");
                email = email.Replace("-", " ");
                email = email.Replace(".", " ");
                email = email.Substring(0, email.Length - 1);
                name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(email.ToLower());
            }
            return name;
        }

        private static void SaveParticipants(string name)
        {
            var selectedEvent = repository.Events.Where(x => x.Name.Equals(name)).FirstOrDefault();
            if (selectedEvent != null)
            {
                var numberOfUsers = repository.Users.Count();
                var numberOfParticipants = random.Next(1, numberOfUsers);
                var users = repository.Users.ToList();
                for (int i = 0; i < numberOfParticipants; i++)
                {
                    int index = random.Next(0, users.Count());
                    var useremail = users.ElementAt(index).Email;
                    users.RemoveAt(index);
                    var participant = new UserEvent
                    {
                        EventId = selectedEvent.EventId,
                        UserEmail = useremail,
                        NumberOfSpots = random.Next(1, selectedEvent.AvailableSpots)
                    };
                    repository.AddToUserEvents(participant);
                }
            }
        }

        private static void SaveEvents(int num, string name)
        {
            var time = DateTime.Now.AddSeconds(num + 1);
            var categoryId = GetRandomCategoryId();
            var newEvent = new Event
            {
                Name = name,
                CategoryId = categoryId,
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam",
                AvailableSpots = random.Next(1, 10),
                InformationUrl = "Lorem ipsum is fun",
                StartTime = time,
                DeadLine = time,
                IsAutomaticDrawing = true,
                Created = DateTime.Now,
                Creator = repository.Users.FirstOrDefault().Email
            };
            repository.AddToEvents(newEvent);
        }

        public static int GetRandomCategoryId()
        {
            var numberofCategories = repository.Categories.Count();
            int index = random.Next(0, numberofCategories);
            return repository.Categories.ToList().ElementAt(index).CategoryId;
        }
    }
}
