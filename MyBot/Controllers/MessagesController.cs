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
        private readonly Game game = new Game();

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
                if (type == "Hit")
                {
                    string line, column;
                    if (answer.entities[0].type == "Line")
                    {
                        line = (string) answer.entities[0].entity;
                        column = (string) answer.entities[1].entity;
                    }
                    else
                    {
                        line = (string)answer.entities[1].entity;
                        column = (string)answer.entities[0].entity;
                    }
                    gameReply = game.Play(activity.Recipient.Id, type, line, column);
                }
                else
                {
                    gameReply = game.Play(activity.Recipient.Id, type);
                }
                Activity reply = activity.CreateReply(gameReply);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}