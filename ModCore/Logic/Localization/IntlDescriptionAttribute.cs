using System;

namespace ModCore.Logic.Localization
{
    /// <inheritdoc />
    /// <summary>
    /// Gives this command, group, or argument a description, that can include localization tokens.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public sealed class ComputedDescriptionAttribute : Attribute
    {
        private readonly string _description;

        /// <summary>
        /// Gets the description for this command, group, or argument.
        /// </summary>
        public string Description => Localizer.Localize(_description);

        /// <inheritdoc />
        /// <summary>
        /// Gives this command, group, or argument a description, which is used when listing help.
        /// </summary>
        /// <param name="description"></param>
        public ComputedDescriptionAttribute(string description)
        {
            this._description = description;
        }
    }
}