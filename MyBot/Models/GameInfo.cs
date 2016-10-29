using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MyBot.Models
{
    public class GameInfo
    {
        public long Id { get; set; }
        public string RecipientId { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int MyAliveCells { get; set; }
        public int EnemyAliveCells { get; set; }
        public int EnemyLine { get; set; }
        public int EnemyColumn { get; set; }
        public bool GameStarted { get; set; }
        public string MyField { get; set; }
        public string EnemyField { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<GameInfo> GameInfos { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}