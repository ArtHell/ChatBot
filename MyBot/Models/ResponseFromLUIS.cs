using System.Collections.Generic;

namespace MyBot.Models
{
    public class ResponseFromLuis
    {
        public List<Intent> Intents { get; set; }
        public string HitColumn { get; set; }
        public string HitLine { get; set; }
    }
}