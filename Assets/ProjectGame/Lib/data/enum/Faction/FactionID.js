// FactionID - Pure JavaScript Version
export const FactionID = {
    None: 0,  // デフォルト値（C#互換）
    Human: 1,  // 人類
    Werewolf: 2,  // 人狼
    Max: 3
};

export const FactionIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toFactionID(id) {
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
        const start = FactionID.Human;
        const max = FactionID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(FactionID).includes(id)) {
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
        const start = FactionID.Human;
        const max = FactionID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(FactionID).includes(id)) {
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
        const start = FactionID.Human;
        const max = FactionID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(FactionID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return FactionID.None;
    }
};
