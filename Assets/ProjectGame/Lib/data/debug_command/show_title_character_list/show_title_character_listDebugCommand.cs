using UnityEngine;
using System.Collections.Generic;
using GameCore.Tables;

namespace GameCore.DebugCommand
{
    // show_title_character_list コマンドの実処理。ここに手動でロジックを実装してください。
    // このファイルは初回生成時にのみ作られ、以後の生成では上書きされません。
    public class show_title_character_listDebugCommand : Baseshow_title_character_listDebugCommand
    {
        protected override void Execute(show_title_character_listArgs args, show_title_character_listResult result)
        {

            List<JsonObject> characters = new List<JsonObject>();

            foreach(var data in TitleCore.Instance.GuestTitleCharacter)
            {
                JsonObject character = new JsonObject();
                var row = data.Key.GetRow();
                character["character_id"] = row.TableID.ToString();
                character["character_name"] = row.Name;
                characters.Add(character);
                
            }

        }
    }
}