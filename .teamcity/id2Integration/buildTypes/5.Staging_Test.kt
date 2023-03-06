package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id2Integration_5AcceptanceTestsStaging2 : BuildType({
    templates(AbsoluteId("PiperAcceptanceTests"))
    id("ST1_Test")
    name = "5. Acceptance Tests Staging"

    buildNumberPattern = "${id2Integration_1BuildTestPackageAndPushReleaseCandida2.depParamRefs.buildNumber}"

    params {
        param("env.DEPLOYMENT_ENV_NAME", "st1")
        param("env.DEPLOYMENT_ENV_TYPE", "Staging")
    }

    triggers {
        finishBuildTrigger {
            id = "TRIGGER_218"
            buildTypeExtId = "${id2Integration_4DeploySt1SmokeTestUpdateTrafficWeightAn.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(id2Integration_4DeploySt1SmokeTestUpdateTrafficWeightAn) {
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})
