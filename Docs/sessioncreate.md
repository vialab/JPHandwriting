# Create Session Script

The `create_session.py` script is located in the repository root. It reads from the `VocabItems` list in `vocab_items.json`, also located in the root.

To add a new item, simply add it as a new element to the `VocabItems` list.

Run the script like so: 

```sh
python ./create_session.py [-h] --id ID [--items ITEMS] [--traced-items TRACED_ITEMS]
```

where `ID` is the user's ID, `ITEMS` is the number of items to give the user to learn, and `TRACED_ITEMS` is the number of items to enable tracing on.