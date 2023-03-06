package id2Integration

import id2Integration.buildTypes.*
import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.Project

object Project : Project({
    id("2Integration")
    name = "2. Integration"
    description = "branch : master - environment: c99 and st1 (staging)"

    buildType(id2Integration_4DeploySt1SmokeTestUpdateTrafficWeightAn)
    buildType(id2Integration_5AcceptanceTestsStaging2)
    buildType(id2Integration_1BuildTestPackageAndPushReleaseCandida2)
    buildType(id2Integration_6PushReleasePackageManual)
    buildType(id2Integration_2DeployC23SmokeTestUpdateTrafficWeightAn)
    buildType(id2Integration_3AcceptanceTestsC232)

    params {
        param("GIT_REPOSITORY_SOURCE_BRANCH", "master")
    }
})
