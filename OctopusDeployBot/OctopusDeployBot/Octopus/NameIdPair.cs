namespace OctopusDeployBot.Octopus
{
    public class NameIdPair
    {
        public NameIdPair(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}