# 🎭 RPCommands

![EXILED](https://img.shields.io/badge/EXILED-Supported-green?style=for-the-badge)
[![Version](https://img.shields.io/github/v/release/PiwnicaUwU/RPCommands?style=for-the-badge)](https://github.com/PiwnicaUwU/RPCommands/releases/latest)
## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📜 Description
**RPCommands** is a plugin for **EXILED** that adds narrative text commands, enabling players to enhance their roleplaying experience on SCP: Secret Laboratory servers.

With this plugin, you can create immersive descriptions of actions, thoughts, and surroundings that will be visible to nearby players.

This plugin includes two versions - one without HSM (HintServiceMeow) and one with it. It is recommended to use the version with HSM, but it is not required.

## 🛠️ Features
✅ **.me**
✅ **.do**
✅ **.look**
✅ **.ooc** 
✅ **.try**

## ⚙️ Configuration
The `config.yml` file allows you to customize the plugin, such as the message visibility range and text formatting.

```yaml
# true = Plugin enabled, false = plugin disabled
is_enabled: true
# Enable debug logs - don't work
debug: false
# Command settings, do not remove {0}, {1}, or {2}
me:
  range: 15
  duration: 5
  format: '<color=green>「Me」</color> <color=#FFFF00>{0}</color> : {1}'
do:
  range: 15
  duration: 5
  format: '<color=green>「Do」</color> <color=#FFFF00>{0}</color> : {1}'
look:
  range: 15
  duration: 5
  format: '<color=green>「Look」</color> <color=#FFFF00>{0}</color> : {1}'
ooc:
  range: 15
  duration: 5
  format: '<color=green>「Ooc」</color> <color=#FFFF00>{0}</color> : {1}'
try:
  range: 15
  duration: 5
  format: '<color=green>「Try」</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!'
```
## 🌐 Translation
The `translations.yml` file allows you to translate the plugin.

```yaml
# Message shown when the round has not started.
round_not_started: 'You cannot use this command because the round has not started yet.'
# Usage message for commands.
usage: 'Usage: .{0} <message>'
# Message shown when a non-player tries to use a command.
only_players: 'Only players can use this command.'
# Message shown when a command is successfully sent.
message_sent: 'Message has been sent.'
# Dictionary of command names used in the system.
command_names:
  me: me
  do: do
  look: look
  ooc: ooc
  try: try
# Dictionary of command descriptions.
commands:
  me: Narrative command 'Me'.
  do: Narrative command 'Do'.
  look: Narrative command 'Look'.
  ooc: Narrative command 'Ooc'.
  try: Narrative command 'Try'.
# Dictionary of results for try command.
try_result:
  success: successfully
  fail: unsuccessfully
```

## 📦 Installation
```plaintext
1. Download the `.dll` file from releases and place it in the `Plugins` folder.
2. Start the server to generate the configuration file.
3. Customize the `config` according to your preferences.
4. Restart the server and enjoy the plugin.
```

## 🐾 HintServiceMeow (HSM) Integration
This plugin utilizes **HSM (HintServiceMeow)** for displaying hints. While HSM is **not required** to use the plugin, it is **highly recommended** for enhancing the overall experience and quality of the plugin.

HSM allows for more polished hint display and improved functionality, providing a smoother experience for players on the server.

For optimal performance, it’s advised to install **HSM** alongside this plugin.

## 🔗 Links
- 📖 [EXILED](https://github.com/ExMod-Team/EXILED)
- 🐾 [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow)


## 👥 Author
```plaintext
👤 .Piwnica  
📧 Contact: Discord - .piwnica2137
```

---

💡 *Got ideas for new features? Report them in Issues or Pull Requests!*
