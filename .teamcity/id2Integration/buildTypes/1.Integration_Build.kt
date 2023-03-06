package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object id2Integration_1BuildTestPackageAndPushReleaseCandida2 : BuildType({
    templates(AbsoluteId("PiperReleaseBuildTestPackagePush"))
    id("C99_Build")
    name = "1. Build, Test, Package And Push Release Candidate"
})
