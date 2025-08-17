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
✅ **.wear**
✅ **.unwear**
✅ **.punch**
✅ **.heal**
✅ **.clean**
✅ **.Zone**
✅ **.cuff**
✅ **.uncuff**


## ⚙️ Configuration
The `config.yml` file allows you to customize the plugin, such as the message visibility range and text formatting.

```yaml
# true = Plugin enabled, false = plugin disabled
is_enabled: true
# Enable debug logs
debug: false
# Enables or disables the in-game credit tag for the plugin's author.
is_credit_tag_enabled: true
# List of banned words. Messages containing any of these words will be blocked. It is recommended to not delete 'size'
banned_words:
- 'size'
- '<size>'
# If false, SCPs will not be able to use RP Commands.
allow_scp_to_use_commands: false
# If true, sender will see a console message with the command they used if it's shown to others.
show_command_in_sender_console: true
# If true, spectators of players who are within range of the command will also see the hint.
show_hints_to_spectators_of_receivers: true
# Command settings, do not remove {0}, {1}, or {2}. For handler use: 'Client' for Player's Console, 'RemoteAdmin' for RA Text-Based.
me:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Me」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
do:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Do」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
look:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Look」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
ooc:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Ooc」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
try:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Try」</color><color=#FFFF00>{0}</color> : tried to {1} and {2} did it!'
  handler: Client
desc:
  range: 15
  duration: 5
  cooldown: 3
  format: '<color=green>「Desc」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
assist:
  range: 0
  duration: 0
  cooldown: 3
  format: '<color=red>[ASSIST]</color> <color=#ffcc00>{0}</color>: {1}'
  handler: Client
custom_info:
  range: 0
  duration: 0
  cooldown: 0
  format: ''
  handler: Client
# Maximum length of custom info
max_custom_info_length: 250
unwear:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
radio:
  range: 0
  duration: 5
  cooldown: 3
  format: '<color=green>「Radio」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
wear:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
# Determines how the .wear command functions. Available options: RoleChange, ModelChange
wear_mode: RoleChange
# Duration of the disguise from the .wear command in seconds. Set to -1 for infinite duration.
wear_duration: 180
punch:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
# Damage dealt by the .punch command.
punch_damage: 5
# Push force multiplier for the .punch command.
punch_push_force: 0.699999988
clean:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
heal:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
# Amount of health restored by the .heal command.
heal_amount: 65
# Item required to use the .heal command.
heal_item: Medkit
cuff:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
# Choose how cuffing affects a player's inventory. Options: SaveAndRestore, DropOnGround
cuff_behavior: SaveAndRestore
# Determines whether all SCPs can be cuffed.
can_cuff_all_scps: false
# A list of SCPs that are cuffable by default.
cuffable_scps:
- Scp049
# A list of items that can be used to cuff players.
cuffing_items:
- GunE11SR
- GunLogicer
un_cuff:
  range: 0
  duration: 5
  cooldown: 3
  format: ''
  handler: Client
name:
  range: 0
  duration: 0
  cooldown: 0
  format: ''
  handler: Client
# If you want to increase size of the zone, set Range to higher value.
zone:
  range: 5
  duration: 30
  cooldown: 10
  format: '<color=green>「Zone」</color><color=#FFFF00>{0}</color> : {1}'
  handler: Client
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
  wear: true
  punch: true
  clean: true
  heal: true
  cuff: true
  uncuff: true
  name: true
  zone: true
  unwear: true
```
## 🌐 Translation
The `translations.yml` file allows you to translate the plugin.

```yaml
# Message shown when .name command is used.
name_response: 'Your name is {0} and your custom info is {1}'
# Message shown to the player after successfully creating a zone.
zone_success: 'Zone successfully created. It will disappear in {0} seconds.'
# Message shown when you try to cuff player without holding a weapon.
weapon_required_message: 'You must be holding a weapon to use this command!'
# Message shown when the round has not started.
round_not_started: 'You cannot use this command because the round has not started yet.'
# Usage message for commands.
usage: 'Usage: .{0} <message>'
# Message shown when banned word is detected.
banned_word_detected: 'Your message contains a banned word and has been blocked.'
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
# Message shown when the player tries to use the .wear command with no ragdolls in range.
no_dead_body_found: 'No dead body found.'
# Message shown when the player fails to put on the clothes for an unspecified reason.
wore_failure: 'You cannot put on these clothes.'
# Success message displayed to the player after successfully using the .wear command.
wore: 'You put on the clothes of the deceased.'
# Message shown when the player attempts to wear the ragdoll of an SCP.
scp_clothes_not_allowed: 'You can''t wear SCP ragdolls.'
# Message displayed when an SCP player tries to use the .wear command.
scp_cantwear: 'SCPs cannot wear clothes.'
# Message shown to the player when their disguise from the .wear command expires.
disguise_worn_off: 'Your disguise has worn off.'
# Message shown when there is no target in range for a command.
no_target_in_range: 'No target in range.'
# Message for punch command cooldown.
punch_cooldown: 'You can use this command again in {0} seconds.'
# Message for the player who successfully used the punch command.
punch_success: 'You successfully punched <color=green>{0}</color>!'
# Hint message for the player who got punched.
punch_hint_target: '<color=red>You were punched by {0}</color>!'
# Message when no ragdoll is found nearby for the clean command.
no_ragdoll_nearby: 'There is no body nearby to clean up.'
# Message for successfully cleaning a ragdoll.
clean_success: 'The body has been cleaned up.'
# Message when a player tries to use the heal command without holding the required item.
heal_item_required: 'You must be holding a Medkit to use this command.'
# Message for the player who successfully used the heal command.
heal_success: 'You have healed <color=green>{0}</color>.'
# Hint message for the player who got healed.
heal_hint_target: '<color=green>You have been healed by {0}.</color>'
# Message shown when player is scp and try use unwear command.
scp_cant_unwear: 'You can''t use this command as Scp.'
# Message shown when you are not disguised.
not_disguised: 'You are not disguised.'
# Message shown in result when player use unwear command.
unwore: 'Disguise was worn off.'
# Message shown in result of command usage failure.
unwore_failure: 'Discguise cannot be worn off.'
# Message shown in result of command usage sucess.
disguise_removed: 'Disguise was sucessfully worn off.'
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
  wear: wear
  punch: punch
  clean: clean
  heal: heal
  cuff: cuff
  uncuff: uncuff
  name: name
  zone: zone
  unwear: unwear
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
  wear: allows to wear a dead body.
  punch: Punches a player
  clean: Cleans up the nearest ragdoll.
  heal: Use a Medkit to heal player.
  cuff: cuffs player.
  uncuff: uncuffs player.
  name: shows your name and custom info.
  zone: Creates a zone.
  unwear: allows player to wear off disguise
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
👤 02319478_334
📧 Contact: Discord -> .czyliadi_
```

---

💡 *Got ideas for new features? Report them in Issues or Pull Requests!*
