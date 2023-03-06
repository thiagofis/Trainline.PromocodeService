package id1Development.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id1Development_2DeploySmokeTestUpdateTrafficWeightAndPr : BuildType({
    templates(AbsoluteId("PiperDeploy"))
    id("Dev_Deploy")
    name = "2. Deploy, Smoke Test, Update Traffic Weight and Promote"

    params {
        param("env.TRAFFIC_WEIGHT", "50")
        param("env.DEPLOYMENT_ENV_TYPE", "Cluster")
        param("env.DEPLOYMENT_ENV_NAME", "c22")
        param("env.AFTER_TOGGLE_SLEEP_TIME_IN_SECONDS", "45")
        param("env.APP_VERSION", "${id1Development_1BuildTestPackageAndPushAlpha2.depParamRefs.buildNumber}")
    }

    triggers {
        finishBuildTrigger {
            id = "TRIGGER_190"
            buildTypeExtId = "${id1Development_1BuildTestPackageAndPushAlpha2.id}"
            successfulOnly = true
            branchFilter = "+:*"
        }
    }

    dependencies {
        snapshot(id1Development_1BuildTestPackageAndPushAlpha2) {
        }
    }
})
