
using Match3Game.Domain.Match;

namespace Match3Game.Application.DTOs
{
    public class SwapResult
    {
        public bool IsValid {get;}
        public string Reason {get;}
        public MatchResult Match {get;}

        public SwapResult(bool isValid, string reason, MatchResult match)
        {
            IsValid = isValid;
            Reason = reason;
            Match = match;
        }

        public static SwapResult Valid(MatchResult match) => 
            new(true, null, match);

        public static SwapResult Invalid(string reason) => 
            new(false, reason, MatchResult.Empty);
        
    }

}
