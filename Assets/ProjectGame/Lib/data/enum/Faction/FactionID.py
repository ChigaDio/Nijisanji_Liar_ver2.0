from enum import IntEnum
from typing import Callable, List, Any


class FactionID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Human = 1  # 人類
    Werewolf = 2  # 人狼
    MAX = 3

def to_int(id: "FactionID") -> int:
    return int(id)


def to_FactionID(id: int) -> "FactionID":
    return FactionID(id)


def to_index(id: "FactionID") -> int:
    return int(id) - 1


def for_id(action: Callable[["FactionID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = FactionID.Human.value
    for i in range(start, FactionID.MAX.value):
        try:
            value = FactionID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["FactionID"], bool]) -> List["FactionID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["FactionID"] = []
    start = FactionID.Human.value
    for i in range(start, FactionID.MAX.value):
        try:
            value = FactionID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["FactionID"], bool]) -> "FactionID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = FactionID.Human.value
    for i in range(start, FactionID.MAX.value):
        try:
            value = FactionID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return FactionID.NONE
