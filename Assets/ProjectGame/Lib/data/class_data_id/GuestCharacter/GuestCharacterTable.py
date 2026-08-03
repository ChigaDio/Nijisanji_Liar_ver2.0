from ..BaseClassDataID import BaseClassDataID
from .GuestCharacterRow import GuestCharacterRow
from .GuestCharacterTableID import GuestCharacterTableID
from typing import Dict

class GuestCharacterTable(BaseClassDataID):
    Table: Dict[GuestCharacterTableID, GuestCharacterRow] = {}

    @classmethod
    def _get_enum(cls, name: str):
        return GuestCharacterTableID[name]
    @classmethod
    def _get_row_class(cls):
        return GuestCharacterRow

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
            enum_val = GuestCharacterTableID(enum_int)
            row = GuestCharacterRow()
            row.read(reader)
            self.Table[enum_val] = row
