using Match3Game.Domain.Board;

namespace Match3Game.Domain.Match
{
    /// <summary>
    /// Open/Closed Principle: Yeni match tipleri (diagonal vb.)
    /// bu interface'i implement ederek sisteme eklenir.
    /// mevcut kod yapısı değişmez.
    /// </summary>
    public interface IMatchStrategy
    {
        MatchResult FindMatches(BoardModel board);
    }

}
