D:
cd "D:\Github\luajit-decompiler"
rem python main.py -f "P:\SteamLibrary\steamapps\common\Eastward\content\game\_system\main.lua" -o "P:\SteamLibrary\steamapps\common\Eastward\content\game\script\main.lua"
rem python main.py -f "P:\SteamLibrary\steamapps\common\Eastward\content\game\_system\main_package.lua" -o "P:\SteamLibrary\steamapps\common\Eastward\content\game\script\main_package.lua"
python main.py --recursive "P:\SteamLibrary\steamapps\common\Eastward\content\game\script" --dir_out "P:\SteamLibrary\steamapps\common\Eastward\content\game\script_out" --catch_asserts
