using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyBot.Models;

namespace MyBot.Repositories
{
    public class GameInfoRepository
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public GameInfo GetGameInfo(string recipientId)
        {
            return db.GameInfos.FirstOrDefault(x => x.RecipientId == recipientId);
        }

        public void AddGameInfo(GameInfo gameInfo)
        {
            db.GameInfos.Add(gameInfo);
            db.SaveChanges();
        }

        public void SaveGameInfo(GameInfo gameInfo)
        {
            db.Entry(gameInfo).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
    }
}