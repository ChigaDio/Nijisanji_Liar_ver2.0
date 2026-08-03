// PlaceMapTableID - Pure JavaScript Version
export const PlaceMapTableID = {
    None: 0,  // デフォルト値（C#互換）
    PlaceMap_CafeMap: 1,  // カフェ
    PlaceMap_01: 2,  // 
    PlaceMap_02: 3,  // 
    PlaceMap_03: 4,  // 
    PlaceMap_04: 5,  // 
    PlaceMap_05: 6,  // 
    PlaceMap_06: 7,  // 
    PlaceMap_07: 8,  // 
    PlaceMap_08: 9,  // 
    PlaceMap_09: 10,  // 
    PlaceMap_10: 11,  // 
    PlaceMap_11: 12,  // 
    PlaceMap_12: 13,  // 
    PlaceMap_13: 14,  // 
    PlaceMap_14: 15,  // 
    PlaceMap_15: 16,  // 
    PlaceMap_16: 17,  // 
    PlaceMap_17: 18,  // 
    PlaceMap_18: 19,  // 
    PlaceMap_19: 20,  // 
    PlaceMap_20: 21,  // 
    PlaceMap_21: 22,  // 
    PlaceMap_22: 23,  // 
    PlaceMap_23: 24,  // 
    PlaceMap_24: 25,  // 
    PlaceMap_25: 26,  // 
    PlaceMap_26: 27,  // 
    PlaceMap_27: 28,  // 
    PlaceMap_28: 29,  // 
    PlaceMap_29: 30,  // 
    Max: 31
};

export const PlaceMapTableIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toPlaceMapTableID(id) {
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
        const start = PlaceMapTableID.PlaceMap_CafeMap;
        const max = PlaceMapTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(PlaceMapTableID).includes(id)) {
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
        const start = PlaceMapTableID.PlaceMap_CafeMap;
        const max = PlaceMapTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(PlaceMapTableID).includes(id)) {
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
        const start = PlaceMapTableID.PlaceMap_CafeMap;
        const max = PlaceMapTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(PlaceMapTableID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return PlaceMapTableID.None;
    }
};
