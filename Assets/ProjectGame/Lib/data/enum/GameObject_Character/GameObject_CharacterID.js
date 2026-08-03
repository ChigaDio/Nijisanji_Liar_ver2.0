// GameObject_CharacterID - Pure JavaScript Version
export const GameObject_CharacterID = {
    None: 0,  // デフォルト値（C#互換）
    Prefab: 1,  // GameObject_Character_Prefab
    Max: 2
};

export const GameObject_CharacterIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toGameObject_CharacterID(id) {
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
        const start = GameObject_CharacterID.Prefab;
        const max = GameObject_CharacterID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_CharacterID).includes(id)) {
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
        const start = GameObject_CharacterID.Prefab;
        const max = GameObject_CharacterID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_CharacterID).includes(id)) {
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
        const start = GameObject_CharacterID.Prefab;
        const max = GameObject_CharacterID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameObject_CharacterID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return GameObject_CharacterID.None;
    }
};
