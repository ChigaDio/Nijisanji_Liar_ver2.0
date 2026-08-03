
import os
import sys

if getattr(sys, 'frozen', False):
    # exe実行時
    # 一つ前
    base_dir = os.path.abspath(os.path.join(sys.executable, ".."))
else:
    # 開発時
    base_dir = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))

if base_dir not in sys.path:
    sys.path.append(base_dir)

isDbg = True
# 実行可能ファイルのディレクトリを取得（PyInstaller対応）
if getattr(sys, 'frozen', False):
    # PyInstallerでビルドされた場合
    BASE_DIR = os.path.dirname(sys.executable)
    isDbg = False
else:
    # デバッグ環境（VS Codeなど）
    # main-support/ の1つ上のディレクトリ（project/）を基準にする
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    
def main():
    pass

if __name__ == "__main__":
    main()
    
