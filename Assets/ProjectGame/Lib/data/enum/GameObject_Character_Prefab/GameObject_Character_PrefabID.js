// GameObject_Character_PrefabID - Pure JavaScript Version
export const GameObject_Character_PrefabID = {
    None: 0,  // デフォルト値（C#互換）
    Ange: 1,  // アンジュ
    Ryushen: 2,  // 緑仙
    Belmond: 3,  // ベルモンド
    Himawari: 4,  // 本間ひわまり
    Mashiro: 5,  // ましろ
    Kuzuha: 6,  // 葛葉
    Max: 7
};

export const GameObject_Character_PrefabIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toGameObject_Character_PrefabID(id) {
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
        const start = GameObject_Character_PrefabID.Ange;
        const max = GameObject_Character_PrefabID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_Character_PrefabID).includes(id)) {
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
        const start = GameObject_Character_PrefabID.Ange;
        const max = GameObject_Character_PrefabID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_Character_PrefabID).includes(id)) {
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
        const start = GameObject_Character_PrefabID.Ange;
        const max = GameObject_Character_PrefabID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_Character_PrefabID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return GameObject_Character_PrefabID.None;
    }
};
