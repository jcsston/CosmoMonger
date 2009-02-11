namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CosmoMonger.Models;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class SpeedTest
    {
        public const int LOOP_COUNT = 10;

        [Test]
        public void LINQWhereObject()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Player topPlayer = db.Players.First();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                IQueryable<Player> matchingPlayer = (from p in db.Players
                                                     where p == topPlayer
                                                     select p);
            }
        }

        [Test]
        public void LINQWhereObjectId()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Player topPlayer = db.Players.First();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                IQueryable<Player> matchingPlayer = (from p in db.Players
                                                     where p.PlayerId == topPlayer.PlayerId
                                                     select p);
            }
        }

        [Test]
        public void LINQFirst()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                Player topPlayer = (from p in db.Players
                                    where p.PlayerId == 1
                                    select p).FirstOrDefault();
            }
        }

        [Test]
        public void LINQSingle()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                Player topPlayer = (from p in db.Players
                                    where p.PlayerId == 1
                                    select p).SingleOrDefault();
            }
        }

        [Test]
        public void LINQToArray()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                Player [] players = (from p in db.Players
                                    where p.Alive
                                    select p).ToArray();
                foreach (Player player in players)
                {
                    player.NetWorth++;
                }
            }
        }

        [Test]
        public void LINQToList()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                List<Player> players = (from p in db.Players
                                        where p.Alive
                                        select p).ToList();
                foreach (Player player in players)
                {
                    player.NetWorth++;
                }
            }
        }

        [Test]
        public void LINQToIEnumerable()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            for (int i = 0; i < LOOP_COUNT; i++)
            {
                IEnumerable<Player> players = (from p in db.Players
                                               where p.Alive
                                               select p);
                foreach (Player player in players)
                {
                    player.NetWorth++;
                }
            }
        }
    }
}
