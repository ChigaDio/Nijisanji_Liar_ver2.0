from enum import IntEnum
from typing import Callable, List, Any


class SeatPlaceID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    Place_01 = 1  # 
    Place_02 = 2  # 
    Place_03 = 3  # 
    Place_04 = 4  # 
    Place_05 = 5  # 
    Place_06 = 6  # 
    Place_07 = 7  # 
    Place_08 = 8  # 
    Place_09 = 9  # 
    Place_10 = 10  # 
    MAX = 11

def to_int(id: "SeatPlaceID") -> int:
    return int(id)


def to_SeatPlaceID(id: int) -> "SeatPlaceID":
    return SeatPlaceID(id)


def to_index(id: "SeatPlaceID") -> int:
    return int(id) - 1


def for_id(action: Callable[["SeatPlaceID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = SeatPlaceID.Place_01.value
    for i in range(start, SeatPlaceID.MAX.value):
        try:
            value = SeatPlaceID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["SeatPlaceID"], bool]) -> List["SeatPlaceID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["SeatPlaceID"] = []
    start = SeatPlaceID.Place_01.value
    for i in range(start, SeatPlaceID.MAX.value):
        try:
            value = SeatPlaceID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["SeatPlaceID"], bool]) -> "SeatPlaceID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = SeatPlaceID.Place_01.value
    for i in range(start, SeatPlaceID.MAX.value):
        try:
            value = SeatPlaceID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return SeatPlaceID.NONE
