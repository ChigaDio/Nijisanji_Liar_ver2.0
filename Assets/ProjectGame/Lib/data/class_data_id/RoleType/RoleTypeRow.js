import { BaseClassDataRow } from '../BaseClassDataRow.js';
import { Faction } from '../../enum/Faction/Faction.js';

export class RoleTypeRow extends BaseClassDataRow {
    constructor() {
        super();
        this.name = ""; // 役職名
        this.faction_id = Faction.NONE; // 所属ID
        this.use = false; // 使用フラグ
    }

    read(reader) {
        this.name = reader.readString();
        this.faction_id = Faction.fromInt(reader.readInt32());
        this.use = reader.readBoolean();
    }

    static fromJson(data) {
        const self = new this();
        self.name = data.name ?? "";
        self.faction_id = data.faction_id ?? Faction.NONE;
        self.use = data.use ?? false;
        return self;
    }
}
