using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace WebSwimming
{
    internal class PlayerDataScriptMod : IScriptMod
    {
        public bool ShouldRun(string path) => path == "res://Scenes/Singletons/playerdata.gdc";

        public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
        {
            
            var topOfFile = new MultiTokenWaiter([
                t => t.Type is TokenType.Newline,
                t => t.Type is TokenType.Newline,
            ], allowPartialMatch: true);

            foreach (var token in tokens)
            {
                if (topOfFile.Check(token))
                {
                    yield return token;

                    // Create signals for player swim start and stop
                    // signal _swim_start
                    yield return new Token(TokenType.PrSignal);
                    yield return new IdentifierToken("_swim_start");
                    yield return new Token(TokenType.Newline);

                    // signal _swim_stop
                    yield return new Token(TokenType.PrSignal);
                    yield return new IdentifierToken("_swim_stop");
                    yield return new Token(TokenType.Newline);
                } else {
                    yield return token;
                }
            }
        }
    }
}
