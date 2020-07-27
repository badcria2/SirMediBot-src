// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Linq;
using Microsoft.Bot.Builder.AI.QnA;
using System;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        public QnAMaker EchoBotQnA { get; private set; }
        public EchoBot(QnAMakerEndpoint endpoint)
        {
            // connects to QnA Maker endpoint for each turn
            EchoBotQnA = new QnAMaker(endpoint);
        }
        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    var replyText = $"Echo: {turnContext.Activity.Text}";
        //    await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        //}

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Bienvenido :V!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync($"{ welcomeText } - {member.Name}. SirMediBot te da la bienvenida", cancellationToken: cancellationToken);
                   // await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }

        private async Task AccessQnAMaker(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            try {
                var results = await EchoBotQnA.GetAnswersAsync(turnContext);
                if (results.Any())
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Procesando su respuesta: " + results.First().Answer), cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Lo siento, no contamos respuesta para tal pregunta"), cancellationToken);
                }
            }
            catch(Exception ex)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Ha ocurrido un error, contactate con mi creador: {ex.Message}"), cancellationToken);
            }
            

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // await turnContext.SendActivityAsync(MessageFactory.Text($"Ha ocurrido un error, contactate con mi creador: {turnContext.Activity.Text}"), cancellationToken);

            await AccessQnAMaker(turnContext, cancellationToken);
        }
    }
}
