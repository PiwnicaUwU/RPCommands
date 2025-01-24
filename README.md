# 🎭 NarrativeCommands

![EXILED](https://img.shields.io/badge/EXILED-Supported-green?style=for-the-badge)
![Version](https://img.shields.io/github/v/release/PiwnicaUwU/RPCommands?style=for-the-badge)
![License](https://img.shields.io/github/license/PiwnicaUwU/RPCommands?style=for-the-badge)

## 📜 Description
**NarrativeCommands** is a plugin for **EXILED** that adds narrative text commands, enabling players to enhance their roleplaying experience on SCP: Secret Laboratory servers.

With this plugin, you can create immersive descriptions of actions, thoughts, and surroundings that will be visible to nearby players.

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
# Nothing important, additional logs (might not work)
debug: false
# Enable or disable the usage of HintServiceMeow (REQUIRES HSM TO BE LOCATED IN THE PLUGINS FOLDER). True = Use HSM, False = Use regular hint.
use_h_s_m: true
# The range of narrative commands (in meters). Players within this radius will receive messages related to the given command.
me_range: 15
do_range: 15
look_range: 15
ooc_range: 15
try_range: 15
# Duration of hints for narrative commands (in seconds).
me_duration: 5
do_duration: 5
look_duration: 5
ooc_duration: 5
try_duration: 5
# You can edit hint formatting and colors. Do not edit '{1}' - message or '{0}' - player, otherwise the plugin will break!
me_format: '<b><color=green>[Me]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
do_format: '<b><color=green>[Do]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
look_format: '<b><color=green>[Look]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
ooc_format: '<b><color=green>[Ooc]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
try_format: '<b><color=green>[Try]</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!</b></size></align>'
# Use this config only if you disabled HSM option.
n_o_h_s_m_me_format: '<size=25><align=left><b><color=green>[Me]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
n_o_h_s_m_do_format: '<size=25><align=left><b><color=green>[Do]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
n_o_h_s_m_look_format: '<size=25><align=left><b><color=green>[Look]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
n_o_h_s_m_ooc_format: '<size=25><align=left><b><color=green>[Ooc]</color> <color=#FFFF00>{0}</color> : {1}</b></size></align>'
n_o_h_s_m_try_format: '<size=25><align=left><b><color=green>[Try]</color> <color=#FFFF00>{0}</color> : tried to {1} and {2} did it!</b></size></align>'
```

## 📦 Installation
```plaintext
1. Download the `.dll` file and place it in the `Plugins` folder.
2. Start the server to generate the configuration file.
3. Customize the `config` according to your preferences.
4. Restart the server and enjoy the plugin.
```

## 🐾 HintServiceMeow (HSM) Integration
This plugin utilizes **HSM (HintServiceMeow)** for displaying hints. While HSM is **not required** to use the plugin, it is **highly recommended** for enhancing the overall experience and quality of the plugin.

HSM allows for more polished hint display and improved functionality, providing a smoother experience for players on the server.

For optimal performance, it’s advised to install **HSM** alongside this plugin.

## 📝 Requirements
```plaintext
- 🔹 **EXILED**
```

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
