from ..BaseCustomClassData import BaseCustomClassData

class BaseCharacterStats(BaseCustomClassData):
    def __init__(self):
        super().__init__()
        self.charisma = 0.0  # カリスマ
        self.intuition = 0.0  # 直感
        self.reasoning = 0.0  # ロジック
        self.appeal = 0.0  # 可愛さ
        self.deception = 0.0  # 演技力
        self.stealth = 0.0  # ステルス

    def read(self, reader):
        self.charisma = reader.read_float()
        self.intuition = reader.read_float()
        self.reasoning = reader.read_float()
        self.appeal = reader.read_float()
        self.deception = reader.read_float()
        self.stealth = reader.read_float()

    def load_json(self, data):
        self.charisma = data.get('charisma', 0.0)
        self.intuition = data.get('intuition', 0.0)
        self.reasoning = data.get('reasoning', 0.0)
        self.appeal = data.get('appeal', 0.0)
        self.deception = data.get('deception', 0.0)
        self.stealth = data.get('stealth', 0.0)
