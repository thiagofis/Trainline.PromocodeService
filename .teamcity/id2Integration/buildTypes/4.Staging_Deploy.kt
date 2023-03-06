package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id2Integration_4DeploySt1SmokeTestUpdateTrafficWeightAn : BuildType({
    templates(AbsoluteId("PiperDeploy"))
    id("ST1_Deploy")
    name = "4. Deploy st1, Smoke Test, Update Traffic Weight and Promote"

    params {
        param("env.TRAFFIC_WEIGHT", "50")
        param("env.DEPLOYMENT_ENV_TYPE", "Staging")
        param("env.DEPLOYMENT_ENV_NAME", "st1")
        param("env.AFTER_TOGGLE_SLEEP_TIME_IN_SECONDS", "45")
        param("env.APP_VERSION", "${id2Integration_1BuildTestPackageAndPushReleaseCandida2.depParamRefs.buildNumber}")
    }

    triggers {
        finishBuildTrigger {
            id = "TRIGGER_216"
            buildTypeExtId = "${id2Integration_3AcceptanceTestsC232.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(id2Integration_3AcceptanceTestsC232) {
        }
    }
})
