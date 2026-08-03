import { BaseCustomClassData } from '../BaseCustomClassData.js';

export class BaseCharacterImpression extends BaseCustomClassData {
    constructor() {
        super();
        this.suspicion = 0.0; // 疑惑
        this.favorability = 0.0; // 友好度
    }

    read(view, offset) {
        let o = offset;
        this.suspicion = view.getFloat32(o, true); o += 4;
        this.favorability = view.getFloat32(o, true); o += 4;
        return o;
    }

    loadJson(data) {
        this.suspicion = data.suspicion ?? 0.0;
        this.favorability = data.favorability ?? 0.0;
    }
}
