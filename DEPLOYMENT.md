Add guidelines on how to deploy this service here

# Deployment

Deployment to test environments:

* When merged to **develop** the service will be deployed to cluster environment.

* Manually Run the deployment to Staging step (st1).
    * You will need to specify the version and your CORP credentials.
    * Monitor [Staging dashboards](MONITORING.md#Staging) for errors.
    * For senstive changes we tend to leave them in Staging for a while, to see if any other Clusters report any errors.

To go to **Production**:

1. Open all the monitoring dashboards listed in [Monitoring](MONITORING.md)

1. Deploy to the offline slice

1. Traffic weight and canary
    - For small changes, update to canary at **50%**
    - For big changes, better run this step twice: first to **30%** and then to **60%**.
  
1. If canary is good after a few minutes, then Promote

1. Monitor for 30-60 minutes (see [monitoring](MONITORING.md))

    Failure scenarios
    - Increase in response times
    - Increase in error rates
    - Loss of logging or NR metrics

1. In case of something bad happening
    1. Undo your changes:
        - If you've already promoted: Roll back
        - If you've not promoted: Update traffic weight to **0%**
    1. Tell `#platform_operations` tagging `@platformops` in Slack.