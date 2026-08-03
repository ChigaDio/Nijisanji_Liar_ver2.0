from ..BaseClassDataRow import BaseClassDataRow
from ...class_data.CharacterStats.CharacterStats import CharacterStats
from ...enum.GameObject_Character_Prefab.GameObject_Character_Prefab import GameObject_Character_Prefab

class GuestCharacterRow(BaseClassDataRow):
    def __init__(self):
        super().__init__()
        self.use = False  # 使用フラグ
        self.name = ""  # 名前
        self.characterStats = CharacterStats()  # ステータス
        self.image_color = 0  # キャラのイメージカラー
        self.prefab_id = GameObject_Character_Prefab.NONE  # プレファブID

    def read(self, reader):
        self.use = reader.read_bool()
        self.name = reader.read_string()
        self.characterStats = CharacterStats()
        self.characterStats.read(reader)
        self.image_color = color()  # Unsupported
        self.prefab_id = GameObject_Character_Prefab(reader.read_int32())

    @classmethod
    def from_json(cls, data: dict):
        self = cls()
        self.use = data.get('use', False)
        self.name = data.get('name', "")
        if 'characterStats' in data and data['characterStats'] is not None:
            self.characterStats = CharacterStats()
            self.characterStats.load_json(data['characterStats'])
        else:
            self.characterStats = CharacterStats()
        self.image_color = data.get('image_color', 0)
        self.prefab_id = data.get('prefab_id', GameObject_Character_Prefab.NONE)
        return self
