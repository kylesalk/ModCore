using ModCore.Api;
using ModCore.Logic.HelpTemplating;

namespace ModCore.Entities
{
    /// <summary>
    /// Shared singleton services where the same instance is provided to all shards.
    /// </summary>
    public sealed class SharedServices
    {
        public Templater Templater { get; internal set; }
        public Perspective Perspective { get; internal set; }
    }
}