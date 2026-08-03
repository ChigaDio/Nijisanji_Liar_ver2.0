from ..BaseCustomClassData import BaseCustomClassData

class BasePlaceData(BaseCustomClassData):
    def __init__(self):
        super().__init__()
        self.position = [0.0, 0.0, 0.0]  # 座標

    def read(self, reader):
        self.position = [reader.read_float(), reader.read_float(), reader.read_float()]

    def load_json(self, data):
        self.position = data.get('position', [0.0, 0.0, 0.0])
