from enum import IntEnum
from typing import Callable, List, Any


class GameSceneID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    MorningRoom = 1  # MorningRoom
    MAX = 2

def to_int(id: "GameSceneID") -> int:
    return int(id)


def to_GameSceneID(id: int) -> "GameSceneID":
    return GameSceneID(id)


def to_index(id: "GameSceneID") -> int:
    return int(id) - 1


def for_id(action: Callable[["GameSceneID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = GameSceneID.MorningRoom.value
    for i in range(start, GameSceneID.MAX.value):
        try:
            value = GameSceneID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["GameSceneID"], bool]) -> List["GameSceneID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["GameSceneID"] = []
    start = GameSceneID.MorningRoom.value
    for i in range(start, GameSceneID.MAX.value):
        try:
            value = GameSceneID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["GameSceneID"], bool]) -> "GameSceneID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = GameSceneID.MorningRoom.value
    for i in range(start, GameSceneID.MAX.value):
        try:
            value = GameSceneID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return GameSceneID.NONE
