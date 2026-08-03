from enum import IntEnum
from typing import Callable, List, Any


class Sound_UIID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Title = 1  # Sound_UI_Title
    MAX = 2

def to_int(id: "Sound_UIID") -> int:
    return int(id)


def to_Sound_UIID(id: int) -> "Sound_UIID":
    return Sound_UIID(id)


def to_index(id: "Sound_UIID") -> int:
    return int(id) - 1


def for_id(action: Callable[["Sound_UIID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = Sound_UIID.Title.value
    for i in range(start, Sound_UIID.MAX.value):
        try:
            value = Sound_UIID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["Sound_UIID"], bool]) -> List["Sound_UIID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["Sound_UIID"] = []
    start = Sound_UIID.Title.value
    for i in range(start, Sound_UIID.MAX.value):
        try:
            value = Sound_UIID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["Sound_UIID"], bool]) -> "Sound_UIID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = Sound_UIID.Title.value
    for i in range(start, Sound_UIID.MAX.value):
        try:
            value = Sound_UIID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return Sound_UIID.NONE
