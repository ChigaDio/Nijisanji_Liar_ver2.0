from enum import IntEnum
from typing import Callable, List, Any


class Sound_TitleID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Main = 1  # Sound_Title_Main
    MAX = 2

def to_int(id: "Sound_TitleID") -> int:
    return int(id)


def to_Sound_TitleID(id: int) -> "Sound_TitleID":
    return Sound_TitleID(id)


def to_index(id: "Sound_TitleID") -> int:
    return int(id) - 1


def for_id(action: Callable[["Sound_TitleID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = Sound_TitleID.Main.value
    for i in range(start, Sound_TitleID.MAX.value):
        try:
            value = Sound_TitleID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["Sound_TitleID"], bool]) -> List["Sound_TitleID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["Sound_TitleID"] = []
    start = Sound_TitleID.Main.value
    for i in range(start, Sound_TitleID.MAX.value):
        try:
            value = Sound_TitleID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["Sound_TitleID"], bool]) -> "Sound_TitleID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = Sound_TitleID.Main.value
    for i in range(start, Sound_TitleID.MAX.value):
        try:
            value = Sound_TitleID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return Sound_TitleID.NONE
