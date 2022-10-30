# TikTokLoaderTgBot

## Description
Telegram bot for downloading videos from tiktok.

Send the bot a video link and it will send you a video in return.

## Plans for new features
- the bot only responds to the owner
- video download status (0-100%)
- (?) multi-download of all videos from the profile
- (?) owner-only debugging mode (enable/disable with bot command)
- (?) usage statistics? (number of downloaded videos per day/week/month/year)


## Docker

```bash
$ docker run -e "botToken=your:token" hard1n/tiktokloadertgbot
```
