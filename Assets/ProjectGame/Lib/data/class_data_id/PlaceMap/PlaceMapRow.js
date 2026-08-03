import { BaseClassDataRow } from '../BaseClassDataRow.js';

export class PlaceMapRow extends BaseClassDataRow {
    constructor() {
        super();
        this.name = ""; // 名前
        this.use = false; // 使用フラグ
        this.place_map = 0; // プレスデータ（辞書）
    }

    read(reader) {
        this.name = reader.readString();
        this.use = reader.readBoolean();
        this.place_map = 0; // Unsupported
    }

    static fromJson(data) {
        const self = new this();
        self.name = data.name ?? "";
        self.use = data.use ?? false;
        self.place_map = data.place_map ?? 0;
        return self;
    }
}
