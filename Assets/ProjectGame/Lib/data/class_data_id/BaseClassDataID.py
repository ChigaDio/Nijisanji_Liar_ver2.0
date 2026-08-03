
from abc import ABC
from typing import Dict
from enum import Enum
from BaseTable import BaseTable
from BaseClassDataRow import BaseClassDataRow


class BaseClassDataID(BaseTable, ABC):
    Table: Dict[Enum, BaseClassDataRow] = {}

    def release(self):
        self.__class__.Table.clear()

    @classmethod
    def load_from_json(cls, json_data: dict):
        cls.Table.clear()
        for enum_name, row_data in json_data.items():
            try:
                enum_val = cls._get_enum(enum_name)
            except (KeyError, AttributeError):
                raise ValueError(f"Unknown enum name: {enum_name}")
            row = cls._get_row_class().from_json(row_data)
            cls.Table[enum_val] = row

    @classmethod
    def _get_enum(cls, name: str):
        raise NotImplementedError("サブクラスでオーバーライドしてください")

    @classmethod
    def _get_row_class(cls):
        raise NotImplementedError("サブクラスでオーバーライドしてください")
    