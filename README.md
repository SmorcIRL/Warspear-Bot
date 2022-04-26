# WarspearBot
This is a simple example of a bot for Warspear Online, written for educational purposes. I haven't played this game for a very long time and did not aim to write a full-fledged bot, but maybe someone will develop the idea.

# Requirements
For the bot to work correctly, game should be run in the VirtualBox VM with 800x600 resolution (bot uses CV, so resolution matter). The idea was to allow to run bot for multiple game instances, however this feature currently is not implemented. Bot uses WinAPI, so it can only be launched under Windows. A prepared virtual machine instance with installed game and proper resolution can be downloaded [here](https://drive.google.com/file/d/1_PxGSkoHfWc4qv1mF2oyn3h4c4eWev6u/view?usp=sharing). Bot is able to farm only the initial mobs "Fairy" in locations on Firstborn noob island and is designed for a druid 5+ level.

# Overview
https://user-images.githubusercontent.com/53185719/165392483-489157b6-8133-497b-af0a-bd491a9b025d.mp4

# VM vs emulator
Why VMs when we have a lot of Android emulators like BlueStacks? Well, I've tried some of them and even got some nice results. For example, they allow you to choose such a screen resolution, that the entire location will be visible, all 28x28 cells. It's like a cheat in itself, but for a bot it also allow you to navigate between locations easily. Combine this with [maps](https://wsdb.xyz/map/en/firstborn/128), that you can download and parse, and your bot will be able to move to any location you want. However, emulators have some major problems. First of all, they consume a lot of GPU and memory (DNO why), for my desktop it was like 20-50% GPU for one instance. And also give you a bit blurred picture - that can be a problem for CV. In the same time, clear Debian VM consumes only CPU (~like a game running on the host machine) and a few memory and can easily be replicated, even on environments with no GPU at all. Also in VM you get nice picture and can count on pixel-perfect matches with CV template matching.
