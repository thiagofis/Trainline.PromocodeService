package id1Development.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id1Development_3AcceptanceTests2 : BuildType({
    templates(AbsoluteId("PiperAcceptanceTests"))
    id("Dev_Test")
    name = "3. Acceptance Tests"

    buildNumberPattern = "${id1Development_1BuildTestPackageAndPushAlpha2.depParamRefs.buildNumber}"

    params {
        param("env.DEPLOYMENT_ENV_NAME", "c22")
        param("env.DEPLOYMENT_ENV_TYPE", "Cluster")
    }

    triggers {
        finishBuildTrigger {
            id = "TRIGGER_220"
            buildTypeExtId = "${id1Development_2DeploySmokeTestUpdateTrafficWeightAndPr.id}"
            successfulOnly = true
            branchFilter = "+:*"
        }
    }

    dependencies {
        snapshot(id1Development_2DeploySmokeTestUpdateTrafficWeightAndPr) {
            reuseBuilds = ReuseBuilds.NO
        }
    }
    
    disableSettings("RUNNER_146")
})
