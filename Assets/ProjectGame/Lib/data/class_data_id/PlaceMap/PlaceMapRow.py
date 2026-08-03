from ..BaseClassDataRow import BaseClassDataRow

class PlaceMapRow(BaseClassDataRow):
    def __init__(self):
        super().__init__()
        self.name = ""  # 名前
        self.use = False  # 使用フラグ
        self.place_map = 0  # プレスデータ（辞書）

    def read(self, reader):
        self.name = reader.read_string()
        self.use = reader.read_bool()
        self.place_map = dictionary()  # Unsupported

    @classmethod
    def from_json(cls, data: dict):
        self = cls()
        self.name = data.get('name', "")
        self.use = data.get('use', False)
        self.place_map = data.get('place_map', 0)
        return self
