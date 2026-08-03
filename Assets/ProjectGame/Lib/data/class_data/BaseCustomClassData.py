
from abc import ABC, abstractmethod
class BaseCustomClassData(ABC):
    @abstractmethod
    def read(self, reader):
        pass

    def load_json(self, data):
        pass
    