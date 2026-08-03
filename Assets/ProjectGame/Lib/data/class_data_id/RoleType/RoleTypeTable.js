import { BaseClassDataID } from '../BaseClassDataID.js';
import { RoleTypeRow } from './RoleTypeRow.js';
import { RoleTypeTableID } from './RoleTypeTableID.js';

export class RoleTypeTable extends BaseClassDataID {
    static Table = new Map();

    static _getEnum(name) {
        return RoleTypeTableID[name];
    }

    static _getRowClass() {
        return RoleTypeRow;
    }

    read(reader) {
        this.constructor.Table.clear();
        const rowCount = reader.readInt32();
        const colCount = reader.readInt32();
        for (let i = 0; i < colCount; i++) {
            const lenName = reader.readInt32();
            reader.readString(); // col name（ヘッダー読み飛ばし）
            const lenType = reader.readInt32();
            reader.readString(); // col type
        }
        for (let r = 0; r < rowCount; r++) {
            const enumInt = reader.readInt32();
            const enumVal = RoleTypeTableID[Object.keys(RoleTypeTableID).find(key => RoleTypeTableID[key] === enumInt)] || enumInt;
            const row = new RoleTypeRow();
            row.read(reader);
            this.constructor.Table.set(enumVal, row);
        }
    }

    static getRow(id) {
        return this.Table.get(id) ?? null;
    }
}
