def hello_world(name: str) -> str:
    return f"Hello, {name}!"

class Person:
    def __init__(self, name: str, age: int):
        self.name = name
        self.age = age

def create_person(name: str, age: int) -> Person:
    return Person(name, age)