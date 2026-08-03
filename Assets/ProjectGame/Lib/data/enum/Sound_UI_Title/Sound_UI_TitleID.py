from enum import IntEnum
from typing import Callable, List, Any


class Sound_UI_TitleID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    SelectMove = 1  # 移動音
    PushEnter = 2  # 決定音
    MAX = 3

def to_int(id: "Sound_UI_TitleID") -> int:
    return int(id)


def to_Sound_UI_TitleID(id: int) -> "Sound_UI_TitleID":
    return Sound_UI_TitleID(id)


def to_index(id: "Sound_UI_TitleID") -> int:
    return int(id) - 1


def for_id(action: Callable[["Sound_UI_TitleID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = Sound_UI_TitleID.SelectMove.value
    for i in range(start, Sound_UI_TitleID.MAX.value):
        try:
            value = Sound_UI_TitleID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["Sound_UI_TitleID"], bool]) -> List["Sound_UI_TitleID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["Sound_UI_TitleID"] = []
    start = Sound_UI_TitleID.SelectMove.value
    for i in range(start, Sound_UI_TitleID.MAX.value):
        try:
            value = Sound_UI_TitleID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["Sound_UI_TitleID"], bool]) -> "Sound_UI_TitleID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = Sound_UI_TitleID.SelectMove.value
    for i in range(start, Sound_UI_TitleID.MAX.value):
        try:
            value = Sound_UI_TitleID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return Sound_UI_TitleID.NONE
