using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using ModCore.Entities;
using ModCore.Logic;

namespace ModCore.Listeners
{
    public class ErrorLog
    {
        [AsyncListener(EventTypes.CommandErrored)]
        public static async Task CommandError(ModCoreShard bot, CommandErrorEventArgs e)
        {
            var cfg = e.Context.GetGuildSettings();
            var ce = cfg.CommandError;
            var ctx = e.Context;
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Critical, "Commands", e.Exception 
                + $"\nError verbosity: chat.{ce.Chat} actionlog.{ce.ActionLog}", DateTime.Now);

            if (e.Exception is CommandNotFoundException)
                return;

            switch (ce.Chat)
            {
                default:
                case CommandErrorVerbosity.None:
                    break;

                case CommandErrorVerbosity.Name:
                    await ctx.RespondAsync($"**Command {e.Command.QualifiedName} Errored!**\n`{e.Exception.GetType()}`");
                    break;
                case CommandErrorVerbosity.NameDesc:
                    await ctx.RespondAsync($"**Command {e.Command.QualifiedName} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                    break;
                case CommandErrorVerbosity.Exception:
                    var stream = new MemoryStream();
                    var writer = new StreamWriter(stream);
                    writer.Write(e.Exception.ToString());
                    writer.Flush();
                    stream.Position = 0;
                    await ctx.RespondWithFileAsync("exception.txt", stream, $"**Command `{e.Command.QualifiedName} {e.Command.Arguments}` Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                    break;
            }

            if (!cfg.ActionLog.Enable) return;
            
            switch (ce.ActionLog)
            {
                default:
                case CommandErrorVerbosity.None:
                    break;

                case CommandErrorVerbosity.Name:
                    await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} Errored!**\n`{e.Exception.GetType()}`");
                    break;
                case CommandErrorVerbosity.NameDesc:
                    await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}");
                    break;
                case CommandErrorVerbosity.Exception:
                    var st = e.Exception.StackTrace;

                    st = st.Length > 1000 ? st.Substring(0, 1000) : st;
                    var b = new DiscordEmbedBuilder().WithDescription(st);
                    await ctx.LogMessageAsync($"**Command {e.Command.QualifiedName} {e.Command.Arguments} Errored!**\n`{e.Exception.GetType()}`:\n{e.Exception.Message}", b);
                    break;
            }
        }
    }
}
