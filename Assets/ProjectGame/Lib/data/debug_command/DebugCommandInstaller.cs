namespace GameCore.DebugCommand
{
    // 登録済みの全DebugCommandをレジストリに登録する（自動生成・編集不要）
    public static class DebugCommandInstaller
    {
        public static void InstallAll()
        {
            DebugCommandRegistry.Register(new show_title_character_listDebugCommand());
        }
    }
}