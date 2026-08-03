from enum import IntEnum
from typing import Callable, List, Any


class PlaceMapTableID(IntEnum):
    NONE = 0  # デフォルト値（C#のNoneに相当）
    PlaceMap_CafeMap = 1  # カフェ
    PlaceMap_01 = 2  # 
    PlaceMap_02 = 3  # 
    PlaceMap_03 = 4  # 
    PlaceMap_04 = 5  # 
    PlaceMap_05 = 6  # 
    PlaceMap_06 = 7  # 
    PlaceMap_07 = 8  # 
    PlaceMap_08 = 9  # 
    PlaceMap_09 = 10  # 
    PlaceMap_10 = 11  # 
    PlaceMap_11 = 12  # 
    PlaceMap_12 = 13  # 
    PlaceMap_13 = 14  # 
    PlaceMap_14 = 15  # 
    PlaceMap_15 = 16  # 
    PlaceMap_16 = 17  # 
    PlaceMap_17 = 18  # 
    PlaceMap_18 = 19  # 
    PlaceMap_19 = 20  # 
    PlaceMap_20 = 21  # 
    PlaceMap_21 = 22  # 
    PlaceMap_22 = 23  # 
    PlaceMap_23 = 24  # 
    PlaceMap_24 = 25  # 
    PlaceMap_25 = 26  # 
    PlaceMap_26 = 27  # 
    PlaceMap_27 = 28  # 
    PlaceMap_28 = 29  # 
    PlaceMap_29 = 30  # 
    MAX = 31

def to_int(id: "PlaceMapTableID") -> int:
    return int(id)


def to_PlaceMapTableID(id: int) -> "PlaceMapTableID":
    return PlaceMapTableID(id)


def to_index(id: "PlaceMapTableID") -> int:
    return int(id) - 1


def for_id(action: Callable[["PlaceMapTableID"], None]):
    if action is None:
        raise ValueError("action cannot be None")
    start = PlaceMapTableID.PlaceMap_CafeMap.value
    for i in range(start, PlaceMapTableID.MAX.value):
        try:
            value = PlaceMapTableID(i)
            action(value)
        except ValueError:
            continue  # 未定義の値はスキップ


def find_all(predicate: Callable[["PlaceMapTableID"], bool]) -> List["PlaceMapTableID"]:
    if predicate is None:
        raise ValueError("predicate cannot be None")
    results: List["PlaceMapTableID"] = []
    start = PlaceMapTableID.PlaceMap_CafeMap.value
    for i in range(start, PlaceMapTableID.MAX.value):
        try:
            value = PlaceMapTableID(i)
            if predicate(value):
                results.append(value)
        except ValueError:
            continue
    return results


def find(predicate: Callable[["PlaceMapTableID"], bool]) -> "PlaceMapTableID":
    if predicate is None:
        raise ValueError("predicate cannot be None")
    start = PlaceMapTableID.PlaceMap_CafeMap.value
    for i in range(start, PlaceMapTableID.MAX.value):
        try:
            value = PlaceMapTableID(i)
            if predicate(value):
                return value
        except ValueError:
            continue
    return PlaceMapTableID.NONE
