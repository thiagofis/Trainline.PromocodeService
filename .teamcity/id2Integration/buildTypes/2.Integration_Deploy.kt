package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id2Integration_2DeployC23SmokeTestUpdateTrafficWeightAn : BuildType({
    templates(AbsoluteId("PiperDeploy"))
    id("C99_Deploy")
    name = "2. Deploy c99, Smoke Test, Update Traffic Weight and Promote"

    params {
        param("env.TRAFFIC_WEIGHT", "50")
        param("env.DEPLOYMENT_ENV_TYPE", "Cluster")
        param("env.DEPLOYMENT_ENV_NAME", "c99")
        param("env.AFTER_TOGGLE_SLEEP_TIME_IN_SECONDS", "45")
        param("env.APP_VERSION", "${id2Integration_1BuildTestPackageAndPushReleaseCandida2.depParamRefs.buildNumber}")
    }
    
    triggers {
        finishBuildTrigger {
            id = "TRIGGER_193"
            buildTypeExtId = "${id2Integration_1BuildTestPackageAndPushReleaseCandida2.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(id2Integration_1BuildTestPackageAndPushReleaseCandida2) {
        }
    }
})
