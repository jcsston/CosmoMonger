namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using CosmoMonger.Models;
    using NUnit.Framework;

    /// <summary>
    /// Summary description for NpcTest
    /// </summary>
    [TestFixture]
    public class NpcTest
    {
        [Test]
        public void CalculatePriceMultipler()
        {
            Npc npcRow = new Npc();
            CosmoMonger.Models.Npcs.NpcGoodBalancer npc = new CosmoMonger.Models.Npcs.NpcGoodBalancer(npcRow);

            for (int goodCount = 0; goodCount < 20; goodCount++)
            {
                double priceMultipler1 = npc.CalculatePriceMultipler(100, 9, goodCount);
                double priceMultipler2 = npc.CalculatePriceMultiplerOld(100, 9, goodCount);

                Console.WriteLine("GoodCount: {0} New: {1} Old: {2}", goodCount, priceMultipler1, priceMultipler2);
            }
        }
    }
}
