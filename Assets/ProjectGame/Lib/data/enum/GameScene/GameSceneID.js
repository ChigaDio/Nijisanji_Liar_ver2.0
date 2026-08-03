// GameSceneID - Pure JavaScript Version
export const GameSceneID = {
    None: 0,  // デフォルト値（C#互換）
    MorningRoom: 1,  // MorningRoom
    Max: 2
};

export const GameSceneIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toGameSceneID(id) {
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
        const start = GameSceneID.MorningRoom;
        const max = GameSceneID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameSceneID).includes(id)) {
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
        const start = GameSceneID.MorningRoom;
        const max = GameSceneID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameSceneID).includes(id)) {
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
        const start = GameSceneID.MorningRoom;
        const max = GameSceneID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(GameSceneID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return GameSceneID.None;
    }
};
