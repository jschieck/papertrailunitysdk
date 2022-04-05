# Papertrail for 7 Days to Die

7 Days to Die Modlet: Allow seamless integration between 7DTD’s logging system and Papertrail.

All logging that already exists in 7 Days to Die will be automatically forwarded to Papertrail for recording and viewing to help track down bugs, monitor player activity, or provide support to users.

## Papertrail Account Setup

To get started, [sign up for a free account](https://papertrailapp.com/signup?plan=free) to get access to an unlimited number of apps, and some free logging each month.

At the time of this writing, Papertrail provides the following features for the free account:

- 50 MB/month
  - *this represents the total size of all logs you're allowed to upload each month. 50MB is a wildly large amount of data for a single 7DTD server, but with enough people/activity, it is possible to hit this limit*
- 48 hours search
  - *searching in the free tier will pull up results from as far back as 48 hrs. If you want/need to be able to search logs further back from that, you'd need to explore their paid tiers*
- 7 days archive
  - *this is how long they'll keep your logs archived for before those logs start to be purged from the system*
  - *ha, 7 Days.. to die.. get it? I'll move on*
- Unlimited Systems
  - *i.e. they don't limit the number of systems that can be logging to their service under your account - so if you need to log 3+ servers, that's totally fine*
- Unlimited Users
  - *got another admin or moderator you want to have access to your logs for troubleshooting? totally supported*

## Modlet Setup

1. Download the [latest release](https://github.com/jonathan-robertson/papertrail-for-7dtd/releases/latest).
2. Unzip this to your server's `Mods/papertrail-for-7dtd` folder.
3. Start or Restart your server to load this modlet.
4. Open your Log Destinations page in your Papertrail account by choosing Settings -> Log Desinations or by visiting [this page](https://papertrailapp.com/account/destinations).
    - *observe the hostname:port listed here and use this data in the next step*
5. Run the following command in your server: `papertrail set <hostname> <port> [name]`
    - Example: `papertrail set logs3.papertrailapp.com 514 TheBestServerEver` (*note that name must be 1 word*)
    - *you can do this by logging into your server in-game and pressing the F1 key to access the admin console, or by connecting via Telnet.*
    - *`<hostname>` and `<port>` are required, but if you don't set a `<name>`, one will be provided as '7dtd-server'... but it might make more sense to set your name to match your server's display name.*
6. Once successfully set, go to the [Papertrail Events Page](https://my.papertrailapp.com/events) and enjoy your logs :)

Note: for additional options, run `help papertrail`

## Questions and Answers

### Why Use a Log Aggregator Service?

- If you run multiple servers, this provides a single location to view all your logs.
- If your logs are purged regularly or are purged immediately on restart (which some hosts do), this provides you with time to refer back to or look through the logs to investigate issues.
- If viewing logs with the options your host has provided leave you feeling frustrated, centralized logging visibility from a single page will be a game-changer for you.
  - *it's even easy to view this from a phone while you're on the go!*

### Why Papertrail, Specifically?

1. This is simply a log aggregator that I've used before while working at a previous company, so this is one of the options I was already familiar with.
2. More importantly: you can [sign up for a free account](https://papertrailapp.com/signup?plan=free)!
3. *No, this project is not sponsored; I do not make any money for this modlet or for suggesting Papertrail is a good option.*

## Recognition

This modlet is based off [jschieck](https://github.com/jschieck)'s [Papertrail Unity SDK](https://github.com/jschieck/papertrailunitysdk).

If interested in using Papertrail within a Unity Project you're developing, I'd encourage you to check out [the blog post](https://blog.papertrailapp.com/improve-live-ops-for-games-using-papertrail/) since it goes into more detail about how this Unity SDK works.
