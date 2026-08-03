
from abc import ABC, abstractmethod
from typing import Dict, Any


class BaseClassDataRow(ABC):
    @abstractmethod
    def read(self, reader):
        pass

    @classmethod
    def from_json(cls, data: dict) -> 'BaseClassDataRow':
        raise NotImplementedError("from_json must be implemented in subclass")
    