using System.Numerics;
using Content.Server.Administration;
using Content.Shared.Administration;
using Content.Shared.Maps;
using Robust.Shared.Console;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Interaction;

[AdminCommand(AdminFlags.Debug)]
public sealed class TilePryCommand : LocalizedEntityCommands
{
    [Dependency] private readonly ITileDefinitionManager _tiles = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;

    public override string Command => "tilepry";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var player = shell.Player;
        if (player?.AttachedEntity is not { } attached)
            return;

        if (args.Length != 1)
        {
            shell.WriteLine(Help);
            return;
        }

        if (!int.TryParse(args[0], out var radius))
        {
            shell.WriteError(Loc.GetString($"cmd-tilepry-invalid", ("arg", args[0])));
            return;
        }

        if (radius < 0)
        {
            shell.WriteError(Loc.GetString($"cmd-tilepry-radius-negative"));
            return;
        }

        var xform = EntityManager.GetComponent<TransformComponent>(attached);

        var playerGrid = xform.GridUid;

        if (!EntityManager.TryGetComponent<MapGridComponent>(playerGrid, out var mapGrid))
            return;

        var playerPosition = xform.Coordinates;

        for (var i = -radius; i <= radius; i++)
        {
            for (var j = -radius; j <= radius; j++)
            {
                var tile = _map.GetTileRef(playerGrid.Value, mapGrid, playerPosition.Offset(new Vector2(i, j)));
                var coordinates = _map.GridTileToLocal(playerGrid.Value, mapGrid, tile.GridIndices);
                var tileDef = (ContentTileDefinition)_tiles[tile.Tile.TypeId];

                if (!tileDef.CanCrowbar)
                    continue;

                var plating = _tiles["Plating"];
                _map.SetTile(playerGrid.Value, mapGrid, coordinates, new Tile(plating.TileId));
            }
        }
    }
}
