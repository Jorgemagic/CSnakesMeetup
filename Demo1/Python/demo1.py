# Hello world
def hello_world(name: str) -> str:
    return f"Hello, {name}!"

# Classes
class Person:
    def __init__(self, name: str, age: int):
        self.name = name
        self.age = age

def create_person(name: str, age: int) -> Person:
    return Person(name, age)

# Async support
import asyncio
from typing import Coroutine

async def async_function() -> Coroutine[int, None, None]:
    await asyncio.sleep(1)
    return 42

# Crossing the bridge too frenquently
import numpy as np

def make_square_2d_array(n: int) -> np.ndarray:
    return np.zeros((n, n))

def set_random(arr, i: int, j: int) -> None:
    arr[i, j] = np.random.random()

# Marshalling return values unnecessarily
from typing import Any
def log_value(value: str) -> Any:
    print(f"Received value: {value}")
    return 1;

# Passing a large amount of data
def get_data1(data: list[float]) -> None:
    return

def get_data2(data: bytes) -> None:
    return;
    
# Buffer Protocol
import numpy as np
from collections.abc import Buffer

def example_array() -> Buffer:
    return np.array([True, False, True, False, False], dtype=np.bool_)

def example_array_2d() -> Buffer:
    return np.array([[1,2,3], [4,5,6]], dtype=np.int32)