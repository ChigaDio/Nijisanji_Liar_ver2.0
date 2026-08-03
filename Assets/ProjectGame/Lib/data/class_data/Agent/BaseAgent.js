import { BaseCustomClassData } from '../BaseCustomClassData.js';
import { CharacterStats } from '../CharacterStats/CharacterStats.js';
import { GuestCharacterTableID } from '../../class_data_id/GuestCharacter/GuestCharacterTableID.js';
import { RoleTypeTableID } from '../../class_data_id/RoleType/RoleTypeTableID.js';

export class BaseAgent extends BaseCustomClassData {
    constructor() {
        super();
        this.guest_character_id = GuestCharacterTableID.NONE; // ゲストID(Noneならプレイヤー)
        this.character_stats = new CharacterStats(); // キャラステータス
        this.role_type = RoleTypeTableID.NONE; // 役職ID
    }

    read(view, offset) {
        let o = offset;
        this.guest_character_id = GuestCharacterTableID(view.getInt32(o, true)); o += 4;
        this.character_stats = new CharacterStats();
        o = this.character_stats.read(view, o);
        this.role_type = RoleTypeTableID(view.getInt32(o, true)); o += 4;
        return o;
    }

    loadJson(data) {
        this.guest_character_id = data.guest_character_id ?? GuestCharacterTableID.NONE;
        if (data.character_stats !== undefined && data.character_stats !== null) {
            this.character_stats = new CharacterStats();
            this.character_stats.loadJson(data.character_stats);
        } else {
            this.character_stats = new CharacterStats();
        }
        this.role_type = data.role_type ?? RoleTypeTableID.NONE;
    }
}
