## Refactored 脚本使用说明（Unity 新手向）

这份指南会用尽量通俗的方式，带你从 0 到 1 使用本目录下的脚本，并解释它们在 Unity 里的作用与连接方式。

### 你需要先了解的 5 个 Unity 概念
- GameObject：场景里的“物体”（可以是UI面板、玩家、按钮、空物体等）。
- Component：挂在 GameObject 上的“脚本/功能模块”。选中物体后，在 Inspector 面板里能看到并设置其组件。
- Serialize/Inspector 暴露：脚本里 public 字段或带 [SerializeField] 的字段，会显示在 Inspector 面板中，便于拖拽引用或改参数。
- UI 系统：
  - UI 需要在一个 `Canvas` 下；
  - UI 按钮是 `Button`，进度条可以用 `Slider`，图片是 `Image`；
  - 需要 `EventSystem` 才能响应点击/触摸。
- 场景切换/显隐：通过加载场景，或对某些 GameObject 调用 `SetActive(true/false)` 来切换内容。

---

## 一、DualButtonProgressController（双按钮小游戏控制器）
合并了原先的 ClickButton 与 ClickButtonMiniGame 的核心玩法：
- 一个“长按”按钮（按住逐渐填充进度）
- 一个“连点”按钮（每次点击加一点，松开会按速率衰减）
- 倒计时/达成判定
- 结束后的处理策略（加载场景 或 显隐某些根物体）

### 1. 放置与必备
1) 在场景中新建一个空物体，命名如 `MiniGameController`。
2) 在它的 Inspector 点击 Add Component，添加 `DualButtonProgressController`。
3) 确保场景里存在 `EventSystem`（创建 UI 按钮时 Unity 通常会自动生成）。

### 2. 连接按钮与进度
- Buttons：
  - `buttonHold`：拖入“长按的 UI 按钮”（Button 组件）。
  - `buttonRapid`：拖入“连点的 UI 按钮”。
- 进度目标（两路各自设置）：
  - 你可以用 `Image`（通过 `fillAmount` 填充）或 `Slider`（通过 `value` 显示）。
  - 对于“长按”进度：
    - 选择 `target1Type` 为 `Image` 或 `Slider`；
    - 然后把对应的 `progressImage1` 或 `progressSlider1` 拖到槽位。
  - 对于“连点”进度：
    - 同理设置 `target2Type` + 拖入 `progressImage2`/`progressSlider2`。
- 倒计时显示（可选）：
  - `countdownTMP`：拖入一个 `TextMeshProUGUI` 文本（如“30、29、28 ...”）。

### 3. 参数解释
- `countdownTime`：总倒计时（秒）。
- `holdDuration`：从 0 到满值需要按住的时间（秒），越小越快满。
- `rapidDecayPerSecond`：连点进度每秒衰减多少（0.5 表示每秒 -0.5）。
- `rapidClickIncrease`：每次点击连点按钮增加多少进度（0.1 表示 +10%）。

### 4. 结束策略（两选一或都不用）
- `SceneLoadEndStrategy`：结束后延迟加载某个场景。
  - 做法：在 `MiniGameController` 下再 Add Component，添加 `SceneLoadEndStrategy`；
  - 把这个组件拖到控制器的 `endStrategy` 槽位；
  - 设置 `targetSceneName`（比如 `SampleScene`）和 `delay`（延迟秒数）。
- `ToggleRootsEndStrategy`：结束后隐藏小游戏根、显示主场景根。
  - 同样 Add Component 添加 `ToggleRootsEndStrategy`；
  - 拖入 `miniGameRoot`（小游戏的父物体）和 `mainSceneRoot`（主场景内容父物体）；
  - 把组件拖到 `endStrategy` 槽位；设定 `delay`。

> 注意：两种策略是不同组件，只需要把其中一个拖到 `endStrategy` 即可。若都不设置，达成后不会自动切换。

### 5. 常见排错
- 按钮点击没反应：检查场景是否有 `EventSystem` 和 `Canvas`；按钮是否可交互；物体没被 `SetActive(false)`。
- 进度不动：确认进度目标类型与组件引用匹配（Image 就填 image 槽，Slider 就填 slider 槽）。
- 倒计时不显示：`countdownTMP` 是否拖了 `TextMeshProUGUI`。
- 结束不切场景/界面：是否正确添加并指定了 `endStrategy` 组件和参数。

