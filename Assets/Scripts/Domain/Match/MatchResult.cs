using System.Collections.Generic;
using System.Linq;

namespace Match3Game.Domain.Match
{
    /// <summary>
    /// Bir match tarama turunda bulunan tüm grupları taşır.
    /// </summary>
    public class MatchResult
    {
        public static readonly MatchResult Empty = new(new List<MatchGroup>());

        public IReadOnlyList<MatchGroup> Groups {get;}
        public bool HasMatch => Groups.Count > 0;
        public int TotalTiles => Groups.Sum(g => g.Tiles.Count);

        public MatchResult(List<MatchGroup> groups)
        {
            Groups = groups.AsReadOnly();
        }
    }

}
