// GameObjectID - Pure JavaScript Version
export const GameObjectID = {
    None: 0,  // デフォルト値（C#互換）
    Character_kuzuha: 1,  // 葛葉
    Character_Ange: 2,  // アンジュ
    Character_Ryushen: 3,  // 緑仙
    Character_Belmond: 4,  // ベルモンド
    Character_Himawari: 5,  // 本間ひわまり
    Character_Mashiro: 6,  // ましろ
    Max: 7
};

export const GameObjectIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toGameObjectID(id) {
        return id;
    },

    /**
     * 0-based indexに変換
     */
    toIndex(id) {
        return Number(id) - 1;
    },

    /**
     * すべてのIDに対して処理を実行
     */
    forID(action) {
        if (typeof action !== 'function') {
            throw new Error('action must be a function');
        }
        const start = GameObjectID.Character_kuzuha;
        const max = GameObjectID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObjectID).includes(id)) {
                action(id);
            }
        }
    },

    /**
     * 条件に合うすべてのIDを返す
     */
    findAll(predicate) {
        if (typeof predicate !== 'function') {
            throw new Error('predicate must be a function');
        }
        const results = [];
        const start = GameObjectID.Character_kuzuha;
        const max = GameObjectID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObjectID).includes(id)) {
                if (predicate(id)) {
                    results.push(id);
                }
            }
        }
        return results;
    },

    /**
     * 条件に合う最初のIDを返す（見つからなければ None）
     */
    find(predicate) {
        if (typeof predicate !== 'function') {
            throw new Error('predicate must be a function');
        }
        const start = GameObjectID.Character_kuzuha;
        const max = GameObjectID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObjectID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return GameObjectID.None;
    }
};
