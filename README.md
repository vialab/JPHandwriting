# KikuKaku 

This is the virtual reality component for the KikuKaku undergraduate honors thesis. It has so far only been tested on an Oculus Quest 2, though theoretically it should work on any VR headset that the OpenXR SDK supports.

Also see: the [character recognition API](https://github.com/vialab/JPHandwritingModel) and the web-facing component (TBD).

# Setup

## Sessions

Sessions for each user are loaded from a .json file. These files can be created via a Python script in the root. Run

```sh
python3 create_session.py -h
``` 

to see what it does. 

The resulting file is saved as `SessionData/[userID].json` and a folder named after the ID is created in `SessionLogs`.

## Program

1. Make sure you have Unity 2022.3.11f1 installed.
2. Clone this repository and the [character recognition API repository](https://github.com/vialab/JPHandwritingModel). (Make sure the API repo is checked out to the `fastapi` branch.)
3. Set up the API (there are instructions on its repo).
4. Open this repository in Unity.
5. Fill the user's ID in by clicking on the Game object and filling in the "User ID" field in the Inspector.
6. Put on your VR headset and press Play.

# Contributing

As of 2024-11-28, commit messages follow the [Conventional Commits](https://www.conventionalcommits.org/) format. Install [pre-commit](https://pre-commit.com/) with a Python package manager globally in order to commit changes.

# Credits
See [CREDITS.md](CREDITS.md).
