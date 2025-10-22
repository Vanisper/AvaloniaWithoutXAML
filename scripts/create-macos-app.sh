#!/bin/bash

# 创建 macOS 应用包的脚本
# 用法: ./create-macos-app.sh <项目名称> <发布目录>

set -e

# 检查参数
if [ $# -ne 2 ]; then
    echo "用法: $0 <项目名称> <发布目录>"
    echo "示例: $0 HelloWorldLabel ./publish/HelloWorldLabel"
    exit 1
fi

PROJECT_NAME="$1"
PUBLISH_DIR="$2"
APP_NAME="${PROJECT_NAME}.app"
CONTENTS_DIR="Release/${APP_NAME}/Contents"
MACOS_DIR="${CONTENTS_DIR}/MacOS"
RESOURCES_DIR="${CONTENTS_DIR}/Resources"

echo "正在创建 macOS 应用包: ${APP_NAME}"

# 创建应用包目录结构
mkdir -p "${MACOS_DIR}"
mkdir -p "${RESOURCES_DIR}"

# 复制可执行文件和依赖库
if [ -d "${PUBLISH_DIR}" ]; then
    cp -r "${PUBLISH_DIR}"/* "${MACOS_DIR}/"
else
    echo "错误: 发布目录 ${PUBLISH_DIR} 不存在"
    exit 1
fi

# 复制图标文件（如果存在）
if [ -f "Resources/icon.icns" ]; then
    cp "Resources/icon.icns" "${RESOURCES_DIR}/"
fi

# 创建 Info.plist 文件
cat > "${CONTENTS_DIR}/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleIconFile</key>
    <string>icon.icns</string>
    <key>CFBundleIdentifier</key>
    <string>com.company.${PROJECT_NAME}</string>
    <key>CFBundleName</key>
    <string>${PROJECT_NAME}</string>
    <key>CFBundleVersion</key>
    <string>1</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.12</string>
    <key>CFBundleExecutable</key>
    <string>${PROJECT_NAME}</string>
    <key>CFBundleInfoDictionaryVersion</key>
    <string>6.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
</dict>
</plist>
EOF

echo "macOS 应用包创建完成: Release/${APP_NAME}"
echo "你可以双击运行或使用 'open Release/${APP_NAME}' 命令启动应用"