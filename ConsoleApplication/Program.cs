using BasicEFCore_SamuraiApp.Data;
using BasicEFCore_SamuraiApp.Domain;
using System;
using System.Dynamic;
using System.Linq;

namespace ConsoleApplication
{
    class Program
    {
        private static SamuraiContext context = new SamuraiContext();

        static void Main(string[] args)
        {
            context.Database.EnsureCreated();

            GetSamurai("Before Add;");
            AddSamurai();
            GetSamurai("After Add;");

            Console.WriteLine("Press any key...");
            Console.ReadKey();


        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name="Samurai One" };
            context.Samurais.Add(samurai);
            context.SaveChanges();
        }

        private static void GetSamurai(string text)
        {
            var samurais = context.Samurais.ToList();
            Console.WriteLine($"{text}: Samurai count is {samurais.Count()}");
            foreach (var item in samurais)
            {
                Console.WriteLine(item.Name);
            }
        }
    }
}
