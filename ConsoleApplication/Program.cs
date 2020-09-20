using BasicEFCore_SamuraiApp.Data;
using BasicEFCore_SamuraiApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
            AddBulkSamurai();
            InsertVariousType();
            GetSamurai("After Add;");

            BatchUpdateSamurai();
            Filter();
            RetriveAndDeleteSamurai();


            //--Related data 

            InsertNewSamuraiWithAQuote();
            AddQuoteToExistingSamuraiWhileTracked();
            AddQuoteToExistingSamuraiNotTracked();

            ModifyRelatedDataWhenTracked();
            ModifyRelatedDataWhenWithoutTracked();


            // Creating and changing and Query Many-to-Many relationship samurai and battle => table name => samuraiBattle
            JoinBattleAndSamurai();
            AddNewSamuraiIntoExistingBattle();
            RemoveJoinBetweenSamuraiAndBattleSimple();

            GetSamuraiWithBattle();

            replaceASamuraiHourse();
            getHorseWithSamurai();

            // many to one relationship => many (samurai) to one (clan)
            GetClanWuthSamurai();

            Console.WriteLine("Press any key...");
            Console.ReadKey();


        }

        private static void GetClanWuthSamurai()
        {
            // var clan = context.Clans.Include(x=>x. ??????? ) // no navigation

            var clan = context.Clans.Find(1);
            var samuraiList_ForClan = context.Samurais.Where(x => x.Clan.Id == 1).ToList();
        }

        private static void getHorseWithSamurai()
        {
            var horse_WithOut_Samurai = context.Set<Horse>().Find(1);

            var horse_With_Samurai = context.Samurais.Include(s => s.Horse)
                .FirstOrDefault(s => s.Horse.Id == 3);

            var horse_With_Samurais = context.Samurais
                .Where(s => s.Horse != null)
                .Select(s => new
                {
                    Horse = s.Horse,
                    Samurai = s
                })
                .ToList();
        }

        private static void replaceASamuraiHourse()
        {
            var samurai = context.Samurais
                .Include(x => x.Horse)
                .FirstOrDefault(x => x.Id == 9);

            samurai.Horse = new Horse { Name = "Trigger" };
            context.SaveChanges();

            //=> this will delete existing horse from samurai and then insert new horse for samurai 
            // (coz constant only allow 1 horse to 1 samurai )

            // but if horse object isn't in memory , then EFCore won't know to delete old and insert new horse
            //var samurai_horse_wrong = context.Samurais.Find(9);
            //samurai_horse_wrong.Horse = new Horse { Name = "wrong entry" };
            //context.SaveChanges();


        }

        private static void GetSamuraiWithBattle()
        {
            var samuraiWithBattle = context.Samurais
                .Include(s => s.SamuraiBattles)
                .ThenInclude(sb => sb.Battle)
                .FirstOrDefault(s => s.Id == 9);

            var samuraiWithBattleCleaner = context.Samurais
                .Where(s => s.Id == 8)
                .Select(s => new
                {
                    Samurai = s,
                    Battles = s.SamuraiBattles.Select(x => x.Battle)
                })
                .FirstOrDefault();


        }

        private static void RemoveJoinBetweenSamuraiAndBattleSimple()
        {
            var join = new SamuraiBattle { BattleId = 1, SamuraiId = 9 };
            context.Remove(join);
            context.SaveChanges();
        }

        private static void AddNewSamuraiIntoExistingBattle()
        {
            var battle = context.Battles.Find(9);
            battle.SamuraiBattles.Add(new SamuraiBattle { SamuraiId = 8 });
            context.SaveChanges();
        }

        private static void JoinBattleAndSamurai()
        {
            // Samurai and battle already exist and we have their Ids
            var sbJoin = new SamuraiBattle { BattleId = 1, SamuraiId = 9 };
            context.Add(sbJoin);
            context.SaveChanges();
        }

        private static void ModifyRelatedDataWhenWithoutTracked()
        {
            var samurai = context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 9);
            var quote = samurai.Quotes[0];
            quote.Text += "Did you hear that again?";

            using (var newcontext = new SamuraiContext())
            {
                // newcontext.Quotes.Update(quote); 
                // this quote still attach to samurai and samurai has attached to 3 other quotes 
                // so update command will update all 3 quote object 

                // newcontext.Quotes.Attach(quote); 
                // if i used attach, all of them would be tracked, but marked (state) as unchanged
                // even the one i modified. so that's not going to work in this case


                newcontext.Entry(samurai).State = EntityState.Modified;
                // in ef core => entry will focus specifically on the entry that you pass in , and it will ignore anything else that attched to it
                // this will only update quote object 


                newcontext.SaveChanges();
            }

        }

        private static void ModifyRelatedDataWhenTracked()
        {
            var samurai = context.Samurais.Include(s => s.Quotes).FirstOrDefault(s => s.Id == 9);
            samurai.Quotes[0].Text = "Did you hear that?";
            context.SaveChanges();
        }


        private static void AddQuoteToExistingSamuraiNotTracked()
        {
            var samurai = context.Samurais.Find(9);
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that i've saved you"
            });


            using var _dbContext = new SamuraiContext();
            //_dbContext.Samurais.Update(samurai);
            //_dbContext.SaveChanges();
            // this above query update samurai (not needed) and insert new quote.. to enhance performance //use attach => make unchanged entity state


            _dbContext.Samurais.Attach(samurai);
            _dbContext.SaveChanges();


        }

        private static void AddQuoteToExistingSamuraiWhileTracked()
        {
            var samurai = context.Samurais.FirstOrDefault();
            samurai.Quotes.Add(new Quote
            {
                Text = "I bet you're happy that i've saved you"
            });

            context.SaveChanges();
        }

        private static void InsertNewSamuraiWithAQuote()
        {
            var samurai = new Samurai
            {
                Name = "Kamnei Shimada",
                Quotes = new System.Collections.Generic.List<Quote>
                {
                    new Quote { Text = "I've come to save you" }

                }
            };

            context.Samurais.Add(samurai);
            context.SaveChanges();
        }

        private static void InsertVariousType()
        {
            var samurai = new Samurai { Name = "Kikuchio" };
            var clan = new Clan { ClanName = "Imperial Clan" };

            context.AddRange(samurai, clan);
            context.SaveChanges();
        }

        private static void AddBulkSamurai()
        {
            // atlest 4 record needed to perform bulk insert

            var samuraiB1 = new Samurai { Name = "Samurai bulk 1" };
            var samuraiB2 = new Samurai { Name = "Samurai bulk 2" };
            var samuraiB3 = new Samurai { Name = "Samurai bulk 3" };
            var samuraiB4 = new Samurai { Name = "Samurai bulk 4" };
            var samuraiB5 = new Samurai { Name = "Samurai bulk 5" };

            context.Samurais.AddRange(samuraiB1, samuraiB2, samuraiB3, samuraiB4, samuraiB5);
            context.SaveChanges();
        }

        private static void AddSamurai()
        {
            var samurai = new Samurai { Name = "Samurai One" };
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

        static void Filter()
        {
            var filter = "%Nepal";
            var data = context.Samurais
                 .Where(s => EF.Functions.Like(s.Name, filter))
                 .ToList();

            var name = "Kikuchio";
            // var last = context.Samurais.LastOrDefault(x => x.Name == name); //runtime error 
            var last = context.Samurais.OrderBy(x => x.Id).LastOrDefault(x => x.Name == name);

        }

        static void BatchUpdateSamurai()
        {
            // atlest 4 record needed to perform bulk update

            var samurais = context.Samurais.Skip(1).Take(5).ToList();
            samurais.ForEach(x => x.Name += " Nepal");
            context.SaveChanges();
        }

        static void RetriveAndDeleteSamurai()
        {

            var samurai = context.Samurais.Find(9);
            context.Samurais.Remove(samurai);
            context.SaveChanges();
        }

    }
}
