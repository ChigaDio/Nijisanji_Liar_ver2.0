import { BaseClassDataID } from '../BaseClassDataID.js';
import { GuestCharacterRow } from './GuestCharacterRow.js';
import { GuestCharacterTableID } from './GuestCharacterTableID.js';

export class GuestCharacterTable extends BaseClassDataID {
    static Table = new Map();

    static _getEnum(name) {
        return GuestCharacterTableID[name];
    }

    static _getRowClass() {
        return GuestCharacterRow;
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
            const enumVal = GuestCharacterTableID[Object.keys(GuestCharacterTableID).find(key => GuestCharacterTableID[key] === enumInt)] || enumInt;
            const row = new GuestCharacterRow();
            row.read(reader);
            this.constructor.Table.set(enumVal, row);
        }
    }

    static getRow(id) {
        return this.Table.get(id) ?? null;
    }
}
