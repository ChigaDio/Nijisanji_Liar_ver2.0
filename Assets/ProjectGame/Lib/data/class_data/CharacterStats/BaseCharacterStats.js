import { BaseCustomClassData } from '../BaseCustomClassData.js';

export class BaseCharacterStats extends BaseCustomClassData {
    constructor() {
        super();
        this.charisma = 0.0; // カリスマ
        this.intuition = 0.0; // 直感
        this.reasoning = 0.0; // ロジック
        this.appeal = 0.0; // 可愛さ
        this.deception = 0.0; // 演技力
        this.stealth = 0.0; // ステルス
    }

    read(view, offset) {
        let o = offset;
        this.charisma = view.getFloat32(o, true); o += 4;
        this.intuition = view.getFloat32(o, true); o += 4;
        this.reasoning = view.getFloat32(o, true); o += 4;
        this.appeal = view.getFloat32(o, true); o += 4;
        this.deception = view.getFloat32(o, true); o += 4;
        this.stealth = view.getFloat32(o, true); o += 4;
        return o;
    }

    loadJson(data) {
        this.charisma = data.charisma ?? 0.0;
        this.intuition = data.intuition ?? 0.0;
        this.reasoning = data.reasoning ?? 0.0;
        this.appeal = data.appeal ?? 0.0;
        this.deception = data.deception ?? 0.0;
        this.stealth = data.stealth ?? 0.0;
    }
}
