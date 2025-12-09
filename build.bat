@echo off
echo 正在编译 GitSwitchTool.exe ...

:: 调用 Windows 内置的 C# 编译器 (csc.exe)
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:Git切换助手.exe GitSwitchTool.cs

if %errorlevel% == 0 (
    echo.
    echo [成功] 编译完成！请查看目录下的 "GitHelper.exe"
    echo 你可以将这个 exe 复制到任何 Git 项目根目录下使用。
) else (
    echo.
    echo [失败] 编译出错，请检查代码。
)
pause