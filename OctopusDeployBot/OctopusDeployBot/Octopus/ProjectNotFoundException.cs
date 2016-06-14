using System;

namespace OctopusDeployBot.Octopus
{
    public class ProjectNotFoundException : ApplicationException
    {
        public ProjectNotFoundException(string projectName) : base($"Could not locate project '{projectName}'")
        {
            this.ProjectName = projectName;
        }

        public string ProjectName { get; private set; } 
    }
}