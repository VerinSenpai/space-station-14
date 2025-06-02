using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server.Chat.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class SetOOCCommand : LocalizedEntityCommands
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public override string Command => "setooc";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length > 1)
        {
            shell.WriteError(Loc.GetString("cmd-setooc-too-many-arguments-error"));
            return;
        }

        var ooc = _cfg.GetCVar(CCVars.OocEnabled);

        if (args.Length == 0)
            ooc = !ooc;

        if (args.Length == 1 && !bool.TryParse(args[0], out ooc))
        {
            shell.WriteError(Loc.GetString("cmd-setooc-invalid-argument-error"));
            return;
        }

        _cfg.SetCVar(CCVars.OocEnabled, ooc);

        shell.WriteLine(Loc.GetString(ooc ? "cmd-setooc-ooc-enabled" : "cmd-setooc-ooc-disabled"));
    }
}
