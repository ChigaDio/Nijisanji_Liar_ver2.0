from ..BaseCustomClassData import BaseCustomClassData

class BaseCharacterImpression(BaseCustomClassData):
    def __init__(self):
        super().__init__()
        self.suspicion = 0.0  # 疑惑
        self.favorability = 0.0  # 友好度

    def read(self, reader):
        self.suspicion = reader.read_float()
        self.favorability = reader.read_float()

    def load_json(self, data):
        self.suspicion = data.get('suspicion', 0.0)
        self.favorability = data.get('favorability', 0.0)
