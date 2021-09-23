# Eastward Resources Exporter

### Project "GPackTools"
* 包含解析.g的文件流
* 还有一些方便处理解包后的文件名的工具方法

### Project "AssetIndexPeeker"
* 遍历asset_index统计出现过的AssetNode类型
* 遍历asset_index统计出现过的ObjectFiles类型

### Project "FixLibExt"
* 用于解包lib.g后，出现的文件都以‘_'结尾，进行批量处理结尾重命名为.lua

### Project "ScriptLibraryExporter"
* 用于导出script.g中出现的所有脚本文件，并还原为asset_index中定义的原资源名

### Project "ShaderScriptExporter"
* 用于导出script.g中出现的所有shader源文件(shader/vsh/fsh)，并还原为asset_index中定义的原资源名

### BatchScripts
* 目录中包含解包script、lib后将luajit字节码还原为lua源代码
