import argparse
import json
import os
import pathlib
import random
from typing import Any

# prompt user for User ID, number of items, and number of items with tracing on
# limitations: only built-in libraries

MAX_ITEMS = 3

class VocabItem:
    def __init__(self, name: str, tracing: bool) -> None:
        self.Type = name
        self.EnableTracing = tracing 

    def to_json(self) -> dict[str, str]:
        return {
            "Type": self.Type, 
            "Data": json.dumps({"EnableTracing": self.EnableTracing})
        }

def import_items() -> list[str]:
    with open("vocab_items.json", "r") as file:
        contents = json.load(file)
        return contents["VocabItems"]

def generate_items(num_items: int, num_tracing: int) -> list:
    vocab = import_items()

    generated_vocab = []
    num_traced = 0

    for i in range(num_items):
        tracing = False

        if (num_traced < num_tracing):
            tracing = bool(random.getrandbits(1))

            if (tracing):
                num_traced += 1

        vocab_item = vocab.pop(random.randint(0, len(vocab) - 1))
        generated_vocab.append(VocabItem(vocab_item, tracing))

    return generated_vocab

def generate_json(items: list[VocabItem], id: str) -> dict[str, Any]:
    return {
        "UserID": id,
        "VocabItems": [x.to_json() for x in items]
    }

def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description = "Automatically creates sessions for users for user studies.",
    )

    parser.add_argument(
        "--id", 
        type = str,
        required = True,
        help = "The user ID to generate a session file for."
    )

    parser.add_argument(
        "--items",
        type = int,
        default = MAX_ITEMS,
        help = "The number of items to generate."
    )

    parser.add_argument(
        "--traced-items",
        type = int,
        default = MAX_ITEMS // 2,
        help = "The number of items to enable tracing for."
    )

    return parser.parse_args()

def main():
    args = parse_args()
    contents = generate_json(generate_items(args.items, args.traced_items), args.id)

    with open(os.path.join("SessionData", f"{args.id}.json"), "w") as file:
        json_contents = json.dumps(contents)
        file.write(json_contents)

    print(f"Session has been generated and saved in the SessionData folder as {args.id}.json")

    pathlib.Path(f"SessionLogs/{args.id}").mkdir(parents = True, exist_ok = True)
    print(f"Created SessionLogs/{args.id} folder")

if __name__ == "__main__":
    main()