from ..BaseClassDataID import BaseClassDataID
from .PlaceMapRow import PlaceMapRow
from .PlaceMapTableID import PlaceMapTableID
from typing import Dict

class PlaceMapTable(BaseClassDataID):
    Table: Dict[PlaceMapTableID, PlaceMapRow] = {}

    @classmethod
    def _get_enum(cls, name: str):
        return PlaceMapTableID[name]
    @classmethod
    def _get_row_class(cls):
        return PlaceMapRow

    def read(self, reader):
        self.Table.clear()
        row_count = reader.read_int32()
        col_count = reader.read_int32()
        for _ in range(col_count):
            len_name = reader.read_int32()
            _ = reader.read_string()  # col name（ヘッダー読み飛ばし）
            len_type = reader.read_int32()
            _ = reader.read_string()  # col type
        for _ in range(row_count):
            enum_int = reader.read_int32()
            enum_val = PlaceMapTableID(enum_int)
            row = PlaceMapRow()
            row.read(reader)
            self.Table[enum_val] = row
