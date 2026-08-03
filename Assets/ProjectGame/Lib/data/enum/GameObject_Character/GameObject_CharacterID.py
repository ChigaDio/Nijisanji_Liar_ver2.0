from enum import IntEnum
from typing import Callable, List, Any


class GameObject_CharacterID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Prefab = 1  # GameObject_Character_Prefab
    MAX = 2

def to_int(id: "GameObject_CharacterID") -> int:
    return int(id)


def to_GameObject_CharacterID(id: int) -> "GameObject_CharacterID":
    return GameObject_CharacterID(id)


def to_index(id: "GameObject_CharacterID") -> int:
    return int(id) - 1


def for_id(action: Callable[["GameObject_CharacterID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = GameObject_CharacterID.Prefab.value
    for i in range(start, GameObject_CharacterID.MAX.value):
        try:
            value = GameObject_CharacterID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["GameObject_CharacterID"], bool]) -> List["GameObject_CharacterID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["GameObject_CharacterID"] = []
    start = GameObject_CharacterID.Prefab.value
    for i in range(start, GameObject_CharacterID.MAX.value):
        try:
            value = GameObject_CharacterID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["GameObject_CharacterID"], bool]) -> "GameObject_CharacterID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = GameObject_CharacterID.Prefab.value
    for i in range(start, GameObject_CharacterID.MAX.value):
        try:
            value = GameObject_CharacterID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return GameObject_CharacterID.NONE
