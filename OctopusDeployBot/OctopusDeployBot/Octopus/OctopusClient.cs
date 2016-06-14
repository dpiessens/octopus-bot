using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Octopus.Client;

namespace OctopusDeployBot.Octopus
{
    public class OctopusClient
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["Octopus.Url"];
        private static readonly string ApiKey = ConfigurationManager.AppSettings["Octopus.ApiKey"];
        private static readonly OctopusRepository Repository = new OctopusRepository(new OctopusServerEndpoint(ServerUrl, ApiKey));

        public static List<NameIdPair> GetEnvironments()
        {
            return Repository.Environments.FindAll().Select(p => new NameIdPair(p.Id, p.Name)).ToList();
        }

        public static List<NameIdPair> GetProjects()
        {
            return Repository.Projects.FindAll().OrderBy(p => p.Name).Select(p => new NameIdPair(p.Id, p.Name)).ToList();
        }

        public static string GetDeploymentStatus(string projectId, string environmentId)
        {
            var project = Repository.Projects.Get(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }

            var results = Repository.Deployments.FindAll(new[] {projectId}, new[] {environmentId});
            if (results == null || results.TotalResults == 0)
            {
                return null;
            }

            var latestItem = results.Items.First();
            var release = Repository.Releases.Get(latestItem.ReleaseId);
            var status = Repository.Tasks.Get(latestItem.TaskId);

            var finalState = status.FinishedSuccessfully ? "sucessfully" : "with errors";
            var completedDate = status.CompletedTime.GetValueOrDefault();
            return $"Release {release.Version} was deployed at {completedDate.DateTime.ToShortTimeString()} on {completedDate.Date.ToShortDateString()} {finalState}";

        }
    }
}