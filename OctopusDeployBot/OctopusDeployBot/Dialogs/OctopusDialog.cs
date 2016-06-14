using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using OctopusDeployBot.Forms;
using OctopusDeployBot.Octopus;

namespace OctopusDeployBot.Dialogs
{
    [Serializable]
    [LuisModel("782d7388-c2a2-4727-8347-35918eb44403", "d9f0a13cf5164dd6a3643e178211f722")]
    public class OctopusDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            var userUtterance = result.Query;
            await context.PostAsync($"Sorry, I didn't understand \"{userUtterance}\".");
            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckDeployment")]
        public async Task ChangeInfo(IDialogContext context, LuisResult result)
        {
            var form = new DeploymentForm();
            var formDialog = new FormDialog<DeploymentForm>(form, DeploymentForm.BuildForm, FormOptions.PromptInStart, result.Entities);

            context.Call(formDialog, OnComplete);
        }
        private async Task OnComplete(IDialogContext context, IAwaitable<DeploymentForm> result)
        {
            try
            {
                var form = await result;
                var response = OctopusClient.GetDeploymentStatus(form.Project, form.Environment) ??
                         "Sorry I could find any deployments for that project and environment";

                await context.PostAsync(response);
            }
            catch (OperationCanceledException)
            {
                await context.PostAsync("Operation Cancelled");
            }

            context.Wait(MessageReceived);
        }
    }
}