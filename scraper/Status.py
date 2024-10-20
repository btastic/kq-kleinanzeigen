from enum import Enum


class Status(Enum):
    """The status of an item"""
    AVAILABLE = 0
    RESERVED = 1
    DELETED = 2