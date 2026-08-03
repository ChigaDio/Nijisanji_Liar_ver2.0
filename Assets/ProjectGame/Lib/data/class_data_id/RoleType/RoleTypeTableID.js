// RoleTypeTableID - Pure JavaScript Version
export const RoleTypeTableID = {
    None: 0,  // デフォルト値（C#互換）
    Villager: 1,  // 村人
    Werewolf: 2,  // 人狼
    RoleType_02: 3,  // 
    RoleType_03: 4,  // 
    RoleType_04: 5,  // 
    RoleType_05: 6,  // 
    RoleType_06: 7,  // 
    RoleType_07: 8,  // 
    RoleType_08: 9,  // 
    RoleType_09: 10,  // 
    RoleType_10: 11,  // 
    Max: 12
};

export const RoleTypeTableIDExtensions = {
    /**
     * Enumを数値に変換
     */
    toInt(id) {
        return Number(id);
    },

    /**
     * 数値をEnumに変換
     */
    toRoleTypeTableID(id) {
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
        const start = RoleTypeTableID.Villager;
        const max = RoleTypeTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(RoleTypeTableID).includes(id)) {
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
        const start = RoleTypeTableID.Villager;
        const max = RoleTypeTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(RoleTypeTableID).includes(id)) {
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
        const start = RoleTypeTableID.Villager;
        const max = RoleTypeTableID.Max;
        for (let id = start; id < max; id++) {
            if (Object.values(RoleTypeTableID).includes(id)) {
                if (predicate(id)) {
                    return id;
                }
            }
        }
        return RoleTypeTableID.None;
    }
};
