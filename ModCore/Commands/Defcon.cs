using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using ModCore.Logic.Localization;

namespace ModCore.Commands
{
    [Group("defcon")]
    [Aliases("df", "antiraid", "raid")]
    [Description(
        "Controls for automated anti-raiding mechanisms.\n" +
        "During DEFCON mode, some settings are temporarily changed to protect your server. All actions taken by the" +
        " anti-raid will be logged to a new channel, normally called `#defcon`.\n" +
        "DEFCON tries to be as non-destructive as possible, but some actions could lead to false positives. Please" +
        " read the information provided for the DEFCON level you're looking at before blindly selecting it.")]
    public class Defcon : BaseCommandModule
    {
        private static readonly string[] LevelDescs =
        {
            @"DEFCON 5 (FADE OUT): This is the lowest level.
\* There are no automated preventive measures taken to combat automated raiding.
\* Any changes to server configuration made by ModCore will be restored to the default state.",

            @"DEFCON 4 (DOUBLE TAKE): Paranoid mode.
\* Verification level is set to Medium if not already higher.
\* Chat is set to 10 seconds slow mode for members who haven't been in the guild for over a month.
\* Explicit content filter set to scan messages from members without a role if not already higher.",

            @"DEFCON 3 (ROUND HOUSE): Active threat mode.
\* Verification level is set to table-flip mode if not already higher.
\* Chat is set to 30 seconds slow mode for all members.
\* Explicit content filter set to scan messages from all members.
\* Suspicious permissions will be removed from the `@everyone` role.
\* Role states will be disabled until the DEFCON state is lifted.",

            @"DEFCON 2 (FAST PACE): Concurrent threat mode.
\* Verification level is set to table-flip mode if not already higher.
\* New members are immediately kicked with a message.
\* Chat is set to 30 seconds slow mode for all members.
\* Only messages containing alphanumerical (regardless of language or alphabet) text (or whitespace) are allowed. Violators of this rule will be muted for two minutes.
\* All messages will be checked by the Perspective API, and those deemed toxic will be met with a two-minute mute.

Inherited from DEFCON 3:
\* Explicit content filter set to scan messages from all members.
\* Suspicious permissions will be removed from the `@everyone` role.
\* Role states will be disabled until the DEFCON state is lifted.",

            @"DEFCON 1 (COCKED PISTOL): Uncontrollable threat mode.
\* All invite links are removed.
\* New members are immediately kicked with a message.
\* Users who attempt to create invite links will be immediately banned.
\* Chat is set to 2 minutes slow mode for members without roles, and 30 seconds for members with roles.
\* Only messages containing alphanumerical (regardless of language or alphabet) text (or whitespace) are allowed. Violators of this rule will be muted until the DEFCON state is lifted.
\* All messages will be checked by the Perspective API, and those deemed toxic will be met with a mute until the DEFCON state is lifted.
\* All channels will be hidden for all bots except ModCore.
\* Attachments are disabled for all members.

Inherited from DEFCON 3:
\* Suspicious permissions will be removed from the `@everyone` role.
\* Role states will be disabled until the DEFCON state is lifted.",
        };

        [Command("5"), Description("Sets the DEFCON level to 5. $Defcon5.Description")]
        public async Task FadeOutAsync(CommandContext ctx,
            [Description("New command prefix for this guild")]
            string prefix = null)
        {
        }
    }
}