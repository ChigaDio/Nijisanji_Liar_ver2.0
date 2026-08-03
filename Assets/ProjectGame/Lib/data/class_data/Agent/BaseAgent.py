from ..BaseCustomClassData import BaseCustomClassData
from ...class_data.CharacterStats.CharacterStats import CharacterStats
from ...class_data_id.GuestCharacter.GuestCharacterTableID import GuestCharacterTableID
from ...class_data_id.RoleType.RoleTypeTableID import RoleTypeTableID

class BaseAgent(BaseCustomClassData):
    def __init__(self):
        super().__init__()
        self.guest_character_id = GuestCharacterTableID.NONE  # ゲストID(Noneならプレイヤー)
        self.character_stats = CharacterStats()  # キャラステータス
        self.role_type = RoleTypeTableID.NONE  # 役職ID

    def read(self, reader):
        self.guest_character_id = GuestCharacterTableID(reader.read_int32())
        self.character_stats = CharacterStats()
        self.character_stats.read(reader)
        self.role_type = RoleTypeTableID(reader.read_int32())

    def load_json(self, data):
        self.guest_character_id = data.get('guest_character_id', GuestCharacterTableID.NONE)
        if 'character_stats' in data and data['character_stats'] is not None:
            self.character_stats = CharacterStats()
            self.character_stats.load_json(data['character_stats'])
        else:
            self.character_stats = CharacterStats()
        self.role_type = data.get('role_type', RoleTypeTableID.NONE)