---

## 二、PressFillProgress（按下填充/点击叠加/自动衰减）
合并了 `ClickToFillProgress` 与 `HoldToFillProgress`。你可以实现：
- 按住时进度上升；
- 松开后自动衰减（可关）；
- 配合按钮点击手动“叠加一次”。

### 1. 放置与目标选择
1) 选一个 UI 元素（通常是按钮的可视部分）或单独新建一个空物体；
2) Add Component 添加 `PressFillProgress`；
3) 选择 `targetType`：`Image` 或 `Slider`；把对应的 `targetImage` 或 `targetSlider` 拖进去。

### 2. 行为与速度
- `enableClickAccumulation`：是否允许通过调用 `IncrementOnce()` 来“点一下加一点”。
- `enableAutoDecay`：松开后是否自动往回掉。
- `clickIncrement`：每次点击叠加的量（0.1=10%）。
- `holdSpeed`：按住每秒增加多少。
- `decaySpeed`：每秒衰减多少。

### 3. 如何触发“点击叠加一次”
- 在一个 UI Button 的 OnClick 事件中，拖入含有 `PressFillProgress` 的对象，选择函数 `PressFillProgress -> IncrementOnce` 即可。

### 4. 必要条件
- 需要 `EventSystem`（统一 UI 交互组件都需要）。
- `PressFillProgress` 自身会响应按下/抬起（基于指针事件），务必保证它所在的对象或其子层级上有可被点击的 Graphic（例如带 Image 的 UI）。

### 5. 排错
- 按住没反应：该对象或其子物体需要有可点击的 UI Graphic（例如 `Image`），或者把脚本挂在真正可点击的 UI 上。
- 不衰减：是否关闭了 `enableAutoDecay`，或 `decaySpeed` 太小。

---

## 三、SinWaveMover（通用正弦位移动画）
用一个脚本替代 EyeMove / PupilMove / FloatFaceUpDown。
- `direction`：朝哪个方向摆动（常见：`(1,0,0)` 水平，`(0,1,0)` 垂直）。
- `amplitude`：幅度（摆动距离）。
- `speed`：速度（摆动快慢）。
- `useLocalSpace`：是否基于 `localPosition`（UI/子物体通常勾选）。

### 使用步骤
1) 在任何需要轻微漂浮或左右摆动的物体上 Add Component 添加 `SinWaveMover`；
2) 设置方向/幅度/速度；
3) 运行即可看到来回摆动。

---

## 四、DialogTyper（对话+打字机+E键提示）
把对话 UI 的显示、逐字打字、E 键提示三件事做成了一个通用组件。

### 1. 放置与连接
1) 准备一个对话面板 `dialogPanel`（通常是一个 UI 面板 GameObject）。
2) 在面板内有一个 `TextMeshProUGUI` 文本，拖到 `dialogTMPText`；
3) 准备一个 E 键提示（可以是场景里一个 `SpriteRenderer` 的小图标），拖到 `eKeyPrompt`；
4) 选中任一合适的 GameObject（比如一个 NPC 的空物体），Add Component 添加 `DialogTyper`，把以上引用拖入；
5) 在 `message` 中写要显示的文字，`typingSpeed` 设置打字速度。

### 2. 控制玩家是否在可交互范围
- 本组件不强制要求触发器，但提供了方法：
  - 当玩家进入可交互范围时，调用 `dialogTyper.SetPlayerInRange(true)`；
  - 离开时调用 `dialogTyper.SetPlayerInRange(false)`。
- 常见做法：在带 2D 触发器的对象上写一个很小的辅助脚本，在 `OnTriggerEnter2D/OnTriggerExit2D` 里调用上面的方法即可（你也可以沿用你项目里已存在的触发器）。

### 3. E 键行为
- 当 `requireEToStart = true` 时：
  - 玩家在范围内按 `E`，对话面板弹出并开始打字；
  - 再按一次 `E`，会立即补全剩余文字。

### 4. 事件钩子
- `OnDialogShown`：显示对话时触发；
- `OnTypingFinished`：逐字打字结束时触发；
- `OnDialogHidden`：隐藏对话时触发。

