# 🎭 RPCommands

![EXILED](https://img.shields.io/badge/EXILED-Supported-green?style=for-the-badge)
[![Version](https://img.shields.io/github/v/release/PiwnicaUwU/RPCommands?style=for-the-badge)](https://github.com/PiwnicaUwU/RPCommands/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/PiwnicaUwU/RPCommands/total?style=for-the-badge)](https://github.com/PiwnicaUwU/RPCommands/releases)

## 📜 Description
**RPCommands** is a plugin for **EXILED** that adds narrative text commands, enabling players to enhance their roleplaying experience on SCP: Secret Laboratory servers.

With this plugin, you can create immersive descriptions of actions, thoughts, and surroundings that will be visible to nearby players.

## ⚠️ Warning: Common Issue with Incorrect Usage  

Some users download the Plugin but do not install or use it correctly on their server. If you downloaded the **Plugin** but do not have **HintServiceMeow** installed, the plugin WILL NOT work properly.  

To avoid issues:  
✅ If you want to use the **Plugin**, make sure you have **HintServiceMeow installed in Your plugins folder**.  

If you encounter any problems, check your server logs or console and ensure the plugin is configured correctly.

## 🛠️ Features
✅ **.me**
✅ **.do**
✅ **.look**
✅ **.ooc** 
✅ **.try**
✅ **.desc**
✅ **.custom-info**
✅ **.assist**
✅ **.radio**

## ⚙️ Configuration
The `config.yml` file allows you to customize the plugin, such as the message visibility range and text formatting.

```yaml
# true = Plugin enabled, false = plugin disabled
is_enabled: true
# Enable debug logs - don't work
debug: false
# If false, SCPs will not be able to use narrative commands.
allow_scp_to_use_commands: false
# If true, sender will see a console message with the command they used if it's shown to others.
show_command_in_sender_console: true
# If true, spectators of the player who used the command will also see the hint.
show_hints_to_spectators_of_sender: true
# If true, spectators of players who are within range of the command will also see the hint.
show_hints_to_spectators_of_receivers: true
# Command settings, do not remove {0}, {1}, or {2}
me:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}'
do:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}'
look:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}'
ooc:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}'
try:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!'
desc:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}'
assist:
  range: 0
  duration: 0
  cooldown: 3
  format: '<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}'
custom_info:
  range: 0
  duration: 0
  cooldown: 0
  format: ''
radio:
  range: 0
  duration: 5
  cooldown: 3
  format: '<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}'
# Maximum length of custom info
max_custom_info_length: 250
# Enable or disable specific commands
enabled_commands:
  me: true
  do: true
  look: true
  ooc: true
  try: true
  desc: true
  custom-info: true
  assist: true
  radio: true
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
# Message shown when a non-human tries to use a command.
only_humans: 'Only humans can use this command.'
# Message shown when Command Sender is not alive.
only_alive: 'You must be alive to use this command.'
# Cooldown message when a player tries to use a command too quickly.
command_cooldown: 'You must wait {0} seconds before using the command again.'
# Message shown when a command is successfully sent.
message_sent: 'Message has been sent.'
# Message shown when custom info is set.
custom_info_set: 'Your custom info has been set!'
# Message shown when the set custom info is too long.
custom_info_too_long: 'Custom info is too long!'
# Message shown when a command is disabled.
command_disabled: 'This command is disabled.'
# Message shown when a player tries to use radio command without holding a radio.
radio_required: 'You must be holding a radio to use this command.'
# Message shown when a assist request is sent.
help_request_sent: 'Your assist request has been sent to the staff.'
# Dictionary of command names used in the system.
command_names:
  me: me
  do: do
  look: look
  ooc: ooc
  try: try
  desc: desc
  custom-info: custom-info
  assist: assist
  radio: radio
# Dictionary of command descriptions.
commands:
  me: Narrative command 'Me'.
  do: Narrative command 'Do'.
  look: Narrative command 'Look'.
  ooc: Narrative command 'Ooc'.
  try: Narrative command 'Try'.
  desc: Narrative command 'Desc'.
  custom-info: Sets your custom info.
  assist: Sends a assist request to the staff chat.
  radio: Sends a radio message to other players holding radios.
# Dictionary of results for try command.
try_result:
  success: successfully
  fail: unsuccessfully
```

## 📦 Installation
```plaintext
1. Download the `.dll` file from releases and place it in the `Plugins` folder.
2. Make sure you installed HSM as well.
3. Start the server to generate the configuration file.
4. Customize the `config` according to your preferences.
5. Restart the server and enjoy the plugin.
```

## 🐾 HintServiceMeow (HSM) Integration
This plugin utilizes **HSM (HintServiceMeow)** for displaying hints.

HSM allows for more polished hint display and improved functionality, providing a smoother experience for players on the server.

## 🔗 Links
- 📖 [EXILED](https://github.com/ExSLMod-Team/EXILED)
- 🐾 [HintServiceMeow](https://github.com/MeowServer/HintServiceMeow)


## 👥 Authors
```plaintext
👤 .Piwnica  
📧 Contact: Discord -> .piwnica2137
👤 .Adamczyli
📧 Contact: Discord -> .czyliadi_
```

---

💡 *Got ideas for new features? Report them in Issues or Pull Requests!*
