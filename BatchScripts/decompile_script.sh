#!/bin/bash
pwd
cd "/Users/Kanbaru/GitWorkspace/luajit-decompiler"
pwd
python3 main.py --recursive "/Users/Kanbaru/Library/Application Support/Steam/steamapps/common/Eastward/content/game/script" --dir_out "/Users/Kanbaru/Library/Application Support/Steam/steamapps/common/Eastward/content/game/script_out" --catch_asserts
