using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace ModCore.Commands
{
    [Group("defcon")]
    [Aliases("df", "antiraid", "raid")]
    [Description("$DefconGuide")]
    public class Defcon : BaseCommandModule
    {
        [Command("5"), Aliases("fade-out", "fadeout", "disable", "off", "o", "0"), 
         Description("Sets the DEFCON level to 5. $Defcon5.Description")]
        public async Task FadeOutAsync(CommandContext ctx)
        {
        }
    }
}