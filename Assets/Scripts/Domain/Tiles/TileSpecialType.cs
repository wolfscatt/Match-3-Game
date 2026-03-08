namespace Match3Game.Domain.Tiles
{
    public enum TileSpecialType
    {
        None = 0,
        Bomb = 1,           // 3x3 alan temizler
        RocketH = 2,        // Yatay satır temizler
        RocketV = 3,        // Dikey satır temizler
        ColorBomb = 4       // Tüm aynı renkleri temizler
    }

}
