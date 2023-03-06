package id1Development

import id1Development.buildTypes.*
import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.Project

object Project : Project({
    id("1Development")
    name = "1. Development"
    description = "branch :  develop - environment: c22 - consul"

    buildType(id1Development_1BuildTestPackageAndPushAlpha2)
    buildType(id1Development_2DeploySmokeTestUpdateTrafficWeightAndPr)
    buildType(id1Development_3AcceptanceTests2)

    params {
        param("GIT_REPOSITORY_SOURCE_BRANCH", "develop")
    }
})
