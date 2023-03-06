package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.finishBuildTrigger

object id2Integration_3AcceptanceTestsC232 : BuildType({
    templates(AbsoluteId("PiperAcceptanceTests"))
    id("C99_Test")
    name = "3. Acceptance Tests c99"

    buildNumberPattern = "${id2Integration_1BuildTestPackageAndPushReleaseCandida2.depParamRefs.buildNumber}"

    params {
        param("env.DEPLOYMENT_ENV_NAME", "c99")
        param("env.DEPLOYMENT_ENV_TYPE", "Cluster")
    }

    triggers {
        finishBuildTrigger {
            id = "TRIGGER_219"
            buildTypeExtId = "${id2Integration_2DeployC23SmokeTestUpdateTrafficWeightAn.id}"
            successfulOnly = true
        }
    }

    dependencies {
        snapshot(id2Integration_2DeployC23SmokeTestUpdateTrafficWeightAn) {
            onDependencyFailure = FailureAction.FAIL_TO_START
        }
    }
})
