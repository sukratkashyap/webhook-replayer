# Webhook Replayer

Inside a companies network, there are times when some services wants to call a webhook which is sitting outside the comapny's network. Most of the times, companies cyber security doesn't allow to set proxy on these services. I created this tool to help with that. This tool allows you to replay your webhook HTTP call using proxy settings.

Now, since all the different webhook calls need to be made one server. What I have done is that if you pass the real hostname in the query string of this webhook-replayer. It will replay there using the proxy settings you have set.

Basically, you new webhook URL will become:

```
http://webhook-replayer.company.com/?_to=https://realwebhook.com
```
`_to`: query string parameter allows the replayer to determine where the request should go.


## Running locally

```bash
export http_proxy=""
export https_proxy=""
```

```bash
# Running project
dotnet run --project WebhookReplayer

# Running tests
dotnet test
```

## Docker Image

```bash
docker run -d \
    -e http_proxy="" \
    -e https_proxy="" \
    -e no_proxy="" \
    -p 5000:80 \
    --name "webhook-replayer" \
    --restart always \
    sukratkashyap/webhook-replayer:latest
```

## Contribution

I am really open to contribution and pull requests. If you feel this tool is unnecessary and something else is better already. I would love to know about it.
