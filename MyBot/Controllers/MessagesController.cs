using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using MyBot.Models;

namespace MyBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private Game game = new Game();

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                var answer = Luis.Analyze(activity.Text);

                var intents = new List<Intent>();

                foreach (var intent in answer.intents)
                {
                    intents.Add(new Intent() { Type = intent.intent, Score = intent.score });
                }

                var type = intents.OrderByDescending(x => x.Score).First().Type;
                string gameReply;
                if (type == "hit")
                {
                    gameReply = game.Play(type, (string) answer.entities[0].entity, (string) answer.entities[1].entity);
                }
                else
                {
                    gameReply = game.Play(type);
                }
                Activity reply = activity.CreateReply(gameReply);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}