from enum import IntEnum
from typing import Callable, List, Any


class Sound_Title_MainID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Main_BGM = 1  # タイトルのBGM
    MAX = 2

def to_int(id: "Sound_Title_MainID") -> int:
    return int(id)


def to_Sound_Title_MainID(id: int) -> "Sound_Title_MainID":
    return Sound_Title_MainID(id)


def to_index(id: "Sound_Title_MainID") -> int:
    return int(id) - 1


def for_id(action: Callable[["Sound_Title_MainID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = Sound_Title_MainID.Main_BGM.value
    for i in range(start, Sound_Title_MainID.MAX.value):
        try:
            value = Sound_Title_MainID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["Sound_Title_MainID"], bool]) -> List["Sound_Title_MainID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["Sound_Title_MainID"] = []
    start = Sound_Title_MainID.Main_BGM.value
    for i in range(start, Sound_Title_MainID.MAX.value):
        try:
            value = Sound_Title_MainID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["Sound_Title_MainID"], bool]) -> "Sound_Title_MainID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = Sound_Title_MainID.Main_BGM.value
    for i in range(start, Sound_Title_MainID.MAX.value):
        try:
            value = Sound_Title_MainID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return Sound_Title_MainID.NONE
