using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace WebSwimming
{
    internal class PlayerScriptMod : IScriptMod
    {
        public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

        public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
        {
            var topOfFile = new MultiTokenWaiter([
                t => t.Type is TokenType.Newline,
                t => t.Type is TokenType.Newline,
            ], allowPartialMatch: true);

            // func _on_water_detect_area_entered(area):
            //   if gravity_disable: return
            //   
            //   ^
            var water = new MultiTokenWaiter([
                t => t.Type is TokenType.PrFunction,
                t => t is IdentifierToken { Name: "_on_water_detect_area_entered" },
                t => t is IdentifierToken { Name: "gravity_disable" },
                t => t.Type is TokenType.CfReturn,
                t => t.Type is TokenType.Newline,
            ], allowPartialMatch: true);

            foreach (var token in tokens)
            {
                if (topOfFile.Check(token))
                {
                    yield return token;

                    // Create variable for swimming state
                    // var swimming = false
                    yield return new Token(TokenType.PrVar);
                    yield return new IdentifierToken("swimming");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new BoolVariant(false));
                    yield return new Token(TokenType.Newline);
                }
                else if (water.Check(token))
                {
                    yield return token;

                    // if area.is_in_group("water"):
                    //   if not swimming:
                    //     swimming = true
                    //     PlayerData.emit_signal("_swim_start")
                    // elif swimming:
                    //  swimming = false
                    //  PlayerData.emit_signal("_swim_stop")
                    // return

                    // if area.is_in_group("water"):
                    yield return new Token(TokenType.CfIf);
                    yield return new IdentifierToken("area");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("is_in_group");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new ConstantToken(new StringVariant("water"));
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 2);

                    // if not swimming:
                    yield return new Token(TokenType.CfIf);
                    yield return new Token(TokenType.OpNot);
                    yield return new IdentifierToken("swimming");
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 3);

                    // swimming = true
                    yield return new IdentifierToken("swimming");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new BoolVariant(true));
                    yield return new Token(TokenType.Newline, 3);

                    // PlayerData.emit_signal("_swim_start")
                    yield return new IdentifierToken("PlayerData");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("emit_signal");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new ConstantToken(new StringVariant("_swim_start"));
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 1);

                    // else if swimming:
                    yield return new Token(TokenType.CfElif);
                    yield return new IdentifierToken("swimming");
                    yield return new Token(TokenType.Colon);
                    yield return new Token(TokenType.Newline, 2);

                    // swimming = false
                    yield return new IdentifierToken("swimming");
                    yield return new Token(TokenType.OpAssign);
                    yield return new ConstantToken(new BoolVariant(false));
                    yield return new Token(TokenType.Newline, 2);

                    // PlayerData.emit_signal("_swim_stop")
                    yield return new IdentifierToken("PlayerData");
                    yield return new Token(TokenType.Period);
                    yield return new IdentifierToken("emit_signal");
                    yield return new Token(TokenType.ParenthesisOpen);
                    yield return new ConstantToken(new StringVariant("_swim_stop"));
                    yield return new Token(TokenType.ParenthesisClose);
                    yield return new Token(TokenType.Newline, 1);

                    // return
                    yield return new Token(TokenType.CfReturn);
                    yield return new Token(TokenType.Newline, 1);
                }
                else
                {
                    // return the original token
                    yield return token;
                }
            }
        }
    }
}
