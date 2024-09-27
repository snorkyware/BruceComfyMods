using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;

namespace ComfyGizmo
{
    public sealed class HammerTableManager
    {
        private static readonly string _searsCatalogGUID = "redseiko.valheim.searscatalog";
        private static readonly string _columnConfigSection = "BuildHud.Panel";
        private static readonly string _columnConfigKey = "buildHudPanelColumns";
        private static readonly int _defaultColumnCount = 15;

        private static BaseUnityPlugin _searsCatalog;
        private static ConfigEntry<int> _searsCatalogColumnsConfigEntry;

        private static Dictionary<string, Vector2Int> _pieceLocations = new();
        private static int _cachedAvailablePieceCount = -1;


        private static bool _targetSelection;

        public static void Initialize()
        {
            if (!IsSearsCatalogEnabled()) return;

            FindSearsCatalogPlugin();
        }

        public static void SelectTargetPiece(Player player)
        {
            if (IsHammerTableChanged(player) || !IsHammerTableCached()) CacheHammerTable(player);

            if (!HasCachedPiece(player.GetHoveringPiece())) return;

            _targetSelection = true;
            SetSelectedPiece(player, player.GetHoveringPiece());
        }

        public static bool IsTargetSelected()
        {
            return _targetSelection;
        }

        public static int GetColumnCount()
        {
            if (!IsSearsCatalogEnabled()) return _defaultColumnCount;

            return GetSearsCatalogColumnCount();
        }

        public static bool HasCachedPiece(Piece piece)
        {
            return _pieceLocations.ContainsKey(GetPieceIdentifier(piece));
        }

        public static void SetSelectedPiece(Player player, Piece piece)
        {
            var pieceLocation = _pieceLocations[GetPieceIdentifier(piece)];
            var previousCategory = player.m_buildPieces.m_selectedCategory;

            player.m_buildPieces.m_selectedCategory = (Piece.PieceCategory)pieceLocation.x;
            player.SetSelectedPiece(new Vector2Int(pieceLocation.y % GetColumnCount(),
                pieceLocation.y / GetColumnCount()));
            player.SetupPlacementGhost();

            if (previousCategory != player.m_buildPieces.m_selectedCategory)
                Hud.m_instance.UpdatePieceList(
                    player,
                    new Vector2Int(pieceLocation.y % 15, pieceLocation.y / 15),
                    (Piece.PieceCategory)pieceLocation.x,
                    true);
        }

        public static bool IsHammerTableCached()
        {
            return _cachedAvailablePieceCount != -1;
        }

        public static void CacheHammerTable(Player player)
        {
            var hammerPieceTable = player.m_buildPieces;
            _cachedAvailablePieceCount = 0;
            _pieceLocations = new Dictionary<string, Vector2Int>();

            for (var i = 0; i < hammerPieceTable.m_availablePieces.Count; i++)
            {
                var categoryPieces = hammerPieceTable.m_availablePieces[i];

                for (var j = 0; j < categoryPieces.Count; j++)
                {
                    if (_pieceLocations.ContainsKey(GetPieceIdentifier(categoryPieces[j]))) continue;

                    _pieceLocations.Add(GetPieceIdentifier(categoryPieces[j]), new Vector2Int(i, j));
                    _cachedAvailablePieceCount++;
                }
            }
        }

        public static bool IsHammerTableChanged(Player player)
        {
            if (!player || !player.m_buildPieces || player.m_buildPieces.m_availablePieces == null) return false;

            var currentPieceCount = 0;

            for (var i = 0; i < player.m_buildPieces.m_availablePieces.Count; i++)
                currentPieceCount += player.m_buildPieces.m_availablePieces[i].Count;

            if (currentPieceCount == _cachedAvailablePieceCount) return false;

            return true;
        }

        private static string GetPieceIdentifier(Piece piece)
        {
            return piece.m_name + piece.m_description;
        }

        public static bool IsSearsCatalogEnabled()
        {
            FindSearsCatalogPlugin();

            if (!_searsCatalog) return false;

            return true;
        }

        public static int GetSearsCatalogColumnCount()
        {
            if (_searsCatalogColumnsConfigEntry != null) return _searsCatalogColumnsConfigEntry.Value;

            if (_searsCatalog.Config.TryGetEntry(
                new ConfigDefinition(_columnConfigSection, _columnConfigKey), out ConfigEntry<int> columns))
            {
                _searsCatalogColumnsConfigEntry = columns;
                return columns.Value;
            }

            return _defaultColumnCount;
        }

        private static void FindSearsCatalogPlugin()
        {
            var loadedPlugins = GetLoadedPlugins();

            if (loadedPlugins == null) return;

            var plugins =
                loadedPlugins
                    .Where(plugin => plugin.Info.Metadata.GUID == _searsCatalogGUID)
                    .ToDictionary(plugin => plugin.Info.Metadata.GUID);

            if (plugins.TryGetValue(_searsCatalogGUID, out var plugin)) _searsCatalog = plugin;
        }

        private static IEnumerable<BaseUnityPlugin> GetLoadedPlugins()
        {
            return
                Chainloader.PluginInfos
                    .Where(x => x.Value != null && x.Value.Instance != null)
                    .Select(x => x.Value.Instance);
        }
    }
}
