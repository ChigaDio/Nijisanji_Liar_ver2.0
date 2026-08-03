from enum import IntEnum
from typing import Callable, List, Any


class GameObjectID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Character_kuzuha = 1  # 葛葉
    Character_Ange = 2  # アンジュ
    Character_Ryushen = 3  # 緑仙
    Character_Belmond = 4  # ベルモンド
    Character_Himawari = 5  # 本間ひわまり
    Character_Mashiro = 6  # ましろ
    MAX = 7

def to_int(id: "GameObjectID") -> int:
    return int(id)


def to_GameObjectID(id: int) -> "GameObjectID":
    return GameObjectID(id)


def to_index(id: "GameObjectID") -> int:
    return int(id) - 1


def for_id(action: Callable[["GameObjectID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = GameObjectID.Character_kuzuha.value
    for i in range(start, GameObjectID.MAX.value):
        try:
            value = GameObjectID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["GameObjectID"], bool]) -> List["GameObjectID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["GameObjectID"] = []
    start = GameObjectID.Character_kuzuha.value
    for i in range(start, GameObjectID.MAX.value):
        try:
            value = GameObjectID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["GameObjectID"], bool]) -> "GameObjectID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = GameObjectID.Character_kuzuha.value
    for i in range(start, GameObjectID.MAX.value):
        try:
            value = GameObjectID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return GameObjectID.NONE
