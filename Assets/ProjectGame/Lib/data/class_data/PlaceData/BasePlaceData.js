import { BaseCustomClassData } from '../BaseCustomClassData.js';

export class BasePlaceData extends BaseCustomClassData {
    constructor() {
        super();
        this.position = [0.0, 0.0, 0.0]; // 座標
    }

    read(view, offset) {
        let o = offset;
        this.position = [view.getFloat32(o, true), view.getFloat32(o + 4, true), view.getFloat32(o + 8, true)]; o += 12;
        return o;
    }

    loadJson(data) {
        this.position = data.position ?? [0.0, 0.0, 0.0];
    }
}
