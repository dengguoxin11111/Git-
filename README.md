# Git-
人力外包，披甲专属
这是一个为你定制的专业 `README.md` 文档。你可以直接把它保存为 `README.md` 文件，放在你的项目根目录下，方便自己查看或分享给同事。

-----

# 🎭 Git 身份切换助手 (Git Identity Switcher)

  

这是一个专为 Windows 开发环境设计的**轻量级 Git 身份管理工具**。它无需安装任何第三方环境（如 Python/Java），单文件直接运行。

主要用于在多人协作、外包管理或通过“马甲”提交代码时，快速切换当前项目的 Git 提交人身份（`user.name` 和 `user.email`），并提供实时的提交量统计看板。

## ✨ 核心功能

  * **⚡️ 一键切换身份**：下拉选择预设用户，点击即可修改当前仓库的 `.git/config` 配置。
  * **📊 工作量审计看板**：实时统计当前仓库所有人的提交次数，并按降序生成排行榜（基于 `git shortlog`）。
  * **🛠 动态用户管理**：支持在界面上直接【添加】或【删除】常用账号。
  * **💾 自动持久化**：用户列表会自动保存到同目录下的 `git_users.txt` 文件中。
  * **🚀 绿色免安装**：基于 .NET Framework 4.0 原生编译，兼容 Win7/10/11，即插即用。

## 📂 安装与使用

本工具为**绿色单文件版**，无需安装。

### 1\. 放置位置 (重要❗️)

请务必将 `GitSwitchTool.exe` 复制到**你的 Git 项目根目录下**运行。

> **什么是项目根目录？**
> 就是包含 `.git` 隐藏文件夹的那一层。通常也是你的 `pom.xml`, `package.json` 或 `.sln` 文件所在的目录。

**正确的文件结构示范：**

```text
D:\Work\Project_YZ001\
│
├── .git/                 <-- 核心 Git 目录
├── src/                  <-- 源代码
├── README.md
│
└── GitSwitchTool.exe     <-- ✅ 请放在这里！
```

### 2\. 运行

双击 `GitSwitchTool.exe` 即可启动。

### 3\. 操作说明

  * **切换用户**：在下拉框选择目标身份，点击【立即切换身份】。
  * **新增用户**：点击【添加】，输入显示名称、Git Name 和 Email。
  * **查看统计**：下方的列表会自动显示当前项目的提交排行榜。每次切换身份后，列表会自动刷新。

## ⚙️ 编译指南 (开发者)

如果你需要修改源码重新编译，请确保已安装 .NET Framework (Windows 自带)。

**源码文件**：`GitSwitchTool.cs`
**编译脚本** (`build.bat`)：

```batch
@echo off
echo 正在编译 GitSwitchTool...
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:GitSwitchTool.exe GitSwitchTool.cs
pause
```

## ❓ 常见问题

**Q: 打开软件提示“未检测到 Git 仓库配置”？**
A: 说明你没把 exe 文件放到 Git 项目的根目录下。请移动文件位置后重试。

**Q: 切换后 IDEA/VS Code 里的提交人没变？**
A: IDE 有时会缓存 Git 配置。切换成功后，请关闭 IDE 的提交窗口（Commit Window）并重新打开，或者重启 IDE 即可看到新身份。

**Q: 排行榜里出现了乱码？**
A: 工具默认使用 UTF-8 编码读取 Git 日志。请确保你的 Git 配置也是 UTF-8 (通常 Windows Git Bash 默认已配置好)。

-----

**Internal Tool for DevOps & Workload Management**
