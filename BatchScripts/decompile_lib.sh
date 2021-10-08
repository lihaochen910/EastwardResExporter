#!/bin/bash
cd "/Users/Kanbaru/GitWorkspace/luajit-decompiler"
python3 main.py --recursive "/Users/Kanbaru/Library/Application Support/Steam/steamapps/common/Eastward/content/game/lib" --dir_out "/Users/Kanbaru/Library/Application Support/Steam/steamapps/common/Eastward/content/game/lib_out" --catch_asserts
