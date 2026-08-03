// SeatPlaceID - Pure JavaScript Version
export const SeatPlaceID = {
    None: 0,  // デフォルト値（C#互換）
    Place_01: 1,  // 
    Place_02: 2,  // 
    Place_03: 3,  // 
    Place_04: 4,  // 
    Place_05: 5,  // 
    Place_06: 6,  // 
    Place_07: 7,  // 
    Place_08: 8,  // 
    Place_09: 9,  // 
    Place_10: 10,  // 
    Max: 11
};

export const SeatPlaceIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toSeatPlaceID(id) {
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
        const start = SeatPlaceID.Place_01;
        const max = SeatPlaceID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(SeatPlaceID).includes(id)) {
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
        const start = SeatPlaceID.Place_01;
        const max = SeatPlaceID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(SeatPlaceID).includes(id)) {
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
        const start = SeatPlaceID.Place_01;
        const max = SeatPlaceID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(SeatPlaceID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return SeatPlaceID.None;
    }
};
