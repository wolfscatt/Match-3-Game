using System.Collections.Generic;
using Match3Game.Domain.Tiles;
using UnityEngine;

namespace Match3Game.Domain.Match
{
    /// <summary>
    /// Eşleşen tile'ların bir grubunu temsil eder.
    /// 3'lü, 4'lü, 5'li, L/T shape hepsi bu class ile ifade edilir.
    /// </summary>
    public class MatchGroup
    {
        public IReadOnlyList<TileModel> Tiles {get;}
        public TileColor Color {get;}
        public MatchShape Shape {get;}

        // 4+ match -> special tile üretmeli mi?
        public bool ShouldSpawnSpecial => Tiles.Count >= 4;
        public TileSpecialType SpawnedSpecialType => ResolveSpecialType();

        public MatchGroup(List<TileModel> tiles, MatchShape shape)
        {
            Tiles = tiles.AsReadOnly();
            Color = tiles[0].Color;
            Shape = shape;
        }

        private TileSpecialType ResolveSpecialType()
        {
            return Shape switch
            {
                MatchShape.FiveInRow => TileSpecialType.ColorBomb,
                MatchShape.LShape => TileSpecialType.Bomb,
                MatchShape.TShape => TileSpecialType.Bomb,
                MatchShape.FourInRow => TileSpecialType.RocketH,
                MatchShape.FourInCol => TileSpecialType.RocketV,
                _ => TileSpecialType.None
            };
        }
    }
    public enum MatchShape
    {
        ThreeInRow,
        ThreeInCol,
        FourInRow,
        FourInCol,
        FiveInRow,
        LShape,
        TShape
    }

}
