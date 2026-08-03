from ..BaseClassDataRow import BaseClassDataRow
from ...enum.Faction.Faction import Faction

class RoleTypeRow(BaseClassDataRow):
    def __init__(self):
        super().__init__()
        self.name = ""  # 役職名
        self.faction_id = Faction.NONE  # 所属ID
        self.use = False  # 使用フラグ

    def read(self, reader):
        self.name = reader.read_string()
        self.faction_id = Faction(reader.read_int32())
        self.use = reader.read_bool()

    @classmethod
    def from_json(cls, data: dict):
        self = cls()
        self.name = data.get('name', "")
        self.faction_id = data.get('faction_id', Faction.NONE)
        self.use = data.get('use', False)
        return self
