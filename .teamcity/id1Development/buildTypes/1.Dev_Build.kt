package id1Development.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.commitStatusPublisher

object id1Development_1BuildTestPackageAndPushAlpha2 : BuildType({
    templates(AbsoluteId("PiperBuildTestPackagePush"))
    id("Dev_Build")
    name = "1. Build, Test, Package and Push Alpha"

    allowExternalStatus = true
})
