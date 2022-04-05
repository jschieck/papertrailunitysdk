# Papertrail for 7 Days to Die

7 Days to Die Modlet: Allow seamless integration between 7DTD’s logging system and Papertrail. All logging that already exists in 7 Days to Die will be automatically forwarded to Papertrail for recording and viewing to help track down bugs, monitor player activity, or provide support to users.

## Papertrail Account Setup

To get started, [sign up for a free account](https://papertrailapp.com/signup?plan=free) to get access to an unlimited number of apps, and some free logging each month.

## Modlet Setup

1. Download the [latest release](https://github.com/jonathan-robertson/papertrail-for-7dtd/releases/latest).
2. Unzip this to your server's `Mods/papertrail-for-7dtd` folder.
3. Start or Restart your server to load this modlet.
4. Open your Log Destinations page in your Papertrail account by choosing Settings -> Log Desinations or by visiting [this page](https://papertrailapp.com/account/destinations).
    - *observe the hostname:port listed here and use this data in the next step*
5. Run the following command in your server: `papertrail set <hostname> <port> [name]`
    - Example: `papertrail set logs3.papertrailapp.com 514 "The Best Server Ever"`
    - *you can do this by logging into your server in-game and pressing the F1 key to access the admin console, or by connecting via Telnet.*
    - *`<hostname>` and `<port>` are required, but if you don't set a `<name>`, one will be provided as '7dtd-server'... but it might make more sense to set your name to match your server's display name.*
6. Once successfully set, go to the [Papertrail Events Page](https://my.papertrailapp.com/events) and enjoy your logs :)

## Credits

This modlet is based off [jschieck](https://github.com/jschieck)'s [Papertrail Unity SDK](https://github.com/jschieck/papertrailunitysdk).

If interested in using Papertrail within a Unity Project you're developing, I'd encourage you to check out [the blog post](https://blog.papertrailapp.com/improve-live-ops-for-games-using-papertrail/) since it goes into more detail about how this Unity SDK works.
