import { BaseClassDataRow } from '../BaseClassDataRow.js';
import { CharacterStats } from '../../../class_data/CharacterStats/CharacterStats.js';
import { GameObject_Character_Prefab } from '../../enum/GameObject_Character_Prefab/GameObject_Character_Prefab.js';

export class GuestCharacterRow extends BaseClassDataRow {
    constructor() {
        super();
        this.use = false; // 使用フラグ
        this.name = ""; // 名前
        this.characterStats = new CharacterStats(); // ステータス
        this.image_color = 0; // キャラのイメージカラー
        this.prefab_id = GameObject_Character_Prefab.NONE; // プレファブID
    }

    read(reader) {
        this.use = reader.readBoolean();
        this.name = reader.readString();
        this.characterStats = new CharacterStats();
        this.characterStats.read(reader);
        this.image_color = 0; // Unsupported
        this.prefab_id = GameObject_Character_Prefab.fromInt(reader.readInt32());
    }

    static fromJson(data) {
        const self = new this();
        self.use = data.use ?? false;
        self.name = data.name ?? "";
        if (data.characterStats !== undefined && data.characterStats !== null) {
            self.characterStats = new CharacterStats();
            self.characterStats.loadJson(data.characterStats);
        } else {
            self.characterStats = new CharacterStats();
        }
        self.image_color = data.image_color ?? 0;
        self.prefab_id = data.prefab_id ?? GameObject_Character_Prefab.NONE;
        return self;
    }
}
