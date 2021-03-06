﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;

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

                var type = answer.topScoringIntent.intent.Value;
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
                    gameReply = game.Play(activity.From.Id, type, line, column);
                }
                else
                {
                    gameReply = game.Play(activity.From.Id, type);
                }
                Activity reply = activity.CreateReply(gameReply);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}