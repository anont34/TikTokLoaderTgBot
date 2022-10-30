# TikTokLoaderTgBot

## Description
Telegram bot for downloading videos from tiktok.

Send the bot a video link and it will send you a video in return.

## Plans for new features
- change RAM usage for video downloads to upload to server media. (required for weak VPS) + The ability to choose between these modes
- the bot only responds to the owner
- video download status (0-100%)
- Ability to enable "general queue" for video download + message about your queue number (Reducing the load on the server/bot)
- (?) multi-download of all videos from the profile
- (?) owner-only debugging mode (enable/disable with bot command)
- (?) usage statistics? (number of downloaded videos per day/week/month/year)


## What works:
- Video download via short and long link. (the first one when you share through the app, the second one you copy from the browser)
- Error handling: invalid link, video does not exist, video is larger than 50MB (limit for telegram bots)
- 

## Docker

```bash
$ docker run -e "botToken=your:token" hard1n/tiktokloadertgbot
```
