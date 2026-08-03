from enum import IntEnum
from typing import Callable, List, Any


class RoleTypeTableID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Villager = 1  # 村人
    Werewolf = 2  # 人狼
    RoleType_02 = 3  # 
    RoleType_03 = 4  # 
    RoleType_04 = 5  # 
    RoleType_05 = 6  # 
    RoleType_06 = 7  # 
    RoleType_07 = 8  # 
    RoleType_08 = 9  # 
    RoleType_09 = 10  # 
    RoleType_10 = 11  # 
    MAX = 12

def to_int(id: "RoleTypeTableID") -> int:
    return int(id)


def to_RoleTypeTableID(id: int) -> "RoleTypeTableID":
    return RoleTypeTableID(id)


def to_index(id: "RoleTypeTableID") -> int:
    return int(id) - 1


def for_id(action: Callable[["RoleTypeTableID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = RoleTypeTableID.Villager.value
    for i in range(start, RoleTypeTableID.MAX.value):
        try:
            value = RoleTypeTableID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["RoleTypeTableID"], bool]) -> List["RoleTypeTableID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["RoleTypeTableID"] = []
    start = RoleTypeTableID.Villager.value
    for i in range(start, RoleTypeTableID.MAX.value):
        try:
            value = RoleTypeTableID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["RoleTypeTableID"], bool]) -> "RoleTypeTableID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = RoleTypeTableID.Villager.value
    for i in range(start, RoleTypeTableID.MAX.value):
        try:
            value = RoleTypeTableID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return RoleTypeTableID.NONE
