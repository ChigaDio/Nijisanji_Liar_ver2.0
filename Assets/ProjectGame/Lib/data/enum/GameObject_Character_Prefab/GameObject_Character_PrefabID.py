from enum import IntEnum
from typing import Callable, List, Any


class GameObject_Character_PrefabID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Ange = 1  # アンジュ
    Ryushen = 2  # 緑仙
    Belmond = 3  # ベルモンド
    Himawari = 4  # 本間ひわまり
    Mashiro = 5  # ましろ
    Kuzuha = 6  # 葛葉
    MAX = 7

def to_int(id: "GameObject_Character_PrefabID") -> int:
    return int(id)


def to_GameObject_Character_PrefabID(id: int) -> "GameObject_Character_PrefabID":
    return GameObject_Character_PrefabID(id)


def to_index(id: "GameObject_Character_PrefabID") -> int:
    return int(id) - 1


def for_id(action: Callable[["GameObject_Character_PrefabID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = GameObject_Character_PrefabID.Ange.value
    for i in range(start, GameObject_Character_PrefabID.MAX.value):
        try:
            value = GameObject_Character_PrefabID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["GameObject_Character_PrefabID"], bool]) -> List["GameObject_Character_PrefabID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["GameObject_Character_PrefabID"] = []
    start = GameObject_Character_PrefabID.Ange.value
    for i in range(start, GameObject_Character_PrefabID.MAX.value):
        try:
            value = GameObject_Character_PrefabID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["GameObject_Character_PrefabID"], bool]) -> "GameObject_Character_PrefabID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = GameObject_Character_PrefabID.Ange.value
    for i in range(start, GameObject_Character_PrefabID.MAX.value):
        try:
            value = GameObject_Character_PrefabID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return GameObject_Character_PrefabID.NONE