你可以用这些事件去联动其它系统（例如开始小游戏、播放音效等）。

### 5. 常见排错
- 文本不显示：确认 `dialogTMPText` 是 TextMeshProUGUI，并且在面板下；
- E 键没反应：`SetPlayerInRange(true)` 是否被调用；面板或提示是否被错误隐藏；
- 打字速度太快/慢：调整 `typingSpeed`。

---

## 五、DialogMiniGameAdapter（对话完成后切换到小游戏）
把“对话结束 -> 打开小游戏界面/关闭主界面”的流程独立出来，避免把逻辑写死在对话里。

### 1. 放置与连接
1) 在任意对象上 Add Component 添加 `DialogMiniGameAdapter`；
2) `dialogTyper`：拖入上面配置好的 `DialogTyper`；
3) `mainSceneRoot`：拖入主场景内容的父物体（整个主 UI/主世界的根）；
4) `miniGameRoot`：拖入小游戏 UI/内容的父物体；
5) 运行后，当 `DialogTyper` 的打字完成事件触发时，会自动：隐藏对话、隐藏主界面、显示小游戏。

### 2. 返回主场景
- 你可以在小游戏里的一个“返回”按钮 OnClick 里，拖入 `DialogMiniGameAdapter`，调用 `ReturnToMain()`，即可隐藏小游戏并显示主界面，同时恢复 E 键提示。

### 3. 常见排错
- 对话完成后没切换：检查 `DialogMiniGameAdapter.dialogTyper` 是否正确指向；
- 主界面/小游戏没有显隐：确认 `mainSceneRoot` / `miniGameRoot` 引用正确，且它们本身不是 `null`；
- 返回按钮无效：是否真的把 `ReturnToMain()` 绑到了按钮 OnClick。

---

## 实操示例（从 0 搭一个最小双按钮小游戏）
1) 新建 `Canvas`（UI 根），确保场景里有 `EventSystem`；
2) 在 Canvas 下新建两个 Button：`HoldButton` 和 `RapidButton`；
3) 新建两个进度 UI：一个 `Image`（设置为 Filled 类型），一个 `Slider`；
4) 新建空物体 `MiniGameController`，添加 `DualButtonProgressController`：
   - `buttonHold` 拖入 `HoldButton`；
   - `buttonRapid` 拖入 `RapidButton`；
   - 选择 `target1Type = Image` 并拖入 `progressImage1`；
   - 选择 `target2Type = Slider` 并拖入 `progressSlider2`；
   - 可选：添加 `ToggleRootsEndStrategy` 并设置 `miniGameRoot = Canvas`、`mainSceneRoot = 你的主界面根`，再把组件拖到 `endStrategy`；
5) 运行游戏，按住 Hold、快速点 Rapid，就能看到进度变化；长按进度满后按策略切换。

---

## 迁移对照表（旧 -> 新）
- ClickButton、ClickButtonMiniGame -> DualButtonProgressController（+ 可选 EndStrategy）
- ClickToFillProgress、HoldToFillProgress -> PressFillProgress
- EyeMove、PupilMove、FloatFaceUpDown -> SinWaveMover
- DialogTrigger、DialogTriggerMiniGame -> DialogTyper（对话） + DialogMiniGameAdapter（切换）

---

## 常见问题（FAQ）
- Q：我怎么知道该把什么拖到哪个槽位？
  - A：选中脚本组件（比如 DualButtonProgressController），看每个字段的类型与注释：Button 拖 Button，Image 拖 Image，Slider 拖 Slider。字段名里也提示了用途。
- Q：运行时报 MissingReference 或 NullReference？
  - A：说明某个槽位没拖或物体在运行中被销毁/隐藏导致引用丢失。回到场景检查 Inspector 是否都已赋值。
- Q：UI 按钮没反应？
  - A：是否有 `EventSystem`、该按钮是否在 `Canvas` 下、按钮是否被遮挡、是否被禁用、Raycast Target 是否开启。
- Q：我可以混合使用 Image 和 Slider 吗？
  - A：可以。两个进度通道各自独立配置类型与引用。

—— 若你希望我直接在场景里替换旧脚本并连好引用，也可以告诉我场景里对应物体名称与期望行为，我可以继续帮你调整。 