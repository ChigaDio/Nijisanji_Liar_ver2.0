// Sound_UI_TitleID - Pure JavaScript Version
export const Sound_UI_TitleID = {
    None: 0,  // デフォルト値（C#互換）
    SelectMove: 1,  // 移動音
    PushEnter: 2,  // 決定音
    Max: 3
};

export const Sound_UI_TitleIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toSound_UI_TitleID(id) {
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
        const start = Sound_UI_TitleID.SelectMove;
        const max = Sound_UI_TitleID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(Sound_UI_TitleID).includes(id)) {
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
        const start = Sound_UI_TitleID.SelectMove;
        const max = Sound_UI_TitleID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(Sound_UI_TitleID).includes(id)) {
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
        const start = Sound_UI_TitleID.SelectMove;
        const max = Sound_UI_TitleID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(Sound_UI_TitleID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return Sound_UI_TitleID.None;
    }
};
