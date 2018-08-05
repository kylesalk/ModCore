using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace ModCore.Logic.Localization
{
    public class LocalizationHelpFormatter : BaseHelpFormatter
    {
        private DiscordEmbedBuilder EmbedBuilder { get; }
        private Command Command { get; set; }
        private Localizer Localizer { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new l18n help formatter.
        /// </summary>
        /// <param name="ctx">Context in which this formatter is being invoked.</param>
        /// <param name="localizer">Localizer service.</param>
        public LocalizationHelpFormatter(CommandContext ctx, Localizer localizer)
            : base(ctx)
        {
            this.EmbedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Help")
                .WithColor(0x007FFF);
            this.Localizer = localizer;
        }

        /// <summary>
        /// Helper localize function
        /// </summary>
        /// <param name="text">text to localize</param>
        /// <returns>text with tokens replaced by their local variants</returns>
        private string L(string text) => this.Localizer.Localize(text);

        /// <inheritdoc />
        /// <summary>
        /// Sets the command this help message will be for.
        /// </summary>
        /// <param name="command">Command for which the help message is being produced.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.Command = command;

            this.EmbedBuilder.WithDescription(
                $"{Formatter.InlineCode(command.Name)}: {L(command.Description ?? "$Help.NoDescription")}");

            if (command is CommandGroup cgroup && cgroup.IsExecutableWithoutSubcommands)
                this.EmbedBuilder.WithDescription(
                    L($"{this.EmbedBuilder.Description}\n\n$Help.GroupIsExecutableWithoutSubcommands"));

            if (command.Aliases?.Any() == true)
                this.EmbedBuilder.AddField(L("$Help.CommandAliasesTitle"), string.Join(", ", command.Aliases.Select(Formatter.InlineCode)));

            if (command.Overloads?.Any() != true) return this;
            
            var sb = new StringBuilder();

            foreach (var ovl in command.Overloads.OrderByDescending(x => x.Priority))
            {
                sb.Append('`').Append(command.QualifiedName);

                foreach (var arg in ovl.Arguments)
                    sb.Append(arg.IsOptional || arg.IsCatchAll ? " [" : " <").Append(arg.Name).Append(arg.IsCatchAll ? "..." : "").Append(arg.IsOptional || arg.IsCatchAll ? ']' : '>');

                sb.Append("`\n");

                foreach (var arg in ovl.Arguments)
                    sb.Append('`').Append(arg.Name).Append(" (").Append(this.CommandsNext.GetUserFriendlyTypeName(arg.Type)).Append(")`: ").Append(arg.Description ?? "No description provided.").Append('\n');

                sb.Append('\n');
            }

            this.EmbedBuilder.AddField(L("$Help.CommandArgumentsTitle"), sb.ToString().Trim());

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the subcommands for this command, if applicable. This method will be called with filtered data.
        /// </summary>
        /// <param name="subcommands">Subcommands for this command group.</param>
        /// <returns>This help formatter.</returns>
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.EmbedBuilder.AddField(this.Command != null ? L("$Help.SubcommandsTitle") : L("$Help.CommandsTitle"), string.Join(", ", subcommands.Select(x => Formatter.InlineCode(x.Name))));

            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// Construct the help message.
        /// </summary>
        /// <returns>Data for the help message.</returns>
        public override CommandHelpMessage Build()
        {
            if (this.Command == null)
                this.EmbedBuilder.WithDescription(L("$Help.TopLevelListing"));

            return new CommandHelpMessage(embed: this.EmbedBuilder.Build());
        }
    }
}