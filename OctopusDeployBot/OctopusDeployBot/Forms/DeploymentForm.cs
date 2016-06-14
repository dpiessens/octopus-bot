using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using OctopusDeployBot.Octopus;

namespace OctopusDeployBot.Forms
{
    [Serializable]
    [Template(TemplateUsage.NotUnderstood, "I do not understand \"{0}\".", "Try again, I don't get \"{0}\".")]
    public class DeploymentForm
    {
        public string Environment { get; set; }

        public string Project { get; set; }

        public static IForm<DeploymentForm> BuildForm()
        {
            return new FormBuilder<DeploymentForm>()
                        .Field(new FieldReflector<DeploymentForm>(nameof(Project))
                            .SetType(null)
                            .ReplaceTemplate(new TemplateAttribute(
                                TemplateUsage.EnumSelectOne,
                                "Pick a project: {||}")
                            {
                                ChoiceStyle = ChoiceStyleOptions.PerLine
                            })
                            .SetDefine((state, field) =>
                            {
                                foreach (var prod in OctopusClient.GetProjects())
                                    field
                                        .AddDescription(prod.Id, prod.Name)
                                        .AddTerms(prod.Id, prod.Name);

                                return Task.FromResult(true);
                            }))
                        .Field(new FieldReflector<DeploymentForm>(nameof(Environment))
                            .SetType(null)
                            .ReplaceTemplate(new TemplateAttribute(
                                TemplateUsage.EnumSelectOne, 
                                "What evironment would you like to check? {||}")
                                {
                                    ChoiceStyle = ChoiceStyleOptions.PerLine
                                })
                            .SetDefine((state, field) =>
                            {
                                foreach (var prod in OctopusClient.GetEnvironments())
                                    field
                                        .AddDescription(prod.Id, prod.Name)
                                        .AddTerms(prod.Id, prod.Name);

                                return Task.FromResult(true);
                            }))
                        .AddRemainingFields()
                        .Build();
        }
    }
}