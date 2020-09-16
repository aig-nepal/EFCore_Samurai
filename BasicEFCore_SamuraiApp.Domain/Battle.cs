using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicEFCore_SamuraiApp.Domain
{
    public class Battle
    {
        public Battle()
        {
            SamuraiBattles = new List<SamuraiBattle>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndTime { get; set; }

        // Many to Many 
        public List<SamuraiBattle> SamuraiBattles { get; set; }
    }

    public class SamuraiBattle
    {
        public int SamuraiId { get; set; }
        public int BattleId { get; set; }

        public Samurai Samurai { get; set; }
        public Battle  Battle { get; set; }

    }
}
