# Papertrail for 7 Days to Die

[![Tested with A20.3 b3](https://img.shields.io/badge/A20.3%20b3-tested-blue.svg)](https://7daystodie.com/)

7 Days to Die Modlet: Allow seamless integration between 7DTD’s logging system and Papertrail.

All logging that already exists in 7 Days to Die will be automatically forwarded to Papertrail for recording and viewing to help track down bugs, monitor player activity, or provide support to users.

## Setup

See the [project readme](PapertrailFor7DTD/README.md), which will also be included within the modlet download.

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
