
from abc import ABC, abstractmethod


class BaseTable(ABC):
    @abstractmethod
    def read(self, reader):
        pass

    @abstractmethod
    def release(self):
        pass
    